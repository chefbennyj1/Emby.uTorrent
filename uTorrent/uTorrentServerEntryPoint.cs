using System;
using System.IO;
using System.Reflection;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Logging;

namespace uTorrent
{
    public class UTorrentServerEntryPoint : IServerEntryPoint
    {
        private IAuthenticationRepository AuthenticationRepository { get; set; }
        private ILogger Log { get; set; }
        public UTorrentServerEntryPoint(IAuthenticationRepository auth, ILogManager logMan)
        {
            Log = logMan.GetLogger(Plugin.Instance.Name);
            AuthenticationRepository = auth;
        }

        private static void WriteResourceToFile(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource?.CopyTo(file);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            //Log.Info(Plugin.Instance.DataFolderPath);
            //if (!Directory.Exists(Plugin.Instance.DataFolderPath))
            //{
            //    Directory.CreateDirectory(Plugin.Instance.DataFolderPath);
            //}
            //if (!File.Exists($"{Plugin.Instance.DataFolderPath}/uTorrentWebSocketMessenger.exe"))
            //{
            //    WriteResourceToFile(GetType().Namespace + ".Assets.uTorrentWebSocketMessenger.exe", $"{Plugin.Instance.DataFolderPath}/uTorrentWebSocketMessenger.exe");
            //}
            //var authenticationQueryResult = AuthenticationRepository.Get(new AuthenticationInfoQuery() { DeviceId = "uTorrentEventManager" });
            //if (authenticationQueryResult.TotalRecordCount <= 0)
            //{
            //    AuthenticationRepository.Create(new AuthenticationInfo()
            //    {
            //        AccessToken = new Guid().ToString(),
            //        AppName = "uTorrentWebApi",
            //        AppVersion = "1",
            //        DateCreated = DateTime.Now,
            //        DateLastActivity = DateTimeOffset.Now,
            //        DeviceId = "uTorrentEventManager",
            //        DeviceName = "uTorrentWebApi"
            //    });
            //}
        }
    }
}
