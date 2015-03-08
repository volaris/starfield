using StarfieldClient;

namespace StarfieldClient
{
    public interface IStarfieldDriver
    {
        // called at every render iteration
        void Render(StarfieldModel Starfield);
        // called to initialize the driver when it is selected
        void Start(StarfieldModel Starfield);
        // called when another driver is selected
        void Stop();
    }
}
