using System;
using System.Collections.Generic;
using System.Diagnostics;

using Delaunay.Geo;
using Delaunay.LR;

using StarfieldUtils.MathUtils;

namespace Delaunay
{
		
	public sealed class Site: ICoord, IComparable
	{
		private static Stack<Site> _pool = new Stack<Site> ();
		public static Site Create (Vec2D p, uint index, float weight, uint color)
		{
			if (_pool.Count > 0) {
				return _pool.Pop ().Init (p, index, weight, color);
			} else {
				return new Site (p, index, weight, color);
			}
		}
		
		internal static void SortSites (List<Site> sites)
		{
//			sites.sort(Site.compare);
			sites.Sort (); // XXX: Check if this works
		}

		/**
		 * sort sites on y, then x, coord
		 * also change each site's _siteIndex to match its new position in the list
		 * so the _siteIndex can be used to identify the site for nearest-neighbor queries
		 * 
		 * haha "also" - means more than one responsibility...
		 * 
		 */
		public int CompareTo (System.Object obj) // XXX: Really, really worried about this because it depends on how sorting works in AS3 impl - Julian
		{
			Site s2 = (Site)obj;

			int returnValue = Voronoi.CompareByYThenX (this, s2);
			
			// swap _siteIndex values if necessary to match new ordering:
			uint tempIndex;
			if (returnValue == -1) {
				if (this._siteIndex > s2._siteIndex) {
					tempIndex = this._siteIndex;
					this._siteIndex = s2._siteIndex;
					s2._siteIndex = tempIndex;
				}
			} else if (returnValue == 1) {
				if (s2._siteIndex > this._siteIndex) {
					tempIndex = s2._siteIndex;
					s2._siteIndex = this._siteIndex;
					this._siteIndex = tempIndex;
				}
				
			}
			
			return returnValue;
		}


		private static readonly float EPSILON = .005f;
		/**
		This ABSOLUTELY has to be public! Otherwise you CANNOT workaround
		the major accuracy-bugs in the AS3Delaunay library (because it does NOT
		use stable, consistent data, sadly: you cannot compare two Vec2D objects
		and get a correct answer to "isEqual", it corrupts them at a micro level :( )
		*/
        public static bool CloseEnough(Vec2D p0, Vec2D p1)
		{
            return p0.DistanceTo(p1) < EPSILON;
		}

        private Vec2D _coord;
        public Vec2D Coord
        {
			get { return _coord;}
		}
		
		public uint color;
		public float weight;
		
		private uint _siteIndex;
		
		// the edges that define this Site's Voronoi region:
		private List<Edge> _edges;
		internal List<Edge> edges {
			get { return _edges;}
		}
		/**
		 which end of each edge hooks up with the previous edge in _edges:
		 
		 This MUST BE exposed - it is absurd to hide this, without it the Site
		 is generating corrupt data (the .edges property is meaningless without
		 access to this list)
		 */
		private List<Side> _edgeOrientations;
		public List<Side> edgeOrientations {
		get { return _edgeOrientations; }
		}
		// ordered list of points that define the region clipped to bounds:
        private List<Vec2D> _region;

        private Site(Vec2D p, uint index, float weight, uint color)
		{
//			if (lock != PrivateConstructorEnforcer)
//			{
//				throw new Error("Site constructor is private");
//			}
			Init (p, index, weight, color);
		}

        private Site Init(Vec2D p, uint index, float weight, uint color)
		{
			_coord = p;
			_siteIndex = index;
			this.weight = weight;
			this.color = color;
			_edges = new List<Edge> ();
			_region = null;
			return this;
		}
		
		public override string ToString ()
		{
			return "Site " + _siteIndex.ToString () + ": " + Coord.ToString ();
		}

        private void Move(Vec2D p)
		{
			Clear ();
			_coord = p;
		}
		
		public void Dispose ()
		{
//			_coord = null;
			Clear ();
			_pool.Push (this);
		}
		
		private void Clear ()
		{
			if (_edges != null) {
				_edges.Clear ();
				_edges = null;
			}
			if (_edgeOrientations != null) {
				_edgeOrientations.Clear ();
				_edgeOrientations = null;
			}
			if (_region != null) {
				_region.Clear ();
				_region = null;
			}
		}
		
		public void AddEdge (Edge edge)
		{
			_edges.Add (edge);
		}
		
		public Edge NearestEdge ()
		{
			_edges.Sort (delegate (Edge a, Edge b) {
				return Edge.CompareSitesDistances (a, b);
			});
			return _edges [0];
		}
		
		public List<Site> NeighborSites ()
		{
			if (_edges == null || _edges.Count == 0) {
				return new List<Site> ();
			}
			if (_edgeOrientations == null) { 
				ReorderEdges ();
			}
			List<Site> list = new List<Site> ();
			Edge edge;
			for (int i = 0; i < _edges.Count; i++) {
				edge = _edges [i];
				list.Add (NeighborSite (edge));
			}
			return list;
		}
			
		private Site NeighborSite (Edge edge)
		{
			if (this == edge.leftSite) {
				return edge.rightSite;
			}
			if (this == edge.rightSite) {
				return edge.leftSite;
			}
			return null;
		}

        internal List<Vec2D> Region(Rect clippingBounds)
		{
			if (_edges == null || _edges.Count == 0) {
                return new List<Vec2D>();
			}
			if (_edgeOrientations == null) { 
				ReorderEdges ();
				_region = ClipToBounds (clippingBounds);
				if ((new Polygon (_region)).Winding () == Winding.CLOCKWISE) {
					_region.Reverse ();
				}
			}
			return _region;
		}
		
		private void ReorderEdges ()
		{
			//trace("_edges:", _edges);
			EdgeReorderer reorderer = new EdgeReorderer (_edges, VertexOrSite.VERTEX);
			_edges = reorderer.edges;
			//trace("reordered:", _edges);
			_edgeOrientations = reorderer.edgeOrientations;
			reorderer.Dispose ();
		}

        private List<Vec2D> ClipToBounds(Rect bounds)
		{
            List<Vec2D> points = new List<Vec2D>();
			int n = _edges.Count;
			int i = 0;
			Edge edge;
			while (i < n && ((_edges[i] as Edge).visible == false)) {
				++i;
			}
			
			if (i == n) {
				// no edges visible
                return new List<Vec2D>();
			}
			edge = _edges [i];
			Side orientation = _edgeOrientations [i];

			if (edge.clippedEnds [orientation] == null) {
				Debugger.Log(0, "[ERROR]", "XXX: Null detected when there should be a Vec2D!");
			}
			if (edge.clippedEnds [SideHelper.Other (orientation)] == null) {
                Debugger.Log(0, "[ERROR]", "XXX: Null detected when there should be a Vec2D!");
			}
			points.Add ((Vec2D)edge.clippedEnds [orientation]);
			points.Add ((Vec2D)edge.clippedEnds [SideHelper.Other (orientation)]);
			
			for (int j = i + 1; j < n; ++j) {
				edge = _edges [j];
				if (edge.visible == false) {
					continue;
				}
				Connect (points, j, bounds);
			}
			// close up the polygon by adding another corner point of the bounds if needed:
			Connect (points, i, bounds, true);
			
			return points;
		}
		
		private void Connect (List<Vec2D> points, int j, Rect bounds, bool closingUp = false)
		{
			Vec2D rightPoint = points [points.Count - 1];
			Edge newEdge = _edges [j] as Edge;
			Side newOrientation = _edgeOrientations [j];
			// the point that  must be connected to rightPoint:
			if (newEdge.clippedEnds [newOrientation] == null) {
                Debugger.Log(0, "[ERROR]", "XXX: Null detected when there should be a Vec2D!");
			}
			Vec2D newPoint = (Vec2D)newEdge.clippedEnds [newOrientation];
			if (!CloseEnough (rightPoint, newPoint)) {
				// The points do not coincide, so they must have been clipped at the bounds;
				// see if they are on the same border of the bounds:
				if (rightPoint.X != newPoint.X
					&& rightPoint.Y != newPoint.Y) {
					// They are on different borders of the bounds;
					// insert one or two corners of bounds as needed to hook them up:
					// (NOTE this will not be correct if the region should take up more than
					// half of the bounds rect, for then we will have gone the wrong way
					// around the bounds and included the smaller part rather than the larger)
					int rightCheck = BoundsCheck.Check (rightPoint, bounds);
					int newCheck = BoundsCheck.Check (newPoint, bounds);
					float px, py;
					if ((rightCheck & BoundsCheck.RIGHT) != 0) {
						px = bounds.xMax;
						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.yMax;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.yMin;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							if (rightPoint.Y - bounds.y + newPoint.Y - bounds.y < bounds.height) {
								py = bounds.yMin;
							} else {
								py = bounds.yMax;
							}
							points.Add (new Vec2D (px, py));
							points.Add (new Vec2D (bounds.xMin, py));
						}
					} else if ((rightCheck & BoundsCheck.LEFT) != 0) {
						px = bounds.xMin;
						if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							py = bounds.yMax;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							py = bounds.yMin;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.RIGHT) != 0) {
							if (rightPoint.Y - bounds.y + newPoint.Y - bounds.y < bounds.height) {
								py = bounds.yMin;
							} else {
								py = bounds.yMax;
							}
							points.Add (new Vec2D (px, py));
							points.Add (new Vec2D (bounds.xMax, py));
						}
					} else if ((rightCheck & BoundsCheck.TOP) != 0) {
						py = bounds.yMin;
						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.xMax;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.xMin;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.BOTTOM) != 0) {
							if (rightPoint.X - bounds.x + newPoint.X - bounds.x < bounds.width) {
								px = bounds.xMin;
							} else {
								px = bounds.xMax;
							}
							points.Add (new Vec2D (px, py));
							points.Add (new Vec2D (px, bounds.yMax));
						}
					} else if ((rightCheck & BoundsCheck.BOTTOM) != 0) {
						py = bounds.yMax;
						if ((newCheck & BoundsCheck.RIGHT) != 0) {
							px = bounds.xMax;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.LEFT) != 0) {
							px = bounds.xMin;
							points.Add (new Vec2D (px, py));
						} else if ((newCheck & BoundsCheck.TOP) != 0) {
							if (rightPoint.X - bounds.x + newPoint.X - bounds.x < bounds.width) {
								px = bounds.xMin;
							} else {
								px = bounds.xMax;
							}
							points.Add (new Vec2D (px, py));
							points.Add (new Vec2D (px, bounds.yMin));
						}
					}
				}
				if (closingUp) {
					// newEdge's ends have already been added
					return;
				}
				points.Add (newPoint);
			}
			if (newEdge.clippedEnds [SideHelper.Other (newOrientation)] == null) {
				Debugger.Log(0, "[ERROR]", "XXX: Null detected when there should be a Vec2D!");
			}
			Vec2D newRightPoint = (Vec2D)newEdge.clippedEnds [SideHelper.Other (newOrientation)];
			if (!CloseEnough (points [0], newRightPoint)) {
				points.Add (newRightPoint);
			}
		}
								
		public float x {
			get { return (float)_coord.X;}
		}
		internal float y {
			get { return (float)_coord.Y;}
		}
		
		public float Dist (ICoord p)
		{
			return (float)p.Coord.DistanceTo(this._coord);
		}

	}
}

//	class PrivateConstructorEnforcer {}

//	import flash.geom.Point;
//	import flash.geom.Rectangle;
	
static class BoundsCheck
{
	public static readonly int TOP = 1;
	public static readonly int BOTTOM = 2;
	public static readonly int LEFT = 4;
	public static readonly int RIGHT = 8;
		
	/**
		 * 
		 * @param point
		 * @param bounds
		 * @return an int with the appropriate bits set if the Point lies on the corresponding bounds lines
		 * 
		 */
	public static int Check (Vec2D point, Rect bounds)
	{
		int value = 0;
		if (point.X == bounds.xMin) {
			value |= LEFT;
		}
		if (point.X == bounds.xMax) {
			value |= RIGHT;
		}
		if (point.Y == bounds.yMin) {
			value |= TOP;
		}
		if (point.Y == bounds.yMax) {
			value |= BOTTOM;
		}
		return value;
	}
}