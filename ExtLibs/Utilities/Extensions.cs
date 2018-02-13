using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public static class Extensions
    {
        public static string TrimUnPrintable(this string input)
        {
            return Regex.Replace(input, @"[^\u0020-\u007E]", String.Empty);
        }

        public static async void Async(this Action function)
        {
            await Task.Run(() => { function(); });
        }

        public static async Task<TOut> Async<TOut>(this Func<TOut> function)
        {
            return await Task.Run(() => { return function(); });
        }

        public static async Task<TOut> Async<TIn, TOut>(this Func<TIn, TOut> function, TIn input)
        {
            return await Task.Run(() => { return function(input); });
        }

        public static void Add<T, T2>(this List<Tuple<T, T2>> input, T in1, T2 in2)
        {
            input.Add(new Tuple<T, T2>(in1, in2));
        }

        public static void Add<T, T2, T3>(this List<Tuple<T, T2, T3>> input, T in1, T2 in2, T3 in3)
        {
            input.Add(new Tuple<T, T2, T3>(in1, in2, in3));
        }

        public static bool IsNumber(this object value)
        {
            if (Equals(value, null))
            {
                return false;
            }

            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
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
    }
}
