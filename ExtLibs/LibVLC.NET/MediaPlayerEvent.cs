////////////////////////////////////////////////////////////////////////////////
//
//  MediaPlayerEvent.cs - This file is part of LibVLC.NET.
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

#pragma warning disable 1591

namespace LibVLC.NET
{

  //****************************************************************************
  public enum MediaPlayerEvent
    : int
  {
    Backward = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerBackward,
    Buffering = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerBuffering,
    EncounteredError = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerEncounteredError,
    EndReached = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerEndReached,
    Forward = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerForward,
    LengthChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerLengthChanged,
    MediaChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerMediaChanged,
    NothingSpecial = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerNothingSpecial,
    Opening = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerOpening,
    PausableChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPausableChanged,
    Paused = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPaused,
    Playing = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPlaying,
    PositionChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerPositionChanged,
    SeekableChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerSeekableChanged,
    SnapshotTaken = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerSnapshotTaken,
    Stopped = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerStopped,
    TimeChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerTimeChanged,
    TitleChanged = LibVLCLibrary.libvlc_event_e.libvlc_MediaPlayerTitleChanged 

  } // enum MediaPlayerEvent

  //****************************************************************************
  public delegate void MediaPlayerEventHandler(object sender, MediaPlayerEventArgs e);

  //****************************************************************************
  public class MediaPlayerEventArgs
    : EventArgs
  {

    //==========================================================================
    public MediaPlayerEventArgs(MediaPlayerEvent mediaPlayerEvent)
    {
      m_Event = mediaPlayerEvent;
    }

    #region Event

    //==========================================================================
    private readonly MediaPlayerEvent m_Event;

    //==========================================================================                
    /// <summary>
    ///   Gets the value of Event of the MediaPlayerEventArgs.
    /// </summary>
    public MediaPlayerEvent Event
    {
      get
      {
        // Dispatcher.VerifyAccess();

        return m_Event;
      }
    }

    #endregion // Event


  } // class MediaPlayerEventArgs

}