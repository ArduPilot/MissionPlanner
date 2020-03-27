using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using log4net;
using Newtonsoft.Json;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using MissionPlanner.Utilities;

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
            kErrFailFileProtected,

            /// File is write protected
            kErrBusy
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

        public MemoryStream GetFile(string file, CancellationTokenSource cancel, bool burst = true)
        {
            kCmdOpenFileRO(file, out var size, cancel);
            if (size == -1)
                return null;
            MemoryStream answer;
            if (!burst)
                answer = kCmdReadFile(file, size, cancel);
            else
                answer = kCmdBurstReadFile(file, size, cancel);
            kCmdResetSessions();
            return answer;
        }

        public void UploadFile(string file, string srcfile, CancellationTokenSource cancel)
        {
            var size = 0;
            kCmdCreateFile(file, ref size, cancel);
            kCmdWriteFile(srcfile, cancel);
            kCmdResetSessions();
        }

        public bool kCmdOpenFileRO(string file, out int size, CancellationTokenSource cancel)
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
            log.Info("get " + payload.opcode + " " + file);
            var timeout = new RetryTimeout();
            size = -1;
            var localsize = size;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrFail)
                    {
                        //stop trying
                        timeout.Retries = 0;
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                    {
                        kCmdResetSessions();
                        timeout.RetriesCurrent = 0;
                    }

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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
            return ans;
        }

        public MemoryStream kCmdBurstReadFile(string file, int size, CancellationTokenSource cancel)
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
            SortedList<uint, uint> chunkSortedList = new SortedList<uint, uint>();
            MemoryStream answer = new MemoryStream();
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
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
                // we have lost data - use retry after timeout
                if (answer.Position != ftphead.offset)
                {
                    seq_no = (ushort)(ftphead.seq_number + 1);
                    payload.seq_number = seq_no;
                    fileTransferProtocol.payload = payload;
                    timeout.RetriesCurrent = 0;
                    return true;
                }

                // got a valid segment, so reset retrys
                timeout.RetriesCurrent = 0;
                timeout.ResetTimeout();

                chunkSortedList[ftphead.offset] = ftphead.offset + ftphead.size;

                answer.Seek(ftphead.offset, SeekOrigin.Begin);
                answer.Write(ftphead.data, 0, ftphead.size);
                timeout.ResetTimeout();
                //log.Debug(ftphead);
                seq_no = (ushort) (ftphead.seq_number + 1);
                // if rerequest needed
                payload.offset = ftphead.offset + ftphead.size;
                payload.seq_number = seq_no;
                fileTransferProtocol.payload = payload;
                // ignore the burst read first response
                if(ftphead.size > 0)
                    Progress?.Invoke(file, (int)((float)payload.offset / size * 100.0));
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            timeout.DoWork();
            Progress?.Invoke(file, 100);
            _mavint.UnSubscribeToPacketType(sub);
            answer.Position = 0;
            return answer;
        }

        public bool kCmdCalcFileCRC32(string file, ref uint crc32, CancellationTokenSource cancel)
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
            log.Info("get " + payload.opcode + " " + file);
            var timeout = new RetryTimeout(3, 30000);
            crc32 = UInt32.MaxValue;
            var localcrc32 = crc32;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrBusy)
                        timeout.RetriesCurrent = 0;

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
                localcrc32 = BitConverter.ToUInt32(ftphead.data, 0);
                log.Info(ftphead.req_opcode + " " + file + " " + localcrc32);
                timeout.Complete = true;
                return true;
            });
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            crc32 = localcrc32;
            return ans;
        }

        private static readonly uint[] crc32_table = new uint[]
        {
            0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832,
            0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
            0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856, 0x646ba8c0, 0xfd62f97a,
            0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
            0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3,
            0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
            0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab,
            0xb6662d3d, 0x76dc4190, 0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
            0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01, 0x6b6b51f4,
            0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
            0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65, 0x4db26158, 0x3ab551ce, 0xa3bc0074,
            0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
            0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525,
            0x206f85b3, 0xb966d409, 0xce61e49f, 0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81,
            0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615,
            0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
            0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7, 0xfed41b76,
            0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
            0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c, 0x36034af6,
            0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
            0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7,
            0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
            0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7,
            0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
            0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 0xa00ae278,
            0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc,
            0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9, 0xbdbdf21c, 0xcabac28a, 0x53b39330,
            0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
            0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
        };

        public static uint crc_crc32(uint crc, byte[] buf)
        {
            uint size = (uint)buf.Length;
            for (uint i = 0; i < size; i++)
            {
                crc = crc32_table[(crc ^ buf[i]) & 0xff] ^ (crc >> 8);
            }

            return crc;
        }

        public bool kCmdCreateDirectory(string file, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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

                    if (errorcode == FTPErrorCode.kErrFail)
                    {
                        //stop trying
                        timeout.Retries = 0;
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdCreateFile(string file, ref int size, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrFail)
                    {
                        //stop trying
                        timeout.Retries = 0;
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                    {
                        kCmdResetSessions();
                        timeout.RetriesCurrent = 0;
                    }

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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
            return ans;
        }

        public List<FtpFileInfo> kCmdListDirectory(string dir, CancellationTokenSource cancel)
        {
            log.Info("kCmdListDirectory: " + dir);
            List <FtpFileInfo> answer = new List<FtpFileInfo>();
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
            Progress?.Invoke(dir + " Listing", 0);
            var timeout = new RetryTimeout(5);
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL,
                message =>
                {
                    if (cancel != null && cancel.IsCancellationRequested)
                    {
                        timeout.RetriesCurrent = 999;
                        return true;
                    }
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
                            timeout.Retries = 0;
                        }
                        else
                        {
                            log.Error(ftphead.req_opcode + " " + errorcode);
                        }

                        if (errorcode == FTPErrorCode.kErrEOF)
                            timeout.Complete = true;

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
                                var size = ulong.Parse(items[1]);
                                answer.Add(new FtpFileInfo(items[0], dir, false, size));
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

                    // 0 records
                    if(answer.Count == 0)
                        timeout.Complete = true;

                    Progress?.Invoke(dir + " " + answer.Count, answer.Count % 100);

                    payload.offset = (uint) answer.Count;
                    payload.seq_number = seq_no++;
                    fileTransferProtocol.payload = payload;
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                    timeout.ResetTimeout();
                    timeout.RetriesCurrent = 0;
                    return true;
                });
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            Progress?.Invoke(dir + " Ready", 100);

            return answer;
        }

        public bool kCmdOpenFileWO(string file, ref int size, CancellationTokenSource cancel)
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
            log.Info("get " + payload.opcode + " " + file);
            var timeout = new RetryTimeout();
            size = -1;
            var localsize = size;
            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrFail)
                    {
                        //stop trying
                        timeout.Retries = 0;
                    }

                    if (errorcode == FTPErrorCode.kErrNoSessionsAvailable)
                    {
                        kCmdResetSessions();
                        timeout.RetriesCurrent = 0;
                    }

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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            size = localsize;
            return ans;
        }

        public MemoryStream kCmdReadFile(string file, int size, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if (errorcode == FTPErrorCode.kErrFail)
                    {
                        //stop trying
                        timeout.Retries = 0;
                    }

                    if (errorcode == FTPErrorCode.kErrEOF)
                    {
                        if (ftphead.offset + ftphead.size >= size)
                        {
                            timeout.Complete = true;
                            return true;
                        }
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
                // we have lost data - use retry after timeout
                if (answer.Position != ftphead.offset)
                {
                    timeout.RetriesCurrent = 0;
                    return true;
                }
                // got a valid segment, so reset retrys
                timeout.RetriesCurrent = 0;
                timeout.ResetTimeout();

                answer.Seek(ftphead.offset, SeekOrigin.Begin);
                answer.Write(ftphead.data, 0, ftphead.size);
                Progress?.Invoke(file, (int)((float)payload.offset / size * 100.0));
                if (ftphead.offset + ftphead.size >= size)
                {
                    timeout.Complete = true;
                    return true;
                }

                payload.offset = ftphead.offset + ftphead.size;
                payload.seq_number = seq_no++;
                fileTransferProtocol.payload = payload;
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                return true;
            });
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            timeout.DoWork();
            Progress?.Invoke(file, 100);
            _mavint.UnSubscribeToPacketType(sub);
            answer.Position = 0;
            return answer;
        }

        public bool kCmdRemoveDirectory(string file, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdRemoveFile(string file, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdRename(string src, string dest, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdResetSessions()
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
                        timeout.Retries = 0;
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
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdTerminateSession()
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
                        timeout.Retries = 0;
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
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdTruncateFile(string file, CancellationTokenSource cancel)
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
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return true;
                }
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
                        timeout.Retries = 0;
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
            timeout.WorkToDo = () =>
            {
                if (cancel != null && cancel.IsCancellationRequested)
                {
                    timeout.RetriesCurrent = 999;
                    return;
                }
                _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
            };
            var ans = timeout.DoWork();
            _mavint.UnSubscribeToPacketType(sub);
            return ans;
        }

        public bool kCmdWriteFile(byte[] data, uint destoffset, string friendlyname, CancellationTokenSource cancel)
        {
            RetryTimeout timeout = new RetryTimeout();
            KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdWriteFile,
                seq_number = seq_no++,
                offset = destoffset,
                session = 0
            };
            fileTransferProtocol.payload = payload;
            log.Info("get " + payload.opcode + " " + friendlyname + " ");

            {
                var size = data.Length;
                var bytes_read = 0;
                if (size == 0)
                    return false;
                sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
                {
                    Console.WriteLine("G " + DateTime.Now.ToString("O"));
                    if (cancel != null && cancel.IsCancellationRequested)
                    {
                        timeout.RetriesCurrent = 999;
                        return true;
                    }
                    var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                    FTPPayloadHeader ftphead = msg.payload;
                    // error at far end
                    if (ftphead.opcode == FTPOpcode.kRspNak)
                    {
                        var errorcode = (FTPErrorCode)ftphead.data[0];
                        if (errorcode == FTPErrorCode.kErrFailErrno)
                        {
                            var _ftp_errno = (errno)ftphead.data[1];
                            log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                            timeout.Retries = 0;
                        }
                        else
                        {
                            log.Error(ftphead.req_opcode + " " + errorcode);
                        }

                        if (errorcode == FTPErrorCode.kErrFail)
                        {
                            //stop trying
                            timeout.Retries = 0;
                        }

                        return true;
                    }

                    // not for us or bad seq no
                    if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                        return true;
                    // only ack's
                    if (ftphead.opcode != FTPOpcode.kRspAck)
                        return true;
                    if ((payload.offset - destoffset) >= size)
                    {
                        timeout.Complete = true;
                        return true;
                    }

                    // send next
                    payload.offset += (uint)payload.data.Length;
                    payload.data = data.Skip((int)payload.offset - (int)destoffset).Take(239).ToArray();
                    bytes_read = payload.data.Length;
                    payload.size = (uint8_t)bytes_read;
                    payload.seq_number = seq_no++;
                    fileTransferProtocol.payload = payload;
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                    Progress?.Invoke(friendlyname, (int)((float)payload.offset / size * 100.0));
                    timeout.ResetTimeout();
                    Console.WriteLine("S " + DateTime.Now.ToString("O"));
                    return true;
                });
                // fill buffer
                payload.offset = destoffset;
                payload.data = data.Skip((int)payload.offset - (int)destoffset).Take(239).ToArray();
                bytes_read = payload.data.Length;
                payload.size = (uint8_t)bytes_read;
                //  package it
                fileTransferProtocol.payload = payload;
                // send it
                timeout.WorkToDo = () =>
                {
                    if (cancel != null && cancel.IsCancellationRequested)
                    {
                        timeout.RetriesCurrent = 999;
                        return;
                    }
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                };
                var ans = timeout.DoWork();
                Progress?.Invoke(friendlyname, 100);
                _mavint.UnSubscribeToPacketType(sub);
                return ans;
            }
        }


        public bool kCmdWriteFile(string srcfile, CancellationTokenSource cancel)
        {
            return kCmdWriteFile(new BufferedStream(File.OpenRead(srcfile), 1024 * 1024), srcfile, cancel);
        }

        public bool kCmdWriteFile(Stream srcfile, string friendlyname, CancellationTokenSource cancel)
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
            log.Info("get " + payload.opcode + " " + friendlyname + " ");
            using (var stream = srcfile)
            {
                var size = stream.Length;
                var bytes_read = 0;
                if (size == 0)
                    return false;
                sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
                {
                    Console.WriteLine("G " + DateTime.Now.ToString("O"));
                    if (cancel != null && cancel.IsCancellationRequested)
                    {
                        timeout.RetriesCurrent = 999;
                        return true;
                    }
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
                            timeout.Retries = 0;
                        }
                        else
                        {
                            log.Error(ftphead.req_opcode + " " + errorcode);
                        }

                        if (errorcode == FTPErrorCode.kErrFail)
                        {
                            //stop trying
                            timeout.Retries = 0;
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
                    Array.Resize(ref payload.data, bytes_read);
                    payload.size = (uint8_t) bytes_read;
                    payload.seq_number = seq_no++;
                    fileTransferProtocol.payload = payload;
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                    Progress?.Invoke(friendlyname, (int)((float)payload.offset / size * 100.0));
                    timeout.ResetTimeout();
                    Console.WriteLine("S " + DateTime.Now.ToString("O"));
                    return true;
                });
                // fill buffer
                payload.offset = (uint32_t) stream.Position;
                payload.data = new byte[239];
                bytes_read = stream.Read(payload.data, 0, payload.data.Length);
                Array.Resize(ref payload.data, bytes_read);
                payload.size = (uint8_t) bytes_read;
                //  package it
                fileTransferProtocol.payload = payload;
                // send it
                timeout.WorkToDo = () =>
                {
                    if (cancel != null && cancel.IsCancellationRequested)
                    {
                        timeout.RetriesCurrent = 999;
                        return;
                    }
                    _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
                };
                var ans = timeout.DoWork();
                Progress?.Invoke(friendlyname, 100);
                _mavint.UnSubscribeToPacketType(sub);
                return ans;
            }
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
            public FtpFileInfo(string name, string parent, bool isdirectory = false, ulong size = 0)
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
            public ulong Size { get; set; }

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

        public event Action<string, int> Progress;
    }
}