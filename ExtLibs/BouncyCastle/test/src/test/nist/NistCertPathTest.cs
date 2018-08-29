using System;
using System.Collections;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Tests.Nist
{
	[TestFixture]
	public class NistCertPathTest
	{
		private static readonly string GOOD_CA_CERT = "GoodCACert";

		private static readonly string GOOD_CA_CRL = "GoodCACRL";

		private static readonly string TRUST_ANCHOR_ROOT_CRL = "TrustAnchorRootCRL";

		private static readonly string TRUST_ANCHOR_ROOT_CERTIFICATE = "TrustAnchorRootCertificate";

		private static readonly char[] PKCS12_PASSWORD = "password".ToCharArray();

		private static readonly string ANY_POLICY = "2.5.29.32.0";
		private static readonly string NIST_TEST_POLICY_1 = "2.16.840.1.101.3.2.1.48.1";
		private static readonly string NIST_TEST_POLICY_2 = "2.16.840.1.101.3.2.1.48.2";
		private static readonly string NIST_TEST_POLICY_3 = "2.16.840.1.101.3.2.1.48.3";

		private static IDictionary certs = new Hashtable();
		private static IDictionary crls = new Hashtable();

		private static ISet noPolicies = new HashSet();
		private static ISet anyPolicy = new HashSet();
		private static ISet nistTestPolicy1 = new HashSet();
		private static ISet nistTestPolicy2 = new HashSet();
		private static ISet nistTestPolicy3 = new HashSet();
		private static ISet nistTestPolicy1And2 = new HashSet();

		static NistCertPathTest()
		{
			anyPolicy.Add(ANY_POLICY);

			nistTestPolicy1.Add(NIST_TEST_POLICY_1);
			nistTestPolicy2.Add(NIST_TEST_POLICY_2);
			nistTestPolicy3.Add(NIST_TEST_POLICY_3);
			nistTestPolicy1And2.Add(NIST_TEST_POLICY_1);
			nistTestPolicy1And2.Add(NIST_TEST_POLICY_2);

		}

		[Test]
		public void TestValidSignaturesTest1()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "ValidCertificatePathTest1EE", GOOD_CA_CERT },
				new string[] { GOOD_CA_CRL, TRUST_ANCHOR_ROOT_CRL });
		}

		[Test]
		public void TestInvalidCASignatureTest2()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "ValidCertificatePathTest1EE", "BadSignedCACert" },
				new string[] { "BadSignedCACRL", TRUST_ANCHOR_ROOT_CRL },
				1,
				"TrustAnchor found but certificate validation failed.");
		}

		[Test]
		public void TestInvalidEESignatureTest3()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "InvalidEESignatureTest3EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL },
				0,
				"Could not validate certificate signature.");
		}
        
		[Test]
		public void TestValidDSASignaturesTest4()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "DSACACert", "ValidDSASignaturesTest4EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "DSACACRL" });
		}

		[Test]
		// 4.1.5
		public void TestValidDSAParameterInheritanceTest5()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "DSACACert", "DSAParametersInheritedCACert", "ValidDSAParameterInheritanceTest5EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "DSACACRL", "DSAParametersInheritedCACRL" });
		}

		[Test]
		public void TestInvalidDSASignaturesTest6()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "DSACACert", "InvalidDSASignatureTest6EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "DSACACRL" },
				0,
				"Could not validate certificate signature.");
		}

		[Test]
		public void TestCANotBeforeDateTest1()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "BadnotBeforeDateCACert", "InvalidCAnotBeforeDateTest1EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "BadnotBeforeDateCACRL" },
				1,
				"Could not validate certificate: certificate not valid until 20470101120100GMT+00:00");
		}
        
		[Test]
		public void TestInvalidEENotBeforeDateTest2()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "InvalidEEnotBeforeDateTest2EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL },
				0,
				"Could not validate certificate: certificate not valid until 20470101120100GMT+00:00");
		}

		[Test]
		public void TestValidPre2000UTCNotBeforeDateTest3()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "Validpre2000UTCnotBeforeDateTest3EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL });
		}

		[Test]
		public void TestValidGeneralizedTimeNotBeforeDateTest4()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "ValidGeneralizedTimenotBeforeDateTest4EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL });
		}

		[Test]
		public void TestInvalidCANotAfterDateTest5()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "BadnotAfterDateCACert", "InvalidCAnotAfterDateTest5EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "BadnotAfterDateCACRL" },
				1,
				"Could not validate certificate: certificate expired on 20020101120100GMT+00:00");
		}

		[Test]
		public void TestInvalidEENotAfterDateTest6()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "InvalidEEnotAfterDateTest6EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL },
				0,
				"Could not validate certificate: certificate expired on 20020101120100GMT+00:00");
		}

		[Test]
		public void TestInvalidValidPre2000UTCNotAfterDateTest7()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "Invalidpre2000UTCEEnotAfterDateTest7EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL },
				0,
				"Could not validate certificate: certificate expired on 19990101120100GMT+00:00");
		}

		[Test]
		public void TestInvalidNegativeSerialNumberTest15()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "NegativeSerialNumberCACert", "InvalidNegativeSerialNumberTest15EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "NegativeSerialNumberCACRL" },
				0,
                // NOTE: Date/time part is locale-dependent
                //"Certificate revocation after Thu Apr 19 14:57:20",
                "Certificate revocation after",
                "reason: keyCompromise");
		}

		//
		// 4.8 Certificate Policies
		//
		[Test]
		public void TestAllCertificatesSamePolicyTest1()
		{
			string[] certList = new string[] { GOOD_CA_CERT, "ValidCertificatePathTest1EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				certList,
				crlList,
				noPolicies);

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				certList,
				crlList,
				nistTestPolicy1);

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				certList,
				crlList,
				nistTestPolicy2,
				-1,
				"Path processing failed on policy.");

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				certList,
				crlList,
				nistTestPolicy1And2);
		}

		[Test]
		public void TestAllCertificatesNoPoliciesTest2()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "NoPoliciesCACert", "AllCertificatesNoPoliciesTest2EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "NoPoliciesCACRL" });

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { "NoPoliciesCACert", "AllCertificatesNoPoliciesTest2EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, "NoPoliciesCACRL" },
				noPolicies,
				1,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestDifferentPoliciesTest3()
		{
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "PoliciesP2subCACert", "DifferentPoliciesTest3EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL, "PoliciesP2subCACRL" });

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "PoliciesP2subCACert", "DifferentPoliciesTest3EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL, "PoliciesP2subCACRL" },
				noPolicies,
				1,
				"No valid policy tree found when one expected.");

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "PoliciesP2subCACert", "DifferentPoliciesTest3EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL, "PoliciesP2subCACRL" },
				nistTestPolicy1And2,
				1,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestDifferentPoliciesTest4()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "GoodsubCACert", "DifferentPoliciesTest4EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL, "GoodsubCACRL" },
				0,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestDifferentPoliciesTest5()
		{
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE,
				new string[] { GOOD_CA_CERT, "PoliciesP2subCA2Cert", "DifferentPoliciesTest5EE" },
				new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL, "PoliciesP2subCA2CRL" },
				0,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestOverlappingPoliciesTest6()
		{
			string[] certList = new string[] { "PoliciesP1234CACert", "PoliciesP1234subCAP123Cert", "PoliciesP1234subsubCAP123P12Cert", "OverlappingPoliciesTest6EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP1234CACRL", "PoliciesP1234subCAP123CRL", "PoliciesP1234subsubCAP123P12CRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test]
		public void TestDifferentPoliciesTest7()
		{
			string[] certList = new string[] { "PoliciesP123CACert", "PoliciesP123subCAP12Cert", "PoliciesP123subsubCAP12P1Cert", "DifferentPoliciesTest7EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP123CACRL", "PoliciesP123subCAP12CRL", "PoliciesP123subsubCAP12P1CRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList,
				0,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestDifferentPoliciesTest8()
		{
			string[] certList = new string[] { "PoliciesP12CACert", "PoliciesP12subCAP1Cert", "PoliciesP12subsubCAP1P2Cert", "DifferentPoliciesTest8EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP12CACRL", "PoliciesP12subCAP1CRL", "PoliciesP12subsubCAP1P2CRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList,
				1,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestDifferentPoliciesTest9()
		{
			string[] certList = new string[] { "PoliciesP123CACert", "PoliciesP123subCAP12Cert", "PoliciesP123subsubCAP12P2Cert", "PoliciesP123subsubsubCAP12P2P1Cert", "DifferentPoliciesTest9EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP123CACRL", "PoliciesP123subCAP12CRL", "PoliciesP123subsubCAP2P2CRL", "PoliciesP123subsubsubCAP12P2P1CRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList,
				1,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestAllCertificatesSamePoliciesTest10()
		{
			string[] certList = new string[] { "PoliciesP12CACert", "AllCertificatesSamePoliciesTest10EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP12CACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2);
		}

		[Test]
		public void TestAllCertificatesAnyPolicyTest11()
		{
			string[] certList = new string[] { "anyPolicyCACert", "AllCertificatesanyPolicyTest11EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "anyPolicyCACRL" };

			PkixCertPathValidatorResult result = DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);

			result = DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
		}

		[Test]
		public void TestDifferentPoliciesTest12()
		{
			string[] certList = new string[] { "PoliciesP3CACert", "DifferentPoliciesTest12EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP3CACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList,
				0,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestAllCertificatesSamePoliciesTest13()
		{
			string[] certList = new string[] { "PoliciesP123CACert", "AllCertificatesSamePoliciesTest13EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP123CACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy3);
		}

		[Test]
		public void TestAnyPolicyTest14()
		{
			string[] certList = new string[] { "anyPolicyCACert", "AnyPolicyTest14EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "anyPolicyCACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test]
		public void TestUserNoticeQualifierTest15()
		{
			string[] certList = new string[] { "UserNoticeQualifierTest15EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test]
		public void TestUserNoticeQualifierTest16()
		{
			string[] certList = new string[] { GOOD_CA_CERT, "UserNoticeQualifierTest16EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL };

			PkixCertPathValidatorResult result = DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);

			result = DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test] 
		public void TestUserNoticeQualifierTest17()
		{
			string[] certList = new string[] { GOOD_CA_CERT, "UserNoticeQualifierTest17EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, GOOD_CA_CRL };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test]
		public void TestUserNoticeQualifierTest18()
		{
			string[] certList = new string[] { "PoliciesP12CACert", "UserNoticeQualifierTest18EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "PoliciesP12CACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2);
		}

		[Test]
		public void TestUserNoticeQualifierTest19()
		{
			string[] certList = new string[] { "UserNoticeQualifierTest19EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy2,
				-1,
				"Path processing failed on policy.");
		}

		[Test]
		public void TestInvalidInhibitPolicyMappingTest1()
		{
			string[] certList = new string[] { "inhibitPolicyMapping0CACert", "inhibitPolicyMapping0subCACert", "InvalidinhibitPolicyMappingTest1EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "inhibitPolicyMapping0CACRL", "inhibitPolicyMapping0subCACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"No valid policy tree found when one expected.");
		}

		[Test]
		public void TestValidInhibitPolicyMappingTest2()
		{
			string[] certList = new string[] { "inhibitPolicyMapping1P12CACert", "inhibitPolicyMapping1P12subCACert", "ValidinhibitPolicyMappingTest2EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "inhibitPolicyMapping1P12CACRL", "inhibitPolicyMapping1P12subCACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, nistTestPolicy1);
		}

		// 4.12.7
		[Test]
		public void TestValidSelfIssuedInhibitAnyPolicyTest7()
		{
			string[] certList = new string[] { "inhibitAnyPolicy1CACert", "inhibitAnyPolicy1SelfIssuedCACert", "inhibitAnyPolicy1subCA2Cert", "ValidSelfIssuedinhibitAnyPolicyTest7EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "inhibitAnyPolicy1CACRL", "inhibitAnyPolicy1subCA2CRL" };

			doBuilderTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null, false, false);
		}

		// 4.4.19
		[Test]
		public void TestValidSeparateCertificateandCRLKeysTest19()
		{
			string[] certList = new string[] { "SeparateCertificateandCRLKeysCertificateSigningCACert", "SeparateCertificateandCRLKeysCRLSigningCert", "ValidSeparateCertificateandCRLKeysTest19EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "SeparateCertificateandCRLKeysCRL" };

			doBuilderTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null, false, false);
		}

		[Test]
		public void TestValidpathLenConstraintTest13()
		{
			string[] certList = new string[] { "pathLenConstraint6CACert", "pathLenConstraint6subCA4Cert", "pathLenConstraint6subsubCA41Cert", "pathLenConstraint6subsubsubCA41XCert", "ValidpathLenConstraintTest13EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "pathLenConstraint6CACRL", "pathLenConstraint6subCA4CRL", "pathLenConstraint6subsubCA41CRL", "pathLenConstraint6subsubsubCA41XCRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null);
		}

		// 4.4.10
		[Test]
		public void TestInvalidUnknownCRLExtensionTest10()
		{
			string[] certList = new string[] { "UnknownCRLExtensionCACert", "InvalidUnknownCRLExtensionTest10EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "UnknownCRLExtensionCACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"CRL contains unsupported critical extensions.");
		}

		// 4.14.3
		[Test]
		public void TestInvaliddistributionPointTest3()
		{
			string[] certList = new string[] { "distributionPoint1CACert", "InvaliddistributionPointTest3EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "distributionPoint1CACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
		}

		// 4.14.5
		[Test]
		public void TestValiddistributionPointTest5()
		{
			string[] certList = new string[] { "distributionPoint2CACert", "ValiddistributionPointTest5EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "distributionPoint2CACRL" };

			DoTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null);
		}

		// 4.14.8
		[Test]
		public void TestInvaliddistributionPointTest8()
		{
			string[] certList = new string[] { "distributionPoint2CACert", "InvaliddistributionPointTest8EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "distributionPoint2CACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
		}

		// 4.14.9
		[Test]
		public void TestInvaliddistributionPointTest9()
		{
			string[] certList = new string[] { "distributionPoint2CACert", "InvaliddistributionPointTest9EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "distributionPoint2CACRL" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
		}

		// 4.14.17
		[Test]
		public void TestInvalidonlySomeReasonsTest17()
		{
			string[] certList = new string[] { "onlySomeReasonsCA2Cert", "InvalidonlySomeReasonsTest17EE" };
			string[] crlList = new string[] { TRUST_ANCHOR_ROOT_CRL, "onlySomeReasonsCA2CRL1", "onlySomeReasonsCA2CRL2" };

			DoExceptionTest(TRUST_ANCHOR_ROOT_CERTIFICATE, certList, crlList, null,
				0,
				"Certificate status could not be determined.");
		}

		// section 4.14: tests 17, 24, 25, 30, 31, 32, 33, 35

		// section 4.15: tests 5, 7
		private void DoExceptionTest(
			string		trustAnchor,
			string[]	certs,
			string[]	crls,
			int			index,
			string		message)
		{
			try
			{
				DoTest(trustAnchor, certs, crls);

				Assert.Fail("path accepted when should be rejected");
			}
			catch (PkixCertPathValidatorException e)
			{
				Assert.AreEqual(index, e.Index);
				Assert.AreEqual(message, e.Message);
			}
		}

		private void DoExceptionTest(
			string		trustAnchor,
			string[]	certs,
			string[]	crls,
			ISet		policies,
			int			index,
			string		message)
		{
			try
			{
				DoTest(trustAnchor, certs, crls, policies);
				Assert.Fail("path accepted when should be rejected");
			}
			catch (PkixCertPathValidatorException e)
			{
				Assert.AreEqual(index, e.Index);
				Assert.AreEqual(message, e.Message);
			}
		}

		private void DoExceptionTest(
			string		trustAnchor,
			string[]	certs,
			string[]	crls,
			int			index,
			string		mesStart,
			string		mesEnd)
		{
			try
			{
				DoTest(trustAnchor, certs, crls);

				Assert.Fail("path accepted when should be rejected");
			}
			catch (PkixCertPathValidatorException e)
			{
				Assert.AreEqual(index, e.Index);
				Assert.IsTrue(e.Message.StartsWith(mesStart));
				Assert.IsTrue(e.Message.EndsWith(mesEnd));
			}
		}

		private PkixCertPathValidatorResult DoTest(
			string trustAnchor,
			string[] certs,
			string[] crls)
		{
			return DoTest(trustAnchor, certs, crls, null);
		}

		private PkixCertPathValidatorResult DoTest(
			string trustAnchor,
			string[] certs,
			string[] crls,
			ISet policies)
		{
			ISet trustedSet = new HashSet();
			trustedSet.Add(GetTrustAnchor(trustAnchor));

			IList x509Certs = new ArrayList();
			IList x509Crls = new ArrayList();
			X509Certificate endCert = LoadCert(certs[certs.Length - 1]);

			for (int i = 0; i != certs.Length - 1; i++)
			{
				x509Certs.Add(LoadCert(certs[i]));
			}

			x509Certs.Add(endCert);

			PkixCertPath certPath = new PkixCertPath(x509Certs);

			for (int i = 0; i != crls.Length; i++)
			{
				x509Crls.Add(LoadCrl(crls[i]));
			}

			IX509Store x509CertStore = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(x509Certs));
			IX509Store x509CrlStore = X509StoreFactory.Create(
				"CRL/Collection",
				new X509CollectionStoreParameters(x509Crls));

//			CertPathValidator validator = CertPathValidator.GetInstance("PKIX");
			PkixCertPathValidator validator = new PkixCertPathValidator();
			PkixParameters parameters = new PkixParameters(trustedSet);

			parameters.AddStore(x509CertStore);
			parameters.AddStore(x509CrlStore);
			parameters.IsRevocationEnabled = true;

			if (policies != null)
			{
				parameters.IsExplicitPolicyRequired = true;
				parameters.SetInitialPolicies(policies);
			}

			// Perform validation as of this date since test certs expired
			parameters.Date = new DateTimeObject(DateTime.Parse("1/1/2011"));

			return validator.Validate(certPath, parameters);
		}

		private PkixCertPathBuilderResult doBuilderTest(
			string		trustAnchor,
			string[]	certs,
			string[]	crls,
			ISet		initialPolicies,
			bool		policyMappingInhibited,
			bool		anyPolicyInhibited)
		{
			ISet trustedSet = new HashSet();
			trustedSet.Add(GetTrustAnchor(trustAnchor));

			IList x509Certs = new ArrayList();
			IList x509Crls = new ArrayList();
			X509Certificate endCert = LoadCert(certs[certs.Length - 1]);

			for (int i = 0; i != certs.Length - 1; i++)
			{
				x509Certs.Add(LoadCert(certs[i]));
			}

			x509Certs.Add(endCert);

			for (int i = 0; i != crls.Length; i++)
			{
				x509Crls.Add(LoadCrl(crls[i]));
			}

			IX509Store x509CertStore = X509StoreFactory.Create(
				"Certificate/Collection",
				new X509CollectionStoreParameters(x509Certs));
			IX509Store x509CrlStore = X509StoreFactory.Create(
				"CRL/Collection",
				new X509CollectionStoreParameters(x509Crls));

//			CertPathBuilder builder = CertPathBuilder.GetInstance("PKIX");   
			PkixCertPathBuilder builder = new PkixCertPathBuilder();

			X509CertStoreSelector endSelector = new X509CertStoreSelector();

			endSelector.Certificate = endCert;

			PkixBuilderParameters builderParams = new PkixBuilderParameters(trustedSet, endSelector);

			if (initialPolicies != null)
			{
				builderParams.SetInitialPolicies(initialPolicies);
				builderParams.IsExplicitPolicyRequired = true;
			}
			if (policyMappingInhibited)
			{
				builderParams.IsPolicyMappingInhibited = policyMappingInhibited;
			}
			if (anyPolicyInhibited)
			{
				builderParams.IsAnyPolicyInhibited = anyPolicyInhibited;
			}

			builderParams.AddStore(x509CertStore);
			builderParams.AddStore(x509CrlStore);

			// Perform validation as of this date since test certs expired
			builderParams.Date = new DateTimeObject(DateTime.Parse("1/1/2011"));

			try
			{
				return (PkixCertPathBuilderResult) builder.Build(builderParams);
			}
			catch (PkixCertPathBuilderException e)
			{               
				throw e.InnerException;
			}
		}

		private X509Certificate LoadCert(
			string certName)
		{
			X509Certificate cert = (X509Certificate)certs[certName];

			if (cert != null)
			{
				return cert;
			}

			Stream fs = null;

			try
			{
				fs = SimpleTest.GetTestDataAsStream("PKITS.certs." + certName + ".crt");

				cert = new X509CertificateParser().ReadCertificate(fs);

				certs[certName] = cert;

				return cert;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("exception loading certificate " + certName + ": " + e);
			}
			finally
			{
				fs.Close();
			}
		}

		private X509Crl LoadCrl(
			string crlName)
			//throws Exception
		{
			X509Crl crl = (X509Crl)certs[crlName];
        
			if (crl != null)
			{
				return crl;
			}

			Stream fs = null;

			try
			{
				fs = SimpleTest.GetTestDataAsStream("PKITS.crls." + crlName + ".crl");

				crl = new X509CrlParser().ReadCrl(fs);

				crls[crlName] = crl;

				return crl;
			}
			catch (Exception)
			{
				throw new InvalidOperationException("exception loading CRL: " + crlName);
			}
			finally
			{
				fs.Close();
			}
		}

		private TrustAnchor GetTrustAnchor(string trustAnchorName)
		{
			X509Certificate cert = LoadCert(trustAnchorName);
			Asn1OctetString extBytes = cert.GetExtensionValue(X509Extensions.NameConstraints);

			if (extBytes != null)
			{
				Asn1Encodable extValue = X509ExtensionUtilities.FromExtensionValue(extBytes);

				return new TrustAnchor(cert, extValue.GetDerEncoded());
			}

			return new TrustAnchor(cert, null);
		}
	}
}
