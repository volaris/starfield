using System;
using Starfield;
using System.Collections.Generic;
using System.Timers;
using System.Drawing;
using System.Threading.Tasks;

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
        private List<double> weights;
        private List<Task> faders;
        private System.Timers.Timer mix;
        // TODO: ReaderWriterLockSlim?
        private object lockObject = new object();

        public StarfieldMixer(StarfieldModel display)
		{
			this.display = display;
			channels = new List<StarfieldModel>();
            weights = new List<double>();
            faders = new List<Task>();
            mix = new System.Timers.Timer(30);
            mix.Elapsed += HandleElapsed;
        }

        void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                for (ulong x = 0; x < display.NumX; x++)
                {
                    for (ulong y = 0; y < display.NumY; y++)
                    {
                        for (ulong z = 0; z < display.NumZ; z++)
                        {
                            Color[] colors = new Color[channels.Count];

                            for (int i = 0; i < channels.Count; i++)
                            {
                                StarfieldModel channel = channels[i];
                                colors[i] = channel.GetColor((int)x, (int)y, (int)z);
                            }

                            Color mixed = ColorUtils.ColorUtils.Blend(colors, weights.ToArray());

                            display.SetColor((int)x, (int)y, (int)z, mixed);
                        }
                    }
                }
            }
        }

		public StarfieldModel AddChannel()
		{
            StarfieldModel channel;
            lock (lockObject)
            {
                channel = new StarfieldModel(display.XStep, display.YStep, display.ZStep, display.NumX, display.NumY, display.NumZ);
                channels.Add(channel);
                weights.Add(0d);
            }
			return channel;
		}

		public void RemoveChannel(StarfieldModel channel)
		{
            lock (lockObject)
            {
                weights.RemoveAt(channels.IndexOf(channel));
                channels.Remove(channel);
            }
		}

        public void CrossFade(StarfieldModel channelIn, StarfieldModel channelOut, TimeSpan duration, FadeStyle fadeStyle)
        {
            lock (lockObject)
            {
                FadeOut(channelOut, duration, fadeStyle);
                FadeIn(channelIn, duration, fadeStyle);
            }
        }

        public void FadeOut(TimeSpan duration, FadeStyle fadeStyle)
        {
            lock (lockObject)
            {
                foreach(StarfieldModel channel in channels)
                {
                    FadeOut(channel, duration, fadeStyle);
                }
            }
        }

        public void FadeOut(StarfieldModel channel, TimeSpan duration, FadeStyle fadeStyle)
        {
            lock (lockObject)
            {
                int index = channels.IndexOf(channel);
                Task fader = faders[index];
                if (fader != null && !fader.IsCompleted)
                {
                    // TODO: use CancelationTokenSource to replace the fader instaed
                    return;
                }

                faders[index] = Task.Run(() => doFadeOut(channel, duration, fadeStyle));
            }
        }

        public void FadeIn(StarfieldModel channel, TimeSpan duration, FadeStyle fadeStyle)
        {
            lock (lockObject)
            {
                int index = channels.IndexOf(channel);
                Task fader = faders[index];
                if (fader != null && !fader.IsCompleted)
                {
                    // TODO: use CancelationTokenSource to replace the fader instaed
                    return;
                }

                faders[index] = Task.Run(() => doFadeIn(channel, duration, fadeStyle));
            }
        }

        public void SetChannelValue(StarfieldModel channel, float level)
        {   
            // clamp the input
            level = Math.Min(1.0f, Math.Max(0.0f, level));

            lock (lockObject)
            {
                weights[channels.IndexOf(channel)] = level;
            }
        }

        private double sinFade(double t)
        {
            // clamp the input
            t = Math.Min(1.0f, Math.Max(0.0f, t));

            // scale and shift
            t *= Math.PI;
            t += Math.PI;

            return Math.Cos(t);
        }

        private double linearFade(double t)
        {
            // clamp the input
            t = Math.Min(1.0f, Math.Max(0.0f, t));

            return t;
        }

        private void doFadeOut(StarfieldModel channel, TimeSpan duration, FadeStyle fadeStyle)
        {
            DateTime start = DateTime.Now;
            double startWeight;
            TimeSpan runtime;

            lock (lockObject)
            {
                startWeight = weights[channels.IndexOf(channel)];
            }

            runtime = DateTime.Now - start;

            while(runtime < duration)
            {
                double percent = runtime.TotalMilliseconds / duration.TotalMilliseconds;

                lock(lockObject)
                {
                    switch(fadeStyle)
                    {
                        case FadeStyle.Linear:
                            weights[channels.IndexOf(channel)] = startWeight * linearFade(1 - percent);
                            break;
                        case FadeStyle.Sin:
                            weights[channels.IndexOf(channel)] = startWeight * sinFade(1 - percent);
                            break;
                    }
                }

                runtime = DateTime.Now - start;
            }

            lock(lockObject)
            {
                weights[channels.IndexOf(channel)] = 0d;
            }
        }

        private void doFadeIn(StarfieldModel channel, TimeSpan duration, FadeStyle fadeStyle)
        {
            DateTime start = DateTime.Now;
            double startWeight;
            TimeSpan runtime;

            lock (lockObject)
            {
                startWeight = weights[channels.IndexOf(channel)];
            }

            runtime = DateTime.Now - start;

            while (runtime < duration)
            {
                double percent = runtime.TotalMilliseconds / duration.TotalMilliseconds;

                lock (lockObject)
                {
                    switch (fadeStyle)
                    {
                        case FadeStyle.Linear:
                            weights[channels.IndexOf(channel)] = startWeight * linearFade(percent);
                            break;
                        case FadeStyle.Sin:
                            weights[channels.IndexOf(channel)] = startWeight * sinFade(percent);
                            break;
                    }
                }

                runtime = DateTime.Now - start;
            }

            lock (lockObject)
            {
                weights[channels.IndexOf(channel)] = 1.0d;
            }
        }
	}
}

