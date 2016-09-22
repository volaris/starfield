using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils.ColorUtils;

// Adapted from https://github.com/zestyping/openpixelcontrol/blob/master/python/raver_plaid.py

namespace StarfieldDrivers.Animation
{
    [DriverType(DriverTypes.Ambient)]
    public class RaverPlaid : IStarfieldDriver
    {
        #region Private members
        DateTime time;

        int freq_r = 24;
        int freq_g = 24;
        int freq_b = 24;

        int speed_r = 7;
        int speed_g = -13;
        int speed_b = 19;
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            double t = (DateTime.Now - time).TotalSeconds;
            ulong numPixels = Starfield.NumX * Starfield.NumY * Starfield.NumZ;
            for (ulong i = 0; i < numPixels; i++)
            {
                int x = (int)(i / (Starfield.NumZ * Starfield.NumY));
                int z = (int)((i % (Starfield.NumZ * Starfield.NumY)) / Starfield.NumY);
                int y = (int)((Starfield.NumY - 1) - (i % (Starfield.NumZ * Starfield.NumY)) % Starfield.NumY);

                double pct = (double)i / numPixels;

                double pct_jittered = (pct * 77) % 37;

                double blackstripes = (Math.Cos((pct_jittered/1 - (t * .05d)) * Math.PI * 2) / 2 + .5d)* 3.0d - 1.5d;
                double blackstripes_offset = (Math.Cos((t / 60 - .9d) * Math.PI * 2) / 2 + .5d) * 3.5d - .5d;
                blackstripes = Math.Min(1, Math.Max(0, blackstripes + blackstripes_offset));

                int r = (int)(blackstripes * ((Math.Cos((t / speed_r + pct * freq_r) * Math.PI * 2) + 1) / 2) * 256);
                int g = (int)(blackstripes * ((Math.Cos((t / speed_g + pct * freq_g) * Math.PI * 2) + 1) / 2) * 256);
                int b = (int)(blackstripes * ((Math.Cos((t / speed_b + pct * freq_b) * Math.PI * 2) + 1) / 2) * 256);
                Color toDraw = Color.FromArgb(r, g, b);
                Starfield.SetColor(x, y, z, toDraw);
            }
        }

        public void Start(StarfieldModel Starfield)
        {
            time = DateTime.Now;
        }

        public void Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Raver Plaid";
        }
        #endregion
    }
}
