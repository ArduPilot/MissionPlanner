using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
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

        BufferedStream basestream;
        private int _count;
        List<uint> linestartoffset = new List<uint>();

        Dictionary<byte, List<uint>> messageindex = new Dictionary<byte, List<uint>>();

        bool binary = false;

        object locker = new object();

        int indexcachelineno = -1;
        String currentindexcache = null;

        public CollectionBuffer(Stream instream)
        {
            for (int a = 0; a <= byte.MaxValue; a++)
            {
                messageindex[(byte) a] = new List<uint>();
            }

            basestream = new BufferedStream(instream, 1024*256);

            if (basestream.ReadByte() == BinaryLog.HEAD_BYTE1)
            {
                if (basestream.ReadByte() == BinaryLog.HEAD_BYTE2)
                {
                    binary = true;
                }
            }

            // back to start
            basestream.Seek(0, SeekOrigin.Begin);

            setlinecount();

            basestream.Seek(0, SeekOrigin.Begin);
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
                    var ans = binlog.ReadMessageTypeOffset(basestream);

                    if (ans == null)
                        continue;

                    byte type = ans.Item1;
                    messageindex[type].Add((uint)(ans.Item2));

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

                // build fmt line database using type
                foreach (var item in GetEnumeratorType("FMT"))
                {
                    //Console.WriteLine("Found FMT");
                    bool t = true;
                }
            }
            else
            {
                // first line starts at 0
                linestartoffset.Add(0);

                while (basestream.Position < basestream.Length)
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
                    var dfitem = dflog.GetDFItemFromLine(item.ToString(), b);
                    if (dfitem.msgtype != null && dflog.logformat.ContainsKey(dfitem.msgtype))
                    {
                        var type = (byte)dflog.logformat[dfitem.msgtype].Id;

                        messageindex[type].Add(linestartoffset[b]);
                    }
                    b++;
                }
            }

            indexcachelineno = -1;
        }

        public String this[int index]
        {
            get
            {
                // prevent multi io to file
                lock (locker)
                {
                    // return cached value is same index
                    if (indexcachelineno == index)
                        return currentindexcache;

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

                    if (linestartoffset[index] != basestream.Position)
                        basestream.Seek(linestartoffset[index], SeekOrigin.Begin);

                    if (binary)
                    {
                        var answer = binlog.ReadMessage(basestream);

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
                yield return dflog.GetDFItemFromLine(this[position - 1], position - 1);
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
            foreach (var type in types)
            {
                if (dflog.logformat.ContainsKey(type))
                {
                    var typeid = (byte) dflog.logformat[type].Id;

                    foreach (var item in messageindex[typeid])
                    {
                        slist.Add(item);
                    }
                }
            }

            int position = 0; // state
            while (position < Count)
            {
                position++;

                if (slist.Contains(linestartoffset[position - 1]))
                {
                    yield return dflog.GetDFItemFromLine(this[position - 1].ToString(), position - 1);
                }
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
        }

        public bool EndOfStream 
        {
            get
            {
                return (indexcachelineno >= (linestartoffset.Count-1)); 
            }
        }

        public String ReadLine()
        {
            return this[indexcachelineno+1];
        }
    }
}