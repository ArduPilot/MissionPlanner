using System;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
    public class RegressionTest
    {
        public static readonly ITest[] tests =
        {
			new AdditionalInformationSyntaxUnitTest(),
			new AdmissionSyntaxUnitTest(),
			new AdmissionsUnitTest(),
			new AttributeTableUnitTest(),
            new BiometricDataUnitTest(),
            new BitStringTest(),
			new CertHashUnitTest(),
			new CertificateTest(),
            new CmsTest(),
            new CommitmentTypeIndicationUnitTest(),
            new CommitmentTypeQualifierUnitTest(),
			new ContentHintsUnitTest(),
			new CscaMasterListTest(),
			new DataGroupHashUnitTest(),
			new DeclarationOfMajorityUnitTest(),
			new DerApplicationSpecificTest(),
			new DerUtf8StringTest(),
			new EncryptedPrivateKeyInfoTest(),
            new EqualsAndHashCodeTest(),
			new EssCertIDv2UnitTest(),
            new GeneralizedTimeTest(),
			new GeneralNameTest(),
			new GenerationTest(),
			new InputStreamTest(),
            new Iso4217CurrencyCodeUnitTest(),
			new IssuingDistributionPointUnitTest(),
			new KeyUsageTest(),
            new LDSSecurityObjectUnitTest(),
            new MiscTest(),
			new MonetaryLimitUnitTest(),
			new MonetaryValueUnitTest(),
			new NameOrPseudonymUnitTest(),
			new NamingAuthorityUnitTest(),
			new NetscapeCertTypeTest(),
            new OcspTest(),
            new OidTest(),
			new OtherCertIDUnitTest(),
			new OtherSigningCertificateUnitTest(),
			new ParsingTest(),
			new PersonalDataUnitTest(),
			new Pkcs10Test(),
            new Pkcs12Test(),
			new PkiFailureInfoTest(),
			new ProcurationSyntaxUnitTest(),
			new ProfessionInfoUnitTest(),
			new QCStatementUnitTest(),
			new ReasonFlagsTest(),
			new RequestedCertificateUnitTest(),
			new RestrictionUnitTest(),
			new SemanticsInformationUnitTest(),
            new SetTest(),
            new SignerLocationUnitTest(),
            new SmimeTest(),
			new StringTest(),
			new SubjectKeyIdentifierTest(),
			new TagTest(),
			new TargetInformationTest(),
			new TypeOfBiometricDataUnitTest(),
			new UtcTimeTest(),
			new X509ExtensionsTest(),
			new X509NameTest(),
            new X9Test(),
        };

        public static void Main(
            string[] args)
        {
            for (int i = 0; i != tests.Length; i++)
            {
                ITestResult  result = tests[i].Perform();
                Console.WriteLine(result);
            }
        }
    }
}
