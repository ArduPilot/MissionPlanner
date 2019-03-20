using System;

using NUnit.Framework;

using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class X509CertificatePairTest
		: SimpleTest
	{
		public override void PerformTest()
		{
			//CertificateFactory cf = CertificateFactory.getInstance("X.509");
			X509CertificateParser cf = new X509CertificateParser();

			X509Certificate rootCert = (X509Certificate) cf.ReadCertificate(CertPathTest.rootCertBin);
			X509Certificate interCert = (X509Certificate) cf.ReadCertificate(CertPathTest.interCertBin);
			X509Certificate finalCert = (X509Certificate) cf.ReadCertificate(CertPathTest.finalCertBin);

			X509CertificatePair pair1 = new X509CertificatePair(rootCert, interCert);
			X509CertificatePair pair2 = new X509CertificatePair(rootCert, interCert);
			X509CertificatePair pair3 = new X509CertificatePair(interCert, finalCert);
			X509CertificatePair pair4 = new X509CertificatePair(rootCert, finalCert);
			X509CertificatePair pair5 = new X509CertificatePair(rootCert, null);
			X509CertificatePair pair6 = new X509CertificatePair(rootCert, null);
			X509CertificatePair pair7 = new X509CertificatePair(null, rootCert);
			X509CertificatePair pair8 = new X509CertificatePair(null, rootCert);

			if (!pair1.Equals(pair2))
			{
				Fail("pair1 pair2 equality test");
			}

			if (!pair5.Equals(pair6))
			{
				Fail("pair1 pair2 equality test");
			}

			if (!pair7.Equals(pair8))
			{
				Fail("pair1 pair2 equality test");
			}

			if (pair1.Equals(null))
			{
				Fail("pair1 null equality test");
			}

			if (pair1.GetHashCode() != pair2.GetHashCode())
			{
				Fail("pair1 pair2 hashCode equality test");
			}

			if (pair1.Equals(pair3))
			{
				Fail("pair1 pair3 inequality test");
			}

			if (pair1.Equals(pair4))
			{
				Fail("pair1 pair4 inequality test");
			}

			if (pair1.Equals(pair5))
			{
				Fail("pair1 pair5 inequality test");
			}

			if (pair1.Equals(pair7))
			{
				Fail("pair1 pair7 inequality test");
			}

			if (pair5.Equals(pair1))
			{
				Fail("pair5 pair1 inequality test");
			}

			if (pair7.Equals(pair1))
			{
				Fail("pair7 pair1 inequality test");
			}

			if (pair1.Forward != rootCert)
			{
				Fail("pair1 forward test");
			}

			if (pair1.Reverse != interCert)
			{
				Fail("pair1 reverse test");
			}

			if (!AreEqual(pair1.GetEncoded(), pair2.GetEncoded()))
			{
				Fail("encoding check");
			}

			pair4 = new X509CertificatePair(rootCert, TestUtilities.CreateExceptionCertificate(false));

			try
			{
				pair4.GetEncoded();

				Fail("no exception on bad GetEncoded()");
			}
			catch (CertificateEncodingException)
			{
				// expected
			}

			pair4 = new X509CertificatePair(rootCert, TestUtilities.CreateExceptionCertificate(true));

			try
			{
				pair4.GetEncoded();

				Fail("no exception on exception GetEncoded()");
			}
			catch (CertificateEncodingException)
			{
				// expected
			}
		}

		public override string Name
		{
			get { return "X509CertificatePair"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new X509CertificatePairTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
