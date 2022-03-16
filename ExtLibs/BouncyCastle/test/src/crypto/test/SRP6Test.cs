using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class Srp6Test
		: SimpleTest
	{
	    private static BigInteger FromHex(string hex)
	    {
	        return new BigInteger(1, Hex.Decode(hex));
	    }

        private readonly SecureRandom random = new SecureRandom();

	    public override string Name
	    {
	        get { return "SRP6"; }
	    }

	    public override void PerformTest()
	    {
	    	rfc5054AppendixBTestVectors();

            testMutualVerification(Srp6StandardGroups.rfc5054_1024);
            testClientCatchesBadB(Srp6StandardGroups.rfc5054_1024);
            testServerCatchesBadA(Srp6StandardGroups.rfc5054_1024);

			testWithRandomParams(256);
			testWithRandomParams(384);
			testWithRandomParams(512);
	    }

	    private void rfc5054AppendixBTestVectors()
	    {
	    	byte[] I = Encoding.UTF8.GetBytes("alice");
	    	byte[] P = Encoding.UTF8.GetBytes("password123");
	    	byte[] s = Hex.Decode("BEB25379D1A8581EB5A727673A2441EE");
            BigInteger N = Srp6StandardGroups.rfc5054_1024.N;
            BigInteger g = Srp6StandardGroups.rfc5054_1024.G;
	    	BigInteger a = FromHex("60975527035CF2AD1989806F0407210BC81EDC04E2762A56AFD529DDDA2D4393");
	    	BigInteger b = FromHex("E487CB59D31AC550471E81F00F6928E01DDA08E974A004F49E61F5D105284D20");

	    	BigInteger expect_k = FromHex("7556AA045AEF2CDD07ABAF0F665C3E818913186F");
	    	BigInteger expect_x = FromHex("94B7555AABE9127CC58CCF4993DB6CF84D16C124");
	    	BigInteger expect_v = FromHex("7E273DE8696FFC4F4E337D05B4B375BEB0DDE1569E8FA00A9886D812"
	            + "9BADA1F1822223CA1A605B530E379BA4729FDC59F105B4787E5186F5"
	            + "C671085A1447B52A48CF1970B4FB6F8400BBF4CEBFBB168152E08AB5"
	            + "EA53D15C1AFF87B2B9DA6E04E058AD51CC72BFC9033B564E26480D78"
	            + "E955A5E29E7AB245DB2BE315E2099AFB");
	    	BigInteger expect_A = FromHex("61D5E490F6F1B79547B0704C436F523DD0E560F0C64115BB72557EC4"
	            + "4352E8903211C04692272D8B2D1A5358A2CF1B6E0BFCF99F921530EC"
	            + "8E39356179EAE45E42BA92AEACED825171E1E8B9AF6D9C03E1327F44"
	            + "BE087EF06530E69F66615261EEF54073CA11CF5858F0EDFDFE15EFEA"
	            + "B349EF5D76988A3672FAC47B0769447B");
	    	BigInteger expect_B = FromHex("BD0C61512C692C0CB6D041FA01BB152D4916A1E77AF46AE105393011"
	            + "BAF38964DC46A0670DD125B95A981652236F99D9B681CBF87837EC99"
	            + "6C6DA04453728610D0C6DDB58B318885D7D82C7F8DEB75CE7BD4FBAA"
	            + "37089E6F9C6059F388838E7A00030B331EB76840910440B1B27AAEAE"
	            + "EB4012B7D7665238A8E3FB004B117B58");
	    	BigInteger expect_u = FromHex("CE38B9593487DA98554ED47D70A7AE5F462EF019");
	    	BigInteger expect_S = FromHex("B0DC82BABCF30674AE450C0287745E7990A3381F63B387AAF271A10D"
	            + "233861E359B48220F7C4693C9AE12B0A6F67809F0876E2D013800D6C"
	            + "41BB59B6D5979B5C00A172B4A2A5903A0BDCAF8A709585EB2AFAFA8F"
	            + "3499B200210DCC1F10EB33943CD67FC88A2F39A4BE5BEC4EC0A3212D"
	            + "C346D7E474B29EDE8A469FFECA686E5A");

	    	BigInteger k = Srp6Utilities.CalculateK(new Sha1Digest(), N, g);
	    	if (!k.Equals(expect_k))
	    	{
	    		Fail("wrong value of 'k'");
	    	}

	    	BigInteger x = Srp6Utilities.CalculateX(new Sha1Digest(), N, s, I, P);
	    	if (!x.Equals(expect_x))
	    	{
	    		Fail("wrong value of 'x'");
	    	}

	    	Srp6VerifierGenerator gen = new Srp6VerifierGenerator();
	    	gen.Init(N, g, new Sha1Digest());
	    	BigInteger v = gen.GenerateVerifier(s, I, P);
	    	if (!v.Equals(expect_v))
	    	{
	    		Fail("wrong value of 'v'");
	    	}

	        Srp6Client client = new MySrp6Client(a);
	        client.Init(N, g, new Sha1Digest(), random);

	    	BigInteger A = client.GenerateClientCredentials(s, I, P);
	    	if (!A.Equals(expect_A))
	    	{
	    		Fail("wrong value of 'A'");
	    	}

	    	Srp6Server server = new MySrp6Server(b);
	        server.Init(N, g, v, new Sha1Digest(), random);

	    	BigInteger B = server.GenerateServerCredentials();
	    	if (!B.Equals(expect_B))
	    	{
	    		Fail("wrong value of 'B'");
	    	}

	        BigInteger u = Srp6Utilities.CalculateU(new Sha1Digest(), N, A, B);
	    	if (!u.Equals(expect_u))
	    	{
	    		Fail("wrong value of 'u'");
	    	}

	        BigInteger clientS = client.CalculateSecret(B);
	        if (!clientS.Equals(expect_S))
	    	{
	    		Fail("wrong value of 'S' (client)");
	    	}

	        BigInteger serverS = server.CalculateSecret(A);
	        if (!serverS.Equals(expect_S))
	    	{
	    		Fail("wrong value of 'S' (server)");
	    	}
	    }

		private void testWithRandomParams(int bits)
		{
	        DHParametersGenerator paramGen = new DHParametersGenerator();
	        paramGen.Init(bits, 25, random);
	        DHParameters parameters = paramGen.GenerateParameters();

            testMutualVerification(new Srp6GroupParameters(parameters.P, parameters.G));
		}

        private void testMutualVerification(Srp6GroupParameters group)
	    {
	        byte[] I = Encoding.UTF8.GetBytes("username");
	        byte[] P = Encoding.UTF8.GetBytes("password");
	        byte[] s = new byte[16];
	        random.NextBytes(s);

	        Srp6VerifierGenerator gen = new Srp6VerifierGenerator();
	        gen.Init(group, new Sha256Digest());
	        BigInteger v = gen.GenerateVerifier(s, I, P);

	        Srp6Client client = new Srp6Client();
	        client.Init(group, new Sha256Digest(), random);

	        Srp6Server server = new Srp6Server();
	        server.Init(group, v, new Sha256Digest(), random);

            BigInteger A = client.GenerateClientCredentials(s, I, P);
	        BigInteger B = server.GenerateServerCredentials();

	        BigInteger clientS = client.CalculateSecret(B);
	        BigInteger serverS = server.CalculateSecret(A);

	        if (!clientS.Equals(serverS))
	        {
	            Fail("SRP agreement failed - client/server calculated different secrets");
	        }
	    }

        private void testClientCatchesBadB(Srp6GroupParameters group)
	    {
	        byte[] I = Encoding.UTF8.GetBytes("username");
	        byte[] P = Encoding.UTF8.GetBytes("password");
	        byte[] s = new byte[16];
	        random.NextBytes(s);

	        Srp6Client client = new Srp6Client();
	        client.Init(group, new Sha256Digest(), random);

	        client.GenerateClientCredentials(s, I, P);

	        try
	        {
	        	client.CalculateSecret(BigInteger.Zero);
	        	Fail("Client failed to detect invalid value for 'B'");
	        }
	        catch (CryptoException)
	        {
	        	// Expected
	        }

	        try
	        {
	        	client.CalculateSecret(group.N);
	        	Fail("Client failed to detect invalid value for 'B'");
	        }
	        catch (CryptoException)
	        {
	        	// Expected
	        }
	    }

        private void testServerCatchesBadA(Srp6GroupParameters group)
	    {
	        byte[] I = Encoding.UTF8.GetBytes("username");
	        byte[] P = Encoding.UTF8.GetBytes("password");
	        byte[] s = new byte[16];
	        random.NextBytes(s);

	        Srp6VerifierGenerator gen = new Srp6VerifierGenerator();
	        gen.Init(group, new Sha256Digest());
	        BigInteger v = gen.GenerateVerifier(s, I, P);

	        Srp6Server server = new Srp6Server();
	        server.Init(group, v, new Sha256Digest(), random);

	        server.GenerateServerCredentials();

	        try
	        {
	        	server.CalculateSecret(BigInteger.Zero);
	        	Fail("Client failed to detect invalid value for 'A'");
	        }
	        catch (CryptoException)
	        {
	        	// Expected
	        }

	        try
	        {
	        	server.CalculateSecret(group.N);
	        	Fail("Client failed to detect invalid value for 'A'");
	        }
	        catch (CryptoException)
	        {
	        	// Expected
	        }
	    }

	    public static void Main(string[] args)
	    {
	        RunTest(new Srp6Test());
	    }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();
            Assert.AreEqual(Name + ": Okay", resultText);
        }

		private class MySrp6Client
			: Srp6Client
		{
			private readonly BigInteger nonRandomPrivA;

			internal MySrp6Client(BigInteger nonRandomPrivA)
			{
				this.nonRandomPrivA = nonRandomPrivA;
			}

            protected override BigInteger SelectPrivateValue()
            {
                return nonRandomPrivA;
            }
		}

		private class MySrp6Server
			: Srp6Server
		{
			private readonly BigInteger nonRandomPrivB;

			internal MySrp6Server(BigInteger nonRandomPrivB)
			{
				this.nonRandomPrivB = nonRandomPrivB;
			}

            protected override BigInteger SelectPrivateValue()
            {
                return nonRandomPrivB;
            }
		}
	}
}
