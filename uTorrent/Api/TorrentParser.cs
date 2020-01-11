using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using MediaBrowser.Model.Serialization;
using uTorrent.Helpers;

namespace uTorrent.Api
{
    public class TorrentParser
    {
        
        public static List<Torrent> ParseTorrentListInfo(List<object[]> obj, string SortBy = null)
        {
            var config = Plugin.Instance.Configuration;
            var dir = config.FinishedDownloadsLocation ?? string.Empty;

            // ReSharper disable once ComplexConditionExpression
            var list = obj.Select(t => new Torrent
            {
                Hash = t[0].ToString(),
                Status = t[1].ToString() == "201" ? "started" :
                    t[1].ToString() == "136" ? "stopped" :
                    t[1].ToString() == "233" ? "paused" :
                    t[1].ToString() == "130" ? "re-check" : "queued",
                Name = t[2].ToString(),
                Size = FileSizeConversions.SizeSuffix(Convert.ToInt64(t[3])),
                TotalBytes = t[3].ToString(),
                Progress = t[4].ToString(),
                Downloaded = t[5].ToString(),
                Uploaded = t[6].ToString(),
                Ratio = t[7].ToString(),
                UploadSpeed = t[8].ToString(),
                DownloadSpeed = FileSizeConversions.SizeSuffix(Convert.ToInt64(t[9])),
                Eta = t[10].ToString() == "0"
                    ? "Complete"
                    : (Convert.ToInt32(t[10].ToString()) / 60).ToString() + " minute(s)",
                Label = t[11].ToString(),
                PeersConnected = t[12].ToString(),
                PeersInSwarm = t[13].ToString(),
                SeedsConnected = t[14].ToString(),
                SeedsInSwarm = t[15].ToString(),
                Availability = t[16].ToString(),
                TorrentQueueOrder = t[17].ToString(),
                Remaining = t[18].ToString(),
                AddedDate = getAddedDate(dir, t[2].ToString())
            });

            switch (SortBy)
            {
                case "DateAdded":
                    return list.OrderBy(t => DateTime.Parse(t.AddedDate)).Reverse().ToList();
                case "Name":
                    return list.ToList();
                case "FileSize":
                    return list.OrderBy(t => Convert.ToInt64(t.TotalBytes)).Reverse().ToList();
            }
            return list.ToList();
        }
        
        private static string getAddedDate(string dir, string torrentName)
        {
            try { 
                foreach (var folder in Directory.GetDirectories(dir))
                {
                    if (folder == dir + "\\" + torrentName)
                    {
                        return File.GetCreationTime(folder).ToString("U");
                    }
                }
           
            } catch { }
            return string.Empty;
        }
    }
    
}
