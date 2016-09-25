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

namespace StarfieldDrivers.Drivers
{
    [DriverType(DriverTypes.SoundResponsive)]
    class MultiGlobeVU : IStarfieldDriver
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
        int numGlobes = 4;
        float outerRadius = 0.0f;
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

        public int NumGlobes
        {
            get { return numGlobes; }
            set { numGlobes = Math.Max(1,value); }
        }

        public float Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        #endregion

        #region Constructors
        public MultiGlobeVU()
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
            outerRadius = 4.0f + ((maxDistance - 4.0f) * (vu / 255f));
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
            if(globes.Count > numGlobes)
            {
                for(int i = globes.Count - 1; i >= numGlobes; i--)
                {
                    globes.RemoveAt(i);
                }
            }
            else if(globes.Count < numGlobes)
            {
                for(int i = 0; i < numGlobes - globes.Count; i++)
                {
                    Globe globe = new Globe();
                    globe.InnerRadius = 0.0f;
                    globes.Add(globe);
                }
            }

            int numColumns = (int)Math.Sqrt(numGlobes);
            int numRows = (int)Math.Ceiling((float)numGlobes / (float)numColumns);

            int globeIndex = 0;

            for(int i = 0; i < numColumns; i++)
            {
                if(globeIndex >= globes.Count)
                {
                    break;
                }
                for(int j = 0; j < numRows; j++)
                {
                    if (globeIndex >= globes.Count)
                    {
                        break;
                    }

                    float columnWidth = ((Starfield.NumX - 1) * Starfield.XStep) / numColumns;
                    float rowWidth = ((Starfield.NumZ - 1) * Starfield.ZStep) / numRows;

                    float x = i * columnWidth + columnWidth / 2;
                    float y = ((Starfield.NumY - 1) * Starfield.YStep) / 2;
                    float z = j * rowWidth + rowWidth / 2;

                    globes[globeIndex].location = new Vec3D(x, y, z);

                    globeIndex++;
                }
            }

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

                        foreach(Globe globe in globes)
                        {
                            float distanceToCenter = (float)Math.Sqrt(Math.Pow(xPos - globe.location.X, 2) + Math.Pow(yPos - globe.location.Y, 2) + Math.Pow(zPos - globe.location.Z, 2));

                            if (distanceToCenter < outerRadius && distanceToCenter > globe.InnerRadius)
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
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Multi Globe VU";
        }
        #endregion
    }
}
