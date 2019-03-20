using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    /**
     * A simple test designed to conduct a TLS handshake with an external TLS client.
     * <p/>
     * Please refer to GnuTLSSetup.html or OpenSSLSetup.html (under 'docs'), and x509-*.pem files in
     * this package (under 'src/test/resources') for help configuring an external TLS client.
     */
    public class TlsServerTest
    {
        private static readonly SecureRandom secureRandom = new SecureRandom();

        public static void Main(string[] args)
        {
            int port = 5556;

            TcpListener ss = new TcpListener(IPAddress.Any, port);
            ss.Start();
            Stream stdout = Console.OpenStandardOutput();
            try
            {
                while (true)
                {
                    TcpClient s = ss.AcceptTcpClient();
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine("Accepted " + s);
                    ServerThread st = new ServerThread(s, stdout);
                    Thread t = new Thread(new ThreadStart(st.Run));
                    t.Start();
                }
            }
            finally
            {
                ss.Stop();
            }
        }

        internal class ServerThread
        {
            private readonly TcpClient s;
            private readonly Stream stdout;

            internal ServerThread(TcpClient s, Stream stdout)
            {
                this.s = s;
                this.stdout = stdout;
            }

            public void Run()
            {
                try
                {
                    MockTlsServer server = new MockTlsServer();
                    TlsServerProtocol serverProtocol = new TlsServerProtocol(s.GetStream(), secureRandom);
                    serverProtocol.Accept(server);
                    Stream log = new TeeOutputStream(serverProtocol.Stream, stdout);
                    Streams.PipeAll(serverProtocol.Stream, log);
                    serverProtocol.Close();
                }
                finally
                {
                    try
                    {
                        s.Close();
                    }
                    catch (IOException)
                    {
                    }
                    finally
                    {
                    }
                }
            }
        }
    }
}
