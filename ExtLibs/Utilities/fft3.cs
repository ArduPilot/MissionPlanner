using System;
using System.Collections.Generic;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class fft3
    {
        public int N;

        // forward coeffs -2 PI e^iw -- normalized (divided by N)
        alglib.complex[] coeffs;
        // inverse coeffs 2 PI e^iw
        alglib.complex[] icoeffs;

        public alglib.complex[] freqs;

        public double[] @in;

        alglib.complex oldest_data, newest_data;

        int idx;

        public fft3(int N)
        {
            this.N = N;

            coeffs = new alglib.complex[N];
            icoeffs = new alglib.complex[N];
            freqs = new alglib.complex[N+1];
            @in = new double[N];


            init();
        }

        void init_coeffs()
        {
            for (int i = 0; i < N; ++i)
            {
                double a = -2.0 * PI * i / (double)(N);
                coeffs[i] = new alglib.complex(cos(a)/* / N */, sin(a) /* / N */);
            }
            for (int i = 0; i < N; ++i)
            {
                double a = 2.0 * PI * i / (double)(N);
                icoeffs[i] =new  alglib.complex(cos(a), sin(a));
            }
        }

        void init()
        {
            // clear data
            for (int i = 0; i < N; ++i)
                @in[i] = 0;
            // seed rand()
            //srand(857);
            init_coeffs();
            oldest_data = newest_data = 0.0;
            idx = 0;
        }

        public void add_data(double inp)
        {
            oldest_data = @in[idx] ;
            newest_data = @in[idx] = inp / (double)(N);
        }

        // sliding dft
        public void sdft()
        {
            alglib.complex delta = newest_data - oldest_data;
            int ci = 0;
            for (int i = 0; i < N; ++i)
            {
                freqs[i] += delta * coeffs[ci];
                if ((ci += idx) >= N)
                    ci -= N;
            }

            if (++idx == N) idx = 0;
        }

        public void isdft()
        {
            alglib.complex delta = newest_data - oldest_data;
            int ci = 0;
            for (int i = 0; i < N; ++i)
            {
                freqs[i] += delta * icoeffs[ci];
                if ((ci += idx) >= N)
                    ci -= N;
            }

            if (++idx == N) idx = 0;
        }

        double mag(alglib.complex c)
        {
            return sqrt(c.x * c.x + c.y * c.y);
        }

        public double[] powr_spectrum()
        {
            double SCALE = 20 / Math.Log(10);
            double[] powr = new double[N];
            for (int i = 0; i < N / 2; ++i)
            {                
                powr[i] = mag(freqs[i]);

                powr[i] = SCALE * Math.Log(powr[i] + double.Epsilon);
            }

            return powr;
        }

        private const double PI = Math.PI;

        public double cos(double d) => Math.Cos(d);
        public double sin(double d) => Math.Sin(d);
        public double sqrt(double d) => Math.Sqrt(d);

    }
}
