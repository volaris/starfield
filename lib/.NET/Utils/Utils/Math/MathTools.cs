/*
 * Originally by Dmitry Shesterkin
 * Adapted for non-unity use by Lane Haury
 *
 * Copyright (c) 2013 Dmitry Shesterkin
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace AlgorithmDemo.MathUtils
{
    /*public static class MathTools
    {
        public const float epsilon = 1e-10f;

        //1e-20f is too small and sometimes leads to incorrect detection of zero vector
        public const float sqrEpsilon = 1e-10f;


        //Scale point along stretchAxis by stretchFactor
        public static Vector StretchAlongAxis(Vector point, Vector stretchAxis, float stretchFactor)
        {
            var upVector = Vector.Up;

            //Check if vectors are colliniar
            if (upVector.Dot(stretchAxis) >= 1.0f - epsilon)
                upVector = Vector.Left;

            var right = upVector.Cross(stretchAxis);
            var up = stretchAxis.Cross(right);
            var forward = stretchAxis;

            right = right.Normalized;
            up = up.Normalized;
            forward = forward.Normalized;

            Matrix4x4 rotate = new Matrix4x4();
            rotate.SetColumn(0, right);
            rotate.SetColumn(1, up);
            rotate.SetColumn(2, forward);
            rotate[3, 3] = 1.0F;

            Matrix4x4 scale = new Matrix4x4();
            scale[0, 0] = 1.0f;
            scale[1, 1] = 1.0f;
            scale[2, 2] = stretchFactor;
            scale[3, 3] = 1.0f;

            Matrix4x4 trans = rotate * scale * rotate.transpose;

            return trans.MultiplyPoint3x4(point);
        }

        public delegate Vector TransformPointDlg(Vector point);

        public static void DrawTestCube(Vector center, TransformPointDlg transform)
        {
            var count = 15;
            var dist = 0.1f;

            for (int x = 0; x < count; ++x)
                for (int y = 0; y < count; ++y)
                    for (int z = 0; z < count; ++z)
                    {
                        if (x == 0 || x == count - 1 || y == 0 || y == count - 1 || z == 0 || z == count - 1)
                        {
                            var pos = new Vector(dist * (x - count / 2), dist * (y - count / 2), dist * (z - count / 2));
                            pos = transform(pos);
                            //Debug.DrawLine(center + pos * 0.9f, center + pos, Color.red);
                        }
                    }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -180)
                angle += 360;

            if (angle > 180)
                angle -= 360;

            return Math.Max(Math.Min(angle, min), max);
        }

        //Returns the closiest point to cur position on bounds of cld
        public static Vector CalcPointOnBounds(Collider cld, Vector cur)
        {
            SphereCollider sphc = cld as SphereCollider;

            if (!sphc)
                return cld.ClosestPointOnBounds(cur);
            else
            {
                //cld.ClosestPointOnBounds returns not precise values for spheres
                //Fortunately they could be calculated easily
                var realPos = sphc.transform.position + sphc.center;
                var dir = cur - realPos;
                var realScale = sphc.transform.lossyScale;
                var realRadius = sphc.radius * Math.Max(realScale.x, realScale.y, realScale.z);
                var dirLength = dir.magnitude;

                //BoxCollider.ClosestPointOnBounds returns cur if points are inside the volume
                if (dirLength < realRadius)
                    return cur;

                var dirFraction = realRadius / dirLength;
                return realPos + dirFraction * dir;
            }
        }

        public static string ToS(Vector vec)
        {
            return System.String.Format("{0:0.00000}:[{1:0.00000}, {2:0.00000}, {3:0.00000}]", vec.Magnitude, vec.x, vec.y, vec.z);
        }

        //Projects vectors on plane XZ and calculate angle between them
        public static float AngleXZProjected(Vector vec1, Vector vec2)
        {
            vec1.y = 0;
            vec2.y = 0;

            return vec1.AngleTo(vec2);
        }

        //Projects vectors on plane XZ and turn it right on 90deg
        public static Vector RightVectorXZProjected(Vector vec)
        {
            //We do the same as:
            //  vec.y = 0;
            //  return Quaternion.AngleAxis(90, Vector3.up) * vec;
            //but faster
            //http://en.wikipedia.org/wiki/Rotation_matrix#Basic_rotations

            const float sin = 1.0f;
            const float cos = 0.0f;

            return new Vector(cos * vec.x + sin * vec.z, 0, -sin * vec.x + cos * vec.z);
        }

        //Returns magnitude of vector vec projected on vecNormal
        public static float VecProjectedLength(Vector vec, Vector vecNormal)
        {
            var proj = Project(vec, vecNormal);
            return proj.Magnitude * Math.Sign(proj.Dot(vecNormal));
        }

        //Check that Quaternion is not NaN
        public static bool IsValid(Quaternion q)
        {
#pragma warning disable 1718
            return q == q; //Comparisons to NaN always return false, no matter what the value of the float is.
#pragma warning restore 1718
        }

        //Check that Vector3 is not NaN
        public static bool IsValid(Vector v)
        {
#pragma warning disable 1718
            return v == v; //Comparisons to NaN always return false, no matter what the value of the float is.
#pragma warning restore 1718
        }

        //Map interval of angles between vectors [0..Pi] to interval [0..1]
        //Vectors a and b must be normalized
        public static float AngleToFactor(Vector a, Vector b)
        {
            //plot((1-cos(x))/2, x = 0..Pi);
            return (1 - a.Dot(b)) / 2;
        }

        public static Quaternion RandomYawPitchRotation()
        {
            Random rand = new Random();
            return Quaternion.Euler((float)((rand.NextDouble()*180)-90), (float)((rand.NextDouble()*360)-180), 0);
        }

        public static Vector RandomVectorInBox(float boxSize)
        {
            Random rand = new Random();
            return new Vector((float)((boxSize * rand.NextDouble()) - boxSize), (float)((boxSize * rand.NextDouble()) - boxSize), (float)((boxSize * rand.NextDouble()) - boxSize));
        }

        public delegate void PlacePoint(int x, int y);

        //Place totalNumber of elements in order of square
        public static void FillSquareUniform(float totalNumber, PlacePoint dlg)
        {
            int mainSize = (int)System.Math.Sqrt(totalNumber);

            int lbrd = -mainSize / 2;
            int rbrd = lbrd + mainSize;

            for (int x = lbrd; x < rbrd; ++x)
                for (int y = lbrd; y < rbrd; ++y)
                    dlg(x, y);

            int restOfItems = (int)(totalNumber + 0.5f) - mainSize * mainSize;
            --lbrd;
            ++rbrd;

            for (int y = rbrd - 1; y > lbrd && restOfItems > 0; --y, --restOfItems)
                dlg(rbrd - 1, y);

            for (int x = lbrd + 1; restOfItems > 0; ++x, --restOfItems)
                dlg(x, rbrd - 1);
        }

        public static Vector Project(Vector a, Vector b)
        {
            float a1 = a.Magnitude * (float)Math.Cos(a.AngleTo(b));
            Vector unitB = b.Normalized;
            return unitB * a1;
        }
    }*/
}
