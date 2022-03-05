using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using uTorrent.Helpers;

namespace uTorrent.Api
{
    public enum SortBy
    {
        NAME_ASCENDING = 0,
        NAME_DESCENDING = 1,
        DATE_ASCENDING = 2,
        DATE_DESCENDING = 3,
        CONTENT_MOVIE =4,
        CONTENT_TV_SHOW = 5
    }

    public class UTorrentService : IService
    {
        [Route("/RemoveTorrent", "GET", Summary = "Remove Torrent End Point")]
        public class RemoveTorrent : IReturn<string>
        {
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
            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }
        
        [Route("/AddTorrentUrl", "POST", Summary = "Add Torrent List End Point")]
        public class AddTorrentUrl : IReturn<string>
        {
            [ApiMember(Name = "Url", Description = "Url", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "POST")]
            public string Url { get; set; }
        }

        [Route("/GetTorrentDataUpdate", "GET", Summary = "Get a list of updated torrent Data")]
        public class UpdatedTorrentData
        {
            public List<Torrent> torrents { get; set; }
        }

        [Route("/GetTorrentData", "GET", Summary = "Torrent List End Point")]
        public class TorrentData : IReturn<string>
        {
            //[ApiMember(Name = "SortBy", Description = "SortBy", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
            //public string SortBy { get; set; }

            [ApiMember(Name = "Limit", Description = "Limit", IsRequired = true, DataType = "int", ParameterType = "query", Verb = "GET")]
            public int Limit { get; set; }

            [ApiMember(Name = "StartIndex", Description = "StartIndex", IsRequired = true, DataType = "int", ParameterType = "query", Verb = "GET")]
            public int StartIndex { get; set; }
           
            [ApiMember(Name = "IsDownloading", Description = "IsDownloading", IsRequired = false, DataType = "bool?", ParameterType = "query", Verb = "GET")]
            public bool? IsDownloading { get; set; }

            [ApiMember(Name = "SortBy", Description = "SortBy", IsRequired = false, DataType = "int?", ParameterType = "query", Verb = "GET")]
            public SortBy? SortBy { get; set; }


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
            
            
        }

        [Route("/SetSettingsData", "GET", Summary = "Torrent List End Point")]
        public class SetSettings : IReturn<string>
        {
           
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

        private static ISessionManager SessionManager { get; set; }
        private  static IJsonSerializer JsonSerializer  { get; set; }
        private ILibraryManager LibraryManager { get; set; }
        private ILogger Log { get; }
        private static string CacheId { get; set; }
        
        private static List<Torrent> Torrents = new List<Torrent>();
        
        private static string gui  => "/gui/?";
        private static string list => "&list=1";
        private string token       => "token=";
        private string cache       => "&cid=";
        private string getSettings => "&action=getsettings";
        private string setSettings => "&action=setsetting";

        
        //private static Timer TorrentMonitor = new Timer(UpdateTorrentData);
        //private static int MonitorUpdate = 5000;
        public UTorrentService(IJsonSerializer json, ILogManager logMan, ISessionManager ses, ILibraryManager libraryManager)
        {
            JsonSerializer = json;
            Log = logMan.GetLogger(Plugin.Instance.Name);
            SessionManager = ses;
            LibraryManager = libraryManager;
        }

        // ReSharper disable MethodNameNotMeaningful
        public string Get(RemoveTorrent request)
        {
            HttpWebResponse response = null;
            try
            {
                const string endpoint = "&action=removedata&hash=";
                var config = Plugin.Instance.Configuration;
                if (config.userName is null)
                {
                    return JsonSerializer.SerializeToString(new StatusResponse()
                    {
                        status = "No configuration present"
                    }); 
                }
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{Token()}{endpoint}{request.Id}";

                var client = new TorrentClient();
                try
                {
                    response = client.Get(url, "application/x-www-form-urlencoded");
                }
                catch
                {
                    Log.Warn("Make sure username and Password are correct");
                }
               

                return JsonSerializer.SerializeToString(new StatusResponse()
                {
                    status = response?.StatusCode.ToString()
                });

            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                {
                    status = ex.Message
                });
            }
        }

        public string Get(SetSettings request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                {
                    status = "No configuration present"
                }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{Token()}{setSettings}&s={request.SettingName}&v={request.SettingValue}";
            var client = new TorrentClient();
            HttpWebResponse response = null;
            try
            {
                response = client.Get(url, "application/x-www-form-urlencoded");
            }
            catch
            {
                Log.Warn("Make sure Username and password is correct");
            }

            return JsonSerializer.SerializeToString(new StatusResponse()
            {
                status = response?.StatusCode.ToString()
            });

        }

        public string Get(Settings request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                    { status = "No configuration present" }); 
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{Token()}{getSettings}";
            var client = new TorrentClient();
            var response = client.Get(url, "application/json");
            if (response.StatusCode != HttpStatusCode.OK) return string.Empty;
            using (var receiveStream = response.GetResponseStream())
            {
                if (receiveStream == null) return string.Empty;
                using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                {
                    var data = sr.ReadToEnd();
                    return data;
                }
            }
        }

        public string Get(UpdatedTorrentData request)
        {
            var torrents = new List<Torrent>();
            var config = Plugin.Instance.Configuration;
            if (config.userName is null) return string.Empty;
            if (string.IsNullOrEmpty(CacheId)) return string.Empty;

            var requestToken = Token();
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}{cache}{CacheId}";
            using (var response = new TorrentClient().Get(url, "application/json"))
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;

                    using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);

                        CacheId = results.torrentc;

                        torrents.AddRange(TorrentParser.ParseTorrentListInfo(results.torrentp)); 
                        var totalDownloadRate = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32((string) t.DownloadSpeed))).Split(' ');
                        var totalUploadRate = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
                        var totalRecordCount = torrents.Count;
                        return JsonSerializer.SerializeToString(new TorrentData()
                        {
                            torrents = torrents,
                            TotalRecordCount = totalRecordCount,
                            sizeDownload = totalDownloadRate[0],
                            sizeSuffixDownload = totalDownloadRate[1],
                            sizeUpload = totalUploadRate[0],
                            sizeSuffixUpload = totalUploadRate[1],
                            sizeTotalDriveSpace = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                            sizeTotalDriveSpaceBytes = torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
                        });

                    }
                }
            }
        }

    
        public string Get(TorrentData request)
        {
            var torrents = new List<Torrent>();
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return string.Empty; 
            }
            var requestToken = Token();
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}";

            using (var response = new TorrentClient().Get(url, "application/json"))
            {
                if (response.StatusCode != HttpStatusCode.OK) return string.Empty;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return string.Empty;

                    using (var sr = new StreamReader(receiveStream,
                        Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                    {
                        var data = sr.ReadToEnd();

                        var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);

                        CacheId = results.torrentc;
                        
                        

                        if (request.IsDownloading.HasValue)
                        {
                            if (request.IsDownloading.Value)
                            {
                                var temp = new List<Torrent>();
                                temp.AddRange(TorrentParser.ParseTorrentListInfo(results.torrents)); //Add the new data to the master list
                                torrents = temp.Where(t => Convert.ToInt32(t.Progress) < 1000).ToList();
                            }
                        }
                        else
                        {
                            torrents.AddRange(TorrentParser.ParseTorrentListInfo(results.torrents)); //Add the new data to the master list
                        }

                        if (request.SortBy.HasValue)
                        {
                            List<Torrent> orderList;
                            switch (request.SortBy.Value)
                            {
                                case SortBy.NAME_ASCENDING:
                                    orderList = torrents.OrderBy(t => t.Name).ToList();
                                    break;
                                case SortBy.NAME_DESCENDING:
                                    orderList = torrents.OrderByDescending(t => t.Name).ToList();
                                    break;
                                case SortBy.DATE_ASCENDING:
                                    orderList = torrents.OrderBy(t => DateTime.ParseExact(t.AddedDate, "dd/MM/yyyy", null)).ToList();
                                    break;
                                case SortBy.DATE_DESCENDING:
                                    orderList = torrents.OrderByDescending(t => DateTime.ParseExact(t.AddedDate, "dd/MM/yyyy", null)).ToList();
                                    break;
                                case SortBy.CONTENT_MOVIE :
                                    orderList = torrents.OrderBy(t => t.MediaInfo?.MediaType == MediaType.MOVIE).ToList();
                                    break;
                                case SortBy.CONTENT_TV_SHOW :
                                    orderList = torrents.OrderBy(t => t.MediaInfo?.MediaType == MediaType.TV_SHOW).ToList();
                                    break;
                                default:
                                    orderList = torrents.OrderBy(t => t.Name).ToList();
                                    break;
                            }
                            torrents = orderList;
                        }
                    }
                }
                


                var totalDownloadRate = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32((string) t.DownloadSpeed))).Split(' ');
                var totalUploadRate = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
                var totalRecordCount = torrents.Count;
                var limit = request.Limit > (totalRecordCount - request.StartIndex) ? totalRecordCount - request.StartIndex : request.Limit;
                var torrentChunk = torrents.GetRange(request.StartIndex, limit);
                

                Log.Info($"Torrent Chunk size is: {torrentChunk.Count()} of {totalRecordCount}");
                //switch (request.SortBy)
                //{
                //    //case "DateAdded" : Torrents = Torrents.OrderBy(t => DateTime.Parse(t.AddedDate)).Reverse().ToList(); break;
                //    //case "Name"      : Torrents = Torrents.OrderBy(t => t.Name).ToList(); break;
                //    //case "FileSize"  : Torrents = Torrents.OrderBy(t => Convert.ToInt64(t.TotalBytes)).Reverse().ToList(); break;
                //}

                return JsonSerializer.SerializeToString(new TorrentData()
                {
                    torrents = torrentChunk,
                    TotalRecordCount = totalRecordCount,
                    sizeDownload = totalDownloadRate[0],
                    sizeSuffixDownload = totalDownloadRate[1],
                    sizeUpload = totalUploadRate[0],
                    sizeSuffixUpload = totalUploadRate[1],
                    sizeTotalDriveSpace = FileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                    sizeTotalDriveSpaceBytes = torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
                });
            }
        }

        public string Get(GetToken request)
        {
            return JsonSerializer.SerializeToString(new GetToken()
            {
                token = Token()
            });
        }

        public string Post(AddTorrentUrl request)
        {
            Log.Info($"Request to Upload torrent from: {request.Url}");
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "No configuration present" }); 
            }
            try
            {
                const string endpoint = "&action=add-url&s=";
                var url               = $"http://{config.ipAddress}:{config.port}{gui}{token}{Token()}{endpoint}{request.Url}";
                var client            = new TorrentClient();
                var response          = client.Get(url, "application/x-www-form-urlencoded");

                return JsonSerializer.SerializeToString(new StatusResponse() { status = response.StatusCode.ToString() });

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
                var url      = $"http://{config.ipAddress}:{config.port}{endpoint}{request.Token}";
                var client   = new TorrentClient();
                var response = client.Get(url, "application/x-www-form-urlencoded");
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
                var url      = $"http://{config.ipAddress}:{config.port}{gui}{token}{Token()}{endpoint}";
                var client   = new TorrentClient();
                var response = client.Get(url, "application/x-www-form-urlencoded");
                return JsonSerializer.SerializeToString(new StatusResponse() { status = response.StatusCode.ToString() });
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

        //private static void UpdateTorrentData(object sender)
        //{
        //    var totalDownloadRate = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt32((string) t.DownloadSpeed))).Split(' ');
        //    var totalUploadRate = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
        //    var totalRecordCount = Torrents.Count;

        //    TorrentMonitor.Change(Timeout.Infinite, Timeout.Infinite);
        //    if (CacheId is null)
        //    {
        //        TorrentMonitor.Change(MonitorUpdate, Timeout.Infinite);
        //        return;
        //    }
        //    var config = Plugin.Instance.Configuration;
        //    if (config.userName is null)
        //    {
        //        return; 
        //    }

        //    var requestToken = Token();
        //    var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}{cache}{CacheId}";

        //    using (var response = new TorrentClient().Get(url, "application/json"))
        //    {
        //        if (response.StatusCode != HttpStatusCode.OK) return;

        //        using (var receiveStream = response.GetResponseStream())
        //        {
        //            if (receiveStream == null) return;

        //            using (var sr = new StreamReader(receiveStream,
        //                Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
        //            {
        //                var data = sr.ReadToEnd();

        //                var results = JsonSerializer.DeserializeFromString<UTorrentResponse>(data);

        //                CacheId = results.torrentc;

        //                var result =  JsonSerializer.SerializeToString(new TorrentData()
        //                {
        //                    torrents                 = TorrentParser.ParseTorrentListInfo(results.torrentp),
        //                    TotalRecordCount         = totalRecordCount,
        //                    sizeDownload             = totalDownloadRate[0],
        //                    sizeSuffixDownload       = totalDownloadRate[1],
        //                    sizeUpload               = totalUploadRate[0],
        //                    sizeSuffixUpload         = totalUploadRate[1],
        //                    sizeTotalDriveSpace      = FileSizeConversions.SizeSuffix(Torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
        //                    sizeTotalDriveSpaceBytes = Torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
        //                });

        //                SessionManager.SendMessageToAdminSessions("TorrentUpdate", result, CancellationToken.None);

        //                TorrentMonitor.Change(MonitorUpdate, Timeout.Infinite);
        //            }
        //        }
        //    }

        //}

        private void UpdateTorrentList()
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return; 
            }

            var requestToken = Token();
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}";

            url += CacheId == null ? string.Empty : $"{cache}{CacheId}";

            using (var response = new TorrentClient().Get(url, "application/json"))
            {
                if (response.StatusCode != HttpStatusCode.OK) return;

                using (var receiveStream = response.GetResponseStream())
                {
                    if (receiveStream == null) return;

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
                            TorrentParser.ParseTorrentListInfo(
                                Torrents.Count <= 0 ? results.torrents : results.torrentp);

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

                        //TorrentMonitor.Change(2000, Timeout.Infinite);
                    }
                }
            }
        }

        private string Token()
        {
            const string endpoint = "/gui/token.html";
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "No configuration present" }); 
            }
            var url      = $"http://{config.ipAddress}:{config.port}{endpoint}";
            var client   = new TorrentClient();
            var response = client.Get(url, "application/x-www-form-urlencoded");
            if (response.StatusCode != HttpStatusCode.OK) return string.Empty;
            using (var receiveStream = response.GetResponseStream())
            {
                if (receiveStream == null) return string.Empty;
                using (var sr = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet ?? throw new InvalidOperationException())))
                {
                    var data = sr.ReadToEnd();

                    return (data.Split('>')[2]).Split('<')[0];
                
                }
            }
        }
    }
}