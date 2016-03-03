using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    public class SoundProcessor
    {
        private SoundProcessor()
        {

        }

        public static BaseSoundProcessor GetSoundProcessor()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return null;
                case PlatformID.Unix:
                    return null;
                case PlatformID.Win32NT:
                    return new CSCoreLoopbackSoundProcessor();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
