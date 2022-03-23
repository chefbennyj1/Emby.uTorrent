using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using MediaBrowser.Model.System;

namespace uTorrent.Core.RssLookups
{
    public class RssClient
    {
        public Rss Get(string url)
        {
            var config = Plugin.Instance.Configuration;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/xml";
                
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
               
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UTorrentServerEntryPoint.Instance.Log.Info(response.StatusCode.ToString());
                    using (var receiveStream = response.GetResponseStream())
                    {
                        if (receiveStream != null)
                        {
                            using (var sr = new StreamReader(receiveStream))
                            {
                                var data = sr.ReadToEnd();
                                UTorrentServerEntryPoint.Instance.Log.Info(data);
                                var serializer = new XmlSerializer(typeof(Rss));
                                using (StringReader reader = new StringReader(data))
                                {
                                    return (Rss)serializer.Deserialize(reader);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                UTorrentServerEntryPoint.Instance.Log.Info("UTORRENT CLIENT ERROR: " + ex.Message);
                throw new Exception("Can not create client");
            }

            return new Rss();
        }
    }
   
}
