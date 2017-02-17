using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Starfield
{
    /**
     * <summary>    This class provides methods to load Starfield drivers. Use this if you want to create a new application to run the Starfield. </summary>
     *
     * <remarks>    Volar, 2/10/2017. </remarks>
     */

    public class DriverLoader
    {
        /**
         * <summary>    Driver filter delegate. This delegate type is used to filter the drivers based on the driver type. For example, selecting only ambient drivers. </summary>
         *
         * <param name="type">  The type. </param>
         *
         * <returns>    A bool. True includes this driver in the set, False excludes it. </returns>
         */

        public delegate bool DriverFilterDelegate(Type type);

        /**
         * <summary>    Loads all the drivers in the plugins directory. </summary>
         *
         * <param name="driverList">    List of drivers. </param>
         */

        public static void LoadPlugins(ICollection<IStarfieldDriver> driverList)
        {
            LoadPlugins(driverList, delegate(Type type) { return true; });
        }

        /**
         * <summary>    Loads the drivers that match the filter in the plugins directory. </summary>
         *
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         * <param name="driverFilter">  A filter specifying the driver type(s) to include. </param>
         */

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

        /**
         * <summary>    Loads all the builtin drivers from the current assembly. </summary>
         *
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         */

        public static void LoadBuiltinDrivers(ICollection<IStarfieldDriver> driverList)
        {
            LoadBuiltinDrivers(driverList, delegate(Type type) { return true; });
        }

        /**
         * <summary>    Loads builtin drivers from the current assembly that match the filter. </summary>
         *
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         * <param name="driverFilter">  A filter specifying the driver type(s) to include. </param>
         */

        public static void LoadBuiltinDrivers(ICollection<IStarfieldDriver> driverList, DriverFilterDelegate driverFilter)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                loadType(driverList, type, driverFilter);
            }
        }

        /**
         * <summary>    Loads all drivers in the StarfieldDrivers.dll assembly. </summary>
         *
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         */

        public static void LoadDefaultDrivers(ICollection<IStarfieldDriver> driverList)
        {
            LoadDefaultDrivers(driverList, delegate(Type type) { return true; });
        }

        /**
         * <summary>    Loads all drivers in the StarfieldDrivers.dll assembly. </summary>
         *
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         * <param name="driverFilter">  A filter specifying the driver type(s) to include. </param>
         */

        public static void LoadDefaultDrivers(ICollection<IStarfieldDriver> driverList, DriverFilterDelegate driverFilter)
        {
            foreach (Type type in Assembly.LoadFrom("StarfieldDrivers.dll").GetTypes())
            {
                loadType(driverList, type, driverFilter);
            }
        }

        /**
         * <summary>    Loads drivers from a custom path. </summary>
         *
         * <remarks>    Volar, 2/10/2017. </remarks>
         *
         * <param name="fileName">      Filename of the file. </param>
         * <param name="driverList">    List of drivers to fill with driver instances. </param>
         */
        // TODO: does this need a filter?
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
