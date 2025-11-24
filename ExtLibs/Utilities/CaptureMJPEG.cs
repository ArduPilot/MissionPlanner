using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using log4net;
using System.Drawing;

namespace MissionPlanner.Utilities
{
    public class CaptureMJPEG
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Thread asyncthread;
        static bool running = false;
        public static string URL = @"http://127.0.0.1:56781/map.jpg";

        static DateTime lastimage = DateTime.Now;
        static int fps = 0;
        private static event EventHandler<Bitmap> _onNewImage;
        public static event EventHandler<Bitmap> onNewImage
        {
            add { _onNewImage += value; }
            remove { _onNewImage -= value; }
        }

        public static void runAsync()
        {
            while (asyncthread != null && asyncthread.IsAlive)
            {
                running = false;
                Thread.Sleep(1);
            }

            asyncthread = new Thread(new ThreadStart(getUrl))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
                Name = "mjpg stream reader"
            };

            asyncthread.Start();
        }

        public static void Stop()
        {
            running = false;
        }

        public static string ReadLine(BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();

            DateTime deadline = DateTime.Now.AddSeconds(5);

            while (DateTime.Now < deadline) {
                try
                {
                    byte by = br.ReadByte();
                    deadline = DateTime.Now.AddSeconds(5);
                    sb.Append((char) by);
                    if (by == '\n')
                        break;
                }
                catch { }
            }

            sb = sb.Replace("\r\n", "");

            log.Debug(sb.ToString());

            return sb.ToString();
        }

        static void getUrl()
        {
            running = true;

            start:
            
            try
            {
                // Create a request using a URL that can receive a post.
                var request = (HttpWebRequest)WebRequest.Create(URL);
                // Set the Method property of the request to POST. 
                request.Method = "GET";
                request.KeepAlive = true;
                request.AllowReadStreamBuffering = false;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Headers.Add("Accept-Encoding", "gzip,deflate");
                request.Accept = "multipart/x-mixed-replace";
                ServicePointManager.Expect100Continue = false;
                // Get the response. 
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Display the status. 
                    log.Debug(response.StatusDescription);
                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    using (var br = new BinaryReader(dataStream))
                    {

                        // Try to get boundary from header; if absent, detect in body.
                        string contentType = response.Headers["Content-Type"] ?? response.ContentType;
                        string boundary = null;
                        bool atPartStart = false;

                        try { boundary = ExtractBoundary(contentType); } // may throw if header missing
                        catch
                        {
                            // No Content-Type or no boundary=... -> sniff it from body
                            boundary = DetectBoundaryFromBody(br); // positions us AFTER the first boundary line
                            atPartStart = true; // next read is headers of first part
                        }

                        // Optionally set a read timeout (not all streams support it)
                        try
                        {
                            dataStream.ReadTimeout = 10000; // 10 seconds 
                            br.BaseStream.ReadTimeout = 10000;
                        }
                        catch { }

                        while (running)
                        {
                            try
                            {
                                if (!atPartStart) SeekToBoundaryLine(br, boundary); // find start of next part
                                else atPartStart = false;

                                var headers = ReadHeadersCI(br);

                                byte[] jpeg;
                                if (
                                    headers.TryGetValue("Content-Length", out var lenStr) &&
                                    int.TryParse(lenStr, out var len) &&
                                    len > 0
                                )
                                {
                                    jpeg = ReadExact(br, len);
                                    SafeConsumeCrlf(br); // swallow optional trailing CRLF
                                }
                                else jpeg = ReadUntilNextBoundary(br, boundary);

                                try
                                {
                                    using (var ms = new MemoryStream(jpeg))
                                    using (var bmp = new System.Drawing.Bitmap(ms))
                                    {
                                        fps++;
                                        if (lastimage.Second != DateTime.Now.Second)
                                        {
                                            Console.WriteLine("MJPEG " + fps);
                                            fps = 0;
                                            lastimage = DateTime.Now;
                                        }

                                        _onNewImage?.Invoke(null, (Bitmap)bmp.Clone());
                                    }
                                }
                                catch (Exception ex) { log.Info(ex); }
                            }
                            catch (Exception ex) { log.Info(ex); break; } // reconnect 
                        }

                        _onNewImage?.Invoke(null, null);
                    }
                }
            }
            catch (Exception ex) { log.Error(ex); }

            if (running)
                goto start;

            running = false;
        }


        // Additional modifications
        static string ExtractBoundary(string contentType)
        {
            // Typical: "multipart/x-mixed-replace; boundary=frame" (no leading "--")
            if (string.IsNullOrEmpty(contentType) || !contentType.Contains("boundary="))
                throw new InvalidOperationException("MJPEG boundary not present in Content-Type.");

            var ct = contentType;
            var idx = ct.IndexOf("boundary=", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) throw new InvalidOperationException("MJPEG boundary not present in Content-Type.");

            var val = ct.Substring(idx + "boundary=".Length).Trim();
            // strip quotes if present
            if (val.Length >= 2 && ((val[0] == '"' && val.Last() == '"') || (val[0] == '\'' && val.Last() == '\'')))
                val = val.Substring(1, val.Length - 2);
            return val;
        }
        
        static string DetectBoundaryFromBody(BinaryReader br)
        {
            // RFC 2046 allows an optional preamble; the first boundary appears as:  --<token>\r\n
            // We read ASCII lines until we hit a line that starts with "--".
            string line;
            while ((line = ReadLine(br)) != null)
            {
                if (line.StartsWith("--") && line.Length > 2)
                {
                    var token = line.Substring(2).Trim();
                    // If it's an end marker like "--boundary--", strip the trailing "--"
                    if (token.EndsWith("--", StringComparison.Ordinal))
                        token = token.Substring(0, token.Length - 2);
                    if (token.Length > 0)
                        return token;
                }
                // else continue through preamble/garbage
            }
            throw new InvalidOperationException("Could not detect MJPEG boundary from body.");
        }


        static void SeekToBoundaryLine(BinaryReader br, string boundary)
        {
            // Boundary line looks like:  --<boundary>\r\n
            string target = "--" + boundary;
            string line;
            do
            {
                line = ReadLine(br);
                if (line == null) throw new EndOfStreamException("MJPEG stream closed while seeking boundary.");
            } while (!line.StartsWith(target, StringComparison.Ordinal));
            // Now positioned right before headers
        }

        static Dictionary<string, string> ReadHeadersCI(BinaryReader br)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string line;
            while (!string.IsNullOrEmpty(line = ReadLine(br)))
            {
                int c = line.IndexOf(':');
                if (c > 0)
                {
                    string key = line.Substring(0, c).Trim();
                    string val = line.Substring(c + 1).Trim();
                    headers[key] = val;
                }
                // else: ignore malformed lines
            }

            return headers;
        }

        static byte[] ReadExact(BinaryReader br, int len)
        {
            var buf = new byte[len];
            int off = 0;
            while (len > 0)
            {
                int n = br.Read(buf, off, len);
                if (n <= 0)
                    throw new EndOfStreamException("Unexpected end of stream while reading Content-Length body.");
                off += n;
                len -= n;
            }

            return buf;
        }

        static void SafeConsumeCrlf(BinaryReader br)
        {
            // Best effort: consume \r?\n if present
            try
            {
                int b = br.PeekChar();
                if (b == '\r')
                {
                    br.ReadByte();
                    if (br.PeekChar() == '\n') br.ReadByte();
                }
                else if (b == '\n')
                {
                    br.ReadByte();
                }
            }
            catch
            {
            }
        }

        static byte[] ReadUntilNextBoundary(BinaryReader br, string boundary)
        {
            // We will read byte-by-byte to avoid over-reading beyond the boundary (NetworkStream is not seekable).
            // Detect the sequence "\r\n--<boundary>"
            var sep = Encoding.ASCII.GetBytes("\r\n--" + boundary);
            int match = 0;

            using (var ms = new MemoryStream(256 * 1024))
            {
                while (true)
                {
                    int b = br.Read();
                    if (b < 0) throw new EndOfStreamException("MJPEG stream closed while reading frame.");

                    ms.WriteByte((byte)b);

                    if ((byte)b == sep[match])
                    {
                        match++;
                        if (match == sep.Length)
                        {
                            // Remove the boundary bytes from the end of frame
                            ms.SetLength(ms.Length - sep.Length);
                            // Also remove the preceding CR if we captured it as part of JPEG tail (not strictly required)
                            long len = ms.Length;
                            if (len > 0 && ms.GetBuffer()[len - 1] == (byte)'\r')
                                ms.SetLength(len - 1);
                            // At this point, the stream is positioned right after "--<boundary>";
                            // Next call will read the rest of the boundary line + headers.
                            return ms.ToArray();
                        }
                    }
                    else
                    {
                        // Reset partial match if current byte doesn't continue the prefix
                        match = (sep[0] == (byte)b) ? 1 : 0;
                    }
                }
            }
        }
    }
}
