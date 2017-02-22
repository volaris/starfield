using System;


/**
// namespace: StarfieldUtils.ColorUtils.ColorSpace
//
// summary:	Classes that represent different color spaces and utilities to convert between them.
 */

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /**
     * <summary>    A CIELAB color space. </summary>
     */

    public class CIELAB
    {
        private double l;
        private double a;
        private double b;

        /**
         * <summary>    Gets or sets the lightness. 0 for black and 100 for white.</summary>
         *
         * <value>  The lightness. </value>
         */

        public double L
        {
            get { return l; }
            set { l = Math.Min(100.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the Green/Magenta component. Negative for green, positive for magenta.</summary>
         *
         * <value>  a. </value>
         */

        public double A
        {
            get { return a; }
            set { a = value; }
        }

        /**
         * <summary>    Gets or sets the blue/yellow component. Negative for blue, positive for yellow</summary>
         *
         * <value>  The b. </value>
         */

        public double B
        {
            get { return b; }
            set { b = value; }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="l"> The lightness component. </param>
         * <param name="a"> The a component. </param>
         * <param name="b"> The b component. </param>
         */

        public CIELAB(double l, double a, double b)
        {
            this.L = l;
            this.A = a;
            this.B = b;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(CIELAB left, CIELAB right)
        {
            return (left.L == right.L) &&
                (left.A == right.A) &&
                (left.B == right.B);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(CIELAB left, CIELAB right)
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
            if(obj == null || !(obj is CIELAB))
            {
                return false;
            }

            return this == (CIELAB)obj;
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
            return l.GetHashCode() ^ a.GetHashCode() ^ b.GetHashCode();
        }
    }
}

