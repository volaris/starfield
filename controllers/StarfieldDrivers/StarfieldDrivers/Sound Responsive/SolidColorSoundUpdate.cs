using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.SoundUtils;

namespace StarfieldDrivers.Drivers
{
    [DriverType(DriverTypes.Experimental)]
    class SolidColorSoundUpdate : IStarfieldDriver
    {
        #region Private Members
        Color current = Color.Black;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        BaseSoundProcessor soundProcessor;
        #endregion

        #region Constructors
        public SolidColorSoundUpdate()
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
            Random rand = new Random();
            current = rainbow7[rand.Next(rainbow7.Length - 1)];
        }
        #endregion

        #region IStarfieldDriver Implementation
        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, current);
                    }
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
            return "Solid Color Sound Responsive - Artifacts";
        }
        #endregion
    }
}
