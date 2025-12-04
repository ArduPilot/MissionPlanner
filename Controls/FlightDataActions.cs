using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class FlightDataActions : UserControl
    {
        private const int MinGroupBoxWidth = 350;

        public FlightDataActions()
        {
            InitializeComponent();
            scrollPanel.Resize += ScrollPanel_Resize;
        }

        private void ScrollPanel_Resize(object sender, EventArgs e)
        {
            UpdateGroupBoxWidths();
        }

        private void UpdateGroupBoxWidths()
        {
            if (scrollPanel == null) return;

            // Account for vertical scrollbar if visible
            int scrollBarWidth = scrollPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            int availableWidth = scrollPanel.ClientSize.Width - scrollBarWidth - mainFlowLayout.Padding.Horizontal;

            // FlowLayoutPanel default margin is 3 on each side = 6 total per control
            int marginPerBox = 6;

            // Calculate how many boxes fit per row at minimum width
            int boxesPerRow = Math.Max(1, availableWidth / (MinGroupBoxWidth + marginPerBox));
            if (boxesPerRow > 4) boxesPerRow = 4;

            // Calculate the width for each box to fill the row
            int boxWidth = (availableWidth / boxesPerRow) - marginPerBox;
            boxWidth = Math.Max(boxWidth, MinGroupBoxWidth);

            grpCommand.Width = boxWidth;
            grpSetpoints.Width = boxWidth;
            grpTools.Width = boxWidth;
            grpPayload.Width = boxWidth;
        }

        public void Initialize()
        {
            ThemeManager.ApplyThemeTo(this);
            UpdateGroupBoxWidths();
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

        // Payload group
        public TrackBar TrackBarPitch => trackBarPitch;
        public TrackBar TrackBarRoll => trackBarRoll;
        public TrackBar TrackBarYaw => trackBarYaw;
        public TextBox TXT_gimbalPitchPos => txtPitchPos;
        public TextBox TXT_gimbalRollPos => txtRollPos;
        public TextBox TXT_gimbalYawPos => txtYawPos;
        public MyButton BUT_resetGimbalPos => btnResetGimbal;
        public MyButton BUT_GimbalVideo => btnGimbalVideo;

        #endregion
    }
}
