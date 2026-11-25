using System;
using System.IO;
using System.Xml.Serialization;

namespace MissionPlanner.AIWaypointPlanner
{
    [Serializable]
    [XmlRoot("AIWaypointPlannerPreferences")]
    public sealed class PluginPreferences
    {
        public PluginPreferences()
        {
            LanguageCode = UiStrings.DefaultLanguageCode;
        }

        public string LanguageCode { get; set; }

        public PluginPreferences Normalize()
        {
            return new PluginPreferences
            {
                LanguageCode = UiStrings.NormalizeLanguageCode(LanguageCode)
            };
        }
    }

    public sealed class PluginPreferencesStore
    {
        private readonly string filePath;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(PluginPreferences));

        public PluginPreferencesStore()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MissionPlanner",
                "AIWaypointPlanner",
                "preferences.xml"))
        {
        }

        public PluginPreferencesStore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("A preferences file path is required.", "filePath");
            this.filePath = Path.GetFullPath(filePath);
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public PluginPreferences Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return CreateDefaults();

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var preferences = serializer.Deserialize(stream) as PluginPreferences;
                    return preferences == null ? CreateDefaults() : preferences.Normalize();
                }
            }
            catch
            {
                // Missing, damaged, or inaccessible preferences must not prevent the plugin from opening.
                return CreateDefaults();
            }
        }

        public void Save(PluginPreferences preferences)
        {
            if (preferences == null)
                throw new ArgumentNullException("preferences");

            string directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidOperationException("The preferences path has no parent directory.");

            Directory.CreateDirectory(directory);
            string temporaryPath = Path.Combine(
                directory,
                Path.GetFileName(filePath) + "." + Guid.NewGuid().ToString("N") + ".tmp");

            try
            {
                using (var stream = new FileStream(
                    temporaryPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    serializer.Serialize(stream, preferences.Normalize());
                    stream.Flush(true);
                }

                if (File.Exists(filePath))
                    File.Replace(temporaryPath, filePath, null);
                else
                    File.Move(temporaryPath, filePath);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }

        public static PluginPreferences CreateDefaults()
        {
            return new PluginPreferences
            {
                LanguageCode = UiStrings.DefaultLanguageCode
            };
        }
    }
}
