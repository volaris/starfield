using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class CIEXYZ
    {
        private double x;
        private double y;
        private double z;

        public double X
        {
            get { return x; }
            set { x = Math.Min(0.9505d, Math.Max(0.0, value)); }
        }

        public double Y
        {
            get { return y; }
            set { y = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Z
        {

            get { return z; }
            set { z = Math.Min(1.089d, Math.Max(0.0d, value)); }
        }

        public CIEXYZ(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(CIEXYZ left, CIEXYZ right)
        {
            return (left.X == right.X) &&
                (left.Y == right.Y) &&
                (left.Z == right.Z);
        }

        public static bool operator !=(CIEXYZ left, CIEXYZ right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is CIEXYZ))
            {
                return false;
            }

            return this == (CIEXYZ)obj;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
        }
    }
}

