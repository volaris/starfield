using System;
using Starfield;

namespace Starfield
{
    /**
     * <summary>    Interface for starfield drivers. </summary>
     */

    public interface IStarfieldDriver
    {
        /**
         * <summary>    Renders the given Starfield. Called at every render iteration.</summary>
         *
         * <param name="Starfield"> The starfield model to render to. </param>
         */
        void Render(StarfieldModel Starfield);

        /**
         * <summary>    Starts the starfield driver. </summary>
         * <para>    Called to initialize the driver when it is selected 
         *           use the constructor to initialize state that will 
         *           remain initialized for the life of the client, use 
         *           this initialize state that should only be 
         *           initialized while this driver is running or needs 
         *           to be reset when this driver takes over</para>
         *
         * <param name="Starfield"> The starfield. </param>
         */
        void Start(StarfieldModel Starfield);

        /** <summary>    Stops the starfield driver. </summary>  
         *  <para>    Called when another driver is selected. Clean up 
         *            anything that should not be running while another 
         *            driver is controlling the starfield</para>
         */
        void Stop();
    }

    public class CustomDriver
    {
        public IStarfieldDriver Driver;
        public String Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
