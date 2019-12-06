////////////////////////////////////////////////////////////////////////////////
//
//  VideoFormat.cs - This file is part of LibVLC.NET.
//
//    Copyright (C) 2011 Boris Richter <himself@boris-richter.net>
//
//  --------------------------------------------------------------------------
//  
//    LibVLC.NET is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    LibVLC.NET is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with LibVLC.NET. If not, see <http://www.gnu.org/licenses/>.
//
//  --------------------------------------------------------------------------
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
  ///   Describes the format of a video frame.
  /// </summary>
  public struct VideoFormat
  {

    //==========================================================================
    /// <summary>
    ///   Gets or sets the width of the video in pixels.
    /// </summary>
    public int Width;

    //==========================================================================
    /// <summary>
    ///   Gets the height of the video in pixels.
    /// </summary>
    public int Height;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the pixel format of the video frame.
    /// </summary>
    public PixelFormat PixelFormat;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the stride of a video frame which typically is its 
    ///   width multiplied by the bytes per pixel.
    /// </summary>
    public int Stride;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the size of the video frame buffer in bytes.
    /// </summary>
    public int BufferSize;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the buffer.
    /// </summary>
    public IntPtr Buffer;

      //==========================================================================
    /// <summary>
    ///   Override <see cref="Object.ToString"/> and returns a string 
    ///   representation of the video size.
    /// </summary>
    /// <returns>
    ///   A string representing the video format.
    /// </returns>
    public override string ToString()
    {
      return String.Format("{0}x{1}@{2}", Width, Height, PixelFormat);
    }

  } // class VideoFrame

}
