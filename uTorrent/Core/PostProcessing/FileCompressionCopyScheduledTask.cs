using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace uTorrent.Core.PostProcessing
{
    public class FileCompressionCopyScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private ILogger logger                 { get; set; }
        private IFileSystem FileSystem         { get; }
        private ILogManager LogManager         { get; }
        private ISessionManager SessionManager { get; }

        // ReSharper disable once TooManyDependencies
        public FileCompressionCopyScheduledTask(IFileSystem file, ILogManager logManager, ISessionManager sesMan)
        {
            FileSystem     = file;
            LogManager     = logManager;
            SessionManager = sesMan;
            logger = LogManager.GetLogger(Plugin.Instance.Name);
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var config = Plugin.Instance.Configuration;

            if (config.FinishedDownloadsLocation is null || config.EmbyAutoOrganizeFolderPath is null) return;

            var monitoredDirectoryInfo     = FileSystem.GetDirectories(path: config.FinishedDownloadsLocation);

            var monitoredDirectoryContents = monitoredDirectoryInfo.ToList();
            
            logger.Info("Found: " + monitoredDirectoryContents.Count() + " folders in " + config.FinishedDownloadsLocation);
            
            foreach (var mediaFolder in monitoredDirectoryContents)
            {
                //Ignore this directory if there is an 'extraction marker' file present because we have already extracted the contents of this folder.
                if (FileSystem.FileExists(Path.Combine(mediaFolder.FullName, "####emby.extracted####"))) continue;

                logger.Info("New media folder found for extraction: " + mediaFolder.FullName);
                
                IEnumerable<FileSystemMetadata> newMediaFiles;

                try
                {
                    newMediaFiles = FileSystem.GetFiles(mediaFolder.FullName);
                }
                catch(IOException) //The files are in use, get it next time.
                {
                    continue;
                }

                CreateExtractionMarker(mediaFolder.FullName, logger);

                foreach (var file in newMediaFiles)
                {
                    if (file.FullName.IndexOf("sample", StringComparison.OrdinalIgnoreCase) >= 0) continue;

                    logger.Info("File checked: " + file.Name);

                    switch (file.Extension)
                    {
                        case ".rar":

                            logger.Info("Found new compressed file to extract: " + file.Name);
                            await Task.Run(
                                () => UnzipAndCopyFiles.BeginCompressedFileExtraction(file.FullName, file.Name, logger,
                                    progress, config, SessionManager), cancellationToken);
                            break;

                        case ".mkv":
                        case ".avi":
                        case ".mp4":

                            logger.Info("Found new file to copy: " + file.Name);
                            await Task.Run(
                                () => CopyFiles.BeginFileCopy(file.FullName, file.Name, progress,
                                    Plugin.Instance.Configuration, SessionManager), cancellationToken);
                            break;
                    }
                }


            }
            
            progress.Report(100);
            
        }
        
        private static void CreateExtractionMarker(string folderPath, ILogger logger)
        {
            logger.Info("Creating extraction marker: " + folderPath + "\\####emby.extracted####");
            using var sr = new StreamWriter(folderPath + "\\####emby.extracted####");
            sr.Flush();
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                // Every so often
                new TaskTriggerInfo
                {
                    Type          = TaskTriggerInfo.TriggerInterval,
                    IntervalTicks = TimeSpan.FromMinutes(5).Ticks
                }
            };
        }

        public string Name        => "Extract new media files";
        public string Description => "Extract new files available in the configured watch folder into Emby's Auto Organize folder.";
        public string Category    => "Library";
        public string Key         => "FileCompressionCopy";
        public bool IsHidden      => !Plugin.Instance.Configuration.EnableTorrentUnpacking;
        public bool IsEnabled     => Plugin.Instance.Configuration.EnableTorrentUnpacking;
        public bool IsLogged      => true;
    }
}