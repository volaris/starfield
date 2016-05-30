using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.Sound
{
    public class SoundUtils
    {
        public static float[] convertPCM( short[] leftChannel, short[] rightChannel )
        {
           float[] result = new float[leftChannel.Length];
           for( int i = 0; i < leftChannel.Length; i++ )
              result[i] = (leftChannel[i] + rightChannel[i]) / 2 / 32768.0f;
           return result;
        }
    }
}
