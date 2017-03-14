using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using StarfieldUtils.MathUtils;
using StarfieldUtils.DisplayUtils;
using System.Drawing;

namespace StarfieldDrivers.Projection
{
    /** <summary>    A shape projection test using shapes. </summary> */
    [DriverType(DriverTypes.Experimental)]
    public class ShapeProjectionTest : IStarfieldDriver
    {
        Render2D renderer;

        /**
         * <summary>    Gets or sets the wrap time. </summary>
         *
         * <value>  The wrap time. </value>
         */

        public int WrapTime { get; set; }
        int time = 0;

        /**
         * <summary>    Gets or sets the camera position. </summary>
         *
         * <value>  The camera position. </value>
         */

        public Vec3D Camera { get; set; } 

        int drawIndex = 0;

        /** <summary>    Default constructor. </summary> */
        public ShapeProjectionTest()
        {
            WrapTime = 30 * 10;
            Camera = new Vec3D(-10, 5, -5);
        }

        /**
         * <summary>    Renders to the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        public void Render(StarfieldModel Starfield)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(400, 400);
            Graphics g = Graphics.FromImage(bmp);
            Pen myPen = new Pen(Color.Blue, 50);

            g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);

            switch (drawIndex)
            {
                case 0:
                    g.DrawEllipse(myPen, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    break;
                case 1:
                    g.DrawRectangle(myPen, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    break;
                case 2:
                    g.DrawLine(myPen, 0, 0, bmp.Width / 2, bmp.Height);
                    g.DrawLine(myPen, bmp.Width / 2, bmp.Height, bmp.Width, 0);
                    g.DrawLine(myPen, bmp.Width, 0, 0, 0);
                    break;
                case 3:
                    double pct = time / (double)WrapTime;
                    int width = (int)(pct * bmp.Width);
                    int height = (int)(pct * bmp.Height);
                    g.DrawEllipse(myPen, new Rectangle(bmp.Width / 2 - width / 2, bmp.Height / 2 - height / 2, width, height));
                    break;
                default:
                    drawIndex = 0;
                    break;
            }

            time = (time + 1) % WrapTime;
            if(time == 0)
            {
                drawIndex++;
            }
            renderer.Render(bmp, Render2D.RenderStyle.LetterBox);
        }

        /**
         * <summary>    Starts rendering to the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        public void Start(StarfieldModel Starfield)
        {
            renderer = new Render2D(Starfield, Camera);
        }

        /** <summary>    Stops this object. </summary> */
        public void Stop()
        {
        }

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return "2D projection test - Shapes";
        }
    }
}
