using System;
using System.Text;

namespace Org.BouncyCastle.Utilities.Test
{
    public class SimpleTestResult : ITestResult
    {
        private static readonly string Separator = SimpleTest.NewLine;

        private bool success;
        private string message;
        private Exception exception;

        public SimpleTestResult(
			bool	success,
			string	message)
        {
            this.success = success;
            this.message = message;
        }

        public SimpleTestResult(
			bool		success,
			string		message,
			Exception	exception)
        {
            this.success = success;
            this.message = message;
            this.exception = exception;
        }

		public static ITestResult Successful(
            ITest	test,
            string	message)
        {
            return new SimpleTestResult(true, test.Name + ": " + message);
        }

        public static ITestResult Failed(
            ITest	test,
            string	message)
        {
            return new SimpleTestResult(false, test.Name + ": " + message);
        }

        public static ITestResult Failed(
            ITest		test,
            string		message,
            Exception	t)
        {
            return new SimpleTestResult(false, test.Name + ": " + message, t);
        }

        public static ITestResult Failed(
            ITest	test,
            string	message,
            object	expected,
            object	found)
        {
            return Failed(test, message + Separator + "Expected: " + expected + Separator + "Found   : " + found);
        }

        public static string FailedMessage(
			string	algorithm,
			string	testName,
			string	expected,
            string	actual)
        {
            StringBuilder sb = new StringBuilder(algorithm);
            sb.Append(" failing ").Append(testName);
            sb.Append(Separator).Append("    expected: ").Append(expected);
            sb.Append(Separator).Append("    got     : ").Append(actual);
            return sb.ToString();
        }

		public bool IsSuccessful()
        {
            return success;
        }

        public override string ToString()
        {
            return message;
        }

		public Exception GetException()
        {
            return exception;
        }
    }
}
