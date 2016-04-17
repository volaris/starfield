using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    public class SoundProcessor
    {
        static BaseSoundProcessor processor = null;
        private SoundProcessor()
        {

        }

        // TODO: need to differentiate loopback and external input
        public static BaseSoundProcessor GetSoundProcessor()
        {
            if (processor != null)
            {
                return processor;
            }
            else
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.MacOSX:
                        processor = new NullSoundProcessor();
                        return processor;
                    case PlatformID.Unix:
                        processor = new NullSoundProcessor();
                        return processor;
                    case PlatformID.Win32NT:
                        processor = new CSCoreLoopbackSoundProcessor();
                        return processor;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
