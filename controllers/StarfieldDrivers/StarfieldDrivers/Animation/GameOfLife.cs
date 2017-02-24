using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;

namespace StarfieldDrivers.Animation
{
    /** <summary>    3D game of life. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class GameOfLife : IStarfieldDriver
    {
        #region Private Members
        bool[, ,] nextField, currentField;
        int birthLowerThreshold = 7;
        int birthUpperThreshold = 9;
        int deathLowerThreshold = 5;
        int deathUpperThreshold = 10;
        Color dead = Color.Black;
        Color alive = Color.Green;
        int seedDenominator = 5;
        DateTime lastUpdate = DateTime.MinValue;
        int delay = 1000; //ms
        bool restart = false;
        #endregion

        #region Public Properties
        public Color Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        public int BirthLowerThreshold
        {
            get { return birthLowerThreshold; }
            set { birthLowerThreshold = value; }
        }

        public int BirthUpperThreshold
        {
            get { return birthUpperThreshold; }
            set { birthUpperThreshold = value; }
        }

        public Color Dead
        {
            get { return dead; }
            set { dead = value; }
        }

        public int DeathLowerThreshold
        {
            get { return deathLowerThreshold; }
            set { deathLowerThreshold = value; }
        }

        public int DeathUpperThreshold
        {
            get { return deathUpperThreshold; }
            set { deathUpperThreshold = value; }
        }

        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        public int SeedDenominator
        {
            get { return seedDenominator; }
            set { seedDenominator = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            if ((DateTime.Now - lastUpdate).TotalMilliseconds < Delay)
            {
                return;
            }

            bool allDead = true;
            bool noChange = true;

            for(ulong x = 0; x < Starfield.NumX; x++)
            {
                for(ulong y = 0; y < Starfield.NumY; y++)
                {
                    for(ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        if(currentField[x,y,z])
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, alive);
                            allDead = false;
                        }
                        else
                        {
                            Starfield.SetColor((int)x, (int)y, (int)z, dead);
                        }
                        int sum = 0;
                        Starfield.SetColor((int)x, (int)y, (int)z, currentField[x, y, z] ? alive : dead);
                        for(int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                for (int k = -1; k < 2; k++)
                                {
                                    int x_temp = (int)x+i;
                                    int y_temp = (int)y + j;
                                    int z_temp = (int)z + k;
                                    if(!(i == 0 && j == 0 && k == 0) &&
                                       (x_temp < (int)Starfield.NumX && x_temp >= 0) &&
                                       (y_temp < (int)Starfield.NumY && y_temp >= 0) &&
                                       (z_temp < (int)Starfield.NumZ && z_temp >= 0))

                                    {
                                        if (currentField[x_temp, y_temp, z_temp])
                                        {
                                            sum++;
                                        }
                                    }
                                }
                            }
                        }
                        if(currentField[x,y,z] && (sum < deathLowerThreshold || sum > deathUpperThreshold))
                        {
                            nextField[x, y, z] = false;
                            noChange = false;
                        }
                        else if(!currentField[x,y,z] && (sum >= birthLowerThreshold && sum <= birthUpperThreshold))
                        {
                            nextField[x, y, z] = true;
                            noChange = false;
                        }
                        else
                        {
                            nextField[x, y, z] = currentField[x, y, z];
                        }
                    }
                }
            }

            currentField = nextField;

            if(allDead || restart)
            {
                Start(Starfield);
                restart = false;
            }
            if(noChange)
            {
                restart = true;
            }
            lastUpdate = DateTime.Now;
        }

        public void Start(StarfieldModel Starfield)
        {
            Random rand = new Random();
            nextField = new bool[Starfield.NumX, Starfield.NumY, Starfield.NumZ];
            currentField = new bool[Starfield.NumX, Starfield.NumY, Starfield.NumZ];


            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        if(rand.Next(SeedDenominator-1) == 0)
                        {
                            currentField[x, y, z] = true;
                        }
                        else
                        {
                            currentField[x, y, z] = false;
                        }
                    }
                }
            }
        }

        public void Stop()
        {
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Game of Life";
        }
        #endregion
    }
}
