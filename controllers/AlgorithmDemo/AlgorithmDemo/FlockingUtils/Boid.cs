using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using AlgorithmDemo.MathUtils;
using System.Drawing;

namespace AlgorithmDemo.FlockingUtils
{
    class Boid
    {
        private static Random rnd = new Random();
        private static float sight = 75f;
        private static float space = 8f;
        private static float speed = 4f;
        public float dX = 0f;
        public float dY = 0f;
        public float dZ = 0f;
        public Vector Position;
        public Color Color;

        public Boid(float x, float y, float z, Color color)
        {
            Position = new Vector(x, y, z);
            Color = color;
        }

        public Boid(Vector center, int boundary, Color color)
        {
            Random rand = new Random();
            Position = new Vector(center.x + (float)((2 * boundary) * rand.NextDouble() - boundary), center.y + (float)((2 * boundary) * rand.NextDouble() - boundary), center.z + (float)((2 * boundary) * rand.NextDouble() - boundary));
            Color = color;
        }

        public void Move(List<Boid> boids, Vector goal, Vector center)
        {
            Flock(boids, goal, center);
            CheckSpeed();
            Position.x += dX;
            Position.y += dY;
            Position.z += dZ;
        }

        private void Flock(List<Boid> boids, Vector goal, Vector center)
        {
            // flocking
            dX = 0;
            dY = 0;
            dZ = 0;
            foreach (Boid boid in boids)
            {
                float distance = Position.DistanceTo(boid.Position);
                if (boid != this)
                {
                    if (distance < space)
                    {
                        // Create space.
                        dX += Position.x - boid.Position.x;
                        dY += Position.y - boid.Position.y;
                        dZ += Position.z - boid.Position.z;
                    }
                    else if (distance < sight)
                    {
                        // Flock together.
                        dX += (boid.Position.x - Position.z) * 0.05f;
                        dY += (boid.Position.y - Position.y) * 0.05f;
                        dZ += (boid.Position.z - Position.z) * 0.05f;

                        // Align movement.
                        dX += boid.dX * 0.5f;
                        dY += boid.dY * 0.5f;
                        dZ += boid.dZ * 0.5f;
                    }
                }
            }

            // move towards goal
            Vector toGoal = goal - Position;
            //Console.WriteLine("goal: " + goal);
            //Console.WriteLine("position: " + Position);
            dX += toGoal.x * 0.7f;
            dY += toGoal.y * 0.7f;
            dZ += toGoal.z * 0.7f;


            // spinning around flock direction
            Vector rotationVector = goal - center;
            Quaternion rotation = new Quaternion();
            rotation.FromAxisAngle(rotationVector, (float)(Math.PI / 2));
            Vector normalizedPoint = Position - center;

            rotation.Rotate(normalizedPoint);
            Vector rotationGoal = normalizedPoint + center;
            Vector rotationDiff = rotationGoal - Position;

            dX += rotationDiff.x;
            dY += rotationDiff.y;
            dZ += rotationDiff.z;
        }

        private void CheckSpeed()
        {
            float s = speed / 4f;
            float val = (new Vector(dX, dY, dZ)).Magnitude;
            if (val > s)
            {
                dX = dX * s / val;
                dY = dY * s / val;
                dZ = dZ * s / val;
            }
        }
    }
}
