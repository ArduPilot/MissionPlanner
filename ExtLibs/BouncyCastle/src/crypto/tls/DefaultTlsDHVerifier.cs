using System;
using System.Collections;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsDHVerifier
        : TlsDHVerifier
    {
        public static readonly int DefaultMinimumPrimeBits = 2048;

        protected static readonly IList DefaultGroups = Platform.CreateArrayList();

        private static void AddDefaultGroup(DHParameters dhParameters)
        {
            DefaultGroups.Add(dhParameters);
        }

        static DefaultTlsDHVerifier()
        {
            AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe2048);
            AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe3072);
            AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe4096);
            AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe6144);
            AddDefaultGroup(DHStandardGroups.rfc7919_ffdhe8192);

            AddDefaultGroup(DHStandardGroups.rfc3526_1536);
            AddDefaultGroup(DHStandardGroups.rfc3526_2048);
            AddDefaultGroup(DHStandardGroups.rfc3526_3072);
            AddDefaultGroup(DHStandardGroups.rfc3526_4096);
            AddDefaultGroup(DHStandardGroups.rfc3526_6144);
            AddDefaultGroup(DHStandardGroups.rfc3526_8192);
        }

        // IList is (DHParameters)
        protected readonly IList mGroups;
        protected readonly int mMinimumPrimeBits;

        /// <summary>Accept various standard DH groups with 'P' at least <c>DefaultMinimumPrimeBits</c> bits.</summary>
        public DefaultTlsDHVerifier()
            : this(DefaultMinimumPrimeBits)
        {
        }

        /// <summary>Accept various standard DH groups with 'P' at least the specified number of bits.</summary>
        public DefaultTlsDHVerifier(int minimumPrimeBits)
            : this(DefaultGroups, minimumPrimeBits)
        {
        }

        /// <summary>Accept a custom set of group parameters, subject to a minimum bitlength for 'P'.</summary>
        /// <param name="groups">An <c>IList</c> of acceptable <c>DHParameters</c>.</param>
        /// <param name="minimumPrimeBits">The minimum acceptable bitlength of the 'P' parameter.</param>
        public DefaultTlsDHVerifier(IList groups, int minimumPrimeBits)
        {
            this.mGroups = groups;
            this.mMinimumPrimeBits = minimumPrimeBits;
        }

        public virtual bool Accept(DHParameters dhParameters)
        {
            return CheckMinimumPrimeBits(dhParameters) && CheckGroup(dhParameters);
        }

        public virtual int MinimumPrimeBits
        {
            get { return mMinimumPrimeBits; }
        }

        protected virtual bool AreGroupsEqual(DHParameters a, DHParameters b)
        {
            return a == b || (AreParametersEqual(a.P, b.P) && AreParametersEqual(a.G, b.G));
        }

        protected virtual bool AreParametersEqual(BigInteger a, BigInteger b)
        {
            return a == b || a.Equals(b);
        }

        protected virtual bool CheckGroup(DHParameters dhParameters)
        {
            foreach (DHParameters group in mGroups)
            {
                if (AreGroupsEqual(dhParameters, group))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool CheckMinimumPrimeBits(DHParameters dhParameters)
        {
            return dhParameters.P.BitLength >= MinimumPrimeBits;
        }
    }
}
