using System;

namespace Org.BouncyCastle.Utilities.Test
{
    public interface ITestResult
    {
        bool IsSuccessful();

        Exception GetException();

        string ToString();
    }
}
