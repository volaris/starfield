using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class YUV
    {
        private double y;
        private double u;
        private double v;

        public double Y
        {
            get { return y; }
            set { y = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double U
        {
            get { return u; }
            set { u = Math.Min(0.436d, Math.Max(0.0d, value)); }
        }

        public double V
        {
            get { return v; }
            set { v = Math.Min(0.615d, Math.Max(0.0d, value)); }
        }

        public YUV(double y, double u, double v)
        {
            this.Y = y;
            this.U = u;
            this.V = v;
        }

        public static bool operator ==(YUV left, YUV right)
        {
            return (left.Y == right.Y) &&
                (left.U == right.U) &&
                (left.V == right.V);
        }

        public static bool operator !=(YUV left, YUV right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is YUV))
            {
                return false;
            }

            return this == (YUV)obj;
        }

        public override int GetHashCode()
        {
            return y.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
        }
    }
}

