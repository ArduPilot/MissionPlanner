using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKml.Dom;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MissionPlanner.Utilities
{
    public class Spectrogram
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Image<Rgba32> GenerateImage(DFLogBuffer cb, out double[] freqtout,
            out List<(double timeus, double[] value)> allfftdata, string type = "ACC1", string field = "AccX",
            string timeus = "TimeUS", int min = -80, int max = -20)
        {
            allfftdata = new List<(double timeus, double[] value)>();

            if (cb.SeenMessageTypes.Contains("ISBH"))
            {
                int sensorno = (type.Contains("1") ? 0 : (type.Contains("2") ? 1 : 2));
                int sensor = type.Contains("ACC") ? 0 : 1;
                int Ns = -1;
                int type1 = -1;
                int instance = -1;
                double sample_rate = -1;
                double multiplier = -1;

                var data = cb.GetEnumeratorType(new string[] {"ISBH", "ISBD"})
                    .SelectMany(
                        item =>
                        {
                            if (item.msgtype == "ISBH")
                            {
                                Ns = int.Parse(item.items[cb.dflog.FindMessageOffset(item.msgtype, "N")],
                                    CultureInfo.InvariantCulture);
                                type1 = int.Parse(item.items[cb.dflog.FindMessageOffset(item.msgtype, "type")],
                                    CultureInfo.InvariantCulture);
                                instance = int.Parse(item.items[cb.dflog.FindMessageOffset(item.msgtype, "instance")],
                                    CultureInfo.InvariantCulture);

                                if (instance != sensorno || type1 != sensor)
                                    return new (double time, double d)[0];
                                
                                sample_rate = double.Parse(
                                    item.items[cb.dflog.FindMessageOffset(item.msgtype, "smp_rate")],
                                    CultureInfo.InvariantCulture);

                                multiplier = double.Parse(
                                    item.items[cb.dflog.FindMessageOffset(item.msgtype, "mul")],
                                    CultureInfo.InvariantCulture);
                            }
                            else if (item.msgtype.StartsWith("ISBD"))
                            {
                                var Nsdata = int.Parse(item.items[cb.dflog.FindMessageOffset(item.msgtype, "N")],
                                    CultureInfo.InvariantCulture);

                                if (Ns != Nsdata)
                                    return new (double time, double d)[0];

                                if (instance != sensorno || type1 != sensor)
                                    return new (double time, double d)[0];

                                //int offsetX = cb.dflog.FindMessageOffset(item.msgtype, "x");
                                //int offsetY = cb.dflog.FindMessageOffset(item.msgtype, "y");
                                //int offsetZ = cb.dflog.FindMessageOffset(item.msgtype, "z");
                                int offsetTime = cb.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                                double time = double.Parse(item.items[offsetTime], CultureInfo.InvariantCulture);

                                var ua = (BinaryLog.UnionArray) item.GetRaw(field.ToLower()
                                    .Substring(field.Length - 1));

                                return ua.Shorts.Take(ua.ShortsLength).Select(ds =>
                                    {
                                        double d = ((double) ds / multiplier);
                                        return (time, d);
                                    })
                                    .ToArray();
                            }

                            return new (double time, double d)[0];
                        }).ToArray();

                var bins = 10;
                int N = 1 << bins;

                var fft = new FFT2();

                var freqt = fft.FreqTable(N, (int)sample_rate);

                // time , freq , [color] &freqcount

                double lasttime = 0;
                int totalsamples = data.Count();
                int count = totalsamples / N;
                int done = 0;
                // 50% overlap
                int divisor = 4;
                count *= divisor;
                var img = new Image<Rgba32>(count, freqt.Length);
                log.Debug("done and count ");
                while (count > 1) // skip last part
                {
                    var fftdata = data.Skip((int) (N * (done / (double) divisor))).Take(N);

                    if (fftdata.Count() < N)
                    {
                        break;
                    }

                    var timeusvalue = fftdata.Min(a => a.time);

                    var fftanswerz = fft.rin(fftdata.Select(a => (double) a.d).ToArray(),
                        (uint) bins);

                    allfftdata.Add((timeusvalue, fftanswerz));

                    //plotlydata.root.z.Add(fftanswerz.Select(a => a > 2 ? 0 : a).ToArray());
                    freqt.Select((y, i) => img[done, (freqt.Length - 1) - i] = GetColor(fftanswerz[i], min, max))
                        .ToArray();

                    count--;
                    done++;
                }

                freqtout = freqt;

                return img;
            }

            {
                var bins = 10;
                int N = 1 << bins;

                var fft = new FFT2();

                var acc1data = cb.GetEnumeratorType(type).ToArray();
                log.Debug(type);
                var firstsample = acc1data.Take(N);
                var samplemin = double.Parse(firstsample.Min(a => a[timeus]));
                var samplemax = double.Parse(firstsample.Max(a => a[timeus]));

                log.Debug("samplemin " + samplemin + " samplemax " + samplemax);

                var timedelta = samplemin - samplemax;

                log.Debug(" timedelta " + timedelta);

                var freqt = fft.FreqTable(N, (int) (1000 / (N / timedelta)));

                // time , freq , [color] &freqcount

                double lasttime = 0;
                int totalsamples = acc1data.Count();
                int count = totalsamples / N;
                int done = 0;
                // 50% overlap
                int divisor = 4;
                count *= divisor;
                var img = new Image<Rgba32>(count, freqt.Length);
                log.Debug("done and count ");
                while (count > 1) // skip last part
                {
                    var fftdata = acc1data.Skip((int) (N * (done / (double) divisor))).Take(N);

                    if (fftdata.Count() < N)
                    {
                        break;
                    }

                    var timeusvalue = double.Parse(fftdata.Min(a => a[timeus]));

                    timedelta = timedelta * 0.99 + (timeusvalue - lasttime) * 0.01;
                    lasttime = timeusvalue;

                    //var time = fftdata.Skip(N / 2).First().time.ToString("o");

                    var fftanswerz = fft.rin(fftdata.Select(a => (double) (float) a.GetRaw(field)).ToArray(),
                        (uint) bins);

                    allfftdata.Add((timeusvalue, fftanswerz));

                    //plotlydata.root.z.Add(fftanswerz.Select(a => a > 2 ? 0 : a).ToArray());
                    freqt.Select((y, i) => img[done, (freqt.Length - 1) - i] = GetColor(fftanswerz[i], min, max))
                        .ToArray();

                    count--;
                    done++;
                }

                freqtout = fft.FreqTable(N, (int) (1.0 / (N / (timedelta * divisor))));

                return img;
            }
        }

        static double SCALE = 20 / Math.Log(10);

        static Rgba32 GetRainbowColor(byte i)
        {
            return HSL2RGB(i / 255.0, 0.5, 0.5);
        }

        public static Rgba32 HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l; // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int) h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            Rgba32 rgb = new Rgba32(0, 0, 0);
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(g * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            return rgb;
        }

        static Rgba32 GetColor(double actualValue, int min = -80, int max = -20)
        {
            var scale = SCALE * Math.Log(actualValue + double.Epsilon);

            scale = MathHelper.mapConstrained(scale, min, max, 0, 255);

            return GetRainbowColor((byte) (255-scale));
        }
    }
}