using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KinectPresenceMonitor
{
    class KinectMonitor
    {
        private KinectSensor kinectSensor;
        private CoordinateMapper coordinateMapper;
        private BodyFrameReader bodyFrameReader;
        private AudioBeamFrameReader audioBeamFrameReader;
        private Body[] bodies;

        private TcpListener bodyServer;
        private TcpListener audioServer;

        private object bodyClientsLock = new object();
        private object audioClientsLock = new object();

        List<Socket> bodyClients = new List<Socket>();
        List<Socket> audioClients = new List<Socket>();

        FileStream bodyLog = null;
        FileStream audioLog = null;

        /// <summary>
        /// Number of samples captured from Kinect audio stream each millisecond.
        /// </summary>
        private const int SamplesPerMillisecond = 16;

        /// <summary>
        /// Number of bytes in each Kinect audio stream sample (32-bit IEEE float).
        /// </summary>
        private const int BytesPerSample = sizeof(float);

        /// <summary>
        /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
        /// </summary>
        private const int MinEnergy = -90;

        /// <summary>
        /// Will be allocated a buffer to hold a single sub frame of audio data read from audio stream.
        /// </summary>
        private byte[] audioBuffer = null;

        public KinectMonitor()
        {
            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            bodyLog = File.Open("body.log", FileMode.OpenOrCreate, FileAccess.Write);
            audioLog = File.Open("audio.log", FileMode.OpenOrCreate, FileAccess.Write);

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // get the depth (display) extents
            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.audioBuffer = new byte[this.kinectSensor.AudioSource.SubFrameLengthInBytes];

            // Set the TcpListeners
            Int32 bodyPort = 13000;
            Int32 audioPort = 13001;
            IPAddress localAddr = IPAddress.Parse("0.0.0.0");
            
            bodyServer = new TcpListener(localAddr, bodyPort);
            audioServer = new TcpListener(localAddr, audioPort);

            // Start listening for client requests.
            bodyServer.Start();
            audioServer.Start();

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.IsAvailableChanged;

            // set up the bodyFrameReader callback for when new frames arrive
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += this.FrameArrived;

            // set up the audioBeamFrameReader callback for when new frames arrive
            this.audioBeamFrameReader = this.kinectSensor.AudioSource.OpenReader();
            this.audioBeamFrameReader.FrameArrived += AudioBeamFrameReader_FrameArrived;

            // open the sensor
            this.kinectSensor.Open();
            Console.WriteLine("IsOpen: " + this.kinectSensor.IsOpen);

            Console.WriteLine("Kinect monitor initialized");
        }

        private void AudioBeamFrameReader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            AudioBeamFrameList frameList = e.FrameReference.AcquireBeamFrames();

            if (frameList != null)
            {
                // AudioBeamFrameList is IDisposable
                using (frameList)
                {
                    AudioBeam beam = frameList[0].AudioBeam;
                    IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;
                    
                    float accumulatedSquareSum = 0;
                    int accumulatedSampleCount = 0;

                    KinectAudioData audioData = new KinectAudioData();

                    // Loop over all sub frames, extract audio buffer and beam information
                    foreach (AudioBeamSubFrame subFrame in subFrameList)
                    {
                        float beamAngle = subFrame.BeamAngle;
                        float beamAngleConfidence = subFrame.BeamAngleConfidence;

                        KinectAudioSubframeData subFrameData = new KinectAudioSubframeData();

                        // Process audio buffer
                        subFrame.CopyFrameDataToArray(this.audioBuffer);

                        for (int i = 0; i < this.audioBuffer.Length; i += BytesPerSample)
                        {
                            // Extract the 32-bit IEEE float sample from the byte array
                            float audioSample = BitConverter.ToSingle(this.audioBuffer, i);
                            subFrameData.Samples.Add(audioSample);

                            accumulatedSquareSum += audioSample * audioSample;
                            ++accumulatedSampleCount;
                        }

                        float meanSquare = accumulatedSquareSum / accumulatedSampleCount;

                        if (meanSquare > 1.0f)
                        {
                            // A loud audio source right next to the sensor may result in mean square values
                            // greater than 1.0. Cap it at 1.0f for display purposes.
                            meanSquare = 1.0f;
                        }

                        // Calculate energy in dB, in the range [MinEnergy, 0], where MinEnergy < 0
                        float energy = MinEnergy;

                        if (meanSquare > 0)
                        {
                            energy = (float)(10.0 * Math.Log10(meanSquare));
                        }

                        subFrameData.Energy = energy;
                        audioData.SubFrames.Add(subFrameData);
                    }

                    TransmitAudioData(audioData);
                    SaveAudioData(audioData);
                }
            }
        }

        private void SaveAudioData(KinectAudioData audioData)
        {
            byte[] data = SerializeObjectToBSON<KinectAudioData>(audioData);
            audioLog.Write(data, 0, data.Length);
            audioLog.Flush();
        }

        private void TransmitAudioData(KinectAudioData audioData)
        {
            lock (audioClientsLock)
            {
                byte[] data = SerializeObjectToBSON<KinectAudioData>(audioData);
                
                audioClients.RemoveAll(client => client.Connected == false);

                foreach (Socket client in audioClients)
                {
                    if (client.Connected)
                    {
                        client.Send(data);
                    }
                }
            }
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
                bodyData.Sensor = this.kinectSensor;

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

                    HandState leftHand = body.HandLeftState;
                    HandState rightHand = body.HandRightState;

                    Console.Write("\tLeft Hand: ");
                    switch (leftHand)
                    {
                        case HandState.Closed:
                            Console.Write("Closed");
                            break;
                        case HandState.Lasso:
                            Console.Write("Lasso");
                            break;
                        case HandState.Open:
                            Console.Write("Open");
                            break;
                        default:
                            Console.Write("N/A");
                            break;
                    }
                    Console.Write("\n");

                    Console.Write("\tRight Hand: ");
                    switch (rightHand)
                    {
                        case HandState.Closed:
                            Console.Write("Closed");
                            break;
                        case HandState.Lasso:
                            Console.Write("Lasso");
                            break;
                        case HandState.Open:
                            Console.Write("Open");
                            break;
                        default:
                            Console.Write("N/A");
                            break;
                    }
                    Console.Write("\n");
                    
                    // Add the body to the growing list of presence data
                    bodyData.Bodies.Add(body);
                }
                
                // Pass the data along to the destination server
                TransmitBodyData(bodyData);
                SaveBodyData(bodyData);
            }
        }

        private void SaveBodyData(KinectPresenceData bodyData)
        {
            byte[] data = SerializeObjectToBSON<KinectPresenceData>(bodyData);
            bodyLog.Write(data, 0, data.Length);
            bodyLog.Flush();
        }

        private void TransmitBodyData(KinectPresenceData bodyData)
        {
            lock (bodyClientsLock)
            {
                byte[] data = SerializeObjectToBSON<KinectPresenceData>(bodyData);

                bodyClients.RemoveAll(client => client.Connected == false);
            
                foreach (Socket client in bodyClients)
                {
                    if (client.Connected)
                    {
                        client.Send(data);
                    }
                }
            }
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

        public void Run()
        {
            var bodyServerThread = new Thread(() =>
            {
                while (true)
                {
                    Socket client = bodyServer.AcceptSocket();
                    lock (bodyClientsLock)
                    {
                        bodyClients.Add(client);
                    }
                }
            });

            var audioServerThread = new Thread(() =>
            {
                while (true)
                {
                    Socket client = audioServer.AcceptSocket();
                    lock (audioClientsLock)
                    {
                        audioClients.Add(client);
                    }
                }
            });

            bodyServerThread.Start();
            audioServerThread.Start();

            bodyServerThread.Join();
        }

        private byte[] SerializeObjectToBSON<T>(T obj)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //serializer.TypeNameHandling = TypeNameHandling.Objects; // uncomment this line to directly deserialize into known objects
                serializer.Serialize(writer, obj);
            }
            return ms.ToArray();
        }
    }
}
