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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace DirectShowLib
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES
    
    /// <summary>
    /// From AMVPSIZE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVPSize
    {
        public int           dwWidth;                // the width
        public int           dwHeight;               // the height
    }

    /// <summary>
    /// From DDVIDEOPORTCONNECT
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DDVideoPortConnect
    {
        public int dwSize;
        public int dwPortWidth;
        public Guid guidTypeID;
        public int dwFlags;
        public IntPtr dwReserved1;
    }

    /// <summary>
    /// From AMVPDATAINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VPDataInfo
    {
        public int dwSize;
        public int dwMicrosecondsPerField;
        public AMVPDimInfo amvpDimInfo;
        public int dwPictAspectRatioX;
        public int dwPictAspectRatioY;
        public bool bEnableDoubleClock;
        public bool bEnableVACT;
        public bool bDataIsInterlaced;
        public int lHalfLinesOdd;
        public bool bFieldPolarityInverted;
        public int dwNumLinesInVREF;
        public int lHalfLinesEven;
        public int dwReserved1;
    }

    /// <summary>
    /// From AMVPDIMINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVPDimInfo
    {
        public int dwFieldWidth;
        public int dwFieldHeight;
        public int dwVBIWidth;
        public int dwVBIHeight;
        public Rectangle rcValidRegion;
    }

#endif

    /// <summary>
    /// From AMVP_MODE
    /// </summary>
    public enum AMVP_Mode
    {
        Weave,
        BobInterleaved,
        BobNonInterleaved,
        SkipEven,
        SkipOdd
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPBaseConfig
    {
        [PreserveSig]
        int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        int SetInvertPolarity(
            );

        [PreserveSig]
        int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BC29A660-30E3-11d0-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPConfig : IVPBaseConfig
    {
    #region IVPBaseConfig Methods

        [PreserveSig]
        new int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        new int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        new int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        new int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        new int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        new int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        new int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        new int SetInvertPolarity(
            );

        [PreserveSig]
        new int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        new int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        new int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        new int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        new int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );

        #endregion

        [PreserveSig]
        int IsVPDecimationAllowed(
            [MarshalAs(UnmanagedType.Bool)] out bool pbIsDecimationAllowed
            );

        [PreserveSig]
        int SetScalingFactors(
            AMVPSize pamvpSize
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EC529B00-1A1F-11D1-BAD9-00609744111A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPVBIConfig : IVPBaseConfig
    {
    #region IVPBaseConfig Methods

        [PreserveSig]
        new int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        new int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        new int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        new int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        new int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        new int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        new int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        new int SetInvertPolarity(
            );

        [PreserveSig]
        new int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        new int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        new int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        new int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        new int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EC529B01-1A1F-11D1-BAD9-00609744111A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPVBINotify : IVPBaseNotify
    {
    #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion
    }

#endif

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPBaseNotify
    {
        [PreserveSig]
        int RenegotiateVPParameters();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C76794A1-D6C5-11d0-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPNotify : IVPBaseNotify
    {
        #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion

        [PreserveSig]
        int SetDeinterlaceMode(
            AMVP_Mode mode
            );

        [PreserveSig]
        int GetDeinterlaceMode(
            out AMVP_Mode pMode
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EBF47183-8764-11d1-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPNotify2 : IVPNotify
    {
        #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion

        #region IVPNotify Methods

        [PreserveSig]
        new int SetDeinterlaceMode(
            AMVP_Mode mode
            );

        [PreserveSig]
        new int GetDeinterlaceMode(
            out AMVP_Mode pMode
            );

        #endregion

        [PreserveSig]
        int SetVPSyncMaster(
            [MarshalAs(UnmanagedType.Bool)] bool bVPSyncMaster
            );

        [PreserveSig]
        int GetVPSyncMaster(
            [MarshalAs(UnmanagedType.Bool)] out bool pbVPSyncMaster
            );
    }

    #endregion
}
