using System;
using System.IO;

using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class TlsTestServerProtocol
        :   TlsServerProtocol
    {
        protected readonly TlsTestConfig config;

        public TlsTestServerProtocol(Stream stream, SecureRandom secureRandom, TlsTestConfig config)
            : base(stream, secureRandom)
        {
            this.config = config;
        }
    }
}
