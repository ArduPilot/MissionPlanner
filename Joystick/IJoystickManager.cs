using System.Collections.Generic;

namespace MissionPlanner.Joystick
{
    public interface IJoystickManager
    {
        IList<DeviceInstance> GetDevices();
        IJoystick GetJoyStickByName(string name);
    }

    public class DeviceInstance
    {
        public DeviceInstance(string productName)
        {
            ProductName = productName;
        }

        public string ProductName { get; }
    }

    public interface IJoystick
    {
        void Dispose(bool disposing);
        void Acquire();
        void Poll();
        MyJoystickState CurrentJoystickState();
        void Unacquire();
        Capabilities Capabilities { get; }
        bool IsDisposed { get; }
    }

    public class Capabilities
    {
        public Capabilities(int buttonCount, int povCount)
        {
            ButtonCount = buttonCount;
            PovCount = povCount;
        }

        public int ButtonCount { get; set; }
        public int PovCount { get; set; }
    }
}
