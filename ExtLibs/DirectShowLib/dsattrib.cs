#region license

/*
DirectShowLib - Provide access to DirectShow interfaces via .NET
Copyright (C) 2007
http://sourceforge.net/projects/directshownet/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#endregion

using System;
using System.Runtime.InteropServices;

namespace DirectShowLib.BDA
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// Defines
    /// </summary>
    public enum SAMPLE_SEQ
    {
        SequenceHeader = 1,
        GOPHeader = 2,
        PictureHeader = 3,

        SequenceStart = 1,
        SeekPoint = 2,
        FrameStart = 3
    }

    /// <summary>
    /// Defines
    /// </summary>
    public enum SAMPLE_SEQ_CONTENT
    {
        Unknown = 0,
        IFrame = 1,
        PFrame = 2,
        BFrame = 3,
        StandAloneFrame = 1,
        RefFrame = 2,
        NonRefFrame = 3
    }

    /// <summary>
    /// From VA_VIDEO_FORMAT
    /// </summary>
    public enum VA_VIDEO_FORMAT
    {
        Component = 0,
        PAL = 1,
        NTSC = 2,
        SECAM = 3,
        MAC = 4,

        Unspecified = 5
    }

    /// <summary>
    /// VA_COLOR_PRIMARIES
    /// </summary>
    public enum VA_COLOR_PRIMARIES
    {
        ITU_R_BT_709 = 1,
        UNSPECIFIED = 2,
        ITU_R_BT_470_SYSTEM_M = 4,
        ITU_R_BT_470_SYSTEM_B_G = 5,
        SMPTE_170M = 6,
        SMPTE_240M = 7,
        H264_GENERIC_FILM = 8
    }

    /// <summary>
    /// From VA_TRANSFER_CHARACTERISTICS
    /// </summary>
    public enum VA_TRANSFER_CHARACTERISTICS
    {
        ITU_R_BT_709 = 1,
        UNSPECIFIED = 2,
        ITU_R_BT_470_SYSTEM_M = 4,
        ITU_R_BT_470_SYSTEM_B_G = 5,
        SMPTE_170M = 6,
        SMPTE_240M = 7,
        LINEAR = 8,
        H264_LOG_100_TO_1 = 9,
        H264_LOG_316_TO_1 = 10
    }

    /// <summary>
    /// VA_MATRIX_COEFFICIENTS
    /// </summary>
    public enum VA_MATRIX_COEFFICIENTS
    {
        H264_RGB = 0,
        ITU_R_BT_709 = 1,
        UNSPECIFIED = 2,
        FCC = 4,
        ITU_R_BT_470_SYSTEM_B_G = 5,
        SMPTE_170M = 6,
        SMPTE_240M = 7,
        H264_YCgCo = 8
    }

    /// <summary> 
    /// UDCR_TAG 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class UDCR_TAG
    {
        public byte bVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public byte KID;
        public long ullBaseCounter;
        public long ullBaseCounterRange;
        public bool fScrambled;
        public byte bStreamMark;
        public int dwReserved1;
        public int dwReserved2;
    }

    /// <summary>
    /// From VA_OPTIONAL_VIDEO_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VA_OPTIONAL_VIDEO_PROPERTIES
    {
        public short dwPictureHeight;
        public short dwPictureWidth;
        public short dwAspectRatioX;
        public short dwAspectRatioY;
        public VA_VIDEO_FORMAT VAVideoFormat;
        public VA_COLOR_PRIMARIES VAColorPrimaries;
        public VA_TRANSFER_CHARACTERISTICS VATransferCharacteristics;
        public VA_MATRIX_COEFFICIENTS VAMatrixCoefficients;
    }

    /// <summary>
    /// From TRANSPORT_PROPERTIES
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class TRANSPORT_PROPERTIES
    {
        public int PID;
        public long PCR;
        public long Value;
    } 

    /// <summary>
    /// From PBDA_TAG_ATTRIBUTE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class PBDA_TAG_ATTRIBUTE
    {
        public Guid TableUUId;
        public byte TableId;
        public short VersionNo;
        public int TableDataSize;
        public byte TableData; // Array of bytes
    } 

    /// <summary>
    /// From CAPTURE_STREAMTIME
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class CAPTURE_STREAMTIME
    {
        public long StreamTime;
    }

    /// <summary>
    /// From DSHOW_STREAM_DESC
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class DSHOW_STREAM_DESC
    {
        public int VersionNo;
        public int StreamId;
        public bool Default;
        public bool Creation;
        public int Reserved;
    }

    /// <summary>
    /// From SAMPLE_LIVE_STREAM_TIME
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class SAMPLE_LIVE_STREAM_TIME
    {
        public long qwStreamTime;
        public long qwLiveTime;
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("583ec3cc-4960-4857-982b-41a33ea0a006"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAttributeSet
    {
        [PreserveSig]
        int SetAttrib(
          [In] Guid guidAttribute,
          [In] IntPtr pbAttribute,
          [In] int dwAttributeLength
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("52dbd1ec-e48f-4528-9232-f442a68f0ae1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAttributeGet
    {
        [PreserveSig]
        int GetCount([Out] out int plCount);

        [PreserveSig]
        int GetAttribIndexed(
          [In] int lIndex,
          [Out] out Guid guidAttribute,
          [In, Out] IntPtr pbAttribute,
          [In, Out] ref int dwAttributeLength
          );

        [PreserveSig]
        int GetAttrib(
          [In] Guid guidAttribute,
          [In, Out] IntPtr pbAttribute,
          [In, Out] ref int dwAttributeLength
          );
    }

#endif

    #endregion
}
