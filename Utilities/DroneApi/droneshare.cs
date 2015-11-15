using fastJSON;
using log4net;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;
using MissionPlanner.Utilities.DroneApi.UI;

namespace MissionPlanner.Utilities.DroneApi
{
    public class droneshare
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static bool validcred = false;
        private static string uuidcached = "";
        private static Dictionary<string, string> accountuuids = new Dictionary<string, string>();

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return string.Join("&", array);
        }

        public static void doUserAndPassword()
        {
            string droneshareusername = MainV2.getConfig("droneshareusername");

            InputBox.Show("Username", "Username", ref droneshareusername);

            MainV2.config["droneshareusername"] = droneshareusername;

            string dronesharepassword = MainV2.getConfig("dronesharepassword");

            if (dronesharepassword != "")
            {
                try
                {
                    // fail on bad entry
                    var crypto = new Crypto();
                    dronesharepassword = crypto.DecryptString(dronesharepassword);
                }
                catch
                {
                }
            }

            InputBox.Show("Password", "Password", ref dronesharepassword, true);

            var crypto2 = new Crypto();

            string encryptedpw = crypto2.EncryptString(dronesharepassword);

            MainV2.config["dronesharepassword"] = encryptedpw;
        }

        public static void doUpload(string file)
        {
            if (file == null)
                return;

            if (!validcred)
            {
                doUserAndPassword();
            }

            string droneshareusername = MainV2.getConfig("droneshareusername");

            string dronesharepassword = MainV2.getConfig("dronesharepassword");

            if (dronesharepassword != "")
            {
                try
                {
                    // fail on bad entry
                    var crypto = new Crypto();
                    dronesharepassword = crypto.DecryptString(dronesharepassword);
                }
                catch
                {
                }
            }


            String tempguid = null;

            if (!doLogin(droneshareusername, dronesharepassword))
            {
                CustomMessageBox.Show("Bad Credentials", Strings.ERROR);
                return;
            }

            validcred = true;

            if (uuidcached == "")
            {
                UI.VehicleSelection veh = new VehicleSelection(accountuuids);
                veh.ShowDialog();

                tempguid = veh.uuid;
                uuidcached = veh.uuid;
            }
            else
            {
                tempguid = uuidcached;
            }

            string viewurl = Utilities.DroneApi.droneshare.doUpload(file, droneshareusername, dronesharepassword,
                tempguid);

            if (viewurl != "")
            {
                try
                {
                    System.Diagnostics.Process.Start(viewurl);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show("Failed to open url " + viewurl);
                }
            }
        }

        public static bool doLogin(string userId, string userPass)
        {
            String baseUrl = APIConstants.URL_BASE;
            NameValueCollection @params = new NameValueCollection();
            @params.Add("api_key", APIConstants.apiKey);
            @params.Add("login", userId);
            @params.Add("password", userPass);

            String queryParams = ToQueryString(@params);
            String webAppUploadUrl = String.Format("{0}/api/v1/auth/login", baseUrl);

            var request = (HttpWebRequest) WebRequest.Create(webAppUploadUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = queryParams.Length;
            request.CookieContainer = cookieJar;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(ASCIIEncoding.ASCII.GetBytes(queryParams), 0, queryParams.Length);
            }
            try
            {
                var response = (HttpWebResponse) request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var JSONnobj = JSON.Instance.ToObject<object>(responseString);

                var data = (Dictionary<string, object>) JSONnobj;

                try
                {
                    var vehlist = (List<object>) data["vehicles"];

                    foreach (var item in vehlist)
                    {
                        Dictionary<string, object> nitem = (Dictionary<string, object>) item;
                        string name = nitem["name"].ToString();
                        string uuid = nitem["uuid"].ToString();

                        Console.WriteLine(name + " " + uuid);
                        accountuuids[name] = uuid;
                    }
                }
                catch
                {
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        //http://api.3drobotics.com/swagger-ui/

        public static bool doVehicleCreate(string name, string uuid)
        {
            String baseUrl = APIConstants.URL_BASE;
            NameValueCollection @params = new NameValueCollection();
            @params.Add("api_key", APIConstants.apiKey);
            @params.Add("uuid", uuid);
            @params.Add("name", name);

            String queryParams = ToQueryString(@params);
            String webAppUploadUrl = String.Format("{0}/api/v1/vehicle", baseUrl);

            var request = (HttpWebRequest) WebRequest.Create(webAppUploadUrl);
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = queryParams.Length;
            request.CookieContainer = cookieJar;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(ASCIIEncoding.ASCII.GetBytes(queryParams), 0, queryParams.Length);
            }
            try
            {
                var response = (HttpWebResponse) request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var JSONnobj = JSON.Instance.ToObject<object>(responseString);

                var data = (Dictionary<string, object>) JSONnobj;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string doUpload(string file, string userId, string userPass, string vehicleId)
        {
            if (vehicleId == null)
                vehicleId = Guid.NewGuid().ToString();

            String baseUrl = APIConstants.URL_BASE;
            NameValueCollection @params = new NameValueCollection();
            @params.Add("api_key", APIConstants.apiKey);
            @params.Add("login", userId);
            @params.Add("password", userPass);
            @params.Add("privacy", "PRIVATE");
            @params.Add("autoCreate", "false");
            String queryParams = ToQueryString(@params);
            String webAppUploadUrl = String.Format("{0}/api/v1/mission/upload/{1}", baseUrl, vehicleId, queryParams);

            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                // http post
                string JSONresp = UploadFilesToRemoteUrl(webAppUploadUrl, file, @params);

                var JSONnobj = JSON.Instance.ToObject<object>(JSONresp);

                object[] data = (object[]) JSONnobj;

                var item2 = ((Dictionary<string, object>) data[0]);

                string answer = item2["viewURL"].ToString();

                return answer;
            }
            catch (WebException ex)
            {
                var answer = ex.Response as HttpWebResponse;
                if (answer != null)
                    CustomMessageBox.Show(answer.StatusDescription);
                else
                    CustomMessageBox.Show("Failed to upload\n" + ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            }

            return "";
        }


        public static string UploadFilesToRemoteUrl(string url, string file, NameValueCollection nvc)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest httpWebRequest2 = (HttpWebRequest) WebRequest.Create(url);
            httpWebRequest2.ContentType = "multipart/form-data; boundary=" + boundary;
            //httpWebRequest2.ContentType = APIConstants.TLOG_MIME_TYPE;
            httpWebRequest2.Method = "POST";
            httpWebRequest2.KeepAlive = true;
            httpWebRequest2.Credentials = System.Net.CredentialCache.DefaultCredentials;
            httpWebRequest2.Accept = "application/json, text/plain, */*";
            httpWebRequest2.AllowWriteStreamBuffering = false;
            httpWebRequest2.Timeout = 7200000; // 2 hrs
            httpWebRequest2.CookieContainer = cookieJar;

            Stream memStream = new System.IO.MemoryStream();

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] boundarybytes2 = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary);

            string formdataTemplate = "\r\n--" + boundary +
                                      "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            foreach (string key in nvc.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: " +
                                    APIConstants.TLOG_MIME_TYPE + "\r\n\r\n";

            if (file.ToLower().EndsWith(".log"))
                headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

            string header = string.Format(headerTemplate, "file", Path.GetFileName(file));

            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

            memStream.Write(headerbytes, 0, headerbytes.Length);

            using (FileStream fileStream = new FileStream(file, FileMode.Open,
                FileAccess.Read))
            {
                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
            }

            // write last boundry
            memStream.Write(boundarybytes2, 0, boundarybytes2.Length);
            // write last -- to last boundry
            memStream.Write(new byte[] {(byte) '-', (byte) '-'}, 0, 2);

            httpWebRequest2.ContentLength = memStream.Length;

            Stream requestStream = httpWebRequest2.GetRequestStream();

            memStream.Position = 0;

            while (memStream.Position < memStream.Length)
            {
                Console.WriteLine("Upload file " + memStream.Position + "/" + memStream.Length);
                byte[] tempBuffer = new byte[1024];
                int read = memStream.Read(tempBuffer, 0, tempBuffer.Length);
                requestStream.Write(tempBuffer, 0, read);
                requestStream.Flush();
            }
            requestStream.Close();
            memStream.Close();

            WebResponse webResponse2 = httpWebRequest2.GetResponse();

            Stream stream2 = webResponse2.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream2);

            string answer = reader2.ReadToEnd();

            webResponse2.Close();
            httpWebRequest2 = null;
            webResponse2 = null;

            return answer;
        }

        public static CookieContainer cookieJar = new CookieContainer();
    }
}