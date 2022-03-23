using System.Collections.Generic;

namespace uTorrent.Core.uTorrent
{
    public class TorrentList
    {
        public int build                     { get; set; }
        public List<List<object>> label      { get; set; }
        public List<object[]> torrents       { get; set; } //The Initial List of Torrents
        public List<object[]> torrentp       { get; set; } //Only the torrents which have changed since the last request
        public string torrentc               { get; set; } //The cacheId from the prior request
        public List<object[]> rssfeeds   { get; set; }
        public List<List<object>> rssfilters { get; set; }
    }
}
