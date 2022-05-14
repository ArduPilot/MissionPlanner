using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public string address = Settings.Instance.GetString("ex_api_address");
        public int port = Settings.Instance.GetInt32("ex_api_port");

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

            Task.Run(() =>
            {
                try
                {
                    var psk = new BasicTlsPskIdentity(username, token.MakeBytes());
                    var pskclient = new DTLSPsk(psk);

                    DtlsClientProtocol client = new DtlsClientProtocol(new Org.BouncyCastle.Security.SecureRandom());
                    DatagramTransport transport = new UDPTransport(address, port);
                    var dtlstx = client.Connect(pskclient, transport);

                    MainV2.comPort.OnPacketReceived += (sender, message) =>
                    {
                        dtlstx.Send(message.buffer, 0, message.buffer.Length);
                    };

                    var buf = new byte[dtlstx.GetReceiveLimit()];

                    while (MainV2.comPort.BaseStream.IsOpen)
                    {
                        try
                        {
                            var read = dtlstx.Receive(buf, 0, buf.Length, 1000);
                            lock (MainV2.comPort.objlock)
                            {
                                if (MainV2.comPort.BaseStream.IsOpen)
                                    MainV2.comPort.BaseStream.Write(buf, 0, read);
                            }
                        }
                        catch (Exception ex) { }
                    }
                } catch (Exception ex) {
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
        public DTLSPsk(TlsPskIdentity pskIdentity) : base(pskIdentity)
        {
        }

        public override int[] GetCipherSuites()
        {
            return new int[]
            {
                CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM
                //Cipher Suite: TLS_PSK_WITH_AES_128_GCM_SHA256 (0x00a8)
            };
        }

        public override ProtocolVersion MinimumVersion
        {
            get { return ProtocolVersion.DTLSv10; }
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {

        }

        public override ProtocolVersion ClientVersion => ProtocolVersion.DTLSv12;
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
            while(mRecordQueue.Available < len && endtime > DateTime.Now)
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
