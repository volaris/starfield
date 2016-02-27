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
    class SoundResponsiveSimplexSmokeGradientChange : IStarfieldDriver
    {
        #region Private Members
        float primaryColor = 4.0f/6.0f;
        float secondaryColor = 0;
        int numOctaves = 4;
        float persistance = .25f;
        float lacunarity = 2.0f;
        static float time = 0;
        bool capAtMax = true;
        float timeStep = .005f;
        float spread = 0f;
        float minSpread = .2f;
        bool smoothed = false;
        float smoothFactor = .05f;
        float threshold = .75f;
        bool fade = true;
        float fadeThreshold = .1f;
        int count = 0;
        int countMax = 3;
        float lastSpread = 0;
        CSCoreLoopbackSoundProcessor soundProcessor;
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

       public float PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }

       public float SecondaryColor
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

        public int CountMax
        {
            get { return countMax; }
            set { countMax = value; }
        }
        #endregion

        #region Constructors
        public SoundResponsiveSimplexSmokeGradientChange()
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
            if (count % CountMax == 0)
            {
                lastSpread = 0;
                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = Starfield.NumY - 1; y > 0; y--)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, Starfield.GetColor((int)x, (int)y - 1, (int)z));
                        }
                    }
                }
            }

            if (Spread > lastSpread)
            {
                lastSpread = Spread;
                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color toDraw = Color.Black;
                        float n = .5f + SimplexNoise.fbm_noise4((float)x / (float)Starfield.NumX, 0, (float)z / (float)Starfield.NumZ, time, NumOctaves, Persistance, Lacunarity);

                        //n *= (Spread < MinSpread) ? MinSpread : Spread;
                        //Threshold = 1.0f - ((Spread < MinSpread) ? MinSpread : Spread);

                        n = n * Spread + MinSpread;

                        if (n > Threshold)
                        {
                            n -= Threshold;
                            n *= 1 / (1 - Threshold);
                            n *= (float)Math.Abs(this.PrimaryColor - this.SecondaryColor) + (float)Math.Min(this.PrimaryColor, this.SecondaryColor);
                            toDraw = ColorUtils.GetVibrantColorGradient(n);
                            //toDraw = ColorUtils.GetGradientColor(PrimaryColor, SecondaryColor, n, CapAtMax);
                        }
                        else if (Fade && n > (Threshold - FadeThreshold))
                        {
                            n -= (Threshold - FadeThreshold);
                            n *= 1 / FadeThreshold;
                            toDraw = ColorUtils.GetVibrantColorGradient(PrimaryColor);
                            toDraw = ColorUtils.GetGradientColor(Color.Black, toDraw, n, CapAtMax);
                            //toDraw = ColorUtils.GetGradientColor(Color.Black, PrimaryColor, n, CapAtMax);
                        }
                        Starfield.SetColor((int)x, 0, (int)z, toDraw);
                    }
                }
            }
            time = (time + TimeStep);
            count++;
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            soundProcessor = new CSCoreLoopbackSoundProcessor();
            soundProcessor.ArtifactDelay = 100;
            soundProcessor.OnFrameUpdate += soundProcessor_OnFrameUpdate;
            soundProcessor.OnArtifactDetected += soundProcessor_OnArtifactDetected;
        }

        void soundProcessor_OnArtifactDetected(Artifact artifact)
        {
            Random rand = new Random();
            PrimaryColor = (float)rand.NextDouble();

            SecondaryColor = (float)rand.NextDouble();
        }

        void IStarfieldDriver.Stop()
        {
            soundProcessor = null;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Sound Responsive Simplex Smoke With Gradient Change";
        }
        #endregion
    }
}
