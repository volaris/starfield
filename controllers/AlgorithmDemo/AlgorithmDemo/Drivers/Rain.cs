using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    public class Rain : IStarfieldDriver
    {
        Random rand = new Random();
        Color RainColor = Color.Blue;
        Color LightningColor = Color.Yellow;
        int Time = 0;
        int WrapTime = 20;
        Color[, ,] RainState;
        bool Lightning = true;
        bool Down = true;

        public Rain()
        {
        }

        public void Render(StarfieldModel Starfield)
        {
            if (Time == 0)
            {
                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y - 1; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            RainState[x, y, z] = RainState[x, y + 1, z];
                        }
                    }
                }

                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Color toDraw = Color.Black;

                        int val = rand.Next(20);
                        if (val == 1)
                        {
                            toDraw = RainColor;
                        }
                        RainState[x, Starfield.NUM_Y - 1, z] = toDraw;
                    }
                }

                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, RainState[x, y, z]);
                        }
                    }
                }

                if(Lightning)
                {
                    for (ulong x = 0; x < Starfield.NUM_X; x++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            int val = rand.Next(1000);
                            if (val == 1)
                            {
                                GenerateLightning(Starfield, (int)x, (int)z, (int)(Starfield.NUM_Y - 1));
                            }
                        }
                    }
                }
            }

            Time = (Time + 1) % WrapTime;
        }

        public void GenerateLightning(StarfieldModel Starfield, int x, int z, int y)
        {
            if(y == 0)
            {
                Starfield.SetColor(x, y, z, LightningColor);
                return;
            }

            int behavior = rand.Next(10);
            if(behavior == 9) // split
            {
                int dir = rand.Next(2);
                if(dir == 0)
                {
                    if (x < (int)(Starfield.NUM_X - 1))
                    {
                        Starfield.SetColor(x + 1, y, z, LightningColor);
                        GenerateLightning(Starfield, x + 1, z, y - 1);
                    }
                    if (x > 0)
                    {
                        Starfield.SetColor(x - 1, y, z, LightningColor);
                        GenerateLightning(Starfield, x - 1, z, y - 1);
                    }
                }
                else
                {
                    if (z < (int)(Starfield.NUM_Z - 1))
                    {
                        Starfield.SetColor(x, y, z + 1, LightningColor);
                        GenerateLightning(Starfield, x, z + 1, y - 1);
                    }
                    if (z > 0)
                    {
                        Starfield.SetColor(x, y, z - 1, LightningColor);
                        GenerateLightning(Starfield, x, z - 1, y - 1);
                    }
                }
            }
            if(behavior > 5) // curve
            {
                int axis = rand.Next(2);
                int dir = rand.Next(2);
                if (axis == 0)
                {
                    if (dir == 0 && x < (int)(Starfield.NUM_X - 1))
                    {
                        Starfield.SetColor(x + 1, y, z, LightningColor);
                        GenerateLightning(Starfield, x + 1, z, y - 1);
                    }
                    if (dir == 1 && x > 0)
                    {
                        Starfield.SetColor(x - 1, y, z, LightningColor);
                        GenerateLightning(Starfield, x - 1, z, y - 1);
                    }
                }
                else
                {
                    if (dir == 0 && z < (int)(Starfield.NUM_Z - 1))
                    {
                        Starfield.SetColor(x, y, z + 1, LightningColor);
                        GenerateLightning(Starfield, x, z + 1, y - 1);
                    }
                    if (dir == 1 && z > 0)
                    {
                        Starfield.SetColor(x, y, z - 1, LightningColor);
                        GenerateLightning(Starfield, x, z - 1, y - 1);
                    }
                }
            }
            else // straight
            {
                Starfield.SetColor(x, y, z, LightningColor);
                GenerateLightning(Starfield, x, z, y - 1);
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
            return "Rain";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            RainState = new Color[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        if (Time == 0)
                        {
                            RainState[x, y, z] = Color.Black;
                        }
                    }
                }
            }
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
