using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;

namespace StarfieldClient.Networking
{
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

        // interval between flushing pixel data to the server
        public ulong AnimationInterval = 30;

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
                // pack the array 
                // [Pixel0 Red, Pixel0 Green, Pixel0 Blue, Pixel1 Red, ..., PixelN Red, PixelN Green, PixelN Blue]
                for (ulong i = 0; i < (ulong)(pixelData.Length / 3); i++)
                {
                    ulong x = i / (this.model.NumZ * this.model.NumY);
                    ulong z = (i % (this.model.NumZ * this.model.NumY)) / this.model.NumY;
                    ulong y = (this.model.NumY - 1) - ((i % (this.model.NumZ * this.model.NumY)) % this.model.NumY);

                    pixelData[3 * i] = (byte)(this.model.GetColor((int)x, (int)y, (int)z).R);
                    pixelData[(3 * i) + 1] = (byte)(this.model.GetColor((int)x, (int)y, (int)z).G);
                    pixelData[(3 * i) + 2] = (byte)(this.model.GetColor((int)x, (int)y, (int)z).B);
                }

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

        public void Stop()
        {
            flush.Enabled = false;
            client.Disconnect();
        }
    }
}
