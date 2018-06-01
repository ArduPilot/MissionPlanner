////////////////////////////////////////////////////////////////////////////////
//
//  MediaPlayerState.cs - This file is part of LibVLC.NET.
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
//  $LastChangedRevision: 4950 $
//  $LastChangedDate: 2011-04-05 21:48:05 +0200 (Tue, 05 Apr 2011) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System;

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents the finite state of a media player.
  /// </summary>
  /// <seealso cref="MediaPlayer.State"/>
  public enum MediaPlayerState
    : int
  {
    //==========================================================================
    /// <summary>
    ///   The media player is currently idle.
    /// </summary>
    NothingSpecial = LibVLCLibrary.libvlc_state_t.libvlc_NothingSpecial,

    //==========================================================================
    /// <summary>
    ///   The media player is currently opening a media.
    /// </summary>
    Opening = LibVLCLibrary.libvlc_state_t.libvlc_Opening,

    //==========================================================================
    /// <summary>
    ///   The media player is currently playing a media.
    /// </summary>
    Playing = LibVLCLibrary.libvlc_state_t.libvlc_Playing,

    //==========================================================================
    /// <summary>
    ///   The media player is currently buffering.
    /// </summary>
    Buffering = LibVLCLibrary.libvlc_state_t.libvlc_Buffering,

    //==========================================================================
    /// <summary>
    ///   Playback of a media has currently paused.
    /// </summary>
    Paused = LibVLCLibrary.libvlc_state_t.libvlc_Paused,

    //==========================================================================
    /// <summary>
    ///   The media player has reached the end of a media.
    /// </summary>
    Ended = LibVLCLibrary.libvlc_state_t.libvlc_Ended,

    //==========================================================================
    /// <summary>
    ///   The media player has stopped playing a media.
    /// </summary>
    Stopped = LibVLCLibrary.libvlc_state_t.libvlc_Stopped,

    //==========================================================================
    /// <summary>
    ///   There has been an error opening a media.
    /// </summary>
    Error = LibVLCLibrary.libvlc_state_t.libvlc_Error,

  } // enum MediaPlayerState

}