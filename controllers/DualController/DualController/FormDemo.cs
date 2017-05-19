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
using StarfieldUtils.DisplayUtils;

namespace DualController
{
    /** <summary>    Values that represent driver location. </summary> */
    public enum DriverLocation
    {
        Interior,
        Exterior
    }

    /** <summary>    The GUI for the Dual Controller. </summary> */
    public partial class FormDemo : Form
    {
        // how often IStarfieldDriver.Render() is called  in milliseconds
        int RenderInterval = 30;

        // Starfield model class, stores the colors
        SplitStarfieldModel Model;
        StarfieldMapper Mapper;

        // Starfield client class, handles communication with the Starfield
        TCPStarfieldClient Client;

        // how often the algorithm should be switched
        int AlgorithmSwitchInterval = 600000; // 10 Min

        StarfieldMixer MixerInterior, MixerExterior;

        // The available drivers
        List<IStarfieldDriver> DriversInterior, DriversExterior;

        // The algorithm that is currently rendering to the display
        IStarfieldDriver[] CurrentDriversInterior, CurrentDriversExterior;
        StarfieldModel[] ChannelsInterior, ChannelsExterior;
        int primaryInterior = 0;
        int primaryExterior = 0;

        // The algorithm that is currently rendering to the display
        //IStarfieldDriver CurrentDriverInterior;
        //IStarfieldDriver CurrentDriverExterior;

        // lock object to prevent multiple threads from modifying the the
        // starfield at the same time
        Object RenderLock = new Object();

        // endpoint that we will try to connect to first and that will be
        // displayed when the app starts up
        string DefaultIP = "127.0.0.1";
        int DefaultPort = 7890;
        System.Timers.Timer render;
        System.Timers.Timer algorithmSwitchInterior;
        System.Timers.Timer algorithmSwitchExterior;

        Random rand;

        /** <summary>    Default constructor. </summary> */
        public FormDemo()
        {
            InitializeComponent();

            textBoxIP.Text = DefaultIP;
            textBoxPort.Text = DefaultPort.ToString();

            DriversInterior = new List<IStarfieldDriver>();
            DriversExterior = new List<IStarfieldDriver>();

            // load builtin drivers
            DriverLoader.LoadBuiltinDrivers(DriversInterior);
            DriverLoader.LoadBuiltinDrivers(DriversExterior);

            // load default drivers
            DriverLoader.LoadDefaultDrivers(DriversInterior);
            DriverLoader.LoadDefaultDrivers(DriversExterior);

            // load plugins
            DriverLoader.LoadPlugins(DriversInterior);
            DriverLoader.LoadPlugins(DriversExterior);

            foreach (IStarfieldDriver driver in DriversInterior)
            {
                comboBoxInteriorAlgorithm.Items.Add(driver);
            }

            foreach (IStarfieldDriver driver in DriversExterior)
            {
                comboBoxExteriorAlgorithm.Items.Add(driver);
            }

            reconnect();

            CurrentDriversInterior = new IStarfieldDriver[2];
            CurrentDriversExterior = new IStarfieldDriver[2];
            ChannelsInterior = new StarfieldModel[2];
            ChannelsExterior = new StarfieldModel[2];
            MixerInterior = new StarfieldMixer(Model.Model1);
            MixerExterior = new StarfieldMixer(Model.Model2);
            MixerInterior.FadeCompleted += MixerInterior_FadeCompleted;
            MixerExterior.FadeCompleted += MixerExterior_FadeCompleted;
            ChannelsInterior[0] = MixerInterior.AddChannel();
            ChannelsInterior[1] = MixerInterior.AddChannel();
            ChannelsExterior[0] = MixerExterior.AddChannel();
            ChannelsExterior[1] = MixerExterior.AddChannel();

            rand = new Random();

            if (comboBoxInteriorAlgorithm.Items.Count > 0)
            {
                // this will cause the event combo box selected index changed
                // event handler to be called which will set the first driver
                // in the list to be the current driver and call its
                // IStarfieldDriver.Start() method
                comboBoxInteriorAlgorithm.SelectedIndex = 0;
            }

            if (comboBoxExteriorAlgorithm.Items.Count > 0)
            {
                // this will cause the event combo box selected index changed
                // event handler to be called which will set the first driver
                // in the list to be the current driver and call its
                // IStarfieldDriver.Start() method
                comboBoxExteriorAlgorithm.SelectedIndex = 0;
            }

            // create the algorithm switch timers
            algorithmSwitchInterior = new System.Timers.Timer(AlgorithmSwitchInterval);
            algorithmSwitchInterior.Elapsed += algorithmSwitchInterior_Elapsed;
            algorithmSwitchExterior = new System.Timers.Timer(AlgorithmSwitchInterval);
            algorithmSwitchExterior.Elapsed += algorithmSwitchExterior_Elapsed;

            checkBoxAmbientExterior.Checked = true;

            // start the render timer
            render = new System.Timers.Timer(RenderInterval);
            render.Elapsed += render_Elapsed;
            render.Start();

            MixerInterior.FadeIn(ChannelsInterior[primaryInterior], new TimeSpan(0, 0, 3), StarfieldMixer.FadeStyle.Sin);
            MixerExterior.FadeIn(ChannelsExterior[primaryExterior], new TimeSpan(0, 0, 3), StarfieldMixer.FadeStyle.Sin);
        }

        private void algorithmSwitchExterior_Elapsed(object sender, ElapsedEventArgs e)
        {
            SwitchAlgorithm(true, DriverLocation.Exterior);
        }

        private void algorithmSwitchInterior_Elapsed(object sender, ElapsedEventArgs e)
        {
            SwitchAlgorithm(true, DriverLocation.Interior);
        }

        private delegate void updateText();

        /**
         * <summary>    Fade completed. </summary>
         *
         * <param name="type">      The type. </param>
         * <param name="channel">   The channel. </param>
         */

        void FadeCompleted(DriverLocation type, StarfieldModel channel)
        {
            IStarfieldDriver[] CurrentDrivers = (type == DriverLocation.Exterior) ? CurrentDriversExterior : CurrentDriversInterior;
            int primary = (type == DriverLocation.Exterior) ? primaryExterior : primaryInterior;
            StarfieldModel[] Channels = (type == DriverLocation.Exterior) ? ChannelsExterior : ChannelsInterior;
            TextBox textBoxAlgorithm = (type == DriverLocation.Exterior) ? textBoxExteriorAlgorithm : textBoxInteriorAlgorithm;
            
            for (int i = 0; i < CurrentDrivers.Length; i++)
            {
                if (i != primary && channel == Channels[i])
                {
                    CurrentDrivers[i].Stop();
                    CurrentDrivers[i] = null;
                }
                else if (i == primary)
                {
                    // both fades may not be complete, but we're close enough
                    this.Invoke(new updateText(delegate() { textBoxAlgorithm.Text = CurrentDrivers[primary].ToString(); }), null);
                }
            }
        }

        /**
         * <summary>    Mixer exterior fade completed. </summary>
         *
         * <param name="Sender">    Source of the event. </param>
         * <param name="channel">   The channel. </param>
         */

        void MixerExterior_FadeCompleted(object Sender, StarfieldModel channel)
        {
            FadeCompleted(DriverLocation.Exterior, channel);
        }

        /**
         * <summary>    Mixer interior fade completed. </summary>
         *
         * <param name="Sender">    Source of the event. </param>
         * <param name="channel">   The channel. </param>
         */

        private void MixerInterior_FadeCompleted(object Sender, StarfieldModel channel)
        {
            FadeCompleted(DriverLocation.Interior, channel);
        }

        /**
         * <summary>
         * request that the current driver renders a frame to the display if a driver hasn't completed
         * rendering the previous frame, drop this frame.
         * </summary>
         *
         * <param name="sender">    Source of the event. </param>
         * <param name="e">         Elapsed event information. </param>
         */

        void render_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(RenderLock, ref lockTaken);
            if (lockTaken)
            {
                try
                {
                    this.CurrentDriversInterior[primaryInterior].Render(ChannelsInterior[primaryInterior]);
                    this.CurrentDriversExterior[primaryExterior].Render(ChannelsExterior[primaryExterior]);
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

        /**
         * <summary>
         * the user has selected a new algorithm, stop the old one and start the new one.
         * </summary>
         *
         * <param name="sender">    Source of the event. </param>
         * <param name="e">         Event information. </param>
         */

        private void comboBoxInteriorAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                if (this.CurrentDriversInterior[primaryInterior] != null)
                {
                    this.CurrentDriversInterior[primaryInterior].Stop();
                }
                this.CurrentDriversInterior[primaryInterior] = ((IStarfieldDriver)comboBoxInteriorAlgorithm.SelectedItem);
                this.CurrentDriversInterior[primaryInterior].Start(ChannelsInterior[primaryInterior]);
                propertyGridInteriorDriver.SelectedObject = this.CurrentDriversInterior[primaryInterior];
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        /** <summary>    connect to the new starfield. </summary> */
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
                Model = new SplitStarfieldModel(2, 2, 2, 15, 20, 4, 1);
                Mapper = new StarfieldMapper(StarfieldModel.HomeStarfield(), Model);

                Client = new TCPStarfieldClient(Mapper, System.Net.IPAddress.Parse(ip), port);
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        /**
         * <summary>    Switch algorithm. </summary>
         *
         * <param name="running">   True to running. </param>
         * <param name="driver">    The driver. </param>
         */

        private void SwitchAlgorithm(bool running, DriverLocation driver)
        {
            string oldDriver = String.Empty;
            string newDriver = String.Empty;

            List<IStarfieldDriver> Drivers = (driver == DriverLocation.Exterior) ? DriversExterior : DriversInterior;

            // The algorithm that is currently rendering to the display
            IStarfieldDriver[] CurrentDrivers = (driver == DriverLocation.Exterior) ? CurrentDriversExterior : CurrentDriversInterior;
            StarfieldModel[] Channels = (driver == DriverLocation.Exterior) ? ChannelsExterior : ChannelsInterior;
            int primary = (driver == DriverLocation.Exterior) ? primaryExterior : primaryInterior;
            StarfieldMixer Mixer = (driver == DriverLocation.Exterior) ? MixerExterior : MixerInterior;
            TextBox textBoxAlgorithm = (driver == DriverLocation.Exterior) ? textBoxExteriorAlgorithm : textBoxInteriorAlgorithm;

            System.Threading.Monitor.Enter(RenderLock);

            IStarfieldDriver next;

            // select a new driver
            do
            {
                next = Drivers[rand.Next(Drivers.Count)];
            } while (next == CurrentDrivers[primary]);

            newDriver = next.ToString();

            try
            {
                if (running)
                {
                    // we already have one running so find an empty channel
                    // start driving that channel and set the mixer to crossfade
                    // from the old driver to the newly selected one
                    oldDriver = CurrentDrivers[primary].ToString();

                    for (int i = 0; i < CurrentDrivers.Length; i++)
                    {
                        if (CurrentDrivers[i] == null)
                        {
                            CurrentDrivers[i] = next;
                            next.Start(Channels[i]);
                            Mixer.CrossFade(Channels[i], Channels[primary], new TimeSpan(0, 0, 5), StarfieldMixer.FadeStyle.Sin);
                            if (driver == DriverLocation.Exterior)
                            {
                                primaryExterior = i;
                            }
                            else
                            {
                                primaryInterior = i;
                            }
                            break;
                        }
                    }

                    textBoxAlgorithm.Text = String.Format("{0} => {1}", oldDriver, newDriver);
                }
                else
                {
                    // we aren't running so just enable the new driver and 
                    // fade in
                    CurrentDrivers[primary] = next;
                    next.Start(Channels[primary]);
                    Mixer.FadeIn(Channels[primary], new TimeSpan(0, 0, 3), StarfieldMixer.FadeStyle.Sin);

                    textBoxAlgorithm.Text = String.Format("=> {0}", newDriver);
                }
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        /**
         * <summary>    reset the current driver. </summary>
         *
         * <param name="sender">    Source of the event. </param>
         * <param name="e">         Event information. </param>
         */

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                if (this.CurrentDriversInterior[primaryInterior] != null)
                {
                    this.CurrentDriversInterior[primaryInterior].Stop();
                    this.CurrentDriversInterior[primaryInterior].Start(this.Model.Model1);
                }
                if (this.CurrentDriversExterior[primaryExterior] != null)
                {
                    this.CurrentDriversExterior[primaryExterior].Stop();
                    this.CurrentDriversExterior[primaryExterior].Start(this.Model.Model2);
                }
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        /**
         * <summary>
         * the user wants to change the maximum brightness of the starfield, update the client.
         * </summary>
         *
         * <param name="sender">    Source of the event. </param>
         * <param name="e">         Event information. </param>
         */

        private void trackBarInteriorBrightness_ValueChanged(object sender, EventArgs e)
        {
            Model.Model1.Brightness = (float)trackBarInteriorBrightness.Value / (float)trackBarInteriorBrightness.Maximum;
        }

        private void trackBarRenderSpeed_ValueChanged(object sender, EventArgs e)
        {
            if(trackBarRenderSpeed.Value == 0 && render.Enabled)
            {
                render.Enabled = false;
            }
            else if(trackBarRenderSpeed.Value > 0 && !render.Enabled)
            {
                render.Enabled = true;
            }
        }

        private void checkBoxInteriorAmbient_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxInteriorAlgorithm.Visible = !checkBoxInteriorAmbient.Checked;
            propertyGridInteriorDriver.Enabled = !checkBoxInteriorAmbient.Checked;
            textBoxInteriorAlgorithm.Visible = checkBoxInteriorAmbient.Checked;
            buttonNextInteriorAlgorithm.Visible = checkBoxInteriorAmbient.Checked;
            if (checkBoxInteriorAmbient.Checked)
            {
                algorithmSwitchInterior.Start();
                SwitchAlgorithm(true, DriverLocation.Interior);
            }
            else
            {
                algorithmSwitchInterior.Stop();
                comboBoxInteriorAlgorithm_SelectedIndexChanged(comboBoxInteriorAlgorithm, new EventArgs());
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

        private void comboBoxAlgorithmExterior_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                if (this.CurrentDriversExterior[primaryExterior] != null)
                {
                    this.CurrentDriversExterior[primaryExterior].Stop();
                }
                this.CurrentDriversExterior[primaryExterior] = ((IStarfieldDriver)comboBoxExteriorAlgorithm.SelectedItem);
                this.CurrentDriversExterior[primaryExterior].Start(ChannelsExterior[primaryExterior]);
                propertyGridExteriorDriver.SelectedObject = this.CurrentDriversExterior[primaryExterior];
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

        private void checkBoxAmbientExterior_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxExteriorAlgorithm.Visible = !checkBoxAmbientExterior.Checked;
            propertyGridExteriorDriver.Enabled = !checkBoxAmbientExterior.Checked;
            textBoxExteriorAlgorithm.Visible = checkBoxAmbientExterior.Checked;
            buttonNextExteriorAlgorithm.Visible = checkBoxAmbientExterior.Checked;
            if (checkBoxAmbientExterior.Checked)
            {
                algorithmSwitchExterior.Start();
                SwitchAlgorithm(true, DriverLocation.Exterior);
            }
            else
            {
                algorithmSwitchExterior.Stop();
                comboBoxAlgorithmExterior_SelectedIndexChanged(comboBoxExteriorAlgorithm, new EventArgs());
            }
        }

        private void trackBarBrightnessExterior_Scroll(object sender, EventArgs e)
        {
            Model.Model2.Brightness = (float)trackBarExteriorBrightness.Value / (float)trackBarExteriorBrightness.Maximum;
        }

        private void buttonNextInteriorAlgorithm_Click(object sender, EventArgs e)
        {
            SwitchAlgorithm(true, DriverLocation.Interior);
        }

        private void buttonNextExteriorAlgorithm_Click(object sender, EventArgs e)
        {
            SwitchAlgorithm(true, DriverLocation.Exterior);
        }
    }
}
