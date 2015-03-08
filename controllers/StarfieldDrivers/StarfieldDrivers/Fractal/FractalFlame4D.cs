using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarfieldClient;
using System.Drawing;
using StarfieldUtils;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers
{
    class FractalFlame4D : IStarfieldDriver
    {
        #region Enums
        enum State
        {
            Hold,
            FadeIn,
            FadeOut,
        }

        enum Variants
        {
            Prime4D
        }
        #endregion

        #region Structs
        private struct ColorStruct
        {
            public int index;
            public Color color;
        }
        #endregion

        #region Private Members
        Random rand;
        Color primaryColor = Color.Red;
        Color secondaryColor = Color.Blue;
        State state = State.Hold;
        double[, , , ] colors;
        double[, , , ] alphas;
        Color[, , , ] toDraw;
        int step = 0;
        int numSteps = 5;
        int time = 0;
        int holdTime = 64;
        int subTime = 0;
        int subTimeSteps = 10;
        #endregion

        #region Public Properties
        public int HoldTime
        {
            get { return holdTime; }
            set { holdTime = value; }
        }

        public int NumSteps
        {
            get { return numSteps; }
            set { numSteps = value; }
        }

        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

        public Color SecondaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
        }

        public int SubTimeSteps
        {
            get { return subTimeSteps; }
            set { subTimeSteps = value; }
        }
        #endregion

        #region Constructors
        public FractalFlame4D()
        {
        }
        #endregion

        #region IStarfieldDriver Implemention
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            if(subTime == 0)
            {
                subTime = (subTime + 1) % subTimeSteps;
            }
            else
            {
                subTime = (subTime + 1) % subTimeSteps;
                return;
            }
            Console.WriteLine("time: " + time);
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        if(this.state == State.FadeIn)
                        {
                            Color baseColor = toDraw[x, y, z, (ulong)time];
                            Starfield.SetColor((int)x, (int)y, (int)z, Color.FromArgb((step * baseColor.A)/numSteps, (step * baseColor.R)/numSteps, (step * baseColor.G)/numSteps, (step * baseColor.B)/numSteps));
                            
                            if(step == numSteps)
                            {
                                step = 0;

                                GenerateFlame(Starfield);
                                state = State.Hold;
                            }
                        }
                        if(this.state == State.FadeOut)
                        {
                            Color baseColor = Starfield.GetColor((int)x, (int)y, (int)z);
                            Starfield.SetColor((int)x, (int)y, (int)z, Color.FromArgb((int)(.9 * baseColor.A), (int)(.9 * baseColor.R), (int)(.9 * baseColor.G), (int)(.9 * baseColor.B)));

                            if(step == numSteps)
                            {
                                step = 0;
                                state = State.FadeIn;
                            }
                        }
                        if(this.state == State.Hold && time < holdTime - 1)
                        {
                            Color current = toDraw[x, y, z, (ulong)time];
                            Color next = toDraw[x, y, z, (ulong)time+1];
                            int redDiff = current.R - next.R;
                            int greenDiff = current.G - next.G;
                            int blueDiff = current.B - next.B;

                            redDiff *= subTime;
                            redDiff /= subTimeSteps;
                            greenDiff *= subTime;
                            greenDiff /= subTimeSteps;
                            blueDiff *= subTime;
                            blueDiff /= subTimeSteps;

                            Color draw = ColorUtils.GetGradientColor(current, next, ((float)subTime) / subTimeSteps, true);

                            Starfield.SetColor((int)x, (int)y, (int)z, draw);
                        }
                        else if(this.state == State.Hold)
                        {
                            Color current = toDraw[x, y, z, (ulong)time];
                            Starfield.SetColor((int)x, (int)y, (int)z, current);
                        }
                    }
                }
            }
            if(state != State.Hold)
            {
                step++;
            }

            time = (time + 1) % holdTime;
            if (time == 0)
            {
                state = State.FadeOut;
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            this.state = State.Hold;
            rand = new Random();
            GenerateFlame(Starfield);
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "4D Fractal Flame";
        }
        #endregion

        #region Private Members
        private void GenerateFlame(StarfieldModel Starfield)
        {
            AffineCoefs4d[] coefs_arr = new AffineCoefs4d[3];
            for(int i = 0; i < coefs_arr.Length; i++)
            {
                coefs_arr[i] = GenerateRandomAffine();
            }

            colors = new double[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z, holdTime];
            alphas = new double[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z, holdTime];
            toDraw = new Color[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z, holdTime];

            int xPos = 0;
            int yPos = 0;
            int zPos = 0;
            int tPos = 0;

            int count = -20;

            double xHigh = 1;
            double xLow = -1;
            double yHigh = 1;
            double yLow = -1;
            double zHigh = 1;
            double zLow = -1;
            double tHigh = 1;
            double tLow = -1;

            double xScale = Starfield.NUM_X / (xHigh - xLow);
            double yScale = Starfield.NUM_Y / (yHigh - yLow);
            double zScale = Starfield.NUM_Z / (zHigh - zLow);
            double tScale = holdTime / (tHigh - tLow);

            double xOffset = Starfield.NUM_X / 2;
            double yOffset = Starfield.NUM_Y / 2;
            double zOffset = Starfield.NUM_Z / 2;
            double tOffset = holdTime / 2;

            double color = rand.NextDouble();
            double alpha = 0;
            double maxAlpha = 0;

            double pointX = rand.NextDouble() * (xHigh - xLow) - (xHigh - xLow) / 2;
            double pointY = rand.NextDouble() * (yHigh - yLow) - (yHigh - yLow) / 2;
            double pointZ = rand.NextDouble() * (zHigh - zLow) - (zHigh - zLow) / 2;
            double pointT = rand.NextDouble() * (tHigh - tLow) - (tHigh - tLow) / 2;

            double IfsPointX = 0;
            double IfsPointY = 0;
            double IfsPointZ = 0;
            double IfsPointT = 0;

            while (count < (int)(Starfield.NUM_X * Starfield.NUM_Y * Starfield.NUM_Z * (ulong)holdTime))
            {
                count++;

                AffineCoefs4d coefs = coefs_arr[rand.Next(0, coefs_arr.Length)];

                IfsPointX = pointX * coefs.a + pointY * coefs.b + pointZ * coefs.c + pointT * coefs.d + coefs.e;
                IfsPointY = pointX * coefs.f + pointY * coefs.g + pointZ * coefs.h + pointT * coefs.i + coefs.j;
                IfsPointZ = pointX * coefs.k + pointY * coefs.l + pointZ * coefs.m + pointT * coefs.n + coefs.o;
                IfsPointT = pointX * coefs.p + pointY * coefs.q + pointZ * coefs.r + pointT * coefs.s + coefs.t;

                ApplyVariant(0, IfsPointX, IfsPointY, IfsPointZ, IfsPointT, ref pointX, ref pointY, ref pointZ, ref pointT);

                color = (color + 1 / 2);

                if(pointX > xLow && pointX < xHigh &&
                   pointY > yLow && pointY < yHigh &&
                   pointZ > zLow && pointZ < zHigh &&
                   pointT > tLow && pointT < tHigh &&
                   count > 20)
                {
                    xPos = (int)(pointX * xScale + xOffset);
                    yPos = (int)(pointY * yScale + yOffset);
                    zPos = (int)(pointZ * zScale + zOffset);
                    tPos = (int)(pointT * tScale + tOffset);

                    alpha = ++alphas[xPos, yPos, zPos, tPos];
                    colors[xPos, yPos, zPos, tPos] += color;

                    if(alpha >= maxAlpha)
                    {
                        maxAlpha = alpha;
                    }
                }
            }

            double alphaScaleColor = 0;
            double alphaLog = 0;
            double logDensity = 0;
            double gammasSum = 0;
            double gamma = 0;
            double alphaGamma;
            double mappingScale;

            byte[, ,] imageAlpha = new byte[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            byte[, ,] imageRed = new byte[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            byte[, ,] imageGreen = new byte[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            byte[, ,] imageBlue = new byte[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];

            int pixelColor = 0;
            byte fixedBrightness = 0;
            Color[] palette = GenerateRandomPalette(256);

            gamma = (1 - .2) * (1 / 5);

            int tempPixel = 0;

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        for(ulong t = 0; t < (ulong)holdTime; t++)
                        {
                            if(alphas[x,y,z,t] != 0 && maxAlpha != 1)
                            {
                                alphaLog = Math.Log(alphas[x, y, z, t], maxAlpha);
                                alphaScaleColor = alphaLog / alphas[x, y, z, t];
                            }
                            else
                            {
                                alphaLog = 0;
                                alphaScaleColor = 0;
                            }

                            logDensity = alphaScaleColor * alphas[x, y, z, t];

                            if(alphaLog == 0)
                            {
                                alphaGamma = 0;
                            }
                            else
                            {
                                alphaGamma = .2 / (alphaLog * 2.2);
                            }

                            gammasSum = gamma + alphaGamma;

                            if(gammasSum == 0)
                            {
                                mappingScale = 0;
                            }
                            else
                            {
                                mappingScale = Math.Pow(alphaLog, gammasSum);
                            }

                            pixelColor = (int)(logDensity * (255));

                            fixedBrightness = (byte)(130 * logDensity);
                        
                            tempPixel = (int)(palette[pixelColor].R * mappingScale) + fixedBrightness;
                            tempPixel = Math.Min(tempPixel, 255);
                            tempPixel = Math.Max(tempPixel, 0);
                            imageRed[x, y, z] = (byte)tempPixel;

                            tempPixel = (int)(palette[pixelColor].G * mappingScale) + fixedBrightness;
                            tempPixel = Math.Min(tempPixel, 255);
                            tempPixel = Math.Max(tempPixel, 0);
                            imageGreen[x, y, z] = (byte)tempPixel;

                            tempPixel = (int)(palette[pixelColor].B * mappingScale) + fixedBrightness;
                            tempPixel = Math.Min(tempPixel, 255);
                            tempPixel = Math.Max(tempPixel, 0);
                            imageBlue[x, y, z] = (byte)tempPixel;

                            imageAlpha[x, y, z] = (byte)(255 * alphaLog);

                            toDraw[x, y, z, t] = Color.FromArgb(imageAlpha[x, y, z], imageRed[x, y, z], imageGreen[x, y, z], imageBlue[x, y, z]);
                        }
                    }
                }
            }

            state = State.FadeOut;
        }

        private void ApplyVariant(int index, double x, double y, double z, double t, ref double xOut, ref double yOut, ref double zOut, ref double tOut)
        {
            double rSquared = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(t, 2);
            switch(index)
            {
                case (int)Variants.Prime4D:
                    xOut = x * Math.Sin(rSquared) - y * Math.Cos(rSquared);
                    yOut = x * Math.Cos(rSquared) + y * Math.Sin(rSquared);
                    zOut = z * Math.Sin(rSquared) - t * Math.Cos(rSquared);
                    tOut = Math.Cos(t);//z * Math.Sin(rSquared) + t * Math.Cos(rSquared);
                    break;
            }
        }

        private AffineCoefs4d GenerateRandomAffine()
        {
            AffineCoefs4d coefs = new AffineCoefs4d();

            coefs.a = rand.NextDouble() * 2 - 1;
            coefs.b = rand.NextDouble() * 2 - 1;
            coefs.c = rand.NextDouble() * 2 - 1;
            coefs.d = rand.NextDouble() * 2 - 1;
            coefs.e = rand.NextDouble() * 2 - 1;
            coefs.f = rand.NextDouble() * 2 - 1;
            coefs.g = rand.NextDouble() * 2 - 1;
            coefs.h = rand.NextDouble() * 2 - 1;
            coefs.i = rand.NextDouble() * 2 - 1;
            coefs.j = rand.NextDouble() * 2 - 1;
            coefs.k = rand.NextDouble() * 2 - 1;
            coefs.l = rand.NextDouble() * 2 - 1;
            coefs.m = rand.NextDouble() * 2 - 1;
            coefs.n = rand.NextDouble() * 2 - 1;
            coefs.o = rand.NextDouble() * 2 - 1;
            coefs.p = rand.NextDouble() * 2 - 1;
            coefs.q = rand.NextDouble() * 2 - 1;
            coefs.r = rand.NextDouble() * 2 - 1;
            coefs.s = rand.NextDouble() * 2 - 1;
            coefs.t = rand.NextDouble() * 2 - 1;

            return coefs;
        }

        private Color[] GenerateRandomPalette(int NumColors)
        {
            int smooth = 10;
            int nodeQuantity = 2;

            Color[] palette = new Color[NumColors];

            List<ColorStruct> nodes = new List<ColorStruct>();
            ColorStruct node = new ColorStruct();

            for (int i = 0; i < nodeQuantity; i++)
            {
                node.index = (int)(rand.NextDouble() * NumColors);
                node.color = Color.FromArgb((int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255), (int)(rand.NextDouble() * 255));
                nodes.Add(node);
            }

            nodes.Sort(delegate(ColorStruct s1, ColorStruct s2)
            {
                return s1.index.CompareTo(s2.index);
            });

            node = nodes[0];
            palette[node.index] = Color.FromArgb(node.color.R, node.color.G, node.color.B);
            int lastIndex = node.index;

            for (int i = 1; i < nodes.Count; i++)
            {
                node = nodes[i];
                palette[node.index] = Color.FromArgb(node.color.R, node.color.G, node.color.B);

                Blend(lastIndex, node.index, palette);

                lastIndex = node.index;
            }

            node = nodes[0];
            palette[0] = Color.FromArgb(node.color.R, node.color.G, node.color.B);

            node = nodes[nodes.Count - 1];
            palette[255] = Color.FromArgb(node.color.R, node.color.G, node.color.B);

            Blend(0, nodes[0].index, palette);
            Blend(nodes[nodes.Count - 1].index, 255, palette);
            Smooth(smooth, palette);

            return palette;
        }

        private void Blend(int index1, int index2, Color[] palette)
        {
            double red, green, blue;
            double redStep, greenStep, blueStep, distance;

            if (index1 < 0 || index1 > 255)
                throw new ArgumentException("First index number is out of palette size range.");
            if (index2 < 0 || index2 > 255)
                throw new ArgumentException("Second index number is out of palette size range.");

            Color
                color1 = palette[index1],
                color2 = palette[index2];

            red = color1.R;
            green = color1.G;
            blue = color1.B;

            if (index2 > index1 + 1)
            {
                distance = index2 - index1 + 1;

                redStep = (color2.R - red) / distance;
                greenStep = (color2.G - green) / distance;
                blueStep = (color2.B - blue) / distance;

                for (int i = index1 + 1; i < index2; i++)
                {
                    red += redStep;
                    green += greenStep;
                    blue += blueStep;
                    palette[i] = Color.FromArgb((int)Math.Round(red), (int)Math.Round(green), (int)Math.Round(blue));
                }
            }
            else if (index2 < index1 - 1)
            {
                distance = index2 + 1 + 256 - index1;

                redStep = (color2.R - red) / distance;
                greenStep = (color2.G - green) / distance;
                blueStep = (color2.B - blue) / distance;

                for (int i = index1 + 1; i < 256; i++)
                {
                    red += redStep;
                    green += greenStep;
                    blue += blueStep;
                    palette[i] = Color.FromArgb((int)Math.Round(red), (int)Math.Round(green), (int)Math.Round(blue));
                }

                for (int i = 0; i < index2; i++)
                {
                    red += redStep;
                    green += greenStep;
                    blue += blueStep;
                    palette[i] = Color.FromArgb((int)Math.Round(red), (int)Math.Round(green), (int)Math.Round(blue));
                }
            }
        }

        private void Smooth(int softness, Color[] palette)
        {
            int red, green, blue;
            for (int j = 0; j < softness; j++)
            {
                red = (int)((palette[255].R + palette[0].R + palette[1].R) / 3);
                green = (int)((palette[255].G + palette[0].G + palette[1].G) / 3);
                blue = (int)((palette[255].B + palette[0].B + palette[1].B) / 3);
                palette[0] = Color.FromArgb(red, green, blue);

                for (int i = 1; i < 255; i++)
                {
                    red = (int)((palette[i - 1].R + palette[i].R + palette[i + 1].R) / 3);
                    green = (int)((palette[i - 1].G + palette[i].G + palette[i + 1].G) / 3);
                    blue = (int)((palette[i - 1].B + palette[i].B + palette[i + 1].B) / 3);
                    palette[i] = Color.FromArgb(red, green, blue);
                }

                red = (int)((palette[254].R + palette[255].R + palette[0].R) / 3);
                green = (int)((palette[254].G + palette[255].G + palette[0].G) / 3);
                blue = (int)((palette[254].B + palette[255].B + palette[0].B) / 3);
                palette[255] = Color.FromArgb(red, green, blue);

            }
        }
        #endregion
    }
}
