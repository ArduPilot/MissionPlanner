using System;
using System.IO;
using System.Threading;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    [TestFixture]
    public class TlsPskProtocolTest
    {
        [Test]
        public void TestClientServer()
        {
            SecureRandom secureRandom = new SecureRandom();

            PipedStream clientPipe = new PipedStream();
            PipedStream serverPipe = new PipedStream(clientPipe);

            TlsClientProtocol clientProtocol = new TlsClientProtocol(clientPipe, secureRandom);
            TlsServerProtocol serverProtocol = new TlsServerProtocol(serverPipe, secureRandom);

            Server server = new Server(serverProtocol);

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            MockPskTlsClient client = new MockPskTlsClient(null);
            clientProtocol.Connect(client);

            // NOTE: Because we write-all before we read-any, this length can't be more than the pipe capacity
            int length = 1000;

            byte[] data = new byte[length];
            secureRandom.NextBytes(data);

            Stream output = clientProtocol.Stream;
            output.Write(data, 0, data.Length);

            byte[] echo = new byte[data.Length];
            int count = Streams.ReadFully(clientProtocol.Stream, echo);

            Assert.AreEqual(count, data.Length);
            Assert.IsTrue(Arrays.AreEqual(data, echo));

            output.Close();

            serverThread.Join();
        }

        internal class Server
        {
            private readonly TlsServerProtocol mServerProtocol;

            internal Server(TlsServerProtocol serverProtocol)
            {
                this.mServerProtocol = serverProtocol;
            }

            public void Run()
            {
                try
                {
                    MockPskTlsServer server = new MockPskTlsServer();
                    mServerProtocol.Accept(server);
                    Streams.PipeAll(mServerProtocol.Stream, mServerProtocol.Stream);
                    mServerProtocol.Close();
                }
                catch (Exception)
                {
                    //throw new RuntimeException(e);
                }
            }
        }
    }
}
