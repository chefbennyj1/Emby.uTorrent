using System;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace uTorrent
{
    public class UTorrentServerEntryPoint : IServerEntryPoint
    {
        private IJsonSerializer JsonSerializer { get; }
        private ILogger Log { get; set; }

        public UTorrentServerEntryPoint(IAuthenticationRepository auth, ILogManager logMan, IJsonSerializer json)
        {
            Log = logMan.GetLogger(Plugin.Instance.Name);
            JsonSerializer = json;
        }

        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {

        }
    }
}
