using Microsoft.DirectX.DirectInput;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Joystick {
  public partial class ConfigureCameraJoystick : Form {

    bool startup = true;

    int noButtons = 0;

    public ConfigureCameraJoystick() {
      InitializeComponent();

      //MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
    }

    private void expo_ch2_TextChanged(object sender, EventArgs e) {

    }

    private void ConfigureCameraJoystick_Load(object sender, EventArgs e) {

      try {
        DeviceList joysticklist = CameraJoystick.getDevices();

        foreach(DeviceInstance device in joysticklist) {
          CMB_joysticks.Items.Add(device.ProductName);
        }
      } catch (Exception ex) {
        CustomMessageBox.Show("Error geting joystick list: do you have the directx redist installed?\n\n\n" + ex.Message);
        this.Close();
        return;
      }

      if(CMB_joysticks.Items.Count > 0 && CMB_joysticks.SelectedIndex == -1)
        CMB_joysticks.SelectedIndex = 0;

      try {
        if(Settings.Instance.ContainsKey("camerajoystick_name") && Settings.Instance["camerajoystick_name"].ToString() != "")
          CMB_joysticks.Text = Settings.Instance["camerajoystick_name"].ToString();
      } catch {
      }


      PAN_AXIS.DataSource = (Enum.GetValues(typeof(CameraJoystick.joystickaxis)));
      TILT_AXIS.DataSource = (Enum.GetValues(typeof(CameraJoystick.joystickaxis)));
      ZOOM_AXIS.DataSource = (Enum.GetValues(typeof(CameraJoystick.joystickaxis)));

      PAN_CH.DataSource = (Enum.GetValues(typeof(Channels)));
      TILT_CH.DataSource = (Enum.GetValues(typeof(Channels)));
      ZOOM_CH.DataSource = (Enum.GetValues(typeof(Channels)));



      var tempjoystick = new CameraJoystick();

      ConfigStatusLabel.Text += " " + MainV2.comPort.MAV.cs.firmware.ToString();

      foreach(CameraJoystick.CameraAxis ca in Enum.GetValues(typeof(CameraJoystick.CameraAxis))) {
        var config = tempjoystick.getChannel(ca);
        string caname = Enum.GetName(typeof(CameraJoystick.CameraAxis), ca).ToUpper();

        findandsetcontrol(caname + "_AXIS", config.axis.ToString());
        findandsetcontrol(caname + "_CH", Enum.GetName(typeof(Channels), (Channels)config.channel));
        findandsetcontrol(caname + "_EXPO", config.expo.ToString());
        findandsetcontrol(caname + "_REV", config.reverse.ToString());
        findandsetcontrol(caname + "_OVERRIDETHRESHOLD", config.overridecenter.ToString());
      }

      if(MainV2.camerajoystick != null && MainV2.camerajoystick.enabled) {
        timer1.Start();
        BUT_enable.Text = "Disable";
      }

      startup = false;
    }

    int[] getButtonNumbers() {
      int[] temp = new int[128];
      temp[0] = -1;
      for(int a = 0; a < temp.Length - 1; a++) {
        temp[a + 1] = a;
      }
      return temp;
    }

    void findandsetcontrol(string ctlname, string value) {
      var ctls = this.Controls.Find(ctlname, false);
      if(ctls == null || ctls.Count() == 0) return;

      var ctl = ctls[0];
      if(ctl == null) return;

      if(ctl is CheckBox) {
        ((CheckBox)ctl).Checked = (value.ToLower() == "false") ? false : true;
      } else if(ctl is NumericUpDown) {
        decimal dval;
        if(Decimal.TryParse(value, out dval)) {
          dval = Math.Max(((NumericUpDown)ctl).Minimum, dval);
          dval = Math.Min(dval, ((NumericUpDown)ctl).Maximum);
          ((NumericUpDown)ctl).Value = dval;
        } else {
          ((NumericUpDown)ctl).Value = ((NumericUpDown)ctl).Minimum;
        }
      } else {
        ((Control)ctl).Text = value;
      }
    }

    private void BUT_enable_Click(object sender, EventArgs e) {
      if(MainV2.camerajoystick == null || MainV2.camerajoystick.enabled == false) {
        try {
          if(MainV2.camerajoystick != null)
            MainV2.camerajoystick.UnAcquireJoyStick();
        } catch {
        }

        // all config is loaded from the xmls
        CameraJoystick joy = new CameraJoystick();

        //show error message if a joystick is not connected when Enable is clicked
        if(!joy.start(CMB_joysticks.Text)) {
          CustomMessageBox.Show("Please Connect a Joystick", "No Joystick");
          return;
        }

        Settings.Instance["camerajoystick_name"] = CMB_joysticks.Text;

        MainV2.camerajoystick = joy;
        MainV2.camerajoystick.enabled = true;

        BUT_enable.Text = "Disable";

      } else {
        MainV2.camerajoystick.enabled = false;
        MainV2.camerajoystick.clearRCOverride();
        MainV2.camerajoystick = null;
        BUT_enable.Text = "Enable";
      }
    }

    private void BUT_save_Click(object sender, EventArgs e) {
      CameraJoystick.self.saveconfig();
    }

    private void timer1_Tick(object sender, EventArgs e) {
      try {
        if(MainV2.camerajoystick == null || MainV2.camerajoystick.enabled == false) {
          //Console.WriteLine(DateTime.Now.Millisecond + " start ");
          CameraJoystick joy = MainV2.camerajoystick;
          if(joy == null) {
            joy = new CameraJoystick();
            if(PAN_AXIS.Text != "") {
              joy.setChannel(
                CameraJoystick.CameraAxis.Pan,
                (int)(Channels)Enum.Parse(typeof(Channels), PAN_CH.Text, true),
                (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), PAN_AXIS.Text),
                PAN_REV.Checked,
                Convert.ToInt32(PAN_EXPO.Value),
                Convert.ToInt32(PAN_OVERRIDETHRESHOLD.Value)
              );
            }

            if(TILT_AXIS.Text != "") {
              joy.setChannel(
                CameraJoystick.CameraAxis.Tilt,
                (int)(Channels)Enum.Parse(typeof(Channels), TILT_CH.Text, true),
                (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), TILT_AXIS.Text),
                TILT_REV.Checked,
                Convert.ToInt32(TILT_EXPO.Value),
                Convert.ToInt32(TILT_OVERRIDETHRESHOLD.Value)
              );
            }

            if(ZOOM_AXIS.Text != "") {
              joy.setChannel(
                CameraJoystick.CameraAxis.Zoom,
                (int)(Channels)Enum.Parse(typeof(Channels), ZOOM_CH.Text, true),
                (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), ZOOM_AXIS.Text),
                ZOOM_REV.Checked,
                Convert.ToInt32(ZOOM_EXPO.Value),
                Convert.ToInt32(ZOOM_OVERRIDETHRESHOLD.Value)
              );
            }

            joy.AcquireJoystick(CMB_joysticks.Text);

            joy.name = CMB_joysticks.Text;

            
            noButtons = joy.getNumButtons();

            noButtons = Math.Min(15, noButtons);

            for(int f = 0; f < noButtons; f++) {
              string name = (f).ToString();

              doButtontoUI(name, 10, ZOOM_AXIS.Bottom + 20 + f * 25);

              var config = joy.getButton(f);

              joy.setButton(f, config);
            }

            MainV2.camerajoystick = joy;

            ThemeManager.ApplyThemeTo(this);

            CMB_joysticks.SelectedIndex = CMB_joysticks.Items.IndexOf(joy.name);
          }

          var pch = joy.getChannel(CameraJoystick.CameraAxis.Pan);
          if(pch.channel > 0) {
            CameraJoystick.setOverrideCh(pch.channel, joy.getValueForChannel(CameraJoystick.CameraAxis.Pan, CMB_joysticks.Text));
          }
          var tch = joy.getChannel(CameraJoystick.CameraAxis.Tilt);
          if(tch.channel > 0) {
            CameraJoystick.setOverrideCh(tch.channel, joy.getValueForChannel(CameraJoystick.CameraAxis.Tilt, CMB_joysticks.Text));
          }
          var zch = joy.getChannel(CameraJoystick.CameraAxis.Zoom);
          if(zch.channel > 0) {
            CameraJoystick.setOverrideCh(zch.channel, joy.getValueForChannel(CameraJoystick.CameraAxis.Zoom, CMB_joysticks.Text));
          }
        }
      } catch(InputLostException ex) {
        ex.ToString();
        if(MainV2.camerajoystick != null && MainV2.camerajoystick.enabled == true) {
          BUT_enable_Click(null, null);
        }
      } catch {
      }
      try {
        PAN_OUTPUT.Value = CameraJoystick.getOverrideCh((int)(Channels)Enum.Parse(typeof(Channels), PAN_CH.Text, true));
        TILT_OUTPUT.Value = CameraJoystick.getOverrideCh((int)(Channels)Enum.Parse(typeof(Channels), TILT_CH.Text, true));
        ZOOM_OUTPUT.Value = CameraJoystick.getOverrideCh((int)(Channels)Enum.Parse(typeof(Channels), ZOOM_CH.Text, true));
      } catch { }

      try {
        PAN_OUTPUT.maxline = MainV2.camerajoystick.getRawValueForChannel(CameraJoystick.CameraAxis.Pan);
        TILT_OUTPUT.maxline = MainV2.camerajoystick.getRawValueForChannel(CameraJoystick.CameraAxis.Tilt);
        ZOOM_OUTPUT.maxline = MainV2.camerajoystick.getRawValueForChannel(CameraJoystick.CameraAxis.Zoom);
      } catch { }

      try {
        for(int f = 0; f < noButtons; f++) {
          string name = (f).ToString();
          ((HorizontalProgressBar)this.Controls.Find("hbar" + name, false)[0]).Value =
              MainV2.camerajoystick.isButtonPressed(f) ? 100 : 0;
        }
      } catch {
      } // this is for buttons - silent fail
    }

    private void CMB_joysticks_Click(object sender, EventArgs e) {
      CMB_joysticks.Items.Clear();

      DeviceList joysticklist = CameraJoystick.getDevices();

      foreach(DeviceInstance device in joysticklist) {
        CMB_joysticks.Items.Add(device.ProductName);
      }

      if(CMB_joysticks.Items.Count > 0 && CMB_joysticks.SelectedIndex == -1)
        CMB_joysticks.SelectedIndex = 0;
    }

    private void PAN_REV_CheckedChanged(object sender, EventArgs e) {
       if (MainV2.camerajoystick != null)
                MainV2.camerajoystick.setReverse(CameraJoystick.CameraAxis.Pan, ((CheckBox) sender).Checked);
    }

    private void TILT_REV_CheckedChanged(object sender, EventArgs e) {
       if (MainV2.camerajoystick != null)
                MainV2.camerajoystick.setReverse(CameraJoystick.CameraAxis.Tilt, ((CheckBox) sender).Checked);
    }

    private void ZOOM_REV_CheckedChanged(object sender, EventArgs e) {
       if (MainV2.camerajoystick != null)
                MainV2.camerajoystick.setReverse(CameraJoystick.CameraAxis.Zoom, ((CheckBox) sender).Checked);
    }

    private void PAN_AUTODET_Click(object sender, EventArgs e) {
      PAN_AXIS.Text = CameraJoystick.getMovingAxis(CMB_joysticks.Text, 16000).ToString();
    }

    private void TILT_AUTODET_Click(object sender, EventArgs e) {
      TILT_AXIS.Text = CameraJoystick.getMovingAxis(CMB_joysticks.Text, 16000).ToString();
    }

    private void ZOOM_AUTODET_Click(object sender, EventArgs e) {
      ZOOM_AXIS.Text = CameraJoystick.getMovingAxis(CMB_joysticks.Text, 16000).ToString();
    }

    private void PAN_AXIS_SelectedIndexChanged(object sender, EventArgs e) {
      if(startup || MainV2.camerajoystick == null)
        return;

      Channels Pan_ch = Channels.DISABLE;
      if(Enum.TryParse<Channels>(PAN_CH.Text, out Pan_ch)) {
        MainV2.camerajoystick.setChannel(
          CameraJoystick.CameraAxis.Pan,
          (int)Pan_ch,
          (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), PAN_AXIS.Text),
          PAN_REV.Checked,
          Convert.ToInt32(PAN_EXPO.Value),
          Convert.ToInt32(PAN_OVERRIDETHRESHOLD.Value)
        );
      }
    }

    private void TILT_AXIS_SelectedIndexChanged(object sender, EventArgs e) {
      if(startup || MainV2.camerajoystick == null)
        return;

      Channels Tilt_ch = Channels.DISABLE;
      if(Enum.TryParse<Channels>(TILT_CH.Text, out Tilt_ch)) {
        MainV2.camerajoystick.setChannel(
          CameraJoystick.CameraAxis.Tilt,
          (int)Tilt_ch,
          (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), TILT_AXIS.Text),
          TILT_REV.Checked,
          Convert.ToInt32(TILT_EXPO.Value),
          Convert.ToInt32(TILT_OVERRIDETHRESHOLD.Value)
        );
      }
    }

    private void ZOOM_AXIS_SelectedIndexChanged(object sender, EventArgs e) {
      if(startup || MainV2.camerajoystick == null)
        return;

      Channels Zoom_ch = Channels.DISABLE;
      if(Enum.TryParse<Channels>(ZOOM_CH.Text, out Zoom_ch)) {
        MainV2.camerajoystick.setChannel(
          CameraJoystick.CameraAxis.Zoom,
          (int)Zoom_ch,
          (CameraJoystick.joystickaxis)Enum.Parse(typeof(CameraJoystick.joystickaxis), ZOOM_AXIS.Text),
          ZOOM_REV.Checked,
          Convert.ToInt32(ZOOM_EXPO.Value),
          Convert.ToInt32(ZOOM_OVERRIDETHRESHOLD.Value)
        );
      }
    }

    private void cmbbutton_SelectedIndexChanged(object sender, EventArgs e) {
      if(startup || MainV2.camerajoystick == null)
        return;

      string name = ((ComboBox)sender).Name.Replace("cmbbutton", "");

      MainV2.camerajoystick.changeButton((int.Parse(name)), int.Parse(((ComboBox)sender).Text));
    }

    private void BUT_detbutton_Click(object sender, EventArgs e) {
      string name = ((MyButton)sender).Name.Replace("mybut", "");

      ComboBox cmb = (ComboBox)(this.Controls.Find("cmbbutton" + name, false)[0]);
      cmb.Text = CameraJoystick.getPressedButton(CMB_joysticks.Text).ToString();
    }

    void doButtontoUI(string name, int x, int y) {
      MyLabel butlabel = new MyLabel();
      ComboBox butnumberlist = new ComboBox();
      MissionPlanner.Controls.MyButton but_detect = new MissionPlanner.Controls.MyButton();
      HorizontalProgressBar hbar = new HorizontalProgressBar();
      ComboBox cmbaction = new ComboBox();
      MissionPlanner.Controls.MyButton but_settings = new MissionPlanner.Controls.MyButton();

      var config = CameraJoystick.self.getButton(int.Parse(name));

      // do this here so putting in text works
      this.Controls.AddRange(new Control[] { butlabel, butnumberlist, but_detect, hbar, cmbaction, but_settings });

      butlabel.Location = new Point(x, y);
      butlabel.Size = new Size(47, 13);
      butlabel.Text = "Button " + (int.Parse(name) + 1);

      butnumberlist.Location = new Point(72, y);
      butnumberlist.Size = new Size(70, 21);
      butnumberlist.DataSource = getButtonNumbers();
      butnumberlist.DropDownStyle = ComboBoxStyle.DropDownList;
      butnumberlist.Name = "cmbbutton" + name;
      //if (Settings.Instance["butno" + name] != null)
      //  butnumberlist.Text = (Settings.Instance["butno" + name].ToString());
      //if (config.buttonno != -1)
      butnumberlist.Text = config.buttonno.ToString();
      butnumberlist.SelectedIndexChanged += new EventHandler(cmbbutton_SelectedIndexChanged);

      but_detect.Location = new Point(PAN_AUTODET.Left, y);
      but_detect.Size = PAN_AUTODET.Size;
      but_detect.Text = PAN_AUTODET.Text;
      but_detect.Name = "mybut" + name;
      but_detect.Click += new EventHandler(BUT_detbutton_Click);

      hbar.Location = new Point(PAN_OUTPUT.Left, y);
      hbar.Size = PAN_OUTPUT.Size;
      hbar.Name = "hbar" + name;

      cmbaction.Location = new Point(hbar.Right + 5, y);
      cmbaction.Size = new Size(100, 21);

      cmbaction.DataSource = Enum.GetNames(typeof(CameraJoystick.buttonfunction));
      //Common.getModesList(MainV2.comPort.MAV.cs);
      //cmbaction.ValueMember = "Key";
      //cmbaction.DisplayMember = "Value";
      cmbaction.Tag = name;
      cmbaction.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbaction.Name = "cmbaction" + name;
      //if (Settings.Instance["butaction" + name] != null)
      //  cmbaction.Text = Settings.Instance["butaction" + name].ToString();
      //if (config.function != Joystick.buttonfunction.ChangeMode)
      cmbaction.Text = config.function.ToString();
      cmbaction.SelectedIndexChanged += cmbaction_SelectedIndexChanged;

      but_settings.Location = new Point(cmbaction.Right + 5, y);
      but_settings.Size = PAN_AUTODET.Size;
      but_settings.Text = "Settings";
      but_settings.Name = "butsettings" + name;
      but_settings.Click += but_settings_Click;
      but_settings.Tag = cmbaction;

      if((but_settings.Bottom + 30) > this.Height)
        this.Height += 25;
    }

    void cmbaction_SelectedIndexChanged(object sender, EventArgs e) {
      int num = int.Parse(((Control)sender).Tag.ToString());
      var config = CameraJoystick.self.getButton(num);
      config.function =
          (CameraJoystick.buttonfunction)Enum.Parse(typeof(CameraJoystick.buttonfunction), ((Control)sender).Text);
      CameraJoystick.self.setButton(num, config);
    }

    void but_settings_Click(object sender, EventArgs e) {
      var cmb = ((Control)sender).Tag as ComboBox;

      switch((CameraJoystick.buttonfunction)Enum.Parse(typeof(CameraJoystick.buttonfunction), cmb.SelectedItem.ToString())) {
        case CameraJoystick.buttonfunction.Do_Repeat_Relay:
          new Joy_Do_Repeat_Relay((string)cmb.Tag).ShowDialog();
          break;
        case CameraJoystick.buttonfunction.Do_Repeat_Servo:
          new Joy_Do_Repeat_Servo((string)cmb.Tag).ShowDialog();
          break;
        case CameraJoystick.buttonfunction.Do_Set_Relay:
          new Joy_Do_Set_Relay((string)cmb.Tag).ShowDialog();
          break;
        case CameraJoystick.buttonfunction.Do_Set_Servo:
          new Joy_Do_Set_Servo((string)cmb.Tag).ShowDialog();
          break;
        case CameraJoystick.buttonfunction.Button_axis0:
          new Joy_Button_axis((string)cmb.Tag).ShowDialog();
          break;
        case CameraJoystick.buttonfunction.Button_axis1:
          new Joy_Button_axis((string)cmb.Tag).ShowDialog();
          break;
        default:
          CustomMessageBox.Show("No settings to set", "No settings");
          break;
      }
    }

    private void CMB_joysticks_SelectedIndexChanged(object sender, EventArgs e) {
      try {
        if(MainV2.camerajoystick != null && MainV2.camerajoystick.enabled == false)
          MainV2.camerajoystick.UnAcquireJoyStick();
      } catch {
      }
    }

    private void ConfigureCameraJoystick_FormClosed(object sender, FormClosedEventArgs e) {
      timer1.Stop();

      if(MainV2.camerajoystick != null && MainV2.camerajoystick.enabled == false) {
        MainV2.camerajoystick.UnAcquireJoyStick();
        MainV2.camerajoystick = null;
      }
    }
  }

  public enum Channels {
    DISABLE=0,
    Roll=1,
    Pitch=2,
    Throttle=3,
    Rudder=4,
    CH5=5,
    CH6=6,
    CH7=7,
    CH8=8,
  }
}
