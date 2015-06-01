using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
{
    public class CollectionBuffer<T> : IList<T>, ICollection<T>, IEnumerable<T>, IDisposable
    {
        BufferedStream basestream;
        private int _count;
        List<long> linestartoffset = new List<long>();

        Dictionary<byte, List<long>> messageindex = new Dictionary<byte, List<long>>();

        bool binary = false;

        int indexcachelineno = -1;
        object currentindexcache = null;

        public CollectionBuffer(Stream instream)
        {
            for (byte a = 0; a < byte.MaxValue; a++) 
            {
                messageindex[a] = new List<long>();
            }

            basestream = new BufferedStream(instream,1024*256);

            if (basestream.ReadByte() == BinaryLog.HEAD_BYTE1)
            {
                if (basestream.ReadByte() == BinaryLog.HEAD_BYTE2)
                {
                    binary = true;
                }
            }

            // back to start
            basestream.Seek(0, SeekOrigin.Begin);

            _count = getlinecount();

            basestream.Seek(0, SeekOrigin.Begin);
        }

        int getlinecount()
        {
            int offset = 0;

            byte[] buffer = new byte[1024 * 1024];

            var lineCount = 0;

            if (binary)
            {
                while (basestream.Position < basestream.Length)
                {
                    offset = 0;

                    // seek back 5 on each buffer fill
                    if (basestream.Position > 10)
                        basestream.Seek(-5, SeekOrigin.Current);

                    long startpos = basestream.Position;

                    int read = basestream.Read(buffer, offset, buffer.Length);

                    // 5 byte overlap
                    while (read > 2)
                    {
                        if (buffer[offset] == BinaryLog.HEAD_BYTE1 && buffer[offset + 1] == BinaryLog.HEAD_BYTE2)
                        {
                            byte type = buffer[offset + 2];
                            messageindex[type].Add(startpos + offset);

                            linestartoffset.Add(startpos + offset);
                            lineCount++;
                        }

                        offset++;
                        read--;
                    }
                }
                return lineCount;
            }

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
                        linestartoffset.Add(startpos + 1 + offset);
                        lineCount++;
                    }

                    offset++;
                    read--;
                }
            }
            return lineCount;
        }


        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                // return cached value is same index
                if (indexcachelineno == index)
                    return (T)currentindexcache;

                long startoffset = linestartoffset[index];
                long endoffset=startoffset;

                if ((index + 1) >= linestartoffset.Count)
                {
                    endoffset = basestream.Length;
                }
                else
                {
                    endoffset = linestartoffset[index + 1];
                }

                int length = (int)(endoffset - startoffset);

                if (linestartoffset[index] != basestream.Position)
                    basestream.Seek(linestartoffset[index], SeekOrigin.Begin);

                if (binary)
                {
                    var answer = BinaryLog.ReadMessage(basestream);

                    currentindexcache = (object)answer;
                    indexcachelineno = index;
                }
                else
                {
                    byte[] data = new byte[length];

                    basestream.Read(data, 0, length);

                    currentindexcache = (object)ASCIIEncoding.ASCII.GetString(data);
                    indexcachelineno = index;
                }

                return (T)currentindexcache;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            basestream.Close();
            _count = 0;
            linestartoffset.Clear();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int position = 0; // state
            while (position < Count)
            {
                position++;
                yield return this[position-1];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            basestream.Close();
            linestartoffset = null;
        }
    }
}