﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class GitHubContent
    {
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
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                if (type.GetField(kv.Key) != null)
                    type.GetField(kv.Key).SetValue(obj, kv.Value);
                if (type.GetProperty(kv.Key) != null)
                    type.GetProperty(kv.Key).SetValue(obj, kv.Value,null);
            }
            return (T)obj;
        }

        public static List<FileInfo> GetDirContent(string owner, string repo, string path)
        {
            if (path != "") {
                path = "/contents" + path;
            }

            List<FileInfo> answer = new List<FileInfo>();

            string url = String.Format("{0}/{1}/{2}{3}",githubapiurl, owner, repo, path);

            WebClient wc = new WebClient();
            string content = wc.DownloadString(url);

            var output = fastJSON.JSON.Instance.ToObject<object[]>(content);

            foreach (Dictionary<string,object> item in output) 
            {
                FileInfo fi = (FileInfo)GetObject<FileInfo>(item);
             //   string t1 = item["type"].ToString();
             //   string t2 =item["path"].ToString();
                answer.Add(fi);
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

            WebClient wc = new WebClient();
            string content = wc.DownloadString(url);

            Dictionary<string,object> output = (Dictionary<string,object>)fastJSON.JSON.Instance.Parse(content);

            byte[] filecontent = Convert.FromBase64String(output["content"].ToString());

            return filecontent;
        }

        
    }
}
