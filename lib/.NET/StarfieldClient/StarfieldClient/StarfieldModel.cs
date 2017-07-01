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
    /**
     * <summary>    This is the core Starfield class. It defines and manages all of the properties that determine what is displayed on the Starfield</summary>
     */

    public class StarfieldModel
    {
        // these should match the values for the simulator or the Starfield
        protected const float DEFAULT_X_STEP = 2;
        protected const float DEFAULT_Y_STEP = 2;
        protected const float DEFAULT_Z_STEP = 2;
        protected const ulong DEFAULT_NUM_X = 7;
        protected const ulong DEFAULT_NUM_Y = 4;
        protected const ulong DEFAULT_NUM_Z = 5;

        /** <summary>    distance in feet between pixels in the x direction. </summary> */
        public float XStep = 2;
        /** <summary>    distance in feet between pixels in the y direction. </summary> */
        public float YStep = 2;
        /** <summary>    distance in feet between pixels in the z direction. </summary> */
        public float ZStep = 2;

        /** <summary>    Number of lights in the x direction. </summary> */
        public ulong NumX = 7;
        /** <summary>    Number of lights in the y direction. </summary> */
        public ulong NumY = 4;
        /** <summary>    Number of lights in the z direction. </summary> */
        public ulong NumZ = 5;

        // change the maximum brightness of the starfield
        // this field's range is [0,1]
        float brightness = 1.0f;

        // set up the dimming timer
        protected Timer dimmer = new Timer(3000);
        /** <summary>    True to enable, false to disable the dimmer. </summary> */
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

        private bool needSafetyLight = false;

        private int minLevel = 0x50;

        /**
         * <summary>    Gets or sets the brightness scaler. </summary>
         *
         * <value>  The brightness scaler [0,1]. </value>
         */

        public float Brightness
        {
            get { return brightness; }
            set { brightness = Math.Min(Math.Max(0f,value),1f); }
        }

        /**
         * <summary>    Gets or sets a value indicating whether the Starfield needs safety light. If true, the Starfield will ensure a minimum level of brightness in all of the lights.</summary>
         *
         * <value>  True if the Starfield needs safety light, false if not. </value>
         */

        public bool NeedSafetyLight
        {
            get { return needSafetyLight; }
            set { needSafetyLight = value; }
        }

        /**
         * <summary>    Gets or sets the minimum level used for safety lighting. Compared againts the sum of the 3 component levels (R, G, B).</summary>
         *
         * <value>  The minimum level. </value>
         */

        public int MinLevel
        {
            get { return minLevel; }
            set { minLevel = value; }
        }

        /**
         * <summary>    create a model with the defaults for the home prototype. </summary>
         *
         * <returns>    A 12'x8'x7.5' StarfieldModel with 2' spacing. </returns>
         */

        public static StarfieldModel HomeStarfield()
        {
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 7, 4, 5);
        }

        /**
         * <summary>    create a model with the defaults for the 2016 version. </summary>
         *
         * <returns>    A 20'x20'x10' StarfieldModel with 2' spacing and mirrored strands (the 20' strands were turned into 10'). </returns>
         */

        public static StarfieldModel Starfield2016()
        {
            return (StarfieldModel)new CriticalStarfieldModel(2.0f, 2.0f, 2.0f, 11, 10, 11, new PresenceClient());
        }

        /**
         * <summary>    create a model with the defaults for the 2017 version. </summary>
         *
         * <returns>    A 20'x20'x10' StarfieldModel with 2' spacing. </returns>
         */

        public static StarfieldModel Starfield2017()
        {
            return (StarfieldModel)new CriticalStarfieldModel(2.0f, 2.0f, 2.0f, 11, 5, 11, new PresenceClient());
        }

        /**
         * <summary>    create a model with the defaults for the Critical NW version. </summary>
         *
         * <returns>    A 20'x20'x10' StarfieldModel with 2' spacing. </returns>
         */

        public static StarfieldModel CriticalNWStarfield()
        {
            return Starfield2017();
        }

        /**
         * <summary>    create a model with the defaults for the Burning Man version. </summary>
         *
         * <returns>    A 20'x20'x10' StarfieldModel with 2' spacing. </returns>
         */

        public static StarfieldModel BurningManStarfield()
        {
            return Starfield2017();
        }

        /**
         * <summary>    create a model with the defaults for the Critical NW version. </summary>
         *
         * <returns>    A 20'x20'x20' StarfieldModel with 2' spacing. </returns>
         */

        public static StarfieldModel DreamCriticalNWStarfield()
        {
            return new StarfieldModel(2.0f, 2.0f, 2.0f, 11, 10, 11, new PresenceClient());
        }

        /**
         * <summary>    create a model with the defaults for the Burning Man version. </summary>
         *
         * <returns>    A 60'x60'x30' StarfieldModel with 4' horizontal spacing and 2' vertical. </returns>
         */

        public static StarfieldModel DreamBurningManStarfield()
        {
            return new StarfieldModel(4.0f, 2.0f, 4.0f, 16, 15, 16);
        }

        /**
         * <summary>    Keke mohy. </summary>
         *
         * <returns>    A StarfieldModel for use with the Keke Mohy art car. </returns>
         */

        public static StarfieldModel KekeMohy()
        {
            // 14 x 60 x 2 pixel art car, step in y is tiny, x, z is larger and unknown
            return new StarfieldModel(2.0f, .083f, 6.0f, 14, 60, 2);
        }

        /**
         * <summary>    Create a model with arbitrary parameters without presence. </summary>
         *
         * <param name="xStep">             Distance between lights in the x direction. </param>
         * <param name="yStep">             Distance between lights in the y direction. </param>
         * <param name="zStep">             Distance between lights in the z direction. </param>
         * <param name="numX">              Number of lights in the x direction. </param>
         * <param name="numY">              Number of lights in the y direction. </param>
         * <param name="numZ">              Number of lights in the z direction. </param>
         */

        public StarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ) : this(xStep,
                                                                                                                yStep,
                                                                                                                zStep,
                                                                                                                numX,
                                                                                                                numY,
                                                                                                                numZ,
                                                                                                                null)
        {

        }

        /**
         * <summary>    Create a model with arbitrary parameters with presence. </summary>
         *
         * <param name="xStep">             Distance between lights in the x direction. </param>
         * <param name="yStep">             Distance between lights in the y direction. </param>
         * <param name="zStep">             Distance between lights in the z direction. </param>
         * <param name="numX">              Number of lights in the x direction. </param>
         * <param name="numY">              Number of lights in the y direction. </param>
         * <param name="numZ">              Number of lights in the z direction. </param>
         * <param name="presenceClient">    Client to collect presence data from the presence service. </param>
         */

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

        /**
         * <summary>    create a model with the global defaults. </summary>
         */

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
                            if ((red + blue + green) > MinLevel)
                            {
                                LEDColors[x, z, y] = Color.FromArgb(red, green, blue);
                            }
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
                                presence[y][x].activity = activity[x][y].activity;
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

        /**
         * <summary>    set all LEDs to black. </summary>
         */

        public virtual void Clear()
        {
            System.Threading.Monitor.Enter(lockObject);

            for (ulong x = 0; x < NumX; x++)
            {
                for (ulong y = 0; y < NumY; y++)
                {
                    for (ulong z = 0; z < NumZ; z++)
                    {
                        if (NeedSafetyLight)
                        {
                            LEDColors[x, z, y] = Color.FromArgb(MinLevel / 3, MinLevel / 3, MinLevel / 3);
                        }
                        else
                        {
                            LEDColors[x, z, y] = Color.Black;
                        }
                    }
                }
            }

            System.Threading.Monitor.Exit(lockObject);
        }

        /**
         * <summary>    set the LED color at (x, y, z) to the given color. </summary>
         *
         * <param name="x">     The x coordinate. </param>
         * <param name="y">     The y coordinate. </param>
         * <param name="z">     The z coordinate. </param>
         * <param name="color"> The color. </param>
         */

        public virtual void SetColor(int x, int y, int z, Color color)
        {
            System.Threading.Monitor.Enter(lockObject);

            if (NeedSafetyLight && (color.R + color.G + color.B) < MinLevel)
            {
                LEDColors[x, z, y] = Color.FromArgb(MinLevel/3, MinLevel/3, MinLevel/3);
            }
            else
            {
                LEDColors[x, z, y] = color;
            }

            System.Threading.Monitor.Exit(lockObject);
        }

        /**
         * <summary>    returns the color of the LED at (x, y, z) </summary>
         *
         * <param name="x"> The x coordinate. </param>
         * <param name="y"> The y coordinate. </param>
         * <param name="z"> The z coordinate. </param>
         *
         * <returns>    The color. </returns>
         */

        public virtual Color GetColor(int x, int y, int z)
        {
            Color color = LEDColors[x, z, y];
            return Color.FromArgb((int)(color.R * brightness), (int)(color.G * brightness), (int)(color.B * brightness));
        }

        /**
         * <summary>    Gets the presence information for the Starfield. </summary>
         *
         * <returns>    The presence. </returns>
         */

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

        /**
         * <summary>    Stops the presence client. </summary>
         *
         */

        public void Stop()
        {
            dimmer.Enabled = false;
            runPresenceClientThread = false;
        }
    }
}
