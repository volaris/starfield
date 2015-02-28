using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using Leap;

namespace StarfieldClient
{
    public class StarfieldModel
    {
        // these should match the values for the simulator or the starfield
        private const float DEFAULT_X_STEP = 2;
        private const float DEFAULT_Y_STEP = 2;
        private const float DEFAULT_Z_STEP = 2;
        private const ulong DEFAULT_NUM_X = 7;//16;//32;
        private const ulong DEFAULT_NUM_Y = 4;//7;//14;
        private const ulong DEFAULT_NUM_Z = 5;//16;//32;

        public float X_STEP = 2;
        public float Y_STEP = 2;
        public float Z_STEP = 2;
        public ulong NUM_X = 7;//16;//32;
        public ulong NUM_Y = 4;//7;//14;
        public ulong NUM_Z = 5;//16;//32;
        public ulong ANIMATION_INTERVAL = 30;
        Timer dimmer = new Timer(3000);
        Timer flush;
        public bool EnableDimmer = false;
        OPCClient client;
        byte[] pixelData;
        Object lockObject = new Object();

        public static Color[, ,] LEDColors;

        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ, System.Net.IPAddress ip, int port)
        {
            this.X_STEP = xStep;
            this.Y_STEP = yStep;
            this.Z_STEP = zStep;
            this.NUM_X = numX;
            this.NUM_Y = numY;
            this.NUM_Z = numZ;

            LEDColors = new Color[NUM_X, NUM_Z, NUM_Y];
            pixelData = new byte[NUM_X * NUM_Y * NUM_Z * 3];

            client = new OPCClient(ip, port);
            if (!client.CanConnect())
            {
                Console.WriteLine("couldn't connect");
            }
            dimmer.Elapsed += dimmer_Elapsed;
            dimmer.Start();

            flush = new Timer(ANIMATION_INTERVAL);
            flush.Elapsed += flush_Elapsed;
            flush.Start();
        }

        public StarfieldModel()
            : this(DEFAULT_X_STEP, 
                   DEFAULT_Y_STEP,
                   DEFAULT_Z_STEP, 
                   DEFAULT_NUM_X,
                   DEFAULT_NUM_Y, 
                   DEFAULT_NUM_Z, 
                   System.Net.IPAddress.Loopback, 
                   7890)
        {
        }

        public StarfieldModel(System.Net.IPAddress ip, int port) : this(2, 2, 2, 7, 4, 5, ip, port)
        {
        }

        void flush_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(lockObject, ref lockTaken);

            if (lockTaken)
            {
                // pack the array
                for (ulong i = 0; i < (ulong)(pixelData.Length / 3); i++)
                {
                    ulong x = i / (NUM_Z * NUM_Y);
                    ulong z = (i % (NUM_Z * NUM_Y)) / NUM_Y;
                    ulong y = (NUM_Y - 1) - ((i % (NUM_Z * NUM_Y)) % NUM_Y);

                    pixelData[3 * i] = LEDColors[x, z, y].R;
                    pixelData[(3 * i) + 1] = LEDColors[x, z, y].G;
                    pixelData[(3 * i) + 2] = LEDColors[x, z, y].B;
                }

                // send it
                client.PutPixels(0, pixelData);
            }
            else
            {
                Console.WriteLine("Frame Dropped");
            }

            if (lockTaken)
            {
                System.Threading.Monitor.Exit(lockObject);
            }
        }

        void dimmer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (EnableDimmer)
            {
                for (ulong x = 0; x < NUM_X; x++)
                {
                    for (ulong y = 0; y < NUM_Y; y++)
                    {
                        for (ulong z = 0; z < NUM_Z; z++)
                        {
                            byte red = (byte)(LEDColors[x, z, y].R * .94);
                            byte green = (byte)(LEDColors[x, z, y].G * .94);
                            byte blue = (byte)(LEDColors[x, z, y].B * .94);
                            LEDColors[x, z, y] = Color.FromArgb(red, green, blue);
                        }
                    }
                }
            }
        }

        // TODO: pull this out of this class
        public void DrawAt(Vector brushPosition, Frame frame)
        {
            // normalize location
            InteractionBox box = frame.InteractionBox;
            Vector normalizedPoint = box.NormalizePoint(brushPosition);

            // convert leap coordinates to starfield coordinates
            Vector starfieldPoint = new Vector(normalizedPoint.x * (NUM_X - 1), normalizedPoint.y * (NUM_Y - 1), ((1.0f - normalizedPoint.z) * (NUM_Z - 1)));

            // convert starfield coordinates to the closest LED and draw it
            LEDColors[(int)Math.Round(starfieldPoint.x), (int)Math.Round(starfieldPoint.z), (int)Math.Round(starfieldPoint.y)] = Color.FromArgb(0, 0xFF, 0);
        }

        public void Clear()
        {
            for (ulong x = 0; x < NUM_X; x++)
            {
                for (ulong y = 0; y < NUM_Y; y++)
                {
                    for (ulong z = 0; z < NUM_Z; z++)
                    {
                        byte red = 0;
                        byte green = 0;
                        byte blue = 0;
                        LEDColors[x, z, y] = Color.FromArgb(red, green, blue);
                    }
                }
            }
        }

        public void SetColor(int x, int y, int z, Color color)
        {
            LEDColors[x, z, y] = color;
        }

        public Color GetColor(int x, int y, int z)
        {
            return LEDColors[x, z, y];
        }
    }
}
