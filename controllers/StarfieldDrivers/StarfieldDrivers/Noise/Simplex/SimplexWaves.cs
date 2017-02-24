using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers.Noise.Simplex
{
    /** <summary>    Simplex noise based waves. </summary> */
    [DriverType(DriverTypes.Experimental)]
    public class SimplexWaves : IStarfieldDriver
    {
        #region Private Members
        Color primaryColor = Color.Red;
        Color secondaryColor = Color.Blue;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        float time = 0;
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
         * <summary>    Gets or sets the color of the primary. </summary>
         *
         * <value>  The color of the primary. </value>
         */

        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

        /**
         * <summary>    Gets or sets the color of the secondary. </summary>
         *
         * <value>  The color of the secondary. </value>
         */

        public Color SecondaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
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
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color toDraw = Color.Black;
                        float n = .5f + SimplexNoise.fbm_noise3((float)x / (float)Starfield.NumX, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);
                        if (.3f * n * Starfield.NumY > y)
                        {
                           toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
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
            return "Simplex Waves";
        }
        #endregion
    }
}
