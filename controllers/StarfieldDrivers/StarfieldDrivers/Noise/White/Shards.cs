using System;
using System.Collections.Generic;
using Starfield;
using StarfieldUtils.MathUtils;
using System.Drawing;
using StarfieldUtils.ColorUtils;

namespace StarfieldDrivers.Noise.White
{
    /** <summary>    Shards animation. </summary> */
    [DriverType(DriverTypes.Ambient)]
    public class Shards : IStarfieldDriver
    {
        internal class Shard
        {
            public int length;
            public Vec3D location;
            public float velocity;
        }

        /**
         * <summary>    Values that represent source planes for the shard. </summary>
         */

        public enum SourcePlane
        {
            /** <summary>    An enum constant representing the X plane option (X=0 & X=Max). </summary> */
            X,
            /** <summary>    An enum constant representing the Y plane option (Y=0 & Y=Max). </summary> */
            Y,
            /** <summary>    An enum constant representing the Z plane option (Z=0 & Z=Max). </summary> */
            Z
        }

        #region Private Members
        Random rand = new Random();
        Color primaryColor = Color.Blue;
        Color secondaryColor = Color.Red;

        float velocityMin = .03f;
        float velocityMax = .3f;
        float animationDuration = 1f;
        float newPercentMax = .01f;
        float newPercentMin = .001f;
        int length = 3;
        SourcePlane sourcePlane;
        int max = 50;

        StarfieldModel buffer;

        List<Shard> shards = new List<Shard>();
        #endregion

        #region Public Properties

        /**
         * <summary>    Gets or sets the source for the source planes. </summary>
         *
         * <value>  The source. </value>
         */

        public SourcePlane Source
        {
            get { return sourcePlane; }
            set { this.sourcePlane = value; }
        }

        /**
         * <summary>    Gets or sets the duration of the animation. </summary>
         *
         * <value>  The animation duration. </value>
         */

        public float AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }

        /**
         * <summary>    Gets or sets the new shard min velocity. </summary>
         *
         * <value>  The min velocity. </value>
         */

        public float VelocityMin
        {
            get { return velocityMin; }
            set { velocityMin = value; }
        }

        /**
         * <summary>    Gets or sets the new shard max velocity. </summary>
         *
         * <value>  The velocity. </value>
         */

        public float VelocityMax
        {
            get { return velocityMax; }
            set { velocityMax = value; }
        }

        /**
         * <summary>    Gets or sets the length of new shards. </summary>
         *
         * <value>  The length. </value>
         */

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /**
         * <summary>    Gets or sets the maximum number of shards. </summary>
         *
         * <value>  The maximum value. </value>
         */

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        /**
         * <summary>    Gets or sets the minimum percentage chance of a new shard. </summary>
         *
         * <value>  The primary draw color. </value>
         */

        public float NewPercentMin
        {
            get { return newPercentMin; }
            set { newPercentMin = value; }
        }

        /**
         * <summary>    Gets or sets the minimum percentage chance of a new shard. </summary>
         *
         * <value>  The primary draw color. </value>
         */

        public float NewPercentMax
        {
            get { return newPercentMax; }
            set { newPercentMax = value; }
        }

        /**
         * <summary>    Gets or sets the primary draw color. </summary>
         *
         * <value>  The primary draw color. </value>
         */

        public Color PrimaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
        }

        /**
         * <summary>    Gets or sets the secondary draw color. </summary>
         *
         * <value>  The secondary draw color. </value>
         */

        public Color SecondaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }
        #endregion

        #region IStarfieldDriver Implementation

        /**
         * <summary>    Renders the given Starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        public void Render(StarfieldModel Starfield)
        {
            switch (sourcePlane)
            {
                case SourcePlane.X:
                    for (ulong y = 0; y < Starfield.NumY; y++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            float newPercent = (1.0f - (float)(shards.Count / max)) + NewPercentMin;
                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(0, y, z);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = velocity;
                                shards.Add(newShard);
                            }

                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(Starfield.NumX - 1, y, z);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = -velocity;
                                shards.Add(newShard);
                            }
                        }
                    }
                    break;
                case SourcePlane.Y:
                    for (ulong x = 0; x < Starfield.NumY; x++)
                    {
                        for (ulong z = 0; z < Starfield.NumZ; z++)
                        {
                            float newPercent = (1.0f - (float)(shards.Count / max)) + NewPercentMin;
                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(x, 0, z);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = velocity;
                                shards.Add(newShard);
                            }

                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(x, Starfield.NumY - 1, z);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = -velocity;
                                shards.Add(newShard);
                            }
                        }
                    }
                    break;
                case SourcePlane.Z:
                    for (ulong x = 0; x < Starfield.NumX; x++)
                    {
                        for (ulong y = 0; y < Starfield.NumY; y++)
                        {
                            float newPercent = (1.0f - (float)(shards.Count / max)) + NewPercentMin;
                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(x, y, 0);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = velocity;
                                shards.Add(newShard);
                            }

                            if (rand.NextDouble() < newPercent)
                            {
                                Shard newShard = new Shard();
                                newShard.length = length;
                                newShard.location = new Vec3D(x, y, Starfield.NumZ - 1);
                                float velocity = velocityMin + (velocityMax - velocityMin) * (float)rand.NextDouble();
                                newShard.velocity = -velocity;
                                shards.Add(newShard);
                            }
                        }
                    }
                    break;
            }

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        buffer.SetColor((int)x, (int)y, (int)z, Color.Black);
                    }
                }
            }

            UpdateShardPositions(Starfield);

            foreach (Shard shard in shards)
            {
                RenderShard(buffer, shard);
            }

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, buffer.GetColor((int)x, (int)y, (int)z));
                    }
                }
            }
        }

        /**
         * <summary>    Starts the given starfield. </summary>
         *
         * <param name="Starfield"> The starfield. </param>
         */

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            buffer = new StarfieldModel(Starfield.XStep, Starfield.YStep, Starfield.ZStep, Starfield.NumX, Starfield.NumY, Starfield.NumZ);
        }

        /** <summary>    Stops this object. </summary> */
        void IStarfieldDriver.Stop()
        {
            shards.Clear();
        }
        #endregion

        #region Private Methods

        private void RenderShard(StarfieldModel Starfield, Shard Shard)
        {
            Vec3D floor = new Vec3D(Math.Floor(Shard.location.X), Math.Floor(Shard.location.Y), Math.Floor(Shard.location.Z));
            Vec3D difference = Shard.location - floor;
            Vec3D ceil = new Vec3D(Math.Ceiling(difference.X), Math.Ceiling(difference.Y), Math.Ceiling(difference.Z));

            double pct = Math.Max(difference.X, Math.Max(difference.Y, difference.Z));

            if(Shard.velocity > 0)
            {
                pct = 1.0f - pct;
            }

            Vec3D unity = new Vec3D(0, 0, 0);
            switch(sourcePlane)
            {
                case SourcePlane.X:
                    unity = new Vec3D(1, 0, 0);
                    break;
                case SourcePlane.Y:
                    unity = new Vec3D(0, 1, 0);
                    break;
                case SourcePlane.Z:
                    unity = new Vec3D(0, 0, 1);
                    break;
            }

            Vec3D renderDirection = -1 * Shard.velocity * unity;
            renderDirection.X = renderDirection.X > 0 ? Math.Ceiling(renderDirection.X) : Math.Floor(renderDirection.X);
            renderDirection.Y = renderDirection.Y > 0 ? Math.Ceiling(renderDirection.Y) : Math.Floor(renderDirection.Y);
            renderDirection.Z = renderDirection.Z > 0 ? Math.Ceiling(renderDirection.Z) : Math.Floor(renderDirection.Z);

            for (int i = 0; i < length + 2; i++)
            {
                int renderX = (int)Math.Ceiling(renderDirection.X) * i + (int)floor.X;
                int renderY = (int)Math.Ceiling(renderDirection.Y) * i + (int)floor.Y;
                int renderZ = (int)Math.Ceiling(renderDirection.Z) * i + (int)floor.Z;

                if ((renderX < 0) || (renderX >= (int)Starfield.NumX) ||
                    (renderY < 0) || (renderY >= (int)Starfield.NumY) ||
                    (renderZ < 0) || (renderZ >= (int)Starfield.NumZ))
                {
                    // ran off the end;
                    break;
                }

                Color draw;

                if(i == 0) // start
                {
                    draw = primaryColor;
                    draw = ColorUtils.GetGradientColor(Color.Black, draw, 1.0f - (float)pct, true);
                }
                else if (i == length + 1) // end
                {
                    draw = secondaryColor;
                    draw = ColorUtils.GetGradientColor(Color.Black, draw, (float)pct, true);
                }
                else // in between
                {
                    draw = ColorUtils.GetGradientColor(primaryColor, secondaryColor, (float)(i - pct) / (float)(length), true);
                }

                ColorUtils.Blend(draw, Starfield.GetColor(renderX, renderY, renderZ));
                Starfield.SetColor(renderX, renderY, renderZ, draw);
            }
        }

        private void UpdateShardPositions(StarfieldModel Starfield)
        {
            List<Shard> shardsToRemove = new List<Shard>();

            Vec3D unity = new Vec3D(0, 0, 0);
            switch (sourcePlane)
            {
                case SourcePlane.X:
                    unity = new Vec3D(1, 0, 0);
                    break;
                case SourcePlane.Y:
                    unity = new Vec3D(0, 1, 0);
                    break;
                case SourcePlane.Z:
                    unity = new Vec3D(0, 0, 1);
                    break;
            }

            foreach (Shard shard in shards)
            {
                shard.location += (unity * shard.velocity);
                if ((shard.location.X < 0 - length) || (shard.location.X > Starfield.NumX + (ulong)length) ||
                    (shard.location.Y < 0 - length) || (shard.location.Y > Starfield.NumY + (ulong)length) ||
                    (shard.location.Z < 0 - length) || (shard.location.Z > Starfield.NumZ + (ulong)length))
                {
                    shardsToRemove.Add(shard);
                }
            }

            foreach(Shard shard in shardsToRemove)
            {
                shards.Remove(shard);
            }
        }

        #endregion

        #region Overrides

        /**
         * <summary>    Returns a string that represents the current object. </summary>
         *
         * <returns>    A string that represents the current object. </returns>
         */

        public override string ToString()
        {
            return "White Noise Shards";
        }
        #endregion
    }
}
