using System;
using System.IO;
using System.Threading;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    [TestFixture]
    public class DtlsTestCase
    {
        private static void CheckDtlsVersion(ProtocolVersion version)
        {
            if (version != null && !version.IsDtls)
                throw new InvalidOperationException("Non-DTLS version");
        }

        [Test, TestCaseSource(typeof(DtlsTestSuite), "Suite")]
        public void RunTest(TlsTestConfig config)
        {
            CheckDtlsVersion(config.clientMinimumVersion);
            CheckDtlsVersion(config.clientOfferVersion);
            CheckDtlsVersion(config.serverMaximumVersion);
            CheckDtlsVersion(config.serverMinimumVersion);

            SecureRandom secureRandom = new SecureRandom();

            DtlsTestClientProtocol clientProtocol = new DtlsTestClientProtocol(secureRandom, config);
            DtlsTestServerProtocol serverProtocol = new DtlsTestServerProtocol(secureRandom, config);

            MockDatagramAssociation network = new MockDatagramAssociation(1500);

            TlsTestClientImpl clientImpl = new TlsTestClientImpl(config);
            TlsTestServerImpl serverImpl = new TlsTestServerImpl(config);

            Server server = new Server(this, serverProtocol, network.Server, serverImpl);

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            Exception caught = null;
            try
            {
                DatagramTransport clientTransport = network.Client;

                if (TlsTestConfig.DEBUG)
                {
                    clientTransport = new LoggingDatagramTransport(clientTransport, Console.Out);
                }

                DtlsTransport dtlsClient = clientProtocol.Connect(clientImpl, clientTransport);

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
            }
            catch (Exception e)
            {
                caught = e;
                LogException(caught);
            }

            server.Shutdown(serverThread);

            // TODO Add checks that the various streams were closed

            Assert.AreEqual(config.expectFatalAlertConnectionEnd, clientImpl.FirstFatalAlertConnectionEnd, "Client fatal alert connection end");
            Assert.AreEqual(config.expectFatalAlertConnectionEnd, serverImpl.FirstFatalAlertConnectionEnd, "Server fatal alert connection end");

            Assert.AreEqual(config.expectFatalAlertDescription, clientImpl.FirstFatalAlertDescription, "Client fatal alert description");
            Assert.AreEqual(config.expectFatalAlertDescription, serverImpl.FirstFatalAlertDescription, "Server fatal alert description");

            if (config.expectFatalAlertConnectionEnd == -1)
            {
                Assert.IsNull(caught, "Unexpected client exception");
                Assert.IsNull(server.mCaught, "Unexpected server exception");
            }
        }

        protected void LogException(Exception e)
        {
            if (TlsTestConfig.DEBUG)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        internal class Server
        {
            private readonly DtlsTestCase mOuter;
            private readonly DtlsTestServerProtocol mServerProtocol;
            private readonly DatagramTransport mServerTransport;
            private readonly TlsTestServerImpl mServerImpl;

            private volatile bool isShutdown = false;
            internal Exception mCaught = null;

            internal Server(DtlsTestCase outer, DtlsTestServerProtocol serverProtocol,
                DatagramTransport serverTransport, TlsTestServerImpl serverImpl)
            {
                this.mOuter = outer;
                this.mServerProtocol = serverProtocol;
                this.mServerTransport = serverTransport;
                this.mServerImpl = serverImpl;
            }

            public void Run()
            {
                try
                {
                    DtlsTransport dtlsServer = mServerProtocol.Accept(mServerImpl, mServerTransport);
                    byte[] buf = new byte[dtlsServer.GetReceiveLimit()];
                    while (!isShutdown)
                    {
                        int length = dtlsServer.Receive(buf, 0, buf.Length, 100);
                        if (length >= 0)
                        {
                            dtlsServer.Send(buf, 0, length);
                        }
                    }
                    dtlsServer.Close();
                }
                catch (Exception e)
                {
                    mCaught = e;
                    mOuter.LogException(mCaught);
                }
            }

            internal void Shutdown(Thread serverThread)
            {
                if (!isShutdown)
                {
                    isShutdown = true;
                    serverThread.Interrupt();
                    serverThread.Join();
                }
            }
        }
    }
}
