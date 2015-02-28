﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    class SolidColorSoundUpdate : IStarfieldDriver
    {
        bool Rendering = false;
        Color current = Color.Black;
        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];
        SoundUtils.CSCoreLoopbackSoundProcessor soundProcessor;

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

        void IStarfieldDriver.Render(StarfieldModel Starfield)
        {
            if(!Rendering)
            {
                return;
            }

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, current);
                    }
                }
            }
        }

        System.Windows.Forms.Panel IStarfieldDriver.GetConfigPanel()
        {
            throw new NotImplementedException();
        }

        void IStarfieldDriver.ApplyConfig()
        {
            throw new NotImplementedException();
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            Rendering = true;
            soundProcessor = new SoundUtils.CSCoreLoopbackSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
            Rendering = false;
        }

        void soundProcessor_OnArtifactDetected(SoundUtils.Artifact artifact)
        {
            Random rand = new Random();
            current = rainbow7[rand.Next(rainbow7.Length - 1)];
        }

        public override string ToString()
        {
            return "Solid Color Sound Responsive - Artifacts";
        }
    }
}
