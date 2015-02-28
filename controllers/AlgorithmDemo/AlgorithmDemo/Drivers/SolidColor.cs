using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    public class SolidColor : IStarfieldDriver
    {
        Color DrawColor = Color.Black;

        public void Render(StarfieldModel Starfield)
        {
            for(ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, DrawColor);
                    }
                }
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
            return "Solid Color";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
