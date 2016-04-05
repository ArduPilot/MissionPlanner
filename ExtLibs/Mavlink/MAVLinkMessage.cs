using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkMessage
    {
        public byte header;
        public byte length;

        public byte incompat_flags;
        public byte compat_flags;

        public byte seq;
        public byte sysid;
        public byte compid;

        public byte dialect;

        public ushort messid;
        public object data;

        public ushort crc16;

        public MAVLinkMessage(byte header1, byte length1, byte seq1, byte sysid1, byte compid1, byte messid1, object data)
        {
            this.header = header1;
            this.length = length1;
            this.seq = seq1;
            this.sysid = sysid1;
            this.compid = compid1;
            this.messid = messid1;
            this.data = data;
        }
    }
}
