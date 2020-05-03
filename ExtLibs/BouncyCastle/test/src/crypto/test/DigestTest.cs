using System;

using Org.BouncyCastle.Crypto;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	public abstract class DigestTest
		: SimpleTest
	{
		private IDigest digest;
		private string[] input;
		private string[] results;

		protected DigestTest(
			IDigest digest,
			string[] input,
			string[] results)
		{
			this.digest = digest;
			this.input = input;
			this.results = results;
		}

		public override string Name
		{
			get { return digest.AlgorithmName; }
		}

		public override void PerformTest()
		{
			byte[] resBuf = new byte[digest.GetDigestSize()];

			for (int i = 0; i < input.Length - 1; i++)
			{
				byte[] msg = toByteArray(input[i]);

				vectorTest(digest, i, resBuf, msg, Hex.Decode(results[i]));
			}

			byte[] lastV = toByteArray(input[input.Length - 1]);
			byte[] lastDigest = Hex.Decode(results[input.Length - 1]);

			vectorTest(digest, input.Length - 1, resBuf, lastV, Hex.Decode(results[input.Length - 1]));

			//
			// clone test
			//
			digest.BlockUpdate(lastV, 0, lastV.Length/2);

			// clone the Digest
			IDigest d = CloneDigest(digest);

			digest.BlockUpdate(lastV, lastV.Length/2, lastV.Length - lastV.Length/2);
			digest.DoFinal(resBuf, 0);

			if (!AreEqual(lastDigest, resBuf))
			{
				Fail("failing clone vector test", results[results.Length - 1], Hex.ToHexString(resBuf));
			}

			d.BlockUpdate(lastV, lastV.Length/2, lastV.Length - lastV.Length/2);
			d.DoFinal(resBuf, 0);

			if (!AreEqual(lastDigest, resBuf))
			{
				Fail("failing second clone vector test", results[results.Length - 1], Hex.ToHexString(resBuf));
			}

			//
			// memo test
			//
			IMemoable m = (IMemoable)digest;

			digest.BlockUpdate(lastV, 0, lastV.Length/2);

			// copy the Digest
			IMemoable copy1 = m.Copy();
			IMemoable copy2 = copy1.Copy();

			digest.BlockUpdate(lastV, lastV.Length/2, lastV.Length - lastV.Length/2);
			digest.DoFinal(resBuf, 0);

			if (!AreEqual(lastDigest, resBuf))
			{
				Fail("failing memo vector test", results[results.Length - 1], Hex.ToHexString(resBuf));
			}

			m.Reset(copy1);

			digest.BlockUpdate(lastV, lastV.Length/2, lastV.Length - lastV.Length/2);
			digest.DoFinal(resBuf, 0);

			if (!AreEqual(lastDigest, resBuf))
			{
				Fail("failing memo reset vector test", results[results.Length - 1], Hex.ToHexString(resBuf));
			}

			IDigest md = (IDigest)copy2;

			md.BlockUpdate(lastV, lastV.Length/2, lastV.Length - lastV.Length/2);
			md.DoFinal(resBuf, 0);

			if (!AreEqual(lastDigest, resBuf))
			{
				Fail("failing memo copy vector test", results[results.Length - 1], Hex.ToHexString(resBuf));
			}
		}

		private byte[] toByteArray(
			string input)
		{
			byte[] bytes = new byte[input.Length];

			for (int i = 0; i != bytes.Length; i++)
			{
				bytes[i] = (byte)input[i];
			}

			return bytes;
		}

		private void vectorTest(
			IDigest digest,
			int count,
			byte[] resBuf,
			byte[] input,
			byte[] expected)
		{
			digest.BlockUpdate(input, 0, input.Length);
			digest.DoFinal(resBuf, 0);

			if (!AreEqual(resBuf, expected))
			{
				Fail("Vector " + count + " failed got " + Hex.ToHexString(resBuf));
			}
		}

		protected abstract IDigest CloneDigest(IDigest digest);

		//
		// optional tests
		//
		protected void millionATest(
			string expected)
		{
			byte[] resBuf = new byte[digest.GetDigestSize()];

			for (int i = 0; i < 1000000; i++)
			{
				digest.Update((byte)'a');
			}

			digest.DoFinal(resBuf, 0);

			if (!AreEqual(resBuf, Hex.Decode(expected)))
			{
				Fail("Million a's failed");
			}
		}

		protected void sixtyFourKTest(
			string expected)
		{
			byte[] resBuf = new byte[digest.GetDigestSize()];

			for (int i = 0; i < 65536; i++)
			{
				digest.Update((byte)(i & 0xff));
			}

			digest.DoFinal(resBuf, 0);

			if (!AreEqual(resBuf, Hex.Decode(expected)))
			{
				Fail("64k test failed");
			}
		}
	}
}
