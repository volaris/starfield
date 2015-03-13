using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;
using StarfieldUtils.SoundUtils;

namespace AlgorithmDemo.Drivers
{
    class SoundResponsiveGlobesRandomFix : IStarfieldDriver
    {
        #region Private Members
        Color current = Color.Black;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        CSCoreLoopbackSoundProcessor soundProcessor;
        float maxDistance;
        Random rand = new Random();
        Queue<Globe> globes = new Queue<Globe>();
        #endregion

        #region Constructors
        public SoundResponsiveGlobesRandomFix()
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
            Globe globe = new Globe();
            globe.OuterRadius = 2.0f;
            globe.InnerRadius = -22f;
            globe.color = rainbow7[rand.Next(rainbow7.Length - 1)];
            globes.Enqueue(globe);
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            float centerX = (Starfield.NumX * 4.0f) / 2;
            float centerY = (Starfield.NumY * 4.0f) / 2;
            float centerZ = (Starfield.NumZ * 4.0f) / 2;

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
                        Color toDraw = Color.Black;

                        float distanceToCenter = (float)Math.Sqrt(Math.Pow(xPos - centerX, 2) + Math.Pow(yPos - centerY, 2) + Math.Pow(zPos - centerZ, 2));

                        try
                        {
                            foreach (Globe globe in globes)
                            {
                                if (distanceToCenter < globe.OuterRadius && distanceToCenter > globe.InnerRadius)
                                {
                                    toDraw = globe.color;
                                }
                            }
                        }
                        catch
                        {
                            // TODO: this shouldn't be needed now that we have the render lock
                            return;
                        }

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }

            foreach(Globe globe in globes)
            {
                globe.OuterRadius += 2f;
                globe.InnerRadius += 2f;
            }

            while(globes.Count > 0)
            {
                Globe globe = globes.Peek();
                if (globe.InnerRadius > maxDistance)
                {
                    globes.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = new CSCoreLoopbackSoundProcessor();
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
            return "Artifact Triggered Globes Random Fix";
        }
        #endregion
    }
}
