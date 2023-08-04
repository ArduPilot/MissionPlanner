using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissionPlanner.Utilities
{
    public class GitHubContent
    {
        internal static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //http://developer.github.com/v3/repos/contents/#get-contents
        //GET /repos/:owner/:repo/contents/:path
        public static string githubapiurl = "https://api.github.com/repos";

        //https://github.com/diydrones/ardupilot/tree/master/Tools/Frame_params
        //https://api.github.com/repos/octokit/octokit.rb

        public class FileInfo
        {
            public TypeEnum type;
            public string encoding = "";
            public long size = 0;
            public string name { get; set; }
            public string path = "";
            public string content = "";
            public string sha = "";
            public string url = "";
            public string git_url = "";
            public string html_url = "";
            public Links _links { get; set; }

            public partial class Links
            {
                public Uri Self { get; set; }
                public Uri Git { get; set; }
                public Uri Html { get; set; }
            }

            public enum TypeEnum { Dir, File };
        }

        public static List<FileInfo> GetDirContent(string owner, string repo, string path, string filter = "")
        {
            if (path != "")
            {
                path = "/contents" + path;
            }

            path = path.TrimEnd('/', '\\');

            List<FileInfo> answer = new List<FileInfo>();

            string url = String.Format("{0}/{1}/{2}{3}", githubapiurl, owner, repo, path);

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
            string content = client.GetStringAsync(url).GetAwaiter().GetResult();

            var output = JsonConvert.DeserializeObject<FileInfo[]>(content);

            foreach (var fi in output)
            {
                if (fi.name.ToLower().Contains(filter.ToLower()))
                {
                    answer.Add(fi);
                }
                log.Info(fi.name);
            }

            return answer;
        }

        public static byte[] GetFileContent(string owner, string repo, string path)
        {
            if (path != "")
            {
                path = "/contents" + path;
            }

            string url = String.Format("{0}/{1}/{2}{3}", githubapiurl, owner, repo, path);

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
            string content = client.GetStringAsync(url).GetAwaiter().GetResult();

            Dictionary<string, object> output = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

            if (output == null)
                return null;

            byte[] filecontent = Convert.FromBase64String(output["content"].ToString());

            return filecontent;
        }
    }
}