using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;

namespace DualController
{
    // maps a flat starfield back on to the cuboid starfield so that the underlying serialization works out properly
    class StarfieldMapper : StarfieldModel
    {
        StarfieldModel flat;

        public StarfieldMapper(StarfieldModel actual, StarfieldModel flat) :
            base(actual.XStep, actual.YStep, actual.ZStep, actual.NumX, actual.NumY, actual.NumZ)
        {
            this.flat = flat;
        }

        public override void Clear()
        {
            base.Clear();
            flat.Clear();
        }

        // set the LED at (x, y, z) to the given color
        public override void SetColor(int x, int y, int z, Color color)
        {
            throw new InvalidOperationException("Can not set color directly using this class, use one of the split models");
        }

        // returns the color of the LED at (x, y, z)
        public override Color GetColor(int x, int y, int z)
        {
            int mappedX = x * (int)this.NumZ + z;
            int mappedZ = 0;

            return flat.GetColor(mappedX, y, mappedZ);
        }
    }
}
