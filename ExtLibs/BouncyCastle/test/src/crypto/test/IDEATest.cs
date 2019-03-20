using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class IdeaTest
        : CipherTest
    {
        public override string Name
        {
            get { return "IDEA"; }
        }

        internal static SimpleTest[] tests = new SimpleTest[]
        {
            new BlockCipherVectorTest(0, new IdeaEngine(),
                new KeyParameter(Hex.Decode("00112233445566778899AABBCCDDEEFF")),
                "000102030405060708090a0b0c0d0e0f",
                "ed732271a7b39f475b4b2b6719f194bf"),
            new BlockCipherVectorTest(0, new IdeaEngine(),
                new KeyParameter(Hex.Decode("00112233445566778899AABBCCDDEEFF")),
                "f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff",
                "b8bc6ed5c899265d2bcfad1fc6d4287d")
        };

        public IdeaTest()
            : base(tests, new IdeaEngine(), new KeyParameter(new byte[32]))
        {
        }

        public static void Main(
            string[] args)
        {
            ITest test = new IdeaTest();
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
