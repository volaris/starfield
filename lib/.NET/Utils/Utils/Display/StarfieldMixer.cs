using System;
using StarfieldClient;
using System.Collections.Generic;
using System.Timers;
using System.Drawing;
using System.Threading;

namespace StarfieldUtils.DisplayUtils
{
	public class StarfieldMixer
	{
		public enum FadeStyle
		{
			Linear,
			Sin
		}

		// this is the actual Starfield that we want to render to
		private StarfieldModel display;
		private List<StarfieldModel> channels;
        private System.Timers.Timer mix;
        private Thread fader;

        public StarfieldMixer(StarfieldModel display)
		{
			this.display = display;
			channels = new List<StarfieldModel>();
            mix = new System.Timers.Timer(30);
            mix.Elapsed += HandleElapsed;
        }

        void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            for(ulong x = 0; x < display.NumX; x++) 
            {
                for (ulong y = 0; y < display.NumY; y++) 
                {
                    for (ulong z = 0; z < display.NumZ; z++)
                    {
                        int sumRed = 0;
                        int sumGreen = 0;
                        int sumBlue = 0;

                        foreach(StarfieldModel channel in channels)
                        {
                            Color color = channel.GetColor((int)x, (int)y, (int)z);
                            sumRed += color.R;
                            sumGreen += color.G;
                            sumBlue += color.B;
                        }
                            
                        Color mixed = Color.FromArgb(sumRed / channels.Count, sumGreen / channels.Count, sumBlue / channels.Count);

                        display.SetColor((int)x, (int)y, (int)z, mixed);
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

        public void SetChannelValue(StarfieldModel channel, float level)
        {
            // clamp the input
            level = Math.Min(1.0f, Math.Max(0.0f, level));


        }
	}
}

