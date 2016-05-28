﻿using System;
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
    public class WhackAMole : IStarfieldDriver
    {
        private enum State
        {
            wait,
            win
        }

        #region Private Members
        Color drawColor = Color.Red;
        Color winColor = Color.Blue;
        Color waitColor = Color.Red;
        double maxHeight = 8.0;
        double height = 4.0;
        double fadeRate = .95;
        System.Drawing.Point target;
        Random rand = new Random();
        State state = State.wait;
        int step = 0;
        int numSteps = 60;
        #endregion

        #region Public Properties
        public Color WinColor
        {
            get { return winColor; }
            set { winColor = value; }
        }

        public Color WaitColor
        {
            get { return waitColor; }
            set { waitColor = value; }
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
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        if (state == State.wait)
                        {
                            if (x >= (ulong)(target.X - 1) && x <= (ulong)target.X &&
                                z >= (ulong)(target.Y - 1) && z <= (ulong)target.Y)
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, drawColor);
                                if(activity[(int)x][(int)z].activity > 0)
                                {
                                    win();
                                }
                            }
                            else
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                            }
                        }

                        if (state == State.win)
                        {
                            if (x >= (ulong)(target.X - 1) && x <= (ulong)target.X &&
                                   z >= (ulong)(target.Y - 1) && z <= (ulong)target.Y)
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, drawColor);
                            }
                            else
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                            }
                        }
                    }
                }
            }

            if(state == State.win)
            {
                step++;
                if (step >= numSteps)
                {
                    chooseNextTarget();
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

            chooseNextTarget();
        }

        void chooseNextTarget()
        {
            target.X = rand.Next(1, 11);
            target.Y = rand.Next(1, 11);
            state = State.wait;
            drawColor = WaitColor;
        }

        void win()
        {
            drawColor = WinColor;
            state = State.win;
            step = 0;
        }

        void IStarfieldDriver.Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Whack A Mole";
        }
        #endregion
    }
}
