using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	/**
	* PkiFailureInfoTest
	*/
	[TestFixture]
	public class PkiFailureInfoTest
		: SimpleTest
	{
		// A correct hex encoded BAD_DATA_FORMAT PkiFailureInfo
		private static readonly byte[] CORRECT_FAILURE_INFO = Base64.Decode("AwIANQ==");

		public override string Name
		{
			get { return "PkiFailureInfo"; }
		}

        private void DoTestEncoding()
		{
			DerBitString bitString = (DerBitString)Asn1Object.FromByteArray(CORRECT_FAILURE_INFO);
			PkiFailureInfo correct = new PkiFailureInfo(bitString);

			PkiFailureInfo bug = new PkiFailureInfo(
                PkiFailureInfo.BadRequest | PkiFailureInfo.BadTime | PkiFailureInfo.BadDataFormat | PkiFailureInfo.IncorrectData);

			if (!Arrays.AreEqual(correct.GetDerEncoded(), bug.GetDerEncoded()))
			{
				Fail("encoding doesn't match");
			}
		}

		public override void PerformTest()
		{
            BitStringConstantTester.testFlagValueCorrect(0, PkiFailureInfo.BadAlg);
            BitStringConstantTester.testFlagValueCorrect(1, PkiFailureInfo.BadMessageCheck);
            BitStringConstantTester.testFlagValueCorrect(2, PkiFailureInfo.BadRequest);
            BitStringConstantTester.testFlagValueCorrect(3, PkiFailureInfo.BadTime);
            BitStringConstantTester.testFlagValueCorrect(4, PkiFailureInfo.BadCertId);
            BitStringConstantTester.testFlagValueCorrect(5, PkiFailureInfo.BadDataFormat);
            BitStringConstantTester.testFlagValueCorrect(6, PkiFailureInfo.WrongAuthority);
            BitStringConstantTester.testFlagValueCorrect(7, PkiFailureInfo.IncorrectData);
            BitStringConstantTester.testFlagValueCorrect(8, PkiFailureInfo.MissingTimeStamp);
            BitStringConstantTester.testFlagValueCorrect(9, PkiFailureInfo.BadPop);
            BitStringConstantTester.testFlagValueCorrect(10, PkiFailureInfo.CertRevoked);
            BitStringConstantTester.testFlagValueCorrect(11, PkiFailureInfo.CertConfirmed);
            BitStringConstantTester.testFlagValueCorrect(12, PkiFailureInfo.WrongIntegrity);
            BitStringConstantTester.testFlagValueCorrect(13, PkiFailureInfo.BadRecipientNonce);
            BitStringConstantTester.testFlagValueCorrect(14, PkiFailureInfo.TimeNotAvailable);
            BitStringConstantTester.testFlagValueCorrect(15, PkiFailureInfo.UnacceptedPolicy);
            BitStringConstantTester.testFlagValueCorrect(16, PkiFailureInfo.UnacceptedExtension);
            BitStringConstantTester.testFlagValueCorrect(17, PkiFailureInfo.AddInfoNotAvailable);
            BitStringConstantTester.testFlagValueCorrect(18, PkiFailureInfo.BadSenderNonce);
            BitStringConstantTester.testFlagValueCorrect(19, PkiFailureInfo.BadCertTemplate);
            BitStringConstantTester.testFlagValueCorrect(20, PkiFailureInfo.SignerNotTrusted);
            BitStringConstantTester.testFlagValueCorrect(21, PkiFailureInfo.TransactionIdInUse);
            BitStringConstantTester.testFlagValueCorrect(22, PkiFailureInfo.UnsupportedVersion);
            BitStringConstantTester.testFlagValueCorrect(23, PkiFailureInfo.NotAuthorized);
            BitStringConstantTester.testFlagValueCorrect(24, PkiFailureInfo.SystemUnavail);
            BitStringConstantTester.testFlagValueCorrect(25, PkiFailureInfo.SystemFailure);
            BitStringConstantTester.testFlagValueCorrect(26, PkiFailureInfo.DuplicateCertReq);

            DoTestEncoding();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PkiFailureInfoTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
