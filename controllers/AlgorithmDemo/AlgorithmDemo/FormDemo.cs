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
using StarfieldClient;
using System.Timers;
using AlgorithmDemo.Drivers;

namespace AlgorithmDemo
{
    public partial class FormDemo : Form
    {
        int RenderInterval = 30;
        StarfieldModel Model;
        IStarfieldDriver CurrentDriver;
        Object RenderLock = new Object();
        string DefaultIP = "192.168.0.102";
        int DefaultPort = 7890;

        public FormDemo()
        {
            InitializeComponent();

            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if(typeof(IStarfieldDriver).IsAssignableFrom(type) && !type.IsInterface)
                {
                    comboBoxAlgorithm.Items.Add((IStarfieldDriver)Activator.CreateInstance(type));
                }
            }
            textBoxIP.Text = DefaultIP;
            textBoxPort.Text = DefaultPort.ToString();
            comboBoxAlgorithm.SelectedIndex = 0;
            comboBoxStarfield.Items.Add("Home Starfield");
            comboBoxStarfield.Items.Add("Critical NW Starfield");
            comboBoxStarfield.Items.Add("Burning Man Starfield");
            comboBoxStarfield.SelectedIndex = 0;
            System.Timers.Timer render = new System.Timers.Timer(RenderInterval);
            render.Elapsed += render_Elapsed;
            render.Start();
        }

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

        private void comboBoxStarfield_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ip = textBoxIP.Text;
            int port = int.Parse(textBoxPort.Text);
            
            System.Threading.Monitor.Enter(RenderLock);

            try
            {
                switch (comboBoxStarfield.SelectedIndex)
                {
                    case 0:
                        Model = StarfieldModel.HomeStarfield(System.Net.IPAddress.Parse(ip), port);
                        break;
                    case 1:
                        Model = StarfieldModel.CriticalNWStarfield(System.Net.IPAddress.Parse(ip), port);
                        break;
                    case 2:
                        Model = StarfieldModel.BurningManStarfield(System.Net.IPAddress.Parse(ip), port);
                        break;
                }
            }
            catch
            { }
            finally
            {
                System.Threading.Monitor.Exit(RenderLock);
            }
        }

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
    }
}
