﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using IronPython.Compiler;
using MissionPlanner.Controls;

namespace MissionPlanner.Mavlink
{
    public class MAVList : IEnumerable<MAVState>
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
            int id = GetID((byte)sysid, (byte)compid);

            hiddenlist[id] = new MAVState() { sysid = sysid, compid = compid };
        }

        public MAVState this[int sysid, int compid]
        {
            get
            {
                int id = GetID((byte)sysid, (byte)compid);

                // 3dr radio special case
                if (hiddenlist.ContainsKey(id))
                    return hiddenlist[id];

                if (!masterlist.ContainsKey(id))
                    return new MAVState();

                return masterlist[id];
            }
            set
            {
                int id = GetID((byte)sysid, (byte)compid);

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

        public void Clear()
        {
            masterlist.Clear();
        }

        public bool Contains(byte sysid, byte compid, bool includehidden = true)
        {
            foreach (var item in masterlist.ToArray())
            {
                if (item.Value.sysid == sysid && item.Value.compid == compid)
                    return true;
            }

            if (includehidden)
            {
                foreach (var item in hiddenlist.ToArray())
                {
                    if (item.Value.sysid == sysid && item.Value.compid == compid)
                        return true;
                }
            }

            return false;
        }

        internal void Create(byte sysid, byte compid)
        {
            int id = GetID((byte)sysid, (byte)compid);

            // move from hidden to visible
            if (hiddenlist.ContainsKey(id))
            {
                masterlist[id] = hiddenlist[id];
                hiddenlist.Remove(id);
            }

            if (!masterlist.ContainsKey(id))
                masterlist[id] = new MAVState();
        }

        public IEnumerator<MAVState> GetEnumerator()
        {
            foreach (var key in masterlist.Values.ToArray())
            {
                yield return key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public static int GetID(byte sysid, byte compid)
        {
           return  sysid*256 + compid;
        }
    }
}