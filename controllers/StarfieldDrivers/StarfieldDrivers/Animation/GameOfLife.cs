using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarfieldClient;
using System.Drawing;

namespace StarfieldDrivers.Animation
{
    class GameOfLife : IStarfieldDriver
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

        public int SeedDenominator
        {
            get { return seedDenominator; }
            set { seedDenominator = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            //TODO: optimize by saving partial sums
            for(ulong x = 0; x < Starfield.NumX; x++)
            {
                for(ulong y = 0; y < Starfield.NumY; y++)
                {
                    for(ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        int sum = 0;
                        Starfield.SetColor((int)x, (int)y, (int)z, currentField[x, y, z] ? alive : dead);
                        for(int i = Math.Max(0, ((int)x) - 1); i < Math.Min((int)Starfield.NumX - 1, (int)x + 1); i++)
                        {
                            for (int j = Math.Max(0, ((int)y) - 1); j < Math.Min((int)Starfield.NumY - 1, (int)y + 1); j++)
                            {
                                for (int k = Math.Max(0, ((int)z) - 1); k < Math.Min((int)Starfield.NumZ - 1, (int)z + 1); k++)
                                {
                                    if(!(i == (int)x && j == (int)y && k == (int)z))
                                    {
                                        sum++;
                                    }
                                }
                            }
                        }
                        if(currentField[x,y,z] && (sum < deathLowerThreshold || sum > deathUpperThreshold))
                        {
                            nextField[x, y, z] = false;
                        }
                        else if(!currentField[x,y,z] && (sum > birthLowerThreshold && sum < birthUpperThreshold))
                        {
                            nextField[x, y, z] = true;
                        }
                        else
                        {
                            nextField[x, y, z] = currentField[x, y, z];
                        }
                    }
                }
            }

            currentField = nextField;
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
