////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.MediaPlayer.cs - This file is part of LibVLC.NET.
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

#pragma warning disable 1591

namespace LibVLC.NET
{
  
  //****************************************************************************
  partial class LibVLCLibrary
  {

    //==========================================================================
    public class libvlc_track_description_t
    {
      public uint i_id;
      public string psz_name;
      public libvlc_track_description_t p_next;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    private struct internal_libvlc_track_description_t
    {
      public uint i_id;

      [MarshalAs(UnmanagedType.LPStr)]
      public string psz_name;

      //[MarshalAs(UnmanagedType.LPStruct)]
      //public libvlc_track_description_t p_next;
      public IntPtr p_next;
    }


    //==========================================================================
    // libvlc_media_player_t * libvlc_media_player_new (libvlc_instance_t *p_libvlc_instance)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_player_new_signature(IntPtr p_instance);

    //==========================================================================
    private readonly libvlc_media_player_new_signature m_libvlc_media_player_new;

    //==========================================================================
    public IntPtr libvlc_media_player_new(IntPtr p_instance)
    {
      VerifyAccess();

      return m_libvlc_media_player_new(p_instance);
    }


    //==========================================================================
    // libvlc_media_player_t * libvlc_media_player_new_from_media (libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_player_new_from_media_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_player_new_from_media_signature m_libvlc_media_player_new_from_media;

    //==========================================================================
    public IntPtr libvlc_media_player_new_from_media(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_player_new_from_media(p_md);
    }


    //==========================================================================
    // void libvlc_media_player_release (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_player_release_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_release_signature m_libvlc_media_player_release;

    //==========================================================================
    public void libvlc_media_player_release(IntPtr p_mi)
    {
      VerifyAccess();

      m_libvlc_media_player_release(p_mi);
    }

    /*
      void libvlc_media_player_retain (libvlc_media_player_t *p_mi)
    */


    //==========================================================================
    // void libvlc_media_player_set_media (libvlc_media_player_t *p_mi, libvlc_media_t *p_md)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_player_set_media_signature(IntPtr p_mi, IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_player_set_media_signature m_libvlc_media_player_set_media;

    //==========================================================================
    public void libvlc_media_player_set_media(IntPtr p_mi, IntPtr p_md)
    {
      VerifyAccess();

      m_libvlc_media_player_set_media(p_mi, p_md);
    }


    //==========================================================================
    // libvlc_media_t * libvlc_media_player_get_media (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_player_get_media_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_media_signature m_libvlc_media_player_get_media;

    //==========================================================================
    public IntPtr libvlc_media_player_get_media(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_media(p_mi);
    }


    //==========================================================================
    // libvlc_event_manager_t * libvlc_media_player_event_manager (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_media_player_event_manager_signature(IntPtr p_md);

    //==========================================================================
    private readonly libvlc_media_player_event_manager_signature m_libvlc_media_player_event_manager;

    //==========================================================================
    public IntPtr libvlc_media_player_event_manager(IntPtr p_md)
    {
      VerifyAccess();

      return m_libvlc_media_player_event_manager(p_md);
    }

    /*
      int libvlc_media_player_is_playing (libvlc_media_player_t *p_mi)
    */


    //==========================================================================
    // int libvlc_media_player_play (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_play_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_play_signature m_libvlc_media_player_play;

    //==========================================================================
    public int libvlc_media_player_play(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_play(p_mi);
    }

    /*
      void libvlc_media_player_set_pause (libvlc_media_player_t *mp, int do_pause)
    */


    //==========================================================================
    // void libvlc_media_player_pause (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_pause_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_pause_signature m_libvlc_media_player_pause;

    //==========================================================================
    public int libvlc_media_player_pause(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_pause(p_mi);
    }


    //==========================================================================
    // void libvlc_media_player_stop (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_player_stop_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_stop_signature m_libvlc_media_player_stop;

    //==========================================================================
    public void libvlc_media_player_stop(IntPtr p_mi)
    {
      VerifyAccess();

      m_libvlc_media_player_stop(p_mi);
    }


    //==========================================================================
    // void libvlc_video_set_callbacks (libvlc_media_player_t *mp, void *(*lock)(void *opaque, void **plane), void(*unlock)(void *opaque, void *picture, void *const *plane), void(*display)(void *opaque, void *picture), void *opaque)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr libvlc_video_lock_cb(IntPtr opaque, ref IntPtr planes);

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void libvlc_video_unlock_cb(IntPtr opaque, IntPtr picture, ref IntPtr planes);

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void libvlc_video_display_cb(IntPtr opaque, IntPtr picture);

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_video_set_callbacks_signature(IntPtr mp, libvlc_video_lock_cb _lock, libvlc_video_unlock_cb unlock, libvlc_video_display_cb display);

    //==========================================================================
    private readonly libvlc_video_set_callbacks_signature m_libvlc_video_set_callbacks;

    //==========================================================================
    public void libvlc_video_set_callbacks(IntPtr mp, libvlc_video_lock_cb _lock, libvlc_video_unlock_cb unlock, libvlc_video_display_cb display)
    {
      VerifyAccess();

      m_libvlc_video_set_callbacks(mp, _lock, unlock, display);
    }


    //==========================================================================
    // void libvlc_video_set_format (libvlc_media_player_t *mp, const char *chroma, unsigned width, unsigned height, unsigned pitch)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_video_set_format_signature(IntPtr mp, string chroma, uint width, uint height, uint pitch);

    //==========================================================================
    private readonly libvlc_video_set_format_signature m_libvlc_video_set_format;

    //==========================================================================
    public void libvlc_video_set_format(IntPtr mp, string chroma, uint width, uint height, uint pitch)
    {
      VerifyAccess();

      m_libvlc_video_set_format(mp, chroma, width, height, pitch);
    }


    //==========================================================================
    // void libvlc_video_set_format_callbacks (libvlc_media_player_t *mp, libvlc_video_format_cb setup, libvlc_video_cleanup_cb cleanup)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint libvlc_video_format_cb(ref IntPtr opaque, ref uint chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines);

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void libvlc_video_cleanup_cb(IntPtr opaque);

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_video_set_format_callbacks_signature(IntPtr mp, libvlc_video_format_cb setup, libvlc_video_cleanup_cb cleanup);

    //==========================================================================
    private readonly libvlc_video_set_format_callbacks_signature m_libvlc_video_set_format_callbacks;

    //==========================================================================
    public void libvlc_video_set_format_callbacks(IntPtr mp, libvlc_video_format_cb setup, libvlc_video_cleanup_cb cleanup)
    {
      VerifyAccess();

      m_libvlc_video_set_format_callbacks(mp, setup, cleanup);
    }

    /*
      void libvlc_media_player_set_nsobject (libvlc_media_player_t *p_mi, void *drawable)
      void * libvlc_media_player_get_nsobject (libvlc_media_player_t *p_mi)
      void libvlc_media_player_set_agl (libvlc_media_player_t *p_mi, uint32_t drawable)
      uint32_t libvlc_media_player_get_agl (libvlc_media_player_t *p_mi)
      void libvlc_media_player_set_xwindow (libvlc_media_player_t *p_mi, uint32_t drawable)
      uint32_t libvlc_media_player_get_xwindow (libvlc_media_player_t *p_mi)
      void libvlc_media_player_set_hwnd (libvlc_media_player_t *p_mi, void *drawable)
      void * libvlc_media_player_get_hwnd (libvlc_media_player_t *p_mi)
    */

    //==========================================================================
    // libvlc_time_t libvlc_media_player_get_length (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long libvlc_media_player_get_length_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_length_signature m_libvlc_media_player_get_length;

    //==========================================================================
    public long libvlc_media_player_get_length(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_length(p_mi);
    }


    //==========================================================================
    // libvlc_time_t libvlc_media_player_get_time (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long libvlc_media_player_get_time_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_time_signature m_libvlc_media_player_get_time;

    //==========================================================================
    public long libvlc_media_player_get_time(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_time(p_mi);
    }


    //==========================================================================
    // void libvlc_media_player_set_time (libvlc_media_player_t *p_mi, libvlc_time_t i_time)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long libvlc_media_player_set_time_signature(IntPtr p_mi, long f_pos);

    //==========================================================================
    private readonly libvlc_media_player_set_time_signature m_libvlc_media_player_set_time;

    //==========================================================================
    public long libvlc_media_player_set_time(IntPtr p_mi, long f_pos)
    {
      VerifyAccess();

      return m_libvlc_media_player_set_time(p_mi, f_pos);
    }


    //==========================================================================
    // float libvlc_media_player_get_position (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float libvlc_media_player_get_position_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_position_signature m_libvlc_media_player_get_position;

    //==========================================================================
    public float libvlc_media_player_get_position(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_position(p_mi);
    }


    //==========================================================================
    // void libvlc_media_player_set_position (libvlc_media_player_t *p_mi, float f_pos)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate long libvlc_media_player_set_position_signature(IntPtr p_mi, float f_pos);

    //==========================================================================
    private readonly libvlc_media_player_set_position_signature m_libvlc_media_player_set_position;

    //==========================================================================
    public long libvlc_media_player_set_position(IntPtr p_mi, float f_pos)
    {
      VerifyAccess();

      return m_libvlc_media_player_set_position(p_mi, f_pos);
    }
    

    //==========================================================================
    // void libvlc_media_player_set_chapter (libvlc_media_player_t *p_mi, int i_chapter)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_media_player_set_chapter_signature(IntPtr p_mi, int i_chapter);

    //==========================================================================
    private readonly libvlc_media_player_set_chapter_signature m_libvlc_media_player_set_chapter;

    //==========================================================================
    public void libvlc_media_player_set_chapter(IntPtr p_mi, int i_chapter)
    {
      VerifyAccess();

      m_libvlc_media_player_set_chapter(p_mi, i_chapter);
    }


    //==========================================================================
    //int libvlc_media_player_get_chapter (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_get_chapter_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_chapter_signature m_libvlc_media_player_get_chapter;

    //==========================================================================
    public int libvlc_media_player_get_chapter(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_chapter(p_mi);
    }


    //==========================================================================
    //int libvlc_media_player_get_chapter_count (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_get_chapter_count_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_chapter_count_signature m_libvlc_media_player_get_chapter_count;

    //==========================================================================
    public int libvlc_media_player_get_chapter_count(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_chapter_count(p_mi);
    }


    /*
  int libvlc_media_player_will_play (libvlc_media_player_t *p_mi)
  int libvlc_media_player_get_chapter_count_for_title (libvlc_media_player_t *p_mi, int i_title)
  void libvlc_media_player_set_title (libvlc_media_player_t *p_mi, int i_title)
  int libvlc_media_player_get_title (libvlc_media_player_t *p_mi)
  int libvlc_media_player_get_title_count (libvlc_media_player_t *p_mi)
     */


    //==========================================================================
    // void libvlc_media_player_previous_chapter (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_previous_chapter_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_previous_chapter_signature m_libvlc_media_player_previous_chapter;

    //==========================================================================
    public void libvlc_media_player_previous_chapter(IntPtr p_mi)
    {
      VerifyAccess();

      m_libvlc_media_player_previous_chapter(p_mi);
    }


    //==========================================================================
    // void libvlc_media_player_next_chapter (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_next_chapter_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_next_chapter_signature m_libvlc_media_player_next_chapter;

    //==========================================================================
    public void libvlc_media_player_next_chapter(IntPtr p_mi)
    {
      VerifyAccess();

      m_libvlc_media_player_next_chapter(p_mi);
    }


    /*
  float libvlc_media_player_get_rate (libvlc_media_player_t *p_mi)
  int libvlc_media_player_set_rate (libvlc_media_player_t *p_mi, float rate)
*/

    //==========================================================================
    // libvlc_state_t libvlc_media_player_get_state (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate libvlc_state_t libvlc_media_player_get_state_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_state_signature m_libvlc_media_player_get_state;

    //==========================================================================
    public libvlc_state_t libvlc_media_player_get_state(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_state(p_mi);
    }


    //==========================================================================
    // float libvlc_media_player_get_fps (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate float libvlc_media_player_get_fps_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_get_fps_signature m_libvlc_media_player_get_fps;

    //==========================================================================
    public float libvlc_media_player_get_fps(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_get_fps(p_mi);
    }

    /*
      unsigned libvlc_media_player_has_vout (libvlc_media_player_t *p_mi)
     */
      
    //==========================================================================
    // int libvlc_media_player_is_seekable (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_is_seekable_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_is_seekable_signature m_libvlc_media_player_is_seekable;

    //==========================================================================
    public int libvlc_media_player_is_seekable(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_is_seekable(p_mi);
    }

    //==========================================================================
    // int libvlc_media_player_can_pause (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_can_pause_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_can_pause_signature m_libvlc_media_player_can_pause;

    //==========================================================================
    public int libvlc_media_player_can_pause(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_can_pause(p_mi);
    }


    //==========================================================================
    // void libvlc_media_player_next_frame (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_media_player_next_frame_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_media_player_next_frame_signature m_libvlc_media_player_next_frame;

    //==========================================================================
    public int libvlc_media_player_next_frame(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_media_player_next_frame(p_mi);
    }

    //==========================================================================
    //void libvlc_track_description_release (libvlc_track_description_t *p_track_description)
    //void libvlc_track_description_list_release	(	libvlc_track_description_t * 	p_track_description	 ) 	

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_track_description_list_release_signature(IntPtr p_track_description);

    //==========================================================================
    private readonly libvlc_track_description_list_release_signature m_libvlc_track_description_list_release;

    //==========================================================================
    public int libvlc_track_description_list_release(libvlc_track_description_t p_track_description)
    {
      return 0;
    }

  } // class LibVLCLibrary

}
