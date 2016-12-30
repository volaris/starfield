using System.Collections.Generic;
using System;

using StarfieldUtils.MathUtils;

namespace Delaunay
{
	namespace Geo
	{
		public sealed class Polygon
		{
			private List<Vec2D> _vertices;

            public List<Vec2D> Vertices
            {
                get { return _vertices; }
            }

            public Polygon(List<Vec2D> vertices)
			{
				_vertices = vertices;
			}

			public float Area ()
			{
				return Math.Abs (SignedDoubleArea () * 0.5f); // XXX: I'm a bit nervous about this; not sure what the * 0.5 is for, bithacking?
			}

			public Winding Winding ()
			{
				float signedDoubleArea = SignedDoubleArea ();
				if (signedDoubleArea < 0) {
					return Geo.Winding.CLOCKWISE;
				}
				if (signedDoubleArea > 0) {
					return Geo.Winding.COUNTERCLOCKWISE;
				}
				return Geo.Winding.NONE;
			}

			private float SignedDoubleArea () // XXX: I'm a bit nervous about this because Actionscript represents everything as doubles, not floats
			{
				int index, nextIndex;
				int n = _vertices.Count;
                Vec2D point, next;
				float signedDoubleArea = 0; // Losing lots of precision?
				for (index = 0; index < n; ++index) {
					nextIndex = (index + 1) % n;
					point = _vertices [index];
					next = _vertices [nextIndex];
					signedDoubleArea += (float)(point.X * next.Y - next.X * point.Y);
				}
				return signedDoubleArea;
			}

            // approximation, not a robust implementation
            public Vec2D Centroid()
            {
                double xavg = 0;
                double yavg = 0;

                foreach (Vec2D vertex in _vertices)
                {
                    xavg += vertex.X;
                    yavg += vertex.Y;
                }

                xavg /= _vertices.Count;
                yavg /= _vertices.Count;

                return new Vec2D(xavg, yavg);
            }

            public Polygon Scale(float scaler)
            {
                Vec2D centroid = Centroid();
                List<Vec2D> scaled = new List<Vec2D>();
                foreach (Vec2D vertex in _vertices)
                {
                    Vec2D modified = scaler * (vertex - centroid) + centroid;
                    scaled.Add(modified);
                }
                return new Polygon(scaled);
            }

            public Polygon Smooth(int iterations)
            {
                Polygon polygon = this;
                for(int i = 0; i < iterations; i++)
                {
                    polygon = polygon.Smooth();
                }
                return polygon;
            }

            public Polygon Smooth()
            {
                List<Vec2D> smoothed = new List<Vec2D>();

                for (int i = 0; i < _vertices.Count; i ++)
                {
                    Vec2D p0, p1;

                    p1 = _vertices[i];
                    if (i == 0)
                    {
                        p0 = _vertices[_vertices.Count - 1];
                    }
                    else
                    {
                        p0 = _vertices[i - 1];
                    }

                    smoothed.Add(((p1 - p0) * .25) + p0);
                    smoothed.Add(((p1 - p0) * .75) + p0);
                }

                return new Polygon(smoothed);
            }

            public bool ContainsPoint(Vec2D point)
            {
                Vec2D p1, p2;
                bool inside = false;

                if (_vertices.Count < 3)
                {
                    return inside;
                }

                Vec2D oldPoint = new Vec2D(_vertices[_vertices.Count - 1].X, _vertices[_vertices.Count - 1].Y);

                for (int i = 0; i < _vertices.Count; i++)
                {
                    Vec2D newPoint = new Vec2D(_vertices[i].X, _vertices[i].Y);

                    if (newPoint.X > oldPoint.X)
                    {
                        p1 = oldPoint;
                        p2 = newPoint;
                    }
                    else
                    {
                        p1 = newPoint;
                        p2 = oldPoint;
                    }

                    if ((newPoint.X < point.X) == (point.X <= oldPoint.X) &&
                       ((long)point.Y - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(point.X - p1.X))
                    {
                        inside = !inside;
                    }

                    oldPoint = newPoint;
                }

                return inside;
            }

            public List<Vec2D> Intersections(LineSegment lineSegment)
            {
                List<Vec2D> intersections = new List<Vec2D>();
                for(int i = 0; i < _vertices.Count; i++)
                {

                }

                return intersections;
            }

            public List<Vec2D> GetVerticesBetween(Vec2D pt1, Vec2D pt2, Winding winding)
            {
                List<Vec2D> foundVertices = new List<Vec2D>();
                int i1, i2;

                if(winding == Geo.Winding.NONE)
                {
                    return foundVertices;
                }

                if(_vertices.Contains(pt1) && _vertices.Contains(pt2) && (i1 = _vertices.IndexOf(pt1)) != (i2 = _vertices.IndexOf(pt2)))
                {
                    if (winding != this.Winding())
                    {
                        int t = i2;
                        i2 = i1;
                        i1 = t;
                    }

                    int i = (i1 + 1) % _vertices.Count;
                    while(i != i2)
                    {
                        foundVertices.Add(_vertices[i]);
                        i = (i + 1) % _vertices.Count;
                    }
                    Console.WriteLine(String.Format("Merging: {0}-{1} found {2}", i1, i2, foundVertices.Count));
                }
                return foundVertices;
            }

            public LineSegment Intersection(LineSegment lineSegment)
            {
                for (int i = 0; i < _vertices.Count; i++)
                {
                    Vec2D intersection;
                    LineSegment poly = new LineSegment(i == 0 ? _vertices[_vertices.Count - 1] : _vertices[i - 1], _vertices[i]);
                    intersection = LineSegment.Intersection(lineSegment, poly);
                    if(intersection != null)
                    {
                        return poly;
                    }
                }

                return null;
            }
		}
	}
}