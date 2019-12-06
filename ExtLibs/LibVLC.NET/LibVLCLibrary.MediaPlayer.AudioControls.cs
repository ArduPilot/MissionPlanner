////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.MediaPlayer.AudioControls.cs 
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
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace LibVLC.NET
{

  //****************************************************************************
  partial class LibVLCLibrary
  {

    /*
      VLC_PUBLIC_API libvlc_audio_output_t * libvlc_audio_output_list_get (libvlc_instance_t *p_instance)
      VLC_PUBLIC_API void libvlc_audio_output_list_release (libvlc_audio_output_t *p_list)
      VLC_PUBLIC_API int libvlc_audio_output_set (libvlc_media_player_t *p_mi, const char *psz_name)
      VLC_PUBLIC_API int libvlc_audio_output_device_count (libvlc_instance_t *p_instance, const char *psz_audio_output)
      VLC_PUBLIC_API char * libvlc_audio_output_device_longname (libvlc_instance_t *p_instance, const char *psz_audio_output, int i_device)
      VLC_PUBLIC_API char * libvlc_audio_output_device_id (libvlc_instance_t *p_instance, const char *psz_audio_output, int i_device)
      VLC_PUBLIC_API void libvlc_audio_output_device_set (libvlc_media_player_t *p_mi, const char *psz_audio_output, const char *psz_device_id)
      VLC_PUBLIC_API int libvlc_audio_output_get_device_type (libvlc_media_player_t *p_mi)
      VLC_PUBLIC_API void libvlc_audio_output_set_device_type (libvlc_media_player_t *p_mi, int device_type)
      VLC_PUBLIC_API void libvlc_audio_toggle_mute (libvlc_media_player_t *p_mi)
      VLC_PUBLIC_API int libvlc_audio_get_mute (libvlc_media_player_t *p_mi)
      VLC_PUBLIC_API void libvlc_audio_set_mute (libvlc_media_player_t *p_mi, int status)
    */

    //==========================================================================
    // int libvlc_audio_get_volume (libvlc_media_player_t *p_mi)
    
    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_audio_get_volume_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_audio_get_volume_signature m_libvlc_audio_get_volume;

    //==========================================================================
    public int libvlc_audio_get_volume(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_audio_get_volume(p_mi);
    }

    //==========================================================================
    // int libvlc_audio_set_volume (libvlc_media_player_t *p_mi, int i_volume)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_audio_set_volume_signature(IntPtr p_mi, int i_volume);

    //==========================================================================
    private readonly libvlc_audio_set_volume_signature m_libvlc_audio_set_volume;

    //==========================================================================
    public int libvlc_audio_set_volume(IntPtr p_mi, int i_volume)
    {
      VerifyAccess();

      return m_libvlc_audio_set_volume(p_mi, i_volume);
    }


    //==========================================================================
    // int libvlc_audio_get_track_count (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_audio_get_track_count_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_audio_get_track_count_signature m_libvlc_audio_get_track_count;

    //==========================================================================
    public int libvlc_audio_get_track_count(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_audio_get_track_count(p_mi);
    }



    //==========================================================================
    // libvlc_track_description_t * libvlc_audio_get_track_description (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr libvlc_audio_get_track_description_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_audio_get_track_description_signature m_libvlc_audio_get_track_description;

    //==========================================================================
    public libvlc_track_description_t libvlc_audio_get_track_description(IntPtr p_mi)
    {
      VerifyAccess();

      IntPtr pointer = m_libvlc_audio_get_track_description(p_mi);
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
    // int libvlc_audio_get_track (libvlc_media_player_t *p_mi)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_audio_get_track_signature(IntPtr p_mi);

    //==========================================================================
    private readonly libvlc_audio_get_track_signature m_libvlc_audio_get_track;

    //==========================================================================
    public int libvlc_audio_get_track(IntPtr p_mi)
    {
      VerifyAccess();

      return m_libvlc_audio_get_track(p_mi);
    }


    //==========================================================================
    // int libvlc_audio_set_track (libvlc_media_player_t *p_mi, int i_track)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_audio_set_track_signature(IntPtr p_mi, int i_track);

    //==========================================================================
    private readonly libvlc_audio_set_track_signature m_libvlc_audio_set_track;

    //==========================================================================
    public int libvlc_audio_set_track(IntPtr p_mi, int i_track)
    {
      VerifyAccess();

      return m_libvlc_audio_set_track(p_mi, i_track);
    }


    /*
      VLC_PUBLIC_API int libvlc_audio_get_channel (libvlc_media_player_t *p_mi)
      VLC_PUBLIC_API int libvlc_audio_set_channel (libvlc_media_player_t *p_mi, int channel)
      VLC_PUBLIC_API int64_t libvlc_audio_get_delay (libvlc_media_player_t *p_mi)
      VLC_PUBLIC_API int libvlc_audio_set_delay (libvlc_media_player_t *p_mi, int64_t i_delay)
    */

  } // class LibVLCLibrary

}
