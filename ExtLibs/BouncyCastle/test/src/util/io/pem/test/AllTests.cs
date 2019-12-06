using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;

#if !LIB
using NUnit.Core;
#endif
using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Utilities.IO.Pem.Tests
{
	[TestFixture]
	public class AllTests
	{
#if !LIB
        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("PEM Utilities Tests");
                suite.Add(new AllTests());
                return suite;
            }
        }
#endif

        [Test]
		public void TestPemLength()
		{
			for (int i = 1; i != 60; i++)
			{
				lengthTest("CERTIFICATE", new ArrayList(), new byte[i]);
			}

			lengthTest("CERTIFICATE", new ArrayList(), new byte[100]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[101]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[102]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[103]);

			lengthTest("CERTIFICATE", new ArrayList(), new byte[1000]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[1001]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[1002]);
			lengthTest("CERTIFICATE", new ArrayList(), new byte[1003]);

			IList headers = new ArrayList();
			headers.Add(new PemHeader("Proc-Type", "4,ENCRYPTED"));
			headers.Add(new PemHeader("DEK-Info", "DES3,0001020304050607"));
			lengthTest("RSA PRIVATE KEY", headers, new byte[103]);
		}

		private void lengthTest(string type, IList headers, byte[] data)
		{
			StringWriter sw = new StringWriter();
			PemWriter pWrt = new PemWriter(sw);

			PemObject pemObj = new PemObject(type, headers, data);
			pWrt.WriteObject(pemObj);
			pWrt.Writer.Close();

			Assert.AreEqual(sw.ToString().Length, pWrt.GetOutputSize(pemObj));
		}
	}
}
