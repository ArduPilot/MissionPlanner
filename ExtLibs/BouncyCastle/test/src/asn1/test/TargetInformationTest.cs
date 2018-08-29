using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class TargetInformationTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "TargetInformation"; }
		}

		public override void PerformTest()
		{
			Target[] targets = new Target[2];
			Target targetName = new Target(Target.Choice.Name, new GeneralName(GeneralName.DnsName, "www.test.com"));
			Target targetGroup = new Target(Target.Choice.Group, new GeneralName(GeneralName.DirectoryName, "o=Test, ou=Test"));
			targets[0] = targetName;
			targets[1] = targetGroup;
			Targets targetss = new Targets(targets);
			TargetInformation targetInformation1 = new TargetInformation(targetss);
			// use an Target array
			TargetInformation targetInformation2 = new TargetInformation(targets);
			// targetInformation1 and targetInformation2 must have same
			// encoding.
			if (!targetInformation1.Equals(targetInformation2))
			{
				Fail("targetInformation1 and targetInformation2 should have the same encoding.");
			}
			TargetInformation targetInformation3 = TargetInformation.GetInstance(targetInformation1.ToAsn1Object());
			TargetInformation targetInformation4 = TargetInformation.GetInstance(targetInformation2.ToAsn1Object());
			if (!targetInformation3.Equals(targetInformation4))
			{
				Fail("targetInformation3 and targetInformation4 should have the same encoding.");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new TargetInformationTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
