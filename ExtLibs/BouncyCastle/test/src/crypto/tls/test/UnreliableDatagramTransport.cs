using System;
using System.IO;

using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class UnreliableDatagramTransport
        :   DatagramTransport
    {
        private readonly DatagramTransport transport;
        private readonly Random random;
        private readonly int percentPacketLossReceiving, percentPacketLossSending;

        public UnreliableDatagramTransport(DatagramTransport transport, Random random,
            int percentPacketLossReceiving, int percentPacketLossSending)
        {
            if (percentPacketLossReceiving < 0 || percentPacketLossReceiving > 100)
                throw new ArgumentException("out of range", "percentPacketLossReceiving");
            if (percentPacketLossSending < 0 || percentPacketLossSending > 100)
                throw new ArgumentException("out of range", "percentPacketLossSending");

            this.transport = transport;
            this.random = random;
            this.percentPacketLossReceiving = percentPacketLossReceiving;
            this.percentPacketLossSending = percentPacketLossSending;
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
            long endMillis = DateTimeUtilities.CurrentUnixMs() + waitMillis;
            for (;;)
            {
                int length = transport.Receive(buf, off, len, waitMillis);
                if (length < 0 || !LostPacket(percentPacketLossReceiving))
                {
                    return length;
                }

                Console.WriteLine("PACKET LOSS (" + length + " byte packet not received)");

                long now = DateTimeUtilities.CurrentUnixMs();
                if (now >= endMillis)
                {
                    return -1;
                }

                waitMillis = (int)(endMillis - now);
            }
        }

        public virtual void Send(byte[] buf, int off, int len)
        {
            if (LostPacket(percentPacketLossSending))
            {
                Console.WriteLine("PACKET LOSS (" + len + " byte packet not sent)");
            }
            else
            {
                transport.Send(buf, off, len);
            }
        }

        public virtual void Close()
        {
            transport.Close();
        }

        private bool LostPacket(int percentPacketLoss)
        {
            return percentPacketLoss > 0 && random.Next(100) < percentPacketLoss;
        }
    }
}
