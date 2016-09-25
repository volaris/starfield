using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.SoundUtils;
using StarfieldUtils.MathUtils;

namespace StarfieldDrivers.Drivers
{
    class Globe
    {
        public float OuterRadius;
        public float InnerRadius;
        public Color color;
        public Vec3D location;
    }

    [DriverType(DriverTypes.SoundResponsive)]
    class SoundResponsiveGlobes : IStarfieldDriver
    {
        #region Private Members
        Color current = Color.Black;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        BaseSoundProcessor soundProcessor;
        float maxDistance;
        ConcurrentQueue<Globe> globes = new ConcurrentQueue<Globe>();
        private bool onsetOnly = true;
        #endregion

        #region Public Properties
        public bool OnsetOnly
        {
            get
            {
                return onsetOnly;
            }
            set
            {
                onsetOnly = value;
            }
        }
        #endregion

        #region Constructors
        public SoundResponsiveGlobes()
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
        void soundProcessor_OnArtifactDetected(Artifact artifact)
        {
            if (!onsetOnly || artifact.Type == ArtifactDetectionAlgorithm.Onset)
            {
                Random rand = new Random();
                Globe globe = new Globe();
                globe.OuterRadius = 2.0f;
                globe.InnerRadius = -22f;
                globe.color = rainbow7[rand.Next(rainbow7.Length - 1)];
                globes.Enqueue(globe);
            }
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            float centerX = ((Starfield.NumX - 1) * Starfield.XStep) / 2;
            float centerY = ((Starfield.NumY - 1) * Starfield.YStep) / 2;
            float centerZ = ((Starfield.NumZ - 1) * Starfield.ZStep) / 2;

            maxDistance = (float)Math.Sqrt(Math.Pow(0 - centerX, 2) + Math.Pow(0 - centerY, 2) + Math.Pow(0 - centerZ, 2));

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        float xPos = x * Starfield.XStep;
                        float yPos = y * Starfield.YStep;
                        float zPos = z * Starfield.ZStep;
                        Color toDraw = Color.Black;

                        float distanceToCenter = (float)Math.Sqrt(Math.Pow(xPos - centerX, 2) + Math.Pow(yPos - centerY, 2) + Math.Pow(zPos - centerZ, 2));

                        try
                        {
                            foreach (Globe globe in globes)
                            {
                                if(globe == null)
                                {
                                    continue;
                                }
                                if (distanceToCenter < globe.OuterRadius && distanceToCenter > globe.InnerRadius)
                                {
                                    toDraw = globe.color;
                                }
                            }
                        }
                        catch
                        {
                            // TODO: we shouldn't need this now that we have the render lock
                            return;
                        }

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }

            foreach(Globe globe in globes)
            {
                //TODO: scale for starfield size?
                globe.OuterRadius += 1f;
                globe.InnerRadius += 1f;
            }

            while(globes.Count > 0)
            {
                Globe globe;
                while (!globes.TryPeek(out globe)) { };
                if (globe.InnerRadius > maxDistance)
                {
                    while (!globes.TryDequeue(out globe)) { };
                }
                else
                {
                    break;
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = SoundProcessor.GetSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Artifact Triggered Globes";
        }
        #endregion
    }
}
