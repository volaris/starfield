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

    // any class that processes sound for the starfield should implement
    // this interface
    public interface ISoundProcessor
    {
        event OnArtifactDetectedHandler OnArtifactDetected;
        event OnFrameUpdateHandler OnFrameUpdate;
    }
}
