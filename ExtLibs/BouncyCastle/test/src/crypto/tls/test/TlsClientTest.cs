using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    /**
     * A simple test designed to conduct a TLS handshake with an external TLS server.
     * <p/>
     * Please refer to GnuTLSSetup.html or OpenSSLSetup.html (under 'docs'), and x509-*.pem files in
     * this package (under 'src/test/resources') for help configuring an external TLS server.
     */
    public class TlsClientTest
    {
        private static readonly SecureRandom secureRandom = new SecureRandom();

        public static void Main(string[] args)
        {
            string hostname = "localhost";
            int port = 5556;

            long time1 = DateTime.UtcNow.Ticks;

            MockTlsClient client = new MockTlsClient(null);
            TlsClientProtocol protocol = OpenTlsConnection(hostname, port, client);
            protocol.Close();

            long time2 = DateTime.UtcNow.Ticks;
            Console.WriteLine("Elapsed 1: " + (time2 - time1)/TimeSpan.TicksPerMillisecond + "ms");

            client = new MockTlsClient(client.GetSessionToResume());
            protocol = OpenTlsConnection(hostname, port, client);

            long time3 = DateTime.UtcNow.Ticks;
            Console.WriteLine("Elapsed 2: " + (time3 - time2)/TimeSpan.TicksPerMillisecond + "ms");

            byte[] req = Encoding.UTF8.GetBytes("GET / HTTP/1.1\r\n\r\n");

            Stream tlsStream = protocol.Stream;
            tlsStream.Write(req, 0, req.Length);
            tlsStream.Flush();

            StreamReader reader = new StreamReader(tlsStream);

            String line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(">>> " + line);
            }

            protocol.Close();
        }

        internal static TlsClientProtocol OpenTlsConnection(string hostname, int port, TlsClient client)
        {
            TcpClient tcp = new TcpClient(hostname, port);

            TlsClientProtocol protocol = new TlsClientProtocol(tcp.GetStream(), secureRandom);
            protocol.Connect(client);
            return protocol;
        }
    }
}
