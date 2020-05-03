#if !LIB
using System;

using NUnit.Core;
using NUnit.Framework;

using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tsp.Tests
{
    public class AllTests
    {
        public static void Main(string[] args)
        {
            Suite.Run(new NullListener(), NUnit.Core.TestFilter.Empty);
        }

        [Suite]
        public static TestSuite Suite
        {
            get
            {
                TestSuite suite = new TestSuite("TSP Tests");
                suite.Add(new GenTimeAccuracyUnitTest());
                suite.Add(new ParseTest());
                suite.Add(new TimeStampTokenInfoUnitTest());
                suite.Add(new TspTest());
                return suite;
            }
        }
    }
}
#endif
