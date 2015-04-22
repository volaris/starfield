using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class HSB
    {
        private double hue;
        private double saturation;
        private double brightness;

        public double Hue
        {
            get { return hue; }
            set { hue = Math.Min(360, Math.Max(0, value)); }
        }

        public double Saturation
        {
            get { return saturation; }
            set { saturation = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Brightness
        {
            get { return brightness; }
            set { brightness = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public HSB(double hue, double saturation, double brightness)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Brightness = brightness;
        }

        public static bool operator ==(HSB left, HSB right)
        {
            return (left.Hue == right.Hue) &&
                (left.Saturation == right.Saturation) &&
                (left.Brightness == right.Brightness);
        }

        public static bool operator !=(HSB left, HSB right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is HSB))
            {
                return false;
            }

            return this == (HSB)obj;
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^ Brightness.GetHashCode();
        }
    }
}

