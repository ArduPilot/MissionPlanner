namespace MissionPlanner.ArduPilot
{
    /// <summary>
    /// https://github.com/ArduPilot/ardupilot/blob/master/libraries/AP_Motors/AP_Motors_Class.h
    /// </summary>
    public enum motor_frame_type
    {
        MOTOR_FRAME_TYPE_PLUS = 0,
        MOTOR_FRAME_TYPE_X = 1,
        MOTOR_FRAME_TYPE_V = 2,
        MOTOR_FRAME_TYPE_H = 3,
        MOTOR_FRAME_TYPE_VTAIL = 4,
        MOTOR_FRAME_TYPE_ATAIL = 5,
        MOTOR_FRAME_TYPE_PLUSREV = 6, // plus with reversed motor direction
        MOTOR_FRAME_TYPE_Y6B = 10,
        MOTOR_FRAME_TYPE_Y6F = 11, // for FireFlyY6
        MOTOR_FRAME_TYPE_BF_X = 12, // X frame, betaflight ordering
        MOTOR_FRAME_TYPE_DJI_X = 13, // X frame, DJI ordering
        MOTOR_FRAME_TYPE_CW_X = 14, // X frame, clockwise ordering
        MOTOR_FRAME_TYPE_I = 15, // (sideways H) octo only
        MOTOR_FRAME_TYPE_NYT_PLUS = 16, // plus frame, no differential torque for yaw
        MOTOR_FRAME_TYPE_NYT_X = 17, // X frame, no differential torque for yaw
        MOTOR_FRAME_TYPE_BF_X_REV = 18, // X frame, betaflight ordering, reversed motors
    };
}