using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    public class ConvertColorSpace
    {
        public static System.Drawing.Color ToRGB(CIELAB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static System.Drawing.Color ToRGB(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static System.Drawing.Color ToRGB(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static System.Drawing.Color ToRGB(HSB colorSpace)
        {
            double r = 0.0d;
            double g = 0.0d;
            double b = 0.0d;

            if(colorSpace.Saturation == 0)
            {
                r = g = b = colorSpace.Brightness;
            }
            else
            {
                double hSecPos = colorSpace.Hue/ 60.0d;
                int hSec = (int)Math.Floor(hSecPos);
                double hSecFrac = hSecPos - hSec;

                double p = b * (1.0 - colorSpace.Saturation);
                double q = b * (1.0 - (colorSpace.Saturation * hSecFrac));
                double t = b * (1.0 - (colorSpace.Saturation * (1 - hSecFrac)));

                switch(hSec)
                {
                    case 0:
                        r = b;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = b;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = b;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = colorSpace.Brightness;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = colorSpace.Brightness;
                        break;
                    case 5:
                        r = b;
                        g = p;
                        b = q;
                        break;
                }
            }

            return System.Drawing.Color.FromArgb(
                Convert.ToInt32(Math.Round(r)),
                Convert.ToInt32(Math.Round(g)),
                Convert.ToInt32(Math.Round(b))
            );
        }

        public static System.Drawing.Color ToRGB(HSL colorSpace)
        {
            throw new NotImplementedException();
        }

        public static System.Drawing.Color ToRGB(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIELAB ToCIELAB(System.Drawing.Color colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToCIEXYZ(colorSpace));
        }

        public static CIELAB ToCIELAB(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIELAB ToCIELAB(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIELAB ToCIELAB(HSB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIELAB ToCIELAB(HSL colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIELAB ToCIELAB(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIEXYZ ToCIEXYZ(System.Drawing.Color colorSpace)
        {
            double rNormalized = (double)colorSpace.R / 255.0d;
            double gNormalized = (double)colorSpace.G / 255.0d;
            double bNormalized = (double)colorSpace.B / 255.0d;

            double standardRed = (rNormalized > 0.04045) ? 
                Math.Pow((rNormalized + 0.055) / (1 + 0.055), 2.2) : (rNormalized / 12.92);
            double standardGreen = (gNormalized > 0.04045) ? 
                Math.Pow((gNormalized + 0.055) / (1 + 0.055), 2.2) : (gNormalized / 12.92);
            double standardBlue = (bNormalized > 0.04045) ?
                Math.Pow((bNormalized + 0.055) / (1 + 0.055), 2.2) : (bNormalized / 12.92);

            return new CIEXYZ(
                (standardRed * 0.4124 + standardGreen * 0.3576 + standardBlue * 0.1805),
                (standardRed * 0.2126 + standardGreen * 0.7152 + standardBlue * 0.0722),
                (standardRed * 0.0193 + standardGreen * 0.1192 + standardBlue * 0.9505)
            );
        }

        public static CIEXYZ ToCIEXYZ(CIELAB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIEXYZ ToCIEXYZ(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIEXYZ ToCIEXYZ(HSB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIEXYZ ToCIEXYZ(HSL colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CIEXYZ ToCIEXYZ(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CMYK ToCMYK(System.Drawing.Color colorSpace)
        {
            double c = (double)(255 - colorSpace.R) / 255.0d;
            double m = (double)(255 - colorSpace.G) / 255.0d;
            double y = (double)(255 - colorSpace.B) / 255.0d;

            double k = (double)Math.Min(c, Math.Min(m, y));

            if(k == 1.0)
            {
                return new CMYK(0,0,0,1);
            }
            else
            {
                return new CMYK((c - k) / (1 - k), (m - k) / (1 - k), (y - k) / (1 - k), k);
            }
        }

        public static CMYK ToCMYK(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToCMYK(System.Drawing.Color.Pink);
        }

        public static CMYK ToCMYK(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CMYK ToCMYK(HSB colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        public static CMYK ToCMYK(HSL colorSpace)
        {
            throw new NotImplementedException();
        }

        public static CMYK ToCMYK(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSB ToHSB(System.Drawing.Color colorSpace)
        {
            double rNormalized = (double)colorSpace.R / 255.0d;
            double gNormalized = (double)colorSpace.G / 255.0d;
            double bNormalized = (double)colorSpace.B / 255.0d;

            double max = Math.Max(rNormalized, Math.Max(gNormalized, bNormalized));
            double min = Math.Min(rNormalized, Math.Min(gNormalized, bNormalized));
            double diff = max - min;
            double gbDiff = gNormalized - bNormalized;

            double h = 0.0;
            double s;
            double b = max;

            if(max == rNormalized && gNormalized >= bNormalized)
            {
                h = 60 * gbDiff / diff;
            }
            else
            if(max == rNormalized && gNormalized < bNormalized)
            {
                h = 60 * gbDiff / diff + 360.0d;
            }
            else
            if(max == gNormalized)
            {
                h = 60 * (bNormalized - rNormalized) / diff + 120;
            }
            else
            if(max == bNormalized)
            {
                h = 60 * (rNormalized - gNormalized) / diff + 240;
            }

            if(max == 0)
            {
                s = 0.0d;
            }
            else
            {
                s = 10 - min / max;
            }

            return new HSB(h, s, b);
        }

        public static HSB ToHSB(CIELAB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSB ToHSB(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSB ToHSB(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSB ToHSB(HSL colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSB ToHSB(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSL ToHSL(System.Drawing.Color colorSpace)
        {
            double rNormalized = (double)colorSpace.R / 255.0d;
            double gNormalized = (double)colorSpace.G / 255.0d;
            double bNormalized = (double)colorSpace.B / 255.0d;

            double max = Math.Max(rNormalized, Math.Max(gNormalized, bNormalized));
            double min = Math.Min(rNormalized, Math.Min(gNormalized, bNormalized));
            double diff = max - min;
            double gbDiff = gNormalized - bNormalized;

            double h = 0.0d;
            double s = 0.0d;
            double l = (max + min) / 2.0d;

            if(max == rNormalized && gNormalized >= bNormalized)
            {
                h = 60 * gbDiff / diff;
            }
            else if(max == rNormalized && gNormalized < bNormalized)
            {
                h = 60 * gbDiff / diff + 360.0d;
            }
            else if(max == gNormalized)
            {
                h = 60 * (bNormalized - rNormalized) / diff + 120;
            }
            else if(max == bNormalized)
            {
                h = 60 * (rNormalized - gNormalized) / diff + 240;
            }

            if(l == 0 && max == min)
            {
                s = 0.0d;
            }
            else if(0 < l && l <= 0.5d)
            {
                s = diff / (max + min);
            }
            else if(l > 0.5)
            {
                s = diff / (2 - (max + min));
            }

            return new HSL(h, s, l);
        }

        public static HSL ToHSL(CIELAB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSL ToHSL(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSL ToHSL(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static HSL ToHSL(HSB colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        public static HSL ToHSL(YUV colorSpace)
        {
            throw new NotImplementedException();
        }

        public static YUV ToYUV(System.Drawing.Color colorSpace)
        {
            double rNormalized = (double)colorSpace.R / 255.0d;
            double gNormalized = (double)colorSpace.G / 255.0d;
            double bNormalized = (double)colorSpace.B / 255.0d;

            double y = 0.299 * rNormalized + 0.587 * gNormalized + 0.114 * bNormalized;
            double u = -0.14713 * rNormalized -0.28886 * gNormalized + 0.436 * bNormalized;
            double v = 0.615 * rNormalized -0.51499 * gNormalized -0.10001 * bNormalized;

            return new YUV(y, u, v);

        }

        public static YUV ToYUV(CIELAB colorSpace)
        {
            throw new NotImplementedException();
        }

        public static YUV ToYUV(CIEXYZ colorSpace)
        {
            throw new NotImplementedException();
        }

        public static YUV ToYUV(CMYK colorSpace)
        {
            throw new NotImplementedException();
        }

        public static YUV ToYUV(HSB colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }

        public static YUV ToYUV(HSL colorSpace)
        {
            throw new NotImplementedException();
        }
    }
}

