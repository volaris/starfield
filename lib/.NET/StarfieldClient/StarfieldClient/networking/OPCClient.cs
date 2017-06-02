using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Starfield.Networking
{
    class OPCClient
    {
        IPEndPoint dest = null;
        IPAddress ip = IPAddress.Loopback;
        int port = 7890;
        TcpClient client;
        bool connecting = false;

        /**
         * <summary>    Constructor. Creates an OPC connection to 127.0.0.1:7890</summary>
         */
        public OPCClient()
        {
            dest = new IPEndPoint(ip, port);
            client = new TcpClient();
        }

        /**
         * <summary>    Constructor. Creates an OPC connection to an arbitrary endpoint</summary>
         *
         * <param name="ip">        The IP. </param>
         * <param name="port">      The port. </param>
         */
        public OPCClient(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            dest = new IPEndPoint(ip, port);
            client = new TcpClient();
        }

        /**
         * <summary>    Constructor. Creates an OPC connection to an arbitrary endpoint</summary>
         *
         * <param name="endpoint">        The endpoint. </param>
         */
        public OPCClient(IPEndPoint endpoint)
        {
            this.ip = endpoint.Address;
            this.port = endpoint.Port;
            dest = new IPEndPoint(ip, port);
            client = new TcpClient();
        }

        // attempt to connect to the server, returns true if successful
        public bool CanConnect()
        {
            if (client.Connected)
            {
                return true;
            }
            
            if(connecting)
            {
                return false;
            }

            try
            {
                client.BeginConnect(dest.Address, dest.Port, ConnectCallback, this);
                connecting = true;
                return false;
            }
            catch
            {
                // We may have had a connection that got disconnected. Reset 
                // the client object. If we were connected, Connect will 
                // fail without this.
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

        public void Disconnect()
        {
            client.Close();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                TcpClient client = ((OPCClient)ar.AsyncState).client;

                // Complete the connection.  
                client.EndConnect(ar);

                ((OPCClient)ar.AsyncState).connecting = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
