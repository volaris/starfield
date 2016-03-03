using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils.MathUtils;
using StarfieldUtils.ColorUtils;
using StarfieldUtils.SoundUtils;

namespace StarfieldDrivers
{
   [DriverType(DriverTypes.SoundResponsive)]
    class SoundResponsiveSimplexClouds : IStarfieldDriver
    {
        #region Private Members
        Color primaryColor = Color.Blue;
        Color secondaryColor = Color.Red;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        float spread = 0f;
        float minSpread = .2f;
        float maxSpread = .6f;
        bool smoothed = false;
        float smoothFactor = .05f;
        float threshold = .75f;
        bool fade = true;
        float fadeThreshold = .1f;
        private float rate = .8f;
        BaseSoundProcessor soundProcessor;
        #endregion

        #region Public Properties
        public bool CapAtMax
        {
            get { return capAtMax; }
            set { capAtMax = value; }
        }

        public int NumOctaves
        {
            get { return numOctaves; }
            set { numOctaves = value; }
        }

        public float Persistance
        {
            get { return persistance; }
            set { persistance = value; }
        }

        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }

        public float Spread
        {
            get { return spread; }
            set { spread = value; }
        }

        public float MinSpread
        {
            get { return minSpread; }
            set { minSpread = value; }
        }

        public float MaxSpread
        {
            get { return maxSpread; }
            set { maxSpread = value; }
        }

        public float SmoothFactor
        {
            get { return smoothFactor; }
            set { smoothFactor = value; }
        }

       public bool Smoothed
        {
            get { return smoothed; }
            set { smoothed = value; }
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

        public bool Fade
        {
            get { return fade; }
            set { fade = value; }
        }

        public float FadeThreshold
        {
            get { return fadeThreshold; }
            set { fadeThreshold = value; }
        }

        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        public float Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        #endregion

        #region Constructors
        public SoundResponsiveSimplexClouds()
        {
        }
        #endregion

        #region Event Handlers
        void soundProcessor_OnFrameUpdate(Frame frame)
        {
            byte vu = Math.Max(frame.VU[0], frame.VU[1]);
            float val =  vu / 128f;// 255f;
            if (!Smoothed)
            {
                spread = val;
            }
            else
            {
                float diff = Math.Abs(val - spread);
                if(val > spread)
                {
                    if (diff > SmoothFactor)
                    {
                        spread += SmoothFactor;
                    }
                }
                else if(spread > val)
                {
                    if (diff > SmoothFactor)
                    {
                        spread -= SmoothFactor;
                    }
                }
            }
            spread = Math.Min(spread, 1.0f);
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
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, (float)y / (float)Starfield.NumY, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);
                        Color atPos = Starfield.GetColor((int)x, (int)y, (int)z);
                        Color toDraw = Color.FromArgb((int)(rate * atPos.R), (int)(rate * atPos.G), (int)(rate * atPos.B));

                        Threshold = 1.0f - ((Spread * (MaxSpread - MinSpread)) + MinSpread);

                        if (n > Threshold)
                        {
                            n -= Threshold;
                            n *= 1 / (1 - threshold);
                            toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
                        }
                        else if (Fade && n > (Threshold - FadeThreshold))
                        {
                            n -= (Threshold - FadeThreshold);
                            n *= 1 / FadeThreshold;
                            toDraw = ColorUtils.GetGradientColor(Color.Black, PrimaryColor, n, CapAtMax);
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
            time = (time + TimeStep);
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = SoundProcessor.GetSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnFrameUpdate += soundProcessor_OnFrameUpdate;
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Sound Responsive Simplex Clouds";
        }
        #endregion
    }
}
