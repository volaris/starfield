using System;
using Starfield;

namespace Starfield
{
    /**
     * <summary>    Interface for Starfield drivers. Implement this interface to create a new animation.</summary>
     */

    public interface IStarfieldDriver
    {
        /**
         * <summary>    Renders the given Starfield. Called at every render iteration.</summary>
         *
         * <param name="Starfield"> The Starfield model to render to. </param>
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

    /**
     * <summary>    This class is used to create named instances for configurable sets of Starfield animations that can use multiple instances of a single driver type with different settings. </summary>
     *
     * <remarks>    Volar, 2/10/2017. </remarks>
     */

    public class CustomDriver
    {
        /** <summary>    The instance of the driver used to render the animation. </summary> */
        public IStarfieldDriver Driver;
        /** <summary>    The name of this instance. </summary> */
        public String Name;

        /**
         * <summary>    Returns the name of this object. </summary>
         *
         * <returns>    The name of object. </returns>
         */

        public override string ToString()
        {
            return Name;
        }
    }
}
