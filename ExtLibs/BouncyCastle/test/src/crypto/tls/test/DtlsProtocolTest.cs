using System;
using System.IO;
using System.Threading;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    [TestFixture]
    public class DtlsProtocolTest
    {
        [Test]
        public void TestClientServer()
        {
            SecureRandom secureRandom = new SecureRandom();

            DtlsClientProtocol clientProtocol = new DtlsClientProtocol(secureRandom);
            DtlsServerProtocol serverProtocol = new DtlsServerProtocol(secureRandom);

            MockDatagramAssociation network = new MockDatagramAssociation(1500);

            Server server = new Server(serverProtocol, network.Server);

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            DatagramTransport clientTransport = network.Client;

            clientTransport = new UnreliableDatagramTransport(clientTransport, secureRandom, 0, 0);

            clientTransport = new LoggingDatagramTransport(clientTransport, Console.Out);

            MockDtlsClient client = new MockDtlsClient(null);

            DtlsTransport dtlsClient = clientProtocol.Connect(client, clientTransport);

            for (int i = 1; i <= 10; ++i)
            {
                byte[] data = new byte[i];
                Arrays.Fill(data, (byte)i);
                dtlsClient.Send(data, 0, data.Length);
            }

            byte[] buf = new byte[dtlsClient.GetReceiveLimit()];
            while (dtlsClient.Receive(buf, 0, buf.Length, 100) >= 0)
            {
            }

            dtlsClient.Close();

            server.Shutdown(serverThread);
        }

        internal class Server
        {
            private readonly DtlsServerProtocol mServerProtocol;
            private readonly DatagramTransport mServerTransport;
            private volatile bool isShutdown = false;

            internal Server(DtlsServerProtocol serverProtocol, DatagramTransport serverTransport)
            {
                this.mServerProtocol = serverProtocol;
                this.mServerTransport = serverTransport;
            }

            public void Run()
            {
                try
                {
                    MockDtlsServer server = new MockDtlsServer();
                    DtlsTransport dtlsServer = mServerProtocol.Accept(server, mServerTransport);
                    byte[] buf = new byte[dtlsServer.GetReceiveLimit()];
                    while (!isShutdown)
                    {
                        int length = dtlsServer.Receive(buf, 0, buf.Length, 1000);
                        if (length >= 0)
                        {
                            dtlsServer.Send(buf, 0, length);
                        }
                    }
                    dtlsServer.Close();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                }
            }

            internal void Shutdown(Thread serverThread)
            {
                if (!isShutdown)
                {
                    isShutdown = true;
                    serverThread.Join();
                }
            }
        }
    }
}
