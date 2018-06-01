using System;

#region Rfc info
/*
2.2 AAAA data format

   A 128 bit IPv6 address is encoded in the data portion of an AAAA
   resource record in network byte order (high-order byte first).
 */
#endregion

namespace Heijden.DNS
{
    class RecordAAAA : Record
	{
        public string Address;

		public RecordAAAA(RecordReader rr)
		{
            Address = string.Format("{0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}:{6:x}:{7:x}",
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16(),
                rr.ReadUInt16());
		}

		public override string ToString()
		{
			return Address.ToString();
		}

	}
}
