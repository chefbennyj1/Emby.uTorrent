using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using MediaBrowser.Controller.Session;
using uTorrent.Configuration;

// ReSharper disable ComplexConditionExpression
// ReSharper disable TooManyArguments
// ReSharper disable TooManyDependencies

namespace uTorrent.Core.PostProcessing
{
    public static class CopyFiles
    {
        private static ExtractionInfo CurrentObjective { get; set; }
        private static IProgress<double> Progress      { get; set; }
        private static ISessionManager SessionManager  { get; set; }

        public static void BeginFileCopy(string fileFullName, string fileName, IProgress<double> progress, PluginConfiguration config, ISessionManager sessionManager)
        {
            Progress       = progress;
            SessionManager = sessionManager;

            var key          = Path.GetFileNameWithoutExtension(fileName);
            var extractPath  = Path.Combine(config.EmbyAutoOrganizeFolderPath, key);
            var source       = new FileInfo(fileName: fileFullName);
            var destination  = new FileInfo(Path.Combine(extractPath, fileName));

            if (destination.Exists) destination.Delete();

            Directory.CreateDirectory(extractPath);

            CurrentObjective = new ExtractionInfo { Name = Path.GetFileNameWithoutExtension(fileName) };
            

            CopyFileCallbackAction MyCallback(FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred)
            {
                CurrentObjective.Progress = Math.Round((totalBytesTransferred / (double) totalFileSize) * 100.0, 1);
                
                SessionManager.SendMessageToAdminSessions("ExtractionProgress", CurrentObjective, CancellationToken.None);

                Progress.Report(CurrentObjective.Progress);
                
                return CopyFileCallbackAction.CONTINUE;
            }

            CopyFile(source, destination, CopyFileOptions.NONE, MyCallback);
        }

        private static void CopyFile(FileInfo source, FileInfo destination, CopyFileOptions options, CopyFileCallback callback)
        {
            CopyFile(source, destination, options, callback, null);
        }

       
        // ReSharper disable once FlagArgument
        private static void CopyFile(FileInfo source, FileInfo destination, CopyFileOptions options, CopyFileCallback callback, object state)
        {
            if (source == null)                        throw new ArgumentNullException(nameof(source));
            if (destination == null)                   throw new ArgumentNullException(nameof(destination));
            if ((options & ~CopyFileOptions.ALL) != 0) throw new ArgumentOutOfRangeException(nameof(options));
            
            var copyProgressRoutine = callback == null ? null : new CopyProgressRoutine(new CopyProgressData(source, destination, callback, state).CallbackHandler);

            bool cancel = false;

            if (!CopyFileEx(source.FullName, destination.FullName, copyProgressRoutine, IntPtr.Zero, ref cancel, (int) options))
            {
                throw new IOException(new Win32Exception().Message);
            }
        }

        private class CopyProgressData
        {
            private FileInfo Source           { get; }
            private FileInfo Destination      { get; }
            private CopyFileCallback Callback { get; }
            private object State              { get; }

           
            public CopyProgressData(FileInfo source, FileInfo destination, CopyFileCallback callback, object state)
            {
                Source      = source;
                Destination = destination;
                Callback    = callback;
                State       = state;
            }

            public int CallbackHandler
            (long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, int streamNumber, int callbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data)
            {
                return (int) Callback(Source, Destination, State, totalFileSize, totalBytesTransferred);
            }
        }

        private delegate int CopyProgressRoutine
        (long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, int streamNumber, int callbackReason, IntPtr sourceFile, IntPtr destinationFile, IntPtr data);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref bool pbCancel, int dwCopyFlags);
    }

    public delegate CopyFileCallbackAction CopyFileCallback
        (FileInfo source, FileInfo destination, object state, long totalFileSize, long totalBytesTransferred);

    public enum CopyFileCallbackAction
    {
        CONTINUE = 0,
        CANCEL   = 1,
        STOP     = 2,
        QUIET    = 3
    }

    [Flags]
    public enum CopyFileOptions
    {
        NONE                         = 0x0,
        FAIL_IF_DESTINATION_EXISTS   = 0x1,
        RESTARTABLE                  = 0x2,
        ALLOW_DECRYPTED_DESTINATION  = 0x8,
        ALL                          = FAIL_IF_DESTINATION_EXISTS | RESTARTABLE | ALLOW_DECRYPTED_DESTINATION
    }
}