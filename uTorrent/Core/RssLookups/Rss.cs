using System.Collections.Generic;
using System.Xml.Serialization;
using uTorrent.Core.uTorrent;
using uTorrent.Utils;

namespace uTorrent.Core.RssLookups
{
    [XmlRoot(ElementName="image")]
    public class Image {
        [XmlElement(ElementName="title")]
        public string Title { get; set; }
        [XmlElement(ElementName="url")]
        public string Url { get; set; }
        [XmlElement(ElementName="link")]
        public string Link { get; set; }
        [XmlElement(ElementName="width")]
        public string Width { get; set; }
        [XmlElement(ElementName="height")]
        public string Height { get; set; }
        [XmlElement(ElementName="description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName="item")]
    public class Item {
        [XmlElement(ElementName="title")]
        public string Title { get; set; }
        [XmlElement(ElementName="pubDate")]
        public string PubDate { get; set; }
        [XmlElement(ElementName="link")]
        public string Link { get; set; }
        public MediaInfo MediaInfo { get; set; }
    }

    [XmlRoot(ElementName="channel")]
    public class Channel {
        [XmlElement(ElementName="title")]
        public string Title { get; set; }
        [XmlElement(ElementName="link")]
        public string Link { get; set; }
        [XmlElement(ElementName="copyright")]
        public string Copyright { get; set; }
        [XmlElement(ElementName="description")]
        public string Description { get; set; }
        [XmlElement(ElementName="language")]
        public string Language { get; set; }
        [XmlElement(ElementName="webMaster")]
        public string WebMaster { get; set; }
        [XmlElement(ElementName="image")]
        public Image Image { get; set; }
        [XmlElement(ElementName="item")]
        public List<Item> Items { get; set; }
        public string SortName { get; set; }
    }

    [XmlRoot(ElementName="rss")]
    public class Rss {
        [XmlElement(ElementName="channel")]
        public Channel Channel { get; set; }
        [XmlAttribute(AttributeName="version")]
        public string Version { get; set; }
    }
}
