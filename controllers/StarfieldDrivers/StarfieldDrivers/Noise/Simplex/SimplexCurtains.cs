﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarfieldClient;
using System.Drawing;
using StarfieldUtils;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers
{
    class SimplexCurtains : IStarfieldDriver
    {
        #region Private Members
        Color primaryColor = Color.Blue;
        Color secondaryColor = Color.Red;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        float upperThreshold = .5f;
        float lowerThreshold = .4f;
        bool highContrast = false;
        #endregion

        #region Public Properties
        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
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

        public float LowerThreshold
        {
            get { return lowerThreshold; }
            set { lowerThreshold = value; }
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

        public float UpperThreshold
        {
            get { return upperThreshold; }
            set { upperThreshold = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
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
                        if (n < UpperThreshold && n > LowerThreshold)
                        {
                            if (HighContrast)
                            {
                                toDraw = PrimaryColor;
                            }
                            else
                            {
                                n -= LowerThreshold;
                                n *= 1 / (UpperThreshold - LowerThreshold);
                                toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
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
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Simplex Noise Curtains";
        }
        #endregion
    }
}