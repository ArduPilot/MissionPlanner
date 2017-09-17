using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;
using SharpDX.DirectInput;

namespace MissionPlanner.Joystick
{
    public class JoystickManagerWindows : IJoystickManager
    {
        static DirectInput directInput = new DirectInput();

        public IList<DeviceInstance> GetDevices()
        {
            var list = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            return list.Select(j => new DeviceInstance(j.ProductName)).ToList();
        }

        public IJoystick GetJoyStickByName(string name)
        {
            var joysticklist = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

            foreach (var device in joysticklist)
            {
                if (device.ProductName.TrimUnPrintable() == name)
                {
                    return new JoystickWindows(directInput, device.InstanceGuid);
                }
            }

            return null;
        }
    }

    public class JoystickWindows : IJoystick
    {
        private SharpDX.DirectInput.Joystick joystick;

        public JoystickWindows(DirectInput directInput, Guid deviceInstanceGuid)
        {
            this.joystick = new SharpDX.DirectInput.Joystick(directInput, deviceInstanceGuid);
            this.Capabilities = new Capabilities(joystick.Capabilities.ButtonCount, joystick.Capabilities.PovCount);
        }

        /// <summary>
        /// Implement reccomended best practice dispose pattern
        /// http://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            try
            {
                //not sure if this is a problem from the finalizer?
                if (disposing && joystick != null && joystick.Properties != null)
                    joystick.Unacquire();
            }
            catch
            {
            }

            try
            {
                if (disposing && joystick != null)
                    joystick.Dispose();
            }
            catch
            {
            }

            //tell gc not to call finalize, this object will be GC'd quicker now.
        }

        public void Acquire()
        {
            joystick.Acquire();
        }

        public void Poll()
        {
            joystick.Poll();
        }

        public MyJoystickState CurrentJoystickState()
        {
            return joystick.CurrentJoystickState();
        }

        public void Unacquire()
        {
            joystick.Unacquire();
        }

        public Capabilities Capabilities { get; }
        public bool IsDisposed => joystick.IsDisposed;
    }
}
