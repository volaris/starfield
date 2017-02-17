using System;
using Starfield;
using System.Collections.Generic;
using System.Timers;
using System.Drawing;
using System.Threading.Tasks;

namespace StarfieldUtils.DisplayUtils
{
    /**
     * <summary>    A used to fade between/mix the outputs of multiple starfield drivers. </summary>
     *
     */

	public class StarfieldMixer
	{
        /**
         * <summary>    Values that represent fade styles. </summary>
         *
         */

		public enum FadeStyle
		{
             /** <summary>    An enum constant representing the linear option. </summary> */
			Linear,
             /** <summary>    An enum constant representing the Sine option. </summary> */
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

        /**
         * <summary>    Handler, called when the fade completed. </summary>
         *
         * <param name="Sender">    Source of the event. </param>
         * <param name="channel">   The channel. </param>
         */

        public delegate void FadeCompletedHandler(object Sender, StarfieldModel channel);
        /** <summary>    Event queue for all listeners interested in FadeCompleted events. </summary> */
        public event FadeCompletedHandler FadeCompleted;

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="display">   The display. </param>
         */

        public StarfieldMixer(StarfieldModel display)
		{
			this.display = display;
			channels = new List<StarfieldModel>();
            weights = new List<double>();
            faders = new List<Task>();
            mix = new System.Timers.Timer(30);
            mix.Elapsed += HandleElapsed;
            mix.Start();
        }

        void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                int numWeights = 0;
                bool multiWeight = false;

                for (int i = 0; i < weights.Count; i++)
                {
                    if(weights[i] > 0)
                    {
                        numWeights++;
                    }
                }

                if(numWeights > 1)
                {
                    multiWeight = true;
                }

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

                            Color mixed = multiWeight ? ColorUtils.ColorUtils.BlendAveraged(colors, weights.ToArray()) : ColorUtils.ColorUtils.BlendRaw(colors, weights.ToArray());

                            display.SetColor((int)x, (int)y, (int)z, mixed);
                        }
                    }
                }
            }
        }

        /**
         * <summary>    Adds a channel. </summary>
         *
         * <returns>    A StarfieldModel that should be used by the driver driving this channel. </returns>
         */

		public StarfieldModel AddChannel()
		{
            StarfieldModel channel;
            lock (lockObject)
            {
                channel = new StarfieldModel(display.XStep, display.YStep, display.ZStep, display.NumX, display.NumY, display.NumZ);
                channels.Add(channel);
                weights.Add(0d);
                faders.Add(null);
            }
			return channel;
		}

        /**
         * <summary>    Removes the channel described by channel. </summary>
         *
         * <param name="channel">   The StarfieldModel backing the channel. </param>
         */

		public void RemoveChannel(StarfieldModel channel)
		{
            lock (lockObject)
            {
                weights.RemoveAt(channels.IndexOf(channel));
                channels.Remove(channel);
            }
		}

        /**
         * <summary>    Cross fade between two channels. </summary>
         *
         * <param name="channelIn">     The channel to fade in. </param>
         * <param name="channelOut">    The channel to fade out. </param>
         * <param name="duration">      The duration of the fade. </param>
         * <param name="fadeStyle">     The fade style. </param>
         */

        public void CrossFade(StarfieldModel channelIn, StarfieldModel channelOut, TimeSpan duration, FadeStyle fadeStyle)
        {
            lock (lockObject)
            {
                FadeOut(channelOut, duration, fadeStyle);
                FadeIn(channelIn, duration, fadeStyle);
            }
        }

        /**
         * <summary>    Fade out  all channels. </summary>
         *
         * <param name="duration">  The duration. </param>
         * <param name="fadeStyle"> The fade style. </param>
         */

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

        /**
         * <summary>    Fade out the given channel. </summary>
         *\
         * <param name="channel">   The channel. </param>
         * <param name="duration">  The duration. </param>
         * <param name="fadeStyle"> The fade style. </param>
         */

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

        /**
         * <summary>    Fade in the given channel. </summary>
         *
         * <param name="channel">   The channel. </param>
         * <param name="duration">  The duration. </param>
         * <param name="fadeStyle"> The fade style. </param>
         */

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

        /**
         * <summary>    Sets the fade percent for the given channel. </summary>
         *
         * <remarks>    Volar, 2/13/2017. </remarks>
         *
         * <param name="channel">   The channel. </param>
         * <param name="level">     The level [0.0,1.0]. </param>
         */

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

            t = (1.0d + Math.Cos(t)) / 2.0d;

            return Math.Min(1.0f, Math.Max(0.0f, t));
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

            if (FadeCompleted != null)
            {
                FadeCompleted(this, channel);
            }
        }

        private void doFadeIn(StarfieldModel channel, TimeSpan duration, FadeStyle fadeStyle)
        {
            DateTime start = DateTime.Now;
            double startWeight;
            TimeSpan runtime;

            lock (lockObject)
            {
                startWeight = 1.0d - weights[channels.IndexOf(channel)];
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

            if(FadeCompleted != null)
            {
                FadeCompleted(this, channel);
            }
        }
	}
}

