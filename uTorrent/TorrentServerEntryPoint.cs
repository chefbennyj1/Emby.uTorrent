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
        public ILogger Log { get; }
        public static UTorrentServerEntryPoint Instance { get; private set; }
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