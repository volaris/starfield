using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    /**
     * <summary>    A simple 2D vector. </summary>
     */

    public class Vec2D
    {
        /** <summary>    The base 3D vector. </summary> */
        protected Vec3D baseVector;

        /**
         * <summary>    Gets the zero length vector from the origin. </summary>
         */

        public static Vec2D Zero
        {
            get { return new Vec2D(0, 0); }
        }

        /**
         * <summary>    Gets or sets the x component of this vector. </summary>
         */

        public double X
        {
            get { return baseVector.X; }
            set { this.baseVector.X = value; }
        }

        /**
         * <summary>    Gets or sets the y component of this vector. </summary>
         */

        public double Y
        {
            get { return baseVector.Y; }
            set { this.baseVector.Y = value; }
        }

        /**
         * <summary>    Gets the unity vector along the x axis. </summary>
         *
         * <value>  The unity vector along the x axis. </value>
         */

        public static Vec2D XAxis
        {
            get { return new Vec2D(1, 0); }
        }
        /**
         * <summary>    Gets the unity vector along the y axis. </summary>
         *
         * <value>  The unity vector along the y axis. </value>
         */

        public static Vec2D YAxis
        {
            get { return new Vec2D(0, 1); }
        }

        /**
         * <summary>    Gets the magnitude of this vector. </summary>
         *
         * <value>  The magnitude. </value>
         */

        public double Magnitude
        {
            get { return baseVector.Magnitude; }
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <remarks>    Volar, 2/13/2017. </remarks>
         *
         * <param name="X"> The X component. </param>
         * <param name="Y"> The Y component. </param>
         */

        public Vec2D(double X, double Y)
        {
            this.baseVector = new Vec3D(X, Y, 0);
        }

        /**
         * <summary>    Distance from this vector (as a point) to the supplied vector (as a point). </summary>
         *
         * <param name="vector">    The vector. </param>
         *
         * <returns>    A double. </returns>
         */

        public double DistanceTo(Vec2D vector)
        {
            return this.baseVector.DistanceTo(vector.baseVector);
        }

        /**
         * <summary>    Dot product of this vector with the given vector. </summary>
         *
         * <param name="vector">    The vector. </param>
         *
         * <returns>    A double. </returns>
         */

        public double Dot(Vec2D vector)
        {
            return baseVector.Dot(vector.baseVector);
        }

        /**
         * <summary>    Subtraction operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec2D operator -(Vec2D left, Vec2D right)
        {
            Vec3D result = left.baseVector - right.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        /**
         * <summary>    Addition operator. </summary>
         *
         * <param name="left">  The left. </param>
         * <param name="right"> The right. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec2D operator +(Vec2D left, Vec2D right)
        {
            Vec3D result = left.baseVector + right.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        /**
         * <summary>    Multiplication operator. </summary>
         *
         * <param name="vector">    The vector. </param>
         * <param name="scalar">    The scalar. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec2D operator *(Vec2D vector, double scalar)
        {
            Vec3D result = vector.baseVector * scalar;
            return new Vec2D(result.X, result.Y);
        }

        /**
         * <summary>    Multiplication operator. </summary>
         *
         * <param name="scalar">    The scalar. </param>
         * <param name="vector">    The vector. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec2D operator *(double scalar, Vec2D vector)
        {
            Vec3D result = scalar * vector.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        /**
         * <summary>    Division operator. </summary>
         *
         * <param name="vector">    The vector. </param>
         * <param name="scalar">    The scalar. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static Vec2D operator /(Vec2D vector, double scalar)
        {
            Vec3D result = vector.baseVector / scalar;
            return new Vec2D(result.X, result.Y);
        }

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return String.Format("X:{0} Y:{1}", baseVector.X, baseVector.Y);
        }
    }
}
