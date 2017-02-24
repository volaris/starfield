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
    /** <summary>    Simplex noise based clouds. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class SimplexClouds : IStarfieldDriver
    {
        #region Private Members
        Color primaryColor = Color.Blue;
        Color secondaryColor = Color.Red;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        float threshold = .75f;
        float fadeInThreshold = .1f;
        bool highContrast = false;
        bool fade = true;
        #endregion

        #region Public Properties

        /**
         * <summary>    Gets or sets a value indicating whether noise values should be clamped. </summary>
         *
         * <value>  True if values should be clamped, false if not. </value>
         */

        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

        /**
         * <summary>    Gets or sets a value indicating whether the fade. </summary>
         *
         * <value>  True if fade, false if not. </value>
         */

        public bool Fade
        {
            get { return fade; }
            set { fade = value; }
        }

        /**
         * <summary>    Gets or sets the fade in threshold. </summary>
         *
         * <value>  The fade in threshold. </value>
         */

        public float FadeInThreshold
        {
            get { return fadeInThreshold; }
            set { fadeInThreshold = value; }
        }

        /**
         * <summary>    Gets or sets a value indicating whether the high contrast. </summary>
         *
         * <value>  True if high contrast, false if not. </value>
         */

        public bool HighContrast
        {
            get { return highContrast; }
            set { highContrast = value; }
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
         * <summary>    Gets or sets the primary color. </summary>
         *
         * <value>  The color of the primary. </value>
         */

        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

        /**
         * <summary>    Gets or sets the secondary color. </summary>
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

        /**
         * <summary>    Gets or sets the threshold. </summary>
         *
         * <value>  The threshold. </value>
         */

        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
        #endregion

        #region IstarfieldDriver Implementation

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
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, (float)y / (float)Starfield.NumY, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);
                        Color toDraw = Color.Black;
                        if (n > Threshold)
                        {
                            if (HighContrast)
                            {
                                toDraw = PrimaryColor;
                            }
                            else
                            {
                                n -= Threshold;
                                n *= 1 / (1 - threshold);
                                toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
                            }
                        }
                        else if (Fade && !HighContrast && n > (Threshold - FadeInThreshold))
                        {
                            n -= (Threshold - FadeInThreshold);
                            n *= 1 / FadeInThreshold;
                            toDraw = ColorUtils.GetGradientColor(Color.Black, PrimaryColor, n, CapAtMax);
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
            return "Simplex Noise Clouds";
        }
        #endregion
    }
}
