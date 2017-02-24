using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;

namespace StarfieldDrivers.Noise.White
{
    /** <summary>    Static noise animation. </summary> */
    [DriverType(DriverTypes.Experimental)]
    public class Static : IStarfieldDriver
    {
        #region Private Members
        Random rand = new Random();
        Color DrawColor = Color.Blue;
        int Time = 0;
        int WrapTime = 4;
        #endregion

        #region IStarfieldDriver Implementation

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        public void Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
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

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        /** <summary>    Stops this object. </summary> */
        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return "Static";
        }
        #endregion
    }
}
