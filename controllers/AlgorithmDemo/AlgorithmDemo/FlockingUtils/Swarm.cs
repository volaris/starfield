using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.Drawing;

namespace AlgorithmDemo.FlockingUtils
{
    class Swarm
    {
        public List<Boid> Boids = new List<Boid>();
        public Vector Goal = null;

        public Swarm(Vector center, int boundary, int count, Color[] colors)
        {
            int ColorIndex = 0;
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                Vector position = new Vector(center.x + (float)((2 * boundary) * rand.NextDouble() - boundary), center.y + (float)((2 * boundary) * rand.NextDouble() - boundary), center.z + (float)((2 * boundary) * rand.NextDouble() - boundary));
                Boids.Add(new Boid(position.x, position.y, position.z, colors[ColorIndex]));
                ColorIndex = (ColorIndex + 1) % colors.Length;
            }
        }

        public void MoveBoids()
        {
            Vector center = GetCenter();
            foreach (Boid boid in Boids)
            {
                boid.Move(Boids, Goal, center);
            }
        }

        public Vector GetCenter()
        {
            float centerX = 0;
            float centerY = 0;
            float centerZ = 0;

            foreach (Boid boid in Boids)
            {
                centerX += boid.Position.x;
                centerY += boid.Position.y;
                centerZ += boid.Position.z;
            }

            centerX /= Boids.Count;
            centerY /= Boids.Count;
            centerZ /= Boids.Count;

            Vector center = new Vector(centerX, centerY, centerZ);

            return center;
        }
    }
}
