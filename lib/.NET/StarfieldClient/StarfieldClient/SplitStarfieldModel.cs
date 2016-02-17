using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;

namespace Starfield
{
    public class SplitStarfieldModel : StarfieldModel
    {
        protected StarfieldModel model1;
        protected StarfieldModel model2;

        public StarfieldModel Model1
        {
            get
            {
                return model1;
            }
        }

        public StarfieldModel Model2
        {
            get
            {
                return model2;
            }
        }

        // create a model with arbitrary parameters
        public SplitStarfieldModel(float xStep, float yStep, float zStep, ulong numX1, ulong numX2, ulong numY, ulong numZ) : 
            base(xStep, yStep, zStep, numX1 + numX2, numY, numZ)
        {
            System.Threading.Monitor.Enter(lockObject);

            this.model1 = new StarfieldModel(xStep, yStep, zStep, numX1, numY, numZ);
            this.model2 = new StarfieldModel(xStep, yStep, zStep, numX2, numY, numZ);

            System.Threading.Monitor.Exit(lockObject);
        }

        // set all LEDs to black
        public override void Clear()
        {
            System.Threading.Monitor.Enter(lockObject);

            base.Clear();

            for (ulong x = 0; x < model1.NumX; x++)
            {
                for (ulong y = 0; y < model1.NumY; y++)
                {
                    for (ulong z = 0; z < model1.NumZ; z++)
                    {
                        model1.SetColor((int)x, (int)y, (int)z, Color.Black);
                    }
                }
            }

            for (ulong x = 0; x < model2.NumX; x++)
            {
                for (ulong y = 0; y < model2.NumY; y++)
                {
                    for (ulong z = 0; z < model2.NumZ; z++)
                    {
                        model2.SetColor((int)x, (int)y, (int)z, Color.Black);
                    }
                }
            }

            System.Threading.Monitor.Exit(lockObject);
        }

        // set the LED at (x, y, z) to the given color
        public override void SetColor(int x, int y, int z, Color color)
        {
            throw new InvalidOperationException("Can not set color directly using this class, use one of the split models");
        }

        // returns the color of the LED at (x, y, z)
        public override Color GetColor(int x, int y, int z)
        {
            Color color;
            if(x >= (int)model1.NumX)
            {
                x -= (int)model1.NumX;
                color = model2.GetColor(x, y, z);
            }
            else
            {
                color = model1.GetColor(x, y, z); 
            }
            return Color.FromArgb((int)(color.R * this.Brightness), (int)(color.G * this.Brightness), (int)(color.B * this.Brightness));
        }
    }
}
