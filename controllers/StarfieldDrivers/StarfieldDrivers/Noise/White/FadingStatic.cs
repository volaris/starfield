using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Starfield;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers.Noise.White
{
    /** <summary>    Fading static animation. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class FadingStatic : IStarfieldDriver
    {
        #region Private Members
        Random rand = new Random();
        Color drawColor = Color.Blue;
        float time = 0f;
        float increment = .03f;
        float animationDuration = 1f;
        bool flash = false;

        int[, ,] Prev;
        int[, ,] Next;
        #endregion

        #region Public Properties

        /**
         * <summary>    Gets or sets the duration of the animation. </summary>
         *
         * <value>  The animation duration. </value>
         */

        public float AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }

        /**
         * <summary>    Gets or sets the draw color. </summary>
         *
         * <value>  The draw color. </value>
         */

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        /**
         * <summary>    Gets or sets a value indicating whether or not to flash. </summary>
         *
         * <value>  True if flash, false if not. </value>
         */

        public bool Flash
        {
            get { return flash; }
            set { flash = value; }
        }

        /**
         * <summary>    Gets or sets the amount to increment by. </summary>
         *
         * <value>  The amount to increment by. </value>
         */

        public float Increment
        {
            get { return increment; }
            set { increment = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        public void Render(StarfieldModel Starfield)
        {

            if (time == 0f)
            {
                Prev = Next;
                Next = new int[Starfield.NumX, Starfield.NumY, Starfield.NumZ];

                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = 0; y < Starfield.NumY; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            if (time == 0)
                            {
                                Next[x, y, z] = rand.Next(2);
                            }
                        }
                    }
                }
            }

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color prevColor = Prev[x,y,z] > 0 ? DrawColor : Color.Black;
                        Color nextColor = Next[x, y, z] > 0 ? DrawColor : Color.Black;
                        Color toDraw = ColorUtils.GetGradientColor(prevColor, nextColor, time, !flash);

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }

            if (time < AnimationDuration)
            {
                time += Increment;
                if(!flash && time > AnimationDuration)
                {
                    time = AnimationDuration;
                }
            }
            else
            {
                time = 0f;
            }
        }

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            Prev = new int[Starfield.NumX, Starfield.NumY, Starfield.NumZ];
            Next = new int[Starfield.NumX, Starfield.NumY, Starfield.NumZ];
        }

        /** <summary>    Stops this object. </summary> */
        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return "Fading Static";
        }
        #endregion
    }
}
