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

        public static Image<Rgba32> GenerateImage(DFLogBuffer cb, string type= "ACC1", string field= "AccX", string timeus = "TimeUS")
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

            double[] freqt = fft.FreqTable(N, (int) (1000 / (N / timedelta)));

            // time , freq , [color] &freqcount

            int totalsamples = acc1data.Count();
            int count = totalsamples / N;
            int done = 0;
            // 50% overlap
            int divisor = 2;
            count *= divisor; 
            var img = new Image<Rgba32>(count, freqt.Length);
            log.Debug("done and count ");
            while (count > 1) // skip last part
            {
                var fftdata = acc1data.Skip((N * (done/ divisor))).Take(N);

                var time = fftdata.Skip(N / 2).First().time.ToString("o");

                var fftanswerz = fft.rin(fftdata.Select(a => (double) (float) a.GetRaw(field)).ToArray(), (uint) bins);

                //plotlydata.root.z.Add(fftanswerz.Select(a => a > 2 ? 0 : a).ToArray());
                freqt.Select((y, i) => img[done, (freqt.Length-1) - i] = GetColor(fftanswerz[i])).ToArray();

                count--;
                done++;
            }

            return img;
        }

        static Rgba32 GetColor(double actualValue)
        {
            var scale = (actualValue / 2.0) * 255.0;

            if (actualValue > 2)
                scale = 255;

            return new Rgba32((byte)scale, 255 - (byte)scale, 0, 255);
        }
    }
}
