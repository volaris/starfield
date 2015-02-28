using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AlgorithmDemo.FluidUtils
{
    public struct Vec3D
    {
        public double X;
        public double Y;
        public double Z;
    }

    public struct Particle
    {
        public double Mass; // m - kg
        public Vec3D Position; // X - m
        public Vec3D Velocity; // v - m/s
        public Vec3D Force; // f - N/m^3
        public double RestDensity; // rho_0 - kg/m^3
        public double ActualDensity; // rho - kg/m^3
        public double Viscosity; // mu - Ns/m^2
        public double Stiffness; // k - gas constant - Nm/kg
        public double InterfaceTensionColor; // c^i - l
        public double SurfaceTensionColor; // c^s - l
        public double Temperature; // T - Celsius
        public Vec3D TensionColorFieldLaplacianValue;
        public Vec3D TensionNormal;
        public Vec3D SurfaceColorFieldLaplacianValue;
        public Vec3D SurfaceNormal;
        public Color DrawColor;
        public Color ColorDelta;
    }
}
