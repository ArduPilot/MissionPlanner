using System;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	public abstract class CipherTest
		: SimpleTest
	{
		private SimpleTest[]      _tests;
		private IBlockCipher _engine;
		private KeyParameter _validKey;

//		protected CipherTest(
//			SimpleTest[]	tests)
//		{
//			_tests = tests;
//		}

		protected CipherTest(
			SimpleTest[]	tests,
			IBlockCipher	engine,
			KeyParameter	validKey)
		{
			_tests = tests;
			_engine = engine;
			_validKey = validKey;
		}

		public override void PerformTest()
		{
			for (int i = 0; i != _tests.Length; i++)
			{
				_tests[i].PerformTest();
			}

			if (_engine != null)
			{
				//
				// state tests
				//
				byte[] buf = new byte[_engine.GetBlockSize()];

				try
				{
					_engine.ProcessBlock(buf, 0, buf, 0);

					Fail("failed initialisation check");
				}
				catch (InvalidOperationException)
				{
					// expected
				}

				bufferSizeCheck((_engine));
			}
		}

		private void bufferSizeCheck(
			IBlockCipher engine)
		{
			byte[] correctBuf = new byte[engine.GetBlockSize()];
			byte[] shortBuf = new byte[correctBuf.Length / 2];

			engine.Init(true, _validKey);

			try
			{
				engine.ProcessBlock(shortBuf, 0, correctBuf, 0);

				Fail("failed short input check");
			}
			catch (DataLengthException)
			{
				// expected
			}

			try
			{
				engine.ProcessBlock(correctBuf, 0, shortBuf, 0);

				Fail("failed short output check");
			}
			catch (DataLengthException)
			{
				// expected
			}

			engine.Init(false, _validKey);

			try
			{
				engine.ProcessBlock(shortBuf, 0, correctBuf, 0);

				Fail("failed short input check");
			}
			catch (DataLengthException)
			{
				// expected
			}

			try
			{
				engine.ProcessBlock(correctBuf, 0, shortBuf, 0);

				Fail("failed short output check");
			}
			catch (DataLengthException)
			{
				// expected
			}
		}
	}
}
