using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Comms;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public static class Extensions
    {
        public static Action MessageLoop;
        //https://medium.com/rubrikkgroup/understanding-async-avoiding-deadlocks-e41f8f2c6f5d
        public static T AwaitSync<T>(this Task<T> infunc)
        {
            var tsk = Task.Run<T>(async () =>
            {
                return await infunc.ConfigureAwait(true);
            });

            return tsk.GetAwaiter().GetResult();
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        /// <summary>
        /// Chunk based on a field selector from the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="checkmatching"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ChunkByField<T>(this IEnumerable<T> source, Func<T,T, int, bool> checkmatching)
        {
            while (source.Any())
            {
                // store the first element to compare against
                T first = source.First();
                int include = 0;
                // build the list by comparing the first to all after it
                var sublist = source.TakeWhile((a,count) =>
                {
                    if (first.Equals(a))
                    {
                        include++;
                        return true;
                    }

                    var check = checkmatching(first, a, count);
                    if(check)
                        include++;
                    return check;
                }).ToList(); // tolist improves performance vs ienumerable requery on every access
                yield return sublist;
                // continue on in the list
                source = source.Skip(include);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> extras )
        {
            extras.ForEach(a => list.Add(a));
        }

        public static void Stop(this System.Threading.Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void Start(this System.Threading.Timer timer, int intervalms)
        {
            timer.Change(intervalms, intervalms);
        }

        public static byte[] ToByteArray(this char[] input)
        {
            return input.Select(a => (byte) a).ToArray();
        }

        public static string ToJSON(this object msg, Formatting fmt)
        {
            return JsonConvert.SerializeObject(msg, fmt, new JsonSerializerSettings()
            {
                Error =
                    (sender, args) => { args.ErrorContext.Handled = true; }
            });
        }

        public static string ToJSON(this object msg)
        {
            return msg.ToJSON(Formatting.Indented);
        }

        public static T FromJSON<T>(this string msg)
        {
            return JsonConvert.DeserializeObject<T>(msg);
        }

        public static string CleanString(this string dirtyString)
        {
            return new String(dirtyString.Where(Char.IsLetterOrDigit).ToArray());
        }

        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
        
        public static byte[] MakeSize(this byte[] buffer, int length)
        {
            if (buffer.Length == length)
                return buffer;
            Array.Resize(ref buffer, length);
            return buffer;
        }

        public static byte[] MakeBytesSize(this string item, int length)
        {
            var buffer = ASCIIEncoding.ASCII.GetBytes(item);
            if (buffer.Length == length)
                return buffer;
            Array.Resize(ref buffer, length);
            return buffer;
        }
        public static byte[] MakeBytes(this string item)
        {
            var buffer = ASCIIEncoding.ASCII.GetBytes(item);
            return buffer;
        }

        public static char[] MakeCharSize(this string item, int length)
        {
            var buffer = item.ToCharArray();
            if (buffer.Length == length)
                return buffer;
            Array.Resize(ref buffer, length);
            return buffer;
        }
        public static MemoryStream ToMemoryStream(this byte[] buffer)
        {
            return new MemoryStream(buffer);
        }

        public static string TrimUnPrintable(this string input)
        {
            return Regex.Replace(input, @"[^\u0020-\u007E]", String.Empty);
        }

        public static double ConvertToDouble(this object input)
        {
            if (input.GetType() == typeof(float))
            {
                return (float)input;
            }
            if (input.GetType() == typeof(double))
            {
                return (double)input;
            }
            if (input.GetType() == typeof(ulong))
            {
                return (ulong)input;
            }
            if (input.GetType() == typeof(long))
            {
                return (long)input;
            }
            if (input.GetType() == typeof(int))
            {
                return (int)input;
            }
            if (input.GetType() == typeof(uint))
            {
                return (uint)input;
            }
            if (input.GetType() == typeof(short))
            {
                return (short)input;
            }
            if (input.GetType() == typeof(ushort))
            {
                return (ushort)input;
            }
            if (input.GetType() == typeof(byte))
            {
                return (byte)input;
            }
            if (input.GetType() == typeof(sbyte))
            {
                return (sbyte)input;
            }
            if (input.GetType() == typeof(bool))
            {
                return (bool)input ? 1 : 0;
            }
            if (input.GetType() == typeof(string))
            {
                double ans = 0;
                if (double.TryParse((string)input, out ans))
                {
                    return ans;
                }
            }
            if (input is Enum)
            {
                return Convert.ToInt32(input);
            }

            if (input == null)
                throw new Exception("Bad Type Null");
            else
                throw new Exception("Bad Type " + input.GetType().ToString());
        }

        public static void CallWithTimeout(this Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }

        public static void CallWithTimeout<T>(Action<T> action, int timeoutMilliseconds, T data)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action(data);
            };

            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }

        public static void Add<T, T2>(this List<Tuple<T, T2>> input, T in1, T2 in2)
        {
            input.Add(new Tuple<T, T2>(in1, in2));
        }

        public static void Add<T, T2, T3>(this List<Tuple<T, T2, T3>> input, T in1, T2 in2, T3 in3)
        {
            input.Add(new Tuple<T, T2, T3>(in1, in2, in3));
        }

        public static bool IsNumber(this string value)
        {
            decimal num;
            return decimal.TryParse(value, out num);
        }

        public static bool IsNumber(this object value)
        {
            return IsNumber(value?.GetType());
        }

        public static bool IsNumber(this Type value)
        {
            if (value == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(value))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (value.IsGenericType && value.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(value).IsNumber();
                    }

                    return false;
                default:
                    return false;
            }
        }

        public static IEnumerable<MAVLink.MAVLinkMessage> GetMessageOfType(this CommsFile commsFile,
            MAVLink.MAVLINK_MSG_ID[] packetids = null, bool hasTimestamp = false)
        {
            var parse = new MAVLink.MavlinkParse(hasTimestamp);

            var list = packetids.Cast<uint>();

            while (commsFile.BytesToRead > 0)
            {
                var packet = parse.ReadPacket(commsFile.BaseStream);
                if (packet == null)
                    continue;
                if (packetids == null || list.Contains(packet.msgid))
                    yield return packet;
            }
        }

        public static void DeDupOrderedList<T>(this List<T> list)
        {
            int a = 0;
            while (a < (list.Count-2))
            {
                if (list[a].Equals(list[a + 1]))
                {
                    list.RemoveAt(a + 1);
                    continue;
                }

                a++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="source">Source list</param>
        /// <param name="seed">Start value</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed,
            Func<TAccumulate, TSource, TSource, TAccumulate> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (source.Count() == 0)
                return seed;
            TAccumulate val = seed;
            TSource last = source.First();
            int a = -1;
            foreach (TSource item in source)
            {
                a++;
                if (a == 0)
                {
                    last = item;
                    continue;
                }

                val = func(val, last, item);
                last = item;
            }
            return val;
        }

        public static IEnumerable<int> SteppedRange(int fromInclusive, int toExclusive, int step)
        {
            for (var i = fromInclusive; i < toExclusive; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<double> SteppedRange(double fromInclusive, double toExclusive, double step)
        {
            for (var i = fromInclusive; i < toExclusive; i += step)
            {
                yield return i;
            }
        }

        public static IEnumerable<T> CloseLoop<T>(this IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                yield return item;
            }

            if (!list.First().Equals(list.Last()))
                yield return list.First();
        }

        public static IEnumerable<Tuple<T, T, T>> PrevNowNext<T>(this IEnumerable<T> list, T InitialValue = default(T), T InvalidValue = default(T))
        {
            T prev = InvalidValue;
            T now = InvalidValue;
            T next = InitialValue;
            int a = -1;
            foreach (var item in list)
            {
                a++;
                prev = now;
                now = next;
                next = item;
                if(a==0)
                    continue;
                yield return new Tuple<T, T, T>(prev, now, next);
            }

            yield return new Tuple<T, T, T>(now, next, InvalidValue);
        }

        public static IEnumerable<Tuple<T, T>> NowNextBy2<T>(this IEnumerable<T> list)
        {
            T now = default(T);
            T next = default(T);

            int a = -1;
            foreach (var item in list)
            {
                a++;
                now = next;
                next = item;
                if(a % 2 == 0)
                    continue;
                yield return new Tuple<T, T>(now, next);
            }
        }

        public static object GetPropertyOrField(this object obj, string name)
        {
            var type = obj.GetType();
            var pi = type.GetProperty(name);
            if (pi == null)
            {
                var fi1 = type.GetField(name);
                return fi1.GetValue(obj);
            }
            return pi.GetValue(obj);
        }

        static ConcurrentDictionary<Action,long> reentryDictionary = new ConcurrentDictionary<Action, long>();

        public static void ProtectReentry(Action action)
        {
            long m_InFunction = reentryDictionary.ContainsKey(action) ? reentryDictionary[action] : 0;

            if (Interlocked.CompareExchange(ref m_InFunction, 1, 0) == 0)
            {
                // We're not in the function
                try
                {
                    action();
                }
                finally
                {
                    long temp;
                    reentryDictionary.TryRemove(action, out temp);
                }
            }
            else
            {
                // We're already in the function
            }
        }

        public static int toUnixTime(this DateTime dateTime)
        {
            return (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime fromUnixTime(this int time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        public static double toUnixTimeDouble(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime fromUnixTime(this double time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        public static (int degrees, int minutes, float seconds) toDMS(this double angle)
        {
            double degrees = angle;
            double minutes = (degrees - (int) degrees) * 60;
            double seconds = (minutes - (int) minutes) * 60;

            return ((int) degrees, (int)minutes, (float)seconds);
        }

        public static double EvaluateMath(this String input)
        {
            if (input == null || input == "")
                return 0;

            String expr = "(" + input + ")";
            Stack<String> ops = new Stack<String>();
            Stack<Double> vals = new Stack<Double>();

            for (int i = 0; i < expr.Length; i++)
            {
                String s = expr.Substring(i, 1);
                if (s.Equals("(")) { }
                else if (s.Equals("+")) ops.Push(s);
                else if (s.Equals("-")) ops.Push(s);
                else if (s.Equals("*")) ops.Push(s);
                else if (s.Equals("/")) ops.Push(s);
                else if (s.Equals("sqrt")) ops.Push(s);
                else if (s.Equals(")"))
                {
                    int count = ops.Count;
                    while (count > 0)
                    {
                        String op = ops.Pop();
                        double v = vals.Pop();
                        if (op.Equals("+")) v = vals.Pop() + v;
                        else if (op.Equals("-")) v = vals.Pop() - v;
                        else if (op.Equals("*")) v = vals.Pop() * v;
                        else if (op.Equals("/")) v = vals.Pop() / v;
                        else if (op.Equals("sqrt")) v = Math.Sqrt(v);
                        vals.Push(v);

                        count--;
                    }
                }
                else vals.Push(Double.Parse(s));
            }
            return vals.Pop();
        }
    }
}
