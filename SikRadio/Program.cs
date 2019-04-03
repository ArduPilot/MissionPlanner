﻿/*
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

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            log.Info("App Start");

            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config());
        }
    }
}