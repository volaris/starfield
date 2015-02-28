using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmDemo.SoundUtils
{
    public delegate void OnArtifactDetectedHandler(Artifact artifact);
    public delegate void OnFrameUpdateHandler(Frame frame);

    public enum ArtifactDetectionAlgorithm
    {
        NaiveImportantNotes,
        TempoChange,
        MoodChange,
        KeyChange,
    }

    public class Artifact
    {
        public ArtifactDetectionAlgorithm Type;
    }

    public class Frame
    {
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

        public List<float[]> EQ;
        public List<float[]> Samples;
        public List<float[]> FFT;
        public List<byte> VU;

        public Frame()
        {
            EQ = new List<float[]>();
            Samples = new List<float[]>();
            FFT = new List<float[]>();
            VU = new List<byte>();
        }
    }

    interface ISoundProcessor
    {
        event OnArtifactDetectedHandler OnArtifactDetected;
        event OnFrameUpdateHandler OnFrameUpdate;
    }
}
