using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace StarfieldUtils.ColorUtils
{
    public class ColorUtils
    {
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
                redDiff = (byte)(Math.Max(0, Math.Min(255, redDelta * n)));
                blueDiff = (byte)(Math.Max(0, Math.Min(255, blueDelta * n)));
                greenDiff = (byte)(Math.Max(0, Math.Min(255, greenDelta * n)));
            }
            else
            {
                redDiff = (byte)((redDelta * n));
                blueDiff = (byte)((blueDelta * n));
                greenDiff = (byte)((greenDelta * n));
            }

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
    }
}
