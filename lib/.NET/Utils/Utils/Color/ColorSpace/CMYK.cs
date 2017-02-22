using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /** <summary>    CMYK color space. </summary> */
    public class CMYK
    {
        private double cyan;
        private double magenta;
        private double yellow;
        private double black;

        /**
         * <summary>    Gets or sets the cyan component. </summary>
         *
         * <value>  The cyan. </value>
         */

        public double Cyan
        {
            get { return cyan; }
            set { cyan = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the magenta component. </summary>
         *
         * <value>  The magenta. </value>
         */

        public double Magenta
        {
            get { return magenta; }
            set { magenta = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the yellow component. </summary>
         *
         * <value>  The yellow. </value>
         */

        public double Yellow
        {
            get { return yellow; }
            set { yellow = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the black component. </summary>
         *
         * <value>  The black. </value>
         */

        public double Black
        {
            get { return black; }
            set { black = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="cyan">      The cyan. </param>
         * <param name="magenta">   The magenta. </param>
         * <param name="yellow">    The yellow. </param>
         * <param name="black">     The black. </param>
         */

        public CMYK(double cyan, double magenta, double yellow, double black)
        {
            this.Cyan = cyan;
            this.Magenta = magenta;
            this.Yellow = yellow;
            this.Black = black;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(CMYK left, CMYK right)
        {
            return (left.Cyan == right.Cyan) &&
                (left.Magenta == right.Magenta) &&
                (left.Yellow == right.Yellow) &&
                (left.Black == right.Black);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(CMYK left, CMYK right)
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
            if(obj == null || !(obj is CMYK))
            {
                return false;
            }
            else
            {
                return this == (CMYK)obj;
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
            return cyan.GetHashCode() ^ magenta.GetHashCode() ^ yellow.GetHashCode() ^ black.GetHashCode();
        }
    }
}

