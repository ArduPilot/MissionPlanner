using System;
using System.Collections.Generic;
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

        public static Image<Rgba32> GenerateImage(DFLogBuffer cb, out double[] freqtout, out List<(double timeus, double[] value)> allfftdata, string type= "ACC1", string field= "AccX", string timeus = "TimeUS")
        {
            allfftdata = new List<(double timeus, double[] value)>();
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
            var totalmax = 2.0;
            while (count > 1) // skip last part
            {
                var fftdata = acc1data.Skip((int)(N * (done/ (double)divisor))).Take(N);

                if (fftdata.Count() < N)
                {
                    break;
                }

                var timeusvalue = double.Parse(fftdata.Min(a => a[timeus]));

                timedelta = timedelta * 0.99 + (timeusvalue - lasttime) * 0.01;
                lasttime = timeusvalue;

                //var time = fftdata.Skip(N / 2).First().time.ToString("o");

                var fftanswerz = fft.rin(fftdata.Select(a => (double) (float) a.GetRaw(field)).ToArray(), (uint) bins);

                allfftdata.Add((timeusvalue, fftanswerz));

                var min = fftanswerz.Min();
                var max = fftanswerz.Max();
                totalmax = Math.Max(totalmax, Math.Min(max, 2));
                //plotlydata.root.z.Add(fftanswerz.Select(a => a > 2 ? 0 : a).ToArray());
                freqt.Select((y, i) => img[done, (freqt.Length - 1) - i] = GetColor(fftanswerz[i], totalmax)).ToArray();

                count--;
                done++;
            }

            freqtout = fft.FreqTable(N, (int) (1.0 / (N / (timedelta * divisor))));

            return img;
        }

        static Rgba32 GetColor(double actualValue, double max)
        {
            var scale = 20 * Math.Log10(actualValue / max);

            scale = MathHelper.mapConstrained(scale, -100, 0, 0, 255);

            return new Rgba32((byte) scale, (byte) (255 - scale), (byte) (0));

            return new Rgba32((byte)scale, (byte)255 - (byte)scale, (byte)0, (byte)255);
        }
    }
}
