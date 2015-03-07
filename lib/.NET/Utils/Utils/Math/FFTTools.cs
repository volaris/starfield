using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    public class FFTTools
    {
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
        public static void ComputeFFTPolarMag(float[] samples, float[] fft, out float dcComponent)
        {
            int nextPowerOf2 = RoundToNextPowerOf2(samples.Length);
            if (fft.Length < nextPowerOf2)
                throw new Exception("length of fft result array must be big enough for next power of two");
            Array.Copy(samples, fft, samples.Length);//Copy FFT input into the array that will hold the output so we don't destroy the input samples
            Exocortex.DSP.Fourier.RFFT(fft, Exocortex.DSP.FourierDirection.Forward);
            for (int i = 2; i < nextPowerOf2; i++) fft[i] /= nextPowerOf2 / 2;//Divide all bins by N/2 to obtain amplitude of respective sinusoids(http://www.dspguide.com/ch8/5.htm)
            fft[0] /= nextPowerOf2;//Special treatment of DC component
            //Convert from rectangular coordinates to just the magnitude of polar coordinates - disregarding the phase 'coordinate'
            //(http://www.dsprelated.com/showmessage/58198/1.php) 
            dcComponent = fft[0];
            for (int i = 1; i < nextPowerOf2 / 2; i++)//Skip the first pairas first bin is at index 2 and 3
                fft[i - 1] = (float)Math.Sqrt(fft[i * 2] * fft[i * 2] + fft[i * 2 + 1] * fft[i * 2 + 1]);
            for (int i = nextPowerOf2 / 2 - 1; i < fft.Length; i++)//Zerothe rest
                fft[i] = 0.0f;
            //The fft array now holds plottable fft results from index 0 toN/2
        }

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
