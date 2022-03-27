using System.Collections.Generic;


namespace uTorrent.Core.uTorrent
{
   
    public class Files {
        public int length { get; set; }
        public IList<string> path { get; set; }
    }
    public class Info 
    {
        public string name { get; set; }
        public string pieces { get; set; }
        public int @private { get; set; }
        public string source { get; set; }
        public IList<Files> files { get; set; }
        public string x_cross_seed { get; set; }

    }
    public class TorrentFileInfo 
    {
        public string announce { get; set; }
        public string comment { get; set; }

        //[JsonPropertyName("created by")]
        public string CreatedBy { get; set; }

        //[JsonPropertyName("creation date")]
        public int CreationDate { get; set; }
        public Info info { get; set; }

    }
}
