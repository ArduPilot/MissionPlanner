﻿using System;
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

        public MAVState this[int sysid, int compid]
        {
            get
            {
                int id = (byte)sysid*256 + (byte)compid;

                if (!masterlist.ContainsKey(id))
                    return new MAVState();

                return masterlist[id];
            }
            set
            {
                int id = (byte)sysid * 256 + (byte)compid;

                masterlist[id] = value;
            }
        }

        public int Count { get { return masterlist.Count; } }

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

        public bool Contains(byte sysid, byte compid)
        {
            foreach (var item in masterlist)
            {
                if (item.Value.sysid == sysid && item.Value.compid == compid)
                    return true;
            }

            return false;
        }

        internal void Create(byte sysid, byte compid)
        {
            int id = (byte)sysid * 256 + (byte)compid;

            if (!masterlist.ContainsKey(id))
                masterlist[id] = new MAVState();
        }
    }
}
