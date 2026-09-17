namespace MissionPlanner.Joystick
{
    public enum buttonfunction
    {
        ChangeMode,
        Do_Set_Relay,
        Do_Repeat_Relay,
        Do_Set_Servo,
        Do_Repeat_Servo,
        Arm,
        Disarm,
        Digicam_Control,
        TakeOff,
        Mount_Mode,
        Toggle_Pan_Stab,
        Gimbal_pnt_track,
        Mount_Control_0,
        Button_axis0,
        Button_axis1,
        /// <summary>
        /// Trigger an ArduPilot RCx_OPTION auxiliary function via MAV_CMD_DO_AUX_FUNCTION.
        /// p1 = aux function number (RCx_OPTION value), p2 = auxfunctiontrigger behaviour
        /// </summary>
        Aux_Function,
    }
}