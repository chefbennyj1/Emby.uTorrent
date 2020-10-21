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
       [Route("/RemoveTorrent", "GET", Summary = "Remove Torrent End Point")]
        public class RemoveTorrent : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            
            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }

        [Route("/StopTorrent", "GET", Summary = "Start Torrent End Point")]
        public class StopTorrent : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            
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
            
            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }
        
        [Route("/AddTorrentUrl", "GET", Summary = "Add Torrent List End Point")]
        public class AddTorrentUrl : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
          
            [ApiMember(Name = "Url", Description = "Url", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Url { get; set; }
        }
        
        [Route("/GetTorrentData", "GET", Summary = "Torrent List End Point")]
        public class TorrentData : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            [ApiMember(Name = "SortBy", Description = "SortBy", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string SortBy { get; set; }
            [ApiMember(Name = "StartIndex", Description = "Start Index", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
            public int? StartIndex { get; set; }

            public List<Torrent> torrents          { get; set; }
            public string sizeDownload             { get; set; }
            public string sizeSuffixDownload       { get; set; }
            public string sizeUpload               { get; set; }
            public string sizeSuffixUpload         { get; set; }
            public string sizeTotalDriveSpace      { get; set; }
            public string sizeTotalDriveSpaceBytes { get; set; }
            public int TotalRecordCount            { get; set; }
        }
        
        [Route("/GetSettingsData", "GET", Summary = "Torrent List End Point")]
        public class Settings : IReturn<string>
        {
            [ApiMember(Name = "Token", Description = "Token", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Token { get; set; }
            
        }

        [Route("/SetSettingsData", "GET", Summary = "Torrent List End Point")]
        public class SetSettings : IReturn<string>
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

            [ApiMember(Name = "SettingName", Description = "Setting Name", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string SettingName { get; set; }
            [ApiMember(Name = "SettingValue", Description = "Setting Value", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string SettingValue { get; set; }
        }

        [Route("/GetToken", "GET", Summary = "Torrent Token End Point")]
        public class GetToken : IReturn<string>
        {
            public string token { get; set; }
        }

        private static IJsonSerializer JsonSerializer  { get; set; }
        
        private string CacheId                         { get; set; }

        private List<Torrent> Torrents = new List<Torrent>();
       
        private string gui         => "/gui/?";
        private string list        => "&list=1";
        private string token       => "token=";
        private string cache       => "&cid=";
        private string getSettings => "&action=getsettings";
        private string setSettings => "&action=setsetting";
        

        public UTorrentService(IJsonSerializer json)
        {
            JsonSerializer = json;
        }

        // ReSharper disable MethodNameNotMeaningful
        public string Get(RemoveTorrent request)
        {
            try
            {
                const string endpoint = "&action=removedata&hash=";
                var config = Plugin.Instance.Configuration;
                if (config.userName is null)
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        { status = "No configuration present" }); 
                }
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{endpoint}{request.Id}";

                var client = new TorrentClient();
                var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");

                return JsonSerializer.SerializeToString(new StatusResponse()
                { status = response.StatusCode.ToString() });

            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = ex.Message });
            }
        }

        public string  Get(SetSettings request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = "No configuration present" }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{setSettings}&s={request.SettingName}&v={request.SettingValue}";
            var client = new TorrentClient();
            var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");
            return JsonSerializer.SerializeToString(new StatusResponse()
            { status = response.StatusCode.ToString() });

        }

        public string  Get(Settings request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = "No configuration present" }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{getSettings}";
            var client = new TorrentClient();
            var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");
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

        public string  Get(TorrentData request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = "No configuration present" }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{list}";

            url += CacheId == null ? string.Empty : $"{cache}{CacheId}";

            var client = new TorrentClient();
            var response = client.GetHttpWebResponse(url,  "application/x-www-form-urlencoded");

            if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

            using (var receiveStream = response.GetResponseStream())
            {
                if (receiveStream == null) return string.Empty;

                using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                {
                    var data = sr.ReadToEnd();

                    var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);

                    CacheId = results.torrentc;

                    //List<Torrent> torrents count would be empty on our first request
                    //We fill our "torrents" list with the entire torrent data from the API.

                    //"torrentp" results are only items that have changed since the last request using the cacheId from the prior request

                    //This means we have to replace the data stored in our "List<Torrent> torrents" which matches the new data returned in torrentp list from the api.

                    var torrentListChanges =
                        TorrentParser.ParseTorrentListInfo(Torrents.Count <= 0 ? results.torrents : results.torrentp, request.SortBy);

                    if (Torrents.Count <= 0)
                    {
                        Torrents = torrentListChanges; // add torrents to the master list.
                    }
                    else
                    {
                        //Remove any torrent data that has changed from the master list by comparing torrent Hash's
                        Torrents = Torrents.Where(t1 => torrentListChanges.Any(t2 => t1.Hash != t2.Hash)).ToList();
                        Torrents.AddRange(torrentListChanges); //Add the new data to the master list
                    }

                    var totalDownloadRate = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt32(t.DownloadSpeed))).Split(' ');
                    var totalUploadRate = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
                    var totalRecordCount = Torrents.Count;

                    //if (request.StartIndex != null)
                    //{
                    //    Torrents = Torrents.GetRange(request.StartIndex.Value, 20);
                    //}

                    return JsonSerializer.SerializeToString(new TorrentData()
                    {
                        torrents = Torrents,
                        TotalRecordCount = totalRecordCount,
                        sizeDownload = totalDownloadRate[0],
                        sizeSuffixDownload = totalDownloadRate[1],
                        sizeUpload = totalUploadRate[0],
                        sizeSuffixUpload = totalUploadRate[1],
                        sizeTotalDriveSpace = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                        sizeTotalDriveSpaceBytes = Torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
                    });
                }
            }


        }

        public string  Get(GetToken request)
        {
            const string endpoint = "/gui/token.html";
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "No configuration present" }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{endpoint}";
            var client = new TorrentClient();
            var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");
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

        public string  Get(AddTorrentUrl request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = "No configuration present" }); 
            }
            try
            {
                const string endpoint = "&action=add-url&s=";
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{endpoint}{request.Url}";
                var client = new TorrentClient();
                var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");

                return JsonSerializer.SerializeToString(new StatusResponse()
                { status = response.StatusCode.ToString() });

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
                var config = Plugin.Instance.Configuration;
                if (config.userName is null)
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        { status = "No configuration present" }); 
                }
                var url = $"http://{config.ipAddress}:{config.port}{endpoint}{request.Token}";

                var client = new TorrentClient();
                var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");
                return JsonSerializer.SerializeToString(new StatusResponse()
                { status = response.StatusCode.ToString() });

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
                var config = Plugin.Instance.Configuration;
                if (config.userName is null)
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                        { status = "No configuration present" }); 
                }
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{request.Token}{endpoint}";
                var client = new TorrentClient();
                var response = client.GetHttpWebResponse(url, "application/x-www-form-urlencoded");
                return JsonSerializer.SerializeToString(new StatusResponse()
                { status = response.StatusCode.ToString() });
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = ex.Message });
            }
        }
        
        private class StatusResponse
        {
            public string status { get; set; }
        }

        
    }
}
