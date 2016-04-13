using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using IronPython.Compiler;
using MissionPlanner.Controls;

namespace MissionPlanner.Mavlink
{
    public class MAVList
    {
        private Dictionary<int, MAVState> masterlist = new Dictionary<int, MAVState>();

        private Dictionary<int, MAVState> hiddenlist = new Dictionary<int, MAVState>();

        public MAVList()
        {
            // add blank item
            hiddenlist.Add(0,new MAVState());
        }

        public void AddHiddenList(byte sysid, byte compid)
        {
            int id = (byte)sysid * 256 + (byte)compid;

            hiddenlist[id] = new MAVState() { sysid = sysid, compid = compid };
        }

        public MAVState this[int sysid, int compid]
        {
            get
            {
                int id = (byte) sysid*256 + (byte) compid;

                // 3dr radio special case
                if (hiddenlist.ContainsKey(id))
                    return hiddenlist[id];

                if (!masterlist.ContainsKey(id))
                    return new MAVState();

                return masterlist[id];
            }
            set
            {
                int id = (byte) sysid*256 + (byte) compid;

                masterlist[id] = value;
            }
        }

        public int Count
        {
            get { return masterlist.Count; }
        }

        public List<int> GetRawIDS()
        {
            return masterlist.Keys.ToList<int>();
        }

        public MAVState[] GetMAVStates()
        {
            return masterlist.Values.ToArray<MAVState>();
        }

        public void Clear()
        {
            masterlist.Clear();
        }

        public bool Contains(byte sysid, byte compid, bool includehidden = true)
        {
            foreach (var item in masterlist)
            {
                if (item.Value.sysid == sysid && item.Value.compid == compid)
                    return true;
            }

            if (includehidden)
            {
                foreach (var item in hiddenlist)
                {
                    if (item.Value.sysid == sysid && item.Value.compid == compid)
                        return true;
                }
            }

            return false;
        }

        internal void Create(byte sysid, byte compid)
        {
            int id = (byte) sysid*256 + (byte) compid;

            // move from hidden to visible
            if (hiddenlist.ContainsKey(id))
            {
                masterlist[id] = hiddenlist[id];
                hiddenlist.Remove(id);
            }

            if (!masterlist.ContainsKey(id))
                masterlist[id] = new MAVState();
        }
    }
}