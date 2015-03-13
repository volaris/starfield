using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldUtils.FlockingUtils;
using StarfieldClient;
using System.Timers;
using StarfieldUtils.MathUtils;

namespace AlgorithmDemo.Drivers
{
    class BoidSwarms : IStarfieldDriver
    {
        #region Private Members

        int numSwarms = 3;
        int numBoidsPerSwarm = 7;
        float locationMultiplier = 4.0f;
        float goalThreshold = 3.0f;
        bool trails = true;
        List<Swarm> Swarms = new List<Swarm>();
        List<int> GoalIndexes = new List<int>();
        List<Vec3D> Goals = new List<Vec3D>();
        List<Timer> Timers = new List<Timer>();
        int Time = 0;
        int WrapTime = 5;
        bool alternateDirections = false;

        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];

        #endregion

        #region Public Properties
        public bool AlternateDirections
        {
            get { return alternateDirections; }
            set { alternateDirections = value; }
        }

        public float GoalThreshold
        {
            get { return goalThreshold; }
            set { goalThreshold = value; }
        }

        public float LocationMultiplier
        {
            get { return locationMultiplier; }
            set { locationMultiplier = value; }
        }

        public int NumBoidsPerSwarm
        {
            get { return numBoidsPerSwarm; }
            set { numBoidsPerSwarm = value; }
        }

        public int NumSwarms
        {
            get { return numSwarms; }
            set { numSwarms = value; }
        }

        public bool Trails
        {
            get { return trails; }
            set { trails = value; }
        }
        #endregion

        #region Constructors
        public BoidSwarms()
        {
            rainbow10[0] = rainbow7[0] = Color.FromArgb(0xFF, 0, 0);
            rainbow10[1] = rainbow7[1] = Color.FromArgb(0xFF, 0xA5, 0);
            rainbow10[2] = rainbow7[2] = Color.FromArgb(0xFF, 0xFF, 0);
            rainbow10[3] = rainbow7[3] = Color.FromArgb(0, 0x80, 0);
            rainbow10[4] = Color.FromArgb(0, 0xFF, 0);
            rainbow10[5] = Color.FromArgb(0, 0xA5, 0x80);
            rainbow10[6] = rainbow7[4] = Color.FromArgb(0, 0, 0xFF);
            rainbow10[7] = rainbow7[5] = Color.FromArgb(0x4B, 0, 0x82);
            rainbow10[8] = rainbow7[6] = Color.FromArgb(0xFF, 0, 0xFF);
            rainbow10[9] = Color.FromArgb(0xEE, 0x82, 0xEE);

            // create goals
            Goals.Add((new Vec3D(10, 24, 10)));
            Goals.Add((new Vec3D(30, 4, 10)));
            Goals.Add((new Vec3D(50, 24, 10)));
            Goals.Add((new Vec3D(50, 4, 30)));
            Goals.Add((new Vec3D(50, 4, 50)));

            int skip = Math.Max(1, Goals.Count / NumSwarms);
            int startIndex = 0;

            // create swarms
            for(int i = 0; i < NumSwarms; i++)
            {
                Swarms.Add(new Swarm(Goals[startIndex], (int)(2*LocationMultiplier), NumBoidsPerSwarm, rainbow7));
                startIndex = (startIndex + skip) % Goals.Count;
                GoalIndexes.Add(0);
                Swarms[i].Goal = Goals[GoalIndexes[i]];
                Timer timer = new Timer();
                timer.Elapsed += timer_Elapsed;
                timer.Interval = 20000;
                timer.Start();
                Timers.Add(timer);
            }
        }
        #endregion

        #region Event Handlers
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // watchdog

            int i = Timers.IndexOf((Timer)sender);

            int goal = GoalIndexes[i];
            goal = (goal + 1) % Goals.Count;
            GoalIndexes[i] = goal;
            Swarms[i].Goal = Goals[goal];
        }
        #endregion

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            if (Time == 0)
            {
                // render
                for (ulong x = 0; x < Starfield.NumX; x++)
                {
                    for (ulong y = 0; y < Starfield.NumY; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            if (Trails)
                            {
                                Color prev = Starfield.GetColor((int)x, (int)y, (int)z);
                                Starfield.SetColor((int)x, (int)y, (int)z, Color.FromArgb((int)(prev.R * .9f), (int)(prev.G * .9f), (int)(prev.B * .9f)));
                            }
                            else
                            {
                                Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                            }
                        }
                    }
                }

                foreach (Swarm swarm in Swarms)
                {
                    foreach (Boid boid in swarm.Boids)
                    {
                        Vec3D position = boid.Position;
                        int x = (int)Math.Round(position.X / LocationMultiplier);
                        int y = (int)Math.Round(position.Y / LocationMultiplier);
                        int z = (int)Math.Round(position.Z / LocationMultiplier);

                        if (x > 0 && (ulong)x < Starfield.NumX &&
                           y > 0 && (ulong)y < Starfield.NumY &&
                           z > 0 && (ulong)z < Starfield.NumZ)
                        {
                            Starfield.SetColor(x, y, z, boid.Color);
                        }
                    }
                }

                // move
                foreach (Swarm swarm in Swarms)
                {
                    swarm.MoveBoids();
                }

                // update goals
                for (int i = 0; i < Swarms.Count; i++)
                {
                    Vec3D goallDiff = Swarms[i].GetCenter() - Swarms[i].Goal;
                    if (goallDiff.Magnitude < GoalThreshold)
                    {
                        Timers[i].Stop();
                        Timers[i].Start();
                        int goal = GoalIndexes[i];
                        goal = (goal + 1) % Goals.Count;
                        GoalIndexes[i] = goal;
                        Swarms[i].Goal = Goals[goal];
                    }
                }
            }

            Time = (Time + 1) % WrapTime;
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
            return "Boid Swarm";
        }
        #endregion 
    }
}
