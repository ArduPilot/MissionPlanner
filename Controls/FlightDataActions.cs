using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class FlightDataActions : UserControl
    {
        public FlightDataActions()
        {
            InitializeComponent();
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
