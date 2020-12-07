using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.MsgBox;
using MissionPlanner.Radio;
using ZedGraph;

namespace SikRadio
{
    public partial class Rssi : UserControl, ISikRadioForm
    {
        private readonly Sikradio inter = new Sikradio();
        private readonly RollingPointPairList plotdatanoicel = new RollingPointPairList(1200);
        private readonly RollingPointPairList plotdatanoicer = new RollingPointPairList(1200);

        private readonly RollingPointPairList plotdatarssil = new RollingPointPairList(1200);
        private readonly RollingPointPairList plotdatarssir = new RollingPointPairList(1200);
        private int tickStart;
        RFD.RFD900.TSession _Session;

        public Rssi()
        {
            InitializeComponent();

            zedGraphControl1.GraphPane.AddCurve("RSSI Local", plotdatarssil, Color.Red, SymbolType.None);
            zedGraphControl1.GraphPane.AddCurve("RSSI Remote", plotdatarssir, Color.Green, SymbolType.None);
            zedGraphControl1.GraphPane.AddCurve("Noise Local", plotdatanoicel, Color.Blue, SymbolType.None);
            zedGraphControl1.GraphPane.AddCurve("Noise Remote", plotdatanoicer, Color.Orange, SymbolType.None);

            zedGraphControl1.GraphPane.Title.Text = "RSSI";

            Terminal.SetupStreamWriter();
        }

        public void Connect()
        {
            if (_Session == null)
            {
                RFD.RFD900.TSession Session = null;

                if (RFDLib.Utils.Retry(() =>
                {
                    Session = new RFD.RFD900.TSession(SikRadio.Config.comPort, MainV2.comPort.BaseStream.BaudRate);
                    return Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND;
                }
                , 3))
                {
                    if (RFDLib.Utils.Retry(() => Session.ATCClient.DoQuery("AT&T=RSSI", true).Contains("RSSI"), 3))
                    {
                        Session.AssumeMode(RFD.RFD900.TSession.TMode.TRANSPARENT);

                        tickStart = Environment.TickCount;

                        timer1.Start();

                        _Session = Session;
                    }
                    else
                    {
                        var ATIReply = Session.ATCClient.DoQuery("ATI", true);
                        if (RFDLib.Text.Contains(ATIReply, "async"))
                        {
                            MissionPlanner.MsgBox.CustomMessageBox.Show("Firmware doesn't support RSSI reporting");
                        }
                        else
                        {
                            MissionPlanner.MsgBox.CustomMessageBox.Show("Failed to enter RSSI reporting mode.");
                        }
                    }
                }
                else
                {
                    MissionPlanner.MsgBox.CustomMessageBox.Show("Failed to put modem into AT command mode.");
                }
            }
        }

        public void Disconnect()
        {
            if (_Session != null)
            {
                timer1.Stop();

                System.Diagnostics.Debug.WriteLine("Putting into AT command mode");
                if (_Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
                {
                    System.Diagnostics.Debug.WriteLine("Doing AT&T command");
                    inter.doCommand(Config.comPort, "AT&T");
                    System.Diagnostics.Debug.WriteLine("Putting into transparent mode");
                    _Session.PutIntoTransparentMode();

                    _Session.Dispose();
                    _Session = null;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var comPort = SikRadio.Config.comPort;

            if ((comPort != null) && comPort.IsOpen)
            {
                comPort.WriteLine("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

                if (comPort.BytesToRead < 50)
                    return;

                try
                {
                    var line = comPort.ReadLine();

                    /*
    L/R RSSI: 12/0  L/R noise: 17/0 pkts: 0  txe=0 rxe=0 stx=0 srx=0 ecc=0/0 temp=61 dco=0
    L/R RSSI: 12/0  L/R noise: 16/0 pkts: 0  txe=0 rxe=0 stx=0 srx=0 ecc=0/0 temp=61 dco=0
                     */

                    var rssi = new Regex(@"RSSI: ([0-9]+)/([0-9]+)\s+L/R noise: ([0-9]+)/([0-9]+)");

                    var match = rssi.Match(line);

                    if (match.Success)
                    {
                        var time = (Environment.TickCount - tickStart) / 1000.0;

                        plotdatarssil.Add(time, double.Parse(match.Groups[1].Value));
                        plotdatarssir.Add(time, double.Parse(match.Groups[2].Value));
                        plotdatanoicel.Add(time, double.Parse(match.Groups[3].Value));
                        plotdatanoicer.Add(time, double.Parse(match.Groups[4].Value));


                        // Make sure the Y axis is rescaled to accommodate actual data
                        zedGraphControl1.AxisChange();

                        // Force a redraw

                        zedGraphControl1.Invalidate();

                        if (Terminal.sw != null)
                        {
                            Terminal.sw.Write(line);
                            Terminal.sw.Flush();
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public string Header
        {
            get
            {
                return "RSSI";
            }
        }
    }
}