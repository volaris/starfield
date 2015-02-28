using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StarfieldClient;
using System.Drawing;
using AlgorithmDemo.Utils;
using AlgorithmDemo.MathUtils;

namespace AlgorithmDemo.Drivers
{
    class FractalFlame : IStarfieldDriver
    {
        enum State
        {
            Sleep,
            FadeIn,
            FadeOut
        }

        enum Variants
        {
            Prime3D
        }

        Random rand;
        Color PrimaryColor = Color.Red;
        Color SecondaryColor = Color.Blue;
        State state = State.Sleep;
        double[, ,] colors;
        double[, ,] alphas;
        Color[, ,] toDraw;
        int step = 0;
        int numSteps = 5;
        System.Timers.Timer newFractal = new System.Timers.Timer(5000);
        StarfieldModel Starfield;

        public FractalFlame()
        {
            newFractal.Elapsed += newFractal_Elapsed;
        }

        void newFractal_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GenerateFlame(Starfield);
        }

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        if(this.state == State.Sleep)
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, toDraw[x, y, z]);
                        }
                        if(this.state == State.FadeIn)
                        {
                            Color baseColor = toDraw[x, y, z];
                            Starfield.SetColor((int)x, (int)y, (int)z, Color.FromArgb((step * baseColor.A)/numSteps, (step * baseColor.R)/numSteps, (step * baseColor.G)/numSteps, (step * baseColor.B)/numSteps));
                            
                            if(step == numSteps)
                            {
                                step = 0;
                                state = State.Sleep;
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
                    }
                }
            }
            if(state != State.Sleep)
            {
                step++;
            }
        }

        Panel IStarfieldDriver.GetConfigPanel()
        {
            throw new NotImplementedException();
        }

        void IStarfieldDriver.ApplyConfig()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Fractal Flame";
        }


        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            this.Starfield = Starfield;
            this.state = State.Sleep;
            rand = new Random();
            GenerateFlame(Starfield);
            newFractal.Start();
        }

        void IStarfieldDriver.Stop()
        {
            newFractal.Stop();
        }

        void GenerateFlame(StarfieldModel Starfield)
        {
            AffineCoefs3d[] coefs_arr = new AffineCoefs3d[3];
            for(int i = 0; i < coefs_arr.Length; i++)
            {
                coefs_arr[i] = GenerateRandomAffine();
            }

            colors = new double[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            alphas = new double[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            toDraw = new Color[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];

            int xPos = 0;
            int yPos = 0;
            int zPos = 0;

            int count = -20;

            double xHigh = 1;
            double xLow = -1;
            double yHigh = 1;
            double yLow = -1;
            double zHigh = 1;
            double zLow = -1;

            double xScale = Starfield.NUM_X / (xHigh - xLow);
            double yScale = Starfield.NUM_Y / (yHigh - yLow);
            double zScale = Starfield.NUM_Z / (zHigh - zLow);

            double xOffset = Starfield.NUM_X / 2;
            double yOffset = Starfield.NUM_Y / 2;
            double zOffset = Starfield.NUM_Z / 2;

            double color = rand.NextDouble();
            double alpha = 0;
            double maxAlpha = 0;

            double pointX = rand.NextDouble() * (xHigh - xLow) - (xHigh - xLow) / 2;
            double pointY = rand.NextDouble() * (yHigh - yLow) - (yHigh - yLow) / 2;
            double pointZ = rand.NextDouble() * (zHigh - zLow) - (zHigh - zLow) / 2;

            double IfsPointX = 0;
            double IfsPointY = 0;
            double IfsPointZ = 0;

            while(count < 6000)//(StarfieldModel.NUM_X * StarfieldModel.NUM_Y * StarfieldModel.NUM_Z))
            {
                count++;

                AffineCoefs3d coefs = coefs_arr[rand.Next(0, coefs_arr.Length)];

                IfsPointX = pointX * coefs.a + pointY * coefs.b + pointZ * coefs.c + coefs.d;
                IfsPointY = pointX * coefs.e + pointY * coefs.f + pointZ * coefs.g + coefs.h;
                IfsPointZ = pointX * coefs.i + pointY * coefs.j + pointZ * coefs.k + coefs.l;

                ApplyVariant(0, IfsPointX, IfsPointY, IfsPointZ, ref pointX, ref pointY, ref pointZ);

                color = (color + 1 / 2);

                if(pointX > xLow && pointX < xHigh &&
                   pointY > yLow && pointY < yHigh &&
                   pointZ > zLow && pointZ < zHigh &&
                   count > 20)
                {
                    xPos = (int)(pointX * xScale + xOffset);
                    yPos = (int)(pointY * yScale + yOffset);
                    zPos = (int)(pointZ * zScale + zOffset);

                    alpha = ++alphas[xPos, yPos, zPos];
                    colors[xPos, yPos, zPos] += color;

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
                        if(alphas[x,y,z] != 0 && maxAlpha != 1)
                        {
                            alphaLog = Math.Log(alphas[x, y, z], maxAlpha);
                            alphaScaleColor = alphaLog / alphas[x, y, z];
                        }
                        else
                        {
                            alphaLog = 0;
                            alphaScaleColor = 0;
                        }

                        logDensity = alphaScaleColor * alphas[x, y, z];

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

                        toDraw[x, y, z] = Color.FromArgb(imageAlpha[x, y, z], imageRed[x, y, z], imageGreen[x, y, z], imageBlue[x, y, z]);
                    }
                }
            }

            state = State.FadeOut;
        }

        void ApplyVariant(int index, double x, double y, double z, ref double xOut, ref double yOut, ref double zOut)
        {
            double rSquared = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2);
            switch(index)
            {
                case (int)Variants.Prime3D:
                    xOut = x * Math.Sin(rSquared) - y * Math.Cos(rSquared);
                    yOut = x * Math.Cos(rSquared) + y * Math.Sin(rSquared);
                    zOut = z;
                    break;
            }
        }

        AffineCoefs3d GenerateRandomAffine()
        {
            AffineCoefs3d coefs = new AffineCoefs3d();

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

            return coefs;
        }

        private struct ColorStruct
        {
            public int index;
            public Color color;
        }

        Color[] GenerateRandomPalette(int NumColors)
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

        public void Blend(int index1, int index2, Color[] palette)
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

        public void Smooth(int softness, Color[] palette)
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
    }
}
