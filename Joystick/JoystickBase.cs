using System;
using System.Collections.Generic;
using System.Collections;
using log4net;
using System.Reflection;
using System.IO;
using MissionPlanner.Utilities;
using SharpDX.DirectInput;
using static MissionPlanner.Joystick.JoystickProperties;

namespace MissionPlanner.Joystick
{

    public abstract class JoystickBase : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected SharpDX.DirectInput.Joystick joystick;
        protected MyJoystickState state;
        protected static DirectInput directInput = new DirectInput();
        public bool enabled = false;
        protected bool[] buttonpressed = new bool[128];
        public string name;
        public bool elevons = false;

        public bool manual_control = false;

        protected virtual string joystickconfigbutton { get; set; } = "joystickbuttons.xml";
        protected virtual string joystickconfigaxis { get; set; } = "joystickaxis.xml";

        protected virtual string joystickname { get; set; } = "";

        // set to default midpoint
        protected int hat1 = 65535 / 2;
        protected int hat2 = 65535 / 2;
        protected int custom0 = 65535 / 2;
        protected int custom1 = 65535 / 2;

        protected JoyChannel[] JoyChannels = new JoyChannel[9]; // we are base 1
        protected JoyButton[] JoyButtons = new JoyButton[128]; // base 0



        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Implement reccomended best practice dispose pattern
        /// http://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="disposing"></param>
        virtual protected void Dispose(bool disposing)
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
            GC.SuppressFinalize(this);
        }

        //no need for finalizer...
        //~Joystick()
        //{
        //    Dispose(false);
        //}

        public JoystickBase(string joystickconfigbutton, string joystickconfigaxis)
        {
            for (int a = 0; a < JoyButtons.Length; a++)
                JoyButtons[a].buttonno = -1;
           loadconfig(joystickconfigbutton, joystickconfigaxis);
        }

        public virtual void loadconfig(string joystickconfigbutton, string joystickconfigaxis)
        {
            log.Info("Loading joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save for later
            this.joystickconfigbutton = Settings.GetUserDataDirectory() + joystickconfigbutton;
            this.joystickconfigaxis = Settings.GetUserDataDirectory() + joystickconfigaxis;

            // load config
            if (File.Exists(this.joystickconfigbutton) && File.Exists(this.joystickconfigaxis))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof(JoyButton[]), new Type[] { typeof(JoyButton) });

                    using (StreamReader sr = new StreamReader(this.joystickconfigbutton))
                    {
                        JoyButtons = (JoyButton[])reader.Deserialize(sr);
                    }
                }
                catch
                {
                }

                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof(JoyChannel[]),
                            new Type[] { typeof(JoyChannel) });

                    using (StreamReader sr = new StreamReader(this.joystickconfigaxis))
                    {
                        JoyChannels = (JoyChannel[])reader.Deserialize(sr);
                    }
                }
                catch
                {
                }
            }
        }

        public void saveconfig()
        {
            log.Info("Saving joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(JoyButton[]), new Type[] { typeof(JoyButton) });

            using (StreamWriter sw = new StreamWriter(joystickconfigbutton))
            {
                writer.Serialize(sw, JoyButtons);
            }

            writer = new System.Xml.Serialization.XmlSerializer(typeof(JoyChannel[]), new Type[] { typeof(JoyChannel) });

            using (StreamWriter sw = new StreamWriter(joystickconfigaxis))
            {
                writer.Serialize(sw, JoyChannels);
            }
        }

        

        public static IList<DeviceInstance> getDevices()
        {
            return directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
        }

        public static SharpDX.DirectInput.Joystick getJoyStickByName(string name)
        {
            var joysticklist = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName.TrimUnPrintable() == name)
                {
                    return new SharpDX.DirectInput.Joystick(directInput, device.InstanceGuid);
                }
            }

            return null;
        }

        public SharpDX.DirectInput.Joystick AcquireJoystick(string name)
        {
            joystick = getJoyStickByName(name);

            if (joystick == null)
                return null;

            joystick.Acquire();

            joystick.Poll();

            return joystick;
        }

        public virtual bool start(string name, string ThreadName="")
        {

            joystick = AcquireJoystick(name);

            if (joystick == null)
                return false;

            enabled = true;

            System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                Name = ThreadName + "Joystick loop",
                Priority = System.Threading.ThreadPriority.AboveNormal,
                IsBackground = true
            };
            t11.Start();

            return true;
        }

        public static joystickaxis getMovingAxis(string name, int threshold)
        {

            var joystick = new Joystick().AcquireJoystick(name);

            if (joystick == null)
                return joystickaxis.ARx;

            joystick.Poll();

            System.Threading.Thread.Sleep(300);

            joystick.Poll();

            var obj = joystick.CurrentJoystickState();
            Hashtable values = new Hashtable();

            // get the state of the joystick before.
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = int.Parse(property.GetValue(obj, null).ToString());
            }
            values["Slider1"] = obj.GetSlider()[0];
            values["Slider2"] = obj.GetSlider()[1];
            values["Hatud1"] = obj.GetPointOfView()[0];
            values["Hatlr2"] = obj.GetPointOfView()[0];
            values["Custom1"] = 0;
            values["Custom2"] = 0;

            CustomMessageBox.Show("Please move the joystick axis you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                joystick.Poll();
                System.Threading.Thread.Sleep(50);
                var nextstate = joystick.CurrentJoystickState();

                int[] slider = nextstate.GetSlider();

                int[] hat1 = nextstate.GetPointOfView();

                type = nextstate.GetType();
                properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));

                    log.InfoFormat("test name {0} old {1} new {2} ", property.Name, values[property.Name],
                        int.Parse(property.GetValue(nextstate, null).ToString()));
                    log.InfoFormat("{0}  {1} {2}", property.Name, (int)values[property.Name],
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold));
                    if ((int)values[property.Name] >
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold) ||
                        (int)values[property.Name] <
                        (int.Parse(property.GetValue(nextstate, null).ToString()) - threshold))
                    {
                        log.Info(property.Name);
                        joystick.Unacquire();
                        return (joystickaxis)Enum.Parse(typeof(joystickaxis), property.Name);
                    }
                }

                // slider1
                if ((int)values["Slider1"] > (slider[0] + threshold) ||
                    (int)values["Slider1"] < (slider[0] - threshold))
                {
                    joystick.Unacquire();
                    return joystickaxis.Slider1;
                }

                // slider2
                if ((int)values["Slider2"] > (slider[1] + threshold) ||
                    (int)values["Slider2"] < (slider[1] - threshold))
                {
                    joystick.Unacquire();
                    return joystickaxis.Slider2;
                }

                // Hatud1
                if ((int)values["Hatud1"] != (hat1[0]))
                {
                    joystick.Unacquire();
                    return joystickaxis.Hatud1;
                }

                // Hatlr2
                if ((int)values["Hatlr2"] != (hat1[0]))
                {
                    joystick.Unacquire();
                    return joystickaxis.Hatlr2;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return joystickaxis.None;
        }

        public static int getPressedButton(string name)
        {

            var joystick = getJoyStickByName(name);

            if (joystick == null)
                return -1;

            //joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

            System.Threading.Thread.Sleep(500);

            joystick.Poll();

            var obj = joystick.CurrentJoystickState();

            var buttonsbefore = obj.GetButtons();

            CustomMessageBox.Show(
                "Please press the joystick button you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                joystick.Poll();
                var nextstate = joystick.CurrentJoystickState();

                var buttons = nextstate.GetButtons();

                for (int a = 0; a < joystick.Capabilities.ButtonCount; a++)
                {
                    if (buttons[a] != buttonsbefore[a])
                        return a;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return -1;
        }

        public void setReverse(int channel, bool reverse)
        {
            JoyChannels[channel].reverse = reverse;
        }

        public void setAxis(int channel, joystickaxis axis)
        {
            JoyChannels[channel].axis = axis;
        }

        public void setChannel(int channel, joystickaxis axis, bool reverse, int expo)
        {
            JoyChannel joy = new JoyChannel();
            joy.axis = axis;
            joy.channel = channel;
            joy.expo = expo;
            joy.reverse = reverse;

            JoyChannels[channel] = joy;
        }

        public void setChannel(JoyChannel chan)
        {
            JoyChannels[chan.channel] = chan;
        }

        public JoyChannel getChannel(int channel)
        {
            return JoyChannels[channel];
        }

        public void setButton(int arrayoffset, JoyButton buttonconfig)
        {
            JoyButtons[arrayoffset] = buttonconfig;
        }

        public JoyButton getButton(int arrayoffset)
        {
            return JoyButtons[arrayoffset];
        }

        public void changeButton(int buttonid, int newid)
        {
            JoyButtons[buttonid].buttonno = newid;
        }

        public int getHatSwitchDirection()
        {
            return (state.GetPointOfView())[0];
        }

        public int getNumberPOV()
        {
            return joystick.Capabilities.PovCount;
        }

        protected int BOOL_TO_SIGN(bool input)
        {
            return input ? -1 : 1;
        }

        /// <summary>
        /// Updates the rcoverride values and controls the mode changes
        /// </summary>
        protected abstract void mainloop();

        public abstract void clearRCOverride();

        public void DoJoystickButtonFunction()
        {
            foreach (JoyButton but in JoyButtons)
            {
                if (but.buttonno != -1)
                {
                    getButtonState(but, but.buttonno);
                }
            }
        }

        protected abstract void ProcessButtonEvent(JoyButton but, bool buttondown);
        

        
        /*
        const int RESXu = 1024;
        const int RESXul = 1024;
        const int RESXl = 1024;
        const int RESKul = 100;
        

        ushort expou(ushort x, ushort k)
        {
          // k*x*x*x + (1-k)*x
          return ((ulong)x*x*x/0x10000*k/(RESXul*RESXul/0x10000) + (RESKul-k)*x+RESKul/2)/RESKul;
        }
        // expo-funktion:
        // ---------------
        // kmplot
        // f(x,k)=exp(ln(x)*k/10) ;P[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20]
        // f(x,k)=x*x*x*k/10 + x*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]
        // f(x,k)=x*x*k/10 + x*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]
        // f(x,k)=1+(x-1)*(x-1)*(x-1)*k/10 + (x-1)*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]

        short expo(short x, short k)
        {
            if (k == 0) return x;
            short y;
            bool neg = x < 0;
            if (neg) x = -x;
            if (k < 0)
            {
                y = RESXu - expou((ushort)(RESXu - x), (ushort)-k);
            }
            else
            {
                y = expou((ushort)x, (ushort)k);
            }
            return neg ? -y : y;
        }

        */



        public void UnAcquireJoyStick()
        {
            if (joystick == null)
                return;
            joystick.Unacquire();
        }

        /// <summary>
        /// Button press check with debounce
        /// </summary>
        /// <param name="buttonno"></param>
        /// <returns></returns>
        bool getButtonState(JoyButton but, int buttonno)
        {
            var buts = state.GetButtons();

            // button down
            bool ans = buts[buttonno] && !buttonpressed[buttonno]; // press check + debounce
            if (ans)
                ButtonDown(but);

            // button up
            ans = !buts[buttonno] && buttonpressed[buttonno];
            if (ans)
                ButtonUp(but);

            buttonpressed[buttonno] = buts[buttonno]; // set only this button

            return ans;
        }

        void ButtonDown(JoyButton but)
        {
            ProcessButtonEvent(but, true);
        }

        void ButtonUp(JoyButton but)
        {
            ProcessButtonEvent(but, false);
        }

        public int getNumButtons()
        {
            if (joystick == null)
                return 0;
            return joystick.Capabilities.ButtonCount;
        }

        public joystickaxis getJoystickAxis(int channel)
        {
            try
            {
                return JoyChannels[channel].axis;
            }
            catch
            {
                return joystickaxis.None;
            }
        }

        public bool isButtonPressed(int buttonno)
        {
            var buts = state.GetButtons();

            if (buts == null || JoyButtons[buttonno].buttonno < 0)
                return false;

            return buts[JoyButtons[buttonno].buttonno];
        }

        public virtual short getValueForChannel(int channel, string name)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, JoyChannels[channel].reverse,
                JoyChannels[channel].expo);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        public virtual short getRawValueForChannel(int channel)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, false, 0);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        protected short pickchannel(int chan, joystickaxis axis, bool rev, int expo)
        {
            int raw;
            return pickchannel(chan, axis, rev, expo, out raw);
        }

        protected short pickchannel(int chan, joystickaxis axis, bool rev, int expo, out int rawJSVal)
        {
            rawJSVal = 0;
            int min, max, trim = 0;

            if (MainV2.comPort.MAV.param.Count > 0)
            {
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MIN"))
                    {
                        min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
                        max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
                        trim = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_TRIM"]);
                    }
                    else
                    {
                        min = 1000;
                        max = 2000;
                        trim = 1500;
                    }
                }
                catch
                {
                    min = 1000;
                    max = 2000;
                    trim = 1500;
                }
            }
            else
            {
                min = 1000;
                max = 2000;
                trim = 1500;
            }

            if (manual_control)
            {
                min = -1000;
                max = 1000;
                trim = 0;
            }

            if (chan == 3)
            {
                trim = (min + max) / 2;
                //                trim = min; // throttle
            }

            int range = Math.Abs(max - min);

            int working = 0;

            switch (axis)
            {
                case joystickaxis.None:
                    working = ushort.MaxValue / 2;
                    break;
                case joystickaxis.Pass:
                    working = (int)(((float)(trim - min) / range) * ushort.MaxValue);
                    break;
                case joystickaxis.ARx:
                    working = state.ARx;
                    break;

                case joystickaxis.ARy:
                    working = state.ARy;
                    break;

                case joystickaxis.ARz:
                    working = state.ARz;
                    break;

                case joystickaxis.AX:
                    working = state.AX;
                    break;

                case joystickaxis.AY:
                    working = state.AY;
                    break;

                case joystickaxis.AZ:
                    working = state.AZ;
                    break;

                case joystickaxis.FRx:
                    working = state.FRx;
                    break;

                case joystickaxis.FRy:
                    working = state.FRy;
                    break;

                case joystickaxis.FRz:
                    working = state.FRz;
                    break;

                case joystickaxis.FX:
                    working = state.FX;
                    break;

                case joystickaxis.FY:
                    working = state.FY;
                    break;

                case joystickaxis.FZ:
                    working = state.FZ;
                    break;

                case joystickaxis.Rx:
                    working = state.Rx;
                    break;

                case joystickaxis.Ry:
                    working = state.Ry;
                    break;

                case joystickaxis.Rz:
                    working = state.Rz;
                    break;

                case joystickaxis.VRx:
                    working = state.VRx;
                    break;

                case joystickaxis.VRy:
                    working = state.VRy;
                    break;

                case joystickaxis.VRz:
                    working = state.VRz;
                    break;

                case joystickaxis.VX:
                    working = state.VX;
                    break;

                case joystickaxis.VY:
                    working = state.VY;
                    break;

                case joystickaxis.VZ:
                    working = state.VZ;
                    break;

                case joystickaxis.X:
                    working = state.X;
                    break;

                case joystickaxis.Y:
                    working = state.Y;
                    break;

                case joystickaxis.Z:
                    working = state.Z;
                    break;

                case joystickaxis.Slider1:
                    int[] slider = state.GetSlider();
                    working = slider[0];
                    break;

                case joystickaxis.Slider2:
                    int[] slider1 = state.GetSlider();
                    working = slider1[1];
                    break;

                case joystickaxis.Hatud1:
                    hat1 = (int)Constrain(hat1, 0, 65535);
                    working = hat1;
                    break;

                case joystickaxis.Hatlr2:
                    hat2 = (int)Constrain(hat2, 0, 65535);
                    working = hat2;
                    break;

                case joystickaxis.Custom1:
                    working = (int)(((float)(custom0 - min) / range) * ushort.MaxValue);
                    working = (int)Constrain(working, 0, 65535);
                    break;

                case joystickaxis.Custom2:
                    working = (int)(((float)(custom1 - min) / range) * ushort.MaxValue);
                    working = (int)Constrain(working, 0, 65535);
                    break;
            }
            // between 0 and 65535 - convert to int -500 to 500
            working = (int)map(working, 0, 65535, -500, 500);

            if (rev)
                working *= -1;

            // save for later
            int raw = working;
            rawJSVal = raw;

            working = (int)Expo(working, expo, min, max, trim);

            //add limits to movement
            working = Math.Max(min, working);
            working = Math.Min(max, working);

            return (short)working;
        }

        public static double Expo(double input, double expo, double min, double max, double mid)
        {
            // input range -500 to 500

            double expomult = expo / 100.0;

            if (input >= 0)
            {
                // linear scale
                double linearpwm = map(input, 0, 500, mid, max);

                double expomid = (max - mid) / 2;

                double factor = 0;

                // over half way though input
                if (input > 250)
                {
                    factor = 250 - (input - 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor * expomult);
            }
            else
            {
                double linearpwm = map(input, -500, 0, min, mid);

                double expomid = (mid - min) / 2;

                double factor = 0;

                // over half way though input
                if (input < -250)
                {
                    factor = -250 - (input + 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor * expomult);
            }
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        double Constrain(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }
    }
}
