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
        void Render(StarfieldModel Starfield); // called at every render iteration
        Panel GetConfigPanel();
        void ApplyConfig();
        void Start(StarfieldModel Starfield);
        void Stop();
    }
}
