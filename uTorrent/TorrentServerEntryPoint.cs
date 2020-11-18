using System;
using System.Threading;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace uTorrent
{
    public class UTorrentServerEntryPoint : IServerEntryPoint
    {
        private IJsonSerializer JsonSerializer { get; }
        public ILogger Log { get; set; }
        public static UTorrentServerEntryPoint Instance { get; set; }
        public UTorrentServerEntryPoint(IAuthenticationRepository auth, ILogManager logMan, IJsonSerializer json)
        {
            Log = logMan.GetLogger(Plugin.Instance.Name);
            JsonSerializer = json;
            Instance = this;
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