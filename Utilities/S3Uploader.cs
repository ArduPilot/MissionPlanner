using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Net;
using System.IO;


namespace ArdupilotMega.Utilities
{
    public class S3Uploader
    {
        private string bucketName;
        private Amazon.S3.Transfer.TransferUtility transferUtility;

        public int Progress;

        public S3Uploader(string bucketName)
        {
            this.bucketName = bucketName;
            this.transferUtility = new Amazon.S3.Transfer.TransferUtility("AKIAIALOFNWOTXDMVF3Q", "d0mcWo3UkDD95rE9KyFxowbmPnr9t1Y4RbmHvwGA");
        }

        public void UploadTlog(string source)
        {
            bucketName = "s3-droneshare";
            Random rnd = new Random();
            UploadFile(source, "uploads/MP" + Math.Abs(rnd.Next(int.MaxValue)) + System.IO.Path.GetExtension(source));
        }

        public void UploadFile(string filePath, string toPath)
        {
            AsyncCallback callback = new AsyncCallback(uploadComplete);
            var uploadRequest = new TransferUtilityUploadRequest();
            uploadRequest.FilePath = filePath;
            uploadRequest.BucketName = bucketName;
            uploadRequest.Key = toPath;
            //uploadRequest.AddHeader("x-amz-acl", "private");
            uploadRequest.UploadProgressEvent += new EventHandler<UploadProgressArgs>(uploadRequest_UploadProgressEvent);
            transferUtility.BeginUpload(uploadRequest, callback, toPath);
        }

        void uploadRequest_UploadProgressEvent(object sender, UploadProgressArgs e)
        {
            Console.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
            Progress = e.PercentDone;
        }

        private void uploadComplete(IAsyncResult result)
        {
            try
            {
                transferUtility.EndUpload(result);
            }
            catch (ArgumentException ex) { System.Windows.Forms.CustomMessageBox.Show("Error bad argument\n\n " + ex.ToString()); return; }
            catch (WebException ex) { System.Windows.Forms.CustomMessageBox.Show("Error communicating with server\n\n" + ex.ToString()); return; }
            catch (AmazonS3Exception ex) { System.Windows.Forms.CustomMessageBox.Show("Error accessing amazon\n\n" + ex.ToString()); return; }

            Progress = 100;
            var x = result;
            Console.WriteLine("Upload Done");

            Uri baseurl = new Uri("http://upload.droneshare.com/api/upload/froms3.json");

           JSonRequest(baseurl.ToString(), (string)x.AsyncState );
        }

        private void JSonRequest(string url, string key)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            
              string json = "{\"key\":\"" + key + "\",\"userId\": \"\",\"userPass\":\"\"}";

             httpWebRequest.ContentLength =  json.Length;


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
              
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
               

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpResponse.StatusCode >= HttpStatusCode.OK && (int)httpResponse.StatusCode < 300)
                {

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        if (result.StartsWith("\"") && result.EndsWith("\""))
                        {
                            System.Diagnostics.Process.Start("http://upload.droneshare.com/view/" + result.Trim(new char[] { '"' }));
                        }
                    }
                }
                else if (httpResponse.StatusCode == HttpStatusCode.NotAcceptable)
                {
                    System.Windows.Forms.CustomMessageBox.Show("Flightlog was to short, please upload a longer flight log.");
                }
                else
                {
                    System.Windows.Forms.CustomMessageBox.Show("Error procerssing flight log " + httpResponse.StatusCode.ToString());
                }

            }
        }

    }
}
