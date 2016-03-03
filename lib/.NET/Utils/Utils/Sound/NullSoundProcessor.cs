using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    public class NullSoundProcessor : BaseSoundProcessor
    {
        // this class provides the interface but doesn't do any sound processing
        // used for platforms where sound response isn't implemented
        public NullSoundProcessor()
        {
        }
    }
}
