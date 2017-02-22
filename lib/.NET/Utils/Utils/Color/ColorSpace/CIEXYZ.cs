using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    /** <summary>    CIE XYZ color space. Also known as CIE 1931.</summary> */
    public class CIEXYZ
    {
        private double x;
        private double y;
        private double z;

        /**
         * <summary>    Gets or sets the x component. This is a linear combination of cone response curves. </summary>
         *
         * <value>  The x coordinate. </value>
         */

        public double X
        {
            get { return x; }
            set { x = Math.Min(0.9505d, Math.Max(0.0, value)); }
        }

        /**
         * <summary>    Gets or sets the y component. This is the luminance. </summary>
         *
         * <value>  The y coordinate. </value>
         */

        public double Y
        {
            get { return y; }
            set { y = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Gets or sets the z component. This is quasi-equal to blue stimulation. </summary>
         *
         * <value>  The z coordinate. </value>
         */

        public double Z
        {

            get { return z; }
            set { z = Math.Min(1.089d, Math.Max(0.0d, value)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="x"> The x component. </param>
         * <param name="y"> The y component. </param>
         * <param name="z"> The z component. </param>
         */

        public CIEXYZ(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(CIEXYZ left, CIEXYZ right)
        {
            return (left.X == right.X) &&
                (left.Y == right.Y) &&
                (left.Z == right.Z);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(CIEXYZ left, CIEXYZ right)
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
            if(obj == null || !(obj is CIEXYZ))
            {
                return false;
            }

            return this == (CIEXYZ)obj;
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
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
        }
    }
}

