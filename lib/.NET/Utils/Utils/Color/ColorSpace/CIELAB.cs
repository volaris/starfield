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

        public double L
        {
            get { return l; }
            set { l = Math.Min(100.0d, Math.Max(0.0d, value)); }
        }

        public double A
        {
            get { return a; }
            set { a = value; }
        }

        public double B
        {
            get { return b; }
            set { b = value; }
        }

        public CIELAB(double l, double a, double b)
        {
            this.L = l;
            this.A = a;
            this.B = b;
        }

        public static bool operator ==(CIELAB left, CIELAB right)
        {
            return (left.L == right.L) &&
                (left.A == right.A) &&
                (left.B == right.B);
        }

        public static bool operator !=(CIELAB left, CIELAB right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is CIELAB))
            {
                return false;
            }

            return this == (CIELAB)obj;
        }

        public override int GetHashCode()
        {
            return l.GetHashCode() ^ a.GetHashCode() ^ b.GetHashCode();
        }
    }
}

