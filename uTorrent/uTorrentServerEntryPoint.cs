using System;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Serialization;


namespace uTorrent
{
    public class UTorrentServerEntryPoint : IServerEntryPoint
    {
        public UTorrentServerEntryPoint Instance { get; set; }
        
        public static IJsonSerializer jsonSerializer { get; set; }
        public UTorrentServerEntryPoint(IJsonSerializer json)
        {
            Instance = this;
            jsonSerializer = json;

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
