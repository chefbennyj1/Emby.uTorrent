using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using uTorrent.Api;
using uTorrent.Helpers;

namespace uTorrent
{
    
    public class UTorrentService : IService
    {
        [Route("/StopTorrent", "GET", Summary = "Start Torrent End Point")]
        public class StopTorrent : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }

            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }

            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }

            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }

            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }

            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }

        [Route("/StartTorrent", "GET", Summary = "Start Torrent End Point")]
        public class StartTorrent : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }

            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }

            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }

            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }

            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }

            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }
        
        [Route("/AddTorrentUrl", "GET", Summary = "Add Torrent List End Point")]
        public class AddTorrentUrl : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }
            [ApiMember(Name = "Url", Description = "Url", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Url { get; set; }
        }

        [Route("/GetDownloads", "GET", Summary = "Downloading Torrent List End Point")]
        public class GetDownloads : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }
        }

        [Route("/GetFiles", "GET", Summary = "Torrent List End Point")]
        public class GetFiles : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }
            [ApiMember(Name = "SortBy", Description = "SortBy", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string SortBy { get; set; }


            public string TorrentFiles { get; set; }
        }

        [Route("/GetToken", "GET", Summary = "Torrent Token End Point")]
        public class GetToken : IReturn<string>
        {
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }

            public string token { get; set; }
        }

        [Route("/GetTotalDriveSpaceUsed", "GET", Summary = "Torrent List End Point")]
        public class GetTotalDriveSpaceUsed : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }

            public string TorrentFiles { get; set; }
        }

        [Route("/GetDownloadRate", "GET", Summary = "")]
        public class DownloadRate : IReturn<string>
        {

            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "IpAddress", Description = "IpAddress", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string IpAddress { get; set; }
            [ApiMember(Name = "Port", Description = "Port", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Port { get; set; }
            [ApiMember(Name = "UserName", Description = "UserName", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string UserName { get; set; }
            [ApiMember(Name = "Password", Description = "Password", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Password { get; set; }
            
            public string size { get; set; }
            public string sizeSuffix { get; set; }
        }
      
        private static  IJsonSerializer JsonSerializer { get; set; }
        private string cacheId { get; set; } = null;
        private List<Torrent> torrents = new List<Torrent>();

        public UTorrentService(IJsonSerializer json)
        {
            JsonSerializer = json;
        }

        // ReSharper disable MethodNameNotMeaningful

        public string Get(GetFiles request)
        {
            
            const string gui = "/gui/?";
            const string list = "&list=1";
            const string token = "token=";
            const string cache = "&cid=";

            var url = $"http://{request.IpAddress}:{request.Port}{gui}{token}{request.Token}{list}";
            url += cacheId == null ? string.Empty : $"{cache}{cacheId}";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;
                    using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);
                        
                        cacheId = results.torrentc;

                        //torrents count 0 means first request
                        //torrentsp are only items returned that have changed since the last request of data using the cacheId from the prior request
                        var torrentListChanges = TorrentParser.ParseTorrentListInfo(torrents.Count <= 0 ? results.torrents : results.torrentp, request.SortBy);

                        if (torrents.Count <= 0) {
                            torrents = torrentListChanges; //First update request would hold all the torrents, add them all to the master list.
                            return JsonSerializer.SerializeToString(torrents); //Just return the list now.
                        }

                        torrents = torrents.Where(t1 => torrentListChanges.Any(t2 => t1.Hash != t2.Hash)).ToList(); //Should remove any torrent data that has changed from the master list by comparing torrent Hash's
                        torrents.AddRange(torrentListChanges); //Add the new data to the mast list

                    }
                }
            }
            return JsonSerializer.SerializeToString(torrents);
        }

        public string Get(GetToken request)
        {
            const string endpoint = "/gui/token.html";
            var url = $"http://{request.IpAddress}:{request.Port}{endpoint}";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;
                    using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        return JsonSerializer.SerializeToString(new GetToken()
                        {
                            token = (data.Split('>')[2]).Split('<')[0]
                        });
                    }
                }
            }
        }

        public string Get(GetDownloads request)
        {
            var torrents = new List<Torrent>();
            const string endpoint = "/gui/?list=1&token=";
            var url = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);
           
            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;
                    using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);
                        torrents = TorrentParser.ParseTorrentListInfo(results.torrents).Where(t => Convert.ToInt32(t.Progress) < 1000).ToList();
                    }
                }
            }

            return JsonSerializer.SerializeToString(torrents);
        }

        public string Get(AddTorrentUrl request)
        {
            try
            {
                const string endpoint = "&action=add-url&s=";
                var url = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        {status = response.StatusCode.ToString()});
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = ex.Message });
            }
        }

        public string Get(StopTorrent request)
        {
            try
            {
                const string endpoint = "&action=stop&hash=";
                var url = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        { status = response.StatusCode.ToString() });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = ex.Message });
            }
        }

        public string Get(StartTorrent request)
        {
            try
            {
                const string endpoint = "&action=start&hash=";
                var url = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        { status = response.StatusCode.ToString() });
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = ex.Message });
            }
        }
        
        public string Get(GetTotalDriveSpaceUsed request)
        {
            var totalBytes = 0L;

            const string endpoint = "/gui/?list=1&token=";
            var url = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);

            using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;
                    using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);
                        var torrents = TorrentParser.ParseTorrentListInfo(results.torrents);
                        foreach (var torrent in torrents)
                        {
                            totalBytes += Convert.ToInt64(torrent.TotalBytes);
                        }
                        return JsonSerializer.SerializeToString(new DriveSizeResponse
                        {
                            size = FileSizeConversions.SizeSuffix(totalBytes),
                            bytes = totalBytes
                        });
                    }
                }
            }
            
        }

        public string Get(DownloadRate request)
        {
            const string gui = "/gui/?";
            const string list = "&list=1";
            const string token = "token=";
            const string cache = "&cid=";

            var url = $"http://{request.IpAddress}:{request.Port}{gui}{token}{request.Token}{list}";
            url += cacheId == null ? string.Empty : $"{cache}{cacheId}";

            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            httpWebRequest.Credentials = new NetworkCredential(request.UserName, request.Password);

            using (var response = (HttpWebResponse) httpWebRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;
                List<Torrent> torrentList;
                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;
                    using (var sr = new StreamReader(receiveStream,
                        Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);

                        //torrents count 0 means first request
                        //torrentsp are only items returned that have changed since the last request of data using the cacheId from the prior request
                        torrentList = TorrentParser.ParseTorrentListInfo(torrents.Count <= 0 ? results.torrents : results.torrentp); 
                        cacheId = results.torrentc;

                    }
                }

                var total = FileSizeConversions.SizeSuffix(torrentList.Sum(t => Convert.ToInt32(t.DownloadSpeed))).Split(' ');
                return JsonSerializer.SerializeToString(new DownloadRate()
                {
                    size = total[0],
                    sizeSuffix = total[1]
                });

            }
        }

        // ReSharper disable  UnusedAutoPropertyAccessor.Local
        private class DriveSizeResponse
        {
            public string size { get; set; }
            public long bytes { get; set; }
        }

        private class StatusResponse
        {
            public string status { get; set; }
        }
    }
}
