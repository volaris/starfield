using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils.MathUtils;

namespace StarfieldDrivers.Noise.Simplex
{
    /** <summary>    Simplex noise based rainbows. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class RainbowSimplex : IStarfieldDriver
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

        #region Constructors
        /** <summary>    Default constructor. </summary> */
        public RainbowSimplex()
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

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color toDraw;
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, (float)y / (float)Starfield.NumY, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);

                        if (n > 0 && n < 1)
                        {
                            int index = (int)(9 * Math.Round(n, 1));
                            toDraw = rainbow10[index];
                            Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                        }
                        else
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                        }
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
            return "Rainbow Simplex Noise";
        }
        #endregion
    }
}
