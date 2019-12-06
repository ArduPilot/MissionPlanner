using System;

using Org.BouncyCastle.Crypto.Parameters;

namespace Org.BouncyCastle.Crypto.Tests
{
    public class AeadTestUtilities
    {
        internal static AeadParameters ReuseKey(AeadParameters p)
        {
            return new AeadParameters(null, p.MacSize, p.GetNonce(), p.GetAssociatedText());
        }
    }
}
