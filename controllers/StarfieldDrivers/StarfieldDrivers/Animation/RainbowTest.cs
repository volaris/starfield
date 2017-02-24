using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers.Animation
{
    /** <summary>    Animated rainbow. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class RainbowTest : IStarfieldDriver
    {
        #region Private members
        int time = 0;
        const int NUM_FRAMES = 2;
        const int NUM_STEPS = 10; 
        int currentGoal = 1;
        int startStep = 0;
        Color[] rainbow = new Color[7] { Color.FromArgb(0xFF, 0, 0),
										 Color.FromArgb(0xFF, 0xA5, 0),
										 Color.FromArgb(0xFF, 0xFF, 0),
										 Color.FromArgb(0, 0x80, 0),
										 Color.FromArgb(0, 0, 0xFF),
										 Color.FromArgb(0x4B, 0, 0x82),
									     Color.FromArgb(0xFF, 0, 0xFF) };
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            time = (time++) % NUM_FRAMES;
            if (time == 0)
            {
                int localGoal = currentGoal;
                int localStep = startStep;
                Color goal, start, current;

                for (ulong i = 0; i < Starfield.NumX * Starfield.NumY * Starfield.NumZ; i++)
                {
                    goal = rainbow[localGoal];
                    if (localGoal > 0)
                    {
                        start = rainbow[localGoal - 1];
                    }
                    else
                    {
                        start = rainbow[rainbow.Length - 1];
                    }
                    current = ColorUtils.GetGradientColor(start, goal, (float)localStep / NUM_STEPS, true);
                    int x = (int)(i / (Starfield.NumZ * Starfield.NumY));
                    int z = (int)((i % (Starfield.NumZ * Starfield.NumY)) / Starfield.NumY);
                    int y = (int)((Starfield.NumY - 1) - (i % (Starfield.NumZ * Starfield.NumY)) % Starfield.NumY);
                    Starfield.SetColor(x, y, z, current);
                    if (localStep == NUM_STEPS)
                    {
                        localStep = 0;
                        localGoal = (localGoal + 1) % rainbow.Length;
                    }
                    else
                    {
                        localStep++;
                    }
                }

                if (startStep == NUM_STEPS)
                {
                    startStep = 0;
                    currentGoal = (currentGoal + 1) % rainbow.Length;
                }
                else
                {
                    startStep++;
                }
            }
        }

        public void Start(StarfieldModel Starfield)
        {
        }

        public void Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Rainbow Test";
        }
        #endregion
    }
}
