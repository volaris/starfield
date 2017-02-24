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
    public class PresenceClient
    {
        string url;

        /** <summary>    Default constructor. </summary> */
        public PresenceClient()
        {
            this.url = "http://localhost:8000/activity.json";
        }

        /**
         * <summary>    Constructor. </summary>
         *
         * <param name="URL">   URL of the remote presence server. </param>
         */

        public PresenceClient(string URL)
        {
            this.url = URL;
        }

        /**
         * <summary>    Gets the latest. </summary>
         *
         * <returns>    The latest. </returns>
         */

        public List<List<Activity>> GetLatest()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    string json = wc.DownloadString(url);
                    List<List<Activity>> activity = JsonConvert.DeserializeObject<List<List<Activity>>>(json);

                    return activity;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
