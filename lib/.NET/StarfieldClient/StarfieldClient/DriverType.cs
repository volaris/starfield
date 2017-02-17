using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfield
{
    /**
     * <summary>    Defines the general classes of drivers the Starfield supports. This is used to
     *              allow a control application to select an appropriate driver. </summary>
     */

    public enum DriverTypes
    {
         /** <summary>    just an animation. </summary> */
        Ambient,
         /** <summary>    an animation that responds to input, will work as an ambient animation. </summary> */
        AmbientInteractive,
         /** <summary>    responds to sound. </summary> */
        SoundResponsive,
         /** <summary>    only responds to interactivity. </summary> */
        Interactive,
         /** <summary>    responds to sound and interactivity. </summary> */
        InteractiveSoundResponsive,
         /** <summary>    still in development. </summary> */
        Experimental
    }

    /**
     * <summary>    The DriverType attribute is used to tag a class at compile time, drivers that
     *              connect over the network will need to supply this information when they connect. </summary>
     */

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DriverType : Attribute
    {

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        private readonly DriverTypes type;

        /**
         * <summary>    This is a positional argument. </summary>
         *
         * <param name="type">  The type. </param>
         */

        public DriverType(DriverTypes type) 
        { 
            this.type = type;     
        }

        /**
         * <summary>    Gets the type. </summary>
         *
         * <value>  The type. </value>
         */

        public DriverTypes Type
        {
            get { return type; }
        }
    }
}
