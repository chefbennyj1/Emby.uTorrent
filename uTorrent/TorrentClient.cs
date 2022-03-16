using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Serialization;

namespace uTorrent {
    public class TorrentClient : IServerEntryPoint
    {
        private IJsonSerializer JsonSerializer { get; set; }
        public static TorrentClient Instance { get; set; }
        public TorrentClient(IJsonSerializer json)
        {
            JsonSerializer = json;
            Instance = this;
        }
        public T Get<T>(string url, string contentType) where T : new()
        {
            var config = Plugin.Instance.Configuration;
            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(config.userName, config.password);
                httpWebRequest.ContentType = contentType;
                var response =  (HttpWebResponse) httpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK) 
                {
                    using (var receiveStream = response.GetResponseStream())
                    {
                        if (receiveStream != null)
                        {
                            using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                            {
                                var data = sr.ReadToEnd();
                                return JsonSerializer.DeserializeFromString<T>(data);
                            }
                        }
                        
                    }
                }
            }
            catch(Exception ex)
            {
               UTorrentServerEntryPoint.Instance.Log.Info("UTORRENT CLIENT ERROR: " + ex.Message);
               throw new Exception("Can not create client");
            }

            return new T();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Run()
        {
            //throw new NotImplementedException();
        }
    }
}