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

        // TODO: need to differentiate loopback and external input
        public static BaseSoundProcessor GetSoundProcessor()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    return new NullSoundProcessor();
                case PlatformID.Unix:
                    return new NullSoundProcessor();
                case PlatformID.Win32NT:
                    return new CSCoreLoopbackSoundProcessor();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
