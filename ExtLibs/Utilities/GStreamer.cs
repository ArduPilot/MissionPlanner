using log4net;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using gsize = System.UInt64;
using GstClockTime = System.UInt64;
using guint = System.UInt32;

namespace MissionPlanner.Utilities
{
    public class GStreamer
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<Process> processList = new List<Process>();

        static object _lock = new object();

        private static event EventHandler<Bitmap> _onNewImage;

        public static event EventHandler<Bitmap> onNewImage
        {
            add { _onNewImage += value; }
            remove { _onNewImage -= value; }
        }

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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_init_check(argc, argv, out error);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_init_check(argc, argv, out error);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_version_string();
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_version_string();
                        break;
                }
            }


            public static UIntPtr gst_buffer_extract(IntPtr raw, UIntPtr offset, byte[] dest, UIntPtr size)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_buffer_extract(raw, offset, dest, size);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_element_set_state(pipeline, gST_STATE_PLAYING);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_element_set_state(pipeline, gST_STATE_PLAYING);
                        break;
                }
            }


            public static IntPtr gst_parse_launch(string cmdline, out IntPtr error)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_parse_launch(cmdline, out error);
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_parse_launch(cmdline, out error);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_parse_launch(cmdline, out error);
                        break;
                }
            }



            public static IntPtr gst_element_get_bus(IntPtr pipeline)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_element_get_bus(pipeline);
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_element_get_bus(pipeline);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_element_get_bus(pipeline);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_app_sink_try_pull_sample(appsink, timeout);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_app_sink_try_pull_sample(appsink, timeout);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_bin_get_by_name(pipeline, name);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_bin_get_by_name(pipeline, name);
                        break;
                }
            }


            public static IntPtr gst_sample_get_buffer(IntPtr sample)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_sample_get_buffer(sample);
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_sample_get_buffer(sample);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_sample_get_buffer(sample);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_sample_get_caps(sample);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_sample_get_caps(sample);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_structure_get_int(structure, fieldname, out value);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_structure_get_int(structure, fieldname, out value);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_caps_get_structure(caps, index);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_caps_get_structure(caps, index);
                        break;
                }
            }



            public static bool gst_buffer_map(IntPtr buffer, out GstMapInfo info, GstMapFlags GstMapFlags)
            {
                switch (Backend)
                {
                    default:
                    case BackendEnum.Windows:
                        return WinNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_buffer_map(buffer, out info, GstMapFlags);
                        break;
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
                        break;
                    case BackendEnum.Linux:
                        return LinuxNativeMethods.gst_app_sink_is_eos(appsink);
                        break;
                    case BackendEnum.Android:
                        return AndroidNativeMethods.gst_app_sink_is_eos(appsink);
                        break;
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
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public IntPtr[] user_data; //4
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public IntPtr[] _gst_reserved; //4
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
        public struct GstObject
        {
            IntPtr _lock;
            public string name;
            public Object parent;
            public uint flags;
            IntPtr controlBindings;
            public int control_rate;
            public int last_sync;

            private IntPtr[] _gstGstReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GError
        {
            public UInt32 domain; // typedef guint32 GQuark;
            public int code;
            public string message;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static Thread StartA(string stringpipeline)
        {
            var th = new Thread(ThreadStart) {IsBackground = true, Name = "gstreamer"};

            th.Start(stringpipeline);

            return th;
        }


        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        static void ThreadStart(object datao)
        {
            string stringpipeline = (string)datao;
            int argc = 1;
            string[] argv = new string[] { "-vvv" };

            try
            {
                

                //https://github.com/GStreamer/gstreamer/blob/master/tools/gst-launch.c#L1125
                NativeMethods.gst_init(ref argc, argv);
            }
            catch (DllNotFoundException ex)
            {
                CustomMessageBox.Show("The file was not found at " + gstlaunch +
                                      "\nPlease verify permissions " + ex.ToString());
                return;
            }
            catch (BadImageFormatException)
            {
                CustomMessageBox.Show("The incorrect exe architecture has been detected at " + gstlaunch +
                                      "\nPlease install gstreamer for the correct architecture");
                return;
            }

            uint v1 = 0, v2 = 0, v3 = 0, v4 = 0;
            NativeMethods.gst_version(out v1, out v2, out v3, out v4);

            log.InfoFormat("GStreamer {0}.{1}.{2}.{3}", v1, v2, v3, v4);

            IntPtr error;
            NativeMethods.gst_init_check(IntPtr.Zero, IntPtr.Zero, out error);

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
                //@"videotestsrc ! video/x-raw, width=1280, height=720, framerate=30/1 ! x264enc speed-preset=1 threads=1 sliced-threads=1 mb-tree=0 rc-lookahead=0 sync-lookahead=0 bframes=0 ! rtph264pay ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink",
                //@"-v udpsrc port=5601 buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink",
                //@"rtspsrc location=rtsp://192.168.1.252/video1 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink",
                out error);

            //rtspsrc location=rtsp://192.168.1.21/live ! application/x-rtp ! rtph265depay ! avdec_h265 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink
            //videotestsrc ! video/x-raw, width=1280, height=720, framerate=30/1 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink 

            if (error != IntPtr.Zero)
            {
                var er = Marshal.PtrToStructure<GError>(error);
                log.Error("gst_parse_launch: " + er.message);
                return;
            }

            NativeMethods.gst_debug_bin_to_dot_file(pipeline, GstDebugGraphDetails.GST_DEBUG_GRAPH_SHOW_ALL,
                "pipeline");

            log.Info("graphviz of pipeline is at " + Path.GetTempPath() + "pipeline.dot");

            //var msg = GStreamer.gst_bus_timed_pop_filtered(bus, GStreamer.GST_CLOCK_TIME_NONE, GStreamer.GstMessageType.GST_MESSAGE_ERROR | GStreamer.GstMessageType.GST_MESSAGE_EOS);


            // appsink is part of the parse launch
            var appsink = NativeMethods.gst_bin_get_by_name(pipeline, "outsink");
            log.Info("got appsink ");
            //var appsink = NativeMethods.gst_element_factory_make("appsink", null);

            bool newdata = false;
            GstAppSinkCallbacks callbacks = new GstAppSinkCallbacks();
            //var callbackhandle = GCHandle.Alloc(callbacks, GCHandleType.Pinned);
            /*callbacks.new_buffer += (sink, data) =>
            {
                newdata = true;
                return GstFlowReturn.GST_FLOW_OK;
            };
            callbacks.new_preroll += (sink, data) =>
            {
                log.Info("new_preroll");
                return GstFlowReturn.GST_FLOW_OK;
            };
            callbacks.eos += (sink, data) => { log.Info("EOS"); };
            */

            NativeMethods.gst_app_sink_set_drop(appsink, true);
            NativeMethods.gst_app_sink_set_max_buffers(appsink, 1);
            // callback fail on linux
            //NativeMethods.gst_app_sink_set_callbacks(appsink, callbacks, IntPtr.Zero, IntPtr.Zero);
            log.Info("set appsink params ");
            /* Start playing */
            var running = NativeMethods.gst_element_set_state(pipeline, GstState.GST_STATE_PLAYING) != GstStateChangeReturn.GST_STATE_CHANGE_FAILURE;
            log.Info("set playing ");
            /* Wait until error or EOS */
            var bus = NativeMethods.gst_element_get_bus(pipeline);

            int Width = 0;
            int Height = 0;
            int trys = 0;
            // prevent it falling out of scope
            GstAppSinkCallbacks callbacks2 = callbacks;

            run = true;
            log.Info("start frame loop gst_app_sink_is_eos");
            while (run && !NativeMethods.gst_app_sink_is_eos(appsink))
            {
                try
                {
                    //log.Info("gst_app_sink_try_pull_sample ");
                    var sample = NativeMethods.gst_app_sink_try_pull_sample(appsink, GST_SECOND * 5);
                    if (sample != IntPtr.Zero)
                    {
                        trys = 0;
                        //var caps = gst_app_sink_get_caps(appsink);
                        //log.Info("gst_sample_get_caps ");
                        var caps = NativeMethods.gst_sample_get_caps(sample);
                        var caps_s = NativeMethods.gst_caps_get_structure(caps, 0);
                        NativeMethods.gst_structure_get_int(caps_s, "width", out Width);
                        NativeMethods.gst_structure_get_int(caps_s, "height", out Height);

                        //var capsstring = gst_caps_to_string(caps_s);
                        //var structure = gst_sample_get_info(sample);
                        //var structstring = gst_structure_to_string(structure);
                        //log.Info("gst_sample_get_buffer ");
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

            NativeMethods.gst_element_set_state(pipeline, GstState.GST_STATE_NULL);
            NativeMethods.gst_buffer_unref(bus);

            //callbackhandle.Free();

            // cleanup
            _onNewImage?.Invoke(null, null);

            log.Info("Gstreamer Exit");
        }

        ~GStreamer()
        {
            StopAll();
        }

        static GStreamer()
        {
            UdpPort = 5600;
            OutputPort = 1235;

            var dataDirectory = Settings.GetDataDirectory();
            var gstdir = Path.Combine(dataDirectory, @"gstreamer\1.0\x86_64\bin\libgstreamer-1.0-0.dll");

       
            SetGSTPath(gstdir);
        }

        private static void SetGSTPath(string gstdir)
        {
            //
            //C:\gstreamer\1.0\x86_64\bin\gst-launch-1.0.exe

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

        //C:\ProgramData\Mission Planner\gstreamer\1.0\x86_64\bin

        //gst-launch-1.0.exe  videotestsrc pattern=ball  is-live=true ! video/x-raw,width=640,height=480 ! clockoverlay ! x264enc ! rtph264pay ! udpsink host=127.0.0.1 port=5600
        //gst-launch-1.0.exe -v udpsrc port=5600 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue leaky=2 ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false
        //udpsrc port=5600 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue leaky=2 ! video/x-raw,format=BGRx ! appsink name=outsink

        //gst-launch-1.0.exe -v videotestsrc !  video/x-raw,format=BGRA,framerate=25/1 ! videoconvert ! autovideosink

        //gst-launch-1.0 videotestsrc pattern=ball ! x264enc ! rtph264pay ! udpsink host=127.0.0.1 port=5600
        //gst-launch-1.0 udpsrc port=5600 caps='application/x-rtp, media=(string)video, clock-rate=(int)90000, encoding-name=(string)H264' ! rtph264depay ! avdec_h264 ! autovideosink fps-update-interval=1000 sync=false

        //gst-launch-1.0.exe videotestsrc ! video/x-raw, width=1280, height=720, framerate=25/1 ! x264enc ! rtph264pay ! udpsink port=1234 host=192.168.0.1
        //gst-launch-1.0.exe -v udpsrc port=1234 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false

        // list plugins
        // gst-inspect-1.0

        public static string gstlaunch
        {
            get { return Settings.Instance["gstlaunchexe"]; }
            set { Settings.Instance["gstlaunchexe"] = value; }
        }

        public static bool gstlaunchexists
        {
            get
            {
                if (Android)
                    return true;
                return File.Exists(gstlaunch); 
            }
        }

        public static string LookForGstreamer()
        {
            List<string> dirs = new List<string>();

            // linux
            dirs.Add("/usr/lib/x86_64-linux-gnu");
            // rpi
            dirs.Add("/usr/lib/arm-linux-gnueabihf");
            // current
            dirs.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            // settings
            dirs.Add(Settings.GetDataDirectory());
            // custom settings
            dirs.Add(Settings.CustomUserDataDirectory);
            // sitl bindle
            dirs.Add(GStreamer.BundledPath);

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
                        return ans.First();
                    }
                }
            }

            log.Info("No gstreamer found");
            return "";
        }

        public static int UdpPort { get; set; }

        public static int OutputPort { get; set; }

        private static bool isrunning
        {
            get
            {
                return processList != null && processList.Count > 0 && !processList.Any(a =>
                {
                    try
                    {
                        return a.HasExited;
                    }
                    catch
                    {
                        return true;
                    }
                });
            }
        }

        // custom search path for .so
        public static string BundledPath { get; set; }
        public static bool Android { get; set; }

        public static Process Start(string custompipelinesrc = "", bool externalpipeline = false,
            bool allowmultiple = false)
        {
            // prevent starting 2 process's
            lock (_lock)
            {
                if (!allowmultiple && isrunning)
                {
                    log.Info("already running");
                    return null;
                }

                if (GStreamer.gstlaunchexists)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(gstlaunch,
                        String.Format(
                            "-v udpsrc port={0} buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ",
                            UdpPort));

                    if (custompipelinesrc != "")
                    {
                        psi.Arguments = custompipelinesrc;

                        if (!externalpipeline)
                        {
                            psi.Arguments += String.Format(
                                " ! queue leaky=2 ! tcpserversink host=127.0.0.1 port={0} sync=false",
                                OutputPort);
                        }
                        else
                        {
                            psi.Arguments += " ! decodebin ! queue leaky=2 ! autovideosink";
                        }
                    }
                    else
                    {
                        if (!externalpipeline)
                        {
                            psi.Arguments += String.Format(
                                " ! queue leaky=2 ! jpegenc ! queue leaky=2 ! tcpserversink host=127.0.0.1 port={0} sync=false",
                                OutputPort);
                        }
                        else
                        {
                            psi.Arguments += " ! decodebin ! queue leaky=2 ! autovideosink";
                        }
                    }

                    //"-v udpsrc port=5600 buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! queue ! rtpvrawpay ! giosink location=\\\\\\\\.\\\\pipe\\\\gstreamer");

                    //avenc_mjpeg

                    psi.UseShellExecute = false;

                    log.Info("Starting " + psi.FileName + " " + psi.Arguments);

                    psi.RedirectStandardInput = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;

                    var process = Process.Start(psi);
                    GStreamer.processList.Add(process);

                    process.Exited += delegate(object sender, EventArgs args) { Stop(process); };

                    process.ErrorDataReceived += (sender, args) => { log.Error(args.Data); };
                    process.OutputDataReceived += (sender, args) => { log.Info(args.Data); };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    System.Threading.ThreadPool.QueueUserWorkItem(_Start);

                    return process;
                }
                else
                {
                    log.Info("No gstreamer found");
                }
            }

            return null;
        }

        private static void NamedPipeConnect(NamedPipeServerStream pipeServer)
        {
            // 1080 * 1920 * 4(int) = 8294400 * 30fps = buffer 1/3 sec
            using (var stream = new BufferedStream(pipeServer, 1024 * 1024 * 9 * 10))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (pipeServer.IsConnected)
                    {
                        //readJPGData(stream, ms);

                        readRTPData(stream, ms);

                        System.Threading.Thread.Sleep(0);
                    }

                    //cleanup on disconnect
                    _onNewImage?.Invoke(null, null);
                }
            }
        }

        static void _Start(object nothing)
        {
            try
            {
                var deadline = DateTime.Now.AddSeconds(20);

                log.Info("_Start");

                while (DateTime.Now < deadline)
                {
                    try
                    {
                        TcpClient client = new TcpClient("127.0.0.1", OutputPort);
                        Console.WriteLine("Port open");
                        client.Close();
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Port closed");
                    }
                }

                using (TcpClient client = new TcpClient("127.0.0.1", OutputPort))
                {
                    client.ReceiveBufferSize = 1024 * 1024 * 5; // 5mb

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var stream = new BufferedStream(client.GetStream(), 1024 * 1024 * 5))
                        {
                            DateTime lastdata = DateTime.Now;
                            while (client.Client.Connected)
                            {
                                int bytestoread = client.Available;

                                while (bytestoread > 0)
                                {
                                    //bytestoread -= readRTPData(stream, ms);
                                    bytestoread -= readJPGData(stream, ms);
                                    lastdata = DateTime.Now;
                                }

                                // up to 100 fps or 50 with 10ms process time
                                System.Threading.Thread.Sleep(10);

                                if (lastdata.AddSeconds(5) < DateTime.Now)
                                {
                                    client.Client.Send(new byte[0]);
                                }
                            }

                            //cleanup on disconnect
                            _onNewImage?.Invoke(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _onNewImage?.Invoke(null, null);
                log.Error(ex);
            }
        }

        class rtpheader
        {
            // 2 bits
            public byte version { get; set; }

            // 1 bit
            public byte padding { get; set; }

            // 1 bit
            public byte extension { get; set; }

            // 4 bits
            public byte csrccount { get; set; }

            // 1 bits
            public byte marker { get; set; }

            // 7 bits
            public byte payloadtype { get; set; }

            // 16 bits
            public ushort sequencenumber { get; set; }

            // 32 bits
            public uint timestamp { get; set; }

            // 32 bits
            public uint ssrc { get; set; }

            public ushort extsequencenumber { get; set; }

            public ushort length { get; set; }
            public byte F { get; set; }
            public ushort lineno { get; set; }
            public byte C { get; set; }
            public ushort offset { get; set; }

            // only is C is set above;
            public ushort length2 { get; set; }

            public byte F2 { get; set; }
            public ushort lineno2 { get; set; }
            public byte C2 { get; set; }
            public ushort offset2 { get; set; }

            // rfc 4175 - https://tools.ietf.org/html/rfc4175
            public rtpheader(byte[] buffer)
            {
                version = (byte) ((buffer[0] >> 6) & 3);
                padding = (byte) ((buffer[0] >> 5) & 1);
                extension = (byte) ((buffer[0] >> 4) & 1);
                csrccount = (byte) ((buffer[0] >> 0) & 15);

                marker = (byte) ((buffer[1] >> 7) & 1);
                payloadtype = (byte) ((buffer[1] >> 0) & 127);

                sequencenumber = (ushort) (buffer[2] << 8 + buffer[3]);
                timestamp = (uint) (buffer[4] << 24 + buffer[5] << 26 + buffer[6] << 8 + buffer[7]);
                ssrc = (uint) (buffer[8] << 24 + buffer[9] << 26 + buffer[10] << 8 + buffer[11]);

                // 2 bytes https://fossies.org/linux/gst-plugins-good/gst/rtp/gstrtpvrawpay.c #384   
                // always 0    
                extsequencenumber = (ushort) (buffer[12] << 8 + buffer[13]);

                // 6 byte - https://fossies.org/linux/gst-plugins-good/gst/rtp/gstrtpvrawpay.c #467
                // (pixels * pgroup) / xinc   ;    pgroup = 4/32bit BGRA   xinc = 1 for BGRA
                length = (ushort) ((buffer[14] << 8) + buffer[15]);
                F = (byte) (buffer[16] >> 7);
                // height line number
                lineno = (ushort) (((buffer[16] & 127) << 8) + buffer[17]);
                // there is more than one height here
                C = (byte) (buffer[18] >> 7);
                offset = (ushort) (((buffer[18] & 127) << 8) + buffer[19]);

                if (buffer.Length >= 26)
                {
                    length2 = (ushort) ((buffer[20] << 8) + buffer[21]);
                    F2 = (byte) (buffer[22] >> 7);
                    // height line number
                    lineno2 = (ushort) (((buffer[22] & 127) << 8) + buffer[23]);
                    // there is more than one height here
                    C2 = (byte) (buffer[24] >> 7);
                    offset2 = (ushort) (((buffer[24] & 127) << 8) + buffer[25]);

                    return;
                }

                var actoffset = offset * 4;

                //Console.WriteLine("rtp {0} - mark {1} {2} {3} {4} {5} {6}={7} {8}", payloadtype, marker, sequencenumber, length, C, lineno, offset, actoffset,F);
            }
        }

        static byte rtpbyte1 = 0x80;

        // mark bit notset
        static byte rtpbyte2 = 0x60;

        // mark bit set
        static byte rtpbyte2_2 = 0xe0;

        // image width
        static int width = 0;

        private static byte[] buffer;
        private static Bitmap img;

        public static int readRTPData(Stream stream, MemoryStream ms)
        {
            int readamount = 0;

            var ch1 = stream.ReadByte();
            readamount++;
            if (ch1 == rtpbyte1)
            {
                var ch2 = stream.ReadByte();
                readamount++;
                // handle 2 rtpbyte1's in a row
                if (ch2 == rtpbyte1)
                {
                    ch1 = ch2;
                    ch2 = stream.ReadByte();
                    readamount++;
                }

                if (ch2 == rtpbyte2 || ch2 == rtpbyte2_2)
                {
                    byte[] headerBytes = new byte[4 * 5];
                    headerBytes[0] = (byte) ch1;
                    headerBytes[1] = (byte) ch2;

                    readamount += stream.Read(headerBytes, 2, headerBytes.Length - 2);

                    GStreamer.rtpheader header = new rtpheader(headerBytes);

                    // this check is the same as rtpbyte1 and rtpbyte2/rtpbyte2_2
                    if (header.version == 2 && header.payloadtype == 96 && header.extsequencenumber == 0)
                    {
                        // read additial C
                        if (header.C > 0)
                        {
                            var oldsize = headerBytes.Length;
                            Array.Resize(ref headerBytes, headerBytes.Length + 6);
                            readamount += stream.Read(headerBytes, oldsize, 6);

                            header = new rtpheader(headerBytes);
                        }

                        var pixels = header.length / 4;
                        if (header.C > 0 && header.lineno == 0)
                        {
                            width = header.offset + pixels;
                        }

                        //p0 + (lin * ystride) + (offs * pgroup), length
                        var fileoffset = (header.lineno) * width * 4 + header.offset * 4;
                        if (fileoffset != ms.Position)
                        {
                        }

                        ms.Seek(fileoffset, SeekOrigin.Begin);

                        int neededbytes = header.length + header.length2;

                        if (buffer == null || buffer.Length < (neededbytes))
                            buffer = new byte[neededbytes];

                        var read = stream.Read(buffer, 0, neededbytes);
                        ms.Write(buffer, 0, read);
                        readamount += read;

                        if (header.marker > 0 && width != 0)
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            try
                            {
                                if (img == null || img.Width < width || img.Height < header.lineno + 1)
                                    img = new Bitmap(width, header.lineno + 1, SKColorType.Bgra8888);

                                lock (img)
                                {
                                    BitmapData bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                        ImageLockMode.WriteOnly, img.PixelFormat);

                                    IntPtr ptr = bmpData.Scan0;

                                    Marshal.Copy(ms.GetBuffer(), 0, ptr, (int) img.Width * img.Height * 4);

                                    img.UnlockBits(bmpData);
                                }

                                //img.Save("test.bmp");

                                _onNewImage?.Invoke(null, img);

                                tempno++;
                                persecond++;

                                if (lastsecond.Second != DateTime.Now.Second)
                                {
                                    Console.WriteLine("image {0}x{1} size {2} miss {3} ps {4}",
                                        img.Width,
                                        img.Height, 0, miss, persecond);
                                    persecond = 0;
                                    lastsecond = DateTime.Now;
                                    miss = 0;
                                }

                                ms.SetLength(0);
                            }
                            catch
                            {
                                ms.SetLength(0);
                            }
                        }
                    }
                    else
                    {
                        miss++;
                        Console.WriteLine("packet failed header check ");
                    }
                }
                else
                {
                    miss++;
                    Console.WriteLine("out of sync2 {0:X}", ch1);
                }
            }
            else
            {
                miss++;
                Console.WriteLine("out of sync1 {0:X}", ch1);
            }

            return readamount;
        }

        static int tempno = 0;
        static int miss = 0;
        static int persecond = 0;
        static DateTime lastsecond = DateTime.Now;

        private static int readJPGData(Stream stream, MemoryStream ms)
        {
            var bytesread = 0;

            // start header
            byte ch3 = (byte) stream.ReadByte();
            bytesread++;
            if (ch3 == 0xff)
            {
                byte ch4 = (byte) stream.ReadByte();
                bytesread++;
                if (ch4 == 0xd8)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.WriteByte(ch3);
                    ms.WriteByte(ch4);
                    int last = 0;
                    do
                    {
                        int datach = stream.ReadByte();
                        bytesread++;
                        if (datach < 0)
                            break;

                        ms.WriteByte((byte) datach);

                        if (last == 0xff)
                        {
                            if (datach == 0xd9)
                                break;
                        }

                        last = datach;
                    } while (true);

                    ms.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        var temp = Bitmap.FromStream(ms);

                        //File.WriteAllBytes(tempno + ".bmp", ms.ToArray());

                        _onNewImage?.Invoke(null, temp);

                        tempno++;
                        persecond++;

                        if (lastsecond.Second != DateTime.Now.Second)
                        {
                            Console.WriteLine("image {0}x{1} size {2} miss {3} ps {4}",
                                temp.Width,
                                temp.Height, 0, miss, persecond);
                            persecond = 0;
                            lastsecond = DateTime.Now;
                            miss = 0;
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    miss++;
                }
            }
            else
            {
                miss++;
            }

            return bytesread;
        }

        public static void Stop(Process run)
        {
            try
            {
                log.Info("Stop");

                if (run != null)
                {
                    try
                    {
                        log.Info("StandardInput close");
                        run.StandardInput.Write('\x3');
                        run.StandardInput.Close();
                    }
                    catch
                    {
                    }

                    if (!run.CloseMainWindow())
                    {
                        Thread.Sleep(100);
                        log.Info("Kill");
                        run.Kill();
                    }

                    log.Info("Close");
                    run.Close();
                }
            }
            catch
            {
            }

            run = null;
        }

        private static bool run = true;
        public static void StopAll()
        {
            run = false;
            Thread.Sleep(50);
            foreach (var process in processList)
            {
                Stop(process);
            }
        }

        public static void DownloadGStreamer(Action<int, string> status = null)
        {
            string output = "";
            string url = "";

            if(RuntimeInformation.OSArchitecture == Architecture.Arm || RuntimeInformation.OSArchitecture == Architecture.Arm64)
                return;

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

            try
            {
                Download.getFilefromNet(url, output, status: status);

                status?.Invoke(50, "Extracting..");
                ZipFile.ExtractToDirectory(output, Settings.GetDataDirectory());
                status?.Invoke(100, "Done.");
            }
            catch (WebException ex)
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
            }
        }
    }
}