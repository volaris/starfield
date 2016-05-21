using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;

namespace StreamReplicator
{
    class Endpoint
    {
        public string ip;
        public int port;
    }

    class Config
    {
        public int port;
        public Endpoint[] host_list;
    }

    class Program
    {
        static void Main(string[] args)
        {
            TcpListener inputStream;
            List<TcpClient> outputs = new List<TcpClient>();
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            string path = @"stream_replicator.json";
            if(args.Length > 1)
            {
                path = args[1];
            }
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));

            inputStream = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), config.port);

            foreach(Endpoint host in config.host_list)
            {
                outputs.Add(new TcpClient(host.ip, host.port));
            }

            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = inputStream.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    foreach (TcpClient outClient in outputs)
                    {
                        NetworkStream outStream = outClient.GetStream();
                        stream.Write(bytes, 0, i);
                    }
                }

                // Shutdown and end connection
                client.Close();
            }
        }
    }
}
