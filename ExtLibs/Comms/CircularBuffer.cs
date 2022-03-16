using System.Collections.Generic;

namespace MissionPlanner.Comms
{
    public class CircularBuffer<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _size;

        public CircularBuffer(int size)
        {
            _queue = new Queue<T>(size);
            _size = size;
        }

        public void Add(T obj)
        {
            if (_queue.Count == _size)
            {
                _queue.Dequeue();
                _queue.Enqueue(obj);
            }
            else
            {
                _queue.Enqueue(obj);
            }
        }

        public T Read()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public int Length()
        {
            return _queue.Count;
        }
    }
}