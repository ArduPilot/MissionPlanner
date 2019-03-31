using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class CollectionBuffer : IEnumerable<String>, IDisposable
    {
        // used for binary log line conversion
        BinaryLog binlog = new BinaryLog();

        // used for fmt messages
        public DFLog dflog 
        {
            get { return _dflog; }
        }

        DFLog _dflog = new DFLog();

        Stream basestream;
        private int _count;
        List<uint> linestartoffset = new List<uint>();

        List<uint>[] messageindex = new List<uint>[256];
        List<uint>[] messageindexline = new List<uint>[256];

        bool binary = false;

        object locker = new object();

        int indexcachelineno = -1;
        String currentindexcache = null;

        public CollectionBuffer(Stream instream)
        {
            for (int a = 0; a < messageindex.Length; a++)
            {
                messageindex[a] = new List<uint>();
                messageindexline[a] = new List<uint>();
            }

            basestream = new MemoryStream((int) instream.Length);
            instream.CopyTo(basestream);
            basestream.Position = 0;
            instream.Close();

            if (basestream.ReadByte() == BinaryLog.HEAD_BYTE1)
            {
                if (basestream.ReadByte() == BinaryLog.HEAD_BYTE2)
                {
                    binary = true;
                }
            }

            // back to start
            basestream.Position = 0;
            DateTime start = DateTime.Now;
            setlinecount();
            Console.WriteLine("CollectionBuffer-linecount: " + (DateTime.Now - start).TotalMilliseconds);
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
                int amax = Math.Min(2000, _count - 1);
                for (int a = 0; a < amax; a++)
                {
                    dflog.GetDFItemFromLine(this[a].ToString(), a);
                }
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
                    FMT[int.Parse(item["Type"])] = new Tuple<int, string, string, string>(
                        int.Parse(item["Length"].Trim()),
                        item["Name"].Trim(),
                        item["Format"].Trim(),
                        item.items.Skip(dflog.FindMessageOffset("FMT", "Columns")).FirstOrDefault());

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
                }
                catch { }
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

            foreach (var item in GetEnumeratorType(new[]
            {
                "GPS", "GPS2"
            }))
            {
                // get first gps time
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

                for (var i = 0; i < msgtype.Value.Item4.Split(',').Length; i++)
                {
                    var field = msgtype.Value.Item4.Split(',')[i].Trim();
                    var unit = units.Skip(i).First().Value;
                    var binfmt = binfmts[i];
                    var multi = 1.0;
                    double.TryParse(multipliers.Skip(i).First().Value, out multi);

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

        public Dictionary<int, Tuple<int, string, string, string>> FMT { get; set; } = new Dictionary<int, Tuple<int, string, string, string>>();
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
                        var items = binlog.ReadMessageObjects(basestream, basestream.Length);

                        //var test = dflog.GetDFItemFromLine(this[index], index);

                        var answer =  new DFLog.DFItem(dflog, items, (int)indexin);

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
                position++;
                yield return this[(long)position - 1];
            }
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorType(string type)
        {
            return GetEnumeratorType(new string[] {type});
        }

        public IEnumerable<DFLog.DFItem> GetEnumeratorType(string[] types)
        {
            // get the ids for the passed in types
            SortedSet<long> slist = new SortedSet<long>();
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
                position++;
                yield return this[position - 1];
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