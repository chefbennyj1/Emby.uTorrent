using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MediaBrowser.Common.Net;
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
        
        [Route("/GetTorrentData", "GET", Summary = "Torrent List End Point")]
        public class TorrentData : IReturn<string>
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


            public List<Torrent> torrents { get; set; }
            public string sizeDownload { get; set; }
            public string sizeSuffixDownload { get; set; }
            public string sizeUpload { get; set; }
            public string sizeSuffixUpload { get; set; }
            public string sizeTotalDriveSpace { get; set; }
            public string sizeTotalDriveSpaceBytes { get; set; }
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

        private static IJsonSerializer JsonSerializer  { get; set; }
        private string CacheId                         { get; set; }
        private List<Torrent> torrents = new List<Torrent>();
        
        private string gui   => "/gui/?";
        private string list  => "&list=1";
        private string token => "token=";
        private string cache => "&cid=";
        
        
        public UTorrentService(IJsonSerializer json, IHttpClient client)
        {
            JsonSerializer = json;
            
        }

        // ReSharper disable MethodNameNotMeaningful

        public string Get(TorrentData request)
        {
            var url = $"http://{request.IpAddress}:{request.Port}{gui}{token}{request.Token}{list}";

            url += CacheId == null ? string.Empty : $"{cache}{CacheId}";

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
                        var data    = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);
                        
                        CacheId     = results.torrentc;

                        //torrents count 0 means first request
                        //torrentsp are only items returned that have changed since the last request
                        //using the cacheId from the prior request

                        var torrentListChanges = 
                            TorrentParser.ParseTorrentListInfo(torrents.Count <= 0 ? results.torrents : results.torrentp, request.SortBy);

                        if (torrents.Count <= 0)
                        {
                            torrents = torrentListChanges; //First update request would hold all the torrents, add them all to the master list.
                        }
                        else
                        {
                            //Should remove any torrent data that has changed from the master list by comparing torrent Hash's
                            torrents = torrents.Where(t1 => torrentListChanges.Any(t2 => t1.Hash != t2.Hash)).ToList();
                            torrents.AddRange(torrentListChanges); //Add the new data to the master list
                        }

                        var totalDownloadRate = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.DownloadSpeed))).Split(' ');
                        var totalUploadRate   = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');

                        return JsonSerializer.SerializeToString(new UTorrentService.TorrentData()
                        {
                            torrents                 = torrents,
                            sizeDownload             = totalDownloadRate[0],
                            sizeSuffixDownload       = totalDownloadRate[1],
                            sizeUpload               = totalUploadRate[0],
                            sizeSuffixUpload         = totalUploadRate[1],
                            sizeTotalDriveSpace      = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                            sizeTotalDriveSpaceBytes = torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()

                        });
                    }
                }
            }
            
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

                        return JsonSerializer.SerializeToString(new UTorrentService.GetToken()
                        {
                            token = (data.Split('>')[2]).Split('<')[0]
                        });
                    }
                }
            }
        }

        public string Get(AddTorrentUrl request)
        {
            try
            {
                const string endpoint = "&action=add-url&s=";
                var url               = $"http://{request.IpAddress}:{request.Port}{endpoint}{request.Token}";
                var httpWebRequest    = (HttpWebRequest)WebRequest.Create(url);

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
