using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using uTorrent.Core.uTorrent;
using uTorrent.Core.uTorrent.Settings;
using uTorrent.Utils;

namespace uTorrent.Api
{
    public enum SortBy
    {
        NAME_ASCENDING = 0,
        NAME_DESCENDING = 1,
        DATE_ASCENDING = 2,
        DATE_DESCENDING = 3,
        CONTENT_MOVIE = 4,
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
            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string",
                ParameterType = "query", Verb = "GET")]
            public string Id { get; set; }
        }

        [Route("/StartTorrent", "GET", Summary = "Start Torrent End Point")]
        public class StartTorrent : IReturn<string>
        {
            [ApiMember(Name = "Id", Description = "Hash", IsRequired = true, DataType = "string", ParameterType = "query", Verb = "GET")]
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

            [ApiMember(Name = "FilterDownloadingOnly", Description = "Filter Downloading Only", IsRequired = false, DataType = "bool?", ParameterType = "query", Verb = "GET")]
            public bool? FilterDownloadingOnly { get; set; }

            [ApiMember(Name = "SortBy", Description = "SortBy", IsRequired = false, DataType = "int?", ParameterType = "query", Verb = "GET")]
            public SortBy? SortBy { get; set; }

            //public List<RssFeed> RssFeeds { get; set; }
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
        public class SettingsRequest : IReturn<string>
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
        
        private static ISessionManager SessionManager { get; set; }
        private static IJsonSerializer JsonSerializer { get; set; }
        private ILibraryManager LibraryManager        { get; set; }
        private ILogger Log                           { get; }
        private static string CacheId                 { get; set; }

        
        private static string gui         => "/gui/?";
        private static string list        => "&list=1";
        private static string token       => "token=";
        private static string cache       => "&cid=";
        private static string getSettings => "&action=getsettings";
        private static string setSettings => "&action=setsetting";
        private static string remove      => "&action=remove&hash=";
        private static string start       => "&action=start&hash=";
        private static string stop        => "&action=stop&hash=";
        private static string addUrl      => "&action=add-url&s=";

        public UTorrentService(IJsonSerializer json, ILogManager logMan, ISessionManager ses, ILibraryManager libraryManager)
        {
            JsonSerializer = json;
            Log            = logMan.GetLogger(Plugin.Instance.Name);
            SessionManager = ses;
            LibraryManager = libraryManager;
           
        }

        // ReSharper disable MethodNameNotMeaningful
        public string Get(RemoveTorrent request)
        {
            try
            {
                var config = Plugin.Instance.Configuration;
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{remove}{request.Id}";
                var result = uTorrentClient.Instance.Get<object>(url, "application/json");
                return result.ToString();

            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse()
                {
                    status = ex.Message
                });
            }
        }

        public string Get(StartTorrent request)
        {
            try
            {
                var config = Plugin.Instance.Configuration;
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{start}{request.Id}";
                var result = uTorrentClient.Instance.Get<object>(url, "application/json");

                return result.ToString();
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = ex.Message });
            }
        }

        public string Get(StopTorrent request)
        {
            try
            {
                var config = Plugin.Instance.Configuration;
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{stop}{request.Id}";
                var result = uTorrentClient.Instance.Get<object>(url, "application/json");
                return result.ToString();

            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = ex.Message });
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
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{setSettings}&s={request.SettingName}&v={request.SettingValue}";
            uTorrentClient.Instance.Get<object>(url, "application/x-www-form-urlencoded");
            
            return JsonSerializer.SerializeToString(new StatusResponse()
            {
                status = "OK"
            });

        }

        public string Get(SettingsRequest request)
        {
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "No configuration present" });
            }
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{getSettings}";
           
            var response = uTorrentClient.Instance.Get<Settings>(url, "application/json");
            return JsonSerializer.SerializeToString(response);
           
        }

        public string Get(UpdatedTorrentData request)
        {
            var torrents = new List<Torrent>();
            var fileSizeConversions = new FileSizeConversions();
            var config = Plugin.Instance.Configuration;

            if (config.userName is null) return string.Empty;
            if (string.IsNullOrEmpty(CacheId)) return string.Empty;

            var requestToken = GetRequestToken();
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}{cache}{CacheId}";
            var results = uTorrentClient.Instance.Get<TorrentList>(url, "application/json");

            CacheId = results.torrentc;

            //var settings = uTorrentClient.Instance.Get<Settings>($"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{getSettings}", "application/json");

            torrents.AddRange(TorrentParser.ParseTorrentData(results.torrentp, LibraryManager));

            var totalDownloadRate = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32((string)t.DownloadSpeed))).Split(' ');
            var totalUploadRate = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
            var totalRecordCount = torrents.Count;

            return JsonSerializer.SerializeToString(new TorrentData()
            {
                torrents = torrents,
                TotalRecordCount = totalRecordCount,
                sizeDownload = totalDownloadRate[0],
                sizeSuffixDownload = totalDownloadRate[1],
                sizeUpload = totalUploadRate[0],
                sizeSuffixUpload = totalUploadRate[1],
                sizeTotalDriveSpace = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                sizeTotalDriveSpaceBytes = torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
            });
        }


        public string Get(TorrentData request)
        {
            var torrents = new List<Torrent>();
            var fileSizeConversions = new FileSizeConversions();
            //var rssFeed = new List<RssFeed>();
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return string.Empty;
            }
            var requestToken = GetRequestToken();
            var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{list}";
            
            var results = uTorrentClient.Instance.Get<TorrentList>(url, "application/json");

            CacheId = results.torrentc;

            //var settings = uTorrentClient.Instance.Get<Settings>($"http://{config.ipAddress}:{config.port}{gui}{token}{requestToken}{getSettings}", "application/json");
            
            if (request.FilterDownloadingOnly.HasValue)
            {
                if (request.FilterDownloadingOnly.Value)
                {
                    //Only downloading torrents
                    //var temp = new List<Torrent>();
                    torrents.Clear();
                    torrents.AddRange(TorrentParser.ParseTorrentData(results.torrents, LibraryManager).Where(t => Convert.ToInt32(t.Progress) < 1000)); //Add the new data to the temp list
                                                                                                                                        
                }
            }
            else
            {
                //Only seeding torrents
                //var temp = new List<Torrent>();
                torrents.Clear();
                torrents.AddRange(TorrentParser.ParseTorrentData(results.torrents, LibraryManager).Where(t => Convert.ToInt32(t.Progress) >= 1000)); //Add the new data to the temp list
                
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
                    case SortBy.CONTENT_MOVIE:
                        orderList = torrents.OrderBy(t => t.MediaInfo?.MediaType == MediaType.MOVIE).ToList();
                        break;
                    case SortBy.CONTENT_TV_SHOW:
                        orderList = torrents.OrderBy(t => t.MediaInfo?.MediaType == MediaType.SERIES).ToList();
                        break;
                    default:
                        orderList = torrents.OrderBy(t => t.Name).ToList();
                        break;
                }
                torrents = orderList;
            }
           
            var totalDownloadRate = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32((string)t.DownloadSpeed))).Split(' ');
            var totalUploadRate = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt32(t.UploadSpeed))).Split(' ');
            var totalRecordCount = torrents.Count;
            var limit = request.Limit > (totalRecordCount - request.StartIndex) ? totalRecordCount - request.StartIndex : request.Limit;
            var torrentChunk = torrents.GetRange(request.StartIndex, limit);


            Log.Info($"Torrent Chunk size is: {torrentChunk.Count()} of {totalRecordCount}");

            return JsonSerializer.SerializeToString(new TorrentData()
            {
                //RssFeeds = rssFeed,
                torrents                 = torrentChunk,
                TotalRecordCount         = totalRecordCount,
                sizeDownload             = totalDownloadRate[0],
                sizeSuffixDownload       = totalDownloadRate[1],
                sizeUpload               = totalUploadRate[0],
                sizeSuffixUpload         = totalUploadRate[1],
                sizeTotalDriveSpace      = fileSizeConversions.SizeSuffix(torrents.Sum(t => Convert.ToInt64(t.TotalBytes))),
                sizeTotalDriveSpaceBytes = torrents.Sum(t => Convert.ToInt64(t.TotalBytes)).ToString()
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
                var url = $"http://{config.ipAddress}:{config.port}{gui}{token}{GetRequestToken()}{addUrl}{HttpUtility.UrlDecode(request.Url)}";
                uTorrentClient.Instance.Get<object>(url, "application/json");
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "OK" });
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = ex.Message });
            }
        }
        
        private class StatusResponse
        {
            public string status { get; set; }
        }

        private string GetRequestToken()
        {
            const string endpoint = "/gui/token.html";
            var config = Plugin.Instance.Configuration;
            if (config.userName is null)
            {
                return JsonSerializer.SerializeToString(new StatusResponse() { status = "No configuration present" });
            }
            var url = $"http://{config.ipAddress}:{config.port}{endpoint}";
            var data = (string)uTorrentClient.Instance.Get<object>(url, "application/json");
            return (data.Split('>')[2]).Split('<')[0];
        }
    }
}