namespace MissionPlanner.Joystick
{
    public static class Extensions
    {
        public static MyJoystickState CurrentJoystickState(this SharpDX.DirectInput.Joystick joystick)
        {
            return new MyJoystickState(joystick.GetCurrentState());
        }
    }
}