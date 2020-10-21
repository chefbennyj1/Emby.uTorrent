using System.Net;

namespace uTorrent {
    public class TorrentClient
    {
        public HttpWebResponse GetHttpWebResponse(string url, string contentType)
        {
            var config = Plugin.Instance.Configuration;
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(config.userName, config.password);
            httpWebRequest.ContentType = contentType;
            return (HttpWebResponse) httpWebRequest.GetResponse();
        }
    }
}