using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utils
{
    public static class MathUtils
    {
        public static double GetTrueRandom() {
            int seed = (int)DateTime.Now.Ticks;
            Random random = new Random(seed);
            return random.NextDouble();
        }
        public static double GetTrueRandom(double low, double high) {
            return low + (high - low) * GetTrueRandom();
        }
        public static int GetTrueRandom(int low, int high) {
            return (int)Math.Round(GetTrueRandom((double)low, (double)high));
        }

        public static double GetScale(double max) {
            // based on Excel graphs, first adapted for ProphIT
            // this implementation assumes everything is positive
            int i = 1;
            int state = 0;
            while (true) {
                i *= 10;
                if (max < (double)i) {
                    state = 1;
                    break;
                }
                if (max < (double)(2 * i)) {
                    state = 2;
                    break;
                }
                if (max < (double)(5 * i)) {
                    state = 5;
                    break;
                }
            }
            int stepSize = Convert.ToInt32(i / 10) * state;
            int numSteps = Convert.ToInt32(Math.Ceiling(max / stepSize)) + 1;
            return (double)numSteps * stepSize;
        }

        public static double Pow(double x, int n) {
            if (n == 0) {
                return 1.0;
            }
            double ans = 1.0;
            int N = Math.Abs(n);
            for (int i = 0; i < N; i++) {
                ans *= x;
            }
            return (n > 0) ? ans : 1.0 / ans;
        }
    }
}
