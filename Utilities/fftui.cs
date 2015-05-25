using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace MissionPlanner.Utilities
{
    public partial class fftui : Form
    {
        public fftui()
        {
            InitializeComponent();
        }

        private void BUT_run_Click(object sender, EventArgs e)
        {
            Utilities.FFT2 fft = new FFT2();

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "*.wav|*.wav";

            ofd.ShowDialog();

            if (!File.Exists(ofd.FileName))
                return;

            var st = File.OpenRead(ofd.FileName);

            int bins = 10;

            double[] buffer = new double[1 << bins];

            int a = 0;

            while (st.Position < st.Length)
            {
                byte[] temp = new byte[2];
                var read = st.Read(temp, 0, temp.Length);

                var val = (double)BitConverter.ToInt16(temp, 0);

                buffer[a] = val;

                a++;

                if (a == (1 << bins))
                {
                    var fftanswer = fft.rin(buffer, (uint)bins);

                    var freqt = fft.FreqTable(buffer.Length, 1000);

                    ZedGraph.PointPairList ppl = new ZedGraph.PointPairList();

                    for (int b = 0; b < fftanswer.Length; b++)
                    {
                        ppl.Add(freqt[b], fftanswer[b]);
                    }

                    double xMin, xMax, yMin, yMax;

                    var curve = new LineItem("FFT", ppl, Color.Red, SymbolType.Diamond);

                    curve.GetRange(out xMin, out xMax, out yMin, out  yMax, true, false, zedGraphControl1.GraphPane);

                    zedGraphControl1.GraphPane.XAxis.Title.Text = "Freq Hz";
                    zedGraphControl1.GraphPane.YAxis.Title.Text = "Amplitude";
                    zedGraphControl1.GraphPane.Title.Text = "FFT";
                    zedGraphControl1.GraphPane.CurveList.Clear();
                    zedGraphControl1.GraphPane.CurveList.Add(curve);

                    zedGraphControl1.Invalidate();
                    zedGraphControl1.AxisChange();

                    zedGraphControl1.Refresh();

                    int width = Console.WindowWidth - 1;
                    int height = Console.WindowHeight - 1;

                    int r = 1;
                    foreach (var ff in fftanswer)
                    {
                        int col = (int)((r / (double)fftanswer.Length) * width);
                        int row = (int)((ff * 0.2) + 0.5);

                        //Console.SetCursorPosition(col, height - row);
                        Console.Write("*");
                        r++;
                    }

                    // 50% overlap
                    st.Seek(-(1 << bins) / 2, SeekOrigin.Current);
                    a = 0;
                    buffer = new double[buffer.Length];
                    //Console.Clear();
                }
            }
        }

        //FMT, 131, 43, IMU, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp
        //FMT, 135, 43, IMU2, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp
        //FMT, 149, 43, IMU3, IffffffIIf, TimeMS,GyrX,GyrY,GyrZ,AccX,AccY,AccZ,ErrG,ErrA,Temp

        //FMT, 172, 23, ACC1, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 173, 23, ACC2, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 174, 23, ACC3, IIfff, TimeMS,TimeUS,AccX,AccY,AccZ
        //FMT, 175, 23, GYR1, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ
        //FMT, 176, 23, GYR2, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ
        //FMT, 177, 23, GYR3, IIfff, TimeMS,TimeUS,GyrX,GyrY,GyrZ

        private void myButton1_Click(object sender, EventArgs e)
        {
            Utilities.FFT2 fft = new FFT2();

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "*.log|*.log";

            ofd.ShowDialog();

            if (!File.Exists(ofd.FileName))
                return;

            var file = new StreamReader(File.OpenRead(ofd.FileName));

            int bins = (int)NUM_bins.Value;

            int N = 1 << bins;

            double[] datainGX = new double[N];
            double[] datainGY = new double[N];
            double[] datainGZ = new double[N];
            double[] datainAX = new double[N];
            double[] datainAY = new double[N];
            double[] datainAZ = new double[N];

            List<double[]> avg = new List<double[]>();

            // 6
            avg.Add(new double[N / 2]);
            avg.Add(new double[N / 2]);
            avg.Add(new double[N / 2]);
            avg.Add(new double[N / 2]);
            avg.Add(new double[N / 2]);
            avg.Add(new double[N / 2]);

            object[] datas = new object[] { datainGX, datainGY, datainGZ, datainAX, datainAY, datainAZ };
            string[] datashead = new string[] { "GYR1-GyrX", "GYR1-GyrY", "GYR1-GyrZ", "ACC1-AccX", "ACC1-AccY", "ACC1-AccZ" };
            Color[] color = new Color[] { Color.Red, Color.Green, Color.Black, Color.Violet, Color.Blue, Color.Orange };
            ZedGraphControl[] ctls = new ZedGraphControl[] { zedGraphControl1,zedGraphControl2,zedGraphControl3,zedGraphControl4,zedGraphControl5,zedGraphControl6 };

            int samplecount = 0;

            double lasttime = 0;
            double timedelta = 0;
            double[] freqt = null;
            double samplerate = 0;
            

            while (!file.EndOfStream)
            {
                var item = Log.DFLog.GetDFItemFromLine(file.ReadLine(), 0);

                if (item.msgtype == "ACC1")
                {
                    int offsetAX = Log.DFLog.FindMessageOffset("ACC1", "AccX");
                    int offsetAY = Log.DFLog.FindMessageOffset("ACC1", "AccY");
                    int offsetAZ = Log.DFLog.FindMessageOffset("ACC1", "AccZ");
                    int offsetTime = Log.DFLog.FindMessageOffset("ACC1", "TimeUS");

                    double time = double.Parse(item.items[offsetTime]) / 1000.0;

                    if (time != lasttime)
                        timedelta = timedelta * 0.99 + (time - lasttime) * 0.01;

                    datainAX[samplecount] = double.Parse(item.items[offsetAX]);
                    datainAY[samplecount] = double.Parse(item.items[offsetAY]);
                    datainAZ[samplecount] = double.Parse(item.items[offsetAZ]);

                    if (lasttime != time && lasttime != 0)
                        samplecount++;

                    lasttime = time;
                }
                else if (item.msgtype == "GYR1")
                {
                    int offsetGX = Log.DFLog.FindMessageOffset("GYR1", "GyrX");
                    int offsetGY = Log.DFLog.FindMessageOffset("GYR1", "GyrY");
                    int offsetGZ = Log.DFLog.FindMessageOffset("GYR1", "GyrZ");
                    int offsetTime = Log.DFLog.FindMessageOffset("ACC1", "TimeUS");

                    double time = double.Parse(item.items[offsetTime]) / 1000.0;

                    if (time != lasttime)
                        timedelta = timedelta * 0.99 + (time - lasttime) * 0.01;

                    datainGX[samplecount] = double.Parse(item.items[offsetGX]);
                    datainGY[samplecount] = double.Parse(item.items[offsetGY]);
                    datainGZ[samplecount] = double.Parse(item.items[offsetGZ]);

                    if (lasttime != time && lasttime != 0)
                        samplecount++;

                    lasttime = time;
                }

                if (samplecount == N)
                {
                    int inputdataindex = 0;

                    if (freqt == null)
                    {
                        // 1000 16000 800/760
                        if (timedelta > 2 && timedelta < 4) // 2.5
                            samplerate = 400; // 400*2.5 = 1000
                        if (timedelta > 10 && timedelta < 30) // 20
                            samplerate = 50; // 20 * 50 = 1000
                        if (timedelta < 2) // 1
                            samplerate = 1000;
                        if (timedelta < 0.8) // 0.625
                            samplerate = 1600;


                        if (samplerate == 0)
                            samplerate = Math.Round(1000 / timedelta, 1);
                        freqt = fft.FreqTable(N, (int)samplerate);
                    }

                    foreach (var itemlist in datas)
                    {
                        var fftanswer = fft.rin((double[])itemlist, (uint)bins);

                        for (int b = 0; b < N/2; b++)
                        {
                            if (freqt[b] < (double)NUM_startfreq.Value)
                                continue;

                            avg[inputdataindex][b] += fftanswer[b] * (1.0 / (N / 2.0));
                        }

                        samplecount = 0;
                        inputdataindex++;
                    }                
                }
            }

            int controlindex = 0;
            foreach (var item in avg)
            {
                ZedGraph.PointPairList ppl = new ZedGraph.PointPairList(freqt, item);

                //double xMin, xMax, yMin, yMax;

                var curve = new LineItem(datashead[controlindex], ppl, color[controlindex], SymbolType.None);

                //curve.GetRange(out xMin, out xMax, out yMin, out  yMax, true, false, ctls[c].GraphPane);

                ctls[controlindex].GraphPane.Legend.IsVisible = false;

                ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                ctls[controlindex].GraphPane.Title.Text = "FFT " + datashead[controlindex] + " - " + Path.GetFileName(ofd.FileName) + " - " + samplerate + "hz input";

                ctls[controlindex].GraphPane.CurveList.Clear();

                ctls[controlindex].GraphPane.CurveList.Add(curve);

                ctls[controlindex].Invalidate();
                ctls[controlindex].AxisChange();

                ctls[controlindex].Refresh();

                controlindex++;
            }
        }
    }
}
