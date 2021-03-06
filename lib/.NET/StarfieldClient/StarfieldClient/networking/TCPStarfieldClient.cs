﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;
using Starfield;

namespace Starfield.Networking
{
    /** <summary>    A TCP starfield client. This class sends the model's data to the pixel drivers via TCP. </summary> */
    public class TCPStarfieldClient
    {
        private StarfieldModel model;
        private OPCClient client;
        private byte[] pixelData;
        private Timer flush;

        // Flush lock: this ensures that only one thread is sending data to
        // the server at a time. If we don't do this, it can cause the data
        // stream to get jumbled.
        private Object lockObject = new Object();

        /** <summary>    interval between flushing pixel data to the server. </summary> */
        public ulong AnimationInterval = 30;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="starfield"> The starfield Model. </param>
         * <param name="ip">        The IP. </param>
         * <param name="port">      The port. </param>
         */

        public TCPStarfieldClient(StarfieldModel starfield, IPAddress ip, int port)
        {
            this.model = starfield;

            pixelData = new byte[this.model.NumX * this.model.NumY * this.model.NumZ * 3];

            client = new OPCClient(ip, port);
            if (!client.CanConnect())
            {
                Console.WriteLine("couldn't connect");
            }

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
                client.PutPixels(0, pixelData);
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
            client.Disconnect();
        }
    }
}
