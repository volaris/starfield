using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using StarfieldUtils.MathUtils;

namespace StarfieldUtils.FlockingUtils
{
    /** <summary>    Represents an individual boid for the boid swarming algorithm. </summary> */
    public class Boid
    {
        private static Random rnd = new Random();
        private static double sight = 75f;
        private static double space = 8f;
        private static double speed = 4f;
        public double dX = 0f;
        public double dY = 0f;
        public double dZ = 0f;
        public Vec3D Position;
        public Color Color;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="x">     The x coordinate. </param>
         * <param name="y">     The y coordinate. </param>
         * <param name="z">     The z coordinate. </param>
         * <param name="color"> The color. </param>
         */

        public Boid(double x, double y, double z, Color color)
        {
            Position = new Vec3D(x, y, z);
            Color = color;
        }

        /**
         * <summary>    Constructor with random placement. </summary>
         *
         * <param name="center">    The center. </param>
         * <param name="boundary">  The boundary. </param>
         * <param name="color">     The color. </param>
         */

        public Boid(Vec3D center, int boundary, Color color)
        {
            Random rand = new Random();
            Position = new Vec3D(center.X + ((2 * boundary) * rand.NextDouble() - boundary), center.Y + ((2 * boundary) * rand.NextDouble() - boundary), center.Z + ((2 * boundary) * rand.NextDouble() - boundary));
            Color = color;
        }

        /**
         * <summary>    Movement step function. </summary>
         *
         * <param name="boids">     The boids. </param>
         * <param name="goal">      The goal. </param>
         * <param name="center">    The center. </param>
         */

        public void Move(List<Boid> boids, Vec3D goal, Vec3D center)
        {
            Flock(boids, goal, center);
            CheckSpeed();
            Position.X += dX;
            Position.Y += dY;
            Position.Z += dZ;
        }

        private void Flock(List<Boid> boids, Vec3D goal, Vec3D center)
        {
            // flocking
            dX = 0;
            dY = 0;
            dZ = 0;
            foreach (Boid boid in boids)
            {
                float distance = (float)Position.DistanceTo(boid.Position);
                if (boid != this)
                {
                    if (distance < space)
                    {
                        // Create space.
                        dX += Position.X - boid.Position.X;
                        dY += Position.Y - boid.Position.Y;
                        dZ += Position.Z - boid.Position.Z;
                    }
                    else if (distance < sight)
                    {
                        // Flock together.
                        dX += (boid.Position.X - Position.Z) * 0.05f; //TODO: Position.z looks like a bug, investigate
                        dY += (boid.Position.Y - Position.Y) * 0.05f;
                        dZ += (boid.Position.Z - Position.Z) * 0.05f;

                        // Align movement.
                        dX += boid.dX * 0.5f;
                        dY += boid.dY * 0.5f;
                        dZ += boid.dZ * 0.5f;
                    }
                }
            }

            // move towards goal
            Vec3D toGoal = goal - Position;
            dX += toGoal.X * 0.7f;
            dY += toGoal.Y * 0.7f;
            dZ += toGoal.Z * 0.7f;


            // spinning around flock direction
            Vec3D rotationVector = goal - center;
            Quaternion rotation = new Quaternion();
            rotation.FromAxisAngle(rotationVector, (Math.PI / 2));
            Vec3D normalizedPoint = Position - center;

            rotation.Rotate(normalizedPoint);
            Vec3D rotationGoal = normalizedPoint + center;
            Vec3D rotationDiff = rotationGoal - Position;

            dX += rotationDiff.X;
            dY += rotationDiff.Y;
            dZ += rotationDiff.Z;
        }

        private void CheckSpeed()
        {
            double s = speed / 4f;
            double val = (new Vec3D(dX, dY, dZ)).Magnitude;
            if (val > s)
            {
                dX = dX * s / val;
                dY = dY * s / val;
                dZ = dZ * s / val;
            }
        }
    }
}
