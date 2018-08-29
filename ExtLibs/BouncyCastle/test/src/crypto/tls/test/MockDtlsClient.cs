using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class MockDtlsClient
        :   DefaultTlsClient
    {
        protected TlsSession mSession;

        public MockDtlsClient(TlsSession session)
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

        public override void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("DTLS client received alert: " + AlertLevel.GetText(alertLevel)
                + ", " + AlertDescription.GetText(alertDescription));
        }

        public override ProtocolVersion ClientVersion
        {
            get { return ProtocolVersion.DTLSv12; }
        }

        public override ProtocolVersion MinimumVersion
        {
            get { return ProtocolVersion.DTLSv10; }
        }

        //public override int[] GetCipherSuites()
        //{
        //    return Arrays.Concatenate(base.GetCipherSuites(),
        //        new int[]
        //        {
        //            CipherSuite.DRAFT_TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
        //        });
        //}

        public override IDictionary GetClientExtensions()
        {
            IDictionary clientExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(base.GetClientExtensions());
            TlsExtensionsUtilities.AddEncryptThenMacExtension(clientExtensions);
            TlsExtensionsUtilities.AddExtendedMasterSecretExtension(clientExtensions);
            {
                /*
                 * NOTE: If you are copying test code, do not blindly set these extensions in your own client.
                 */
                TlsExtensionsUtilities.AddMaxFragmentLengthExtension(clientExtensions, MaxFragmentLength.pow2_9);
                TlsExtensionsUtilities.AddPaddingExtension(clientExtensions, mContext.SecureRandom.Next(16));
                TlsExtensionsUtilities.AddTruncatedHMacExtension(clientExtensions);
            }
            return clientExtensions;
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
            base.NotifyServerVersion(serverVersion);

            Console.WriteLine("Negotiated " + serverVersion);
        }

        public override TlsAuthentication GetAuthentication()
        {
            return new MyTlsAuthentication(mContext);
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

        internal class MyTlsAuthentication
            :   TlsAuthentication
        {
            private readonly TlsContext mContext;

            internal MyTlsAuthentication(TlsContext context)
            {
                this.mContext = context;
            }

            public virtual void NotifyServerCertificate(Certificate serverCertificate)
            {
                X509CertificateStructure[] chain = serverCertificate.GetCertificateList();
                Console.WriteLine("DTLS client received server certificate chain of length " + chain.Length);
                for (int i = 0; i != chain.Length; i++)
                {
                    X509CertificateStructure entry = chain[i];
                    // TODO Create fingerprint based on certificate signature algorithm digest
                    Console.WriteLine("    fingerprint:SHA-256 " + TlsTestUtilities.Fingerprint(entry) + " ("
                        + entry.Subject + ")");
                }
            }

            public virtual TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
            {
                byte[] certificateTypes = certificateRequest.CertificateTypes;
                if (certificateTypes == null || !Arrays.Contains(certificateTypes, ClientCertificateType.rsa_sign))
                    return null;

                return TlsTestUtilities.LoadSignerCredentials(mContext, certificateRequest.SupportedSignatureAlgorithms,
                    SignatureAlgorithm.rsa, "x509-client.pem", "x509-client-key.pem");
            }
        };
    }
}
