using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    public class TestFill : IStarfieldDriver
    {
        Color DrawColor = Color.Black;
        Color[] Colors = { Color.Red, Color.Green, Color.Blue };
        int ColorIndex = 0;
        int fillIndex = 0;
        int Delay = 25;
        int Step = 0;

        public void Render(StarfieldModel Starfield)
        {
            if (Step == 0)
            {
                int i = 0;
                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            Color toDraw = Color.Black;
                            if(i <= fillIndex)
                            {
                                toDraw = Colors[ColorIndex];
                            }
                            Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                            i++;
                        }
                    }
                }

                fillIndex = (fillIndex + 1) % (int)(Starfield.NUM_X * Starfield.NUM_Y * Starfield.NUM_Z);
                if(fillIndex == 0)
                {
                    ColorIndex = (ColorIndex + 1) % Colors.Length;
                }
            }

            Step = (Step + 1) % Delay;
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
            return "Test Fill";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
