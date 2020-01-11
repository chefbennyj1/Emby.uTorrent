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

        [Route("/GetTotalDriveSpace", "GET", Summary = "Torrent List End Point")]
        public class GetTotalDriveSpace : IReturn<string>
        {
            public string size { get; set; }
        }
      
        private static  IJsonSerializer JsonSerializer { get; set; }

        public UTorrentService(IJsonSerializer json)
        {
            JsonSerializer = json;
        }

        // ReSharper disable MethodNameNotMeaningful

        public string Get(GetFiles request)
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
                        torrents = TorrentParser.ParseTorrentListInfo(results.torrents, request.SortBy);

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
