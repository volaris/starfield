using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class CMYK
    {
        private double cyan;
        private double magenta;
        private double yellow;
        private double black;

        public double Cyan
        {
            get { return cyan; }
            set { cyan = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Magenta
        {
            get { return magenta; }
            set { magenta = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Yellow
        {
            get { return yellow; }
            set { yellow = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public double Black
        {
            get { return black; }
            set { black = Math.Min(1.0d, Math.Max(0.0d, value)); }
        }

        public CMYK(double cyan, double magenta, double yellow, double black)
        {
            this.Cyan = cyan;
            this.Magenta = magenta;
            this.Yellow = yellow;
            this.Black = black;
        }

        public static bool operator ==(CMYK left, CMYK right)
        {
            return (left.Cyan == right.Cyan) &&
                (left.Magenta == right.Magenta) &&
                (left.Yellow == right.Yellow) &&
                (left.Black == right.Black);
        }

        public static bool operator !=(CMYK left, CMYK right)
        {
            return !(left == right);
        }

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

        public override int GetHashCode()
        {
            return cyan.GetHashCode() ^ magenta.GetHashCode() ^ yellow.GetHashCode() ^ black.GetHashCode();
        }
    }
}

