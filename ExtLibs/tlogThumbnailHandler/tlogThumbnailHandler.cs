using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace tlogThumbnailHandler
{
    //C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe /regfile tlogThumbnailHandler.dll

    //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\regasm /regfile tlogThumbnailHandler.dll

    // reg add HKEY_CLASSES_ROOT\.tlog\shellex\{BB2E617C-0920-11D1-9A0B-00C04FC2D6C1} /d {f3b857f1-0b79-4e77-9d0b-8b8b7e874f56}

    // reg add HKEY_CLASSES_ROOT\MissionPlanner.tlog\shellex\{BB2E617C-0920-11D1-9A0B-00C04FC2D6C1} /d {f3b857f1-0b79-4e77-9d0b-8b8b7e874f56}

    enum IEIFLAG
    {
        ASYNC = 0x0001, // ask the extractor if it supports ASYNC extract (free threaded)      
        CACHE = 0x0002, // returned from the extractor if it does NOT cache the thumbnail      
        ASPECT = 0x0004, // passed to the extractor to beg it to render to the aspect ratio of the supplied rect       
        OFFLINE = 0x0008, // if the extractor shouldn't hit the net to get any content neede for the rendering     
        GLEAM = 0x0010, // does the image have a gleam ? this will be returned if it does       
        SCREEN = 0x0020, // render as if for the screen (this is exlusive with IEIFLAG_ASPECT )      
        ORIGSIZE = 0x0040, // render to the approx size passed, but crop if neccessary       
        NOSTAMP = 0x0080, // returned from the extractor if it does NOT want an icon stamp on the thumbnail    
        NOBORDER = 0x0100, // returned from the extractor if it does NOT want an a border around the thumbnail    
        QUALITY = 0x0200 // passed to the Extract method to indicate that a slower, higher quality image is desired, re-compute the thumbnail    
    }

    [ComVisible(false)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [ComImport]
    [GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage
    {       
        [PreserveSig]
        long GetLocation(out StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags);
   
        [PreserveSig]
        long Extract(out IntPtr phBmpThumbnail);
    }

    [ComImport]
    [Guid("953BB1EE-93B4-11d1-98A3-00C04FB687DA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage2 : IExtractImage
    {
        int GetDateStamp([In, Out]ref System.Runtime.InteropServices.ComTypes.FILETIME pDateStamp);
    }

    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    //[ProgId("IExtractImage.TxtThumbnailHandler")]
    [Guid("f3b857f1-0b79-4e77-9d0b-8b8b7e874f56")]
    public class tlogThumbnailHandler : IExtractImage, IPersistFile
    {
        private Size m_size = Size.Empty; 
        private string m_filename = String.Empty;
        private const long S_OK = 0x00000000L;
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

        public tlogThumbnailHandler()
        {
            /*
            NativeMethods.AllocConsole();
            IntPtr stdHandle = NativeMethods.GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
             */
            Console.WriteLine("tlogThumbnailHandler ctor");
        }

        public long GetLocation(out StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags)
        {
            Console.WriteLine("tlogThumbnailHandler GetLocation");
            pszPathBuffer = new StringBuilder();
            pszPathBuffer.Append(m_filename);
            m_size = new Size(prgSize.cx, prgSize.cy);

            if (((IEIFLAG)pdwFlags & IEIFLAG.ASYNC) != 0)
                return E_PENDING;
            return S_OK;
        }

        public long Extract(out IntPtr phBmpThumbnail)
        {
            Console.WriteLine("tlogThumbnailHandler Extract");
            string jpgfile = m_filename + ".jpg";

            Bitmap bmp = new Bitmap(m_size.Width, m_size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                Console.WriteLine("doing " + jpgfile);

                if (File.Exists(jpgfile))
                {
                    using (FileStream stream = File.OpenRead(jpgfile))
                    {
                        using (Image map = Bitmap.FromFile(m_filename + ".jpg"))
                        {
                            g.DrawImage(map, 0, 0, m_size.Width, m_size.Height);
                        }
                    }
                }
                else
                {
                    AddFiletoGenerate(m_filename);

                    g.DrawString("No Map\nGenerated", SystemFonts.DefaultFont, Brushes.Black, new PointF(0, 0));
                }
            }

            phBmpThumbnail = bmp.GetHbitmap();

            return S_OK;
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
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void GetClassID(out Guid pClassID)
        {
            Console.WriteLine("tlogThumbnailHandler GetClassID");
            throw new NotImplementedException();
        }

        public void GetCurFile(out string ppszFileName)
        {
            Console.WriteLine("tlogThumbnailHandler GetCurFile");
            throw new NotImplementedException();
        }

        public int IsDirty()
        {
            Console.WriteLine("tlogThumbnailHandler IsDirty");
            throw new NotImplementedException();
        }

        public void Load(string pszFileName, int dwMode)
        {
            Console.WriteLine("tlogThumbnailHandler Load");
            m_filename = pszFileName;
        }

        public void Save(string pszFileName, bool fRemember)
        {
            Console.WriteLine("tlogThumbnailHandler Save");
            throw new NotImplementedException();
        }

        public void SaveCompleted(string pszFileName)
        {
            Console.WriteLine("tlogThumbnailHandler SaveCompleted");
            throw new NotImplementedException();
        }
    }

}
