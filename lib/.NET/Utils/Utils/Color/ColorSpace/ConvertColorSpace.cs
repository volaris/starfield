using System;

namespace StarfieldUtils.ColorUtils.ColorSpace
{
    //TODO: RGB conversion is lossy. Intermediate conversions should be to a non lossy color space like CIELAB instead.

    /** <summary>    A class to convert color spaces. </summary> */
    public class ConvertColorSpace
    {
        /**
         * <summary>    Converts a CIELAB color to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

        public static System.Drawing.Color ToRGB(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToRGB(ConvertColorSpace.ToCIEXYZ(colorSpace));
        }

        /**
         * <summary>    Converts a CIEXYZ color  to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

        public static System.Drawing.Color ToRGB(CIEXYZ colorSpace)
        {
            double[] rgb = new double[3];
            rgb[0] = colorSpace.X * 3.2410 - colorSpace.Y * 1.5374 - colorSpace.Z * 0.4986;
            rgb[1] = -colorSpace.X * 0.9692 + colorSpace.Y * 1.8760 - colorSpace.Z * 0.0416;
            rgb[2] = colorSpace.X * 0.0556 - colorSpace.Y * 0.2040 + colorSpace.Z * 1.0570;

            for (int i = 0; i < 3; i++)
            {
                rgb[i] = (rgb[i] <= 0.0031308) ? 12.92 * rgb[i] : (1 + 0.055) * Math.Pow(rgb[i], (1.0 / 2.4)) - 0.055;
            }

            return System.Drawing.Color.FromArgb(
                Convert.ToInt32(Math.Round(rgb[0] * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round(rgb[1] * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round(rgb[2] * 255.0d, MidpointRounding.AwayFromZero)));
        }

        /**
         * <summary>    Converts a CMYK color to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

        public static System.Drawing.Color ToRGB(CMYK colorSpace)
        {
            return System.Drawing.Color.FromArgb(
                Convert.ToInt32(Math.Round((1 - colorSpace.Cyan) * (1 - colorSpace.Black) * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round((1 - colorSpace.Magenta) * (1 - colorSpace.Black) * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round((1 - colorSpace.Yellow) * (1 - colorSpace.Black) * 255.0d, MidpointRounding.AwayFromZero)));
        }

        /**
         * <summary>    Converts a HSB color to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

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
                Convert.ToInt32(Math.Round(r * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round(g * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round(b * 255.0d, MidpointRounding.AwayFromZero)));
        }

        /**
         * <summary>    Converts a HSL color to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

        public static System.Drawing.Color ToRGB(HSL colorSpace)
        {
            if(colorSpace.Saturation == 0)
            {
                return System.Drawing.Color.FromArgb(
                    Convert.ToInt32(Math.Round(colorSpace.Luminosity * 255.0d, MidpointRounding.AwayFromZero)),
                    Convert.ToInt32(Math.Round(colorSpace.Luminosity * 255.0d, MidpointRounding.AwayFromZero)),
                    Convert.ToInt32(Math.Round(colorSpace.Luminosity * 255.0d, MidpointRounding.AwayFromZero)));
            }
            else
            {
                double q = (colorSpace.Luminosity < 0.5) ? (colorSpace.Luminosity * (1.0 + colorSpace.Saturation)) :
                    (colorSpace.Luminosity + colorSpace.Saturation - (colorSpace.Luminosity * colorSpace.Saturation));
                double p = (2.0 * colorSpace.Luminosity) - q;

                double hueNormalized = colorSpace.Hue / 360.0;
                double[] rgb = new double[3];
                rgb[0] = hueNormalized + (1.0 / 3.0);
                rgb[1] = hueNormalized;
                rgb[2] = hueNormalized - (1.0 / 3.0);

                for (int i = 0; i < 3; i++)
                {
                    if (rgb[i] < 0) rgb[i] += 1.0;
                    if (rgb[i] > 1) rgb[i] -= 1.0;

                    if ((rgb[i] * 6) < 1)
                    {
                        rgb[i] = p + ((q - p) * 6.0 * rgb[i]);
                    }
                    else if ((rgb[i] * 2.0) < 1)
                    {
                        rgb[i] = q;
                    }
                    else if ((rgb[i] * 3.0) < 2)
                    {
                        rgb[i] = p + (q - p) * ((2.0 / 3.0) - rgb[i]) * 6.0;
                    }
                    else rgb[i] = p;
                }


                return System.Drawing.Color.FromArgb(
                    Convert.ToInt32(Math.Round(rgb[0] * 255.0d, MidpointRounding.AwayFromZero)),
                    Convert.ToInt32(Math.Round(rgb[1] * 255.0d, MidpointRounding.AwayFromZero)),
                    Convert.ToInt32(Math.Round(rgb[2] * 255.0d, MidpointRounding.AwayFromZero)));
            }
        }

        /**
         * <summary>    Converts a YUV color to a RGB color. </summary>
         *
         * <param name="colorSpace">    The color. </param>
         *
         * <returns>    ColorSpace as a System.Drawing.Color. </returns>
         */

        public static System.Drawing.Color ToRGB(YUV colorSpace)
        {
            return System.Drawing.Color.FromArgb(
                Convert.ToInt32(Math.Round((colorSpace.Y + 1.139837398373983740 * colorSpace.V) * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round((colorSpace.Y - 0.3946517043589703515 * colorSpace.U - 0.5805986066674976801 * colorSpace.V) * 255.0d, MidpointRounding.AwayFromZero)),
                Convert.ToInt32(Math.Round((colorSpace.Y + 2.032110091743119266 * colorSpace.U) * 255.0d, MidpointRounding.AwayFromZero)));
        }

        /**
         * <summary>    Converts a RGB color to a CIELAB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(System.Drawing.Color colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToCIEXYZ(colorSpace));
        }

        delegate double xyzTransform(double d);

        /**
         * <summary>    Converts a CIEXYZ color to a CIELAB. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(CIEXYZ colorSpace)
        {
            xyzTransform f = x => ((x > 0.008856d) ? Math.Pow(x, (1.0d / 3.0d)) : (7.787d * x + 16.0d / 116.0d));

            return new CIELAB(
                116.0d * f(colorSpace.Y) - 16,
                500.0d * f(colorSpace.X / 0.9505d) - f(colorSpace.Y),
                200.0d * f(colorSpace.Y) - f(colorSpace.Z / 1.089d));
        }

        /**
         * <summary>    Converts a CMYK color to a CIELAB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(CMYK colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSB color to a CIELAB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(HSB colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSL color to a CIELAB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(HSL colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a YUV color to a CIELAB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIELAB. </returns>
         */

        public static CIELAB ToCIELAB(YUV colorSpace)
        {
            return ConvertColorSpace.ToCIELAB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a RGB color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

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

        /**
         * <summary>    Converts a CIELAB color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

        public static CIEXYZ ToCIEXYZ(CIELAB colorSpace)
        {
            double delta = 6.0 / 29.0;

            double fy = (colorSpace.L + 16) / 116.0;
            double fx = fy + (colorSpace.A / 500.0);
            double fz = fy - (colorSpace.B / 200.0);

            double deltaSquared = Math.Pow(delta, 2);
            double fxCubed = Math.Pow(fx, 3);
            double fyCubed = Math.Pow(fy, 3);
            double fzCubed = Math.Pow(fz, 3);

            return new CIEXYZ(
                (fx > delta) ? 0.9505d * fxCubed : (fx - 16.0 / 116.0) * 3 * (deltaSquared) * 0.9505,
                (fy > delta) ? fyCubed : (fy - 16.0 / 116.0) * 3 * (deltaSquared),
                (fz > delta) ? 1.089d * fzCubed : (fz - 16.0 / 116.0) * 3 * (deltaSquared) * 1.089d);
        }

        /**
         * <summary>    Converts a CMYK color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

        public static CIEXYZ ToCIEXYZ(CMYK colorSpace)
        {
            return ConvertColorSpace.ToCIEXYZ(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSB color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

        public static CIEXYZ ToCIEXYZ(HSB colorSpace)
        {
            return ConvertColorSpace.ToCIEXYZ(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSL color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

        public static CIEXYZ ToCIEXYZ(HSL colorSpace)
        {
            return ConvertColorSpace.ToCIEXYZ(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a YUV color to a CIEXYZ color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CIEXYZ. </returns>
         */

        public static CIEXYZ ToCIEXYZ(YUV colorSpace)
        {
            return ConvertColorSpace.ToCIEXYZ(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a RGB color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

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

        /**
         * <summary>    Converts a CIELAB color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

        public static CMYK ToCMYK(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CIEXYC color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

        public static CMYK ToCMYK(CIEXYZ colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSB color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

        public static CMYK ToCMYK(HSB colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSL color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

        public static CMYK ToCMYK(HSL colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a YUV color to a CMYK color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a CMYK. </returns>
         */

        public static CMYK ToCMYK(YUV colorSpace)
        {
            return ConvertColorSpace.ToCMYK(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a RGB color to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

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

        /**
         * <summary>    Converts a CIELAB color to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

        public static HSB ToHSB(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToHSB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CIEXYC color to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

        public static HSB ToHSB(CIEXYZ colorSpace)
        {
            return ConvertColorSpace.ToHSB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CMYK color to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

        public static HSB ToHSB(CMYK colorSpace)
        {
            return ConvertColorSpace.ToHSB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSL color to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

        public static HSB ToHSB(HSL colorSpace)
        {
            return ConvertColorSpace.ToHSB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a YUV to an HSB color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSB. </returns>
         */

        public static HSB ToHSB(YUV colorSpace)
        {
            return ConvertColorSpace.ToHSB(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a RGB to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

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

        /**
         * <summary>    Converts a CIELAB to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

        public static HSL ToHSL(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CIEXYZ to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

        public static HSL ToHSL(CIEXYZ colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CMYK to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

        public static HSL ToHSL(CMYK colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSB color to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

        public static HSL ToHSL(HSB colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a YUV color to an HSL color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a HSL. </returns>
         */

        public static HSL ToHSL(YUV colorSpace)
        {
            return ConvertColorSpace.ToHSL(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a RGB to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

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

        /**
         * <summary>    Converts a CIELAB color to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

        public static YUV ToYUV(CIELAB colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CIEXYZ color to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

        public static YUV ToYUV(CIEXYZ colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a CMYK color to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

        public static YUV ToYUV(CMYK colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSB color to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

        public static YUV ToYUV(HSB colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }

        /**
         * <summary>    Converts a HSL color to a YUV color. </summary>
         *
         * <param name="colorSpace">    The color space. </param>
         *
         * <returns>    ColorSpace as a YUV. </returns>
         */

        public static YUV ToYUV(HSL colorSpace)
        {
            return ConvertColorSpace.ToYUV(ConvertColorSpace.ToRGB(colorSpace));
        }
    }
}

