using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace MissionPlanner
{
    /// <summary>
    /// Used for packet collection to later display
    /// </summary>
    public class PacketInspector<T>
    {
        Dictionary<uint, Dictionary<uint, T>> _history = new Dictionary<uint, Dictionary<uint, T>>();

        Dictionary<uint, Dictionary<uint, List<irate>>> _rate = new Dictionary<uint, Dictionary<uint, List<irate>>>();

        Dictionary<uint, Dictionary<uint, List<irate>>> _bps = new Dictionary<uint, Dictionary<uint, List<irate>>>();

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

        public double SeenBps(byte sysid, byte compid, uint msgid)
        {
            var id = GetID(sysid, compid);
            var end = DateTime.Now;
            var start = end.AddSeconds(-3);
            var data = toArray(_bps[id][msgid]);
            try
            {
                var starttime = data.First().dateTime;
                starttime = starttime < start ? start : starttime;
                var msgbps = data.Where(a =>
                {
                    return (a.dateTime > start && a.dateTime < end);
                }).Sum(a => a.value / (end - starttime).TotalSeconds);
                return msgbps;
            }
            catch
            {
                return 0;
            }
        }

        public double SeenBps(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);
            var end = DateTime.Now;
            var start = end.AddSeconds(-3);
            var data = _bps[id].SelectMany(a => toArray(a.Value));
            try
            {
                var starttime = data.First().dateTime;
                starttime = starttime < start ? start : starttime;
                var msgbps = data.Where(a =>
                {
                    return (a.dateTime > start && a.dateTime < end);
                }).Sum(a => a.value / (end - starttime).TotalSeconds);
                return msgbps;
            }
            catch
            {
                return 0;
            }
        }

        public void Add(byte sysid, byte compid, uint msgid, T message, int size)
        {
            var id = GetID(sysid, compid);

            lock (_lock)
            {
                // must call clear on any new unseen item to init dictionarys
                if (!_history.ContainsKey(id))
                    Clear(sysid, compid);

                _history[id][msgid] = message;

                if (!_bps[id].ContainsKey(msgid))
                    _bps[id][msgid] = new List<irate>();

                _bps[id][msgid].Add(new irate(DateTime.Now, size));

                if (!_rate[id].ContainsKey(msgid))
                    _rate[id][msgid] = new List<irate>();

                _rate[id][msgid].Add(new irate(DateTime.Now, 1));

                while (_rate[id][msgid].Count > RateHistory)
                    _rate[id][msgid].RemoveAt(0);

                while (_bps[id][msgid].Count > RateHistory)
                    _bps[id][msgid].RemoveAt(0);
            }
        }

        IEnumerable<T> toArray<T>(IEnumerable<T> input)
        {
            lock (_lock)
            {
                return input.ToArray();
            }
        }

        public IEnumerable<T> GetPacketMessages()
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
                _history = new Dictionary<uint, Dictionary<uint, T>>();
                _rate = new Dictionary<uint, Dictionary<uint, List<irate>>>();
                _bps = new Dictionary<uint, Dictionary<uint, List<irate>>>();
            }

            NewSysidCompid?.Invoke(this, null);
        }

        public void Clear(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);
            lock (_lock)
            {
                _history[id] = new Dictionary<uint, T>();
                _rate[id] = new Dictionary<uint, List<irate>>();
                _bps[id] = new Dictionary<uint, List<irate>>();
            }

            NewSysidCompid?.Invoke(this, null);
        }

        public IEnumerable<T> this[byte sysid, byte compid]
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
