using System;

using StarfieldUtils.MathUtils;

namespace Delaunay
{	
	namespace Geo
	{
		public sealed class Circle
		{
			public Vec2D center;
			public float radius;
		
			public Circle (float centerX, float centerY, float radius)
			{
                this.center = new Vec2D(centerX, centerY);
				this.radius = radius;
			}
		
			public override string ToString ()
			{
				return "Circle (center: " + center.ToString () + "; radius: " + radius.ToString () + ")";
			}

		}
	}
}