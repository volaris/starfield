using System;
using System.Collections.Generic;

using StarfieldUtils.MathUtils;

namespace Delaunay
{
	namespace Geo
	{
		public sealed class LineSegment
		{
			public static int CompareLengths_MAX (LineSegment segment0, LineSegment segment1)
			{
                float length0 = (float)segment0.p0.DistanceTo(segment0.p1);
                float length1 = (float)segment1.p0.DistanceTo(segment1.p1);
				if (length0 < length1) {
					return 1;
				}
				if (length0 > length1) {
					return -1;
				}
				return 0;
			}
		
			public static int CompareLengths (LineSegment edge0, LineSegment edge1)
			{
				return - CompareLengths_MAX (edge0, edge1);
			}

            public Vec2D p0;
            public Vec2D p1;
		
			public LineSegment (Vec2D p0, Vec2D p1)
			{
				this.p0 = p0;
				this.p1 = p1;
			}

            public bool IntersectsWith(LineSegment lineSegment)
            {
                return LineSegment.Intersection(this, lineSegment) == null;
            }

            private Vec2D Intersection(LineSegment lineSegment, bool includeEndpoints = false)
            {
                Vec2D A = this.p0;
                Vec2D B = this.p1;
                Vec2D C = lineSegment.p0;
                Vec2D D = lineSegment.p1;
                Vec2D E = B - A;
                Vec2D F = D - C;
                Vec2D P = new Vec2D(-1 * E.Y, E.X);
                double h = (A - C).Dot(P) / F.Dot(P);

                if ((h > 0 && h < 1) || (includeEndpoints && (h == 0 || h == 1)))
                {
                    Vec2D intersection = C + F * h;
                    return intersection;
                }
                else
                {
                    return null;
                }
            }

            public static Vec2D Intersection(LineSegment l1, LineSegment l2, bool includeEndpoints = false)
            {
                Vec2D intersection1 = l1.Intersection(l2, includeEndpoints);
                Vec2D intersection2 = l2.Intersection(l1, includeEndpoints);

                if(intersection1 != null && intersection2 != null)
                {
                    Console.WriteLine(String.Format("Seg1: {0} Seg2: {1} Intersection: {2} {3}", l1, l2, intersection1, intersection2));
                    return intersection1;
                }
                return null;
            }

            public override string ToString()
            {
                return String.Format("P0: [{0}] P1: [{1}]", this.p0, this.p1);
            }
		}
	}
}