using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;
using Starfield.Presence;

namespace StarfieldDrivers
{
    class Orb
    {
        public float radius;
        public Color color;
        public Vec3D location;
        public Vec3D goal;
    }

    [DriverType(DriverTypes.Interactive)]
    class ConnectiveOrbs : IStarfieldDriver
    {
        #region Private Members
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        float timeStep = .005f;
        float radius = 3.0f;
        float height = 4.0f;
        Random rand = new Random();
        List<Orb> orbs = new List<Orb>();
        double fadeRate = .95;
        Color drawColor = Color.Purple;
        double velocity = .002;
        float probScalar = .003f;
        #endregion

        #region Public Properties
        public int NumOctaves
        {
            get { return numOctaves; }
            set { numOctaves = value; }
        }

        public float Persistance
        {
            get { return persistance; }
            set { persistance = value; }
        }

        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float ProbabilityScalar
        {
            get { return probScalar; }
            set { probScalar = value; }
        }

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }

        public double FadeRate
        {
            get { return fadeRate; }
            set { fadeRate = value; }
        }

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        public double Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            List<List<Activity>> activity = Starfield.GetPresence();
            int numPoints = 0;
            KMeansPoint[] points = new KMeansPoint[Starfield.NumX * Starfield.NumZ];

            for(int x = 0; x < (int)Starfield.NumX; x++)
            {
                for(int z = 0; z < (int)Starfield.NumZ; z++)
                {
                    if(activity[x][z].activity > 0)
                    {
                        points[numPoints] = new KMeansPoint();
                        points[numPoints].x = x;
                        points[numPoints].y = z;
                        numPoints++;
                    }
                }
            }

            KMeansPoint[] trimmed = new KMeansPoint[numPoints];
            for(int i = 0; i < numPoints; i++)
            {
                trimmed[i] = points[i];
            }
            KMeansResult result = KMeans.Cluster(trimmed, 5, 15);

            for (int i = 0; i < result.centroids.Length; i++)
            {
                KMeansPoint centroid = new KMeansPoint();

                float n = .5f + SimplexNoise.fbm_noise4((float)result.centroids[i].x / (float)Starfield.NumX, (Height / Starfield.YStep) / (float)Starfield.NumY, (float)result.centroids[i].y / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);
                float r = (float)rand.NextDouble();

                if (r < (n * ProbabilityScalar) && result.centroids.Length > 1)
                {
                    // create orb
                    Orb orb = new Orb();
                    orb.location = new Vec3D(result.centroids[i].x * Starfield.XStep, Height, result.centroids[i].y * Starfield.ZStep);
                    orb.color = DrawColor;
                    int goal;
                    while ((goal = rand.Next(result.centroids.Length)) == i) { } ;
                    orb.goal = new Vec3D(result.centroids[goal].x * Starfield.XStep, Height, result.centroids[goal].y * Starfield.ZStep);
                    orb.radius = Radius;
                    orbs.Add(orb);
                }
            }

            // fade
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color prev = Starfield.GetColor((int)x, (int)y, (int)z);
                        Color next = Color.FromArgb((int)(FadeRate * prev.R), (int)(FadeRate * prev.G), (int)(FadeRate * prev.B));
                        Starfield.SetColor((int)x, (int)y, (int)z, next);
                    }
                }
            }

            List<Orb> toRemove = new List<Orb>();

            // render orbs
            foreach(Orb orb in orbs)
            {
                bool rendered = false;
                for (ulong x = (ulong)Math.Max(0, Math.Floor((orb.location.X - orb.radius) / Starfield.XStep)); x < (ulong)Math.Min(Starfield.NumX, Math.Ceiling((orb.location.X + orb.radius) / Starfield.XStep)); x++)
                {
                    for (ulong y = (ulong)Math.Max(0, Math.Floor((orb.location.Y - orb.radius) / Starfield.YStep)); y < (ulong)Math.Min(Starfield.NumY, Math.Ceiling((orb.location.Y + orb.radius) / Starfield.YStep)); y++)
                    {
                        for (ulong z = (ulong)Math.Max(0, Math.Floor((orb.location.Z - orb.radius) / Starfield.ZStep)); z < (ulong)Math.Min(Starfield.NumZ, Math.Ceiling((orb.location.Z + orb.radius) / Starfield.ZStep)); z++)
                        {
                            rendered = true;
                            Vec3D renderPoint = new Vec3D(x * Starfield.XStep, y * Starfield.YStep, z * Starfield.ZStep);
                            if (orb.location.DistanceTo(renderPoint) < orb.radius)
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, DrawColor);
                            }

                            //update orb
                            Vec3D direction = orb.goal - orb.location;
                            direction = (velocity / direction.Magnitude) * direction;
                            orb.location += direction;

                            if((orb.goal - orb.location).Magnitude < 1.0d)
                            {
                                toRemove.Add(orb);
                            }
                        }
                    }
                }

                if(!rendered)
                {
                    toRemove.Add(orb);
                }
            }

            foreach(Orb orb in toRemove)
            {
                orbs.Remove(orb);
            }

            time = (time + TimeStep);
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Presence - Connective Orbs";
        }
        #endregion
    }
}