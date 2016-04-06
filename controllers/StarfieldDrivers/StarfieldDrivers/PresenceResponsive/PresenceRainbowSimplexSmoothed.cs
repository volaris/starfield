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
    [DriverType(DriverTypes.Interactive)]
    class PresenceResponsiveRainbowSimplexSmoothed : IStarfieldDriver
    {
        #region Private Members
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        float radius = 4.0f;
        float height = 4.0f;
        #endregion

        #region Public Properties
        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

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

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }
        #endregion

        #region Constructors
        public PresenceResponsiveRainbowSimplexSmoothed()
        {
            rainbow10[0] = rainbow7[0] = Color.FromArgb(0xFF, 0, 0);
            rainbow10[1] = rainbow7[1] = Color.FromArgb(0xFF, 0xA5, 0);
            rainbow10[2] = rainbow7[2] = Color.FromArgb(0xFF, 0xFF, 0);
            rainbow10[3] = rainbow7[3] = Color.FromArgb(0, 0x80, 0);
            rainbow10[4] = Color.FromArgb(0, 0xFF, 0);
            rainbow10[5] = Color.FromArgb(0, 0xA5, 0x80);
            rainbow10[6] = rainbow7[4] = Color.FromArgb(0, 0, 0xFF);
            rainbow10[7] = rainbow7[5] = Color.FromArgb(0x4B, 0, 0x82);
            rainbow10[8] = rainbow7[6] = Color.FromArgb(0xFF, 0, 0xFF);
            rainbow10[9] = Color.FromArgb(0xEE, 0x82, 0xEE);
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            Color toDraw = Color.Black;
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
            KMeansResult result = KMeans.Cluster(trimmed, 20, 15);

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, (float)y / (float)Starfield.NumY, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);

                        if (n > 0 && n < 1)
                        {
                            int index1 = (int)(Math.Floor(9 * n));
                            int index2 = (int)(Math.Ceiling(9 * n));
                            float percent = (9 * n) - index1;
                            toDraw = ColorUtils.GetGradientColor(rainbow10[index1], rainbow10[index2], percent, true);
                        }
                        else
                        {
                            if (n < 0)
                            {
                                toDraw = rainbow10[0];
                            }
                            if (n > 1)
                            {
                                toDraw = rainbow10[9];
                            }
                        }

                        double val = 0;
                        KMeansPoint point = new KMeansPoint();
                        point.x = x * Starfield.XStep;
                        point.y = z * Starfield.ZStep;

                        for(int i = 0; i < result.centroids.Length; i++)
                        {
                            KMeansPoint centroid = new KMeansPoint();
                            centroid.x = result.centroids[i].x * Starfield.XStep;
                            centroid.y = result.centroids[i].y * Starfield.ZStep;

                            double dist = point.distance(centroid);
                            if(dist < this.radius)
                            {
                                val = Math.Max(val, 3.5 * (Math.Cos(dist * (Math.PI/(2 * radius))) + 1));
                            }
                        }

                        if(y * Starfield.YStep > val)
                        {
                            toDraw = Color.Black;
                        }

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
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
            return "Presence - Smooth Rainbow Simplex Noise";
        }
        #endregion
    }
}