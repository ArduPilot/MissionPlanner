using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum AppLanguage
    {
        English,
        ChineseSimplified,
        Russian
    }

    public sealed class UiLanguageOption
    {
        public UiLanguageOption(AppLanguage language, string languageCode, string displayName)
        {
            Language = language;
            LanguageCode = languageCode;
            DisplayName = displayName;
        }

        public AppLanguage Language { get; private set; }
        public string LanguageCode { get; private set; }
        public string DisplayName { get; private set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public static class UiStrings
    {
        public const string DefaultLanguageCode = "en-US";
        public const string ChineseLanguageCode = "zh-CN";
        public const string RussianLanguageCode = "ru-RU";

        private static readonly IDictionary<string, string> English = LoadCatalog(DefaultLanguageCode);
        private static readonly IDictionary<string, string> Chinese = LoadCatalog(ChineseLanguageCode);
        private static readonly IDictionary<string, string> Russian = LoadCatalog(RussianLanguageCode);

        public static string Get(string languageCode, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            string value;
            IDictionary<string, string> selected = GetCatalog(languageCode);
            if (selected.TryGetValue(key, out value))
                return value;
            if (English.TryGetValue(key, out value))
                return value;
            return key;
        }

        public static string Format(string languageCode, string key, params object[] arguments)
        {
            return string.Format(
                GetCulture(languageCode),
                Get(languageCode, key),
                arguments ?? new object[0]);
        }

        public static string NormalizeLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return DefaultLanguageCode;

            string value = languageCode.Trim();
            if (value.Equals(ChineseLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                value.Equals("zh", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("zh-SG", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("zh-CHS", StringComparison.OrdinalIgnoreCase))
            {
                return ChineseLanguageCode;
            }
            if (value.Equals(RussianLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                value.Equals("ru", StringComparison.OrdinalIgnoreCase))
            {
                return RussianLanguageCode;
            }
            if (value.Equals(DefaultLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                value.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                return DefaultLanguageCode;
            }
            return DefaultLanguageCode;
        }

        public static AppLanguage ParseLanguage(string languageCode)
        {
            string normalized = NormalizeLanguageCode(languageCode);
            if (normalized == ChineseLanguageCode)
                return AppLanguage.ChineseSimplified;
            if (normalized == RussianLanguageCode)
                return AppLanguage.Russian;
            return AppLanguage.English;
        }

        public static IList<UiLanguageOption> CreateLanguageOptions()
        {
            return new List<UiLanguageOption>
            {
                new UiLanguageOption(AppLanguage.English, DefaultLanguageCode, "English"),
                new UiLanguageOption(AppLanguage.ChineseSimplified, ChineseLanguageCode, "简体中文"),
                new UiLanguageOption(AppLanguage.Russian, RussianLanguageCode, "Русский")
            };
        }

        public static IEnumerable<string> GetEnglishKeys()
        {
            return new List<string>(English.Keys);
        }

        public static IEnumerable<string> GetCatalogKeys(string languageCode)
        {
            return new List<string>(GetCatalog(languageCode).Keys);
        }

        private static IDictionary<string, string> GetCatalog(string languageCode)
        {
            string normalized = NormalizeLanguageCode(languageCode);
            if (normalized == ChineseLanguageCode)
                return Chinese;
            if (normalized == RussianLanguageCode)
                return Russian;
            return English;
        }

        private static CultureInfo GetCulture(string languageCode)
        {
            return CultureInfo.GetCultureInfo(NormalizeLanguageCode(languageCode));
        }

        private static IDictionary<string, string> LoadCatalog(string languageCode)
        {
            string resourceName = "MissionPlanner.AIWaypointPlanner.Localization." + languageCode + ".json";
            using (Stream stream = typeof(UiStrings).Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Missing embedded language catalog: " + resourceName);

                using (var reader = new StreamReader(stream))
                {
                    var serializer = new JavaScriptSerializer { MaxJsonLength = 256 * 1024 };
                    var values = serializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
                    if (values == null || values.Count == 0)
                        throw new InvalidOperationException("Empty embedded language catalog: " + resourceName);

                    var catalog = new Dictionary<string, string>(values, StringComparer.Ordinal);
                    // Keep the release identifier in code; translators only position its placeholder.
                    catalog["App.Title"] = catalog["App.Title"].Replace("{version}", PluginIdentity.Version);
                    return catalog;
                }
            }
        }
    }
}
