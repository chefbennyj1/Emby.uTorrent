using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uTorrent.Helpers;

namespace uTorrent.Api
{
    public class TorrentParser
    {
        public static List<Torrent> ParseTorrentListInfo(List<object[]> obj, string sortBy = null)
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
                TotalBytes            = (string)t[3],
                Progress              = (string)t[4],
                Downloaded            = (string)t[5],
                Uploaded              = (string)t[6],
                Ratio                 = (string)t[7],
                UploadSpeed           = (string)t[8],
                DownloadSpeedFriendly = FileSizeConversions.SizeSuffix(Convert.ToInt64((string)t[9])),
                DownloadSpeed         = (string)t[9],
                Eta                   = Convert.ToInt32((string)t[10]) <= 0 ? t[1].ToString().Trim() != "130" ? "Complete" : "Checking" : (Convert.ToInt32((string)t[10]) / 60) + " minute(s)",
                Label                 = (string)t[11],
                PeersConnected        = (string)t[12],
                PeersInSwarm          = (string)t[13],
                SeedsConnected        = (string)t[14],
                SeedsInSwarm          = (string)t[15],
                Availability          = (string)t[16],
                TorrentQueueOrder     = (string)t[17],
                Remaining             = (string)t[18],
                AddedDate             = getAddedDate(dir, (string)t[2])
            });

            switch (sortBy)
            {
                case "DateAdded" : return list.OrderBy(t => DateTime.Parse(t.AddedDate)).Reverse().ToList();
                case "Name"      : return list.ToList();
                case "FileSize"  : return list.OrderBy(t => Convert.ToInt64(t.TotalBytes)).Reverse().ToList();
            }
            return list.ToList();
        }
        
        //Older versions of uTorrent doesn't have the date added parameter return in the API.
        //We'll use the creation time of the file to get the added date for our service
        private static string getAddedDate(string dir, string torrentName)
        {
            try { 

                foreach (var folder in Directory.GetDirectories(dir))
                {
                    if (folder == $"{dir}\\{torrentName}")
                    {
                        return File.GetCreationTime(folder).ToString("U");
                    }
                }
           
            } catch { }

            return DateTime.Now.ToString("U");
        }
    }
    
}
