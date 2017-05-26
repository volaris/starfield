using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPresenceMonitor
{
    [ProtoContract]
    class KinectPresenceData
    {
        [ProtoMember(1)]
        public int SourceID { get; set; }
        [ProtoMember(2)]
        public List<PresenceNode> PresenceList { get; set; }
    }

    [ProtoContract]
    class PresenceNode
    {
        [ProtoMember(3)]
        public double x { get; set; }
        [ProtoMember(4)]
        public double y { get; set; }
        [ProtoMember(5)]
        public double z { get; set; }
        [ProtoMember(6)]
        public double height { get; set; }
        [ProtoMember(7)]
        public double width { get; set; }
    }
}
