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
        #region Private Members
        Color drawColor = Color.Black;
        Color[] colors = { Color.Red, Color.Green, Color.Blue };
        int colorIndex = 0;
        int fillIndex = 0;
        int delay = 25;
        int step = 0;
        #endregion

        #region Public Properties
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            if (step == 0)
            {
                int i = 0;
                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = 0; y < Starfield.NumY; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            Color toDraw = Color.Black;
                            if(i <= fillIndex)
                            {
                                toDraw = Colors[colorIndex];
                            }
                            Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                            i++;
                        }
                    }
                }

                fillIndex = (fillIndex + 1) % (int)(Starfield.NumX * Starfield.NumY * Starfield.NumZ);
                if(fillIndex == 0)
                {
                    colorIndex = (colorIndex + 1) % Colors.Length;
                }
            }

            step = (step + 1) % Delay;
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
            return "Test Fill";
        }
        #endregion
    }
}
