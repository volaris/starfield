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
            comboBoxAlgorithm.SelectedIndex = 0;
            Model = new StarfieldModel(System.Net.IPAddress.Parse("192.168.0.102"), 7890);//System.Net.IPAddress.Parse("127.0.0.1"), 7890 );//
            System.Timers.Timer render = new System.Timers.Timer(RenderInterval);
            render.Elapsed += render_Elapsed;
            render.Start();
        }

        void render_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (RenderLock)
            {
                this.CurrentDriver.Render(this.Model);
            }
        }

        private void comboBoxAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock(RenderLock)
            {
                if (this.CurrentDriver != null)
                {
                    this.CurrentDriver.Stop();
                }
                this.CurrentDriver = ((IStarfieldDriver)comboBoxAlgorithm.SelectedItem);
                this.CurrentDriver.Start(this.Model);
            }
        }
    }
}
