using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StarfieldUtils.ColorUtils
{
    /**
     * <summary>    Various helper functions for manipulating color. </summary>
     */

    public class ColorUtils
    {
        /**
         * <summary>    use n as a percent to determine the color between start and goal n should be in
         *              the range [0.0,1.0], if it isn't wraparound can occur, preventOverlow will cap the value. </summary>
         *
         * <param name="start">             The start. </param>
         * <param name="goal">              The goal. </param>
         * <param name="n">                 The percent of the gradient between start and goal [0.0,1.0]. </param>
         * <param name="preventOverflow">   True to prevent overflow. </param>
         *
         * <returns>    The gradient color. </returns>
         */

        public static Color GetGradientColor(Color start, Color goal, float n, bool preventOverflow)
        {
            byte redDelta, goalRed, startRed, redDiff, red;
            byte greenDelta, goalGreen, startGreen, greenDiff, green;
            byte blueDelta, goalBlue, startBlue, blueDiff, blue;

            goalRed = goal.R;
            goalGreen = goal.G;
            goalBlue = goal.B;

            startRed = start.R;
            startGreen = start.G;
            startBlue = start.B;

            redDelta = (byte)System.Math.Abs(goalRed - startRed);
            blueDelta = (byte)System.Math.Abs(goalBlue - startBlue);
            greenDelta = (byte)System.Math.Abs(goalGreen - startGreen);

            if (preventOverflow)
            {
                n = Math.Min(1.0f, Math.Max(0f, n));
            }

            redDiff = (byte)((redDelta * n));
            blueDiff = (byte)((blueDelta * n));
            greenDiff = (byte)((greenDelta * n));

            if (goalRed < startRed)
            {
                red = (byte)(startRed - redDiff);
            }
            else
            {
                red = (byte)(startRed + redDiff);
            }

            if (goalGreen < startGreen)
            {
                green = (byte)(startGreen - greenDiff);
            }
            else
            {
                green = (byte)(startGreen + greenDiff);
            }

            if (goalBlue < startBlue)
            {
                blue = (byte)(startBlue - blueDiff);
            }
            else
            {
                blue = (byte)(startBlue + blueDiff);
            }

            return Color.FromArgb(red, green, blue);
        }

        /**
         * <summary>    takes a list of know reference points and uses n as a percent between them. </summary>
         *
         * <param name="colors">            The colors. </param>
         * <param name="n">                 The percent of the gradient on the multi-stop color path [0.0,1.0]. </param>
         * <param name="preventOverflow">   True to prevent overflow. </param>
         *
         * <returns>    The multi color gradient. </returns>
         */

        public static Color GetMultiColorGradient(Color[] colors, float n, bool preventOverflow)
        {
            if(n < 0)
            {
                return colors[0];
            }
            if(n > 1)
            {
                return colors[colors.Length - 1];
            }

            int index1 = (int)(Math.Floor((colors.Length - 1) * n));
            int index2 = (int)(Math.Ceiling((colors.Length - 1) * n));
            float percent = ((colors.Length - 1) * n) - index1;
            return StarfieldUtils.ColorUtils.ColorUtils.GetGradientColor(colors[index1], colors[index2], percent, true);
        }

        /**
         * <summary>    Retrieves a random vibrant color. For the purposes of the starfield, vibrant colors are defined as those without
         *              any grey component, this means that only two of the color components can have values. </summary>
         *
         * <returns>    The random vibrant color. </returns>
         */

        public static Color GetRandomVibrantColor()
        {
            Random rand = new Random();
            double val = rand.NextDouble();
            Color color1, color2;

            double increment = 1.0 / 6;

            if(val < increment) // red: 255 green: variable increasing
            {
                color1 = Color.FromArgb(0xFF, 0, 0);
                color2 = Color.FromArgb(0xFF, 0xFF, 0);
                return GetGradientColor(color1, color2, (float)(val * 6), true);
            }
            else if (val < 2 * increment) // green: 255 red: variable decreasing
            {
                color1 = Color.FromArgb(0xFF, 0xFF, 0);
                color2 = Color.FromArgb(0, 0xFF, 0);
                return GetGradientColor(color1, color2, (float)((val - increment) * 6), true);
            }
            else if(val < 3 * increment) // green: 255 blue: variable increasing
            {
                color1 = Color.FromArgb(0, 0xFF, 0);
                color2 = Color.FromArgb(0, 0xFF, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 2 * increment) * 6), true);
            }
            else if(val < 4 * increment) // blue: 255 green: variable decreasing
            {
                color1 = Color.FromArgb(0, 0xFF, 0xFF);
                color2 = Color.FromArgb(0, 0, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 3 * increment) * 6), true);
            }
            else if(val < 5 * increment) // blue 255 red: variable increasing
            {
                color1 = Color.FromArgb(0, 0, 0xFF);
                color2 = Color.FromArgb(0xFF, 0, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 4 * increment) * 6), true);
            }
            else // (val <= 1) // red: 255 blue: variable decresing
            {
                color1 = Color.FromArgb(0xFF, 0, 0xFF);
                color2 = Color.FromArgb(0xFF, 0, 0);
                return GetGradientColor(color1, color2, (float)((val - 5 * increment) * 6), true);
            }
        }




        /**
         * <summary>  This function takes a single float as a percentage of red to red
                      on the vibrant color spectrum as described above. Both 0.0 and 1.0
                      will yield red. </summary>
         * <para>  For the purposes of the starfield, vibrant colors are defined as those 
         *         without any grey component, this means that only two of the color 
         *         components can have values.</para>
         *
         * <param name="n"> The float to process. </param>
         *
         * <returns>    The vibrant color gradient. </returns>
         */
        // TODO: refactor GetRandomVibrantColor to use this function.
        public static Color GetVibrantColorGradient(float n)
        {
            Random rand = new Random();
            double val = n;
            Color color1, color2;

            double increment = 1.0 / 6;

            if (val < increment) // red: 255 green: variable increasing
            {
                color1 = Color.FromArgb(0xFF, 0, 0);
                color2 = Color.FromArgb(0xFF, 0xFF, 0);
                return GetGradientColor(color1, color2, (float)(val * 6), true);
            }
            else if (val < 2 * increment) // green: 255 red: variable decreasing
            {
                color1 = Color.FromArgb(0xFF, 0xFF, 0);
                color2 = Color.FromArgb(0, 0xFF, 0);
                return GetGradientColor(color1, color2, (float)((val - increment) * 6), true);
            }
            else if (val < 3 * increment) // green: 255 blue: variable increasing
            {
                color1 = Color.FromArgb(0, 0xFF, 0);
                color2 = Color.FromArgb(0, 0xFF, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 2 * increment) * 6), true);
            }
            else if (val < 4 * increment) // blue: 255 green: variable decreasing
            {
                color1 = Color.FromArgb(0, 0xFF, 0xFF);
                color2 = Color.FromArgb(0, 0, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 3 * increment) * 6), true);
            }
            else if (val < 5 * increment) // blue 255 red: variable increasing
            {
                color1 = Color.FromArgb(0, 0, 0xFF);
                color2 = Color.FromArgb(0xFF, 0, 0xFF);
                return GetGradientColor(color1, color2, (float)((val - 4 * increment) * 6), true);
            }
            else // (val <= 1) // red: 255 blue: variable decresing
            {
                color1 = Color.FromArgb(0xFF, 0, 0xFF);
                color2 = Color.FromArgb(0xFF, 0, 0);
                return GetGradientColor(color1, color2, (float)((val - 5 * increment) * 6), true);
            }
        }

        /**
         * <summary>    Blend the colors using a weighted average. </summary>
         *
         * <remarks>    Volar, 2/13/2017. </remarks>
         *
         * <exception cref="ArgumentException"> Thrown when colors and weights don't have the same number of elements. </exception>
         *
         * <param name="colors">    The colors. </param>
         * <param name="weights">   The weights. </param>
         *
         * <returns>    A Color. </returns>
         */

        public static Color BlendAveraged(Color[] colors, double[] weights)
        {
            double red = 0;
            double green = 0;
            double blue = 0;
            double weight = 0;

            if(colors.Length != weights.Length)
            {
                throw new ArgumentException("colors and weights must have the same number of elements");
            }

            for(int i = 0; i < colors.Length; i++)
            {
                red += colors[i].R * weights[i];
                green += colors[i].G * weights[i];
                blue += colors[i].B * weights[i];
                weight += weights[i];
            }

            red /= weight;
            green /= weight;
            blue /= weight;

            return Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
        }

        /**
         * <summary>    Blend using raw weights. </summary>
         *
         * <exception cref="ArgumentException"> Thrown when colors and weights don't have the same number of elements. </exception>
         *
         * <param name="colors">    The colors. </param>
         * <param name="weights">   The weights. </param>
         *
         * <returns>    A Color. </returns>
         */

        public static Color BlendRaw(Color[] colors, double[] weights)
        {
            double red = 0;
            double green = 0;
            double blue = 0;

            if (colors.Length != weights.Length)
            {
                throw new ArgumentException("colors and weights must have the same number of elements");
            }

            for (int i = 0; i < colors.Length; i++)
            {
                red += colors[i].R * weights[i];
                green += colors[i].G * weights[i];
                blue += colors[i].B * weights[i];
            }

            return Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
        }

        /**
         * <summary>    Converts a color to a gray scale. </summary>
         *
         * <param name="color"> The color to convert. </param>
         *
         * <returns>    Color as a grayscal Color. </returns>
         */

        public static Color ToGrayScale(Color color)
        {
            ColorSpace.CIELAB cielab = ColorSpace.ConvertColorSpace.ToCIELAB(color);
            cielab.A = 0;
            cielab.B = 0;
            return ColorSpace.ConvertColorSpace.ToRGB(cielab);
        }
    }
}
