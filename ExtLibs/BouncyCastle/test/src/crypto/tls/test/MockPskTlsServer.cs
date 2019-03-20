using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class MockPskTlsServer
        :   PskTlsServer
    {
        internal MockPskTlsServer()
            :   base(new MyIdentityManager())
        {
        }

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("TLS-PSK server raised alert: " + AlertLevel.GetText(alertLevel)
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

        public override void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("TLS-PSK server received alert: " + AlertLevel.GetText(alertLevel)
                + ", " + AlertDescription.GetText(alertDescription));
        }

        public override void NotifyHandshakeComplete()
        {
            base.NotifyHandshakeComplete();

            byte[] pskIdentity = mContext.SecurityParameters.PskIdentity;
            if (pskIdentity != null)
            {
                string name = Strings.FromUtf8ByteArray(pskIdentity);
                Console.WriteLine("TLS-PSK server completed handshake for PSK identity: " + name);
            }
        }

        protected override int[] GetCipherSuites()
        {
            return new int[]{ CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384, CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA };
        }

        protected override ProtocolVersion MaximumVersion
        {
            get { return ProtocolVersion.TLSv12; }
        }

        protected override ProtocolVersion MinimumVersion
        {
            get { return ProtocolVersion.TLSv12; }
        }

        public override ProtocolVersion GetServerVersion()
        {
            ProtocolVersion serverVersion = base.GetServerVersion();

            Console.WriteLine("TLS-PSK server negotiated " + serverVersion);

            return serverVersion;
        }

        protected override TlsEncryptionCredentials GetRsaEncryptionCredentials()
        {
            return TlsTestUtilities.LoadEncryptionCredentials(mContext, new string[]{"x509-server.pem", "x509-ca.pem"},
                "x509-server-key.pem");
        }

        internal class MyIdentityManager
            :   TlsPskIdentityManager
        {
            public virtual byte[] GetHint()
            {
                return Strings.ToUtf8ByteArray("hint");
            }

            public virtual byte[] GetPsk(byte[] identity)
            {
                if (identity != null)
                {
                    string name = Strings.FromUtf8ByteArray(identity);
                    if (name.Equals("client"))
                    {
                        return new byte[16];
                    }
                }
                return null;
            }
        }
    }
}
