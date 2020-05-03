using System;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
    public class RegressionTest
    {
		// These tests were ported from org.bouncycastle.jce.provider.test in Java build
        public static ITest[] tests = new ITest[]
		{
			new FipsDesTest(),
			new DesEdeTest(),
			new AesTest(),
			new CamelliaTest(),
			new SeedTest(),
			new AesSicTest(),
			new Gost28147Test(),
			new PbeTest(),
			new BlockCipherTest(),
			new MacTest(),
			new HMacTest(),
//			new SealedTest(),
			new RsaTest(),
			new DHTest(),
			new DsaTest(),
//			new ImplicitlyCaTest(),
			new ECNRTest(),
			new ECDsa5Test(),
			new Gost3410Test(),
			new ElGamalTest(),
			new IesTest(),
			new SigTest(),
			new AttrCertTest(),
			new CertTest(),
			new Pkcs10CertRequestTest(),
			new EncryptedPrivateKeyInfoTest(),  // Also in Org.BouncyCastle.Pkcs.Tests
//			new KeyStoreTest(),
//			new Pkcs12StoreTest(), // Already in Org.BouncyCastle.Pkcs.Tests
			new DigestTest(),
			new PssTest(),
			new WrapTest(),
//			new DoFinalTest(),
			new CipherStreamTest(),
			new NamedCurveTest(),
			new PkixTest(),
//			new NetscapeCertRequestTest(),
			new X509StoreTest(),
//			new X509StreamParserTest(),
			new X509CertificatePairTest(),
			new CertPathTest(),
//			new CertStoreTest(),
			new CertPathValidatorTest(),
			new CertPathBuilderTest(),
			new ECEncodingTest(),
//			new AlgorithmParametersTest(),
			new NistCertPathTest(),
			new PkixPolicyMappingTest(),
//			new SlotTwoTest(),
			new PkixNameConstraintsTest(),
			new NoekeonTest(),
			new AttrCertSelectorTest(),
//			new SerialisationTest(),
			new MqvTest(),
			new CMacTest(),
			new Crl5Test(),
		};

		public static void Main(
            string[] args)
        {
            for (int i = 0; i != tests.Length; i++)
            {
                ITestResult result = tests[i].Perform();

				Console.WriteLine(result);
            }
		}
    }
}
