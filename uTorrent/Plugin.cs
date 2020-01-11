using System;
using System.Collections.Generic;
using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using uTorrent.Configuration;

namespace uTorrent
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
        public static Plugin Instance { get; private set; }

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public override string Name => "µTorrent";

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "TorrentPluginConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginConfigurationPage.html",
                    EnableInMainMenu = true,
                    DisplayName = "µTorrent"
                },
                new PluginPageInfo()
                {
                    Name = "TorrentPluginConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginConfigurationPage.js",
                },
                new PluginPageInfo
                {
                    Name = "Chart.bundle.js",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.Chart.bundle.js"
                }
            };
        }

        public override Guid Id => new Guid("b1390c15-5b4f-4038-bb58-b71b9ef4211b");


        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".thumb.jpg");
        }

        public ImageFormat ThumbImageFormat => ImageFormat.Jpg;

    }
}
