using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils;
using StarfieldUtils.MathUtils;

namespace StarfieldDrivers.Noise.Simplex
{
    /** <summary>    Simplex noise based twinkling. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class SimplexTwinkle : IStarfieldDriver
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

        /**
         * <summary>    Gets or sets a value indicating whether to clamp noise values. </summary>
         *
         * <value>  True if values should be capped, false if not. </value>
         */

        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

        /**
         * <summary>    Gets or sets the draw color. </summary>
         *
         * <value>  The color of the draw. </value>
         */

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        /**
         * <summary>    Gets or sets the lacunarity. </summary>
         *
         * <value>  The lacunarity. </value>
         */

        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        /**
         * <summary>    Gets or sets the maximum time. </summary>
         *
         * <value>  The maximum time. </value>
         */

        public int MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }

        /**
         * <summary>    Gets or sets the number of octaves. </summary>
         *
         * <value>  The total number of octaves. </value>
         */

        public int NumOctaves
        {
            get { return numOctaves; }
            set { numOctaves = value; }
        }

        /**
         * <summary>    Gets or sets the off percent. </summary>
         *
         * <value>  The off percent. </value>
         */

        public float OffPercent
        {
            get { return offPercent; }
            set { offPercent = value; }
        }

        /**
         * <summary>    Gets or sets the persistance. </summary>
         *
         * <value>  The persistance. </value>
         */

        public float Persistance
        {
            get { return persistance; }
            set { persistance = value; }
        }

        /**
         * <summary>    Gets or sets the scale. </summary>
         *
         * <value>  The scale. </value>
         */

        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /**
         * <summary>    Gets or sets the time step. </summary>
         *
         * <value>  The time step. </value>
         */

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            offColor = Color.FromArgb((int)(drawColor.R * offPercent), (int)(drawColor.G * offPercent), (int)(drawColor.B * offPercent));
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, (float)y / (float)Starfield.NumY, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);
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

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            times = new int[Starfield.NumX, Starfield.NumY, Starfield.NumZ];
            rand = new Random();
            Color OffColor = Color.FromArgb((int)(this.DrawColor.R * offPercent), (int)(this.DrawColor.G * offPercent), (int)(this.DrawColor.B * offPercent));

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        times[x, y, z] = 0;
                    }
                }
            }
        }

        /** <summary>    Stops this object. </summary> */
        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return "Simplex Twinkle";
        }
        #endregion
    }
}
