using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace MissionPlanner.AIWaypointPlanner
{
    [Serializable]
    public sealed class ApiProfileRecord
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public ApiProtocol Protocol { get; set; }
        public ApiAuthenticationMode AuthenticationMode { get; set; }
        public string Model { get; set; }
        public string ProjectId { get; set; }
        public bool RememberApiKey { get; set; }
        public DateTime LastUsedUtc { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    [XmlRoot("AIWaypointPlannerApiProfiles")]
    public sealed class ApiProfileDocument
    {
        [XmlArray("Profiles")]
        [XmlArrayItem("Profile")]
        public List<ApiProfileRecord> Profiles { get; set; } = new List<ApiProfileRecord>();
    }

    public sealed class ApiProfileStore
    {
        private const string CredentialPrefix = "MissionPlanner.AIWaypointPlanner.ApiProfile.";
        private readonly string filePath;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(ApiProfileDocument));

        public ApiProfileStore()
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MissionPlanner", "AIWaypointPlanner");
            filePath = Path.Combine(directory, "api-profiles.xml");
        }

        public IList<ApiProfileRecord> Load()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<ApiProfileRecord>();

                using (var stream = File.OpenRead(filePath))
                {
                    var document = serializer.Deserialize(stream) as ApiProfileDocument;
                    return document == null || document.Profiles == null
                        ? new List<ApiProfileRecord>()
                        : document.Profiles.Where(IsUsable).ToList();
                }
            }
            catch
            {
                // A damaged preferences file must not prevent the plugin from opening.
                return new List<ApiProfileRecord>();
            }
        }

        public void Save(IEnumerable<ApiProfileRecord> profiles)
        {
            string directory = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(directory);
            string temporary = filePath + ".tmp";
            var document = new ApiProfileDocument
            {
                Profiles = profiles.Where(IsUsable).OrderByDescending(p => p.LastUsedUtc).ToList()
            };
            using (var stream = File.Create(temporary))
                serializer.Serialize(stream, document);
            if (File.Exists(filePath))
                File.Replace(temporary, filePath, null);
            else
                File.Move(temporary, filePath);
        }

        public static string CredentialTargetFor(string profileName)
        {
            using (var sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(profileName ?? string.Empty));
                return CredentialPrefix + BitConverter.ToString(digest).Replace("-", string.Empty).Substring(0, 32);
            }
        }

        private static bool IsUsable(ApiProfileRecord profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Name) &&
                   !string.IsNullOrWhiteSpace(profile.BaseUrl) && !string.IsNullOrWhiteSpace(profile.Model);
        }
    }
}
