using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class MockPskTlsClient
        :   PskTlsClient
    {
        internal TlsSession mSession;

        internal MockPskTlsClient(TlsSession session)
            :   this(session, new BasicTlsPskIdentity("client", new byte[16]))
        {
        }

        internal MockPskTlsClient(TlsSession session, TlsPskIdentity pskIdentity)
            :   base(pskIdentity)
        {
            this.mSession = session;
        }

        public override TlsSession GetSessionToResume()
        {
            return this.mSession;
        }

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("TLS-PSK client raised alert: " + AlertLevel.GetText(alertLevel)
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
            output.WriteLine("TLS-PSK client received alert: " + AlertLevel.GetText(alertLevel)
                + ", " + AlertDescription.GetText(alertDescription));
        }

        public override void NotifyHandshakeComplete()
        {
            base.NotifyHandshakeComplete();

            TlsSession newSession = mContext.ResumableSession;
            if (newSession != null)
            {
                byte[] newSessionID = newSession.SessionID;
                string hex = Hex.ToHexString(newSessionID);

                if (this.mSession != null && Arrays.AreEqual(this.mSession.SessionID, newSessionID))
                {
                    Console.WriteLine("Resumed session: " + hex);
                }
                else
                {
                    Console.WriteLine("Established session: " + hex);
                }

                this.mSession = newSession;
            }
        }

        public override int[] GetCipherSuites()
        {
            return new int[]{ CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384, CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384,
                CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA };
        }

        public override ProtocolVersion MinimumVersion
        {
	        get { return ProtocolVersion.TLSv12; }
        }

        public override IDictionary GetClientExtensions()
        {
            IDictionary clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
            TlsExtensionsUtilities.AddEncryptThenMacExtension(clientExtensions);
            return clientExtensions;
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
            base.NotifyServerVersion(serverVersion);

            Console.WriteLine("TLS-PSK client negotiated " + serverVersion);
        }

        public override TlsAuthentication GetAuthentication()
        {
            return new MyTlsAuthentication(mContext);
        }

        internal class MyTlsAuthentication
            :   ServerOnlyTlsAuthentication
        {
            private readonly TlsContext mContext;

            internal MyTlsAuthentication(TlsContext context)
            {
                this.mContext = context;
            }

            public override void NotifyServerCertificate(Certificate serverCertificate)
            {
                X509CertificateStructure[] chain = serverCertificate.GetCertificateList();
                Console.WriteLine("TLS-PSK client received server certificate chain of length " + chain.Length);
                for (int i = 0; i != chain.Length; i++)
                {
                    X509CertificateStructure entry = chain[i];
                    // TODO Create fingerprint based on certificate signature algorithm digest
                    Console.WriteLine("    fingerprint:SHA-256 " + TlsTestUtilities.Fingerprint(entry) + " ("
                        + entry.Subject + ")");
                }
            }
        };
    }
}
