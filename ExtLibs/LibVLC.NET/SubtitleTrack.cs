////////////////////////////////////////////////////////////////////////////////
//
//  SubtitleTrack.cs - This file is part of LibVLC.NET.
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

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents a subtitle track.
  /// </summary>
  public sealed class SubtitleTrack
    : Track
  {

    //==========================================================================
    internal SubtitleTrack(int index, LibVLCLibrary.libvlc_track_description_t trackDescription, LibVLCLibrary.libvlc_media_track_info_t? trackInfo)
      : base(index, trackDescription, trackInfo)
    {
      // ...
    }

  } // class SubtitleTrack

}
