using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class DtlsTestClientProtocol
        :   DtlsClientProtocol
    {
        protected readonly TlsTestConfig config;

        public DtlsTestClientProtocol(SecureRandom secureRandom, TlsTestConfig config)
            : base(secureRandom)
        {
            this.config = config;
        }

        protected override byte[] GenerateCertificateVerify(ClientHandshakeState state, DigitallySigned certificateVerify)
        {
            if (certificateVerify.Algorithm != null && config.clientAuthSigAlgClaimed != null)
            {
                certificateVerify = new DigitallySigned(config.clientAuthSigAlgClaimed, certificateVerify.Signature);
            }

            return base.GenerateCertificateVerify(state, certificateVerify);
        }
    }
}
