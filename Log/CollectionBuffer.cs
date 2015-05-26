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

        int indexcachelineno = -1;
        object currentindexcache = null;

        public CollectionBuffer(Stream instream)
        {
            basestream = new BufferedStream(instream,1024*256);

            _count = getlinecount();

            basestream.Seek(0, SeekOrigin.Begin);
        }

        int getlinecount()
        {
            // first line starts at 0
            linestartoffset.Add(0);

            int offset = 0;

            byte[] buffer = new byte[1024 * 1024];

            var lineCount = 0;
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

                if ((index + 1) > linestartoffset.Count)
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

                byte[] data = new byte[length];

                basestream.Read(data,0,length);

                currentindexcache = (object)ASCIIEncoding.ASCII.GetString(data);
                indexcachelineno = index;

                return (T)currentindexcache;

                /*
                while (basestream.Position < basestream.Length)
                {
                    byte cha = (byte)basestream.ReadByte();

                    sb.Append((char)cha);
                    if (cha == '\n')
                    {
                        break;
                    }
                }

                return (T)(object)sb.ToString();
                 */
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
            throw new NotImplementedException();
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