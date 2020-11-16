using System;
using System.Net;

namespace uTorrent {
    public class TorrentClient
    {
        public HttpWebResponse Get(string url, string contentType)
        {
            var config = Plugin.Instance.Configuration;
            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(config.userName, config.password);
                httpWebRequest.ContentType = contentType;
                return (HttpWebResponse) httpWebRequest.GetResponse();
            }
            catch(Exception ex)
            {
               UTorrentServerEntryPoint.Instance.Log.Info("UTORRENT CLIENT ERROR: " + ex.Message);
               throw new Exception("Can not create client");
            }
        }
    }
}