using System;
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
        Color PrimaryColor = Color.Blue;
        Color SecondaryColor = Color.Red;
        int NumOctaves = 4;
        float Persistance = .25f;
        float Lacunarity = 2.0f;
        static float Time = 0;
        bool CapAtMax = false;
        float TimeStep = .005f;
        float Threshold = .75f;
        float GradientMultiple = 2;
        bool HighContrast = false;

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NUM_X, (float)y / (float)Starfield.NUM_Y, (float)z / (float)Starfield.NUM_Z, Time, NumOctaves, Persistance, Lacunarity);
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
                                n *= GradientMultiple;
                                toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
                            }
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
            Time = (Time + TimeStep);
        }

        Panel IStarfieldDriver.GetConfigPanel()
        {
            throw new NotImplementedException();
        }

        void IStarfieldDriver.ApplyConfig()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Simplex Noise Clouds";
        }


        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
