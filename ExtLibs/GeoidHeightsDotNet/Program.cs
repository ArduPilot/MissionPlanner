using System;
using System.Collections.Generic;
using System.Text;

namespace GeoidHeightsDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            double h = GeoidHeights.undulation(38.6281550, 269.7791550);
            Console.WriteLine(h);
            h = GeoidHeights.undulation(-14.621217, 305.021114);
            Console.WriteLine(h);
            h = GeoidHeights.undulation(46.874319, 102.448729);
            Console.WriteLine(h);
            h = GeoidHeights.undulation(-23.617446, 133.874712);
            Console.WriteLine(h);
            h = GeoidHeights.undulation(38.625473, 359.999500);
            Console.WriteLine(h);
            h = GeoidHeights.undulation(-00.466744, 0.002300);
            Console.WriteLine(h);
        }
    }
}
