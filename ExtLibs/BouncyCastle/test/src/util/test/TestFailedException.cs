using System;

namespace Org.BouncyCastle.Utilities.Test
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class TestFailedException
        : Exception
    {
        private ITestResult _result;

        public TestFailedException(
            ITestResult result)
        {
            _result = result;
        }

        public ITestResult GetResult()
        {
            return _result;
        }
    }
}
