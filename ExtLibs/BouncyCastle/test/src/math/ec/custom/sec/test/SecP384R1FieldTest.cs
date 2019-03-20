using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Math.EC.Custom.Sec.Tests
{
    [TestFixture]
    public class SecP384R1FieldTest
    {
        private static readonly SecureRandom Random = new SecureRandom();

        private static readonly X9ECParameters DP = CustomNamedCurves
            .GetByOid(SecObjectIdentifiers.SecP384r1);
        private static readonly BigInteger Q = DP.Curve.Field.Characteristic;

        [Test]
        public void TestMultiply1()
        {
            int COUNT = 1000;

            for (int i = 0; i < COUNT; ++i)
            {
                ECFieldElement x = GenerateMultiplyInput_Random();
                ECFieldElement y = GenerateMultiplyInput_Random();

                BigInteger X = x.ToBigInteger(), Y = y.ToBigInteger();
                BigInteger R = X.Multiply(Y).Mod(Q);

                ECFieldElement z = x.Multiply(y);
                BigInteger Z = z.ToBigInteger();

                Assert.AreEqual(R, Z);
            }
        }

        [Test]
        public void TestMultiply2()
        {
            int COUNT = 100;
            ECFieldElement[] inputs = new ECFieldElement[COUNT];
            BigInteger[] INPUTS = new BigInteger[COUNT];

            for (int i = 0; i < inputs.Length; ++i)
            {
                inputs[i] = GenerateMultiplyInput_Random();
                INPUTS[i] = inputs[i].ToBigInteger();
            }

            for (int j = 0; j < inputs.Length; ++j)
            {
                for (int k = 0; k < inputs.Length; ++k)
                {
                    BigInteger R = INPUTS[j].Multiply(INPUTS[k]).Mod(Q);

                    ECFieldElement z = inputs[j].Multiply(inputs[k]);
                    BigInteger Z = z.ToBigInteger();

                    Assert.AreEqual(R, Z);
                }
            }
        }

        [Test]
        public void TestSquare()
        {
            int COUNT = 1000;

            for (int i = 0; i < COUNT; ++i)
            {
                ECFieldElement x = GenerateMultiplyInput_Random();

                BigInteger X = x.ToBigInteger();
                BigInteger R = X.Multiply(X).Mod(Q);

                ECFieldElement z = x.Square();
                BigInteger Z = z.ToBigInteger();

                Assert.AreEqual(R, Z);
            }
        }

        [Test]
        public void TestSquare_CarryBug()
        {
            int COUNT = 100;

            for (int i = 0; i < COUNT; ++i)
            {
                ECFieldElement x = GenerateSquareInput_CarryBug();

                BigInteger X = x.ToBigInteger();
                BigInteger R = X.Multiply(X).Mod(Q);

                ECFieldElement z = x.Square();
                BigInteger Z = z.ToBigInteger();

                Assert.AreEqual(R, Z);
            }
        }

        /*
         * Based on another example input demonstrating the carry propagation bug in Nat192.square, as
         * reported by Joseph Friel on dev-crypto.
         */
        [Test]
        public void TestSquare_CarryBug_Reported()
        {
            ECFieldElement x = FE(new BigInteger("2fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffd", 16));

            BigInteger X = x.ToBigInteger();
            BigInteger R = X.Multiply(X).Mod(Q);

            ECFieldElement z = x.Square();
            BigInteger Z = z.ToBigInteger();

            Assert.AreEqual(R, Z);
        }

        private ECFieldElement FE(BigInteger x)
        {
            return DP.Curve.FromBigInteger(x);
        }

        private ECFieldElement GenerateMultiplyInput_Random()
        {
            return FE(new BigInteger(DP.Curve.FieldSize + 32, Random).Mod(Q));
        }

        private ECFieldElement GenerateSquareInput_CarryBug()
        {
            uint[] x = Nat_Create(12);
            x[0] = (uint)Random.NextInt() >> 1;
            x[6] = 2;
            x[10] = 0xFFFF0000;
            x[11] = 0xFFFFFFFF;

            return FE(Nat_ToBigInteger(12, x));
        }

        private static uint[] Nat_Create(int len)
        {
            return new uint[len];
        }

        private static BigInteger Nat_ToBigInteger(int len, uint[] x)
        {
            byte[] bs = new byte[len << 2];
            for (int i = 0; i < len; ++i)
            {
                uint x_i = x[i];
                if (x_i != 0)
                {
                    Pack_UInt32_To_BE(x_i, bs, (len - 1 - i) << 2);
                }
            }
            return new BigInteger(1, bs);
        }

        private static void Pack_UInt32_To_BE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n >> 24);
            bs[off + 1] = (byte)(n >> 16);
            bs[off + 2] = (byte)(n >> 8);
            bs[off + 3] = (byte)(n);
        }
    }
}
