﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.DirectX.DirectInput;
//using OpenTK.Input;
using System.Reflection;
using System.IO;

namespace MissionPlanner.Joystick
{
    public class Joystick : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        Device joystick;
        JoystickState state;
        public bool enabled = false;
        byte[] buttonpressed = new byte[128];
        public string name;
        public bool elevons = false;

        public static Joystick self;

        string joystickconfigbutton = "joystickbuttons.xml";
        string joystickconfigaxis = "joystickaxis.xml";

        // set to default midpoint
        int hat1 = 65535 / 2;
        int hat2 = 65535 / 2;
        int custom1 = 65535 / 2;
        int custom2 = 65535 / 2;


        public struct JoyChannel
        {
            public int channel;
            public joystickaxis axis;
            public bool reverse;
            public int expo;
        }

        public struct JoyButton
        {
            /// <summary>
            /// System button number
            /// </summary>
            public int buttonno;
            /// <summary>
            /// Fucntion we are doing for this button press
            /// </summary>
            public buttonfunction function;
            /// <summary>
            /// Mode we are changing to on button press
            /// </summary>
            public string mode;
            /// <summary>
            /// param 1
            /// </summary>
            public float p1;
            /// <summary>
            /// param 2
            /// </summary>
            public float p2;
            /// <summary>
            /// param 3
            /// </summary>
            public float p3;
            /// <summary>
            /// param 4
            /// </summary>
            public float p4;
            /// <summary>
            /// Relay state
            /// </summary>
            public bool state;
        }

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
            Mount_Control_0
        }


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
            catch { }

            try
            {
                if (disposing && joystick != null)
                    joystick.Dispose();
            }
            catch { }
            
            //tell gc not to call finalize, this object will be GC'd quicker now.
            GC.SuppressFinalize(this);
        }
        //no need for finalizer...
        //~Joystick()
        //{
        //    Dispose(false);
        //}

        public Joystick()
        {
            self = this;

            for (int a = 0; a < JoyButtons.Length; a++)
                JoyButtons[a].buttonno = -1;

            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                loadconfig("joystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml", "joystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
            {
                loadconfig("joystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml", "joystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
            {
                loadconfig("joystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml", "joystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
            }
            else
            {
                loadconfig();
            }
        }

        public void loadconfig(string joystickconfigbutton = "joystickbuttons.xml", string joystickconfigaxis = "joystickaxis.xml") 
        {
            log.Info("Loading joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save for later
            this.joystickconfigbutton = Application11.StartupPath + Path.DirectorySeparatorChar + joystickconfigbutton;
            this.joystickconfigaxis = Application11.StartupPath + Path.DirectorySeparatorChar + joystickconfigaxis;

            // load config
            if (File.Exists(joystickconfigbutton) && File.Exists(joystickconfigaxis))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(JoyButton[]), new Type[] { typeof(JoyButton) });

                    using (StreamReader sr = new StreamReader(joystickconfigbutton))
                    {
                        JoyButtons = (JoyButton[])reader.Deserialize(sr);
                    }
                }
                catch { }

                try
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(JoyChannel[]), new Type[] { typeof(JoyChannel) });

                    using (StreamReader sr = new StreamReader(joystickconfigaxis))
                    {
                        JoyChannels = (JoyChannel[])reader.Deserialize(sr);
                    }
                }
                catch { }
            }
        }

        public void saveconfig()
        {
            log.Info("Saving joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save config
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(JoyButton[]), new Type[] { typeof(JoyButton) });

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

        JoyChannel[] JoyChannels = new JoyChannel[9]; // we are base 1
        JoyButton[] JoyButtons = new JoyButton[128]; // base 0

        public static DeviceList getDevices()
        {
            return Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
        }

        public bool start(string name)
        {
            self.name = name;
            DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

            bool found = false;

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName == name)
                {
                    joystick = new Device(device.InstanceGuid);
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;

            joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

            System.Threading.Thread.Sleep(100);

            enabled = true;

            System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                Name = "Joystick loop",
                Priority = System.Threading.ThreadPriority.AboveNormal,
                IsBackground = true
            };
            t11.Start();

            return true;
        }

        public static joystickaxis getMovingAxis(string name, int threshold)
        {
            self.name = name;
            DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

            bool found = false;

            Device joystick = null;

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName == name)
                {
                    joystick = new Device(device.InstanceGuid);
                    found = true;
                    break;
                }
            }
            if (!found)
                return joystickaxis.ARx;

            joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

           // CustomMessageBox.Show("Please ensure you have calibrated your joystick in Windows first");

            // need a pause between aquire and poll
            System.Threading.Thread.Sleep(100);

            joystick.Poll();

            System.Threading.Thread.Sleep(50);

            JoystickState obj = joystick.CurrentJoystickState;
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
                JoystickState nextstate = joystick.CurrentJoystickState;

                int[] slider = nextstate.GetSlider();

                int[] hat1 = nextstate.GetPointOfView();

                type = nextstate.GetType();
                properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));

                    log.InfoFormat("test name {0} old {1} new {2} ", property.Name, values[property.Name], int.Parse(property.GetValue(nextstate, null).ToString()));
                    log.InfoFormat("{0}  {1} {2}", property.Name, (int)values[property.Name], (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold));
                    if ((int)values[property.Name] > (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold) ||
                        (int)values[property.Name] < (int.Parse(property.GetValue(nextstate, null).ToString()) - threshold))
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
            self.name = name;
            DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

            bool found = false;

            Device joystick = null;

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName == name)
                {
                    joystick = new Device(device.InstanceGuid);
                    found = true;
                    break;
                }
            }
            if (!found)
                return -1;

            joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

            System.Threading.Thread.Sleep(500);

            joystick.Poll();

            JoystickState obj = joystick.CurrentJoystickState;

            byte[] buttonsbefore = obj.GetButtons();

            CustomMessageBox.Show("Please press the joystick button you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                joystick.Poll();
                JoystickState nextstate = joystick.CurrentJoystickState;

                byte[] buttons = nextstate.GetButtons();

                for (int a = 0; a < joystick.Caps.NumberButtons; a++)
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
            return joystick.Caps.NumberPointOfViews;
        }

        int BOOL_TO_SIGN(bool input)
        {
            if (input == true)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Updates the rcoverride values and controls the mode changes
        /// </summary>
        void mainloop()
        {
            while (enabled)
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    //joystick stuff
                    joystick.Poll();
                    state = joystick.CurrentJoystickState;

                    //Console.WriteLine(state);

                    if (getNumberPOV() > 0)
                    {

                        int pov = getHatSwitchDirection();

                        if (pov != -1)
                        {
                            int angle = pov / 100;

                            //0 = down = 18000
                            //0 = up = 0

                            // 0
                            if (angle > 270 || angle < 90)
                                hat1 += 500;
                            // 180
                            if (angle > 90 && angle < 270)
                                hat1 -= 500;
                            // 90
                            if (angle > 0 && angle < 180)
                                hat2 += 500;
                            // 270
                            if (angle > 180 && angle < 360)
                                hat2 -= 500;
                        }
                    }

                    if (elevons)
                    {
                        //g.channel_roll.set_pwm(BOOL_TO_SIGN(g.reverse_elevons) * (BOOL_TO_SIGN(g.reverse_ch2_elevon) * int(ch2_temp - elevon2_trim) - BOOL_TO_SIGN(g.reverse_ch1_elevon) * int(ch1_temp - elevon1_trim)) / 2 + 1500);
                        //g.channel_pitch.set_pwm(                                 (BOOL_TO_SIGN(g.reverse_ch2_elevon) * int(ch2_temp - elevon2_trim) + BOOL_TO_SIGN(g.reverse_ch1_elevon) * int(ch1_temp - elevon1_trim)) / 2 + 1500);
                        ushort roll = pickchannel(1, JoyChannels[1].axis, false, JoyChannels[1].expo);
                        ushort pitch = pickchannel(2, JoyChannels[2].axis, false, JoyChannels[2].expo);

                        if (getJoystickAxis(1) != Joystick.joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech1 = (ushort)(BOOL_TO_SIGN(JoyChannels[1].reverse) * ((int)(pitch - 1500) - (int)(roll - 1500)) / 2 + 1500);
                        if (getJoystickAxis(2) != Joystick.joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech2 = (ushort)(BOOL_TO_SIGN(JoyChannels[2].reverse) * ((int)(pitch - 1500) + (int)(roll - 1500)) / 2 + 1500);
                    }
                    else
                    {
                        if (getJoystickAxis(1) != Joystick.joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech1 = pickchannel(1, JoyChannels[1].axis, JoyChannels[1].reverse, JoyChannels[1].expo);//(ushort)(((int)state.Rz / 65.535) + 1000);
                        if (getJoystickAxis(2) != Joystick.joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech2 = pickchannel(2, JoyChannels[2].axis, JoyChannels[2].reverse, JoyChannels[2].expo);//(ushort)(((int)state.Y / 65.535) + 1000);
                    }
                    if (getJoystickAxis(3) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech3 = pickchannel(3, JoyChannels[3].axis, JoyChannels[3].reverse, JoyChannels[3].expo);//(ushort)(1000 - ((int)slider[0] / 65.535) + 1000);
                    if (getJoystickAxis(4) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech4 = pickchannel(4, JoyChannels[4].axis, JoyChannels[4].reverse, JoyChannels[4].expo);//(ushort)(((int)state.X / 65.535) + 1000);

                    if (getJoystickAxis(5) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech5 = pickchannel(5, JoyChannels[5].axis, JoyChannels[5].reverse, JoyChannels[5].expo);
                    if (getJoystickAxis(6) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech6 = pickchannel(6, JoyChannels[6].axis, JoyChannels[6].reverse, JoyChannels[6].expo);
                    if (getJoystickAxis(7) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech7 = pickchannel(7, JoyChannels[7].axis, JoyChannels[7].reverse, JoyChannels[7].expo);
                    if (getJoystickAxis(8) != Joystick.joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech8 = pickchannel(8, JoyChannels[8].axis, JoyChannels[8].reverse, JoyChannels[8].expo);

                    // disable button actions when not connected.
                    if (MainV2.comPort.BaseStream.IsOpen)
                        DoJoystickButtonFunction();

                    //Console.WriteLine("{0} {1} {2} {3}", MainV2.comPort.MAV.cs.rcoverridech1, MainV2.comPort.MAV.cs.rcoverridech2, MainV2.comPort.MAV.cs.rcoverridech3, MainV2.comPort.MAV.cs.rcoverridech4);
                }
                catch (InputLostException ex)
                {
                    log.Error(ex);
                    clearRCOverride();
                    MainV2.instance.Invoke((System.Action)
                    delegate
                    {
                        CustomMessageBox.Show("Lost Joystick","Lost Joystick");
                    });                    
                    return;
                }
                catch (Exception ex) { log.Info("Joystick thread error " + ex.ToString()); } // so we cant fall out
            }
        }

        public void clearRCOverride()
        {
            // disable it, before continuing
            this.enabled = false;

            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;

            rc.chan1_raw = 0;
            rc.chan2_raw = 0;
            rc.chan3_raw = 0;
            rc.chan4_raw = 0;
            rc.chan5_raw = 0;
            rc.chan6_raw = 0;
            rc.chan7_raw = 0;
            rc.chan8_raw = 0;

            try
            {

                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);

                MainV2.comPort.sendPacket(rc);
                MainV2.comPort.sendPacket(rc);
                MainV2.comPort.sendPacket(rc);
            }
            catch (Exception ex) { log.Error(ex); }
        }

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

        void ProcessButtonEvent(JoyButton but, bool buttondown)
        {
            if (but.buttonno != -1)
            {
                // only do_set_relay uses the button up option
                if (buttondown == false)
                 if (but.function != buttonfunction.Do_Set_Relay)
                    return;

                switch (but.function)
                {
                    case buttonfunction.ChangeMode:
                        string mode = but.mode;
                        if (mode != null)
                        {
                            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                            {
                                try
                                {
                                    MainV2.comPort.setMode(mode);
                                }
                                catch { CustomMessageBox.Show("Failed to change Modes"); }
                            });
                        }
                        break;
                    case buttonfunction.Mount_Mode:
                        
                            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                            {
                                try
                                {
                                    MainV2.comPort.setParam("MNT_MODE", but.p1);
                                }
                                catch { CustomMessageBox.Show("Failed to change mount mode"); }
                            });
                        
                        break;
                        
                    case buttonfunction.Arm:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                            {
                                try
                                {
                                    MainV2.comPort.doARM(true);
                                }
                                catch { CustomMessageBox.Show("Failed to Arm"); }
                            });
                        break;
                    case buttonfunction.TakeOff:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                        {
                            try
                            {
                                MainV2.comPort.setMode("Guided");
                                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) {
                                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                                }
                                else
                                {
                                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 20);
                                }
                            }
                            catch { CustomMessageBox.Show("Failed to takeoff"); }
                        });
                        break;
                    case buttonfunction.Disarm:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                            {
                                try
                                {
                                    MainV2.comPort.doARM(false);
                                }
                                catch { CustomMessageBox.Show("Failed to Disarm"); }
                            });
                        break;
                    case buttonfunction.Do_Set_Relay:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                         {
                             try
                             {
                                 int number = (int)but.p1;
                                 int state = buttondown == true ? 1 : 0;
                                 MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_RELAY, number, state, 0, 0, 0, 0, 0);
                             }
                             catch { CustomMessageBox.Show("Failed to DO_SET_RELAY"); }
                         });
                        break;
                    case buttonfunction.Digicam_Control:
                        MainV2.comPort.setDigicamControl(true);
                        break;
                    case buttonfunction.Do_Repeat_Relay:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                            {
                                try
                                {
                                    int relaynumber = (int)but.p1;
                                    int repeat = (int)but.p2;
                                    int time = (int)but.p3;
                                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_RELAY, relaynumber, repeat, time, 0, 0, 0, 0);
                                }
                                catch { CustomMessageBox.Show("Failed to DO_REPEAT_RELAY"); }
                            });
                        break;
                    case buttonfunction.Do_Set_Servo:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                           {
                               try
                               {
                                   int channel = (int)but.p1;
                                   int pwm = (int)but.p2;
                                   MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, channel, pwm, 0, 0, 0, 0, 0);
                               }
                               catch { CustomMessageBox.Show("Failed to DO_SET_SERVO"); }
                           });
                        break;
                    case buttonfunction.Do_Repeat_Servo:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                           {
                               try
                               {
                                   int channelno = (int)but.p1;
                                   int pwmvalue = (int)but.p2;
                                   int repeattime = (int)but.p3;
                                   int delay_ms = (int)but.p4;
                                   MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_SERVO, channelno, pwmvalue, repeattime, delay_ms, 0, 0, 0);
                               }
                               catch { CustomMessageBox.Show("Failed to DO_REPEAT_SERVO"); }
                           });
                        break;
                    case buttonfunction.Toggle_Pan_Stab:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                          {
                              try
                              {
                                  float current = (float)MainV2.comPort.MAV.param["MNT_STAB_PAN"];
                                  float newvalue = (current > 0) ? 0 : 1;
                                  MainV2.comPort.setParam("MNT_STAB_PAN", newvalue);
                              }
                              catch { CustomMessageBox.Show("Failed to Toggle_Pan_Stab"); }
                          });
                        break;
                    case buttonfunction.Gimbal_pnt_track:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                        {
                            try
                            {
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0, MainV2.comPort.MAV.cs.gimballat, MainV2.comPort.MAV.cs.gimballng, (float)MainV2.comPort.MAV.cs.GimbalPoint.Alt);
                            }
                            catch { CustomMessageBox.Show("Failed to Gimbal_pnt_track"); }
                        });
                        break;
                    case buttonfunction.Mount_Control_0:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                        {
                            try
                            {
                                MainV2.comPort.setMountControl(0,0,0,false);
                            }
                            catch { CustomMessageBox.Show("Failed to Mount_Control_0"); }
                        });
                        break;
                }
            }
        }

        public enum joystickaxis
        {
            None,
            Pass,
            ARx,
            ARy,
            ARz,
            AX,
            AY,
            AZ,
            FRx,
            FRy,
            FRz,
            FX,
            FY,
            FZ,
            Rx,
            Ry,
            Rz,
            VRx,
            VRy,
            VRz,
            VX,
            VY,
            VZ,
            X,
            Y,
            Z,
            Slider1,
            Slider2,
            Hatud1,
            Hatlr2,
            Custom1,
            Custom2
        }

        const int RESXu = 1024;
        const int RESXul = 1024;
        const int RESXl = 1024;
        const int RESKul = 100;
        /*

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

        public Device AcquireJoystick(string name)
        {
            DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

            bool found = false;

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName == name)
                {
                    joystick = new Device(device.InstanceGuid);
                    found = true;
                    break;
                }
            }

            if (!found)
                return null;

            joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

            System.Threading.Thread.Sleep(500);

            joystick.Poll();

            return joystick;
        }

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
            byte[] buts = state.GetButtons();

            // button down
            bool ans = buts[buttonno] > 0 && buttonpressed[buttonno] == 0; // press check + debounce
            if (ans)
                ButtonDown(but);

            // button up
            ans = buts[buttonno] == 0 && buttonpressed[buttonno] > 0;
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
            return joystick.Caps.NumberButtons;
        }

        public joystickaxis getJoystickAxis(int channel)
        {
            try
            {
                return JoyChannels[channel].axis;
            }
            catch { return joystickaxis.None; }
        }

        public bool isButtonPressed(int buttonno)
        {
            byte[] buts = state.GetButtons();

            if (buts == null || JoyButtons[buttonno].buttonno < 0)
                return false;

            return buts[JoyButtons[buttonno].buttonno] > 0;
        }

        public ushort getValueForChannel(int channel, string name)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState;

            ushort ans = pickchannel(channel, JoyChannels[channel].axis, JoyChannels[channel].reverse, JoyChannels[channel].expo);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        ushort pickchannel(int chan, joystickaxis axis, bool rev, int expo)
        {
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
                    custom1 = (int)Constrain(custom1, 0, 65535);
                    working = custom1;
                    break;

                case joystickaxis.Custom2:
                    custom2 = (int)Constrain(custom2, 0, 65535);
                    working = custom2;
                    break;
            }
            // between 0 and 65535 - convert to int -500 to 500
            working = (int)(working / 65.535) - 500;

            if (rev)
                working *= -1;

            // save for later
            int raw = working;

            working = (int)Expo(working, expo, min, max, trim);

            /*
            // calc scale from actualy pwm range
            float scale = range / 1000.0f;

            


            double B = 4 * (expo / 100.0);
            double A = 1 - 0.25 * B;

            double t_in = working / 1000.0;
            double t_out = 0;
            double mid = trim / 1000.0;

            t_out = A * (t_in) + B * Math.Pow((t_in), 3);

            t_out = mid + t_out * scale;

            //            Console.WriteLine("tin {0} tout {1}",t_in,t_out);

            working = (int)(t_out * 1000);

             
            if (expo == 0)
            {
                working = (int)(raw) + trim;
            }*/

            //add limits to movement
            working = Math.Max(min, working);
            working = Math.Min(max, working);

            return (ushort)working;
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
                    factor = 250 - (input-250); 
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