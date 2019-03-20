using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.Mavlink
{
    /// <summary>
    /// Used for packet collection to later display
    /// </summary>
    public class MAVInspector
    {
        Dictionary<uint,Dictionary<uint,MAVLink.MAVLinkMessage>> _history = new Dictionary<uint, Dictionary<uint, MAVLink.MAVLinkMessage>>();

        Dictionary<uint, Dictionary<uint, List<irate>>> _rate = new Dictionary<uint, Dictionary<uint, List<irate>>>();

        public int RateHistory { get; set; } = 200;

        object _lock = new object();

        public event EventHandler NewSysidCompid;

        struct irate
        {
            internal DateTime dateTime;
            internal int value;

            internal irate(DateTime now, int v) : this()
            {
                dateTime = now;
                value = v;
            }
        }

        public List<byte> SeenSysid()
        {
            List<byte> sysids = new List<byte>();
            foreach (var id in toArray(_history.Keys))
            {
                sysids.Add(GetFromID(id).sysid);
            }

            return sysids;
        }

        public List<byte> SeenCompid()
        {
            List<byte> compids = new List<byte>();
            foreach (var id in toArray(_history.Keys))
            {
                compids.Add(GetFromID(id).compid);
            }

            return compids;
        }

        public double SeenRate(byte sysid, byte compid, uint msgid)
        {
            var id = GetID(sysid, compid);
            var end = DateTime.Now;
            var start = end.AddSeconds(-3);
            var data = toArray(_rate[id][msgid]);
            try
            {
                var starttime = data.First().dateTime;
                starttime = starttime < start ? start : starttime;
                var msgrate = data.Where(a =>
                {
                    return (a.dateTime > start && a.dateTime < end);
                }).Sum(a => a.value / (end - starttime).TotalSeconds);
                return msgrate;
            }
            catch
            {
                return 0;
            }
        }

        public void Add(MAVLink.MAVLinkMessage message)
        {
            var id = GetID(message.sysid, message.compid);

            lock (_lock)
            {
                if (!_history.ContainsKey(id))
                    Clear(message.sysid, message.compid);

                _history[id][message.msgid] = message;


                if (!_rate[id].ContainsKey(message.msgid))
                    _rate[id][message.msgid] = new List<irate>();

                _rate[id][message.msgid].Add(new irate(DateTime.Now, 1));

                while (_rate[id][message.msgid].Count > RateHistory)
                    _rate[id][message.msgid].RemoveAt(0);
            }
        }

        IEnumerable<T> toArray<T>(IEnumerable<T> input)
        {
            lock (_lock)
            {
                return input.ToArray();
            }
        }

        public IEnumerable<MAVLink.MAVLinkMessage> GetMAVLinkMessages()
        {
            foreach (var messages in toArray(_history.Values))
            {
                foreach (var msg in toArray(messages.Values))
                {
                    yield return msg;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _history = new Dictionary<uint, Dictionary<uint, MAVLink.MAVLinkMessage>>();
                _rate = new Dictionary<uint, Dictionary<uint, List<irate>>>();
            }

            NewSysidCompid?.Invoke(this, null);
        }

        public void Clear(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);
            lock (_lock)
            {
                _history[id] = new Dictionary<uint, MAVLink.MAVLinkMessage>();
                _rate[id] = new Dictionary<uint, List<irate>>();
            }

            NewSysidCompid?.Invoke(this, null);
        }

        public IEnumerable<MAVLink.MAVLinkMessage> this[byte sysid, byte compid]
        {
            get
            {
                var id = GetID(sysid, compid);

                foreach (var msg in toArray(_history[id].Values))
                {
                    yield return msg;
                }
            }
        }

        uint GetID(byte sysid, byte compid)
        {
            return sysid * 256u + compid;
        }

        (byte sysid, byte compid) GetFromID(uint id)
        {
            return ((byte)(id >> 8), (byte)(id & 0xff));
        }
    }
}
