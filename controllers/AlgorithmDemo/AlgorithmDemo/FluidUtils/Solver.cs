using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AlgorithmDemo.FluidUtils
{
    class Solver
    {
        public double SmoothingLength = 1;
        public double delta_t = .006;
        public static double mu = 50;
        public static double k = 20;
        public double sigma_i = .6;
        public double sigma_s = .6;
        public Vec3D FarCorner;

        public Solver()
        {
            FarCorner.X = 1;
            FarCorner.Y = 1;
            FarCorner.Z = 1;
        }

        public static Particle GetParticle(bool polar, Color color)
        {
            Particle p = new Particle();
            p.Viscosity = mu;
            p.SurfaceTensionColor = 1;
            p.DrawColor = color;
            p.Stiffness = k;

            if (polar)
            {
                p.InterfaceTensionColor = -.5;
                p.Mass = .006;
                p.RestDensity = 500;
            }
            else
            {
                p.InterfaceTensionColor = .5;
                p.Mass = .012;
                p.RestDensity = 1000;
            }

            return p;
        }

        public void ComputeDensities(Particle[] Particles)
        {
            for(int i = 0; i < Particles.Length; i++)
            {
                Particles[i].ActualDensity = 0;
            }

            for(int i = 0; i < Particles.Length; i++)
            {
                Particles[i].ActualDensity += Particles[i].Mass * DefaultKernels.WPoly6(0, SmoothingLength);
                for(int j = i+1; j < Particles.Length; j++)
                {
                    double dx = Particles[j].Position.X - Particles[i].Position.X;
                    double dy = Particles[j].Position.Y - Particles[i].Position.Y;
                    double dz = Particles[j].Position.Z - Particles[i].Position.Z;

                    double r = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));

                    double den = Particles[j].Mass * DefaultKernels.WPoly6(r, SmoothingLength);
                    Particles[i].ActualDensity += den;
                    Particles[j].ActualDensity += den;
                }
            }
        }

        public void ComputeBasicForces(Particle[] Particles, Vec3D[] ExternalForces)
        {
            Vec3D fPressure;
            Vec3D fViscosity;
            Vec3D fExternal;

            for(int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Force.X = 0;
                Particles[i].Force.Y = 0;
                Particles[i].Force.Z = 0;
            }

            for(int i = 0; i < Particles.Length; i++)
            {
                for(int j = i+1; j < Particles.Length; j++)
                {
                    Vec3D dx;
                    dx.X = Particles[i].Position.X - Particles[j].Position.X;
                    dx.Y = Particles[i].Position.Y - Particles[j].Position.Y;
                    dx.Z = Particles[i].Position.Z - Particles[j].Position.Z;
                    double r2 = Math.Pow(dx.X, 2) + Math.Pow(dx.Y, 2) + Math.Pow(dx.Z, 2);
                    double r = Math.Sqrt(r2);
                    Vec3D ddx;
                    ddx.X = Particles[j].Velocity.X - Particles[i].Velocity.X;
                    ddx.Y = Particles[j].Velocity.Y - Particles[i].Velocity.Y;
                    ddx.Z = Particles[j].Velocity.Z - Particles[i].Velocity.Z;

                    double pi = Particles[i].Stiffness * (Particles[i].ActualDensity - Particles[i].RestDensity);
                    double pj = Particles[j].Stiffness * (Particles[j].ActualDensity - Particles[j].RestDensity);

                    fPressure.X = -1 * Particles[j].Mass * (pi + pj) / (2 * Particles[j].ActualDensity) * DefaultKernels.WSpikyGradient(r, SmoothingLength) * dx.X;
                    fPressure.Y = -1 * Particles[j].Mass * (pi + pj) / (2 * Particles[j].ActualDensity) * DefaultKernels.WSpikyGradient(r, SmoothingLength) * dx.Y;
                    fPressure.Z = -1 * Particles[j].Mass * (pi + pj) / (2 * Particles[j].ActualDensity) * DefaultKernels.WSpikyGradient(r, SmoothingLength) * dx.Z;

                    fViscosity.X = ((Particles[i].Viscosity + Particles[j].Viscosity) / 2) * Particles[j].Mass * (ddx.X / Particles[j].ActualDensity) * DefaultKernels.WViscosityLaplacian(r, SmoothingLength);
                    fViscosity.Y = ((Particles[i].Viscosity + Particles[j].Viscosity) / 2) * Particles[j].Mass * (ddx.Y / Particles[j].ActualDensity) * DefaultKernels.WViscosityLaplacian(r, SmoothingLength);
                    fViscosity.Z = ((Particles[i].Viscosity + Particles[j].Viscosity) / 2) * Particles[j].Mass * (ddx.Z / Particles[j].ActualDensity) * DefaultKernels.WViscosityLaplacian(r, SmoothingLength);

                    fExternal = ApplyExternalForces(Particles[i]);

                    Particles[i].Force.X += fPressure.X + fViscosity.X + fExternal.X;
                    Particles[i].Force.Y += fPressure.Y + fViscosity.Y + fExternal.Y;
                    Particles[i].Force.Z += fPressure.Z + fViscosity.Z + fExternal.Z;


                    Particles[j].Force.X -= fPressure.X + fViscosity.X;
                    Particles[j].Force.Y -= fPressure.Y + fViscosity.Y;
                    Particles[j].Force.Z -= fPressure.Z + fViscosity.Z;
                }
            }
        }

        public Vec3D ApplyExternalForces(Particle particle)
        {
            //additional damping?
            Vec3D force;
            force.X = 0;
            force.Y = -9.81;
            force.Z = 0;

            return force;
        }

        public void ComputeInterfaceFources(Particle[] Particles)
        {
            Vec3D fInterface;
            Vec3D fSurfaceTension;

            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].TensionNormal.X = 0;
                Particles[i].TensionNormal.Y = 0;
                Particles[i].TensionNormal.Z = 0;

                Particles[i].SurfaceNormal.X = 0;
                Particles[i].SurfaceNormal.Y = 0;
                Particles[i].SurfaceNormal.Z = 0;

                Particles[i].TensionColorFieldLaplacianValue.X = 0;
                Particles[i].TensionColorFieldLaplacianValue.Y = 0;
                Particles[i].TensionColorFieldLaplacianValue.Z = 0;

                Particles[i].SurfaceColorFieldLaplacianValue.X = 0;
                Particles[i].SurfaceColorFieldLaplacianValue.Y = 0;
                Particles[i].SurfaceColorFieldLaplacianValue.Z = 0;
            }

            for (int i = 0; i < Particles.Length; i++)
            {
                for (int j = i + 1; j < Particles.Length; j++)
                {
                    Vec3D dx;
                    dx.X = Particles[i].Position.X - Particles[j].Position.X;
                    dx.Y = Particles[i].Position.Y - Particles[j].Position.Y;
                    dx.Z = Particles[i].Position.Z - Particles[j].Position.Z;
                    double r2 = Math.Pow(dx.X, 2) + Math.Pow(dx.Y, 2) + Math.Pow(dx.Z, 2);
                    double r = Math.Sqrt(r2);

                    double tensionNormalVal = (Particles[j].Mass * Particles[j].InterfaceTensionColor / Particles[j].ActualDensity) * DefaultKernels.WPoly6Gradient(r, SmoothingLength);
                    double surfaceNormalVal = (Particles[j].Mass * Particles[j].SurfaceTensionColor / Particles[j].ActualDensity) * DefaultKernels.WPoly6Gradient(r, SmoothingLength);
                    double tensionFieldVal = (Particles[j].Mass * Particles[j].InterfaceTensionColor / Particles[j].ActualDensity) * DefaultKernels.WPoly6Laplacian(r, SmoothingLength);
                    double surfaceFieldVal = (Particles[j].Mass * Particles[j].SurfaceTensionColor / Particles[j].ActualDensity) * DefaultKernels.WPoly6Laplacian(r, SmoothingLength);

                    Particles[i].TensionNormal.X += tensionNormalVal * dx.X;
                    Particles[i].TensionNormal.Y += tensionNormalVal * dx.Y;
                    Particles[i].TensionNormal.Z += tensionNormalVal * dx.Z;

                    Particles[j].TensionNormal.X -= tensionNormalVal * dx.X;
                    Particles[j].TensionNormal.Y -= tensionNormalVal * dx.Y;
                    Particles[j].TensionNormal.Z -= tensionNormalVal * dx.Z;

                    Particles[i].SurfaceNormal.X += surfaceNormalVal * dx.X;
                    Particles[i].SurfaceNormal.Y += surfaceNormalVal * dx.Y;
                    Particles[i].SurfaceNormal.Z += surfaceNormalVal * dx.Z;

                    Particles[j].SurfaceNormal.X -= surfaceNormalVal * dx.X;
                    Particles[j].SurfaceNormal.Y -= surfaceNormalVal * dx.Y;
                    Particles[j].SurfaceNormal.Z -= surfaceNormalVal * dx.Z;

                    Particles[i].TensionColorFieldLaplacianValue.X += tensionFieldVal * dx.X;
                    Particles[i].TensionColorFieldLaplacianValue.Y += tensionFieldVal * dx.Y;
                    Particles[i].TensionColorFieldLaplacianValue.Z += tensionFieldVal * dx.Z;

                    Particles[j].TensionColorFieldLaplacianValue.X -= tensionFieldVal * dx.X;
                    Particles[j].TensionColorFieldLaplacianValue.Y -= tensionFieldVal * dx.Y;
                    Particles[j].TensionColorFieldLaplacianValue.Z -= tensionFieldVal * dx.Z;

                    Particles[i].SurfaceColorFieldLaplacianValue.X += surfaceFieldVal * dx.X;
                    Particles[i].SurfaceColorFieldLaplacianValue.Y += surfaceFieldVal * dx.Y;
                    Particles[i].SurfaceColorFieldLaplacianValue.Z += surfaceFieldVal * dx.Z;

                    Particles[j].SurfaceColorFieldLaplacianValue.X -= surfaceFieldVal * dx.X;
                    Particles[j].SurfaceColorFieldLaplacianValue.Y -= surfaceFieldVal * dx.Y;
                    Particles[j].SurfaceColorFieldLaplacianValue.Z -= surfaceFieldVal * dx.Z;
                }

                double tensionNormalMagnitude = Math.Sqrt(Math.Pow(Particles[i].TensionNormal.X, 2) + Math.Pow(Particles[i].TensionNormal.Y, 2) + Math.Pow(Particles[i].TensionNormal.Z, 2));
                fInterface.X = -1 * sigma_i * Particles[i].TensionColorFieldLaplacianValue.X * Particles[i].TensionNormal.X / tensionNormalMagnitude;
                fInterface.Y = -1 * sigma_i * Particles[i].TensionColorFieldLaplacianValue.Y * Particles[i].TensionNormal.Y / tensionNormalMagnitude;
                fInterface.Z = -1 * sigma_i * Particles[i].TensionColorFieldLaplacianValue.Z * Particles[i].TensionNormal.Z / tensionNormalMagnitude;
                
                double surfaceNormalMagnitude = Math.Sqrt(Math.Pow(Particles[i].SurfaceNormal.X, 2) + Math.Pow(Particles[i].SurfaceNormal.Y, 2) + Math.Pow(Particles[i].SurfaceNormal.Z, 2));
                fSurfaceTension.X = -1 * sigma_s * Particles[i].SurfaceColorFieldLaplacianValue.X * Particles[i].SurfaceNormal.X / surfaceNormalMagnitude;
                fSurfaceTension.Y = -1 * sigma_s * Particles[i].SurfaceColorFieldLaplacianValue.Y * Particles[i].SurfaceNormal.Y / surfaceNormalMagnitude;
                fSurfaceTension.Z = -1 * sigma_s * Particles[i].SurfaceColorFieldLaplacianValue.Z * Particles[i].SurfaceNormal.Z / surfaceNormalMagnitude;

                Particles[i].Force.X += fInterface.X + fSurfaceTension.X;
                Particles[i].Force.Y += fInterface.Y + fSurfaceTension.Y;
                Particles[i].Force.Z += fInterface.Z + fSurfaceTension.Z;
            }
        }

        public void Integrate(Particle[] Particles)
        {
            for(int i = 0; i < Particles.Length; i++)
            {
                Vec3D acceleration;
                acceleration.X = Particles[i].Force.X / Particles[i].Mass;
                acceleration.Y = Particles[i].Force.Y / Particles[i].Mass;
                acceleration.Z = Particles[i].Force.Z / Particles[i].Mass;

                Vec3D newPosition;
                newPosition.X = Particles[i].Position.X;
                newPosition.Y = Particles[i].Position.Y;
                newPosition.Z = Particles[i].Position.Z;

                newPosition.X += Particles[i].Velocity.X * delta_t + acceleration.X * Math.Pow(delta_t, 2);
                newPosition.Y += Particles[i].Velocity.Y * delta_t + acceleration.Y * Math.Pow(delta_t, 2);
                newPosition.Z += Particles[i].Velocity.Z * delta_t + acceleration.Z * Math.Pow(delta_t, 2);

                if(newPosition.X < 0 || newPosition.X > FarCorner.X)
                {
                    newPosition.X = Particles[i].Position.X;
                    Particles[i].Velocity.X *= -1;
                }

                if (newPosition.Y < 0 || newPosition.Y > FarCorner.Y)
                {
                    newPosition.Y = Particles[i].Position.Y;
                    Particles[i].Velocity.Y *= -1;
                }

                if (newPosition.Z < 0 || newPosition.Z > FarCorner.Z)
                {
                    newPosition.Z = Particles[i].Position.Z;
                    Particles[i].Velocity.Z *= -1;
                }
            }
        }

        public void Diffuse(Particle[] Particles)
        {
            for(int i = 0; i < Particles.Length; i++)
            {
                for(int j = i+1; i < Particles.Length; j++)
                {

                }
            }
        }
    }
}
