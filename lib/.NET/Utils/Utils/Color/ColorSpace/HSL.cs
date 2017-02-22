using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /** <summary>    HSL color space. </summary> */
    public class HSL
    {
        private double hue;
        private double saturation;
        private double luminosity;

        /**
         * <summary>    Gets or sets the hue. </summary>
         *
         * <value>  The hue. </value>
         */

        public double Hue
        {
            get { return hue; }
            set { hue = Math.Min(360.0d, Math.Max(0.0d, value)); }
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
         * <summary>    Gets or sets the luminosity. </summary>
         *
         * <value>  The luminosity. </value>
         */

        public double Luminosity
        {
            get { return luminosity; }
            set { luminosity = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="hue">           The hue. </param>
         * <param name="saturation">    The saturation. </param>
         * <param name="luminosity">    The luminosity. </param>
         */

        public HSL(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(HSL left, HSL right)
        {
            return (left.Hue == right.Hue) &&
                (left.Saturation == right.Saturation) &&
                (left.Luminosity == right.Luminosity);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(HSL left, HSL right)
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
            if(obj == null || !(obj is HSL))
            {
                return false;
            }
            else
            {
                return this == (HSL)obj;
            }
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
            return hue.GetHashCode() ^ saturation.GetHashCode() ^ luminosity.GetHashCode();
        }
    }
}

