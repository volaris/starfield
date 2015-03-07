using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldUtils.MathUtils;

namespace StarfieldUtils.FlockingUtils
{
    public class Swarm
    {
        public List<Boid> Boids = new List<Boid>();
        public Vec3D Goal = null;

        public Swarm(Vec3D center, int boundary, int count, Color[] colors)
        {
            int ColorIndex = 0;
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                Vec3D position = new Vec3D(center.X + (float)((2 * boundary) * rand.NextDouble() - boundary), center.Y + (float)((2 * boundary) * rand.NextDouble() - boundary), center.Z + (float)((2 * boundary) * rand.NextDouble() - boundary));
                Boids.Add(new Boid(position.X, position.Y, position.Z, colors[ColorIndex]));
                ColorIndex = (ColorIndex + 1) % colors.Length;
            }
        }

        public void MoveBoids()
        {
            Vec3D center = GetCenter();
            foreach (Boid boid in Boids)
            {
                boid.Move(Boids, Goal, center);
            }
        }

        public Vec3D GetCenter()
        {
            double centerX = 0;
            double centerY = 0;
            double centerZ = 0;

            foreach (Boid boid in Boids)
            {
                centerX += boid.Position.X;
                centerY += boid.Position.Y;
                centerZ += boid.Position.Z;
            }

            centerX /= Boids.Count;
            centerY /= Boids.Count;
            centerZ /= Boids.Count;

            Vec3D center = new Vec3D(centerX, centerY, centerZ);

            return center;
        }
    }
}
