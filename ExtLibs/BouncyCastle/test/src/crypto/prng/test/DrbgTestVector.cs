using System;
using System.Collections;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Prng.Test
{
    public class DrbgTestVector
    {
        private IDigest _digest;
        private IBlockCipher _cipher;
        private int _keySizeInBits;
        private IEntropySource _eSource;
        private bool _pr;
        private string _nonce;
        private string _personalisation;
        private int _ss;
        private String[] _ev;
        private IList _ai = new ArrayList();

        public DrbgTestVector(IDigest digest, IEntropySource eSource, bool predictionResistance, string nonce,
            int securityStrength, string[] expected)
        {
            _digest = digest;
            _eSource = eSource;
            _pr = predictionResistance;
            _nonce = nonce;
            _ss = securityStrength;
            _ev = expected;
            _personalisation = null;
        }

        public DrbgTestVector(IBlockCipher cipher, int keySizeInBits, IEntropySource eSource, bool predictionResistance,
            string nonce, int securityStrength, string[] expected)
        {
            _cipher = cipher;
            _keySizeInBits = keySizeInBits;
            _eSource = eSource;
            _pr = predictionResistance;
            _nonce = nonce;
            _ss = securityStrength;
            _ev = expected;
            _personalisation = null;
        }

        public IDigest Digest
        {
            get { return _digest; }
        }

        public IBlockCipher Cipher
        {
            get { return _cipher; }
        }

        public int KeySizeInBits
        {
            get { return _keySizeInBits; }
        }

        public DrbgTestVector AddAdditionalInput(string input)
        {
            _ai.Add(input);
            return this;
        }

        public DrbgTestVector SetPersonalizationString(string p)
        {
            _personalisation = p;
            return this;
        }

        public IEntropySource EntropySource
        {
            get { return _eSource; }
        }

        public bool PredictionResistance
        {
            get { return _pr; }
        }

        public byte[] GetNonce()
        {
            return _nonce == null ? null : Hex.Decode(_nonce);
        }

        public byte[] GetPersonalizationString()
        {
             return _personalisation == null ? null : Hex.Decode(_personalisation);
        }

        public int SecurityStrength
        {
            get { return _ss; }
        }

        public byte[] GetExpectedValue(int index)
        {
            return Hex.Decode(_ev[index]);
        }

        public byte[] GetAdditionalInput(int position)
        {
            if (position >= _ai.Count)
                return null;

            return Hex.Decode((string)_ai[position]);
        }
    }
}
