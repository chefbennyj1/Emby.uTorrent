using System;
using System.IO;
using System.Net;
using System.Text;

namespace uTorrentWebSocketMessenger
{
    public class ApiClient
    {
        public static string SendMessageUpdateTorrentData(string url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create($"{url}");
            using (var response = (HttpWebResponse) httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return null;
                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return null;
                    using (var sr = new StreamReader(receiveStream,
                        Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}
