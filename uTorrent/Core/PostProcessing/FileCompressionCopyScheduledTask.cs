using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Tasks;
using uTorrent.Core.Bencode2Json;
using uTorrent.Core.uTorrent;

namespace uTorrent.Core.PostProcessing
{
    public class FileCompressionCopyScheduledTask : IScheduledTask, IConfigurableScheduledTask
    {
        private ILogger logger                 { get; set; }
        private IFileSystem FileSystem         { get; }
        private ILogManager LogManager         { get; }
        private ISessionManager SessionManager { get; }
        private IJsonSerializer JsonSerializer { get; set; }

        // ReSharper disable once TooManyDependencies
        public FileCompressionCopyScheduledTask(IFileSystem file, ILogManager logManager, ISessionManager sesMan, IJsonSerializer jsonSerializer)
        {
            FileSystem     = file;
            LogManager     = logManager;
            SessionManager = sesMan;
            JsonSerializer = jsonSerializer;
            logger = LogManager.GetLogger(Plugin.Instance.Name);
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var config = Plugin.Instance.Configuration;

            if (config.FinishedDownloadsLocation is null || config.EmbyAutoOrganizeFolderPath is null) return;
            
            //Torrent files may have been saved in the Finished downloads directory, but do not have a parent folder.
            //Move the torrent file into a parent folder.
            //There will be more then one copy of the torrent file in the monitored folder.
            //It is currently unknown how to move a torrent file from its download location, and reload it in uTorrent from it's new directory path.
            //something to do with the resume.dat file in the uTorrent folder.
            foreach (var sourceTorrentFile in Directory.GetFiles(config.FinishedDownloadsLocation))
            {
                var sourceTorrentFileInfo = FileSystem.GetFileInfo(sourceTorrentFile);
                var parentFolderPath      = Path.Combine(config.FinishedDownloadsLocation, Path.GetFileNameWithoutExtension(sourceTorrentFileInfo.Name));
                
                if (Directory.Exists(parentFolderPath)) continue;
               
                Directory.CreateDirectory(parentFolderPath);
               
                logger.Info($"Parent Folder created: {parentFolderPath}");
                try
                {
                    logger.Info($"Copying torrent file {sourceTorrentFileInfo.FullName} to parent folder {parentFolderPath}");
                    File.Copy(sourceTorrentFileInfo.FullName, Path.Combine(parentFolderPath, sourceTorrentFileInfo.Name!));
                }
                catch(Exception ex)
                {
                    logger.Info($"Unable to copy torrent into parent folder: {parentFolderPath}\n {ex.Message}");
                }
            }


            var monitoredDirectoryInfo     = FileSystem.GetDirectories(path: config.FinishedDownloadsLocation);

            var monitoredDirectoryContents = monitoredDirectoryInfo.ToList();
            
            logger.Info("Found: " + monitoredDirectoryContents.Count() + " folders in " + config.FinishedDownloadsLocation);

            var foldersToProcess = monitoredDirectoryContents.Where(folder => !FileSystem.FileExists(Path.Combine(folder.FullName, "####emby.extracted####"))).ToList();

            logger.Info("Found: " + foldersToProcess.Count() + " folders to process " + config.FinishedDownloadsLocation);
            
            foreach (var monitoredTorrentFolder in monitoredDirectoryContents)
            {
               
                //Exaction file is created after the torrent has been extracted
                var extractionMarker = Path.Combine(monitoredTorrentFolder.FullName, "####emby.extracted####");
               
                
                //Ignore this directory if there is an 'extraction marker' file present. The contents of this folder have been extracted for sorting.
                //If the extraction marker exists, and no sorting maker was created, then ignore this folder for extraction, and continue.
                if (FileSystem.FileExists(extractionMarker)) continue;
               
                //The following folder will be extracted.
                logger.Info("New media folder found for extraction: " + monitoredTorrentFolder.FullName);

                IEnumerable<FileSystemMetadata> newMediaFiles;
                try
                {
                    newMediaFiles = FileSystem.GetFiles(monitoredTorrentFolder.FullName);
                }
                catch(IOException) //The files are in use, get it next time.
                {
                    continue;
                }

                CreateExtractionMarker(monitoredTorrentFolder.FullName, logger);

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
            using var sw = new StreamWriter(folderPath + "\\####emby.extracted####");
            sw.Flush();
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