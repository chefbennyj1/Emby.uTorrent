using System.Collections.Generic;

namespace uTorrent.Core.RssFeed
{
    public class RssFeed
    {
      public string FeedId { get; set; }
      public string Enabled{ get; set; }
      public string UseFeedTitle { get; set; }
      public string UserSelected { get; set; }
      public string Programmed { get; set; }
      public string DownloadState { get; set; }
      public string Name { get; set; }
      public string Url { get; set; }
      public string NextUpdate { get; set; }
      public List<Entry> Entries { get; set; }
    }

    public class Entry
    {
       public string name { get; set; }
       public string name_full{ get; set; }
       public string url { get; set; }
       public int quality{ get; set; }
       public int codec { get; set; }
       public int timestamp { get; set; }
      // public int season { get; set; }
       public int episode { get; set; }
       public int episode_to { get; set; }
       public int feed_id { get; set; }
       public bool repack { get; set; }
       public bool in_history { get; set; }
    }
}
