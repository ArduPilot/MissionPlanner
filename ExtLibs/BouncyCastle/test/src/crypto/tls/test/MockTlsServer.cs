using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class MockTlsServer
        :   DefaultTlsServer
    {
        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("TLS server raised alert: " + AlertLevel.GetText(alertLevel)
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
            output.WriteLine("TLS server received alert: " + AlertLevel.GetText(alertLevel)
                + ", " + AlertDescription.GetText(alertDescription));
        }

        protected override int[] GetCipherSuites()
        {
            return Arrays.Concatenate(base.GetCipherSuites(),
                new int[]
                {
                    CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                });
        }

        protected override ProtocolVersion MaximumVersion
        {
            get { return ProtocolVersion.TLSv12; }
        }

        public override ProtocolVersion GetServerVersion()
        {
            ProtocolVersion serverVersion = base.GetServerVersion();

            Console.WriteLine("TLS server negotiated " + serverVersion);

            return serverVersion;
        }

        public override CertificateRequest GetCertificateRequest()
        {
            byte[] certificateTypes = new byte[]{ ClientCertificateType.rsa_sign,
                ClientCertificateType.dss_sign, ClientCertificateType.ecdsa_sign };

            IList serverSigAlgs = null;
            if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(mServerVersion))
            {
                serverSigAlgs = TlsUtilities.GetDefaultSupportedSignatureAlgorithms();
            }

            IList certificateAuthorities = new ArrayList();
            certificateAuthorities.Add(TlsTestUtilities.LoadCertificateResource("x509-ca.pem").Subject);

            return new CertificateRequest(certificateTypes, serverSigAlgs, certificateAuthorities);
        }

        public override void NotifyClientCertificate(Certificate clientCertificate)
        {
            X509CertificateStructure[] chain = clientCertificate.GetCertificateList();
            Console.WriteLine("TLS server received client certificate chain of length " + chain.Length);
            for (int i = 0; i != chain.Length; i++)
            {
                X509CertificateStructure entry = chain[i];
                // TODO Create fingerprint based on certificate signature algorithm digest
                Console.WriteLine("    fingerprint:SHA-256 " + TlsTestUtilities.Fingerprint(entry) + " ("
                    + entry.Subject + ")");
            }
        }

        protected override TlsEncryptionCredentials GetRsaEncryptionCredentials()
        {
            return TlsTestUtilities.LoadEncryptionCredentials(mContext, new string[]{ "x509-server.pem", "x509-ca.pem" },
                "x509-server-key.pem");
        }

        protected override TlsSignerCredentials GetRsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, mSupportedSignatureAlgorithms, SignatureAlgorithm.rsa,
                "x509-server.pem", "x509-server-key.pem");
        }
    }
}
