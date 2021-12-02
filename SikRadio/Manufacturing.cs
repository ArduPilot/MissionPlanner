using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFD900Tools
{
    public partial class Manufacturing : UserControl, SikRadio.ISikRadioForm
    {
        object _Locker = new object();

        public Manufacturing()
        {
            InitializeComponent();
        }

        public void Connect()
        {
            lock (_Locker)
            {

            }
        }

        public void Disconnect()
        {
            lock (_Locker)
            {

            }
        }

        void LogString(string s)
        {
            lstLog.Items.Add(s);
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        void LogStringNonMainThread(string s)
        {
            this.BeginInvoke(new Action(() => LogString(s)));
        }

        void EnableButtons(bool Enable)
        {
            btnLockdownAU.Enabled = Enable;
            btnLockdownNZ.Enabled = Enable;
            btnLockdownUS.Enabled = Enable;
            btnLockdownEurope.Enabled = Enable;
            btnLockdownIndia.Enabled = Enable;
            btnQueryLockStatus.Enabled = Enable;
        }

        void DoLockdownBackground(RFD.RFD900.RFD900xux.TCountry Country)
        {
            lock (_Locker)
            {
                this.BeginInvoke(new Action(() => EnableButtons(false)));
                this.BeginInvoke(new Action(() => lstLog.Items.Clear()));
                LogStringNonMainThread("Locking down for " + Country.ToString() + "...");
                //Switch to AT command mode.
                LogStringNonMainThread("Putting into AT command mode...");
                var Session = new RFD.RFD900.TSession(SikRadio.Config.comPort, MissionPlanner.MainV2.comPort.BaseStream.BaudRate);
                var Mode = Session.PutIntoATCommandMode();
                if (Mode == RFD.RFD900.TSession.TMode.AT_COMMAND)
                {
                    //Get the existing version string using ATI.
                    LogStringNonMainThread("Getting the existing version string...");
                    var ATIResult = Session.ATCClient.DoQuery("ATI", true);
                    LogStringNonMainThread("Existing version is \"" + ATIResult + "\"");

                    var Mdm = Session.GetModemObject();
                    if (Mdm == null || !(Mdm is RFD.RFD900.RFD900xuxRevN))
                    {
                        LogStringNonMainThread("Don't know how to lock this modem for country, aborting, aborting.");
                    }
                    else
                    {
                        RFD.RFD900.RFD900xuxRevN RFDX = (RFD.RFD900.RFD900xuxRevN)Mdm;

                        var CountryCode = RFDX.GetCountryCode();
                        if (!RFD.RFD900.RFD900xuxRevN.GetIsCountryLocked(CountryCode))
                        {
                            var Band = RFDX.GetBand();

                            //Check band is acceptable.
                            if (RFD.RFD900.RFD900xuxRevN.GetCanBeLockedToCountry(Band, Country))
                            {
                                //AT+C32=x
                                LogStringNonMainThread("Locking down for country...");
                                bool Success = RFDX.WriteCounty(Country);
                                if (Success)
                                {
                                    LogStringNonMainThread("Modem reported done OK.");
                                    //AT&W
                                    LogStringNonMainThread("Writing to flash...");
                                    Success = Session.ATCClient.DoCommand("AT&W");
                                    if (Success)
                                    {
                                        LogStringNonMainThread("Modem reported done OK.");
                                        //ATZ
                                        LogStringNonMainThread("Resetting modem...");
                                        Success = Session.ATCClient.DoCommand("ATZ");
                                        Success = true; //ATZ command doesn't return OK.
                                        if (Success)
                                        {
                                            //LogStringNonMainThread("Modem reported done OK.");
                                            //Get the version string using ATI and verify country code.
                                            LogStringNonMainThread("Putting into AT command mode...");
                                            Session.AssumeMode(RFD.RFD900.TSession.TMode.TRANSPARENT);
                                            Mode = Session.PutIntoATCommandMode();
                                            if (Mode == RFD.RFD900.TSession.TMode.AT_COMMAND)
                                            {
                                                //Get the version string using ATI.
                                                LogStringNonMainThread("Getting the version string...");
                                                ATIResult = Session.ATCClient.DoQuery("ATI", true);
                                                LogStringNonMainThread("Resulting version is \"" + ATIResult + "\"");
                                                LogStringNonMainThread("Finished.");
                                            }
                                            else
                                            {
                                                LogStringNonMainThread("Failed to put into AT command mode, aborting.");
                                            }
                                        }
                                        else
                                        {
                                            LogStringNonMainThread("Command didn't return OK.  Aborting.");
                                        }
                                    }
                                    else
                                    {
                                        LogStringNonMainThread("Couldn't lock for country.  Aborting.");
                                    }
                                }
                                else
                                {
                                    LogStringNonMainThread("Command didn't return OK.  Aborting.");
                                }
                            }
                            else
                            {
                                LogStringNonMainThread("Modems with band " + Band.ToString() + " cannot be locked to " + Country.ToString());
                            }
                        }
                        else
                        {
                            LogStringNonMainThread("Already locked to country " + CountryCode + ".  Aborting");
                        }
                    }
                }
                else
                {
                    LogStringNonMainThread("Failed to put into AT command mode, aborting.");
                }
                this.BeginInvoke(new Action(() => EnableButtons(true)));
            }
        }

        void DoLockdown(RFD.RFD900.RFD900xux.TCountry Country)
        {
            System.Threading.Thread Worker = new System.Threading.Thread(() => DoLockdownBackground(Country));
            Worker.Start();
        }

        void DoQueryLockdownBackground()
        {
            lock (_Locker)
            {
                this.BeginInvoke(new Action(() => EnableButtons(false)));
                this.BeginInvoke(new Action(() => lstLog.Items.Clear()));
                LogStringNonMainThread("Querying lockdown status...");
                //Switch to AT command mode.
                LogStringNonMainThread("Putting into AT command mode...");
                var Session = new RFD.RFD900.TSession(SikRadio.Config.comPort, MissionPlanner.MainV2.comPort.BaseStream.BaudRate);
                var Mode = Session.PutIntoATCommandMode();
                if (Mode == RFD.RFD900.TSession.TMode.AT_COMMAND)
                {
                    //Get the existing version string using ATI.
                    LogStringNonMainThread("Getting the existing version string...");
                    var ATIResult = Session.ATCClient.DoQuery("ATI", true);
                    LogStringNonMainThread("Existing version is \"" + ATIResult + "\"");
                    var Mdm = Session.GetModemObject();

                    if (Mdm == null)
                    {
                        LogStringNonMainThread("Having trouble communicating with modem.");
                        LogStringNonMainThread("Finished.");
                    }
                    else
                    {
                        if (Mdm is RFD.RFD900.RFD900xuxRevN)
                        {
                            var CountryCode = ((RFD.RFD900.RFD900xuxRevN)Mdm).GetCountryCode();
                            if (!RFD.RFD900.RFD900xuxRevN.GetIsCountryLocked(CountryCode))
                            {
                                //AT+C32=x
                                LogStringNonMainThread("Doesn't appear to be locked down.");
                                LogStringNonMainThread("Finished.");
                            }
                            else
                            {
                                LogStringNonMainThread("Already locked to country " + CountryCode + ".");
                                LogStringNonMainThread("Finished.");
                            }
                        }
                        else
                        {
                            LogStringNonMainThread("Don't know how to lock this modem to country.");
                            LogStringNonMainThread("Finished.");
                        }
                    }
                }
                else
                {
                    LogStringNonMainThread("Failed to put into AT command mode, aborting.");
                }
                this.BeginInvoke(new Action(() => EnableButtons(true)));
            }
        }

        void DoQueryLockdown()
        {
            System.Threading.Thread Worker = new System.Threading.Thread(DoQueryLockdownBackground);
            Worker.Start();
        }

        void btnLockdownAU_Click(object sender, EventArgs e)
        {
            DoLockdown(RFD.RFD900.RFD900xux.TCountry.AU);
        }

        void btnLockdownNZ_Click(object sender, EventArgs e)
        {
            DoLockdown(RFD.RFD900.RFD900xux.TCountry.NZ);
        }

        void btnLockdownUS_Click(object sender, EventArgs e)
        {
            DoLockdown(RFD.RFD900.RFD900xux.TCountry.US);
        }

        private void btnQueryLockdown_Click(object sender, EventArgs e)
        {
            DoQueryLockdown();
        }

        public string Header
        {
            get
            {
                return "Manufacturing";
            }
        }

        private void BtnLockdownEurope_Click(object sender, EventArgs e)
        {
            DoLockdown(RFD.RFD900.RFD900xux.TCountry.EU);
        }

        private void BtnLockdownIndia_Click(object sender, EventArgs e)
        {
            DoLockdown(RFD.RFD900.RFD900xux.TCountry.India);
        }
    }
}
