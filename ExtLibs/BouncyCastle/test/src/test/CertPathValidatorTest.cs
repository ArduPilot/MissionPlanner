using System;
using System.Collections;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class CertPathValidatorTest
        : SimpleTest
    {
        private byte[] AC_PR = Base64.Decode(
            "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlFU1RDQ0F6R2dBd0lC"
            + "QWdJQkJUQU5CZ2txaGtpRzl3MEJBUVVGQURDQnRERUxNQWtHQTFVRUJoTUNR"
            + "bEl4DQpFekFSQmdOVkJBb1RDa2xEVUMxQ2NtRnphV3d4UFRBN0JnTlZCQXNU"
            + "TkVsdWMzUnBkSFYwYnlCT1lXTnBiMjVoDQpiQ0JrWlNCVVpXTnViMnh2WjJs"
            + "aElHUmhJRWx1Wm05eWJXRmpZVzhnTFNCSlZFa3hFVEFQQmdOVkJBY1RDRUp5"
            + "DQpZWE5wYkdsaE1Rc3dDUVlEVlFRSUV3SkVSakV4TUM4R0ExVUVBeE1vUVhW"
            + "MGIzSnBaR0ZrWlNCRFpYSjBhV1pwDQpZMkZrYjNKaElGSmhhWG9nUW5KaGMy"
            + "bHNaV2x5WVRBZUZ3MHdNakEwTURReE9UTTVNREJhRncwd05UQTBNRFF5DQpN"
            + "elU1TURCYU1HRXhDekFKQmdOVkJBWVRBa0pTTVJNd0VRWURWUVFLRXdwSlEx"
            + "QXRRbkpoYzJsc01UMHdPd1lEDQpWUVFERXpSQmRYUnZjbWxrWVdSbElFTmxj"
            + "blJwWm1sallXUnZjbUVnWkdFZ1VISmxjMmxrWlc1amFXRWdaR0VnDQpVbVZ3"
            + "ZFdKc2FXTmhNSUlCSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQVE4QU1JSUJD"
            + "Z0tDQVFFQXMwc0t5NGsrDQp6b016aldyMTQxeTVYQ045UGJMZERFQXN2cjZ4"
            + "Z0NCN1l5bEhIQ1NBYmpGR3dOQ0R5NlVxN1h0VjZ6UHdIMXpGDQpFWENlS3Jm"
            + "UUl5YXBXSEZ4V1VKajBMblFrY1RZM1FOR1huK0JuVk9EVTZDV3M1c3NoZktH"
            + "RXZyVlQ1Z214V1NmDQp4OFlsdDgzY1dwUE1QZzg3VDlCaHVIbHQzazh2M2Ev"
            + "NmRPbmF2dytOYTAyZExBaDBlNzZqcCtQUS9LK0pHZlBuDQphQjVVWURrZkd0"
            + "em5uTTNBV01tY3VJK0o0ek5OMDZaa3ZnbDFsdEo2UU1qcnZEUFlSak9ndDlT"
            + "cklpY1NmbEo4DQptVDdHWGRRaXJnQUNXc3g1QURBSklRK253TU1vNHlyTUtx"
            + "SlFhNFFDMHhhT0QvdkdVcG9SaDQzT0FTZFp3c3YvDQpPWFlybmVJeVAwVCs4"
            + "UUlEQVFBQm80RzNNSUcwTUQwR0ExVWRId1EyTURRd01xQXdvQzZHTEdoMGRI"
            + "QTZMeTloDQpZM0poYVhvdWFXTndZbkpoYzJsc0xtZHZkaTVpY2k5TVExSmhZ"
            + "M0poYVhvdVkzSnNNQklHQTFVZElBUUxNQWt3DQpCd1lGWUV3QkFRRXdIUVlE"
            + "VlIwT0JCWUVGREpUVFlKNE9TWVB5T09KZkVMZXhDaHppK2hiTUI4R0ExVWRJ"
            + "d1FZDQpNQmFBRklyNjhWZUVFUk0xa0VMNlYwbFVhUTJreFBBM01BNEdBMVVk"
            + "RHdFQi93UUVBd0lCQmpBUEJnTlZIUk1CDQpBZjhFQlRBREFRSC9NQTBHQ1Nx"
            + "R1NJYjNEUUVCQlFVQUE0SUJBUUJRUFNoZ1lidnFjaWV2SDVVb3ZMeXhkbkYr"
            + "DQpFcjlOeXF1SWNkMnZ3Y0N1SnpKMkQ3WDBUcWhHQ0JmUEpVVkdBVWorS0NP"
            + "SDFCVkgva1l1OUhsVHB1MGtKWFBwDQpBQlZkb2hJUERqRHhkbjhXcFFSL0Yr"
            + "ejFDaWtVcldIMDR4eTd1N1p6UUpLSlBuR0loY1FpOElyRm1PYkllMEc3DQpY"
            + "WTZPTjdPRUZxY21KTFFHWWdtRzFXMklXcytQd1JwWTdENGhLVEFoVjFSNkVv"
            + "amE1L3BPcmVDL09kZXlQWmVxDQo1SUZTOUZZZk02U0Npd2hrK3l2Q1FHbVo0"
            + "YzE5SjM0ZjVFYkRrK1NQR2tEK25EQ0E3L3VMUWNUMlJURE14SzBaDQpuZlo2"
            + "Nm1Sc0ZjcXRGaWdScjVFcmtKZDdoUVV6eHNOV0VrNzJEVUFIcVgvNlNjeWtt"
            + "SkR2V0plSUpqZlcNCi0tLS0tRU5EIENFUlRJRklDQVRFLS0tLS0NCg==");

        private byte[] AC_RAIZ_ICPBRASIL = Base64.Decode(
            "LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlFdURDQ0E2Q2dBd0lC"
            + "QWdJQkJEQU5CZ2txaGtpRzl3MEJBUVVGQURDQnRERUxNQWtHQTFVRUJoTUNR"
            + "bEl4DQpFekFSQmdOVkJBb1RDa2xEVUMxQ2NtRnphV3d4UFRBN0JnTlZCQXNU"
            + "TkVsdWMzUnBkSFYwYnlCT1lXTnBiMjVoDQpiQ0JrWlNCVVpXTnViMnh2WjJs"
            + "aElHUmhJRWx1Wm05eWJXRmpZVzhnTFNCSlZFa3hFVEFQQmdOVkJBY1RDRUp5"
            + "DQpZWE5wYkdsaE1Rc3dDUVlEVlFRSUV3SkVSakV4TUM4R0ExVUVBeE1vUVhW"
            + "MGIzSnBaR0ZrWlNCRFpYSjBhV1pwDQpZMkZrYjNKaElGSmhhWG9nUW5KaGMy"
            + "bHNaV2x5WVRBZUZ3MHdNVEV4TXpBeE1qVTRNREJhRncweE1URXhNekF5DQpN"
            + "elU1TURCYU1JRzBNUXN3Q1FZRFZRUUdFd0pDVWpFVE1CRUdBMVVFQ2hNS1NV"
            + "TlFMVUp5WVhOcGJERTlNRHNHDQpBMVVFQ3hNMFNXNXpkR2wwZFhSdklFNWhZ"
            + "Mmx2Ym1Gc0lHUmxJRlJsWTI1dmJHOW5hV0VnWkdFZ1NXNW1iM0p0DQpZV05o"
            + "YnlBdElFbFVTVEVSTUE4R0ExVUVCeE1JUW5KaGMybHNhV0V4Q3pBSkJnTlZC"
            + "QWdUQWtSR01URXdMd1lEDQpWUVFERXloQmRYUnZjbWxrWVdSbElFTmxjblJw"
            + "Wm1sallXUnZjbUVnVW1GcGVpQkNjbUZ6YVd4bGFYSmhNSUlCDQpJakFOQmdr"
            + "cWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBd1BNdWR3WC9odm0r"
            + "VWgyYi9sUUFjSFZBDQppc2FtYUxrV2Rrd1A5L1MvdE9LSWdSckw2T3krWklH"
            + "bE9VZGQ2dVl0azlNYS8zcFVwZ2NmTkFqMHZZbTVnc3lqDQpRbzllbXNjK3g2"
            + "bTRWV3drOWlxTVpTQ0s1RVFrQXEvVXQ0bjdLdUxFMStnZGZ0d2RJZ3hmVXNQ"
            + "dDRDeU5yWTUwDQpRVjU3S00yVVQ4eDVycm16RWpyN1RJQ0dwU1VBbDJnVnFl"
            + "NnhhaWkrYm1ZUjFRcm1XYUJTQUc1OUxya3Jqcll0DQpiUmhGYm9VRGUxREsr"
            + "NlQ4czVMNms4Yzhva3BiSHBhOXZlTXp0RFZDOXNQSjYwTVdYaDZhblZLbzFV"
            + "Y0xjYlVSDQp5RWVOdlpuZVZSS0FBVTZvdXdkakR2d2xzYUt5ZEZLd2VkMFRv"
            + "UTQ3Ym1VS2djbSt3VjNlVFJrMzZVT25Ud0lEDQpBUUFCbzRIU01JSFBNRTRH"
            + "QTFVZElBUkhNRVV3UXdZRllFd0JBUUF3T2pBNEJnZ3JCZ0VGQlFjQ0FSWXNh"
            + "SFIwDQpjRG92TDJGamNtRnBlaTVwWTNCaWNtRnphV3d1WjI5MkxtSnlMMFJR"
            + "UTJGamNtRnBlaTV3WkdZd1BRWURWUjBmDQpCRFl3TkRBeW9EQ2dMb1lzYUhS"
            + "MGNEb3ZMMkZqY21GcGVpNXBZM0JpY21GemFXd3VaMjkyTG1KeUwweERVbUZq"
            + "DQpjbUZwZWk1amNtd3dIUVlEVlIwT0JCWUVGSXI2OFZlRUVSTTFrRUw2VjBs"
            + "VWFRMmt4UEEzTUE4R0ExVWRFd0VCDQovd1FGTUFNQkFmOHdEZ1lEVlIwUEFR"
            + "SC9CQVFEQWdFR01BMEdDU3FHU0liM0RRRUJCUVVBQTRJQkFRQVpBNWMxDQpV"
            + "L2hnSWg2T2NnTEFmaUpnRldwdm1EWldxbFYzMC9iSEZwajhpQm9iSlNtNXVE"
            + "cHQ3VGlyWWgxVXhlM2ZRYUdsDQpZakplKzl6ZCtpelBSYkJxWFBWUUEzNEVY"
            + "Y3drNHFwV3VmMWhIcmlXZmRyeDhBY3FTcXI2Q3VRRndTcjc1Rm9zDQpTemx3"
            + "REFEYTcwbVQ3d1pqQW1RaG5aeDJ4SjZ3ZldsVDlWUWZTLy9KWWVJYzdGdWUy"
            + "Sk5MZDAwVU9TTU1haUsvDQp0NzllbktOSEVBMmZ1cEgzdkVpZ2Y1RWg0YlZB"
            + "TjVWb2hyVG02TVk1M3g3WFFaWnIxTUU3YTU1bEZFblNlVDB1DQptbE9BalIy"
            + "bUFidlNNNVg1b1NaTnJtZXRkenlUajJmbENNOENDN01MYWIwa2tkbmdSSWxV"
            + "QkdIRjEvUzVubVBiDQpLKzlBNDZzZDMzb3FLOG44DQotLS0tLUVORCBDRVJU"
            + "SUZJQ0FURS0tLS0tDQo=");

        private byte[] schefer = Base64.Decode(
            "MIIEnDCCBAWgAwIBAgICIPAwDQYJKoZIhvcNAQEEBQAwgcAxCzAJBgNVBAYT"
            + "AkRFMQ8wDQYDVQQIEwZIRVNTRU4xGDAWBgNVBAcTDzY1MDA4IFdpZXNiYWRl"
            + "bjEaMBgGA1UEChMRU0NIVUZBIEhPTERJTkcgQUcxGjAYBgNVBAsTEVNDSFVG"
            + "QSBIT0xESU5HIEFHMSIwIAYDVQQDExlJbnRlcm5ldCBCZW51dHplciBTZXJ2"
            + "aWNlMSowKAYJKoZIhvcNAQkBFht6ZXJ0aWZpa2F0QHNjaHVmYS1vbmxpbmUu"
            + "ZGUwHhcNMDQwMzMwMTEwODAzWhcNMDUwMzMwMTEwODAzWjCBnTELMAkGA1UE"
            + "BhMCREUxCjAIBgNVBAcTASAxIzAhBgNVBAoTGlNIUyBJbmZvcm1hdGlvbnNz"
            + "eXN0ZW1lIEFHMRwwGgYDVQQLExM2MDAvMDU5NDktNjAwLzA1OTQ5MRgwFgYD"
            + "VQQDEw9TY2hldHRlciBTdGVmYW4xJTAjBgkqhkiG9w0BCQEWFlN0ZWZhbi5T"
            + "Y2hldHRlckBzaHMuZGUwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBAJD0"
            + "95Bi76fkAMjJNTGPDiLPHmZXNsmakngDeS0juzKMeJA+TjXFouhYh6QyE4Bl"
            + "Nf18fT4mInlgLefwf4t6meIWbiseeTo7VQdM+YrbXERMx2uHsRcgZMsiMYHM"
            + "kVfYMK3SMJ4nhCmZxrBkoTRed4gXzVA1AA8YjjTqMyyjvt4TAgMBAAGjggHE"
            + "MIIBwDAJBgNVHRMEAjAAMBEGCWCGSAGG+EIBAQQEAwIEsDALBgNVHQ8EBAMC"
            + "BNAwOQYJYIZIAYb4QgENBCwWKlplcnRpZmlrYXQgbnVyIGZ1ZXIgU0NIVUZB"
            + "LU9ubGluZSBndWVsdGlnLjAdBgNVHQ4EFgQUXReirhBfg0Yhf6MsBWoo/nPa"
            + "hGwwge0GA1UdIwSB5TCB4oAUf2UyCaBV9JUeG9lS1Yo6OFBUdEKhgcakgcMw"
            + "gcAxCzAJBgNVBAYTAkRFMQ8wDQYDVQQIEwZIRVNTRU4xGDAWBgNVBAcTDzY1"
            + "MDA4IFdpZXNiYWRlbjEaMBgGA1UEChMRU0NIVUZBIEhPTERJTkcgQUcxGjAY"
            + "BgNVBAsTEVNDSFVGQSBIT0xESU5HIEFHMSIwIAYDVQQDExlJbnRlcm5ldCBC"
            + "ZW51dHplciBTZXJ2aWNlMSowKAYJKoZIhvcNAQkBFht6ZXJ0aWZpa2F0QHNj"
            + "aHVmYS1vbmxpbmUuZGWCAQAwIQYDVR0RBBowGIEWU3RlZmFuLlNjaGV0dGVy"
            + "QHNocy5kZTAmBgNVHRIEHzAdgRt6ZXJ0aWZpa2F0QHNjaHVmYS1vbmxpbmUu"
            + "ZGUwDQYJKoZIhvcNAQEEBQADgYEAWzZtN9XQ9uyrFXqSy3hViYwV751+XZr0"
            + "YH5IFhIS+9ixNAu8orP3bxqTaMhpwoU7T/oSsyGGSkb3fhzclgUADbA2lrOI"
            + "GkeB/m+FArTwRbwpqhCNTwZywOp0eDosgPjCX1t53BB/m/2EYkRiYdDGsot0"
            + "kQPOVGSjQSQ4+/D+TM8=");

        public override void PerformTest()
        {
            X509CertificateParser certParser = new X509CertificateParser();
            X509CrlParser crlParser = new X509CrlParser();

            // initialise CertStore
            X509Certificate rootCert = certParser.ReadCertificate(CertPathTest.rootCertBin);
            X509Certificate interCert = certParser.ReadCertificate(CertPathTest.interCertBin);
            X509Certificate finalCert = certParser.ReadCertificate(CertPathTest.finalCertBin);
            X509Crl rootCrl = crlParser.ReadCrl(CertPathTest.rootCrlBin);
            X509Crl interCrl = crlParser.ReadCrl(CertPathTest.interCrlBin);

            IList x509Certs = new ArrayList();
            x509Certs.Add(rootCert);
            x509Certs.Add(interCert);
            x509Certs.Add(finalCert);

            IList x509Crls = new ArrayList();
            x509Crls.Add(rootCrl);
            x509Crls.Add(interCrl);

//			CollectionCertStoreParameters ccsp = new CollectionCertStoreParameters(list);
//			CertStore store = CertStore.GetInstance("Collection", ccsp);
//			X509CollectionStoreParameters ccsp = new X509CollectionStoreParameters(list);
            IX509Store x509CertStore = X509StoreFactory.Create(
                "Certificate/Collection",
                new X509CollectionStoreParameters(x509Certs));
            IX509Store x509CrlStore = X509StoreFactory.Create(
                "CRL/Collection",
                new X509CollectionStoreParameters(x509Crls));

            // NB: Month is 1-based in .NET
            //DateTime validDate = new DateTime(2008,9,4,14,49,10).ToUniversalTime();
            DateTime validDate = new DateTime(2008, 9, 4, 5, 49, 10);

            //validating path
            IList certchain = new ArrayList();
            certchain.Add(finalCert);
            certchain.Add(interCert);

//			CertPath cp = CertificateFactory.GetInstance("X.509").GenerateCertPath(certchain);
            PkixCertPath cp = new PkixCertPath(certchain);
            ISet trust = new HashSet();
            trust.Add(new TrustAnchor(rootCert, null));

//			CertPathValidator cpv = CertPathValidator.GetInstance("PKIX");
            PkixCertPathValidator cpv = new PkixCertPathValidator();
            PkixParameters param = new PkixParameters(trust);
            param.AddStore(x509CertStore);
            param.AddStore(x509CrlStore);
            param.Date = new DateTimeObject(validDate);
            MyChecker checker = new MyChecker();
            param.AddCertPathChecker(checker);

            PkixCertPathValidatorResult result = (PkixCertPathValidatorResult)cpv.Validate(cp, param);
            PkixPolicyNode policyTree = result.PolicyTree;
            AsymmetricKeyParameter subjectPublicKey = result.SubjectPublicKey;

            if (checker.GetCount() != 2)
            {
                Fail("checker not evaluated for each certificate");
            }
            
            if (!subjectPublicKey.Equals(finalCert.GetPublicKey()))
            {
                Fail("wrong public key returned");
            }

            IsTrue(result.TrustAnchor.TrustedCert.Equals(rootCert));

            // try a path with trust anchor included.
            certchain.Clear();
            certchain.Add(finalCert);
            certchain.Add(interCert);
            certchain.Add(rootCert);

            cp = new PkixCertPath(certchain);

            cpv = new PkixCertPathValidator();
            param = new PkixParameters(trust);
            param.AddStore(x509CertStore);
            param.AddStore(x509CrlStore);
            param.Date = new DateTimeObject(validDate);
            checker = new MyChecker();
            param.AddCertPathChecker(checker);

            result = (PkixCertPathValidatorResult)cpv.Validate(cp, param);

            IsTrue(result.TrustAnchor.TrustedCert.Equals(rootCert));

            //
            // invalid path containing a valid one test
            //
            try
            {
                // initialise CertStore
                rootCert = certParser.ReadCertificate(AC_RAIZ_ICPBRASIL);
                interCert = certParser.ReadCertificate(AC_PR);
                finalCert = certParser.ReadCertificate(schefer);

                x509Certs = new ArrayList();
                x509Certs.Add(rootCert);
                x509Certs.Add(interCert);
                x509Certs.Add(finalCert);

//				ccsp = new CollectionCertStoreParameters(list);
//				store = CertStore.GetInstance("Collection", ccsp);
//				ccsp = new X509CollectionStoreParameters(list);
                x509CertStore = X509StoreFactory.Create(
                    "Certificate/Collection",
                    new X509CollectionStoreParameters(x509Certs));

                // NB: Month is 1-based in .NET
                //validDate = new DateTime(2004,3,21,2,21,10).ToUniversalTime();
                validDate = new DateTime(2004, 3, 20, 19, 21, 10);

                //validating path
                certchain = new ArrayList();
                certchain.Add(finalCert);
                certchain.Add(interCert);

//				cp = CertificateFactory.GetInstance("X.509").GenerateCertPath(certchain);
                cp = new PkixCertPath(certchain);
                trust = new HashSet();
                trust.Add(new TrustAnchor(rootCert, null));

//				cpv = CertPathValidator.GetInstance("PKIX");
                cpv = new PkixCertPathValidator();
                param = new PkixParameters(trust);
                param.AddStore(x509CertStore);
                param.IsRevocationEnabled = false;
                param.Date = new DateTimeObject(validDate);

                result =(PkixCertPathValidatorResult) cpv.Validate(cp, param);
                policyTree = result.PolicyTree;
                subjectPublicKey = result.SubjectPublicKey;

                Fail("Invalid path validated");
            }
            catch (Exception e)
            {
                if (e is PkixCertPathValidatorException 
                    && e.Message.StartsWith("Could not validate certificate signature."))
                {
                    return;
                }
                Fail("unexpected exception", e);
            }
        }

        public override string Name
        {
            get { return "CertPathValidator"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new CertPathValidatorTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        private class MyChecker
            : PkixCertPathChecker
        {
            private static int count;

            public override void Init(bool forward)
            {
            }

            public override bool IsForwardCheckingSupported()
            {
                return true;
            }

            public override ISet GetSupportedExtensions()
            {
                return null;
            }

            public override void Check(X509Certificate cert, ISet unresolvedCritExts)
            {
                count++;
            }

            public int GetCount()
            {
                return count;
            }
        }
    }
}
