using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Starfield.Presence
{
    public class PresenceClient
    {
        string url;

        public PresenceClient()
        {
            this.url = "http://localhost:8000/activity.json";
        }

        public PresenceClient(string URL)
        {
            this.url = URL;
        }

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
