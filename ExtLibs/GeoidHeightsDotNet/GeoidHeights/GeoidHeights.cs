using System;
using System.Collections.Generic;
using System.Text;

namespace GeoidHeightsDotNet
{
    public static class GeoidHeights
    {
        const int l_value = 65341;
        const int nmax = 360;

        static double[] drts = new double[1301];
        static double[] dirt = new double[1301];

        static GeoidHeights()
        {
            int nmx2p = 2 * nmax + 1;

            for (int n = 1; n <= nmx2p; n++)
            {
                drts[n] = Math.Sqrt(n);
                dirt[n] = 1 / drts[n];
            }
        }

        public static double undulation(double degLat, double degLon)
        {
            double lat = degLat * Math.PI / 180;
            double lon = degLon * Math.PI / 180;

            double rlat, gr, re;
            int i, j, m;
            int k = nmax + 1;
            double[] p = new double[l_value + 1];
            double[] sinml = new double[361 + 1];
            double[] cosml = new double[361 + 1];
            
            radgra(lat, lon, out rlat, out gr, out re);
            rlat = Math.PI / 2 - rlat;
            double[] rleg = new double[361 + 1];

            for (j = 1; j <= k; j++)
            {
                m = j - 1;
                legfdn(m, rlat, rleg);
                for (i = j; i <= k; i++)
                    p[(i - 1) * i / 2 + m + 1] = rleg[i];
            }

            dscml(lon, sinml, cosml);

            return hundu(p, sinml, cosml, gr, re);
        }

        static void radgra(double lat, double lon, out double rlat, out double gr, out double re)
        /*this subroutine computes geocentric distance to the point,
        the geocentric latitude,and
        an approximate value of normal gravity at the point based
        the constants of the wgs84(g873) system are used*/
        {
            const double a = 6378137;
            const double e2 = .00669437999013;
            const double geqt = 9.7803253359;
            const double k = .00193185265246;

            double n;
            double t1 = Math.Sin(lat) * Math.Sin(lat), t2, x, y, z;
            n = a / Math.Sqrt(1 - e2 * t1);
            t2 = n * Math.Cos(lat);
            x = t2 * Math.Cos(lon);
            y = t2 * Math.Sin(lon);
            z = (n * (1 - e2)) * Math.Sin(lat);
            re = Math.Sqrt(x * x + y * y + z * z);/*compute the geocentric radius*/
            rlat = Math.Atan(z / Math.Sqrt(x * x + y * y));/*compute the geocentric latitude*/
            gr = geqt * (1 + k * t1) / Math.Sqrt(1 - e2 * t1);/*compute normal gravity:units are m/sec**2*/
        }

        static void legfdn(int m, double theta, double[] rleg)
        /*this subroutine computes  all normalized legendre function
        in "rleg". order is always
        m, and colatitude is always theta  (radians). maximum deg
        is  nmx. all calculations in double precision.
        ir  must be set to zero before the first call to this sub.
        the dimensions of arrays  rleg must be at least equal to  nmx+1.
        Original programmer :Oscar L. Colombo, Dept. of Geodetic Science
        the Ohio State University, August 1980
        ineiev: I removed the derivatives, for they are never computed here*/
        {
            double cothet;
            double sithet;
            double[] rlnn = new double[361 + 1];
            int nmx1 = nmax + 1;
            int m1 = m + 1;
            int m2 = m + 2;
            int m3 = m + 3;
            int n, n1, n2;

            cothet = Math.Cos(theta);
            sithet = Math.Sin(theta);

            /*compute the legendre functions*/
            rlnn[1] = 1;
            rlnn[2] = sithet * drts[3];
            for (n1 = 3; n1 <= m1; n1++)
            {
                n = n1 - 1;
                n2 = 2 * n;
                rlnn[n1] = drts[n2 + 1] * dirt[n2] * sithet * rlnn[n];
            }
            switch (m)
            {
                case 1:
                    rleg[2] = rlnn[2];
                    rleg[3] = drts[5] * cothet * rleg[2];
                    break;
                case 0:
                    rleg[1] = 1;
                    rleg[2] = cothet * drts[3];
                    break;
            }
            rleg[m1] = rlnn[m1];
            if (m2 <= nmx1)
            {
                rleg[m2] = drts[m1 * 2 + 1] * cothet * rleg[m1];
                if (m3 <= nmx1)
                    for (n1 = m3; n1 <= nmx1; n1++)
                    {
                        n = n1 - 1;
                        if ((m == 0 && n < 2) || (m == 1 && n < 3)) continue;
                        n2 = 2 * n;
                        rleg[n1] = drts[n2 + 1] * dirt[n + m] * dirt[n - m] *
                                (drts[n2 - 1] * cothet * rleg[n1 - 1] - drts[n + m - 1] * drts[n - m - 1] * dirt[n2 - 3] * rleg[n1 - 2]);
                    }
            }
        }

        static double hundu(double[] p, double[] sinml, double[] cosml, double gr, double re)
        {
            /*constants for wgs84(g873);gm in units of m**3/s**2*/
            const double gm = .3986004418e15;
            const double ae = 6378137;
            double arn, ar, ac, a, sum, sumc, tempc, temp;
            int k, n, m;
            ar = ae / re;
            arn = ar;
            ac = a = 0;
            k = 3;
            for (n = 2; n <= nmax; n++)
            {
                arn *= ar;
                k++;
                sum = p[k] * Coef.hc[k];
                sumc = p[k] * Coef.cc[k];
                for (m = 1; m <= n; m++)
                {
                    k++;
                    tempc = Coef.cc[k] * cosml[m] + Coef.cs[k] * sinml[m];
                    temp = Coef.hc[k] * cosml[m] + Coef.hs[k] * sinml[m];
                    sumc += p[k] * tempc;
                    sum += p[k] * temp;
                }
                ac += sumc;
                a += sum * arn;
            }
            ac += Coef.cc[1] + p[2] * Coef.cc[2] + p[3] * (Coef.cc[3] * cosml[1] + Coef.cs[3] * sinml[1]);
            /*add haco=ac/100 to convert height anomaly on the ellipsoid to the undulation
            add -0.53m to make undulation refer to the wgs84 ellipsoid.*/
            return a * gm / (gr * re) + ac / 100 - .53;
        }

        static void dscml(double rlon, double[] sinml, double[] cosml)
        {
            double a, b;
            int m;
            a = Math.Sin(rlon);
            b = Math.Cos(rlon);
            sinml[1] = a;
            cosml[1] = b;
            sinml[2] = 2 * b * a;
            cosml[2] = 2 * b * b - 1;
            for (m = 3; m <= nmax; m++)
            {
                sinml[m] = 2 * b * sinml[m - 1] - sinml[m - 2];
                cosml[m] = 2 * b * cosml[m - 1] - cosml[m - 2];
            }
        }
    }
}
