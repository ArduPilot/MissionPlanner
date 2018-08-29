using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class MockDatagramAssociation
    {
        private int mtu;
        private MockDatagramTransport client, server;

        public MockDatagramAssociation(int mtu)
        {
            this.mtu = mtu;

            IList clientQueue = new ArrayList();
            IList serverQueue = new ArrayList();

            this.client = new MockDatagramTransport(this, clientQueue, serverQueue);
            this.server = new MockDatagramTransport(this, serverQueue, clientQueue);
        }

        public virtual DatagramTransport Client
        {
            get { return client; }
        }

        public virtual DatagramTransport Server
        {
            get { return server; }
        }

        private class MockDatagramTransport
            :   DatagramTransport
        {
            private readonly MockDatagramAssociation mOuter;

            private IList receiveQueue, sendQueue;

            internal MockDatagramTransport(MockDatagramAssociation outer, IList receiveQueue, IList sendQueue)
            {
                this.mOuter = outer;
                this.receiveQueue = receiveQueue;
                this.sendQueue = sendQueue;
            }

            public virtual int GetReceiveLimit()
            {
                return mOuter.mtu;
            }

            public virtual int GetSendLimit()
            {
                return mOuter.mtu;
            }

            public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
            {
                lock (receiveQueue)
                {
                    if (receiveQueue.Count < 1)
                    {
                        try
                        {
                            Monitor.Wait(receiveQueue, waitMillis);
                        }
                        catch (ThreadInterruptedException)
                        {
                            // TODO Keep waiting until full wait expired?
                        }
                        if (receiveQueue.Count < 1)
                        {
                            return -1;
                        }
                    }
                    byte[] packet = (byte[])receiveQueue[0];
                    receiveQueue.RemoveAt(0);
                    int copyLength = System.Math.Min(len, packet.Length);
                    Array.Copy(packet, 0, buf, off, copyLength);
                    return copyLength;
                }
            }

            public virtual void Send(byte[] buf, int off, int len)
            {
                if (len > mOuter.mtu)
                {
                    // TODO Simulate rejection?
                }

                byte[] packet = Arrays.CopyOfRange(buf, off, off + len);

                lock (sendQueue)
                {
                    sendQueue.Add(packet);
                    Monitor.PulseAll(sendQueue);
                }
            }

            public virtual void Close()
            {
                // TODO?
            }
        }
    }
}
