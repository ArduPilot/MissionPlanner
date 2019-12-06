using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    /**
     * Test the reading and writing of EncryptedPrivateKeyInfo objects using
     * the test vectors provided at
     * <a href="http://www.rsasecurity.com/rsalabs/pkcs/pkcs-5/index.html">
     * RSA's Pkcs5 Page</a>.
     * <br/>
     * The vectors are Base 64 encoded and encrypted using the password "password"
     * (without quotes). They should all yield the same PrivateKeyInfo object.
     */
    [TestFixture]
    public class EncryptedPrivateKeyInfoTest
        : ITest
    {
		static byte[] sample1 = Base64.Decode(
			  "MIIBozA9BgkqhkiG9w0BBQ0wMDAbBgkqhkiG9w0BBQwwDgQIfWBDXwLp4K4CAggA"
			+ "MBEGBSsOAwIHBAiaCF/AvOgQ6QSCAWDWX4BdAzCRNSQSANSuNsT5X8mWYO27mr3Y"
			+ "9c9LoBVXGNmYWKA77MI4967f7SmjNcgXj3xNE/jmnVz6hhsjS8E5VPT3kfyVkpdZ"
			+ "0lr5e9Yk2m3JWpPU7++v5zBkZmC4V/MwV/XuIs6U+vykgzMgpxQg0oZKS9zgmiZo"
			+ "f/4dOCL0UtCDnyOSvqT7mCVIcMDIEKu8QbVlgZYBop08l60EuEU3gARUo8WsYQmO"
			+ "Dz/ldx0Z+znIT0SXVuOwc+RVItC5T/Qx+aijmmpt+9l14nmaGBrEkmuhmtdvU/4v"
			+ "aptewGRgmjOfD6cqK+zs0O5NrrJ3P/6ZSxXj91CQgrThGfOv72bUncXEMNtc8pks"
			+ "2jpHFjGMdKufnadAD7XuMgzkkaklEXZ4f5tU6heIIwr51g0GBEGF96gYPFnjnSQM"
			+ "75JE02Clo+DfcfXpcybPTwwFg2jd6JTTOfkdf6OdSlA/1XNK43FA");

		static byte[] sample2 = Base64.Decode(
			  "MIIBpjBABgkqhkiG9w0BBQ0wMzAbBgkqhkiG9w0BBQwwDgQIeFeOWl1jywYCAggA"
			+ "MBQGCCqGSIb3DQMHBAjUJ5eGBhQGtQSCAWBrHrRgqO8UUMLcWzZEtpk1l3mjxiF/"
			+ "koCMkHsFwowgyWhEbgIkTgbSViK54LVK8PskekcGNLph+rB6bGZ7pPbL5pbXASJ8"
			+ "+MkQcG3FZdlS4Ek9tTJDApj3O1UubZGFG4uvTlJJFbF1BOJ3MkY3XQ9Gl1qwv7j5"
			+ "6e103Da7Cq9+oIDKmznza78XXQYrUsPo8mJGjUxPskEYlzwvHjKubRnYm/K6RKhi"
			+ "5f4zX4BQ/Dt3H812ZjRXrsjAJP0KrD/jyD/jCT7zNBVPH1izBds+RwizyQAHwfNJ"
			+ "BFR78TH4cgzB619X47FDVOnT0LqQNVd0O3cSwnPrXE9XR3tPayE+iOB15llFSmi8"
			+ "z0ByOXldEpkezCn92Umk++suzIVj1qfsK+bv2phZWJPbLEIWPDRHUbYf76q5ArAr"
			+ "u4xtxT/hoK3krEs/IN3d70qjlUJ36SEw1UaZ82PWhakQbdtu39ZraMJB");

		static byte[] sample3 = Base64.Decode(
			  "MIIBrjBIBgkqhkiG9w0BBQ0wOzAeBgkqhkiG9w0BBQwwEQQIrHyQPBZqWLUCAggA"
			+ "AgEQMBkGCCqGSIb3DQMCMA0CAToECEhbh7YZKiPSBIIBYCT1zp6o5jpFlIkgwPop"
			+ "7bW1+8ACr4exqzkeb3WflQ8cWJ4cURxzVdvxUnXeW1VJdaQZtjS/QHs5GhPTG/0f"
			+ "wtvnaPfwrIJ3FeGaZfcg2CrYhalOFmEb4xrE4KyoEQmUN8tb/Cg94uzd16BOPw21"
			+ "RDnE8bnPdIGY7TyL95kbkqH23mK53pi7h+xWIgduW+atIqDyyt55f7WMZcvDvlj6"
			+ "VpN/V0h+qxBHL274WA4dj6GYgeyUFpi60HdGCK7By2TBy8h1ZvKGjmB9h8jZvkx1"
			+ "MkbRumXxyFsowTZawyYvO8Um6lbfEDP9zIEUq0IV8RqH2MRyblsPNSikyYhxX/cz"
			+ "tdDxRKhilySbSBg5Kr8OfcwKp9bpinN96nmG4xr3Tch1bnVvqJzOQ5+Vva2WwVvH"
			+ "2JkWvYm5WaANg4Q6bRxu9vz7DuhbJjQdZbxFezIAgrJdSe92B00jO/0Kny1WjiVO"
			+ "6DA=");

		public string Name
		{
			get { return "EncryptedPrivateKeyInfoTest"; }
		}

        private ITestResult DoTest(
            int		id,
            byte[]	sample)
        {
            EncryptedPrivateKeyInfo info;
            try
            {
				info = EncryptedPrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(sample));
            }
            catch (Exception e)
            {
                return new SimpleTestResult(false, Name + ": test " + id + " failed construction - exception "
                    + e.ToString());
            }

			byte[] bytes;
            try
            {
            	bytes = info.GetDerEncoded();
            }
            catch (Exception e)
            {
                return new SimpleTestResult(false,
                    Name + ": test " + id + " failed writing - exception " + e.ToString());
            }

            if (!Arrays.AreEqual(bytes, sample))
            {
                try
                {
                    Asn1Object obj = Asn1Object.FromByteArray(bytes);

                    return new SimpleTestResult(false, Name + ": test " + id
                        + " length mismatch - expected " + sample.Length + SimpleTest.NewLine
                        + Asn1Dump.DumpAsString(info) + " got " + bytes.Length + SimpleTest.NewLine
                        + Asn1Dump.DumpAsString(obj));
                }
                catch (Exception e)
                {
                    return new SimpleTestResult(false, Name + ": test " + id + " data mismatch - exception " + e.ToString());
                }
            }

            return new SimpleTestResult(true, Name + ": test " + id + " Okay");
        }

        public ITestResult Perform()
        {
            ITestResult  result = DoTest(0, sample1);
            if (!result.IsSuccessful())
            {
                return result;
            }

            result = DoTest(1, sample2);
            if (!result.IsSuccessful())
            {
                return result;
            }

            result = DoTest(2, sample3);
            if (!result.IsSuccessful())
            {
                return result;
            }

            return new SimpleTestResult(true, Name + ": Okay");
        }

		public static void Main(
			string[] args)
		{
			ITest test = new EncryptedPrivateKeyInfoTest();
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
