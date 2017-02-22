using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    /** <summary>    A quaternion. </summary> */
    public struct Quaternion
    {
        /** <summary>    The X coordinate. </summary> */
        public double X;
        /** <summary>    The Y coordinate. </summary> */
        public double Y;
        /** <summary>    The Z coordinate. </summary> */
        public double Z;
        /** <summary>    The W coordinate. </summary> */
        public double W;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="w"> The W coordinate. </param>
         * <param name="x"> The X coordinate. </param>
         * <param name="y"> The Y coordinate. </param>
         * <param name="z"> The Z coordinate. </param>
         */

        public Quaternion(double w, double x, double y, double z)
        {
            W = w; X = x; Y = y; Z = z;
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="w"> The W coordinate. </param>
         * <param name="v"> The Vec3D with x,y,z coordinates. </param>
         */

        public Quaternion(float w, Vec3D v)
        {
            W = w; X = v.X; Y = v.Y; Z = v.Z;
        }

        /**
         * <summary>    Gets or sets the X,Y,Z coordinates as a Vec3D. </summary>
         *
         * <value>  The Vec3D. </value>
         */

        public Vec3D V
        {
            set { X = value.X; Y = value.Y; Z = value.Z; }
            get { return new Vec3D(X, Y, Z); }
        }

        /** <summary>    Normalizes this quaternion to unit length.</summary> */
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

        /** <summary>    Transposition (reversal) of the elements of this quaternion. </summary> */
        public void Conjugate()
        {
            X = -X; Y = -Y; Z = -Z;
        }

        /**
         * <summary>    Conversts Euler angles to a quaternion. </summary>
         *
         * <param name="x"> The X coordinate. </param>
         * <param name="y"> The Y coordinate. </param>
         * <param name="z"> The Z coordinate. </param>
         *
         * <returns>    A Quaternion. </returns>
         */

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

        /**
         * <summary>    From axis angle. </summary>
         *
         * <param name="axis">          The axis. </param>
         * <param name="angleRadian">   The angle radian. </param>
         */

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

        /**
         * <summary>    Copies this object. </summary>
         *
         * <returns>    A Quaternion. </returns>
         */

        public Quaternion Copy()
        {
            return new Quaternion(W, X, Y, Z);
        }

        /**
         * <summary>    Multiplies the given quaternion with this one. </summary>
         *
         * <param name="q"> The Quaternion to multiply. </param>
         */

        public void Multiply(Quaternion q)
        {
            this *= q;
        }

        /**
         * <summary>    Rotate the given point using this quaternion. -1 V'=q*V*q</summary>
         *
         * <param name="pt">    The point. </param>
         */

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

        /**
         * <summary>    Rotates the given points using this quaternion. </summary>
         *
         * <param name="nodes"> The nodes. </param>
         */

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

        /**
         * <summary>    Multiplying q2 with q1 is meaning of doing q1 first then q2. </summary>
         *
         * <param name="q2">    The second Quaternion. </param>
         * <param name="q1">    The first Quaternion. </param>
         *
         * <returns>    The combined quaternion. </returns>
         */

        public static Quaternion operator *(Quaternion q2, Quaternion q1)
        {
            double nw = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;
            double nx = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            double ny = q1.W * q2.Y + q1.Y * q2.W + q1.Z * q2.X - q1.X * q2.Z;
            double nz = q1.W * q2.Z + q1.Z * q2.W + q1.X * q2.Y - q1.Y * q2.X;
            return new Quaternion(nw, nx, ny, nz);
        }

        /**
         * <summary>    Indicates whether this instance and a specified object are equal. </summary>
         *
         * <param name="obj">   Another object to compare to. </param>
         *
         * <returns>
         * true if <paramref name="obj" />
         *  and this instance are the same type and represent the same value; otherwise, false.
         * </returns>
         */

        public override bool Equals(object obj)
        {
            if(obj is Quaternion)
            {
                return this == (Quaternion)obj;
            }
            return base.Equals(obj);
        }

        /**
         * <summary>    Returns the hash code for this instance. </summary>
         *
         * <returns>    A 32-bit signed integer that is the hash code for this instance. </returns>
         */

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Z ^ (int)W;
        }

        /**
         * <summary>    Equality operator. </summary>
         *
         * <param name="q1">    The first Quaternion. </param>
         * <param name="q2">    The second Quaternion. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return (q1.X == q2.X) && (q1.Y == q2.Y) && (q1.Z == q2.Z) && (q1.W == q2.W);
        }

        /**
         * <summary>    Inequality operator. </summary>
         *
         * <param name="q1">    The first Quaternion. </param>
         * <param name="q2">    The second Quaternion. </param>
         *
         * <returns>    The result of the operation. </returns>
         */

        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return (q1.X != q2.X) || (q1.Y != q2.Y) || (q1.Z != q2.Z) || (q1.W != q2.W);
        }
    }
}
