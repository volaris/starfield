using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.CFDUtils
{
    public class DefaultKernels
    {
        public static double WPoly6(double Radius, double H)
        {
            if(0 <= Radius && Radius <= H)
            {
                return (315 / (64 * System.Math.PI * System.Math.Pow(H, 9))) * System.Math.Pow(System.Math.Pow(H, 2) - System.Math.Pow(Radius, 2), 3);
            }
            else
            {
                return 0;
            }
        }

        public static double WPoly6Gradient(double Radius, double H)
        {
            if (0 <= Radius && Radius <= H)
            {
                return (315 / (64 * System.Math.PI * System.Math.Pow(H, 9))) * 6 * System.Math.Pow(Math.Pow(H, 2) - System.Math.Pow(Radius, 2), 2) * Radius;
            }
            else
            {
                return 0;
            }
        }

        public static double WPoly6Laplacian(double Radius, double H)
        {
            if (0 <= Radius && Radius <= H)
            {
                return (315 / (64 * Math.PI * Math.Pow(H, 9))) * 12 * Math.Pow(Math.Pow(H, 2) - Math.Pow(Radius, 2), 1) * Math.Pow(Radius, 2) * Math.Pow(Math.Pow(H, 2) - Math.Pow(Radius, 2), 2);
            }
            else
            {
                return 0;
            }
        }

        public static double WSpiky(double Radius, double H)
        {
            if (0 <= Radius && Radius <= H)
            {
                return (15 / (Math.PI * Math.Pow(H, 6))) * Math.Pow(H - Radius, 3);
            }
            else
            {
                return 0;
            }
        }

        public static double WSpikyGradient(double Radius, double H)
        {
            if(0 <= Radius && Radius <= H)
            {
                return (-45 / (Math.PI * Math.Pow(H, 6))) * Math.Pow(H - Radius, 2);
            }
            else
            {
                return 0;
            }
        }

        public static double WViscosity(double Radius, double H)
        {
            if (0 <= Radius && Radius <= H)
            {
                return (15 / (2 * Math.PI * Math.Pow(H, 3))) * ((-1 * (Math.Pow(Radius, 3)/(2*Math.Pow(H,3))) + (Math.Pow(Radius,2)/Math.Pow(H, 2)) + (H/(2*Radius)) - 1));
            }
            else
            {
                return 0;
            }
        }

        public static double WViscosityLaplacian(double Radius, double H)
        {
            return (45 / (Math.PI * Math.Pow(H, 6))) * (H - Radius);
        }
    }
}
