﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        private long _count;
        List<long> linestartoffset = new List<long>();

        /// <summary>
        /// Type and offsets
        /// </summary>
        List<long>[] messageindex = new List<long>[256];

        /// <summary>
        /// Type and line numbers
        /// </summary>
        List<long>[] messageindexline = new List<long>[256];

        bool binary = false;

        object locker = new object();

        long indexcachelineno = -1;
        String currentindexcache = null;

        public DFLogBuffer(string filename) : this(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            _filename = filename;
        }

        public DFLogBuffer(Stream instream)
        {
            if (instream is FileStream)
                _filename = ((FileStream)instream).Name;

            dflog = new DFLog(this);
            for (int a = 0; a < messageindex.Length; a++)
            {
                messageindex[a] = new List<long>(0);
                messageindexline[a] = new List<long>(0);
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
            DateTime start = DateTime.Now;
            setlinecount();
            Console.WriteLine("DFLogBuffer-linecount: " + Count + " time(ms): " +
                              (DateTime.Now - start).TotalMilliseconds);
            basestream.Position = 0;
        }

        void setlinecount()
        {
            if (!LoadCache())
            {
                byte[] buffer = new byte[1024 * 1024];

                var lineCount = 0l;
                if (binary)
                {
                    long length = basestream.Length;
                    while (basestream.Position < length)
                    {
                        var ans = binlog.ReadMessageTypeOffset(basestream, length);

                        if (ans.MsgType == 0 && ans.Offset == 0)
                            continue;

                        byte type = ans.Item1;
                        messageindex[type].Add(ans.Item2);
                        messageindexline[type].Add(lineCount);

                        linestartoffset.Add(ans.Item2);
                        lineCount++;

                        if (lineCount % 1000000 == 0)
                            Console.WriteLine("reading lines " + lineCount + " " +
                                              ((basestream.Position / (double)length) * 100.0));
                    }

                    _count = lineCount;

                    // build fmt line database to pre seed the FMT message
                    messageindexline[128].ForEach(a => dflog.FMTLine(this[(int)a]));

                    try
                    {
                        foreach (var item in dflog.logformat)
                        {
                            var id = item.Value.Id;
                            var type = item.Value.Name;
                            if (messageindex[id].Count != 0)
                                Console.WriteLine("Seen " + type + " count " + messageindex[id].Count);
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
                                linestartoffset.Add((uint)(startpos + 1 + offset));
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

                            messageindex[type].Add(linestartoffset[b]);
                            messageindexline[type].Add((uint)b);
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
                    var instancevalue = item.raw[b.Value.index + 1].ToString();

                    if (!b.Value.value.Contains(instancevalue))
                        b.Value.value.Add(instancevalue);

                    if (a > 2000)
                        break;
                    a++;
                }
            }

            if (Unit.Count > 0)
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

            if (Mult.Count > 0)
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

        [Serializable]
        struct cache
        {
            public List<long>[] messageindex;
            public List<long>[] messageindexline;
            public List<long> linestartoffset;
            public long lineCount;
        }

        private string CachePath
        {
            get
            {
                try
                {
                    return Path.GetTempPath() + Path.GetFileNameWithoutExtension(_filename) + new FileInfo(_filename).Length;
                }
                catch
                {
                    return Path.GetTempFileName();
                }
            }
        }

        private void SaveCache()
        {
            // save cache if file is over 300mb
            if (basestream.Length < 1024 * 1024 * 300)
                return;
            //save cache
            cache cache = new cache();
            cache.messageindex = messageindex;
            cache.messageindexline = messageindexline;
            cache.linestartoffset = linestartoffset;
            cache.lineCount = _count;

            using (var file = File.OpenWrite(CachePath))
            {
                using (GZipStream gs = new GZipStream(file, CompressionMode.Compress))
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(gs, cache);
                }
            }
        }

        private bool LoadCache()
        {
            if (File.Exists(CachePath))
            {
                //load cache
                cache cache = new cache();
                BinaryFormatter deserializer = new BinaryFormatter();
                using (var file = File.OpenRead(CachePath))
                {
                    using (GZipStream gs = new GZipStream(file, CompressionMode.Decompress))
                    {
                        cache = (cache)deserializer.Deserialize(gs);
                    }
                }

                messageindex = cache.messageindex;
                messageindexline = cache.messageindexline;
                linestartoffset = cache.linestartoffset;
                _count = cache.lineCount;

                // build fmt line database to pre seed the FMT message
                messageindexline[128].ForEach(a => dflog.FMTLine(this[(int)a]));
                return true;
            }

            return false;
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
                        messageindex[type.Id].ForEach(a =>
                        {
                            basestream.Seek(a, SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["FMTU"];

                        messageindex[type.Id].ForEach(a =>
                        {
                            basestream.Seek(a, SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["UNIT"];

                        messageindex[type.Id].ForEach(a =>
                        {
                            basestream.Seek(a, SeekOrigin.Begin);
                            int read = basestream.Read(buffer, 0, type.Length);
                            file.Write(buffer, 0, read);
                        });

                        type = dflog.logformat["MULT"];

                        messageindex[type.Id].ForEach(a =>
                        {
                            basestream.Seek(a, SeekOrigin.Begin);
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
                    endoffset = basestream.Length;
                }
                else
                {
                    endoffset = linestartoffset[index + 1];
                }

                int length = (int)(endoffset - startoffset);

                // prevent multi io to file
                lock (locker)
                {
                    if (linestartoffset[index] != basestream.Position)
                        basestream.Seek(linestartoffset[index], SeekOrigin.Begin);

                    if (binary)
                    {
                        var items = binlog.ReadMessageObjects(basestream, endoffset);

                        var answer = new DFLog.DFItem(dflog, items, (int)indexin);

                        return answer;
                    }
                    else
                    {
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
                    endoffset = basestream.Length;
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
                        var answer = binlog.ReadMessage(basestream, basestream.Length);

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
            List<long> slist = new List<long>();
            foreach (var type in instances.Keys)
            {
                if (dflog.logformat.ContainsKey(type))
                {
                    var typeid = (byte)dflog.logformat[type].Id;

                    foreach (var item in messageindexline[typeid])
                    {
                        slist.Add(item);
                    }
                }
            }

            if (types.Length > 1)
                slist.Sort();

            int progress = DateTime.Now.Second;
            // work through list of lines
            foreach (var l in slist)
            {
                if (DateTime.Now.Second != progress)
                {
                    Console.WriteLine(l);
                    progress = DateTime.Now.Second;
                }

                var ans = this[(long)l];
                if (!instances.ContainsKey(ans.msgtype))
                    continue;
                var inst = instances[ans.msgtype];
                // instance was requested, and its not a match
                //if (inst != "" && ans.instance != inst)
                if (!inst.Contains("") && !inst.Contains(ans.instance))
                    continue;
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
            messageindex = null;
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

                for (int a = 0; a < messageindex.Length; a++)
                {
                    if (messageindex[a].Count > 0 && FMT.ContainsKey(a))
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