using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            public string type = "";
            public string encoding = "";
            public long size = 0;
            public string name { get; set; }
            public string path = "";
            public string content = "";
            public string sha = "";
            public string url = "";
            public string git_url = "";
            public string html_url = "";
            public Dictionary<string, object> _links = new Dictionary<string, object>();
        }

        static T GetObject<T>(Dictionary<string, object> dict)
        {
            Type type = typeof (T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                try
                {
                    if (type.GetField(kv.Key) != null)
                        type.GetField(kv.Key).SetValue(obj, kv.Value);
                    if (type.GetProperty(kv.Key) != null)
                        type.GetProperty(kv.Key).SetValue(obj, kv.Value, null);
                }
                catch { }
            }
            return (T) obj;
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

            WebRequest wr = WebRequest.Create(url);
            ((HttpWebRequest) wr).AllowAutoRedirect = true;
            ((HttpWebRequest) wr).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            var response = wr.GetResponse();
            var respstream = response.GetResponseStream();

            string content = new StreamReader(respstream).ReadToEnd();

            respstream.Close();

            //WebClient wc = new WebClient();
            //string content = wc.DownloadString(url);

            var output = JsonConvert.DeserializeObject<object[]>(content);

            foreach (JObject itemjobject in output)
            {
                var item = itemjobject.ToObject<Dictionary<string, object>>();
                FileInfo fi = (FileInfo) GetObject<FileInfo>(item);
                //   string t1 = item["type"].ToString();
                //   string t2 =item["path"].ToString();

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

            WebRequest wr = WebRequest.Create(url);
            ((HttpWebRequest) wr).AllowAutoRedirect = true;
            ((HttpWebRequest) wr).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            using (var response = wr.GetResponse())
            {
                var respstream = response.GetResponseStream();

                string content = new StreamReader(respstream).ReadToEnd();

                respstream.Close();

                Dictionary<string, object> output = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                if (output == null)
                    return null;

                byte[] filecontent = Convert.FromBase64String(output["content"].ToString());

                return filecontent;
            }
        }
    }
}