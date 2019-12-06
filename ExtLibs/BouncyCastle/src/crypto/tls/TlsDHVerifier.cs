using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>An interface for verifying that Diffie-Hellman parameters are acceptable.</summary>
    public interface TlsDHVerifier
    {
        /// <summary>Verify that the given <c>DHParameters</c> are acceptable.</summary>
        /// <param name="dhParameters">The <c>DHParameters</c> to verify.</param>
        /// <returns>true if (and only if) the specified parameters are acceptable.</returns>
        bool Accept(DHParameters dhParameters);
    }
}
