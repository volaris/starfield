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
            List<Endpoint> failed = new List<Endpoint>();
            List<Endpoint> tempFailed = new List<Endpoint>();
            DateTime lastConnect = DateTime.Now;
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
                try
                {
                    outputs.Add(new TcpClient(host.ip, host.port));
                }
                catch
                {
                    Console.WriteLine("Failed to connect: {0}:{1}", host.ip, host.port);
                    failed.Add(host);
                }
            }

            inputStream.Start();

            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = inputStream.AcceptTcpClient();
                Console.WriteLine("Connected!");

                foreach(Endpoint failedHost in failed)
                {
                    lastConnect = DateTime.Now;
                    try
                    {
                        outputs.Add(new TcpClient(failedHost.ip, failedHost.port));
                    }
                    catch
                    {
                        tempFailed.Add(failedHost);
                    }
                }

                failed.Clear();
                foreach(Endpoint host in tempFailed)
                {
                    failed.Add(host);
                }
                tempFailed.Clear();

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                try
                {
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        List<TcpClient> toRemove = new List<TcpClient>();

                        foreach (TcpClient outClient in outputs)
                        {
                            try
                            {
                                NetworkStream outStream = outClient.GetStream();
                                stream.Write(bytes, 0, i);
                            }
                            catch
                            {
                                Endpoint host = new Endpoint();
                                host.ip = ((System.Net.IPEndPoint)outClient.Client.RemoteEndPoint).Address.ToString();
                                host.port = ((System.Net.IPEndPoint)outClient.Client.RemoteEndPoint).Port;
                                failed.Add(host);
                                toRemove.Add(outClient);
                            }
                        }

                        foreach (TcpClient remove in toRemove)
                        {
                            outputs.Remove(remove);
                        }

                        if (failed.Count > 0 && (DateTime.Now - lastConnect).TotalSeconds > 5)
                        {
                            foreach (Endpoint failedHost in failed)
                            {
                                lastConnect = DateTime.Now;
                                try
                                {
                                    outputs.Add(new TcpClient(failedHost.ip, failedHost.port));
                                }
                                catch
                                {
                                    tempFailed.Add(failedHost);
                                }
                            }

                            failed.Clear();
                            foreach (Endpoint host in tempFailed)
                            {
                                failed.Add(host);
                            }
                            tempFailed.Clear();
                        }
                    }
                }
                catch
                { }

                // Shutdown and end connection
                client.Close();
            }
        }
    }
}
