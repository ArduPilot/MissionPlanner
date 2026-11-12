using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Controls;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MissionPlannerTests.OpenGL
{
    /// <summary>
    /// Shows the real FlightData HUD with its OpenGL renderer and validates what the driver drew.
    /// Has no test-framework dependency: it is used by the MSTest wrapper on Windows and by
    /// Linux/OpenGLSelfTest.cs inside the packaged Linux build.
    ///
    /// Environment:
    ///   MP_OPENGL32_PATH       Windows only. Full path of an opengl32.dll (e.g. Mesa llvmpipe from
    ///                          mesa-dist-win) to load instead of the system one.
    ///   MP_EXPECT_GL_RENDERER  Regex the GL_RENDERER string must match (e.g. "llvmpipe"). When set,
    ///                          the check fails if a different driver was used.
    ///   MP_GL_SCREENSHOT       File to save the captured OpenGL frame to (PNG).
    /// </summary>
    public static class HudOpenGLProbe
    {
        public class Result
        {
            public string Vendor, Renderer, Version, OpenGLLibrary;
            public bool OpenGLStillEnabled;
            public int GLFrames;
            public Bitmap Frame;
            public string Failure;
            public bool Passed { get { return Failure == null; } }

            public override string ToString()
            {
                return string.Format(
                    "GL_VENDOR={0}\nGL_RENDERER={1}\nGL_VERSION={2}\nopengl library={3}\nHUD opengl enabled={4}\nGL frames={5}\nresult={6}",
                    Vendor, Renderer, Version, OpenGLLibrary, OpenGLStillEnabled, GLFrames,
                    Failure ?? "PASS");
            }
        }

        // Distinctive colours so the frame can be checked without a reference image.
        static readonly Color Sky = Color.FromArgb(0, 0, 255);
        static readonly Color Ground = Color.FromArgb(255, 0, 0);

        public static Result Run(int width = 640, int height = 480, int timeoutMs = 30000)
        {
            Result result = null;
            Exception crash = null;
            // WinForms + WGL/GLX want a dedicated STA UI thread regardless of the caller.
            var thread = new Thread(() =>
            {
                try { result = RunOnUiThread(width, height, timeoutMs); }
                catch (Exception ex) { crash = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            if (!thread.Join(timeoutMs + 30000))
                return new Result { Failure = "HUD OpenGL probe hung" };
            if (crash != null)
                return new Result { Failure = "HUD OpenGL probe threw: " + crash };
            return result;
        }

        static Result RunOnUiThread(int width, int height, int timeoutMs)
        {
            var result = new Result();
            PreloadOpenGL(result);

            using (var form = new Form())
            using (var hud = new HUD())
            {
                form.Text = "HUD OpenGL probe";
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(0, 0);
                form.ClientSize = new Size(width, height);

                hud.opengl = true;
                hud.Dock = DockStyle.Fill;
                hud.skyColor1 = hud.skyColor2 = Sky;
                hud.groundColor1 = hud.groundColor2 = Ground;
                hud.roll = 0;
                hud.pitch = 0;
                hud.heading = 90;

                hud.OnCustomPaint += h =>
                {
                    // Runs inside doPaint with the HUD's context current and before SwapBuffers,
                    // so the back buffer holds the complete frame. Skip GDI frames.
                    if (!h.opengl || GraphicsContext.CurrentContext == null)
                        return;
                    result.GLFrames++;
                    if (result.GLFrames < 3)
                        return; // let textures/fonts settle
                    if (result.Frame == null)
                        Capture(h, result);
                };

                form.Controls.Add(hud);
                form.Show();

                var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
                while (result.Frame == null && DateTime.UtcNow < deadline && hud.opengl)
                {
                    hud.Invalidate();
                    Application.DoEvents();
                    Thread.Sleep(50); // HUD skips paints closer than 30 ms apart
                }

                result.OpenGLStillEnabled = hud.opengl;
                form.Close();
            }

            Validate(result);
            return result;
        }

        static void Capture(HUD hud, Result result)
        {
            result.Vendor = GL.GetString(StringName.Vendor);
            result.Renderer = GL.GetString(StringName.Renderer);
            result.Version = GL.GetString(StringName.Version);

            int w = hud.ClientSize.Width, h = hud.ClientSize.Height;
            var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bmp.PixelFormat);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 4);
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            result.Frame = bmp;

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                result.Failure = "GL error after capture: " + err;
        }

        static void Validate(Result r)
        {
            if (r.Failure != null)
                return;
            if (!r.OpenGLStillEnabled)
            {
                r.Failure = "HUD fell back to GDI+ (OpenGL context creation or first GL call failed; see log)";
                return;
            }
            if (r.Frame == null)
            {
                r.Failure = "No OpenGL frame was rendered (GL frames seen: " + r.GLFrames + ")";
                return;
            }

            var shot = Environment.GetEnvironmentVariable("MP_GL_SCREENSHOT");
            if (!string.IsNullOrEmpty(shot))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(shot)));
                r.Frame.Save(shot, ImageFormat.Png);
            }

            var expected = Environment.GetEnvironmentVariable("MP_EXPECT_GL_RENDERER");
            if (!string.IsNullOrEmpty(expected) &&
                (r.Renderer == null || !Regex.IsMatch(r.Renderer, expected, RegexOptions.IgnoreCase)))
            {
                r.Failure = "GL_RENDERER '" + r.Renderer + "' does not match expected /" + expected + "/";
                return;
            }

            // Level attitude: sky fills the top half, ground the bottom half, apart from the
            // overlays (tapes, pitch ladder, text). Require each to dominate its own half.
            double skyTop = Fraction(r.Frame, 0, r.Frame.Height / 2, Sky);
            double groundBottom = Fraction(r.Frame, r.Frame.Height / 2, r.Frame.Height, Ground);
            double groundTop = Fraction(r.Frame, 0, r.Frame.Height / 2, Ground);
            double skyBottom = Fraction(r.Frame, r.Frame.Height / 2, r.Frame.Height, Sky);
            int colours = DistinctColours(r.Frame);

            var stats = string.Format(" (sky top {0:P0}, ground bottom {1:P0}, ground top {2:P0}, sky bottom {3:P0}, colours {4})",
                skyTop, groundBottom, groundTop, skyBottom, colours);
            if (skyTop < 0.3 || groundBottom < 0.3)
                r.Failure = "Artificial horizon not rendered" + stats;
            else if (groundTop > 0.05 || skyBottom > 0.05)
                r.Failure = "Artificial horizon rendered in the wrong place/orientation" + stats;
            else if (colours < 16)
                r.Failure = "HUD overlays (text, ladder, tapes) not rendered" + stats;
        }

        static double Fraction(Bitmap bmp, int y0, int y1, Color c)
        {
            int match = 0, total = 0;
            for (int y = y0; y < y1; y += 2)
                for (int x = 0; x < bmp.Width; x += 2)
                {
                    var p = bmp.GetPixel(x, y);
                    total++;
                    if (Math.Abs(p.R - c.R) < 24 && Math.Abs(p.G - c.G) < 24 && Math.Abs(p.B - c.B) < 24)
                        match++;
                }
            return total == 0 ? 0 : (double)match / total;
        }

        static int DistinctColours(Bitmap bmp)
        {
            var seen = new System.Collections.Generic.HashSet<int>();
            for (int y = 0; y < bmp.Height; y += 2)
                for (int x = 0; x < bmp.Width; x += 2)
                    seen.Add(bmp.GetPixel(x, y).ToArgb() & 0xF0F0F0);
            return seen.Count;
        }

        // ---- Windows: make a Mesa opengl32.dll win over System32's ----
        // Windows resolves later loads of "opengl32.dll" (OpenTK's DllImport, gdi32's
        // SetPixelFormat) to an already-loaded module of that name, so loading Mesa first
        // by full path swaps the implementation for the whole process.

        static bool IsWindows { get { return Environment.OSVersion.Platform == PlatformID.Win32NT; } }

        static void PreloadOpenGL(Result result)
        {
            if (!IsWindows)
            {
                result.OpenGLLibrary = "(system libGL)";
                return;
            }
            var path = Environment.GetEnvironmentVariable("MP_OPENGL32_PATH");
            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("MP_OPENGL32_PATH does not exist", path);
                if (GetModuleHandle("opengl32.dll") != IntPtr.Zero && !SameFile(LoadedOpenGL(), path))
                    throw new InvalidOperationException("A different opengl32.dll is already loaded: " + LoadedOpenGL());
                const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x8; // resolve libgallium_wgl.dll beside it
                if (LoadLibraryEx(Path.GetFullPath(path), IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH) == IntPtr.Zero)
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "LoadLibrary " + path);
            }
            else
            {
                LoadLibraryEx("opengl32.dll", IntPtr.Zero, 0);
            }
            result.OpenGLLibrary = LoadedOpenGL();
        }

        static string LoadedOpenGL()
        {
            var handle = GetModuleHandle("opengl32.dll");
            if (handle == IntPtr.Zero)
                return null;
            var sb = new StringBuilder(1024);
            GetModuleFileName(handle, sb, sb.Capacity);
            return sb.ToString();
        }

        static bool SameFile(string a, string b)
        {
            return a != null && string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);
    }
}
