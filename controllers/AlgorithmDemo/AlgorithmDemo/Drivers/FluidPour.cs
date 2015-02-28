using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldClient;
using AlgorithmDemo.FluidUtils;

namespace AlgorithmDemo.Drivers
{
    public class FluidPour : IStarfieldDriver
    {
        Solver fluidSolver = new Solver();

        List<Particle> fluid = new List<Particle>();
        Vec3D[] externalForces;

        public void Render(StarfieldModel Starfield)
        {
            Console.WriteLine("Start Render");
            if(fluid.Count < 4000)
            {
                Vec3D position1;
                position1.X = .2d;
                position1.Y = .7d;
                position1.Z = .2d;
                Vec3D position2;
                position2.X = .7d;
                position2.Y = .7d;
                position2.Z = .7d;
                for(int i = 0; i < 100; i++)
                {
                    Vec3D actual;
                    if(i%2 == 0)
                    {
                        actual.X = position1.X;
                        actual.Y = position1.Y;
                        actual.Z = position1.Z + (double)i*.001;
                    }
                    else
                    {
                        actual.X = position1.X + (double)i * .001;
                        actual.Y = position1.Y;
                        actual.Z = position1.Z;
                    }
                    Particle p = Solver.GetParticle(true, Color.Blue);
                    p.Position = actual;
                   // Console.WriteLine("adding particle at {0},{1},{2}", actual.X, actual.Y, actual.Z);
                    fluid.Add(p);
                }
                for(int i = 0; i < 100; i++)
                {
                    Vec3D actual;
                    if(i%2 == 0)
                    {
                        actual.X = position2.X;
                        actual.Y = position2.Y;
                        actual.Z = position2.Z + (double)i * .001;
                    }
                    else
                    {
                        actual.X = position2.X + (double)i * .001;
                        actual.Y = position2.Y;
                        actual.Z = position2.Z;
                    }
                    Particle p = Solver.GetParticle(false, Color.Red);
                    p.Position = actual;
                    //Console.WriteLine("adding particle at {0},{1},{2}", actual.X, actual.Y, actual.Z);
                    fluid.Add(p);
                }
            }

            for (ulong x = 0; x < Starfield.NUM_X; x++)
            {
                for (ulong y = 0; y < Starfield.NUM_Y; y++)
                {
                    for (ulong z = 0; z < Starfield.NUM_Z; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                    }
                }
            }

            //Console.WriteLine("particle count {0}", fluid.Count);

            foreach(Particle p in fluid)
            {
                Vec3D light, scaled;
                scaled.X = p.Position.X * ((double)(Starfield.NUM_X - 1) / fluidSolver.FarCorner.X);
                scaled.Y = p.Position.Y * ((double)(Starfield.NUM_Y - 1) / fluidSolver.FarCorner.Y);
                scaled.Z = p.Position.Z * ((double)(Starfield.NUM_Z - 1) / fluidSolver.FarCorner.Z);

                //Console.WriteLine("particle at: {0},{1},{2}", (int)p.Position.X, (int)p.Position.Y, (int)p.Position.Z);
                //Console.WriteLine("scaled to: {0},{1},{2}", (int)scaled.X, (int)scaled.Y, (int)scaled.Z);

                light.X = Math.Round(scaled.X);
                light.Y = Math.Round(scaled.Y);
                light.Z = Math.Round(scaled.Z);

                if (Math.Sqrt(Math.Pow(light.X, 2) + Math.Pow(light.Y, 2) + Math.Pow(light.Z, 2)) < 100)
                {
                    //Console.WriteLine("Drawing at: {0},{1},{2}", (int)light.X, (int)light.Y, (int)light.Z);
                    Starfield.SetColor((int)light.X, (int)light.Y, (int)light.Z, p.DrawColor);
                }
            }

            fluidSolver.ComputeDensities(fluid.ToArray());
            fluidSolver.ComputeBasicForces(fluid.ToArray(), externalForces);
            fluidSolver.ComputeInterfaceFources(fluid.ToArray());
            fluidSolver.Integrate(fluid.ToArray());

            Console.WriteLine("End Render");
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
            return "Basic Fluid";
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
        }

        void IStarfieldDriver.Stop()
        {
        }
    }
}
