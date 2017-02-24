using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers.Animation
{
    /** <summary>    3D sin wave </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class CenterWave : IStarfieldDriver
    {
        #region Private Members
        Color backColor = Color.Black;
        Color drawColor = Color.Blue;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        bool rainbow = true;
        float interval = 1.3f;
        ulong step = 0;
        #endregion

        #region Constructors
        /** <summary>    Default constructor. </summary> */
        public CenterWave()
        {
            rainbow10[0] = rainbow7[0] = Color.FromArgb(0xFF, 0, 0);
            rainbow10[1] = rainbow7[1] = Color.FromArgb(0xFF, 0xA5, 0);
            rainbow10[2] = rainbow7[2] = Color.FromArgb(0xFF, 0xFF, 0);
            rainbow10[3] = rainbow7[3] = Color.FromArgb(0, 0x80, 0);
            rainbow10[4] = Color.FromArgb(0, 0xFF, 0);
            rainbow10[5] = Color.FromArgb(0, 0xA5, 0x80);
            rainbow10[6] = rainbow7[4] = Color.FromArgb(0, 0, 0xFF);
            rainbow10[7] = rainbow7[5] = Color.FromArgb(0x4B, 0, 0x82);
            rainbow10[8] = rainbow7[6] = Color.FromArgb(0xFF, 0, 0xFF);
            rainbow10[9] = Color.FromArgb(0xEE, 0x82, 0xEE);
        }
        #endregion

        #region Public Properties

        /**
         * <summary>    Gets or sets the color of the background. </summary>
         *
         * <value>  The color of the background. </value>
         */

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        /**
         * <summary>    Gets or sets the draw color if not using a rainbow. </summary>
         *
         * <value>  The color of the draw. </value>
         */

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        /**
         * <summary>    Gets or sets the interval between animation updates. </summary>
         *
         * <value>  The interval. </value>
         */

        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /**
         * <summary>    Gets or sets a value indicating whether or not to use single or rainbow colors rainbow. </summary>
         *
         * <value>  True if rainbow, false if not. </value>
         */

        public bool Rainbow
        {
            get { return rainbow; }
            set { rainbow = value; }
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
            double distance;
            Vec3D center = new Vec3D(((double)Starfield.NumX - 1) / 2, 0, ((double)Starfield.NumZ - 1) / 2);
            Vec3D point;
            for(ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong z = 0; z < Starfield.NumZ; z++)
                {
                    point = new Vec3D(x, 0, z);
                    distance = point.DistanceTo(center)/9.899495*8;
                    double height = (Starfield.NumY / 2) + Math.Sin(distance / interval + (double)step / 50) * (Starfield.NumY / 2);

                    ulong drawY = (ulong)Math.Min(Starfield.NumY - 1, Math.Max(0, Math.Round(height)));

                    for(ulong y = 0; y < Starfield.NumY; y++)
                    {
                        Color toDraw = backColor;
                        if(y == drawY)
                        {
                            if (rainbow)
                            {
                                toDraw = ColorUtils.GetMultiColorGradient(rainbow7, 1.0f-((float)drawY / (Starfield.NumY-1)), true);
                            }
                            else
                            {
                                toDraw = drawColor;
                            }
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
            step++;
        }

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            step = 0;
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
            return "Center Wave";
        }
        #endregion
    }
}
