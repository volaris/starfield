using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfield
{
    // defines the general classes of drivers the Starfield supports
    // this is used to allow a driver program to select a random driver for the appropriate time
    public enum DriverTypes
    {
        Ambient,            // just an animation
        AmbientInteractive, // an animation that responds to input, will work as an ambient animation
        SoundResponsive,    // responds to sound
        Interactive,        // only responds to interactivity
        Experimental        // still in development
    }

    // the DriverType attribute is used to tag a class at compile time, drivers that connect over the 
    // network will need to supply this information when they connect
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DriverType : Attribute
    {

        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        private readonly DriverTypes type;
    
        // This is a positional argument
        public DriverType(DriverTypes type) 
        { 
            this.type = type;     
        }
   
        public DriverTypes Type
        {
            get { return type; }
        }
    }
}
