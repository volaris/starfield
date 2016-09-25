using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;

namespace StarfieldDrivers.Drivers
{
    [DriverType(DriverTypes.Ambient)]
    public class Rain : IStarfieldDriver
    {
        #region Private Members
        Random rand = new Random();
        Color rainColor = Color.Blue;
        Color lightningColor = Color.Yellow;
        int Time = 0;
        int wrapTime = 20;
        Color[, ,] rainState;
        bool lightning = true;
        bool down = true;
        #endregion

        #region Public Properties
        public bool Down
        {
            get { return down; }
            set { down = value; }
        }

        public bool Lightning
        {
            get { return lightning; }
            set { lightning = value; }
        }

        public Color LightningColor
        {
            get { return lightningColor; }
            set { lightningColor = value; }
        }

        public Color RainColor
        {
            get { return rainColor; }
            set { rainColor = value; }
        }

        public int WrapTime
        {
            get { return wrapTime; }
            set { wrapTime = value; }
        }
        #endregion

        #region Constructors
        public Rain()
        {
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            if (Time == 0)
            {
                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = 0; y < Starfield.NumY - 1; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            rainState[x, y, z] = rainState[x, y + 1, z];
                        }
                    }
                }

                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color toDraw = Color.Black;

                        int val = rand.Next(20);
                        if (val == 1)
                        {
                            toDraw = RainColor;
                        }
                        rainState[x, Starfield.NumY - 1, z] = toDraw;
                    }
                }

                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = 0; y < Starfield.NumY; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, rainState[x, y, z]);
                        }
                    }
                }

                if(Lightning)
                {
                    for (ulong x = 0; x < Starfield.NumX; x++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            int val = rand.Next(1000);
                            if (val == 1)
                            {
                                GenerateLightning(Starfield, (int)x, (int)z, (int)(Starfield.NumY - 1));
                            }
                        }
                    }
                }
            }

            Time = (Time + 1) % WrapTime;
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            rainState = new Color[Starfield.NumX, Starfield.NumY, Starfield.NumZ];

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        if (Time == 0)
                        {
                            rainState[x, y, z] = Color.Black;
                        }
                    }
                }
            }
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Rain";
        }
        #endregion

        #region Private Methods
        private void GenerateLightning(StarfieldModel Starfield, int x, int z, int y)
        {
            if (y == 0)
            {
                Starfield.SetColor(x, y, z, LightningColor);
                return;
            }

            int behavior = rand.Next(10);
            if (behavior == 9) // split
            {
                int dir = rand.Next(2);
                if (dir == 0)
                {
                    if (x < (int)(Starfield.NumX - 1))
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
                    if (z < (int)(Starfield.NumZ - 1))
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
            if (behavior > 5) // curve
            {
                int axis = rand.Next(2);
                int dir = rand.Next(2);
                if (axis == 0)
                {
                    if (dir == 0 && x < (int)(Starfield.NumX - 1))
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
                    if (dir == 0 && z < (int)(Starfield.NumZ - 1))
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
        #endregion
    }
}
