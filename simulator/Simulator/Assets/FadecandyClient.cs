using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Timers;

public class FadecandyClient : MonoBehaviour {
	public const int port = 7890;
	private Thread serverThread;
	static TcpListener listener;
	OPCClient client;
	public const ulong ANIMATION_INTERVAL = 30;
	System.Timers.Timer dimmer = new System.Timers.Timer(300);
	System.Timers.Timer flush = new System.Timers.Timer(ANIMATION_INTERVAL);

	private enum OPCCommands
	{
		SetPixelColors = 0,
		SystemExclusive = 255
	}

	// Use this for initialization
	void Start () 
	{
		client = new OPCClient();
		if (!client.CanConnect())
		{
			Debug.Log("couldn't connect");
		}
		dimmer.Elapsed += dimmer_Elapsed;
		dimmer.Start();
		
		flush.Elapsed += flush_Elapsed;
		flush.Start();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void flush_Elapsed(object sender, ElapsedEventArgs e)
	{
		byte[] pixelData = new byte[StarfieldGenerator.NumX * StarfieldGenerator.NumY * StarfieldGenerator.NumZ * 3];
		
		// pack the array
		for (ulong i = 0; i < (ulong)StarfieldGenerator.LEDColors.Length; i++)
		{
			Color32 color = StarfieldGenerator.GetLEDColor(i);

			pixelData[3 * i] = color.r;
			pixelData[(3 * i) + 1] = color.g;
			pixelData[(3 * i) + 2] = color.b;
			//if (LEDColors[x, z, y].R > 0 || LEDColors[x, z, y].G > 0 || LEDColors[x, z, y].B > 0)
			//{
			//    Console.WriteLine("[{0:X} {1:X} {2:X}]", pixelData[3 * i], pixelData[(3 * i) + 1], pixelData[(3 * i) + 2]);
			//}
		}
		
		// send it
		client.PutPixels(0, pixelData);
	}
	
	void dimmer_Elapsed(object sender, ElapsedEventArgs e)
	{
		for (ulong x = 0; x < StarfieldGenerator.NumX; x++)
		{
			for (ulong y = 0; y < StarfieldGenerator.NumY; y++)
			{
				for (ulong z = 0; z < StarfieldGenerator.NumZ; z++)
				{
					byte red = (byte)(StarfieldGenerator.LEDColors[x, z, y].r * .94);
					byte green = (byte)(StarfieldGenerator.LEDColors[x, z, y].r * .94);
					byte blue = (byte)(StarfieldGenerator.LEDColors[x, z, y].r * .94);
					StarfieldGenerator.LEDColors[x, z, y] = new Color32(red, green, blue, 0xFF);
					//if (LEDColors[x, z, y].R > 0 || LEDColors[x, z, y].G > 0 || LEDColors[x, z, y].B > 0)
					//{
					//    Console.WriteLine(LEDColors[x, z, y]);
					//}
				}
			}
		}
	}
}

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
			return false;
		}
	}
	
	public void PutPixels(byte channel, byte[] pixels)
	{
		if (client.Connected)
		{
			BinaryWriter writer = new BinaryWriter(client.GetStream());
			writer.Write(channel);
			writer.Write((byte)0); // Set pixel colours
			//writer.Write((ushort)pixels.Length);
			writer.Write((byte)((pixels.Length & 0xFF00) >> 8));
			writer.Write((byte)(pixels.Length & 0x00FF));
			writer.Write(pixels);
		}
	}
}
