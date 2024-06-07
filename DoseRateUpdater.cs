using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class DoseRateUpdater
    {
        private System.Windows.Forms.Timer tickFunc;

        public  float userThreshold { get; set; }
        public float userDetSense1 { get; set; }
        public float userDetSense2 { get; set; }

        public static float finalValue1 { get; set; } //ganesh
        public static string detectorSensitivity1 { get; set; }   //ganesh
        public static string detectorSensitivity2 { get; set; } //ganesh
        public static string threshold { get; set; } //ganesh

        float GMone;
        float GMtwo;

        public DoseRateUpdater()
        {
            // Load previously stored values (if any)
            userThreshold = LoadSetting("userThreshold", 0.0f);
            userDetSense1 = LoadSetting("userDetSense1", 0.0f);
            userDetSense2 = LoadSetting("userDetSense2", 0.0f);
        }

        private float LoadSetting(string key, float defaultValue)
        {
            // Implement logic to load the setting value from a persistent storage location (e.g., registry, file)
            return defaultValue;
        }

        private void SaveSetting(string key, float value)
        {
            // Implement logic to save the setting value to a persistent storage location (e.g., registry, file)
        }

        public void showDoseRate(ToolStripMenuItem toolStripMenuItem)
        {
            PopupForm popup = new PopupForm(this); // Pass 'this' instance (which is DoseRateUpdater)

            if (popup.ShowDialog() == DialogResult.OK)
            {
                // Update user variables with specific user input 
                userThreshold = float.Parse(popup.txtThreshold.Text);
                userDetSense1 = float.Parse(popup.txtDetectorSensitivity1.Text);
                userDetSense2 = float.Parse(popup.txtDetectorSensitivity2.Text);

                // Update static properties
                threshold = popup.txtThreshold.Text;
                detectorSensitivity1 = popup.txtDetectorSensitivity1.Text;
                detectorSensitivity2 = popup.txtDetectorSensitivity2.Text;

                // Save the updated settings
                SaveSetting("userThreshold", userThreshold);
                SaveSetting("userDetSense1", userDetSense1);
                SaveSetting("userDetSense2", userDetSense2);
            }

            popup.Dispose();

            if (tickFunc == null)
            {
                tickFunc = new System.Windows.Forms.Timer();
                tickFunc.Interval = 1000; // Set the interval to 1 second
                tickFunc.Tick += (sender, e) => RadiationDetection(sender, e, toolStripMenuItem);
                tickFunc.Start();

            }
            else if (!tickFunc.Enabled)
            {
                tickFunc.Start();
            }
        }

        public void RadiationDetection(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                string message = MainV2.comPort.MAV.cs.message;

                if (message != null)
                {
                    string pattern1 = @"^GMs\s\d(\d{4})";
                    string pattern2 = @"^GMs\s\d\d\d\d\d\s\d\d\d\d\d\s\d(\d{4})";

                    Match match1 = Regex.Match(message, pattern1);
                    if (match1.Success)
                    {
                        string extractedValue1 = match1.Groups[1].Value;
                        float GM1 = float.Parse(extractedValue1);
                        GMone = GM1;
                    }
                    else
                    {
                        toolStripMenuItem.Text = "no data";
                    }

                    Match match2 = Regex.Match(message, pattern2);
                    if (match2.Success)
                    {
                        string extractedValue2 = match2.Groups[1].Value;
                        float GM2 = float.Parse(extractedValue2);
                        GMtwo = GM2;
                    }
                    else
                    {
                        toolStripMenuItem.Text = "no data";
                    }

                    if (userThreshold <= GMtwo)
                    {
                        finalValue1 = GMtwo / userDetSense2;
                    }
                    else
                    {
                        finalValue1 = GMone / userDetSense1;
                    }

                    // Update UI elements based on finalValue1
                    toolStripMenuItem.Text = finalValue1.ToString("F2") + "\n" + "nsv/h";
                    toolStripMenuItem.ForeColor = finalValue1 >= userThreshold ? Color.Red : Color.Black;

                    // Call the other program to log data to CSV
                    //DataLogger.LogData(DateTime.Now, MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng, finalValue1, userThreshold);
                }
                else
                {
                    toolStripMenuItem.Text = "no data";
                    toolStripMenuItem.ForeColor = Color.Red;
                }
            }
            else
            {
                toolStripMenuItem.Text = "no data";
                toolStripMenuItem.ForeColor = Color.Red;
            }
        }
    }

    public class PopupForm : Form
    {
        private Label lblDetectorSensitivity1;
        private Label lblDetectorSensitivity2;
        private Label lblThreshold;

        public TextBox txtDetectorSensitivity1;
        public TextBox txtDetectorSensitivity2;
        public TextBox txtThreshold;

        private Button btnSubmit;

        private DoseRateUpdater updater;

        public PopupForm(DoseRateUpdater updater)
        {
            this.updater = updater;
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            txtThreshold.Text = updater.userThreshold.ToString();
            txtDetectorSensitivity1.Text = updater.userDetSense1.ToString();
            txtDetectorSensitivity2.Text = updater.userDetSense2.ToString();
        }


        private void InitializeComponent()
        {
            // Labels for user input descriptions
            lblDetectorSensitivity1 = new Label();
            lblDetectorSensitivity1.Text = "Detector sensitivity 1:";
            lblDetectorSensitivity1.AutoSize = true; // Allow automatic sizing
            lblDetectorSensitivity1.Anchor = (AnchorStyles.Left | AnchorStyles.Top); // Anchor to top-left corner

            lblDetectorSensitivity2 = new Label();
            lblDetectorSensitivity2.Text = "Detector sensitivity 2:";
            lblDetectorSensitivity2.AutoSize = true;
            lblDetectorSensitivity2.Anchor = (AnchorStyles.Left | AnchorStyles.Top);

            lblThreshold = new Label();
            lblThreshold.Text = "Threshold:";
            lblThreshold.AutoSize = true;
            lblThreshold.Anchor = (AnchorStyles.Left | AnchorStyles.Top);

            // TextBoxes for user input
            txtDetectorSensitivity1 = new TextBox();
            txtDetectorSensitivity1.Dock = DockStyle.Top; // Position at the top of the form
            txtDetectorSensitivity1.KeyDown += TxtDetectorSensitivity1_KeyDown;

            txtDetectorSensitivity2 = new TextBox();
            txtDetectorSensitivity2.Dock = DockStyle.Top;
            txtDetectorSensitivity2.KeyDown += TxtDetectorSensitivity2_KeyDown; ;

            txtThreshold = new TextBox();
            txtThreshold.Dock = DockStyle.Top;
            txtThreshold.KeyDown += TxtThreshold_KeyDown;

            // Arrange controls in a vertical layout (using a TableLayoutPanel)
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill; // Fill the entire form
            panel.AutoSize = true; // Allow automatic sizing for the panel
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Labels
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // TextBoxes (flexible space)
            panel.Controls.Add(lblDetectorSensitivity1, 0, 0);
            panel.Controls.Add(txtDetectorSensitivity1, 1, 0);
            panel.Controls.Add(lblDetectorSensitivity2, 0, 1);
            panel.Controls.Add(txtDetectorSensitivity2, 1, 1);
            panel.Controls.Add(lblThreshold, 0, 2);
            panel.Controls.Add(txtThreshold, 1, 2);

            Controls.Add(panel); // Add the panel to the form

            // Initialize the Submit button and add it to the form
            btnSubmit = new Button();
            btnSubmit.Text = "OK";
            btnSubmit.Dock = DockStyle.Bottom; // Position at bottom
            btnSubmit.Click += BtnSubmit_Click;
            Controls.Add(btnSubmit);

            // Reduce the form height (adjust as needed)
            this.ClientSize = new System.Drawing.Size(300, 125); // Example size

            // Other form properties and settings can be configured here
            this.Text = "Detector Parameters";
        }


        //Enter key for Data - Copy code 

        private void TxtDetectorSensitivity1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the ding sound
                txtDetectorSensitivity2.Focus(); // Move to next field
            }
        }

        private void TxtDetectorSensitivity2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the ding sound
                txtThreshold.Focus(); // Move to next field
            }
        }

        private void TxtThreshold_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the ding sound
                SubmitValues();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            SubmitValues();
        }



        private void SubmitValues()
        {
            // Retrieve user input from TextBoxes
            string detectorSensitivity1 = txtDetectorSensitivity1.Text;
            string detectorSensitivity2 = txtDetectorSensitivity2.Text;
            string threshold = txtThreshold.Text;

            // Validate inputs if necessary

            // Close the form and return DialogResult.OK (or handle errors)
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}


