using System;

namespace Org.BouncyCastle.Asn1.BC
{
	public abstract class BCObjectIdentifiers
	{
        /**
         *  iso.org.dod.internet.private.enterprise.legion-of-the-bouncy-castle
         *<p>
         *  1.3.6.1.4.1.22554
         */
        public static readonly DerObjectIdentifier bc = new DerObjectIdentifier("1.3.6.1.4.1.22554");

        /**
         * pbe(1) algorithms
         * <p>
         * 1.3.6.1.4.1.22554.1
         */
        public static readonly DerObjectIdentifier bc_pbe        = bc.Branch("1");

        /**
         * SHA-1(1)
         * <p>
         * 1.3.6.1.4.1.22554.1.1
         */
        public static readonly DerObjectIdentifier bc_pbe_sha1   = bc_pbe.Branch("1");

        /** SHA-2.SHA-256; 1.3.6.1.4.1.22554.1.2.1 */
        public static readonly DerObjectIdentifier bc_pbe_sha256 = bc_pbe.Branch("2.1");
        /** SHA-2.SHA-384; 1.3.6.1.4.1.22554.1.2.2 */
        public static readonly DerObjectIdentifier bc_pbe_sha384 = bc_pbe.Branch("2.2");
        /** SHA-2.SHA-512; 1.3.6.1.4.1.22554.1.2.3 */
        public static readonly DerObjectIdentifier bc_pbe_sha512 = bc_pbe.Branch("2.3");
        /** SHA-2.SHA-224; 1.3.6.1.4.1.22554.1.2.4 */
        public static readonly DerObjectIdentifier bc_pbe_sha224 = bc_pbe.Branch("2.4");

        /**
         * PKCS-5(1)|PKCS-12(2)
         */
        /** SHA-1.PKCS5;  1.3.6.1.4.1.22554.1.1.1 */
        public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs5    = bc_pbe_sha1.Branch("1");
        /** SHA-1.PKCS12; 1.3.6.1.4.1.22554.1.1.2 */
        public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12   = bc_pbe_sha1.Branch("2");

        /** SHA-256.PKCS12; 1.3.6.1.4.1.22554.1.2.1.1 */
        public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs5  = bc_pbe_sha256.Branch("1");
        /** SHA-256.PKCS12; 1.3.6.1.4.1.22554.1.2.1.2 */
        public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12 = bc_pbe_sha256.Branch("2");

        /**
         * AES(1) . (CBC-128(2)|CBC-192(22)|CBC-256(42))
         */
        /** 1.3.6.1.4.1.22554.1.1.2.1.2 */
        public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes128_cbc   = bc_pbe_sha1_pkcs12.Branch("1.2");
        /** 1.3.6.1.4.1.22554.1.1.2.1.22 */
        public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes192_cbc   = bc_pbe_sha1_pkcs12.Branch("1.22");
        /** 1.3.6.1.4.1.22554.1.1.2.1.42 */
        public static readonly DerObjectIdentifier bc_pbe_sha1_pkcs12_aes256_cbc   = bc_pbe_sha1_pkcs12.Branch("1.42");

        /** 1.3.6.1.4.1.22554.1.1.2.2.2 */
        public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes128_cbc = bc_pbe_sha256_pkcs12.Branch("1.2");
        /** 1.3.6.1.4.1.22554.1.1.2.2.22 */
        public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes192_cbc = bc_pbe_sha256_pkcs12.Branch("1.22");
        /** 1.3.6.1.4.1.22554.1.1.2.2.42 */
        public static readonly DerObjectIdentifier bc_pbe_sha256_pkcs12_aes256_cbc = bc_pbe_sha256_pkcs12.Branch("1.42");

        /**
         * signature(2) algorithms
         */
        public static readonly DerObjectIdentifier bc_sig        = bc.Branch("2");

        /**
         * Sphincs-256
         */
        public static readonly DerObjectIdentifier sphincs256                      = bc_sig.Branch("1");
        public static readonly DerObjectIdentifier sphincs256_with_BLAKE512        = sphincs256.Branch("1");
        public static readonly DerObjectIdentifier sphincs256_with_SHA512          = sphincs256.Branch("2");
        public static readonly DerObjectIdentifier sphincs256_with_SHA3_512        = sphincs256.Branch("3");

        /**
         * XMSS
         */
        public static readonly DerObjectIdentifier xmss                      = bc_sig.Branch("2");
        public static readonly DerObjectIdentifier xmss_with_SHA256          = xmss.Branch("1");
        public static readonly DerObjectIdentifier xmss_with_SHA512          = xmss.Branch("2");
        public static readonly DerObjectIdentifier xmss_with_SHAKE128        = xmss.Branch("3");
        public static readonly DerObjectIdentifier xmss_with_SHAKE256        = xmss.Branch("4");

        /**
         * XMSS^MT
         */
        public static readonly DerObjectIdentifier xmss_mt                      = bc_sig.Branch("3");
        public static readonly DerObjectIdentifier xmss_mt_with_SHA256          = xmss_mt.Branch("1");
        public static readonly DerObjectIdentifier xmss_mt_with_SHA512          = xmss_mt.Branch("2");
        public static readonly DerObjectIdentifier xmss_mt_with_SHAKE128        = xmss_mt.Branch("3");
        public static readonly DerObjectIdentifier xmss_mt_with_SHAKE256        = xmss_mt.Branch("4");

        /**
         * key_exchange(3) algorithms
         */
        public static readonly DerObjectIdentifier bc_exch = bc.Branch("3");

        /**
         * NewHope
         */
        public static readonly DerObjectIdentifier newHope = bc_exch.Branch("1");
	}
}