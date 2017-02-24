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
using StarfieldUtils.DisplayUtils;
using System.Timers;
using Newtonsoft.Json;

namespace Ambient
{
    /** <summary>    User interface for the Configurable Controller. </summary> */
    public partial class FormDemo : Form
    {
        // how often IStarfieldDriver.Render() is called  in milliseconds
        int RenderInterval = 30;

        // how often the algorithm should be switched
        int AlgorithmSwitchInterval = 600000; // 10 Min

        // Starfield model class, stores the colors
        StarfieldModel Model;

        // Starfield client class, handles communication with the Starfield
        TCPStarfieldClient Client;

        // The available drivers
        List<IStarfieldDriver> Drivers;

        // The algorithm that is currently rendering to the display
        IStarfieldDriver[] CurrentDrivers;
        StarfieldModel[] Channels;
        int primary = 0;

        StarfieldMixer Mixer;

        // lock object to prevent multiple threads from modifying the the
        // starfield at the same time
        Object RenderLock = new Object();

        // endpoint that we will try to connect to first and that will be
        // displayed when the app starts up
        string DefaultIP = "127.0.0.1";//"192.168.0.50";//
        int DefaultPort = 7891;
        System.Timers.Timer render;
        System.Timers.Timer algorithmSwitch;

        string DefaultPath = "default.json";

        Random rand;

        /** <summary>    Default constructor. </summary> */
        public FormDemo()
        {
            InitializeComponent();

            textBoxIP.Text = DefaultIP;
            textBoxPort.Text = DefaultPort.ToString();

            reloadDrivers();

            // set up the starfield type combo box
            comboBoxStarfield.Items.Add("Home Starfield");
            comboBoxStarfield.Items.Add("Critical NW Starfield");
            comboBoxStarfield.Items.Add("Burning Man Starfield");
            comboBoxStarfield.Items.Add("KekeMohy");
            comboBoxStarfield.SelectedIndex = 1;

            CurrentDrivers = new IStarfieldDriver[2];
            Channels = new StarfieldModel[2];
            Mixer = new StarfieldMixer(Model);
            Mixer.FadeCompleted += Mixer_FadeCompleted;
            Channels[0] = Mixer.AddChannel();
            Channels[1] = Mixer.AddChannel();
            rand = new Random();

            SwitchAlgorithm(false);

            // start the algorithm switch timer
            algorithmSwitch = new System.Timers.Timer(AlgorithmSwitchInterval);
            algorithmSwitch.Elapsed += algorithmSwitch_Elapsed;
            algorithmSwitch.Start();

            // start the render timer
            render = new System.Timers.Timer(RenderInterval);
            render.Elapsed += render_Elapsed;
            render.Start();
        }

        private delegate void updateText();

        void Mixer_FadeCompleted(object Sender, StarfieldModel channel)
        {
            for (int i = 0; i < CurrentDrivers.Length; i++)
            {
                if(i != primary && channel == Channels[i])
                {
                    CurrentDrivers[i].Stop();
                    CurrentDrivers[i] = null;
                } 
                else if (i == primary)
                {
                    // both fades may not be complete, but we're close enough
                    this.Invoke(new updateText(delegate() {textBoxAlgorithm.Text = CurrentDrivers[primary].ToString();}), null);
                }
            }
        }

        void algorithmSwitch_Elapsed(object sender, ElapsedEventArgs e)
        {
            SwitchAlgorithm(true);
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
                    for (int i = 0; i < CurrentDrivers.Length; i++)
                    {
                        IStarfieldDriver driver = CurrentDrivers[i];

                        if (driver != null)
                        {
                            // render all running drivers
                            driver.Render(Channels[i]);
                        }
                    }
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
         * the user has requested that we render to a different Starfield size, update the client.
         * </summary>
         *
         * <param name="sender">    Source of the event. </param>
         * <param name="e">         Event information. </param>
         */

        private void comboBoxStarfield_SelectedIndexChanged(object sender, EventArgs e)
        {
            reconnect();
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
                    case 3:
                        Model = StarfieldModel.KekeMohy();
                        break;
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

        private void reloadDrivers()
        {
            String FileName = null;
            if(System.IO.File.Exists(DefaultPath))
            {
                FileName = DefaultPath;
            }
            OpenFileDialog myOFD = new OpenFileDialog();
            if (myOFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileName = myOFD.FileName;
            }
            if (FileName != null)
            {
                List<CustomDriver> list = new List<CustomDriver>();
                DriverLoader.LoadCustomDrivers(FileName, list);
                foreach (CustomDriver driver in list)
                {
                    listBoxDrivers.Items.Add(driver);
                }
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

        private void SwitchAlgorithm(bool running)
        {
            string oldDriver = String.Empty;
            string newDriver = String.Empty;
            System.Threading.Monitor.Enter(RenderLock);

            IStarfieldDriver next;

            // select a new driver
            if (radioButtonRandom.Checked)
            {
                int nextIndex;
                do
                {
                    nextIndex = rand.Next(listBoxDrivers.Items.Count);
                    next = ((CustomDriver)listBoxDrivers.Items[nextIndex]).Driver;
                } while (next == CurrentDrivers[primary]);
                listBoxDrivers.SelectedIndex = nextIndex;
            }
            else if(radioButtonSequential.Checked)
            {
                int nextIndex = (listBoxDrivers.SelectedIndex + 1) % CurrentDrivers.Length;
                listBoxDrivers.SelectedIndex = nextIndex;
                next = ((CustomDriver)listBoxDrivers.Items[nextIndex]).Driver;
            }
            else if(radioButtonManual.Checked)
            {
                return;
            }
            else
            {
                return;
            }

            newDriver = next.ToString();

            try
            {
                if(running)
                {
                    // we already have one running so find an empty channel
                    // start driving that channel and set the mixer to crossfade
                    // from the old driver to the newly selected one
                    oldDriver = CurrentDrivers[primary].ToString();

                    for(int i = 0; i < CurrentDrivers.Length; i++)
                    {
                        if (CurrentDrivers[i] == null)
                        {
                            CurrentDrivers[i] = next;
                            next.Start(Channels[i]);
                            Mixer.CrossFade(Channels[i], Channels[primary], new TimeSpan(0, 0, 5), StarfieldMixer.FadeStyle.Sin);
                            primary = i;
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

        private void buttonNext_Click(object sender, EventArgs e)
        {
            SwitchAlgorithm(true);
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            System.Threading.Monitor.Enter(RenderLock);
            reloadDrivers();
            System.Threading.Monitor.Exit(RenderLock);
        }
    }
}
