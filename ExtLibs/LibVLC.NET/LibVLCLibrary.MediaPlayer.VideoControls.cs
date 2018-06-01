////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.MediaPlayer.VideoControls.cs 
//    - This file is part of LibVLC.NET.
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 1591

namespace LibVLC.NET
{
  
  //****************************************************************************
  partial class LibVLCLibrary
  {

    //void libvlc_toggle_fullscreen (libvlc_media_player_t *p_mi)
    //void libvlc_set_fullscreen (libvlc_media_player_t *p_mi, int b_fullscreen)
    //int libvlc_get_fullscreen (libvlc_media_player_t *p_mi)
    //void libvlc_video_set_key_input (libvlc_media_player_t *p_mi, unsigned on)
    //void libvlc_video_set_mouse_input (libvlc_media_player_t *p_mi, unsigned on)


    //==========================================================================
    // int libvlc_video_get_size (libvlc_media_player_t *p_mi, unsigned num, unsigned *px, unsigned *py)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_size_signature(IntPtr p_mi, uint num, out uint px, out uint py);

    //==========================================================================
    private readonly libvlc_video_get_size_signature m_libvlc_video_get_size;

    //==========================================================================
    public int libvlc_video_get_size(IntPtr p_mi, uint num, out uint px, out uint py)
    {
      VerifyAccess();

      return m_libvlc_video_get_size(p_mi, num, out px, out py);
    }


    //==========================================================================
    // int libvlc_video_get_height (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_height_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_height_signature m_libvlc_video_get_height;

    //==========================================================================
    [Obsolete]
    public int libvlc_video_get_height(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_video_get_height(p_mi);
    }


    //==========================================================================
    // int libvlc_video_get_width (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_width_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_width_signature m_libvlc_video_get_width;

    //==========================================================================
    [Obsolete]
    public int libvlc_video_get_width(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_video_get_width(p_mi);
    }


    //int libvlc_video_get_cursor (libvlc_media_player_t *p_mi, unsigned num, int *px, int *py)
    //float libvlc_video_get_scale (libvlc_media_player_t *p_mi)
    //void libvlc_video_set_scale (libvlc_media_player_t *p_mi, float f_factor)
    //char * libvlc_video_get_aspect_ratio (libvlc_media_player_t *p_mi)
    // void libvlc_video_set_aspect_ratio (libvlc_media_player_t *p_mi, const char *psz_aspect)


    //==========================================================================
    // int libvlc_video_get_spu (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_spu_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_spu_signature m_libvlc_video_get_spu;

    //==========================================================================
    public int libvlc_video_get_spu(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_video_get_spu(p_mi);
    }

    
    //int libvlc_video_get_spu_count (libvlc_media_player_t *p_mi)


    //==========================================================================
    //libvlc_track_description_t * libvlc_video_get_spu_description (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_video_get_spu_description_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_spu_description_signature m_libvlc_video_get_spu_description;

    //==========================================================================
    public libvlc_track_description_t libvlc_video_get_spu_description(IntPtr p_mi)
    {
      VerifyAccess();

      IntPtr pointer = m_libvlc_video_get_spu_description(p_mi);
      if(pointer == IntPtr.Zero)
        return null;

      internal_libvlc_track_description_t track_description = (internal_libvlc_track_description_t)Marshal.PtrToStructure(pointer, typeof(internal_libvlc_track_description_t));
      libvlc_track_description_t result = new libvlc_track_description_t { i_id = track_description.i_id, psz_name = track_description.psz_name };
      libvlc_track_description_t current = result;
      while(track_description.p_next != IntPtr.Zero)
      {
        track_description = (internal_libvlc_track_description_t)Marshal.PtrToStructure(track_description.p_next, typeof(internal_libvlc_track_description_t));
        current.p_next = new libvlc_track_description_t { i_id = track_description.i_id, psz_name = track_description.psz_name };
        current = current.p_next;
      }
      current.p_next = null;

      m_libvlc_track_description_list_release(pointer);

      return result;
    }


    //==========================================================================
    // int libvlc_video_set_spu (libvlc_media_player_t *p_mi, unsigned i_spu)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_set_spu_signature(IntPtr p_mi, int i_spu);

    //==========================================================================
    private readonly libvlc_video_set_spu_signature m_libvlc_video_set_spu;

    //==========================================================================
    public int libvlc_video_set_spu(IntPtr p_mi, int i_spu)
    {
      VerifyAccess();

      return m_libvlc_video_set_spu(p_mi, i_spu);
    }


    //int libvlc_video_set_subtitle_file (libvlc_media_player_t *p_mi, const char *psz_subtitle)
    //libvlc_track_description_t * libvlc_video_get_title_description (libvlc_media_player_t *p_mi)
    //libvlc_track_description_t * libvlc_video_get_chapter_description (libvlc_media_player_t *p_mi, int i_title)
    //char * libvlc_video_get_crop_geometry (libvlc_media_player_t *p_mi)
    //void libvlc_video_set_crop_geometry (libvlc_media_player_t *p_mi, const char *psz_geometry)
    //int libvlc_video_get_teletext (libvlc_media_player_t *p_mi)
    //void libvlc_video_set_teletext (libvlc_media_player_t *p_mi, int i_page)
    //void libvlc_toggle_teletext (libvlc_media_player_t *p_mi)


    //==========================================================================
    // int libvlc_video_get_track_count (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_track_count_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_track_count_signature m_libvlc_video_get_track_count;

    //==========================================================================
    public int libvlc_video_get_track_count(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_video_get_track_count(p_mi);
    }

    
    //==========================================================================
    // libvlc_track_description_t * libvlc_video_get_track_description (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_video_get_track_description_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_track_description_signature m_libvlc_video_get_track_description;

    //==========================================================================
    public libvlc_track_description_t libvlc_video_get_track_description(IntPtr p_mi)
    {
      VerifyAccess();

      IntPtr pointer = m_libvlc_video_get_track_description(p_mi);
      if(pointer == IntPtr.Zero)
        return null;

      // I still have no clue whether the returned descriptions have to be freed
      // in any way; but i assume not 

      internal_libvlc_track_description_t track_description = (internal_libvlc_track_description_t)Marshal.PtrToStructure(pointer, typeof(internal_libvlc_track_description_t));
      libvlc_track_description_t result = new libvlc_track_description_t { i_id = track_description.i_id, psz_name = track_description.psz_name };
      libvlc_track_description_t current = result;
      while(track_description.p_next != IntPtr.Zero)
      {
        track_description = (internal_libvlc_track_description_t)Marshal.PtrToStructure(track_description.p_next, typeof(internal_libvlc_track_description_t));
        current.p_next = new libvlc_track_description_t { i_id = track_description.i_id, psz_name = track_description.psz_name };
        current = current.p_next;
      }
      current.p_next = null;

      m_libvlc_track_description_list_release(pointer);

      return result;
    }


    //==========================================================================
    // int libvlc_video_get_track (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_get_track_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_video_get_track_signature m_libvlc_video_get_track;

    //==========================================================================
    public int libvlc_video_get_track(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_video_get_track(p_mi);
    }


    //==========================================================================
    // int libvlc_video_set_track (libvlc_media_player_t *p_mi, int i_track)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_video_set_track_signature(IntPtr p_mi, int i_track);

    //==========================================================================
    private readonly libvlc_video_set_track_signature m_libvlc_video_set_track;

    //==========================================================================
    public int libvlc_video_set_track(IntPtr p_mi, int i_track)
    {
      VerifyAccess();

      return m_libvlc_video_set_track(p_mi, i_track);
    }

    //int libvlc_video_take_snapshot (libvlc_media_player_t *p_mi, unsigned num, const char *psz_filepath, unsigned int i_width, unsigned int i_height)
    //void libvlc_video_set_deinterlace (libvlc_media_player_t *p_mi, const char *psz_mode)
    //int libvlc_video_get_marquee_int (libvlc_media_player_t *p_mi, unsigned option)
    //char * libvlc_video_get_marquee_string (libvlc_media_player_t *p_mi, unsigned option)
    //void libvlc_video_set_marquee_int (libvlc_media_player_t *p_mi, unsigned option, int i_val)
    //void libvlc_video_set_marquee_string (libvlc_media_player_t *p_mi, unsigned option, const char *psz_text)
    //int libvlc_video_get_logo_int (libvlc_media_player_t *p_mi, unsigned option)
    //void libvlc_video_set_logo_int (libvlc_media_player_t *p_mi, unsigned option, int value)
    //void libvlc_video_set_logo_string (libvlc_media_player_t *p_mi, unsigned option, const char *psz_value)
    //int libvlc_video_get_adjust_int (libvlc_media_player_t *p_mi, unsigned option)
    //void libvlc_video_set_adjust_int (libvlc_media_player_t *p_mi, unsigned option, int value)
    //float libvlc_video_get_adjust_float (libvlc_media_player_t *p_mi, unsigned option)
    //void libvlc_video_set_adjust_float (libvlc_media_player_t *p_mi, unsigned option, float value)

  } // class LibVLCLibrary

}
