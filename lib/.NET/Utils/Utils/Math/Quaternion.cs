﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    public struct Quaternion
    {
        public double X, Y, Z, W;

        public Quaternion(double w, double x, double y, double z)
        {
            W = w; X = x; Y = y; Z = z;
        }

        public Quaternion(float w, Vec3D v)
        {
            W = w; X = v.X; Y = v.Y; Z = v.Z;
        }

        public Vec3D V
        {
            set { X = value.X; Y = value.Y; Z = value.Z; }
            get { return new Vec3D(X, Y, Z); }
        }

        public void Normalise()
        {
            double m = W * W + X * X + Y * Y + Z * Z;
            if (m > 0.001)
            {
                m = Math.Sqrt(m);
                W /= m;
                X /= m;
                Y /= m;
                Z /= m;
            }
            else
            {
                W = 1; X = 0; Y = 0; Z = 0;
            }
        }

        public void Conjugate()
        {
            X = -X; Y = -Y; Z = -Z;
        }

        public static Quaternion Euler(double x, double y, double z)
        {
            var quatX = new Quaternion();
            quatX.FromAxisAngle(Vec3D.XAxis, x);

            var quatY = new Quaternion();
            quatY.FromAxisAngle(Vec3D.YAxis, y);

            var quatZ = new Quaternion();
            quatZ.FromAxisAngle(Vec3D.ZAxis, z);

            return (quatZ * quatX) * quatZ;
        }

        public void FromAxisAngle(Vec3D axis, double angleRadian)
        {
            double m = axis.Magnitude;
            if (m > 0.0001)
            {
                double ca = Math.Cos(angleRadian / 2);
                double sa = Math.Sin(angleRadian / 2);
                X = axis.X / m * sa;
                Y = axis.Y / m * sa;
                Z = axis.Z / m * sa;
                W = ca;
            }
            else
            {
                W = 1; X = 0; Y = 0; Z = 0;
            }
        }

        public Quaternion Copy()
        {
            return new Quaternion(W, X, Y, Z);
        }

        public void Multiply(Quaternion q)
        {
            this *= q;
        }

        //                  -1
        // V'=q*V*q     ,
        public void Rotate(Vec3D pt)
        {
            this.Normalise();
            Quaternion q1 = this.Copy();
            q1.Conjugate();

            Quaternion qNode = new Quaternion(0, pt.X, pt.Y, pt.Z);
            qNode = this * qNode * q1;
            pt.X = qNode.X;
            pt.Y = qNode.Y;
            pt.Z = qNode.Z;
        }

        public void Rotate(Vec3D[] nodes)
        {
            this.Normalise();
            Quaternion q1 = this.Copy();
            q1.Conjugate();
            for (int i = 0; i < nodes.Length; i++)
            {
                Quaternion qNode = new Quaternion(0, nodes[i].X, nodes[i].Y, nodes[i].Z);
                qNode = this * qNode * q1;
                nodes[i].X = qNode.X;
                nodes[i].Y = qNode.Y;
                nodes[i].Z = qNode.Z;
            }
        }

        // Multiplying q1 with q2 is meaning of doing q2 firstly then q1
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            double nw = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;
            double nx = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            double ny = q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z;
            double nz = q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X;
            return new Quaternion(nw, nx, ny, nz);
        }

        public override bool Equals(object obj)
        {
            if(obj is Quaternion)
            {
                return this == (Quaternion)obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Z ^ (int)W;
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return (q1.X == q2.X) && (q1.Y == q2.Y) && (q1.Z == q2.Z) && (q1.W == q2.W);
        }

        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return (q1.X != q2.X) || (q1.Y != q2.Y) || (q1.Z != q2.Z) || (q1.W != q2.W);
        }
    }
}
