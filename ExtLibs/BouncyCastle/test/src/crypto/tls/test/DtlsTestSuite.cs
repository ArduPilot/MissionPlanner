using System;
using System.Collections;

using NUnit.Framework;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public class DtlsTestSuite
    {
        // Make the access to constants less verbose 
        internal class C : TlsTestConfig {}

        public DtlsTestSuite()
        {
        }

        public static IEnumerable Suite()
        {
            IList testSuite = new ArrayList();

            AddFallbackTests(testSuite);
            AddVersionTests(testSuite, ProtocolVersion.DTLSv10);
            AddVersionTests(testSuite, ProtocolVersion.DTLSv12);

            return testSuite;
        }

        private static void AddFallbackTests(IList testSuite)
        {
            {
                TlsTestConfig c = CreateDtlsTestConfig(ProtocolVersion.DTLSv12);
                c.clientFallback = true;

                AddTestCase(testSuite, c, "FallbackGood");
            }

            /*
             * NOTE: Temporarily disabled automatic test runs because of problems getting a clean exit
             * of the DTLS server after a fatal alert. As of writing, manual runs show the correct
             * alerts being raised
             */

#if false
            {
                TlsTestConfig c = CreateDtlsTestConfig(ProtocolVersion.DTLSv12);
                c.clientOfferVersion = ProtocolVersion.DTLSv10;
                c.clientFallback = true;
                c.ExpectServerFatalAlert(AlertDescription.inappropriate_fallback);

                AddTestCase(testSuite, c, "FallbackBad");
            }
#endif

            {
                TlsTestConfig c = CreateDtlsTestConfig(ProtocolVersion.DTLSv12);
                c.clientOfferVersion = ProtocolVersion.DTLSv10;

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

            /*
             * NOTE: Temporarily disabled automatic test runs because of problems getting a clean exit
             * of the DTLS server after a fatal alert. As of writing, manual runs show the correct
             * alerts being raised
             */

#if false
            /*
             * Server only declares support for SHA1/RSA, client selects MD5/RSA. Since the client is
             * NOT actually tracking MD5 over the handshake, we expect fatal alert from the client.
             */
            if (TlsUtilities.IsTlsV12(version))
            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
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
                TlsTestConfig c = CreateDtlsTestConfig(version);
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
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_VALID;
                c.clientAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.rsa);
                c.clientAuthSigAlgClaimed = new SignatureAndHashAlgorithm(HashAlgorithm.sha1, SignatureAlgorithm.ecdsa);
                c.serverCertReqSigAlgs = TlsUtilities.GetDefaultECDsaSignatureAlgorithms();
                c.ExpectServerFatalAlert(AlertDescription.decrypt_error);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifySigAlgMismatch");
            }

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_INVALID_VERIFY;
                c.ExpectServerFatalAlert(AlertDescription.decrypt_error);

                AddTestCase(testSuite, c, prefix + "BadCertificateVerifySignature");
            }

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_INVALID_CERT;
                c.ExpectServerFatalAlert(AlertDescription.bad_certificate);

                AddTestCase(testSuite, c, prefix + "BadClientCertificate");
            }

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
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
                TlsTestConfig c = CreateDtlsTestConfig(version);
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
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.clientSendSignatureAlgorithms = false;
                c.serverAuthSigAlg = new SignatureAndHashAlgorithm(HashAlgorithm.md5, SignatureAlgorithm.rsa);
                c.ExpectClientFatalAlert(AlertDescription.illegal_parameter);

                AddTestCaseDebug(testSuite, c, prefix + "BadServerKeyExchangeSigAlg2");
            }
#endif

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);

                AddTestCase(testSuite, c, prefix + "GoodDefault");
            }

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.serverCertReq = C.SERVER_CERT_REQ_NONE;

                AddTestCase(testSuite, c, prefix + "GoodNoCertReq");
            }

            {
                TlsTestConfig c = CreateDtlsTestConfig(version);
                c.clientAuth = C.CLIENT_AUTH_NONE;

                AddTestCase(testSuite, c, prefix + "GoodOptionalCertReqDeclined");
            }
        }

        private static void AddTestCase(IList testSuite, TlsTestConfig config, String name)
        {
            testSuite.Add(new TestCaseData(config).SetName(name));
        }

        private static TlsTestConfig CreateDtlsTestConfig(ProtocolVersion version)
        {
            TlsTestConfig c = new TlsTestConfig();
            c.clientMinimumVersion = ProtocolVersion.DTLSv10;
            c.clientOfferVersion = ProtocolVersion.DTLSv12;
            c.serverMaximumVersion = version;
            c.serverMinimumVersion = ProtocolVersion.DTLSv10;
            return c;
        }

        public static void RunTests()
        {
            foreach (TestCaseData data in Suite())
            {
                Console.WriteLine(data.TestName);
                new DtlsTestCase().RunTest((TlsTestConfig)data.Arguments[0]);
            }
        }
    }
}
