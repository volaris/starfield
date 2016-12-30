using System;

using StarfieldUtils.MathUtils;

namespace Delaunay
{	
	namespace Geo
	{
		public sealed class Rect
		{
			public float xMin, xMax, yMin, yMax;

            public float x
            {
                get { return xMin; }
                set { xMin = value; }
            }

            public float y
            {
                get { return yMin; }
                set { yMin = value; }
            }

            public float width
            {
                get { return xMax - xMin; }
                set { xMax = xMin + value; }
            }

            public float height
            {
                get { return yMax - yMin; }
                set { yMax = yMin + value; }
            }

            public Rect(float x, float y, float width, float height)
            {
                this.xMin = x;
                this.yMin = y;
                this.width = width;
                this.height = height;
            }
		}
	}
}