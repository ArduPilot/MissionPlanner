using System;
using System.Collections;
using System.IO;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Bcpg.OpenPgp
{
    /// <remarks>
    /// Class to hold a single master public key and its subkeys.
    /// <p>
    /// Often PGP keyring files consist of multiple master keys, if you are trying to process
    /// or construct one of these you should use the <c>PgpPublicKeyRingBundle</c> class.
    /// </p>
    /// </remarks>
    public class PgpPublicKeyRing
        : PgpKeyRing
    {
        private readonly IList keys;

        public PgpPublicKeyRing(
            byte[] encoding)
            : this(new MemoryStream(encoding, false))
        {
        }

        internal PgpPublicKeyRing(
            IList pubKeys)
        {
            this.keys = pubKeys;
        }

        public PgpPublicKeyRing(
            Stream inputStream)
        {
            this.keys = Platform.CreateArrayList();

            BcpgInputStream bcpgInput = BcpgInputStream.Wrap(inputStream);

            PacketTag initialTag = bcpgInput.NextPacketTag();
            if (initialTag != PacketTag.PublicKey && initialTag != PacketTag.PublicSubkey)
            {
                throw new IOException("public key ring doesn't start with public key tag: "
                    + "tag 0x" + ((int)initialTag).ToString("X"));
            }

            PublicKeyPacket pubPk = (PublicKeyPacket) bcpgInput.ReadPacket();
            TrustPacket trustPk = ReadOptionalTrustPacket(bcpgInput);

            // direct signatures and revocations
            IList keySigs = ReadSignaturesAndTrust(bcpgInput);

            IList ids, idTrusts, idSigs;
            ReadUserIDs(bcpgInput, out ids, out idTrusts, out idSigs);

            keys.Add(new PgpPublicKey(pubPk, trustPk, keySigs, ids, idTrusts, idSigs));


            // Read subkeys
            while (bcpgInput.NextPacketTag() == PacketTag.PublicSubkey)
            {
                keys.Add(ReadSubkey(bcpgInput));
            }
        }

        /// <summary>Return the first public key in the ring.</summary>
        public virtual PgpPublicKey GetPublicKey()
        {
            return (PgpPublicKey) keys[0];
        }

        /// <summary>Return the public key referred to by the passed in key ID if it is present.</summary>
        public virtual PgpPublicKey GetPublicKey(
            long keyId)
        {
            foreach (PgpPublicKey k in keys)
            {
                if (keyId == k.KeyId)
                {
                    return k;
                }
            }

            return null;
        }

        /// <summary>Allows enumeration of all the public keys.</summary>
        /// <returns>An <c>IEnumerable</c> of <c>PgpPublicKey</c> objects.</returns>
        public virtual IEnumerable GetPublicKeys()
        {
            return new EnumerableProxy(keys);
        }

        public virtual byte[] GetEncoded()
        {
            MemoryStream bOut = new MemoryStream();

            Encode(bOut);

            return bOut.ToArray();
        }

        public virtual void Encode(
            Stream outStr)
        {
            if (outStr == null)
                throw new ArgumentNullException("outStr");

            foreach (PgpPublicKey k in keys)
            {
                k.Encode(outStr);
            }
        }

        /// <summary>
        /// Returns a new key ring with the public key passed in either added or
        /// replacing an existing one.
        /// </summary>
        /// <param name="pubRing">The public key ring to be modified.</param>
        /// <param name="pubKey">The public key to be inserted.</param>
        /// <returns>A new <c>PgpPublicKeyRing</c></returns>
        public static PgpPublicKeyRing InsertPublicKey(
            PgpPublicKeyRing	pubRing,
            PgpPublicKey		pubKey)
        {
            IList keys = Platform.CreateArrayList(pubRing.keys);
            bool found = false;
            bool masterFound = false;

            for (int i = 0; i != keys.Count; i++)
            {
                PgpPublicKey key = (PgpPublicKey) keys[i];

                if (key.KeyId == pubKey.KeyId)
                {
                    found = true;
                    keys[i] = pubKey;
                }
                if (key.IsMasterKey)
                {
                    masterFound = true;
                }
            }

            if (!found)
            {
                if (pubKey.IsMasterKey)
                {
                    if (masterFound)
                        throw new ArgumentException("cannot add a master key to a ring that already has one");

                    keys.Insert(0, pubKey);
                }
                else
                {
                    keys.Add(pubKey);
                }
            }

            return new PgpPublicKeyRing(keys);
        }

        /// <summary>Returns a new key ring with the public key passed in removed from the key ring.</summary>
        /// <param name="pubRing">The public key ring to be modified.</param>
        /// <param name="pubKey">The public key to be removed.</param>
        /// <returns>A new <c>PgpPublicKeyRing</c>, or null if pubKey is not found.</returns>
        public static PgpPublicKeyRing RemovePublicKey(
            PgpPublicKeyRing	pubRing,
            PgpPublicKey		pubKey)
        {
            IList keys = Platform.CreateArrayList(pubRing.keys);
            bool found = false;

            for (int i = 0; i < keys.Count; i++)
            {
                PgpPublicKey key = (PgpPublicKey) keys[i];

                if (key.KeyId == pubKey.KeyId)
                {
                    found = true;
                    keys.RemoveAt(i);
                }
            }

            return found ? new PgpPublicKeyRing(keys) : null;
        }

        internal static PgpPublicKey ReadSubkey(BcpgInputStream bcpgInput)
        {
            PublicKeyPacket	pk = (PublicKeyPacket) bcpgInput.ReadPacket();
            TrustPacket kTrust = ReadOptionalTrustPacket(bcpgInput);

            // PGP 8 actually leaves out the signature.
            IList sigList = ReadSignaturesAndTrust(bcpgInput);

            return new PgpPublicKey(pk, kTrust, sigList);
        }
    }
}
