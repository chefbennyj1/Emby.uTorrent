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
                    Name                 = "TorrentPluginDownloadingConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginDownloadingConfigurationPage.html",
                    EnableInMainMenu     = true,
                    DisplayName          = "µTorrent"
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginDownloadingConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginDownloadingConfigurationPage.js",
                },
                new PluginPageInfo
                {
                    Name                 = "TorrentPluginSeedingConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginSeedingConfigurationPage.html",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginSeedingConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginSeedingConfigurationPage.js",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginSettingsConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginSettingsConfigurationPage.html",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginSettingsConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginSettingsConfigurationPage.js",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginUploadConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginUploadConfigurationPage.html",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginUploadConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginUploadConfigurationPage.js",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginPostProcessingConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginPostProcessingConfigurationPage.html",
                },
                new PluginPageInfo()
                {
                    Name                 = "TorrentPluginPostProcessingConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.TorrentPluginPostProcessingConfigurationPage.js",
                },
                new PluginPageInfo()
                {
                Name                 = "Chart.js",
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.Chart.js",
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
