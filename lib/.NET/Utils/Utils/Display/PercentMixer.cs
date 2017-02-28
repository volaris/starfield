using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;

namespace StarfieldUtils.Display
{
    /**
     * <summary>    A threshold mixer. This class is used to blend two starfield driver outputs into a single starfield.
     *              It combines the colors in the two starfield outputs using the greyscale percentage in a third and 
     *              stores the output. </summary>
     */

    public class PercentMixer : StarfieldModel
    {
        private StarfieldModel one, two, percent, output;

        /**
         * <summary>    Gets the first starfield. </summary>
         */

        public StarfieldModel One
        {
            get
            {
                return one;
            }
        }

        /**
         * <summary>    Gets the second starfield. </summary>
         */

        public StarfieldModel Two
        {
            get
            {
                return two;
            }
        }
       
        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="starfieldOne">  The starfield one. </param>
         * <param name="starfieldTwo">  The starfield two. </param>
         * <param name="percent">       The percent. </param>
         */

        public PercentMixer(StarfieldModel starfieldOne, StarfieldModel starfieldTwo, StarfieldModel percent)
        {
            this.one = starfieldOne;
            this.two = starfieldTwo;
            this.percent = percent;
        }

        /**
         * <summary>    returns the color of the LED at (x, y, z) </summary>
         *
         * <param name="x"> The x coordinate. </param>
         * <param name="y"> The y coordinate. </param>
         * <param name="z"> The z coordinate. </param>
         *
         * <returns>    The color. </returns>
         */

        public override Color GetColor(int x, int y, int z)
        {
            Color color1 = one.GetColor(x, y, z);
            Color color2 = one.GetColor(x, y, z);
            Color colorPct = ColorUtils.ColorUtils.ToGrayScale(percent.GetColor(x, y, z));
            float pct = (float)colorPct.R / 255.0f;

            Color output = Color.FromArgb((int)(color1.R * pct + color2.R * (1.0f - pct)),
                                          (int)(color1.G * pct + color2.G * (1.0f - pct)),
                                          (int)(color1.B * pct + color2.B * (1.0f - pct)));

            return output;
        }

        /**
         * <summary>    This function will throw an exception for this implementation of the StarifeldModel. 
         *              This is a starfield mixer, SetColor should not be called directly. It must be called on one of the 3 component starfields.</summary>
         */

        public override void SetColor(int x, int y, int z, Color color)
        {
            throw new NotImplementedException("This is a starfield mixer, SetColor should not be called directly. It must be called on one of the 3 component starfields.");
        }

        /**
         * <summary>    This function will throw an exception for this implementation of the StarifeldModel. 
         *              This is a starfield mixer, Clear should not be called directly. It must be called on one of the 3 component starfields.</summary>
         */

        public override void Clear()
        {
            throw new NotImplementedException("This is a starfield mixer, Clear should not be called directly. It must be called on one of the 3 component starfields.");
        }
    }
}
