using System;
/*
 3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */

namespace Heijden.DNS
{
    class RecordA : Record
	{
        public string Address;

		public RecordA(RecordReader rr)
		{
            Address = string.Format("{0}.{1}.{2}.{3}",
                rr.ReadByte(),
                rr.ReadByte(),
                rr.ReadByte(),
                rr.ReadByte());


		}

		public override string ToString()
		{
			return Address;
		}

	}
}
