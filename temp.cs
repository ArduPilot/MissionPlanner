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

namespace MissionPlanner
{
    public partial class temp : Form
    {
        private static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public temp()
        {
            InitializeComponent();

            //if (System.Diagnostics.Debugger.IsAttached) {
            try
            {
                Controls.OpenGLtest ogl = new Controls.OpenGLtest();

                this.Controls.Add(ogl);

                ogl.Dock = DockStyle.Fill;
            }
            catch { }
           // }

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
            MagCalib.ProcessLog(0);
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
                List<string> log = Log.BinaryLog.ReadLog(ofd.FileName);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "log|*.log";

                DialogResult res = sfd.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(sfd.OpenFile());
                    foreach (string line in log)
                    {
                        sw.Write(line);
                    }
                    sw.Close();
                }
            }
        }

        private void BUT_followleader_Click(object sender, EventArgs e)
        {
            new Swarm.FollowPathControl().Show();
        }

        private void BUT_compassmot_Click(object sender, EventArgs e)
        {
            MissionPlanner.MagMotor mot = new MissionPlanner.MagMotor();

            mot.StartCalibration();
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

                    xmlwriter.WriteElementString("url", new Uri(software.url).LocalPath.TrimStart('/','\\'));
                    xmlwriter.WriteElementString("url2560", new Uri(software.url2560).LocalPath.TrimStart('/', '\\'));
                    xmlwriter.WriteElementString("url2560-2", new Uri(software.url2560_2).LocalPath.TrimStart('/', '\\'));
                    xmlwriter.WriteElementString("urlpx4", new Uri(software.urlpx4v1).LocalPath.TrimStart('/', '\\'));
                    xmlwriter.WriteElementString("urlpx4v2", new Uri(software.urlpx4v2).LocalPath.TrimStart('/', '\\'));
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
                }

                xmlwriter.WriteEndElement();
                xmlwriter.WriteEndDocument();
            }
        }
    }
}
