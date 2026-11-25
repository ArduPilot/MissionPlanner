using log4net;
using MissionPlanner.Utilities.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Friendly zh-Hans translation of firmware STATUSTEXT messages.
    /// Display layer only - the raw text always stays in the log, anything
    /// unmatched (or when the ui culture is not chinese) is returned unchanged.
    /// </summary>
    public static class StatusTextTranslator
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // AP_Arming emits the same check failure as "PreArm: X" / "Pre-Arm: X" / "Arm: X"
        private static readonly string[] armingPrefixes = { "PreArm:", "Pre-Arm:", "Arm:" };

        private static readonly object loadlock = new object();
        private static volatile bool loaded;
        private static Dictionary<string, string> exactmap;
        private static List<(Regex regex, string template)> regexlist;

        /// <summary>
        /// translation is only applied when the ui culture is chinese
        /// </summary>
        private static bool Enabled
        {
            get
            {
                // Settings["language"] is the user's explicit pick and is updated live by
                // MainV2.changelanguage(); the rx thread's CurrentUICulture is captured at
                // SerialReader start and goes stale on a mid-session language switch, which
                // would delay translated STATUSTEXT until reconnect. Prefer the live setting
                // and fall back to the thread culture when the user never picked a language.
                var lang = Settings.Instance["language"]?.ToString();
                if (!string.IsNullOrEmpty(lang))
                    return lang == "zh-CN" || lang == "zh-Hans";

                switch (CultureInfo.CurrentUICulture.Name)
                {
                    case "zh-CN":
                    case "zh-Hans":
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static string Translate(string raw)
        {
            try
            {
                if (string.IsNullOrEmpty(raw))
                    return raw;

                if (!Enabled)
                    return raw;

                EnsureLoaded();

                if (exactmap == null)
                    return raw;

                var hit = Lookup(raw);
                if (hit != null)
                    return hit;

                // the same arming check failure is sent with different prefix tokens,
                // retry the other prefixes and put the raw prefix back in the display form
                foreach (var prefix in armingPrefixes)
                {
                    if (!raw.StartsWith(prefix, StringComparison.Ordinal))
                        continue;

                    var body = raw.Substring(prefix.Length);

                    foreach (var other in armingPrefixes)
                    {
                        if (other == prefix)
                            continue;

                        hit = Lookup(other + body);
                        if (hit == null)
                            continue;

                        foreach (var token in armingPrefixes)
                        {
                            if (hit.StartsWith(token, StringComparison.Ordinal))
                                return prefix + hit.Substring(token.Length);
                        }

                        return hit;
                    }
                }

                return raw;
            }
            catch (Exception ex)
            {
                log.Error("STATUSTEXT translation failed", ex);
                return raw;
            }
        }

        private static void EnsureLoaded()
        {
            if (loaded)
                return;

            lock (loadlock)
            {
                if (loaded)
                    return;

                try
                {
                    var exact = new Dictionary<string, string>();
                    var regexes = new List<(Regex regex, string template)>();

                    var array = JArray.Parse(Resources.statustext_zhHans);

                    foreach (var item in array)
                    {
                        var match = item.Value<JToken>("match");
                        var value = match?.Value<string>("value");
                        var zh = item.Value<string>("zh");
                        var type = match?.Value<string>("type");

                        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value) ||
                            string.IsNullOrEmpty(zh))
                            continue;

                        if (type == "exact")
                            exact[value] = zh;
                        else if (type == "regex")
                            regexes.Add((new Regex(value, RegexOptions.Compiled | RegexOptions.CultureInvariant), zh));
                    }

                    // published only once complete, reads are lock free afterwards
                    exactmap = exact;
                    regexlist = regexes;

                    log.Info("STATUSTEXT translation table loaded: " + exactmap.Count + " exact, " + regexlist.Count + " regex");
                }
                catch (Exception ex)
                {
                    // keep the tables null so every lookup passes the raw text through
                    exactmap = null;
                    regexlist = null;
                    log.Error("STATUSTEXT translation table load failed, translation disabled", ex);
                }

                loaded = true;
            }
        }

        private static string Lookup(string text)
        {
            string zh;
            if (exactmap.TryGetValue(text, out zh))
                return zh;

            foreach (var entry in regexlist)
            {
                var m = entry.regex.Match(text);
                if (m.Success)
                    return ReplacePlaceholders(entry.template, m);
            }

            return null;
        }

        // the corpus template references capture groups as {0} for group 1, {1} for group 2...
        private static string ReplacePlaceholders(string template, Match m)
        {
            return Regex.Replace(template, @"\{(\d+)\}", g =>
            {
                var index = int.Parse(g.Groups[1].Value) + 1;
                return index < m.Groups.Count ? m.Groups[index].Value : g.Value;
            });
        }
    }
}
