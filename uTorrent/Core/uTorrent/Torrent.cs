using uTorrent.Utils;

#nullable enable
namespace uTorrent.Core.uTorrent 
{
    public class Torrent
    {
        public string? Hash                  { get; set; }

        public string? Status                { get; set; }

        public string? Name                  { get; set; }

        public MediaInfo? MediaInfo { get; set; }

        public string? Size                  { get; set; }

        public string? TotalBytes            { get; set; }

        public string? Progress              { get; set; }

        public string? Downloaded            { get; set; }

        public string? Uploaded              { get; set; }

        public string? Ratio                 { get; set; }

        public string? UploadSpeed           { get; set; }

        public string? DownloadSpeed         { get; set; }

        public string? DownloadSpeedFriendly { get; set; }

        public string? Eta                   { get; set; }

        public string? Label                 { get; set; }

        public string? PeersConnected        { get; set; }

        public string? PeersInSwarm          { get; set; }

        public string? SeedsConnected        { get; set; }

        public string? SeedsInSwarm          { get; set; }

        public string? Availability          { get; set; }

        public string? TorrentQueueOrder     { get; set; }

        public string? Remaining             { get; set; }

        public string? Path                  { get; set; }

        public string? AddedDate             { get; set; }

        public bool Extracted                { get; set; }

        public TorrentFileInfo? TorrentFileInfo { get; set; }

    }
    

}