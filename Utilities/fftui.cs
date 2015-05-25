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
                    var fftanswer = fft.rin(buffer, 1000, (uint)bins);

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
            string[] datashead = new string[] { "GyrX", "GyrY", "GyrZ", "AccX", "AccY", "AccZ" };
            Color[] color = new Color[] { Color.Red, Color.Green, Color.Black, Color.Violet, Color.Blue, Color.Orange };
            ZedGraphControl[] ctls = new ZedGraphControl[] { zedGraphControl1,zedGraphControl2,zedGraphControl3,zedGraphControl4,zedGraphControl5,zedGraphControl6 };

            int samplecount = 0;

            double lasttime = 0;
            double timedelta = 0;
            double[] freqt = null;
            

            while (!file.EndOfStream)
            {
                var item = Log.DFLog.GetDFItemFromLine(file.ReadLine(), 0);

                if (item.msgtype == "IMU")
                {
                    int offsetGX = Log.DFLog.FindMessageOffset("IMU", "GyrX");
                    int offsetGY = Log.DFLog.FindMessageOffset("IMU", "GyrY");
                    int offsetGZ = Log.DFLog.FindMessageOffset("IMU", "GyrZ");
                    int offsetAX = Log.DFLog.FindMessageOffset("IMU", "AccX");
                    int offsetAY = Log.DFLog.FindMessageOffset("IMU", "AccY");
                    int offsetAZ = Log.DFLog.FindMessageOffset("IMU", "AccZ");
                    int offsetTime = Log.DFLog.FindMessageOffset("IMU", "TimeMS");

                    double time = double.Parse(item.items[offsetTime]);

                    if (lasttime == 0)
                        lasttime = time;

                    timedelta = timedelta * 0.9 + (time - lasttime) * 0.1;

                    lasttime = time;

                    datainGX[samplecount] = double.Parse(item.items[offsetGX]);
                    datainGY[samplecount] = double.Parse(item.items[offsetGY]);
                    datainGZ[samplecount] = double.Parse(item.items[offsetGZ]);
                    datainAX[samplecount] = double.Parse(item.items[offsetAX]);
                    datainAY[samplecount] = double.Parse(item.items[offsetAY]);
                    datainAZ[samplecount] = double.Parse(item.items[offsetAZ]);
                    samplecount++;
                }
                else if (item.msgtype == "IMU2")
                {
                    //datainGX[a] = double.Parse(item.items[offsetGX]);
                    //datainGY[a] = double.Parse(item.items[offsetGY]);
                    //datainGZ[a] = double.Parse(item.items[offsetGZ]);
                    //datainAX[a] = double.Parse(item.items[offsetAX]);
                    //datainAY[a] = double.Parse(item.items[offsetAY]);
                    //datainAZ[a] = double.Parse(item.items[offsetAZ]);
                    //a++;
                }

                if (samplecount == N)
                {
                    int inputdataindex = 0;

                    if (freqt == null)
                        freqt = fft.FreqTable(N, (int)Math.Round(1000 / timedelta,0));

                    foreach (var itemlist in datas)
                    {
                        var fftanswer = fft.rin((double[])itemlist, 1000, (uint)bins);

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
                ctls[controlindex].GraphPane.Title.Text = "FFT " + datashead[controlindex];

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
