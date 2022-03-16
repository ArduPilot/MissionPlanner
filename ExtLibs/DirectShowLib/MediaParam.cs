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
using System.Security;

namespace DirectShowLib.DMO
{
    #region Declarations

    sealed public class MediaParamTimeFormat
    {
        private MediaParamTimeFormat()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> GUID_TIME_REFERENCE </summary>
        public static readonly Guid Reference = new Guid(0x93ad712b, 0xdaa0, 0x4ffe, 0xbc, 0x81, 0xb0, 0xce, 0x50, 0x0f, 0xcd, 0xd9);

        /// <summary> GUID_TIME_MUSIC </summary>
        public static readonly Guid Music = new Guid(0x0574c49d, 0x5b04, 0x4b15, 0xa5, 0x42, 0xae, 0x28, 0x20, 0x30, 0x11, 0x7b);

        /// <summary> GUID_TIME_SAMPLES, audio capture category </summary>
        public static readonly Guid Samples = new Guid(0xa8593d05, 0x0c43, 0x4984, 0x9a, 0x63, 0x97, 0xaf, 0x9e, 0x02, 0xc4, 0xc0);
    }

    /// <summary>
    /// From MP_DATA
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MPData
    {
        [FieldOffset(0)] public bool vBool;
        [FieldOffset(0)] public float vFloat;
        [FieldOffset(0)] public int vInt;
    }

    /// <summary>
    /// From MP_ENVELOPE_SEGMENT
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=8)]
    public struct MPEnvelopeSegment
    {
        public long rtStart;
        public long rtEnd;
        public MPData valStart;
        public MPData valEnd;
        public MPCaps iCurve;
        public MPFlags flags;
    }

    /// <summary>
    /// From MPF_ENVLP_* defines
    /// </summary>
    [Flags]
    public enum MPFlags
    {
        Standard = 0x0,
        BeginCurrentVal = 0x1,
        BeginNeutralVal = 0x2
    }

    /// <summary>
    /// From MP_TYPE
    /// </summary>
    public enum MPType
    {
        // Fields
        BOOL = 2,
        ENUM = 3,
        FLOAT = 1,
        INT = 0,
        MAX = 4
    }

    /// <summary>
    /// From MP_CAPS_CURVE* defines
    /// </summary>
    [Flags]
    public enum MPCaps
    {
        None = 0,
        Jump = 0x1,
        Linear = 0x2,
        Square = 0x4,
        InvSquare = 0x8,
        Sine = 0x10
    }

    /// <summary>
    /// From MP_PARAMINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct ParamInfo
    {
        public MPType mpType;
        public MPCaps mopCaps;
        public MPData mpdMinValue;
        public MPData mpdMaxValue;
        public MPData mpdNeutralValue;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x20)]
        public string szUnitText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x20)]
        public string szLabel;
    }

    #endregion

    #region Interfaces

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("6D6CBB60-A223-44AA-842F-A2F06750BE6D")]
    public interface IMediaParamInfo
    {
        [PreserveSig]
        int GetParamCount(
            out int pdwParams
            );

        [PreserveSig]
        int GetParamInfo(
            [In] int dwParamIndex,
            out ParamInfo pInfo
            );

        [PreserveSig]
        int GetParamText(
            [In] int dwParamIndex,
            out IntPtr ip
            );

        [PreserveSig]
        int GetNumTimeFormats(
            out int pdwNumTimeFormats
            );

        [PreserveSig]
        int GetSupportedTimeFormat(
            [In] int dwFormatIndex,
            out Guid pguidTimeFormat
            );

        [PreserveSig]
        int GetCurrentTimeFormat(
            out Guid pguidTimeFormat,
            out int pTimeData
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("6D6CBB61-A223-44AA-842F-A2F06750BE6E")]
    public interface IMediaParams
    {
        [PreserveSig]
        int GetParam(
            [In] int dwParamIndex,
            out MPData pValue
            );

        [PreserveSig]
        int SetParam(
            [In] int dwParamIndex,
            [In] MPData value
            );

        [PreserveSig]
        int AddEnvelope(
            [In] int dwParamIndex,
            [In] int cSegments,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] MPEnvelopeSegment [] pEnvelopeSegments
            );

        [PreserveSig]
        int FlushEnvelope(
            [In] int dwParamIndex,
            [In] long refTimeStart,
            [In] long refTimeEnd
            );

        [PreserveSig]
        int SetTimeFormat(
            [In] Guid MediaParamTimeFormat,
            [In] int mpTimeData
            );
    }

    #endregion
}
