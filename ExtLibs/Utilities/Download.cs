using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace MissionPlanner.Utilities
{
    public static class DownloadExt
    {
        public static DateTime LastModified(this HttpContentHeaders headers)
        {
            if (headers.Any(h => h.Key.Equals("Last-Modified")))
                return DateTime.Parse(headers.First(h => h.Key.Equals("Last-Modified")).Value.First());
            return DateTime.MinValue;
        }

        public static int ContentLength(this HttpContentHeaders headers)
        {
            if (headers.Any(h => h.Key.Equals("Content-Length")))
                return int.Parse(headers.First(h => h.Key.Equals("Content-Length")).Value.First());
            return -1;
        }
    }


    public class DownloadStream : Stream
    {
        private long _length;
        string _uri = "";
        public int chunksize { get; set; } = 1000 * 250;

        static HttpClient client = new HttpClient();

        private static object _lock = new object();
        /// <summary>
        /// static global cache of instance cache
        /// </summary>
        static readonly Dictionary<string, Dictionary<long, MemoryStream>> _cacheChunks = new Dictionary<string, Dictionary<long, MemoryStream>>();
        /// <summary>
        /// instances
        /// </summary>
        static readonly List<DownloadStream> _instances = new List<DownloadStream>();
        /// <summary>
        /// per instance cache
        /// </summary>
        Dictionary<long,MemoryStream> _chunks = new Dictionary<long, MemoryStream>();

        DateTime _lastread = DateTime.MinValue;

        static void expireCache()
        {
            List<string> seen = new List<string>();
            lock (_lock)
            {
                foreach (var downloadStream in _instances.ToArray())
                {
                    // only process a uri once
                    if (seen.Contains(downloadStream._uri))
                        continue;
                    seen.Add(downloadStream._uri);

                    // total instances with this uri
                    var uris = _instances.Where(a => { return a._uri == downloadStream._uri; });
                    // total instance with thsi uri and old lastread
                    var uridates = _instances.Where(a =>
                    {
                        return a._uri == downloadStream._uri && a._lastread < DateTime.Now.AddSeconds(-180);
                    });

                    // check if they are equal and expire
                    if (uris.Count() == uridates.Count())
                    {
                        _cacheChunks.Remove(downloadStream._uri);
                        foreach (var uridate in uridates.ToArray())
                        {
                            _instances.Remove(uridate);
                        }
                    }
                }
            }
        }

        private static Timer _timer;

        static DownloadStream()
        {
            _timer = new Timer(a => { expireCache(); }, null, 1000 * 30, 1000 * 30);
            if (!String.IsNullOrEmpty(Settings.Instance.UserAgent))
                client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);
        }

        public DownloadStream(string uri)
        {
            _uri = uri;
            SetLength(Download.GetFileSize(uri));

            lock (_lock)
            {
                _instances.Add(this);
           
                if (_cacheChunks.ContainsKey(uri))
                {
                    _chunks = _cacheChunks[uri];
                }
                else
                {
                    _cacheChunks[uri] = _chunks;
                }
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _lastread = DateTime.Now;
            var start = Position;
            var end = start + count;


            // return data
            // check to see if this spans a chunk
            getAllData(start, end);

            var bytestoget = count;
            var bytesgot = 0;


                //var leftinchunk = Position % chunksize == 0 ? chunksize : chunksize - (Position % chunksize);
                //bytesgot += Read(buffer, offset + bytesgot, (int)Math.Min(bytestoget - bytesgot, leftinchunk));
            

            //while (bytesgot < bytestoget)
            {
                var chunk = ChunkThatHasOffset(Position);

                var positioninchunk = Position - chunk.Key;
                var chunkleft = chunk.Value.Length - positioninchunk;

                var maxcount = (int)Math.Min(chunkleft, count);

                lock (chunk.Value)
                {
                    chunk.Value.Position = positioninchunk;
                    chunk.Value.Read(buffer, offset, maxcount);
                }
                //Array.Copy(chunk.Value.ToArray(), positioninchunk, buffer, offset, maxcount);

                bytesgot += maxcount;
                offset += maxcount;
                Position += maxcount;
            }

            if (bytesgot < bytestoget)
                bytesgot += Read(buffer, offset, bytestoget - bytesgot);
            
            return bytesgot;
        }

        public bool getAllData(long start, long end)
        {
            if (chunksize < 1024 * 2)
                chunksize = 1024 * 2;

            var chunkThatHasOurStart = ChunkThatHasOffset(start);

            if (chunkThatHasOurStart.Value == null)
            {
                // get it all
                GetChunk(start);
                return true;
            }

            var targetpos = chunkThatHasOurStart.Key + chunkThatHasOurStart.Value.Length;
            while (targetpos < end)
            {
                var chunk = ChunkThatHasOffset(targetpos);
                if (chunk.Value == null)
                {
                    // get it all
                    GetChunk(targetpos);

                    chunk = ChunkThatHasOffset(targetpos);
                }

                targetpos += chunk.Value.Length;
            }

            return true;
        }

        private KeyValuePair<long, MemoryStream> ChunkThatHasOffset(long offset)
        {
            lock (_lock)
            {
                return _chunks.FirstOrDefault(a => a.Key <= offset && a.Key + a.Value.Length > offset);
            }
        }

        private static List<string> gettingChunk = new List<string>();
        private static object gettingChunkLock = new object();

        private void GetChunk(long start)
        {
            var key = _uri.ToLower() + "-" + start;
            try
            {
                var test = false;
                do
                {
                    lock (gettingChunkLock)
                    {
                        // see if we are already getting it
                        test = gettingChunk.Contains(key);

                        if (!test)
                        {
                            gettingChunk.Add(key);
                            break;
                        }
                    }
                    Thread.Sleep(50);
                } while (test);

                // we have it already
                if (_chunks.ContainsKey(start))
                    return;

                var end = Math.Min(Length, start + chunksize);

                // cache it
                var request = new HttpRequestMessage() {RequestUri = new Uri(_uri)};
                request.Headers.Range = new RangeHeaderValue(start, end);

                Console.WriteLine("{0}: {1} - {2} {3}", _uri, start, end, end-start);

                MemoryStream ms = new MemoryStream();
                using (Stream stream = client.SendAsync(request).GetAwaiter().GetResult().Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                {
                    stream.CopyTo(ms);

                    lock (_lock)
                    {
                        _chunks[start] = ms;
                    }
                }
            }
            finally
            {
                lock (gettingChunkLock)
                {
                    gettingChunk.Remove(key);
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            //Console.WriteLine("Seek: {0} {1}", offset, origin);
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else if (origin == SeekOrigin.End)
                Position = Length + offset;

            return Position;
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("No write");
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = true;
        public override bool CanWrite { get; } = false;
        public override long Length
        {
            get { return _length; }
        }
        public override long Position { get; set; }
    }

    public class Download
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<string> PostAsync(string uri, string data)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(uri, new StringContent(data));

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => (content));
        }

        public static async Task<string> GetAsync(string uri)
        {
            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(uri);
            return await Task.Run(() => (content));
        }

        public struct HTTPResult
        {
            public string content;
            public HttpStatusCode status;
        }

        /// <summary>
        /// Perform a GET request and return the content and status code
        /// </summary>
        public static async Task<HTTPResult> GetAsyncWithStatus(string uri)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => (new HTTPResult() { content = content, status = response.StatusCode }));
        }

        public static event EventHandler<HttpRequestMessage> RequestModification;

        public static async Task<bool> getFilefromNetAsync(string url, string saveto, Action<int, string> status = null)
        {
            try
            {
                log.Info("Get " + url);

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                RequestModification?.Invoke(url, request);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    lock (log)
                        log.Info(url + " " +(response).StatusCode.ToString());
                    if ((response).StatusCode != HttpStatusCode.OK)
                        return false;

                    if (File.Exists(saveto))
                    {
                        DateTime lastfilewrite = new FileInfo(saveto).LastWriteTime;
                        DateTime lasthttpmod = response.Content.Headers.LastModified.HasValue
                            ? response.Content.Headers.LastModified.Value.DateTime
                            : DateTime.MinValue;

                        if (lasthttpmod < lastfilewrite)
                        {
                            if ((response).Content.Headers.ContentLength == new FileInfo(saveto).Length)
                            {
                                lock (log)
                                    log.Info(url + " " + "got LastModified " + saveto + " " +
                                             (response).Content.Headers.LastModified +
                                             " vs " + new FileInfo(saveto).LastWriteTime);
                                response.Dispose();
                                return true;
                            }
                        }
                    }

                    int size = 0;
                    using (Stream resstream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (FileStream fs = new FileStream(saveto + ".new", FileMode.Create))
                    {
                        byte[] buf1 = new byte[1024];

                        DateTime lastupdate = DateTime.MinValue;
                        DateTime starttime = DateTime.Now;
                        var contlen = response.Content.Headers.ContentLength;

                        while (resstream.CanRead)
                        {
                            int len = await resstream.ReadAsync(buf1, 0, 1024).ConfigureAwait(false);
                            if (len == 0)
                                break;
                            fs.Write(buf1, 0, len);

                            size += len;

                            var elapsed = (DateTime.Now - starttime).TotalSeconds;
                            var percent = ((size / (float) contlen) * 100.0f);
                            if (lastupdate.Second != DateTime.Now.Second)
                            {
                                lastupdate = DateTime.Now;
                                log.InfoFormat("{0} bps {1} {2}s {3}% of {4}     \r", size / elapsed, size, elapsed,
                                    percent, contlen);
                                var timeleft = TimeSpan.FromSeconds(((elapsed / percent) * (100 - percent)));
                                status?.Invoke((int) percent,
                                    "Downloading.. ETA: " +
                                    //DateTime.Now.AddSeconds(((elapsed / percent) * (100 - percent))).ToShortTimeString()
                                    formatTimeSpan(timeleft)
                                );
                            }
                        }

                        fs.Flush();
                        fs.Close();
                    }

                    log.Info("Got " + url + " " + size);

                    if (File.Exists(saveto))
                    {
                        // try prevent System.UnauthorizedAccessException: Access to the path
                        GC.Collect();
                        File.SetAttributes(saveto, FileAttributes.Normal);
                        File.Delete(saveto);
                    }

                    File.Move(saveto + ".new", saveto);

                    return true;
                }
            }
            catch (Exception ex)
            {
                lock (log)
                    log.Info("getFilefromNetAsync(): " + ex.ToString());
                return false;
            }
        }

        static Download()
        {
            if (!String.IsNullOrEmpty(Settings.Instance.UserAgent))
                client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);
        }

        static HttpClient client = new HttpClient();
        public static bool getFilefromNet(string url, string saveto, Action<int, string> status = null)
        {
            try
            {
                lock (log)
                    log.Info(url);
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);
                client.Timeout = TimeSpan.FromSeconds(30);

                // Get the response.
                var response = client.GetAsync(url).Result;
                // Display the status.
                lock (log)
                    log.Info(response.ReasonPhrase);
                if (!response.IsSuccessStatusCode)
                    return false;

                if (File.Exists(saveto))
                {
                    DateTime lastfilewrite = new FileInfo(saveto).LastWriteTime;
                    DateTime lasthttpmod = response.Content.Headers.LastModified();

                    if (lasthttpmod < lastfilewrite)
                    {
                        if (response.Content.Headers.ContentLength() == new FileInfo(saveto).Length)
                        {
                            lock (log)
                                log.Info("got LastModified " + saveto + " " + (response.Content.Headers).LastModified() +
                                     " vs " + new FileInfo(saveto).LastWriteTime);
                            return true;
                        }
                    }
                }

                // Get the stream containing content returned by the server.
                Stream dataStream = response.Content.ReadAsStreamAsync().Result;

                long bytes = response.Content.Headers.ContentLength();
                long contlen = bytes;

                byte[] buf1 = new byte[1024];

                if (!Directory.Exists(Path.GetDirectoryName(saveto)))
                    Directory.CreateDirectory(Path.GetDirectoryName(saveto));

                FileStream fs = new FileStream(saveto + ".new", FileMode.Create);

                DateTime lastupdate = DateTime.MinValue;
                DateTime starttime = DateTime.Now;
                int got = 0;

                while (dataStream.CanRead && bytes > 0)
                {
                    int len = dataStream.Read(buf1, 0, buf1.Length);
                    bytes -= len;
                    got += len;
                    fs.Write(buf1, 0, len);

                    var elapsed = (DateTime.Now - starttime).TotalSeconds;
                    var percent = ((got / (float)contlen) * 100.0f);
                    if (lastupdate.Second != DateTime.Now.Second)
                    {
                        lastupdate = DateTime.Now;
                        Console.WriteLine("{0} bps {1} {2}s {3}% of {4}     \r", got / elapsed, got, elapsed,
                            percent, contlen);
                        var timeleft = TimeSpan.FromSeconds(((elapsed / percent) * (100 - percent)));
                        status?.Invoke((int)percent,
                            "Downloading.. ETA: " +
                            //DateTime.Now.AddSeconds(((elapsed / percent) * (100 - percent))).ToShortTimeString()
                            formatTimeSpan(timeleft)
                        );
                    }
                }

                fs.Close();
                dataStream.Close();

                if (File.Exists(saveto))
                {
                    File.Delete(saveto);
                }
                File.Move(saveto + ".new", saveto);

                return true;
            }
            catch (Exception ex)
            {
                lock (log)
                    log.Info("getFilefromNet(): " + ex.ToString());
                return false;
            }
        }

        public static async Task<bool> CheckHTTPFileExistsAsync(string url)
        {
            return await Task.Run(() =>
            {
                return CheckHTTPFileExists(url);
            });
        }

        public static bool CheckHTTPFileExists(string url)
        {
            bool result = false;
            Uri uri;
            Uri.TryCreate(url, UriKind.Absolute, out uri);

            if (url == null || url == "" || uri == null)
                return false;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);
            client.Timeout = TimeSpan.FromSeconds(30);
            var resp = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).Result;
            return resp.IsSuccessStatusCode;

        }

        //https://stackoverflow.com/questions/13606523/retrieving-partial-content-using-multiple-http-requsets-to-fetch-data-via-parlle
        [Obsolete]
        public static void ParallelDownloadFile(string uri, string filePath, int chunkSize = 0, Action<int,string> status = null)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            // determine file size first
            long size = GetFileSize(uri);

            if (chunkSize == 0)
                chunkSize = 1024 * 1024 * 10;

            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                file.SetLength(size); // set the length first

                var starttime = DateTime.Now;
                var got = 0L;
                DateTime lastupdate = DateTime.MinValue;

                object syncObject = new object(); // synchronize file writes
                Parallel.ForEach(LongRange(0, 1 + size / chunkSize), new ParallelOptions { MaxDegreeOfParallelism = 3 }, (start) =>
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    if (!String.IsNullOrEmpty(Settings.Instance.UserAgent))
                        ((HttpWebRequest)request).UserAgent = Settings.Instance.UserAgent;
                    var minrange = start * chunkSize;
                    var maxrange = Math.Min(start * chunkSize + chunkSize - 1, size);
                    request.AddRange(minrange, maxrange);
                    log.Info(String.Format("chunk {0} {1} {2}-{3}", start, uri, minrange, maxrange));
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    log.Info(start + " " + uri + " " + response.StatusCode + " " + response.ContentLength);

                    if (response.StatusCode != HttpStatusCode.PartialContent && start != 0)
                    {
                        // fallback to single connection;
                        response.Close();
                        return;
                    }

                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] array = new byte[1024 * 80];
                        int count;
                        while ((count = stream.Read(array, 0, array.Length)) != 0)
                        {
                            lock (syncObject)
                            {
                                file.Seek(minrange, SeekOrigin.Begin);
                                file.Write(array, 0, count);
                                got += count;
                                minrange += count;
                                var elapsed = (DateTime.Now - starttime).TotalSeconds;
                                var percent = ((got / (float) size) * 100.0f);
                                if (lastupdate.Second != DateTime.Now.Second)
                                {
                                    lastupdate = DateTime.Now;
                                    Console.WriteLine("{0} bps {1} {2}s {3}% of {4}     \r", got / elapsed, got, elapsed,
                                        percent, size);
                                    var timeleft = TimeSpan.FromSeconds(((elapsed / percent) * (100 - percent)));
                                    status?.Invoke((int) percent,
                                        "Downloading.. ETA: " +
                                       //DateTime.Now.AddSeconds(((elapsed / percent) * (100 - percent))).ToShortTimeString()
                                        formatTimeSpan(timeleft)
                                        );
                                }
                            }
                        }
                    }
                });

                status?.Invoke(100, "Complete");
            }
        }

        private static string formatTimeSpan(TimeSpan timeleft)
        {
            if (timeleft.TotalHours >= 1)
                return timeleft.TotalHours.ToString("0.0") + " Hours";
            if (timeleft.TotalSeconds >= 60)
                return timeleft.Minutes + ":" + timeleft.Seconds.ToString("00") + " Minutes";

            return timeleft.Seconds + " Seconds";
        }

        static Dictionary<string,long> fileSizeCache = new Dictionary<string, long>();

        public static long GetFileSize(string uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            lock (fileSizeCache)
            {
                if (fileSizeCache.ContainsKey(uri) && fileSizeCache[uri] > 0)
                    return fileSizeCache[uri];

                var responce = client.GetAsync(uri);
                var len = responce.GetAwaiter().GetResult().Content.Headers.ContentLength();
                fileSizeCache[uri] = len;
                responce.Result.Dispose();
                return len;
            }
        }

        private static IEnumerable<long> LongRange(long start, long count)
        {
            long i = 0;
            while (true)
            {
                if (i >= count)
                {
                    yield break;
                }
                yield return start + i;
                i++;
            }
        }
    }
}
