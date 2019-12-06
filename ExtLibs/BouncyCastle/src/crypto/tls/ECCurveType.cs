namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// RFC 4492 5.4
    /// </summary>
    public abstract class ECCurveType
    {
        /**
         * Indicates the elliptic curve domain parameters are conveyed verbosely, and the
         * underlying finite field is a prime field.
         */
        public const byte explicit_prime = 1;

        /**
         * Indicates the elliptic curve domain parameters are conveyed verbosely, and the
         * underlying finite field is a characteristic-2 field.
         */
        public const byte explicit_char2 = 2;

        /**
         * Indicates that a named curve is used. This option SHOULD be used when applicable.
         */
        public const byte named_curve = 3;

        /*
         * Values 248 through 255 are reserved for private use.
         */
    }
}
