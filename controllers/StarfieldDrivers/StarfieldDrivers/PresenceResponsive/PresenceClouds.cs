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
    [DriverType(DriverTypes.Experimental)]
    public class PresenceClouds : IStarfieldDriver
    {
        #region Private Members
        Color drawColor = Color.Red;
        #endregion

        #region Public Properties
        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
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
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        double activityPct = activity[(int)x][(int)z].activity / 100.0;
                        Color toDraw = Color.FromArgb((int)(activityPct * DrawColor.R), (int)(activityPct * DrawColor.G), (int)(activityPct * DrawColor.B));
                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Presence Responsive Clouds";
        }
        #endregion
    }
}
