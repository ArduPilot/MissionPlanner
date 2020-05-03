using System;

using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class TimeTest
	{
		[Test]
		public void CheckCmsTimeVsX509Time()
		{
			DateTime now = DateTime.UtcNow;

			// Time classes only have a resolution of seconds
			now = SimpleTest.MakeUtcDateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            Org.BouncyCastle.Asn1.Cms.Time cmsTime = new Org.BouncyCastle.Asn1.Cms.Time(now);
			Org.BouncyCastle.Asn1.X509.Time x509Time = new Org.BouncyCastle.Asn1.X509.Time(now);

//			Assert.AreEqual(cmsTime.Date, x509Time.ToDateTime());
			Assert.AreEqual(now, cmsTime.Date);
			Assert.AreEqual(now, x509Time.ToDateTime());
		}
	}
}
