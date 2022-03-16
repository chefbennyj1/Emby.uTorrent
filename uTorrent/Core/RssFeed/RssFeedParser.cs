﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace uTorrent.Core.RssFeed
{
    public class RssFeedParser
    {
        public static List<RssFeed> ParseTorrentRssFeed(List<object[]> obj)
        {
            var list = obj.Select(rss => new RssFeed
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
    }
}
