using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.ExtendedObjects;
using log4net;
using Newtonsoft.Json;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class MAVFtp
    {
        /// Identifies Directory returned from List command
        const byte kDirentDir = (byte) 'D';

        /// Identifies File returned from List command
        const byte kDirentFile = (byte) 'F';

        /// Identifies Skipped entry from List command
        const byte kDirentSkip = (byte) 'S';

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly byte _compid;
        private readonly MAVLinkInterface _mavint;
        private readonly byte _sysid;

        private MAVLink.mavlink_file_transfer_protocol_t fileTransferProtocol =
            new MAVLink.mavlink_file_transfer_protocol_t();

        private uint16_t seq_no = 0;

        public MAVFtp(MAVLinkInterface mavint, byte sysid, byte compid)
        {
            _mavint = mavint;
            _sysid = sysid;
            _compid = compid;
        }

        public enum errno
        {
            ///<summary>Operation not permitted</summary>
            EPERM = 1,

            ///<summary>No such file or directory</summary>
            ENOENT = 2,

            ///<summary>No such process</summary>
            ESRCH = 3,

            ///<summary>Interrupted system call</summary>
            EINTR = 4,

            ///<summary>Input/output error</summary>
            EIO = 5,

            ///<summary>No such device or address</summary>
            ENXIO = 6,

            ///<summary>Argument list too long</summary>
            E2BIG = 7,

            ///<summary>Exec format error</summary>
            ENOEXEC = 8,

            ///<summary>Bad file descriptor</summary>
            EBADF = 9,

            ///<summary>No child processes</summary>
            ECHILD = 10,

            ///<summary>Resource temporarily unavailable</summary>
            EAGAIN = 11,

            ///<summary>Cannot allocate memory</summary>
            ENOMEM = 12,

            ///<summary>Permission denied</summary>
            EACCES = 13,

            ///<summary>Bad address</summary>
            EFAULT = 14,

            ///<summary>Block device required</summary>
            ENOTBLK = 15,

            ///<summary>Device or resource busy</summary>
            EBUSY = 16,

            ///<summary>File exists</summary>
            EEXIST = 17,

            ///<summary>Invalid cross-device link</summary>
            EXDEV = 18,

            ///<summary>No such device</summary>
            ENODEV = 19,

            ///<summary>Not a directory</summary>
            ENOTDIR = 20,

            ///<summary>Is a directory</summary>
            EISDIR = 21,

            ///<summary>Invalid argument</summary>
            EINVAL = 22,

            ///<summary>Too many open files in system</summary>
            ENFILE = 23,

            ///<summary>Too many open files</summary>
            EMFILE = 24,

            ///<summary>Inappropriate ioctl for device</summary>
            ENOTTY = 25,

            ///<summary>Text file busy</summary>
            ETXTBSY = 26,

            ///<summary>File too large</summary>
            EFBIG = 27,

            ///<summary>No space left on device</summary>
            ENOSPC = 28,

            ///<summary>Illegal seek</summary>
            ESPIPE = 29,

            ///<summary>Read-only file system</summary>
            EROFS = 30,

            ///<summary>Too many links</summary>
            EMLINK = 31,

            ///<summary>Broken pipe</summary>
            EPIPE = 32,

            ///<summary>Numerical argument out of domain</summary>
            EDOM = 33,

            ///<summary>Numerical result out of range</summary>
            ERANGE = 34,

            ///<summary>Resource deadlock avoided</summary>
            EDEADLK = 35,

            ///<summary>File name too long</summary>
            ENAMETOOLONG = 36,

            ///<summary>No locks available</summary>
            ENOLCK = 37,

            ///<summary>Function not implemented</summary>
            ENOSYS = 38,

            ///<summary>Directory not empty</summary>
            ENOTEMPTY = 39,

            ///<summary>Too many levels of symbolic links</summary>
            ELOOP = 40,

            ///<summary>Resource temporarily unavailable</summary>
            EWOULDBLOCK = 11,

            ///<summary>No message of desired type</summary>
            ENOMSG = 42,

            ///<summary>Identifier removed</summary>
            EIDRM = 43,

            ///<summary>Channel number out of range</summary>
            ECHRNG = 44,

            ///<summary>Level 2 not synchronized</summary>
            EL2NSYNC = 45,

            ///<summary>Level 3 halted</summary>
            EL3HLT = 46,

            ///<summary>Level 3 reset</summary>
            EL3RST = 47,

            ///<summary>Link number out of range</summary>
            ELNRNG = 48,

            ///<summary>Protocol driver not attached</summary>
            EUNATCH = 49,

            ///<summary>No CSI structure available</summary>
            ENOCSI = 50,

            ///<summary>Level 2 halted</summary>
            EL2HLT = 51,

            ///<summary>Invalid exchange</summary>
            EBADE = 52,

            ///<summary>Invalid request descriptor</summary>
            EBADR = 53,

            ///<summary>Exchange full</summary>
            EXFULL = 54,

            ///<summary>No anode</summary>
            ENOANO = 55,

            ///<summary>Invalid request code</summary>
            EBADRQC = 56,

            ///<summary>Invalid slot</summary>
            EBADSLT = 57,

            ///<summary>Resource deadlock avoided</summary>
            EDEADLOCK = 35,

            ///<summary>Bad font file format</summary>
            EBFONT = 59,

            ///<summary>Device not a stream</summary>
            ENOSTR = 60,

            ///<summary>No data available</summary>
            ENODATA = 61,

            ///<summary>Timer expired</summary>
            ETIME = 62,

            ///<summary>Out of streams resources</summary>
            ENOSR = 63,

            ///<summary>Machine is not on the network</summary>
            ENONET = 64,

            ///<summary>Package not installed</summary>
            ENOPKG = 65,

            ///<summary>Object is remote</summary>
            EREMOTE = 66,

            ///<summary>Link has been severed</summary>
            ENOLINK = 67,

            ///<summary>Advertise error</summary>
            EADV = 68,

            ///<summary>Srmount error</summary>
            ESRMNT = 69,

            ///<summary>Communication error on send</summary>
            ECOMM = 70,

            ///<summary>Protocol error</summary>
            EPROTO = 71,

            ///<summary>Multihop attempted</summary>
            EMULTIHOP = 72,

            ///<summary>RFS specific error</summary>
            EDOTDOT = 73,

            ///<summary>Bad message</summary>
            EBADMSG = 74,

            ///<summary>Value too large for defined data type</summary>
            EOVERFLOW = 75,

            ///<summary>Name not unique on network</summary>
            ENOTUNIQ = 76,

            ///<summary>File descriptor in bad state</summary>
            EBADFD = 77,

            ///<summary>Remote address changed</summary>
            EREMCHG = 78,

            ///<summary>Can not access a needed shared library</summary>
            ELIBACC = 79,

            ///<summary>Accessing a corrupted shared library</summary>
            ELIBBAD = 80,

            ///<summary>.lib section in a.out corrupted</summary>
            ELIBSCN = 81,

            ///<summary>Attempting to link in too many shared libraries</summary>
            ELIBMAX = 82,

            ///<summary>Cannot exec a shared library directly</summary>
            ELIBEXEC = 83,

            ///<summary>Invalid or incomplete multibyte or wide character</summary>
            EILSEQ = 84,

            ///<summary>Interrupted system call should be restarted</summary>
            ERESTART = 85,

            ///<summary>Streams pipe error</summary>
            ESTRPIPE = 86,

            ///<summary>Too many users</summary>
            EUSERS = 87,

            ///<summary>Socket operation on non-socket</summary>
            ENOTSOCK = 88,

            ///<summary>Destination address required</summary>
            EDESTADDRREQ = 89,

            ///<summary>Message too long</summary>
            EMSGSIZE = 90,

            ///<summary>Protocol wrong type for socket</summary>
            EPROTOTYPE = 91,

            ///<summary>Protocol not available</summary>
            ENOPROTOOPT = 92,

            ///<summary>Protocol not supported</summary>
            EPROTONOSUPPORT = 93,

            ///<summary>Socket type not supported</summary>
            ESOCKTNOSUPPORT = 94,

            ///<summary>Operation not supported</summary>
            EOPNOTSUPP = 95,

            ///<summary>Protocol family not supported</summary>
            EPFNOSUPPORT = 96,

            ///<summary>Address family not supported by protocol</summary>
            EAFNOSUPPORT = 97,

            ///<summary>Address already in use</summary>
            EADDRINUSE = 98,

            ///<summary>Cannot assign requested address</summary>
            EADDRNOTAVAIL = 99,

            ///<summary>Network is down</summary>
            ENETDOWN = 100,

            ///<summary>Network is unreachable</summary>
            ENETUNREACH = 101,

            ///<summary>Network dropped connection on reset</summary>
            ENETRESET = 102,

            ///<summary>Software caused connection abort</summary>
            ECONNABORTED = 103,

            ///<summary>Connection reset by peer</summary>
            ECONNRESET = 104,

            ///<summary>No buffer space available</summary>
            ENOBUFS = 105,

            ///<summary>Transport endpoint is already connected</summary>
            EISCONN = 106,

            ///<summary>Transport endpoint is not connected</summary>
            ENOTCONN = 107,

            ///<summary>Cannot send after transport endpoint shutdown</summary>
            ESHUTDOWN = 108,

            ///<summary>Too many references: cannot splice</summary>
            ETOOMANYREFS = 109,

            ///<summary>Connection timed out</summary>
            ETIMEDOUT = 110,

            ///<summary>Connection refused</summary>
            ECONNREFUSED = 111,

            ///<summary>Host is down</summary>
            EHOSTDOWN = 112,

            ///<summary>No route to host</summary>
            EHOSTUNREACH = 113,

            ///<summary>Operation already in progress</summary>
            EALREADY = 114,

            ///<summary>Operation now in progress</summary>
            EINPROGRESS = 115,

            ///<summary>Stale file handle</summary>
            ESTALE = 116,

            ///<summary>Structure needs cleaning</summary>
            EUCLEAN = 117,

            ///<summary>Not a XENIX named type file</summary>
            ENOTNAM = 118,

            ///<summary>No XENIX semaphores available</summary>
            ENAVAIL = 119,

            ///<summary>Is a named type file</summary>
            EISNAM = 120,

            ///<summary>Remote I/O error</summary>
            EREMOTEIO = 121,

            ///<summary>Disk quota exceeded</summary>
            EDQUOT = 122,

            ///<summary>No medium found</summary>
            ENOMEDIUM = 123,

            ///<summary>Wrong medium type</summary>
            EMEDIUMTYPE = 124,

            ///<summary>Operation canceled</summary>
            ECANCELED = 125,

            ///<summary>Required key not available</summary>
            ENOKEY = 126,

            ///<summary>Key has expired</summary>
            EKEYEXPIRED = 127,

            ///<summary>Key has been revoked</summary>
            EKEYREVOKED = 128,

            ///<summary>Key was rejected by service</summary>
            EKEYREJECTED = 129,

            ///<summary>Owner died</summary>
            EOWNERDEAD = 130,

            ///<summary>State not recoverable</summary>
            ENOTRECOVERABLE = 131,

            ///<summary>Operation not possible due to RF-kill</summary>
            ERFKILL = 132,

            ///<summary>Memory page has hardware error</summary>
            EHWPOISON = 133,

            ///<summary>Operation not supported</summary>
            ENOTSUP = 95,
        }

        /// @brief Error codes returned in Nak response FTPPayloadHeader.data[0].
        public enum FTPErrorCode : uint8_t
        {
            kErrNone,
            kErrFail,

            /// Unknown failure
            kErrFailErrno,

            /// Command failed, errno sent back in FTPPayloadHeader.data[1]
            kErrInvalidDataSize,

            /// FTPPayloadHeader.size is invalid
            kErrInvalidSession,

            /// Session is not currently open
            kErrNoSessionsAvailable,

            /// All available Sessions in use
            kErrEOF,

            /// Offset past end of file for List and Read commands
            kErrUnknownCommand,

            /// Unknown command opcode
            kErrFailFileExists,

            /// File exists already
            kErrFailFileProtected /// File is write protected
        };

        /// Command opcodes
        public enum FTPOpcode : uint8_t
        {
            ///<summary> ignored, always acked</summary>
            kCmdNone,

            ///<summary> Terminates open Read session</summary>
            kCmdTerminateSession,

            ///<summary> Terminates all open Read sessions</summary>
            kCmdResetSessions,

            ///<summary> List files in &lt;path&gt; from offset</summary>
            kCmdListDirectory,

            ///<summary> Opens file at &lt;path&gt; for reading, returns &lt;session&gt;</summary>
            kCmdOpenFileRO,

            ///<summary> Reads &lt;size&gt; bytes from &lt;offset&gt; in &lt;session&gt;</summary>
            kCmdReadFile,

            ///<summary> Creates file at &lt;path&gt; for writing, returns &lt;session&gt;</summary>
            kCmdCreateFile,

            ///<summary> Writes &lt;size&gt; bytes to &lt;offset&gt; in &lt;session&gt;</summary>
            kCmdWriteFile,

            ///<summary> Remove file at &lt;path&gt;</summary>
            kCmdRemoveFile,

            ///<summary> Creates directory at &lt;path&gt;</summary>
            kCmdCreateDirectory,

            ///<summary> Removes Directory at &lt;path&gt;, must be empty</summary>
            kCmdRemoveDirectory,

            ///<summary> Opens file at &lt;path&gt; for writing, returns &lt;session&gt;</summary>
            kCmdOpenFileWO,

            ///<summary> Truncate file at &lt;path&gt; to &lt;offset&gt; length</summary>
            kCmdTruncateFile,

            ///<summary> Rename path1 to path2</summary>
            kCmdRename,

            ///<summary> Calculate CRC32 for file at &lt;path&gt;</summary>
            kCmdCalcFileCRC32,

            ///<summary> Burst download session file</summary>
            kCmdBurstReadFile,

            ///<summary> Ack response</summary>
            kRspAck = 128,

            ///<summary> Nak response</summary>
            kRspNak
        };

        public List<FtpFileInfo> GetDirectory(string dir = "/")
        {
            var answer = kCmdListDirectory(dir);
            return answer;
        }

        public MemoryStream GetFile(string file)
        {
            kCmdOpenFileRO(file, out var size);
            if (size == -1)
                return null;
            //  var answer = kCmdReadFile(file, size);
            var answer = kCmdBurstReadFile(file, size);
            kCmdResetSessions();
            return answer;
        }

        public void kCmdOpenFileRO(string file, out int size)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdOpenFileRO,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            size = -1;
            var localsize = size;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                localsize = BitConverter.ToInt32(ftphead.data, 0);
                log.Info(ftphead.req_opcode + " " + file + " " + localsize);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
        }

        public void test()
        {
            // watch all traffic
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                log.Debug(ftphead);
                Console.WriteLine(ftphead);

                return true;
            });

                var dir = kCmdListDirectory("/");
            kCmdCreateDirectory("/testdir");
            foreach (var ftpFileInfo in dir)
            {
                Console.WriteLine(ftpFileInfo.FullName + " " + ftpFileInfo.Size);
            }
            int size = 0;
            kCmdCreateFile("/testdir/test.txt", ref size);
            File.WriteAllText("test.txt", "this is a test " + DateTime.Now);
            kCmdWriteFile("test.txt");
            kCmdResetSessions();
            int crc = 0;
            kCmdCalcFileCRC32("/testdir/test.txt", ref crc);

            kCmdRename("/testdir/test.txt","/testdir/test2.txt");

            kCmdRemoveFile("/testdir/test2.txt");

            kCmdRemoveDirectory("/testdir");

            _mavint.UnSubscribeToPacketType(sub);
            /*
            var dirlist = GetDirectory();
            foreach (var ftpFileInfo in dirlist)
            {
                if (ftpFileInfo.isDirectory)
                {
                    if (ftpFileInfo.Name == ".." || ftpFileInfo.Name == ".")
                        continue;
                    Directory.CreateDirectory("." + ftpFileInfo.FullName);
                    //var list2 = ftp.GetDirectory(ftpFileInfo.FullName);
                }
                else
                {
                    var stream = GetFile(ftpFileInfo.FullName);
                    if (stream != null)
                    {
                        stream.CopyTo(File.OpenWrite("." + ftpFileInfo.FullName));
                        stream.Dispose();
                    }
                }
            }
            */
        }

        private MemoryStream kCmdBurstReadFile(string file, int size)
        {
            RetryTimeout timeout = new RetryTimeout();
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdBurstReadFile,
                seq_number = seq_no++,
                session = 0,
                offset = 0
            };
            fileTransferProtocol.payload = payload;
            log.Info("get " + payload.opcode + " " + file + " " + size);
            MemoryStream answer = new MemoryStream();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                //log.Debug(ftphead);
                //Console.WriteLine(ftphead);
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrEOF)
                        timeout.Complete = true;
                    return true;
                }

                // not for us or bad seq no - we get multiple packets to one request, so seq_no can be ignored here
                if (payload.opcode != ftphead.req_opcode /*|| payload.seq_number + 1 != ftphead.seq_number*/)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                // reject bad packets
                if (ftphead.offset > size || ftphead.size > size || ftphead.offset + ftphead.size > size ||
                    answer.Length == 0 && ftphead.offset > 0 && size < 239)
                    return true;
                answer.Seek(ftphead.offset, SeekOrigin.Begin);
                answer.Write(ftphead.data, 0, ftphead.size);
                timeout.ResetTimeout();
                //log.Debug(ftphead);
                seq_no = (ushort) (ftphead.seq_number + 1);
                // if rerequest needed
                payload.offset = ftphead.offset + ftphead.size;
                payload.seq_number = seq_no;
                fileTransferProtocol.payload = payload;
                if (ftphead.offset + ftphead.size >= size)
                {
                    log.InfoFormat("Done {0} {1} ", ftphead.burst_complete, ftphead.offset + ftphead.size);
                    timeout.Complete = true;
                    return true;
                }

                if (ftphead.burst_complete == 1)
                {
                    log.InfoFormat("next burst {0} {1} ", ftphead.burst_complete, ftphead.offset + ftphead.size);
                    log.Debug(payload);
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                }

                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            var success = timeout.DoWork();
            if (!success)
            {
                log.Error("failed retries");
            }

            _mavint.UnSubscribeToPacketType(sub);
            answer.Position = 0;
            return answer;
        }

        private void kCmdCalcFileCRC32(string file, ref int crc32)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdCalcFileCRC32,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            crc32 = -1;
            var localcrc32 = crc32;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                localcrc32 = BitConverter.ToInt32(ftphead.data, 0);
                log.Info(ftphead.req_opcode + " " + file + " " + localcrc32);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            crc32 = localcrc32;
        }

        private void kCmdCreateDirectory(string file)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdCreateDirectory,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);

                        if (_ftp_errno == errno.EEXIST)
                            timeout.Complete = true;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                log.Info(ftphead.req_opcode + " " + file + " ");
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdCreateFile(string file, ref int size)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdCreateFile,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            size = -1;
            var localsize = size;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                localsize = BitConverter.ToInt32(ftphead.data, 0);
                log.Info(ftphead.req_opcode + " " + file + " " + localsize);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
        }

        private List<FtpFileInfo> kCmdListDirectory(string dir)
        {
            List<FtpFileInfo> answer = new List<FtpFileInfo>();
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdListDirectory,
                data = ASCIIEncoding.ASCII.GetBytes(dir),
                seq_number = seq_no++,
                offset = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL,
                message =>
                {
                    var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                    FTPPayloadHeader ftphead = msg.payload;
                    // error at far end
                    if (ftphead.opcode == FTPOpcode.kRspNak)
                    {
                        var errorcode = (FTPErrorCode) ftphead.data[0];
                        if (errorcode == FTPErrorCode.kErrFailErrno)
                        {
                            var _ftp_errno = (errno) ftphead.data[1];
                            log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                        }
                        else
                        {
                            log.Error(ftphead.req_opcode + " " + errorcode);
                        }

                        if (errorcode == FTPErrorCode.kErrEOF)
                            timeout.Complete = true;
                        log.Error(ftphead.req_opcode + " " + errorcode);
                        return true;
                    }

                    // not for us or bad seq no
                    if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                        return true;
                    // only ack's
                    if (ftphead.opcode != FTPOpcode.kRspAck)
                        return true;
                    var requested_offset = ftphead.offset;
                    var offset = 0;
                    while (offset < ftphead.size)
                    {
                        var b = ftphead.data[offset++];
                        switch (b)
                        {
                            case kDirentFile:
                                var filename = new StringBuilder();
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                    if (b != 0x0)
                                        filename.Append((char) b);
                                }

                                var items = filename.ToString().Split('\t');
                                var size = int.Parse(items[1]);
                                answer.Add(new FtpFileInfo(items[0], dir, false, (int) size));
                                break;
                            case kDirentDir:
                                var name = new StringBuilder();
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                    if (b != 0x0)
                                        name.Append((char) b);
                                }

                                answer.Add(new FtpFileInfo(name.ToString(), dir, true));
                                break;
                            case kDirentSkip:
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                }

                                answer.Add(new FtpFileInfo("", dir, true));
                                break;
                            default:
                                var nameextra = new StringBuilder();
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                    if (b != 0x0)
                                        nameextra.Append((char) b);
                                }

                                if (nameextra.ToString() != "")
                                    answer.Add(new FtpFileInfo(nameextra.ToString(), dir, false));
                                break;
                        }
                    }

                    payload.offset = (uint) answer.Count;
                    payload.seq_number = seq_no++;
                    fileTransferProtocol.payload = payload;
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                    timeout.ResetTimeout();
                    return true;
                });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return answer;
        }

        private void kCmdOpenFileWO(string file, ref int size)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdOpenFileWO,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            size = -1;
            var localsize = size;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                localsize = BitConverter.ToInt32(ftphead.data, 0);
                log.Info(ftphead.req_opcode + " " + file + " " + localsize);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
        }

        private MemoryStream kCmdReadFile(string file, int size)
        {
            RetryTimeout timeout = new RetryTimeout();
            KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdReadFile,
                seq_number = seq_no++,
                offset = 0,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            log.Info("get " + payload.opcode + " " + file + " " + size);
            MemoryStream answer = new MemoryStream();
            sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                // log.Debug(ftphead.req_opcode + " " + file + " " + ftphead.size + " " + ftphead.offset);
                answer.Seek(ftphead.offset, SeekOrigin.Begin);
                answer.Write(ftphead.data, 0, ftphead.size);
                if (ftphead.offset + ftphead.size >= size)
                {
                    timeout.Complete = true;
                    return true;
                }

                payload.offset = ftphead.offset + ftphead.size;
                payload.seq_number = seq_no++;
                fileTransferProtocol.payload = payload;
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                timeout.ResetTimeout();
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            answer.Position = 0;
            return answer;
        }

        private void kCmdRemoveDirectory(string file)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdRemoveDirectory,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                log.Info(ftphead.req_opcode + " " + file + " ");
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdRemoveFile(string file)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdRemoveFile,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                log.Info(ftphead.req_opcode + " " + file + " ");
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdRename(string src, string dest)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdRename,
                data = ASCIIEncoding.ASCII.GetBytes(src + "\0" + dest),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                log.Info(ftphead.req_opcode + " " + src + " -> " + dest);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdResetSessions()
        {
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdResetSessions,
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            RetryTimeout timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                log.Info(ftphead.opcode + " " + ftphead.req_opcode + " " + ftphead.size);
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            log.Info(payload.opcode);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdTerminateSession()
        {
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdTerminateSession,
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            RetryTimeout timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                log.Info(ftphead.opcode + " " + ftphead.req_opcode + " " + ftphead.size);
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            log.Info(payload.opcode);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdTruncateFile(string file)
        {
            fileTransferProtocol.target_system = _sysid;
            fileTransferProtocol.target_component = _compid;
            fileTransferProtocol.target_network = 0;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdTruncateFile,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            var timeout = new RetryTimeout();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode) ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (errno) ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                        kCmdResetSessions();
                    return true;
                }

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;
                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;
                log.Info(ftphead.req_opcode + " " + file + " ");
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
        }

        private void kCmdWriteFile(string srcfile)
        {
            RetryTimeout timeout = new RetryTimeout();
            KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdWriteFile,
                seq_number = seq_no++,
                offset = 0,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            log.Info("get " + payload.opcode + " " + srcfile + " ");
            using (var stream = new BufferedStream(File.OpenRead(srcfile), 1024 * 1024))
            {
                var size = stream.Length;
                var bytes_read = 0;
                if (size == 0)
                    return;
                sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
                {
                    var msg = (MAVLink.mavlink_file_transfer_protocol_t) message.data;
                    FTPPayloadHeader ftphead = msg.payload;
                    // error at far end
                    if (ftphead.opcode == FTPOpcode.kRspNak)
                    {
                        var errorcode = (FTPErrorCode) ftphead.data[0];
                        if (errorcode == FTPErrorCode.kErrFailErrno)
                        {
                            var _ftp_errno = (errno) ftphead.data[1];
                            log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                        }
                        else
                        {
                            log.Error(ftphead.req_opcode + " " + errorcode);
                        }

                        return true;
                    }

                    // not for us or bad seq no
                    if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                        return true;
                    // only ack's
                    if (ftphead.opcode != FTPOpcode.kRspAck)
                        return true;
                    if (stream.Position >= size)
                    {
                        timeout.Complete = true;
                        return true;
                    }

                    // send next
                    payload.offset = (uint32_t) stream.Position;
                    bytes_read = stream.Read(payload.data, 0, payload.data.Length);
                    payload.size = (uint8_t) bytes_read;
                    payload.seq_number = seq_no++;
                    fileTransferProtocol.payload = payload;
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                    timeout.ResetTimeout();
                    return true;
                });
                // fill buffer
                payload.offset = (uint32_t) stream.Position;
                payload.data = new byte[239];
                bytes_read = stream.Read(payload.data, 0, payload.data.Length);
                payload.size = (uint8_t) bytes_read;
                //  package it
                fileTransferProtocol.payload = payload;
                // send it
                timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                timeout.DoWork();
            }

            _mavint.UnSubscribeToPacketType(sub);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 251)]
        public struct FTPPayloadHeader
        {
            /// sequence number for message
            public uint16_t seq_number;

            /// Session id for read and write commands
            public uint8_t session;

            /// Command opcode
            public FTPOpcode opcode;

            /// Size of data
            public uint8_t size;

            /// Request opcode returned in kRspAck, kRspNak message
            public FTPOpcode req_opcode;

            /// Only used if req_opcode=kCmdBurstReadFile - 1: set of burst packets complete, 0: More burst packets coming.
            public uint8_t burst_complete;

            /// 32 bit aligment padding
            public uint8_t padding;

            /// Offsets for List and Read commands
            public uint32_t offset;

            /// command data, varies by Opcode
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 251 - 12)]
            public uint8_t[] data;

            static public implicit operator byte[](FTPPayloadHeader value)
            {
                if (value.data == null)
                    value.data = new byte[251 - 12];
                value.size = (byte) (value.data.Length);
                value.data = value.data.MakeSize(251 - 12);
                return MavlinkUtil.StructureToByteArray(value);
            }

            static public implicit operator FTPPayloadHeader(byte[] value)
            {
                return MavlinkUtil.ByteArrayToStructure<FTPPayloadHeader>(value, 0);
            }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class FtpFileInfo : System.IO.FileSystemInfo
        {
            public FtpFileInfo(string name, string parent, bool isdirectory = false, int size = -1)
            {
                Name = name;
                isDirectory = isdirectory;
                Size = size;
                Parent = parent;
                this.FullPath = (Parent.EndsWith("/") ? Parent : Parent + '/') + Name;
            }

            public override bool Exists => true;
            public bool isDirectory { get; set; }
            public override string Name { get; }
            public string Parent { get; }
            public int Size { get; set; }

            public override void Delete()
            {
            }

            public override string ToString()
            {
                if (isDirectory)
                    return "Directory: " + Name;
                return "File: " + Name + " " + Size;
            }
        }
    }

    public class RetryTimeout
    {
        public bool Complete = false;
        public int Retries = 3;
        public int TimeoutMS = 1000;
        public Action WorkToDo;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private DateTime _timeOutDateTime = DateTime.MinValue;

        public RetryTimeout(int Retrys = 3, int TimeoutMS = 1000)
        {
            this.Retries = Retrys;
            this.TimeoutMS = TimeoutMS;
        }

        public DateTime TimeOutDateTime
        {
            get
            {
                lock (this) return _timeOutDateTime;
            }
            set
            {
                lock (this) _timeOutDateTime = value;
            }
        }

        public bool DoWork()
        {
            if (WorkToDo == null)
                throw new ArgumentNullException("WorkToDo");
            Complete = false;
            for (int a = 0; a < Retries; a++)
            {
                log.InfoFormat("Retry {0} - {1}", a, TimeOutDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                WorkToDo();
                TimeOutDateTime = DateTime.Now.AddMilliseconds(TimeoutMS);
                while (DateTime.Now < TimeOutDateTime)
                {
                    if (Complete)
                        return true;
                    Thread.Sleep(100);
                    log.Debug("TimeOutDateTime " + TimeOutDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                }
            }

            return false;
        }

        public void ResetTimeout()
        {
            TimeOutDateTime = DateTime.Now.AddMilliseconds(TimeoutMS);
        }
    }
}