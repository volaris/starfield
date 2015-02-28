using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    public class Static : IStarfieldDriver
    {
        Random rand = new Random();
        Color DrawColor = Color.Blue;
        int Time = 0;
        int WrapTime = 4;

        public void Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        if (Time == 0)
                        {
                            Color toDraw = Color.Black;

                            int val = rand.Next(2);
                            if (val == 1)
                            {
                                toDraw = DrawColor;
                            }
                            Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                        }
                    }
                }
            }

            Time = (Time + 1) % WrapTime;
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
            return "Static";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
