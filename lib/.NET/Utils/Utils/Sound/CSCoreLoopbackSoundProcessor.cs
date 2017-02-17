using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StarfieldUtils.SoundUtils
{
    /**
     * <summary>    A sound processor based on the CS Core audio capture library. </summary>
     */

    public class CSCoreLoopbackSoundProcessor : BaseSoundProcessor
    {
        CSCore.SoundIn.WasapiLoopbackCapture loopback;

        // arrays to hold frame data
        float[] soundDataChannel1;
        float[] soundDataChannel2;

        public CSCoreLoopbackSoundProcessor() : base()
        {
            loopback = new CSCore.SoundIn.WasapiLoopbackCapture();
            loopback.Initialize();
            loopback.DataAvailable += loopback_DataAvailable;
            loopback.Stopped += loopback_Stopped;
            loopback.Start();
        }

        void loopback_Stopped(object sender, CSCore.SoundIn.RecordingStoppedEventArgs e)
        {
            Console.WriteLine("STOPPED");
        }
        void loopback_DataAvailable(object sender, CSCore.SoundIn.DataAvailableEventArgs e)
        {
            // we get a byte array, but the data is actually an array of floats
            float[] scaled = new float[e.ByteCount / 4];

            this.soundDataChannel1 = new float[scaled.Length / 2];
            this.soundDataChannel2 = new float[scaled.Length / 2];

            // convert the byte array to an array of IEEE 32 bit floats
            for (int i = 0; i < e.ByteCount / 4; i++)
            {
                scaled[i] = System.BitConverter.ToSingle(e.Data, i * 4);
            }

            // the sound sample data is interleaved, split it out into channels
            for (int i = 0; i < scaled.Length / 2; i++)
            {
                this.soundDataChannel1[i] = scaled[2 * i];
                this.soundDataChannel2[i] = scaled[2 * i + 1];
            }

            this.ProcessFrame(this.soundDataChannel1, this.soundDataChannel2, e.Format.SampleRate);
        }
    }
}
