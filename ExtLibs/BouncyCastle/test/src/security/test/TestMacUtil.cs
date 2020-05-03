using System;
using System.Globalization;
using System.Threading;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;

namespace Org.BouncyCastle.Security.Tests
{
    [TestFixture]
    public class TestMacUtilities
    {
        [Test]
        public void TestCultureIndependence()
        {
            Thread t = Thread.CurrentThread;
            CultureInfo ci = t.CurrentCulture;
            try
            {
                /*
                 * In Hungarian, the "CS" in "HMACSHA256" is linguistically a single character, so "HMAC" is not a prefix.
                 */
                t.CurrentCulture = new CultureInfo("hu-HU");
                IMac mac = MacUtilities.GetMac("HMACSHA256");
                Assert.NotNull(mac);
            }
            catch (Exception e)
            {
                Assert.Fail("Culture-specific lookup failed: " + e.Message);
            }
            finally
            {
                t.CurrentCulture = ci;
            }
        }
    }
}
