using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class CertPathTest
		: SimpleTest
	{
		internal static readonly byte[] rootCertBin = Base64.Decode(
			"MIIBqzCCARQCAQEwDQYJKoZIhvcNAQEFBQAwHjEcMBoGA1UEAxMTVGVzdCBDQSBDZXJ0aWZpY2F0ZTAeFw0wODA5MDQwNDQ1MDhaFw0wODA5MTEwNDQ1MDhaMB4xHDAaBgNVBAMTE1Rlc3QgQ0EgQ2VydGlmaWNhdGUwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAMRLUjhPe4YUdLo6EcjKcWUOG7CydFTH53Pr1lWjOkbmszYDpkhCTT9LOsI+disk18nkBxSl8DAHTqV+VxtuTPt64iyi10YxyDeep+DwZG/f8cVQv97U3hA9cLurZ2CofkMLGr6JpSGCMZ9FcstcTdHB4lbErIJ54YqfF4pNOs4/AgMBAAEwDQYJKoZIhvcNAQEFBQADgYEAgyrTEFY7ALpeY59jL6xFOLpuPqoBOWrUWv6O+zy5BCU0qiX71r3BpigtxRj+DYcfLIM9FNERDoHu3TthD3nwYWUBtFX8N0QUJIdJabxqAMhLjSC744koiFpCYse5Ye3ZvEdFwDzgAQsJTp5eFGgTZPkPzcdhkFJ2p9+OWs+cb24=");
		internal static readonly byte[] interCertBin = Base64.Decode(
			"MIICSzCCAbSgAwIBAgIBATANBgkqhkiG9w0BAQUFADAeMRwwGgYDVQQDExNUZXN0IENBIENlcnRpZmljYXRlMB4XDTA4MDkwNDA0NDUwOFoXDTA4MDkxMTA0NDUwOFowKDEmMCQGA1UEAxMdVGVzdCBJbnRlcm1lZGlhdGUgQ2VydGlmaWNhdGUwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAISS9OOZ2wxzdWny9aVvk4Joq+dwSJ+oqvHUxX3PflZyuiLiCBUOUE4q59dGKdtNX5fIfwyK3cpV0e73Y/0fwfM3m9rOWFrCKOhfeswNTes0w/2PqPVVDDsF/nj7NApuqXwioeQlgTL251RDF4sVoxXqAU7lRkcqwZt3mwqS4KTJAgMBAAGjgY4wgYswRgYDVR0jBD8wPYAUhv8BOT27EB9JaCccJD4YASPP5XWhIqQgMB4xHDAaBgNVBAMTE1Rlc3QgQ0EgQ2VydGlmaWNhdGWCAQEwHQYDVR0OBBYEFL/IwAGOkHzaQyPZegy79CwM5oTFMBIGA1UdEwEB/wQIMAYBAf8CAQAwDgYDVR0PAQH/BAQDAgGGMA0GCSqGSIb3DQEBBQUAA4GBAE4TRgUz4sUvZyVdZxqV+XyNRnqXAeLOOqFGYv2D96tQrS+zjd0elVlT6lFrtchZdOmmX7R6/H/tjMWMcTBICZyRYrvK8cCAmDOI+EIdq5p6lj2Oq6Pbw/wruojAqNrpaR6IkwNpWtdOSSupv4IJL+YU9q2YFTh4R1j3tOkPoFGr");
		internal static readonly byte[] finalCertBin = Base64.Decode(
			"MIICRjCCAa+gAwIBAgIBATANBgkqhkiG9w0BAQUFADAoMSYwJAYDVQQDEx1UZXN0IEludGVybWVkaWF0ZSBDZXJ0aWZpY2F0ZTAeFw0wODA5MDQwNDQ1MDhaFw0wODA5MTEwNDQ1MDhaMB8xHTAbBgNVBAMTFFRlc3QgRW5kIENlcnRpZmljYXRlMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQChpUeo0tPYywWKiLlbWKNJBcCpSaLSlaZ+4+yer1AxI5yJIVHP6SAlBghlbD5Qne5ImnN/15cz1xwYAiul6vGKJkVPlFEe2Mr+g/J/WJPQQPsjbZ1G+vxbAwXEDA4KaQrnpjRZFq+CdKHwOjuPLYS/MYQNgdIvDVEQcTbPQ8GaiQIDAQABo4GIMIGFMEYGA1UdIwQ/MD2AFL/IwAGOkHzaQyPZegy79CwM5oTFoSKkIDAeMRwwGgYDVQQDExNUZXN0IENBIENlcnRpZmljYXRlggEBMB0GA1UdDgQWBBSVkw+VpqBf3zsLc/9GdkK9TzHPwDAMBgNVHRMBAf8EAjAAMA4GA1UdDwEB/wQEAwIFoDANBgkqhkiG9w0BAQUFAAOBgQBLv/0bVDjzTs/y1vN3FUiZNknEbzupIZduTuXJjqv/vBX+LDPjUfu/+iOCXOSKoRn6nlOWhwB1z6taG2usQkFG8InMkRcPREi2uVgFdhJ/1C3dAWhsdlubjdL926bftXvxnx/koDzyrePW5U96RlOQM2qLvbaky2Giz6hrc3Wl+w==");
		internal static readonly byte[] rootCrlBin = Base64.Decode(
			"MIIBYjCBzAIBATANBgkqhkiG9w0BAQsFADAeMRwwGgYDVQQDExNUZXN0IENBIENlcnRpZmljYXRlFw0wODA5MDQwNDQ1MDhaFw0wODA5MDQwNzMxNDhaMCIwIAIBAhcNMDgwOTA0MDQ0NTA4WjAMMAoGA1UdFQQDCgEJoFYwVDBGBgNVHSMEPzA9gBSG/wE5PbsQH0loJxwkPhgBI8/ldaEipCAwHjEcMBoGA1UEAxMTVGVzdCBDQSBDZXJ0aWZpY2F0ZYIBATAKBgNVHRQEAwIBATANBgkqhkiG9w0BAQsFAAOBgQCAbaFCo0BNG4AktVf6jjBLeawP1u0ELYkOCEGvYZE0mBpQ+OvFg7subZ6r3lRIj030nUli28sPFtu5ZQMBNcpE4nS1ziF44RfT3Lp5UgHx9x17Krz781iEyV+7zU8YxYMY9wULD+DCuK294kGKIssVNbmTYXZatBNoXQN5CLIocA==");
		internal static readonly byte[] interCrlBin = Base64.Decode(
			"MIIBbDCB1gIBATANBgkqhkiG9w0BAQsFADAoMSYwJAYDVQQDEx1UZXN0IEludGVybWVkaWF0ZSBDZXJ0aWZpY2F0ZRcNMDgwOTA0MDQ0NTA4WhcNMDgwOTA0MDczMTQ4WjAiMCACAQIXDTA4MDkwNDA0NDUwOFowDDAKBgNVHRUEAwoBCaBWMFQwRgYDVR0jBD8wPYAUv8jAAY6QfNpDI9l6DLv0LAzmhMWhIqQgMB4xHDAaBgNVBAMTE1Rlc3QgQ0EgQ2VydGlmaWNhdGWCAQEwCgYDVR0UBAMCAQEwDQYJKoZIhvcNAQELBQADgYEAEVCr5TKs5yguGgLH+dBzmSPoeSIWJFLsgWwJEit/iUDJH3dgYmaczOcGxIDtbYYHLWIHM+P2YRyQz3MEkCXEgm/cx4y7leAmux5l+xQWgmxFPz+197vaphPeCZo+B7V1CWtm518gcq4mrs9ovfgNqgyFj7KGjcBpWdJE32KMt50=");

		/*
		 * certpath with a circular reference
		 */
		internal static readonly byte[] certA = Base64.Decode(
			"MIIC6jCCAlOgAwIBAgIBBTANBgkqhkiG9w0BAQUFADCBjTEPMA0GA1UEAxMGSW50"
			+ "ZXIzMQswCQYDVQQGEwJDSDEPMA0GA1UEBxMGWnVyaWNoMQswCQYDVQQIEwJaSDEX"
			+ "MBUGA1UEChMOUHJpdmFzcGhlcmUgQUcxEDAOBgNVBAsTB1Rlc3RpbmcxJDAiBgkq"
			+ "hkiG9w0BCQEWFWFybWluQHByaXZhc3BoZXJlLmNvbTAeFw0wNzA0MDIwODQ2NTda"
			+ "Fw0xNzAzMzAwODQ0MDBaMIGlMScwJQYDVQQDHh4AQQByAG0AaQBuACAASADkAGIA"
			+ "ZQByAGwAaQBuAGcxCzAJBgNVBAYTAkNIMQ8wDQYDVQQHEwZadXJpY2gxCzAJBgNV"
			+ "BAgTAlpIMRcwFQYDVQQKEw5Qcml2YXNwaGVyZSBBRzEQMA4GA1UECxMHVGVzdGlu"
			+ "ZzEkMCIGCSqGSIb3DQEJARYVYXJtaW5AcHJpdmFzcGhlcmUuY29tMIGfMA0GCSqG"
			+ "SIb3DQEBAQUAA4GNADCBiQKBgQCfHfyVs5dbxG35H/Thd29qR4NZU88taCu/OWA1"
			+ "GdACI02lXWYpmLWiDgnU0ULP+GG8OnVp1IES9fz2zcrXKQ19xZzsen/To3h5sNte"
			+ "cJpS00XMM24q/jDwy5NvkBP9YIfFKQ1E/0hFHXcqwlw+b/y/v6YGsZCU2h6QDzc4"
			+ "5m0+BwIDAQABo0AwPjAMBgNVHRMBAf8EAjAAMA4GA1UdDwEB/wQEAwIE8DAeBglg"
			+ "hkgBhvhCAQ0EERYPeGNhIGNlcnRpZmljYXRlMA0GCSqGSIb3DQEBBQUAA4GBAJEu"
			+ "KiSfIwsY7SfobMLrv2v/BtLhGLi4RnmjiwzBhuv5rn4rRfBpq1ppmqQMJ2pmA67v"
			+ "UWCY+mNwuyjHyivpCCyJGsZ9d5H09g2vqxzkDBMz7X9VNMZYFH8j/R3/Cfvqks31"
			+ "z0OFslJkeKLa1I0P/dfVHsRKNkLRT3Ws5LKksErQ");

		internal static readonly byte[] certB = Base64.Decode(
			"MIICtTCCAh6gAwIBAgIBBDANBgkqhkiG9w0BAQQFADCBjTEPMA0GA1UEAxMGSW50"
			+ "ZXIyMQswCQYDVQQGEwJDSDEPMA0GA1UEBxMGWnVyaWNoMQswCQYDVQQIEwJaSDEX"
			+ "MBUGA1UEChMOUHJpdmFzcGhlcmUgQUcxEDAOBgNVBAsTB1Rlc3RpbmcxJDAiBgkq"
			+ "hkiG9w0BCQEWFWFybWluQHByaXZhc3BoZXJlLmNvbTAeFw0wNzA0MDIwODQ2Mzha"
			+ "Fw0xNzAzMzAwODQ0MDBaMIGNMQ8wDQYDVQQDEwZJbnRlcjMxCzAJBgNVBAYTAkNI"
			+ "MQ8wDQYDVQQHEwZadXJpY2gxCzAJBgNVBAgTAlpIMRcwFQYDVQQKEw5Qcml2YXNw"
			+ "aGVyZSBBRzEQMA4GA1UECxMHVGVzdGluZzEkMCIGCSqGSIb3DQEJARYVYXJtaW5A"
			+ "cHJpdmFzcGhlcmUuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCxCXIB"
			+ "QRnmVvl2h7Q+0SsRxDLnyM1dJG9jMa+UCCmHy0k/ZHs5VirSbjEJSjkQ9BGeh9SC"
			+ "7JwbMpXO7UE+gcVc2RnWUY+MA+fWIeTV4KtkYA8WPu8wVGCXbN8wwh/StOocszxb"
			+ "g+iLvGeh8CYSRqg6QN3S/02etH3o8H4e7Z0PZwIDAQABoyMwITAPBgNVHRMBAf8E"
			+ "BTADAQH/MA4GA1UdDwEB/wQEAwIB9jANBgkqhkiG9w0BAQQFAAOBgQCtWdirSsmt"
			+ "+CBBCNn6ZnbU3QqQfiiQIomjenNEHESJgaS/+PvPE5i3xWFXsunTHLW321/Km16I"
			+ "7+ZvT8Su1cqHg79NAT8QB0yke1saKSy2C0Pic4HwrNqVBWFNSxMU0hQzpx/ZXDbZ"
			+ "DqIXAp5EfyRYBy2ul+jm6Rot6aFgzuopKg==");

		internal static readonly byte[] certC = Base64.Decode(
			"MIICtTCCAh6gAwIBAgIBAjANBgkqhkiG9w0BAQQFADCBjTEPMA0GA1UEAxMGSW50"
			+ "ZXIxMQswCQYDVQQGEwJDSDEPMA0GA1UEBxMGWnVyaWNoMQswCQYDVQQIEwJaSDEX"
			+ "MBUGA1UEChMOUHJpdmFzcGhlcmUgQUcxEDAOBgNVBAsTB1Rlc3RpbmcxJDAiBgkq"
			+ "hkiG9w0BCQEWFWFybWluQHByaXZhc3BoZXJlLmNvbTAeFw0wNzA0MDIwODQ0Mzla"
			+ "Fw0xNzAzMzAwODQ0MDBaMIGNMQ8wDQYDVQQDEwZJbnRlcjIxCzAJBgNVBAYTAkNI"
			+ "MQ8wDQYDVQQHEwZadXJpY2gxCzAJBgNVBAgTAlpIMRcwFQYDVQQKEw5Qcml2YXNw"
			+ "aGVyZSBBRzEQMA4GA1UECxMHVGVzdGluZzEkMCIGCSqGSIb3DQEJARYVYXJtaW5A"
			+ "cHJpdmFzcGhlcmUuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQD0rLr6"
			+ "f2/ONeJzTb0q9M/NNX+MnAFMSqiQGVBkT76u5nOH4KLkpHXkzI82JI7GuQMzoT3a"
			+ "+RP1hO6FneO92ms2soC6xiOFb4EC69Dfhh87Nww5O35JxVF0bzmbmIAWd6P/7zGh"
			+ "nd2S4tKkaZcubps+C0j9Fgi0hipVicAOUVVoDQIDAQABoyMwITAPBgNVHRMBAf8E"
			+ "BTADAQH/MA4GA1UdDwEB/wQEAwIB9jANBgkqhkiG9w0BAQQFAAOBgQCLPvc1IMA4"
			+ "YP+PmnEldyUoRWRnvPWjBGeu0WheBP7fdcnGBf93Nmc5j68ZN+eTZ5VMuZ99YdvH"
			+ "CXGNX6oodONLU//LlFKdLl5xjLAS5X9p1RbOEGytnalqeiEpjk4+C/7rIBG1kllO"
			+ "dItmI6LlEMV09Hkpg6ZRAUmRkb8KrM4X7A==");

		internal static readonly byte[] certD = Base64.Decode(
			"MIICtTCCAh6gAwIBAgIBBjANBgkqhkiG9w0BAQQFADCBjTEPMA0GA1UEAxMGSW50"
			+ "ZXIzMQswCQYDVQQGEwJDSDEPMA0GA1UEBxMGWnVyaWNoMQswCQYDVQQIEwJaSDEX"
			+ "MBUGA1UEChMOUHJpdmFzcGhlcmUgQUcxEDAOBgNVBAsTB1Rlc3RpbmcxJDAiBgkq"
			+ "hkiG9w0BCQEWFWFybWluQHByaXZhc3BoZXJlLmNvbTAeFw0wNzA0MDIwODQ5NTNa"
			+ "Fw0xNzAzMzAwODQ0MDBaMIGNMQ8wDQYDVQQDEwZJbnRlcjExCzAJBgNVBAYTAkNI"
			+ "MQ8wDQYDVQQHEwZadXJpY2gxCzAJBgNVBAgTAlpIMRcwFQYDVQQKEw5Qcml2YXNw"
			+ "aGVyZSBBRzEQMA4GA1UECxMHVGVzdGluZzEkMCIGCSqGSIb3DQEJARYVYXJtaW5A"
			+ "cHJpdmFzcGhlcmUuY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCae3TP"
			+ "jIVKeASqvNabaiUHAMGUgFxB7L0yUsIj39azLcLtUj4S7XkDf7SMGtYV0JY1XNaQ"
			+ "sHJAsnJivDZc50oiYvqDYfgFZx5+AsN5l5X5rjRzs/OX+Jo+k1OgsIyu6+mf9Kfb"
			+ "5IdWOVB2EcOg4f9tPjLM8CIj9Pp7RbKLyqUUgwIDAQABoyMwITAPBgNVHRMBAf8E"
			+ "BTADAQH/MA4GA1UdDwEB/wQEAwIB9jANBgkqhkiG9w0BAQQFAAOBgQCgr9kUdWUT"
			+ "Lt9UcztSzR3pnHRsyvS0E/z850OKQKS5/VxLEalpFvhj+3EcZ7Y6mFxaaS2B7vXg"
			+ "2YWyqV1PRb6iF7/u9EXkpSTKGrJahwANirCa3V/HTUuPdCE2GITlnWI8h3eVA+xQ"
			+ "D4LF0PXHOkXbwmhXRSb10lW1bSGkUxE9jg==");

		private void doTestExceptions()
		{
			byte[] enc = { (byte)0, (byte)2, (byte)3, (byte)4, (byte)5 };
//			MyCertPath mc = new MyCertPath(enc);
			MemoryStream os = new MemoryStream();
			MemoryStream ins;
			byte[] arr;

			// TODO Support serialization of cert paths?
//			ObjectOutputStream oos = new ObjectOutputStream(os);
//			oos.WriteObject(mc);
//			oos.Flush();
//			oos.Close();

			try
			{
//				CertificateFactory cFac = CertificateFactory.GetInstance("X.509");
				arr = os.ToArray();
				ins = new MemoryStream(arr, false);
//				cFac.generateCertPath(ins);
				new PkixCertPath(ins);
			}
			catch (CertificateException)
			{
				// ignore okay
			}

//			CertificateFactory cf = CertificateFactory.GetInstance("X.509");
			X509CertificateParser cf = new X509CertificateParser();
			IList certCol = new ArrayList();

			certCol.Add(cf.ReadCertificate(certA));
			certCol.Add(cf.ReadCertificate(certB));
			certCol.Add(cf.ReadCertificate(certC));
			certCol.Add(cf.ReadCertificate(certD));

//			CertPathBuilder pathBuilder = CertPathBuilder.GetInstance("PKIX");
			PkixCertPathBuilder pathBuilder = new PkixCertPathBuilder();
			X509CertStoreSelector select = new X509CertStoreSelector();
			select.Subject = ((X509Certificate)certCol[0]).SubjectDN;

			ISet trustanchors = new HashSet();
			trustanchors.Add(new TrustAnchor(cf.ReadCertificate(rootCertBin), null));

//			CertStore certStore = CertStore.getInstance("Collection", new CollectionCertStoreParameters(certCol));
			IX509Store x509CertStore = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(certCol));

			PkixBuilderParameters parameters = new PkixBuilderParameters(trustanchors, select);
			parameters.AddStore(x509CertStore);

			try
			{
				PkixCertPathBuilderResult result = pathBuilder.Build(parameters);
				PkixCertPath path = result.CertPath;
				Fail("found cert path in circular set");
			}
			catch (PkixCertPathBuilderException)
			{
				// expected
			}
		}

		public override void PerformTest()
		{
			X509CertificateParser cf = new X509CertificateParser();

			X509Certificate rootCert = cf.ReadCertificate(rootCertBin);
			X509Certificate interCert = cf.ReadCertificate(interCertBin);
			X509Certificate finalCert = cf.ReadCertificate(finalCertBin);

			//Testing CertPath generation from List
			IList list = new ArrayList();
			list.Add(interCert);
//			CertPath certPath1 = cf.generateCertPath(list);
			PkixCertPath certPath1 = new PkixCertPath(list);

			//Testing CertPath encoding as PkiPath
			byte[] encoded = certPath1.GetEncoded("PkiPath");

			//Testing CertPath generation from InputStream
			MemoryStream inStream = new MemoryStream(encoded, false);
//			CertPath certPath2 = cf.generateCertPath(inStream, "PkiPath");
			PkixCertPath certPath2 = new PkixCertPath(inStream, "PkiPath");

			//Comparing both CertPathes
			if (!certPath2.Equals(certPath1))
			{
				Fail("CertPath differ after encoding and decoding.");
			}

			encoded = certPath1.GetEncoded("PKCS7");

			//Testing CertPath generation from InputStream
			inStream = new MemoryStream(encoded, false);
//			certPath2 = cf.generateCertPath(inStream, "PKCS7");
			certPath2 = new PkixCertPath(inStream, "PKCS7");

			//Comparing both CertPathes
			if (!certPath2.Equals(certPath1))
			{
				Fail("CertPath differ after encoding and decoding.");
			}

			encoded = certPath1.GetEncoded("PEM");

			//Testing CertPath generation from InputStream
			inStream = new MemoryStream(encoded, false);
//			certPath2 = cf.generateCertPath(inStream, "PEM");
			certPath2 = new PkixCertPath(inStream, "PEM");

			//Comparing both CertPathes
			if (!certPath2.Equals(certPath1))
			{
				Fail("CertPath differ after encoding and decoding.");
			}

			//
			// empty list test
			//
			list = new ArrayList();

//			CertPath certPath = CertificateFactory.GetInstance("X.509","BC").generateCertPath(list);
			PkixCertPath certPath = new PkixCertPath(list);
			if (certPath.Certificates.Count != 0)
			{
				Fail("list wrong size.");
			}

			//
			// exception tests
			//
			doTestExceptions();
		}

		public override string Name
		{
			get { return "CertPath"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new CertPathTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}

//		private class MyCertificate : X509Certificate
//		{
//			private readonly string type;
//			private readonly byte[] encoding;
//
//			public MyCertificate(string type, byte[] encoding)
////				: base(type)
//			{
//				this.type = type;
//
//				// don't copy to allow null parameter in test
//				this.encoding = encoding;
//			}
//
//			public override byte[] GetEncoded()
//			{
//				// do copy to force NPE in test
//				return (byte[])encoding.Clone();
//			}
//
//			public override void Verify(AsymmetricKeyParameter publicKey)
//			{
//			}
//
//			public override string ToString()
//			{
//				return "[My test Certificate, type: " + type + "]";
//			}
//
//			public override AsymmetricKeyParameter GetPublicKey()
//			{
//				throw new NotImplementedException();
//
////            return new PublicKey()
////            {
////                public string getAlgorithm()
////                {
////                    return "TEST";
////                }
////
////                public byte[] getEncoded()
////                {
////                    return new byte[] { (byte)1, (byte)2, (byte)3 };
////                }
////
////                public string getFormat()
////                {
////                    return "TEST_FORMAT";
////                }
////            };
//			}
//		}

//		private class MyCertPath : PkixCertPath
//		{
//			private readonly ArrayList certificates;
//
//			private readonly ArrayList encodingNames;
//
//			private readonly byte[] encoding;
//
//			public MyCertPath(byte[] encoding)
//				: base("MyEncoding")
//			{
//				this.encoding = encoding;
//				certificates = new ArrayList();
//				certificates.Add(new MyCertificate("MyEncoding", encoding));
//				encodingNames = new ArrayList();
//				encodingNames.Add("MyEncoding");
//			}
//
//			public override IList Certificates
//			{
//				get { return CollectionUtilities.ReadOnly(certificates); }
//			}
//
//			public override byte[] GetEncoded()
//			{
//				return (byte[])encoding.Clone();
//			}
//
//			public override byte[] GetEncoded(
//				string encoding)
//			{
//				if (Type.Equals(encoding))
//				{
//					return (byte[])this.encoding.Clone();
//				}
//				throw new CertificateEncodingException("Encoding not supported: "
//					+ encoding);
//			}
//
//			public override IEnumerable Encodings
//			{
//				get { return new EnumerableProxy(encodingNames); }
//			}
//		}
	}
}
