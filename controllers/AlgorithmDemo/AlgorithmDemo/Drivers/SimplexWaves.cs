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
    class SimplexWaves : IStarfieldDriver
    {
        Color PrimaryColor = Color.Red;
        Color SecondaryColor = Color.Blue;
        int NumOctaves = 4;
        float Persistance = .25f;
        float Lacunarity = 2.0f;
        static float Time = 0;
        const bool CapAtMax = true;
        const float TimeStep = .005f;

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Color toDraw = Color.Black;
                        float n = .5f + SimplexNoise.fbm_noise3((float)x / (float)Starfield.NUM_X, (float)z / (float)Starfield.NUM_Z, Time, NumOctaves, Persistance, Lacunarity);
                        if (.3f * n * Starfield.NUM_Y > y)
                        {
                           toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
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
            return "Simplex Waves";
        }


        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
