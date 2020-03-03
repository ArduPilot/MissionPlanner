﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public class APFirmware
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public class FirmwareInfo
        {
            [JsonProperty("board_id", NullValueHandling = NullValueHandling.Ignore)]
            public long BoardId { get; set; }

            [JsonProperty("mav-type")] public string MavType { get; set; }

            [JsonProperty("mav-firmware-version-minor", NullValueHandling = NullValueHandling.Ignore)]
            public long MavFirmwareVersionMinor { get; set; }

            [JsonProperty("format")] public string Format { get; set; }

            [JsonProperty("url")] public Uri Url { get; set; }

            [JsonProperty("mav-firmware-version-type")]
            public string MavFirmwareVersionType { get; set; }

            [JsonProperty("mav-firmware-version-patch", NullValueHandling = NullValueHandling.Ignore)]
            public long MavFirmwareVersionPatch { get; set; }

            [JsonProperty("mav-autopilot")] public string MavAutopilot { get; set; }

            [JsonProperty("vehicletype")] public string VehicleType { get; set; }

            [JsonProperty("USBID", NullValueHandling = NullValueHandling.Ignore)]
            public string[] Usbid { get; set; } = new string[0];

            [JsonProperty("platform")] public string Platform { get; set; }

            [JsonProperty("mav-firmware-version", NullValueHandling = NullValueHandling.Ignore)]
            public Version MavFirmwareVersion { get; set; }

            [JsonProperty("bootloader_str", NullValueHandling = NullValueHandling.Ignore)]
            public string[] BootloaderStr { get; set; } = new string[0];

            [JsonProperty("git-sha")] public string GitSha { get; set; }

            [JsonProperty("mav-firmware-version-major", NullValueHandling = NullValueHandling.Ignore)]
            public long MavFirmwareVersionMajor { get; set; }

            [JsonProperty("latest")] public long Latest { get; set; }
        }

        public class ManifestRoot
        {
            [JsonProperty("firmware")] public FirmwareInfo[] Firmware { get; set; }

            [JsonProperty("format-version")] public Version FormatVersion { get; set; }
        }

        // from generate_manifest.py - with map
        public enum RELEASE_TYPES
        {
            BETA, // beta
            DEV, // latest
            OFFICIAL // stable
        }

        public enum MAV_TYPE
        {
            ANTENNA_TRACKER,
            Copter,
            HELICOPTER,
            FIXED_WING,
            GROUND_ROVER,
            SUBMARINE
        }

        public static void GetList(string url = "https://firmware.ardupilot.org/manifest.json.gz", bool force = false)
        {
            if (force == false && Manifest != null)
                return;

            log.Info(url);

            var client = new HttpClient();

            if (!String.IsNullOrEmpty(Settings.Instance.UserAgent))
                client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);

            var manifestgz = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
            var mssrc = new MemoryStream(manifestgz);
            var msdest = new MemoryStream();
            GZipStream gz = new GZipStream(mssrc, CompressionMode.Decompress);
            gz.CopyTo(msdest);
            msdest.Position = 0;
            var manifest = new StreamReader(msdest).ReadToEnd();
            
            Manifest = JsonConvert.DeserializeObject<ManifestRoot>(manifest);

            log.Info(Manifest.Firmware?.Length);
        }

        public static ManifestRoot Manifest { get; set; }

        public static long? GetBoardID(DeviceInfo device)
        {
            GetList();

            // match the board description
            var ans = Manifest.Firmware.Where(a => (
                                                       a.Platform == device.board ||
                                                       a.BootloaderStr.Any(b => b == device.board)) && a.BoardId != 0);

            if (ans.Any())
            {
                return ans.First().BoardId;
            }

            if (device.hardwareid == null)
                return null;

            // match the vid/pid
            Regex vidpid = new Regex("VID_([0-9a-f]+)&PID_([0-9a-f]+)&", RegexOptions.IgnoreCase);

            if (vidpid.IsMatch(device.hardwareid))
            {
                var match = vidpid.Match(device.hardwareid);
                var lookfor = String.Format("0x{0}/0x{1}", match.Groups[1].Value, match.Groups[2].Value);

                var vidandusbdesc = Manifest.Firmware.Where(a => a.Usbid.Any(b => b.ToLower().Contains(lookfor.ToLower())) && a.BoardId != 0);

                if (vidandusbdesc.Any())
                {
                    return vidandusbdesc.First().BoardId;
                }
            }

            return null;
        }

        public static List<FirmwareInfo> GetOptions(DeviceInfo device, RELEASE_TYPES? reltype = null, MAV_TYPE? mav_type = null)
        {
            GetList();

            // match the board description
            var ans = Manifest.Firmware.Where(a =>
                a.Platform == device.board || a.BootloaderStr.Any(b => b == device.board));

            // ignore platform
            ans = Manifest.Firmware;

            if (reltype.HasValue)
                ans = ans.Where(a => a.MavFirmwareVersionType == reltype.Value.ToString());

            if (mav_type.HasValue)
                ans = ans.Where(a => a.MavType == mav_type.Value.ToString());

            // "0x26AC/0x0011"
            //USB\VID_2DAE&PID_1011&REV_0200

            // match the vid/pid
            Regex vidpid = new Regex("VID_([0-9a-f]+)&PID_([0-9a-f]+)&", RegexOptions.IgnoreCase);

            if (vidpid.IsMatch(device.hardwareid))
            {
                var match = vidpid.Match(device.hardwareid);
                var lookfor = String.Format("0x{0}/0x{1}", match.Groups[1].Value, match.Groups[2].Value);

                var vidandusbdesc = ans.Where(a => a.Usbid.Any(b => b.ToLower().Contains(lookfor.ToLower())));

                if (vidandusbdesc.Count() == 0)
                    return ans.ToList();

                return vidandusbdesc.ToList();
            }

            return ans.ToList();
        }

        public static List<FirmwareInfo> GetRelease(RELEASE_TYPES reltype)
        {
            GetList();

            var ans = Manifest.Firmware.Where(a => a.MavFirmwareVersionType == reltype.ToString());

            ans = ans.GroupBy(b => b.MavType)
                .SelectMany(a =>
                    a.Where(b => a.Key == b.MavType && b.MavFirmwareVersion == a.Max(c => c.MavFirmwareVersion)).OrderBy(b=>b.Format));
            /*
                         ans = ans.GroupBy(b => b.MavType).Select(a =>
                a.Where(b => a.Key == b.MavType && b.MavFirmwareVersion == a.Max(c => c.MavFirmwareVersion))
                    .FirstOrDefault());
             */

            return ans.ToList();
        }

        public static void test()
        {
            GetList();

            var cb = Manifest.Firmware.Where(a => a.Platform == "CubeBlack")
                .Where(a => a.MavFirmwareVersionType == "OFFICIAL");

            var vid = Manifest.Firmware.Where(a => a.Usbid.Any(b => b.ToLower().Contains("ac")))
                .Where(a => a.MavFirmwareVersionType == "OFFICIAL");

            var bl = Manifest.Firmware.Where(a => a.BootloaderStr.Any(b => b.ToLower().Contains("cubeblack")))
                .Where(a => a.MavFirmwareVersionType == "OFFICIAL");

            var bid = Manifest.Firmware.Where(a => a.BoardId == 9)
                .Where(a => a.MavFirmwareVersionType == "OFFICIAL");


            var vidandusbdesc = Manifest.Firmware
                .Where(a => a.Usbid.Any(b => b.ToLower().Contains("26ac")))
                .Where(a => a.BootloaderStr.Any(c => c.ToLower().Contains("fmuv3") || c.ToLower().Contains("fmuv3-bl")))
                .Where(a => a.MavFirmwareVersionType == "OFFICIAL");

            var vidandusbdesc1 = Manifest.Firmware
                .Where(a => a.Usbid.Any(b => b.ToLower().Contains("26ac")))
                .Where(a => a.BootloaderStr.Any(c => c.ToLower().Contains("fmuv3") || c.ToLower().Contains("fmuv3-bl")))
                .Where(a => a.MavFirmwareVersionType == "DEV");

            var vidandusbdesc2 = Manifest.Firmware
                .Where(a => a.Usbid.Any(b => b.ToLower().Contains("26ac")))
                .Where(a => a.BootloaderStr.Any(c => c.ToLower().Contains("fmuv3") || c.ToLower().Contains("fmuv3-bl")))
                .Where(a => a.MavFirmwareVersionType == "BETA");
        }
        
    }
}
