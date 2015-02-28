using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StarfieldClient
{
    class OPCClient
    {
        IPEndPoint dest = null;
        IPAddress ip = IPAddress.Loopback;
        int port = 7890;
        TcpClient client;

        public OPCClient()
        {
            dest = new IPEndPoint(ip, port);
            client = new TcpClient();
        }

        public OPCClient(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            dest = new IPEndPoint(ip, port);
            client = new TcpClient();
        }

        public bool CanConnect()
        {
            if (client.Connected)
            {
                return true;
            }

            try
            {
                client.Connect(dest);
                return true;
            }
            catch
            {
                // Reset the client object. If we were connected, Connect will fail without this.
                client = new TcpClient();
                try
                {
                    client.Connect(dest);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void PutPixels(byte channel, byte[] pixels)
        {
            try
            {
                if (client.Connected || CanConnect())
                {
                    BinaryWriter writer = new BinaryWriter(client.GetStream());
                    // OPC header
                    writer.Write(channel);
                    writer.Write((byte)0); // Set pixel colours
                    writer.Write((byte)((pixels.Length & 0xFF00) >> 8));
                    writer.Write((byte)(pixels.Length & 0x00FF));
                    // Pixel Data
                    writer.Write(pixels);
                }
            }
            catch
            { }
        }
    }
}
