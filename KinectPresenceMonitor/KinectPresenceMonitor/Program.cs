using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KinectPresenceMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            KinectMonitor km = new KinectMonitor();

            // Prepare Kinect and setup callbacks
            km.Initialize();

            // Suspend while we're driven by callback events
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
