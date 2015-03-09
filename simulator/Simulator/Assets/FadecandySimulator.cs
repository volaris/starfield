using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class FadecandySimulator : MonoBehaviour {
	public const int port = 7890;
	private Thread serverThread;
	static TcpListener listener;
	static bool stop = false;

	private enum OPCCommands
	{
		SetPixelColors = 0,
		SystemExclusive = 255
	}

	// Use this for initialization
	void Start () 
	{
		listener = new TcpListener(IPAddress.Loopback, port);
		listener.Start();
		serverThread = new Thread(new ThreadStart(Service));
		serverThread.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnApplicationQuit()
	{
		stop = true;
		listener.Stop();
		serverThread.Abort();
	}

	static void Service()
	{
		while(!stop)
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

					//Debug.Log (string.Format("{0:X} {1:X} {2:X} {3:X}", header[0], header[1], header[2], header[3]));

					message = reader.ReadBytes(length);

					HandlePacket(channel, command, message);
				}
			}
			catch(System.Exception e)
			{
				Debug.LogException(e);
			}
			soc.Close();
		}
	}

	static void HandlePacket(byte channel, byte command, byte[] data)
	{
		//Debug.Log(string.Format("New Packet: 0x{0:X} 0x{1:X} Len:{2},0x{2:X}", channel, command, data.Length));
		switch(command)
		{
			case (byte)OPCCommands.SetPixelColors:
			{
				int numPixels = data.Length / 3;
				for(int i = 0; i < numPixels && i < StarfieldGenerator.LEDs.Length; i++)
				{
					byte red = data[3 * i];
					byte green = data[(3 * i) + 1];
					byte blue = data[(3 * i) + 2];
					StarfieldGenerator.SetLEDColor((ulong)i, new Color32(red, green, blue, 0xFF));
				}
				break;
			}
		}
	}
}
