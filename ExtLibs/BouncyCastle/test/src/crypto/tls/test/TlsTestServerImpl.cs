using System;
using System.Collections;
using System.IO;
using System.Threading;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class TlsTestServerImpl
        :   DefaultTlsServer
    {
        protected readonly TlsTestConfig mConfig;

        protected int firstFatalAlertConnectionEnd = -1;
        protected int firstFatalAlertDescription = -1;

        internal TlsTestServerImpl(TlsTestConfig config)
        {
            this.mConfig = config;
        }

        internal int FirstFatalAlertConnectionEnd
        {
            get { return firstFatalAlertConnectionEnd; }
        }

        internal int FirstFatalAlertDescription
        {
            get { return firstFatalAlertDescription; }
        }

        protected override ProtocolVersion MaximumVersion
        {
	        get 
	        { 
                if (mConfig.serverMaximumVersion != null)
                {
                    return mConfig.serverMaximumVersion;
                }

                return base.MaximumVersion;
	        }
        }

        protected override ProtocolVersion MinimumVersion
        {
	        get 
	        { 
                if (mConfig.serverMinimumVersion != null)
                {
                    return mConfig.serverMinimumVersion;
                }

                return base.MinimumVersion;
	        }
        }

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            if (alertLevel == AlertLevel.fatal && firstFatalAlertConnectionEnd == -1)
            {
                firstFatalAlertConnectionEnd = ConnectionEnd.server;
                firstFatalAlertDescription = alertDescription;
            }

            if (TlsTestConfig.DEBUG)
            {
                TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
                output.WriteLine("TLS server raised alert: " + AlertLevel.GetText(alertLevel)
                    + ", " + AlertDescription.GetText(alertDescription));
                if (message != null)
                {
                    SafeWriteLine(output, "> " + message);
                }
                if (cause != null)
                {
                    SafeWriteLine(output, cause);
                }
            }
        }

        public override void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
            if (alertLevel == AlertLevel.fatal && firstFatalAlertConnectionEnd == -1)
            {
                firstFatalAlertConnectionEnd = ConnectionEnd.client;
                firstFatalAlertDescription = alertDescription;
            }

            if (TlsTestConfig.DEBUG)
            {
                TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
                SafeWriteLine(output, "TLS server received alert: " + AlertLevel.GetText(alertLevel)
                    + ", " + AlertDescription.GetText(alertDescription));
            }
        }

        public override ProtocolVersion GetServerVersion()
        {
            ProtocolVersion serverVersion = base.GetServerVersion();

            if (TlsTestConfig.DEBUG)
            {
                Console.WriteLine("TLS server negotiated " + serverVersion);
            }

            return serverVersion;
        }

        public override CertificateRequest GetCertificateRequest()
        {
            if (mConfig.serverCertReq == TlsTestConfig.SERVER_CERT_REQ_NONE)
            {
                return null;
            }

            byte[] certificateTypes = new byte[]{ ClientCertificateType.rsa_sign,
                ClientCertificateType.dss_sign, ClientCertificateType.ecdsa_sign };

            IList serverSigAlgs = null;
            if (TlsUtilities.IsSignatureAlgorithmsExtensionAllowed(mServerVersion))
            {
                serverSigAlgs = mConfig.serverCertReqSigAlgs;
                if (serverSigAlgs == null)
                {
                    serverSigAlgs = TlsUtilities.GetDefaultSupportedSignatureAlgorithms();
                }
            }

            IList certificateAuthorities = new ArrayList();
            certificateAuthorities.Add(TlsTestUtilities.LoadCertificateResource("x509-ca.pem").Subject);

            return new CertificateRequest(certificateTypes, serverSigAlgs, certificateAuthorities);
        }

        public override void NotifyClientCertificate(Certificate clientCertificate)
        {
            bool isEmpty = (clientCertificate == null || clientCertificate.IsEmpty);

            if (isEmpty != (mConfig.clientAuth == TlsTestConfig.CLIENT_AUTH_NONE))
            {
                throw new InvalidOperationException();
            }
            if (isEmpty && (mConfig.serverCertReq == TlsTestConfig.SERVER_CERT_REQ_MANDATORY))
            {
                throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }

            X509CertificateStructure[] chain = clientCertificate.GetCertificateList();

            // TODO Cache test resources?
            if (!isEmpty && !(chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-client.pem"))
                || chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-client-dsa.pem"))
                || chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-client-ecdsa.pem"))))
            {
                throw new TlsFatalAlert(AlertDescription.bad_certificate);
            }

            if (TlsTestConfig.DEBUG)
            {
                Console.WriteLine("TLS server received client certificate chain of length " + chain.Length);
                for (int i = 0; i != chain.Length; i++)
                {
                    X509CertificateStructure entry = chain[i];
                    // TODO Create fingerprint based on certificate signature algorithm digest
                    Console.WriteLine("    fingerprint:SHA-256 " + TlsTestUtilities.Fingerprint(entry) + " ("
                        + entry.Subject + ")");
                }
            }
        }

        protected virtual IList GetSupportedSignatureAlgorithms()
        {
            if (TlsUtilities.IsTlsV12(mContext) && mConfig.serverAuthSigAlg != null)
            {
                IList signatureAlgorithms = new ArrayList(1);
                signatureAlgorithms.Add(mConfig.serverAuthSigAlg);
                return signatureAlgorithms;
            }

            return mSupportedSignatureAlgorithms;
        }

        protected override TlsSignerCredentials GetDsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, GetSupportedSignatureAlgorithms(), SignatureAlgorithm.dsa,
                "x509-server-dsa.pem", "x509-server-key-dsa.pem");
        }

        protected override TlsSignerCredentials GetECDsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, GetSupportedSignatureAlgorithms(), SignatureAlgorithm.ecdsa,
                "x509-server-ecdsa.pem", "x509-server-key-ecdsa.pem");
        }

        protected override TlsEncryptionCredentials GetRsaEncryptionCredentials()
        {
            return TlsTestUtilities.LoadEncryptionCredentials(mContext, new string[]{ "x509-server.pem", "x509-ca.pem" },
                "x509-server-key.pem");
        }

        protected override TlsSignerCredentials GetRsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, GetSupportedSignatureAlgorithms(), SignatureAlgorithm.rsa,
                "x509-server.pem", "x509-server-key.pem");
        }

        private static void SafeWriteLine(TextWriter output, object line)
        {
            try
            {
                output.WriteLine(line);
            }
            catch (ThreadInterruptedException)
            {
                /*
                 * For some reason the NUnit plugin in Visual Studio started throwing these during alert logging
                 */
            }
        }
    }
}
