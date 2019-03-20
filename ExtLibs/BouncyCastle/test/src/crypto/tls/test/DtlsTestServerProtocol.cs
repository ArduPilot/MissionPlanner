using System;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class DtlsTestServerProtocol
        :   DtlsServerProtocol
    {
        protected readonly TlsTestConfig config;

        public DtlsTestServerProtocol(SecureRandom secureRandom, TlsTestConfig config)
            : base(secureRandom)
        {
            this.config = config;
        }
    }
}
