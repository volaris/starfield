using Starfield;

namespace Starfield
{
    public interface IStarfieldDriver
    {
        // called at every render iteration
        void Render(StarfieldModel Starfield);
        // called to initialize the driver when it is selected
        // use the constructor to initialize state that will remain initialized
        // for the life of the client, use this initialize state that should
        // only be initialized while this driver is running or needs to be
        // reset when this driver takes over
        void Start(StarfieldModel Starfield);
        // called when another driver is selected
        // clean up anything that should not be running while another driver
        // is controlling the starfield
        void Stop();
    }
}
