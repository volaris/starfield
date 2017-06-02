using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfield.Networking
{
    public class StarfieldOPC
    {
        public static void PackPixels(StarfieldModel starfield, ref byte[] pixelData)
        {
            // pack the array 
            // [Pixel0 Red, Pixel0 Green, Pixel0 Blue, Pixel1 Red, ..., PixelN Red, PixelN Green, PixelN Blue]
            for (ulong i = 0; i < (ulong)(pixelData.Length / 3); i++)
            {
                ulong x = i / (starfield.NumZ * starfield.NumY);
                ulong z = (i % (starfield.NumZ * starfield.NumY)) / starfield.NumY;
                ulong y = (starfield.NumY - 1) - ((i % (starfield.NumZ * starfield.NumY)) % starfield.NumY);

                pixelData[3 * i] = (byte)(starfield.GetColor((int)x, (int)y, (int)z).R);
                pixelData[(3 * i) + 1] = (byte)(starfield.GetColor((int)x, (int)y, (int)z).G);
                pixelData[(3 * i) + 2] = (byte)(starfield.GetColor((int)x, (int)y, (int)z).B);
            }
        }
    }
}
