using System.Linq;

namespace UAVCAN
{
    public class TransferCRC
    {
        ushort value_ = 0xFFFF;

        public bool check()
        {
            add("123456789".Select(a => (byte)a).ToArray(), 9);

            return get() == 0x29B1;
        }

        public 
            TransferCRC()
        { }

        public void add(byte byte1)
        {
            value_ ^= (ushort)((ushort)byte1 << 8);
            for (byte bit = 0; bit < 8; bit++)
            {
                if ((value_ & 0x8000U) > 0)
                {
                    value_ = (ushort)((ushort)(value_ << 1) ^ 0x1021U);
                }
                else
                {
                    value_ = (ushort)(value_ << 1);
                }
            }
        }

        public void add(byte[] bytes, int len)
        {
            var total = len;
            while (len > 0)
            {
                add(bytes[total - len]);
                len--;
            }
        }

        public static ushort compute(byte[] bytes, int len)
        {
            var temp = new TransferCRC();
            var total = len;
            while (len > 0)
            {
                temp.add(bytes[total - len]);
                len--;
            }

            return temp.get();
        }

        public ushort get() { return value_; }
    }
}