using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StarfieldUtils.SoundUtils
{
    public class CSCoreLoopbackSoundProcessor : ISoundProcessor
    {
        public event OnArtifactDetectedHandler OnArtifactDetected;
        public event OnFrameUpdateHandler OnFrameUpdate;

        // the minimum time between artifact notifications
        // TODO: change this to be per artifact type
        public int ArtifactDelay = 0;
        // minimum volume before generating artifact notifications
        public float MinimumArtifactThreshold = .15f;

        CSCore.SoundIn.WasapiLoopbackCapture loopback;

        // FFT size
        // this size defines the granularity of the frequency buckets 
        private const int FFT_INPUT_SIZE = 512;

        // arrays to hold frame data
        // TODO: don't assume stereo
        float[] fftChannel1;
        float[] fftChannel2;
        float[] soundDataChannel1;
        float[] soundDataChannel2;
        float[] eqDataChannel1;
        float[] eqDataChannel2;
        byte vuChannel1 = 0;
        byte vuChannel2 = 0;

        // the threshold for naive artifact detection
        private float threshold = 0;
        private Random rand = new Random();

        // the last time we detected artifact and sent a notification
        DateTime lastArtifact = DateTime.MinValue;

        public CSCoreLoopbackSoundProcessor()
        {
            loopback = new CSCore.SoundIn.WasapiLoopbackCapture();
            loopback.Initialize();
            loopback.DataAvailable += loopback_DataAvailable;
            loopback.Stopped += loopback_Stopped;
            loopback.Start();

            eqDataChannel1 = new float[Frame.bands.Length];
            eqDataChannel2 = new float[Frame.bands.Length];
        }

        void loopback_Stopped(object sender, CSCore.SoundIn.RecordingStoppedEventArgs e)
        {
            Console.WriteLine("STOPPED");
        }
        void loopback_DataAvailable(object sender, CSCore.SoundIn.DataAvailableEventArgs e)
        {
            float dcComponent;

            // we get a byte array, but the data is actually an array of floats
            float[] scaled = new float[e.ByteCount / 4];

            // if no one is listening, we shouldn't do all of this calculation
            if(this.OnArtifactDetected == null && this.OnFrameUpdate == null)
            {
                return;
            }

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

            float[] fftPartChannel1 = new float[FFT_INPUT_SIZE];
            float[] fftPartChannel2 = new float[FFT_INPUT_SIZE];

            for (int i = 0; (i + 1) * fftPartChannel1.Length < soundDataChannel1.Length; i++)
            {
                // copy the sample data into the FFT input arrays
                Array.Copy(soundDataChannel1, i * fftPartChannel1.Length, fftPartChannel1, 0, fftPartChannel1.Length);
                Array.Copy(soundDataChannel2, i * fftPartChannel2.Length, fftPartChannel2, 0, fftPartChannel2.Length);

                // apply a hamming window, this prevents frequency artifacts
                // in the FFT from the edge of the frame
                for (int j = 0; j < fftPartChannel1.Length; j++)
                {
                    float coefficient = (float)(.54 - .46 * Math.Cos((2 * Math.PI * j) / (fftPartChannel1.Length - 1)));
                    fftPartChannel1[j] *= coefficient;
                    fftPartChannel2[j] *= coefficient;
                }

                // compute the FFT
                fftChannel1 = new float[StarfieldUtils.MathUtils.FFTTools.RoundToNextPowerOf2(fftPartChannel1.Length)];
                StarfieldUtils.MathUtils.FFTTools.ComputeFFTPolarMag(fftPartChannel1, fftChannel1, out dcComponent);

                fftChannel2 = new float[StarfieldUtils.MathUtils.FFTTools.RoundToNextPowerOf2(fftPartChannel2.Length)];
                StarfieldUtils.MathUtils.FFTTools.ComputeFFTPolarMag(fftPartChannel2, fftChannel2, out dcComponent);

                float bucketWidth = e.Format.SampleRate / (fftChannel1.Length / 2);

                int currentBand = 0;

                Array.Clear(eqDataChannel1, 0, eqDataChannel1.Length);
                Array.Clear(eqDataChannel2, 0, eqDataChannel2.Length);

                // lump into EQ buckets
                for (int j = 0; j < fftChannel1.Length / 2; j++)
                {
                    if ((j * bucketWidth) > Frame.bands[currentBand])
                    {
                        currentBand++;
                    }

                    eqDataChannel1[currentBand] += fftChannel1[j];
                    eqDataChannel2[currentBand] += fftChannel2[j];
                }
            }

            // naive threshold based artifact detection
            int maxBucket = 0;
            float maxBucketValue = 0;
            bool artifactDetected = false;

            if (fftChannel1 != null && eqDataChannel1 != null)
            {
                for (int i = 0; i < eqDataChannel1.Length; i++)
                {
                    if (eqDataChannel1[i] > maxBucketValue)
                    {
                        maxBucket = i;
                        maxBucketValue = eqDataChannel1[i];
                    }

                    if (eqDataChannel1[i] > .9f * threshold)
                    {
                        if (eqDataChannel1[i] > threshold)
                        {
                            threshold = eqDataChannel1[i];
                        }
                        artifactDetected = true;
                    }
                    vuChannel1 = (byte)(eqDataChannel1[maxBucket] * 0xFF);
                }
            }

            // calculate the VU
            maxBucket = 0;
            maxBucketValue = 0;
            if (fftChannel1 != null && eqDataChannel2 != null)
            {
                for (int i = 0; i < eqDataChannel2.Length; i++)
                {
                    if (eqDataChannel2[i] > maxBucketValue)
                    {
                        maxBucket = i;
                        maxBucketValue = eqDataChannel2[i];
                    }
                    if (eqDataChannel2[i] > .9f * threshold)
                    {
                        if(eqDataChannel2[i] > threshold)
                        {
                            threshold = eqDataChannel2[i];
                        }
                        artifactDetected = true;
                    }
                    vuChannel2 = (byte)(eqDataChannel2[maxBucket] * 0xFF);
                }
            }

            if(artifactDetected)
            {
                var artifact = new Artifact();
                artifact.Type = ArtifactDetectionAlgorithm.NaiveImportantNotes;
                DateTime now = DateTime.Now;
                // if we have waited long enough since the last one, notify
                // the user of a new artifact
                if (OnArtifactDetected != null && (now - lastArtifact).TotalMilliseconds > ArtifactDelay)
                {
                    lastArtifact = now;
                    OnArtifactDetected(artifact);
                }
            }
            else
            {
                // reduce the threshold
                threshold *= .99f;
                threshold = Math.Max(threshold, MinimumArtifactThreshold);
            }

            // generate the frame notification
            // note that for the FFT, we're only looking at the last FFT
            Frame frame = new Frame();
            frame.EQ.Add(eqDataChannel1);
            frame.EQ.Add(eqDataChannel2);
            frame.FFT.Add(fftChannel1);
            frame.FFT.Add(fftChannel2);
            frame.Samples.Add(soundDataChannel1);
            frame.Samples.Add(soundDataChannel2);
            frame.VU.Add(vuChannel1);
            frame.VU.Add(vuChannel2);
            if (OnFrameUpdate != null)
            {
                OnFrameUpdate(frame);
            }
        }
    }
}
