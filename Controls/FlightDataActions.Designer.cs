namespace MissionPlanner.Controls
{
    partial class FlightDataActions
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.mainFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.grpCommand = new System.Windows.Forms.GroupBox();
            this.tableCommand = new System.Windows.Forms.TableLayoutPanel();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.btnDoAction = new MissionPlanner.Controls.MyButton();
            this.cmbWaypoint = new System.Windows.Forms.ComboBox();
            this.btnSetWP = new MissionPlanner.Controls.MyButton();
            this.cmbModes = new System.Windows.Forms.ComboBox();
            this.btnSetMode = new MissionPlanner.Controls.MyButton();
            this.cmbMount = new System.Windows.Forms.ComboBox();
            this.btnSetMount = new MissionPlanner.Controls.MyButton();
            this.btnRestartMission = new MissionPlanner.Controls.MyButton();
            this.btnResumeMission = new MissionPlanner.Controls.MyButton();
            this.grpSetpoints = new System.Windows.Forms.GroupBox();
            this.tableSetpoints = new System.Windows.Forms.TableLayoutPanel();
            this.lblHomeAlt = new System.Windows.Forms.Label();
            this.btnHomeAlt = new MissionPlanner.Controls.MyButton();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.modSetSpeed = new MissionPlanner.Controls.ModifyandSet();
            this.lblAlt = new System.Windows.Forms.Label();
            this.modSetAlt = new MissionPlanner.Controls.ModifyandSet();
            this.lblLoiterRad = new System.Windows.Forms.Label();
            this.modSetLoiterRad = new MissionPlanner.Controls.ModifyandSet();
            this.grpTools = new System.Windows.Forms.GroupBox();
            this.tableTools = new System.Windows.Forms.TableLayoutPanel();
            this.btnRawSensor = new MissionPlanner.Controls.MyButton();
            this.btnJoystick = new MissionPlanner.Controls.MyButton();
            this.btnMessage = new MissionPlanner.Controls.MyButton();
            this.btnClearTrack = new MissionPlanner.Controls.MyButton();
            this.btnReboot = new MissionPlanner.Controls.MyButton();
            this.btnAbortLanding = new MissionPlanner.Controls.MyButton();
            this.grpPayload = new System.Windows.Forms.GroupBox();
            this.tablePayload = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxPitch = new System.Windows.Forms.GroupBox();
            this.trackBarPitch = new System.Windows.Forms.TrackBar();
            this.txtPitchPos = new System.Windows.Forms.TextBox();
            this.groupBoxYaw = new System.Windows.Forms.GroupBox();
            this.txtYawPos = new System.Windows.Forms.TextBox();
            this.trackBarYaw = new System.Windows.Forms.TrackBar();
            this.groupBoxRoll = new System.Windows.Forms.GroupBox();
            this.txtRollPos = new System.Windows.Forms.TextBox();
            this.trackBarRoll = new System.Windows.Forms.TrackBar();
            this.btnResetGimbal = new MissionPlanner.Controls.MyButton();
            this.btnGimbalVideo = new MissionPlanner.Controls.MyButton();
            this.scrollPanel.SuspendLayout();
            this.mainFlowLayout.SuspendLayout();
            this.grpCommand.SuspendLayout();
            this.tableCommand.SuspendLayout();
            this.grpSetpoints.SuspendLayout();
            this.tableSetpoints.SuspendLayout();
            this.grpTools.SuspendLayout();
            this.tableTools.SuspendLayout();
            this.grpPayload.SuspendLayout();
            this.tablePayload.SuspendLayout();
            this.groupBoxPitch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPitch)).BeginInit();
            this.groupBoxYaw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYaw)).BeginInit();
            this.groupBoxRoll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRoll)).BeginInit();
            this.SuspendLayout();
            // 
            // scrollPanel
            // 
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.Controls.Add(this.mainFlowLayout);
            this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollPanel.Location = new System.Drawing.Point(0, 0);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Size = new System.Drawing.Size(900, 280);
            this.scrollPanel.TabIndex = 0;
            // 
            // mainFlowLayout
            // 
            this.mainFlowLayout.AutoSize = true;
            this.mainFlowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainFlowLayout.Controls.Add(this.grpCommand);
            this.mainFlowLayout.Controls.Add(this.grpSetpoints);
            this.mainFlowLayout.Controls.Add(this.grpTools);
            this.mainFlowLayout.Controls.Add(this.grpPayload);
            this.mainFlowLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainFlowLayout.Location = new System.Drawing.Point(0, 0);
            this.mainFlowLayout.Name = "mainFlowLayout";
            this.mainFlowLayout.Padding = new System.Windows.Forms.Padding(4);
            this.mainFlowLayout.Size = new System.Drawing.Size(879, 483);
            this.mainFlowLayout.TabIndex = 0;
            // 
            // grpCommand
            // 
            this.grpCommand.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpCommand.Controls.Add(this.tableCommand);
            this.grpCommand.Location = new System.Drawing.Point(7, 7);
            this.grpCommand.MinimumSize = new System.Drawing.Size(350, 0);
            this.grpCommand.Name = "grpCommand";
            this.grpCommand.Padding = new System.Windows.Forms.Padding(6);
            this.grpCommand.Size = new System.Drawing.Size(350, 227);
            this.grpCommand.TabIndex = 0;
            this.grpCommand.TabStop = false;
            this.grpCommand.Text = "Control";
            // 
            // tableCommand
            // 
            this.tableCommand.ColumnCount = 2;
            this.tableCommand.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableCommand.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableCommand.Controls.Add(this.cmbAction, 0, 0);
            this.tableCommand.Controls.Add(this.btnDoAction, 1, 0);
            this.tableCommand.Controls.Add(this.cmbWaypoint, 0, 1);
            this.tableCommand.Controls.Add(this.btnSetWP, 1, 1);
            this.tableCommand.Controls.Add(this.cmbModes, 0, 2);
            this.tableCommand.Controls.Add(this.btnSetMode, 1, 2);
            this.tableCommand.Controls.Add(this.cmbMount, 0, 3);
            this.tableCommand.Controls.Add(this.btnSetMount, 1, 3);
            this.tableCommand.Controls.Add(this.btnRestartMission, 0, 4);
            this.tableCommand.Controls.Add(this.btnResumeMission, 1, 4);
            this.tableCommand.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableCommand.Location = new System.Drawing.Point(6, 21);
            this.tableCommand.Name = "tableCommand";
            this.tableCommand.RowCount = 5;
            this.tableCommand.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableCommand.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableCommand.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableCommand.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableCommand.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableCommand.Size = new System.Drawing.Size(338, 200);
            this.tableCommand.TabIndex = 0;
            // 
            // cmbAction
            // 
            this.cmbAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.DropDownWidth = 250;
            this.cmbAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.ItemHeight = 25;
            this.cmbAction.Location = new System.Drawing.Point(3, 8);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(163, 33);
            this.cmbAction.TabIndex = 0;
            // 
            // btnDoAction
            // 
            this.btnDoAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoAction.Location = new System.Drawing.Point(172, 3);
            this.btnDoAction.Name = "btnDoAction";
            this.btnDoAction.Size = new System.Drawing.Size(163, 44);
            this.btnDoAction.TabIndex = 1;
            this.btnDoAction.Text = "Do Action";
            this.btnDoAction.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnDoAction.UseVisualStyleBackColor = true;
            // 
            // cmbWaypoint
            // 
            this.cmbWaypoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWaypoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWaypoint.DropDownWidth = 150;
            this.cmbWaypoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWaypoint.FormattingEnabled = true;
            this.cmbWaypoint.ItemHeight = 25;
            this.cmbWaypoint.Location = new System.Drawing.Point(3, 58);
            this.cmbWaypoint.Name = "cmbWaypoint";
            this.cmbWaypoint.Size = new System.Drawing.Size(163, 33);
            this.cmbWaypoint.TabIndex = 2;
            // 
            // btnSetWP
            // 
            this.btnSetWP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetWP.Location = new System.Drawing.Point(172, 53);
            this.btnSetWP.Name = "btnSetWP";
            this.btnSetWP.Size = new System.Drawing.Size(163, 44);
            this.btnSetWP.TabIndex = 3;
            this.btnSetWP.Text = "Set WP";
            this.btnSetWP.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnSetWP.UseVisualStyleBackColor = true;
            // 
            // cmbModes
            // 
            this.cmbModes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModes.DropDownWidth = 150;
            this.cmbModes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbModes.FormattingEnabled = true;
            this.cmbModes.ItemHeight = 25;
            this.cmbModes.Location = new System.Drawing.Point(3, 103);
            this.cmbModes.Name = "cmbModes";
            this.cmbModes.Size = new System.Drawing.Size(163, 33);
            this.cmbModes.TabIndex = 4;
            this.cmbModes.Visible = false;
            // 
            // btnSetMode
            // 
            this.btnSetMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetMode.Location = new System.Drawing.Point(172, 103);
            this.btnSetMode.Name = "btnSetMode";
            this.btnSetMode.Size = new System.Drawing.Size(163, 1);
            this.btnSetMode.TabIndex = 5;
            this.btnSetMode.Text = "Apply";
            this.btnSetMode.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnSetMode.UseVisualStyleBackColor = true;
            this.btnSetMode.Visible = false;
            // 
            // cmbMount
            // 
            this.cmbMount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMount.DropDownWidth = 150;
            this.cmbMount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMount.FormattingEnabled = true;
            this.cmbMount.ItemHeight = 25;
            this.cmbMount.Location = new System.Drawing.Point(3, 108);
            this.cmbMount.Name = "cmbMount";
            this.cmbMount.Size = new System.Drawing.Size(163, 33);
            this.cmbMount.TabIndex = 6;
            // 
            // btnSetMount
            // 
            this.btnSetMount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetMount.Location = new System.Drawing.Point(172, 103);
            this.btnSetMount.Name = "btnSetMount";
            this.btnSetMount.Size = new System.Drawing.Size(163, 44);
            this.btnSetMount.TabIndex = 7;
            this.btnSetMount.Text = "Set Mount";
            this.btnSetMount.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnSetMount.UseVisualStyleBackColor = true;
            // 
            // btnRestartMission
            // 
            this.btnRestartMission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestartMission.Location = new System.Drawing.Point(3, 153);
            this.btnRestartMission.Name = "btnRestartMission";
            this.btnRestartMission.Size = new System.Drawing.Size(163, 44);
            this.btnRestartMission.TabIndex = 8;
            this.btnRestartMission.Text = "Restart Mission";
            this.btnRestartMission.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnRestartMission.UseVisualStyleBackColor = true;
            // 
            // btnResumeMission
            // 
            this.btnResumeMission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResumeMission.Location = new System.Drawing.Point(172, 153);
            this.btnResumeMission.Name = "btnResumeMission";
            this.btnResumeMission.Size = new System.Drawing.Size(163, 44);
            this.btnResumeMission.TabIndex = 9;
            this.btnResumeMission.Text = "Resume Mission";
            this.btnResumeMission.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnResumeMission.UseVisualStyleBackColor = true;
            // 
            // grpSetpoints
            // 
            this.grpSetpoints.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpSetpoints.Controls.Add(this.tableSetpoints);
            this.grpSetpoints.Location = new System.Drawing.Point(363, 7);
            this.grpSetpoints.MinimumSize = new System.Drawing.Size(350, 0);
            this.grpSetpoints.Name = "grpSetpoints";
            this.grpSetpoints.Padding = new System.Windows.Forms.Padding(6);
            this.grpSetpoints.Size = new System.Drawing.Size(350, 227);
            this.grpSetpoints.TabIndex = 1;
            this.grpSetpoints.TabStop = false;
            this.grpSetpoints.Text = "Setpoints";
            // 
            // tableSetpoints
            // 
            this.tableSetpoints.ColumnCount = 2;
            this.tableSetpoints.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableSetpoints.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableSetpoints.Controls.Add(this.lblHomeAlt, 0, 0);
            this.tableSetpoints.Controls.Add(this.btnHomeAlt, 1, 0);
            this.tableSetpoints.Controls.Add(this.lblSpeed, 0, 1);
            this.tableSetpoints.Controls.Add(this.modSetSpeed, 1, 1);
            this.tableSetpoints.Controls.Add(this.lblAlt, 0, 2);
            this.tableSetpoints.Controls.Add(this.modSetAlt, 1, 2);
            this.tableSetpoints.Controls.Add(this.lblLoiterRad, 0, 3);
            this.tableSetpoints.Controls.Add(this.modSetLoiterRad, 1, 3);
            this.tableSetpoints.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableSetpoints.Location = new System.Drawing.Point(6, 21);
            this.tableSetpoints.Name = "tableSetpoints";
            this.tableSetpoints.RowCount = 4;
            this.tableSetpoints.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableSetpoints.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableSetpoints.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableSetpoints.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableSetpoints.Size = new System.Drawing.Size(338, 200);
            this.tableSetpoints.TabIndex = 0;
            // 
            // lblHomeAlt
            // 
            this.lblHomeAlt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHomeAlt.AutoSize = true;
            this.lblHomeAlt.Location = new System.Drawing.Point(3, 9);
            this.lblHomeAlt.Name = "lblHomeAlt";
            this.lblHomeAlt.Size = new System.Drawing.Size(65, 32);
            this.lblHomeAlt.TabIndex = 0;
            this.lblHomeAlt.Text = "Home Alt (m):";
            // 
            // btnHomeAlt
            // 
            this.btnHomeAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHomeAlt.Location = new System.Drawing.Point(83, 3);
            this.btnHomeAlt.Name = "btnHomeAlt";
            this.btnHomeAlt.Size = new System.Drawing.Size(252, 44);
            this.btnHomeAlt.TabIndex = 1;
            this.btnHomeAlt.Text = "Apply";
            this.btnHomeAlt.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnHomeAlt.UseVisualStyleBackColor = true;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(3, 59);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(51, 32);
            this.lblSpeed.TabIndex = 2;
            this.lblSpeed.Text = "Speed (m/s):";
            // 
            // modSetSpeed
            // 
            this.modSetSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modSetSpeed.ButtonText = "Apply";
            this.modSetSpeed.DecimalPlaces = 0;
            this.modSetSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modSetSpeed.Location = new System.Drawing.Point(80, 50);
            this.modSetSpeed.Margin = new System.Windows.Forms.Padding(0);
            this.modSetSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.modSetSpeed.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modSetSpeed.MinimumSize = new System.Drawing.Size(26, 50);
            this.modSetSpeed.Name = "modSetSpeed";
            this.modSetSpeed.Size = new System.Drawing.Size(258, 50);
            this.modSetSpeed.TabIndex = 3;
            this.modSetSpeed.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblAlt
            // 
            this.lblAlt.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAlt.AutoSize = true;
            this.lblAlt.Location = new System.Drawing.Point(3, 109);
            this.lblAlt.Name = "lblAlt";
            this.lblAlt.Size = new System.Drawing.Size(54, 32);
            this.lblAlt.TabIndex = 4;
            this.lblAlt.Text = "Altitude (m):";
            // 
            // modSetAlt
            // 
            this.modSetAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modSetAlt.ButtonText = "Apply";
            this.modSetAlt.DecimalPlaces = 0;
            this.modSetAlt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.modSetAlt.Location = new System.Drawing.Point(80, 100);
            this.modSetAlt.Margin = new System.Windows.Forms.Padding(0);
            this.modSetAlt.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.modSetAlt.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.modSetAlt.MinimumSize = new System.Drawing.Size(26, 50);
            this.modSetAlt.Name = "modSetAlt";
            this.modSetAlt.Size = new System.Drawing.Size(258, 50);
            this.modSetAlt.TabIndex = 5;
            this.modSetAlt.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblLoiterRad
            // 
            this.lblLoiterRad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLoiterRad.AutoSize = true;
            this.lblLoiterRad.Location = new System.Drawing.Point(3, 159);
            this.lblLoiterRad.Name = "lblLoiterRad";
            this.lblLoiterRad.Size = new System.Drawing.Size(72, 32);
            this.lblLoiterRad.TabIndex = 6;
            this.lblLoiterRad.Text = "Loiter Rad (m):";
            // 
            // modSetLoiterRad
            // 
            this.modSetLoiterRad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.modSetLoiterRad.ButtonText = "Apply";
            this.modSetLoiterRad.DecimalPlaces = 0;
            this.modSetLoiterRad.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.modSetLoiterRad.Location = new System.Drawing.Point(80, 150);
            this.modSetLoiterRad.Margin = new System.Windows.Forms.Padding(0);
            this.modSetLoiterRad.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.modSetLoiterRad.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.modSetLoiterRad.MinimumSize = new System.Drawing.Size(26, 50);
            this.modSetLoiterRad.Name = "modSetLoiterRad";
            this.modSetLoiterRad.Size = new System.Drawing.Size(258, 50);
            this.modSetLoiterRad.TabIndex = 7;
            this.modSetLoiterRad.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // grpTools
            // 
            this.grpTools.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpTools.Controls.Add(this.tableTools);
            this.grpTools.Location = new System.Drawing.Point(7, 240);
            this.grpTools.MinimumSize = new System.Drawing.Size(350, 0);
            this.grpTools.Name = "grpTools";
            this.grpTools.Padding = new System.Windows.Forms.Padding(6);
            this.grpTools.Size = new System.Drawing.Size(350, 177);
            this.grpTools.TabIndex = 2;
            this.grpTools.TabStop = false;
            this.grpTools.Text = "Tools";
            // 
            // tableTools
            // 
            this.tableTools.ColumnCount = 2;
            this.tableTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableTools.Controls.Add(this.btnRawSensor, 0, 0);
            this.tableTools.Controls.Add(this.btnJoystick, 1, 0);
            this.tableTools.Controls.Add(this.btnMessage, 0, 1);
            this.tableTools.Controls.Add(this.btnClearTrack, 1, 1);
            this.tableTools.Controls.Add(this.btnReboot, 0, 2);
            this.tableTools.Controls.Add(this.btnAbortLanding, 1, 2);
            this.tableTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableTools.Location = new System.Drawing.Point(6, 21);
            this.tableTools.Name = "tableTools";
            this.tableTools.RowCount = 3;
            this.tableTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableTools.Size = new System.Drawing.Size(338, 150);
            this.tableTools.TabIndex = 0;
            // 
            // btnRawSensor
            // 
            this.btnRawSensor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRawSensor.Location = new System.Drawing.Point(3, 3);
            this.btnRawSensor.Name = "btnRawSensor";
            this.btnRawSensor.Size = new System.Drawing.Size(163, 44);
            this.btnRawSensor.TabIndex = 0;
            this.btnRawSensor.Text = "Raw Sensor";
            this.btnRawSensor.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnRawSensor.UseVisualStyleBackColor = true;
            // 
            // btnJoystick
            // 
            this.btnJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJoystick.Location = new System.Drawing.Point(172, 3);
            this.btnJoystick.Name = "btnJoystick";
            this.btnJoystick.Size = new System.Drawing.Size(163, 44);
            this.btnJoystick.TabIndex = 1;
            this.btnJoystick.Text = "Joystick";
            this.btnJoystick.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnJoystick.UseVisualStyleBackColor = true;
            // 
            // btnMessage
            // 
            this.btnMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMessage.Location = new System.Drawing.Point(3, 53);
            this.btnMessage.Name = "btnMessage";
            this.btnMessage.Size = new System.Drawing.Size(163, 44);
            this.btnMessage.TabIndex = 2;
            this.btnMessage.Text = "Message";
            this.btnMessage.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnMessage.UseVisualStyleBackColor = true;
            // 
            // btnClearTrack
            // 
            this.btnClearTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearTrack.Location = new System.Drawing.Point(172, 53);
            this.btnClearTrack.Name = "btnClearTrack";
            this.btnClearTrack.Size = new System.Drawing.Size(163, 44);
            this.btnClearTrack.TabIndex = 3;
            this.btnClearTrack.Text = "Clear Track";
            this.btnClearTrack.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnClearTrack.UseVisualStyleBackColor = true;
            // 
            // btnReboot
            // 
            this.btnReboot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReboot.Location = new System.Drawing.Point(3, 103);
            this.btnReboot.Name = "btnReboot";
            this.btnReboot.Size = new System.Drawing.Size(163, 44);
            this.btnReboot.TabIndex = 4;
            this.btnReboot.Text = "Reboot";
            this.btnReboot.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnReboot.UseVisualStyleBackColor = true;
            // 
            // btnAbortLanding
            // 
            this.btnAbortLanding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbortLanding.Location = new System.Drawing.Point(172, 103);
            this.btnAbortLanding.Name = "btnAbortLanding";
            this.btnAbortLanding.Size = new System.Drawing.Size(163, 44);
            this.btnAbortLanding.TabIndex = 5;
            this.btnAbortLanding.Text = "Abort Land";
            this.btnAbortLanding.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnAbortLanding.UseVisualStyleBackColor = true;
            // 
            // grpPayload
            // 
            this.grpPayload.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpPayload.Controls.Add(this.tablePayload);
            this.grpPayload.Location = new System.Drawing.Point(363, 240);
            this.grpPayload.MinimumSize = new System.Drawing.Size(350, 0);
            this.grpPayload.Name = "grpPayload";
            this.grpPayload.Padding = new System.Windows.Forms.Padding(6);
            this.grpPayload.Size = new System.Drawing.Size(350, 236);
            this.grpPayload.TabIndex = 3;
            this.grpPayload.TabStop = false;
            this.grpPayload.Text = "Payload";
            // 
            // tablePayload
            // 
            this.tablePayload.ColumnCount = 2;
            this.tablePayload.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tablePayload.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePayload.Controls.Add(this.groupBoxPitch, 0, 0);
            this.tablePayload.Controls.Add(this.groupBoxYaw, 1, 0);
            this.tablePayload.Controls.Add(this.groupBoxRoll, 1, 1);
            this.tablePayload.Controls.Add(this.btnResetGimbal, 0, 2);
            this.tablePayload.Controls.Add(this.btnGimbalVideo, 1, 2);
            this.tablePayload.Dock = System.Windows.Forms.DockStyle.Top;
            this.tablePayload.Location = new System.Drawing.Point(6, 21);
            this.tablePayload.Name = "tablePayload";
            this.tablePayload.RowCount = 3;
            this.tablePayload.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tablePayload.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tablePayload.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tablePayload.Size = new System.Drawing.Size(338, 210);
            this.tablePayload.TabIndex = 0;
            // 
            // groupBoxPitch
            // 
            this.groupBoxPitch.Controls.Add(this.trackBarPitch);
            this.groupBoxPitch.Controls.Add(this.txtPitchPos);
            this.groupBoxPitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPitch.Location = new System.Drawing.Point(3, 3);
            this.groupBoxPitch.Name = "groupBoxPitch";
            this.tablePayload.SetRowSpan(this.groupBoxPitch, 2);
            this.groupBoxPitch.Size = new System.Drawing.Size(124, 154);
            this.groupBoxPitch.TabIndex = 0;
            this.groupBoxPitch.TabStop = false;
            this.groupBoxPitch.Text = "Tilt";
            // 
            // trackBarPitch
            // 
            this.trackBarPitch.LargeChange = 10;
            this.trackBarPitch.Location = new System.Drawing.Point(62, 11);
            this.trackBarPitch.Maximum = 90;
            this.trackBarPitch.Minimum = -90;
            this.trackBarPitch.Name = "trackBarPitch";
            this.trackBarPitch.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarPitch.Size = new System.Drawing.Size(56, 130);
            this.trackBarPitch.SmallChange = 5;
            this.trackBarPitch.TabIndex = 1;
            this.trackBarPitch.TickFrequency = 10;
            // 
            // txtPitchPos
            // 
            this.txtPitchPos.Location = new System.Drawing.Point(6, 18);
            this.txtPitchPos.Name = "txtPitchPos";
            this.txtPitchPos.Size = new System.Drawing.Size(50, 22);
            this.txtPitchPos.TabIndex = 0;
            // 
            // groupBoxYaw
            // 
            this.groupBoxYaw.Controls.Add(this.txtYawPos);
            this.groupBoxYaw.Controls.Add(this.trackBarYaw);
            this.groupBoxYaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxYaw.Location = new System.Drawing.Point(133, 3);
            this.groupBoxYaw.Name = "groupBoxYaw";
            this.groupBoxYaw.Size = new System.Drawing.Size(202, 74);
            this.groupBoxYaw.TabIndex = 1;
            this.groupBoxYaw.TabStop = false;
            this.groupBoxYaw.Text = "Pan";
            // 
            // txtYawPos
            // 
            this.txtYawPos.Location = new System.Drawing.Point(6, 18);
            this.txtYawPos.Name = "txtYawPos";
            this.txtYawPos.Size = new System.Drawing.Size(51, 22);
            this.txtYawPos.TabIndex = 0;
            // 
            // trackBarYaw
            // 
            this.trackBarYaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarYaw.LargeChange = 10;
            this.trackBarYaw.Location = new System.Drawing.Point(63, 18);
            this.trackBarYaw.Maximum = 180;
            this.trackBarYaw.Minimum = -180;
            this.trackBarYaw.Name = "trackBarYaw";
            this.trackBarYaw.Size = new System.Drawing.Size(133, 56);
            this.trackBarYaw.TabIndex = 1;
            this.trackBarYaw.TickFrequency = 10;
            // 
            // groupBoxRoll
            // 
            this.groupBoxRoll.Controls.Add(this.txtRollPos);
            this.groupBoxRoll.Controls.Add(this.trackBarRoll);
            this.groupBoxRoll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRoll.Location = new System.Drawing.Point(133, 83);
            this.groupBoxRoll.Name = "groupBoxRoll";
            this.groupBoxRoll.Size = new System.Drawing.Size(202, 74);
            this.groupBoxRoll.TabIndex = 2;
            this.groupBoxRoll.TabStop = false;
            this.groupBoxRoll.Text = "Roll";
            // 
            // txtRollPos
            // 
            this.txtRollPos.Location = new System.Drawing.Point(6, 19);
            this.txtRollPos.Name = "txtRollPos";
            this.txtRollPos.Size = new System.Drawing.Size(51, 22);
            this.txtRollPos.TabIndex = 0;
            // 
            // trackBarRoll
            // 
            this.trackBarRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarRoll.LargeChange = 10;
            this.trackBarRoll.Location = new System.Drawing.Point(63, 19);
            this.trackBarRoll.Maximum = 90;
            this.trackBarRoll.Minimum = -90;
            this.trackBarRoll.Name = "trackBarRoll";
            this.trackBarRoll.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBarRoll.RightToLeftLayout = true;
            this.trackBarRoll.Size = new System.Drawing.Size(133, 56);
            this.trackBarRoll.TabIndex = 1;
            this.trackBarRoll.TickFrequency = 10;
            // 
            // btnResetGimbal
            // 
            this.btnResetGimbal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetGimbal.Location = new System.Drawing.Point(3, 163);
            this.btnResetGimbal.Name = "btnResetGimbal";
            this.btnResetGimbal.Size = new System.Drawing.Size(124, 44);
            this.btnResetGimbal.TabIndex = 3;
            this.btnResetGimbal.Text = "Reset Position";
            this.btnResetGimbal.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnResetGimbal.UseVisualStyleBackColor = true;
            // 
            // btnGimbalVideo
            // 
            this.btnGimbalVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGimbalVideo.Location = new System.Drawing.Point(133, 163);
            this.btnGimbalVideo.Name = "btnGimbalVideo";
            this.btnGimbalVideo.Size = new System.Drawing.Size(202, 44);
            this.btnGimbalVideo.TabIndex = 4;
            this.btnGimbalVideo.Text = "Video Control";
            this.btnGimbalVideo.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnGimbalVideo.UseVisualStyleBackColor = true;
            // 
            // FlightDataActions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scrollPanel);
            this.MinimumSize = new System.Drawing.Size(0, 280);
            this.Name = "FlightDataActions";
            this.Size = new System.Drawing.Size(900, 280);
            this.scrollPanel.ResumeLayout(false);
            this.scrollPanel.PerformLayout();
            this.mainFlowLayout.ResumeLayout(false);
            this.grpCommand.ResumeLayout(false);
            this.tableCommand.ResumeLayout(false);
            this.grpSetpoints.ResumeLayout(false);
            this.tableSetpoints.ResumeLayout(false);
            this.tableSetpoints.PerformLayout();
            this.grpTools.ResumeLayout(false);
            this.tableTools.ResumeLayout(false);
            this.grpPayload.ResumeLayout(false);
            this.tablePayload.ResumeLayout(false);
            this.groupBoxPitch.ResumeLayout(false);
            this.groupBoxPitch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPitch)).EndInit();
            this.groupBoxYaw.ResumeLayout(false);
            this.groupBoxYaw.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarYaw)).EndInit();
            this.groupBoxRoll.ResumeLayout(false);
            this.groupBoxRoll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRoll)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel scrollPanel;
        private System.Windows.Forms.FlowLayoutPanel mainFlowLayout;

        // Command group
        private System.Windows.Forms.GroupBox grpCommand;
        private System.Windows.Forms.TableLayoutPanel tableCommand;
        private System.Windows.Forms.ComboBox cmbAction;
        private MissionPlanner.Controls.MyButton btnDoAction;
        private System.Windows.Forms.ComboBox cmbWaypoint;
        private MissionPlanner.Controls.MyButton btnSetWP;
        private System.Windows.Forms.ComboBox cmbModes;
        private MissionPlanner.Controls.MyButton btnSetMode;
        private System.Windows.Forms.ComboBox cmbMount;
        private MissionPlanner.Controls.MyButton btnSetMount;
        private MissionPlanner.Controls.MyButton btnRestartMission;
        private MissionPlanner.Controls.MyButton btnResumeMission;

        // Setpoints group
        private System.Windows.Forms.GroupBox grpSetpoints;
        private System.Windows.Forms.TableLayoutPanel tableSetpoints;
        private System.Windows.Forms.Label lblHomeAlt;
        private MissionPlanner.Controls.MyButton btnHomeAlt;
        private System.Windows.Forms.Label lblSpeed;
        private MissionPlanner.Controls.ModifyandSet modSetSpeed;
        private System.Windows.Forms.Label lblAlt;
        private MissionPlanner.Controls.ModifyandSet modSetAlt;
        private System.Windows.Forms.Label lblLoiterRad;
        private MissionPlanner.Controls.ModifyandSet modSetLoiterRad;

        // Tools group
        private System.Windows.Forms.GroupBox grpTools;
        private System.Windows.Forms.TableLayoutPanel tableTools;
        private MissionPlanner.Controls.MyButton btnRawSensor;
        private MissionPlanner.Controls.MyButton btnJoystick;
        private MissionPlanner.Controls.MyButton btnMessage;
        private MissionPlanner.Controls.MyButton btnClearTrack;
        private MissionPlanner.Controls.MyButton btnReboot;
        private MissionPlanner.Controls.MyButton btnAbortLanding;

        // Payload group
        private System.Windows.Forms.GroupBox grpPayload;
        private System.Windows.Forms.TableLayoutPanel tablePayload;
        private System.Windows.Forms.GroupBox groupBoxPitch;
        private System.Windows.Forms.TrackBar trackBarPitch;
        private System.Windows.Forms.TextBox txtPitchPos;
        private System.Windows.Forms.GroupBox groupBoxYaw;
        private System.Windows.Forms.TrackBar trackBarYaw;
        private System.Windows.Forms.TextBox txtYawPos;
        private System.Windows.Forms.GroupBox groupBoxRoll;
        private System.Windows.Forms.TrackBar trackBarRoll;
        private System.Windows.Forms.TextBox txtRollPos;
        private MissionPlanner.Controls.MyButton btnResetGimbal;
        private MissionPlanner.Controls.MyButton btnGimbalVideo;
    }
}
