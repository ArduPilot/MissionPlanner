using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;

namespace wix
{
    class Program
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        public const int ERROR_SUCCESS = 0;
        /// <summary>
        /// Incorrect function.
        /// </summary>
        public const int ERROR_INVALID_FUNCTION = 1;
        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        public const int ERROR_FILE_NOT_FOUND = 2;
        /// <summary>
        /// The system cannot find the path specified.
        /// </summary>
        public const int ERROR_PATH_NOT_FOUND = 3;
        /// <summary>
        /// The system cannot open the file.
        /// </summary>
        public const int ERROR_TOO_MANY_OPEN_FILES = 4;
        /// <summary>
        /// Access is denied.
        /// </summary>
        public const int ERROR_ACCESS_DENIED = 5;

        const Int32 DRIVER_PACKAGE_REPAIR = 0x00000001;
        const Int32 DRIVER_PACKAGE_SILENT = 0x00000002;
        const Int32 DRIVER_PACKAGE_FORCE = 0x00000004;
        const Int32 DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT = 0x00000008;
        const Int32 DRIVER_PACKAGE_LEGACY_MODE = 0x00000010;
        const Int32 DRIVER_PACKAGE_DELETE_FILES = 0x00000020;

        [DllImport("DIFXApi.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 DriverPackagePreinstall(string DriverPackageInfPath, Int32 Flags);

        static void driverinstall()
        {
            int result = DriverPackagePreinstall(@"..\Drivers\Arduino MEGA 2560.inf", 0);
            if (result != 0)
                MessageBox.Show("Driver installation failed. " + result);

        }

        static int no = 0;

        static StreamWriter sw;

        static List<string> components = new List<string>();

        static Hashtable dircache = new Hashtable();

        static string mainexeid = "";

        static string basedir = "";

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Bad Directory");
                return;
            }

            if (args[0] == "driver")
            {
                driverinstall();
                return;
            }

            string path = args[0];
            basedir = path;
            //Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar+ 
            string file = "installer.wxs";

            string outputfilename = "MissionPlanner";

            if (args.Length > 1)
                outputfilename = args[1];

            sw = new StreamWriter(file);

            header();

            sw.WriteLine("<Directory Id=\"MissionPlanner\" Name=\"Mission Planner\">");

            sw.WriteLine(@"<Component Id=""InstallDirPermissions"" Guid=""{525389D7-EB3C-4d77-A5F6-A285CF99437D}"" KeyPath=""yes""> 
                        <CreateFolder> 
                            <Permission User=""Everyone"" GenericAll=""yes"" /> 
                        </CreateFolder>");
            sw.WriteLine(@"</Component>");

            //sw.WriteLine("<File Id=\"_" + no + "\" Source=\"" + file + "\" />");

            dodirectory(path, 0);


            footer(path);

            sw.Close();

            /*
            System.Diagnostics.Process P = new System.Diagnostics.Process();
            P.StartInfo.FileName = "cmd.exe";
                
            P.StartInfo.Arguments =  " /c \"" + Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "installer.bat\"";
            P.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            P.Start();
            */
            //Console.ReadLine();

            string exepath = Path.GetFullPath(path) + Path.DirectorySeparatorChar + "MissionPlanner.exe";
            string version = Assembly.LoadFile(exepath).GetName().Version.ToString();

            System.Diagnostics.FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(exepath);

            string fn = outputfilename+"-" + fvi.FileVersion;

            StreamWriter st = new StreamWriter("create.bat", false);

            st.WriteLine("del installer.wixobj");

            st.WriteLine(@"""%wix%\bin\candle"" installer.wxs -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension");

            st.WriteLine(@"""%wix%\bin\light"" installer.wixobj ""%wix%\bin\difxapp_x86.wixlib"" -o " + fn + ".msi -ext WiXNetFxExtension -ext WixDifxAppExtension -ext WixUIExtension.dll -ext WixUtilExtension -ext WixIisExtension");

            st.WriteLine(@"""C:\Program Files\7-Zip\7z.exe"" a -tzip -xr!*.log -xr!*.log* -xr!cameras.xml -xr!firmware.hex -xr!*.zip -xr!stats.xml -xr!*.bin -xr!*.xyz -xr!*.sqlite -xr!*.dxf -xr!*.zip -xr!*.h -xr!*.param -xr!ParameterMetaData.xml -xr!translation -xr!mavelous_web -xr!stats.xml -xr!driver -xr!*.etag -xr!srtm -xr!*.rlog -xr!*.zip -xr!*.tlog -xr!config.xml -xr!gmapcache -xr!eeprom.bin -xr!dataflash.bin -xr!*.new -xr!*.log -xr!ArdupilotPlanner.log* -xr!cameras.xml -xr!firmware.hex -xr!*.zip -xr!stats.xml -xr!ParameterMetaData.xml -xr!*.etag -xr!*.rlog -xr!*.tlog -xr!config.xml -xr!gmapcache -xr!eeprom.bin -xr!dataflash.bin -xr!*.new " + fn + @".zip " + path + "*");

            st.WriteLine("pause");

            //st.WriteLine("googlecode_upload.py -s \"Mission Planner zip file, " + fvi.FileVersion + "\" -p ardupilot-mega \"" + fn + @".zip""");

            //st.WriteLine("googlecode_upload.py -s \"Mission Planner installer\" -p ardupilot-mega " + fn + ".msi");

            st.WriteLine(@"c:\cygwin\bin\ln.exe -f -s " + fn + ".zip " + outputfilename + "-latest.zip");
            st.WriteLine(@"c:\cygwin\bin\ln.exe -f -s " + fn + ".msi " + outputfilename + "-latest.msi");

            st.WriteLine(@"c:\cygwin\bin\rsync.exe -Pv --password-file=/cygdrive/c/users/hog/diyrsync.txt " + fn + ".zip michael@firmware.diydrones.com::MissionPlanner/");
            st.WriteLine(@"c:\cygwin\bin\rsync.exe -Pv --password-file=/cygdrive/c/users/hog/diyrsync.txt " + fn + ".msi michael@firmware.diydrones.com::MissionPlanner/");

            st.WriteLine(@"c:\cygwin\bin\rsync.exe -Pv --password-file=/cygdrive/c/users/hog/diyrsync.txt -l MissionPlanner-latest.zip michael@firmware.diydrones.com::MissionPlanner/");
            st.WriteLine(@"c:\cygwin\bin\rsync.exe -Pv --password-file=/cygdrive/c/users/hog/diyrsync.txt -l MissionPlanner-latest.msi michael@firmware.diydrones.com::MissionPlanner/");

            st.Close();

            runProgram("create.bat");


        }

        static void runProgram(string run)
        {
            System.Diagnostics.Process P = new System.Diagnostics.Process();
            P.StartInfo.FileName = run;

//            P.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            P.StartInfo.UseShellExecute = true;
            P.Start();
        }

        static void header()
        {
            string newid = System.Guid.NewGuid().ToString();

            newid = "*";

            StreamReader sr = new StreamReader(File.OpenRead("../Properties/AssemblyInfo.cs"));

            string version = "0";

            while (!sr.EndOfStream) {
                string line = sr.ReadLine();
                if (line.Contains("AssemblyFileVersion"))
                {
                    string[] items = line.Split(new char[] { '"' },StringSplitOptions.RemoveEmptyEntries);
                    version = items[1];
                    break;
                }
            }
            sr.Close();

            string data = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Wix xmlns=""http://schemas.microsoft.com/wix/2006/wi"" xmlns:netfx=""http://schemas.microsoft.com/wix/NetFxExtension"" xmlns:difx=""http://schemas.microsoft.com/wix/DifxAppExtension"" xmlns:iis='http://schemas.microsoft.com/wix/IIsExtension' >


    <Product Id=""" + newid + @""" Name=""Mission Planner"" Language=""1033"" Version=""" + version + @""" Manufacturer=""Michael Oborne"" UpgradeCode=""{625389D7-EB3C-4d77-A5F6-A285CF99437D}"">

        <Package Description=""Mission Planner Installer"" Comments=""Mission Planner Installer"" Manufacturer=""Michael Oborne"" InstallerVersion=""200"" Compressed=""yes"" />


<Upgrade Id=""{625389D7-EB3C-4d77-A5F6-A285CF99437D}"">
    <UpgradeVersion OnlyDetect=""yes"" Minimum=""" + version + @""" Property=""NEWERVERSIONDETECTED"" IncludeMinimum=""no"" />
    <UpgradeVersion OnlyDetect=""no"" Minimum=""0.0.0"" Maximum=""" + version + @""" Property=""OLDERVERSIONBEINGUPGRADED"" IncludeMinimum=""yes"" IncludeMaximum=""yes"" />
</Upgrade>

<InstallExecuteSequence>
    <RemoveExistingProducts After=""InstallInitialize"" />
</InstallExecuteSequence>


        <PropertyRef Id=""NETFRAMEWORK40FULL"" />

        <Condition Message=""This application requires .NET Framework 4.0. Please install the .NET Framework then run this installer again.""><![CDATA[Installed OR NETFRAMEWORK40FULL]]></Condition>

        <Media Id=""1"" Cabinet=""product.cab"" EmbedCab=""yes"" />

        <Directory Id=""TARGETDIR"" Name=""SourceDir"">
            <Directory Id=""ProgramFilesFolder"" Name=""PFiles"">
                ";

            sw.WriteLine(data);
        }

        static void footer(string path)
        {

            string data = @"
                </Directory>
            </Directory>

            <Directory Id=""ProgramMenuFolder"">
                <Directory Id=""ApplicationProgramsFolder"" Name=""Mission Planner"" />
            </Directory>

        </Directory>



<Binary Id=""signedcer""  SourceFile=""..\Drivers\signed.cer"" />
  
  <CustomAction  Id='Install_signed_Driver86' Execute='deferred' 
  Directory='Drivers'  ExeCommand='[Drivers]DPInstx86.exe' Return='ignore' Impersonate='no'/>
  <CustomAction  Id='Install_signed_Driver64' Execute='deferred' 
  Directory='Drivers'  ExeCommand='[Drivers]DPInstx64.exe' Return='ignore' Impersonate='no'/>

 <InstallExecuteSequence>
    <Custom Action=""Install_signed_Driver86""  After=""CreateShortcuts"">NOT 
	Installed AND NOT VersionNT64</Custom>
    <Custom Action=""Install_signed_Driver64""  After=""CreateShortcuts"">NOT 
	Installed AND VersionNT64</Custom>
  </InstallExecuteSequence>


        <DirectoryRef Id=""ApplicationProgramsFolder"">
            <Component Id=""ApplicationShortcut"" Guid=""*"">
                <Shortcut Id=""ApplicationStartMenuShortcut10"" Name=""Mission Planner"" Description=""Mission Planner"" Target=""[MissionPlanner]MissionPlanner.exe"" WorkingDirectory=""MissionPlanner"" />
                <Shortcut Id=""UninstallProduct"" Name=""Uninstall Mission Planner"" Description=""Uninstalls My Application"" Target=""[System64Folder]msiexec.exe"" Arguments=""/x [ProductCode]"" />
                <RegistryValue Root=""HKCU"" Key=""Software\MichaelOborne\MissionPlanner"" Name=""installed"" Type=""integer"" Value=""1"" KeyPath=""yes"" />

                <RemoveFolder Id=""dltApplicationProgramsFolder"" Directory=""ApplicationProgramsFolder"" On=""uninstall"" />

                <iis:Certificate Id=""rootcert"" StoreLocation=""localMachine"" StoreName=""root"" Overwrite='yes' BinaryKey='signedcer' Request=""no"" Name='Michael Oborne' />
            </Component>
        </DirectoryRef>


        <Feature Id=""Complete"" Title=""Mission Planner"" Level=""1"">
            <ComponentRef Id=""InstallDirPermissions"" />
";
            sw.WriteLine(data);

            foreach (string comp in components)
            {
                sw.WriteLine(@"<ComponentRef Id="""+comp+@""" />");
            }

data = @"
            
            <ComponentRef Id=""ApplicationShortcut"" />
        </Feature>
        
            <!-- Step 2: Add UI to your installer / Step 4: Trigger the custom action -->
    <Property Id=""WIXUI_INSTALLDIR"" Value=""MissionPlanner"" />

<Property Id=""ApplicationFolderName"" Value=""MissionPlanner"" /> 

<WixVariable Id=""WixUILicenseRtf"" Value=""licence.rtf"" />

    <UI>
        <UIRef Id=""WixUI_InstallDir"" />
        <Publish Dialog=""ExitDialog"" 
            Control=""Finish"" 
            Event=""DoAction"" 
            Value=""LaunchApplication"">WIXUI_EXITDIALOGOPTIONALCHECKBOX = 1 and NOT Installed</Publish>
    </UI>
    <Property Id=""WIXUI_EXITDIALOGOPTIONALCHECKBOXTEXT"" Value=""Launch Mission Planner"" />

    <!-- Step 3: Include the custom action -->
    <Property Id=""WixShellExecTarget"" Value=""[#" + mainexeid + @"]"" />
    <CustomAction Id=""LaunchApplication"" 
        BinaryKey=""WixCA"" 
        DllEntry=""WixShellExec""
        Impersonate=""yes"" />
    </Product>
    
</Wix>";

            sw.WriteLine(data);
        }

        static void dodirectory(string path, int level = 1)
        {
            string[] dirs = Directory.GetDirectories(path);

            if (level != 0)
            {
                if (dircache.ContainsKey(Path.GetFileName(path).Replace('-', '_')))
                {
                    sw.WriteLine("<Directory Id=\"" + Path.GetFileName(path).Replace('-', '_') + no + "\" Name=\"" + Path.GetFileName(path) + "\">");
                }
                else
                {
                    sw.WriteLine("<Directory Id=\"" + Path.GetFileName(path).Replace('-', '_') + "\" Name=\"" + Path.GetFileName(path) + "\">");
                }

                dircache[Path.GetFileName(path).Replace('-', '_')] = "";
            }

            string[] files = Directory.GetFiles(path);

            no++;

            sw.WriteLine("<Component Id=\""+fixname(Path.GetFileName(path)) +"_"+no+"\" Guid=\""+ System.Guid.NewGuid().ToString() +"\">");
            components.Add(fixname(Path.GetFileName(path)) + "_" + no);

            foreach (string filepath in files)
            {
                if (filepath.ToLower().EndsWith("release\\config.xml") || filepath.ToLower().Contains(".log") || filepath.ToLower().StartsWith("joystick") ||
                    filepath.ToLower().StartsWith("camera.xml") || filepath.ToLower().StartsWith("firmware.hex") || filepath.ToLower().EndsWith(".param") ||
                    filepath.ToLower().EndsWith(".bin") || filepath.ToLower().EndsWith(".etag") || filepath.ToLower().EndsWith("parametermetadata.xml") ||
                    filepath.ToLower().EndsWith(".zip") || filepath.ToLower().EndsWith(".rlog") || filepath.ToLower().Contains("stats.xml"))
                    continue;

                no++;
                

                if (filepath.EndsWith("MissionPlanner.exe")) {
                    mainexeid = "_" + no;

                    sw.WriteLine("<File Id=\"_" + no + "\" Source=\"" + filepath + "\" ><netfx:NativeImage Id=\"ngen_MissionPlannerexe\"/> </File>");

                    sw.WriteLine(@"<ProgId Id='MissionPlanner.tlog' Description='Telemetry Log'>
  <Extension Id='tlog' ContentType='application/tlog'>
     <Verb Id='open' Command='Open' TargetFile='"+mainexeid+@"' Argument='""%1""' />
  </Extension>
</ProgId>");

                } else {
                    sw.WriteLine("<File Id=\"_" + fixname(Path.GetFileName(filepath))+ "_" + no + "\" Source=\"" + filepath + "\" />");
                }
            }

            // put placeholder into dir
            if (files.Length == 0)
            {
                File.WriteAllText(basedir + Path.DirectorySeparatorChar + "aircraft/placeholder.txt", "");
                sw.WriteLine("<File Id=\"_placeholder_" + no + "\" Source=\"" + basedir + Path.DirectorySeparatorChar + "aircraft/placeholder.txt" + "\" />");
                no++;
            }

            sw.WriteLine("</Component>");

            foreach (string dir in dirs)
            {
                if (dir.ToLower().EndsWith("gmapcache") || dir.ToLower().EndsWith("srtm") || dir.ToLower().EndsWith("logs"))
                    continue;
                dodirectory(dir);
            }

            if (level != 0)
                sw.WriteLine("</Directory>");
        }

        static string fixname(string name)
        {
            name = name.Replace("-", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(".", "_");

            return name;
        }
    }
}
