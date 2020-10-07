using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.MsgBox;
using MissionPlanner.Radio;
using MissionPlanner.Utilities;
using Microsoft.VisualBasic;

namespace MissionPlanner.Radio
{
    public partial class Sikradio : UserControl, SikRadio.ISikRadioForm
    {
        public delegate void LogEventHandler(string message, int level = 0);

        public delegate void ProgressEventHandler(double completed);

        /// <summary>
        /// An alternative function for getting the serial port.  Can be left as null
        /// if no alternative.
        /// </summary>
        public Func<ICommsSerial> GetCommsSerialAlt;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool beta;

        private readonly string firmwarefile = Path.GetTempFileName();
        private Dictionary<Control, bool> _DefaultLocalEnabled = new Dictionary<Control, bool>();
        private Dictionary<ComboBox, object> _DefaultCBObjects = new Dictionary<ComboBox, object>();
        RFD.RFD900.TSession _Session;
        ExtraParamControlsSet _LocalExtraParams;
        ExtraParamControlsSet _RemoteExtraParams;

        /*
ATI5
S0: FORMAT=25
S1: SERIAL_SPEED=57
S2: AIR_SPEED=64
S3: NETID=40
S4: TXPOWER=30
S5: ECC=1
S6: MAVLINK=1
S7: OPPRESEND=1
S8: MIN_FREQ=915000
S9: MAX_FREQ=928000
S10: NUM_CHANNELS=50
S11: DUTY_CYCLE=100
S12: LBT_RSSI=0
S13: MANCHESTER=0
S14: RTSCTS=0
S15: MAX_WINDOW=131
         */

        public Sikradio()
        {
            InitializeComponent();

            // hide advanced view
            //SPLIT_local.Panel2Collapsed = true;
            //SPLIT_remote.Panel2Collapsed = true;

            _LocalExtraParams = new ExtraParamControlsSet(lblNODEID, NODEID,
                lblDESTID, DESTID, lblTX_ENCAP_METHOD, TX_ENCAP_METHOD, lblRX_ENCAP_METHOD, RX_ENCAP_METHOD,
                lblMAX_DATA, MAX_DATA,
                new Control[] {lblMAX_RETRIES, MAX_RETRIES,
                lblGLOBAL_RETRIES, GLOBAL_RETRIES, lblSER_BRK_DETMS, SER_BRK_DETMS,
                lblANT_MODE, ANT_MODE}, false);

            _RemoteExtraParams = new ExtraParamControlsSet(lblRNODEID, RNODEID,
                lblRDESTID, RDESTID, lblRTX_ENCAP_METHOD, RTX_ENCAP_METHOD, lblRRX_ENCAP_METHOD, RRX_ENCAP_METHOD,
                lblRMAX_DATA, RMAX_DATA,
                new Control[] {lblRMAX_RETRIES, RMAX_RETRIES,
                lblRGLOBAL_RETRIES, RGLOBAL_RETRIES, lblRSER_BRK_DETMS, RSER_BRK_DETMS,
                lblRANT_MODE, RANT_MODE}, true);

            // setup netid
            NETID.DataSource = Enumerable.Range(0, 500).ToArray();
            RNETID.DataSource = Enumerable.Range(0, 500).ToArray();

            MAVLINK.DisplayMember = "Value";
            MAVLINK.ValueMember = "Key";
            SetupComboForMavlink(MAVLINK, false);
            RMAVLINK.DisplayMember = "Value";
            RMAVLINK.ValueMember = "Key";
            SetupComboForMavlink(RMAVLINK, false);

            MAX_WINDOW.DataSource = Enumerable.Range(33, 131 - 32).ToArray();
            RMAX_WINDOW.DataSource = Enumerable.Range(33, 131 - 32).ToArray();

            foreach (Control C in groupBoxLocal.Controls)
            {
                _DefaultLocalEnabled[C] = C.Enabled;
            }
            foreach (Control C in groupBoxRemote.Controls)
            {
                _DefaultLocalEnabled[C] = C.Enabled;
            }

            SaveDefaultCBObjects(SERIAL_SPEED);
            SaveDefaultCBObjects(RSERIAL_SPEED);

            SaveDefaultCBObjects(AIR_SPEED);
            SaveDefaultCBObjects(RAIR_SPEED);

            SaveDefaultCBObjects(NETID);
            SaveDefaultCBObjects(RNETID);

            SaveDefaultCBObjects(NUM_CHANNELS);
            SaveDefaultCBObjects(RNUM_CHANNELS);

            SaveDefaultCBObjects(MAX_WINDOW);
            SaveDefaultCBObjects(RMAX_WINDOW);

            this.Disposed += DisposedEvtHdlr;
        }

        public void Connect()
        {
            var S = GetSession();
        }

        public void Disconnect()
        {
            var S = _Session;
            if (S != null)
            {
                S.PutIntoTransparentMode();
            }
            EndSession();
        }

        void DisposedEvtHdlr(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void SaveDefaultCBObjects(ComboBox CB)
        {
            if (CB.DataSource == null)
            {
                List<object> LO = new List<object>();
                foreach (var O in CB.Items)
                {
                    LO.Add(O);
                }
                _DefaultCBObjects[CB] = LO;
            }
            else
            {
                _DefaultCBObjects[CB] = CB.DataSource;
            }
        }

        private void RestoreAllDefaultCBObjects()
        {
            foreach (var kvp in _DefaultCBObjects)
            {
                if (kvp.Value is List<object>)
                {
                    kvp.Key.DataSource = null;
                    kvp.Key.Items.Clear();
                    List<object> LO = (List<object>)kvp.Value;
                    foreach (var O in LO)
                    {
                        kvp.Key.Items.Add(O);
                    }
                }
                else
                {
                    kvp.Key.DataSource = kvp.Value;
                }
            }
        }

        private bool getFirmware(Uploader.Board device, bool custom = false)
        {
            if (custom)
            {
                return getFirmwareLocal(device == Uploader.Board.DEVICE_ID_RFD900X);
            }

            if (device == Uploader.Board.DEVICE_ID_HM_TRP)
            {
                if (beta)
                {
                    return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/beta/radio~hm_trp.ihx", firmwarefile);
                }
                return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/stable/radio~hm_trp.ihx",
                    firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900)
            {
                if (beta)
                {
                    return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/beta/radio~rfd900.ihx", firmwarefile);
                }
                return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/stable/radio~rfd900.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900A)
            {
                if (beta)
                {
                    return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/beta/radio~rfd900a.ihx",
                        firmwarefile);
                }
                return Download.getFilefromNet("https://firmware.ardupilot.org/SiK/stable/radio~rfd900a.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900U)
            {
                if (beta)
                {
                    return Download.getFilefromNet("https://files.rfdesign.com.au/Files/firmware/MPSiK%20V2.6%20rfd900u.ihx", firmwarefile);
                }
                return Download.getFilefromNet("https://files.rfdesign.com.au/Files/firmware/RFDSiK%20V1.9%20rfd900u.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900P)
            {
                if (beta)
                {
                    return Download.getFilefromNet("https://files.rfdesign.com.au/Files/firmware/MPSiK%20V2.6%20rfd900p.ihx", firmwarefile);
                }
                return Download.getFilefromNet("https://files.rfdesign.com.au/Files/firmware/RFDSiK%20V1.9%20rfd900p.ihx", firmwarefile);
            }
            if (device == Uploader.Board.DEVICE_ID_RFD900X)
            {
                return Download.getFilefromNet("http://files.rfdesign.com.au/Files/firmware/RFDSiK%20V2.60%20rfd900x.bin", firmwarefile);
            }
            return false;
        }

        /// <summary>
        /// Loads a local firmware file after prompting user for file.
        /// </summary>
        /// <returns></returns>
        private bool getFirmwareLocal(bool Bin)
        {
            using (var openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = Bin ? "Firmware|*.bin" : "Firmware|*.hex;*.ihx";
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(openFileDialog1.FileName, firmwarefile, true);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.CustomMessageBox.Show("Error copying file\n" + ex, "ERROR");
                        return false;
                    }
                    return true;
                }

                return false;
            }
        }

        private void Sleep(int mstimeout, ICommsSerial comPort = null)
        {
            var endtime = DateTime.Now.AddMilliseconds(mstimeout);

            while (DateTime.Now < endtime)
            {
                Thread.Sleep(1);
                Application.DoEvents();

                // prime the mavlinkserial loop with data.
                if (comPort != null)
                {
                    var test = comPort.BytesToRead;
                    test++;
                }
            }
        }

        /// <summary>
        /// Tries to upload firmware to the modem using xmodem mode (after prompting user for file)
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        bool upload_xmodem(ICommsSerial comPort)
        {
            // try xmodem mode
            // xmodem - short cts to ground
            try
            {
                uploader_LogEvent("Trying XModem Mode");
                //comPort.BaudRate = 57600;
                comPort.BaudRate = MainV2.comPort.BaseStream.BaudRate;
                comPort.ReadTimeout = 1000;

                Thread.Sleep(2000);
                var tempd = comPort.ReadExisting();
                Console.WriteLine(tempd);
                comPort.Write("U");
                Thread.Sleep(1000);
                var resp1 = Serial_ReadLine(comPort); // echo
                var resp2 = Serial_ReadLine(comPort); // echo 2
                var tempd2 = comPort.ReadExisting(); // posibly bootloader info / use to sync
                // identify
                comPort.Write("i");
                // responce is rfd900....
                var resp3 = Serial_ReadLine(comPort); //echo
                var resp4 = Serial_ReadLine(comPort); // newline
                var resp5 = Serial_ReadLine(comPort); // bootloader info
                uploader_LogEvent(resp5);
                if (resp5.Contains("RFD900"))
                {
                    // start upload
                    comPort.Write("u");
                    var resp6 = Serial_ReadLine(comPort); // echo
                    var resp7 = Serial_ReadLine(comPort); // Ready
                    if (resp7.Contains("Ready"))
                    {
                        comPort.ReadTimeout = 3500;
                        // responce is C
                        var isC = comPort.ReadByte();
                        var temp = comPort.ReadExisting();
                        if (isC == 'C')
                        {
                            if (getFirmwareLocal(false))
                            {

                                XModem.LogEvent += uploader_LogEvent;
                                XModem.ProgressEvent += uploader_ProgressEvent;
                                // start file send
                                XModem.Upload(firmwarefile,
                                    comPort);
                                XModem.LogEvent -= uploader_LogEvent;
                                XModem.ProgressEvent -= uploader_ProgressEvent;
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                log.Error(ex2);
            }

            return false;
        }

        private void BUT_upload_Click(object sender, EventArgs e)
        {
            ProgramFirmware(false);
        }

        private void iHex_ProgressEvent(double completed)
        {
            try
            {
                Progressbar.Value = (int)(completed * 100);
                Application.DoEvents();
            }
            catch
            {
            }
        }

        private void uploader_LogEvent(string message, int level = 0)
        {
            try
            {
                if (level == 0)
                {
                    Console.Write(message);
                    lbl_status.Text = message;
                    log.Info(message);
                    Application.DoEvents();
                }
                else if (level < 5) // 5 = byte data
                {
                    log.Debug(message);
                }
            }
            catch
            {
            }
        }

        private void iHex_LogEvent(string message, int level = 0)
        {
            try
            {
                if (level == 0)
                {
                    lbl_status.Text = message;
                    Console.WriteLine(message);
                    log.Info(message);
                    Application.DoEvents();
                }
            }
            catch
            {
            }
        }

        private void uploader_ProgressEvent(double completed)
        {
            try
            {
                Progressbar.Value = (int)Math.Min(completed * 100, 100);
                Application.DoEvents();
            }
            catch
            {
            }
        }

        string GetParamNumber(string Part1)
        {
            Part1 = Part1.Trim();
            int S = Part1.IndexOf('S');
            return Part1.Substring(S + 1);
        }

        private void BUT_savesettings_Click(object sender, EventArgs e)
        {
            //EndSession();
            var Session = GetSession();
            
            if (Session == null)
            {
                return;
            }

            EnableConfigControls(false);
            EnableProgrammingControls(false);
            lbl_status.Text = "Connecting";

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                doCommand(Session.Port, "AT&T", false, 1);

                Session.Port.DiscardInBuffer();

                lbl_status.Text = "Doing Command";


                if (RTI.Text != "")
                {
                    // remote
                    var answer = doCommand(Session.Port, "RTI5", true);

                    var items = answer.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in items)
                    {
                        //if (item.StartsWith("S"))
                        {
                            int multipoint_fix = -1;
                            if (item.StartsWith("["))
                            {
                                multipoint_fix = item.IndexOf(']') + 1;
                            }
                            var mod_item = item;
                            if (multipoint_fix > 0 && item.Length > multipoint_fix)
                            {
                                mod_item = item.Substring(multipoint_fix).Trim();
                            }
                            var values = mod_item.Split(':', '=');

                            if (values.Length == 3)
                            {
                                values[1] = values[1].Replace("/", "_");

                                var controls = groupBoxRemote.Controls.Find("R" + values[1].Trim(), true);

                                if (controls.Length > 0)
                                {
                                    if (controls[0].GetType() == typeof(CheckBox))
                                    {
                                        var value = ((CheckBox)controls[0]).Checked ? "1" : "0";

                                        if (value != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "RTS" + values[0].Trim().TrimStart('S') + "=" + value);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                if (values[1] == "ENCRYPTION_LEVEL")
                                                {
                                                    // set this on the local radio as well.
                                                    doCommand(Session.Port, "ATS" + values[0].Trim().TrimStart('S') + "=" + value);
                                                    // both radios should now be using the default key
                                                }
                                                else
                                                {
                                                    MsgBox.CustomMessageBox.Show("Set Command error");
                                                }
                                            }
                                        }
                                    }
                                    else if (!controls[0].Name.Contains("FORMAT") && controls[0] is TextBox)
                                    {
                                        if (controls[0].Text != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "RTS" + values[0].Trim().TrimStart('S') + "=" + controls[0].Text);
                                            
                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                    else if (controls[0].Name.Contains("MAVLINK")) //
                                    {
                                        if (((ComboBox)controls[0]).SelectedValue.ToString() != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "RTS" + values[0].Trim().TrimStart('S') + "=" + ((ComboBox) controls[0]).SelectedValue);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                    else if (controls[0] is ComboBox)
                                    {
                                        string CBValue = GetCBValue((ComboBox)controls[0]);
                                        if (CBValue != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "RTS" + values[0].Trim().TrimStart('S') + "=" + CBValue);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Sleep(100);
                }

                Session.Port.DiscardInBuffer();
                {
                    //local
                    string answer = "";
                    for (int n = 0; n < 5; n++)
                    {
                        answer = doCommand(Session.Port, "ATI5", true);
                        if (answer.Length != 0)
                        {
                            break;
                        }
                    }

                    var items = answer.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in items)
                    {
                        //if (item.StartsWith("S"))
                        {
                            var values = item.Split(':', '=');

                            if (values.Length == 3)
                            {
                                values[1] = values[1].Replace("/", "_");

                                var controls = groupBoxLocal.Controls.Find(values[1].Trim(), true);

                                if (controls.Length > 0)
                                {
                                    if (controls[0].GetType() == typeof(CheckBox))
                                    {
                                        var value = ((CheckBox)controls[0]).Checked ? "1" : "0";

                                        if (value != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "ATS" + GetParamNumber(values[0]) + "=" + value);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                    else if (controls[0] is TextBox)
                                    {
                                    }
                                    else if (controls[0].Name.Contains("MAVLINK")) //
                                    {
                                        if (((ComboBox)controls[0]).SelectedValue.ToString() != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "ATS" + GetParamNumber(values[0]) + "=" + ((ComboBox) controls[0]).SelectedValue);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                    else if (controls[0] is ComboBox)
                                    {
                                        string CBValue = GetCBValue((ComboBox)controls[0]);
                                        if (CBValue != values[2].Trim())
                                        {
                                            var cmdanswer = doCommand(Session.Port,
                                                "ATS" + GetParamNumber(values[0]) + "=" + CBValue);

                                            if (cmdanswer.Contains("OK"))
                                            {
                                            }
                                            else
                                            {
                                                MsgBox.CustomMessageBox.Show("Set Command error");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // set encryption keys at the same time, so if we are enabled we dont lose comms.
                    // we have set encryption to on for both radios, they will be using the default key atm
                    if (RENCRYPTION_LEVEL.Checked)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(txt_Raeskey.Text, @"\A\b[0-9a-fA-F]+\b\Z")
                            && (txt_Raeskey.Text.Length <= 32))
                        {
                            doCommand(Session.Port, "RT&E=" + txt_Raeskey.Text.PadRight(32, '0'), true);
                        }
                        else
                        {
                            //Complain that encryption key invalid.
                            lbl_status.Text = "Fail";
                            MsgBox.CustomMessageBox.Show("Encryption key not valid hex number <= 32 hex numerals");
                        }
                    }
                    if (ENCRYPTION_LEVEL.Checked)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(txt_aeskey.Text, @"\A\b[0-9a-fA-F]+\b\Z")
                            && (txt_aeskey.Text.Length <= 32))
                        {
                            doCommand(Session.Port, "AT&E=" + txt_aeskey.Text.PadRight(32, '0'), true);
                        }
                        else
                        {
                            //Complain that encryption key invalid.
                            lbl_status.Text = "Fail";
                            MsgBox.CustomMessageBox.Show("Encryption key not valid hex number <= 32 hex numerals");
                        }
                    }


                    if (RTI.Text != "")
                    {
                        // write it
                        doCommand(Session.Port, "RT&W");

                        // return to normal mode
                        doCommand(Session.Port, "RTZ");
                    }

                    // write it
                    var cmdwriteanswer = doCommand(Session.Port, "AT&W");
                    if (!cmdwriteanswer.Contains("OK"))
                    {
                        MsgBox.CustomMessageBox.Show("Failed to save parameters");
                    }

                    // return to normal mode
                    doCommand(Session.Port, "ATZ");
                }

                lbl_status.Text = "Done";
            }
            else
            {
                // return to normal mode
                doCommand(Session.Port, "ATZ");

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode");
            }

            //Need to do this because modem rebooted.
            Session.PutIntoATCommandModeAssumingInTransparentMode();

            EnableConfigControls(true);
            EnableProgrammingControls(true);

            if (GetCommsSerialAlt == null)
            {
                EndSessionClosePort();
            }
        }

        /// <summary>
        /// Return an array of ints in a linear progression, but end is always included as the end.
        /// </summary>
        /// <param name="start">The start</param>
        /// <param name="step">The step</param>
        /// <param name="end">The end, always included</param>
        /// <returns>The array of ints, never null</returns>
        public static IEnumerable<int> Range(int start, int step, int end)
        {
            bool GotEnd = false;
            /*
             * To speed things up, might be best to use array and calculate
             * length ahead of time.
             * 
             * length = ((end - start - 1) / step) + 2
             * 
             * 1, 13, 3 = 5
             * 1, 14, 3 = 6
             * 1, 15, 3 = 6
             * 
             */

            try
            {

                int[] list;
                int index = 0;

                if (start == end)
                {
                    list = new int[1];
                }
                else
                {
                    list = new int[((end - start - 1) / step) + 2];
                }

                for (var a = start; a <= end; a += step)
                {
                    if (a == end)
                    {
                        GotEnd = true;
                    }
                    list[index++] = a;
                }

                if (!GotEnd)
                {
                    list[index++] = end;
                }

                return list;
            }
            catch (Exception e)
            {
                //Console.WriteLine();

                throw e;
            }
        }


        private bool SetupCBWithSetting(ComboBox CB, Dictionary<string, RFD.RFD900.TSetting> Settings,
            string Value, bool Remote)
        {
            string SettingName;

            if (Remote)
            {
                SettingName = CB.Name.Substring(1);
            }
            else
            {
                SettingName = CB.Name;
            }
            if (Settings.ContainsKey(SettingName))
            {
                var Setting = Settings[SettingName];
                if (Setting.Options != null)
                {
                    //Use options.
                    string[] OptionNames = Setting.GetOptionNames();
                    string OptionName = Setting.GetOptionNameForValue(Value);
                    if (OptionName == null)
                    {
                        Array.Resize(ref OptionNames, OptionNames.Length + 1);
                        OptionNames[OptionNames.Length - 1] = Value;
                        OptionName = Value;
                    }
                    
                    CB.DataSource = OptionNames;
                    CB.Text = OptionName;
                    CB.Tag = Setting;
                    return true;
                }
                if (Setting.Range != null)
                {
                    CB.DataSource = Range(Setting.Range.Min, Setting.Increment, Setting.Range.Max);
                    CB.Text = Value;
                    CB.Tag = null;
                    return true;
                }
            }
            
            CB.Tag = null;
            CB.Text = Value;
            return false;
        }

        private string GetCBValue(ComboBox CB)
        {
            if (CB.Tag != null)
            {
                RFD.RFD900.TSetting Setting = (RFD.RFD900.TSetting)CB.Tag;
                foreach (var O in Setting.Options)
                {
                    if (O.OptionName == CB.Text)
                    {
                        return O.Value.ToString();
                    }
                }
            }
            return CB.Text;
        }

        void SetupComboForMavlink(ComboBox CB,  bool Simple)
        {
            Dictionary<int, string> dict;
            if (Simple)
            {
                dict = Enum.GetValues(typeof(mavlink_option_simple))
                    .Cast<mavlink_option>()
                    .ToDictionary(t => (int)t, t => t.ToString());
            }
            else
            {
                dict = Enum.GetValues(typeof(mavlink_option))
                    .Cast<mavlink_option>()
                    .ToDictionary(t => (int)t, t => t.ToString());
            }

            CB.DataSource = dict.ToArray();
        }

        /// <summary>
        /// Load settings button evt hdlr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUT_getcurrent_Click(object sender, EventArgs e)
        {
            //EndSession();
            var Session = GetSession();

            if (Session == null)
            {
                return;
            }

            ResetAllControls(groupBoxLocal);
            ResetAllControls(groupBoxRemote);

            EnableConfigControls(false);
            EnableProgrammingControls(false);
            lbl_status.Text = "Connecting";

            try
            {
                if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
                {
                    bool SomeSettingsInvalid = false;
                    // cleanup
                    doCommand(Session.Port, "AT&T", false, 1);

                    Session.Port.DiscardInBuffer();

                    lbl_status.Text = "Doing Command ATI & RTI";

                    //Set the text box to show the radio version
                    int multipoint_fix = -1;
                    var ati_str = doCommand(Session.Port, "ATI").Trim();
                    if (ati_str.StartsWith("["))
                    {
                        multipoint_fix = ati_str.IndexOf(']') + 1;
                    }
                    ATI.Text = ati_str;

                    NumberStyles style = NumberStyles.Any;

                    //Get the board frequency.
                    var freqstring = doCommand(Session.Port, "ATI3").Trim();
                    if (multipoint_fix > 0)
                    {
                        freqstring = freqstring.Substring(multipoint_fix).Trim();
                    }

                    if (freqstring.ToLower().Contains('x'))
                        style = NumberStyles.AllowHexSpecifier;

                    var freq =
                        (Uploader.Frequency)
                            Enum.Parse(typeof(Uploader.Frequency),
                                int.Parse(freqstring.ToLower().Replace("x", ""), style).ToString());

                    ATI3.Text = freq.ToString();

                    style = NumberStyles.Any;

                    var boardstring = doCommand(Session.Port, "ATI2").Trim();
                    if (multipoint_fix > 0)
                    {
                        boardstring = boardstring.Substring(multipoint_fix).Trim();
                    }

                    if (boardstring.ToLower().Contains('x'))
                        style = NumberStyles.AllowHexSpecifier;

                    Session.Board =
                        (Uploader.Board)
                            Enum.Parse(typeof(Uploader.Board),
                                int.Parse(boardstring.ToLower().Replace("x", ""), style).ToString());

                    ATI2.Text = Session.Board.ToString();

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        //RFD900x has a new set of acceptable settings ranges...
                        SERIAL_SPEED.DataSource = new int[] { 1, 2, 4, 9, 19, 38, 57, 115, 230, 460 };
                        RSERIAL_SPEED.DataSource = new int[] { 1, 2, 4, 9, 19, 38, 57, 115, 230, 460 };

                        AIR_SPEED.DataSource = new int[] { 4, 64, 125, 250, 500 };
                        RAIR_SPEED.DataSource = new int[] { 4, 64, 125, 250, 500 };

                        if (ATI.Text.Contains("ASYNC"))
                        {
                            NETID.DataSource = Range(0, 1, 255);
                            RNETID.DataSource = Range(0, 1, 255);
                        }
                        else
                        {
                            NETID.DataSource = Range(0, 1, 65535);
                            RNETID.DataSource = Range(0, 1, 65535);
                        }

                        MIN_FREQ.DataSource = Range(902000, 1000, 927000);
                        RMIN_FREQ.DataSource = Range(902000, 1000, 927000);

                        MAX_FREQ.DataSource = Range(903000, 1000, 928000);
                        RMAX_FREQ.DataSource = Range(903000, 1000, 928000);

                        NUM_CHANNELS.DataSource = Range(1, 1, 50);
                        RNUM_CHANNELS.DataSource = Range(1, 1, 50);

                        MAX_WINDOW.DataSource = Range(20, 1, 400);
                        RMAX_WINDOW.DataSource = Range(20, 1, 400);

                    }
                    else
                    {
                        RestoreAllDefaultCBObjects();
                    }

                    // 8 and 9
                    if (freq == Uploader.Frequency.FREQ_915)
                    {
                        MIN_FREQ.DataSource = Range(895000, 1000, 935000);
                        RMIN_FREQ.DataSource = Range(895000, 1000, 935000);

                        MAX_FREQ.DataSource = Range(895000, 1000, 935000);
                        RMAX_FREQ.DataSource = Range(895000, 1000, 935000);
                    }
                    else if (freq == Uploader.Frequency.FREQ_433)
                    {
                        MIN_FREQ.DataSource = Range(414000, 10, 460000);
                        RMIN_FREQ.DataSource = Range(414000, 10, 460000);

                        MAX_FREQ.DataSource = Range(414000, 10, 460000);
                        RMAX_FREQ.DataSource = Range(414000, 10, 460000);
                    }
                    else if (freq == Uploader.Frequency.FREQ_868)
                    {
                        MIN_FREQ.DataSource = Range(849000, 1000, 889000);
                        RMIN_FREQ.DataSource = Range(849000, 1000, 889000);

                        MAX_FREQ.DataSource = Range(849000, 1000, 889000);
                        RMAX_FREQ.DataSource = Range(849000, 1000, 889000);
                    }

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900 ||
                            Session.Board == Uploader.Board.DEVICE_ID_RFD900A
                            || Session.Board == Uploader.Board.DEVICE_ID_RFD900P ||
                            Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        TXPOWER.DataSource = Range(0, 1, 30);
                        RTXPOWER.DataSource = Range(0, 1, 30);
                    }
                    else
                    {
                        TXPOWER.DataSource = Range(0, 1, 20);
                        RTXPOWER.DataSource = Range(0, 1, 20);
                    }

                    if (Session.Board == Uploader.Board.DEVICE_ID_RFD900X)
                    {
                        LBT_RSSI.DataSource = Range(0, 25, 220);
                        RLBT_RSSI.DataSource = Range(0, 25, 220);
                    }
                    else
                    {
                        LBT_RSSI.DataSource = Range(0, 1, 1);
                        RLBT_RSSI.DataSource = Range(0, 1, 1);
                    }

                    if (multipoint_fix == -1)
                    {
                        var AESKey = doCommand(Session.Port, "AT&E?").Trim();
                        if (AESKey.Contains("ERROR"))
                        {
                            txt_aeskey.Text = "";
                            txt_aeskey.Enabled = false;
                        }
                        else
                        {
                            txt_aeskey.Text = AESKey;
                            txt_aeskey.Enabled = true;
                        }
                        SetupComboForMavlink(MAVLINK, false);
                    }
                    else
                    {
                        txt_aeskey.Text = "";
                        txt_aeskey.Enabled = false;
                        SetupComboForMavlink(MAVLINK, true);
                    }

                    RSSI.Text = doCommand(Session.Port, "ATI7").Trim();

                    lbl_status.Text = "Doing Command ATI5";

                    var answer = doCommand(Session.Port, "ATI5", true);

                    var Settings = Session.GetSettings(
                        doCommand(Session.Port, "ATI5?", true),
                        Session.Board);

                    DisableRFD900xControls();

                    var items = answer.Split('\n');

                    foreach (var kvp in _DefaultLocalEnabled)
                    {
                        kvp.Key.Enabled = kvp.Value;
                    }

                    if (ATI.Text.Contains("ASYNC"))
                    {
                        _LocalExtraParams.SetModel(Model.ASYNC);
                    }
                    else
                    {
                        if ((items.Length > 0) && items[0].StartsWith("["))
                        {
                            //This is multipoint firmware.
                            _LocalExtraParams.SetModel(Model.MULTIPOINT);
                        }
                        else if (ATI.Text.Contains("MP on") && (Session.Board == Uploader.Board.DEVICE_ID_RFD900X))
                        {
                            _LocalExtraParams.SetModel(Model.MULTIPOINT_X);
                        }
                        else
                        {
                            //This is p2p firmware.
                            _LocalExtraParams.SetModel(Model.P2P);
                        }
                    }

                    //For each of the settings returned by the radio...
                    foreach (var item in items)
                    {
                        //if (item.StartsWith("S"))
                        {
                            var mod_item = item;
                            if (multipoint_fix > 0 && item.Length > multipoint_fix)
                            {
                                mod_item = item.Substring(multipoint_fix).Trim();
                            }
                            var values = mod_item.Split(new char[] { ':', '=' }, StringSplitOptions.RemoveEmptyEntries);

                            if (values.Length == 3)
                            {
                                values[1] = values[1].Replace("/", "_");

                                var controls = groupBoxLocal.Controls.Find(values[1].Trim(), true);

                                //If there's a control with the same name as the setting...
                                if (controls.Length > 0)
                                {
                                    groupBoxLocal.Enabled = true;
                                    controls[0].Parent.Enabled = true;
                                    controls[0].Enabled = true;

                                    if (controls[0] is CheckBox)
                                    {
                                        ((CheckBox)controls[0]).Checked = values[2].Trim() == "1";
                                    }
                                    else if (controls[0] is TextBox)
                                    {
                                        ((TextBox)controls[0]).Text = values[2].Trim();
                                    }
                                    else if (controls[0].Name.Contains("MAVLINK")) //
                                    {
                                        var ans = Enum.Parse(typeof(mavlink_option), values[2].Trim());
                                        ((ComboBox)controls[0]).Text = ans.ToString();
                                    }
                                    else if (controls[0] is ComboBox)
                                    {
                                        if (!SetupCBWithSetting((ComboBox)controls[0], Settings, 
                                            values[2].Trim(), false))
                                        {
                                            ((ComboBox)controls[0]).Text = values[2].Trim();
                                            if (((ComboBox)controls[0]).Text != values[2].Trim())
                                            {
                                                SomeSettingsInvalid = true;
                                                
                                            }
                                            
                                        }

                                    }
                                }
                            }
                        }
                    }

                    // remote
                    foreach (Control ctl in groupBoxRemote.Controls)
                    {
                        if ((ctl.Name != "RSSI")&& (!(ctl is Label)))
                        {
                            ctl.ResetText();
                        }
                    }

                    Session.Port.DiscardInBuffer();

                    RTI.Text = doCommand(Session.Port, "RTI");

                    if (RTI.Text != "")
                    {

                        try
                        {
                            var resp = doCommand(Session.Port, "RTI2");
                            if (resp.Trim() != "")
                                RTI2.Text =
                                    ((Uploader.Board)Enum.Parse(typeof(Uploader.Board), resp)).ToString();
                        }
                        catch
                        {
                        }

                        if (multipoint_fix == -1)
                        {
                            var AESKey = doCommand(Session.Port, "RT&E?").Trim();
                            if (AESKey.Contains("ERROR"))
                            {
                                txt_Raeskey.Text = "";
                                txt_Raeskey.Enabled = false;
                            }
                            else
                            {
                                txt_Raeskey.Text = AESKey;
                                txt_Raeskey.Enabled = true;
                            }
                            SetupComboForMavlink(RMAVLINK, false);
                        }
                        else
                        {
                            txt_Raeskey.Text = "";
                            txt_Raeskey.Enabled = false;
                            SetupComboForMavlink(RMAVLINK, true);
                        }

                        lbl_status.Text = "Doing Command RTI5";

                        answer = doCommand(Session.Port, "RTI5", true);

                        items = answer.Split('\n');

                        if (RTI.Text.Contains("ASYNC"))
                        {
                            _RemoteExtraParams.SetModel(Model.ASYNC);
                        }
                        else
                        {
                            if ((items.Length > 0) && items[0].StartsWith("["))
                            {
                                //This is multipoint firmware.
                                _RemoteExtraParams.SetModel(Model.MULTIPOINT);
                            }
                            else if (RTI.Text.Contains("MP on") && (Session.Board == Uploader.Board.DEVICE_ID_RFD900X))
                            {
                                _RemoteExtraParams.SetModel(Model.MULTIPOINT_X);
                            }
                            else
                            {
                                //This is 2-point firmware.
                                _RemoteExtraParams.SetModel(Model.P2P);
                            }
                        }

                        foreach (var item in items)
                        {
                            //if (item.StartsWith("S"))
                            {
                                var values = item.Split(new char[] { ':', '=' }, StringSplitOptions.RemoveEmptyEntries);

                                if (values.Length == 3)
                                {
                                    values[1] = values[1].Replace("/", "_");

                                    var controls = groupBoxRemote.Controls.Find("R" + values[1].Trim(), true);

                                    if (controls.Length == 0)
                                        continue;


                                    controls[0].Enabled = true;

                                    if (controls[0] is CheckBox)
                                    {
                                        ((CheckBox)controls[0]).Checked = values[2].Trim() == "1";
                                    }
                                    else if (controls[0] is TextBox)
                                    {
                                        ((TextBox)controls[0]).Text = values[2].Trim();
                                    }
                                    else if (controls[0].Name.Contains("MAVLINK")) //
                                    {
                                        var ans = Enum.Parse(typeof(mavlink_option), values[2].Trim());
                                        ((ComboBox)controls[0]).Text = ans.ToString();
                                    }
                                    else if (controls[0] is ComboBox)
                                    {
                                        if (!SetupCBWithSetting((ComboBox)controls[0], Settings,
                                            values[2].Trim(), true))
                                        {
                                            ((ComboBox)controls[0]).Text = values[2].Trim();
                                            if (((ComboBox)controls[0]).Text != values[2].Trim())
                                            {
                                                SomeSettingsInvalid = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    /*
                                    if (item.Contains("S15"))
                                    {
                                        answer = doCommand(comPort, "RTS15?");
                                        int rts15 = 0;
                                        if (int.TryParse(answer, out rts15))
                                        {
                                            RS15.Enabled = true;
                                            RS15.Text = rts15.ToString();
                                        }
                                    }
                                    */
                                    log.Info("Odd config line :" + item);
                                }
                            }
                        }
                    }

                    // off hook
                    Session.PutIntoTransparentMode();

                    if (SomeSettingsInvalid)
                    {
                        lbl_status.Text = "Done.  Some settings in modem were invalid.";
                    }
                    else
                    {
                        lbl_status.Text = "Done";
                    }
                }
                else
                {
                    // off hook
                    Session.PutIntoTransparentMode();

                    lbl_status.Text = "Fail";
                    MsgBox.CustomMessageBox.Show("Failed to enter command mode");
                }

                //BUT_Syncoptions.Enabled = true;

                //BUT_savesettings.Enabled = true;

                //BUT_SetPPMFailSafe.Enabled = true;

                EnableConfigControls(true);
                
                EnableProgrammingControls(true);
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Error";
                MsgBox.CustomMessageBox.Show("Error during read " + ex);
            }

            if (GetCommsSerialAlt == null)
            {
                EndSessionClosePort();
            }
        }

        private string Serial_ReadLine(ICommsSerial comPort)
        {
            var sb = new StringBuilder();
            var Deadline = DateTime.Now.AddMilliseconds(comPort.ReadTimeout);

            while (DateTime.Now < Deadline)
            {
                if (comPort.BytesToRead > 0)
                {
                    var data = (byte)comPort.ReadByte();
                    sb.Append((char)data);
                    if (data == '\n')
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Send a command to the radio, wait for a response.
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="cmd">The command</param>
        /// <param name="multiLineResponce"></param>
        /// <param name="level"></param>
        /// <returns>The response</returns>
        public string doCommand(ICommsSerial comPort, string cmd, bool multiLineResponce = false, int level = 0)
        {
            if (!comPort.IsOpen)
                return "";

            comPort.DiscardInBuffer();

            lbl_status.Text = "Doing Command " + cmd;
            log.Info("Doing Command " + cmd);
            comPort.ReadTimeout = 1000;

            comPort.Write("\r\n");
            Serial_ReadLine(comPort);
            Thread.Sleep(50);
            comPort.Write(cmd + "\r\n");

            comPort.ReadTimeout = 1000;

            // command echo
            var cmdecho = Serial_ReadLine(comPort);

            if (cmdecho.Contains(cmd))
            {
                var value = "";

                if (multiLineResponce)
                {
                    var deadline = DateTime.Now.AddMilliseconds(1000);
                    while (comPort.BytesToRead > 0 || DateTime.Now < deadline)
                    {
                        try
                        {
                            value = value + Serial_ReadLine(comPort);
                        }
                        catch
                        {
                            value = value + comPort.ReadExisting();
                        }
                    }
                }
                else
                {
                    value = Serial_ReadLine(comPort);

                    if (value == "" && level == 0)
                    {
                        return doCommand(comPort, cmd, multiLineResponce, 1);
                    }
                }

                log.Info(value.Replace('\0', ' '));

                return value;
            }

            comPort.DiscardInBuffer();

            // try again
            if (level == 0)
                return doCommand(comPort, cmd, multiLineResponce, 1);

            return "";
        }

        /// <summary>
        /// Tries to put the radio into AT command mode.
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        public bool doConnect(ICommsSerial comPort)
        {
            try
            {
                Console.WriteLine("doConnect");

                var trys = 1;

                // setup a known enviroment
                comPort.Write("ATO\r\n");

            retry:

                // wait
                Sleep(1500, comPort);
                comPort.DiscardInBuffer();
                // send config string
                comPort.Write("+");
                Sleep(200, comPort);
                comPort.Write("+");
                Sleep(200, comPort);
                comPort.Write("+");
                Sleep(1500, comPort);
                // check for config response "OK"
                log.Info("Connect btr " + comPort.BytesToRead + " baud " + comPort.BaudRate);
                // allow time for data/response

                if (comPort.BytesToRead == 0 && trys <= 3)
                {
                    trys++;
                    log.Info("doConnect retry");
                    goto retry;
                }

                var buffer = new byte[20];
                var len = comPort.Read(buffer, 0, buffer.Length);
                var conn = Encoding.ASCII.GetString(buffer, 0, len);
                log.Info("Connect first response " + conn.Replace('\0', ' ') + " " + conn.Length);
                if (conn.Contains("OK"))
                {
                    //return true;
                }
                else
                {
                    // cleanup incase we are already in cmd mode
                    comPort.Write("\r\n");
                }

                doCommand(comPort, "AT&T", false, 1);

                var version = doCommand(comPort, "ATI");

                log.Info("Connect Version: " + version.Trim() + "\n");

                var regex = new Regex(@"SiK\s+(.*)\s+on\s+(.*)");

                if (regex.IsMatch(version))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void BUT_Syncoptions_Click(object sender, EventArgs e)
        {
            RAIR_SPEED.Text = AIR_SPEED.Text;
            RNETID.Text = NETID.Text;
            RECC.Checked = ECC.Checked;
            RMAVLINK.Text = MAVLINK.Text;
            RMIN_FREQ.Text = MIN_FREQ.Text;
            RMAX_FREQ.Text = MAX_FREQ.Text;
            RNUM_CHANNELS.Text = NUM_CHANNELS.Text;
            RMAX_WINDOW.Text = MAX_WINDOW.Text;
            RENCRYPTION_LEVEL.Checked = ENCRYPTION_LEVEL.Checked;
            txt_Raeskey.Text = txt_aeskey.Text;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MsgBox.CustomMessageBox.Show(@"The Sik Radios have 2 status LEDs, one red and one green.
green LED blinking - searching for another radio 
green LED solid - link is established with another radio 
red LED flashing - transmitting data 
red LED solid - in firmware update mode");
        }

        void DoCommandShowErrorIfNotOK(ICommsSerial Port, string cmd, string ErrorMsg)
        {
            string Result = doCommand(Port, cmd);
            if (!Result.Contains("OK"))
            {
                MsgBox.CustomMessageBox.Show(ErrorMsg);
            }
        }

        private void BUT_resettodefault_Click(object sender, EventArgs e)
        {
            //EndSession();
            var Session = GetSession();
            if (Session == null)
            {
                return;
            }

            lbl_status.Text = "Connecting";

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                if (RTI.Text != "")
                {
                    doCommand(Session.Port, "RT&T");

                    Session.Port.DiscardInBuffer();

                    lbl_status.Text = "Doing Command RTI & AT&F";

                    doCommand(Session.Port, "RT&F");

                    doCommand(Session.Port, "RT&W");

                    lbl_status.Text = "Reset";

                    doCommand(Session.Port, "RTZ");

                    doCommand(Session.Port, "RT&T");
                }

                doCommand(Session.Port, "AT&T", false, 1);

                Session.Port.DiscardInBuffer();

                lbl_status.Text = "Doing Command ATI & AT&F";

                DoCommandShowErrorIfNotOK(Session.Port, "AT&F", "Failed to reset parameters to factory defaults");

                DoCommandShowErrorIfNotOK(Session.Port, "AT&W", "Failed to write parameters to EEPROM");

                lbl_status.Text = "Reset";
                doCommand(Session.Port, "ATZ");

                //Session must be ended because modem rebooted.
                Session.PutIntoATCommandModeAssumingInTransparentMode();
            }
            else
            {
                // off hook
                Session.PutIntoTransparentMode();

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode");
            }
            if (GetCommsSerialAlt == null)
            {
                EndSessionClosePort();
            }
        }

        void UpdateStatus(string Status)
        {
            //lblStatus.Text = Status;
            lbl_status.Text = Status;
            Application.DoEvents();
        }

        void EnableConfigControls(bool Enable)
        {
            groupBoxLocal.Enabled = Enable;
            groupBoxRemote.Enabled = Enable;
            BUT_Syncoptions.Enabled = Enable;
            BUT_SetPPMFailSafe.Enabled = Enable;
            BUT_getcurrent.Enabled = Enable;
            BUT_savesettings.Enabled = Enable;
            BUT_resettodefault.Enabled = Enable;
        }
        public static void ResetAllControls(Control form)
        {
            {
                foreach (Control control in form.Controls)
                {
                    control.Enabled = false;
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.Text = null;
                    }

                    if (control is ComboBox)
                    {
                        ComboBox comboBox = (ComboBox)control;
                        if (comboBox.Items.Count > 0)
                            comboBox.SelectedIndex = 0;
                    }

                    if (control is CheckBox)
                    {
                        CheckBox checkBox = (CheckBox)control;
                        checkBox.Checked = false;
                    }

                    if (control is ListBox)
                    {
                        ListBox listBox = (ListBox)control;
                        listBox.ClearSelected();
                    }
                }
            }
        }

        void EnableProgrammingControls(bool Enable)
        {
            BUT_loadcustom.Enabled = Enable;
        }

        void DisableRFD900xControls()
        {
            GPI1_1R_CIN.Enabled = false;
            RGPI1_1R_CIN.Enabled = false;
            GPO1_1R_COUT.Enabled = false;
            RGPO1_1R_COUT.Enabled = false;
        }

        private void BUT_loadcustom_Click(object sender, EventArgs e)
        {
            ProgramFirmware(true);
        }

        void ProgramFirmware(bool Custom)
        {
            EnableProgrammingControls(false);
            EnableConfigControls(false);

            try
            {
                //EndSession();
                var Session = GetSession();
                UpdateStatus("Determining mode...");
                var Mode = Session.GetMode();
                UpdateStatus("Mode is " + Mode.ToString());
                //Console.WriteLine("Mode is " + Mode.ToString());
                //port.Close();

                RFD.RFD900.RFD900 RFD900 = null;
                UpdateStatus("Putting in to bootloader mode");
                switch (Session.PutIntoBootloaderMode())
                {
                    case RFD.RFD900.TSession.TMode.BOOTLOADER:
                        RFD900 = RFD.RFD900.RFD900APU.GetObjectForModem(Session);
                        break;
                    case RFD.RFD900.TSession.TMode.BOOTLOADER_X:
                        RFD900 = RFD.RFD900.RFD900xux.GetObjectForModem(Session);
                        break;
                    default:
                        break;
                }

                if (RFD900 == null)
                {
                    UpdateStatus("Could not put in to bootloader mode");
                }
                else
                {
                    if (Custom)
                    {
                        UpdateStatus("Asking user for firmware file");
                    }
                    else
                    {
                        UpdateStatus("Getting firmware from internet");
                    }
                    if (getFirmware(RFD900.Board, Custom))
                    {
                        UpdateStatus("Programming firmware into device");
                        if (RFD900.ProgramFirmware(firmwarefile, ProgressEvtHdlr))
                        {
                            UpdateStatus("Programmed firmware into device");
                        }
                        else
                        {
                            UpdateStatus("Programming failed.  (Try again?)");
                        }
                    }
                    else
                    {
                        UpdateStatus("Firmware file selection cancelled");
                    }
                }
            }
            catch
            {

            }
            EnableProgrammingControls(true);
            EnableConfigControls(true);
            //UploadFW(true);
            if (GetCommsSerialAlt == null)
            {
                EndSessionClosePort();
            }
        }

        void ProgressEvtHdlr(double Completed)
        {
            try
            {
                Progressbar.Minimum = 0;
                Progressbar.Maximum = 100;
                Progressbar.Value = Math.Min((int)(Completed * 100F), 100);
                Application.DoEvents();
                
            }
            catch
            {
                //Console.WriteLine("Failed");
            }
        }

        private void Progressbar_Click(object sender, EventArgs e)
        {
            beta = !beta;
            MsgBox.CustomMessageBox.Show("Beta set to " + beta);
        }

        private void linkLabel_mavlink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MAVLINK.SelectedValue = 1;
            MAX_WINDOW.Text = 131.ToString();

            RMAVLINK.SelectedValue = 1;
            RMAX_WINDOW.Text = 131.ToString();
        }

        private void linkLabel_lowlatency_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MAVLINK.SelectedValue = 2;
            MAX_WINDOW.Text = 33.ToString();

            RMAVLINK.SelectedValue = 2;
            RMAX_WINDOW.Text = 33.ToString();
        }

        private enum mavlink_option
        {
            RawData = 0,
            Mavlink = 1,
            LowLatency = 2
        }

        private enum mavlink_option_simple
        {
            RawData = 0,
            Mavlink = 1,
        }

        private void txt_aeskey_TextChanged(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;

            string item = txt.Text;
            if (!(Regex.IsMatch(item, "^[0-9a-fA-F]+$")))
            {
                if (item.Length != 0)
                    txt.Text = item.Remove(item.Length - 1, 1);
                txt.SelectionStart = txt.Text.Length;
            }
        }

        private void BUT_SetPPMFailSafe_Click(object sender, EventArgs e)
        {

            lbl_status.Text = "Connecting";
            RFD.RFD900.TSession Session = GetSession();

            if (Session == null)
            {
                return;
            }

            if (Session.PutIntoATCommandMode() == RFD.RFD900.TSession.TMode.AT_COMMAND)
            {
                // cleanup
                doCommand(Session.Port, "AT&T", false, 1);

                Session.Port.DiscardInBuffer();

                lbl_status.Text = "Doing Command";

                string Result = doCommand(Session.Port, "RT&R");

                // off hook
                //doCommand(Session.Port, "ATO");

                if (Result.Contains("OK"))
                {
                    lbl_status.Text = "Done";
                }
                else
                {
                    lbl_status.Text = "Fail";
                }
            }
            else
            {
                // off hook
                //doCommand(Session.Port, "ATO");

                lbl_status.Text = "Fail";
                MsgBox.CustomMessageBox.Show("Failed to enter command mode");
            }

            if (GetCommsSerialAlt == null)
            {
                EndSessionClosePort();
            }
        }

        void getTelemPortWithRadio(ref ICommsSerial comPort)
        {
            // try telem1

            comPort = new MAVLinkSerialPort(MainV2.comPort, (int)MAVLink.SERIAL_CONTROL_DEV.TELEM1);

            comPort.ReadTimeout = 4000;

            comPort.Open();
        }

        /// <summary>
        /// returns null if failed
        /// </summary>
        /// <returns></returns>
        ICommsSerial GetComPort()
        {
            if (GetCommsSerialAlt == null)
            {
                ICommsSerial comPort;

                try
                {
                    if (MainV2.comPort.BaseStream.PortName.Contains("TCP"))
                    {
                        comPort = new TcpSerial();
                        comPort.BaudRate = MainV2.comPort.BaseStream.BaudRate;
                        comPort.ReadTimeout = 4000;
                        comPort.Open();
                    }
                    else
                    {
                        comPort = new SerialPort();
                        if (MainV2.comPort.BaseStream.IsOpen)
                        {
                            getTelemPortWithRadio(ref comPort);
                        }
                        else
                        {
                            comPort.PortName = MainV2.comPortName;
                            comPort.BaudRate = MainV2.comPortBaud;
                        }

                        comPort.ReadTimeout = 4000;

                        comPort.Open();
                    }

                    return comPort;
                }
                catch
                {
                    MsgBox.CustomMessageBox.Show("Invalid ComPort or in use");
                    return null;
                }
            }
            else
            {
                return GetCommsSerialAlt();
            }
        }

        RFD.RFD900.TSession GetSession()
        {
            if (_Session == null)
            {
                try
                {
                    var p = GetComPort();
                    if (p != null)
                    {
                        _Session = new RFD.RFD900.TSession(p);
                    }
                }
                catch
                {
                    MsgBox.CustomMessageBox.Show("Invalid ComPort or in use");
                    return null;
                }
            }
            else if (_Session.Port.BaudRate != MainV2.comPort.BaseStream.BaudRate ||
                (MainV2.comPort.BaseStream.PortName != "TCP" && (_Session.Port.PortName != MainV2.comPort.BaseStream.PortName)))
            {
                _Session.Dispose();
                _Session = null;
                GetSession();
            }
            return _Session;
        }

        /// <summary>
        /// Put the modem into transparent mode and close the comms port.
        /// </summary>
        private void EndSessionClosePort()
        {
            if (_Session != null)
            {
                try
                {
                    _Session.PutIntoTransparentMode();
                } catch{}
                _Session.Dispose();
                _Session.Port.Close();
                _Session = null;
            }
        }

        private void EndSession()
        {
            if (_Session != null)
            {
                _Session.Dispose();
                _Session = null;
            }
        }

        bool SetSetting(string Designator, int Value, bool Remote)
        {
            var Session = GetSession();

            if (Session == null)
            {
                return false;
            }
            else
            {
                var answer = doCommand(Session.Port, (Remote ? "RT" : "AT")+Designator+"="+Value.ToString(), false);
                return answer.Contains("OK");
            }
        }

        /// <summary>
        /// Handles a change of the local encryption level check box.
        /// </summary>
        /// <param name="sender">ignored</param>
        /// <param name="e">ignored</param>
        private void ENCRYPTION_LEVEL_CheckedChanged(object sender, EventArgs e)
        {
            EncryptionCheckChangedEvtHdlr(ENCRYPTION_LEVEL, "ATI5", "AT&E?", txt_aeskey, false);
            btnRandom.Enabled = ENCRYPTION_LEVEL.Checked;
        }

        /// <summary>
        /// Handles a change of the remote encryption level check box.
        /// </summary>
        /// <param name="sender">ignored</param>
        /// <param name="e">ignored</param>
        private void RENCRYPTION_LEVEL_CheckedChanged(object sender, EventArgs e)
        {
            EncryptionCheckChangedEvtHdlr(RENCRYPTION_LEVEL, "RTI5", "RT&E?", txt_Raeskey, true);
        }

        bool _AlreadyInEncCheckChangedEvtHdlr = false;

        /// <summary>
        /// Handles a change of an encryption level check box.
        /// </summary>
        /// <param name="CB">The checkbox which was changed.  Must not be null.</param>
        /// <param name="ATCommand">The AT command to use to get the settings
        /// from the relevant modem.  Must not be null.</param>
        void EncryptionCheckChangedEvtHdlr(CheckBox CB, string ATCommand, string EncKeyQuery,
            TextBox EncKeyTextBox, bool Remote)
        {
            if (_AlreadyInEncCheckChangedEvtHdlr)
            {
                return;
            }
            _AlreadyInEncCheckChangedEvtHdlr = true;
            try
            {
                //Write setting to radio now.
                var Session = GetSession();

                if (Session == null)
                {
                    return;
                }
                Session.PutIntoATCommandMode();
                var answer = doCommand(Session.Port, ATCommand, true);
                var Settings = Session.GetSettings(answer, Session.Board);
                if (Settings.ContainsKey("ENCRYPTION_LEVEL"))
                {
                    var Setting = Settings["ENCRYPTION_LEVEL"];
                    if (!SetSetting(Setting.Designator, CB.Checked ? 1 : 0, Remote))
                    {
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Something wrong here");
                }

                //Read AES key back out of modem and display it.  
                //BUT_getcurrent_Click(this, null);
                //txt_aeskey.Text = doCommand(Session.Port, "AT&E?").Trim();
                EncKeyTextBox.Text = doCommand(Session.Port, EncKeyQuery).Trim();
                if (CB.Checked && EncKeyTextBox.Text.Length == 0)
                {
                    //Console.WriteLine("Something wrong here");
                }
                lbl_status.Text = "Done.";
            }
            finally
            {
                _AlreadyInEncCheckChangedEvtHdlr = false;
            }
        }

        string GetRandom32BitKey()
        {
            System.Random R = new Random((int)(System.DateTime.Now.Ticks & 0xFFFFFFFF));
            return R.Next().ToString("X8");
        }

        string GetRandomKey()
        {
            return GetRandom32BitKey() + GetRandom32BitKey() + GetRandom32BitKey() + GetRandom32BitKey();
        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            //Key is 16 binary bytes (32 hex digits)
            txt_aeskey.Text = GetRandomKey();
            txt_Raeskey.Text = txt_aeskey.Text;
        }

        private void btnCommsLog_Click(object sender, EventArgs e)
        {
            
        }
    }
}