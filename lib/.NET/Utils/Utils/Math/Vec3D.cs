﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    public class Vec3D
    {
        public double X;
        public double Y;
        public double Z;

        public static Vec3D XAxis
        {
            get { return new Vec3D(1, 0, 0); }
        }

        public static Vec3D YAxis
        {
            get { return new Vec3D(0, 1, 0); }
        }

        public static Vec3D ZAxis
        {
            get { return new Vec3D(0, 0, 1); }
        }

        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2) + Math.Pow(this.Y, 2)); }
        }

        public Vec3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public double DistanceTo(Vec3D vector)
        {
            return Math.Sqrt(Math.Pow(X - vector.X, 2) + Math.Pow(Y - vector.Y, 2) + Math.Pow(Z - vector.Z, 2));
        }

        public double Dot(Vec3D vector)
        {
            return this.X * vector.X + this.Y * vector.Y + this.Z * vector.Z;
        }

        public static Vec3D operator -(Vec3D left, Vec3D right)
        {
            return new Vec3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vec3D operator +(Vec3D left, Vec3D right)
        {
            return new Vec3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vec3D operator *(Vec3D vector, double scalar)
        {
            return new Vec3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
        }

        public static Vec3D operator *(double scalar, Vec3D vector)
        {
            return new Vec3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
        }

        public static Vec3D operator /(Vec3D vector, double scalar)
        {
            return new Vec3D(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
        }
    }
}
