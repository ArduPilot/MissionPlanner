using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class TlsTestClientImpl
        :   DefaultTlsClient
    {
        protected readonly TlsTestConfig mConfig;

        protected int firstFatalAlertConnectionEnd = -1;
        protected int firstFatalAlertDescription = -1;

        internal TlsTestClientImpl(TlsTestConfig config)
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

        public override ProtocolVersion ClientVersion
        {
	        get 
	        { 
                if (mConfig.clientOfferVersion != null)
                {
                    return mConfig.clientOfferVersion;
                }

                return base.ClientVersion;
            }
        }

        public override ProtocolVersion MinimumVersion
        {
	        get 
	        { 
                if (mConfig.clientMinimumVersion != null)
                {
                    return mConfig.clientMinimumVersion;
                }

                return base.MinimumVersion;
	        }
        }

        public override IDictionary GetClientExtensions()
        {
            IDictionary clientExtensions = base.GetClientExtensions();
            if (clientExtensions != null && !mConfig.clientSendSignatureAlgorithms)
            {
                clientExtensions.Remove(ExtensionType.signature_algorithms);
                this.mSupportedSignatureAlgorithms = null;
            }
            return clientExtensions;
        }

        public override bool IsFallback
        {
            get { return mConfig.clientFallback; }
        }

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            if (alertLevel == AlertLevel.fatal && firstFatalAlertConnectionEnd == -1)
            {
                firstFatalAlertConnectionEnd = ConnectionEnd.client;
                firstFatalAlertDescription = alertDescription;
            }

            if (TlsTestConfig.DEBUG)
            {
                TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
                output.WriteLine("TLS client raised alert: " + AlertLevel.GetText(alertLevel)
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
        }

        public override void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
            if (alertLevel == AlertLevel.fatal && firstFatalAlertConnectionEnd == -1)
            {
                firstFatalAlertConnectionEnd = ConnectionEnd.server;
                firstFatalAlertDescription = alertDescription;
            }

            if (TlsTestConfig.DEBUG)
            {
                TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
                output.WriteLine("TLS client received alert: " + AlertLevel.GetText(alertLevel)
                    + ", " + AlertDescription.GetText(alertDescription));
            }
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
            base.NotifyServerVersion(serverVersion);

            if (TlsTestConfig.DEBUG)
            {
                Console.WriteLine("TLS client negotiated " + serverVersion);
            }
        }

        public override TlsAuthentication GetAuthentication()
        {
            return new MyTlsAuthentication(this, mContext);
        }

        protected virtual Certificate CorruptCertificate(Certificate cert)
        {
            X509CertificateStructure[] certList = cert.GetCertificateList();
            certList[0] = CorruptCertificateSignature(certList[0]);
            return new Certificate(certList);
        }

        protected virtual X509CertificateStructure CorruptCertificateSignature(X509CertificateStructure cert)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();
            v.Add(cert.TbsCertificate);
            v.Add(cert.SignatureAlgorithm);
            v.Add(CorruptSignature(cert.Signature));

            return X509CertificateStructure.GetInstance(new DerSequence(v));
        }

        protected virtual DerBitString CorruptSignature(DerBitString bs)
        {
            return new DerBitString(CorruptBit(bs.GetOctets()));
        }

        protected virtual byte[] CorruptBit(byte[] bs)
        {
            bs = Arrays.Clone(bs);

            // Flip a random bit
            int bit = mContext.SecureRandom.Next(bs.Length << 3); 
            bs[bit >> 3] ^= (byte)(1 << (bit & 7));

            return bs;
        }

        internal class MyTlsAuthentication
            :   TlsAuthentication
        {
            private readonly TlsTestClientImpl mOuter;
            private readonly TlsContext mContext;

            internal MyTlsAuthentication(TlsTestClientImpl outer, TlsContext context)
            {
                this.mOuter = outer;
                this.mContext = context;
            }

            public virtual void NotifyServerCertificate(Certificate serverCertificate)
            {
                bool isEmpty = serverCertificate == null || serverCertificate.IsEmpty;

                X509CertificateStructure[] chain = serverCertificate.GetCertificateList();

                // TODO Cache test resources?
                if (isEmpty || !(chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-server.pem"))
                    || chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-server-dsa.pem"))
                    || chain[0].Equals(TlsTestUtilities.LoadCertificateResource("x509-server-ecdsa.pem"))))
                {
                    throw new TlsFatalAlert(AlertDescription.bad_certificate);
                }

                if (TlsTestConfig.DEBUG)
                {
                    Console.WriteLine("TLS client received server certificate chain of length " + chain.Length);
                    for (int i = 0; i != chain.Length; i++)
                    {
                        X509CertificateStructure entry = chain[i];
                        // TODO Create fingerprint based on certificate signature algorithm digest
                        Console.WriteLine("    fingerprint:SHA-256 " + TlsTestUtilities.Fingerprint(entry) + " ("
                            + entry.Subject + ")");
                    }
                }
            }

            public virtual TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
            {
                if (mOuter.mConfig.serverCertReq == TlsTestConfig.SERVER_CERT_REQ_NONE)
                    throw new InvalidOperationException();
                if (mOuter.mConfig.clientAuth == TlsTestConfig.CLIENT_AUTH_NONE)
                    return null;

                byte[] certificateTypes = certificateRequest.CertificateTypes;
                if (certificateTypes == null || !Arrays.Contains(certificateTypes, ClientCertificateType.rsa_sign))
                {
                    return null;
                }

                IList supportedSigAlgs = certificateRequest.SupportedSignatureAlgorithms;
                if (supportedSigAlgs != null && mOuter.mConfig.clientAuthSigAlg != null)
                {
                    supportedSigAlgs = new ArrayList(1);
                    supportedSigAlgs.Add(mOuter.mConfig.clientAuthSigAlg);
                }

                TlsSignerCredentials signerCredentials = TlsTestUtilities.LoadSignerCredentials(mContext,
                    supportedSigAlgs, SignatureAlgorithm.rsa, "x509-client.pem", "x509-client-key.pem");

                if (mOuter.mConfig.clientAuth == TlsTestConfig.CLIENT_AUTH_VALID)
                {
                    return signerCredentials;
                }

                return new MyTlsSignerCredentials(mOuter, signerCredentials);
            }
        };

        internal class MyTlsSignerCredentials
            :   TlsSignerCredentials
        {
            private readonly TlsTestClientImpl mOuter;
            private readonly TlsSignerCredentials mInner;

            internal MyTlsSignerCredentials(TlsTestClientImpl outer, TlsSignerCredentials inner)
            {
                this.mOuter = outer;
                this.mInner = inner;
            }

            public virtual byte[] GenerateCertificateSignature(byte[] hash)
            {
                byte[] sig = mInner.GenerateCertificateSignature(hash);

                if (mOuter.mConfig.clientAuth == TlsTestConfig.CLIENT_AUTH_INVALID_VERIFY)
                {
                    sig = mOuter.CorruptBit(sig);
                }

                return sig;
            }

            public virtual Certificate Certificate
            {
                get
                {
                    Certificate cert = mInner.Certificate;

                    if (mOuter.mConfig.clientAuth == TlsTestConfig.CLIENT_AUTH_INVALID_CERT)
                    {
                        cert = mOuter.CorruptCertificate(cert);
                    }

                    return cert;
                }
            }

            public virtual SignatureAndHashAlgorithm SignatureAndHashAlgorithm
            {
                get { return mInner.SignatureAndHashAlgorithm; }
            }
        }
    }
}
