using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace Starfield.Networking
{
    class TCPStarfieldServer
    {
        public const int DefaultPort = 7890;
        private Thread serverThread;
        private TcpListener listener;
        private bool stop = false;
        private StarfieldModel model;

        public StarfieldModel Starfield
        {
            get { return model; }
        }

        private enum OPCCommands
        {
            SetPixelColors = 0,
            SystemExclusive = 255
        }

        public TCPStarfieldServer(StarfieldModel model)
            : this(model, TCPStarfieldServer.DefaultPort)
        {
        }

        public TCPStarfieldServer(StarfieldModel model, int port)
        {
            this.model = model;
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            serverThread = new Thread(new ThreadStart(Service));
            serverThread.Start();
        }

        private void Service()
        {
            while (!stop)
            {
                Socket soc = listener.AcceptSocket();
                try
                {
                    Stream stream = new NetworkStream(soc);
                    BinaryReader reader = new BinaryReader(stream);

                    while (!stop)
                    {
                        byte[] header = new byte[4];
                        byte[] message;
                        byte channel;
                        byte command;
                        ushort length;
                        header = reader.ReadBytes(4);
                        channel = header[0];
                        command = header[1];
                        length = (ushort)(((ushort)header[2]) << 8);
                        length |= header[3];
                        
                        message = reader.ReadBytes(length);

                        HandlePacket(channel, command, message);
                    }
                }
                catch (Exception)
                {
                }
                soc.Close();
            }
        }

        void HandlePacket(byte channel, byte command, byte[] data)
        {
            switch (command)
            {
                case (byte)OPCCommands.SetPixelColors:
                {
                    int numPixels = data.Length / 3;
                    for (int i = 0; i < numPixels && i < (int)(model.NumX * model.NumY * model.NumZ); i++)
                    {
                        byte red = data[3 * i];
                        byte green = data[(3 * i) + 1];
                        byte blue = data[(3 * i) + 2];

                        ulong x = (ulong)i / (model.NumZ * model.NumY);
                        ulong z = ((ulong)i % (model.NumZ * model.NumY)) / model.NumY;
                        ulong y = (model.NumY - 1) - ((ulong)i % (model.NumZ * model.NumY)) % model.NumY;

                        model.SetColor((int)x, (int)y, (int)z, Color.FromArgb(red, green, blue));
                    }
                    break;
                }
            }
        }

        public void Stop()
        {
            stop = true;
            listener.Stop();
            serverThread.Abort();
        }
    }
}
