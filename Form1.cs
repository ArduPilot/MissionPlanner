
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
            this.Resize += new EventHandler(Form_Resize); // Handle form resize event

            // Set form properties
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new Size(500, 400); // Minimum size for the form
        }

        private void InitializeChart()
        {
            // Set the initial size and location of the chart
            chart1.Size = new Size(400, 200); // Initial fixed size
            chart1.Location = new Point(10, 10); // Initial location within the form

            chart1.ChartAreas.Add("ChartArea1");

            Series series1 = new Series
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2, // Set the thickness of the line
                Color = Color.Blue // Set the color of the line
            };

            chart1.Series.Add(series1);
            Controls.Add(chart1);
        }

        /*
        private void Form_Resize(object sender, EventArgs e)
        {
            // Adjust the chart size dynamically based on form size
            int margin = 20; // Margin from the form's edge
            chart1.Size = new Size(this.ClientSize.Width - 2 * margin, this.ClientSize.Height - 2 * margin);
            chart1.Location = new Point(margin, margin);
        }
      


        private void Form_Resize(object sender, EventArgs e)
        {
            // Adjust the chart size dynamically based on form size
            int margin = 20; // Margin from the form's edge

            // Ensure the new size is valid (greater than zero)
            int newWidth = this.ClientSize.Width - 2 * margin;
            int newHeight = this.ClientSize.Height - 2 * margin;

            if (newWidth > 0 && newHeight > 0)
            {
                chart1.Size = new Size(newWidth, newHeight);
                chart1.Location = new Point(margin, margin);
            }
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer
            {
                Interval = 1000 // 1 second
            };
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
        private readonly Chart chart;
        private readonly string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;

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
                        // Uncomment to show message box
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
                    UpdateChart(currentTime, extractedValue.ToString());
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // Uncomment to show message box
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // Uncomment to show message box
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
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
            this.Resize += new EventHandler(Form_Resize); // Handle form resize event

            // Set form properties
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new Size(500, 400); // Minimum size for the form
        }

        private void InitializeChart()
        {
            // Set the initial size and location of the chart
            chart1.Size = new Size(400, 200); // Initial fixed size
            chart1.Location = new Point(10, 10); // Initial location within the form

            chart1.ChartAreas.Add("ChartArea1");

            Series series1 = new Series
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2, // Set the thickness of the line
                Color = Color.Blue // Set the color of the line
            };

            chart1.Series.Add(series1);
            Controls.Add(chart1);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            // Check if the form is minimized
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            // Adjust the chart size dynamically based on form size
            int margin = 20; // Margin from the form's edge

            // Calculate the new width and height ensuring they are not less than the minimum size
            int newWidth = Math.Max(this.ClientSize.Width - 2 * margin, chart1.MinimumSize.Width);
            int newHeight = Math.Max(this.ClientSize.Height - 2 * margin, chart1.MinimumSize.Height);

            chart1.Size = new Size(newWidth, newHeight);
            chart1.Location = new Point(margin, margin);
        }


        private void StartMessageTimer()
        {
            Timer timer = new Timer
            {
                Interval = 1000 // 1 second
            };
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
        private readonly Chart chart;
        private readonly string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;

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
                        // Uncomment to show message box
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
                    UpdateChart(currentTime, extractedValue.ToString());
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // Uncomment to show message box
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // Uncomment to show message box
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
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

            // Set form properties
            this.Size = new Size(520, 360); // Fixed size for the form
            this.MinimumSize = new Size(520, 360); // Minimum size for the form to prevent resizing
            this.MaximumSize = new Size(520, 360); // Maximum size for the form to prevent resizing
        }

        private void InitializeChart()
        {
            // Set the fixed size and location of the chart
            chart1.Size = new Size(500, 300); // Fixed size
            chart1.Location = new Point(10, 10); // Fixed location within the form

            chart1.ChartAreas.Add("ChartArea1");

            Series series1 = new Series
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2, // Set the thickness of the line
                Color = Color.Blue // Set the color of the line
            };

            chart1.Series.Add(series1);
            Controls.Add(chart1);
        }

        private void StartMessageTimer()
        {
            Timer timer = new Timer
            {
                Interval = 1000 // 1 second
            };
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
        private readonly Chart chart;
        private readonly string pattern = @"^GMs\s\d(\d{4})";
        private bool nullMessageShown = false;
        private bool regexErrorMessageShown = false;
        private bool exceptionMessageShown = false;

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
                        // Uncomment to show message box
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
                    UpdateChart(currentTime, extractedValue.ToString());
                }
                else
                {
                    if (!regexErrorMessageShown)
                    {
                        // Uncomment to show message box
                        // CustomMessageBox.Show("Regex match failed.");
                        regexErrorMessageShown = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!exceptionMessageShown)
                {
                    // Uncomment to show message box
                    // CustomMessageBox.Show("An error occurred: " + ex.Message);
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
