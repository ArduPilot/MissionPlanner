using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <summary>
	/// Test class for {@link PkixNameConstraintValidator}.
	/// The field testXYZ is the name to test.
	/// The field testXYZIsConstraint must be tested if it is permitted and excluded.
	/// The field testXYZIsNotConstraint must be tested if it is not permitted and
	/// not excluded.
	/// Furthermore there are tests for the intersection and union of test names.
	/// </summary>
	[TestFixture]
	public class PkixNameConstraintsTest
		: SimpleTest
	{
		private readonly string testEmail = "test@abc.test.com";

		private readonly string[] testEmailIsConstraint = { "test@abc.test.com", "abc.test.com", ".test.com" };

		private readonly string[] testEmailIsNotConstraint = { ".abc.test.com", "www.test.com", "test1@abc.test.com", "bc.test.com" };

		private readonly string[] email1 = {
			"test@test.com", "test@test.com", "test@test.com", "test@abc.test.com",
			"test@test.com", "test@test.com", ".test.com", ".test.com",
			".test.com", ".test.com", "test.com", "abc.test.com",
			"abc.test1.com", "test.com", "test.com", ".test.com" };

		private readonly string[] email2 = {
			"test@test.abc.com", "test@test.com", ".test.com", ".test.com",
			"test.com", "test1.com", "test@test.com", ".test.com",
			".test1.com", "test.com", "test.com", ".test.com", ".test.com",
			"test1.com", ".test.com", "abc.test.com" };

		private readonly string[] emailintersect = {
			null, "test@test.com", null, "test@abc.test.com", "test@test.com", null,
			null, ".test.com", null, null, "test.com", "abc.test.com", null,
			null, null, "abc.test.com" };

		private readonly string[][] emailunion = new string[16][] {
			new string[] { "test@test.com", "test@test.abc.com" },
			new string[] { "test@test.com" },
			new string[] { "test@test.com", ".test.com" },
			new string[] { ".test.com" },
			new string[] { "test.com" },
			new string[] { "test@test.com", "test1.com" },
			new string[] { ".test.com", "test@test.com" },
			new string[] { ".test.com" },
			new string[] { ".test.com", ".test1.com" },
			new string[] { ".test.com", "test.com" },
			new string[] { "test.com" },
			new string[] { ".test.com" },
			new string[] { ".test.com", "abc.test1.com" },
			new string[] { "test1.com", "test.com" },
			new string[] { ".test.com", "test.com" },
			new string[] { ".test.com" } };

		private readonly string[] dn1 = { "O=test org, OU=test org unit, CN=John Doe" };

		private readonly string[] dn2 = { "O=test org, OU=test org unit" };

		private readonly string[][] dnUnion = new string[1][] {
			new string[] { "O=test org, OU=test org unit" } };

		private readonly string[] dnIntersection = { "O=test org, OU=test org unit, CN=John Doe" };

		private readonly string testDN = "O=test org, OU=test org unit, CN=John Doe";

		private readonly string[] testDNIsConstraint = {
			"O=test org, OU=test org unit",
			"O=test org, OU=test org unit, CN=John Doe" };

		private readonly string[] testDNIsNotConstraint = {
			"O=test org, OU=test org unit, CN=John Doe2",
			"O=test org, OU=test org unit2",
			"OU=test org unit, O=test org, CN=John Doe",
			"O=test org, OU=test org unit, CN=John Doe, L=USA" };

		private readonly string testDNS = "abc.test.com";

		private readonly string[] testDNSIsConstraint = { "test.com", "abc.test.com", "test.com" };

		private readonly string[] testDNSIsNotConstraint = { "wwww.test.com", "ww.test.com", "www.test.com" };

		private readonly string[] dns1 = { "www.test.de", "www.test1.de", "www.test.de" };

		private readonly string[] dns2 = { "test.de", "www.test.de", "www.test.de" };

		private readonly string[] dnsintersect = { "www.test.de", null, null };

		private readonly string[][] dnsunion = new string[3][] {
			new string[] { "test.de" },
			new string[] { "www.test1.de", "www.test.de" },
			new string[] { "www.test.de" } };

		private readonly string testURI = "http://karsten:password@abc.test.com:8080";

		private readonly string[] testURIIsConstraint = { "abc.test.com", ".test.com" };

		private readonly string[] testURIIsNotConstraint = { "xyz.test.com", ".abc.test.com" };

		private readonly string[] uri1 = { "www.test.de", ".test.de", "test1.de", ".test.de" };

		private readonly string[] uri2 = { "test.de", "www.test.de", "test1.de", ".test.de" };

		private readonly string[] uriintersect = { null, "www.test.de", "test1.de", ".test.de" };

		private readonly string[][] uriunion = new string[4][] {
			new string[] { "www.test.de", "test.de" },
			new string[] { ".test.de" },
			new string[] { "test1.de" },
			new string[] { ".test.de" } };

		private readonly byte[] testIP = { (byte)192, (byte)168, 1, 2 };

		private readonly byte[][] testIPIsConstraint = new byte[2][] {
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, 0 },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, 4 } };

		private readonly byte[][] testIPIsNotConstraint = new byte[2][] {
			new byte[] { (byte) 192, (byte) 168, 3, 1, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, 2 },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF, 3 } };

		private readonly byte[][] ip1 = new byte[3][] {
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFE, (byte) 0xFF },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFF, (byte) 0xFF },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFF, (byte) 0x00 } };

		private readonly byte[][] ip2 = new byte[3][] {
			new byte[] { (byte) 192, (byte) 168, 0, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFC, 3 },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFF, (byte) 0xFF },
			new byte[] { (byte) 192, (byte) 168, 0, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFF, (byte) 0x00 } };

		private readonly byte[][] ipintersect = new byte[3][] {
			new byte[] { (byte) 192, (byte) 168, 0, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFE, (byte) 0xFF },
			new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
						(byte) 0xFF, (byte) 0xFF }, null };

		private readonly byte[][][] ipunion = new byte[3][][] {
			new byte[2][] {
							new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
											(byte) 0xFE, (byte) 0xFF },
							new byte[] { (byte) 192, (byte) 168, 0, 1, (byte) 0xFF, (byte) 0xFF,
											(byte) 0xFC, 3 } },
			new byte[1][] {
							new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
											(byte) 0xFF, (byte) 0xFF } },
			new byte[2][] {
							new byte[] { (byte) 192, (byte) 168, 1, 1, (byte) 0xFF, (byte) 0xFF,
											(byte) 0xFF, (byte) 0x00 },
							new byte[] { (byte) 192, (byte) 168, 0, 1, (byte) 0xFF, (byte) 0xFF,
											(byte) 0xFF, (byte) 0x00 } } };

		public override string Name
		{
			get { return "PkixNameConstraintsTest"; }
		}

		public override void PerformTest()
		{
			TestConstraints(GeneralName.Rfc822Name, testEmail,
				testEmailIsConstraint, testEmailIsNotConstraint, email1, email2,
				emailunion, emailintersect);
			TestConstraints(GeneralName.DnsName, testDNS, testDNSIsConstraint,
				testDNSIsNotConstraint, dns1, dns2, dnsunion, dnsintersect);
			TestConstraints(GeneralName.DirectoryName, testDN, testDNIsConstraint,
				testDNIsNotConstraint, dn1, dn2, dnUnion, dnIntersection);
			TestConstraints(GeneralName.UniformResourceIdentifier, testURI,
				testURIIsConstraint, testURIIsNotConstraint, uri1, uri2, uriunion,
				uriintersect);
			TestConstraints(GeneralName.IPAddress, testIP, testIPIsConstraint,
				testIPIsNotConstraint, ip1, ip2, ipunion, ipintersect);
		}

		/**
		 * Tests string based GeneralNames for inclusion or exclusion.
		 * 
		 * @param nameType The {@link GeneralName} type to test.
		 * @param testName The name to test.
		 * @param testNameIsConstraint The names where <code>testName</code> must
		 *            be included and excluded.
		 * @param testNameIsNotConstraint The names where <code>testName</code>
		 *            must not be excluded and included.
		 * @param testNames1 Operand 1 of test names to use for union and
		 *            intersection testing.
		 * @param testNames2 Operand 2 of test names to use for union and
		 *            intersection testing.
		 * @param testUnion The union results.
		 * @param testInterSection The intersection results.
		 * @throws Exception If an unexpected exception occurs.
		 */
		private void TestConstraints(
			int			nameType,
			string		testName,
			string[]	testNameIsConstraint,
			string[]	testNameIsNotConstraint,
			string[]	testNames1,
			string[]	testNames2,
			string[][]	testUnion,
			string[]	testInterSection)
		{
			for (int i = 0; i < testNameIsConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, testNameIsConstraint[i]))));
				constraintValidator.checkPermitted(new GeneralName(nameType, testName));
			}
			for (int i = 0; i < testNameIsNotConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, testNameIsNotConstraint[i]))));
				try
				{
					constraintValidator.checkPermitted(new GeneralName(nameType, testName));
					Fail("not permitted name allowed: " + nameType);
				}
				catch (PkixNameConstraintValidatorException)
				{
					// expected
				}
			}
			for (int i = 0; i < testNameIsConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, testNameIsConstraint[i])));
				try
				{
					constraintValidator.checkExcluded(new GeneralName(nameType, testName));
					Fail("excluded name missed: " + nameType);
				}
				catch (PkixNameConstraintValidatorException)
				{
					// expected
				}
			}
			for (int i = 0; i < testNameIsNotConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, testNameIsNotConstraint[i])));
				constraintValidator.checkExcluded(new GeneralName(nameType, testName));
			}
			for (int i = 0; i < testNames1.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, testNames1[i])));
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, testNames2[i])));
				PkixNameConstraintValidator constraints2 = new PkixNameConstraintValidator();
				for (int j = 0; j < testUnion[i].Length; j++)
				{
					constraints2.AddExcludedSubtree(new GeneralSubtree(
						new GeneralName(nameType, testUnion[i][j])));
				}
				if (!constraints2.Equals(constraintValidator))
				{
					Fail("union wrong: " + nameType);
				}
				constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, testNames1[i]))));
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, testNames2[i]))));
				constraints2 = new PkixNameConstraintValidator();
				if (testInterSection[i] != null)
				{
					constraints2.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
						new GeneralName(nameType, testInterSection[i]))));
				}
				else
				{
					constraints2.IntersectEmptyPermittedSubtree(nameType);
				}
				if (!constraints2.Equals(constraintValidator))
				{
					Fail("intersection wrong: " + nameType);
				}
			}
		}

		/**
		 * Tests byte array based GeneralNames for inclusion or exclusion.
		 * 
		 * @param nameType The {@link GeneralName} type to test.
		 * @param testName The name to test.
		 * @param testNameIsConstraint The names where <code>testName</code> must
		 *            be included and excluded.
		 * @param testNameIsNotConstraint The names where <code>testName</code>
		 *            must not be excluded and included.
		 * @param testNames1 Operand 1 of test names to use for union and
		 *            intersection testing.
		 * @param testNames2 Operand 2 of test names to use for union and
		 *            intersection testing.
		 * @param testUnion The union results.
		 * @param testInterSection The intersection results.
		 * @throws Exception If an unexpected exception occurs.
		 */
		private void TestConstraints(
			int nameType,
			byte[] testName,
			byte[][] testNameIsConstraint,
			byte[][] testNameIsNotConstraint,
			byte[][] testNames1,
			byte[][] testNames2,
			byte[][][] testUnion,
			byte[][] testInterSection)
		{
			for (int i = 0; i < testNameIsConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, new DerOctetString(
					testNameIsConstraint[i])))));
				constraintValidator.checkPermitted(new GeneralName(nameType,
					new DerOctetString(testName)));
			}
			for (int i = 0; i < testNameIsNotConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, new DerOctetString(
					testNameIsNotConstraint[i])))));
				try
				{
					constraintValidator.checkPermitted(new GeneralName(nameType,
						new DerOctetString(testName)));
					Fail("not permitted name allowed: " + nameType);
				}
				catch (PkixNameConstraintValidatorException)
				{
					// expected
				}
			}
			for (int i = 0; i < testNameIsConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, new DerOctetString(testNameIsConstraint[i]))));
				try
				{
					constraintValidator.checkExcluded(new GeneralName(nameType,
						new DerOctetString(testName)));
					Fail("excluded name missed: " + nameType);
				}
				catch (PkixNameConstraintValidatorException)
				{
					// expected
				}
			}
			for (int i = 0; i < testNameIsNotConstraint.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, new DerOctetString(testNameIsNotConstraint[i]))));
				constraintValidator.checkExcluded(new GeneralName(nameType,
					new DerOctetString(testName)));
			}
			for (int i = 0; i < testNames1.Length; i++)
			{
				PkixNameConstraintValidator constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, new DerOctetString(testNames1[i]))));
				constraintValidator.AddExcludedSubtree(new GeneralSubtree(new GeneralName(
					nameType, new DerOctetString(testNames2[i]))));
				PkixNameConstraintValidator constraints2 = new PkixNameConstraintValidator();
				for (int j = 0; j < testUnion[i].Length; j++)
				{
					constraints2.AddExcludedSubtree(new GeneralSubtree(
						new GeneralName(nameType, new DerOctetString(
						testUnion[i][j]))));
				}
				if (!constraints2.Equals(constraintValidator))
				{
					Fail("union wrong: " + nameType);
				}
				constraintValidator = new PkixNameConstraintValidator();
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, new DerOctetString(testNames1[i])))));
				constraintValidator.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
					new GeneralName(nameType, new DerOctetString(testNames2[i])))));
				constraints2 = new PkixNameConstraintValidator();
				if (testInterSection[i] != null)
				{
					constraints2.IntersectPermittedSubtree(new DerSequence(new GeneralSubtree(
						new GeneralName(nameType, new DerOctetString(
						testInterSection[i])))));
				}
				else
				{
					constraints2.IntersectEmptyPermittedSubtree(nameType);
				}

				if (!constraints2.Equals(constraintValidator))
				{
					Fail("intersection wrong: " + nameType);
				}
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PkixNameConstraintsTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
