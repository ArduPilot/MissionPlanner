using System;

namespace Org.BouncyCastle.Crypto.Prng.Test
{
    public class TestEntropySourceProvider
        :   IEntropySourceProvider
    {
        private readonly byte[] data;
        private readonly bool isPredictionResistant;

        internal TestEntropySourceProvider(byte[] data, bool isPredictionResistant)
        {
            this.data = data;
            this.isPredictionResistant = isPredictionResistant;
        }

        public IEntropySource Get(int bitsRequired)
        {
            return new EntropySource(bitsRequired, data, isPredictionResistant);
        }

        internal class EntropySource
            :   IEntropySource
        {
            private readonly int bitsRequired;
            private readonly byte[] data;
            private readonly bool isPredictionResistant;

            int index = 0;

            internal EntropySource(int bitsRequired, byte[] data, bool isPredictionResistant)
            {
                this.bitsRequired = bitsRequired;
                this.data = data;
                this.isPredictionResistant = isPredictionResistant;
            }

            public bool IsPredictionResistant
            {
                get { return isPredictionResistant; }
            }

            public byte[] GetEntropy()
            {
                byte[] rv = new byte[bitsRequired / 8];
                Array.Copy(data, index, rv, 0, rv.Length);
                index += bitsRequired / 8;
                return rv;
            }

            public int EntropySize
            {
                get { return bitsRequired; }
            }
        }
    }
}
