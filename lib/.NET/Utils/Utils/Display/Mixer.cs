using System;
using StarfieldClient;
using System.Collections.Generic;
using System.Timers;
using System.Drawing;
using System.Threading;

namespace StarfieldUtils
{
	public class Mixer
	{
		public enum FadeStyle
		{
			Linear,
			Sin
		}

		// this is the actual Starfield that we want to render to
		private StarfieldModel display;
		private List<StarfieldModel> channels;
        private Timer mix;
        private Thread fader;

		public Mixer(StarfieldModel display)
		{
			this.display = display;
			channels = new List<StarfieldModel>();
            mix = new Timer(30);
            mix.Elapsed += HandleElapsed;
        }

        void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            for(int x = 0; x < display.NumX; x++) 
            {
                for(int y = 0; y < display.NumY; y++) 
                {
                    for(int z = 0; z < display.NumZ; z++)
                    {
                        int sumRed = 0;
                        int sumGreen = 0;
                        int sumBlue = 0;

                        foreach(StarfieldModel channel in channels)
                        {
                            Color color = channel.GetColor(x, y, z);
                            sumRed += color.R;
                            sumGreen += color.G;
                            sumBlue += color.B;
                        }

                        Color mixed = Color.FromArgb(sumRed / channels.Count, sumGreen / channels.Count, sumBlue / channels.Count);

                        display.SetColor(x, y, z, mixed);
                    }
                }
            }
        }

		public StarfieldModel AddChannel()
		{
			StarfieldModel channel = new StarfieldModel (display.XStep, display.YStep, display.ZStep, display.NumX, display.NumY, display.NumZ);
			channels.Add(channel);
			return channel;
		}

		public void RemoveChannel(StarfieldModel channel)
		{
			channels.Remove(channel);
		}

        public void CrossFade(StarfieldModel channelIn, StarfieldModel channelOut, int duration)
        {
            if(fader != null && fader.IsAlive)
            {
                return;
            }
        }

        public void FadeOut(int duration)
        {
            if(fader != null && fader.IsAlive)
            {
                return;
            }
        }

        public void FadeIn(StarfieldModel channel, int duration)
        {
            if(fader != null && fader.IsAlive)
            {
                return;
            }
        }
	}
}

