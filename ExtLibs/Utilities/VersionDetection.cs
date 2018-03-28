using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;

namespace MissionPlanner.Utilities
{
    public class VersionDetection
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Version GetVersion(string input)
        {
            Regex versionregex = new Regex(@"([0-9]+)\.([0-9]+)(\.([0-9]+)|-rc([0-9]+)|([a-z]{2,20})|([a-z]))*");

            //            string ans = mat1.Groups[1].Value.ToString() + mat1.Groups[2].Value.ToString() + mat1.Groups[4].Value.ToString() + mat1.Groups[5].Value.ToString() + mat1.Groups[6].Value.ToString();

            var match = versionregex.Match(input);

            log.Info(input);

            if (match.Success)
            {
                // start with major.monor
                string verstring = match.Groups[1].Value.ToString() + "." + match.Groups[2].Value.ToString();

                if (!String.IsNullOrEmpty(match.Groups[4].Value))
                {
                    verstring += "." + match.Groups[4].Value.ToString();
                }
                if (!String.IsNullOrEmpty(match.Groups[5].Value))
                {
                    // -rc
                    verstring += "." + match.Groups[5].Value.ToString();
                }
                if (!String.IsNullOrEmpty(match.Groups[6].Value))
                {
                    // dev
                    verstring += ".255";
                }
                if (!String.IsNullOrEmpty(match.Groups[7].Value))
                {
                    // convert a to 1, b to 2, etc, it will break at j
                    verstring += "." + (char) ((match.Groups[6].Value.ToString().ToLower()[0]) - 0x30);
                }

                Version version = new Version(verstring);

                log.Info(version.ToString());

                return version;
            }

            throw new Exception("Bad Version");
        }
    }
}