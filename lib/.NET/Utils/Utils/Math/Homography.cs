using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace StarfieldUtils.MathUtils
{
    /** <summary>    A class with various utilities for performing homography calculations. </summary> */
    public class Homography
    {
        /**
         * <summary>    Transforms global coordinates to camera based coordinates. </summary>
         *
         * <param name="cameraLoc"> The camera location. </param>
         * <param name="theta">     The rotations. </param>
         * <param name="point">     The point to transform. </param>
         *
         * <returns>    A Vec3D. </returns>
         */

        public static Vec3D GlobalCoordsToCameraCoords(Vec3D cameraLoc, Vec3D theta, Vec3D point)
        {
            // camera transform
            double[,] aa = { { point.X },
                             { point.Y },
                             { point.Z } };
            Matrix<double> a = Matrix<double>.Build.DenseOfArray(aa);

            double[,] ca =  { { cameraLoc.X },
                              { cameraLoc.Y },
                              { cameraLoc.Z } }; ;
            Matrix<double> c = Matrix<double>.Build.DenseOfArray(ca);

            double[,] rxa = { { 1.0d, 0.0d, 0.0d },
                               { 0.0d, Math.Cos(theta.X), Math.Sin(theta.X) },
                               { 0.0d, -Math.Sin(theta.X), Math.Cos(theta.X) } };
            Matrix<double> rx = Matrix<double>.Build.DenseOfArray(rxa);

            double[,] rya = { { Math.Cos(theta.Y), 0.0d, -Math.Sin(theta.Y) },
                                 { 0.0d, 1.0d, 0.0d },
                                 { Math.Sin(theta.Y), 0.0d, Math.Cos(theta.Y) } };
            Matrix<double> ry = Matrix<double>.Build.DenseOfArray(rya);

            double[,] rza = { { Math.Cos(theta.Z), Math.Sin(theta.Z), 0.0d },
                                { -Math.Sin(theta.Z), Math.Cos(theta.Z), 0.0d },
                                { 0.0d, 0.0d, 1.0d } };
            Matrix<double> rz = Matrix<double>.Build.DenseOfArray(rza);

            Matrix<double> d = rx * (ry * (rz * (a - c)));

            return new Vec3D(d[0, 0], d[1, 0], d[2, 0]);
        }

        /**
         * <summary>    3D to 2D. https://en.wikipedia.org/wiki/3D_projection. </summary>
         *
         * <param name="cameraLoc">         The camera location. </param>
         * <param name="theta">             The point the camera is looking at. </param>
         * <param name="point">             The point to project to the 2D position. </param>
         * <param name="distanceToScreen">  The distance to viewing screen. </param>
         *
         * <returns>
         * A Vec2D representing the x,y,z deltas from the vector between the camera and 
         * viewing screen center.
         * </returns>
         */

        public static Vec2D Project3Dto2D(Vec3D cameraLoc, Vec3D theta, Vec3D point, double distanceToScreen)
        {
            Vec3D dvec = GlobalCoordsToCameraCoords(cameraLoc, theta, point);
            double[,] va = { { dvec.X },
                             { dvec.Y },
                             { dvec.Z } };
            Matrix<double> d = Matrix<double>.Build.DenseOfArray(va);
            Matrix<double> b;

            // projection to screen plane
            if (d[2, 0] == 0.0)
            {
                b = d;
            }
            else
            {
                double ratio = distanceToScreen / d[2, 0];
                b = d * ratio;
            }

            return new Vec2D(b[0, 0], b[1, 0]);
        }

        /**
         * <summary>    Project 3 dto 2D from points. </summary>
         *
         * <param name="cameraLoc">         The camera location. </param>
         * <param name="lookingAt">         The point the camera is centered on. </param>
         * <param name="point">             The point to project to the 2D position. </param>
         * <param name="distanceToScreen">  The distance to viewing screen. </param>
         *
         * <returns>
         * A Vec2D representing the x,y,z deltas from the vector between the camera and 
         * viewing screen center.
         * </returns>
         */

        public static Vec2D Project3Dto2DFromPoints(Vec3D cameraLoc, Vec3D lookingAt, Vec3D point, double distanceToScreen)
        {
            Vec3D vec = lookingAt - cameraLoc;

            double theta = Math.Acos(vec.Y / Math.Sqrt(Math.Pow(vec.X, 2) + Math.Pow(vec.Y, 2) + Math.Pow(vec.Z, 2))) - Math.PI / 2;
            double phi = Math.Atan(vec.X / vec.Z);

            Vec3D angles = new Vec3D(theta, phi, 0);

            return Project3Dto2D(cameraLoc, angles, point, distanceToScreen);
        }
    }
}
