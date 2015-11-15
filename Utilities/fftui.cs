﻿using System;
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

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
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

                    var val = (double) BitConverter.ToInt16(temp, 0);

                    buffer[a] = val;

                    a++;

                    if (a == (1 << bins))
                    {
                        var fftanswer = fft.rin(buffer, (uint) bins);

                        var freqt = fft.FreqTable(buffer.Length, 1000);

                        ZedGraph.PointPairList ppl = new ZedGraph.PointPairList();

                        for (int b = 0; b < fftanswer.Length; b++)
                        {
                            ppl.Add(freqt[b], fftanswer[b]);
                        }

                        double xMin, xMax, yMin, yMax;

                        var curve = new LineItem("FFT", ppl, Color.Red, SymbolType.Diamond);

                        curve.GetRange(out xMin, out xMax, out yMin, out yMax, true, false, zedGraphControl1.GraphPane);

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
                            int col = (int) ((r/(double) fftanswer.Length)*width);
                            int row = (int) ((ff*0.2) + 0.5);

                            //Console.SetCursorPosition(col, height - row);
                            Console.Write("*");
                            r++;
                        }

                        // 50% overlap
                        st.Seek(-(1 << bins)/2, SeekOrigin.Current);
                        a = 0;
                        buffer = new double[buffer.Length];
                        //Console.Clear();
                    }
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

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log|*.log";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new StreamReader(File.OpenRead(ofd.FileName));

                int bins = (int) NUM_bins.Value;

                int N = 1 << bins;

                double[] datainGX = new double[N];
                double[] datainGY = new double[N];
                double[] datainGZ = new double[N];
                double[] datainAX = new double[N];
                double[] datainAY = new double[N];
                double[] datainAZ = new double[N];

                List<double[]> avg = new List<double[]>();

                // 6
                avg.Add(new double[N/2]);
                avg.Add(new double[N/2]);
                avg.Add(new double[N/2]);
                avg.Add(new double[N/2]);
                avg.Add(new double[N/2]);
                avg.Add(new double[N/2]);

                object[] datas = new object[] {datainGX, datainGY, datainGZ, datainAX, datainAY, datainAZ};
                string[] datashead = new string[]
                {"GYR1-GyrX", "GYR1-GyrY", "GYR1-GyrZ", "ACC1-AccX", "ACC1-AccY", "ACC1-AccZ"};
                Color[] color = new Color[]
                {Color.Red, Color.Green, Color.Black, Color.Violet, Color.Blue, Color.Orange};
                ZedGraphControl[] ctls = new ZedGraphControl[]
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                int samplecounta = 0;
                int samplecountg = 0;

                double lasttime = 0;
                double timedelta = 0;
                double[] freqt = null;
                double samplerate = 0;

                Log.DFLog dflog = new Log.DFLog();

                while (!file.EndOfStream)
                {
                    var item = dflog.GetDFItemFromLine(file.ReadLine(), 0);

                    if (item.msgtype == "ACC1")
                    {
                        int offsetAX = dflog.FindMessageOffset("ACC1", "AccX");
                        int offsetAY = dflog.FindMessageOffset("ACC1", "AccY");
                        int offsetAZ = dflog.FindMessageOffset("ACC1", "AccZ");
                        int offsetTime = dflog.FindMessageOffset("ACC1", "TimeUS");

                        double time = double.Parse(item.items[offsetTime])/1000.0;

                        timedelta = timedelta*0.99 + (time - lasttime)*0.01;

                        // we missed gyro data
                        if (samplecounta >= N)
                            continue;

                        datainAX[samplecounta] = double.Parse(item.items[offsetAX]);
                        datainAY[samplecounta] = double.Parse(item.items[offsetAY]);
                        datainAZ[samplecounta] = double.Parse(item.items[offsetAZ]);

                        samplecounta++;

                        lasttime = time;
                    }
                    else if (item.msgtype == "GYR1")
                    {
                        int offsetGX = dflog.FindMessageOffset("GYR1", "GyrX");
                        int offsetGY = dflog.FindMessageOffset("GYR1", "GyrY");
                        int offsetGZ = dflog.FindMessageOffset("GYR1", "GyrZ");
                        int offsetTime = dflog.FindMessageOffset("ACC1", "TimeUS");

                        double time = double.Parse(item.items[offsetTime])/1000.0;

                        // we missed accel data
                        if (samplecountg >= N)
                            continue;

                        datainGX[samplecountg] = double.Parse(item.items[offsetGX]);
                        datainGY[samplecountg] = double.Parse(item.items[offsetGY]);
                        datainGZ[samplecountg] = double.Parse(item.items[offsetGZ]);

                        samplecountg++;
                    }

                    if (samplecounta >= N && samplecountg >= N)
                    {
                        int inputdataindex = 0;

                        foreach (var itemlist in datas)
                        {
                            var fftanswer = fft.rin((double[]) itemlist, (uint) bins);

                            for (int b = 0; b < N/2; b++)
                            {
                                avg[inputdataindex][b] += fftanswer[b]*(1.0/(N/2.0));
                            }

                            samplecounta = 0;
                            samplecountg = 0;
                            inputdataindex++;
                        }
                    }
                }

                if (freqt == null)
                {
                    samplerate = Math.Round(1000/timedelta, 1);
                    freqt = fft.FreqTable(N, (int) samplerate);
                }

                // 0 out all data befor cutoff
                for (int inputdataindex = 0; inputdataindex < 6; inputdataindex++)
                {
                    for (int b = 0; b < N/2; b++)
                    {
                        if (freqt[b] < (double) NUM_startfreq.Value)
                        {
                            avg[inputdataindex][b] = 0;
                            continue;
                        }

                        break;
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
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + datashead[controlindex] + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curve);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].Refresh();

                    controlindex++;
                }
            }
        }

        class datastate
        {
            public string type;
            public double timedelta;
            public double lasttime;
            public List<double> datax = new List<double>();
            public List<double> datay = new List<double>();
            public List<double> dataz = new List<double>();
        }

        private void BUT_log2_Click(object sender, EventArgs e)
        {
            Utilities.FFT2 fft = new FFT2();
            using (
                OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "*.log|*.log";

                ofd.ShowDialog();

                if (!File.Exists(ofd.FileName))
                    return;

                var file = new StreamReader(File.OpenRead(ofd.FileName));

                int bins = (int) NUM_bins.Value;

                int N = 1 << bins;

                Color[] color = new Color[]
                {Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange};
                ZedGraphControl[] ctls = new ZedGraphControl[]
                {
                    zedGraphControl1, zedGraphControl2, zedGraphControl3, zedGraphControl4, zedGraphControl5,
                    zedGraphControl6
                };

                // 3 imus * 2 sets of measurements(gyr/acc)
                datastate[] alldata = new datastate[3*2];
                for (int a = 0; a < alldata.Length; a++)
                    alldata[a] = new datastate();

                Log.DFLog dflog = new Log.DFLog();

                while (!file.EndOfStream)
                {
                    var item = dflog.GetDFItemFromLine(file.ReadLine(), 0);

                    if (item.msgtype == null)
                    {
                        continue;
                    }

                    if (item.msgtype.StartsWith("ACC"))
                    {
                        int sensorno = int.Parse(item.msgtype.Substring(3)) - 1 + 3;
                        alldata[sensorno].type = item.msgtype;

                        int offsetAX = dflog.FindMessageOffset(item.msgtype, "AccX");
                        int offsetAY = dflog.FindMessageOffset(item.msgtype, "AccY");
                        int offsetAZ = dflog.FindMessageOffset(item.msgtype, "AccZ");
                        int offsetTime = dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        double time = double.Parse(item.items[offsetTime])/1000.0;

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta*0.99 +
                                                          (time - alldata[sensorno].lasttime)*0.01;

                        alldata[sensorno].lasttime = time;

                        alldata[sensorno].datax.Add(double.Parse(item.items[offsetAX]));
                        alldata[sensorno].datay.Add(double.Parse(item.items[offsetAY]));
                        alldata[sensorno].dataz.Add(double.Parse(item.items[offsetAZ]));
                    }
                    else if (item.msgtype.StartsWith("GYR"))
                    {
                        int sensorno = int.Parse(item.msgtype.Substring(3)) - 1;
                        alldata[sensorno].type = item.msgtype;

                        int offsetGX = dflog.FindMessageOffset(item.msgtype, "GyrX");
                        int offsetGY = dflog.FindMessageOffset(item.msgtype, "GyrY");
                        int offsetGZ = dflog.FindMessageOffset(item.msgtype, "GyrZ");
                        int offsetTime = dflog.FindMessageOffset(item.msgtype, "TimeUS");

                        double time = double.Parse(item.items[offsetTime])/1000.0;

                        if (time != alldata[sensorno].lasttime)
                            alldata[sensorno].timedelta = alldata[sensorno].timedelta*0.99 +
                                                          (time - alldata[sensorno].lasttime)*0.01;

                        alldata[sensorno].lasttime = time;

                        alldata[sensorno].datax.Add(double.Parse(item.items[offsetGX]));
                        alldata[sensorno].datay.Add(double.Parse(item.items[offsetGY]));
                        alldata[sensorno].dataz.Add(double.Parse(item.items[offsetGZ]));
                    }
                }

                int controlindex = 0;

                foreach (var sensordata in alldata)
                {
                    if (sensordata.datax.Count <= N)
                        continue;

                    double samplerate = 0;

                    samplerate = Math.Round(1000/sensordata.timedelta, 1);

                    double[] freqt = fft.FreqTable(N, (int) samplerate);

                    double[] avgx = new double[N/2];
                    double[] avgy = new double[N/2];
                    double[] avgz = new double[N/2];

                    int totalsamples = sensordata.datax.Count;
                    int count = totalsamples/N;
                    int done = 0;
                    while (count > 1) // skip last part
                    {
                        var fftanswerx = fft.rin(sensordata.datax.Skip(N*done).Take(N).ToArray(), (uint) bins);
                        var fftanswery = fft.rin(sensordata.datay.Skip(N*done).Take(N).ToArray(), (uint) bins);
                        var fftanswerz = fft.rin(sensordata.dataz.Skip(N*done).Take(N).ToArray(), (uint) bins);

                        for (int b = 0; b < N/2; b++)
                        {
                            if (freqt[b] < (double) NUM_startfreq.Value)
                                continue;

                            avgx[b] += fftanswerx[b]/(N/2);
                            avgy[b] += fftanswery[b]/(N/2);
                            avgz[b] += fftanswerz[b]/(N/2);
                        }

                        count--;
                        done++;
                    }

                    ZedGraph.PointPairList pplx = new ZedGraph.PointPairList(freqt, avgx);
                    ZedGraph.PointPairList pply = new ZedGraph.PointPairList(freqt, avgy);
                    ZedGraph.PointPairList pplz = new ZedGraph.PointPairList(freqt, avgz);

                    var curvex = new LineItem(sensordata.type + " x", pplx, color[0], SymbolType.None);
                    var curvey = new LineItem(sensordata.type + " y", pply, color[1], SymbolType.None);
                    var curvez = new LineItem(sensordata.type + " z", pplz, color[2], SymbolType.None);

                    ctls[controlindex].GraphPane.Legend.IsVisible = true;

                    ctls[controlindex].GraphPane.XAxis.Title.Text = "Freq Hz";
                    ctls[controlindex].GraphPane.YAxis.Title.Text = "Amplitude";
                    ctls[controlindex].GraphPane.Title.Text = "FFT " + sensordata.type + " - " +
                                                              Path.GetFileName(ofd.FileName) + " - " + samplerate +
                                                              "hz input";

                    ctls[controlindex].GraphPane.CurveList.Clear();

                    ctls[controlindex].GraphPane.CurveList.Add(curvex);
                    ctls[controlindex].GraphPane.CurveList.Add(curvey);
                    ctls[controlindex].GraphPane.CurveList.Add(curvez);

                    ctls[controlindex].Invalidate();
                    ctls[controlindex].AxisChange();

                    ctls[controlindex].GraphPane.XAxis.Scale.Max = samplerate/2;

                    ctls[controlindex].Refresh();

                    controlindex++;
                }
            }
        }

        private string zedGraphControl_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            return String.Format("{0} hz/{1} rpm", curve[iPt].X, curve[iPt].X*60.0);
        }
    }
}