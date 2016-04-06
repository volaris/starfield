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

namespace AlgorithmDemo
{
    public partial class FormDemo : Form
    {
        // how often IStarfieldDriver.Render() is called  in milliseconds
        int RenderInterval = 30;

        // Starfield model class, stores the colors
        StarfieldModel Model;

        // Starfield client class, handles communication with the Starfield
        TCPStarfieldClient Client;

        // The algorithm that is currently rendering to the display
        IStarfieldDriver CurrentDriver;

        // lock object to prevent multiple threads from modifying the the
        // starfield at the same time
        Object RenderLock = new Object();

        // endpoint that we will try to connect to first and that will be
        // displayed when the app starts up
        string DefaultIP = "127.0.0.1";
        int DefaultPort = 7890; 
        System.Timers.Timer render;

        public FormDemo()
        {
            InitializeComponent();

            textBoxIP.Text = DefaultIP;
            textBoxPort.Text = DefaultPort.ToString();

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

            // set up the starfield type combo box
            comboBoxStarfield.Items.Add("Home Starfield");
            comboBoxStarfield.Items.Add("Critical NW Starfield");
            comboBoxStarfield.Items.Add("Burning Man Starfield");
            comboBoxStarfield.SelectedIndex = 0;

            if (comboBoxAlgorithm.Items.Count > 0)
            {
                // this will cause the event combo box selected index changed
                // event handler to be called which will set the first driver
                // in the list to be the current driver and call its
                // IStarfieldDriver.Start() method
                comboBoxAlgorithm.SelectedIndex = 0;
            }

            // start the render timer
            render = new System.Timers.Timer(RenderInterval);
            render.Elapsed += render_Elapsed;
            render.Start();
        }

        // request that the current driver renders a frame to the display
        // if a driver hasn't completed rendering the previous frame, drop
        // this frame
        void render_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(RenderLock, ref lockTaken);
            if (lockTaken)
            {
                try
                {
                    this.CurrentDriver.Render(this.Model);
                }
                catch
                { }
                finally
                {
                    System.Threading.Monitor.Exit(RenderLock);
                }
            }
            else
            {
                Console.WriteLine("Dropped frame");
            }
        }

        // the user has selected a new algorithm, stop the old one and start the new one
        private void comboBoxAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                if (this.CurrentDriver != null)
                {
                    this.CurrentDriver.Stop();
                }
                this.CurrentDriver = ((IStarfieldDriver)comboBoxAlgorithm.SelectedItem);
                this.CurrentDriver.Start(this.Model);
                propertyGridDriver.SelectedObject = this.CurrentDriver;
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        // the user has requested that we render to a different Starfield size,
        // update the client
        private void comboBoxStarfield_SelectedIndexChanged(object sender, EventArgs e)
        {
            reconnect();
        }

        // connect to the new starfield
        private void reconnect()
        {
            string ip = textBoxIP.Text;
            int port = int.Parse(textBoxPort.Text);

            System.Threading.Monitor.Enter(RenderLock);

            if(Model != null)
            {
                Model.Stop();
            }

            if(Client != null)
            {
                Client.Stop();
            }

            try
            {
                switch (comboBoxStarfield.SelectedIndex)
                {
                    case 0:
                        Model = StarfieldModel.HomeStarfield();
                        break;
                    case 1:
                        Model = StarfieldModel.CriticalNWStarfield();
                        break;
                    case 2:
                        Model = StarfieldModel.BurningManStarfield();
                        break;
                }

                Client = new TCPStarfieldClient(Model, System.Net.IPAddress.Parse(ip), port);

                if (this.CurrentDriver != null)
                {
                    this.CurrentDriver.Stop();
                    this.CurrentDriver.Start(this.Model);
                }
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        // reset the current driver
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                if (this.CurrentDriver != null)
                {
                    this.CurrentDriver.Stop();
                    this.CurrentDriver.Start(this.Model);
                }
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        // try loading an instance of the given type into the algorithm combo 
        // box the type must inherit from IStarfield driver, be a class, and 
        // not be abstract
        private void loadType(Type type)
        {
            if (typeof(IStarfieldDriver).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                comboBoxAlgorithm.Items.Add((IStarfieldDriver)Activator.CreateInstance(type));
            }
        }

        // the user wants to change the maximum brightness of the starfield,
        // update the client
        private void trackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            Model.Brightness = (float)trackBarBrightness.Value / (float)trackBarBrightness.Maximum;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if(trackBar1.Value == 0 && render.Enabled)
            {
                render.Enabled = false;
            }
            else if(trackBar1.Value > 0 && !render.Enabled)
            {
                render.Enabled = true;
            }
        }

        private void buttonReconnect_Click(object sender, EventArgs e)
        {
            string ip = textBoxIP.Text;
            int port = int.Parse(textBoxPort.Text);

            System.Threading.Monitor.Enter(RenderLock); 
            try
            {
                if (Client != null)
                {
                    Client.Stop();
                }

                Client = new TCPStarfieldClient(Model, System.Net.IPAddress.Parse(ip), port);
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }
    }
}
