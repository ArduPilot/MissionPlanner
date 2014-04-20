using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
{
    public class CollectionBuffer<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {

        Stream basestream;
        private int _count;
        List<long> linestartoffset = new List<long>();

        public CollectionBuffer(Stream instream)
        {
            basestream = instream;

            _count = getlinecount();

            basestream.Seek(0, SeekOrigin.Begin);
        }

        int getlinecount()
        {
            var lineCount = 0;
            while (basestream.Position < basestream.Length)
            {
                if (basestream.ReadByte() == '\n')
                {
                    linestartoffset.Add(basestream.Position + 1);
                    lineCount++;
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
                throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}