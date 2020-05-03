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
    public class TlsTestCase
    {
        private static void CheckTlsVersion(ProtocolVersion version)
        {
            if (version != null && !version.IsTls)
                throw new InvalidOperationException("Non-TLS version");
        }

        [Test, TestCaseSource(typeof(TlsTestSuite), "Suite")]
        public void RunTest(TlsTestConfig config)
        {
            CheckTlsVersion(config.clientMinimumVersion);
            CheckTlsVersion(config.clientOfferVersion);
            CheckTlsVersion(config.serverMaximumVersion);
            CheckTlsVersion(config.serverMinimumVersion);

            SecureRandom secureRandom = new SecureRandom();

            PipedStream clientPipe = new PipedStream();
            PipedStream serverPipe = new PipedStream(clientPipe);

            NetworkStream clientNet = new NetworkStream(clientPipe);
            NetworkStream serverNet = new NetworkStream(serverPipe);

            TlsTestClientProtocol clientProtocol = new TlsTestClientProtocol(clientNet, secureRandom, config);
            TlsTestServerProtocol serverProtocol = new TlsTestServerProtocol(serverNet, secureRandom, config);

            TlsTestClientImpl clientImpl = new TlsTestClientImpl(config);
            TlsTestServerImpl serverImpl = new TlsTestServerImpl(config);

            Server server = new Server(this, serverProtocol, serverImpl);

            Thread serverThread = new Thread(new ThreadStart(server.Run));
            serverThread.Start();

            Exception caught = null;
            try
            {
                clientProtocol.Connect(clientImpl);

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
            }
            catch (Exception e)
            {
                caught = e;
                LogException(caught);
            }

            server.AllowExit();
            serverThread.Join();

            Assert.IsTrue(clientNet.IsClosed, "Client Stream not closed");
            Assert.IsTrue(serverNet.IsClosed, "Server Stream not closed");

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

        protected virtual void LogException(Exception e)
        {
            if (TlsTestConfig.DEBUG)
            {
                Console.Error.WriteLine(e);
            }
        }

        internal class Server
        {
            protected readonly TlsTestCase mOuter;
            protected readonly TlsTestServerProtocol mServerProtocol;
            protected readonly TlsTestServerImpl mServerImpl;

            internal bool mCanExit = false;
            internal Exception mCaught = null;

            internal Server(TlsTestCase outer, TlsTestServerProtocol serverProtocol, TlsTestServerImpl serverImpl)
            {
                this.mOuter = outer;
                this.mServerProtocol = serverProtocol;
                this.mServerImpl = serverImpl;
            }

            internal void AllowExit()
            {
                lock (this)
                {
                    mCanExit = true;
                    Monitor.PulseAll(this);
                }
            }

            public void Run()
            {
                try
                {
                    mServerProtocol.Accept(mServerImpl);
                    Streams.PipeAll(mServerProtocol.Stream, mServerProtocol.Stream);
                    mServerProtocol.Close();
                }
                catch (Exception e)
                {
                    mCaught = e;
                    mOuter.LogException(mCaught);
                }

                WaitExit();
            }

            protected void WaitExit()
            {
                lock (this)
                {
                    while (!mCanExit)
                    {
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException)
                        {
                        }
                    }
                }
            }
        }
    }
}
