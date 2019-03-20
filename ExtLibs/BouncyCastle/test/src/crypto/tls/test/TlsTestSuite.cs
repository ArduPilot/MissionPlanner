using System;
using System.Collections;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class TlsTestSuite
    {
        // Make the access to constants less verbose 
        internal class C : TlsTestConfig {}

        public TlsTestSuite()
        {
        }

        public static IEnumerable Suite()
        {
            IList testSuite = new ArrayList();

            AddFallbackTests(testSuite);
            AddVersionTests(testSuite, ProtocolVersion.SSLv3);
            AddVersionTests(testSuite, ProtocolVersion.TLSv10);
            AddVersionTests(testSuite, ProtocolVersion.TLSv11);
            AddVersionTests(testSuite, ProtocolVersion.TLSv12);

            return testSuite;
        }

        private static void AddFallbackTests(IList testSuite)
        {
            {
                TlsTestConfig c = CreateTlsTestConfig(ProtocolVersion.TLSv12);
                c.clientFallback = true;

                AddTestCase(testSuite, c, "FallbackGood");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(ProtocolVersion.TLSv12);
                c.clientOfferVersion = ProtocolVersion.TLSv11;
                c.clientFallback = true;
                c.ExpectServerFatalAlert(AlertDescription.inappropriate_fallback);

                AddTestCase(testSuite, c, "FallbackBad");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(ProtocolVersion.TLSv12);
                c.clientOfferVersion = ProtocolVersion.TLSv11;

                AddTestCase(testSuite, c, "FallbackNone");
            }
        }

        private static void AddVersionTests(IList testSuite, ProtocolVersion version)
        {
            string prefix = version.ToString()
                .Replace(" ", "")
                .Replace("\\", "")
                .Replace(".", "")
                + "_";

            {
                TlsTestConfig c = CreateTlsTestConfig(version);

                AddTestCase(testSuite, c, prefix + "GoodDefault");
            }

            /*
             * Server only declares support for SHA1/RSA, client selects MD5/RSA. Since the client is
             * NOT actually tracking MD5 over the handshake, we expect fatal alert from the client.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_VALID;
                c.clientAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.md5, SignatureAlgorithm.rsa);
                c.serverCertReqSigAlgs = TlsUtilities.GetDefaultRsaSignatureAlgorithms();
                c.ExpectClientFatalAlert(AlertDescription.internal_error);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifyHashAlg");
            }

            /*
             * Server only declares support for SHA1/ECDSA, client selects SHA1/RSA. Since the client is
             * actually tracking SHA1 over the handshake, we expect fatal alert to come from the server
             * when it verifies the selected algorithm against the CertificateRequest supported
             * algorithms.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_VALID;
                c.clientAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.rsa);
                c.serverCertReqSigAlgs = TlsUtilities.GetDefaultECDsaSignatureAlgorithms();
                c.ExpectServerFatalAlert(AlertDescription.illegal_parameter);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifySigAlg");
            }

            /*
             * Server only declares support for SHA1/ECDSA, client signs with SHA1/RSA, but sends
             * SHA1/ECDSA in the CertificateVerify. Since the client is actually tracking SHA1 over the
             * handshake, and the claimed algorithm is in the CertificateRequest supported algorithms,
             * we expect fatal alert to come from the server when it finds the claimed algorithm
             * doesn't match the client certificate.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_VALID;
                c.clientAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.rsa);
                c.clientAuthSigAlgClaimed = new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.ecdsa);
                c.serverCertReqSigAlgs = TlsUtilities.GetDefaultECDsaSignatureAlgorithms();
                c.ExpectServerFatalAlert(AlertDescription.decrypt_error);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifySigAlgMismatch");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_INVALID_VERIFY;
                c.ExpectServerFatalAlert(AlertDescription.decrypt_error);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifySignature");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_INVALID_CERT;
                c.ExpectServerFatalAlert(AlertDescription.bad_certificate);

                AddTestCase(testSuite, c, prefix + "BadClientCertificate");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_NONE;
                c.serverCertReq = C.SERVER_CERT_REQ_MANDATORY;
                c.ExpectServerFatalAlert(AlertDescription.handshake_failure);

                AddTestCase(testSuite, c, prefix + "BadMandatoryCertReqDeclined");
            }

            /*
             * Server selects MD5/RSA for ServerKeyExchange signature, which is not in the default
             * supported signature algorithms that the client sent. We expect fatal alert from the
             * client when it verifies the selected algorithm against the supported algorithms.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.serverAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.md5, SignatureAlgorithm.rsa);
                c.ExpectClientFatalAlert(AlertDescription.illegal_parameter);

                AddTestCase(testSuite, c, prefix + "BadServerKeyExchangeSigAlg");
            }

            /*
             * Server selects MD5/RSA for ServerKeyExchange signature, which is not the default {sha1,rsa}
             * implied by the absent signature_algorithms extension. We expect fatal alert from the
             * client when it verifies the selected algorithm against the implicit default.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientSendSignatureAlgorithms = false;
                c.serverAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.md5, SignatureAlgorithm.rsa);
                c.ExpectClientFatalAlert(AlertDescription.illegal_parameter);

                AddTestCase(testSuite, c, prefix + "BadServerKeyExchangeSigAlg2");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.serverCertReq = C.SERVER_CERT_REQ_NONE;

                AddTestCase(testSuite, c, prefix + "GoodNoCertReq");
            }

            {
                TlsTestConfig c = CreateTlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_NONE;

                AddTestCase(testSuite, c, prefix + "GoodOptionalCertReqDeclined");
            }
        }

        private static void AddTestCase(IList testSuite, TlsTestConfig config, string name)
        {
            testSuite.Add(new TestCaseData(config).SetName(name));
        }

        private static TlsTestConfig CreateTlsTestConfig(ProtocolVersion version)
        {
            TlsTestConfig c = new TlsTestConfig();
            c.clientMinimumVersion = ProtocolVersion.SSLv3;
            c.clientOfferVersion = ProtocolVersion.TLSv12;
            c.serverMaximumVersion = version;
            c.serverMinimumVersion = ProtocolVersion.SSLv3;
            return c;
        }

        public static void RunTests()
        {
            foreach (TestCaseData data in Suite())
            {
                Console.WriteLine(data.TestName);
                new TlsTestCase().RunTest((TlsTestConfig)data.Arguments[0]);
            }
        }
    }
}
