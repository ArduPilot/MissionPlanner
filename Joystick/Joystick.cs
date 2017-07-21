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
    public static class Extensions
    {
        public static MyJoystickState CurrentJoystickState(this SharpDX.DirectInput.Joystick joystick)
        {
            return new MyJoystickState(joystick.GetCurrentState());
        }
    }

    public class MyJoystickState
    {
        internal JoystickState baseJoystickState;

        public MyJoystickState(JoystickState state)
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

    public class Joystick : JoystickBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Joystick self;
        protected override string joystickconfigbutton { get; set; } = "joystickbuttons.xml";
        protected override string joystickconfigaxis { get; set; } = "joystickaxis.xml";

        public Joystick() : base("joystickbuttons" + getFirmware() + ".xml", "joystickaxis" + getFirmware() + ".xml")
        {
            self = this;
        }

        public static string getFirmware()
        {
            switch(MainV2.comPort.MAV.cs.firmware)
            {
                case MainV2.Firmwares.ArduPlane:
                case MainV2.Firmwares.ArduCopter2:
                case MainV2.Firmwares.ArduRover:
                    return MainV2.comPort.MAV.cs.firmware.ToString();
                default:
                    return "";
            }
        }

        public override void loadconfig(string joystickconfigbutton = "joystickbuttons.xml",
            string joystickconfigaxis = "joystickaxis.xml")
        {
            base.loadconfig(joystickconfigbutton, joystickconfigaxis);
        }

        public override bool start(string name, string ThreadName_IGNORED = "")
        {
            self.name = name;

            return base.start(name, "");
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
                            int angle = pov/100;

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
                        short roll = pickchannel(1, JoyChannels[1].axis, false, JoyChannels[1].expo);
                        short pitch = pickchannel(2, JoyChannels[2].axis, false, JoyChannels[2].expo);

                        if (getJoystickAxis(1) != joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech1 =
                                (short)
                                    (BOOL_TO_SIGN(JoyChannels[1].reverse)*((int) (pitch - 1500) - (int) (roll - 1500))/2 +
                                     1500);
                        if (getJoystickAxis(2) != joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech2 =
                                (short)
                                    (BOOL_TO_SIGN(JoyChannels[2].reverse)*((int) (pitch - 1500) + (int) (roll - 1500))/2 +
                                     1500);
                    }
                    else
                    {
                        if (getJoystickAxis(1) != joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech1 = pickchannel(1, JoyChannels[1].axis,
                                JoyChannels[1].reverse, JoyChannels[1].expo);
                                //(ushort)(((int)state.Rz / 65.535) + 1000);
                        if (getJoystickAxis(2) != joystickaxis.None)
                            MainV2.comPort.MAV.cs.rcoverridech2 = pickchannel(2, JoyChannels[2].axis,
                                JoyChannels[2].reverse, JoyChannels[2].expo);
                                //(ushort)(((int)state.Y / 65.535) + 1000);
                    }
                    if (getJoystickAxis(3) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech3 = pickchannel(3, JoyChannels[3].axis, JoyChannels[3].reverse,
                            JoyChannels[3].expo); //(ushort)(1000 - ((int)slider[0] / 65.535) + 1000);
                    if (getJoystickAxis(4) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech4 = pickchannel(4, JoyChannels[4].axis, JoyChannels[4].reverse,
                            JoyChannels[4].expo); //(ushort)(((int)state.X / 65.535) + 1000);

                    if (getJoystickAxis(5) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech5 = pickchannel(5, JoyChannels[5].axis, JoyChannels[5].reverse,
                            JoyChannels[5].expo);
                    if (getJoystickAxis(6) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech6 = pickchannel(6, JoyChannels[6].axis, JoyChannels[6].reverse,
                            JoyChannels[6].expo);
                    if (getJoystickAxis(7) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech7 = pickchannel(7, JoyChannels[7].axis, JoyChannels[7].reverse,
                            JoyChannels[7].expo);
                    if (getJoystickAxis(8) != joystickaxis.None)
                        MainV2.comPort.MAV.cs.rcoverridech8 = pickchannel(8, JoyChannels[8].axis, JoyChannels[8].reverse,
                            JoyChannels[8].expo);

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
                        delegate { CustomMessageBox.Show("Lost Joystick", "Lost Joystick"); });
                    return;
                }
                catch (Exception ex)
                {
                    log.Info("Joystick thread error " + ex.ToString());
                } // so we cant fall out
            }
        }

        public override void clearRCOverride()
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
                MainV2.comPort.sendPacket(rc, rc.target_system, rc.target_component);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void ProcessButtonEvent(JoyButton but, bool buttondown)
        {
            if (but.buttonno != -1)
            {
                // only do_set_relay and Button_axis0-1 uses the button up option
                if (buttondown == false)
                {
                    if (but.function != buttonfunction.Do_Set_Relay &&
                        but.function != buttonfunction.Button_axis0 &&
                        but.function != buttonfunction.Button_axis1)
                    {
                        return;
                    }
                }

                switch (but.function)
                {
                    case buttonfunction.ChangeMode:
                        string mode = but.mode;
                        if (mode != null)
                        {
                            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                            {
                                try
                                {
                                    MainV2.comPort.setMode(mode);
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Failed to change Modes");
                                }
                            });
                        }
                        break;
                    case buttonfunction.Mount_Mode:

                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.setParam("MNT_MODE", but.p1);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to change mount mode");
                            }
                        });

                        break;

                    case buttonfunction.Arm:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.doARM(true);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Arm");
                            }
                        });
                        break;
                    case buttonfunction.TakeOff:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.setMode("Guided");
                                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                                {
                                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                                }
                                else
                                {
                                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 20);
                                }
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to takeoff");
                            }
                        });
                        break;
                    case buttonfunction.Disarm:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.doARM(false);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Disarm");
                            }
                        });
                        break;
                    case buttonfunction.Do_Set_Relay:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int number = (int) but.p1;
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
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int relaynumber = (int) but.p1;
                                int repeat = (int) but.p2;
                                int time = (int) but.p3;
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
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int channel = (int) but.p1;
                                int pwm = (int) but.p2;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, channel, pwm, 0, 0, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_SET_SERVO");
                            }
                        });
                        break;
                    case buttonfunction.Do_Repeat_Servo:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int channelno = (int) but.p1;
                                int pwmvalue = (int) but.p2;
                                int repeattime = (int) but.p3;
                                int delay_ms = (int) but.p4;
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_SERVO, channelno, pwmvalue,
                                    repeattime, delay_ms, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_REPEAT_SERVO");
                            }
                        });
                        break;
                    case buttonfunction.Toggle_Pan_Stab:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                float current = (float) MainV2.comPort.MAV.param["MNT_STAB_PAN"];
                                float newvalue = (current > 0) ? 0 : 1;
                                MainV2.comPort.setParam("MNT_STAB_PAN", newvalue);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Toggle_Pan_Stab");
                            }
                        });
                        break;
                    case buttonfunction.Gimbal_pnt_track:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0,
                                    MainV2.comPort.MAV.cs.gimballat, MainV2.comPort.MAV.cs.gimballng,
                                    (float) MainV2.comPort.MAV.cs.GimbalPoint.Alt);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Gimbal_pnt_track");
                            }
                        });
                        break;
                    case buttonfunction.Mount_Control_0:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                MainV2.comPort.setMountControl(0, 0, 0, false);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Mount_Control_0");
                            }
                        });
                        break;
                    case buttonfunction.Button_axis0:
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int pwmmin = (int) but.p1;
                                int pwmmax = (int) but.p2;

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
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                        {
                            try
                            {
                                int pwmmin = (int) but.p1;
                                int pwmmax = (int) but.p2;

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
                    // these use the "MasterEnabled" property and not the "UserEnabled" property
                    // this is for safety so the AVO can force it off with a switch/button, and MPO cannot override
                    case buttonfunction.Toggle_CameraJoystick:
                        // maybe even show sometype of alert/msg like on the hud if this is enabled/disabled, if possible
                        if (MainV2.camerajoystick != null)
                        {
                            MainV2.camerajoystick.MasterEnabled = !MainV2.camerajoystick.MasterEnabled;
                        }
                        break;
                    case buttonfunction.Switch_CameraJoystick:
                        // this makes it enabled ONLY while the button is down, such as a physical switch
                        // maybe even show sometype of alert/msg like on the hud if this is enabled/disabled, if possible
                        if (MainV2.camerajoystick != null)
                        {
                            if (buttondown) MainV2.camerajoystick.MasterEnabled = true;
                            else MainV2.camerajoystick.MasterEnabled = false;
                        }
                        break;
                }
            }
        }
    }
}