using System;
using System.Collections.Generic;
using System.Text;

namespace uTorrent.Core.uTorrent
{
    public class ResumeTorrentDat
    {
       public int added_on { get; set; }
        public List<List<int>> antivirus { get; set; }
        public string app_owner { get; set; }
        public string app_url { get; set; }
        public List<object> blocks { get; set; }
        public int blocksize { get; set; }
        public int cached { get; set; }
        public string caption { get; set; }
        public int codec { get; set; }
        public int completed_on { get; set; }
        public int corrupt { get; set; }
        public int dht { get; set; }
        public string download_url { get; set; }
        public int downloaded { get; set; }
        public int downspeed { get; set; }
        public int episode { get; set; }
        public int episode_to { get; set; }
        public string feed_url { get; set; }
        public int hashfails { get; set; }
        public string have { get; set; }
        public string info { get; set; }
       // public int LastSeenComplete { get; set; }
        public int last_active { get; set; }
        public int lsd { get; set; }
        public int moved { get; set; }
        public int order { get; set; }
        public int override_seedsettings { get; set; }
        public string path { get; set; }
        public string peers6 { get; set; }
        public string prio { get; set; }
        public int prio2 { get; set; }
        public int quality { get; set; }
        public string resume_valid { get; set; }
        public string rss_name { get; set; }
        public int runtime { get; set; }
        public int scrambled { get; set; }
        public int season { get; set; }
        public int seedtime { get; set; }
        public int started { get; set; }
        public int superseed { get; set; }
        public int superseed_cur_piece { get; set; }
        public int time { get; set; }
        public int trackermode { get; set; }
        public List<string> trackers { get; set; }
        public int ulslots { get; set; }
        public int uploaded { get; set; }
        public int upspeed { get; set; }
        public int use_utp { get; set; }
        public int visible { get; set; }
        public int wanted_ratio { get; set; }
        public int wanted_seedtime { get; set; }
        public int waste { get; set; }
        public List<object> webseeds { get; set; }
    }
    public class ResumeDat
    {
        public string Fileguard { get; set; }
        public Dictionary<string, ResumeTorrentDat> data { get; set; }
    }
}
