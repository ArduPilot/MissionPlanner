////////////////////////////////////////////////////////////////////////////////
//
//  LibVLCLibrary.Core.AsynchronousEvents.cs 
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

    //****************************************************************************
    public enum libvlc_event_e 
      : int
    {
      libvlc_MediaMetaChanged = 0,
      libvlc_MediaSubItemAdded,
      libvlc_MediaDurationChanged,
      libvlc_MediaParsedChanged,
      libvlc_MediaFreed,
      libvlc_MediaStateChanged,

      libvlc_MediaPlayerMediaChanged = 0x100,
      libvlc_MediaPlayerNothingSpecial,
      libvlc_MediaPlayerOpening,
      libvlc_MediaPlayerBuffering,
      libvlc_MediaPlayerPlaying,
      libvlc_MediaPlayerPaused,
      libvlc_MediaPlayerStopped,
      libvlc_MediaPlayerForward,
      libvlc_MediaPlayerBackward,
      libvlc_MediaPlayerEndReached,
      libvlc_MediaPlayerEncounteredError,
      libvlc_MediaPlayerTimeChanged,
      libvlc_MediaPlayerPositionChanged,
      libvlc_MediaPlayerSeekableChanged,
      libvlc_MediaPlayerPausableChanged,
      libvlc_MediaPlayerTitleChanged,
      libvlc_MediaPlayerSnapshotTaken,
      libvlc_MediaPlayerLengthChanged,

      libvlc_MediaListItemAdded = 0x200,
      libvlc_MediaListWillAddItem,
      libvlc_MediaListItemDeleted,
      libvlc_MediaListWillDeleteItem,

      libvlc_MediaListViewItemAdded = 0x300,
      libvlc_MediaListViewWillAddItem,
      libvlc_MediaListViewItemDeleted,
      libvlc_MediaListViewWillDeleteItem,

      libvlc_MediaListPlayerPlayed = 0x400,
      libvlc_MediaListPlayerNextItemSet,
      libvlc_MediaListPlayerStopped,

      libvlc_MediaDiscovererStarted = 0x500,
      libvlc_MediaDiscovererEnded,

      libvlc_VlmMediaAdded = 0x600,
      libvlc_VlmMediaRemoved,
      libvlc_VlmMediaChanged,
      libvlc_VlmMediaInstanceStarted,
      libvlc_VlmMediaInstanceStopped,
      libvlc_VlmMediaInstanceStatusInit,
      libvlc_VlmMediaInstanceStatusOpening,
      libvlc_VlmMediaInstanceStatusPlaying,
      libvlc_VlmMediaInstanceStatusPause,
      libvlc_VlmMediaInstanceStatusEnd,
      libvlc_VlmMediaInstanceStatusError
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_meta_changed
    {
      public libvlc_meta_t meta_type;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_subitem_added
    {
      public IntPtr new_child;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_duration_changed
    {
      public long new_duration;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_parsed_changed
    {
      public int new_status;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_freed
    {
      public IntPtr md;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_state_changed
    {
      public libvlc_state_t new_state;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_buffering
    {
      public float new_cache;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_position_changed
    {
      public float new_position;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_time_changed
    {
      public long new_time;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_title_changed
    {
      public int new_title;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_seekable_changed
    {
      public int new_seekable;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_pausable_changed
    {
      public int new_pausable;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_list_item_added
    {
      public IntPtr item;
      public int index;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_list_will_add_item
    {
      public IntPtr item;
      public int index;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_list_item_deleted
    {
      public IntPtr item;
      public int index;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_list_will_delete_item
    {
      public IntPtr item;
      public int index;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_list_player_next_item_set
    {
      public IntPtr item;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_snapshot_taken
    {
      public IntPtr psz_filename;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_length_changed
    {
      public long new_length;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct vlm_media_event
    {
      public IntPtr psz_media_name;
      public IntPtr psz_instance_name;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct media_player_media_changed
    {
      public IntPtr new_media;
    }

    //==========================================================================
    [StructLayout(LayoutKind.Explicit)]
    public struct libvlc_event_t
    {
      //------------------------------------------------------------------------
      [FieldOffset(0)]
      public libvlc_event_e type;

      //------------------------------------------------------------------------
      [FieldOffset(4)]
      public IntPtr p_obj;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_meta_changed media_meta_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_subitem_added media_subitem_added;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_duration_changed media_duration_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_parsed_changed media_parsed_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_freed media_freed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_state_changed media_state_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_buffering media_player_buffering;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_position_changed media_player_position_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_time_changed media_player_time_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_title_changed media_player_title_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_seekable_changed media_player_seekable_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_pausable_changed media_player_pausable_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_list_item_added media_list_item_added;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_list_will_add_item media_list_will_add_item;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_list_item_deleted media_list_item_deleted;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_list_will_delete_item media_list_will_delete_item;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_list_player_next_item_set media_list_player_next_item_set;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_snapshot_taken media_player_snapshot_taken;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_length_changed media_player_length_changed;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public vlm_media_event vlm_media_event;

      //------------------------------------------------------------------------
      [FieldOffset(8)]
      public media_player_media_changed media_player_media_changed;

    } // struct libvlc_event_t

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void libvlc_callback_t(ref libvlc_event_t event_type, IntPtr user_data);

    // int libvlc_event_attach (libvlc_event_manager_t *p_event_manager, libvlc_event_type_t i_event_type, libvlc_callback_t f_callback, void *user_data)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int libvlc_event_attach_signature(IntPtr p_event_manager, libvlc_event_e i_event_type, libvlc_callback_t callback, IntPtr user_data);

    //==========================================================================
    private readonly libvlc_event_attach_signature m_libvlc_event_attach;

    //==========================================================================
    public int libvlc_event_attach(IntPtr p_event_manager, libvlc_event_e i_event_type, libvlc_callback_t callback, IntPtr user_data)
    {
      VerifyAccess();

      return m_libvlc_event_attach(p_event_manager, i_event_type, callback, user_data);
    }

    // void libvlc_event_detach (libvlc_event_manager_t *p_event_manager, libvlc_event_type_t i_event_type, libvlc_callback_t f_callback, void *p_user_data)

    //==========================================================================
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void libvlc_event_detach_signature(IntPtr p_event_manager, libvlc_event_e i_event_type, libvlc_callback_t callback, IntPtr user_data);

    //==========================================================================
    private readonly libvlc_event_detach_signature m_libvlc_event_detach;

    //==========================================================================
    public void libvlc_event_detach(IntPtr p_event_manager, libvlc_event_e i_event_type, libvlc_callback_t callback, IntPtr user_data)
    {
      VerifyAccess();

      m_libvlc_event_detach(p_event_manager, i_event_type, callback, user_data);
    }

    /*
    const char * libvlc_event_type_name (libvlc_event_type_t event_type)
    */

  } // class LibVLCLibrary

}
