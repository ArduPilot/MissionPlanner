// <copyright file="LogReader.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient.Logs
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Processes Android log files in binary format. You usually get the binary format by
    /// running <c>logcat -B</c>.
    /// </summary>
    public class LogReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogReader"/> class.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> that contains the logcat data.
        /// </param>
        public LogReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Reads the next <see cref="LogEntry"/> from the stream.
        /// </summary>
        /// <returns>
        /// A new <see cref="LogEntry"/> object.
        /// </returns>
        public LogEntry ReadEntry()
        {
            LogEntry value = new LogEntry();

            // Read the log data in binary format. This format is defined at
            // https://android.googlesource.com/platform/system/core/+/master/include/log/logger.h
            var payloadLength = this.ReadUInt16();
            var headerSize = this.ReadUInt16();
            var pid = this.ReadInt32();
            var tid = this.ReadInt32();
            var sec = this.ReadInt32();
            var nsec = this.ReadInt32();

            // If the headerSize is not 0, we have either a logger_entry_v3 or logger_entry_v2 object.
            // For both objects, the size should be 0x18
            uint id = 0;
            if (headerSize != 0)
            {
                if (headerSize == 0x18)
                {
                    id = this.ReadUInt32();
                }
                else
                {
                    throw new Exception();
                }
            }

            byte[] data = this.ReadBytes(payloadLength);

            DateTime timestamp = DateTimeHelper.Epoch.AddSeconds(sec);
            timestamp = timestamp.AddMilliseconds(nsec / 1000000d);

            switch ((LogId)id)
            {
                case LogId.Crash:
                case LogId.Kernel:
                case LogId.Main:
                case LogId.Radio:
                case LogId.System:
                    {
                        // format: <priority:1><tag:N>\0<message:N>\0
                        var priority = data[0];

                        // Find the first \0 byte in the array. This is the seperator
                        // between the tag and the actual message
                        int tagEnd = 1;

                        while (data[tagEnd] != '\0' && tagEnd < data.Length)
                        {
                            tagEnd++;
                        }

                        // Message should be null termintated, so remove the last entry, too (-2 instead of -1)
                        string tag = Encoding.ASCII.GetString(data, 1, tagEnd - 1);
                        string message = Encoding.ASCII.GetString(data, tagEnd + 1, data.Length - tagEnd - 2);

                        return new AndroidLogEntry()
                        {
                            Data = data,
                            ProcessId = pid,
                            ThreadId = tid,
                            TimeStamp = timestamp,
                            Id = id,
                            Priority = priority,
                            Message = message,
                            Tag = tag
                        };
                    }

                case LogId.Events:
                    {
                        // https://android.googlesource.com/platform/system/core.git/+/master/liblog/logprint.c#547
                        var entry = new EventLogEntry()
                        {
                            Data = data,
                            ProcessId = pid,
                            ThreadId = tid,
                            TimeStamp = timestamp,
                            Id = id
                        };

                        // Use a stream on the data buffer. This will make sure that,
                        // if anything goes wrong parsing the data, we never go past
                        // the message boundary itself.
                        using (MemoryStream stream = new MemoryStream(data))
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            var priority = reader.ReadInt32();

                            while (stream.Position < stream.Length)
                            {
                                this.ReadLogEntry(reader, entry.Values);
                            }
                        }

                        return entry;
                    }

                default:
                    return new LogEntry()
                    {
                        Data = data,
                        ProcessId = pid,
                        ThreadId = tid,
                        TimeStamp = timestamp,
                        Id = id
                    };
            }
        }

        private void ReadLogEntry(BinaryReader reader, Collection<object> parent)
        {
            var type = (EventLogType)reader.ReadByte();

            switch (type)
            {
                case EventLogType.Float:
                    parent.Add(reader.ReadSingle());

                    break;

                case EventLogType.Integer:
                    parent.Add(reader.ReadInt32());
                    break;

                case EventLogType.Long:
                    parent.Add(reader.ReadInt64());
                    break;

                case EventLogType.List:
                    var listLength = reader.ReadByte();

                    Collection<object> list = new Collection<object>();

                    for (int i = 0; i < listLength; i++)
                    {
                        this.ReadLogEntry(reader, list);
                    }

                    parent.Add(list);
                    break;

                case EventLogType.String:
                    int stringLength = reader.ReadInt32();
                    byte[] messageData = reader.ReadBytes(stringLength);
                    string message = Encoding.ASCII.GetString(messageData);
                    parent.Add(message);
                    break;
            }
        }
    }
}
