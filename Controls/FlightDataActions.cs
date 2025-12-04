using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class FlightDataActions : UserControl
    {
        private const int MinGroupBoxWidth = 320;
        private const int GroupBoxMargin = 6; // FlowLayoutPanel default margin per side

        public FlightDataActions()
        {
            InitializeComponent();
            this.Resize += FlightDataActions_Resize;
            this.Load += FlightDataActions_Load;
        }

        private void FlightDataActions_Load(object sender, EventArgs e)
        {
            UpdateGroupBoxWidths();
        }

        private void FlightDataActions_Resize(object sender, EventArgs e)
        {
            UpdateGroupBoxWidths();
        }

        private void UpdateGroupBoxWidths()
        {
            if (mainFlowLayout == null) return;

            int availableWidth = mainFlowLayout.ClientSize.Width - mainFlowLayout.Padding.Horizontal;
            int totalMargin = GroupBoxMargin * 2 * 3; // margin on both sides for 3 boxes
            int usableWidth = availableWidth - totalMargin;

            // Calculate how many boxes fit per row at minimum width
            int boxesPerRow = Math.Max(1, usableWidth / MinGroupBoxWidth);
            if (boxesPerRow > 3) boxesPerRow = 3;

            // Calculate the width for each box to fill the row
            int boxWidth = (usableWidth / boxesPerRow) - 1; // -1 to prevent wrapping due to rounding
            boxWidth = Math.Max(boxWidth, MinGroupBoxWidth);

            grpCommand.Width = boxWidth;
            grpSetpoints.Width = boxWidth;
            grpTools.Width = boxWidth;
        }

        public void Initialize()
        {
            ThemeManager.ApplyThemeTo(this);
        }

        #region Exposed Controls - for FlightData.cs to wire up existing event handlers

        // Command group
        public ComboBox CMB_action => cmbAction;
        public MyButton BUTactiondo => btnDoAction;
        public ComboBox CMB_setwp => cmbWaypoint;
        public MyButton BUT_setwp => btnSetWP;
        public ComboBox CMB_modes => cmbModes;
        public MyButton BUT_setmode => btnSetMode;
        public ComboBox CMB_mountmode => cmbMount;
        public MyButton BUT_mountmode => btnSetMount;
        public MyButton BUTrestartmission => btnRestartMission;
        public MyButton BUT_resumemis => btnResumeMission;

        // Setpoints group
        public ModifyandSet modifyandSetSpeed => modSetSpeed;
        public ModifyandSet modifyandSetAlt => modSetAlt;
        public ModifyandSet modifyandSetLoiterRad => modSetLoiterRad;
        public MyButton BUT_Homealt => btnHomeAlt;

        // Tools group
        public MyButton BUT_RAWSensor => btnRawSensor;
        public MyButton BUT_joystick => btnJoystick;
        public MyButton BUT_SendMSG => btnMessage;
        public MyButton BUT_clear_track => btnClearTrack;
        public MyButton BUT_Reboot => btnReboot;
        public MyButton BUT_abortland => btnAbortLanding;

        #endregion
    }
}
