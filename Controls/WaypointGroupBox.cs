using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class WaypointGroupBox : GroupBox
    {
        private TableLayoutPanel tableLayout;
        private ComboBox cmbCommand;
        private TextBox txtParam1;
        private TextBox txtParam2;
        private TextBox txtParam3;
        private TextBox txtParam4;
        private TextBox txtLat;
        private TextBox txtLon;
        private TextBox txtAlt;
        private ComboBox cmbFrame;
        private MyButton btnDelete;
        private MyButton btnUp;
        private MyButton btnDown;
        private Label lblGrad;
        private Label lblAngle;
        private Label lblDist;
        private Label lblAZ;

        private int _waypointIndex;

        public event EventHandler DeleteClicked;
        public event EventHandler UpClicked;
        public event EventHandler DownClicked;
        public event EventHandler<WaypointChangedEventArgs> ValueChanged;

        public int WaypointIndex
        {
            get => _waypointIndex;
            set
            {
                _waypointIndex = value;
                this.Text = $"WP {value + 1}";
            }
        }

        public string Command
        {
            get => cmbCommand.SelectedItem?.ToString() ?? "";
            set { if (cmbCommand.Items.Contains(value)) cmbCommand.SelectedItem = value; }
        }

        public string Param1 { get => txtParam1.Text; set => txtParam1.Text = value; }
        public string Param2 { get => txtParam2.Text; set => txtParam2.Text = value; }
        public string Param3 { get => txtParam3.Text; set => txtParam3.Text = value; }
        public string Param4 { get => txtParam4.Text; set => txtParam4.Text = value; }
        public string Latitude { get => txtLat.Text; set => txtLat.Text = value; }
        public string Longitude { get => txtLon.Text; set => txtLon.Text = value; }
        public string Altitude { get => txtAlt.Text; set => txtAlt.Text = value; }
        public string Frame
        {
            get => cmbFrame.SelectedItem?.ToString() ?? "";
            set { if (cmbFrame.Items.Contains(value)) cmbFrame.SelectedItem = value; }
        }

        public string GradientValue { get => lblGrad.Text; set => lblGrad.Text = value; }
        public string AngleValue { get => lblAngle.Text; set => lblAngle.Text = value; }
        public string DistanceValue { get => lblDist.Text; set => lblDist.Text = value; }
        public string AZValue { get => lblAZ.Text; set => lblAZ.Text = value; }

        public ComboBox CommandComboBox => cmbCommand;
        public ComboBox FrameComboBox => cmbFrame;

        public WaypointGroupBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoSize = false;
            this.Width = 280;
            this.Height = 320;
            this.Padding = new Padding(5);

            tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 14,
                AutoSize = false
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;

            // Command
            AddLabelAndControl("Command:", cmbCommand = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownWidth = 200
            }, row++);

            // Param1-4
            AddLabelAndControl("Param1:", txtParam1 = new TextBox { Dock = DockStyle.Fill }, row++);
            AddLabelAndControl("Param2:", txtParam2 = new TextBox { Dock = DockStyle.Fill }, row++);
            AddLabelAndControl("Param3:", txtParam3 = new TextBox { Dock = DockStyle.Fill }, row++);
            AddLabelAndControl("Param4:", txtParam4 = new TextBox { Dock = DockStyle.Fill }, row++);

            // Lat/Lon/Alt
            AddLabelAndControl("Lat:", txtLat = new TextBox { Dock = DockStyle.Fill }, row++);
            AddLabelAndControl("Lon:", txtLon = new TextBox { Dock = DockStyle.Fill }, row++);
            AddLabelAndControl("Alt:", txtAlt = new TextBox { Dock = DockStyle.Fill }, row++);

            // Frame
            AddLabelAndControl("Frame:", cmbFrame = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            }, row++);

            // Read-only info
            AddLabelAndControl("Grad:", lblGrad = new Label { Dock = DockStyle.Fill, AutoSize = false }, row++);
            AddLabelAndControl("Angle:", lblAngle = new Label { Dock = DockStyle.Fill, AutoSize = false }, row++);
            AddLabelAndControl("Dist:", lblDist = new Label { Dock = DockStyle.Fill, AutoSize = false }, row++);
            AddLabelAndControl("AZ:", lblAZ = new Label { Dock = DockStyle.Fill, AutoSize = false }, row++);

            // Buttons panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = false,
                Height = 30
            };

            btnUp = new MyButton { Text = "Up", Width = 50, Height = 25 };
            btnDown = new MyButton { Text = "Down", Width = 50, Height = 25 };
            btnDelete = new MyButton { Text = "X", Width = 30, Height = 25 };

            btnUp.Click += (s, e) => UpClicked?.Invoke(this, EventArgs.Empty);
            btnDown.Click += (s, e) => DownClicked?.Invoke(this, EventArgs.Empty);
            btnDelete.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

            buttonPanel.Controls.Add(btnUp);
            buttonPanel.Controls.Add(btnDown);
            buttonPanel.Controls.Add(btnDelete);

            tableLayout.SetColumnSpan(buttonPanel, 2);
            tableLayout.Controls.Add(buttonPanel, 0, row);

            this.Controls.Add(tableLayout);

            // Wire up value changed events
            cmbCommand.SelectedIndexChanged += OnValueChanged;
            txtParam1.TextChanged += OnValueChanged;
            txtParam2.TextChanged += OnValueChanged;
            txtParam3.TextChanged += OnValueChanged;
            txtParam4.TextChanged += OnValueChanged;
            txtLat.TextChanged += OnValueChanged;
            txtLon.TextChanged += OnValueChanged;
            txtAlt.TextChanged += OnValueChanged;
            cmbFrame.SelectedIndexChanged += OnValueChanged;

            this.ResumeLayout(false);
        }

        private void AddLabelAndControl(string labelText, Control control, int row)
        {
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tableLayout.Controls.Add(label, 0, row);
            tableLayout.Controls.Add(control, 1, row);
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, new WaypointChangedEventArgs(WaypointIndex));
        }

        public void SetColumnHeaders(string param1, string param2, string param3, string param4)
        {
            // Update labels based on command type
            if (tableLayout.GetControlFromPosition(0, 1) is Label lbl1)
                lbl1.Text = string.IsNullOrEmpty(param1) ? "Param1:" : param1 + ":";
            if (tableLayout.GetControlFromPosition(0, 2) is Label lbl2)
                lbl2.Text = string.IsNullOrEmpty(param2) ? "Param2:" : param2 + ":";
            if (tableLayout.GetControlFromPosition(0, 3) is Label lbl3)
                lbl3.Text = string.IsNullOrEmpty(param3) ? "Param3:" : param3 + ":";
            if (tableLayout.GetControlFromPosition(0, 4) is Label lbl4)
                lbl4.Text = string.IsNullOrEmpty(param4) ? "Param4:" : param4 + ":";
        }
    }

    public class WaypointChangedEventArgs : EventArgs
    {
        public int WaypointIndex { get; }

        public WaypointChangedEventArgs(int index)
        {
            WaypointIndex = index;
        }
    }
}
