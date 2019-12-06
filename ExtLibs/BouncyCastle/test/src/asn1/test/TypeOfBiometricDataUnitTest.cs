using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class TypeOfBiometricDataUnitTest
        : SimpleTest
    {
        public override string Name
        {
			get { return "TypeOfBiometricData"; }
        }

		public override void PerformTest()
        {
            //
            // predefined
            //
            CheckPredefinedType(TypeOfBiometricData.Picture);

            CheckPredefinedType(TypeOfBiometricData.HandwrittenSignature);

			//
            // non-predefined
            //
            DerObjectIdentifier localType = new DerObjectIdentifier("1.1");

			TypeOfBiometricData type = new TypeOfBiometricData(localType);

			CheckNonPredefined(type, localType);

            type = TypeOfBiometricData.GetInstance(type);

            CheckNonPredefined(type, localType);

            Asn1Object obj = type.ToAsn1Object();

            type = TypeOfBiometricData.GetInstance(obj);

            CheckNonPredefined(type, localType);

            type = TypeOfBiometricData.GetInstance(null);

            if (type != null)
            {
                Fail("null GetInstance() failed.");
            }

            try
            {
                TypeOfBiometricData.GetInstance(new object());

                Fail("GetInstance() failed to detect bad object.");
            }
            catch (ArgumentException)
            {
                // expected
            }

            try
            {
                new TypeOfBiometricData(100);

                Fail("constructor failed to detect bad predefined type.");
            }
            catch (ArgumentException)
            {
                // expected
            }

			// Call Equals to avoid unreachable code warning
            if (!Equals(TypeOfBiometricData.Picture, 0))
            {
                Fail("predefined picture should be 0");
            }

			// Call Equals to avoid unreachable code warning
			if (!Equals(TypeOfBiometricData.HandwrittenSignature, 1))
			{
                Fail("predefined handwritten signature should be 1");
            }
        }

		private void CheckPredefinedType(
            int predefinedType)
        {
            TypeOfBiometricData type = new TypeOfBiometricData(predefinedType);

            CheckPredefined(type, predefinedType);

            type = TypeOfBiometricData.GetInstance(type);

			CheckPredefined(type, predefinedType);

			Asn1Object obj = Asn1Object.FromByteArray(type.ToAsn1Object().GetEncoded());

			type = TypeOfBiometricData.GetInstance(obj);

			CheckPredefined(type, predefinedType);
        }

		private void CheckPredefined(
            TypeOfBiometricData	type,
            int					val)
        {
            if (!type.IsPredefined)
            {
                Fail("predefined type expected but not found.");
            }

			if (type.PredefinedBiometricType != val)
            {
                Fail("predefined type does not match.");
            }
        }

		private void CheckNonPredefined(
            TypeOfBiometricData type,
            DerObjectIdentifier val)
        {
            if (type.IsPredefined)
            {
                Fail("predefined type found when not expected.");
            }

			if (!type.BiometricDataOid.Equals(val))
            {
                Fail("data oid does not match.");
            }
        }

		public static void Main(
            string[] args)
        {
            RunTest(new TypeOfBiometricDataUnitTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
