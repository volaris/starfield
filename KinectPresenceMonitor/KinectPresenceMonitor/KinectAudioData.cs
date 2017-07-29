using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPresenceMonitor
{
    public class KinectAudioSubframeData
    {
        public float Energy = 0.0f;
        public List<float> Samples = new List<float>();
    }

    public class KinectAudioData
    {
        public List<KinectAudioSubframeData> SubFrames = new List<KinectAudioSubframeData>();
        public DateTime Time;

        public KinectAudioData()
        {
            Time = DateTime.Now;
        }
    }
}
