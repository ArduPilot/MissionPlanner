using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.DirectX.DirectInput;
//using OpenTK.Input;
using System.Reflection;
using System.IO;
using System.Collections;
using MissionPlanner;
using log4net;
//using MissionPlanner.Joystick;


namespace MissionPlanner.Joystick {
  /// <summary>
  /// Duplicate of Joystick class, renamed to CameraJoystick, some functionality disabled/added
  /// </summary>
  public class CameraJoystick : IDisposable {
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    Device joystick;
    JoystickState state;
    public bool enabled = false;
    public bool UserEnabled = true;
    public bool MasterEnabled = true;
    byte[] buttonpressed = new byte[128];
    public string name;
    //public bool elevons = false;

    public static CameraJoystick self;

    string joystickconfigbutton = "camerajoystickbuttons.xml";
    string joystickconfigaxis = "camerajoystickaxis.xml";

    // set to default midpoint
    int hat1 = 65535 / 2;
    int hat2 = 65535 / 2;
    int custom0 = 65535 / 2;
    int custom1 = 65535 / 2;


    public enum CameraAxis {
      Pan,
      Tilt,
      Zoom,
    }

    public struct CameraJoyAxis {
      public CameraAxis camaxis;
      public int channel;
      public joystickaxis axis;
      public bool reverse;
      public int expo;
      public int overridecenter;
      internal bool rateconv;
    }

    public struct CameraJoyButton {
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

    public enum buttonfunction {
      //ChangeMode,
      Do_Set_Relay,
      Do_Repeat_Relay,
      Do_Set_Servo,
      Do_Repeat_Servo,
      //Arm,
      //Disarm,
      Digicam_Control,
      //TakeOff,
      //Mount_Mode,
      //Toggle_Pan_Stab,
      Gimbal_pnt_track,
      //Mount_Control_0,
      Button_axis0,
      Button_axis1,
      Toggle_CameraJoystick,
      Switch_CameraJoystick,
    }


    public void Dispose() {
      Dispose(true);
    }

    /// <summary>
    /// Implement reccomended best practice dispose pattern
    /// http://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx
    /// </summary>
    /// <param name="disposing"></param>
    virtual protected void Dispose(bool disposing) {
      try {
        //not sure if this is a problem from the finalizer?
        if(disposing && joystick != null && joystick.Properties != null)
          joystick.Unacquire();
      } catch {
      }

      try {
        if(disposing && joystick != null)
          joystick.Dispose();
      } catch {
      }

      //tell gc not to call finalize, this object will be GC'd quicker now.
      GC.SuppressFinalize(this);
    }

    //no need for finalizer...
    //~Joystick()
    //{
    //    Dispose(false);
    //}

    public CameraJoystick() {
      self = this;

      for(int a = 0; a < JoyButtons.Length; a++)
        JoyButtons[a].buttonno = -1;

      if(MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane) {
        loadconfig("camerajoystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml",
            "camerajoystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
      } else if(MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) {
        loadconfig("camerajoystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml",
            "camerajoystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
      } else if(MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover) {
        loadconfig("camerajoystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml",
            "camerajoystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
      } else {
        loadconfig();
      }
    }

    public void loadconfig(string joystickconfigbutton = "camerajoystickbuttons.xml",
        string joystickconfigaxis = "camerajoystickaxis.xml") {
      log.Info("Loading camerajoystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

      // save for later
      this.joystickconfigbutton = Application11.StartupPath + Path.DirectorySeparatorChar + joystickconfigbutton;
      this.joystickconfigaxis = Application11.StartupPath + Path.DirectorySeparatorChar + joystickconfigaxis;

      // load config
      if(File.Exists(joystickconfigbutton) && File.Exists(joystickconfigaxis)) {
        try {
          System.Xml.Serialization.XmlSerializer reader =
              new System.Xml.Serialization.XmlSerializer(typeof(CameraJoyButton[]), new Type[] { typeof(CameraJoyButton) });

          using(StreamReader sr = new StreamReader(joystickconfigbutton)) {
            JoyButtons = (CameraJoyButton[])reader.Deserialize(sr);
          }
        } catch {
        }

        try {
          System.Xml.Serialization.XmlSerializer reader =
              new System.Xml.Serialization.XmlSerializer(typeof(CameraJoyAxis[]),
                  new Type[] { typeof(CameraJoyAxis) });

          using(StreamReader sr = new StreamReader(joystickconfigaxis)) {
            JoyAxes = (CameraJoyAxis[])reader.Deserialize(sr);
          }
        } catch {
        }
      }
    }

    public void saveconfig() {
      log.Info("Saving camerajoystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

      // save config
      System.Xml.Serialization.XmlSerializer writer =
          new System.Xml.Serialization.XmlSerializer(typeof(CameraJoyButton[]), new Type[] { typeof(CameraJoyButton) });

      using(StreamWriter sw = new StreamWriter(joystickconfigbutton)) {
        writer.Serialize(sw, JoyButtons);
      }

      writer = new System.Xml.Serialization.XmlSerializer(typeof(CameraJoyAxis[]), new Type[] { typeof(CameraJoyAxis) });

      using(StreamWriter sw = new StreamWriter(joystickconfigaxis)) {
        writer.Serialize(sw, JoyAxes);
      }
    }

    // 3 camera axes = pan, tilt, zoom
    CameraJoyAxis[] JoyAxes = new CameraJoyAxis[3]; // we are base 0
    CameraJoyButton[] JoyButtons = new CameraJoyButton[128]; // base 0

    public static DeviceList getDevices() {
      return Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
    }

    public bool start(string name) {
      self.name = name;
      DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

      bool found = false;

      foreach(DeviceInstance device in joysticklist) {
        if(device.ProductName == name) {
          joystick = new Device(device.InstanceGuid);
          found = true;
          break;
        }
      }
      if(!found)
        return false;

      joystick.SetDataFormat(DeviceDataFormat.Joystick);

      try {
        joystick.Unacquire();
      } catch { }

      joystick.Acquire();

      System.Threading.Thread.Sleep(100);

      enabled = true;

      System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop)) {
        Name = "CameraJoystick loop",
        //Priority = System.Threading.ThreadPriority.AboveNormal,
        Priority = System.Threading.ThreadPriority.Normal,
        IsBackground = true
      };
      t11.Start();

      return true;
    }

    public static joystickaxis getMovingAxis(string name, int threshold) {
      self.name = name;
      DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

      bool found = false;

      Device joystick = null;

      foreach(DeviceInstance device in joysticklist) {
        if(device.ProductName == name) {
          joystick = new Device(device.InstanceGuid);
          found = true;
          break;
        }
      }
      if(!found)
        return joystickaxis.ARx;

      joystick.SetDataFormat(DeviceDataFormat.Joystick);

      try {
        joystick.Unacquire();
      } catch { }

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
      foreach(PropertyInfo property in properties) {
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

      while(start.AddSeconds(10) > DateTime.Now) {
        joystick.Poll();
        System.Threading.Thread.Sleep(50);
        JoystickState nextstate = joystick.CurrentJoystickState;

        int[] slider = nextstate.GetSlider();

        int[] hat1 = nextstate.GetPointOfView();

        type = nextstate.GetType();
        properties = type.GetProperties();
        foreach(PropertyInfo property in properties) {
          //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));

          log.InfoFormat("test name {0} old {1} new {2} ", property.Name, values[property.Name],
              int.Parse(property.GetValue(nextstate, null).ToString()));
          log.InfoFormat("{0}  {1} {2}", property.Name, (int)values[property.Name],
              (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold));
          if((int)values[property.Name] >
              (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold) ||
              (int)values[property.Name] <
              (int.Parse(property.GetValue(nextstate, null).ToString()) - threshold)) {
            log.Info(property.Name);
            joystick.Unacquire();
            return (joystickaxis)Enum.Parse(typeof(joystickaxis), property.Name);
          }
        }

        // slider1
        if((int)values["Slider1"] > (slider[0] + threshold) ||
            (int)values["Slider1"] < (slider[0] - threshold)) {
          joystick.Unacquire();
          return joystickaxis.Slider1;
        }

        // slider2
        if((int)values["Slider2"] > (slider[1] + threshold) ||
            (int)values["Slider2"] < (slider[1] - threshold)) {
          joystick.Unacquire();
          return joystickaxis.Slider2;
        }

        // Hatud1
        if((int)values["Hatud1"] != (hat1[0])) {
          joystick.Unacquire();
          return joystickaxis.Hatud1;
        }

        // Hatlr2
        if((int)values["Hatlr2"] != (hat1[0])) {
          joystick.Unacquire();
          return joystickaxis.Hatlr2;
        }
      }

      CustomMessageBox.Show("No valid option was detected");

      return joystickaxis.None;
    }

    public static int getPressedButton(string name) {
      self.name = name;
      DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

      bool found = false;

      Device joystick = null;

      foreach(DeviceInstance device in joysticklist) {
        if(device.ProductName == name) {
          joystick = new Device(device.InstanceGuid);
          found = true;
          break;
        }
      }
      if(!found)
        return -1;

      joystick.SetDataFormat(DeviceDataFormat.Joystick);

      try {
        joystick.Unacquire();
      } catch { }

      joystick.Acquire();

      System.Threading.Thread.Sleep(500);

      joystick.Poll();

      JoystickState obj = joystick.CurrentJoystickState;

      byte[] buttonsbefore = obj.GetButtons();

      CustomMessageBox.Show(
          "Please press the joystick button you want assigned to this function after clicking ok");

      DateTime start = DateTime.Now;

      while(start.AddSeconds(10) > DateTime.Now) {
        joystick.Poll();
        JoystickState nextstate = joystick.CurrentJoystickState;

        byte[] buttons = nextstate.GetButtons();

        for(int a = 0; a < joystick.Caps.NumberButtons; a++) {
          if(buttons[a] != buttonsbefore[a])
            return a;
        }
      }

      CustomMessageBox.Show("No valid option was detected");

      return -1;
    }

    public void setReverse(CameraAxis axis, bool reverse) {
      JoyAxes[(int)axis].reverse = reverse;
    }

    public void setAxis(CameraAxis axis, joystickaxis joyaxis) {
      JoyAxes[(int)axis].axis = joyaxis;
    }

    public void setChannel(CameraAxis axis, int channel, joystickaxis joyaxis, bool reverse, int expo, int overridecenter, bool RateConv) {
      CameraJoyAxis joy = new CameraJoyAxis();
      joy.axis = joyaxis;
      joy.channel = channel;
      joy.expo = expo;
      joy.reverse = reverse;
      joy.camaxis = axis;
      joy.overridecenter = overridecenter;
      joy.rateconv = RateConv;

      JoyAxes[(int)axis] = joy;
    }

    public void setChannel(CameraJoyAxis chan) {
      JoyAxes[(int)chan.camaxis] = chan;
    }

    public CameraJoyAxis getChannel(CameraAxis axis) {
      return JoyAxes[(int)axis];
    }

    public void setButton(int arrayoffset, CameraJoyButton buttonconfig) {
      JoyButtons[arrayoffset] = buttonconfig;
    }

    public CameraJoyButton getButton(int arrayoffset) {
      return JoyButtons[arrayoffset];
    }

    public void changeButton(int buttonid, int newid) {
      JoyButtons[buttonid].buttonno = newid;
    }

    public int getHatSwitchDirection() {
      return (state.GetPointOfView())[0];
    }

    public int getNumberPOV() {
      return joystick.Caps.NumberPointOfViews;
    }

    int BOOL_TO_SIGN(bool input) {
      if(input == true) {
        return -1;
      } else {
        return 1;
      }
    }

    /// <summary>
    /// Updates the rcoverride values and controls the mode changes
    /// </summary>
    void mainloop() {
      while(enabled) {
        try {
          System.Threading.Thread.Sleep(50);
          //joystick stuff
          joystick.Poll();
          state = joystick.CurrentJoystickState;

          //Console.WriteLine(state);

          if(getNumberPOV() > 0) {
            int pov = getHatSwitchDirection();

            if(pov != -1) {
              int angle = pov / 100;

              //0 = down = 18000
              //0 = up = 0

              // 0
              if(angle > 270 || angle < 90)
                hat1 += 500;
              // 180
              if(angle > 90 && angle < 270)
                hat1 -= 500;
              // 90
              if(angle > 0 && angle < 180)
                hat2 += 500;
              // 270
              if(angle > 180 && angle < 360)
                hat2 -= 500;
            }
          }

          if(MasterEnabled && UserEnabled) {
            if(getJoystickAxis(CameraAxis.Pan) != CameraJoystick.joystickaxis.None) {

              ushort val = pickchannel(JoyAxes[(int)CameraAxis.Pan].channel, JoyAxes[(int)CameraAxis.Pan].axis,
                  JoyAxes[(int)CameraAxis.Pan].reverse, JoyAxes[(int)CameraAxis.Pan].expo);
              bool doOverride = false;
              int ovc = JoyAxes[(int)CameraAxis.Pan].overridecenter;
              if(ovc > 0) {
                // check for off center
                int trim = getChanTrim(JoyAxes[(int)CameraAxis.Pan].channel);
                if(
                  (val > trim && (val - trim) >= ovc) ||
                  (val < trim && (trim - val) >= ovc)) {
                  doOverride = true;
                }
              } else {
                doOverride = true;
              }

              setOverrideCh(JoyAxes[(int)CameraAxis.Pan].channel, doOverride ? val : (ushort)0);
            }
            if(getJoystickAxis(CameraAxis.Tilt) != CameraJoystick.joystickaxis.None) {
              int tilt_raw;
              ushort val = pickchannel(JoyAxes[(int)CameraAxis.Tilt].channel, JoyAxes[(int)CameraAxis.Tilt].axis,
                   JoyAxes[(int)CameraAxis.Tilt].reverse, JoyAxes[(int)CameraAxis.Tilt].expo, out tilt_raw);
              bool doOverride = false;
              int ovc = JoyAxes[(int)CameraAxis.Tilt].overridecenter;
              if(ovc > 0) {
                // check for off center
                int trim = getChanTrim(JoyAxes[(int)CameraAxis.Tilt].channel);
                if(
                  (val > trim && (val - trim) >= ovc) ||
                  (val < trim && (trim - val) >= ovc)) {
                  doOverride = true;
                }
              } else {
                doOverride = true;
              }

              if(JoyAxes[(int)CameraAxis.Tilt].rateconv) {
                // tilt_raw is a vlaue from -500 to 500, which will end up being our rate
                tilt_raw = tilt_raw / 5;
                // we only do a change of up to 100 per cycle
                int val_tmp = Convert.ToInt32(getOverrideCh(JoyAxes[(int)CameraAxis.Tilt].channel)) + tilt_raw;
                val_tmp = Math.Max(getChanMin(JoyAxes[(int)CameraAxis.Tilt].channel), val_tmp);
                val_tmp = Math.Min(getChanMax(JoyAxes[(int)CameraAxis.Tilt].channel), val_tmp);

                if(val_tmp < UInt16.MinValue) val_tmp = UInt16.MinValue;
                if(val_tmp > UInt16.MaxValue) val_tmp = UInt16.MaxValue;
                val = Convert.ToUInt16(val_tmp);
              }

              setOverrideCh(JoyAxes[(int)CameraAxis.Tilt].channel, doOverride ? val : (ushort)0);
            }
            if(getJoystickAxis(CameraAxis.Zoom) != CameraJoystick.joystickaxis.None) {
              int zoom_raw;
              ushort val = pickchannel(JoyAxes[(int)CameraAxis.Zoom].channel, JoyAxes[(int)CameraAxis.Zoom].axis,
                   JoyAxes[(int)CameraAxis.Zoom].reverse, JoyAxes[(int)CameraAxis.Zoom].expo, out zoom_raw);
              bool doOverride = false;
              int ovc = JoyAxes[(int)CameraAxis.Zoom].overridecenter;
              if(ovc > 0) {
                // check for off center
                int trim = getChanTrim(JoyAxes[(int)CameraAxis.Zoom].channel);
                if(
                  (val > trim && (val - trim) >= ovc) ||
                  (val < trim && (trim - val) >= ovc)) {
                  doOverride = true;
                }
              } else {
                doOverride = true;
              }

              if(JoyAxes[(int)CameraAxis.Zoom].rateconv) {
                // tilt_raw is a vlaue from -500 to 500, which will end up being our rate
                zoom_raw = zoom_raw / 5;
                // we only do a change of up to 100 per cycle
                int val_tmp = Convert.ToInt32(getOverrideCh(JoyAxes[(int)CameraAxis.Zoom].channel)) + zoom_raw;
                val_tmp = Math.Max(getChanMin(JoyAxes[(int)CameraAxis.Zoom].channel), val_tmp);
                val_tmp = Math.Min(getChanMax(JoyAxes[(int)CameraAxis.Zoom].channel), val_tmp);

                if(val_tmp < UInt16.MinValue) val_tmp = UInt16.MinValue;
                if(val_tmp > UInt16.MaxValue) val_tmp = UInt16.MaxValue;
                val = Convert.ToUInt16(val_tmp);
              }

              setOverrideCh(JoyAxes[(int)CameraAxis.Zoom].channel, doOverride ? val : (ushort)0);
            }
          } else {
            if(getJoystickAxis(CameraAxis.Pan) != CameraJoystick.joystickaxis.None) setOverrideCh(JoyAxes[(int)CameraAxis.Pan].channel, (ushort)0);
            if(getJoystickAxis(CameraAxis.Tilt) != CameraJoystick.joystickaxis.None) setOverrideCh(JoyAxes[(int)CameraAxis.Tilt].channel, (ushort)0);
            if(getJoystickAxis(CameraAxis.Zoom) != CameraJoystick.joystickaxis.None) setOverrideCh(JoyAxes[(int)CameraAxis.Zoom].channel, (ushort)0);
          }

          // disable button actions when not connected.
          if(MainV2.comPort.BaseStream.IsOpen)
            DoJoystickButtonFunction();

          Console.WriteLine("{0} {1} {2} {3}", MainV2.comPort.MAV.cs.rcoverridech1, MainV2.comPort.MAV.cs.rcoverridech2, MainV2.comPort.MAV.cs.rcoverridech3, MainV2.comPort.MAV.cs.rcoverridech4);
        } catch(InputLostException ex) {
          log.Error(ex);
          clearRCOverride();
          MainV2.instance.Invoke((System.Action)
              delegate { CustomMessageBox.Show("Lost CameraJoystick", "Lost CameraJoystick"); });
          return;
        } catch(Exception ex) {
          log.Info("CameraJoystick thread error " + ex.ToString());
        } // so we cant fall out
      }
    }

    public static void setOverrideCh(int ch, ushort val) {
      // if we are setting 0 (not overridden) and the main JS has it overridden, then don't set at all, leave at main Joystick value!

      //MainV2.comPort.MAV.cs.CAMERA_rcoverridech
      switch(ch) {
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

    public static ushort getOverrideCh(int ch) {
      switch(ch) {
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
      return (ushort)0;
    }

    public void clearRCOverride(bool forceFullOff=false) {
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

      if(forceFullOff) {
        rc.chan1_raw = 0;
        rc.chan2_raw = 0;
        rc.chan3_raw = 0;
        rc.chan4_raw = 0;
        rc.chan5_raw = 0;
        rc.chan6_raw = 0;
        rc.chan7_raw = 0;
        rc.chan8_raw = 0;
      } else {
        rc.chan1_raw = MainV2.comPort.MAV.cs.rcoverridech1;
        rc.chan2_raw = MainV2.comPort.MAV.cs.rcoverridech2;
        rc.chan3_raw = MainV2.comPort.MAV.cs.rcoverridech3;
        rc.chan4_raw = MainV2.comPort.MAV.cs.rcoverridech4;
        rc.chan5_raw = MainV2.comPort.MAV.cs.rcoverridech5;
        rc.chan6_raw = MainV2.comPort.MAV.cs.rcoverridech6;
        rc.chan7_raw = MainV2.comPort.MAV.cs.rcoverridech7;
        rc.chan8_raw = MainV2.comPort.MAV.cs.rcoverridech8;
      }
      
      
      try {
        if(forceFullOff) {
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
        }
        // if not forceFullOff, we at least run it once
        MainV2.comPort.sendPacket(rc);
      } catch(Exception ex) {
        //log.Error(ex);
      }

      if(forceFullOff && MainV2.joystick != null) {
        MainV2.joystick.clearRCOverride();
      }
    }

    public void DoJoystickButtonFunction() {
      foreach(CameraJoyButton but in JoyButtons) {
        if(but.buttonno != -1) {
          getButtonState(but, but.buttonno);
        }
      }
    }

    void ProcessButtonEvent(CameraJoyButton but, bool buttondown) {
      if(but.buttonno != -1) {
        if(!MasterEnabled || (!UserEnabled && but.function != buttonfunction.Switch_CameraJoystick && but.function != buttonfunction.Toggle_CameraJoystick)) {
          // the camera is disabled using a user defined button action
          return;
        }


        // only do_set_relay and Button_axis0-1 uses the button up option
        if(buttondown == false) {
          if(but.function != buttonfunction.Do_Set_Relay &&
              but.function != buttonfunction.Button_axis0 &&
              but.function != buttonfunction.Button_axis1 &&
              but.function != buttonfunction.Switch_CameraJoystick) {
            return;
          }
        }

        switch(but.function) {
          /*
          case buttonfunction.ChangeMode:
            string mode = but.mode;
            if(mode != null) {
              MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
              {
                try {
                  MainV2.comPort.setMode(mode);
                } catch {
                  CustomMessageBox.Show("Failed to change Modes");
                }
              });
            }
            break;
          case buttonfunction.Mount_Mode:

            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.setParam("MNT_MODE", but.p1);
              } catch {
                CustomMessageBox.Show("Failed to change mount mode");
              }
            });

            break;

          case buttonfunction.Arm:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.doARM(true);
              } catch {
                CustomMessageBox.Show("Failed to Arm");
              }
            });
            break;
          case buttonfunction.TakeOff:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.setMode("Guided");
                if(MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) {
                  MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                } else {
                  MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 20);
                }
              } catch {
                CustomMessageBox.Show("Failed to takeoff");
              }
            });
            break;
          case buttonfunction.Disarm:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.doARM(false);
              } catch {
                CustomMessageBox.Show("Failed to Disarm");
              }
            });
            break;
          */
          case buttonfunction.Do_Set_Relay:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                int number = (int)but.p1;
                int state = buttondown == true ? 1 : 0;
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_RELAY, number, state, 0, 0, 0, 0, 0);
              } catch {
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
              try {
                int relaynumber = (int)but.p1;
                int repeat = (int)but.p2;
                int time = (int)but.p3;
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_RELAY, relaynumber, repeat, time, 0,
                    0, 0, 0);
              } catch {
                CustomMessageBox.Show("Failed to DO_REPEAT_RELAY");
              }
            });
            break;
          case buttonfunction.Do_Set_Servo:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                int channel = (int)but.p1;
                int pwm = (int)but.p2;
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, channel, pwm, 0, 0, 0, 0, 0);
              } catch {
                CustomMessageBox.Show("Failed to DO_SET_SERVO");
              }
            });
            break;
          case buttonfunction.Do_Repeat_Servo:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                int channelno = (int)but.p1;
                int pwmvalue = (int)but.p2;
                int repeattime = (int)but.p3;
                int delay_ms = (int)but.p4;
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_SERVO, channelno, pwmvalue,
                    repeattime, delay_ms, 0, 0, 0);
              } catch {
                CustomMessageBox.Show("Failed to DO_REPEAT_SERVO");
              }
            });
            break;
          /*
          case buttonfunction.Toggle_Pan_Stab:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                float current = (float)MainV2.comPort.MAV.param["MNT_STAB_PAN"];
                float newvalue = (current > 0) ? 0 : 1;
                MainV2.comPort.setParam("MNT_STAB_PAN", newvalue);
              } catch {
                CustomMessageBox.Show("Failed to Toggle_Pan_Stab");
              }
            });
            break;
          */
          case buttonfunction.Gimbal_pnt_track:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0,
                    MainV2.comPort.MAV.cs.gimballat, MainV2.comPort.MAV.cs.gimballng,
                    (float)MainV2.comPort.MAV.cs.GimbalPoint.Alt);
              } catch {
                CustomMessageBox.Show("Failed to Gimbal_pnt_track");
              }
            });
            break;
          /*
          case buttonfunction.Mount_Control_0:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                MainV2.comPort.setMountControl(0, 0, 0, false);
              } catch {
                CustomMessageBox.Show("Failed to Mount_Control_0");
              }
            });
            break;
          */
          case buttonfunction.Button_axis0:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                int pwmmin = (int)but.p1;
                int pwmmax = (int)but.p2;

                if(buttondown)
                  custom0 = pwmmax;
                else
                  custom0 = pwmmin;
              } catch {
                CustomMessageBox.Show("Failed to Button_axis0");
              }
            });
            break;
          case buttonfunction.Button_axis1:
            MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
              try {
                int pwmmin = (int)but.p1;
                int pwmmax = (int)but.p2;

                if(buttondown)
                  custom1 = pwmmax;
                else
                  custom1 = pwmmin;
              } catch {
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
            if(buttondown) this.UserEnabled = true;
            else this.UserEnabled = false;
            break;
        }
      }
    }

    public enum joystickaxis {
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

    public Device AcquireJoystick(string name) {
      DeviceList joysticklist = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);

      bool found = false;

      foreach(DeviceInstance device in joysticklist) {
        if(device.ProductName == name) {
          joystick = new Device(device.InstanceGuid);
          found = true;
          break;
        }
      }

      if(!found)
        return null;

      joystick.SetDataFormat(DeviceDataFormat.Joystick);

      try {
        joystick.Unacquire();
      } catch { }
      joystick.Acquire();

      System.Threading.Thread.Sleep(500);

      joystick.Poll();

      return joystick;
    }

    public void UnAcquireJoyStick() {
      if(joystick == null)
        return;
      joystick.Unacquire();
    }

    /// <summary>
    /// Button press check with debounce
    /// </summary>
    /// <param name="buttonno"></param>
    /// <returns></returns>
    bool getButtonState(CameraJoyButton but, int buttonno) {
      byte[] buts = state.GetButtons();

      // button down
      bool ans = buts[buttonno] > 0 && buttonpressed[buttonno] == 0; // press check + debounce
      if(ans)
        ButtonDown(but);

      // button up
      ans = buts[buttonno] == 0 && buttonpressed[buttonno] > 0;
      if(ans)
        ButtonUp(but);

      buttonpressed[buttonno] = buts[buttonno]; // set only this button

      return ans;
    }

    void ButtonDown(CameraJoyButton but) {
      ProcessButtonEvent(but, true);
    }

    void ButtonUp(CameraJoyButton but) {
      ProcessButtonEvent(but, false);
    }

    public int getNumButtons() {
      if(joystick == null)
        return 0;
      return joystick.Caps.NumberButtons;
    }

    public joystickaxis getJoystickAxis(CameraAxis axis) {
      try {
        return JoyAxes[(int)axis].axis;
      } catch {
        return joystickaxis.None;
      }
    }

    public int getJoystickChannel(CameraAxis axis) {
      try {
        return JoyAxes[(int)axis].channel;
      } catch {
        return 0;
      }
    }

    public bool isButtonPressed(int buttonno) {
      byte[] buts = state.GetButtons();

      if(buts == null || JoyButtons[buttonno].buttonno < 0)
        return false;

      return buts[JoyButtons[buttonno].buttonno] > 0;
    }

    public ushort getValueForChannel(CameraAxis axis, string name) {
      if(joystick == null)
        return 0;

      joystick.Poll();

      state = joystick.CurrentJoystickState;

      ushort ans = pickchannel(JoyAxes[(int)axis].channel, JoyAxes[(int)axis].axis, JoyAxes[(int)axis].reverse,
          JoyAxes[(int)axis].expo);
      //log.DebugFormat("{0} = {1} = {2} = {3}", JoyAxes[(int)axis].channel, ans, state.X, Enum.GetName(typeof(CameraAxis), axis));
      return ans;
    }

    public ushort getRawValueForChannel(CameraAxis axis) {
      if(joystick == null)
        return 0;

      joystick.Poll();

      state = joystick.CurrentJoystickState;

      ushort ans = pickchannel(JoyAxes[(int)axis].channel, JoyAxes[(int)axis].axis, false, 0);
      //log.DebugFormat("{0} = {1} = {2} = {3}", JoyAxes[(int)axis].channel, ans, state.X, Enum.GetName(typeof(CameraAxis), axis));
      return ans;
    }

    int getChanTrim(int chan) {
      if(chan <= 0) return 0;

      int trim = 0;
      if(MainV2.comPort.MAV.param.Count > 0) {
        try {
          if(MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_TRIM")) {
            trim = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_TRIM"]);
          } else {
            trim = 1500;
          }
        } catch {
          trim = 1500;
        }
      } else {
        trim = 1500;
      }
      if(chan == 3) {
        int min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
        int max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
        trim = (min + max) / 2;
        //                trim = min; // throttle
      }
      return trim;
    }

    int getChanMax(int chan) {
      if(chan <= 0) return 0;

      int max = 0;
      if(MainV2.comPort.MAV.param.Count > 0) {
        try {
          if(MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MAX")) {
            max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
          } else {
            max = 3000;
          }
        } catch {
          max = 3000;
        }
      } else {
        max = 3000;
      }
      return max;
    }

    int getChanMin(int chan) {
      if(chan <= 0) return 0;

      int min = 0;
      if(MainV2.comPort.MAV.param.Count > 0) {
        try {
          if(MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MIN")) {
            min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
          } else {
            min = 0;
          }
        } catch {
          min = 0;
        }
      } else {
        min = 0;
      }
      return min;
    }

    ushort pickchannel(int chan, joystickaxis axis, bool rev, int expo) {
      int raw;
      return pickchannel(chan, axis, rev, expo, out raw);
      }

    ushort pickchannel(int chan, joystickaxis axis, bool rev, int expo, out int rawJSVal) {
      rawJSVal = 0;
      if(chan <= 0) return (ushort)0;

      int min, max, trim = 0;

      if(MainV2.comPort.MAV.param.Count > 0) {
        try {
          if(MainV2.comPort.MAV.param.ContainsKey("RC" + chan + "_MIN")) {
            min = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MIN"]);
            max = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_MAX"]);
            trim = (int)(float)(MainV2.comPort.MAV.param["RC" + chan + "_TRIM"]);
          } else {
            min = 1000;
            max = 2000;
            trim = 1500;
          }
        } catch {
          min = 1000;
          max = 2000;
          trim = 1500;
        }
      } else {
        min = 1000;
        max = 2000;
        trim = 1500;
      }
      if(chan == 3) {
        trim = (min + max) / 2;
        //                trim = min; // throttle
      }

      int range = Math.Abs(max - min);

      int working = 0;

      switch(axis) {
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
          custom0 = (int)Constrain(custom0, 0, 65535);
          working = custom0;
          break;

        case joystickaxis.Custom2:
          custom1 = (int)Constrain(custom1, 0, 65535);
          working = custom1;
          break;
      }
      // between 0 and 65535 - convert to int -500 to 500
      working = (int)(working / 65.535) - 500;

      if(rev)
        working *= -1;

      // save for later
      int raw = working;
      rawJSVal = raw;

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

    public static double Expo(double input, double expo, double min, double max, double mid) {
      // input range -500 to 500

      double expomult = expo / 100.0;

      if(input >= 0) {
        // linear scale
        double linearpwm = map(input, 0, 500, mid, max);

        double expomid = (max - mid) / 2;

        double factor = 0;

        // over half way though input
        if(input > 250) {
          factor = 250 - (input - 250);
        } else {
          factor = input;
        }

        return linearpwm - (factor * expomult);
      } else {
        double linearpwm = map(input, -500, 0, min, mid);

        double expomid = (mid - min) / 2;

        double factor = 0;

        // over half way though input
        if(input < -250) {
          factor = -250 - (input + 250);
        } else {
          factor = input;
        }

        return linearpwm - (factor * expomult);
      }
    }

    static double map(double x, double in_min, double in_max, double out_min, double out_max) {
      return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    double Constrain(double value, double min, double max) {
      if(value > max)
        return max;
      if(value < min)
        return min;
      return value;
    }
  }
}
