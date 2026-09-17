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
        public ApiReasoningLevel ReasoningLevel { get; set; } = ApiReasoningLevel.Off;
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
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MissionPlanner", "AIWaypointPlanner", "api-profiles.xml"))
        {
        }

        public ApiProfileStore(string profileFilePath)
        {
            if (string.IsNullOrWhiteSpace(profileFilePath))
                throw new ArgumentException("A profile file path is required.", "profileFilePath");
            filePath = Path.GetFullPath(profileFilePath);
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

        public static string CredentialTargetFor(ApiProfileRecord profile)
        {
            if (profile == null)
                throw new ArgumentNullException("profile");

            return CredentialTargetForProfileConnection(
                profile.Name,
                profile.BaseUrl,
                profile.Protocol,
                profile.AuthenticationMode,
                profile.Model);
        }

        public static string CredentialTargetForProfileConnection(
            string profileName,
            string baseUrl,
            ApiProtocol protocol,
            ApiAuthenticationMode authenticationMode,
            string model)
        {
            string identity = "profile\n" +
                              (profileName ?? string.Empty).Trim().ToLowerInvariant() + "\n" +
                              BuildConnectionIdentity(baseUrl, protocol, authenticationMode, model);
            return CredentialPrefix + "Scoped." + HashIdentity(identity);
        }

        public static string CredentialTargetForEndpoint(
            string baseUrl,
            ApiProtocol protocol,
            ApiAuthenticationMode authenticationMode,
            string model)
        {
            string identity = "endpoint\n" +
                              BuildConnectionIdentity(baseUrl, protocol, authenticationMode, model);
            return CredentialPrefix + "Endpoint." + HashIdentity(identity);
        }

        public static string LegacyCredentialTargetForProfileName(string profileName)
        {
            return CredentialPrefix + HashIdentity(profileName ?? string.Empty);
        }

        private static string BuildConnectionIdentity(
            string baseUrl,
            ApiProtocol protocol,
            ApiAuthenticationMode authenticationMode,
            string model)
        {
            return NormalizeBaseUrl(baseUrl) + "\n" +
                   protocol + "\n" +
                   authenticationMode + "\n" +
                   (model ?? string.Empty).Trim();
        }

        private static string HashIdentity(string identity)
        {
            using (var sha = SHA256.Create())
            {
                byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(identity ?? string.Empty));
                return BitConverter.ToString(digest).Replace("-", string.Empty).Substring(0, 32);
            }
        }

        /// <summary>
        /// Determines whether a saved profile still describes the connection currently
        /// shown in the settings page.  A profile credential is only safe to reuse when
        /// all endpoint-affecting fields match; otherwise the caller must use an
        /// endpoint-scoped credential (or ask the operator for a new session key).
        /// </summary>
        public static bool MatchesConnection(
            ApiProfileRecord profile,
            string baseUrl,
            ApiProtocol protocol,
            ApiAuthenticationMode authenticationMode,
            string model)
        {
            if (profile == null)
                return false;

            return string.Equals(
                       NormalizeBaseUrl(profile.BaseUrl),
                       NormalizeBaseUrl(baseUrl),
                       StringComparison.Ordinal) &&
                   profile.Protocol == protocol &&
                   profile.AuthenticationMode == authenticationMode &&
                   string.Equals(
                       (profile.Model ?? string.Empty).Trim(),
                       (model ?? string.Empty).Trim(),
                       StringComparison.Ordinal);
        }

        private static string NormalizeBaseUrl(string value)
        {
            Uri uri;
            if (Uri.TryCreate((value ?? string.Empty).Trim(), UriKind.Absolute, out uri))
            {
                var builder = new UriBuilder(uri)
                {
                    Scheme = uri.Scheme.ToLowerInvariant(),
                    Host = uri.IdnHost.ToLowerInvariant(),
                    Fragment = string.Empty,
                    Query = string.Empty
                };
                if (uri.IsDefaultPort)
                    builder.Port = -1;
                return builder.Uri.AbsoluteUri.TrimEnd('/');
            }
            return (value ?? string.Empty).Trim().TrimEnd('/');
        }
        private static bool IsUsable(ApiProfileRecord profile)
        {
            return profile != null && !string.IsNullOrWhiteSpace(profile.Name) &&
                   !string.IsNullOrWhiteSpace(profile.BaseUrl) && !string.IsNullOrWhiteSpace(profile.Model);
        }
    }
}
