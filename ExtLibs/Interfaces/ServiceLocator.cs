using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public class ServiceLocator
    {
        private static Dictionary<Type, object> _list = new Dictionary<Type, object>();
        private static object _lock = new object();

        public static void Register<T>(Func<T> start)
        {
            lock (_lock)
            {
                if (_list.ContainsKey(typeof(T)))
                {
                    throw new Exception("Type already exists");
                }

                _list.Add(typeof(T), start());
            }
        }

        public static T Get<T>()
        {
            lock (_lock)
            {
                var ans = _list[typeof(T)];

                return (T)ans;
            }
        }
    }
}