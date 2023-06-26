using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wix
{
    public class Drivers
    {
        public static void process()
        {
            sw = new StreamWriter("drivers.wxs");

            sw.WriteLine(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\" xmlns:netfx=\"http://schemas.microsoft.com/wix/NetFxExtension\" xmlns:difx=\"http://schemas.microsoft.com/wix/DifxAppExtension\" xmlns:iis='http://schemas.microsoft.com/wix/IIsExtension' >\r\n\r\n\r\n\t<Product Id=\"*\" Name=\"UAV Drivers\" Language=\"1033\" Version=\"1.3.12\" Manufacturer=\"Michael Oborne\" UpgradeCode=\"{625389D7-EB3C-4d77-A5F6-A285CF994370}\">\r\n\r\n\t\t<Package Description=\"driver Installer\" Comments=\"driver Installer\" Manufacturer=\"Michael Oborne\" InstallerVersion=\"200\" Compressed=\"yes\" />\r\n\r\n\t\t<InstallExecuteSequence>\r\n\t\t\t<RemoveExistingProducts After=\"InstallInitialize\" />\r\n\t\t</InstallExecuteSequence>\r\n        <Upgrade Id=\"{625389D7-EB3C-4d77-A5F6-A285CF994370}\">\r\n            <UpgradeVersion OnlyDetect=\"yes\" Minimum=\"1.3.12\" Property=\"NEWERVERSIONDETECTED\" IncludeMinimum=\"no\" />\r\n            <UpgradeVersion OnlyDetect=\"no\" Minimum=\"0.0.0\" Maximum=\"1.3.12\" Property=\"OLDERVERSIONBEINGUPGRADED\" IncludeMinimum=\"yes\" IncludeMaximum=\"yes\" />\r\n        </Upgrade>\r\n\t\t<Media Id=\"1\" Cabinet=\"product.cab\" EmbedCab=\"yes\" />\r\n\r\n\t\t<Directory Id=\"TARGETDIR\" Name=\"SourceDir\">\r\n\t\t\t<Directory Id=\"ProgramFilesFolder\" Name=\"PFiles\"> \n   <Directory Id=\"driver\" Name=\"UAV Drivers\">");
            sw.WriteLine(
                "	\t\t\t\t\t<Component Id=\"drivercert\" Guid=\"625389D7-EB3C-4d77-A5F6-A285CF994371\">\r\n\t\t\t\t\t\t<RegistryValue Root=\"HKCU\" Key=\"Software\\MichaelOborne\\driver\" Name=\"installed\" Type=\"integer\" Value=\"1\" KeyPath=\"yes\" />\r\n\r\n\t\t\t\t\t\t<iis:Certificate Id=\"rootcert\" StoreLocation=\"localMachine\" StoreName=\"root\" Overwrite='yes' BinaryKey='signedcer' Request=\"no\" Name='Michael Oborne' />\r\n\t\t\t\t\t</Component>");
            dodirectory("../Drivers", 0);
            sw.WriteLine("    </Directory>\n</Directory>\n</Directory>\n\n");

            sw.WriteLine(
                "  		<Binary Id=\"signedcer\"  SourceFile=\"..\\Drivers\\signed.cer\" />   \r\n   <CustomAction  Id='Drivercleanup' Execute='deferred'      Directory='driver'  ExeCommand='[driver]DriverCleanup.exe' Return='ignore' Impersonate='no'/>  	\r\n	<CustomAction  Id='Install_signed_Driver86' Execute='deferred'    Directory='driver'  ExeCommand='[driver]DPInstx86.exe' Return='ignore' Impersonate='no'/> 	\r\n	<CustomAction  Id='Install_signed_Driver64' Execute='deferred'    Directory='driver'  ExeCommand='[driver]DPInstx64.exe' Return='ignore' Impersonate='no'/>  	\r\n	<InstallExecuteSequence> \r\n			<Custom Action=\"Install_signed_Driver86\"  After=\"CreateShortcuts\">NOT 	Installed AND NOT VersionNT64</Custom> 	\r\n		<Custom Action=\"Install_signed_Driver64\"  After=\"CreateShortcuts\">NOT 	Installed AND VersionNT64</Custom>        \r\n     <Custom Action=\"Drivercleanup\"  After=\"CreateShortcuts\"></Custom> \r\n		</InstallExecuteSequence>     \r\n 		<Feature Id=\"Complete\" Title=\"UAV Drivers\" Level=\"1\">  	\r\n	<ComponentRef Id=\"drivercert\" /> 		\r\n	");
            foreach (string comp in components)
            {
                sw.WriteLine(@"<ComponentRef Id=""" + comp + @""" />");
            }

            sw.WriteLine(
                "	 		\r\n	 		</Feature>  \r\n		<!-- Step 2: Add UI to your installer / Step 4: Trigger the custom action --> 	\r\n	<Property Id=\"WIXUI_INSTALLDIR\" Value=\"driver\" />   \r\n		<Property Id=\"ApplicationFolderName\" Value=\"driver\" /> \r\n  		<WixVariable Id=\"WixUILicenseRtf\" Value=\"licence.rtf\" />  	\r\n	<UI> 		\r\n	<UIRef Id=\"WixUI_InstallDir\" /> 	\r\n	</UI> \r\n	</Product>\r\n </Wix>");
            sw.Close();
        }

        static StreamWriter sw;
        static Hashtable dircache = new Hashtable();
        static List<string> components = new List<string>();
        static int no = 0;
        static bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        static Hashtable usedfns = new Hashtable();

        static string fixname(string name)
        {
            if (name == "")
                return name + "_1";

            name = name.Replace("-", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(".", "_");

            if (IsNumeric(name[0].ToString()))
                name = "_" + name;

            string nameorig = name;

            int a = 1;
            while (usedfns.ContainsKey(name.ToLower()))
            {
                name = nameorig + "_" + a;
                a++;
            }

            usedfns[name.ToLower()] = 1;

            return name;
        }
        static void dodirectory(string path, int level = 1)
        {
            string[] dirs = Directory.GetDirectories(path);

            string tabs = "".PadLeft(level + 1, '\t');
            string tabs2 = "".PadLeft(level + 2, '\t');
            string tabs3 = "".PadLeft(level + 3, '\t');

            if (level != 0)
            {
                if (dircache.ContainsKey(Path.GetFileName(path).Replace('-', '_')))
                {
                    sw.WriteLine(tabs + "<Directory Id=\"" + Path.GetFileName(path).Replace('-', '_') + no + "\" Name=\"" + Path.GetFileName(path) + "\">");
                }
                else
                {
                    sw.WriteLine(tabs + "<Directory Id=\"" + Path.GetFileName(path).Replace('-', '_') + "\" Name=\"" + Path.GetFileName(path) + "\">");
                }

                dircache[Path.GetFileName(path).Replace('-', '_')] = "";
            }

            string[] files = Directory.GetFiles(path);

            no++;

            string compname = fixname(Path.GetFileName(path));
            sw.WriteLine(tabs2 + "<Component Id=\"" + compname + "\" Guid=\"" + System.Guid.NewGuid().ToString() + "\">");
            components.Add(compname);

            foreach (string filepath in files)
            {
                if (filepath.ToLower().EndsWith("release\\config.xml") || filepath.ToLower().StartsWith("joystick") ||
                    filepath.ToLower().StartsWith("camera.xml") || filepath.ToLower().StartsWith("firmware.hex") || filepath.ToLower().EndsWith(".param") ||
                    filepath.ToLower().EndsWith(".bin") || filepath.ToLower().EndsWith(".etag") || filepath.ToLower().EndsWith("parametermetadata.xml") ||
                    filepath.ToLower().EndsWith(".zip") || filepath.ToLower().EndsWith(".rlog") || filepath.ToLower().Contains("stats.xml") || filepath.ToLower().Contains("beta.bat"))
                    continue;

                no++;

                
                    sw.WriteLine(tabs3 + "<File Id=\"" + fixname(Path.GetFileName(filepath)) + "\" Source=\"" + filepath + "\" />");
                
            }

            sw.WriteLine(tabs2 + "</Component>");

            foreach (string dir in dirs)
            {
                if (dir.ToLower().EndsWith("gmapcache") || dir.ToLower().EndsWith("srtm") || dir.ToLower().EndsWith("logs"))
                    continue;
                dodirectory(dir, level + 1);
            }

            if (level != 0)
                sw.WriteLine(tabs + "</Directory>");
        }

    }
}
