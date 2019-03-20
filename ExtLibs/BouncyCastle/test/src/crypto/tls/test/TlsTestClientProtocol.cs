using System;
using System.IO;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class TlsTestClientProtocol
        :   TlsClientProtocol
    {
        protected readonly TlsTestConfig config;

        public TlsTestClientProtocol(Stream stream, SecureRandom secureRandom, TlsTestConfig config)
            : base(stream, secureRandom)
        {
            this.config = config;
        }

        protected override void SendCertificateVerifyMessage(DigitallySigned certificateVerify)
        {
            if (certificateVerify.Algorithm != null && config.clientAuthSigAlgClaimed != null)
            {
                certificateVerify = new DigitallySigned(config.clientAuthSigAlgClaimed, certificateVerify.Signature);
            }

            base.SendCertificateVerifyMessage(certificateVerify);
        }
    }
}
