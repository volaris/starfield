using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.SoundUtils
{
    /**
     * <summary>    this class provides the interface but doesn't do any sound processing used for
     *              platforms where sound response isn't implemented. </summary>
     */

    public class NullSoundProcessor : BaseSoundProcessor
    {
        /**
         * <summary>    Default constructor. </summary>
         */

        public NullSoundProcessor()
        {
        }
    }
}
