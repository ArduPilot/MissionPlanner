﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;

using GMap.NET.WindowsForms;
using GMap.NET.CacheProviders;
using log4net;

using System.Security.Permissions;
using MissionPlanner.Arduino;
using MissionPlanner.Utilities;
using GMap.NET;
using System.Xml;
using IronPython.Hosting;
using IronPython.Runtime.Operations;
using System.Net.Sockets;
using DotSpatial.Projections;
using MissionPlanner.Controls;
using System.Security;

namespace MissionPlanner
{
    public partial class temp : Form
    {
        private static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public temp()
        {
            InitializeComponent();

            if (System.Diagnostics.Debugger.IsAttached) {
            try
            {
                Controls.OpenGLtest2 ogl = new Controls.OpenGLtest2();

                this.Controls.Add(ogl);

                ogl.Dock = DockStyle.Fill;
            }
            catch { }
            }

            MissionPlanner.Utilities.Tracking.AddPage(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void temp_Load(object sender, EventArgs e)
        {

        }

        public static byte[] Swap(params object[] list)
        {
            // The copy is made becuase SetValue won't work on a struct.
            // Boxing was used because SetValue works on classes/objects.
            // Unfortunately, it results in 2 copy operations.
            object thisBoxed = list[0]; // Why make a copy?
            Type test = thisBoxed.GetType();

            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

            // System.Net.IPAddress.NetworkToHostOrder is used to perform byte swapping.
            // To convert unsigned to signed, 'unchecked()' was used.
            // See http://stackoverflow.com/questions/1131843/how-do-i-convert-uint-to-int-in-c

            // Enumerate each structure field using reflection.
            foreach (var field in test.GetFields())
            {
                // field.Name has the field's name.

                object fieldValue = field.GetValue(thisBoxed); // Get value

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());

                switch (typeCode)
                {
                    case TypeCode.Single: // float
                        {
                            Array.Copy(BitConverter.GetBytes((Single)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            Array.Copy(BitConverter.GetBytes((Int32)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            Array.Copy(BitConverter.GetBytes((UInt32)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            Array.Copy(BitConverter.GetBytes((Int16)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            Array.Copy(BitConverter.GetBytes((UInt16)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            Array.Copy(BitConverter.GetBytes((Int64)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            Array.Copy(BitConverter.GetBytes((UInt64)fieldValue), data, offset);
                            break;
                        }
                    case TypeCode.Double:
                        {
                            Array.Copy(BitConverter.GetBytes((Double)fieldValue), data, offset);
                            break;
                        }
                    default:
                        {
                            // System.Diagnostics.Debug.Fail("No conversion provided for this type");
                            break;
                        }
                }; // switch

                offset += Marshal.SizeOf(fieldValue);
            } // foreach

            return data;
        } // Swap



        private void button1_Click(object sender, EventArgs e)
        {


            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "EEPROM.bin|*.bin";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    BinaryReader br = new BinaryReader(sr.BaseStream);
                    byte[] EEPROM = br.ReadBytes(1024 * 4);
                    br.Close();
                    sr.Close();

                    IArduinoComms port = new ArduinoSTK();

                    if (DialogResult.Yes == CustomMessageBox.Show("is this a 1280?", "", MessageBoxButtons.YesNo))
                    {
                        port = new ArduinoSTK();
                        port.BaudRate = 57600;
                    }
                    else
                    {
                        port = new ArduinoSTKv2();
                        port.BaudRate = 115200;
                    }

            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            port.PortName = MissionPlanner.MainV2.comPortName;
            try
            {
                port.Open();

                if (port.connectAP())
                {
                    // waypoints
                    int start = 0;
                    int end = 1024*4;

                        log.Info(start + " to " + end);
                        port.upload(EEPROM, (short)start, (short)(end - start), (short)start);

                        if (port.keepalive())
                        {
                            // Config

                            if (port.keepalive())
                            {
                                System.Threading.Thread.Sleep(2000);
                                //MessageBox.Show("Upload Completed");
                            }
                            else
                            {
                                CustomMessageBox.Show("Communication Error - WPs wrote but no config");
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Communication Error - Bad data");
                        }
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port in use? " + ex.ToString()); port.Close(); }
                }
                catch (Exception) { CustomMessageBox.Show("Error reading file"); }
            }
        }

        private void BUT_wipeeeprom_Click(object sender, EventArgs e)
        {
            byte[] EEPROM = new byte[4*1024];

            for (int i = 0; i < EEPROM.Length;i++)
            {
                EEPROM[i] = 0xff;
            }

            IArduinoComms port = new ArduinoSTK();

            if (DialogResult.Yes == CustomMessageBox.Show("is this a 1280?", "", MessageBoxButtons.YesNo))
            {
                port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else
            {
                port = new ArduinoSTKv2();
                port.BaudRate = 115200;
            }
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            port.PortName = MissionPlanner.MainV2.comPortName;
            try
            {
                port.Open();

                if (port.connectAP())
                {
                    // waypoints
                    int start = 0;
                    int end = 1024*4;

                        log.Info(start + " to " + end);
                        port.upload(EEPROM, (short)start, (short)(end - start), (short)start);

                        if (port.keepalive())
                        {
                            // Config

                            if (port.keepalive())
                            {
                                System.Threading.Thread.Sleep(2000);
                                //MessageBox.Show("Upload Completed");
                            }
                            else
                            {
                                CustomMessageBox.Show("Communication Error - WPs wrote but no config");
                            }
                        }
                        else
                        {
                            CustomMessageBox.Show("Communication Error - Bad data");
                        }
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port in use? " + ex.ToString()); port.Close(); }
        }

        private void BUT_flashdl_Click(object sender, EventArgs e)
        {
            byte[] FLASH = new byte[256 * 1024];

            IArduinoComms port = new ArduinoSTK();

            if (DialogResult.Yes == CustomMessageBox.Show("is this a 1280?", "", MessageBoxButtons.YesNo))
            {
                port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else
            {
                port = new ArduinoSTKv2();
                port.BaudRate = 115200;
            }
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            port.PortName = MissionPlanner.MainV2.comPortName;
            try
            {
                port.Open();

                System.Threading.Thread.Sleep(100);
                
                if (port.connectAP())
                {
                    // waypoints
                    int start = 0;
                    short length = 0x100;

                    log.Info(start + " to " + FLASH.Length);

                    while (start < FLASH.Length)
                    {
                        log.Info("Doing " + length + " at " + start);
                        port.setaddress(start);
                        port.downloadflash(length).CopyTo(FLASH, start);
                        start += length;
                    }

                    StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"flash.bin", false);
                    BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                    bw.Write(FLASH, 0, FLASH.Length);
                    bw.Close();

                    sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"flash.hex", false);
                    for (int i = 0; i < FLASH.Length; i += 16)
                    {
                        string add = string.Format("{0:X4}", i);
                        if (i % (0x1000 << 4) == 0)
                        {
                            if (i != 0)
                                sw.WriteLine(":02000002{0:X4}{1:X2}", ((i >> 4) & 0xf000), 0x100 - (2 + 2 + (((i >> 4) & 0xf000) >> 8) & 0xff));
                        }
                        if (add.Length == 5)
                        {
                            add = add.Substring(1);
                        }
                        sw.Write(":{0:X2}{1}00", 16, add);
                        byte ck = (byte)(16 + (i & 0xff) + ((i >> 8) & 0xff));
                        for (int a = 0; a < 16; a++)
                        {
                            ck += FLASH[i + a];
                            sw.Write("{0:X2}", FLASH[i + a]);
                        }
                        sw.WriteLine("{0:X2}", (byte)(0x100 - ck));
                    }

                    sw.Close();

                    log.Info("Downloaded");
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port in use? " + ex.ToString()); port.Close(); }
        }

        public int swapend(int value)
        {
            int len = Marshal.SizeOf(value);

            byte[] temp = BitConverter.GetBytes(value);

            Array.Reverse(temp);

            return BitConverter.ToInt32(temp, 0);
        }

        private void BUT_flashup_Click(object sender, EventArgs e)
        {
            byte[] FLASH = new byte[1];

            try
            {
                StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex");
                FLASH = readIntelHEXv2(sr);
                sr.Close();

            }
            catch (Exception ex) { CustomMessageBox.Show("Failed to read firmware.hex : " + ex.Message); }
            IArduinoComms port = new ArduinoSTK();

            if (DialogResult.Yes == CustomMessageBox.Show("is this a 1280?", "", MessageBoxButtons.YesNo))
            {
                port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else
            {
                port = new ArduinoSTKv2();
                port.BaudRate = 115200;
            }
          
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = MissionPlanner.MainV2.comPortName;

                port.Open();

                

                if (port.connectAP())
                {
                    log.Info("starting");
                    
                    
                    port.uploadflash(FLASH, 0, FLASH.Length, 0);

                    
                    

                    log.Info("Uploaded");

                    
                }
                else
                {
                    
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();



            }
            catch (Exception ex) {  CustomMessageBox.Show("Check port settings or Port in use? " + ex.ToString()); port.Close(); }

        }

        byte[] readIntelHEX(StreamReader sr)
        {
            byte[] FLASH = new byte[sr.BaseStream.Length / 2];

            int optionoffset = 0;
            int total = 0;

            while (!sr.EndOfStream)
            {

                string line = sr.ReadLine();

                Regex regex = new Regex(@"^:(..)(....)(..)(.*)(..)$"); // length - address - option - data - checksum

                Match match = regex.Match(line);

                int length = Convert.ToInt32(match.Groups[1].Value.ToString(), 16);
                int address = Convert.ToInt32(match.Groups[2].Value.ToString(), 16);
                int option = Convert.ToInt32(match.Groups[3].Value.ToString(), 16);
                log.InfoFormat("len {0} add {1} opt {2}", length, address, option);
                if (option == 0)
                {
                    string data = match.Groups[4].Value.ToString();
                    for (int i = 0; i < length; i++)
                    {
                        byte byte1 = Convert.ToByte(data.Substring(i * 2, 2), 16);
                        FLASH[optionoffset + address] = byte1;
                        address++;
                        if ((optionoffset + address) > total)
                            total = optionoffset + address;
                    }
                }
                else if (option == 2)
                {
                    optionoffset += (int)Convert.ToUInt16(match.Groups[4].Value.ToString(), 16) << 4;
                }
                int checksum = Convert.ToInt32(match.Groups[5].Value.ToString(), 16);
            }

            Array.Resize<byte>(ref FLASH, total);

            return FLASH;
        }

        byte[] readIntelHEXv2(StreamReader sr)
        {
            byte[] FLASH = new byte[sr.BaseStream.Length / 2];

            int optionoffset = 0;
            int total = 0;

            while (!sr.EndOfStream)
            {

                string line = sr.ReadLine();

                if (line.StartsWith(":"))
                {
                    int length = Convert.ToInt32(line.Substring(1, 2), 16);
                    int address = Convert.ToInt32(line.Substring(3, 4), 16);
                    int option = Convert.ToInt32(line.Substring(7, 2), 16);
                    log.InfoFormat("len {0} add {1} opt {2}", length, address, option);

                    if (option == 0)
                    {
                        string data = line.Substring(9, length * 2);
                        for (int i = 0; i < length; i++)
                        {
                            byte byte1 = Convert.ToByte(data.Substring(i * 2, 2), 16);
                            FLASH[optionoffset + address] = byte1;
                            address++;
                            if ((optionoffset + address) > total)
                                total = optionoffset + address;
                        }
                    }
                    else if (option == 2)
                    {
                        optionoffset += (int)Convert.ToUInt16(line.Substring(9, 4), 16) << 4;
                    }
                    int checksum = Convert.ToInt32(line.Substring(line.Length - 2, 2), 16);
                }
                //Regex regex = new Regex(@"^:(..)(....)(..)(.*)(..)$"); // length - address - option - data - checksum
            }

            Array.Resize<byte>(ref FLASH, total);

            return FLASH;
        }

        private void BUT_dleeprom_Click(object sender, EventArgs e)
        {
            IArduinoComms port = new ArduinoSTK();

            if (DialogResult.Yes == CustomMessageBox.Show("is this a 1280?", "", MessageBoxButtons.YesNo))
            {
                port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else
            {
                port = new ArduinoSTKv2();
                port.BaudRate = 115200;
            }
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = MissionPlanner.MainV2.comPortName;

                log.Info("Open Port");
                port.Open();
                log.Info("Connect AP");
                if (port.connectAP())
                {
                    log.Info("Download AP");
                    byte[] EEPROM = new byte[1024*4];

                    for (int a = 0; a < 4 * 1024; a += 0x100)
                    {
                        port.setaddress(a);
                        port.download(0x100).CopyTo(EEPROM,a);
                    }
                    log.Info("Verify State");
                    if (port.keepalive())
                    {
                        StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"EEPROM.bin");
                        BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                        bw.Write(EEPROM, 0, 1024 * 4);
                        bw.Close();

                       
                    }
                    else
                    {
                        CustomMessageBox.Show("Communication Error - Bad data");
                    }
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port Error? " + ex.ToString()); if (port != null && port.IsOpen) { port.Close(); } }

        }

        private void BUT_copy1280_Click(object sender, EventArgs e)
        {
            ArduinoSTK port = new ArduinoSTK();
            port.BaudRate = 57600;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = MissionPlanner.MainV2.comPortName;

                log.Info("Open Port");
                port.Open();
                log.Info("Connect AP");
                if (port.connectAP())
                {
                    log.Info("Download AP");
                    byte[] EEPROM = new byte[1024 * 4];

                    for (int a = 0; a < 4 * 1024; a += 0x100)
                    {
                        port.setaddress(a);
                        port.download(0x100).CopyTo(EEPROM, a);
                    }
                    log.Info("Verify State");
                    if (port.keepalive())
                    {
                        StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"EEPROM1280.bin");
                        BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                        bw.Write(EEPROM, 0, EEPROM.Length);
                        bw.Close();

                        log.Info("Download AP");
                        byte[] FLASH = new byte[1024 * 128];

                        for (int a = 0; a < FLASH.Length; a += 0x100)
                        {
                            port.setaddress(a);
                            port.downloadflash(0x100).CopyTo(FLASH, a);
                        }

                        sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"FLASH1280.bin");
                        bw = new BinaryWriter(sw.BaseStream);
                        bw.Write(FLASH, 0, FLASH.Length);
                        bw.Close();

                    }
                    else
                    {
                        CustomMessageBox.Show("Communication Error - Bad data");
                    }
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port Error? " + ex.ToString()); if (port != null && port.IsOpen) { port.Close(); } }
            
        }

        private void BUT_copy2560_Click(object sender, EventArgs e)
        {
            ArduinoSTKv2 port = new ArduinoSTKv2();
            port.BaudRate = 115200;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = MissionPlanner.MainV2.comPortName;

                log.Info("Open Port");
                port.Open();
                log.Info("Connect AP");
                if (port.connectAP())
                {
                    log.Info("Download AP");
                    byte[] EEPROM = new byte[1024 * 4];

                    for (int a = 0; a < EEPROM.Length; a += 0x100)
                    {
                        port.setaddress(a);
                        port.download(0x100).CopyTo(EEPROM, a);
                    }
                    log.Info("Verify State");
                    if (port.keepalive())
                    {
                        StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"EEPROM2560.bin");
                        BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                        bw.Write(EEPROM, 0, EEPROM.Length);
                        bw.Close();

                        log.Info("Download AP");
                        byte[] FLASH = new byte[1024 * 256];

                        for (int a = 0; a < FLASH.Length; a += 0x100)
                        {
                            port.setaddress(a);
                            port.downloadflash(0x100).CopyTo(FLASH, a);
                        }

                        sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"FLASH2560.bin");
                        bw = new BinaryWriter(sw.BaseStream);
                        bw.Write(FLASH, 0, FLASH.Length);
                        bw.Close();

                    }
                    else
                    {
                        CustomMessageBox.Show("Communication Error - Bad data");
                    }
                }
                else
                {
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();
            }
            catch (Exception ex) { CustomMessageBox.Show("Port Error? " + ex.ToString()); if (port != null && port.IsOpen) { port.Close(); } }
        }

        private void BUT_copyto1280_Click(object sender, EventArgs e)
        {
            IArduinoComms port = new ArduinoSTK();

            port.BaudRate = 57600;

            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.DtrEnable = true;

            StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"EEPROM1280.bin");
            BinaryReader br = new BinaryReader(sr.BaseStream);
            byte[] EEPROM = br.ReadBytes(1024 * 4);
            br.Close();
            sr.Close();

            sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"FLASH1280.bin");
            br = new BinaryReader(sr.BaseStream);
            byte[] FLASH = br.ReadBytes(1024 * 128);
            br.Close();
            sr.Close();

            try
            {
                port.PortName = MissionPlanner.MainV2.comPortName;

                port.Open();



                if (port.connectAP())
                {
                    log.Info("starting");


                    port.uploadflash(FLASH, 0, FLASH.Length, 0);

                    port.upload(EEPROM, 0, (short)EEPROM.Length, 0);


                    log.Info("Uploaded");


                }
                else
                {

                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();



            }
            catch (Exception ex) { CustomMessageBox.Show("Check port settings or Port in use? " + ex.ToString()); port.Close(); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] FLASH = new byte[1];
                try
                {
                    StreamReader sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex");
                    FLASH = readIntelHEXv2(sr);
                    sr.Close();

                }
                catch (Exception ex) { CustomMessageBox.Show("Failed to read firmware.hex : " + ex.Message); }

                StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.bin");
                BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                bw.Write(FLASH, 0, FLASH.Length);
                bw.Close();
            
        }

        private void BUT_geinjection_Click(object sender, EventArgs e)
        {
            GMapControl MainMap = new GMapControl();

            MainMap.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;

            MainMap.CacheLocation = Path.GetDirectoryName(Application.ExecutablePath) + "/gmapcache/";

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            try
            {
                fbd.SelectedPath = @"C:\Users\hog\Documents\albany 2011\New folder";
            }
            catch { }

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            if (fbd.SelectedPath != "") 
            {

                string[] files = Directory.GetFiles(fbd.SelectedPath,"*.jpg",SearchOption.AllDirectories);
                string[] files1 = Directory.GetFiles(fbd.SelectedPath, "*.png", SearchOption.AllDirectories);

                int origlength = files.Length;
                Array.Resize(ref files, origlength + files1.Length);
                Array.Copy(files1, 0, files, origlength, files1.Length);

                foreach (string file in files)
                {
                    log.Info(DateTime.Now.Millisecond +  " Doing "+ file);
                    Regex reg = new Regex(@"Z([0-9]+)\\([0-9]+)\\([0-9]+)");

                    Match mat = reg.Match(file);

                    if (mat.Success == false)
                        continue;

                    int temp = 1 << int.Parse(mat.Groups[1].Value);

                    GMap.NET.GPoint pnt = new GMap.NET.GPoint(int.Parse(mat.Groups[3].Value), int.Parse(mat.Groups[2].Value));

                    BUT_geinjection.Text = file;
                    BUT_geinjection.Refresh();

                    //MainMap.Projection.

                    MemoryStream tile = new MemoryStream();

                    Image Img = Image.FromFile(file);
                    Img.Save(tile,System.Drawing.Imaging.ImageFormat.Jpeg);

                    tile.Seek(0, SeekOrigin.Begin);
                    log.Info(pnt.X + " " + pnt.Y);

                    Application.DoEvents();

                    GMaps.Instance.PrimaryCache.PutImageToCache(tile.ToArray(), Maps.Custom.Instance.DbId, pnt, int.Parse(mat.Groups[1].Value)); 

                   // Application.DoEvents();
                }
            }
        }

        private string getfilepath(int x, int y, int zoom)
        {
            var tileRange = 1 << zoom;

            if (x < 0 || x >= tileRange)
            {
                x = (x % tileRange + tileRange) % tileRange;
            }

            return ("Z" + zoom + "/" + y + "/" + x + ".png");

            //return new GMap.NET.GPoint(x, y);
        }

        private void BUT_clearcustommaps_Click(object sender, EventArgs e)
        {
            GMapControl MainMap = new GMapControl();
            MainMap.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;

            int removed = MainMap.Manager.PrimaryCache.DeleteOlderThan(DateTime.Now, Maps.Custom.Instance.DbId);

            CustomMessageBox.Show("Removed "+removed + " images");

            log.InfoFormat("Removed {0} images", removed);
        }
        private void BUT_lang_edit_Click(object sender, EventArgs e)
        {
            new resedit.Form1().Show();
        }

        private void BUT_georefimage_Click(object sender, EventArgs e)
        {
            new Georefimage().Show();
        }

        private void BUT_follow_me_Click(object sender, EventArgs e)
        {
            FollowMe si = new FollowMe();
            ThemeManager.ApplyThemeTo((Form)si);
            si.Show();
        }

        private void BUT_paramgen_Click(object sender, EventArgs e)
        {
            ParameterMetaDataParser.GetParameterInformation();

            ParameterMetaDataRepository.Reload();
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            new SerialOutputMD().Show();
        }

        private void but_osdvideo_Click(object sender, EventArgs e)
        {
            new OSDVideo().Show();
        }

        public static HIL.XPlane xp;

        private void BUT_xplane_Click(object sender, EventArgs e)
        {
            if (xp == null)
            {
                xp = new HIL.XPlane();

                xp.SetupSockets(49005, 49000, "127.0.0.1");
            }


            System.Threading.ThreadPool.QueueUserWorkItem(runxplanemove);

            //xp.Shutdown();
        }

        void runxplanemove(object o)
        {
            while (xp != null)
            {
                System.Threading.Thread.Sleep(500);
                xp.MoveToPos(MainV2.comPort.MAV.cs.lat, MainV2.comPort.MAV.cs.lng, MainV2.comPort.MAV.cs.alt, MainV2.comPort.MAV.cs.roll, MainV2.comPort.MAV.cs.pitch, MainV2.comPort.MAV.cs.yaw);
            }
        }

        private void BUT_magfit_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "t Log|*.tlog";

            ofd.ShowDialog();

            var com = new Comms.CommsFile();
            com.Open(ofd.FileName);

            MainV2.comPort.BaseStream = com;

            MagCalib.DoGUIMagCalib(false);

            //MagCalib.ProcessLog(0);
        }

        private void but_multimav_Click(object sender, EventArgs e)
        {
            Comms.CommsSerialScan.Scan(false);

            DateTime deadline = DateTime.Now.AddSeconds(50);

            while (Comms.CommsSerialScan.foundport == false)
            {
                System.Threading.Thread.Sleep(100);

                if (DateTime.Now > deadline)
                {
                    CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                    return;
                }
            }

            MAVLinkInterface com2 = new MAVLinkInterface();

            com2.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
            com2.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

            com2.Open(true);

            MainV2.Comports.Add(com2);

            CMB_mavs.DataSource = MainV2.Comports;


        }

        private void CMB_mavs_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports) {

                if (port.ToString() == CMB_mavs.Text) {
                    MainV2.comPort = port;
                }
            }
        }

        private void BUT_swarm_Click(object sender, EventArgs e)
        {
            new Swarm.FormationControl().Show();
        }

        private void BUT_outputMavlink_Click(object sender, EventArgs e)
        {
            new SerialOutputPass().Show();
        }

        private void BUT_outputnmea_Click(object sender, EventArgs e)
        {
            new SerialOutputNMEA().Show();
        }

        private void myButton1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void BUT_simmulti_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            var sim = new GCSViews.Simulation();
            frm.Controls.Add(sim);
            frm.Size = sim.Size;
            sim.Dock = DockStyle.Fill;

            frm.Show();

        
        }

        private void BUT_fwren_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary Log|*.bin";

            ofd.ShowDialog();

            if (File.Exists(ofd.FileName))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "log|*.log";

                DialogResult res = sfd.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    MissionPlanner.Log.BinaryLog.ConvertBin(ofd.FileName, sfd.FileName);
                }
            }
        }

        private void BUT_followleader_Click(object sender, EventArgs e)
        {
            new Swarm.FollowPathControl().Show();
        }

        private void BUT_driverclean_Click(object sender, EventArgs e)
        {
            MissionPlanner.Utilities.CleanDrivers.Clean();
        }

        private void but_compassrotation_Click(object sender, EventArgs e)
        {
            MissionPlanner.Magfitrotation.magfit();
        }

        private void BUT_sorttlogs_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = MainV2.LogDir;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    MissionPlanner.Log.LogSort.SortLogs(Directory.GetFiles(fbd.SelectedPath, "*.tlog"));
                }
                catch { }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
    
        }

        private void BUT_accellogs_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("This scan may take some time.");
            Log.Scan.ScanAccel();
            CustomMessageBox.Show("Scan Complete");
        }

        private void BUT_movingbase_Click(object sender, EventArgs e)
        {
            MovingBase si = new MovingBase();
            ThemeManager.ApplyThemeTo((Form)si);
            si.Show();
        }

        private void but_getfw_Click(object sender, EventArgs e)
        {
            string basedir = Application.StartupPath + Path.DirectorySeparatorChar + "History";

            Directory.CreateDirectory(basedir);

            Firmware fw = new Firmware();

            var list = fw.getFWList();

            using (XmlTextWriter xmlwriter = new XmlTextWriter(basedir + Path.DirectorySeparatorChar + @"firmware2.xml", Encoding.ASCII))
            {
                xmlwriter.Formatting = Formatting.Indented;

                xmlwriter.WriteStartDocument();

                xmlwriter.WriteStartElement("options");

                foreach (var software in list)
                {
                    xmlwriter.WriteStartElement("Firmware");

                    if (software.url != "")
                        xmlwriter.WriteElementString("url", new Uri(software.url).LocalPath.TrimStart('/','\\'));
                    if (software.url2560 != "")
                        xmlwriter.WriteElementString("url2560", new Uri(software.url2560).LocalPath.TrimStart('/', '\\'));
                    if (software.url2560_2 != "")
                        xmlwriter.WriteElementString("url2560-2", new Uri(software.url2560_2).LocalPath.TrimStart('/', '\\'));
                    if (software.urlpx4v1 != "")
                        xmlwriter.WriteElementString("urlpx4", new Uri(software.urlpx4v1).LocalPath.TrimStart('/', '\\'));
                    if (software.urlpx4v2 != "")
                        xmlwriter.WriteElementString("urlpx4v2", new Uri(software.urlpx4v2).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv40 != "")
                        xmlwriter.WriteElementString("urlvrbrainv40", new Uri(software.urlvrbrainv40).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv45 != "")
                        xmlwriter.WriteElementString("urlvrbrainv45", new Uri(software.urlvrbrainv45).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv50 != "")
                        xmlwriter.WriteElementString("urlvrbrainv50", new Uri(software.urlvrbrainv50).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrbrainv51 != "")
                        xmlwriter.WriteElementString("urlvrbrainv51", new Uri(software.urlvrbrainv51).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrherov10 != "")
                        xmlwriter.WriteElementString("urlvrherov10", new Uri(software.urlvrherov10).LocalPath.TrimStart('/', '\\'));
                    if (software.urlvrubrainv51 != "")
                        xmlwriter.WriteElementString("urlvrubrainv51", new Uri(software.urlvrubrainv51).LocalPath.TrimStart('/', '\\'));
                    xmlwriter.WriteElementString("name", software.name);
                    xmlwriter.WriteElementString("desc", software.desc);
                    xmlwriter.WriteElementString("format_version", software.k_format_version.ToString());

                    xmlwriter.WriteEndElement();

                    if (software.url != "")
                    {
                        Common.getFilefromNet(software.url, basedir + new Uri(software.url).LocalPath);
                    }
                    if (software.url2560 != "")
                    {
                        Common.getFilefromNet(software.url2560, basedir + new Uri(software.url2560).LocalPath);
                    }
                    if (software.url2560_2 != "")
                    {
                        Common.getFilefromNet(software.url2560_2, basedir + new Uri(software.url2560_2).LocalPath);
                    }
                    if (software.urlpx4v1 != "")
                    {
                        Common.getFilefromNet(software.urlpx4v1, basedir + new Uri(software.urlpx4v1).LocalPath);
                    }
                    if (software.urlpx4v2 != "")
                    {
                        Common.getFilefromNet(software.urlpx4v2, basedir + new Uri(software.urlpx4v2).LocalPath);
                    }
                    if (software.urlvrbrainv40 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv40, basedir + new Uri(software.urlvrbrainv40).LocalPath);
                    }
                    if (software.urlvrbrainv45 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv45, basedir + new Uri(software.urlvrbrainv45).LocalPath);
                    }
                    if (software.urlvrbrainv50 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv50, basedir + new Uri(software.urlvrbrainv50).LocalPath);
                    }
                    if (software.urlvrbrainv51 != "")
                    {
                        Common.getFilefromNet(software.urlvrbrainv51, basedir + new Uri(software.urlvrbrainv51).LocalPath);
                    }
                    if (software.urlvrherov10 != "")
                    {
                        Common.getFilefromNet(software.urlvrherov10, basedir + new Uri(software.urlvrherov10).LocalPath);
                    }
                    if (software.urlvrubrainv51 != "")
                    {
                        Common.getFilefromNet(software.urlvrubrainv51, basedir + new Uri(software.urlvrubrainv51).LocalPath);
                    }
                }

                xmlwriter.WriteEndElement();
                xmlwriter.WriteEndDocument();
            }
        }

        private void but_loganalysis_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                string xmlfile = MissionPlanner.Utilities.LogAnalyzer.CheckLogFile(ofd.FileName);

                if (File.Exists(xmlfile))
                {
                    var out1 = MissionPlanner.Utilities.LogAnalyzer.Results(xmlfile);

                    MissionPlanner.Controls.LogAnalyzer frm = new Controls.LogAnalyzer(out1);

                    frm.Show();
                }
                else
                {
                    CustomMessageBox.Show("Bad input file");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Warnings.WarningsManager frm = new Warnings.WarningsManager();

            frm.Show();
        }
		
        Comms.MAVLinkSerialPort comport;

        TcpListener listener;

        TcpClient client;

        private void but_mavserialport_Click(object sender, EventArgs e)
        {
            if (comport != null)
                comport.Close();

            comport = new Comms.MAVLinkSerialPort(MainV2.comPort, MAVLink.SERIAL_CONTROL_DEV.GPS1);

            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            listener = new TcpListener(IPAddress.Any,500);

            listener.Start();

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

            if (client != null && client.Connected)
                return;

            // End the operation and display the received data on  
            // the console.
            using (
            client = listener.EndAcceptTcpClient(ar))
            {
                NetworkStream stream = client.GetStream();

                if (!comport.IsOpen)
                    comport.Open();

                while (client.Connected && comport.IsOpen)
                {

                    while (stream.DataAvailable)
                    {
                        var data = new byte[4096];
                        try
                        {
                            int len = stream.Read(data, 0, data.Length);

                            comport.Write(data, 0, len);
                        }
                        catch { }
                    }

                    while (comport.BytesToRead > 0)
                    {
                        var data = new byte[4096];
                        try
                        {
                            int len = comport.Read(data, 0, data.Length);

                            stream.Write(data, 0, len);
                        }
                        catch { }
                    }

                     System.Threading.Thread.Sleep(1);
                }
            }
        }

        private void BUT_magfit2_Click(object sender, EventArgs e)
        {
            MagCalib.ProcessLog(0);
        }

        private void BUT_shptopoly_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Shape file|*.shp";
            DialogResult result = fd.ShowDialog();
            string file = fd.FileName;

            ProjectionInfo pStart = new ProjectionInfo();
            ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;
            bool reproject = false;

            if (File.Exists(file))
            {
                string prjfile = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file) + ".prj";
                if (File.Exists(prjfile))
                {

                    using (StreamReader re = File.OpenText(Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file) + ".prj"))
                    {
                        pStart.ParseEsriString(re.ReadLine());

                        reproject = true;
                    }
                }

                DotSpatial.Data.IFeatureSet fs = DotSpatial.Data.FeatureSet.Open(file);

                fs.FillAttributes();

                int rows = fs.NumRows();

                DataTable dtOriginal = fs.DataTable;
                for (int row = 0; row < dtOriginal.Rows.Count; row++)
                {
                    object[] original = dtOriginal.Rows[row].ItemArray;
                }

                foreach (DataColumn col in dtOriginal.Columns)
                {
                    Console.WriteLine(col.ColumnName + " " + col.DataType.ToString());
                }

                int a = 1;

                string path = Path.GetDirectoryName(file);

                foreach (var feature in fs.Features)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("#Shap to Poly - Mission Planner\r\n");
                    foreach (var point in feature.Coordinates)
                    {
                        if (reproject)
                        {
                            double[] xyarray = { point.X, point.Y };
                            double[] zarray = { point.Z };

                            Reproject.ReprojectPoints(xyarray, zarray, pStart, pESRIEnd, 0, 1);

                            point.X = xyarray[0];
                            point.Y = xyarray[1];
                            point.Z = zarray[0];
                        }

                        sb.Append(point.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\t" + point.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + "\r\n");
                    }

                    log.Info("writting poly to " + path + Path.DirectorySeparatorChar + "poly-" + a + ".poly");
                    File.WriteAllText(path + Path.DirectorySeparatorChar + "poly-" + a + ".poly", sb.ToString());

                    a++;
                }
            }
        }

        private void but_droneshare_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "tlog|*.tlog|*.log|*.log";
            ofd.Multiselect = true;
            ofd.ShowDialog();

            string droneshareusername = MainV2.getConfig("droneshareusername");

            InputBox.Show("Username", "Username", ref droneshareusername);

            MainV2.config["droneshareusername"] = droneshareusername;

            string dronesharepassword = MainV2.getConfig("dronesharepassword");

            if (dronesharepassword != "")
            {
                try
                {
                    // fail on bad entry
                    var crypto = new Crypto();
                    dronesharepassword = crypto.DecryptString(dronesharepassword);
                }
                catch { }
            }

            InputBox.Show("Password", "Password", ref dronesharepassword,true);

            var crypto2 = new Crypto();

            string encryptedpw = crypto2.EncryptString(dronesharepassword);

            MainV2.config["dronesharepassword"] = encryptedpw;            

            if (File.Exists(ofd.FileName)) 
            {
                foreach (var file in ofd.FileNames) 
                {
                    string viewurl = Utilities.DroneApi.droneshare.doUpload(file, droneshareusername, dronesharepassword, Guid.NewGuid().ToString(), Utilities.DroneApi.APIConstants.apiKey);

                    if (viewurl != "")
                        System.Diagnostics.Process.Start(viewurl);
                }
            }

            dronesharepassword = null;
        }

        String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private void but_gimbaltest_Click(object sender, EventArgs e)
        {
            var answer = GimbalPoint.ProjectPoint();
        }

        private void but_mntstatus_Click(object sender, EventArgs e)
        {
            MainV2.comPort.GetMountStatus();
        }

        private void but_maplogs_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = MainV2.LogDir;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Log.LogMap.MapLogs(Directory.GetFiles(fbd.SelectedPath, "*.tlog", SearchOption.AllDirectories));
            }
        }

        private void butlogindex_Click(object sender, EventArgs e)
        {
            Log.LogIndex form = new Log.LogIndex();

            form.Show();
        }

        private void but_droneapi_Click(object sender, EventArgs e)
        {
            
            string droneshareusername = MainV2.getConfig("droneshareusername");

            InputBox.Show("Username", "Username", ref droneshareusername);

            MainV2.config["droneshareusername"] = droneshareusername;

            string dronesharepassword = MainV2.getConfig("dronesharepassword");

            if (dronesharepassword != "")
            {
                try
                {
                    // fail on bad entry
                    var crypto = new Crypto();
                    dronesharepassword = crypto.DecryptString(dronesharepassword);
                }
                catch { }
            }

            InputBox.Show("Password", "Password", ref dronesharepassword,true);

            var crypto2 = new Crypto();

            string encryptedpw = crypto2.EncryptString(dronesharepassword);

            MainV2.config["dronesharepassword"] = encryptedpw;  

            Utilities.DroneApi.DroneProto dp = new Utilities.DroneApi.DroneProto();

            if (dp.connect())
            {
                if (dp.loginUser(droneshareusername, dronesharepassword))
                {
                    MAVLinkInterface mine = new MAVLinkInterface();
                    var comfile = new Comms.CommsFile();
                    mine.BaseStream = comfile;
                    mine.BaseStream.PortName = @"C:\Users\hog\Documents\apm logs\iris 6-4-14\2014-04-06 09-07-32.tlog";
                    mine.BaseStream.Open();

                    comfile.bps = 4000;

                    mine.getHeartBeat();

                    dp.setVechileId(mine.MAV.Guid, 0, mine.MAV.sysid);

                    dp.startMission();

                    while (true) {

                        byte[] packet = mine.readPacket();

                        dp.SendMavlink(packet,0);

                        
                    }

                    dp.close();

                    mine.Close();
                }
            }

        }

        private void but_terrain_Click(object sender, EventArgs e)
        {
            MainV2.comPort.Terrain.checkTerrain(MainV2.comPort.MAV.cs.HomeLocation.Lat, MainV2.comPort.MAV.cs.HomeLocation.Lng);
        }
    }
}
