using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace MissionPlanner.Joystick
{

    public class LinuxJoystickState : IMyJoystickState
    {
        private readonly Dictionary<byte, ushort> _jAxis;
        private readonly Dictionary<byte, bool> _jButton;

        public LinuxJoystickState(Dictionary<byte, ushort> jAxis, Dictionary<byte, bool> jButton)
        {
            _jAxis = jAxis;
            _jButton = jButton;

            for (byte a = 0; a < 128; a++)
            {
                if(!_jButton.ContainsKey(a))
                    _jButton[a] = false;
            }

            for (byte a = 0; a < 128; a++)
            {
                if(!_jAxis.ContainsKey(a))
                    _jAxis[a] = 65535/2;
            }
        }

        public int[] GetSlider()
        {
            return new int[] {_jAxis[6], 65535 / 2};
        }

        public int[] GetPointOfView()
        {
            return new int[] {65535 / 2};
        }

        public bool[] GetButtons()
        {
            return _jButton.Values.ToArray();
        }

        public int AZ      {
            get { return _jAxis[7]; }
        }
        public int AY       {
            get { return _jAxis[8]; }
        }
        public int AX       {
            get { return _jAxis[9]; }
        }
        public int ARz { get; }
        public int ARy { get; }
        public int ARx { get; }
        public int FRx { get; }
        public int FRy { get; }
        public int FRz { get; }
        public int FX { get; }
        public int FY { get; }
        public int FZ { get; }

        public int Rx
        {
            get { return _jAxis[3]; }
        }

        public int Ry
        {
            get { return _jAxis[4]; }
        }

        public int Rz
        {
            get { return _jAxis[5]; }
        }

        public int VRx { get; }
        public int VRy { get; }
        public int VRz { get; }
        public int VX { get; }
        public int VY { get; }
        public int VZ { get; }

        public int X
        {
            get { return _jAxis[0]; }
        }

        public int Y
        {
            get { return _jAxis[1]; }
        }

        public int Z
        {
            get { return _jAxis[2]; }
        }
    }

    public class WindowsJoystickState : IMyJoystickState
    {
        internal JoystickState baseJoystickState;

        public WindowsJoystickState(JoystickState state)
        {
            baseJoystickState = state;
        }

        public int[] GetSlider()
        {
            return baseJoystickState.Sliders;
        }

        public int[] GetPointOfView()
        {
            return baseJoystickState.PointOfViewControllers;
        }

        public bool[] GetButtons()
        {
            return baseJoystickState.Buttons;
        }

        public int AZ
        {
            get { return baseJoystickState.AccelerationZ; }
        }

        public int AY
        {
            get { return baseJoystickState.AccelerationY; }
        }

        public int AX
        {
            get { return baseJoystickState.AccelerationX; }
        }

        public int ARz
        {
            get { return baseJoystickState.AngularAccelerationZ; }
        }

        public int ARy
        {
            get { return baseJoystickState.AngularAccelerationY; }
        }

        public int ARx
        {
            get { return baseJoystickState.AngularAccelerationX; }
        }

        public int FRx
        {
            get { return baseJoystickState.TorqueX; }
        }

        public int FRy
        {
            get { return baseJoystickState.TorqueY; }
        }

        public int FRz
        {
            get { return baseJoystickState.TorqueZ; }
        }

        public int FX
        {
            get { return baseJoystickState.ForceX; }
        }

        public int FY
        {
            get { return baseJoystickState.ForceY; }
        }

        public int FZ
        {
            get { return baseJoystickState.ForceZ; }
        }

        public int Rx
        {
            get { return baseJoystickState.RotationX; }
        }

        public int Ry
        {
            get { return baseJoystickState.RotationY; }
        }

        public int Rz
        {
            get { return baseJoystickState.RotationZ; }
        }

        public int VRx
        {
            get { return baseJoystickState.AngularVelocityX; }
        }

        public int VRy
        {
            get { return baseJoystickState.AngularVelocityY; }
        }

        public int VRz
        {
            get { return baseJoystickState.AngularVelocityZ; }
        }

        public int VX
        {
            get { return baseJoystickState.VelocityX; }
        }

        public int VY
        {
            get { return baseJoystickState.VelocityY; }
        }

        public int VZ
        {
            get { return baseJoystickState.VelocityZ; }
        }

        public int X
        {
            get { return baseJoystickState.X; }
        }

        public int Y
        {
            get { return baseJoystickState.Y; }
        }

        public int Z
        {
            get { return baseJoystickState.Z; }
        }
    }
}