using System;

using NUnit.Framework;

/*
 Basic test interface
 */
namespace Org.BouncyCastle.Utilities.Test
{
    public interface ITest
    {
        string Name { get; }

		[Test]
        ITestResult Perform();
    }
}
