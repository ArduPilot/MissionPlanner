using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin
{
    public class Test
    {
        public static ITest TestMethod { get; set; }
    }
    public interface ITest
    {
        void DoUSB();
    }
}
