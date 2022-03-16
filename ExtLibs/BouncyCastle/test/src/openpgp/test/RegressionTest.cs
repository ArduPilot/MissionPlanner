using System;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    public class RegressionTest
    {
        public static ITest[] tests =
        {
            new PgpKeyRingTest(),
            new PgpRsaTest(),
            new PgpDsaTest(),
            new PgpDsaElGamalTest(),
            new PgpPbeTest(),
            new PgpMarkerTest(),
            new PgpPacketTest(),
            new PgpArmoredTest(),
            new PgpSignatureTest(),
            new PgpClearSignedSignatureTest(),
            new PgpCompressionTest(),
            new PgpNoPrivateKeyTest(),
            new PgpECDHTest(),
            new PgpECDsaTest(),
            new PgpECMessageTest(),
            new PgpParsingTest(),
        };

        public static void Main(
            string[] args)
        {
            foreach (ITest test in tests)
            {
                ITestResult result = test.Perform();
                Console.WriteLine(result);
            }
        }
    }
}
