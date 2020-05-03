using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Asn1.Tests
{
    /// <summary>
    /// Tests used to verify correct decoding of the ENUMERATED type.
    /// </summary>
    [TestFixture]
    public class EnumeratedTest
    {
        /// <summary>
        /// Test vector used to test decoding of multiple items.
        /// </summary>
        /// <remarks>This sample uses an ENUMERATED and a BOOLEAN.</remarks>
        private static readonly byte[] MultipleSingleByteItems = Hex.Decode("30060a01010101ff");

        /// <summary>
        /// Test vector used to test decoding of multiple items.
        /// </summary>
        /// <remarks>This sample uses two ENUMERATEDs.</remarks>
        private static readonly byte[] MultipleDoubleByteItems = Hex.Decode("30080a0201010a020202");

        /// <summary>
        /// Test vector used to test decoding of multiple items.
        /// </summary>
        /// <remarks>This sample uses an ENUMERATED and an OBJECT IDENTIFIER.</remarks>
        private static readonly byte[] MultipleTripleByteItems = Hex.Decode("300a0a0301010106032b0601");

        /// <summary>
        /// Makes sure multiple identically sized values are parsed correctly.
        /// </summary>
        [Test]
        public void TestReadingMultipleSingleByteItems()
        {
            Asn1Object obj = Asn1Object.FromByteArray(MultipleSingleByteItems);

            Assert.IsTrue(obj is DerSequence, "Null ASN.1 SEQUENCE");

            DerSequence sequence = (DerSequence)obj;

            Assert.AreEqual(2, sequence.Count, "2 items expected");

            DerEnumerated enumerated = sequence[0] as DerEnumerated;

            Assert.IsNotNull(enumerated, "ENUMERATED expected");

            Assert.AreEqual(1, enumerated.Value.IntValue, "Unexpected ENUMERATED value");

            DerBoolean boolean = sequence[1] as DerBoolean;

            Assert.IsNotNull(boolean, "BOOLEAN expected");

            Assert.IsTrue(boolean.IsTrue, "Unexpected BOOLEAN value");
        }

        /// <summary>
        /// Makes sure multiple identically sized values are parsed correctly.
        /// </summary>
        [Test]
        public void TestReadingMultipleDoubleByteItems()
        {
            Asn1Object obj = Asn1Object.FromByteArray(MultipleDoubleByteItems);

            Assert.IsTrue(obj is DerSequence, "Null ASN.1 SEQUENCE");

            DerSequence sequence = (DerSequence)obj;

            Assert.AreEqual(2, sequence.Count, "2 items expected");

            DerEnumerated enumerated1 = sequence[0] as DerEnumerated;

            Assert.IsNotNull(enumerated1, "ENUMERATED expected");

            Assert.AreEqual(257, enumerated1.Value.IntValue, "Unexpected ENUMERATED value");

            DerEnumerated enumerated2 = sequence[1] as DerEnumerated;

            Assert.IsNotNull(enumerated2, "ENUMERATED expected");

            Assert.AreEqual(514, enumerated2.Value.IntValue, "Unexpected ENUMERATED value");
        }

        /// <summary>
        /// Makes sure multiple identically sized values are parsed correctly.
        /// </summary>
        [Test]
        public void TestReadingMultipleTripleByteItems()
        {
            Asn1Object obj = Asn1Object.FromByteArray(MultipleTripleByteItems);

            Assert.IsTrue(obj is DerSequence, "Null ASN.1 SEQUENCE");

            DerSequence sequence = (DerSequence)obj;

            Assert.AreEqual(2, sequence.Count, "2 items expected");

            DerEnumerated enumerated = sequence[0] as DerEnumerated;

            Assert.IsNotNull(enumerated, "ENUMERATED expected");

            Assert.AreEqual(65793, enumerated.Value.IntValue, "Unexpected ENUMERATED value");

            DerObjectIdentifier objectId = sequence[1] as DerObjectIdentifier;

            Assert.IsNotNull(objectId, "OBJECT IDENTIFIER expected");

            Assert.AreEqual("1.3.6.1", objectId.Id, "Unexpected OBJECT IDENTIFIER value");
        }
    }
}
