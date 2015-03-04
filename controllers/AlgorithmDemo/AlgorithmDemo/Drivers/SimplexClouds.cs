﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StarfieldClient;
using System.Drawing;
using AlgorithmDemo.Utils;
using AlgorithmDemo.MathUtils;

namespace AlgorithmDemo.Drivers
{
    class SimplexClouds : IStarfieldDriver
    {
        #region Private Members
        Color primaryColor = Color.Blue;
        Color secondaryColor = Color.Red;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        bool capAtMax = false;
        float timeStep = .005f;
        float threshold = .75f;
        float fadeInThreshold = .1f;
        bool highContrast = false;
        bool fade = true;
        #endregion

        #region Public Properties
        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

        public bool Fade
        {
            get { return fade; }
            set { fade = value; }
        }

        public float FadeInThreshold
        {
            get { return fadeInThreshold; }
            set { fadeInThreshold = value; }
        }

        public bool HighContrast
        {
            get { return highContrast; }
            set { highContrast = value; }
        }

        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
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

        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

        public Color SecondaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
        }

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }

        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
        #endregion

        #region IstarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NUM_X, (float)y / (float)Starfield.NUM_Y, (float)z / (float)Starfield.NUM_Z, time, NumOctaves, Persistance, Lacunarity);
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
            return "Simplex Noise Clouds";
        }
        #endregion
    }
}
