using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using uint16_t = System.UInt16;
using uint64_t = System.UInt64;
using uint8_t = System.Byte;

namespace MissionPlanner.Utilities
{
    public class ULog
    {
        private List<message_header_s> messages = new List<message_header_s>();
        private static List<message_format_s> format = new List<message_format_s>();
        private List<message_add_logged_s> id_to_format = new List<message_add_logged_s>();
        private List<message_data_s> data = new List<message_data_s>();

        public void read(Stream stream)
        {
            var header = new byte[] { 0x55, 0x4c, 0x6f, 0x67, 0x01, 0x12, 0x35 };

            var buffer = new byte[2048];
            stream.Read(buffer, 0, 16);

            if (buffer.Search(header, 0) == 0)
            {
                var version = buffer[7];
                var timestamp = BitConverter.ToUInt64(buffer, 8);

                var appendidx = 0ul;

                int len = 0;

                while ((len = stream.Read(buffer, 0, 3)) > 0)
                {
                    if ((ulong)stream.Position >= appendidx && appendidx > 0)
                        break;

                    var msg_header = new message_header_s(buffer);

                    messages.Add(msg_header);

                    var msg_size = stream.Read(buffer, 0, msg_header.msg_size);

                    var message = new Span<byte>(buffer, 0, msg_size);

                    switch ((char)msg_header.msg_type)
                    {
                        case 'B':
                            {
                                var msg = new ulog_message_flag_bits_s(message);

                                if (msg.incompat_flags[0] == 1)
                                {
                                    appendidx = msg.appended_offsets[0] - 16;
                                }
                                Console.WriteLine(msg.ToJSONWithType());
                                break;
                            }
                        case 'F':
                            {
                                var msg = new message_format_s(message);
                                Console.WriteLine(msg.ToJSONWithType());
                                format.Add(msg);
                                break;
                            }
                        case 'I':
                            {
                                var msg = new message_info_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'M':
                            {
                                var msg = new ulog_message_info_multiple_header_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'P': // param
                            {
                                var msg = new message_info_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'Q':
                            {
                                var msg = new ulog_message_parameter_default_header_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'A':
                            {
                                var msg = new message_add_logged_s(message); id_to_format.Add(msg); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'R':
                            {
                                var msg = new message_remove_logged_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'D':
                            {
                                var msg = new message_data_s(message, id_to_format, format); Console.WriteLine(msg.ToJSONWithType());
                                
                                data.Add(msg);

                                break;
                            }
                        case 'L':
                            {
                                var msg = new message_logging_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'C':
                            {
                                var msg = new message_logging_tagged_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'S':
                            {
                                var msg = new message_sync_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        case 'O':
                            {
                                var msg = new message_dropout_s(message); Console.WriteLine(msg.ToJSONWithType()); break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                var groups = messages.GroupBy(a => a.msg_type);

                groups.ForEach(a => Console.WriteLine((char)a.Key + " " + a.Count()));


                var groups2 = data.GroupBy(a => a.Type+"["+a.MultiID+"]");

                groups2.ForEach(a => Console.WriteLine(a.Key + " " + a.Count()));
            }
        }

        public static readonly string[] LOG_LEVELS = new string[] { "EMERG", "ALERT", "CRIT", "ERR", "WARNING", "NOTICE", "INFO", "DEBUG" };

        public static readonly Dictionary<string, Type> TYPES = new Dictionary<string, Type> {
            { "uint8_t", typeof(Byte) },
    {"uint16_t",typeof(UInt16) },
    {"uint32_t",typeof(UInt32) },
    {"uint64_t", typeof(UInt64) },

    {"int8_t", typeof(SByte) },
    {"int16_t",typeof(Int16) },
    {"int32_t", typeof(Int32) },
    {"int64_t", typeof(Int64) },

    {"float", typeof(float) },
    {"double", typeof(double) },
    {"bool", typeof(bool) },
    {"char",typeof(char) },
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_header_s
        {
            public ushort msg_size;

            public byte msg_type;

            public message_header_s(Span<byte> buffer) : this()
            {
                msg_size = BitConverter.ToUInt16(buffer.ToArray(), 0);
                msg_type = buffer[2];
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ulog_message_flag_bits_s
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint8_t[] compat_flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint8_t[] incompat_flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            ///< file offset(s) for appended data if appending bit is set
            public uint64_t[] appended_offsets;

            public ulog_message_flag_bits_s(Span<byte> message) : this()
            {
                compat_flags = message.Slice(0, 8).ToArray();
                incompat_flags = message.Slice(8, 8).ToArray();
                appended_offsets = new[] { BitConverter.ToUInt64(message.Slice(16).ToArray(), 0), BitConverter.ToUInt64(message.Slice(24).ToArray(), 0), BitConverter.ToUInt64(message.Slice(32).ToArray(), 0) };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_format_s
        {
            private byte[] format;

            public message_format_s(Span<byte> enumerable) : this()
            {
                format = enumerable.ToArray();
            }

            public string Format { get => ASCIIEncoding.ASCII.GetString(format); }

            public string Type { get => ASCIIEncoding.ASCII.GetString(format).Split(':')[0]; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_info_s
        {
            public uint8_t key_len;

            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 0)]
            private byte[] key;

            private byte[] value;

            public string Key { get => ASCIIEncoding.ASCII.GetString(key); }

            public string Value
            {
                get
                {
                    var items = Key.Split(' ');
                    var idx = 0;
                    return ConvertType(items[0], value, ref idx).ToString();
                }
            }

            public message_info_s(Span<byte> buffer) : this()
            {
                key_len = buffer[0];
                key = buffer.Slice(1, key_len).ToArray();
                value = buffer.Slice(1 + key_len).ToArray();
            }
        }

        public static object ConvertType(string type, byte[] value, ref int width)
        {
            if (value.Length == 0)
                return "";

            if (type.Contains("["))
            {
                var typestring = type.Split('[')[0];

                if (format.Any(a => a.Type == typestring))
                {
                    var formatdef = format.First(a => a.Format.StartsWith(typestring + ":"));

                    var dataindex = width;

                    var ans = formatdef.Format.Split(':', ';', ' ').Skip(1).NowNextBy2().Select(a =>
                        {
                            var idx = 0;
                            var valueans = ConvertType(a.Item1, new Span<byte>(value, dataindex, value.Length - dataindex).ToArray(), ref idx);
                            dataindex += idx;
                            return valueans;
                        }).ToArray();//.Aggregate((a, b) => a + "," + b);

                    width = dataindex;
                    return ans;
                }
                else if (type.StartsWith("char"))
                {
                    var count = int.Parse(type.Split('[', ']')[1]);

                    width = count;
                    return ASCIIEncoding.ASCII.GetString(value.Take(count).ToArray());
                }
                else
                {
                    var count = int.Parse(type.Split('[', ']')[1]);

                    var dataindex = width;

                    var ans = Enumerable.Range(0, count * 2).NowNextBy2().Select(a =>
                           {
                               var idx = 0;
                               var valueans = ConvertType(typestring, new Span<byte>(value, dataindex, value.Length - dataindex).ToArray(), ref idx);
                               dataindex += idx;
                               return valueans;
                           }).ToArray();//.Aggregate((a, b) => a + "," + b);

                    width = dataindex;
                    return ans.ToArray();
                }
            }

            if (type.StartsWith("uint8_t"))
            {
                width = 1;
                return value[0];
            }

            if (type.StartsWith("uint16_t"))
            {
                width = 2;
                return BitConverter.ToUInt16(value, 0);
            }
            if (type.StartsWith("int16_t"))
            {
                width = 2;
                return BitConverter.ToInt16(value, 0);
            }

            if (type.StartsWith("uint32_t"))
            {
                width = 4;
                return BitConverter.ToUInt32(value, 0);
            }
            if (type.StartsWith("int32_t"))
            {
                width = 4;
                return BitConverter.ToInt32(value, 0);
            }
            if (type.StartsWith("int64_t"))
            {
                width = 8;
                return BitConverter.ToInt64(value, 0);
            }
            if (type.StartsWith("uint64_t"))
            {
                width = 8;
                return BitConverter.ToUInt64(value, 0);
            }
            if (type.StartsWith("float"))
            {
                width = 4;
                return BitConverter.ToSingle(value, 0);
            }
            if (type.StartsWith("double"))
            {
                width = 8;
                return BitConverter.ToDouble(value, 0);
            }

            if (type.StartsWith("bool"))
            {
                width = 1;
                return (value[0] != 0);
            }

            throw new Exception("unhandled type");
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ulog_message_info_multiple_header_s
        {
            public uint8_t is_continued; ///< can be used for arrays
            public uint8_t key_len;

            private byte[] key;

            private byte[] value;

            public string Key { get => ASCIIEncoding.ASCII.GetString(key); }

            public string Value
            {
                get
                {
                    var items = Key.Split(' ');
                    var idx = 0;
                    return ConvertType(items[0], value, ref idx).ToString();
                }
            }

            public ulog_message_info_multiple_header_s(Span<byte> buffer) : this()
            {
                is_continued = buffer[0];
                key_len = buffer[1];
                key = buffer.Slice(2, key_len).ToArray();
                value = buffer.Slice(2 + key_len).ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ulog_message_parameter_default_header_s
        {
            public uint8_t default_types;
            public uint8_t key_len;

            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)]
            private byte[] key;

            private byte[] value;

            public string Key { get => ASCIIEncoding.ASCII.GetString(key); }

            public string Value
            {
                get
                {
                    var items = Key.Split(' ');
                    var idx = 0;
                    return ConvertType(items[0], value, ref idx).ToString();
                }
            }

            public ulog_message_parameter_default_header_s(Span<byte> buffer) : this()
            {
                key_len = buffer[0];
                key = buffer.Slice(2, key_len).ToArray();
                value = buffer.Slice(2 + key_len).ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_add_logged_s
        {
            public uint8_t multi_id;
            public uint16_t msg_id;

            private byte[] message_name;

            public string Message_name { get => ASCIIEncoding.ASCII.GetString(message_name); }

            public message_add_logged_s(Span<byte> message) : this()
            {
                multi_id = message[0];
                msg_id = BitConverter.ToUInt16(message.Slice(1).ToArray(), 0);
                message_name = message.Slice(3).ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_remove_logged_s
        {
            public uint16_t msg_id;

            public message_remove_logged_s(Span<byte> message) : this()
            {
                msg_id = BitConverter.ToUInt16(message.ToArray(), 0);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_data_s
        {
            public uint16_t msg_id;

            public uint8_t[] data;

            public string Decoded { get; set; }

            public Dictionary<string, object> Raw { get; set; }

            public string Type {get;set;}

            public int MultiID{get;set;}

            public message_data_s(Span<byte> message, List<message_add_logged_s> id_to_format, List<message_format_s> format) : this()
            {
                var msg_id = BitConverter.ToUInt16(message.ToArray(), 0);
                this.msg_id = msg_id;
                var data = message.Slice(2).ToArray();
                this.data = data;

                var formatname = id_to_format.First(a => { return a.msg_id == msg_id; });

                Type = formatname.Message_name;
                MultiID = formatname.multi_id;

                var formatdef = format.First(a => a.Format.StartsWith(formatname.Message_name + ":"));

                var dataindex = 0;

                Raw = formatdef.Format.Split(':', ';', ' ').Skip(1).NowNextBy2().Select(a =>
                    {
                        var idx = 0;
                        var value = ConvertType(a.Item1, new Span<byte>(data, dataindex, data.Length - dataindex).ToArray(), ref idx);
                        dataindex += idx;
                        return (a.Item2, value);
                    }
                ).Where(a=>!a.Item1.StartsWith("_padding")).ToDictionary(a => a.Item1, b => b.value);

                Decoded = formatname.Message_name + "[" + formatname.multi_id + "]" + ": " + Raw.Select(a => a.Value).Flatten<object>().Aggregate((a, b) =>
                {
                    return a + "," + b;
                }
                );
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_logging_s
        {
            public uint8_t log_level;
            public uint64_t timestamp;

            private byte[] message;

            public string Message { get => ASCIIEncoding.ASCII.GetString(message); }

            public message_logging_s(Span<byte> message1) : this()
            {
                log_level = message1[0];
                timestamp = BitConverter.ToUInt64(message1.Slice(1).ToArray(), 0);
                message = message1.Slice(9).ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_logging_tagged_s
        {
            public uint8_t log_level;
            public uint16_t tag;
            public uint64_t timestamp;

            public byte[] message;

            public message_logging_tagged_s(Span<byte> message1) : this()
            {
                log_level = message1[0];
                tag = BitConverter.ToUInt16(message1.Slice(1).ToArray(), 0);
                timestamp = BitConverter.ToUInt64(message1.Slice(3).ToArray(), 0);
                message = message1.Slice(11).ToArray();
            }
        }

        public enum ulog_tag : ushort

        {
            unassigned,
            mavlink_handler,
            ppk_handler,
            camera_handler,
            ptp_handler,
            serial_handler,
            watchdog,
            io_service,
            cbuf,
            ulg
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_sync_s
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint8_t[] sync_magic;

            public message_sync_s(Span<byte> message) : this()
            {
                sync_magic = message.ToArray();
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct message_dropout_s
        {
            public uint16_t duration;

            public message_dropout_s(Span<byte> message) : this()
            {
                duration = BitConverter.ToUInt16(message.ToArray(), 0);
            }
        }
    }
}