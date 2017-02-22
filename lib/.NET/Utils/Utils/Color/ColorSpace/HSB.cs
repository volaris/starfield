using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /** <summary>    HSB color space. </summary> */
    public class HSB
    {
        private double hue;
        private double saturation;
        private double brightness;

        /**
         * <summary>    Gets or sets the hue. </summary>
         *
         * <value>  The hue. </value>
         */

        public double Hue
        {
            get { return hue; }
            set { hue = Math.Min(360, Math.Max(0, value)); }
        }

        /**
         * <summary>    Gets or sets the saturation. </summary>
         *
         * <value>  The saturation. </value>
         */

        public double Saturation
        {
            get { return saturation; }
            set { saturation = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the brightness. </summary>
         *
         * <value>  The brightness. </value>
         */

        public double Brightness
        {
            get { return brightness; }
            set { brightness = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="hue">           The hue. </param>
         * <param name="saturation">    The saturation. </param>
         * <param name="brightness">    The brightness. </param>
         */

        public HSB(double hue, double saturation, double brightness)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Brightness = brightness;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(HSB left, HSB right)
        {
            return (left.Hue == right.Hue) &&
                (left.Saturation == right.Saturation) &&
                (left.Brightness == right.Brightness);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(HSB left, HSB right)
        {
            return !(left == right);
        }

        /**
         * <summary>
         * Determines whether the specified <see cref="T:System.Object" />
         *  is equal to the current <see cref="T:System.Object" />
         * .
         * </summary>
         *
         * <param name="obj">   The object to compare with the current object. </param>
         *
         * <returns>
         * true if the specified object  is equal to the current object; otherwise, false.
         * </returns>
         */

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is HSB))
            {
                return false;
            }

            return this == (HSB)obj;
        }

        /**
         * <summary>    Serves as a hash function for a particular type. </summary>
         *
         * <returns>
         * A hash code for the current <see cref="T:System.Object" />
         * .
         * </returns>
         */

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^ Brightness.GetHashCode();
        }
    }
}

