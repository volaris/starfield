using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using System.Drawing;

namespace DualController
{
    /**
     * <summary>
     * maps a flat starfield back on to the cuboid starfield so that the underlying serialization
     * works out properly.
     * </summary>
     */

    class StarfieldMapper : StarfieldModel
    {
        //TODO: move into starfield libs?
        StarfieldModel flat;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="actual">    The actual. </param>
         * <param name="flat">      The flat. </param>
         */

        public StarfieldMapper(StarfieldModel actual, StarfieldModel flat) :
            base(actual.XStep, actual.YStep, actual.ZStep, actual.NumX, actual.NumY, actual.NumZ)
        {
            this.flat = flat;
        }

        /** <summary>    set all LEDs to black. </summary> */
        public override void Clear()
        {
            base.Clear();
            flat.Clear();
        }

        /**
         * <summary>    set the LED at (x, y, z) to the given color. </summary>
         *
         * <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
         *
         * <param name="x">     The x coordinate. </param>
         * <param name="y">     The y coordinate. </param>
         * <param name="z">     The z coordinate. </param>
         * <param name="color"> The color. </param>
         */

        public override void SetColor(int x, int y, int z, Color color)
        {
            throw new InvalidOperationException("Can not set color directly using this class, use one of the split models");
        }

        /**
         * <summary>    returns the color of the LED at (x, y, z) </summary>
         *
         * <param name="x"> The x coordinate. </param>
         * <param name="y"> The y coordinate. </param>
         * <param name="z"> The z coordinate. </param>
         *
         * <returns>    The color. </returns>
         */

        public override Color GetColor(int x, int y, int z)
        {
            int mappedX = x * (int)this.NumZ + z;
            int mappedZ = 0;

            return flat.GetColor(mappedX, y, mappedZ);
        }
    }
}
