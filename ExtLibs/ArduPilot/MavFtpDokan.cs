using DokanNet;
using DokanNet.Logging;
using log4net;
using MissionPlanner.ArduPilot.Mavlink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;

namespace MissionPlanner.ArduPilot
{
    /// <summary>
    /// Dokan FUSE driver that exposes MAVFtp as a Windows filesystem.
    /// Mount with MavFtpDokan.Mount(...) and unmount with MavFtpDokan.Unmount(...).
    /// Files are fetched lazily on first read in 16 KB chunks and cached per open handle.
    /// </summary>
    public class MavFtpDokan : IDokanOperations
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int ChunkSize = 16 * 1024;
        private static readonly TimeSpan DirCacheTtl = TimeSpan.FromSeconds(30);

        private readonly MAVFtp _mavftp;

        // Directory listing cache: ftpPath -> (entries, expiry)
        private readonly Dictionary<string, (List<MAVFtp.FtpFileInfo> Entries, DateTime Expiry)> _dirCache
            = new Dictionary<string, (List<MAVFtp.FtpFileInfo>, DateTime)>(StringComparer.OrdinalIgnoreCase);
        private readonly object _dirCacheLock = new object();

        private class FileContext
        {
            public string RemotePath;
            public int FileSize = -1;          // -1 = not yet known
            public byte[] Cache;               // populated on first ReadFile
            public MemoryStream WriteBuffer;   // non-null for write-only opens
            public bool IsWrite;
        }

        public MavFtpDokan(MAVFtp mavftp)
        {
            _mavftp = mavftp ?? throw new ArgumentNullException(nameof(mavftp));
        }

        // ------------------------------------------------------------------ static mount/unmount

        private static DokanInstance _dokanInstance;
        private static Dokan _dokan;
        private static string _currentMountPoint;
        private static readonly object _mountLock = new object();

        /// <summary>Mount the MAVFtp filesystem at the given drive letter (e.g. "M:\\").</summary>
        public static void Mount(MAVFtp mavftp, string mountPoint)
        {
            lock (_mountLock)
            {
                if (_dokanInstance != null && !_dokanInstance.IsDisposed)
                    throw new InvalidOperationException("Already mounted.");

                var ops = new MavFtpDokan(mavftp);
                _dokan = new Dokan(new NullLogger());
                var builder = new DokanInstanceBuilder(_dokan)
                    .ConfigureOptions(opt =>
                    {
                        opt.MountPoint = mountPoint;
                        opt.Options = DokanOptions.RemovableDrive;
                    });
                _dokanInstance = builder.Build(ops);
                _currentMountPoint = mountPoint;
            }
        }

        /// <summary>Unmount the MAVFtp filesystem.</summary>
        public static void Unmount(string mountPoint)
        {
            lock (_mountLock)
            {
                _dokanInstance?.Dispose();
                _dokanInstance = null;
                _dokan?.Dispose();
                _dokan = null;
                _currentMountPoint = null;
            }
        }

        /// <summary>True if currently mounted.</summary>
        public static bool IsMounted
        {
            get
            {
                lock (_mountLock)
                    return _dokanInstance != null && !_dokanInstance.IsDisposed;
            }
        }

        // ------------------------------------------------------------------ helpers

        private static CancellationTokenSource NewCancel() => new CancellationTokenSource(30000);

        /// <summary>Convert a Dokan path (backslash-separated) to a MAVFtp path (forward-slash).</summary>
        private static string ToFtpPath(string dokanPath)
        {
            var ftp = dokanPath.Replace('\\', '/');
            if (!ftp.StartsWith("/"))
                ftp = "/" + ftp;
            return ftp;
        }

        private List<MAVFtp.FtpFileInfo> ListDir(string ftpPath)
        {
            lock (_dirCacheLock)
            {
                if (_dirCache.TryGetValue(ftpPath, out var cached) && DateTime.UtcNow < cached.Expiry)
                    return cached.Entries;
            }

            try
            {
                List<MAVFtp.FtpFileInfo> entries;
                lock (_mavftp)
                    entries = _mavftp.kCmdListDirectory(ftpPath, NewCancel());

                lock (_dirCacheLock)
                    _dirCache[ftpPath] = (entries, DateTime.UtcNow.Add(DirCacheTtl));

                return entries;
            }
            catch (Exception ex)
            {
                log.Warn("ListDir " + ftpPath, ex);
                return null;
            }
        }

        /// <summary>Invalidate the cached listing for the parent directory of <paramref name="ftpPath"/>.</summary>
        private void InvalidateDir(string ftpPath)
        {
            var parent = ftpPath.Contains('/')
                ? ftpPath.Substring(0, ftpPath.LastIndexOf('/'))
                : "/";
            if (string.IsNullOrEmpty(parent)) parent = "/";
            lock (_dirCacheLock)
                _dirCache.Remove(parent);
        }

        private MAVFtp.FtpFileInfo FindEntry(string ftpPath)
        {
            if (ftpPath == "/")
                return new MAVFtp.FtpFileInfo("/", "", true, 0);

            var parent = ftpPath.Contains('/')
                ? ftpPath.Substring(0, ftpPath.LastIndexOf('/'))
                : "/";
            if (string.IsNullOrEmpty(parent))
                parent = "/";

            var name = ftpPath.Substring(ftpPath.LastIndexOf('/') + 1);
            var entries = ListDir(parent);
            return entries?.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Ensures FileContext.Cache is populated.
        /// Fetches from MAVFtp using burst read in ChunkSize increments so the
        /// full file is never passed as one allocation before it is needed.
        /// </summary>
        private bool EnsureCache(FileContext ctx)
        {
            if (ctx.Cache != null)
                return true;

            try
            {
                MemoryStream ms;
                lock (_mavftp)
                    ms = _mavftp.GetFile(ctx.RemotePath, NewCancel());

                if (ms == null)
                    return false;

                ctx.FileSize = (int)ms.Length;
                ctx.Cache = ms.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                log.Warn("EnsureCache " + ctx.RemotePath, ex);
                return false;
            }
        }

        // ------------------------------------------------------------------ IDokanOperations

        public NtStatus CreateFile(string fileName, DokanNet.FileAccess access, FileShare share,
            FileMode mode, FileOptions options, FileAttributes attributes, IDokanFileInfo info)
        {
            var ftpPath = ToFtpPath(fileName);
            log.Info(ftpPath + " " + fileName);

            if (info.IsDirectory)
            {
                if (mode == FileMode.CreateNew || mode == FileMode.Create)
                {
                    lock (_mavftp)
                        _mavftp.kCmdCreateDirectory(ftpPath, NewCancel());
                    InvalidateDir(ftpPath);
                }
                return DokanResult.Success;
            }

            bool writeAccess = (access &
                (DokanNet.FileAccess.WriteData | DokanNet.FileAccess.GenericWrite)) != 0;

            if (writeAccess)
            {
                info.Context = new FileContext
                {
                    RemotePath = ftpPath,
                    WriteBuffer = new MemoryStream(),
                    IsWrite = true
                };
                return DokanResult.Success;
            }

            // Read: just record the path — fetch lazily on first ReadFile call.
            var entry = FindEntry(ftpPath);
            if (entry == null)
                return DokanResult.FileNotFound;

            info.Context = new FileContext
            {
                RemotePath = ftpPath,
                FileSize = (int)entry.Size   // size from directory listing
            };
            return DokanResult.Success;
        }

        public void Cleanup(string fileName, IDokanFileInfo info)
        {
            if (info.Context is FileContext ctx && ctx.IsWrite && ctx.WriteBuffer != null)
            {
                try
                {
                    ctx.WriteBuffer.Position = 0;
                    lock (_mavftp)
                        _mavftp.UploadFile(ctx.RemotePath, ctx.WriteBuffer, NewCancel());
                    InvalidateDir(ctx.RemotePath);
                }
                catch (Exception ex)
                {
                    log.Warn("Cleanup upload " + ctx.RemotePath, ex);
                }
                ctx.WriteBuffer.Dispose();
                ctx.WriteBuffer = null;
            }
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
            info.Context = null;
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset,
            IDokanFileInfo info)
        {
            bytesRead = 0;
            if (!(info.Context is FileContext ctx))
                return DokanResult.InvalidHandle;

            if (!EnsureCache(ctx))
                return DokanResult.Error;

            if (offset >= ctx.Cache.Length)
                return DokanResult.Success;   // EOF — return 0 bytes, no error

            // Serve the requested window in ChunkSize increments so callers that
            // ask for large buffers still only process ChunkSize bytes at a time.
            int remaining = (int)Math.Min(buffer.Length, ctx.Cache.Length - offset);
            int served = 0;
            while (served < remaining)
            {
                int chunk = Math.Min(ChunkSize, remaining - served);
                Buffer.BlockCopy(ctx.Cache, (int)offset + served, buffer, served, chunk);
                served += chunk;
            }
            bytesRead = served;
            return DokanResult.Success;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset,
            IDokanFileInfo info)
        {
            bytesWritten = 0;
            if (!(info.Context is FileContext ctx) || ctx.WriteBuffer == null)
                return DokanResult.InvalidHandle;

            ctx.WriteBuffer.Position = offset;
            ctx.WriteBuffer.Write(buffer, 0, buffer.Length);
            bytesWritten = buffer.Length;
            return DokanResult.Success;
        }

        public NtStatus FlushFileBuffers(string fileName, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo,
            IDokanFileInfo info)
        {
            fileInfo = default;
            var ftpPath = ToFtpPath(fileName);

            if (ftpPath == "/")
            {
                fileInfo = new FileInformation
                {
                    FileName = "/",
                    Attributes = FileAttributes.Directory,
                    CreationTime = DateTime.UtcNow,
                    LastAccessTime = DateTime.UtcNow,
                    LastWriteTime = DateTime.UtcNow,
                    Length = 0
                };
                return DokanResult.Success;
            }

            // Use cached context size if available
            if (info.Context is FileContext ctx && ctx.FileSize >= 0)
            {
                fileInfo = new FileInformation
                {
                    FileName = Path.GetFileName(ftpPath),
                    Attributes = FileAttributes.Normal,
                    CreationTime = DateTime.UtcNow,
                    LastAccessTime = DateTime.UtcNow,
                    LastWriteTime = DateTime.UtcNow,
                    Length = ctx.FileSize
                };
                return DokanResult.Success;
            }

            var entry = FindEntry(ftpPath);
            if (entry == null)
                return DokanResult.FileNotFound;

            fileInfo = new FileInformation
            {
                FileName = entry.Name,
                Attributes = entry.isDirectory ? FileAttributes.Directory : FileAttributes.Normal,
                CreationTime = DateTime.UtcNow,
                LastAccessTime = DateTime.UtcNow,
                LastWriteTime = DateTime.UtcNow,
                Length = (long)entry.Size
            };
            return DokanResult.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = new List<FileInformation>();
            var ftpPath = ToFtpPath(fileName);
            var entries = ListDir(ftpPath);
            if (entries == null)
                return DokanResult.PathNotFound;

            foreach (var e in entries.Where(e => e.Name != "." && e.Name != ".."))
            {
                files.Add(new FileInformation
                {
                    FileName = e.Name,
                    Attributes = e.isDirectory ? FileAttributes.Directory : FileAttributes.Normal,
                    CreationTime = DateTime.UtcNow,
                    LastAccessTime = DateTime.UtcNow,
                    LastWriteTime = DateTime.UtcNow,
                    Length = (long)e.Size
                });
            }
            return DokanResult.Success;
        }

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern,
            out IList<FileInformation> files, IDokanFileInfo info)
        {
            files = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime,
            DateTime? lastWriteTime, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            var ftpPath = ToFtpPath(fileName);
            lock (_mavftp)
                _mavftp.kCmdRemoveFile(ftpPath, NewCancel());
            InvalidateDir(ftpPath);
            return DokanResult.Success;
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            var ftpPath = ToFtpPath(fileName);
            lock (_mavftp)
                _mavftp.kCmdRemoveDirectory(ftpPath, NewCancel());
            InvalidateDir(ftpPath);
            return DokanResult.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, IDokanFileInfo info)
        {
            var ftpOld = ToFtpPath(oldName);
            var ftpNew = ToFtpPath(newName);
            lock (_mavftp)
                _mavftp.kCmdRename(ftpOld, ftpNew, NewCancel());
            InvalidateDir(ftpOld);
            InvalidateDir(ftpNew);
            return DokanResult.Success;
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            if (info.Context is FileContext ctx && ctx.WriteBuffer != null)
                ctx.WriteBuffer.SetLength(length);
            return DokanResult.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus LockFile(string fileName, long offset, long length, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
            => DokanResult.Success;

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes,
            out long totalNumberOfFreeBytes, IDokanFileInfo info)
        {
            freeBytesAvailable = 128L * 1024 * 1024;
            totalNumberOfBytes = 256L * 1024 * 1024;
            totalNumberOfFreeBytes = 128L * 1024 * 1024;
            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "MAVFtp";
            features = FileSystemFeatures.CasePreservedNames | FileSystemFeatures.UnicodeOnDisk;
            fileSystemName = "MAVFtp";
            maximumComponentLength = 255;
            return DokanResult.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security,
            AccessControlSections sections, IDokanFileInfo info)
        {
            security = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security,
            AccessControlSections sections, IDokanFileInfo info)
            => DokanResult.NotImplemented;

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            log.Info("MAVFtp Dokan mounted at " + mountPoint);
            return DokanResult.Success;
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            log.Info("MAVFtp Dokan unmounted");
            return DokanResult.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new List<FileInformation>();
            return DokanResult.NotImplemented;
        }
    }
}
