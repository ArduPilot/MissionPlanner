using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Smime;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class SmimeTest
		: ITest
	{
		private static byte[] attrBytes = Base64.Decode("MDQGCSqGSIb3DQEJDzEnMCUwCgYIKoZIhvcNAwcwDgYIKoZIhvcNAwICAgCAMAcGBSsOAwIH");
		private static byte[] prefBytes = Base64.Decode("MCwGCyqGSIb3DQEJEAILMR2hGwQIAAAAAAAAAAAYDzIwMDcwMzE1MTczNzI5Wg==");

		public ITestResult Perform()
		{
			SmimeCapabilityVector caps = new SmimeCapabilityVector();

			caps.AddCapability(SmimeCapability.DesEde3Cbc);
			caps.AddCapability(SmimeCapability.RC2Cbc, 128);
			caps.AddCapability(SmimeCapability.DesCbc);

			SmimeCapabilitiesAttribute attr = new SmimeCapabilitiesAttribute(caps);

			SmimeEncryptionKeyPreferenceAttribute pref = new SmimeEncryptionKeyPreferenceAttribute(
				new RecipientKeyIdentifier(new DerOctetString(new byte[8]),
				new DerGeneralizedTime("20070315173729Z"),
				null));

			try
			{
				if (!Arrays.AreEqual(attr.GetEncoded(), attrBytes))
				{
					return new SimpleTestResult(false, Name + ": Failed attr data check");
				}

				Asn1Object o = Asn1Object.FromByteArray(attrBytes);
				if (!attr.Equals(o))
				{
					return new SimpleTestResult(false, Name + ": Failed equality test for attr");
				}

				if (!Arrays.AreEqual(pref.GetEncoded(), prefBytes))
				{
					return new SimpleTestResult(false, Name + ": Failed attr data check");
				}

				o = Asn1Object.FromByteArray(prefBytes);
				if (!pref.Equals(o))
				{
					return new SimpleTestResult(false, Name + ": Failed equality test for pref");
				}

				return new SimpleTestResult(true, Name + ": Okay");
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": Failed - exception " + e.ToString(), e);
			}
		}

		public string Name
		{
			get { return "SMIME"; }
		}

		public static void Main(
			string[] args)
		{
			ITest test = new SmimeTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
