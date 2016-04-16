using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield;
using Starfield.Presence;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;

namespace StarfieldDrivers.PresenceResponsive
{
    [DriverType(DriverTypes.Experimental)]
    public class GenericTrails : IStarfieldDriver
    {
        #region Private Members
        double[, ,] brightness;
        StarfieldModel rawStarfield;
        public static List<IStarfieldDriver> drivers = new List<IStarfieldDriver>();
        IStarfieldDriver driver;
        double maxHeight = 8.0;
        double height = 4.0;
        double fadeRate = .95;
        #endregion

        #region Public Properties
        [TypeConverter(typeof(DriverConverter))]
        public IStarfieldDriver Driver
        {
            get { return driver; }
            set { driver = value; }
        }

        public double FadeRate
        {
            get { return fadeRate; }
            set { fadeRate = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }
        #endregion

        public GenericTrails()
        {
            // load builtin drivers
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                loadType(type);
            }

            // load default drivers
            try
            {
                foreach (Type type in Assembly.LoadFrom("StarfieldDrivers.dll").GetTypes())
                {
                    loadType(type);
                }
            }
            catch
            {
                Console.WriteLine("Unable to load: StarfieldDrivers.dll");
            }

            driver = new StarfieldDrivers.Animation.RainbowTest();
        }

        #region IStarfieldDriver Implementation
        public void Render(StarfieldModel Starfield)
        {
            driver.Render(rawStarfield);
            List<List<Activity>> activity;
            activity = Starfield.GetPresence();

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    if(y * Starfield.YStep > maxHeight)
                    {
                        continue;
                    }
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Color prev = rawStarfield.GetColor((int)x, (int)y, (int)z);
                        brightness[x, y, z] = brightness[x, y, z] * fadeRate;
                        if (activity[(int)x][(int)z].activity > 0)
                        {
                            double activityPct = activity[(int)x][(int)z].activity / 100.0;
                            
                            if(y * Starfield.YStep > height)
                            {
                                activityPct = activityPct * (1 - ((y * Starfield.YStep) - height) / (maxHeight - height));
                            }

                            brightness[x, y, z] = Math.Max(brightness[x, y, z], activityPct);
                        }
                        Color next = Color.FromArgb((int)(brightness[x, y, z] * prev.R), (int)(brightness[x, y, z] * prev.G), (int)(brightness[x, y, z] * prev.B));
                        Starfield.SetColor((int)x, (int)y, (int)z, next);
                    }
                }
            }
        }

        void IStarfieldDriver.Start(StarfieldModel Starfield)
        {
            brightness = new double[Starfield.NumX, Starfield.NumY, Starfield.NumZ];
            rawStarfield = new StarfieldModel(Starfield.XStep, Starfield.YStep, Starfield.ZStep, Starfield.NumX, Starfield.NumY, Starfield.NumZ);
            driver.Start(rawStarfield);

            for (ulong x = 0; x < Starfield.NumX; x++)
            {
                for (ulong y = 0; y < Starfield.NumY; y++)
                {
                    for (ulong z = 0; z < Starfield.NumZ; z++)
                    {
                        Starfield.SetColor((int)x, (int)y, (int)z, Color.Black);
                        brightness[x, y, z] = 0.0d;
                    }
                }
            }
        }

        void IStarfieldDriver.Stop()
        {
            driver.Stop();
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return "Generic Trails";
        }
        #endregion

        // try loading an instance of the given type into the algorithm combo 
        // box the type must inherit from IStarfield driver, be a class, and 
        // not be abstract
        private void loadType(Type type)
        {
            if (typeof(IStarfieldDriver).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && !(type == typeof(GenericTrails)))
            {
                drivers.Add((IStarfieldDriver)Activator.CreateInstance(type));
            }
        }

        class DriverConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(drivers);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    foreach (IStarfieldDriver driver in drivers)
                    {
                        if (driver.ToString() == (string)value)
                        {
                            return driver;
                        }
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
