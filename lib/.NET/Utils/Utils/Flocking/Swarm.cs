using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldUtils.MathUtils;

namespace StarfieldUtils.FlockingUtils
{
    /** <summary>    Represents a swarm of boids. </summary> */
    public class Swarm
    {
        /** <summary>    The boids. </summary> */
        public List<Boid> Boids = new List<Boid>();
        /** <summary>    The goal. </summary> */
        public Vec3D Goal = null;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="center">    The center. </param>
         * <param name="boundary">  The boundary. </param>
         * <param name="count">     Number of. </param>
         * <param name="colors">    The colors. </param>
         */

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

        /** <summary>    Move all the boids. </summary> */
        public void MoveBoids()
        {
            Vec3D center = GetCenter();
            foreach (Boid boid in Boids)
            {
                boid.Move(Boids, Goal, center);
            }
        }

        /**
         * <summary>    Gets the center of the flock. </summary>
         *
         * <returns>    The center. </returns>
         */

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
