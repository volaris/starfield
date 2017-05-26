using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Starfield.Presence
{
    /** <summary>    Used to query presence information. </summary> */
    public interface IPresenceClient
    {
        /**
         * <summary>    Gets the latest. </summary>
         *
         * <returns>    The latest. </returns>
         */

        List<List<Activity>> GetLatest();
    }
}
