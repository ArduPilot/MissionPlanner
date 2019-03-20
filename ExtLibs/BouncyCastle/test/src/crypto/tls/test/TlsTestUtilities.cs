using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    public abstract class TlsTestUtilities
    {
        internal static readonly byte[] RsaCertData = Base64
            .Decode("MIICUzCCAf2gAwIBAgIBATANBgkqhkiG9w0BAQQFADCBjzELMAkGA1UEBhMCQVUxKDAmBgNVBAoMH1RoZSBMZWdpb2"
                + "4gb2YgdGhlIEJvdW5jeSBDYXN0bGUxEjAQBgNVBAcMCU1lbGJvdXJuZTERMA8GA1UECAwIVmljdG9yaWExLzAtBgkq"
                + "hkiG9w0BCQEWIGZlZWRiYWNrLWNyeXB0b0Bib3VuY3ljYXN0bGUub3JnMB4XDTEzMDIyNTA2MDIwNVoXDTEzMDIyNT"
                + "A2MDM0NVowgY8xCzAJBgNVBAYTAkFVMSgwJgYDVQQKDB9UaGUgTGVnaW9uIG9mIHRoZSBCb3VuY3kgQ2FzdGxlMRIw"
                + "EAYDVQQHDAlNZWxib3VybmUxETAPBgNVBAgMCFZpY3RvcmlhMS8wLQYJKoZIhvcNAQkBFiBmZWVkYmFjay1jcnlwdG"
                + "9AYm91bmN5Y2FzdGxlLm9yZzBaMA0GCSqGSIb3DQEBAQUAA0kAMEYCQQC0p+RhcFdPFqlwgrIr5YtqKmKXmEGb4Shy"
                + "pL26Ymz66ZAPdqv7EhOdzl3lZWT6srZUMWWgQMYGiHQg4z2R7X7XAgERo0QwQjAOBgNVHQ8BAf8EBAMCBSAwEgYDVR"
                + "0lAQH/BAgwBgYEVR0lADAcBgNVHREBAf8EEjAQgQ50ZXN0QHRlc3QudGVzdDANBgkqhkiG9w0BAQQFAANBAHU55Ncz"
                + "eglREcTg54YLUlGWu2WOYWhit/iM1eeq8Kivro7q98eW52jTuMI3CI5ulqd0hYzshQKQaZ5GDzErMyM=");

        internal static readonly byte[] DudRsaCertData = Base64
            .Decode("MIICUzCCAf2gAwIBAgIBATANBgkqhkiG9w0BAQQFADCBjzELMAkGA1UEBhMCQVUxKDAmBgNVBAoMH1RoZSBMZWdpb2"
                + "4gb2YgdGhlIEJvdW5jeSBDYXN0bGUxEjAQBgNVBAcMCU1lbGJvdXJuZTERMA8GA1UECAwIVmljdG9yaWExLzAtBgkq"
                + "hkiG9w0BCQEWIGZlZWRiYWNrLWNyeXB0b0Bib3VuY3ljYXN0bGUub3JnMB4XDTEzMDIyNTA1NDcyOFoXDTEzMDIyNT"
                + "A1NDkwOFowgY8xCzAJBgNVBAYTAkFVMSgwJgYDVQQKDB9UaGUgTGVnaW9uIG9mIHRoZSBCb3VuY3kgQ2FzdGxlMRIw"
                + "EAYDVQQHDAlNZWxib3VybmUxETAPBgNVBAgMCFZpY3RvcmlhMS8wLQYJKoZIhvcNAQkBFiBmZWVkYmFjay1jcnlwdG"
                + "9AYm91bmN5Y2FzdGxlLm9yZzBaMA0GCSqGSIb3DQEBAQUAA0kAMEYCQQC0p+RhcFdPFqlwgrIr5YtqKmKXmEGb4Shy"
                + "pL26Ymz66ZAPdqv7EhOdzl3lZWT6srZUMWWgQMYGiHQg4z2R7X7XAgERo0QwQjAOBgNVHQ8BAf8EBAMCAAEwEgYDVR"
                + "0lAQH/BAgwBgYEVR0lADAcBgNVHREBAf8EEjAQgQ50ZXN0QHRlc3QudGVzdDANBgkqhkiG9w0BAQQFAANBAJg55PBS"
                + "weg6obRUKF4FF6fCrWFi6oCYSQ99LWcAeupc5BofW5MstFMhCOaEucuGVqunwT5G7/DweazzCIrSzB0=");

        internal static string Fingerprint(X509CertificateStructure c)
        {
            byte[] der = c.GetEncoded();
            byte[] sha1 = Sha256DigestOf(der);
            byte[] hexBytes = Hex.Encode(sha1);
            string hex = Encoding.ASCII.GetString(hexBytes).ToUpper(CultureInfo.InvariantCulture);

            StringBuilder fp = new StringBuilder();
            int i = 0;
            fp.Append(hex.Substring(i, 2));
            while ((i += 2) < hex.Length)
            {
                fp.Append(':');
                fp.Append(hex.Substring(i, 2));
            }
            return fp.ToString();
        }

        internal static byte[] Sha256DigestOf(byte[] input)
        {
            return DigestUtilities.CalculateDigest("SHA256", input);
        }

        internal static TlsAgreementCredentials LoadAgreementCredentials(TlsContext context,
            string[] certResources, string keyResource)
        {
            Certificate certificate = LoadCertificateChain(certResources);
            AsymmetricKeyParameter privateKey = LoadPrivateKeyResource(keyResource);

            return new DefaultTlsAgreementCredentials(certificate, privateKey);
        }

        internal static TlsEncryptionCredentials LoadEncryptionCredentials(TlsContext context,
            string[] certResources, string keyResource)
        {
            Certificate certificate = LoadCertificateChain(certResources);
            AsymmetricKeyParameter privateKey = LoadPrivateKeyResource(keyResource);

            return new DefaultTlsEncryptionCredentials(context, certificate, privateKey);
        }

        internal static TlsSignerCredentials LoadSignerCredentials(TlsContext context, string[] certResources,
            string keyResource, SignatureAndHashAlgorithm signatureAndHashAlgorithm)
        {
            Certificate certificate = LoadCertificateChain(certResources);
            AsymmetricKeyParameter privateKey = LoadPrivateKeyResource(keyResource);

            return new DefaultTlsSignerCredentials(context, certificate, privateKey, signatureAndHashAlgorithm);
        }

        internal static TlsSignerCredentials LoadSignerCredentials(TlsContext context, IList supportedSignatureAlgorithms,
            byte signatureAlgorithm, string certResource, string keyResource)
        {
            /*
             * TODO Note that this code fails to provide default value for the client supported
             * algorithms if it wasn't sent.
             */

            SignatureAndHashAlgorithm signatureAndHashAlgorithm = null;
            if (supportedSignatureAlgorithms != null)
            {
                foreach (SignatureAndHashAlgorithm alg in supportedSignatureAlgorithms)
                {
                    if (alg.Signature == signatureAlgorithm)
                    {
                        signatureAndHashAlgorithm = alg;
                        break;
                    }
                }

                if (signatureAndHashAlgorithm == null)
                    return null;
            }

            return LoadSignerCredentials(context, new String[]{ certResource, "x509-ca.pem" },
                keyResource, signatureAndHashAlgorithm);
        }

        internal static Certificate LoadCertificateChain(string[] resources)
        {
            X509CertificateStructure[] chain = new X509CertificateStructure[resources.Length];
            for (int i = 0; i < resources.Length; ++i)
            {
                chain[i] = LoadCertificateResource(resources[i]);
            }
            return new Certificate(chain);
        }

        internal static X509CertificateStructure LoadCertificateResource(string resource)
        {
            PemObject pem = LoadPemResource(resource);
            if (pem.Type.EndsWith("CERTIFICATE"))
            {
                return X509CertificateStructure.GetInstance(pem.Content);
            }
            throw new ArgumentException("doesn't specify a valid certificate", "resource");
        }

        internal static AsymmetricKeyParameter LoadPrivateKeyResource(string resource)
        {
            PemObject pem = LoadPemResource(resource);
            if (pem.Type.EndsWith("RSA PRIVATE KEY"))
            {
                RsaPrivateKeyStructure rsa = RsaPrivateKeyStructure.GetInstance(pem.Content);
                return new RsaPrivateCrtKeyParameters(rsa.Modulus, rsa.PublicExponent,
                    rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1,
                    rsa.Exponent2, rsa.Coefficient);
            }
            if (pem.Type.EndsWith("PRIVATE KEY"))
            {
                return PrivateKeyFactory.CreateKey(pem.Content);
            }
            throw new ArgumentException("doesn't specify a valid private key", "resource");
        }

        internal static PemObject LoadPemResource(string resource)
        {
            Stream s = SimpleTest.GetTestDataAsStream("tls." + resource);
            PemReader p = new PemReader(new StreamReader(s));
            PemObject o = p.ReadPemObject();
            p.Reader.Close();
            return o;
        }
    }
}
