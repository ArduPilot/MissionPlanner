////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.Media.cs - This file is part of LibVLC.NET.
//
//    Copyright (C) 2011 Boris Richter <himself@boris-richter.net>
//
//  ==========================================================================
//  
//  LibVLC.NET is free software; you can redistribute it and/or modify it 
//  under the terms of the GNU Lesser General Public License as published by 
//  the Free Software Foundation; either version 2.1 of the License, or (at 
//  your option) any later version.
//    
//  LibVLC.NET is distributed in the hope that it will be useful, but WITHOUT 
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//  FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License 
//  along with LibVLC.NET; if not, see http://www.gnu.org/licenses/.
//
//  ==========================================================================
// 
//  $LastChangedRevision$
//  $LastChangedDate$
//  $LastChangedBy$
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Collections.Generic;

#pragma warning disable 1591

namespace LibVLC.NET
{
  
  //****************************************************************************
  partial class LibVLCLibrary
  {

    //==========================================================================
    public enum libvlc_meta_t
      : int
    {
      libvlc_meta_Title,
      libvlc_meta_Artist,
      libvlc_meta_Genre,
      libvlc_meta_Copyright,
      libvlc_meta_Album,
      libvlc_meta_TrackNumber,
      libvlc_meta_Description,
      libvlc_meta_Rating,
      libvlc_meta_Date,
      libvlc_meta_Setting,
      libvlc_meta_URL,
      libvlc_meta_Language,
      libvlc_meta_NowPlaying,
      libvlc_meta_Publisher,
      libvlc_meta_EncodedBy,
      libvlc_meta_ArtworkURL,
      libvlc_meta_TrackID
    }

    //==========================================================================
    public enum libvlc_state_t
      : int
    {
      libvlc_NothingSpecial = 0,
      libvlc_Opening,
      libvlc_Buffering,
      libvlc_Playing,
      libvlc_Paused,
      libvlc_Stopped,
      libvlc_Ended,
      libvlc_Error
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct libvlc_media_track_info_t_audio
    {
      public uint i_channels;
      public uint i_rate;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct libvlc_media_track_info_t_video
    {
      public uint i_height;
      public uint i_width;
    }

    //==========================================================================
    public enum libvlc_track_type_t
      : int
    {
      libvlc_track_unknown = -1,
      libvlc_track_audio = 0,
      libvlc_track_video = 1,
      libvlc_track_text = 2
    }

    //==========================================================================
    [StructLayout(LayoutKind.Explicit)]
    public struct libvlc_media_track_info_t
    {
      /* Codec fourcc */
      [FieldOffset(0)]
      public uint i_codec;

      [FieldOffset(4)]
      public int i_id;

      [FieldOffset(8)]
      public libvlc_track_type_t i_type;

      [FieldOffset(12)]
      public int i_profile;
      
      [FieldOffset(16)]
      public int i_level;

      [FieldOffset(20)]
      public libvlc_media_track_info_t_audio audio;

      [FieldOffset(20)]
      public libvlc_media_track_info_t_video video;
    }

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_new_location_signature(IntPtr p_instance, string psz_mrl);

    //==========================================================================
    private readonly libvlc_media_new_location_signature m_libvlc_media_new_location;

    //==========================================================================
    public IntPtr libvlc_media_new_location(IntPtr p_instance, string psz_mrl)
    {
      VerifyAccess();

      return m_libvlc_media_new_location(p_instance, psz_mrl);
    }

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_new_path_signature(IntPtr p_instance, IntPtr psz_path);

    //==========================================================================
    private readonly libvlc_media_new_path_signature m_libvlc_media_new_path;

    //==========================================================================
    public IntPtr libvlc_media_new_path(IntPtr p_instance, string psz_path)
    {
      VerifyAccess();

      byte[] parameter = Encoding.UTF8.GetBytes(psz_path).Concat(new byte[] { 0x00 }).ToArray();
      GCHandle handle = GCHandle.Alloc(parameter, GCHandleType.Pinned);
      IntPtr result = m_libvlc_media_new_path(p_instance, handle.AddrOfPinnedObject());
      handle.Free();
      return result;
    }

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_new_as_node_signature(IntPtr p_instance, string psz_mrl);

    //==========================================================================
    private readonly libvlc_media_new_as_node_signature m_libvlc_media_new_as_node;

    //==========================================================================
    public IntPtr libvlc_media_new_as_node(IntPtr p_instance, string psz_mrl)
    {
      VerifyAccess();

      return m_libvlc_media_new_as_node(p_instance, psz_mrl);
    }


    /*
    void libvlc_media_add_option (libvlc_media_t *p_md, const char *ppsz_options)
    void libvlc_media_add_option_flag (libvlc_media_t *p_md, const char *ppsz_options, unsigned i_flags)
    void libvlc_media_retain (libvlc_media_t *p_md)
    */

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_release_signature(IntPtr p_instance);

    //==========================================================================
    private readonly libvlc_media_release_signature m_libvlc_media_release;

    //==========================================================================
    public void libvlc_media_release(IntPtr p_instance)
    {
      VerifyAccess();

      m_libvlc_media_release(p_instance);
    }

    //==========================================================================
    // char * libvlc_media_get_mrl (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_get_mrl_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_get_mrl_signature m_libvlc_media_get_mrl;

    //==========================================================================
    public string libvlc_media_get_mrl(IntPtr p_md)
    {
      VerifyAccess();

      List<byte> buffer = new List<byte>();
      IntPtr result = m_libvlc_media_get_mrl(p_md);
      if(result == IntPtr.Zero)
        return null;

      do
      {
        buffer.Add(Marshal.ReadByte(result));
        result += 1;
      }
      while(buffer[buffer.Count - 1] != 0);

      string mrl = Encoding.ASCII.GetString(buffer.ToArray(), 0, buffer.Count - 1);

      return mrl;
    }

    /*
    libvlc_media_t * libvlc_media_duplicate (libvlc_media_t *p_md)
    char * libvlc_media_get_meta (libvlc_media_t *p_md, libvlc_meta_t e_meta)
    void libvlc_media_set_meta (libvlc_media_t *p_md, libvlc_meta_t e_meta, const char *psz_value)
    int libvlc_media_save_meta (libvlc_media_t *p_md)
    libvlc_state_t libvlc_media_get_state (libvlc_media_t *p_md)
    int libvlc_media_get_stats (libvlc_media_t *p_md, libvlc_media_stats_t *p_stats)
    */

    //==========================================================================
    // struct libvlc_media_list_t* libvlc_media_subitems (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_subitems_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_subitems_signature m_libvlc_media_subitems;

    //==========================================================================
    public IntPtr libvlc_media_subitems(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_subitems(p_md);
    }

    //==========================================================================
    // LIBVLC_API libvlc_event_manager_t* libvlc_media_event_manager	(	libvlc_media_t * 	p_md	 ) 	


    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_event_manager_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_event_manager_signature m_libvlc_media_event_manager;

    //==========================================================================
    public IntPtr libvlc_media_event_manager(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_event_manager(p_md);
    }

    //==========================================================================
    // libvlc_time_t libvlc_media_get_duration (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long libvlc_media_get_duration_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_get_duration_signature m_libvlc_media_get_duration;

    //==========================================================================
    public long libvlc_media_get_duration(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_get_duration(p_md);
    }

    //==========================================================================
    // void libvlc_media_parse (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_parse_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_parse_signature m_libvlc_media_parse;

    //==========================================================================
    public void libvlc_media_parse(IntPtr p_md)
    {
      VerifyAccess();

      m_libvlc_media_parse(p_md);
    }

    //==========================================================================
    // void libvlc_media_parse_async (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_parse_async_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_parse_async_signature m_libvlc_media_parse_async;

    //==========================================================================
    public void libvlc_media_parse_async(IntPtr p_md)
    {
      VerifyAccess();

      m_libvlc_media_parse_async(p_md);
    }

    //==========================================================================
    // int libvlc_media_is_parsed (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_is_parsed_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_is_parsed_signature m_libvlc_media_is_parsed;

    //==========================================================================
    public int libvlc_media_is_parsed(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_is_parsed(p_md);
    }

    /*
    void libvlc_media_set_user_data (libvlc_media_t *p_md, void *p_new_user_data)
    void * libvlc_media_get_user_data (libvlc_media_t *p_md)
    */

    //==========================================================================
    // int libvlc_media_get_tracks_info (libvlc_media_t *p_md, libvlc_media_track_info_t **tracks)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_get_tracks_info_signature(IntPtr p_md, out IntPtr tracks);

    //==========================================================================
    private readonly libvlc_media_get_tracks_info_signature m_libvlc_media_get_tracks_info;

    //==========================================================================
    public int libvlc_media_get_tracks_info(IntPtr p_md, out libvlc_media_track_info_t[] tracks)
    {
      VerifyAccess();

      IntPtr result_buffer;
      int result = m_libvlc_media_get_tracks_info(p_md, out result_buffer);
      if(result < 0)
      {
        tracks = null;
        return result;
      }

      IntPtr buffer = result_buffer;
      tracks = new libvlc_media_track_info_t[result];
      for(int i = 0; i < tracks.Length; i++)
      {
        tracks[i] = (libvlc_media_track_info_t)Marshal.PtrToStructure(buffer, typeof(libvlc_media_track_info_t));
        buffer += Marshal.SizeOf(typeof(libvlc_media_track_info_t));
      }

      libvlc_free(result_buffer);

      return result;
    }

  } // class LibVLCLibrary

}
