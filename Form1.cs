

/*
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly Chart chart1 = new Chart();
        private MessageHandler messageHandler;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(chart1);
            StartMessageTimer();
        }

        private void InitializeChart()
        {
            chart1.Size = new System.Drawing.Size(800, 400);
            chart1.ChartAreas.Add("ChartArea1");

            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Line;
            chart1.Series.Add(series1);
            series1.BorderWidth = 2; // Set the thickness of the line
            series1.Color = Color.Blue; // Set the color of the line

            Controls.Add(chart1);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private Chart chart;

        public MessageHandler(Chart chart)
        {
            this.chart = chart;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = currentTime;
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }


        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            // Add the new data point
            chart.Series[0].Points.AddXY(xAxis, yAxis);

            // Remove the first point if the number of points exceeds a certain threshold
            if (chart.Series[0].Points.Count > MaxDataPoints)
            {
                chart.Series[0].Points.RemoveAt(0);
            }
        }
    }
}
*/


/*
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly Chart chart1 = new Chart();
        private MessageHandler messageHandler;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(chart1);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
        }

        private void InitializeChart()
        {
            chart1.Dock = DockStyle.Fill; // Dock the chart to fill the form
            chart1.ChartAreas.Add("ChartArea1");

            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Line;
            chart1.Series.Add(series1);
            series1.BorderWidth = 2; // Set the thickness of the line
            series1.Color = Color.Blue; // Set the color of the line

            Controls.Add(chart1);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            chart1.Size = new Size(newWidth, newHeight);
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private Chart chart;

        public MessageHandler(Chart chart)
        {
            this.chart = chart;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        //CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = currentTime;
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                       // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    //CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            // Add the new data point
            chart.Series[0].Points.AddXY(xAxis, yAxis);

            // Remove the first point if the number of points exceeds a certain threshold
            if (chart.Series[0].Points.Count > MaxDataPoints)
            {
                chart.Series[0].Points.RemoveAt(0);
            }
        }
    }
}
*/


//zed graph

/*
using System;
using System.Windows.Forms;
using ZedGraph;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly ZedGraphControl zedGraphControl = new ZedGraphControl();
        private MessageHandler messageHandler;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(zedGraphControl);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
        }

        private void InitializeChart()
        {
            zedGraphControl.Dock = DockStyle.Fill; // Dock the chart to fill the form

            GraphPane pane = zedGraphControl.GraphPane;
            pane.Title.Text = "Chart";
            pane.XAxis.Title.Text = "Time";
            pane.YAxis.Title.Text = "Value";

            LineItem lineItem = pane.AddCurve("Data", new PointPairList(), Color.Blue, SymbolType.None);
            lineItem.Line.Width = 2; // Set the thickness of the line

            Controls.Add(zedGraphControl);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            zedGraphControl.Size = new Size(newWidth, newHeight);
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private ZedGraphControl zedGraphControl;

        public MessageHandler(ZedGraphControl zedGraphControl)
        {
            this.zedGraphControl = zedGraphControl;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        // CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = currentTime;
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            double x = (double)new XDate(DateTime.Now);
            double y = double.Parse(yAxis);
            list.Add(x, y);

            if (list.Count > MaxDataPoints)
            {
                list.RemoveAt(0);
            }

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }
    }
}
*/

/*
//zed graph with modifications


using System;
using System.Windows.Forms;
using ZedGraph;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly ZedGraphControl zedGraphControl = new ZedGraphControl();
        private MessageHandler messageHandler;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(zedGraphControl);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
        }

        private void InitializeChart()
        {
            zedGraphControl.Dock = DockStyle.Fill; // Dock the chart to fill the form

            GraphPane pane = zedGraphControl.GraphPane;
            pane.Title.Text = "Chart";
            pane.XAxis.Title.Text = "Time";
            pane.YAxis.Title.Text = "Value";

            LineItem lineItem = pane.AddCurve("Data", new PointPairList(), Color.Blue, SymbolType.None);
            lineItem.Line.Width = 2; // Set the thickness of the line

            Controls.Add(zedGraphControl);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            zedGraphControl.Size = new Size(newWidth, newHeight);
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private ZedGraphControl zedGraphControl;

        public MessageHandler(ZedGraphControl zedGraphControl)
        {
            this.zedGraphControl = zedGraphControl;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        // CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = DateTime.Now.ToString("HH:mm:ss");
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            DateTime time = DateTime.Parse(xAxis);
            double x = (double)new XDate(time);
            double y = double.Parse(yAxis);
            list.Add(x, y);

            if (list.Count > MaxDataPoints)
            {
                list.RemoveAt(0);
            }

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }
    }
}
*/

//zed graph with x-axis chnage

/*
using System;
using System.Windows.Forms;
using ZedGraph;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly ZedGraphControl zedGraphControl = new ZedGraphControl();
        private MessageHandler messageHandler;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(zedGraphControl);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
        }

        private void InitializeChart()
        {
            zedGraphControl.Dock = DockStyle.Fill; // Dock the chart to fill the form

            GraphPane pane = zedGraphControl.GraphPane;
            pane.Title.Text = "Chart";
            pane.XAxis.Title.Text = "Time";
            pane.YAxis.Title.Text = "Value";

            // Set the XAxis to display time in HH:mm:ss format
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.Format = "HH:mm:ss";

            LineItem lineItem = pane.AddCurve("Data", new PointPairList(), Color.Blue, SymbolType.None);
            lineItem.Line.Width = 2; // Set the thickness of the line

            Controls.Add(zedGraphControl);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            zedGraphControl.Size = new Size(newWidth, newHeight);
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private ZedGraphControl zedGraphControl;

        public MessageHandler(ZedGraphControl zedGraphControl)
        {
            this.zedGraphControl = zedGraphControl;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        // CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = DateTime.Now.ToString("HH:mm:ss");
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            DateTime time = DateTime.Parse(xAxis);
            double x = (double)new XDate(time);
            double y = double.Parse(yAxis);
            list.Add(x, y);

            if (list.Count > MaxDataPoints)
            {
                list.RemoveAt(0);
            }

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }
    }
}
*/


/*
//zed graph with time fix
using System;
using System.Windows.Forms;
using ZedGraph;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly ZedGraphControl zedGraphControl = new ZedGraphControl();
        private MessageHandler messageHandler;
        private Timer timer;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(zedGraphControl);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
            this.FormClosing += ChartPopupForm_FormClosing; // Add form closing event handler
        }

        private void InitializeChart()
        {
            zedGraphControl.Dock = DockStyle.Fill; // Dock the chart to fill the form

            GraphPane pane = zedGraphControl.GraphPane;
            pane.Title.Text = "Chart";
            pane.XAxis.Title.Text = "Time";
            pane.YAxis.Title.Text = "Value";

            // Set the XAxis to display time in HH:mm:ss format
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.Format = "HH:mm:ss";

            LineItem lineItem = pane.AddCurve("Data", new PointPairList(), Color.Blue, SymbolType.None);
            lineItem.Line.Width = 2; // Set the thickness of the line

            Controls.Add(zedGraphControl);
        }

        private void StartMessageTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            zedGraphControl.Size = new Size(newWidth, newHeight);
        }

        private void ChartPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the timer
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
            }

            // Dispose the ZedGraphControl
            if (zedGraphControl != null)
            {
                zedGraphControl.Dispose();
            }

            // Unsubscribe from the Resize event
            this.Resize -= ChartPopupForm_Resize;

            // Unsubscribe from the FormClosing event
            this.FormClosing -= ChartPopupForm_FormClosing;
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private ZedGraphControl zedGraphControl;

        public MessageHandler(ZedGraphControl zedGraphControl)
        {
            this.zedGraphControl = zedGraphControl;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        // CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = DateTime.Now.ToString("HH:mm:ss");
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            DateTime time = DateTime.Parse(xAxis);
            double x = (double)new XDate(time);
            double y = double.Parse(yAxis);
            list.Add(x, y);

            if (list.Count > MaxDataPoints)
            {
                list.RemoveAt(0);
            }

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }
    }
}
*/


//working code currently 

/*
using System;
using System.Windows.Forms;
using ZedGraph;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MissionPlanner
{
    public partial class ChartPopupForm : Form
    {
        private readonly ZedGraphControl zedGraphControl = new ZedGraphControl();
        private MessageHandler messageHandler;
        private Timer timer;

        public ChartPopupForm()
        {
            InitializeComponent();
            InitializeChart();
            messageHandler = new MessageHandler(zedGraphControl);
            StartMessageTimer();
            this.Resize += ChartPopupForm_Resize; // Add resize event handler
            this.FormClosing += ChartPopupForm_FormClosing; // Add form closing event handler

            // Ensure the form is sizable and can be minimized independently
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = true;
        }

        private void InitializeChart()
        {
            zedGraphControl.Dock = DockStyle.Fill; // Dock the chart to fill the form

            GraphPane pane = zedGraphControl.GraphPane;
            pane.Title.Text = "Chart";
            pane.XAxis.Title.Text = "Time";
            pane.YAxis.Title.Text = "Value";

            // Set the XAxis to display time in HH:mm:ss format
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.Format = "HH:mm:ss";

            LineItem lineItem = pane.AddCurve("Data", new PointPairList(), Color.Blue, SymbolType.None);
            lineItem.Line.Width = 2; // Set the thickness of the line

            Controls.Add(zedGraphControl);
        }

        private void StartMessageTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string message = GetMessageFromComPort();
            messageHandler.HandleMessage(message);
        }

        private string GetMessageFromComPort()
        {
            // Logic to obtain message from COM port (replace this with your actual logic)
            return MainV2.comPort.MAV.cs.message;
        }

        private void ChartPopupForm_Resize(object sender, EventArgs e)
        {
            // Ensure width and height are positive
            int newWidth = Math.Max(this.ClientSize.Width - 20, 1);
            int newHeight = Math.Max(this.ClientSize.Height - 20, 1);
            zedGraphControl.Size = new Size(newWidth, newHeight);
        }

        private void ChartPopupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the timer
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
            }

            // Dispose the ZedGraphControl
            if (zedGraphControl != null)
            {
                zedGraphControl.Dispose();
            }

            // Unsubscribe from the Resize event
            this.Resize -= ChartPopupForm_Resize;

            // Unsubscribe from the FormClosing event
            this.FormClosing -= ChartPopupForm_FormClosing;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020;

            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_MINIMIZE)
            {
                // Handle the minimize event here if needed
            }

            base.WndProc(ref m);
        }
    }

    public class MessageHandler
    {
        private string xAxis;
        private string yAxis;
        private string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;
        private ZedGraphControl zedGraphControl;

        public MessageHandler(ZedGraphControl zedGraphControl)
        {
            this.zedGraphControl = zedGraphControl;
        }

        public void HandleMessage(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    if (!nullMessageShown)
                    {
                        // CustomMessageBox.Show("Message is null.");
                        nullMessageShown = true;
                    }
                    return;
                }

                nullMessageShown = false;

                Match match = Regex.Match(message, pattern);

                if (match.Success)
                {
                    int extractedValue = int.Parse(match.Groups[1].Value);
                    xAxis = DateTime.Now.ToString("HH:mm:ss");
                    yAxis = extractedValue.ToString();
                    UpdateChart(xAxis, yAxis);
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
                    exceptionMessageShown = true;
                }
            }
        }

        private const int MaxDataPoints = 1000;

        private void UpdateChart(string xAxis, string yAxis)
        {
            GraphPane pane = zedGraphControl.GraphPane;
            LineItem curve = pane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;
            if (list == null)
                return;

            DateTime time = DateTime.Parse(xAxis);
            double x = (double)new XDate(time);
            double y = double.Parse(yAxis);
            list.Add(x, y);

            if (list.Count > MaxDataPoints)
            {
                list.RemoveAt(0);
            }

            zedGraphControl.AxisChange();
            zedGraphControl.Invalidate();
        }
    }
}
*/




