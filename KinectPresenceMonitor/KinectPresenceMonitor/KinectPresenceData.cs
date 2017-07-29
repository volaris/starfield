using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace KinectPresenceMonitor
{
    class KinectPresenceData
    {
        public KinectSensor Sensor { get; set; }
        public List<Body> Bodies { get; set; }
        public DateTime Time;

        public KinectPresenceData()
        {
            this.Bodies = new List<Body>();
            this.Time = DateTime.Now;
        }
    }
}
