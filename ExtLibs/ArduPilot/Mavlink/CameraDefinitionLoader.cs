using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public static class CameraDefinitionLoader
    {
        public sealed class FtpAddress
        {
            public uint System;
            public byte Component;
            public string Path;
        }

        // mftp's bracketed selector is not an IPv6 authority; parse it before using System.Uri.
        public static FtpAddress ParseFtpAddress(string uri, uint system, byte component)
        {
            var match = Regex.Match(uri, @"^(mftp|mavftp)://(.*)$", RegexOptions.IgnoreCase);
            if (!match.Success) throw new ArgumentException("Invalid MAVFTP definition URI");
            string path = match.Groups[2].Value;
            if (match.Groups[1].Value.Equals("mftp", StringComparison.OrdinalIgnoreCase))
            {
                var selector = Regex.Match(path, @"^/?\[;comp=(\d+)\]");
                if (selector.Success)
                {
                    component = byte.Parse(selector.Groups[1].Value);
                    path = path.Substring(selector.Length);
                }
                else if (path.TrimStart('/').StartsWith("[")) throw new ArgumentException("Invalid MAVFTP component selector");
            }
            else if (!path.StartsWith("/"))
            {
                int slash = path.IndexOf('/');
                if (slash < 0) throw new ArgumentException("Missing MAVFTP filename");
                var target = path.Substring(0, slash).Split(':');
                if (target.Length != 2) throw new ArgumentException("Invalid MAVFTP address");
                system = uint.Parse(target[0]);
                component = byte.Parse(target[1]);
                path = path.Substring(slash);
            }
            path = Uri.UnescapeDataString("/" + path.TrimStart('/'));
            if (system == 0 || component == 0 || path == "/" || path.IndexOf('\0') >= 0 || Encoding.UTF8.GetByteCount(path) > 238)
                throw new ArgumentException("Invalid MAVFTP address or filename");
            return new FtpAddress { System = system, Component = component, Path = path };
        }

        /// <summary>Blocking download/parse; caller must run on a worker thread.</summary>
        public static CameraDefinition Load(string uri, MAVState camera, CancellationTokenSource cancel)
        {
            cancel.Token.ThrowIfCancellationRequested();
            byte[] data;
            if (uri.StartsWith("mftp://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("mavftp://", StringComparison.OrdinalIgnoreCase))
            {
                var address = ParseFtpAddress(uri, camera.sysid, camera.compid);
                var ftp = new MAVFtp(camera.parent, address.System, address.Component);
                // Check the advertised size before MAVFTP allocates its receive buffer.
                bool opened = false;
                try
                {
                    opened = ftp.kCmdOpenFileRO(address.Path, out var size, cancel, resetOnNoSessions: false);
                    cancel.Token.ThrowIfCancellationRequested();
                    if (!opened) throw new IOException("Could not open camera definition over MAVFTP");
                    if (size <= 0 || size > CameraDefinition.MaxSize) throw new InvalidDataException("Invalid camera definition size");
                    using (var stream = ftp.kCmdReadFile(address.Path, size, cancel, session: ftp.ReadSession.Value))
                    {
                        cancel.Token.ThrowIfCancellationRequested();
                        if (stream == null || stream.Length != size) throw new IOException("Incomplete MAVFTP camera definition");
                        data = CameraDefinition.ReadLimited(stream);
                    }
                }
                finally { if (ftp.ReadSession.HasValue) ftp.kCmdTerminateSession(ftp.ReadSession.Value); }
            }
            else
            {
                var address = new Uri(uri, UriKind.Absolute);
                for (int redirects = 0; ; redirects++)
                {
                    if (address.Scheme != "http" && address.Scheme != "https") throw new ArgumentException("Definition URI must use MAVFTP, HTTP or HTTPS");
                    var request = (HttpWebRequest)WebRequest.Create(address);
                    request.AllowAutoRedirect = false;
                    request.Timeout = request.ReadWriteTimeout = 15000;
                    using (cancel.Token.Register(request.Abort))
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                        {
                            if (redirects >= 5) throw new IOException("Too many definition redirects");
                            address = new Uri(address, response.Headers["Location"]);
                            continue;
                        }
                        using (var stream = response.GetResponseStream()) data = CameraDefinition.ReadLimited(stream);
                        break;
                    }
                }
            }
            cancel.Token.ThrowIfCancellationRequested();
            return new CameraDefinition(data);
        }
    }
}
