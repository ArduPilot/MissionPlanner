using System;
using System.Collections;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509.Qualified
{
    /**
    * The SemanticsInformation object.
    * <pre>
    *       SemanticsInformation ::= SEQUENCE {
    *         semanticsIdentifier        OBJECT IDENTIFIER   OPTIONAL,
    *         nameRegistrationAuthorities NameRegistrationAuthorities
    *                                                         OPTIONAL }
    *         (WITH COMPONENTS {..., semanticsIdentifier PRESENT}|
    *          WITH COMPONENTS {..., nameRegistrationAuthorities PRESENT})
    *
    *     NameRegistrationAuthorities ::=  SEQUENCE SIZE (1..MAX) OF
    *         GeneralName
    * </pre>
    */
    public class SemanticsInformation
		: Asn1Encodable
    {
        private readonly DerObjectIdentifier	semanticsIdentifier;
        private readonly GeneralName[]			nameRegistrationAuthorities;

		public static SemanticsInformation GetInstance(
			object obj)
        {
            if (obj == null || obj is SemanticsInformation)
            {
                return (SemanticsInformation) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new SemanticsInformation(Asn1Sequence.GetInstance(obj));
            }

			throw new ArgumentException("unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
		}

		public SemanticsInformation(
			Asn1Sequence seq)
        {
            if (seq.Count < 1)
            {
                throw new ArgumentException("no objects in SemanticsInformation");
            }

			IEnumerator e = seq.GetEnumerator();
			e.MoveNext();
            object obj = e.Current;
            if (obj is DerObjectIdentifier)
            {
                semanticsIdentifier = DerObjectIdentifier.GetInstance(obj);
                if (e.MoveNext())
                {
                    obj  = e.Current;
                }
                else
                {
                    obj  = null;
                }
            }

			if (obj  != null)
            {
                Asn1Sequence generalNameSeq = Asn1Sequence.GetInstance(obj );
                nameRegistrationAuthorities = new GeneralName[generalNameSeq.Count];
                for (int i= 0; i < generalNameSeq.Count; i++)
                {
                    nameRegistrationAuthorities[i] = GeneralName.GetInstance(generalNameSeq[i]);
                }
            }
        }

		public SemanticsInformation(
            DerObjectIdentifier semanticsIdentifier,
            GeneralName[] generalNames)
        {
            this.semanticsIdentifier = semanticsIdentifier;
            this.nameRegistrationAuthorities = generalNames;
        }

		public SemanticsInformation(
			DerObjectIdentifier semanticsIdentifier)
        {
            this.semanticsIdentifier = semanticsIdentifier;
        }

        public SemanticsInformation(
			GeneralName[] generalNames)
        {
            this.nameRegistrationAuthorities = generalNames;
        }

		public DerObjectIdentifier SemanticsIdentifier { get { return semanticsIdentifier; } }

		public GeneralName[] GetNameRegistrationAuthorities()
        {
            return nameRegistrationAuthorities;
        }

		public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector seq = new Asn1EncodableVector();

            if (this.semanticsIdentifier != null)
            {
                seq.Add(semanticsIdentifier);
            }

			if (this.nameRegistrationAuthorities != null)
            {
                seq.Add(new DerSequence(nameRegistrationAuthorities));
            }

			return new DerSequence(seq);
        }
    }
}
