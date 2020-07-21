using MediaBrowser.Model.Plugins;

namespace uTorrent.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string ipAddress                 { get; set; }
        public int port                         { get; set; }
        public string userName                  { get; set; }
        public string password                  { get; set; }
        public string FinishedDownloadsLocation { get; set; }
    }
}
