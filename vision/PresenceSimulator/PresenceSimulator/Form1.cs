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
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PresenceSimulator
{

    public partial class Form1 : Form
    {
        SynchronizedCollection<SynchronizedCollection<Activity>> activity = new SynchronizedCollection<SynchronizedCollection<Activity>>();
        public Form1()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panelDraw, new object[] { true });

            comboBox1.SelectedIndex = 0;

            for(int i = 0; i < 11; i++)
            {
                SynchronizedCollection<Activity> y = new SynchronizedCollection<Activity>();
                for(int j = 0; j < 11; j++)
                {
                    Activity act = new Activity();
                    act.activity = 0;
                    y.Add(act);
                }
                activity.Add(y);
            }

            Thread server = new Thread(new ThreadStart(HttpServer));
            server.Start();
        }

        private void HttpServer()
        {
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8000/");

            listener.Start();

            while(true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string responseString = JsonConvert.SerializeObject(activity);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private void panelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            int numPeople = comboBox1.SelectedIndex + 1;

            int[] x = new int[numPeople];
            int[] y = new int[numPeople];

            for (int actX = 0; actX < activity.Count; actX++)
            {
                for (int actY = 0; actY < activity[actX].Count; actY++)
                {

                    activity[actX][actY].activity = 0;
                }
            }

            for (int i = 0; i < numPeople; i++ )
            {
                switch (i)
                {
                    case 0:
                        x[i] = e.X;
                        y[i] = e.Y;
                        break;
                    case 1:
                        x[i] = (panelDraw.Width / 2) + ((panelDraw.Width / 2) - e.X);
                        y[i] = e.Y;
                        break;
                    case 2:
                        x[i] = (panelDraw.Width / 2) + ((panelDraw.Width / 2) - e.X);
                        y[i] = (panelDraw.Height / 2) + ((panelDraw.Height / 2) - e.Y);
                        break;
                    case 3:
                        x[i] = e.X;
                        y[i] = (panelDraw.Height / 2) + ((panelDraw.Height / 2) - e.Y);
                        break;
                }
            }

            for (int actX = 0; actX < activity.Count; actX++)
            {
                for (int actY = 0; actY < activity[actX].Count; actY++)
                {
                    int pixX = actX * 15 + (int)(actX * 5.5) + 15 / 2;
                    int pixY = actY * 15 + (int)(actY * 5.5) + 15 / 2;

                    for(int i = 0; i < x.Length; i++)
                    {
                        double dist = Math.Sqrt(Math.Pow(x[i] - pixX, 2) + Math.Pow(y[i] - pixY, 2));

                        double pct = dist / -.3 + 100.0d;

                        activity[actY][actX].activity = Math.Max(pct, activity[actY][actX].activity);
                    }
                }
            }

            panelDraw.Refresh();
        }

        private void panelDraw_Paint(object sender, PaintEventArgs e)
        {
            Color baseColor = Color.Red;

            for (int x = 0; x < activity.Count; x++)
            {
                for (int y = 0; y < activity[x].Count; y++)
                {
                    int pixX = x * 15 + (int)(x * 5.5);
                    int pixY = y * 15 + (int)(y * 5.5);

                    double pct = activity[y][x].activity / 100;
                    Color draw = Color.FromArgb((int)(baseColor.R * pct), (int)(baseColor.G * pct), (int)(baseColor.B * pct));
                    Brush pen = new SolidBrush(draw);

                    e.Graphics.FillRectangle(pen, new Rectangle(pixX, pixY, 15, 15));
                }
            }
        }
    }

    public class Activity
    {
        public double activity;
    }
}
