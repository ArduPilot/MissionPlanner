////////////////////////////////////////////////////////////////////////////////
//
//  VideoBuffer.cs - This file is part of LibVLC.NET.
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
using System.Runtime.InteropServices;

namespace LibVLC.NET
{

  //****************************************************************************
  /// <summary>
  ///   Represents the video buffer of a media player.
  /// </summary>
  public class VideoBuffer
  {

    //==========================================================================
    /// <summary>
    ///   Create a new instance of the class <see cref="VideoBuffer"/>.
    /// </summary>
    /// <param name="width">
    ///   The width of the video.
    /// </param>
    /// <param name="height">
    ///   The height of the video.
    /// </param>
    /// <param name="pixelFormat">
    ///   The pixel format of the video.
    /// </param>
    public VideoBuffer(uint width, uint height, PixelFormat pixelFormat)
    {
      Width = width;
      Height = height;
      PixelFormat = pixelFormat;
      Stride = Width * 4;
      Lines = Height;
      FrameBuffer = new byte[Stride * Lines];
    }

   //==========================================================================
    private GCHandle m_GCHandle = default(GCHandle);

    //==========================================================================
    internal IntPtr Lock()
    {
      return (m_GCHandle = GCHandle.Alloc(FrameBuffer, GCHandleType.Pinned)).AddrOfPinnedObject();
    }

    //==========================================================================
    internal void Unlock()
    {
      m_GCHandle.Free();
    }

    //==========================================================================
    /// <summary>
    ///   Gets or sets the width of the video in pixels.
    /// </summary>
    public readonly uint Width;

    //==========================================================================
    /// <summary>
    ///   Gets the height of the video in pixels.
    /// </summary>
    public readonly uint Height;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the pixel format of the video frame.
    /// </summary>
    public readonly PixelFormat PixelFormat;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the stride of a video frame which is the width 
    ///   multiplied by the bytes per pixel.
    /// </summary>
    public readonly uint Stride;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the number of scan lines.
    /// </summary>
    public readonly uint Lines;

    //==========================================================================
    /// <summary>
    ///   Gets or sets the video frame buffer.
    /// </summary>
    public readonly byte[] FrameBuffer;

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
