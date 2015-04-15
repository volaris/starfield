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

        // change the maximum brightness of the starfield
        // this field's range is [0,1]
        float brightness = 1.0f;

        // set up the dimming timer
        Timer dimmer = new Timer(3000);
        public bool EnableDimmer = false;

        // Flush lock: this ensures that only one thread is modifying pixel
        // data at a time
        private Object lockObject = new Object();

        // current color state
        public static Color[, ,] LEDColors;

        public float Brightness
        {
            get { return brightness; }
            set { brightness = Math.Min(Math.Max(0f,value),1f); }
        }

        // create a model with the defaults for the home prototype
        public static StarfieldModel HomeStarfield()
        {
            // 12' x 8' x 7.5' spaced at 2' intervals
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 7, 4, 5);
        }

        // create a model with the defaults for the Critical NW version
        public static StarfieldModel CriticalNWStarfield()
        {
            // 20' x 20' x 20' spaced at 2' intervals
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 11, 10, 11);
        }

        // create a model with the defaults for the Burning Man version
        public static StarfieldModel BurningManStarfield()
        {
            // 60' x 60' x 30' spaced at 2' intervals on the y axis and 4' intervals on the x & z axes
            return new StarfieldModel(4.0f, 2.0f, 4.0f, 16, 15, 16);
        }

        // create a model with arbitrary parameters
        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ)
        {
            System.Threading.Monitor.Enter(lockObject);

            this.XStep = xStep;
            this.YStep = yStep;
            this.ZStep = zStep;
            this.NumX = numX;
            this.NumY = numY;
            this.NumZ = numZ;

            LEDColors = new Color[NumX, NumZ, NumY];

            dimmer.Elapsed += dimmer_Elapsed;
            dimmer.Start();

            System.Threading.Monitor.Exit(lockObject);
        }

        // create a model with the global defaults
        public StarfieldModel()
            : this(DEFAULT_X_STEP, 
                   DEFAULT_Y_STEP,
                   DEFAULT_Z_STEP, 
                   DEFAULT_NUM_X,
                   DEFAULT_NUM_Y, 
                   DEFAULT_NUM_Z)
        {
        }

        

        void dimmer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Threading.Monitor.Enter(lockObject);

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

            System.Threading.Monitor.Exit(lockObject);
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
            System.Threading.Monitor.Enter(lockObject);

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

            System.Threading.Monitor.Exit(lockObject);
        }

        // set the LED at (x, y, z) to the given color
        public void SetColor(int x, int y, int z, Color color)
        {
            System.Threading.Monitor.Enter(lockObject);

            LEDColors[x, z, y] = color;

            System.Threading.Monitor.Exit(lockObject);
        }

        // returns the color of the LED at (x, y, z)
        public Color GetColor(int x, int y, int z)
        {
            Color color = LEDColors[x, z, y];
            return Color.FromArgb((int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        public void Stop()
        {
            dimmer.Enabled = false;
        }
    }
}
