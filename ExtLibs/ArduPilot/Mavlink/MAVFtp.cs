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
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class MAVFtp
    {
        /// Identifies Directory returned from List command
        const byte kDirentDir = (byte)'D';

        /// Identifies File returned from List command
        const byte kDirentFile = (byte)'F';

        /// Identifies Skipped entry from List command
        const byte kDirentSkip = (byte)'S';

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

        /// @brief Error codes returned in Nak response FTPPayloadHeader.data[0].
        public enum FTPErrorCode : uint8_t
        {
            kErrNone,
            kErrFail,

            ///< Unknown failure
            kErrFailErrno,

            ///< Command failed, errno sent back in FTPPayloadHeader.data[1]
            kErrInvalidDataSize,

            ///< FTPPayloadHeader.size is invalid
            kErrInvalidSession,

            ///< Session is not currently open
            kErrNoSessionsAvailable,

            ///< All available Sessions in use
            kErrEOF,

            ///< Offset past end of file for List and Read commands
            kErrUnknownCommand,

            ///< Unknown command opcode
            kErrFailFileExists,

            ///< File exists already
            kErrFailFileProtected ///< File is write protected
        };

        /// Command opcodes
        public enum FTPOpcode : uint8_t
        {
            kCmdNone,

            ///< ignored, always acked
            kCmdTerminateSession,

            ///< Terminates open Read session
            kCmdResetSessions,

            ///< Terminates all open Read sessions
            kCmdListDirectory,

            ///< List files in <path> from <offset>
            kCmdOpenFileRO,

            ///< Opens file at <path> for reading, returns <session>
            kCmdReadFile,

            ///< Reads <size> bytes from <offset> in <session>
            kCmdCreateFile,

            ///< Creates file at <path> for writing, returns <session>
            kCmdWriteFile,

            ///< Writes <size> bytes to <offset> in <session>
            kCmdRemoveFile,

            ///< Remove file at <path>
            kCmdCreateDirectory,

            ///< Creates directory at <path>
            kCmdRemoveDirectory,

            ///< Removes Directory at <path>, must be empty
            kCmdOpenFileWO,

            ///< Opens file at <path> for writing, returns <session>
            kCmdTruncateFile,

            ///< Truncate file at <path> to <offset> length
            kCmdRename,

            ///< Rename <path1> to <path2>
            kCmdCalcFileCRC32,

            ///< Calculate CRC32 for file at <path>
            kCmdBurstReadFile,

            ///< Burst download session file

            kRspAck = 128,

            ///< Ack response
            kRspNak ///< Nak response
        };

        public List<FtpFileInfo> GetDirectory(string dir = "/")
        {
            var answer = kCmdListDirectory(dir);

            return answer;
        }

        public MemoryStream GetFile(string file)
        {
            kCmdOpenFileRO(file, out var size);

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

            size = 0;
            var localsize = size;

            var sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode)ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = ftphead.data[1];
                        log.Error(ftphead.req_opcode + " " + errorcode + " " + _ftp_errno);
                    }
                    else
                    {
                        log.Error(ftphead.req_opcode + " " + errorcode);
                    }

                    if(errorcode == FTPErrorCode.kErrNoSessionsAvailable)
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
        private MemoryStream kCmdBurstReadFile(string file, int size)
        {
            RetryTimeout timeout = new RetryTimeout();
            KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdBurstReadFile,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                session = 0
            };

            fileTransferProtocol.payload = payload;

            MemoryStream answer = new MemoryStream();

            sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode)ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = ftphead.data[1];
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

                // not for us or bad seq no
                if (payload.opcode != ftphead.req_opcode || payload.seq_number + 1 != ftphead.seq_number)
                    return true;

                // only ack's
                if (ftphead.opcode != FTPOpcode.kRspAck)
                    return true;

                answer.Seek(ftphead.offset, SeekOrigin.Begin);
                answer.Write(ftphead.data, 0, ftphead.size);
                timeout.ResetTimeout();

                if (ftphead.offset + ftphead.size >= size || ftphead.burst_complete == 1)
                {
                    timeout.Complete = true;
                    return true;
                }

                return true;
            });

            timeout.WorkToDo = () => _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);

            timeout.DoWork();

            _mavint.UnSubscribeToPacketType(sub);
            return answer;
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
                    var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                    FTPPayloadHeader ftphead = msg.payload;
                    // error at far end
                    if (ftphead.opcode == FTPOpcode.kRspNak)
                    {
                        var errorcode = (FTPErrorCode)ftphead.data[0];
                        if (errorcode == FTPErrorCode.kErrFailErrno)
                        {
                            var _ftp_errno = (FTPErrorCode)ftphead.data[1];
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
                                        filename.Append((char)b);
                                }

                                var items = filename.ToString().Split('\t');

                                var size = int.Parse(items[1]);

                                answer.Add(new FtpFileInfo(items[0],dir, false, (int)size));
                                break;
                            case kDirentDir:
                                var name = new StringBuilder();
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                    if (b != 0x0)
                                        name.Append((char)b);
                                }

                                answer.Add(new FtpFileInfo(name.ToString(),dir, true));
                                break;
                            case kDirentSkip:
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                }

                                answer.Add(new FtpFileInfo("",dir, true));
                                break;
                            default:
                                var nameextra = new StringBuilder();
                                while (b != 0x0)
                                {
                                    b = ftphead.data[offset++];
                                    if (b != 0x0)
                                        nameextra.Append((char)b);
                                }

                                if (nameextra.ToString() != "")
                                    answer.Add(new FtpFileInfo(nameextra.ToString(),dir, false));
                                break;
                        }
                    }

                    payload.offset = (uint)answer.Count;
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

        private MemoryStream kCmdReadFile(string file, int size)
        {
            RetryTimeout timeout = new RetryTimeout();
            KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> sub;
            var payload = new FTPPayloadHeader()
            {
                opcode = FTPOpcode.kCmdReadFile,
                data = ASCIIEncoding.ASCII.GetBytes(file),
                seq_number = seq_no++,
                offset = 0
            };

            fileTransferProtocol.payload = payload;

            log.Info("get " + payload.opcode + " " + file + " " + size);

            MemoryStream answer = new MemoryStream();

            sub = _mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL, message =>
            {
                var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                FTPPayloadHeader ftphead = msg.payload;
                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode)ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (FTPErrorCode)ftphead.data[1];
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
            return answer;
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
                var msg = (MAVLink.mavlink_file_transfer_protocol_t)message.data;
                FTPPayloadHeader ftphead = msg.payload;

                log.Info(ftphead.opcode + " " + ftphead.req_opcode + " " + ftphead.size);

                // error at far end
                if (ftphead.opcode == FTPOpcode.kRspNak)
                {
                    var errorcode = (FTPErrorCode)ftphead.data[0];
                    if (errorcode == FTPErrorCode.kErrFailErrno)
                    {
                        var _ftp_errno = (FTPErrorCode)ftphead.data[1];
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

            timeout.DoWork();

            //_mavint.sendPacket(fileTransferProtocol, _sysid, _compid);

            log.Info(payload.opcode);

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

            _mavint.sendPacket(fileTransferProtocol, _sysid, _compid);
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
                if(value.data == null)
                    value.data = new byte[251-12];
                value.size = (byte)(value.data.Length);
                value.data = value.data.MakeSize(251 - 12);
                return MavlinkUtil.StructureToByteArray(value);
            }

            static public implicit operator FTPPayloadHeader(byte[] value)
            {
                return MavlinkUtil.ByteArrayToStructure<FTPPayloadHeader>(value, 0);
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

            public string Parent { get; }

            public override string Name { get; }

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
        public DateTime TimeOutDateTime = DateTime.MinValue;
        public int timeoutMS = 1000;
        public Action WorkToDo;
        public RetryTimeout()
        {

        }

        public bool DoWork()
        {
            Complete = false;

            for (int a = 0; a < Retries; a++)
            {
                WorkToDo();

                TimeOutDateTime = DateTime.Now.AddMilliseconds(timeoutMS);
                while (DateTime.Now < TimeOutDateTime)
                {
                    if (Complete)
                        return true;
                    Thread.Sleep(100);
                }
            }

            return false;
        }

        public void ResetTimeout()
        {
            TimeOutDateTime = DateTime.Now.AddMilliseconds(timeoutMS);
        }
    }
}
