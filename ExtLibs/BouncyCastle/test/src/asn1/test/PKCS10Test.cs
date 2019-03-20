using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    [TestFixture]
    public class Pkcs10Test
        : ITest
    {
        byte[]    req1 = Base64.Decode(
                    "MIHoMIGTAgEAMC4xDjAMBgNVBAMTBVRlc3QyMQ8wDQYDVQQKEwZBbmFUb20xCzAJBgNVBAYTAlNF"
                +   "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALlEt31Tzt2MlcOljvacJgzQVhmlMoqAOgqJ9Pgd3Gux"
                +   "Z7/WcIlgW4QCB7WZT21O1YoghwBhPDMcNGrHei9kHQkCAwEAAaAAMA0GCSqGSIb3DQEBBQUAA0EA"
                +   "NDEI4ecNtJ3uHwGGlitNFq9WxcoZ0djbQJ5hABMotav6gtqlrwKXY2evaIrsNwkJtNdwwH18aQDU"
                +   "KCjOuBL38Q==");

        byte[]    req2 = Base64.Decode(
                   "MIIB6TCCAVICAQAwgagxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpDYWxpZm9ybmlhMRQwEgYDVQQH"
                +  "EwtTYW50YSBDbGFyYTEMMAoGA1UEChMDQUJCMVEwTwYDVQQLHEhQAAAAAAAAAG8AAAAAAAAAdwAA"
                +  "AAAAAABlAAAAAAAAAHIAAAAAAAAAIAAAAAAAAABUAAAAAAAAABxIAAAAAAAARAAAAAAAAAAxDTAL"
                +  "BgNVBAMTBGJsdWUwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBANETRZ+6occCOrFxNhfKIp4C"
                +  "mMkxwhBNb7TnnahpbM9O0r4hrBPcfYuL7u9YX/jN0YNUP+/CiT39HhSe/bikaBPDEyNsl988I8vX"
                +  "piEdgxYq/+LTgGHbjRsRYCkPtmzwBbuBldNF8bV7pu0v4UScSsExmGqqDlX1TbPU8KkPU1iTAgMB"
                +  "AAGgADANBgkqhkiG9w0BAQQFAAOBgQAFbrs9qUwh93CtETk7DeUD5HcdCnxauo1bck44snSV6MZV"
                +  "OCIGaYu1501kmhEvAtVVRr6SEHwimfQDDIjnrWwYsEr/DT6tkTZAbfRd3qUu3iKjT0H0vlUZp0hJ"
                +  "66mINtBM84uZFBfoXiWY8M3FuAnGmvy6ah/dYtJorTxLKiGkew==");

        public string Name
        {
			get
			{
				return "Pkcs10";
			}
        }

        public ITestResult BasicPkcs10Test(
            string  testName,
            byte[]  req)
        {
            try
            {
				CertificationRequest r = new CertificationRequest(
					(Asn1Sequence)Asn1Object.FromByteArray(req));
				byte[] bytes = r.GetDerEncoded();

				if (!Arrays.AreEqual(bytes, req))
				{
					return new SimpleTestResult(false, Name + ": " + testName + " failed comparison test");
				}
			}
            catch (Exception e)
            {
                return new SimpleTestResult(false, Name + ": Exception - " + testName + " " + e.ToString());
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

        public ITestResult Perform()
        {
            ITestResult res = BasicPkcs10Test("basic CR", req1);

            if (!res.IsSuccessful())
            {
                return res;
            }

            return BasicPkcs10Test("Universal CR", req2);
        }

        public static void Main(
            string[] args)
        {
            ITest test = new Pkcs10Test();
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
