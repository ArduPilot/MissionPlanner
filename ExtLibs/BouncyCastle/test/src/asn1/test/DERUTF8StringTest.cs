using System;

using NUnit.Framework;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class DerUtf8StringTest
		: ITest
	{
		/**
			* Unicode code point U+10400 coded as surrogate in two native Java UTF-16
			* code units
			*/
		private readonly static char[] glyph1_utf16 = { (char)0xd801, (char)0xdc00 };

		/**
		 * U+10400 coded in UTF-8
		 */
		private readonly static byte[] glyph1_utf8 = { (byte)0xF0, (byte)0x90, (byte)0x90, (byte)0x80 };

		/**
		 * Unicode code point U+6771 in native Java UTF-16
		 */
		private readonly static char[] glyph2_utf16 = { (char)0x6771 };

		/**
		 * U+6771 coded in UTF-8
		 */
		private readonly static byte[] glyph2_utf8 = { (byte)0xE6, (byte)0x9D, (byte)0xB1 };

		/**
		 * Unicode code point U+00DF in native Java UTF-16
		 */
		private readonly static char[] glyph3_utf16 = { (char)0x00DF };

		/**
		 * U+00DF coded in UTF-8
		 */
		private readonly static byte[] glyph3_utf8 = { (byte)0xC3, (byte)0x9f };

		/**
		 * Unicode code point U+0041 in native Java UTF-16
		 */
		private readonly static char[] glyph4_utf16 = { (char)0x0041 };

		/**
		 * U+0041 coded in UTF-8
		 */
		private readonly static byte[] glyph4_utf8 = { 0x41 };

		private readonly static byte[][] glyphs_utf8 = { glyph1_utf8, glyph2_utf8, glyph3_utf8, glyph4_utf8 };

		private readonly static char[][] glyphs_utf16 = { glyph1_utf16, glyph2_utf16, glyph3_utf16, glyph4_utf16 };

		public ITestResult Perform()
		{
			try
			{
				for (int i = 0; i < glyphs_utf16.Length; i++)
				{
					string s = new string(glyphs_utf16[i]);
					byte[] b1 = new DerUtf8String(s).GetEncoded();
					byte[] temp = new byte[b1.Length - 2];
					Array.Copy(b1, 2, temp, 0, b1.Length - 2);
					byte[] b2 = new DerUtf8String(new DerOctetString(temp).GetOctets()).GetEncoded();
					if (!Arrays.AreEqual(b1, b2))
					{
						return new SimpleTestResult(false, Name + ": failed UTF-8 encoding and decoding");
					}
					if (!Arrays.AreEqual(temp, glyphs_utf8[i]))
					{
						return new SimpleTestResult(false, Name + ": failed UTF-8 encoding and decoding");
					}
				}
			}
			catch (Exception e)
			{
				return new SimpleTestResult(false, Name + ": failed with Exception " + e.Message);
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public string Name
		{
			get
			{
				return "DERUTF8String";
			}
		}

		public static void Main(
			string[] args)
		{
			DerUtf8StringTest test = new DerUtf8StringTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
