using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private int _count;
        List<uint> linestartoffset = new List<uint>();

        /// <summary>
        /// Type and offsets
        /// </summary>
        List<uint>[] messageindex = new List<uint>[256];
        /// <summary>
        /// Type and line numbers
        /// </summary>
        List<uint>[] messageindexline = new List<uint>[256];

        bool binary = false;

        object locker = new object();

        int indexcachelineno = -1;
        String currentindexcache = null;

        public DFLogBuffer(string filename) : this(File.Open(filename,FileMode.Open,FileAccess.Read,FileShare.Read))
        {
        }

        public DFLogBuffer(Stream instream)
        {
            dflog = new DFLog(this);
            for (int a = 0; a < messageindex.Length; a++)
            {
                messageindex[a] = new List<uint>();
                messageindexline[a] = new List<uint>();
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
            Console.WriteLine("DFLogBuffer-linecount: " + Count + " time(ms): " + (DateTime.Now - start).TotalMilliseconds);
            basestream.Position = 0;
        }

        void setlinecount()
        {
            int offset = 0;

            byte[] buffer = new byte[1024*1024];

            var lineCount = 0;

            if (binary)
            {
                long length = basestream.Length;
                while (basestream.Position < length)
                {
                    var ans = binlog.ReadMessageTypeOffset(basestream, length);

                    if (ans.MsgType == 0 && ans.Offset == 0)
                        continue;

                    byte type = ans.Item1;
                    messageindex[type].Add((uint)(ans.Item2));
                    messageindexline[type].Add((uint) lineCount);

                    linestartoffset.Add((uint)(ans.Item2));
                    lineCount++;
                }
                
                _count = lineCount;

                // build fmt line database to pre seed the FMT message
                messageindexline[128].ForEach(a => dflog.FMTLine(this[(int) a]));
            }
            else
            {
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
						continue;
					
                    var msgtype = item.Substring(0, idx);

                    if(msgtype == "FMT")
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

            // build fmt line database using type
            foreach (var item in GetEnumeratorType("FMT"))
            {
                try
                {
                    FMT[int.Parse(item["Type"])] = (
                        int.Parse(item["Length"].Trim()),
                        item["Name"].Trim(),
                        item["Format"].Trim(),
                        item.items.Skip(dflog.FindMessageOffset("FMT", "Columns")).Aggregate((s, s1) => s.Trim() + "," + s1.Trim())
                            .TrimStart(','));

                    dflog.FMTLine(this[item.lineno]);
                }
                catch { }
            }

            foreach (var item in GetEnumeratorType("FMTU"))
            {
                try
                {
                    FMTU[int.Parse(item["FmtType"])] =
                        new Tuple<string, string>(item["UnitIds"].Trim(), item["MultIds"].Trim());

                    if (item["UnitIds"].Trim().Contains("#"))
                    {
                        InstanceType[int.Parse(item["FmtType"])] =
                            (item["UnitIds"].Trim().IndexOf("#"), new List<string>());
                    }
                }
                catch { }
            }

            foreach (var b in InstanceType)
            {
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

            foreach (var item in GetEnumeratorType("UNIT"))
            {
                try
                {
                    Unit[(char)int.Parse(item["Id"])] = item["Label"].Trim();
                }
                catch { }
            }

            foreach (var item in GetEnumeratorType("MULT"))
            {
                try
                {
                    Mult[(char)int.Parse(item["Id"])] = item["Mult"].Trim();
                }
                catch { }
            }

            BuildUnitMultiList();

            // used to set the firmware type
            foreach (var item in GetEnumeratorType(new[]
            {
                "MSG", "PARM"
            }))
            {
                // must be the string version to do the firmware type detection - binarylog
                Console.WriteLine(this[(int) item.lineno]);
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

        private void BuildUnitMultiList()
        {
            foreach (var msgtype in FMT)
            {
                // get unit and mult info
                var fmtu = FMTU.FirstOrDefault(a => a.Key == msgtype.Key);

                if(fmtu.Value == null)
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

        public List<Tuple<string,string,string,double>> UnitMultiList = new List<Tuple<string, string, string, double>>();
        public Dictionary<int, (int index, List<string> value)> InstanceType = new Dictionary<int, (int index, List<string> value)>();

        public Dictionary<int, (int length, string name, string format, string columns)> FMT { get; set; } =
            new Dictionary<int, (int, string, string, string)>();
        public Dictionary<int, Tuple<string, string>> FMTU { get; set; } = new Dictionary<int, Tuple<string, string>>();

        public Dictionary<char, string> Unit { get; set; } = new Dictionary<char, string>();
        public Dictionary<char, string> Mult { get; set; } = new Dictionary<char, string>();

        public DFLog.DFItem this[long indexin]
        {
            get
            {
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

                int length = (int) (endoffset - startoffset);

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
            _count = 0;
            linestartoffset.Clear();
        }

        public int Count
        {
            get { return _count; }
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
            return GetEnumeratorType(new string[] {type});
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorType(string[] types)
        {
            // get the ids for the passed in types
            List<long> slist = new List<long>();
            foreach (var type in types.Distinct())
            {
                if (dflog.logformat.ContainsKey(type))
                {
                    var typeid = (byte) dflog.logformat[type].Id;

                    foreach (var item in messageindexline[typeid])
                    {
                        slist.Add(item);
                    }
                }
            }

            if(types.Length > 1)
                slist.Sort();

            // work through list of lines
            foreach (var l in slist)
            {
                yield return this[(long) l];
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
            get
            {
                return (indexcachelineno >= (linestartoffset.Count-1)); 
            }
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

        public String ReadLine()
        {
            return this[indexcachelineno+1];
        }

        public Tuple<string,double> GetUnit(string type, string header)
        {
            var answer = UnitMultiList.Where(tuple => tuple.Item1 == type && tuple.Item2 == header);

            if (answer.Count() == 0)
                return new Tuple<string, double>("", 1);

            return new Tuple<string, double>(answer.First().Item3, answer.First().Item4);
        }
    }
}