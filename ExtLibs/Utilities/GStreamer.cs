using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using log4net;
using gsize = System.UInt64;
using GstClockTime = System.UInt64;
using guint = System.UInt32;

namespace MissionPlanner.Utilities
{
    public class GStreamer
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private event EventHandler<Bitmap> _onNewImage;
        public event EventHandler<Bitmap> OnNewImage
        {
            add { _onNewImage += value; }
            remove { _onNewImage -= value; }
        }

#pragma warning disable IDE1006 // Naming Styles

        public static class NativeMethods
        {
            public enum BackendEnum
            {
                Windows,
                Linux,
                Android
            }

            public static BackendEnum Backend
            {
                get
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) 
                        return BackendEnum.Windows;
                    if (Environment.OSVersion.Platform == PlatformID.Unix) 
                    {
                        var doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        if (doc.StartsWith("/data/user/"))
                            return BackendEnum.Android;
                        return BackendEnum.Linux; 
                    }

                    return BackendEnum.Windows;
                }
            }

            public static void gst_init(ref int argc, string[] argv)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_init(ref argc, argv);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_init(ref argc, argv);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_init(ref argc, argv);
                        break;
                }
            }

            public static bool gst_init_check(IntPtr argc, IntPtr argv, out IntPtr error)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_init_check(argc, argv, out error);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_init_check(argc, argv, out error);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_init_check(argc, argv, out error);
                }
            }


            public static void gst_version(out guint major,
              out guint minor,
              out guint micro,
              out guint nano)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_version(out major, out minor, out micro, out nano);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_version(out major, out minor, out micro, out nano);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_version(out major, out minor, out micro, out nano);
                        break;
                }
            }

            public static IntPtr gst_version_string()
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_version_string();
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_version_string();
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_version_string();
                }
            }


            public static UIntPtr gst_buffer_extract(IntPtr raw, UIntPtr offset, byte[] dest, UIntPtr size)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                }
            }

            public static IntPtr gst_bus_timed_pop_filtered(IntPtr raw, ulong timeout, int types)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_bus_timed_pop_filtered(raw, timeout, types);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_bus_timed_pop_filtered(raw, timeout, types);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_bus_timed_pop_filtered(raw, timeout, types);
                }
            }

            public static void gst_buffer_extract_dup(IntPtr raw, UIntPtr offset, UIntPtr size, out IntPtr dest,
              out UIntPtr dest_size)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_buffer_extract_dup(raw, offset, size, out dest, out dest_size);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_buffer_extract_dup(raw, offset, size, out dest, out dest_size);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_buffer_extract_dup(raw, offset, size, out dest, out dest_size);
                        break;
                }
            }



            public static GstStateChangeReturn gst_element_set_state(IntPtr pipeline, GstState gST_STATE_PLAYING)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_element_set_state(pipeline, gST_STATE_PLAYING);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_element_set_state(pipeline, gST_STATE_PLAYING);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_element_set_state(pipeline, gST_STATE_PLAYING);
                }
            }


            public static IntPtr gst_parse_launch(string cmdline, out IntPtr error)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_parse_launch(cmdline, out error);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_parse_launch(cmdline, out error);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_parse_launch(cmdline, out error);
                }
            }



            public static IntPtr gst_element_get_bus(IntPtr pipeline)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_element_get_bus(pipeline);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_element_get_bus(pipeline);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_element_get_bus(pipeline);
                }
            }


            public static void gst_debug_bin_to_dot_file(IntPtr pipeline, GstDebugGraphDetails details,
              string file_name)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_debug_bin_to_dot_file(pipeline, details, file_name);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_debug_bin_to_dot_file(pipeline, details, file_name);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_debug_bin_to_dot_file(pipeline, details, file_name);
                        break;
                }
            }




            public static IntPtr gst_app_sink_try_pull_sample(IntPtr appsink,
              GstClockTime timeout)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_app_sink_try_pull_sample(appsink, timeout);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_app_sink_try_pull_sample(appsink, timeout);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_app_sink_try_pull_sample(appsink, timeout);
                }
            }



            public static void gst_app_sink_set_max_buffers(IntPtr appsink, guint max)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_app_sink_set_max_buffers(appsink, max);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_app_sink_set_max_buffers(appsink, max);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_app_sink_set_max_buffers(appsink, max);
                        break;
                }
            }


            public static IntPtr gst_bin_get_by_name(IntPtr pipeline, string name)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_bin_get_by_name(pipeline, name);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_bin_get_by_name(pipeline, name);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_bin_get_by_name(pipeline, name);
                }
            }


            public static IntPtr gst_sample_get_buffer(IntPtr sample)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_sample_get_buffer(sample);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_sample_get_buffer(sample);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_sample_get_buffer(sample);
                }
            }


            public static IntPtr
              gst_sample_get_caps(IntPtr sample)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_sample_get_caps(sample);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_sample_get_caps(sample);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_sample_get_caps(sample);
                }
            }

            public static bool
              gst_structure_get_int(IntPtr structure,
                  string fieldname,
                  out int value)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_structure_get_int(structure, fieldname, out value);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_structure_get_int(structure, fieldname, out value);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_structure_get_int(structure, fieldname, out value);
                }
            }


            public static IntPtr
              gst_caps_get_structure(IntPtr caps,
                  guint index)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_caps_get_structure(caps, index);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_caps_get_structure(caps, index);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_caps_get_structure(caps, index);
                }
            }



            public static bool gst_buffer_map(IntPtr buffer, out GstMapInfo info, GstMapFlags GstMapFlags)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                }
            }


            public static void gst_buffer_unmap(IntPtr buffer, out GstMapInfo info)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_buffer_unmap(buffer, out info);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_buffer_unmap(buffer, out info);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_buffer_unmap(buffer, out info);
                        break;
                }
            }

            public static void gst_sample_unref(IntPtr sample)
            {
                gst_mini_object_unref(sample);
            }

            public static void gst_buffer_unref(IntPtr buffer)
            {
                gst_mini_object_unref(buffer);
            }



            public static void
              gst_mini_object_unref(IntPtr mini_object)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_mini_object_unref(mini_object);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_mini_object_unref(mini_object);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_mini_object_unref(mini_object);
                        break;
                }
            }


            public static bool gst_app_sink_is_eos(IntPtr appsink)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_app_sink_is_eos(appsink);
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_app_sink_is_eos(appsink);
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_app_sink_is_eos(appsink);
                }
            }


            public static void gst_app_sink_set_drop(IntPtr appsink, bool v)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        WinNativeMethods.gst_app_sink_set_drop(appsink, v);
                        break;
                    case BackendEnum.Linux:
                        LinuxNativeMethods.gst_app_sink_set_drop(appsink, v);
                        break;
                    case BackendEnum.Android:
                        AndroidNativeMethods.gst_app_sink_set_drop(appsink, v);
                        break;
                }
            }

            public static string gst_caps_to_string(IntPtr capsS)
            {
                IntPtr raw_ret;
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        raw_ret = WinNativeMethods.gst_caps_to_string(capsS);
                        break;
                    case BackendEnum.Linux:
                        raw_ret = LinuxNativeMethods.gst_caps_to_string(capsS);
                        break;
                    case BackendEnum.Android:
                        raw_ret = AndroidNativeMethods.gst_caps_to_string(capsS);
                        break;
                }

                string ret = PtrToStringGFree(raw_ret);
                return ret;
            }


            public static string Utf8PtrToString(IntPtr ptr)
            {
                if (ptr == IntPtr.Zero)
                    return null;

                int len = (int)(uint)strlen(ptr);
                byte[] bytes = new byte[len];
                Marshal.Copy(ptr, bytes, 0, len);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }

            public static string[] Utf8PtrToString(IntPtr[] ptrs)
            {
                // The last pointer is a null terminator.
                string[] ret = new string[ptrs.Length - 1];
                for (int i = 0; i < ret.Length; i++)
                    ret[i] = Utf8PtrToString(ptrs[i]);
                return ret;
            }

            public static string PtrToStringGFree(IntPtr ptr)
            {
                string ret = Utf8PtrToString(ptr);
                return ret;
            }

            public static string[] PtrToStringGFree(IntPtr[] ptrs)
            {
                // The last pointer is a null terminator.
                string[] ret = new string[ptrs.Length - 1];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = Utf8PtrToString(ptrs[i]);
                }
                return ret;
            }

            static ulong strlen(IntPtr s)
            {
                ulong cnt = 0;
                byte b = Marshal.ReadByte(s, (int)cnt);
                while (b != 0)
                {
                    cnt++;
                    b = Marshal.ReadByte(s, (int)cnt);
                }
                return cnt;
            }
        }

        public static class AndroidNativeMethods
        {
            public const string lib = "libgstreamer_android.so";

            public const string applib = "libgstreamer_android.so";

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, ref IntPtr[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(IntPtr argc, IntPtr argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, string[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(ref int argc, ref IntPtr[] argv, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(IntPtr argc, IntPtr argv, out IntPtr error);


            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_version(out guint major,
                out guint minor,
                out guint micro,
                out guint nano);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_version_string();

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern UIntPtr gst_buffer_extract(IntPtr raw, UIntPtr offset, byte[] dest, UIntPtr size);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr raw, ulong timeout, int types);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_extract_dup(IntPtr raw, UIntPtr offset, UIntPtr size, out IntPtr dest,
                out UIntPtr dest_size);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_pipeline_new(string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_factory_make(string factoryname, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_parse_error(IntPtr msg, out IntPtr err, out IntPtr debug);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_message_get_stream_status_object(IntPtr raw);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern GstStateChangeReturn gst_element_set_state(IntPtr pipeline, GstState gST_STATE_PLAYING);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_parse_launch(string cmdline, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr bus, ulong gST_CLOCK_TIME_NONE,
                GstMessageType gstMessageType);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_get_bus(IntPtr pipeline);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_debug_bin_to_dot_file(IntPtr pipeline, GstDebugGraphDetails details,
                string file_name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_set_stream_status_object(IntPtr raw, IntPtr value);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_app_sink_try_pull_sample(IntPtr appsink,
                GstClockTime timeout);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_app_sink_get_caps(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_max_buffers(IntPtr appsink, guint max);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bin_get_by_name(IntPtr pipeline, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_sample_get_buffer(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_caps(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_info(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                StringBuilder
                gst_structure_to_string(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool
                gst_structure_get_int(IntPtr structure,
                    string fieldname,
                    out int value);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_caps_get_structure(IntPtr caps,
                    guint index);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                IntPtr gst_caps_to_string(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_buffer_map(IntPtr buffer, out GstMapInfo info, GstMapFlags GstMapFlags);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_unmap(IntPtr buffer, out GstMapInfo info);

            public static void gst_sample_unref(IntPtr sample)
            {
                gst_mini_object_unref(sample);
            }

            public static void gst_buffer_unref(IntPtr buffer)
            {
                gst_mini_object_unref(buffer);
            }

            [DllImport(lib, CallingConvention = CallingConvention.StdCall)]
            public static extern void
                gst_caps_unref(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_structure_free(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void
                gst_mini_object_unref(IntPtr mini_object);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_app_sink_is_eos(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_drop(IntPtr appsink, bool v);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_callbacks(IntPtr appsink, GstAppSinkCallbacks callbacks,
                IntPtr user_data, IntPtr notify);
        }

        public static class LinuxNativeMethods
        {
            public const string lib = "libgstreamer-1.0.so.0";

            public const string applib = "libgstapp-1.0.so.0";

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, ref IntPtr[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(IntPtr argc, IntPtr argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, string[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(ref int argc, ref IntPtr[] argv, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(IntPtr argc, IntPtr argv, out IntPtr error);


            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_version(out guint major,
                out guint minor,
                out guint micro,
                out guint nano);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_version_string();

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern UIntPtr gst_buffer_extract(IntPtr raw, UIntPtr offset, byte[] dest, UIntPtr size);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr raw, ulong timeout, int types);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_extract_dup(IntPtr raw, UIntPtr offset, UIntPtr size, out IntPtr dest,
                out UIntPtr dest_size);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_pipeline_new(string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_factory_make(string factoryname, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_parse_error(IntPtr msg, out IntPtr err, out IntPtr debug);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_message_get_stream_status_object(IntPtr raw);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern GstStateChangeReturn gst_element_set_state(IntPtr pipeline, GstState gST_STATE_PLAYING);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_parse_launch(string cmdline, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr bus, ulong gST_CLOCK_TIME_NONE,
                GstMessageType gstMessageType);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_get_bus(IntPtr pipeline);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_debug_bin_to_dot_file(IntPtr pipeline, GstDebugGraphDetails details,
                string file_name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_set_stream_status_object(IntPtr raw, IntPtr value);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_app_sink_try_pull_sample(IntPtr appsink,
                GstClockTime timeout);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_app_sink_get_caps(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_max_buffers(IntPtr appsink, guint max);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bin_get_by_name(IntPtr pipeline, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_sample_get_buffer(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_caps(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_info(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                StringBuilder
                gst_structure_to_string(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool
                gst_structure_get_int(IntPtr structure,
                    string fieldname,
                    out int value);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_caps_get_structure(IntPtr caps,
                    guint index);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                IntPtr gst_caps_to_string(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_buffer_map(IntPtr buffer, out GstMapInfo info, GstMapFlags GstMapFlags);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_unmap(IntPtr buffer, out GstMapInfo info);

            public static void gst_sample_unref(IntPtr sample)
            {
                gst_mini_object_unref(sample);
            }

            public static void gst_buffer_unref(IntPtr buffer)
            {
                gst_mini_object_unref(buffer);
            }

            [DllImport(lib, CallingConvention = CallingConvention.StdCall)]
            public static extern void
                gst_caps_unref(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_structure_free(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void
                gst_mini_object_unref(IntPtr mini_object);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_app_sink_is_eos(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_drop(IntPtr appsink, bool v);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_callbacks(IntPtr appsink, GstAppSinkCallbacks callbacks,
                IntPtr user_data, IntPtr notify);
        }

        public static class WinNativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]

            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetDllDirectory(string lpPathName);

            public const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultDllDirectories(uint DirectoryFlags);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern int AddDllDirectory(string NewDirectory);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);


            public const string lib = "libgstreamer-1.0-0.dll";

            public const string applib = "libgstapp-1.0-0.dll";

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, ref IntPtr[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(IntPtr argc, IntPtr argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_init(ref int argc, string[] argv);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(ref int argc, ref IntPtr[] argv, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_init_check(IntPtr argc, IntPtr argv, out IntPtr error);


            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_version(out guint major,
                out guint minor,
                out guint micro,
                out guint nano);

            [DllImport (lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_version_string ();

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern UIntPtr gst_buffer_extract(IntPtr raw, UIntPtr offset, byte[] dest, UIntPtr size);



            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr raw, ulong timeout, int types);


            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_extract_dup(IntPtr raw, UIntPtr offset, UIntPtr size, out IntPtr dest,
                out UIntPtr dest_size);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_pipeline_new(string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_factory_make(string factoryname, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_parse_error(IntPtr msg, out IntPtr err, out IntPtr debug);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_message_get_stream_status_object(IntPtr raw);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern GstStateChangeReturn gst_element_set_state(IntPtr pipeline, GstState gST_STATE_PLAYING);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_parse_launch(string cmdline, out IntPtr error);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bus_timed_pop_filtered(IntPtr bus, ulong gST_CLOCK_TIME_NONE,
                GstMessageType gstMessageType);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_element_get_bus(IntPtr pipeline);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_debug_bin_to_dot_file(IntPtr pipeline, GstDebugGraphDetails details,
                string file_name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_message_set_stream_status_object(IntPtr raw, IntPtr value);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_app_sink_try_pull_sample(IntPtr appsink,
                GstClockTime timeout);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_app_sink_get_caps(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_max_buffers(IntPtr appsink, guint max);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_bin_get_by_name(IntPtr pipeline, string name);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gst_sample_get_buffer(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_caps(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_sample_get_info(IntPtr sample);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                StringBuilder
                gst_structure_to_string(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool
                gst_structure_get_int(IntPtr structure,
                    string fieldname,
                    out int value);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr
                gst_caps_get_structure(IntPtr caps,
                    guint index);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern
                IntPtr gst_caps_to_string(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_buffer_map(IntPtr buffer, out GstMapInfo info, GstMapFlags GstMapFlags);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_buffer_unmap(IntPtr buffer, out GstMapInfo info);

            public static void gst_sample_unref(IntPtr sample)
            {
                gst_mini_object_unref(sample);
            }

            public static void gst_buffer_unref(IntPtr buffer)
            {
                gst_mini_object_unref(buffer);
            }

            [DllImport(lib, CallingConvention = CallingConvention.StdCall)]
            public static extern void
                gst_caps_unref(IntPtr caps);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_structure_free(IntPtr structure);

            [DllImport(lib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void
                gst_mini_object_unref(IntPtr mini_object);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool gst_app_sink_is_eos(IntPtr appsink);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_drop(IntPtr appsink, bool v);

            [DllImport(applib, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gst_app_sink_set_callbacks(IntPtr appsink, GstAppSinkCallbacks callbacks,
                IntPtr user_data, IntPtr notify);
        }
#pragma warning restore IDE1006 // Naming Styles

        public const UInt64 GST_CLOCK_TIME_NONE = 18446744073709551615;

        public const UInt64 G_USEC_PER_SEC = 1000000;
        public const UInt64 GST_SECOND = ((G_USEC_PER_SEC * (1000)));

        [StructLayout(LayoutKind.Sequential, Size = 200)]
        public struct GstMapInfo
        {
            public IntPtr memory;
            public GstMapFlags flags;
            public IntPtr data;
            public gsize size;

            public gsize maxsize;
        }

        public delegate void eos(IntPtr sink, IntPtr user_data);

        public delegate GstFlowReturn new_preroll(IntPtr sink, IntPtr user_data);

        public delegate GstFlowReturn new_buffer(IntPtr sink, IntPtr user_data);

        public delegate GstFlowReturn new_buffer_list(IntPtr sink, IntPtr user_data);

        public struct GstAppSinkCallbacks
        {
            public eos eos; //void (* eos) (GstAppSink* sink, gpointer user_data);
            public new_preroll new_preroll; //GstFlowReturn(*new_preroll)      (GstAppSink* sink, gpointer user_data);
            public new_buffer new_buffer; //GstFlowReturn(*new_buffer)       (GstAppSink* sink, gpointer user_data);

            public IntPtr _gst_reserved1;
            public IntPtr _gst_reserved2;
            public IntPtr _gst_reserved3;
            public IntPtr _gst_reserved4;
        }

        public enum GstFlowReturn
        {
            /* custom success starts here */
            GST_FLOW_CUSTOM_SUCCESS_2 = 102,
            GST_FLOW_CUSTOM_SUCCESS_1 = 101,
            GST_FLOW_CUSTOM_SUCCESS = 100,

            /* core predefined */
            GST_FLOW_RESEND = 1,
            GST_FLOW_OK = 0,

            /* expected failures */
            GST_FLOW_NOT_LINKED = -1,
            GST_FLOW_WRONG_STATE = -2,

            /* error cases */
            GST_FLOW_UNEXPECTED = -3,
            GST_FLOW_NOT_NEGOTIATED = -4,
            GST_FLOW_ERROR = -5,
            GST_FLOW_NOT_SUPPORTED = -6,

            /* custom error starts here */
            GST_FLOW_CUSTOM_ERROR = -100,
            GST_FLOW_CUSTOM_ERROR_1 = -101,
            GST_FLOW_CUSTOM_ERROR_2 = -102
        }


        public enum GstDebugGraphDetails
        {

            GST_DEBUG_GRAPH_SHOW_MEDIA_TYPE = (1 << 0),

            GST_DEBUG_GRAPH_SHOW_CAPS_DETAILS = (1 << 1),

            GST_DEBUG_GRAPH_SHOW_NON_DEFAULT_PARAMS = (1 << 2),

            GST_DEBUG_GRAPH_SHOW_STATES = (1 << 3),

            GST_DEBUG_GRAPH_SHOW_FULL_PARAMS = (1 << 4),

            GST_DEBUG_GRAPH_SHOW_ALL = ((1 << 4) - 1),

            GST_DEBUG_GRAPH_SHOW_VERBOSE = (-1)

        }

        public enum GstMapFlags
        {
            GST_MAP_READ = 1,
            GST_MAP_WRITE = 2,
            GST_MAP_FLAG_LAST = 65536
        }

        public enum GstState
        {
            GST_STATE_VOID_PENDING = 0,
            GST_STATE_NULL = 1,
            GST_STATE_READY = 2,
            GST_STATE_PAUSED = 3,
            GST_STATE_PLAYING = 4
        }

        public enum GstStateChangeReturn
        {
            GST_STATE_CHANGE_FAILURE = 0,
            GST_STATE_CHANGE_SUCCESS = 1,
            GST_STATE_CHANGE_ASYNC = 2,
            GST_STATE_CHANGE_NO_PREROLL = 3
        }

        public enum GstMessageType
        {
            GST_MESSAGE_UNKNOWN = 0,
            GST_MESSAGE_EOS = (1 << 0),
            GST_MESSAGE_ERROR = (1 << 1),
            GST_MESSAGE_WARNING = (1 << 2),
            GST_MESSAGE_INFO = (1 << 3),
            GST_MESSAGE_TAG = (1 << 4),
            GST_MESSAGE_BUFFERING = (1 << 5),
            GST_MESSAGE_STATE_CHANGED = (1 << 6),
            GST_MESSAGE_STATE_DIRTY = (1 << 7),
            GST_MESSAGE_STEP_DONE = (1 << 8),
            GST_MESSAGE_CLOCK_PROVIDE = (1 << 9),
            GST_MESSAGE_CLOCK_LOST = (1 << 10),
            GST_MESSAGE_NEW_CLOCK = (1 << 11),
            GST_MESSAGE_STRUCTURE_CHANGE = (1 << 12),
            GST_MESSAGE_STREAM_STATUS = (1 << 13),
            GST_MESSAGE_APPLICATION = (1 << 14),
            GST_MESSAGE_ELEMENT = (1 << 15),
            GST_MESSAGE_SEGMENT_START = (1 << 16),
            GST_MESSAGE_SEGMENT_DONE = (1 << 17),
            GST_MESSAGE_DURATION = (1 << 18),
            GST_MESSAGE_LATENCY = (1 << 19),
            GST_MESSAGE_ASYNC_START = (1 << 20),
            GST_MESSAGE_ASYNC_DONE = (1 << 21),
            GST_MESSAGE_REQUEST_STATE = (1 << 22),
            GST_MESSAGE_STEP_START = (1 << 23),
            GST_MESSAGE_QOS = (1 << 24),
            GST_MESSAGE_PROGRESS = (1 << 25),
            GST_MESSAGE_ANY = ~0
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GError
        {
            public UInt32 domain; // typedef guint32 GQuark;
            public int code;
            public string message;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public Thread StartA(string stringpipeline)
        {
            var th = new Thread(ThreadStart) {IsBackground = true, Name = "gstreamer"};

            th.Start(stringpipeline);

            return th;
        }


        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        void ThreadStart(object datao)
        {
            string stringpipeline = (string)datao;
            int argc = 1;
            string[] argv = new string[] { "-vvv" };

            Environment.SetEnvironmentVariable("GST_DEBUG", "*:4");

            try
            {
                //https://github.com/GStreamer/gstreamer/blob/master/tools/gst-launch.c#L1125
                NativeMethods.gst_init(ref argc, argv);
            }
            catch (DllNotFoundException ex)
            {
                CustomMessageBox.Show("The file was not found at " + GstLaunch +
                                      "\nPlease verify permissions " + ex.ToString());
                return;
            }
            catch (BadImageFormatException)
            {
                CustomMessageBox.Show("The incorrect exe architecture has been detected at " + GstLaunch +
                                      "\nPlease install gstreamer for the correct architecture");
                return;
            }

            NativeMethods.gst_version(out uint v1, out uint v2, out uint v3, out uint v4);

            log.InfoFormat("GStreamer {0}.{1}.{2}.{3}", v1, v2, v3, v4);

            NativeMethods.gst_init_check(IntPtr.Zero, IntPtr.Zero, out IntPtr error);

            if (error != IntPtr.Zero)
            {
                var er = Marshal.PtrToStructure<GError>(error);
                log.Error("gst_init_check: " + er.message);
                return;
            }

            /* Set up the pipeline */

            log.InfoFormat("GStreamer parse {0}", stringpipeline);
            var pipeline = NativeMethods.gst_parse_launch(
                stringpipeline,
                out error);

            if (error != IntPtr.Zero)
            {
                var er = Marshal.PtrToStructure<GError>(error);
                log.Error("gst_parse_launch: " + er.message);
                return;
            }

            NativeMethods.gst_debug_bin_to_dot_file(pipeline, GstDebugGraphDetails.GST_DEBUG_GRAPH_SHOW_ALL,
                "pipeline");

            log.Info("graphviz of pipeline is at " + Path.GetTempPath() + "pipeline.dot");

            // appsink is part of the parse launch
            var appsink = NativeMethods.gst_bin_get_by_name(pipeline, "outsink");
            log.Info("got appsink ");

            GstAppSinkCallbacks callbacks = new GstAppSinkCallbacks();

            if (appsink != IntPtr.Zero)
            {
                NativeMethods.gst_app_sink_set_drop(appsink, true);
                NativeMethods.gst_app_sink_set_max_buffers(appsink, 1);
                log.Info("set appsink params ");
            }

            /* Start playing */
            var running = NativeMethods.gst_element_set_state(pipeline, GstState.GST_STATE_PLAYING) != GstStateChangeReturn.GST_STATE_CHANGE_FAILURE;
            log.Info("set playing ");
            /* Wait until error or EOS */
            var bus = NativeMethods.gst_element_get_bus(pipeline);
                      

            int Width = 0;
            int Height = 0;
            // prevent it falling out of scope
            int trys = 0;
            GstAppSinkCallbacks callbacks2 = callbacks;

            run = true;

            // not using appsink
            if (appsink == IntPtr.Zero && running)
            {
                /* Wait until error or EOS */
                NativeMethods.gst_bus_timed_pop_filtered(bus, GST_CLOCK_TIME_NONE,
                    (int)(GstMessageType.GST_MESSAGE_ERROR | GstMessageType.GST_MESSAGE_EOS));
                run = false;

            }
            else {
                var msg = NativeMethods.gst_bus_timed_pop_filtered(bus, 0,
                     (int)(GstMessageType.GST_MESSAGE_ERROR | GstMessageType.GST_MESSAGE_EOS));
                if (msg != IntPtr.Zero)
                    run = false;
            }

            log.Info("start frame loop gst_app_sink_is_eos");
            while (run && !NativeMethods.gst_app_sink_is_eos(appsink))
            {
                try
                {
                        var sample = NativeMethods.gst_app_sink_try_pull_sample(appsink, GST_SECOND * 5);
                    if (sample != IntPtr.Zero)
                    {
                        trys = 0;
                        var caps = NativeMethods.gst_sample_get_caps(sample);
                        var caps_s = NativeMethods.gst_caps_get_structure(caps, 0);
                        NativeMethods.gst_structure_get_int(caps_s, "width", out Width);
                        NativeMethods.gst_structure_get_int(caps_s, "height", out Height);

                        var capsstring = NativeMethods.gst_caps_to_string(caps_s);
                        var buffer = NativeMethods.gst_sample_get_buffer(sample);
                        if (buffer != IntPtr.Zero)
                        {
                            var info = new GstMapInfo();
                            if (NativeMethods.gst_buffer_map(buffer, out info, GstMapFlags.GST_MAP_READ))
                            {
                                var image = new Bitmap(Width, Height, 4 * Width, SkiaSharp.SKColorType.Bgra8888,
                                    info.data);

                                _onNewImage?.Invoke(null, image);
                            }

                            NativeMethods.gst_buffer_unmap(buffer, out info);
                        }

                        NativeMethods.gst_sample_unref(sample);
                    }
                    else
                    {
                        log.Info("failed gst_app_sink_try_pull_sample " + trys + "");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    trys++;
                    if (trys > 12) break;
                }
            }

            if (!NativeMethods.gst_app_sink_is_eos(appsink))
                NativeMethods.gst_element_set_state(pipeline, GstState.GST_STATE_NULL);
            NativeMethods.gst_buffer_unref(bus);

            // cleanup
            _onNewImage?.Invoke(null, null);

            log.Info("Gstreamer Exit");
        }

        ~GStreamer()
        {
            Stop();
        }

        static GStreamer()
        {
            var dataDirectory = Settings.GetDataDirectory();
            var gstdir = Path.Combine(dataDirectory, @"gstreamer\1.0\x86_64\bin\libgstreamer-1.0-0.dll");

            SetGSTPath(gstdir);
        }

        private static void SetGSTPath(string gstdir)
        {
            var orig = gstdir;

            if (!File.Exists(gstdir))
                return;

            gstdir = Path.GetDirectoryName(gstdir);
            gstdir = Path.GetDirectoryName(gstdir);

            // Prepend native path to environment path, to ensure the
            // right libs are being used. - worsk in 461, break in 48
            var path = Environment.GetEnvironmentVariable("PATH");
            if (!path.Contains(gstdir))
            {
                path = Path.Combine(gstdir, "bin") + ";" + Path.Combine(gstdir, "lib") + ";" + path;

                Environment.SetEnvironmentVariable("PATH", path);
            }

            Environment.SetEnvironmentVariable("GSTREAMER_ROOT", gstdir);

            Environment.SetEnvironmentVariable("GSTREAMER_1_0_ROOT_X86_64", gstdir);

            Environment.SetEnvironmentVariable("GST_PLUGIN_PATH", Path.Combine(gstdir, "lib"));

            Environment.SetEnvironmentVariable("GST_DEBUG_DUMP_DOT_DIR", Path.GetTempPath());

            try
            {
                // fix for 48
                WinNativeMethods.SetDefaultDllDirectories(WinNativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                WinNativeMethods.AddDllDirectory(Path.Combine(gstdir, "bin"));
                WinNativeMethods.LoadLibrary(orig);
            }catch { }            
        }

        public static string GstLaunch
        {
            get { return Settings.Instance["gstlaunchexe"]; }
            set { Settings.Instance["gstlaunchexe"] = value; }
        }

        public static bool GstLaunchExists
        {
            get
            {
                if (Android)
                    return true;
                return File.Exists(GstLaunch); 
            }
        }

        public static string LookForGstreamer()
        {
            List<string> dirs = new List<string>
            {
                // linux
                "/usr/lib/x86_64-linux-gnu",
                // rpi
                "/usr/lib/arm-linux-gnueabihf",
                // current
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                // settings
                Settings.GetDataDirectory(),
                // custom settings
                Settings.CustomUserDataDirectory,
                // sitl bindle
                GStreamer.BundledPath
            };

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.Fixed)
                {
                    dirs.Add(d.RootDirectory.Name + "gstreamer");
                    dirs.Add(d.RootDirectory.Name + "Program Files" + Path.DirectorySeparatorChar + "gstreamer");
                    dirs.Add(d.RootDirectory.Name + "Program Files (x86)" + Path.DirectorySeparatorChar + "gstreamer");                    
                }
            }

            var is64bit = Environment.Is64BitProcess;

            foreach (var dir in dirs)
            {
                log.Info($"look in dir {dir}");
                if (Directory.Exists(dir))
                {
                    var ans = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Where(a => a.ToLower().Contains("libgstreamer-1.0-0.dll") || a.ToLower().Contains("libgstreamer-1.0.so.0") || a.ToLower().Contains("libgstreamer_android.so")).ToArray();

                    ans = ans.Where(a =>
                        (!is64bit && !a.ToLower().Contains("_64")) || // windows
                        is64bit && a.ToLower().Contains("_64") || // windows
                        a.ToLower().Contains(".so.") // linux/rpi
                        ).ToArray();

                    if (ans.Length > 0)
                    {
                        log.Info("Found gstreamer " + ans.First());
                        SetGSTPath(ans.First());
                        try
                        {
                            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
                            NativeMethods.gst_version(out v1, out v2, out v3, out v4);

                            log.InfoFormat("GStreamer {0}.{1}.{2}.{3}", v1, v2, v3, v4);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex); 
                            return "";
                        }

                        return ans.First();
                    }
                }
            }

            log.Info("No gstreamer found");
            return "";
        }

        // custom search path for .so
        public static string BundledPath { get; set; }
        public static bool Android { get; set; }

        private bool run = true;
        public void Stop()
        {
            run = false;
            Thread.Sleep(50);
        }

        public static void DownloadGStreamer(Action<int, string> status = null)
        {
            if (RuntimeInformation.OSArchitecture == Architecture.Arm || RuntimeInformation.OSArchitecture == Architecture.Arm64)
                return;

            string output;
            string url;
            if (System.Environment.Is64BitProcess)
            {
                output = Settings.GetDataDirectory() + "gstreamer-1.0-x86_64-1.14.4.zip";
                url = "https://firmware.ardupilot.org/MissionPlanner/gstreamer/gstreamer-1.0-x86_64-1.14.4.zip";
            }
            else
            {
                output = Settings.GetDataDirectory() + "gstreamer-1.0-x86-1.14.4.zip";
                url = "https://firmware.ardupilot.org/MissionPlanner/gstreamer/gstreamer-1.0-x86-1.14.4.zip";
            }


            status?.Invoke(0, "Downloading..");
            int retry = 3;

            while (retry > 0)
            {
                try
                {
                    if (Download.getFilefromNet(url, output, status: status))
                    {

                        status?.Invoke(50, "Extracting..");
                        ZipFile.ExtractToDirectory(output, Settings.GetDataDirectory());
                        status?.Invoke(100, "Done.");

                        break;
                    }
                }
                catch (Exception ex)
                {
                    status?.Invoke(-1, "Error downloading file " + ex.ToString());
                    try
                    {
                        if (File.Exists(output))
                            File.Delete(output);
                    }
                    catch
                    {
                    }
                    status?.Invoke(-1, "Retry");
                } 
                retry--;
            }
        }
    }
}