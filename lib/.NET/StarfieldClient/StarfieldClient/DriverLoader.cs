using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Starfield
{
    public class DriverLoader
    {
        public delegate bool DriverFilterDelegate(Type type);

        public static void LoadPlugins(ICollection<IStarfieldDriver> driverList)
        {
            LoadPlugins(driverList, delegate(Type type) { return true; });
        }

        public static void LoadPlugins(ICollection<IStarfieldDriver> driverList, DriverFilterDelegate driverFilter)
        {
            string pluginPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginPath = System.IO.Path.Combine(pluginPath, "plugins");
            if (System.IO.Directory.Exists(pluginPath))
            {
                string[] plugins = System.IO.Directory.GetFiles(pluginPath, "*.dll");
                foreach (string filename in plugins)
                {
                    try
                    {
                        Assembly plugin = Assembly.LoadFrom(filename);

                        foreach (Type type in plugin.GetTypes())
                        {
                            loadType(driverList, type, driverFilter);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Unable to load: {0}", filename);
                    }
                }
            }
        }

        public static void LoadBuiltinDrivers(ICollection<IStarfieldDriver> driverList)
        {
            LoadBuiltinDrivers(driverList, delegate(Type type) { return true; });
        }

        public static void LoadBuiltinDrivers(ICollection<IStarfieldDriver> driverList, DriverFilterDelegate driverFilter)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                loadType(driverList, type, driverFilter);
            }
        }

        public static void LoadDefaultDrivers(ICollection<IStarfieldDriver> driverList)
        {
            LoadDefaultDrivers(driverList, delegate(Type type) { return true; });
        }

        public static void LoadDefaultDrivers(ICollection<IStarfieldDriver> driverList, DriverFilterDelegate driverFilter)
        {
            foreach (Type type in Assembly.LoadFrom("StarfieldDrivers.dll").GetTypes())
            {
                loadType(driverList, type, driverFilter);
            }
        }

        public static void LoadCustomDrivers(string fileName, ICollection<CustomDriver> driverList)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            System.IO.StreamReader reader = new System.IO.StreamReader(fileName);
            String json = reader.ReadToEnd();
            List<CustomDriver> list = JsonConvert.DeserializeObject<List<CustomDriver>>(json, settings);

            foreach (CustomDriver driver in list)
            {
                driverList.Add(driver);
            }

            reader.Close();
        }

        // try loading an instance of the given type into the algorithm combo 
        // box the type must inherit from IStarfield driver, be a class, and 
        // not be abstract
        private static void loadType(ICollection<IStarfieldDriver> driverList, Type type, DriverFilterDelegate driverFilter)
        {
            if (typeof(IStarfieldDriver).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && driverFilter(type))
            {
                driverList.Add((IStarfieldDriver)Activator.CreateInstance(type));
            }
        }
    }
}
