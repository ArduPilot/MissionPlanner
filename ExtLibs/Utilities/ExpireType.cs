using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class ExpireType<T>
    {
        DateTime _setAt;
        DateTime _expireAt;
        T _value;

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ExpireType(T input, double seconds, T expiredValue)
        {
            Value = input;
            _setAt = DateTime.Now;
            _expireAt = _setAt.AddSeconds(seconds);
        }
    }

    public static class ExpireType
    {
        static Dictionary<object, DateTime> _setAt = new Dictionary<object, DateTime>();
        static Dictionary<object, DateTime> _expireAt = new Dictionary<object, DateTime>();

        public static void Set(object input, double seconds)
        {
            _setAt[input] = DateTime.Now;
            _expireAt[input] = DateTime.Now.AddSeconds(seconds);

            cleanup();
        }

        public static bool HasExpired(object check)
        {
            // has it been set?
            if (_expireAt.ContainsKey(check))
            {
                if (_expireAt[check] < DateTime.Now)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        static void cleanup()
        {
            var now = DateTime.Now;
            foreach (var item in _expireAt.ToArray())
            {
                if (item.Value < now)
                {
                    _setAt.Remove(item);
                    _expireAt.Remove(item);
                }
            }
        }
    }
}
