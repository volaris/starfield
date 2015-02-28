/*
 * Originally by Dmitry Shesterkin
 * Adapted for non-unity use by Lane Haury
 *
 * Copyright (c) 2013 Dmitry Shesterkin
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace AlgorithmDemo.FlockingUtils
{
    /*class Boid1
    {
        public float SpeedMultipliyer = 3.0f;
        public float ViewRadius = 0.5f;
        public float OptDistance = 0.1f;
        public float MinSpeed { get { return 0.1f * SpeedMultipliyer; } }
        public float InclineFactor { get { return 300.0f / SpeedMultipliyer; } }
        public float AligmentForcePart = 0.002f;
        public float TotalForceMultipliyer = 12;
        public float Inertness = 0.5f;
        public float VerticalPriority = 1.0f;
        public float AttractrionForce = 0.02f;
        public float RotationForce = .02f;
        public bool Rotate = false;

        public void FixedUpdate(List<Boid1> Boids)
        {
            //Algorithm based on
            //http://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/
            //http://www.red3d.com/cwr/boids/

            //Bird is affected by 3 base forses:
            // cohesion
            // separation + collisionAvoidance
            // alignmentForce

            var sepForce = new BoidTools.SeparationForce(sts);
            var collAvoid = new BoidTools.CollisionAvoidanceForce(sts, sepForce.Calc(OptDistance));

            //Geometric center of visible birds
            var centeroid = Vector.Zero;

            var collisionAvoidance = Vector.Zero;
            var avgSpeed = Vector.Zero;
            var neighbourCount = 0;

            //Store it as an optimization
            var direction = transform.rotation * Vector.Forward;
            var curPos = transform.position;

            foreach (var vis in Physics.OverlapSphere(curPos, ViewRadius))
            {
                var visPos = vis.transform.position;
                Boid1 boid;
                ITrigger trigger;

                if ((boid = vis.GetComponent<Boid>()) != null) //Birds processing
                {
                    Vector3 separationForce;

                    if (!sepForce.Calc(curPos, visPos, out separationForce))
                        continue;

                    collisionAvoidance += separationForce;
                    ++neighbourCount;
                    centeroid += visPos;
                    avgSpeed += boid.velocity;
                }
                else if ((trigger = vis.GetInterface<ITrigger>()) != null)
                {
                    if (collider.bounds.Intersects(vis.bounds))
                        trigger.OnTouch(this);
                }
                else //Obstacles processing
                {
                    BoidTools.CollisionAvoidanceForce.Force force;
                    if (collAvoid.Calc(curPos, direction, vis, out force))
                    {
                        collisionAvoidance += force.dir;

                        if (dbgSts.enableDrawing && dbgSts.obstaclesAvoidanceDraw)
                            Drawer.DrawRay(force.pos, force.dir, dbgSts.obstaclesAvoidanceColor);
                    }
                }
            }

            if (neighbourCount > 0)
            {
                //Cohesion force. It makes united formula with BoidTools.SeparationForce
                centeroid = centeroid / neighbourCount - curPos;

                //Spherical shape of flock looks unnatural, so let's scale it along y axis
                centeroid.y *= VerticalPriority;

                //Difference between current bird speed and average speed of visible birds
                avgSpeed = avgSpeed / neighbourCount - velocity;
            }

            var positionForce = (1.0f - AligmentForcePart) * SpeedMultipliyer * (centeroid + collisionAvoidance);
            var alignmentForce = AligmentForcePart * avgSpeed / Time.deltaTime;
            var attractionForce = CalculateAttractionForce(sts, curPos, velocity);
            var totalForce = TotalForceMultipliyer * (positionForce + alignmentForce + attractionForce);

            var newVelocity = (1 - Inertness) * (totalForce * Time.deltaTime) + Inertness * velocity;

            velocity = CalcNewVelocity(MinSpeed, velocity, newVelocity, direction);

            var rotation = CalcRotation(InclineFactor, velocity, totalForce);

            if (MathTools.IsValid(rotation))
                gameObject.transform.rotation = rotation;
        }
    }

    public static class BoidTools
    {
        //Force prevents birds from collapsing into the point. Works with cohesion force.
        //Formula bases on assumption that cohesion force is the difference between bird's
        //position and geometric center of visible birds
        public struct SeparationForce
        {
            public SeparationForce(Boid1.Settings sts)
            {
                //We have to compensate cohesion force which in the OptDistance point
                //equals OptDistance / 2
                //solve( {optFactor / OptDistance = OptDistance / 2}, {optFactor} );
                optFactor = sts.OptDistance * sts.OptDistance / 2;
            }

            public bool Calc(Vector3 cur, Vector3 other, out Vector3 force)
            {
                var revDir = cur - other;
                var sqrDist = revDir.sqrMagnitude;

                force = Vector3.zero;

                if (sqrDist < MathTools.sqrEpsilon) // Do not take into account oneself
                    return false;

                //simplify( revDir / dist * (optFactor / dist) );
                force = revDir * (optFactor / sqrDist);
                return true;
            }

            public float Calc(float dist)
            {
                return optFactor / dist;
            }

            readonly float optFactor;
        };


        //There was a delegate instead this define, but it was unoptimal because
        //delegates create garbage:
        //http://stackoverflow.com/questions/1582754/does-using-a-delegate-create-garbage
        //#define COLLISION_AVOIDANCE_SQUARE


        //Force between birds and obstacles
        public struct CollisionAvoidanceForce
        {
            public CollisionAvoidanceForce(float ViewRadius, float SpeedMultiplier, float OptDistance, float sepForceAtOptDistance)
            {
                //We make an asumption that between an obstacle and a bird on the distance OptDistance should exists same
                //force as between two birds on the same distance

                optDistance = OptDistance;

                // Maple:
                // restart;
                // f := x-> factor2*(factor1/x^2 - 1);
                // Mult := 2 * SpeedMultipliyer; #When collision occurs between birds each bird has a force vector and total force is twise bigger than between wall and bird. That's why we're  multiplying force
                // F := { f(ViewRadius) = 0, f(OptDistance) = Mult * sepForceAtOptDistance }:
                // Res := solve( F, {factor1, factor2} );
                // RealConsts := {ViewRadius = 0.5, OptDistance = 0.1, sepForceAtOptDistance = 0.05, SpeedMultipliyer = 3};
                // plot( eval(f(x), eval(Res, RealConsts) ), x = 0..eval(ViewRadius, RealConsts) );

#if COLLISION_AVOIDANCE_SQUARE
        var ViewRadius2 = sts.ViewRadius * sts.ViewRadius;
        var OptDistance2 = sts.OptDistance * sts.OptDistance;
        factor1 = ViewRadius2;
        factor2 = -2 * sts.SpeedMultipliyer * sepForceAtOptDistance * OptDistance2 / ( OptDistance2 - ViewRadius2 );
#else
                factor1 = ViewRadius;
                factor2 = -2 * SpeedMultiplier * sepForceAtOptDistance * OptDistance / (OptDistance - ViewRadius);
#endif
            }

            public struct Force
            {
                public Vector dir;
                public Vector pos;
            };

            public bool Calc(Vector cur, Vector birdDir, Collider cld, out Force force)
            {
                var pointOnBounds = MathTools.CalcPointOnBounds(cld, cur);
                var revDir = cur - pointOnBounds;
                var dist = revDir.magnitude;

                if (dist <= MathTools.epsilon)
                {
                    //Let's setup the direction to outside of colider
                    revDir = (pointOnBounds - cld.transform.position).normalized;

                    //and distance to N percent of OptDistance
                    dist = 0.1f * optDistance;
                }
                else
                    revDir /= dist;

                //Force depends on direction of bird: no need to turn a bird if it is flying in opposite direction
                force.dir = revDir * (CalcImpl(dist) * MathTools.AngleToFactor(revDir, birdDir));
                force.pos = pointOnBounds;
                return true;
            }

#if COLLISION_AVOIDANCE_SQUARE
      float CalcImpl( float dist )
      {
        return factor2 * (factor1 / (dist * dist) - 1);
      }
#else
            float CalcImpl(float dist)
            {
                return factor2 * (factor1 / dist - 1);
            }
#endif

            delegate float ForceDlg(float dist);
            readonly float factor1;
            readonly float factor2;
            readonly float optDistance;
        };
    }*/
}
