using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class MockSrpTlsServer
        :   SrpTlsServer
    {
        internal static readonly Srp6GroupParameters TEST_GROUP = Srp6StandardGroups.rfc5054_1024;
        internal static readonly byte[] TEST_IDENTITY = Strings.ToUtf8ByteArray("client");
        internal static readonly byte[] TEST_PASSWORD = Strings.ToUtf8ByteArray("password");
        internal static readonly byte[] TEST_SALT = Strings.ToUtf8ByteArray("salt");
        internal static readonly byte[] TEST_SEED_KEY = Strings.ToUtf8ByteArray("seed_key");

        internal MockSrpTlsServer()
            :   base(new MyIdentityManager())
        {
        }

        public override void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
            TextWriter output = (alertLevel == AlertLevel.fatal) ? Console.Error : Console.Out;
            output.WriteLine("TLS-SRP server raised alert: " + AlertLevel.GetText(alertLevel)
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
            output.WriteLine("TLS-SRP server received alert: " + AlertLevel.GetText(alertLevel)
                + ", " + AlertDescription.GetText(alertDescription));
        }

        public override void NotifyHandshakeComplete()
        {
            base.NotifyHandshakeComplete();

            byte[] srpIdentity = mContext.SecurityParameters.SrpIdentity;
            if (srpIdentity != null)
            {
                string name = Strings.FromUtf8ByteArray(srpIdentity);
                Console.WriteLine("TLS-SRP server completed handshake for SRP identity: " + name);
            }
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

            Console.WriteLine("TLS-SRP server negotiated " + serverVersion);

            return serverVersion;
        }

        protected override TlsSignerCredentials GetDsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, mSupportedSignatureAlgorithms, SignatureAlgorithm.dsa,
                "x509-server-dsa.pem", "x509-server-key-dsa.pem");
        }

        protected override TlsSignerCredentials GetRsaSignerCredentials()
        {
            return TlsTestUtilities.LoadSignerCredentials(mContext, mSupportedSignatureAlgorithms, SignatureAlgorithm.rsa,
                "x509-server.pem", "x509-server-key.pem");
        }

        internal class MyIdentityManager
            :   TlsSrpIdentityManager
        {
            protected SimulatedTlsSrpIdentityManager unknownIdentityManager = SimulatedTlsSrpIdentityManager.GetRfc5054Default(
                TEST_GROUP, TEST_SEED_KEY);

            public virtual TlsSrpLoginParameters GetLoginParameters(byte[] identity)
            {
                if (Arrays.AreEqual(TEST_IDENTITY, identity))
                {
                    Srp6VerifierGenerator verifierGenerator = new Srp6VerifierGenerator();
                    verifierGenerator.Init(TEST_GROUP, TlsUtilities.CreateHash(HashAlgorithm.sha1));

                    BigInteger verifier = verifierGenerator.GenerateVerifier(TEST_SALT, identity, TEST_PASSWORD);

                    return new TlsSrpLoginParameters(TEST_GROUP, verifier, TEST_SALT);
                }

                return unknownIdentityManager.GetLoginParameters(identity);
            }
        }
    }
}
