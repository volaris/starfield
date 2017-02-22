using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /** <summary>    YUV color space. </summary> */
    public class YUV
    {
        private double y;
        private double u;
        private double v;

        /**
         * <summary>    Gets or sets the luma. </summary>
         *
         * <value>  The luma. </value>
         */

        public double Y
        {
            get { return y; }
            set { y = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the u chrominance. </summary>
         *
         * <value>  The u chrominance. </value>
         */

        public double U
        {
            get { return u; }
            set { u = Math.Min(0.436d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the v chrominance. </summary>
         *
         * <value>  The v chrominance. </value>
         */

        public double V
        {
            get { return v; }
            set { v = Math.Min(0.615d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="y"> The luman. </param>
         * <param name="u"> The u chrominance. </param>
         * <param name="v"> The y chrominance. </param>
         */

        public YUV(double y, double u, double v)
        {
            this.Y = y;
            this.U = u;
            this.V = v;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(YUV left, YUV right)
        {
            return (left.Y == right.Y) &&
                (left.U == right.U) &&
                (left.V == right.V);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(YUV left, YUV right)
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
            if(obj == null || !(obj is YUV))
            {
                return false;
            }

            return this == (YUV)obj;
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
            return y.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
        }
    }
}

