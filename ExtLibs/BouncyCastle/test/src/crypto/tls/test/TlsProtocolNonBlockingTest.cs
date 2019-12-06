using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    [TestFixture]
    public class TlsProtocolNonBlockingTest
    {
        [Test]
        public void TestClientServerFragmented()
        {
            // tests if it's really non-blocking when partial records arrive
            DoTestClientServer(true);
        }

        [Test]
        public void TestClientServerNonFragmented()
        {
            DoTestClientServer(false);
        }

        private static void DoTestClientServer(bool fragment)
        {
            SecureRandom secureRandom = new SecureRandom();

            TlsClientProtocol clientProtocol = new TlsClientProtocol(secureRandom);
            TlsServerProtocol serverProtocol = new TlsServerProtocol(secureRandom);

            clientProtocol.Connect(new MockTlsClient(null));
            serverProtocol.Accept(new MockTlsServer());

            // pump handshake
            bool hadDataFromServer = true;
            bool hadDataFromClient = true;
            while (hadDataFromServer || hadDataFromClient)
            {
                hadDataFromServer = PumpData(serverProtocol, clientProtocol, fragment);
                hadDataFromClient = PumpData(clientProtocol, serverProtocol, fragment);
            }

            // send data in both directions
            byte[] data = new byte[1024];
            secureRandom.NextBytes(data);
            WriteAndRead(clientProtocol, serverProtocol, data, fragment);
            WriteAndRead(serverProtocol, clientProtocol, data, fragment);

            // close the connection
            clientProtocol.Close();
            PumpData(clientProtocol, serverProtocol, fragment);
            serverProtocol.CloseInput();
            CheckClosed(serverProtocol);
            CheckClosed(clientProtocol);
        }

        private static void WriteAndRead(TlsProtocol writer, TlsProtocol reader, byte[] data, bool fragment)
        {
            int dataSize = data.Length;
            writer.OfferOutput(data, 0, dataSize);
            PumpData(writer, reader, fragment);

            Assert.AreEqual(dataSize, reader.GetAvailableInputBytes());
            byte[] readData = new byte[dataSize];
            reader.ReadInput(readData, 0, dataSize);
            AssertArrayEquals(data, readData);
        }

        private static bool PumpData(TlsProtocol from, TlsProtocol to, bool fragment)
        {
            int byteCount = from.GetAvailableOutputBytes();
            if (byteCount == 0)
            {
                return false;
            }

            if (fragment)
            {
                while (from.GetAvailableOutputBytes() > 0)
                {
                    byte[] buffer = new byte[1];
                    from.ReadOutput(buffer, 0, 1);
                    to.OfferInput(buffer);
                }
            }
            else
            {
                byte[] buffer = new byte[byteCount];
                from.ReadOutput(buffer, 0, buffer.Length);
                to.OfferInput(buffer);
            }

            return true;
        }

        private static void CheckClosed(TlsProtocol protocol)
        {
            Assert.IsTrue(protocol.IsClosed);

            try
            {
                protocol.OfferInput(new byte[10]);
                Assert.Fail("Input was accepted after close");
            }
            catch (IOException)
            {
            }

            try
            {
                protocol.OfferOutput(new byte[10], 0, 10);
                Assert.Fail("Output was accepted after close");
            }
            catch (IOException)
            {
            }
        }

        private static void AssertArrayEquals(byte[] a, byte[] b)
        {
            Assert.IsTrue(Arrays.AreEqual(a, b));
        }
    }
}
