using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SharpDX.DirectInput;

namespace MissionPlanner.Joystick
{
    /// <summary>
    /// Source samples are taken from http://mpolaczyk.pl/raspberry-pi-mono-c-joystick-handler/
    /// </summary>
    public class JoystickManagerUnix : IJoystickManager
    {
        private Dictionary<string, IJoystick> joysticks = new Dictionary<string, IJoystick>();

        public IList<DeviceInstance> GetDevices()
        {
            var list = Directory.GetFiles("/dev/input", "js*");
            return list.Select(j => new DeviceInstance(j)).ToList();
        }

        public IJoystick GetJoyStickByName(string name)
        {
            if (joysticks.ContainsKey(name))
            {
                return joysticks[name];
            }

            var j = new JoystickUnix(name);
            joysticks.Add(name, j);
            return j;
        }
    }

    public class JoystickUnix : IJoystick
    {
        private string deviceFile;
        private FileStream fs = null;
        private byte[] buff = new byte[8];
        private Joystick j;
        private Thread thread;
        private bool firstRead = false;

        public JoystickUnix(string deviceFile)
        {
            if (!File.Exists(deviceFile))
                throw new Exception($"Device {deviceFile} not found");

            this.deviceFile = deviceFile;
            this.Capabilities = new Capabilities(0, 0);
            j = new Joystick(this.Capabilities);
        }

        public void Dispose(bool disposing)
        {
            thread.Abort();
            fs.Close();
            fs.Dispose();
            fs = null;
        }

        public void Acquire()
        {
            if (fs != null)
                return;

            Console.WriteLine("Acquirying joystick");
            fs = new FileStream(deviceFile, FileMode.Open);
            thread = new Thread(ReadBuffer);
            thread.Start();
        }

        public void Poll()
        {
        }

        public MyJoystickState CurrentJoystickState()
        {
            var state = new JoystickState();
            if (j.Axis.Count >= 2)
            {
                state.X = j.Axis[0] + 32767;
                state.Y = j.Axis[1] + 32767;
            }
            if (j.Axis.Count >= 4)
            {
                state.AccelerationX = j.Axis[2] + 32767;
                state.AccelerationY = j.Axis[3] + 32767;
            }
            if (j.Axis.Count >= 6)
            {
                state.RotationX = j.Axis[4] + 32767;
                state.RotationY = j.Axis[5] + 32767;
            }

            for (byte i = 0; i < j.Button.Count; i++)
                state.Buttons[i] = j.Button[i];

            return new MyJoystickState(state);
        }

        public void Unacquire()
        {
        }

        public Capabilities Capabilities { get; private set; }
        public bool IsDisposed { get { return fs == null; } }

        private void ReadBuffer()
        {
            while (fs != null && fs.CanRead)
            {
                fs.Read(buff, 0, 8);
                j.DetectChange(buff);
                Thread.Sleep(1);
            }
        }

        private class Joystick
        {
            private readonly Capabilities capabilities;

            public Joystick(Capabilities capabilities)
            {
                this.capabilities = capabilities;
                Button = new Dictionary<byte, bool>();
                Axis = new Dictionary<byte, short>();
            }

            enum STATE : byte { PRESSED = 0x01, RELEASED = 0x00 }
            enum TYPE : byte { AXIS = 0x02, BUTTON = 0x01 }
            enum MODE : byte { CONFIGURATION = 0x80, VALUE = 0x00 }

            /// <summary>
            /// Buttons collection, key: address, bool: value
            /// </summary>
            public Dictionary<byte, bool> Button;

            /// <summary>
            /// Axis collection, key: address, short: value
            /// </summary>
            public Dictionary<byte, short> Axis;

            /// <summary>
            /// Function recognizes flags in buffer and modifies value of button, axis or configuration.
            /// Every new buffer changes only one value of one button/axis. Joystick object have to remember all previous values.
            /// </summary>
            public void DetectChange(byte[] buff)
            {
                // If configuration
                if (checkBit(buff[6], (byte)MODE.CONFIGURATION))
                {
                    if (checkBit(buff[6], (byte)TYPE.AXIS))
                    {
                        // Axis configuration, read address and register axis
                        byte key = (byte)buff[7];
                        if (!Axis.ContainsKey(key))
                        {
                            Axis.Add(key, 0);
                            return;
                        }
                    }
                    else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                    {
                        // Button configuration, read address and register button
                        byte key = (byte)buff[7];
                        if (!Button.ContainsKey(key))
                        {
                            Button.Add((byte)buff[7], false);
                            this.capabilities.ButtonCount = Button.Count;
                            return;
                        }
                    }
                }

                // If new button/axis value
                if (checkBit(buff[6], (byte)TYPE.AXIS))
                {
                    // Axis value, decode U2 and save to Axis dictionary.
                    short value = BitConverter.ToInt16(new byte[2] { buff[4], buff[5] }, 0);
                    Axis[(byte)buff[7]] = value;
                    return;
                }
                else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                {
                    // Bytton value, decode value and save to Button dictionary.
                    Button[(byte)buff[7]] = buff[4] == (byte)STATE.PRESSED;
                    return;
                }
            }

            /// <summary>
            /// Checks if bits that are set in flag are set in value.
            /// </summary>
            bool checkBit(byte value, byte flag)
            {
                byte c = (byte)(value & flag);
                return c == (byte)flag;
            }
        }
    }
}
