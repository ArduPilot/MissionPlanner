using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using MissionPlanner.Utilities;

namespace MissionPlanner.Joystick
{
    public static class JoystickProfileManager
    {
        public const string ProfileExtension = ".joycfg";
        public const string DefaultProfileName = "Default";

        const string ProfilesDirName = "joystick_profiles";
        const string ActiveProfileSetting = "JoystickActiveProfile";
        const int MaxNameLength = 100;

        static readonly Regex AllowedNameRegex =
            new Regex(@"^[A-Za-z0-9 _\-\(\)\.]+$", RegexOptions.Compiled);

        public static string ProfilesDirectory =>
            Path.Combine(Settings.GetUserDataDirectory(), ProfilesDirName);

        public static string ActiveProfile
        {
            get
            {
                if (!Settings.Instance.ContainsKey(ActiveProfileSetting))
                    return null;
                var value = Settings.Instance[ActiveProfileSetting];
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            set
            {
                Settings.Instance[ActiveProfileSetting] = value ?? string.Empty;
            }
        }

        public static IList<string> ListProfiles()
        {
            EnsureProfilesDirectory();
            return Directory
                .GetFiles(ProfilesDirectory, "*" + ProfileExtension, SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static bool ProfileExists(string name)
        {
            ValidateName(name);
            return File.Exists(ProfilePath(name));
        }

        public static void SaveActive(JoystickBase joystick)
        {
            var name = ActiveProfile;
            if (string.IsNullOrEmpty(name))
                throw new InvalidOperationException("No active profile to save");
            SaveAs(joystick, name);
        }

        public static void SaveAs(JoystickBase joystick, string name)
        {
            if (joystick == null)
                throw new ArgumentNullException(nameof(joystick));
            ValidateName(name);
            EnsureProfilesDirectory();

            joystick.saveconfig();
            joystick.ExportConfig(ProfilePath(name));
            ActiveProfile = name;
        }

        public static void Load(JoystickBase joystick, string name)
        {
            if (joystick == null)
                throw new ArgumentNullException(nameof(joystick));
            ValidateName(name);

            var path = ProfilePath(name);
            if (!File.Exists(path))
                throw new FileNotFoundException("Profile not found: " + name, path);

            joystick.ImportConfig(path);
            joystick.loadconfig();
            ActiveProfile = name;
        }

        public static void Delete(string name)
        {
            ValidateName(name);

            var path = ProfilePath(name);
            if (File.Exists(path))
                File.Delete(path);

            if (string.Equals(name, ActiveProfile, StringComparison.OrdinalIgnoreCase))
                ActiveProfile = null;
        }

        public static void Rename(string oldName, string newName)
        {
            ValidateName(oldName);
            ValidateName(newName);

            if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
                return;

            var src = ProfilePath(oldName);
            var dst = ProfilePath(newName);

            if (!File.Exists(src))
                throw new FileNotFoundException("Profile not found: " + oldName, src);
            if (File.Exists(dst))
                throw new IOException("A profile named '" + newName + "' already exists");

            File.Move(src, dst);

            if (string.Equals(oldName, ActiveProfile, StringComparison.OrdinalIgnoreCase))
                ActiveProfile = newName;
        }

        public static void ImportFromFile(string sourcePath, string name)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("Source path required", nameof(sourcePath));
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException("Source file not found", sourcePath);
            ValidateName(name);

            EnsureProfilesDirectory();
            var dst = ProfilePath(name);
            File.Copy(sourcePath, dst, true);
        }

        public static void ExportToFile(string name, string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
                throw new ArgumentException("Target path required", nameof(targetPath));
            ValidateName(name);

            var src = ProfilePath(name);
            if (!File.Exists(src))
                throw new FileNotFoundException("Profile not found: " + name, src);

            File.Copy(src, targetPath, true);
        }

        public static void EnsureDefault()
        {
            EnsureProfilesDirectory();

            if (ListProfiles().Count > 0)
            {
                if (string.IsNullOrEmpty(ActiveProfile) || !ProfileExists(ActiveProfile))
                    ActiveProfile = ListProfiles().First();
                return;
            }

            var userDir = Settings.GetUserDataDirectory();
            var buttonFiles = Directory.GetFiles(userDir, "joystickbutton*.xml", SearchOption.TopDirectoryOnly);
            var axisFiles = Directory.GetFiles(userDir, "joystickaxis*.xml", SearchOption.TopDirectoryOnly);

            if (buttonFiles.Length == 0 && axisFiles.Length == 0)
                return;

            var dst = ProfilePath(DefaultProfileName);
            var tmp = dst + ".tmp";

            if (File.Exists(tmp))
                File.Delete(tmp);

            using (var archive = ZipFile.Open(tmp, ZipArchiveMode.Create))
            {
                foreach (var file in buttonFiles.Concat(axisFiles))
                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
            }

            if (File.Exists(dst))
                File.Replace(tmp, dst, null);
            else
                File.Move(tmp, dst);

            ActiveProfile = DefaultProfileName;
        }

        public static string ProfilePath(string name)
        {
            ValidateName(name);
            return Path.Combine(ProfilesDirectory, name + ProfileExtension);
        }

        static void EnsureProfilesDirectory()
        {
            if (!Directory.Exists(ProfilesDirectory))
                Directory.CreateDirectory(ProfilesDirectory);
        }

        static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be empty", nameof(name));
            if (name.Length > MaxNameLength)
                throw new ArgumentException("Profile name too long (max " + MaxNameLength + " characters)", nameof(name));
            if (!AllowedNameRegex.IsMatch(name))
                throw new ArgumentException("Profile name may only contain letters, digits, spaces and _-().", nameof(name));
            if (name != Path.GetFileName(name))
                throw new ArgumentException("Invalid profile name", nameof(name));
        }
    }
}
