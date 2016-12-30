using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    public class Vec2D
    {
        protected Vec3D baseVector;

        public static Vec2D Zero
        {
            get { return new Vec2D(0, 0); }
        }

        public double X
        {
            get { return baseVector.X; }
            set { this.baseVector.X = value; }
        }

        public double Y
        {
            get { return baseVector.Y; }
            set { this.baseVector.Y = value; }
        }

        public static Vec2D XAxis
        {
            get { return new Vec2D(1, 0); }
        }

        public static Vec2D YAxis
        {
            get { return new Vec2D(0, 1); }
        }

        public double Magnitude
        {
            get { return baseVector.Magnitude; }
        }

        public Vec2D(double X, double Y)
        {
            this.baseVector = new Vec3D(X, Y, 0);
        }

        public double DistanceTo(Vec2D vector)
        {
            return this.baseVector.DistanceTo(vector.baseVector);
        }

        public double Dot(Vec2D vector)
        {
            return baseVector.Dot(vector.baseVector);
        }

        public static Vec2D operator -(Vec2D left, Vec2D right)
        {
            Vec3D result = left.baseVector - right.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        public static Vec2D operator +(Vec2D left, Vec2D right)
        {
            Vec3D result = left.baseVector + right.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        public static Vec2D operator *(Vec2D vector, double scalar)
        {
            Vec3D result = vector.baseVector * scalar;
            return new Vec2D(result.X, result.Y);
        }

        public static Vec2D operator *(double scalar, Vec2D vector)
        {
            Vec3D result = scalar * vector.baseVector;
            return new Vec2D(result.X, result.Y);
        }

        public static Vec2D operator /(Vec2D vector, double scalar)
        {
            Vec3D result = vector.baseVector / scalar;
            return new Vec2D(result.X, result.Y);
        }

        public override string ToString()
        {
            return String.Format("X:{0} Y:{1}", baseVector.X, baseVector.Y);
        }
    }
}
