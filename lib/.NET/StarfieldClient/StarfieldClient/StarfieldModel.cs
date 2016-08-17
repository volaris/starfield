using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using Starfield.Presence;

namespace Starfield
{
    public class StarfieldModel
    {
        // these should match the values for the simulator or the starfield
        protected const float DEFAULT_X_STEP = 2;
        protected const float DEFAULT_Y_STEP = 2;
        protected const float DEFAULT_Z_STEP = 2;
        protected const ulong DEFAULT_NUM_X = 7;
        protected const ulong DEFAULT_NUM_Y = 4;
        protected const ulong DEFAULT_NUM_Z = 5;

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
        protected Timer dimmer = new Timer(3000);
        public bool EnableDimmer = false;

        // Flush lock: this ensures that only one thread is modifying pixel
        // data at a time
        protected Object lockObject = new Object();

        // current color state
        public Color[, ,] LEDColors;

        private PresenceClient presenceClient;
        private List<List<Activity>> presence;
        private bool runPresenceClientThread = true;
        private int presenceUpdateInterval = 10;

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
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 11, 10, 11, new PresenceClient());
        }

        // create a model with the defaults for the Burning Man version
        public static StarfieldModel BurningManStarfield()
        {
            // 60' x 60' x 30' spaced at 2' intervals on the y axis and 4' intervals on the x & z axes
            return new StarfieldModel(4.0f, 2.0f, 4.0f, 16, 15, 16);
        }

        public static StarfieldModel KekeMohy()
        {
            // 14 x 60 x 2 pixel art car, step in y is tiny, x, z is larger and unknown
            return new StarfieldModel(2.0f, .083f, 6.0f, 14, 60, 2);
        }

        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ) : this(xStep,
                                                                                                                yStep,
                                                                                                                zStep,
                                                                                                                numX,
                                                                                                                numY,
                                                                                                                numZ,
                                                                                                                null)
        {

        }

        // create a model with arbitrary parameters
        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ, PresenceClient presenceClient)
        {
            System.Threading.Monitor.Enter(lockObject);

            this.XStep = xStep;
            this.YStep = yStep;
            this.ZStep = zStep;
            this.NumX = numX;
            this.NumY = numY;
            this.NumZ = numZ;

            LEDColors = new Color[NumX, NumZ, NumY];

            this.presenceClient = presenceClient;

            dimmer.Elapsed += dimmer_Elapsed;
            dimmer.Start();

            System.Threading.Thread presenceThread = new System.Threading.Thread(new System.Threading.ThreadStart(presenceThreadWorker));
            presenceThread.Start();

            presence = new List<List<Activity>>();
            for(int x = 0; x < (int)numX; x++)
            {
                List<Activity> xList = new List<Activity>();

                for(int y = 0; y < (int)numZ; y++)
                {
                    Activity act = new Activity();
                    act.activity = 0;
                    xList.Add(act);
                }

                presence.Add(xList);
            }

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

        void presenceThreadWorker()
        {
            while (runPresenceClientThread)
            {
                try
                {
                    if (presenceClient != null)
                    {
                        List<List<Activity>> activity = presenceClient.GetLatest();
                        if (activity == null)
                        {
                            return;
                        }
                        for (int x = 0; x < (int)this.NumX; x++)
                        {
                            for (int y = 0; y < (int)this.NumZ; y++)
                            {
                                presence[x][y].activity = activity[x][y].activity;
                            }
                        }
                    }
                }
                catch(Exception e)
                { }
                System.Threading.Thread.Sleep(presenceUpdateInterval);
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
        public virtual void Clear()
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
        public virtual void SetColor(int x, int y, int z, Color color)
        {
            System.Threading.Monitor.Enter(lockObject);

            LEDColors[x, z, y] = color;

            System.Threading.Monitor.Exit(lockObject);
        }

        // returns the color of the LED at (x, y, z)
        public virtual Color GetColor(int x, int y, int z)
        {
            Color color = LEDColors[x, z, y];
            return Color.FromArgb((int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        public List<List<Activity>> GetPresence()
        {
            System.Threading.Monitor.Enter(lockObject);

            List<List<Activity>> presenceCopy = new List<List<Activity>>();

            foreach(List<Activity> x in presence)
            {
                List<Activity> xCopy = new List<Activity>();
                foreach(Activity act in x)
                {
                    Activity actCopy = new Activity();
                    actCopy.activity = act.activity;
                    xCopy.Add(actCopy);
                }
                presenceCopy.Add(xCopy);
            }

            System.Threading.Monitor.Exit(lockObject);

            return presenceCopy;
        }

        public void Stop()
        {
            dimmer.Enabled = false;
            runPresenceClientThread = false;
        }
    }
}
