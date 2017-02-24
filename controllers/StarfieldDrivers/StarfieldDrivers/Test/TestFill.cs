using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;

namespace StarfieldDrivers.Test
{
    /** <summary>    Fills the starfield with test patterns. </summary> */
    [DriverType(DriverTypes.Experimental)]
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

        /**
         * <summary>    Gets or sets the colors. </summary>
         *
         * <value>  The colors. </value>
         */

        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        /**
         * <summary>    Gets or sets the delay. </summary>
         *
         * <value>  The delay. </value>
         */

        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        /**
         * <summary>    Gets or sets the color of the draw. </summary>
         *
         * <value>  The color of the draw. </value>
         */

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

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

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            step = 0;
            fillIndex = 0;
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
            return "Test Fill";
        }
        #endregion
    }
}
