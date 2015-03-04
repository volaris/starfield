using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;
using AlgorithmDemo.Utils;

namespace AlgorithmDemo.Drivers
{
    public class FadingStatic : IStarfieldDriver
    {
        #region Private Members
        Random rand = new Random();
        Color drawColor = Color.Blue;
        float time = 0f;
        float increment = .03f;
        float animationDuration = 1f;

        int[, ,] Prev;
        int[, ,] Next;
        #endregion

        #region Public Properties
        public float AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }

        public Color DrawColor
        {
            get { return drawColor; }
            set { drawColor = value; }
        }

        public float Increment
        {
            get { return increment; }
            set { increment = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {

            if (time == 0f)
            {
                Prev = Next;
                Next = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];

                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
                        {
                            if (time == 0)
                            {
                                Next[x, y, z] = rand.Next(2);
                            }
                        }
                    }
                }
            }

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Color prevColor = Prev[x,y,z] > 0 ? DrawColor : Color.Black;
                        Color nextColor = Next[x, y, z] > 0 ? DrawColor : Color.Black;
                        Color toDraw = ColorUtils.GetGradientColor(prevColor, nextColor, time, true);

                        Starfield.SetColor((int)x, (int)y, (int)z, toDraw);
                    }
                }
            }

            if (time < AnimationDuration)
            {
                time += Increment;
            }
            else
            {
                time = 0f;
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            Prev = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
            Next = new int[Starfield.NUM_X, Starfield.NUM_Y, Starfield.NUM_Z];
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Fading Static";
        }
        #endregion
    }
}
