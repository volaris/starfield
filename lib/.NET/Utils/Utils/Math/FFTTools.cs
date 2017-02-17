using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    /**
     * <summary>    Class providing various computations related to Fast Fourier Transforms. </summary>
     *
     * <remarks>    Volar, 2/13/2017. </remarks>
     */

    public class FFTTools
    {
        /**
         * <summary>    round a value up in powers of 2. </summary>
         *
         * <param name="x"> The value to round. </param>
         *
         * <returns>    The rounded value. </returns>
         */

        public static int RoundToNextPowerOf2(int x)
        {
            if (x <= 1) return 1;
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;
            return x;
        }

        /**
         * <summary>    Calculates the FFT using polar magnitude. </summary>
         *
         * <exception cref="Exception"> Thrown when length of fft result array must be big enough for next power of two. </exception>
         *
         * <param name="samples">       The samples. </param>
         * <param name="fft">           The FFT. </param>
         * <param name="dcComponent">   [out] The device-context component. </param>
         */

        public static void ComputeFFTPolarMag(float[] samples, float[] fft, out float dcComponent)
        {
            int nextPowerOf2 = RoundToNextPowerOf2(samples.Length);
            if (fft.Length < nextPowerOf2)
            {
                throw new Exception("length of fft result array must be big enough for next power of two");
            }
            // copy FFT input into the array that will hold the output so we don't destroy the input samples
            Array.Copy(samples, fft, samples.Length);
            Exocortex.DSP.Fourier.RFFT(fft, Exocortex.DSP.FourierDirection.Forward);
            // divide all bins by N/2 to obtain amplitude of respective 
            // sinusoids(http://www.dspguide.com/ch8/5.htm)
            for (int i = 2; i < nextPowerOf2; i++)
            {
                fft[i] /= nextPowerOf2 / 2;
            }
            //Special treatment of DC component
            fft[0] /= nextPowerOf2;
            // convert from rectangular coordinates to just the magnitude of 
            // polar coordinates - disregarding the phase 'coordinate'
            // (http://www.dsprelated.com/showmessage/58198/1.php) 
            dcComponent = fft[0];
            // skip the first pair as first bin is at index 2 and 3
            for (int i = 1; i < nextPowerOf2 / 2; i++)
            {
                fft[i - 1] = (float)Math.Sqrt(fft[i * 2] * fft[i * 2] + fft[i * 2 + 1] * fft[i * 2 + 1]);
            }
            // zero the rest
            for (int i = nextPowerOf2 / 2 - 1; i < fft.Length; i++)
            {
                fft[i] = 0.0f;
            }
            // the fft array now holds plottable fft results from index 0 to N/2
        }

        /**
         * <summary>    returns the average of all the bytes in an array. </summary>
         *
         * <remarks>    Volar, 2/13/2017. </remarks>
         *
         * <param name="data">  The data. </param>
         *
         * <returns>    The average value. </returns>
         */

        int Average(byte[] data)
        {
            decimal total = 0;
            for (int i = 0; i < data.Length; i++)
            {
                total += data[i];
            }
            total /= (data.Length + 1);
            return (int)total;
        }
    }
}
