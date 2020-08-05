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
    /// From VMRPresentationFlags
    /// </summary>
    [Flags]
    public enum VMRPresentationFlags
    {
        None = 0,
        SyncPoint = 0x00000001,
        Preroll = 0x00000002,
        Discontinuity = 0x00000004,
        TimeValid = 0x00000008,
        SrcDstRectsValid = 0x00000010
    }

    /// <summary>
    /// From VMRSurfaceAllocationFlags
    /// </summary>
    [Flags]
    public enum VMRSurfaceAllocationFlags
    {
        None = 0,
        PixelFormatValid = 0x01,
        ThreeDTarget = 0x02,
        AllowSysMem = 0x04,
        ForceSysMem = 0x08,
        DirectedFlip = 0x10,
        DXVATarget = 0x20
    }

    /// <summary>
    /// From VMRPRESENTATIONINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRPresentationInfo
    {
        public VMRPresentationFlags dwFlags;
        public IntPtr lpSurf; //LPDIRECTDRAWSURFACE7
        public long rtStart;
        public long rtEnd;
        public Size szAspectRatio;
        public DsRect rcSrc;
        public DsRect rcDst;
        public int dwTypeSpecificFlags;
        public int dwInterlaceFlags;
    }

    /// <summary>
    /// From VMRALLOCATIONINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRAllocationInfo
    {
        public VMRSurfaceAllocationFlags dwFlags;
        //    public BitmapInfoHeader lpHdr;
        //    public DDPixelFormat lpPixFmt;
        public IntPtr lpHdr;
        public IntPtr lpPixFmt;
        public Size szAspectRatio;
        public int dwMinBuffers;
        public int dwMaxBuffers;
        public int dwInterlaceFlags;
        public Size szNativeSize;
    }

#endif

    /// <summary>
    /// From VMRDeinterlaceTech
    /// </summary>
    [Flags]
    public enum VMRDeinterlaceTech
    {
        Unknown = 0x0000,
        BOBLineReplicate = 0x0001,
        BOBVerticalStretch = 0x0002,
        MedianFiltering = 0x0004,
        EdgeFiltering = 0x0010,
        FieldAdaptive = 0x0020,
        PixelAdaptive = 0x0040,
        MotionVectorSteered = 0x0080
    }

    /// <summary>
    /// From VMRBITMAP_* defines
    /// </summary>
    [Flags]
    public enum VMRBitmap
    {
        None = 0,
        Disable = 0x00000001,
        Hdc = 0x00000002,
        EntireDDS = 0x00000004,
        SRCColorKey = 0x00000008,
        SRCRect = 0x00000010
    }


    /// <summary>
    /// From VMRDeinterlacePrefs
    /// </summary>
    [Flags]
    public enum VMRDeinterlacePrefs
    {
        None = 0,
        NextBest = 0x01,
        BOB = 0x02,
        Weave = 0x04,
        Mask = 0x07
    }

    /// <summary>
    /// From VMRMixerPrefs
    /// </summary>
    [Flags]
    public enum VMRMixerPrefs
    {
        None = 0,
        NoDecimation = 0x00000001,
        DecimateOutput = 0x00000002,
        ARAdjustXorY = 0x00000004,
        DecimationReserved = 0x00000008,
        DecimateMask = 0x0000000F,

        BiLinearFiltering = 0x00000010,
        PointFiltering = 0x00000020,
        FilteringMask = 0x000000F0,

        RenderTargetRGB = 0x00000100,
        RenderTargetYUV = 0x00001000,

        RenderTargetYUV420 = 0x00000200,
        RenderTargetYUV422 = 0x00000400,
        RenderTargetYUV444 = 0x00000800,
        RenderTargetReserved = 0x0000E000,
        RenderTargetMask = 0x0000FF00,

        DynamicSwitchToBOB = 0x00010000,
        DynamicDecimateBy2 = 0x00020000,

        DynamicReserved = 0x000C0000,
        DynamicMask = 0x000F0000
    }

    /// <summary>
    /// From VMRRenderPrefs
    /// </summary>
    [Flags]
    public enum VMRRenderPrefs
    {
        RestrictToInitialMonitor = 0x00000000,
        ForceOffscreen = 0x00000001,
        ForceOverlays = 0x00000002,
        AllowOverlays = 0x00000000,
        AllowOffscreen = 0x00000000,
        DoNotRenderColorKeyAndBorder = 0x00000008,
        Reserved = 0x00000010,
        PreferAGPMemWhenMixing = 0x00000020,

        Mask = 0x0000003f,
    }

    /// <summary>
    /// From VMRMode
    /// </summary>
    [Flags]
    public enum VMRMode
    {
        None = 0,
        Windowed = 0x00000001,
        Windowless = 0x00000002,
        Renderless = 0x00000004,
    }

    /// <summary>
    /// From VMR_ASPECT_RATIO_MODE
    /// </summary>
    public enum VMRAspectRatioMode
    {
        None,
        LetterBox
    }

    /// <summary>
    /// From VMRALPHABITMAP
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRAlphaBitmap
    {
        public VMRBitmap dwFlags;
        public IntPtr hdc; // HDC
        public IntPtr pDDS; //LPDIRECTDRAWSURFACE7
        public DsRect rSrc;
        public NormalizedRect rDest;
        public float fAlpha;
        public int clrSrcKey;
    }


    /// <summary>
    /// From VMRDeinterlaceCaps
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRDeinterlaceCaps
    {
        public int dwSize;
        public int dwNumPreviousOutputFrames;
        public int dwNumForwardRefSamples;
        public int dwNumBackwardRefSamples;
        public VMRDeinterlaceTech DeinterlaceTechnology;
    }

    /// <summary>
    /// From VMRFrequency
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRFrequency
    {
        public int dwNumerator;
        public int dwDenominator;
    }

    /// <summary>
    /// From VMRVideoDesc
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct VMRVideoDesc
    {
        public int dwSize;
        public int dwSampleWidth;
        public int dwSampleHeight;
        [MarshalAs(UnmanagedType.Bool)] public bool SingleFieldPerSample;
        public int dwFourCC;
        public VMRFrequency InputSampleFreq;
        public VMRFrequency OutputFrameFreq;
    }

    /// <summary>
    /// From VMRVIDEOSTREAMINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRVideoStreamInfo
    {
        public IntPtr pddsVideoSurface;
        public int dwWidth;
        public int dwHeight;
        public int dwStrmID;
        public float fAlpha;
        public DDColorKey ddClrKey;
        public NormalizedRect rNormal;
    }

    /// <summary>
    /// From DDCOLORKEY
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DDColorKey
    {
        public int dw1;
        public int dw2;
    }

    /// <summary>
    /// From VMRMONITORINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct VMRMonitorInfo
    {
        public VMRGuid guid;
        public DsRect rcMonitor;
        public IntPtr hMon; // HMONITOR
        public int dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] public string szDevice;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)] public string szDescription;
        public long liDriverVersion;
        public int dwVendorId;
        public int dwDeviceId;
        public int dwSubSysId;
        public int dwRevision;
    }

    /// <summary>
    /// From VMRGUID
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VMRGuid
    {
        public IntPtr pGUID; // GUID *
        public Guid GUID;
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("CE704FE7-E71E-41fb-BAA2-C4403E1182F5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImagePresenter
    {
        [PreserveSig]
        int StartPresenting([In] IntPtr dwUserID);

        [PreserveSig]
        int StopPresenting([In] IntPtr dwUserID);

        [PreserveSig]
        int PresentImage(
            [In] IntPtr dwUserID,
            [In] ref VMRPresentationInfo lpPresInfo
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("31ce832e-4484-458b-8cca-f4d7e3db0b52"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurfaceAllocator
    {
        [PreserveSig]
        int AllocateSurface(
            [In] IntPtr dwUserID,
            [In] ref VMRAllocationInfo lpAllocInfo,
            [Out] out int lpdwActualBuffers,
            [In, Out] ref IntPtr lplpSurface // LPDIRECTDRAWSURFACE7
            );

        [PreserveSig]
        int FreeSurface([In] IntPtr dwID);

        [PreserveSig]
        int PrepareSurface(
            [In] IntPtr dwUserID,
            [In] IntPtr lplpSurface, // LPDIRECTDRAWSURFACE7
            [In] int dwSurfaceFlags
            );

        [PreserveSig]
        int AdviseNotify([In] IVMRSurfaceAllocatorNotify lpIVMRSurfAllocNotify);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("aada05a8-5a4e-4729-af0b-cea27aed51e2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurfaceAllocatorNotify
    {
        [PreserveSig]
        int AdviseSurfaceAllocator(
            [In] IntPtr dwUserID,
            [In] IVMRSurfaceAllocator lpIVRMSurfaceAllocator
            );

        [PreserveSig]
        int SetDDrawDevice(
            [In] IntPtr lpDDrawDevice, // LPDIRECTDRAW7
            [In] IntPtr hMonitor // HMONITOR
            );

        [PreserveSig]
        int ChangeDDrawDevice(
            [In] IntPtr lpDDrawDevice, // LPDIRECTDRAW7
            [In] IntPtr hMonitor // HMONITOR
            );

        [PreserveSig]
        int RestoreDDrawSurfaces();

        [PreserveSig]
        int NotifyEvent(
            [In] int EventCode,
            [In] IntPtr Param1,
            [In] IntPtr Param2
            );

        [PreserveSig]
        int SetBorderColor([In] int clrBorder);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("a9849bbe-9ec8-4263-b764-62730f0d15d0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRSurface
    {
        [PreserveSig]
        int IsSurfaceLocked();

        [PreserveSig]
        int LockSurface([Out] out IntPtr lpSurface); // BYTE**

        [PreserveSig]
        int UnlockSurface();

        [PreserveSig]
        int GetSurface([Out, MarshalAs(UnmanagedType.Interface)] out object lplpSurface);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("e6f7ce40-4673-44f1-8f77-5499d68cb4ea"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImagePresenterExclModeConfig : IVMRImagePresenterConfig
    {
        #region IVMRImagePresenterConfig Methods

        [PreserveSig]
        new int SetRenderingPrefs([In] VMRRenderPrefs dwRenderFlags);

        [PreserveSig]
        new int GetRenderingPrefs([Out] out VMRRenderPrefs dwRenderFlags);

        #endregion

        [PreserveSig]
        int SetXlcModeDDObjAndPrimarySurface(
            [In] IntPtr lpDDObj,
            [In] IntPtr lpPrimarySurf
            );

        [PreserveSig]
        int GetXlcModeDDObjAndPrimarySurface(
            [Out] out IntPtr lpDDObj,
            [Out] out IntPtr lpPrimarySurf
            );
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9e5530c5-7034-48b4-bb46-0b8a6efc8e36"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRFilterConfig
    {
        [PreserveSig]
        int SetImageCompositor([In] IVMRImageCompositor lpVMRImgCompositor);

        [PreserveSig]
        int SetNumberOfStreams([In] int dwMaxStreams);

        [PreserveSig]
        int GetNumberOfStreams([Out] out int pdwMaxStreams);

        [PreserveSig]
        int SetRenderingPrefs([In] VMRRenderPrefs dwRenderFlags);

        [PreserveSig]
        int GetRenderingPrefs([Out] out VMRRenderPrefs pdwRenderFlags);

        [PreserveSig]
        int SetRenderingMode([In] VMRMode Mode);

        [PreserveSig]
        int GetRenderingMode([Out] out VMRMode Mode);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0eb1088c-4dcd-46f0-878f-39dae86a51b7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRWindowlessControl
    {
        [PreserveSig]
        int GetNativeVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight,
            [Out] out int lpARWidth,
            [Out] out int lpARHeight
            );

        [PreserveSig]
        int GetMinIdealVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight
            );

        [PreserveSig]
        int GetMaxIdealVideoSize(
            [Out] out int lpWidth,
            [Out] out int lpHeight
            );

        [PreserveSig]
        int SetVideoPosition(
            [In] DsRect lpSRCRect,
            [In] DsRect lpDSTRect
            );

        [PreserveSig]
        int GetVideoPosition(
            [Out] DsRect lpSRCRect,
            [Out] DsRect lpDSTRect
            );

        [PreserveSig]
        int GetAspectRatioMode([Out] out VMRAspectRatioMode lpAspectRatioMode);

        [PreserveSig]
        int SetAspectRatioMode([In] VMRAspectRatioMode AspectRatioMode);

        [PreserveSig]
        int SetVideoClippingWindow([In] IntPtr hwnd); // HWND

        [PreserveSig]
        int RepaintVideo(
            [In] IntPtr hwnd, // HWND
            [In] IntPtr hdc // HDC
            );

        [PreserveSig]
        int DisplayModeChanged();

        /// <summary>
        /// the caller is responsible for free the returned memory by calling CoTaskMemFree.
        /// </summary>
        [PreserveSig]
        int GetCurrentImage([Out] out IntPtr lpDib); // BYTE**

        [PreserveSig]
        int SetBorderColor([In] int Clr);

        [PreserveSig]
        int GetBorderColor([Out] out int lpClr);

        [PreserveSig]
        int SetColorKey([In] int Clr);

        [PreserveSig]
        int GetColorKey([Out] out int lpClr);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ede80b5c-bad6-4623-b537-65586c9f8dfd"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRAspectRatioControl
    {
        [PreserveSig]
        int GetAspectRatioMode([Out] out VMRAspectRatioMode lpdwARMode);

        [PreserveSig]
        int SetAspectRatioMode([In] VMRAspectRatioMode lpdwARMode);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("bb057577-0db8-4e6a-87a7-1a8c9a505a0f"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRDeinterlaceControl
    {
        [PreserveSig]
        int GetNumberOfDeinterlaceModes(
            [In] ref VMRVideoDesc lpVideoDescription,
            [In, Out] ref int lpdwNumDeinterlaceModes,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.Struct)] Guid[] lpDeinterlaceModes
            );

        [PreserveSig]
        int GetDeinterlaceModeCaps(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpDeinterlaceMode,
            [In] ref VMRVideoDesc lpVideoDescription,
            [In, Out] ref VMRDeinterlaceCaps lpDeinterlaceCaps
            );

        [PreserveSig]
        int GetDeinterlaceMode(
            [In] int dwStreamID,
            [Out] out Guid lpDeinterlaceMode
            );

        [PreserveSig]
        int SetDeinterlaceMode(
            [In] int dwStreamID,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid lpDeinterlaceMode
            );

        [PreserveSig]
        int GetDeinterlacePrefs([Out] out VMRDeinterlacePrefs lpdwDeinterlacePrefs);

        [PreserveSig]
        int SetDeinterlacePrefs([In] VMRDeinterlacePrefs lpdwDeinterlacePrefs);

        [PreserveSig]
        int GetActualDeinterlaceMode(
            [In] int dwStreamID,
            [Out] out Guid lpDeinterlaceMode
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("7a4fb5af-479f-4074-bb40-ce6722e43c82"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImageCompositor
    {
        [PreserveSig]
        int InitCompositionTarget(
            [In] IntPtr pD3DDevice,
            [In] IntPtr pddsRenderTarget
            );

        [PreserveSig]
        int TermCompositionTarget(
            [In] IntPtr pD3DDevice,
            [In] IntPtr pddsRenderTarget
            );

        [PreserveSig]
        int SetStreamMediaType(
            [In] int dwStrmID,
            [In] AMMediaType pmt,
            [In, MarshalAs(UnmanagedType.Bool)] bool fTexture
            );

        [PreserveSig]
        int CompositeImage(
            [In] IntPtr pD3DDevice,
            [In] IntPtr pddsRenderTarget,
            [In] AMMediaType pmtRenderTarget,
            [In] long rtStart,
            [In] long rtEnd,
            [In] int dwClrBkGnd,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.Struct, SizeParamIndex=7)] VMRVideoStreamInfo[] pVideoStreamInfo,
            [In] int cStreams
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9f3a1c85-8555-49ba-935f-be5b5b29d178"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRImagePresenterConfig
    {
        [PreserveSig]
        int SetRenderingPrefs([In] VMRRenderPrefs dwRenderFlags);

        [PreserveSig]
        int GetRenderingPrefs([Out] out VMRRenderPrefs dwRenderFlags);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1E673275-0257-40aa-AF20-7C608D4A0428"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMixerBitmap
    {
        [PreserveSig]
        int SetAlphaBitmap([In] ref VMRAlphaBitmap pBmpParms);

        [PreserveSig]
        int UpdateAlphaBitmapParameters([In] ref VMRAlphaBitmap pBmpParms);

        [PreserveSig]
        int GetAlphaBitmapParameters([Out] out VMRAlphaBitmap pBmpParms);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9cf0b1b6-fbaa-4b7f-88cf-cf1f130a0dce"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMonitorConfig
    {
        [PreserveSig]
        int SetMonitor([In] ref VMRGuid pGUID);

        [PreserveSig]
        int GetMonitor([Out] out VMRGuid pGUID);

        [PreserveSig]
        int SetDefaultMonitor([In] ref VMRGuid pGUID);

        [PreserveSig]
        int GetDefaultMonitor([Out] out VMRGuid pGUID);

        [PreserveSig]
        int GetAvailableMonitors(
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.Struct)] VMRMonitorInfo[] pInfo,
            [In] int dwMaxInfoArraySize,
            [Out] out int pdwNumDevices
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("058d1f11-2a54-4bef-bd54-df706626b727"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRVideoStreamControl
    {
        [PreserveSig]
        int SetColorKey([In] ref DDColorKey lpClrKey);

        [PreserveSig]
        int GetColorKey([Out] out DDColorKey lpClrKey);

        [PreserveSig]
        int SetStreamActiveState([In, MarshalAs(UnmanagedType.Bool)] bool fActive);

        [PreserveSig]
        int GetStreamActiveState([Out, MarshalAs(UnmanagedType.Bool)] out bool fActive);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1c1a17b0-bed0-415d-974b-dc6696131599"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVMRMixerControl
    {
        [PreserveSig]
        int SetAlpha(
            [In] int dwStreamID,
            [In] float Alpha
            );

        [PreserveSig]
        int GetAlpha(
            [In] int dwStreamID,
            [Out] out float Alpha
            );

        [PreserveSig]
        int SetZOrder(
            [In] int dwStreamID,
            [In] int dwZ
            );

        [PreserveSig]
        int GetZOrder(
            [In] int dwStreamID,
            [Out] out int dwZ
            );

        [PreserveSig]
        int SetOutputRect(
            [In] int dwStreamID,
            [In] ref NormalizedRect pRect
            );

        [PreserveSig]
        int GetOutputRect(
            [In] int dwStreamID,
            [Out] out NormalizedRect pRect
            );

        [PreserveSig]
        int SetBackgroundClr([In] int ClrBkg);

        [PreserveSig]
        int GetBackgroundClr([Out] out int ClrBkg);

        [PreserveSig]
        int SetMixingPrefs([In] VMRMixerPrefs dwMixerPrefs);

        [PreserveSig]
        int GetMixingPrefs([Out] out VMRMixerPrefs dwMixerPrefs);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("aac18c18-e186-46d2-825d-a1f8dc8e395a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVPManager
    {
        [PreserveSig]
        int SetVideoPortIndex([In] int dwVideoPortIndex);

        [PreserveSig]
        int GetVideoPortIndex([Out] out int dwVideoPortIndex);
    }

    #endregion
}
