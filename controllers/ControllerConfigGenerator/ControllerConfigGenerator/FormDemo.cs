using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Starfield;
using Starfield.Networking;
using System.Timers;
using Newtonsoft.Json;

namespace ControllerConfigGenerator
{
    public partial class FormDemo : Form
    {
        // how often IStarfieldDriver.Render() is called  in milliseconds
        int RenderInterval = 30;

        // The algorithm that is currently rendering to the display
        IStarfieldDriver CurrentDriver;

        public FormDemo()
        {
            InitializeComponent();

            // load builtin drivers
            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
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

            // load plugins
            string pluginPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginPath = System.IO.Path.Combine(pluginPath, "plugins");
            if(System.IO.Directory.Exists(pluginPath))
            {
                Console.WriteLine("Loading Plugins");

                string[] plugins = System.IO.Directory.GetFiles(pluginPath, "*.dll");
                foreach(string filename in plugins)
                {
                    try
                    {
                        Assembly plugin = Assembly.LoadFrom(filename);

                        foreach (Type type in plugin.GetTypes())
                        {
                            loadType(type);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Unable to load: {0}", filename);
                    }
                }
            }

            if (comboBoxAlgorithm.Items.Count > 0)
            {
                // this will cause the event combo box selected index changed
                // event handler to be called which will set the first driver
                // in the list to be the current driver and call its
                // IStarfieldDriver.Start() method
                comboBoxAlgorithm.SelectedIndex = 0;
            }
        }

        // the user has selected a new algorithm, stop the old one and start the new one
        private void comboBoxAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CurrentDriver = ((IStarfieldDriver)(IStarfieldDriver)Activator.CreateInstance((Type)(comboBoxAlgorithm.SelectedItem)));
                propertyGridDriver.SelectedObject = this.CurrentDriver;
            }
            catch
            { }
        }

        // try loading an instance of the given type into the algorithm combo 
        // box the type must inherit from IStarfield driver, be a class, and 
        // not be abstract
        private void loadType(Type type)
        {
            if (typeof(IStarfieldDriver).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                comboBoxAlgorithm.Items.Add(type);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            IStarfieldDriver driver = CurrentDriver;
            CustomDriver listItem = new CustomDriver();
            listItem.Driver = driver;
            listItem.Name = textBoxName.Text;

            foreach(object obj in listBoxDrivers.Items)
            {
                if(listItem.Name == ((CustomDriver)obj).Name)
                {
                    System.Windows.Forms.MessageBox.Show("needs a unique name");
                    return;
                }
            }

            listBoxDrivers.Items.Add(listItem);
            this.CurrentDriver = ((IStarfieldDriver)(IStarfieldDriver)Activator.CreateInstance((Type)(comboBoxAlgorithm.SelectedItem)));
            propertyGridDriver.SelectedObject = this.CurrentDriver;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            string json = JsonConvert.SerializeObject(listBoxDrivers.Items, Formatting.Indented, settings);
            SaveFileDialog mySFD = new SaveFileDialog();
            if(mySFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = mySFD.FileName;
                System.IO.TextWriter writer = System.IO.File.CreateText(file);
                writer.Write(json);
                writer.Flush();
            }
        }

        private void listBoxDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridAdded.SelectedObject = ((CustomDriver)listBoxDrivers.SelectedItem).Driver;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            listBoxDrivers.Items.Remove(listBoxDrivers.SelectedItem);
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOFD = new OpenFileDialog();
            if(myOFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<CustomDriver> list = new List<CustomDriver>();
                DriverLoader.LoadCustomDrivers(myOFD.FileName, list);
                foreach (CustomDriver driver in list)
                {
                    listBoxDrivers.Items.Add(driver);
                }
            }
        }
    }
}
