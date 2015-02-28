using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace AlgorithmDemo.MathUtils
{
    public struct Quaternion
    {
        public float X, Y, Z, W;

        public Quaternion(float w, float x, float y, float z)
        {
            W = w; X = x; Y = y; Z = z;
        }

        public Quaternion(float w, Vector v)
        {
            W = w; X = v.x; Y = v.y; Z = v.z;
        }

        public Vector V
        {
            set { X = value.x; Y = value.y; Z = value.z; }
            get { return new Vector(X, Y, Z); }
        }

        public void Normalise()
        {
            float m = W * W + X * X + Y * Y + Z * Z;
            if (m > 0.001)
            {
                m = (float)Math.Sqrt(m);
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

        public static Quaternion Euler(float x, float y, float z)
        {
            var quatX = new Quaternion();
            quatX.FromAxisAngle(Vector.XAxis, x);

            var quatY = new Quaternion();
            quatY.FromAxisAngle(Vector.YAxis, y);

            var quatZ = new Quaternion();
            quatZ.FromAxisAngle(Vector.ZAxis, z);

            return (quatZ * quatX) * quatZ;
        }

        public void FromAxisAngle(Vector axis, float angleRadian)
        {
            float m = axis.Magnitude;
            if (m > 0.0001)
            {
                float ca = (float)Math.Cos(angleRadian / 2);
                float sa = (float)Math.Sin(angleRadian / 2);
                X = axis.x / m * sa;
                Y = axis.y / m * sa;
                Z = axis.z / m * sa;
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
        public void Rotate(Vector pt)
        {
            this.Normalise();
            Quaternion q1 = this.Copy();
            q1.Conjugate();

            Quaternion qNode = new Quaternion(0, pt.x, pt.y, pt.z);
            qNode = this * qNode * q1;
            pt.x = qNode.X;
            pt.y = qNode.Y;
            pt.z = qNode.Z;
        }

        public void Rotate(Vector[] nodes)
        {
            this.Normalise();
            Quaternion q1 = this.Copy();
            q1.Conjugate();
            for (int i = 0; i < nodes.Length; i++)
            {
                Quaternion qNode = new Quaternion(0, nodes[i].x, nodes[i].y, nodes[i].z);
                qNode = this * qNode * q1;
                nodes[i].x = qNode.X;
                nodes[i].y = qNode.Y;
                nodes[i].z = qNode.Z;
            }
        }

        // Multiplying q1 with q2 is meaning of doing q2 firstly then q1
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            float nw = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;
            float nx = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            float ny = q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z;
            float nz = q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X;
            return new Quaternion(nw, nx, ny, nz);
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
