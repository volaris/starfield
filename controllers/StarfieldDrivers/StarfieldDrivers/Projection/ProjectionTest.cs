using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using StarfieldUtils.MathUtils;
using StarfieldUtils.DisplayUtils;

namespace StarfieldDrivers.Projection
{
    /** <summary>    A 2D -> 3D projection test using images. </summary> */
    [DriverType(DriverTypes.Experimental)]
    public class ProjectionTest : IStarfieldDriver
    {
        string path = null;
        Render2D renderer;
        bool imageValid = false;
        System.Drawing.Image img;

        /**
         * <summary>    Gets or sets the camera position. </summary>
         *
         * <value>  The camera position. </value>
         */

        public Vec3D Camera { get; set; }

        /** <summary>    Default constructor. </summary> */
        public ProjectionTest()
        {
            Camera = new Vec3D(-10, 5, -5);
        }

        /**
         * <summary>    Gets or sets the full pathname of the file to render. </summary>
         *
         * <value>  The full pathname of the file to render. </value>
         */

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                chamgeImage();
            }
        }

        /**
         * <summary>    Renders to the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield to render to. </param>
         */

        public void Render(StarfieldModel Starfield)
        {
            if(imageValid)
            {
                renderer.Render(img, Render2D.RenderStyle.LetterBox);
            }
        }

        /**
         * <summary>    Starts the given starfield. </summary>
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
            return "2D projection test - image";
        }

        private void chamgeImage()
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    img = System.Drawing.Image.FromFile(path);
                    imageValid = true;
                }
            }
            catch
            { }
        }
    }
}
