namespace MissionPlanner.Joystick
{
    public interface IMyJoystickState
    {
        int[] GetSlider();
        int[] GetPointOfView();
        bool[] GetButtons();
        int AZ { get; }
        int AY { get; }
        int AX { get; }
        int ARz { get; }
        int ARy { get; }
        int ARx { get; }
        int FRx { get; }
        int FRy { get; }
        int FRz { get; }
        int FX { get; }
        int FY { get; }
        int FZ { get; }
        int Rx { get; }
        int Ry { get; }
        int Rz { get; }
        int VRx { get; }
        int VRy { get; }
        int VRz { get; }
        int VX { get; }
        int VY { get; }
        int VZ { get; }
        int X { get; }
        int Y { get; }
        int Z { get; }
    }
}