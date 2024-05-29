

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
