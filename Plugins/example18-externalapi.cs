using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle.Utilities.Encoders;

//loadassembly: BouncyCastle

namespace MissionPlanner.plugins
{
    public class example18_externalapi : Plugin.Plugin
    {
        public override string Name { get; } = "External API";

        public override string Version { get; }

        public override string Author { get; }

        public override bool Exit()
        {
            return true;
        }

        public string address = Settings.Instance.GetString("ex_api_address", "droneshare.cubepilot.org");
        public int port = Settings.Instance.GetInt32("ex_api_port", 8042);

        public override bool Init()
        {
            var rootbut = new ToolStripMenuItem("Share to API");
            rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            return true;
        }

        private void but_Click(object sender2, EventArgs e)
        {
            if (InputBox.Show("Server", "Server", ref address) != DialogResult.OK)
                return;
            if (InputBox.Show("Server port", "Server port", ref port) != DialogResult.OK)
                return;

            string username = Settings.Instance.GetString("ex_api_username");
            string token = Settings.Instance.GetString("ex_api_psk");

            if (InputBox.Show("Username", "Username", ref username) != DialogResult.OK)
                return;
            if (InputBox.Show("Token", "Token", ref token) != DialogResult.OK)
                return;

            Settings.Instance["ex_api_address"] = address;
            Settings.Instance["ex_api_port"] = port.ToString();

            Settings.Instance["ex_api_username"] = username;
            Settings.Instance["ex_api_psk"] = token;

            var psk = new BasicTlsPskIdentity(username, token.MakeBytes());
            var pskclient = new DTLSPsk(psk);

            Task.Run(() =>
            {
                try
                {


                    DtlsClientProtocol client = new DtlsClientProtocol();
                    DatagramTransport transport = new UDPTransport(address, port);
                    var dtlstx = client.Connect(pskclient, transport);

                    MainV2.comPort.OnPacketReceived += (sender, message) =>
                    {
                        try
                        {
                            dtlstx.Send(message.buffer, 0, message.buffer.Length);
                        }
                        catch (Exception ex)
                        {
                        }
                    };

                    var buf = new byte[dtlstx.GetReceiveLimit()];

                    while (MainV2.comPort.BaseStream.IsOpen)
                    {
                        try
                        {
                            var read = dtlstx.Receive(buf, 0, buf.Length, 1000);
                            lock (MainV2.comPort.writelock)
                            {
                                if (MainV2.comPort.BaseStream.IsOpen)
                                    MainV2.comPort.BaseStream.Write(buf, 0, read);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, ex.ToString());
                }
            });
        }

        public override bool Loaded()
        {
            return true;
        }
    }

    internal class DTLSPsk : PskTlsClient
    {
        public DTLSPsk(TlsPskIdentity pskIdentity) : base(new BcTlsCrypto(new Org.BouncyCastle.Security.SecureRandom()),
            pskIdentity)
        {
        }

        public override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256
            };
        }

        public override void NotifyAlertRaised(short alertLevel, short alertDescription, string message,
            Exception cause)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("DTLS client raised alert: " + AlertLevel.GetText(alertLevel)
                                                          + ", " + AlertDescription.GetText(alertDescription));
            if (message != null)
            {
                output.WriteLine("> " + message);
            }

            if (cause != null)
            {
                output.WriteLine(cause);
            }
        }

        public override void NotifyAlertReceived(short alertLevel, short alertDescription)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("DTLS client received alert: " + AlertLevel.GetText(alertLevel)
                                                            + ", " + AlertDescription.GetText(alertDescription));
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
            base.NotifyServerVersion(serverVersion);

            Console.WriteLine("DTLS client negotiated " + serverVersion);
        }

        public override TlsAuthentication GetAuthentication()
        {
            return base.GetAuthentication();
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            // this is psk, not needed
            //base.NotifySecureRenegotiation(secureRenegotiation);
        }

        public override void NotifyHandshakeComplete()
        {
            base.NotifyHandshakeComplete();

            ProtocolName protocolName = m_context.SecurityParameters.ApplicationProtocol;
            if (protocolName != null)
            {
                Console.WriteLine("Client ALPN: " + protocolName.GetUtf8Decoding());
            }

            TlsSession newSession = m_context.Session;
            if (newSession != null)
            {
                if (newSession.IsResumable)
                {
                    byte[] newSessionID = newSession.SessionID;
                    string hex = ToHexString(newSessionID);
                    /*
                    if (base.m_session != null && Arrays.AreEqual(base.m_session.SessionID, newSessionID))
                    {
                        Console.WriteLine("Client resumed session: " + hex);
                    }
                    else
                    {
                        Console.WriteLine("Client established session: " + hex);
                    }

                    this.m_session = newSession;
                    */

                    Console.WriteLine("Client established session: " + hex);
                }

                byte[] tlsServerEndPoint = m_context.ExportChannelBinding(ChannelBinding.tls_server_end_point);
                if (null != tlsServerEndPoint)
                {
                    Console.WriteLine("Client 'tls-server-end-point': " + ToHexString(tlsServerEndPoint));
                }

                byte[] tlsUnique = m_context.ExportChannelBinding(ChannelBinding.tls_unique);
                Console.WriteLine("Client 'tls-unique': " + ToHexString(tlsUnique));
            }
        }

        public override IDictionary<int, byte[]> GetClientExtensions()
        {
            if (m_context.SecurityParameters.ClientRandom == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            return base.GetClientExtensions();
        }

        public override void ProcessServerExtensions(IDictionary<int, byte[]> serverExtensions)
        {
            if (m_context.SecurityParameters.ServerRandom == null)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            base.ProcessServerExtensions(serverExtensions);
        }

        protected virtual string ToHexString(byte[] data)
        {
            return data == null ? "(null)" : Hex.ToHexString(data);
        }

        protected override ProtocolVersion[] GetSupportedVersions()
        {
            return ProtocolVersion.DTLSv12.Only();
        }
    }

    public class UDPTransport : DatagramTransport
    {
        private string address;
        private int port;
        private UdpClient _udpclient;

        private readonly ByteQueue mRecordQueue = new ByteQueue();

        public UDPTransport(string address, int port)
        {
            this.address = address;
            this.port = port;

            _udpclient = new UdpClient(address, port);
        }

        public void Close()
        {
            _udpclient.Close();
        }

        public int GetReceiveLimit()
        {
            return 1 << 14;
        }

        public int GetSendLimit()
        {
            return 1 << 14;
        }

        public int Receive(byte[] buf, int off, int len, int waitMillis)
        {
            var endtime = DateTime.Now.AddMilliseconds(waitMillis);
            while (mRecordQueue.Available < len && endtime > DateTime.Now)
            {
                if (_udpclient.Available > 0)
                {
                    IPEndPoint ep = null;
                    var data = _udpclient.Receive(ref ep);

                    mRecordQueue.AddData(data, 0, data.Length);

                    break;
                }

                Thread.Yield();
            }

            len = Math.Min(mRecordQueue.Available, len);
            mRecordQueue.Read(buf, off, len, 0);
            mRecordQueue.RemoveData(len);
            return len;
        }

        public void Send(byte[] buf, int off, int len)
        {
            var span = new ReadOnlySpan<byte>(buf, off, len);
            _udpclient.Send(span.ToArray(), span.Length);
        }
    }
}