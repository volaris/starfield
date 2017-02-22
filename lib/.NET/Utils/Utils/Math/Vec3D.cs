using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StarfieldUtils.MathUtils
{
    /**
     * <summary>    A simple 3D vector. </summary>
     */

    public class Vec3D
    {
        /** <summary>    The X component. </summary> */
        public double X;
        /** <summary>    The Y component. </summary> */
        public double Y;
        /** <summary>    The Z component. </summary> */
        public double Z;

        /**
         * <summary>    Gets the unit vector along the x axis. </summary>
         *
         * <value>  The unit vector along the x axis. </value>
         */

        public static Vec3D XAxis
        {
            get { return new Vec3D(1, 0, 0); }
        }

        /**
         * <summary>    Gets the unit vector along the y axis. </summary>
         *
         * <value>  The unit vector along the y axis. </value>
         */

        public static Vec3D YAxis
        {
            get { return new Vec3D(0, 1, 0); }
        }

        /**
         * <summary>    Gets the unit vector along the z axis. </summary>
         *
         * <value>  The unit vector along the z axis. </value>
         */

        public static Vec3D ZAxis
        {
            get { return new Vec3D(0, 0, 1); }
        }

        /**
         * <summary>    Gets the magnitude of the vector. </summary>
         *
         * <value>  The magnitude. </value>
         */

        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Y, 2)); }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="X"> The X component. </param>
         * <param name="Y"> The Y component. </param>
         * <param name="Z"> The Z component. </param>
         */

        public Vec3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        /**
         * <summary>    Distance from this vector (as a point) to the provided vector (as a point). </summary>
         *
         * <param name="vector">    The vector. </param>
         *
         * <returns>    A double. </returns>
         */

        public double DistanceTo(Vec3D vector)
        {
            return Math.Sqrt(Math.Pow(X - vector.X, 2) + Math.Pow(Y - vector.Y, 2) + Math.Pow(Z - vector.Z, 2));
        }

        /**
         * <summary>    Computes the dot product of this vector with the given vector. </summary>
         *
         * <param name="vector">    The vector. </param>
         *
         * <returns>    A double. </returns>
         */

        public double Dot(Vec3D vector)
        {
            return this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z;
        }

        /**
         * <summary>    Subtraction operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec3D operator -(Vec3D left, Vec3D right)
        {
            return new Vec3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /**
         * <summary>    Addition operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec3D operator +(Vec3D left, Vec3D right)
        {
            return new Vec3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /**
         * <summary>    Multiplication operator. </summary>
         *
         * <param name="vector">    The vector. </param>
         * <param name="scalar">    The scalar. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec3D operator *(Vec3D vector, double scalar)
        {
            return new Vec3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
        }

        /**
         * <summary>    Multiplication operator. </summary>
         *
         * <param name="scalar">    The scalar. </param>
         * <param name="vector">    The vector. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec3D operator *(double scalar, Vec3D vector)
        {
            return new Vec3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
        }

        /**
         * <summary>    Division operator. </summary>
         *
         * <param name="vector">    The vector. </param>
         * <param name="scalar">    The scalar. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec3D operator /(Vec3D vector, double scalar)
        {
            return new Vec3D(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
        }
    }
}
