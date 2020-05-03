using System;

namespace Org.BouncyCastle.Crypto.Prng.Test
{
    public class X931TestVector
    {
        private readonly IBlockCipher engine;
        private readonly IEntropySourceProvider entropyProvider;
        private readonly string key;
        private readonly string dateTimeVector;
        private readonly bool predictionResistant;
        private readonly string[] expected;

        public X931TestVector(IBlockCipher engine, IEntropySourceProvider entropyProvider, string key, string dateTimeVector,
            bool predictionResistant, string[] expected)
        {
            this.engine = engine;
            this.entropyProvider = entropyProvider;
            this.key = key;
            this.dateTimeVector = dateTimeVector;
            this.predictionResistant = predictionResistant;
            this.expected = expected;
        }

        public string DateTimeVector
        {
            get { return dateTimeVector; }
        }

        public IBlockCipher Engine
        {
            get { return engine; }
        }

        public IEntropySourceProvider EntropyProvider
        {
            get { return entropyProvider; }
        }

        public string[] Expected
        {
            get { return expected; }
        }

        public string Key
        {
            get { return key; }
        }

        public bool IsPredictionResistant
        {
            get { return predictionResistant; }
        }
    }
}
