using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;

namespace AlgorithmDemo.Drivers
{
    [DriverType(DriverTypes.Experimental)]
    public class Checkout : IStarfieldDriver
    {
        #region Private Members
        Color drawColor = Color.Purple;
        bool panel = true;
        #endregion

        #region Public Properties
        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        public ulong X
        {
            get; set;
        }

        public bool Panel
        {
            get { return panel; }
            set { panel = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            for(ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color toDraw = Color.Black;
                        if((panel && x == X) || (!panel && z == X))
                        {
                            toDraw = DrawColor;
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
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
            return "Checkout";
        }
        #endregion
    }
}
