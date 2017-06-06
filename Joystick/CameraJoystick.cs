using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MissionPlanner.Joystick.JoystickProperties;

namespace MissionPlanner.Joystick
{
    public class CameraJoystick : JoystickBase
    {

        public enum CameraAxis
        {
            Pan,
            Tilt,
            Zoom,
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static CameraJoystick self;
        protected override string joystickconfigbutton { get; set; } = "camerajoystickbuttons.xml";
        protected override string joystickconfigaxis { get; set; } = "camerajoystickaxis.xml";

        public bool UserEnabled = true;
        public bool MasterEnabled = true;

        public CameraJoystick() : base("camerajoystickbuttons" + getFirmware() + ".xml", "camerajoystickaxis" + getFirmware() + ".xml")
        {
            self = this;
        }

        public static string getFirmware()
        {
            switch (MainV2.comPort.MAV.cs.firmware)
            {
                case MainV2.Firmwares.ArduPlane:
                case MainV2.Firmwares.ArduCopter2:
                case MainV2.Firmwares.ArduRover:
                    return MainV2.comPort.MAV.cs.firmware.ToString();
                default:
                    return "";
            }
        }

        public override void loadconfig(string joystickconfigbutton = "camerajoystickbuttons.xml",
            string joystickconfigaxis = "camerajoystickaxis.xml")
        {
            base.loadconfig(joystickconfigbutton, joystickconfigaxis);
        }

        public override bool start(string name, string ThreadName_IGNORED="")
        {
            self.name = name;

            return base.start(name, "Camera");
        }

        public new static joystickaxis getMovingAxis(string name, int threshold)
        {
            self.name = name;
            return JoystickBase.getMovingAxis(name, threshold);
        }

        public new static int getPressedButton(string name)
        {
            self.name = name;
            return JoystickBase.getPressedButton(name);
        }

        /// <summary>
        /// Updates the rcoverride values and controls the mode changes
        /// </summary>
        protected override void mainloop()
        {
            while (enabled && joystick != null && !joystick.IsDisposed)
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    if (joystick.IsDisposed)
                        return;
                    //joystick stuff
                    joystick.Poll();
                    state = joystick.CurrentJoystickState();

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

                    if (MasterEnabled && UserEnabled)
                    {
                        if (getJoystickAxis((int)CameraAxis.Pan) != joystickaxis.None)
                        {

                            short val = pickchannel(JoyChannels[(int)CameraAxis.Pan].channel, JoyChannels[(int)CameraAxis.Pan].axis,
                                JoyChannels[(int)CameraAxis.Pan].reverse, JoyChannels[(int)CameraAxis.Pan].expo);
                            bool doOverride = false;
                            int ovc = JoyChannels[(int)CameraAxis.Pan].overridecenter;
                            if (ovc > 0)
                            {
                                // check for off center
                                int trim = getChanTrim(JoyChannels[(int)CameraAxis.Pan].channel);
                                if (
                                  (val > trim && (val - trim) >= ovc) ||
                                  (val < trim && (trim - val) >= ovc))
                                {
                                    doOverride = true;
                                }
                            }
                            else
                            {
                                doOverride = true;
                            }

                            setOverrideCh(JoyChannels[(int)CameraAxis.Pan].channel, doOverride ? val : (short)0);
                        }
                        if (getJoystickAxis((int)CameraAxis.Tilt) != joystickaxis.None)
                        {
                            int tilt_raw;
                            short val = pickchannel(JoyChannels[(int)CameraAxis.Tilt].channel, JoyChannels[(int)CameraAxis.Tilt].axis,
                                 JoyChannels[(int)CameraAxis.Tilt].reverse, JoyChannels[(int)CameraAxis.Tilt].expo, out tilt_raw);
                            bool doOverride = false;
                            int ovc = JoyChannels[(int)CameraAxis.Tilt].overridecenter;
                            if (ovc > 0)
                            {
                                // check for off center
                                int trim = getChanTrim(JoyChannels[(int)CameraAxis.Tilt].channel);
                                if (
                                  (val > trim && (val - trim) >= ovc) ||
                                  (val < trim && (trim - val) >= ovc))
                                {
                                    doOverride = true;
                                }
                            }
                            else
                            {
                                doOverride = true;
                            }

                            if (JoyChannels[(int)joystickaxis.Tilt].rateconv)
                            {
                                // tilt_raw is a vlaue from -500 to 500, which will end up being our rate
                                tilt_raw = tilt_raw / 5;
                                // we only do a change of up to 100 per cycle
                                int val_tmp = Convert.ToInt32(getOverrideCh(JoyChannels[(int)CameraAxis.Tilt].channel)) + tilt_raw;
                                val_tmp = Math.Max(getChanMin(JoyChannels[(int)CameraAxis.Tilt].channel), val_tmp);
                                val_tmp = Math.Min(getChanMax(JoyChannels[(int)CameraAxis.Tilt].channel), val_tmp);

                                if (val_tmp < UInt16.MinValue) val_tmp = UInt16.MinValue;
                                if (val_tmp > UInt16.MaxValue) val_tmp = UInt16.MaxValue;
                                val = Convert.ToInt16(val_tmp);
                            }

                            setOverrideCh(JoyChannels[(int)CameraAxis.Tilt].channel, doOverride ? val : (short)0);
                        }
                        if (getJoystickAxis((int)CameraAxis.Zoom) != joystickaxis.None)
                        {
                            int zoom_raw;
                            short val = pickchannel(JoyChannels[(int)CameraAxis.Zoom].channel, JoyChannels[(int)CameraAxis.Zoom].axis,
                                 JoyChannels[(int)CameraAxis.Zoom].reverse, JoyChannels[(int)CameraAxis.Zoom].expo, out zoom_raw);
                            bool doOverride = false;
                            int ovc = JoyChannels[(int)CameraAxis.Zoom].overridecenter;
                            if (ovc > 0)
                            {
                                // check for off center
                                int trim = getChanTrim(JoyChannels[(int)CameraAxis.Zoom].channel);
                                if (
                                  (val > trim && (val - trim) >= ovc) ||
                                  (val < trim && (trim - val) >= ovc))
                                {
                                    doOverride = true;
                                }
                            }
                            else
                            {
                                doOverride = true;
                            }

                            if (JoyChannels[(int)CameraAxis.Zoom].rateconv)
                            {
                                // tilt_raw is a vlaue from -500 to 500, which will end up being our rate
                                zoom_raw = zoom_raw / 5;
                                // we only do a change of up to 100 per cycle
                                int val_tmp = Convert.ToInt32(getOverrideCh(JoyChannels[(int)CameraAxis.Zoom].channel)) + zoom_raw;
                                val_tmp = Math.Max(getChanMin(JoyChannels[(int)CameraAxis.Zoom].channel), val_tmp);
                                val_tmp = Math.Min(getChanMax(JoyChannels[(int)CameraAxis.Zoom].channel), val_tmp);

                                if (val_tmp < UInt16.MinValue) val_tmp = UInt16.MinValue;
                                if (val_tmp > UInt16.MaxValue) val_tmp = UInt16.MaxValue;
                                val = Convert.ToInt16(val_tmp);
                            }

                            setOverrideCh(JoyChannels[(int)CameraAxis.Zoom].channel, doOverride ? val : (short)0);
                        }
                    }
                    else
                    {
                        if (getJoystickAxis((int)CameraAxis.Pan) != joystickaxis.None) setOverrideCh(JoyChannels[(int)CameraAxis.Pan].channel, (short)0);
                        if (getJoystickAxis((int)CameraAxis.Tilt) != joystickaxis.None) setOverrideCh(JoyChannels[(int)CameraAxis.Tilt].channel, (short)0);
                        if (getJoystickAxis((int)CameraAxis.Zoom) != joystickaxis.None) setOverrideCh(JoyChannels[(int)CameraAxis.Zoom].channel, (short)0);
                    }

                    // disable button actions when not connected.
                    if (MainV2.comPort.BaseStream.IsOpen)
                        DoJoystickButtonFunction();

                    //Console.WriteLine("{0} {1} {2} {3}", MainV2.comPort.MAV.cs.rcoverridech1, MainV2.comPort.MAV.cs.rcoverridech2, MainV2.comPort.MAV.cs.rcoverridech3, MainV2.comPort.MAV.cs.rcoverridech4);
                }
                catch (SharpDX.SharpDXException ex)
                {
                    log.Error(ex);
                    clearRCOverride();
                    MainV2.instance.Invoke((System.Action)
                        delegate { CustomMessageBox.Show("Lost Camera Joystick", "Lost Camera Joystick"); });
                    return;
                }
                catch (Exception ex)
                {
                    log.Info("Camera Joystick thread error " + ex.ToString());
                } // so we cant fall out
            }
        }

        public override void clearRCOverride()
        {
            this.clearRCOverride(false);
        }

        public void clearRCOverride(bool forceFullOff)
        {
            // disable it, before continuing
            this.enabled = false;

            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;

            MainV2.comPort.MAV.cs.CAMERA_rcoverridech1 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech2 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech3 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech4 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech5 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech6 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech7 = 0;
            MainV2.comPort.MAV.cs.CAMERA_rcoverridech8 = 0;

            if (forceFullOff)
            {
                rc.chan1_raw = 0;
                rc.chan2_raw = 0;
                rc.chan3_raw = 0;
                rc.chan4_raw = 0;
                rc.chan5_raw = 0;
                rc.chan6_raw = 0;
                rc.chan7_raw = 0;
                rc.chan8_raw = 0;
            }
            else
            {
                rc.chan1_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech1);
                rc.chan2_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech2);
                rc.chan3_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech3);
                rc.chan4_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech4);
                rc.chan5_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech5);
                rc.chan6_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech6);
                rc.chan7_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech7);
                rc.chan8_raw = Convert.ToUInt16(MainV2.comPort.MAV.cs.rcoverridech8);
            }

            try
            {
                if (forceFullOff)
                {
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    System.Threading.Thread.Sleep(20);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    System.Threading.Thread.Sleep(20);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    System.Threading.Thread.Sleep(20);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    System.Threading.Thread.Sleep(20);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    System.Threading.Thread.Sleep(20);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);

                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                    MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
                }

                // if not forceFullOff, we at least run it once
                MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            if (forceFullOff && MainV2.joystick != null)
            {
                MainV2.joystick.clearRCOverride();
            }
        }

        protected override void ProcessButtonEvent(JoyButton but, bool buttondown)
        {
            if (but.buttonno != -1)
            {

                if (!MasterEnabled || (!UserEnabled && but.function != buttonfunction.Switch_CameraJoystick && but.function != buttonfunction.Toggle_CameraJoystick))
                {
                    // the camera is disabled using a user defined button action
                    return;
                }

                // only do_set_relay and Button_axis0-1 uses the button up option
                if (buttondown == false)
                {
                    if (but.function != buttonfunction.Do_Set_Relay &&
                        but.function != buttonfunction.Button_axis0 &&
                        but.function != buttonfunction.Button_axis1 &&
                        but.function != buttonfunction.Switch_CameraJoystick)
                    {
                        return;
                    }
                }

                switch (but.function)
                {
                    
                    case buttonfunction.Do_Set_Relay:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int number = (int)but.p1;
                                int state = buttondown == true ? 1 : 0;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_RELAY, number, state, 0, 0, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_SET_RELAY");
                            }
                        });
                        break;
                    case buttonfunction.Digicam_Control:
                        MainV2.comPort.setDigicamControl(true);
                        break;
                    case buttonfunction.Do_Repeat_Relay:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int relaynumber = (int)but.p1;
                                int repeat = (int)but.p2;
                                int time = (int)but.p3;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_RELAY, relaynumber, repeat, time, 0,
                                    0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_REPEAT_RELAY");
                            }
                        });
                        break;
                    case buttonfunction.Do_Set_Servo:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int channel = (int)but.p1;
                                int pwm = (int)but.p2;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, channel, pwm, 0, 0, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_SET_SERVO");
                            }
                        });
                        break;
                    case buttonfunction.Do_Repeat_Servo:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int channelno = (int)but.p1;
                                int pwmvalue = (int)but.p2;
                                int repeattime = (int)but.p3;
                                int delay_ms = (int)but.p4;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_SERVO, channelno, pwmvalue,
                                    repeattime, delay_ms, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_REPEAT_SERVO");
                            }
                        });
                        break;
                    
                    case buttonfunction.Gimbal_pnt_track:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0,
                                    MainV2.comPort.MAV.cs.gimballat, MainV2.comPort.MAV.cs.gimballng,
                                    (float)MainV2.comPort.MAV.cs.GimbalPoint.Alt);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Gimbal_pnt_track");
                            }
                        });
                        break;
                    
                    case buttonfunction.Button_axis0:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int pwmmin = (int)but.p1;
                                int pwmmax = (int)but.p2;

                                if (buttondown)
                                    custom0 = pwmmax;
                                else
                                    custom0 = pwmmin;
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Button_axis0");
                            }
                        });
                        break;
                    case buttonfunction.Button_axis1:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                int pwmmin = (int)but.p1;
                                int pwmmax = (int)but.p2;

                                if (buttondown)
                                    custom1 = pwmmax;
                                else
                                    custom1 = pwmmin;
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Button_axis1");
                            }
                        });
                        break;
                    case buttonfunction.Toggle_CameraJoystick:
                        // maybe even show sometype of alert/msg like on the hud if this is enabled/disabled, if possible
                        this.UserEnabled = !UserEnabled;
                        break;
                    case buttonfunction.Switch_CameraJoystick:
                        // this makes it enabled ONLY while the button is down, such as a physical switch
                        // maybe even show sometype of alert/msg like on the hud if this is enabled/disabled, if possible
                        if (buttondown) this.UserEnabled = true;
                        else this.UserEnabled = false;
                        break;
                }
            }
        }

        public void setChannel(CameraAxis camaxis, int channel, joystickaxis axis, bool reverse, int expo, int overridecenter, bool RateConv)
        {
            JoyChannel joy = new JoyChannel();
            joy.axis = axis;
            joy.channel = channel;
            joy.expo = expo;
            joy.reverse = reverse;
            joy.camaxis = camaxis;
            joy.overridecenter = overridecenter;
            joy.rateconv = RateConv;

            JoyChannels[(int)axis] = joy;
        }

        public joystickaxis getJoystickAxis(CameraAxis axis)
        {
            return base.getJoystickAxis((int)axis);
        }


        public short getValueForChannel(CameraAxis axis, string name)
        {
            return getValueForChannel((int)axis, name);
        }

        public override short getValueForChannel(int axis, string name)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(JoyChannels[axis].channel, JoyChannels[axis].axis, JoyChannels[axis].reverse, JoyChannels[axis].expo);
            return ans;
        }

        public short getRawValueForChannel(CameraAxis axis)
        {
            return getRawValueForChannel((int)axis);
        }

        public override short getRawValueForChannel(int axis)
        {
           
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(JoyChannels[axis].channel, JoyChannels[axis].axis, false, 0);
            return ans;
        }

        public int getJoystickChannel(CameraAxis axis)
        {
            try
            {
                return JoyChannels[(int)axis].channel;
            }
            catch
            {
                return 0;
            }
        }

        public static void setOverrideCh(int ch, short val)
        {
            // if we are setting 0 (not overridden) and the main JS has it overridden, then don't set at all, leave at main Joystick value!

            //MainV2.comPort.MAV.cs.CAMERA_rcoverridech
            switch (ch)
            {
                case 1:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech1 = val;
                    break;
                case 2:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech2 = val;
                    break;
                case 3:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech3 = val;
                    break;
                case 4:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech4 = val;
                    break;
                case 5:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech5 = val;
                    break;
                case 6:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech6 = val;
                    break;
                case 7:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech7 = val;
                    break;
                case 8:
                    MainV2.comPort.MAV.cs.CAMERA_rcoverridech8 = val;
                    break;
            }
        }

        public static short getOverrideCh(int ch)
        {
            switch (ch)
            {
                case 1:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech1;
                case 2:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech2;
                case 3:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech3;
                case 4:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech4;
                case 5:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech5;
                case 6:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech6;
                case 7:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech7;
                case 8:
                    return MainV2.comPort.MAV.cs.CAMERA_rcoverridech8;
            }
            return (short)0;
        }

        int getChanTrim(int chan)
        {
            if (chan <= 0) return 0;

            int trim = 0;
            if (MainV2.comPort.MAV.param.Count > 0)
            {
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_TRIM"))
                    {
                        trim = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_TRIM"]);
                    }
                    else
                    {
                        trim = 1500;
                    }
                }
                catch
                {
                    trim = 1500;
                }
            }
            else
            {
                trim = 1500;
            }
            if (chan == 3)
            {
                int min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
                int max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
                trim = (min + max) / 2;
                //                trim = min; // throttle
            }
            return trim;
        }

        int getChanMax(int chan)
        {
            if (chan <= 0) return 0;

            int max = 0;
            if (MainV2.comPort.MAV.param.Count > 0)
            {
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MAX"))
                    {
                        max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
                    }
                    else
                    {
                        max = 3000;
                    }
                }
                catch
                {
                    max = 3000;
                }
            }
            else
            {
                max = 3000;
            }
            return max;
        }

        int getChanMin(int chan)
        {
            if (chan <= 0) return 0;

            int min = 0;
            if (MainV2.comPort.MAV.param.Count > 0)
            {
                try
                {
                    if (MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MIN"))
                    {
                        min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
                    }
                    else
                    {
                        min = 0;
                    }
                }
                catch
                {
                    min = 0;
                }
            }
            else
            {
                min = 0;
            }
            return min;
        }
    }
}
