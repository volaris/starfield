using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using ProtoBuf;

namespace KinectPresenceMonitor
{
    class KinectMonitor
    {
        private KinectSensor kinectSensor = null;
        private CoordinateMapper coordinateMapper = null;
        private BodyFrameReader bodyFrameReader = null;
        private List<Tuple<JointType, JointType>> bones;
        private Body[] bodies = null;

        public KinectMonitor()
        {
        }

        public void Initialize()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.IsAvailableChanged;

            // set up the bodyFrameReader callback for when new frames arrive
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += this.FrameArrived;

            // open the sensor
            this.kinectSensor.Open();
            Console.WriteLine("IsOpen: " + this.kinectSensor.IsOpen);

            // connect to central starfield server
            ConnectToStarfield();

            Console.WriteLine("Kinect monitor initialized");
        }

        private void ConnectToStarfield()
        {
            // TODO: implement me
        }

        private void IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            Console.WriteLine("IsAvailable: " + this.kinectSensor.IsAvailable);
        }

        public void FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                KinectPresenceData bodyData = new KinectPresenceData();
                bodyData.PresenceList = new List<PresenceNode>();

                foreach (Body body in this.bodies)
                {
                    // Throw away body 0
                    if (body.TrackingId == 0)
                    {
                        continue;
                    }

                    // Process an actual body
                    Console.WriteLine(body.ToString());
                    CameraSpacePoint center = GetBodyCentroid(body);
                    Console.WriteLine("\t" + center.X + "," + center.Y + "," + center.Z);

                    // Add the body to the growing list of presence data
                    bodyData.PresenceList.Add(new PresenceNode() { x = center.X, y = center.Y, z = center.Z } );
                }

                // Pass the data along to the destination server
                TransmitBodyData(bodyData);
            }
        }

        private void TransmitBodyData(KinectPresenceData bodyData)
        {
            // TODO: implement me
        }

        private CameraSpacePoint GetBodyCentroid(Body body)
        {
            float xCenter = 0;
            float yCenter = 0;
            float zCenter = 0;

            var joints = body.Joints;
            int jointCount = joints.Values.Count();
            foreach (var joint in joints.Values)
            {
                var jointPoint = joint.Position;
                xCenter += jointPoint.X;
                yCenter += jointPoint.Y;
                zCenter += jointPoint.Z;
            }

            CameraSpacePoint center = new CameraSpacePoint();
            center.X = xCenter / jointCount;
            center.Y = yCenter / jointCount;
            center.Z = zCenter / jointCount;
            return center;
        }
    }
}
