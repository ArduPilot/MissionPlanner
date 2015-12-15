////////////////////////////////////////////////////////////////////////////////
//
//  VideoTrack.cs - This file is part of LibVLC.NET.
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
using System.Text;

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents a video track.
  /// </summary>
  public sealed class VideoTrack
    : Track
  {

    //==========================================================================
    internal VideoTrack(int index, LibVLCLibrary.libvlc_track_description_t trackDescription, LibVLCLibrary.libvlc_media_track_info_t? trackInfo)
      : base(index, trackDescription, trackInfo)
    {
      if(trackInfo.HasValue)
      {
        m_Width = (int)trackInfo.Value.video.i_width;
        m_Height = (int)trackInfo.Value.video.i_height;
      }
    }

    //==========================================================================
    /// <summary>
    ///   Overrides <see cref="Track.ToString"/> and returns a string
    ///   representing the video track.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return new StringBuilder(base.ToString())
      .Append(" (")
      .Append(Codec)
      .Append(", ")
      .Append(m_Width).Append("x").Append(m_Height).Append(")")

      .ToString();
    }

    #region Properties

    #region Width

    //==========================================================================
    private readonly int m_Width;

    //==========================================================================                
    /// <summary>
    ///   Gets the width (in pixels) of the video track.
    /// </summary>
    public int Width
    {
      get
      {
        return m_Width;
      }
    }

    #endregion // Width

    #region Height

    //==========================================================================
    private readonly int m_Height;

    //==========================================================================                
    /// <summary>
    ///   Gets the height (in pixels) of the video track.
    /// </summary>
    public int Height
    {
      get
      {
        return m_Height;
      }
    }

    #endregion // Height

    #endregion // Properties

  } // class VideoTrack

}
