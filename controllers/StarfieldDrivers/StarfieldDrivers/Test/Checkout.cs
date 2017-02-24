using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;

namespace StarfieldDrivers.Test
{
    /** <summary>    Animation for doing electronics/display checkout testing. </summary> */
    [DriverType(DriverTypes.Experimental)]
    public class Checkout : IStarfieldDriver
    {
        #region Private Members
        Color drawColor = Color.Purple;
        bool panel = true;
        #endregion

        #region Public Properties

        /**
         * <summary>    Gets or sets the draw color. </summary>
         *
         * <value>  The color of the draw. </value>
         */

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        /**
         * <summary>    Gets or sets the x coordinate to draw. </summary>
         *
         * <value>  The x coordinate. </value>
         */

        public ulong X
        {
            get; set;
        }

        /**
         * <summary>    Gets or sets a value indicating whether to draw a whole panel. </summary>
         *
         * <value>  True if panel, false if not. </value>
         */

        public bool Panel
        {
            get { return panel; }
            set { panel = value; }
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
            return "Checkout";
        }
        #endregion
    }
}
