using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    /**
     * <summary>    Generic utilities for audio processing. </summary>
     */

    public class SoundUtils
    {
        /**
         * <summary>    Convert 2 channels of PCM data into an array of floats [0.0,1.0]. </summary>
         *
         * <remarks>    Volar, 2/13/2017. </remarks>
         *
         * <param name="leftChannel">   The left channel. </param>
         * <param name="rightChannel">  The right channel. </param>
         *
         * <returns>    The PCM converted. </returns>
         */

        public static float[] convertPCM( short[] leftChannel, short[] rightChannel )
        {
           float[] result = new float[leftChannel.Length];
           for( int i = 0; i < leftChannel.Length; i++ )
              result[i] = (leftChannel[i] + rightChannel[i]) / 2 / 32768.0f;
           return result;
        }
    }
}
