using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Asn1.X500;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class AdditionalInformationSyntaxUnitTest
		: Asn1UnitTest
	{
		public override string Name
		{
			get { return "AdditionalInformationSyntax"; }
		}

		public override void PerformTest()
		{
			AdditionalInformationSyntax syntax = new AdditionalInformationSyntax("hello world");

			checkConstruction(syntax, new DirectoryString("hello world"));

			try
			{
				AdditionalInformationSyntax.GetInstance(new Object());

				Fail("GetInstance() failed to detect bad object.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		private void checkConstruction(
			AdditionalInformationSyntax syntax,
			DirectoryString information)
		{
			checkValues(syntax, information);

			syntax = AdditionalInformationSyntax.GetInstance(syntax);

			checkValues(syntax, information);

			Asn1InputStream aIn = new Asn1InputStream(syntax.ToAsn1Object().GetEncoded());

			IAsn1String info = (IAsn1String) aIn.ReadObject();

			syntax = AdditionalInformationSyntax.GetInstance(info);

			checkValues(syntax, information);
		}

		private void checkValues(
			AdditionalInformationSyntax syntax,
			DirectoryString information)
		{
			checkMandatoryField("information", information, syntax.Information);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AdditionalInformationSyntaxUnitTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
