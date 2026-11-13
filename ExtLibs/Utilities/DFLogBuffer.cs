using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class DFLogBuffer : IEnumerable<String>, IDisposable
    {
        // used for binary log line conversion
        BinaryLog binlog = new BinaryLog();

        // used for fmt messages
        public DFLog dflog { get; }

        Stream basestream;
        // the length of basestream, asking a file for it is a system call
        long basestreamlength;
        private long _count;
        List<long> linestartoffset = new List<long>();

        /// <summary>
        /// Type and line numbers, the offset of a line is in linestartoffset
        /// </summary>
        List<int>[] messageindexline = new List<int>[256];

        bool binary = false;

        object locker = new object();

        long indexcachelineno = -1;
        String currentindexcache = null;

        // The part of a binary log last read for this[long]. The messages of one type are read in file
        // order with the other types in between, and a seek of the stream for each of them costs far
        // more than decoding it.
        private const int WindowSize = 256 * 1024;
        // messages no further apart than this are read ahead, further apart they are read one by one
        private const int WindowReadAheadDistance = 32 * 1024;
        private const int WindowSmallSize = 4096;
        byte[] window;
        long windowstart = -1;
        int windowlength;
        long windowlastoffset = -1;

        /// <summary>
        /// Have count bytes of the log at offset in the window. The caller holds locker
        /// </summary>
        /// <returns>false when the log ends before offset + count</returns>
        bool FillWindow(long offset, int count)
        {
            long lastoffset = windowlastoffset;
            windowlastoffset = offset;

            if (windowstart >= 0 && offset >= windowstart && offset + count <= windowstart + windowlength)
                return true;

            if (window == null)
                window = new byte[WindowSize];

            // a first read, or one near the last: more of them are likely to follow it
            bool readahead = lastoffset < 0 || Math.Abs(offset - lastoffset) <= WindowReadAheadDistance;
            long want = Math.Max(count, readahead ? WindowSize : WindowSmallSize);
            want = Math.Min(want, Math.Min(window.Length, basestreamlength - offset));

            windowstart = -1;
            windowlength = 0;

            if (want < count)
                return false;

            if (basestream.Position != offset)
                basestream.Seek(offset, SeekOrigin.Begin);

            int have = 0;
            while (have < want)
            {
                int read = basestream.Read(window, have, (int)want - have);
                if (read <= 0)
                    break;
                have += read;
            }

            windowstart = offset;
            windowlength = have;

            return have >= count;
        }

        public DFLogBuffer(string filename) : this(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            _filename = filename;
        }

        public DFLogBuffer(Stream instream)
        {
            if (instream is FileStream)
                _filename = ((FileStream)instream).Name;

            dflog = new DFLog(this);
            for (int a = 0; a < messageindexline.Length; a++)
            {
                messageindexline[a] = new List<int>(0);
            }

            if (instream.CanSeek)
            {
                basestream = instream;
            }
            else
            {
                Console.WriteLine("DFLogBuffer: not seekable - copying to memorystream");
                basestream = new MemoryStream((int)instream.Length);
                instream.CopyTo(basestream);
                basestream.Position = 0;
                instream.Close();
            }

            if (basestream.ReadByte() == BinaryLog.HEAD_BYTE1)
            {
                if (basestream.ReadByte() == BinaryLog.HEAD_BYTE2)
                {
                    binary = true;
                }
            }

            Console.WriteLine("Binary: " + binary);

            // back to start
            basestream.Position = 0;
            basestreamlength = basestream.Length;
            DateTime start = DateTime.Now;
            setlinecount();
            Console.WriteLine("DFLogBuffer-linecount: " + Count + " time(ms): " +
                              (DateTime.Now - start).TotalMilliseconds);
            basestream.Position = 0;
        }

        void setlinecount()
        {
            if (string.IsNullOrEmpty(_filename) || !LoadCache())
            {
                byte[] buffer = new byte[1024 * 1024];

                var lineCount = 0l;
                if (binary)
                {
                    long length = basestream.Length;
                    binlog.ScanMessages(basestream, length, (type, offset) =>
                    {
                        // lines are addressed by int
                        if (lineCount <= int.MaxValue)
                            messageindexline[type].Add((int)lineCount);

                        linestartoffset.Add(offset);
                        lineCount++;

                        if (lineCount % 1000000 == 0)
                            Console.WriteLine("reading lines " + lineCount + " " +
                                              ((offset / (double)length) * 100.0));
                    });

                    _count = lineCount;

                    // the lists grow by doubling, give back what they did not use
                    linestartoffset.TrimExcess();
                    foreach (var list in messageindexline)
                        list.TrimExcess();

                    // build fmt line database to pre seed the FMT message
                    messageindexline[128].ForEach(a => dflog.FMTLine(this[(int)a]));

                    try
                    {
                        foreach (var item in dflog.logformat)
                        {
                            var id = item.Value.Id;
                            var type = item.Value.Name;
                            if (messageindexline[id].Count != 0)
                                Console.WriteLine("Seen " + type + " count " + messageindexline[id].Count);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    var offset = 0;
                    // first line starts at 0
                    linestartoffset.Add(0);

                    long length = basestream.Length;
                    while (basestream.Position < length)
                    {
                        offset = 0;

                        long startpos = basestream.Position;

                        int read = basestream.Read(buffer, offset, buffer.Length);

                        while (read > 0)
                        {
                            if (buffer[offset] == '\n')
                            {
                                linestartoffset.Add(startpos + 1 + offset);
                                lineCount++;
                            }

                            offset++;
                            read--;
                        }
                    }

                    _count = lineCount;

                    // create msg cache
                    int b = 0;
                    foreach (var item in this)
                    {
                        var idx = item.IndexOf(',');

                        if (idx <= 0)
                        {
                            b++;
                            continue;
                        }

                        var msgtype = item.Substring(0, idx);

                        if (msgtype == "FMT")
                            dflog.FMTLine(item);

                        if (dflog.logformat.ContainsKey(msgtype))
                        {
                            var type = (byte)dflog.logformat[msgtype].Id;

                            messageindexline[type].Add(b);
                        }

                        b++;
                    }
                }

                SaveCache();
            }


            // build fmt line database using type
            foreach (var item in GetEnumeratorType("FMT"))
            {
                try
                {
                    if (item.items == null || item.items.Length == 0)
                        continue;

                    FMT[int.Parse(item["Type"])] = (
                        int.Parse(item["Length"].Trim()),
                        item["Name"].Trim(),
                        item["Format"].Trim(),
                        item.items.Skip(dflog.FindMessageOffset("FMT", "Columns"))
                            .Aggregate((s, s1) => s.Trim() + "," + s1.Trim())
                            .TrimStart(','));

                    dflog.FMTLine(this[item.lineno]);
                }
                catch
                {
                }
            }

            foreach (var item in GetEnumeratorType("FMTU"))
            {
                try
                {
                    if (item.items == null || item.items.Length == 0)
                        continue;

                    FMTU[int.Parse(item["FmtType"])] =
                        new Tuple<string, string>(item["UnitIds"].Trim(), item["MultIds"].Trim());

                    if (item["UnitIds"].Trim().Contains("#"))
                    {
                        InstanceType[int.Parse(item["FmtType"])] =
                            (item["UnitIds"].Trim().IndexOf("#"), new List<string>());
                    }
                }
                catch
                {
                }
            }

            foreach (var b in InstanceType)
            {
                if (!FMT.ContainsKey(b.Key))
                    continue;
                int a = 0;
                foreach (var item in GetEnumeratorType(FMT[b.Key].name))
                {
                    // trimmed, the fields of a text log start with a space
                    var instancevalue = item.raw[b.Value.index + 1].ToString().Trim();

                    if (!b.Value.value.Contains(instancevalue))
                        b.Value.value.Add(instancevalue);

                    if (a > 2000)
                        break;
                    a++;
                }
            }

            if (dflog.logformat.ContainsKey("UNIT"))
                foreach (var item in GetEnumeratorType("UNIT"))
                {
                    try
                    {
                        Unit[(char)int.Parse(item["Id"])] = item["Label"].Trim();
                    }
                    catch
                    {
                    }
                }

            if (dflog.logformat.ContainsKey("MULT"))
                foreach (var item in GetEnumeratorType("MULT"))
                {
                    try
                    {
                        Mult[(char)int.Parse(item["Id"])] = item["Mult"].Trim();
                    }
                    catch
                    {
                    }
                }

            BuildUnitMultiList();

            int limitcount = 0;
            // used to set the firmware type
            foreach (var item in GetEnumeratorType(new[]
                     {
                         "MSG", "PARM"
                     }))
            {
                // must be the string version to do the firmware type detection - binarylog
                var line = this[(int)item.lineno];
                //Console.WriteLine();
                limitcount++;
                if (limitcount > 100000)
                    break;
            }

            // try get gps time - when a dfitem is created and no valid gpstime has been establish the messages are parsed to get a valid gpstime
            // here we just force the parsing of gps messages to get the valid board time to gps time offset
            int gpsa = 0;
            foreach (var item in GetEnumeratorType(new[]
                     {
                         "GPS", "GPS2", "GPSB"
                     }))
            {
                gpsa++;
                int status = 0;
                if (int.TryParse(item["Status"], out status))
                {
                    if (status >= 3)
                        break;
                }

                // get first gps time
                if (gpsa > 2000)
                    break;
            }

            indexcachelineno = -1;
        }

        // any mismatch (old BinaryFormatter caches included) just rebuilds the index
        // 2: the offsets of each type are no longer stored, line numbers are int
        private const int CacheVersion = 2;
        private static readonly byte[] CacheMagic = Encoding.ASCII.GetBytes("MPDFIDX");
        // BinaryWriter/Reader move 8 bytes per call; unbuffered, every call is a separate deflate call
        private const int CacheBufferSize = 1024 * 1024;

        private string CachePath
        {
            get
            {
                try
                {
                    return Path.GetTempPath() + Path.GetFullPath(_filename).Replace("/", "_").Replace("\\", "_").Replace(":", "_") + Path.GetFileNameWithoutExtension(_filename) + new FileInfo(_filename).Length;
                }
                catch
                {
                    return Path.GetTempFileName();
                }
            }
        }

        private void SaveCache()
        {
            if (string.IsNullOrEmpty(_filename))
                return;
            // save cache if file is over 300mb
            if (basestream.Length < 1024 * 1024 * 300)
                return;
            //save cache, it is optional so a failure must not stop the log from opening
            try
            {
                using (var file = File.Create(CachePath))
                // the cache only has to load faster than the log can be indexed again
                using (var gs = new GZipStream(file, CompressionLevel.Fastest))
                using (var bs = new BufferedStream(gs, CacheBufferSize))
                using (var bw = new BinaryWriter(bs))
                {
                    bw.Write(CacheMagic);
                    bw.Write(CacheVersion);
                    bw.Write(_count);
                    WriteCacheList(bw, linestartoffset);
                    bw.Write(messageindexline.Length);
                    for (int a = 0; a < messageindexline.Length; a++)
                    {
                        WriteCacheList(bw, messageindexline[a]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DFLogBuffer: failed to save cache " + ex.Message);
            }
        }

        private bool LoadCache()
        {
            if (!File.Exists(CachePath))
                return false;

            long lineCount;
            List<long> lineStarts;
            var indexLine = new List<int>[messageindexline.Length];

            //load cache
            try
            {
                using (var file = File.OpenRead(CachePath))
                using (var gs = new GZipStream(file, CompressionMode.Decompress))
                using (var bs = new BufferedStream(gs, CacheBufferSize))
                using (var br = new BinaryReader(bs))
                {
                    if (!br.ReadBytes(CacheMagic.Length).SequenceEqual(CacheMagic) || br.ReadInt32() != CacheVersion)
                        return false;

                    // every line is at least one byte, so a valid count never exceeds the log size
                    long length = basestream.Length;
                    lineCount = br.ReadInt64();
                    if (lineCount < 0 || lineCount > length)
                        return false;

                    // text logs also store the leading 0 offset. the indexers use these to seek and
                    // size reads, so they must be in the log and in order
                    lineStarts = ReadCacheList(br, lineCount + 1, length);
                    if (lineStarts.Count != lineCount + (binary ? 0 : 1))
                        return false;
                    for (int i = 1; i < lineStarts.Count; i++)
                        if (lineStarts[i] < lineStarts[i - 1])
                            return false;

                    if (br.ReadInt32() != indexLine.Length)
                        return false;

                    for (int a = 0; a < indexLine.Length; a++)
                    {
                        indexLine[a] = ReadCacheLineList(br, lineCount, lineCount - 1);
                    }
                }
            }
            catch
            {
                return false;
            }

            messageindexline = indexLine;
            linestartoffset = lineStarts;
            _count = lineCount;

            // build fmt line database to pre seed the FMT message
            messageindexline[128].ForEach(a => dflog.FMTLine(this[(int)a]));
            return true;
        }

        private static void WriteCacheList(BinaryWriter bw, List<long> list)
        {
            bw.Write(list.Count);
            foreach (var item in list)
                bw.Write(item);
        }

        private static void WriteCacheList(BinaryWriter bw, List<int> list)
        {
            bw.Write(list.Count);
            foreach (var item in list)
                bw.Write(item);
        }

        private static List<int> ReadCacheLineList(BinaryReader br, long maxCount, long maxValue)
        {
            var count = br.ReadInt32();
            if (count < 0 || count > maxCount)
                throw new InvalidDataException("bad DFLogBuffer cache");

            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                var item = br.ReadInt32();
                if (item < 0 || item > maxValue)
                    throw new InvalidDataException("bad DFLogBuffer cache");
                list.Add(item);
            }
            return list;
        }

        private static List<long> ReadCacheList(BinaryReader br, long maxCount, long maxValue)
        {
            var count = br.ReadInt32();
            if (count < 0 || count > maxCount)
                throw new InvalidDataException("bad DFLogBuffer cache");

            var list = new List<long>(count);
            for (int i = 0; i < count; i++)
            {
                var item = br.ReadInt64();
                if (item < 0 || item > maxValue)
                    throw new InvalidDataException("bad DFLogBuffer cache");
                list.Add(item);
            }
            return list;
        }

        public void SplitLog(int pieces = 0)
        {
            long length = basestream.Length;

            if (pieces > 0)
            {
                long sizeofpiece = length / pieces;

                for (int i = 0; i < pieces; i++)
                {
                    long start = i * sizeofpiece;
                    long end = start + sizeofpiece;

                    using (var file = File.OpenWrite(_filename + "_split" + i + ".bin"))
                    {
                        var type = dflog.logformat["FMT"];

                        var buffer = new byte[1024 * 256];

                        // fmt from entire file
                        messageindexline[type.Id].ForEach(line =>
                        {
                            basestream.Seek(linestartoffset[line], SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["FMTU"];

                        messageindexline[type.Id].ForEach(line =>
                        {
                            basestream.Seek(linestartoffset[line], SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["UNIT"];

                        messageindexline[type.Id].ForEach(line =>
                        {
                            basestream.Seek(linestartoffset[line], SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["MULT"];

                        messageindexline[type.Id].ForEach(line =>
                        {
                            basestream.Seek(linestartoffset[line], SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });



                        var min = long.MaxValue;
                        var max = long.MinValue;

                        // got min and max valid
                        linestartoffset.ForEach(a =>
                        {
                            if (a >= start && a < end)
                            {
                                min = Math.Min(min, a);
                                max = Math.Max(max, a);
                            }
                        });

                        basestream.Seek(min, SeekOrigin.Begin);

                        while (basestream.Position < max)
                        {
                            int readsize = (int)Math.Min((end - basestream.Position), buffer.Length);
                            int read = basestream.Read(buffer, 0, readsize);
                            file.Write(buffer, 0, read);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Invalid pieces parameters");
            }
        }

        private void BuildUnitMultiList()
        {
            foreach (var msgtype in FMT)
            {
                // get unit and mult info
                var fmtu = FMTU.FirstOrDefault(a => a.Key == msgtype.Key);

                if (fmtu.Value == null)
                    continue;

                var units = fmtu.Value.Item1.ToCharArray().Select(a => Unit.FirstOrDefault(b => b.Key == a));
                var multipliers = fmtu.Value.Item2.ToCharArray().Select(a => Mult.FirstOrDefault(b => b.Key == a));
                var binfmts = msgtype.Value.Item3.ToCharArray();
                var itemcount = msgtype.Value.Item4.Split(',').Length;

                if (binfmts.Length != itemcount)
                    continue;

                for (var i = 0; i < itemcount; i++)
                {
                    var field = msgtype.Value.Item4.Split(',')[i].Trim();
                    var unit = units.Skip(i).FirstOrDefault().Value;
                    var binfmt = binfmts[i];
                    var multi = 1.0;
                    double.TryParse(multipliers.Skip(i).FirstOrDefault().Value, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out multi);

                    if (binfmt == 'c' || binfmt == 'C' ||
                        binfmt == 'e' || binfmt == 'E' ||
                        binfmt == 'L')
                    {
                        // these are scaled from the DF format * 100/1e7 etc
                        // to ensure csv's continue to work we dont modify these values
                        // 1 = no change
                        multi = 1;
                    }

                    UnitMultiList.Add(
                        new Tuple<string, string, string, double>(msgtype.Value.Item2, field, unit, multi));
                }
            }
        }

        public List<Tuple<string, string, string, double>> UnitMultiList =
            new List<Tuple<string, string, string, double>>();

        public Dictionary<int, (int index, List<string> value)> InstanceType =
            new Dictionary<int, (int index, List<string> value)>();

        private string _filename = "";

        public Dictionary<int, (int length, string name, string format, string columns)> FMT { get; set; } =
            new Dictionary<int, (int, string, string, string)>();

        public Dictionary<int, Tuple<string, string>> FMTU { get; set; } = new Dictionary<int, Tuple<string, string>>();

        public Dictionary<char, string> Unit { get; set; } = new Dictionary<char, string>();
        public Dictionary<char, string> Mult { get; set; } = new Dictionary<char, string>();

        public DFLog.DFItem this[long indexin]
        {
            get
            {
                if (indexin > int.MaxValue)
                    throw new Exception("index too large");

                var index = (int)indexin;

                long startoffset = linestartoffset[index];
                long endoffset = startoffset;

                if ((index + 1) >= linestartoffset.Count)
                {
                    endoffset = basestreamlength;
                }
                else
                {
                    endoffset = linestartoffset[index + 1];
                }

                int length = (int)(endoffset - startoffset);

                // prevent multi io to file
                lock (locker)
                {
                    if (binary)
                    {
                        object[] items = null;
                        int size = 0;
                        int decoded = 0;

                        // a message is at most BinaryLog.MaxMessageSize, what follows it up to the next
                        // line is not part of it
                        int count = (int)Math.Min(endoffset - startoffset, BinaryLog.MaxMessageSize);

                        if (count > 0 && FillWindow(startoffset, count))
                            decoded = binlog.TryReadMessageObjects(window, (int)(startoffset - windowstart), count,
                                out items, out size);

                        if (decoded != 1)
                        {
                            // from the stream, after the message if it was bad
                            long from = decoded == -1 ? startoffset + size : startoffset;
                            if (from != basestream.Position)
                                basestream.Seek(from, SeekOrigin.Begin);

                            items = binlog.ReadMessageObjects(basestream, endoffset);
                        }

                        var answer = new DFLog.DFItem(dflog, items, (int)indexin);

                        return answer;
                    }
                    else
                    {
                        if (linestartoffset[index] != basestream.Position)
                            basestream.Seek(linestartoffset[index], SeekOrigin.Begin);

                        byte[] data = new byte[length];

                        basestream.Read(data, 0, length);

                        return dflog.GetDFItemFromLine(ASCIIEncoding.ASCII.GetString(data), (int)indexin);
                    }


                }
            }
        }

        public String this[int index]
        {
            get
            {
                long startoffset = linestartoffset[index];
                long endoffset = startoffset;

                if ((index + 1) >= linestartoffset.Count)
                {
                    endoffset = basestreamlength;
                }
                else
                {
                    endoffset = linestartoffset[index + 1];
                }

                int length = (int)(endoffset - startoffset);

                // prevent multi io to file
                lock (locker)
                {
                    // return cached value is same index
                    if (indexcachelineno == index)
                        return currentindexcache;

                    if (linestartoffset[index] != basestream.Position)
                        basestream.Seek(linestartoffset[index], SeekOrigin.Begin);

                    if (binary)
                    {
                        var answer = binlog.ReadMessage(basestream, basestreamlength);

                        currentindexcache = answer;
                        indexcachelineno = index;
                    }
                    else
                    {
                        byte[] data = new byte[length];

                        basestream.Read(data, 0, length);

                        currentindexcache = ASCIIEncoding.ASCII.GetString(data);
                        indexcachelineno = index;
                    }

                    return currentindexcache;
                }
            }
            set { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            basestream.Close();
            basestream.Dispose();
            _count = 0;
            linestartoffset.Clear();
        }

        public int Count
        {
            get
            {
                if (_count > int.MaxValue) Console.WriteLine("log line count is too large");
                return (int)_count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorTypeAll()
        {
            int position = 0; // state
            while (position < Count)
            {
                yield return this[(long)position];
                position++;
            }
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorType(string type)
        {
            return GetEnumeratorType(new string[] { type });
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorType(string[] types)
        {
            Dictionary<string, List<string>> instances = new Dictionary<string, List<string>>();

            types.ForEach(x =>
            {
                // match ACC[0] GPS[0] or ACC or GPS
                var m = Regex.Match(x, @"(\w+)(\[([0-9]+)\])?", RegexOptions.None);
                if (m.Success)
                {
                    if (!instances.ContainsKey(m.Groups[1].ToString()))
                        instances[m.Groups[1].ToString()] = new List<string>();

                    instances[m.Groups[1].ToString()].Add(m.Groups[3].Success ? m.Groups[3].ToString() : "");
                }
                else
                {
                    if (!instances.ContainsKey(x))
                        instances[x] = new List<string>();

                    instances[x].Add("");
                }

                // match ACC1  GYR1
                m = Regex.Match(x, @"(\w+)([0-9]+)$", RegexOptions.None);
                if (m.Success)
                {
                    if (!instances.ContainsKey(m.Groups[1].ToString()))
                        instances[m.Groups[1].ToString()] = new List<string>();

                    instances[m.Groups[1].ToString()]
                        .Add(m.Groups[2].Success ? (int.Parse(m.Groups[2].ToString()) - 1).ToString() : "");
                }
            });

            // get the ids for the passed in types
            var lists = new List<List<int>>();
            foreach (var type in instances.Keys)
            {
                if (dflog.logformat.ContainsKey(type))
                {
                    var typeid = (byte)dflog.logformat[type].Id;

                    lists.Add(messageindexline[typeid]);
                }
            }

            // the lines of a single type are used as they are, the index does not change once built
            List<int> slist;
            if (lists.Count == 1)
            {
                slist = lists[0];
            }
            else
            {
                slist = new List<int>(lists.Sum(a => a.Count));
                foreach (var list in lists)
                    slist.AddRange(list);

                if (types.Length > 1)
                    slist.Sort();
            }

            // types that are wanted whatever their instance
            var allinstances = new HashSet<string>();
            foreach (var inst in instances)
            {
                if (inst.Value.Contains(""))
                    allinstances.Add(inst.Key);
            }

            int progress = Environment.TickCount;
            // work through list of lines
            for (int i = 0; i < slist.Count; i++)
            {
                var l = slist[i];

                // once a second
                if (unchecked(Environment.TickCount - progress) >= 1000)
                {
                    Console.WriteLine(l);
                    progress = Environment.TickCount;
                }

                var ans = this[(long)l];
                var msgtype = ans.msgtype;
                if (!allinstances.Contains(msgtype))
                {
                    List<string> inst;
                    if (!instances.TryGetValue(msgtype, out inst))
                        continue;
                    // instance was requested, and its not a match
                    if (!inst.Contains(ans.instance))
                        continue;
                }

                yield return ans;
            }
        }

        public IEnumerator<String> GetEnumerator()
        {
            int position = 0; // state
            while (position < Count)
            {
                yield return this[position];
                position++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            basestream.Close();
            linestartoffset.Clear();
            linestartoffset = null;
            messageindexline = null;
            GC.Collect();
        }

        public bool EndOfStream
        {
            get { return (indexcachelineno >= (linestartoffset.Count - 1)); }
        }

        public List<string> SeenMessageTypes
        {
            get
            {
                List<string> messagetypes = new List<string>();

                for (int a = 0; a < messageindexline.Length; a++)
                {
                    if (messageindexline[a].Count > 0 && FMT.ContainsKey(a))
                        messagetypes.Add(FMT[a].Item2);
                }

                return messagetypes;
            }
        }

        public Tuple<string, double> GetUnit(string type, string header)
        {
            var answer = UnitMultiList.Where(tuple => tuple.Item1 == type && tuple.Item2 == header);

            if (answer.Count() == 0)
                return new Tuple<string, double>("", 1);

            return new Tuple<string, double>(answer.First().Item3, answer.First().Item4);
        }

        public int getInstanceIndex(string type)
        {
            if (!dflog.logformat.ContainsKey(type))
                return -1;

            var typeno = dflog.logformat[type].Id;

            if (!FMTU.ContainsKey(typeno))
                return -1;

            var unittypes = FMTU[typeno].Item1;

            int colinst = unittypes.IndexOf("#") + 1;
            return colinst;
        }
    }
}