using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StarfieldClient;

namespace AlgorithmDemo.Drivers
{
    interface IStarfieldDriver
    {
        // called at every render iteration
        void Render(StarfieldModel Starfield);
        // called to initialize the driver when it is selected
        void Start(StarfieldModel Starfield);
        // called when another driver is selected
        void Stop();
    }
}
