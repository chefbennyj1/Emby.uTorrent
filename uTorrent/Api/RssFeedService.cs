using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using uTorrent.Core.RssLookups;
using uTorrent.Core.uTorrent;
using uTorrent.Utils;

namespace uTorrent.Api
{
    public class RssFeedService : IService
    {
        [Route("/GetRssFeeds", "GET", Summary = "Return Rss Feed Info")]
        public class RssFeedRequest : IReturn<List<Rss>>
        {
            [ApiMember(Name = "Url", Description = "Rss Feed Url", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
            public string Url { get; set; }
        }
       
        public List<Rss> Get(RssFeedRequest request)
        {
            var config    = Plugin.Instance.Configuration;
            var rssItems  = new List<Rss>();
            var client    = new RssClient();
            var mediaInfo = new MediaInfoParser();

            foreach (var url in config.RssFeedUrls)
            {
                var feedData = client.Get(HttpUtility.UrlDecode(url));
           
                foreach (var item in feedData.Channel.Items)
                {
                    item.MediaInfo = mediaInfo.GetMediaInfo(item.Title);
                }

                feedData.Channel.SortName = feedData.Channel.Items[0].MediaInfo.SortName;

                rssItems.Add(feedData);
            }

            return rssItems;
        }
    }
}
