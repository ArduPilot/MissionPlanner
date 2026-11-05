using MissionPlanner.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    public static class L10NU
    {
        /// <summary>一次性发布的只读快照，保证语言和字典始终成对可见。</summary>
        private sealed class LangSet
        {
            public string Lang;
            public Dictionary<string, string> Map;
        }

        private static readonly object sync = new object();
        private static volatile LangSet current;

        /// <summary>
        /// 返回当前 UI 语言对应的快照。字典在局部变量里构建完成后才通过单次引用
        /// 赋值整体发布，避免并发调用读到只填充了一半的字典而返回未翻译的原文。
        /// </summary>
        private static LangSet GetCurrent()
        {
            var lang = System.Globalization.CultureInfo.CurrentUICulture.Name;

            var snapshot = current;
            if (snapshot != null && snapshot.Lang == lang)
                return snapshot;

            lock (sync)
            {
                snapshot = current;
                if (snapshot != null && snapshot.Lang == lang)
                    return snapshot;

                var map = new Dictionary<string, string>();
                string[] lines = null;

                switch (lang)
                {
                    case "zh-CN":
                    case "zh-Hans":
                        lines = Regex.Split(Resources.strings_zhHans, "\r\n|\r|\n");
                        break;
                    case "uk":
                    case "uk-UA":
                        lines = Regex.Split(Resources.strings_uk, "\r\n|\r|\n");
                        break;
                }

                if (lines != null)
                {
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        string[] kv = Regex.Split(line, "=");
                        if (kv.Length == 2)
                        {
                            map[kv[0]] = kv[1];
                        }
                    }
                }

                snapshot = new LangSet { Lang = lang, Map = map };
                current = snapshot;
                return snapshot;
            }
        }

        public static string GetString(string key, string defaultTo)
        {
            var snapshot = GetCurrent();
            string value;
            if (snapshot.Map.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultTo;
        }

        public static string GetString(string key)
        {
            return GetString(key, defaultTo: key);
        }
    }
}
