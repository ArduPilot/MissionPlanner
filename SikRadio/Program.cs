/*
 * Release notes:
 * 
 * 2.4 - Terminal window now logs in user's documents folder instead of program files folder, to prevent
 *          crash caused by folder not being writable.  RFD900x net ID range now defaults to 16 bit for
 *          p2p firmware and 8 bit for async firmware, if range isn't specified by modem.
 * 2.5 - Added TCP port capability.
 * 2.6 - Can now generate a random encryption key.
 * 2.7 - Resolved issues in which it asks for IP address and port multiple times when using a tx module.
 *          Also resolved issue in which mission planner's sik radio page doesn't load settings via
 *          tx module when the main part of mission planner had had a connection (but was disconnected).
 * 2.8 - Added support for multipoint firmware in RFD900a, +, u and x.
 * 2.9 - Fixed bugs in which it wouldn't switch between tabs properly.
 * 2.10 - Added support for RFD900ux.
 * 2.11 - Fixed bug in which switching straight to RSSI tab after program start caused exception.  Now when
 *          switch to terminal tab, automatically puts modem into AT command mode so user doesn't need to
 *          enter +++.
 * 2.12 - Fixed tooltip text for GPI1_1R/CIN and GPI1_1R/COUT
 * 2.13 - Now if a parameter fails to load, that same parameter is not saved back to the modem.
 * 2.14 - Resolved issue in which modem firmware could not be reprogrammed immediately after programming.
 * 2.15 - Added firmware programming certified check for RFD900X/UX.  Added SBUS, freq/band and antenna parameters
 *          for RFD900x.  
 * 2.16 - Resolved issue in which it was looking for the country string in the wrong part of the ATI command response.
 * 2.17 - Resolved issue in which pin wasn't being set to output/input when RC out / SBUS out checked / unchecked.
 * 2.18 - Added text boxes to settings view for local and remote modems to display country code if locked to a country,
 *          as requested by Moreton.
 * 2.19 - Fixed bug in which programming RFD900x firmware whose file size was a multiple of 128 bytes failed.  
 * 2.20 - Made changes so that it could update firmware and load settings from a modem with multipoint firmware. 
 * 2.21 - Now for modems which use the xmodem protocol for firmware updates (RFD900x and RFD900ux), it waits for
 *          and acknowledgement of the EOT packet before sending the BOOTNEW command.  This may or may not
 *          resolve the intermittent issue in which firmware update results in the modem staying in bootloader mode.
 *          Now programs version 3.01 as the reference firmware for RFD900x to check certified (instead of 3.00).
 * 2.22 - Changed "Set PPM Fail safe" feature so there is one button per local/remote modem.  Made it so it is enabled
 *          depending on GPO1_1R/COUT being set. 
 * 2.24 - Improved instructions to user in message boxes.  Improved handling of cases where it was unable to communicate
 *          with modem.  
 * 2.25 - Now does auto disconnect/reconnect of serial port after every firmware programming regardless of 
 *          success or failure.
 * 2.26 - Now sets status text to "Programming Failed.  (Try Again?)" if it fails to determine which mode the modem
 *          is in when trying to program firmware.
 * 2.28 - Now has a manufacturing page/tab for locking down modems to a country.
 * 2.29 - Resolved bug in which the value ranges available from the remote modem weren't being parsed and used in the
 *          combo box e.g. for the min and max frequency ranges.  Also, added a work-around for an issue in the RFDSiK
 *          RFD900x 3.07 firmware in which it didn't return the complete set of parameters in response to an RTI5? command.
 * 2.30 - For RFD900x multipoint firmware, now allows correct broadcast id for nodedestination parameter.   
 * 2.32 - Supports programming of RFD900ux modems.
 * 2.33 - Corrected available frequency ranges for RFD900+
 * 2.34 - Now handles new settings GPO1_1SBUSIN, GPO1_1SBUSOUT, GPO1_3STATLED, and GPO1_0TXEN485
 * 2.35 - Fixed bug in which re-loading settings when AES was previously enabled would cause it to disable AES.  
 * 2.36 - Added a retry option message window for when programming firmware fails.  
 * 2.37 - Resolved issue in which a MAX_WINDOW setting value which wasn't a multiple of 20 failed to be set in the settings GUI.
 * 2.38 - Manufacturing tab now includes lockdown for EU and India.
 * 2.39 - For checkbox settings for which the modem specifies (in the ATI5? query response) only one option, it now greys-out the checkbox.
 *          This was to cater to the X modem 3.15 firmware in which the ECC paramater can't be changed and is always false.  
 * 2.40 - Now for remote modems which do not specify valid settings ranges, if the remote modem has the same firmware version as the
 *          local modem, then assumes the ranges specified by the local modem.  If the remote modem does not specify valid settings ranges
 *          and is a different firmware version to the local modem, informs the user in a message box and advises to use the same modem
 *          firmware version.  
 * 2.42 - Now gets valid parameter ranges out of remote modem for newer firmware which doesn't support the RTI5? query.  Now supports the
 *          specified available encryption levels given by the modems (e.g. None/128bit/256bit), instead of just enabled/disabled.  
 */

using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace SikRadio
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");
        private const string MANUFACTURER_PASSWORD = "--manufacturing";
        private static bool _Manufacturer = false;

        public static bool Manufacturer
        {
            get
            {
                return _Manufacturer;
            }
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                if (RFDLib.Text.Contains(arg, MANUFACTURER_PASSWORD))
                {
                    _Manufacturer = true;
                    break;
                }
            }

            log.Info("App Start");

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());
        }
    }
}