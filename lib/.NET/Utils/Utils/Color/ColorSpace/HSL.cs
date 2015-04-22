using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class HSL
    {
        private double hue;
        private double saturation;
        private double luminosity;

        public double Hue
        {
            get { return hue; }
            set { hue = Math.Min(360.0d, Math.Max(0.0d, value)); }
        }

        public double Saturation
        {
            get { return saturation; }
            set { saturation = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Luminosity
        {
            get { return luminosity; }
            set { luminosity = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public HSL(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

        public static bool operator ==(HSL left, HSL right)
        {
            return (left.Hue == right.Hue) &&
                (left.Saturation == right.Saturation) &&
                (left.Luminosity == right.Luminosity);
        }

        public static bool operator !=(HSL left, HSL right)
        {
            return !(left == right);
        }

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

        public override int GetHashCode()
        {
            return hue.GetHashCode() ^ saturation.GetHashCode() ^ luminosity.GetHashCode();
        }
    }
}

