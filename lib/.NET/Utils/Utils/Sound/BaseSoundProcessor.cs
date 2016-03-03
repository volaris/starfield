using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    // This event will be called when a music analysis algorithm detects an
    // artifact. The Artifact class will indicate the type and details about
    // it.
    public delegate void OnArtifactDetectedHandler(Artifact artifact);
    // This event will be called for each frame. It gives information like the
    // volume, FFT, and EQ channels for the frame.
    public delegate void OnFrameUpdateHandler(Frame frame);

    // the artifact types, currently only NaiveImportantNotes is implemented
    public enum ArtifactDetectionAlgorithm
    {
        NaiveImportantNotes,
        TempoChange,
        MoodChange,
        KeyChange,
    }

    // information about the artifact that was detected
    public class Artifact
    {
        public ArtifactDetectionAlgorithm Type;
    }

    // information about the current sound frame
    public class Frame
    {
        // a list of bands, the values are the upper values for their 
        // respective bands
        public static float[] bands = 
        {
            30,
            60,
            150,
            300,
            800,
            2000,
            4000,
            6000,
            12000,
            float.PositiveInfinity
        };

        // the list of EQ values for each channel
        // the float array contains the EQ values, a list entry is generated 
        // for each channel
        public List<float[]> EQ;
        // the raw sample values for each channel
        // the float array contains the sample values, a list entry is generated 
        // for each channel
        public List<float[]> Samples;
        // the list of FFT values for each channel
        // the float array contains the FFT values, a list entry is generated 
        // for each channel
        public List<float[]> FFT;
        // the list of volume values for each channel
        // a list entry is generated for each channel
        public List<byte> VU;

        public Frame()
        {
            EQ = new List<float[]>();
            Samples = new List<float[]>();
            FFT = new List<float[]>();
            VU = new List<byte>();
        }
    }

    // any class that processes sound for the starfield should derive
    // from this class
    public class BaseSoundProcessor
    {
        public event OnArtifactDetectedHandler OnArtifactDetected;
        public event OnFrameUpdateHandler OnFrameUpdate;

        private int artifactDelay = 0;
        private float minimumArtifactThreshold = .15f;// FFT size
        // this size defines the granularity of the frequency buckets 
        private const int FFT_INPUT_SIZE = 512;

        // arrays to hold frame data
        // TODO: don't assume stereo
        private float[] fftChannel1;
        private float[] fftChannel2;
        private float[] eqDataChannel1;
        private float[] eqDataChannel2;
        private byte vuChannel1 = 0;
        private byte vuChannel2 = 0;
        
        // the threshold for naive artifact detection
        private float threshold = 0;
        private Random rand = new Random();
        
        // the last time we detected artifact and sent a notification
        protected DateTime lastArtifact = DateTime.MinValue;

        // the minimum time between artifact notifications
        // TODO: change this to be per artifact type
        public int ArtifactDelay
        {
            get
            {
                return artifactDelay;
            }
            set
            {
                artifactDelay = value;
            }
        }

        public float MinimumArtifactThreshold
        {
            get
            {
                return minimumArtifactThreshold;
            }
            set
            {

            }
        }

        protected BaseSoundProcessor()
        {
            eqDataChannel1 = new float[Frame.bands.Length];
            eqDataChannel2 = new float[Frame.bands.Length];
        }

        protected void ProcessFrame(float[] Channel1, float[] Channel2, int SampleRate)
        {
            float dcComponent;

            // if no one is listening, we shouldn't do all of this calculation
            if(this.OnArtifactDetected == null && this.OnFrameUpdate == null)
            {
                return;
            }

            float[] fftPartChannel1 = new float[FFT_INPUT_SIZE];
            float[] fftPartChannel2 = new float[FFT_INPUT_SIZE];

            for (int i = 0; (i + 1) * fftPartChannel1.Length < Channel1.Length; i++)
            {
                // copy the sample data into the FFT input arrays
                Array.Copy(Channel1, i * fftPartChannel1.Length, fftPartChannel1, 0, fftPartChannel1.Length);
                Array.Copy(Channel2, i * fftPartChannel2.Length, fftPartChannel2, 0, fftPartChannel2.Length);

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

                float bucketWidth = SampleRate / (fftChannel1.Length / 2);

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
            frame.Samples.Add(Channel1);
            frame.Samples.Add(Channel2);
            frame.VU.Add(vuChannel1);
            frame.VU.Add(vuChannel2);
            if (OnFrameUpdate != null)
            {
                OnFrameUpdate(frame);
            }
        }
    }
}
