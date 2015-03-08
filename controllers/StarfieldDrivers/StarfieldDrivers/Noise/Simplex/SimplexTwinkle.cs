using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarfieldClient;
using System.Drawing;
using StarfieldUtils;
using StarfieldUtils.MathUtils;

namespace StarfieldDrivers
{
    class SimplexTwinkle : IStarfieldDriver
    {
        #region Private Members
        Color drawColor = Color.Orange;
        Color offColor;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        int[, ,] times;
        Random rand;
        int scale = 47;
        int maxTime = 10;
        float offPercent = .75f;
        #endregion

        #region Public Properties
        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        public int MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }

        public int NumOctaves
        {
            get { return numOctaves; }
            set { numOctaves = value; }
        }

        public float OffPercent
        {
            get { return offPercent; }
            set { offPercent = value; }
        }

        public float Persistance
        {
            get { return persistance; }
            set { persistance = value; }
        }

        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            offColor = Color.FromArgb((int)(drawColor.R * offPercent), (int)(drawColor.G * offPercent), (int)(drawColor.B * offPercent));
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NUM_X, (float)y / (float)Starfield.NUM_Y, (float)z / (float)Starfield.NUM_Z, time, NumOctaves, Persistance, Lacunarity);
                        Color toDraw;
                        if(times[x,y,z] > 0)
                        {
                            times[x,y,z]--;
                            toDraw = offColor;
                        }
                        else
                        {
                            toDraw = DrawColor;

                            int val = Math.Max(0, (int)(n * scale) + 3);

                            int randomVal = rand.Next(val);
                            if (randomVal == 0)
                            {
                                times[x, y, z] = rand.Next(maxTime);
                            }
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
            time = (time + TimeStep);
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            times = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            rand = new Random();
            Color OffColor = Color.FromArgb((int)(this.DrawColor.R * offPercent), (int)(this.DrawColor.G * offPercent), (int)(this.DrawColor.B * offPercent));

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        times[x, y, z] = 0;
                    }
                }
            }
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Simplex Twinkle";
        }
        #endregion
    }
}
