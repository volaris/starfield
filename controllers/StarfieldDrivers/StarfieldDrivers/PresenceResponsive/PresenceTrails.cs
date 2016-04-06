using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using Starfield.Presence;

using System.Drawing;

namespace StarfieldDrivers.PresenceResponsive
{
    [DriverType(DriverTypes.Interactive)]
    public class PresenceTrails : IStarfieldDriver
    {
        #region Private Members
        Color drawColor = Color.Red;
        double maxHeight = 8.0;
        double height = 4.0;
        double fadeRate = .95;
        #endregion

        #region Public Properties
        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        public double FadeRate
        {
            get { return fadeRate; }
            set { fadeRate = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            List<List<Activity>> activity;
            activity = Starfield.GetPresence();

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    if(y * Starfield.YStep > maxHeight)
                    {
                        continue;
                    }
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color prev = Starfield.GetColor((int)x, (int)y, (int)z);
                        Color next = Color.FromArgb((int)(prev.R * fadeRate), (int)(prev.G * fadeRate), (int)(prev.B * fadeRate));
                        if (activity[(int)x][(int)z].activity > 0)
                        {
                            double activityPct = activity[(int)x][(int)z].activity / 100.0;
                            
                            if(y * Starfield.YStep > height)
                            {
                                activityPct = activityPct * (1 - ((y * Starfield.YStep) - height) / (maxHeight - height));
                            }
                            next = Color.FromArgb(Math.Max((int)(activityPct * DrawColor.R), next.R), Math.Max((int)(activityPct * DrawColor.G), next.G), Math.Max((int)(activityPct * DrawColor.B), next.B));
                        }
                        Starfield.SetColor((int)x, (int)y, (int)z, next);
                    }
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                    }
                }
            }
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Presence Trails";
        }
        #endregion
    }
}
