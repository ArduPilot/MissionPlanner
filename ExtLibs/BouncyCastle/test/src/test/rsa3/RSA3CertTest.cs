using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests.Rsa3
{
	/**
	* Marius Schilder's Bleichenbacher's Forgery Attack Tests
	*/
	[TestFixture]
	public class RSA3CertTest
		//extends TestCase
	{
		[Test]
		public void TestA()
		{
			doTest("self-testcase-A.pem");
		}

		[Test]
		public void TestB()
		{
			doTest("self-testcase-B.pem");
		}

		[Test]
		public void TestC()
		{
			doTest("self-testcase-C.pem");
		}

		[Test]
		public void TestD()
		{
			doTest("self-testcase-D.pem");
		}

		[Test]
		public void TestE()
		{
			doTest("self-testcase-E.pem");
		}

		[Test]
		public void TestF()
		{
			doTest("self-testcase-F.pem");
		}

		[Test]
		public void TestG()
		{
			doTest("self-testcase-G.pem");
		}

		[Test]
		public void TestH()
		{
			doTest("self-testcase-H.pem");
		}

		[Test]
		public void TestI()
		{
			doTest("self-testcase-I.pem");
		}

		[Test]
		public void TestJ()
		{
			doTest("self-testcase-J.pem");
		}

		[Test]
		public void TestL()
		{
			doTest("self-testcase-L.pem");
		}

		private void doTest(
			string certName)
		{
			X509Certificate cert = loadCert(certName);
			byte[] tbs = cert.GetTbsCertificate();
			ISigner sig = SignerUtilities.GetSigner(cert.SigAlgName);

			sig.Init(false, cert.GetPublicKey());

			sig.BlockUpdate(tbs, 0, tbs.Length);

			Assert.IsFalse(sig.VerifySignature(cert.GetSignature()));
		}

		private X509Certificate loadCert(
			string certName)
		{
			Stream s = SimpleTest.GetTestDataAsStream("rsa3." + certName);
			TextReader tr = new StreamReader(s);
			PemReader rd = new PemReader(tr);

			return (X509Certificate) rd.ReadObject();
		}

//		public static void main (string[] args) 
//			throws Exception
//		{
//			junit.textui.TestRunner.run(suite());
//		}
//	    
//		public static Test suite() 
//			throws Exception
//		{   
//			TestSuite suite = new TestSuite("Bleichenbacher's Forgery Attack Tests");
//	        
//			suite.addTestSuite(RSA3CertTest.class);
//	        
//			return suite;
//		}
	}
}
