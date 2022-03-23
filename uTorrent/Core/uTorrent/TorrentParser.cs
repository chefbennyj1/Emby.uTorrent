using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Controller.Library;
using uTorrent.Utils;

namespace uTorrent.Core.uTorrent
{
    public class TorrentParser
    {
        public static IEnumerable<Torrent> ParseTorrentData(List<object[]> obj, ILibraryManager libraryManager)
        {
            var finishedDownloadingDirectory = Plugin.Instance.Configuration.FinishedDownloadsLocation;//(string) settings.settings[22][2];
            var mediaInfo                    = new MediaInfoParser();
            var fileSizeConversions          = new FileSizeConversions();

            // ReSharper disable once ComplexConditionExpression
            var list = obj.Select(t => new Torrent
            {
                Hash                  = (string)t[0],
                Status                = ParseStatus(Convert.ToInt32((string)t[1]), Convert.ToInt32((string)t[4])).Trim(), //== "201" ? "started" : (string)t[1] == "136" ? "stopped" : (string)t[1] == "233" ? "paused" : (string)t[1] == "130" ? "re-check" : "queued",
                Name                  = (string)t[2],
                Size                  = fileSizeConversions.SizeSuffix(Convert.ToInt64((string)t[3])),
                MediaInfo             = mediaInfo.GetMediaInfo((string)t[2]),
                TotalBytes            = (string)t[3],
                Progress              = (string)t[4],
                Downloaded            = (string)t[5],
                Uploaded              = (string)t[6],
                Ratio                 = (string)t[7],
                UploadSpeed           = (string)t[8],
                DownloadSpeedFriendly = fileSizeConversions.SizeSuffix(Convert.ToInt64((string)t[9])),
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
                AddedDate             = GetAddedDate(finishedDownloadingDirectory, (string)t[2]),
                Extracted             = IsExtracted(finishedDownloadingDirectory, (string)t[2]),
                
            });

            
            return list;
        }

      
        private static string ParseStatus(int status, int progress)
        {
            switch (status)
            {
                case 201: return "started";
                case 136: return "stopped";
                case 233: return "paused";
                case 130: return "re-check";
                case 137: return progress >= 100 ? "seeding [F]" : "downloading [F]";
                default : return "queued";
            }
        }
        private static TimeSpan CalculateEta(int eta)
        {
            var minutes = eta / 60;
            if (minutes < 0) return TimeSpan.Zero;
            if (minutes > 100000) return TimeSpan.Zero;
            return TimeSpan.FromMinutes(minutes);
        }

        private static bool IsExtracted(string dir, string torrentName)
        {
            try
            {
                var torrentFolder = Path.Combine(dir, torrentName);
                return File.Exists(Path.Combine(torrentFolder, "####emby.extracted####"));
            }
            catch { }

            return false;
        }

        //Older versions of uTorrent don't have the date added parameter return in the API.
        //We'll use the creation time of the file to get the added date for our service
        private static string GetAddedDate(string dir, string torrentName)
        {
            //A Torrent file lives in a parent folder
            try
            {
                foreach (var folder in Directory.GetDirectories(dir))
                {
                    if (folder == Path.Combine(dir, torrentName))
                    {
                        return File.GetCreationTime(folder).ToString("dd/MM/yyyy");
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
        
    }
    
}
