using System;
using System.IO;
using System.Text;

using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class LoggingDatagramTransport
        :   DatagramTransport
    {
        private static readonly string HEX_CHARS = "0123456789ABCDEF";

        private readonly DatagramTransport transport;
        private readonly TextWriter output;
        private readonly long launchTimestamp;

        public LoggingDatagramTransport(DatagramTransport transport, TextWriter output)
        {
            this.transport = transport;
            this.output = output;
            this.launchTimestamp = DateTimeUtilities.CurrentUnixMs();
        }

        public virtual int GetReceiveLimit()
        {
            return transport.GetReceiveLimit();
        }

        public virtual int GetSendLimit()
        {
            return transport.GetSendLimit();
        }

        public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            int length = transport.Receive(buf, off, len, waitMillis);
            if (length >= 0)
            {
                DumpDatagram("Received", buf, off, length);
            }
            return length;
        }

        public virtual void Send(byte[] buf, int off, int len)
        {
            DumpDatagram("Sending", buf, off, len);
            transport.Send(buf, off, len);
        }

        public virtual void Close()
        {
        }

        private void DumpDatagram(string verb, byte[] buf, int off, int len)
        {
            long timestamp = DateTimeUtilities.CurrentUnixMs() - launchTimestamp;
            StringBuilder sb = new StringBuilder("(+" + timestamp + "ms) " + verb + " " + len + " byte datagram:");
            for (int pos = 0; pos < len; ++pos)
            {
                if (pos % 16 == 0)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append("    ");
                }
                else if (pos % 16 == 8)
                {
                    sb.Append('-');
                }
                else
                {
                    sb.Append(' ');
                }
                int val = buf[off + pos] & 0xFF;
                sb.Append(HEX_CHARS[val >> 4]);
                sb.Append(HEX_CHARS[val & 0xF]);
            }
            Dump(sb.ToString());
        }

        private void Dump(string s)
        {
            lock (this) output.WriteLine(s);
        }
    }
}
