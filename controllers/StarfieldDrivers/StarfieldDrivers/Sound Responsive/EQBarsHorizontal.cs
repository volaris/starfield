using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.SoundUtils;
using StarfieldUtils.ColorUtils;

namespace AlgorithmDemo.Drivers
{
    class EQBarisHorizontal : IStarfieldDriver
    {
        #region Private Members
        int current;
        int goal;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        CSCoreLoopbackSoundProcessor soundProcessor;
        float maxDistance;
        bool transitioning = false;
        float gradientPercent;
        float gradientStep = .01f;
        private bool fade = true;
        private float rate = .8f;
        Globe vuGlobe = new Globe();
        #endregion

        #region Public Properties
        public bool Fade
        {
            get { return fade; }
            set { fade = value; }
        }

        public float GradientStep
        {
            get { return gradientStep; }
            set { gradientStep = value; }
        }

        public float Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        #endregion

        #region Constructors
        public EQBarisHorizontal()
        {
            rainbow10[0] = rainbow7[0] = Color.FromArgb(0xFF, 0, 0);
            rainbow10[1] = rainbow7[1] = Color.FromArgb(0xFF, 0xA5, 0);
            rainbow10[2] = rainbow7[2] = Color.FromArgb(0xFF, 0xFF, 0);
            rainbow10[3] = rainbow7[3] = Color.FromArgb(0, 0x80, 0);
            rainbow10[4] = Color.FromArgb(0, 0xFF, 0);
            rainbow10[5] = Color.FromArgb(0, 0xA5, 0x80);
            rainbow10[6] = rainbow7[4] = Color.FromArgb(0, 0, 0xFF);
            rainbow10[7] = rainbow7[5] = Color.FromArgb(0x4B, 0, 0x82);
            rainbow10[8] = rainbow7[6] = Color.FromArgb(0xFF, 0, 0xFF);
            rainbow10[9] = Color.FromArgb(0xEE, 0x82, 0xEE);
        }
        #endregion

        #region Event Handlers
        void soundProcessor_OnFrameUpdate(Frame frame)
        {
            byte vu = Math.Max(frame.VU[0], frame.VU[1]);
            vuGlobe.OuterRadius = 4.0f + ((maxDistance - 4.0f) * (vu / 255f));
        }

        void soundProcessor_OnArtifactDetected(Artifact artifact)
        {
            if (!transitioning)
            {
                gradientPercent = 0f;
                goal = (current + 1) % rainbow7.Length;
                transitioning = true;
            }
        }
        #endregion

        #region IStarfieldDrive Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            float centerX = ((Starfield.NumX - 1) * 4.0f) / 2;
            float centerY = ((Starfield.NumY - 1) * 4.0f) / 2;
            float centerZ = ((Starfield.NumZ - 1) * 4.0f) / 2;

            maxDistance = (float)Math.Sqrt(Math.Pow(0 - centerX, 2) + Math.Pow(0 - centerY, 2) + Math.Pow(0 - centerZ, 2));

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        float xPos = x * 4.0f;
                        float yPos = y * 4.0f;
                        float zPos = z * 4.0f;
                        Color atPos = Starfield.GetColor((int)x, (int)y, (int)z);
                        Color toDraw = fade ? Color.FromArgb((int)(rate * atPos.R), (int)(rate * atPos.G), (int)(rate * atPos.B)) : Color.Black;

                        float distanceToCenter = (float)Math.Sqrt(Math.Pow(xPos - centerX, 2) + Math.Pow(yPos - centerY, 2) + Math.Pow(zPos - centerZ, 2));

                        if (distanceToCenter < vuGlobe.OuterRadius && distanceToCenter > vuGlobe.InnerRadius)
                        {
                            if (!transitioning)
                            {
                                toDraw = rainbow7[current];
                            }
                            else
                            {
                                if(gradientPercent >= 1f)
                                {
                                    current = goal;
                                    transitioning = false;
                                }
                                toDraw = ColorUtils.GetGradientColor(rainbow7[current], rainbow7[goal], gradientPercent, true);
                                gradientPercent += gradientStep;
                            }
                        }

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = new CSCoreLoopbackSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
            soundProcessor.OnFrameUpdate += soundProcessor_OnFrameUpdate;
            current = 0;
            goal = 0;
            transitioning = false;
            vuGlobe.InnerRadius = 0;
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "EQ Bars - Horizontal";
        }
        #endregion
    }
}
