using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Asn1Pkcs = Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
    /**
    * the KeyTransRecipientInformation class for a recipient who has been sent a secret
    * key encrypted using their public key that needs to be used to
    * extract the message.
    */
    public class KeyTransRecipientInformation
        : RecipientInformation
    {
        private KeyTransRecipientInfo info;

		internal KeyTransRecipientInformation(
			KeyTransRecipientInfo	info,
			CmsSecureReadable		secureReadable)
			: base(info.KeyEncryptionAlgorithm, secureReadable)
		{
            this.info = info;
            this.rid = new RecipientID();

			RecipientIdentifier r = info.RecipientIdentifier;

			try
            {
                if (r.IsTagged)
                {
                    Asn1OctetString octs = Asn1OctetString.GetInstance(r.ID);

					rid.SubjectKeyIdentifier = octs.GetOctets();
                }
                else
                {
                    IssuerAndSerialNumber iAnds = IssuerAndSerialNumber.GetInstance(r.ID);

					rid.Issuer = iAnds.Name;
                    rid.SerialNumber = iAnds.SerialNumber.Value;
                }
            }
            catch (IOException)
            {
                throw new ArgumentException("invalid rid in KeyTransRecipientInformation");
            }
        }

		private string GetExchangeEncryptionAlgorithmName(
			DerObjectIdentifier oid)
		{
			if (Asn1Pkcs.PkcsObjectIdentifiers.RsaEncryption.Equals(oid))
			{
				return "RSA//PKCS1Padding";
			}

			return oid.Id;
		}

		internal KeyParameter UnwrapKey(ICipherParameters key)
		{
			byte[] encryptedKey = info.EncryptedKey.GetOctets();
            string keyExchangeAlgorithm = GetExchangeEncryptionAlgorithmName(keyEncAlg.Algorithm);

			try
			{
				IWrapper keyWrapper = WrapperUtilities.GetWrapper(keyExchangeAlgorithm);
				keyWrapper.Init(false, key);

				// FIXME Support for MAC algorithm parameters similar to cipher parameters
				return ParameterUtilities.CreateKeyParameter(
					GetContentAlgorithmName(), keyWrapper.Unwrap(encryptedKey, 0, encryptedKey.Length));
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e)
			{
				throw new CmsException("key invalid in message.", e);
			}
//			catch (IllegalBlockSizeException e)
			catch (DataLengthException e)
			{
				throw new CmsException("illegal blocksize in message.", e);
			}
//			catch (BadPaddingException e)
			catch (InvalidCipherTextException e)
			{
				throw new CmsException("bad padding in message.", e);
			}
		}
		
		/**
        * decrypt the content and return it as a byte array.
        */
        public override CmsTypedStream GetContentStream(
            ICipherParameters key)
        {
			KeyParameter sKey = UnwrapKey(key);

			return GetContentFromSessionKey(sKey);
		}
    }
}
