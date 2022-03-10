using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using uTorrent.Helpers;

namespace uTorrent.Api
{
    public class TorrentParser
    {
        [Flags]
        public enum Status
        {
            STARTED = 1 << 0,           //   1
            CHECKING = 1 << 1,          //   2
            START_AFTER_CHECK = 1 << 2, //   4
            CHECKED = 1 << 3,           //   8
            ERROR = 1 << 4,             //  16
            PAUSED = 1 << 5,            //  32
            QUEUED = 1 << 6,            //  64
            LOADED = 1 << 7,            // 128
        }

        //GetStatus((string)t[1]).ToString(), 
        private static Status GetStatus(string v)
        {
            Enum.TryParse(v, out Status result);
            return result;
        }
        public static List<Torrent> ParseTorrentData(List<object[]> obj)
        {
            var config = Plugin.Instance.Configuration;
            var dir    = config.FinishedDownloadsLocation ?? string.Empty;

            // ReSharper disable once ComplexConditionExpression
            var list = obj.Select(t => new Torrent
            {
                Hash                  = (string)t[0],
                Status                = (string)t[1] == "201" ? "started" : (string)t[1] == "136" ? "stopped" : (string)t[1] == "233" ? "paused" : (string)t[1] == "130" ? "re-check" : "queued",
                Name                  = (string)t[2],
                Size                  = FileSizeConversions.SizeSuffix(Convert.ToInt64((string)t[3])),
                MediaInfo = GetMediaInfo((string)t[2]),
                TotalBytes            = (string)t[3],
                Progress              = (string)t[4],
                Downloaded            = (string)t[5],
                Uploaded              = (string)t[6],
                Ratio                 = (string)t[7],
                UploadSpeed           = (string)t[8],
                DownloadSpeedFriendly = FileSizeConversions.SizeSuffix(Convert.ToInt64((string)t[9])),
                DownloadSpeed         = (string)t[9],
                Eta                   = CalculateEta(Convert.ToInt32((string)t[10])).ToString(),
                Label                 = (string)t[11],
                PeersConnected        = (string)t[12],
                PeersInSwarm          = (string)t[13],
                SeedsConnected        = (string)t[14],
                SeedsInSwarm          = (string)t[15],
                Availability          = (string)t[16],
                TorrentQueueOrder     = (string)t[17],
                Remaining             = (string)t[18],
                AddedDate             = GetAddedDate(dir, (string)t[2])
            });

            
            return list.ToList();
        }

        private static TimeSpan CalculateEta(int eta)
        {
            var minutes = eta / 60;
            if (minutes < 0) return TimeSpan.Zero;
            if (minutes > 100000) return TimeSpan.Zero;
            return TimeSpan.FromMinutes(minutes);
        }
        public static List<RssFeed> ParseTorrentRssFeed(List<object[]> obj)
        {
            var list = obj.Select(rss => new RssFeed()
            {
                FeedId = (string)rss[0],
                Enabled = (string)rss[1],
                UseFeedTitle = (string)rss[2],
                UserSelected = (string)rss[3],
                Programmed = (string)rss[4],
                DownloadState = (string)rss[5],
                Url = ((string)rss[6]).Split('|')[1],
                Name = ((string)rss[6]).Split('|')[0],
                NextUpdate = TimeSpan.FromMilliseconds((double)rss[7]).ToString("hh:mm:ss"),
            });
            return list.ToList();
        }

        ////Older versions of uTorrent doesn't have the date added parameter return in the API.
        ////We'll use the creation time of the file to get the added date for our service
        private static string GetAddedDate(string dir, string torrentName)
        {
            //A Torrent file lives in a parent folder
            try
            {
                foreach (var folder in Directory.GetDirectories(dir))
                {
                    if (folder == $"{dir}\\{torrentName}")
                    {
                        return File.GetCreationTime(folder).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                       

                    }
                }

            }
            catch { }

            //A torrent file exists in the root folder, and did not get a parent folder.
            try
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (file.Contains(torrentName))
                    {
                        return File.GetCreationTime(file).ToString("dd/MM/yyyy");
                    }
                }
            }
            catch
            {

            }

            return DateTime.Now.ToString("dd/MM/yyyy");
        }

        private static MediaInfo GetMediaInfo(string fileName)
        {
            var regexDate = new Regex(@"\b(19|20|21)\d{2}\b");
            var regexTvShow = new Regex(@"(.*?)\.S?(\d{1,2})E?(\d{2})\.(.*)", RegexOptions.IgnoreCase);

            

            var dateMatch = regexDate.Match(fileName);
            if (dateMatch.Success)
            {
                var testTvShow = new Regex(@"([Ss](\d{1,2})[Ee](\d{1,2}))", RegexOptions.IgnoreCase);
                if (testTvShow.Matches(fileName).Count < 1)
                {
                    return new MediaInfo()
                    {
                        SortName = fileName.Split(new[] { dateMatch.Value }, StringSplitOptions.None)[0].Replace('.', ' ').TrimEnd(),
                        Resolution = GetStreamResolutionFromFileName(fileName),
                        MediaType = MediaType.MOVIE
                    };
                }
                
            }

            var tvShowMatchCollection = regexTvShow.Matches(fileName);
           
            foreach (Match match in tvShowMatchCollection)
            {
                int.TryParse(match.Groups[2].Value, out var season);
                int.TryParse(match.Groups[3].Value, out var episode); 
                return new MediaInfo()
                {
                    SortName = match.Groups[1].Value.Replace(".", ""),
                    Resolution = GetStreamResolutionFromFileName(fileName),
                    Season = season,
                    Episode = episode,
                    MediaType = MediaType.TV_SHOW 
                };
            }
            return null;
        }
        private static string GetStreamResolutionFromFileName(string sourceFileName)
        {
            var videoResolutionFlags = new[]
            {
                "480p",
                "720p",
                "1080p",
                "2160p", 
                "4K"
            };
            
            foreach(var resolution in videoResolutionFlags)
            {
                if(sourceFileName.Contains(resolution))
                {
                    return resolution;

                }
            }
            return string.Empty;
            
        }

       
    }
    
}
