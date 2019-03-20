using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class CertPathBuilderTest
        : SimpleTest
    {
        private void baseTest()
        {
//			CertificateFactory cf = CertificateFactory.getInstance("X.509", "BC");
            X509CertificateParser certParser = new X509CertificateParser();
            X509CrlParser crlParser = new X509CrlParser();

            // initialise CertStore
            X509Certificate rootCert = certParser.ReadCertificate(CertPathTest.rootCertBin);
            X509Certificate interCert = certParser.ReadCertificate(CertPathTest.interCertBin);
            X509Certificate finalCert = certParser.ReadCertificate(CertPathTest.finalCertBin);
            X509Crl rootCrl = crlParser.ReadCrl(CertPathTest.rootCrlBin);
            X509Crl interCrl = crlParser.ReadCrl(CertPathTest.interCrlBin);

            IList certList = new ArrayList();
            certList.Add(rootCert);
            certList.Add(interCert);
            certList.Add(finalCert);

            IList crlList = new ArrayList();
            crlList.Add(rootCrl);
            crlList.Add(interCrl);

//			CollectionCertStoreParameters ccsp = new CollectionCertStoreParameters(list);
//			CertStore store = CertStore.getInstance("Collection", ccsp, "BC");
            IX509Store x509CertStore = X509StoreFactory.Create(
                "Certificate/Collection",
                new X509CollectionStoreParameters(certList));
            IX509Store x509CrlStore = X509StoreFactory.Create(
                "CRL/Collection",
                new X509CollectionStoreParameters(crlList));

            // NB: Month is 1-based in .NET
            //DateTime validDate = new DateTime(2008, 9, 4, 14, 49, 10).ToUniversalTime();
            DateTime validDate = new DateTime(2008, 9, 4, 5, 49, 10);

            //Searching for rootCert by subjectDN without CRL
            ISet trust = new HashSet();
            trust.Add(new TrustAnchor(rootCert, null));

//			CertPathBuilder cpb = CertPathBuilder.getInstance("PKIX","BC");
            PkixCertPathBuilder cpb = new PkixCertPathBuilder();
            X509CertStoreSelector targetConstraints = new X509CertStoreSelector();
            targetConstraints.Subject = finalCert.SubjectDN;
            PkixBuilderParameters parameters = new PkixBuilderParameters(trust, targetConstraints);
//			parameters.addCertStore(store);
            parameters.AddStore(x509CertStore);
            parameters.AddStore(x509CrlStore);
            parameters.Date = new DateTimeObject(validDate);
            PkixCertPathBuilderResult result = cpb.Build(parameters);
            PkixCertPath path = result.CertPath;

            if (path.Certificates.Count != 2)
            {
                Fail("wrong number of certs in baseTest path");
            }
        }

        private void v0Test()
        {
            // create certificates and CRLs
            AsymmetricCipherKeyPair rootPair = TestUtilities.GenerateRsaKeyPair();
            AsymmetricCipherKeyPair interPair = TestUtilities.GenerateRsaKeyPair();
            AsymmetricCipherKeyPair endPair = TestUtilities.GenerateRsaKeyPair();

            X509Certificate rootCert = TestUtilities.GenerateRootCert(rootPair);
            X509Certificate interCert = TestUtilities.GenerateIntermediateCert(interPair.Public, rootPair.Private, rootCert);
            X509Certificate endCert = TestUtilities.GenerateEndEntityCert(endPair.Public, interPair.Private, interCert);

            BigInteger revokedSerialNumber = BigInteger.Two;
            X509Crl rootCRL = TestUtilities.CreateCrl(rootCert, rootPair.Private, revokedSerialNumber);
            X509Crl interCRL = TestUtilities.CreateCrl(interCert, interPair.Private, revokedSerialNumber);

            // create CertStore to support path building
            IList certList = new ArrayList();
            certList.Add(rootCert);
            certList.Add(interCert);
            certList.Add(endCert);

            IList crlList = new ArrayList();
            crlList.Add(rootCRL);
            crlList.Add(interCRL);

//			CollectionCertStoreParameters parameters = new CollectionCertStoreParameters(list);
//			CertStore                     store = CertStore.getInstance("Collection", parameters);
            IX509Store x509CertStore = X509StoreFactory.Create(
                "Certificate/Collection",
                new X509CollectionStoreParameters(certList));
            IX509Store x509CrlStore = X509StoreFactory.Create(
                "CRL/Collection",
                new X509CollectionStoreParameters(crlList));

            ISet trust = new HashSet();
            trust.Add(new TrustAnchor(rootCert, null));

            // build the path
//			CertPathBuilder  builder = CertPathBuilder.getInstance("PKIX", "BC");
            PkixCertPathBuilder builder = new PkixCertPathBuilder();
            X509CertStoreSelector pathConstraints = new X509CertStoreSelector();

            pathConstraints.Subject = endCert.SubjectDN;

            PkixBuilderParameters buildParams = new PkixBuilderParameters(trust, pathConstraints);
//			buildParams.addCertStore(store);
            buildParams.AddStore(x509CertStore);
            buildParams.AddStore(x509CrlStore);

            buildParams.Date = new DateTimeObject(DateTime.UtcNow);

            PkixCertPathBuilderResult result = builder.Build(buildParams);
            PkixCertPath path = result.CertPath;

            if (path.Certificates.Count != 2)
            {
                Fail("wrong number of certs in v0Test path");
            }
        }

        public override void PerformTest()
        {
            baseTest();
            v0Test();
        }
        
        public override string Name
        {
            get { return "CertPathBuilder"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new CertPathBuilderTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
