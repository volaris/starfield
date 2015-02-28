using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;
using AlgorithmDemo.Utils;

namespace AlgorithmDemo.Drivers
{
    public class FadingStatic : IStarfieldDriver
    {
        Random rand = new Random();
        Color DrawColor = Color.Blue;
        float Time = 0f;
        float Increment = .03f;
        float AnimationDuration = 1f;

        int[, ,] Prev;
        int[, ,] Next;


        public void Render(StarfieldModel Starfield)
        {

            if (Time == 0f)
            {
                Console.WriteLine("new goal");
                Prev = Next;
                Next = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];

                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            if (Time == 0)
                            {
                                Next[x, y, z] = rand.Next(2);
                            }
                        }
                    }
                }
            }

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Color prevColor = Prev[x,y,z] > 0 ? DrawColor : Color.Black;
                        Color nextColor = Next[x, y, z] > 0 ? DrawColor : Color.Black;
                        Color toDraw = ColorUtils.GetGradientColor(prevColor, nextColor, Time, true);

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }

            if (Time < AnimationDuration)
            {
                Time += Increment;
            }
            else
            {
                Time = 0f;
            }
        }

        public System.Windows.Forms.Panel GetConfigPanel()
        {
            throw new NotImplementedException();
        }

        public void ApplyConfig()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Fading Static";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            Prev = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            Next = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
