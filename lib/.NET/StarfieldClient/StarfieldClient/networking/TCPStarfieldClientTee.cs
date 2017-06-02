using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;

namespace Starfield.Networking
{
    public class TCPStarfieldClientTee
    {
        private StarfieldModel model;
        private OPCClient client;
        private byte[] pixelData;
        private Timer flush;
        List<OPCClient> clients;

        // Flush lock: this ensures that only one thread is sending data to
        // the server at a time. If we don't do this, it can cause the data
        // stream to get jumbled.
        private Object lockObject = new Object();

        /** <summary>    interval between flushing pixel data to the server. </summary> */
        public ulong AnimationInterval = 30;

        /**
         * <summary>    Constructor. Creates a set of OPC connections to arbitrary endpoints</summary>
         *
         * <param name="starfield"> The starfield Model. </param>
         * <param name="servers">        The endpoints. </param>
         */
        TCPStarfieldClientTee(StarfieldModel starfield, List<IPEndPoint> servers)
        {
            clients = new List<OPCClient>();
            foreach(IPEndPoint endpoint in servers)
            {
                OPCClient client = new OPCClient(endpoint);
                clients.Add(client);
                if (!client.CanConnect())
                {
                    Console.WriteLine("couldn't connect");
                }
            }

            this.model = starfield;

            pixelData = new byte[this.model.NumX * this.model.NumY * this.model.NumZ * 3];
            
            flush = new Timer(AnimationInterval);
            flush.Elapsed += flush_Elapsed;
            flush.Start();
        }


        // flush the current color state to the display
        void flush_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(lockObject, ref lockTaken);

            if (lockTaken)
            {
                StarfieldOPC.PackPixels(this.model, ref this.pixelData);

                // send it
                foreach (OPCClient client in clients)
                {
                    client.PutPixels(0, pixelData);
                }
            }
            else
            {
                // another thread was still flushing
                Console.WriteLine("Frame Dropped");
            }

            if (lockTaken)
            {
                System.Threading.Monitor.Exit(lockObject);
            }
        }

        /** <summary>    Stops this object. </summary> */
        public void Stop()
        {
            flush.Enabled = false;
            foreach(OPCClient client in clients)
            {
                client.Disconnect();
            }
        }

        public static TCPStarfieldClientTee CriticalTee(StarfieldModel model)
        {
            List<IPEndPoint> endpoints = new List<IPEndPoint>();
            endpoints.Add(new IPEndPoint(IPAddress.Parse("192.168.0.50"), 7890));
            endpoints.Add(new IPEndPoint(IPAddress.Parse("192.168.0.51"), 7890));
            endpoints.Add(new IPEndPoint(IPAddress.Parse("192.168.0.52"), 7890));
            endpoints.Add(new IPEndPoint(IPAddress.Parse("192.168.0.53"), 7890));
            endpoints.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7890));
            return new TCPStarfieldClientTee(model, endpoints);
        }
    }
}
