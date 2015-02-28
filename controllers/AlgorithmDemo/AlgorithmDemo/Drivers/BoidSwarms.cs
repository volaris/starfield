using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AlgorithmDemo.FlockingUtils;
using Leap;
using StarfieldClient;
using System.Timers;

namespace AlgorithmDemo.Drivers
{
    class BoidSwarms : IStarfieldDriver
    {
        int NumSwarms = 3;
        int NumBoidsPerSwarm = 7;
        float LocationMultiplier = 4.0f;
        float GoalThreshold = 3.0f;
        bool Trails = true;
        List<Swarm> Swarms = new List<Swarm>();
        List<int> GoalIndexes = new List<int>();
        List<Vector> Goals = new List<Vector>();
        List<Timer> Timers = new List<Timer>();
        int Time = 0;
        int WrapTime = 5;
        bool AlternateDirections = false;

        Color[] rainbow10 = new Color[10];
        Color[] rainbow7 = new Color[7];

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
            Goals.Add((new Vector(10, 24, 10)));
            Goals.Add((new Vector(30, 4, 10)));
            Goals.Add((new Vector(50, 24, 10)));
            Goals.Add((new Vector(50, 4, 30)));
            Goals.Add((new Vector(50, 4, 50)));

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

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // watchdog

            int i = Timers.IndexOf((Timer)sender);

            int goal = GoalIndexes[i];
            goal = (goal + 1) % Goals.Count;
            GoalIndexes[i] = goal;
            Swarms[i].Goal = Goals[goal];
        }

        public void Render(StarfieldModel Starfield)
        {
            if (Time == 0)
            {
                // render
                for (ulong x = 0; x < Starfield.NUM_X; x++)
                {
                    for (ulong y = 0; y < Starfield.NUM_Y; y++)
                    {
                        for (ulong z = 0; z < Starfield.NUM_Z; z++)
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
                        Vector position = boid.Position;
                        //Console.WriteLine(String.Format("Position: {0}", position));
                        int x = (int)Math.Round(position.x / LocationMultiplier);
                        int y = (int)Math.Round(position.y / LocationMultiplier);
                        int z = (int)Math.Round(position.z / LocationMultiplier);
                        //Console.WriteLine(String.Format("({0}, {1}, {2})", x, y, z));

                        if (x > 0 && (ulong)x < Starfield.NUM_X &&
                           y > 0 && (ulong)y < Starfield.NUM_Y &&
                           z > 0 && (ulong)z < Starfield.NUM_Z)
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
                    //Console.WriteLine(String.Format("center: {0}", Swarms[i].GetCenter()));
                    //Console.WriteLine(String.Format("goal: {0}", Swarms[i].Goal));
                    Vector goallDiff = Swarms[i].GetCenter() - Swarms[i].Goal;
                    //Console.WriteLine(String.Format("diff: {0}", goallDiff));
                    if (goallDiff.Magnitude < GoalThreshold)
                    {
                        //Console.WriteLine("new goal");
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

        public System.Windows.Forms.Panel GetConfigPanel()
        {
            throw new NotImplementedException();
        }

        public void ApplyConfig()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Boid Swarm";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
