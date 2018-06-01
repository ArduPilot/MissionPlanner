using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using MissionPlanner.Log;

namespace MissionPlanner.Log
{

}

namespace tlogThumbnailHandler
{
    //C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe /regfile /codebase tlogThumbnailHandler.dll

    //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm /regfile /codebase tlogThumbnailHandler.dll

    // reg add HKEY_CLASSES_ROOT\.tlog\shellex\{BB2E617C-0920-11D1-9A0B-00C04FC2D6C1} /d {f3b857f1-0b79-4e77-9d0b-8b8b7e874f56}

    // reg add HKEY_CLASSES_ROOT\MissionPlanner.tlog\shellex\{BB2E617C-0920-11D1-9A0B-00C04FC2D6C1} /d {f3b857f1-0b79-4e77-9d0b-8b8b7e874f56}

    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    [ProgId("tlogThumbnailHandler.tlogThumbnailHandler")]
    [Guid("f3b857f1-0b79-4e77-9d0b-8b8b7e874f56")]
    public class tlogThumbnailHandler : IThumbnailProvider, IInitializeWithStream, IExtractImage2, IExtractImage, IPersistFile, IDisposable
    {
        private Size m_size = Size.Empty; 
        private string m_filename = String.Empty;
        private const long S_OK = 0x00000000L;
        private const long E_FAIL = 0x80004005L;
        private const long E_PENDING = 0x8000000AL;

        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        static string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string queuefile = commonAppData + Path.DirectorySeparatorChar + "Mission Planner" + Path.DirectorySeparatorChar + "tlogimagecache" + Path.DirectorySeparatorChar + "queue.txt";

        private static class NativeMethods
        {
            [DllImport("kernel32.dll",
                   EntryPoint = "GetStdHandle",
                   SetLastError = true,
                   CharSet = CharSet.Auto,
                  CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);
            [DllImport("kernel32.dll",
                EntryPoint = "AllocConsole",
                SetLastError = true,
                CharSet = CharSet.Auto,
                CallingConvention = CallingConvention.StdCall)]

            internal static extern int AllocConsole();
        }

        void WriteLine(string format, params object[] arg)
        {
            try
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " " + format, arg);
            }
            catch
            {
            }
        }

        static bool init = false;

        public tlogThumbnailHandler()
        {
            if (!init)
            {
                /*
                NativeMethods.AllocConsole();
                IntPtr stdHandle = NativeMethods.GetStdHandle(STD_OUTPUT_HANDLE);
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                var standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                */
                init = true;
            }

            //Debugger.Launch();

            WriteLine("tlogThumbnailHandler ctor");
        }

        ~tlogThumbnailHandler()
        {
            try
            {
                WriteLine("tlogThumbnailHandler dtor");
            }
            catch { }

            Dispose();
        }

        public long GetLocation(out StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags)
        {
            WriteLine("tlogThumbnailHandler GetLocation");
            pszPathBuffer = new StringBuilder();
            pszPathBuffer.Append(m_filename);
            m_size = new Size(prgSize.cx, prgSize.cy);

            if (((IEIFLAG)pdwFlags & IEIFLAG.ASYNC) != 0)
                return E_PENDING;
            return S_OK;
        }

        public long Extract(out IntPtr phBmpThumbnail)
        {
            WriteLine("tlogThumbnailHandler Extract");
            string jpgfile = m_filename + ".jpg";
            phBmpThumbnail = IntPtr.Zero;

            try
            {
                var Core = new GMap.NET.Internals.Core();
                GMaps.Instance.UseMemoryCache = false;
                GMaps.Instance.CacheOnIdleRead = false;
                GMaps.Instance.BoostCacheEngine = true;
                GMap.NET.GMaps.Instance.PrimaryCache = new MissionPlanner.Maps.MyImageCache();
                Core.Provider = GMapProviders.GoogleSatelliteMap;

                if (!File.Exists(jpgfile))
                    LogMap.ProcessFile(m_filename);

                Bitmap bmp = new Bitmap(m_size.Width, m_size.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    WriteLine("doing " + jpgfile);

                    if (File.Exists(jpgfile))
                    {
                        using (Image map = Bitmap.FromFile(m_filename + ".jpg"))
                        {
                            g.DrawImage(map, 0, 0, m_size.Width, m_size.Height);
                        }
                    }
                    else
                    {
                        return E_FAIL;
                    }

                    phBmpThumbnail = bmp.GetHbitmap();
                }

                WriteLine("tlogThumbnailHandler Extract Return");

                GC.Collect();

                return S_OK;
            }
            catch (Exception e)
            {
                WriteLine("{0}", e);
                return E_FAIL;
            }
        }

        public void AddFiletoGenerate(string filename)
        {
            try
            {
                if (!Directory.Exists(commonAppData))
                    Directory.CreateDirectory(commonAppData);

                var dir = Path.GetDirectoryName(queuefile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (var st = File.Open(queuefile,FileMode.Append))
                {
                    byte[] data = ASCIIEncoding.ASCII.GetBytes(m_filename + "\r\n");
                    st.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex) { WriteLine(ex.ToString()); }
        }

        public int GetDateStamp([In, Out] ref System.Runtime.InteropServices.ComTypes.FILETIME pDateStamp)
        {
            WriteLine("tlogThumbnailHandler GetDateStamp");
            FileInfo fi = new FileInfo(m_filename);
            pDateStamp = new System.Runtime.InteropServices.ComTypes.FILETIME();
            pDateStamp.dwLowDateTime = (int) fi.LastWriteTime.ToFileTime();
            pDateStamp.dwHighDateTime = (int) (fi.LastWriteTime.ToFileTime() >> 32);

            return (int)S_OK;
        }

        public long GetThumbnail(int squareLength, out IntPtr hBitmap, out int bitmapType)
        {
            //Debugger.Launch();

            WriteLine("tlogThumbnailHandler GetThumbnail");
            hBitmap = IntPtr.Zero;
            bitmapType = (int)WTS_ALPHATYPE.WTSAT_UNKNOWN;

            m_size = new Size(squareLength, squareLength);
            try
            {
                return Extract(out hBitmap);
            }
            catch (Exception e)
            {
                WriteLine("{0}", e);
                return E_FAIL;
            }
        }

        public void GetClassID(out Guid pClassID)
        {
            WriteLine("tlogThumbnailHandler GetClassID");
            throw new NotImplementedException();
        }

        public void GetCurFile(out string ppszFileName)
        {
            WriteLine("tlogThumbnailHandler GetCurFile");
            throw new NotImplementedException();
        }

        public int IsDirty()
        {
            WriteLine("tlogThumbnailHandler IsDirty");
            throw new NotImplementedException();
        }

        public void Load(string pszFileName, int dwMode)
        {
            WriteLine("tlogThumbnailHandler Load {0}", pszFileName);
            m_filename = pszFileName;
        }

        public void Save(string pszFileName, bool fRemember)
        {
            WriteLine("tlogThumbnailHandler Save");
            throw new NotImplementedException();
        }

        public void SaveCompleted(string pszFileName)
        {
            WriteLine("tlogThumbnailHandler SaveCompleted");
            throw new NotImplementedException();
        }

        private string tmpfile = "";

        public long Initialize(IStream stream, int grfMode)
        {
            WriteLine("tlogThumbnailHandler Initialize {0}", grfMode);

            var data = GetStreamContents(stream);

            stream = null;

            tmpfile = Path.GetTempFileName();

            if (data.Length > 20)
            {
                if (data[0] == 0xa3 && data[1] == 0x95)
                {
                    tmpfile += ".bin";
                }
                else if (data[8] == 0xfe || data[0] == 0xfe)
                {
                    tmpfile += ".tlog";
                }
                else
                {
                    tmpfile += ".tlog";
                }
            }

            WriteLine("tlogThumbnailHandler GetThumbnail {0}", tmpfile);

            File.WriteAllBytes(tmpfile, data);

            data = null;

            m_filename = tmpfile;

            return S_OK;
        }

        private byte[] GetStreamContents(IStream stream)
        {
            if (stream == null) return null;

            System.Runtime.InteropServices.ComTypes.STATSTG statData;
            stream.Stat(out statData, 1);

            byte[] Result = new byte[statData.cbSize];

            WriteLine("tlogThumbnailHandler GetStreamContents {0}", statData.cbSize);

            IntPtr P = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(UInt64)));
            try
            {
                stream.Read(Result, Result.Length, P);
            }
            finally
            {
                Marshal.FreeCoTaskMem(P);
            }
            return Result;
        }

        public void Dispose()
        {
            if (File.Exists(tmpfile))
                File.Delete(tmpfile);

            try
            {
                WriteLine("tlogThumbnailHandler Dispose");
            }
            catch { }
        }
    }
}
