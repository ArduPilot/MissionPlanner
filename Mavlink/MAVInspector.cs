using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Mavlink
{
    /// <summary>
    /// Used for packet collection to later display
    /// </summary>
    public class MAVInspector
    {
        Dictionary<uint,Dictionary<uint,MAVLink.MAVLinkMessage>> _history = new Dictionary<uint, Dictionary<uint, MAVLink.MAVLinkMessage>>();

        public void Add(MAVLink.MAVLinkMessage message)
        {
            var id = GetID(message.sysid, message.compid);

            if (!_history.ContainsKey(id))
                Clear(message.sysid, message.compid);

            _history[id][message.msgid] = message;

            Console.WriteLine(message.ToString());
        }

        public IEnumerable<MAVLink.MAVLinkMessage> GetMAVLinkMessages()
        {
            foreach (var messages in _history.Values.ToArray())
            {
                foreach (var msg in messages.Values.ToArray())
                {
                    yield return msg;
                }
            }
        }

        public void Clear()
        {
            _history = new Dictionary<uint, Dictionary<uint, MAVLink.MAVLinkMessage>>();
        }

        public void Clear(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);
            _history[id] = new Dictionary<uint, MAVLink.MAVLinkMessage>();
        }

        public IEnumerable<MAVLink.MAVLinkMessage> this[byte sysid, byte compid]
        {
            get
            {
                var id = GetID(sysid, compid);

                foreach (var msg in _history[id].Values.ToArray())
                {
                    yield return msg;
                }
            }
        }

        uint GetID(byte sysid, byte compid)
        {
            return sysid * 256u + compid;
        }
    }
}
