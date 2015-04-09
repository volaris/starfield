using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
//using Leap;

namespace StarfieldClient
{
    public class StarfieldModel
    {
        // these should match the values for the simulator or the starfield
        private const float DEFAULT_X_STEP = 2;
        private const float DEFAULT_Y_STEP = 2;
        private const float DEFAULT_Z_STEP = 2;
        private const ulong DEFAULT_NUM_X = 7;
        private const ulong DEFAULT_NUM_Y = 4;
        private const ulong DEFAULT_NUM_Z = 5;

        // distance in feet between pixels
        public float XStep = 2;
        public float YStep = 2;
        public float ZStep = 2;

        // number of pixels
        public ulong NumX = 7;
        public ulong NumY = 4;
        public ulong NumZ = 5;

        // interval between flushing pixel data to the server
        public ulong AnimationInterval = 30;

        // change the maximum brightness of the starfield
        // this field's range is [0,1]
        float brightness = 1.0f;

        // set up the dimming timer
        Timer dimmer = new Timer(3000);
        Timer flush;
        public bool EnableDimmer = false;
        OPCClient client;
        byte[] pixelData;

        // Flush lock: this ensures that only one thread is sending data to
        // the server at a time. If we don't do this, it can cause the data
        // stream to get jumbled.
        Object lockObject = new Object();

        // current color state
        public static Color[, ,] LEDColors;

        public float Brightness
        {
            get { return brightness; }
            set { brightness = Math.Min(Math.Max(0f,value),1f); }
        }

        // create a client with the defaults for the home prototype
        public static StarfieldModel HomeStarfield(System.Net.IPAddress ip, int port)
        {
            // 12' x 8' x 7.5' spaced at 2' intervals
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 7, 4, 5, ip, port);
        }

        // create a client with the defaults for the Critical NW version
        public static StarfieldModel CriticalNWStarfield(System.Net.IPAddress ip, int port)
        {
            // 20' x 20' x 20' spaced at 2' intervals
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 11, 10, 11, ip, port);
        }

        // create a client with the defaults for the Burning Man version
        public static StarfieldModel BurningManStarfield(System.Net.IPAddress ip, int port)
        {
            // 60' x 60' x 30' spaced at 2' intervals on the y axis and 4' intervals on the x & z axes
            return new StarfieldModel(4.0f, 2.0f, 4.0f, 16, 15, 16, ip, port);
        }

        // create a client with arbitrary parameters
        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ, System.Net.IPAddress ip, int port)
        {
            this.XStep = xStep;
            this.YStep = yStep;
            this.ZStep = zStep;
            this.NumX = numX;
            this.NumY = numY;
            this.NumZ = numZ;

            LEDColors = new Color[NumX, NumZ, NumY];
            pixelData = new byte[NumX * NumY * NumZ * 3];

            client = new OPCClient(ip, port);
            if (!client.CanConnect())
            {
                Console.WriteLine("couldn't connect");
            }
            dimmer.Elapsed += dimmer_Elapsed;
            dimmer.Start();

            flush = new Timer(AnimationInterval);
            flush.Elapsed += flush_Elapsed;
            flush.Start();
        }

        // create a client with the global defaults
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

        // create a client with the default parameters but to an arbitrary endpoint
        public StarfieldModel(System.Net.IPAddress ip, int port)
            : this(DEFAULT_X_STEP,
                   DEFAULT_Y_STEP,
                   DEFAULT_Z_STEP,
                   DEFAULT_NUM_X,
                   DEFAULT_NUM_Y,
                   DEFAULT_NUM_Z, 
                   ip, 
                   port)
        {
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
                    ulong x = i / (NumZ * NumY);
                    ulong z = (i % (NumZ * NumY)) / NumY;
                    ulong y = (NumY - 1) - ((i % (NumZ * NumY)) % NumY);

                    pixelData[3 * i] = (byte)(LEDColors[x, z, y].R * brightness);
                    pixelData[(3 * i) + 1] = (byte)(LEDColors[x, z, y].G * brightness);
                    pixelData[(3 * i) + 2] = (byte)(LEDColors[x, z, y].B * brightness);
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

        void dimmer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (EnableDimmer)
            {
                for (ulong x = 0; x < NumX; x++)
                {
                    for (ulong y = 0; y < NumY; y++)
                    {
                        for (ulong z = 0; z < NumZ; z++)
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
        /*public void DrawAt(Vector brushPosition, Frame frame)
        {
            // normalize location
            InteractionBox box = frame.InteractionBox;
            Vector normalizedPoint = box.NormalizePoint(brushPosition);

            // convert leap coordinates to starfield coordinates
            Vector starfieldPoint = new Vector(normalizedPoint.x * (NUM_X - 1), normalizedPoint.y * (NUM_Y - 1), ((1.0f - normalizedPoint.z) * (NUM_Z - 1)));

            // convert starfield coordinates to the closest LED and draw it
            LEDColors[(int)Math.Round(starfieldPoint.x), (int)Math.Round(starfieldPoint.z), (int)Math.Round(starfieldPoint.y)] = Color.FromArgb(0, 0xFF, 0);
        }*/

        // set all LEDs to black
        public void Clear()
        {
            for (ulong x = 0; x < NumX; x++)
            {
                for (ulong y = 0; y < NumY; y++)
                {
                    for (ulong z = 0; z < NumZ; z++)
                    {
                        LEDColors[x, z, y] = Color.Black;
                    }
                }
            }
        }

        // set the LED at (x, y, z) to the given color
        public void SetColor(int x, int y, int z, Color color)
        {
            LEDColors[x, z, y] = color;
        }

        // returns the color of the LED at (x, y, z)
        public Color GetColor(int x, int y, int z)
        {
            return LEDColors[x, z, y];
        }
    }
}
