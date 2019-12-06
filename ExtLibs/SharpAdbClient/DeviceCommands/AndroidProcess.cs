//-----------------------------------------------------------------------
// <copyright file="AndroidProcess.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SharpAdbClient.DeviceCommands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents a process running on an Android device.
    /// </summary>
    public class AndroidProcess
    {
        /// <summary>
        /// Gets or sets the Process ID number.
        /// </summary>
        public int ProcessId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent Process ID number.
        /// </summary>
        public int ParentProcessId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total VM size in bytes.
        /// </summary>
        public ulong VirtualSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the process resident set size.
        /// </summary>
        public int ResidentSetSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the memory address of the event the process is waiting for
        /// </summary>
        public ulong WChan
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the process, including arguments, if any.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state of the process.
        /// </summary>
        public AndroidProcessState State
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a <see cref="AndroidProcess"/> from it <see cref="string"/> representation.
        /// </summary>
        /// <param name="line">
        /// A <see cref="string"/> which represents a <see cref="AndroidProcess"/>.
        /// </param>
        /// <returns>
        /// The equivalent <see cref="AndroidProcess"/>.
        /// </returns>
        public static AndroidProcess Parse(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            // See http://man7.org/linux/man-pages/man5/proc.5.html,
            // section /proc/[pid]/stat, for more information about the file format

            // Space delimited, so normally we would just do a string.split
            // The process name may contain spaces but is wrapped within parenteses, all other values (we know of) are
            // numeric.
            // So we parse the pid & process name manually, to account for this, and do the string.split afterwards :-)
            var processNameStart = line.IndexOf('(');
            var processNameEnd = line.IndexOf(')');

            var pid = int.Parse(line.Substring(0, processNameStart));
            var comm = line.Substring(processNameStart + 1, processNameEnd - processNameStart - 1);

            var parts = line.Substring(processNameEnd + 1).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 35)
            {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            // Only fields in Linux 2.1.10 and earlier are listed here,
            // additional fields exist in newer versions of linux.
            var state = parts[0];
            var ppid = ParseInt(parts[1]);
            var pgrp = ParseInt(parts[2]);
            var session = ParseInt(parts[3]);
            var tty_nr = ParseInt(parts[4]);
            var tpgid = ParseInt(parts[5]);
            var flags = ParseUInt(parts[6]);
            var minflt = ParseULong(parts[7]);
            var cminflt = ParseULong(parts[8]);
            var majflt = ParseULong(parts[9]);
            var cmajflt = ParseULong(parts[10]);
            var utime = ParseULong(parts[11]);
            var stime = ParseULong(parts[12]);
            var cutime = ParseLong(parts[13]);
            var cstime = ParseLong(parts[14]);
            var priority = ParseLong(parts[15]);
            var nice = ParseLong(parts[16]);
            var num_threads = ParseLong(parts[17]);
            var itrealvalue = ParseLong(parts[18]);
            var starttime = ParseULong(parts[19]);
            var vsize = ParseULong(parts[20]);
            var rss = int.Parse(parts[21]);
            var rsslim = ParseULong(parts[22]);
            var startcode = ParseULong(parts[23]);
            var endcode = ParseULong(parts[24]);
            var startstack = ParseULong(parts[25]);
            var kstkesp = ParseULong(parts[26]);
            var kstkeip = ParseULong(parts[27]);
            var signal = ParseULong(parts[28]);
            var blocked = ParseULong(parts[29]);
            var sigignore = ParseULong(parts[30]);
            var sigcatch = ParseULong(parts[31]);
            var wchan = ParseULong(parts[32]);
            var nswap = ParseULong(parts[33]);
            var cnswap = ParseULong(parts[34]);

            return new AndroidProcess()
            {
                Name = comm,
                ParentProcessId = ppid,
                State = (AndroidProcessState)Enum.Parse(typeof(AndroidProcessState), state, true),
                ProcessId = pid,
                ResidentSetSize = rss,
                VirtualSize = vsize,
                WChan = wchan
            };
        }

        /// <summary>
        /// Gets a <see cref="string"/> that represents this <see cref="AndroidProcess"/>,
        /// in the format of "<see cref="Name"/> (<see cref="ProcessId"/>)".
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this <see cref="AndroidProcess"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.ProcessId})";
        }

        /// <summary>
        /// Gets the index of the first value of a set of values that is part of a list.
        /// </summary>
        /// <param name="list">
        /// The list in which to search for the value.
        /// </param>
        /// <param name="values">
        /// The values to search for.
        /// </param>
        /// <returns>
        /// The index of the first element in <paramref name="values"/> that is present in the list, or
        /// <c>-1</c>.
        /// </returns>
        private static int IndexOf(List<string> list, params string[] values)
        {
            foreach (var value in values)
            {
                int index = list.IndexOf(value);

                if (index != -1)
                {
                    return index;
                }
            }

            return -1;
        }

        private static int ParseInt(string value)
        {
            return value == "-" ? 0 : int.Parse(value);
        }

        private static uint ParseUInt(string value)
        {
            return value == "-" ? 0 : uint.Parse(value);
        }

        private static long ParseLong(string value)
        {
            return value == "-" ? 0 : long.Parse(value);
        }

        private static ulong ParseULong(string value)
        {
            return value == "-" ? 0 : ulong.Parse(value);
        }
    }
}
