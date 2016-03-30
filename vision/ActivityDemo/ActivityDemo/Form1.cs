using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json;
using System.IO;

namespace ActivityDemo
{
    public class Activity
    {
        public double activity;
    }

    public partial class Form1 : Form
    {
        string path;
        Timer myTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            myTimer.Tick += myTimer_Tick;
        }

        void myTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    List<List<Activity>> foo = JsonConvert.DeserializeObject<List<List<Activity>>>(json);
                    Color baseColor = Color.Red;

                    for (int x = 0; x < foo.Count; x++)
                    {
                        for (int y = 0; y < foo[x].Count; y++)
                        {
                            int pixX = x * 15 + (int)(x * 5.5);
                            int pixY = y * 15 + (int)(y * 5.5);

                            double pct = foo[x][y].activity / 100;
                            Color draw = Color.FromArgb((int)(baseColor.R * pct), (int)(baseColor.G * pct), (int)(baseColor.B * pct));
                            Brush pen = new SolidBrush(draw);

                            panelDraw.CreateGraphics().FillRectangle(pen, new Rectangle(pixX, pixY, 15, 15));
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            OpenFileDialog myOFD = new OpenFileDialog();
            if (myOFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = myOFD.FileName;
                myTimer.Interval = 100;
                myTimer.Enabled = true;
            }
        }
    }
}
