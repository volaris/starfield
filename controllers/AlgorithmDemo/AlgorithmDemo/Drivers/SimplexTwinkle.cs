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
    class SimplexTwinkle : IStarfieldDriver
    {
        Color DrawColor = Color.Orange;
        int NumOctaves = 4;
        float Persistance = .25f;
        float Lacunarity = 2.0f;
        static float Time = 0;
        const bool CapAtMax = true;
        const float TimeStep = .005f;
        int[, ,] times;
        Random rand;
        int scale = 20;
        int maxTime = 10;

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NUM_X, (float)y / (float)Starfield.NUM_Y, (float)z / (float)Starfield.NUM_Z, Time, NumOctaves, Persistance, Lacunarity);
                        Color toDraw;
                        if(times[x,y,z] > 0)
                        {
                            times[x,y,z]--;
                            toDraw = Color.Black;
                        }
                        else
                        {
                            toDraw = DrawColor;

                            int val = Math.Max(0, (int)(n * scale));

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
            return "Simplex Twinkle";
        }


        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            times = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            rand = new Random();

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
    }
}
