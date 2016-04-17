using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.SoundUtils;
using StarfieldUtils.ColorUtils;
using StarfieldUtils.MathUtils;

namespace AlgorithmDemo.Drivers
{
    [DriverType(DriverTypes.SoundResponsive)]
    class EdgeGlobeEQ : IStarfieldDriver
    {
        #region Private Members
        int current;
        int goal;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        BaseSoundProcessor soundProcessor;
        float maxDistance;
        bool transitioning = false;
        float gradientPercent;
        float gradientStep = .01f;
        private bool fade = true;
        private float rate = .8f;
        List<Globe> globes = new List<Globe>();
        List<Vec3D> positions = new List<Vec3D>();
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
        public EdgeGlobeEQ()
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
            globes[0].OuterRadius = 4.0f + ((maxDistance - 4.0f) * (Math.Max(frame.EQ[0][0], frame.EQ[1][0]) + Math.Max(frame.EQ[0][1], frame.EQ[1][1])) / 2);
            globes[1].OuterRadius = 4.0f + ((maxDistance - 4.0f) * (Math.Max(frame.EQ[0][2], frame.EQ[1][2]) + Math.Max(frame.EQ[0][3], frame.EQ[1][3])) / 2);
            globes[2].OuterRadius = 4.0f + ((maxDistance - 4.0f) * Math.Max(frame.EQ[0][4], frame.EQ[1][4]));
            globes[3].OuterRadius = 4.0f + ((maxDistance - 4.0f) * Math.Max(frame.EQ[0][5], frame.EQ[1][5]));
            globes[4].OuterRadius = 4.0f + ((maxDistance - 4.0f) * (Math.Max(frame.EQ[0][6], frame.EQ[1][6]) + Math.Max(frame.EQ[0][7], frame.EQ[1][7])) / 2);
            globes[5].OuterRadius = 4.0f + ((maxDistance - 4.0f) * (Math.Max(frame.EQ[0][8], frame.EQ[1][8]) + Math.Max(frame.EQ[0][9], frame.EQ[1][9])) / 2);
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

            maxDistance = (float)Math.Sqrt(Math.Pow(0 - globes[0].location.X, 2) + Math.Pow(0 - globes[0].location.Y, 2) + Math.Pow(0 - globes[0].location.Z, 2));

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        float xPos = x * Starfield.XStep;
                        float yPos = y * Starfield.YStep;
                        float zPos = z * Starfield.ZStep;
                        Color atPos = Starfield.GetColor((int)x, (int)y, (int)z);
                        Color toDraw = fade ? Color.FromArgb((int)(rate * atPos.R), (int)(rate * atPos.G), (int)(rate * atPos.B)) : Color.Black;

                        foreach (Globe globe in globes)
                        {
                            float distanceToCenter = (float)Math.Sqrt(Math.Pow(xPos - globe.location.X, 2) + Math.Pow(yPos - globe.location.Y, 2) + Math.Pow(zPos - globe.location.Z, 2));

                            if (distanceToCenter < globe.OuterRadius && distanceToCenter > globe.InnerRadius)
                            {
                                if (!transitioning)
                                {
                                    toDraw = rainbow7[(current + globes.IndexOf(globe)) % 7];
                                }
                                else
                                {
                                    if (gradientPercent >= 1f)
                                    {
                                        current = goal;
                                        transitioning = false;
                                    }
                                    toDraw = ColorUtils.GetGradientColor(rainbow7[(current + globes.IndexOf(globe)) % 7], rainbow7[(goal + globes.IndexOf(globe)) % 7], gradientPercent, true);
                                    gradientPercent += gradientStep;
                                }
                            }
                        }

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = SoundProcessor.GetSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
            soundProcessor.OnFrameUpdate += soundProcessor_OnFrameUpdate;
            current = 0;
            goal = 0;
            transitioning = false;

            float farX = ((Starfield.NumX - 1) * Starfield.XStep);
            float farY = ((Starfield.NumY - 1) * Starfield.YStep);
            float farZ = ((Starfield.NumZ - 1) * Starfield.ZStep);
            float centerX = farX / 2;
            float centerY = farY / 2;
            float centerZ = farZ / 2;

            positions.Add(new Vec3D(0, centerY, centerZ));
            positions.Add(new Vec3D(farX, centerY, centerZ));
            positions.Add(new Vec3D(centerX, centerY, 0));
            positions.Add(new Vec3D(centerX, centerY, farZ));
            positions.Add(new Vec3D(centerX, 0, centerZ));
            positions.Add(new Vec3D(centerX, farY, centerZ));

            globes.Clear();

            for (int i = 0; i < 6; i++)
            {
                Globe globe = new Globe();
                globe.InnerRadius = 0.0f;
                globe.OuterRadius = 0.0f;
                globes.Add(globe);
            }

            for(int i = 0; i < 6; i++)
            {
                globes[i].location = positions[i];
            }
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Edge Globe EQ";
        }
        #endregion
    }
}
