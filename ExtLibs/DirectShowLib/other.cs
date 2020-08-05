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
    /// From AM_WST_STYLE
    /// </summary>
    public enum WSTStyle
    {
        None = 0,
        Invers
    }

    /// <summary>
    /// From AMVABUFFERINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVABufferInfo
    {
        public int                   dwTypeIndex;
        public int                   dwBufferIndex;
        public int                   dwDataOffset;
        public int                   dwDataSize;
    }

    /// <summary>
    /// From AMVAUncompDataInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVAUncompDataInfo
    {
        public int                   dwUncompWidth;
        public int                   dwUncompHeight;
        public DDPixelFormat           ddUncompPixelFormat;
    }

    /// <summary>
    /// From AMVAUncompBufferInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVAUncompBufferInfo
    {
        public int                   dwMinNumSurfaces;
        public int                   dwMaxNumSurfaces;
        public DDPixelFormat           ddUncompPixelFormat;
    }

    /// <summary>
    /// From AMVAInternalMemInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVAInternalMemInfo
    {
        public int                   dwScratchMemAlloc;
    }

    /// <summary>
    /// From DDSCAPS2
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DDSCaps2
    {
        public int       dwCaps;
        public int       dwCaps2;
        public int       dwCaps3;
        public int       dwCaps4;
    }

    /// <summary>
    /// From AMVACompBufferInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVACompBufferInfo
    {
        public int                   dwNumCompBuffers;
        public int                   dwWidthToCreate;
        public int                   dwHeightToCreate;
        public int                   dwBytesToAllocate;
        public DDSCaps2                ddCompCaps;
        public DDPixelFormat           ddPixelFormat;
    }

    /// <summary>
    /// From AMVABeginFrameInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVABeginFrameInfo
    {
        public int                dwDestSurfaceIndex;
        public IntPtr               pInputData;  // LPVOID
        public int                dwSizeInputData;
        public IntPtr               pOutputData;  // LPVOID
        public int                dwSizeOutputData;
    }

    /// <summary>
    /// From AMVAEndFrameInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AMVAEndFrameInfo
    {
        public int                   dwSizeMiscData;
        public IntPtr                  pMiscData; // LPVOID
    }

#endif

    /// <summary>
    /// From AM_WST_LEVEL
    /// </summary>
    public enum WSTLevel
    {
        Level1_5 = 0
    }

    /// <summary>
    /// From AM_WST_SERVICE
    /// </summary>
    public enum WSTService
    {
        None = 0,
        Text,
        IDS,
        Invalid
    }

    /// <summary>
    /// From AM_WST_STATE
    /// </summary>
    public enum WSTState
    {
        Off = 0,
        On
    }

    /// <summary>
    /// From AM_WST_DRAWBGMODE
    /// </summary>
    public enum WSTDrawBGMode
    {
        Opaque,
        Transparent
    }

    /// <summary>
    /// Not from DirectShow
    /// </summary>
    public enum MPEGAudioDivider
    {
        CDAudio = 1,
        FMRadio = 2,
        AMRadio = 4
    }

    /// <summary>
    /// From AM_MPEG_AUDIO_DUAL_* defines
    /// </summary>
    public enum MPEGAudioDual
    {
        Merge,
        Left,
        Right
    }

    /// <summary>
    /// Not from DirectShow
    /// </summary>
    public enum MPEGAudioAccuracy
    {
        Best = 0x0000,
        High = 0x4000,
        Full = 0x8000
    }

    /// <summary>
    /// Not from DirectShow
    /// </summary>
    public enum MPEGAudioChannels
    {
        Mono = 1,
        Stereo = 2
    }

    /// <summary>
    /// From AM_WST_PAGE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WSTPage
    {
        public int dwPageNr ;
        public int dwSubPageNr ;
        public IntPtr pucPageData; // BYTE *
    }

    /// <summary>
    /// From ACM_MPEG_LAYER* defines
    /// </summary>
    [Flags]
    public enum AcmMpegHeadLayer : short
    {
        Layer1 = 1,
        Layer2 = 2,
        Layer3 = 4
    }

    /// <summary>
    /// From ACM_MPEG_* defines
    /// </summary>
    [Flags]
    public enum AcmMpegHeadMode : short
    {
        Stereo = 1,
        JointStereo = 2,
        DualChannel = 4,
        SingleChannel = 8
    }

    /// <summary>
    /// From ACM_MPEG_* defines
    /// </summary>
    [Flags]
    public enum AcmMpegHeadFlags : short
    {
        None = 0x0,
        PrivateBit = 0x1,
        Copyright = 0x2,
        OriginalHome = 0x4,
        ProtectionBit = 0x8,
        IDMpeg1 = 0x10
    }

    /// <summary>
    /// From SPEAKER_* defines
    /// </summary>
    [Flags]
    public enum WaveMask
    {
        None = 0x0,
        FrontLeft = 0x1,
        FrontRight = 0x2,
        FrontCenter = 0x4,
        LowFrequency = 0x8,
        BackLeft = 0x10,
        BackRight = 0x20,
        FrontLeftOfCenter = 0x40,
        FrontRightOfCenter = 0x80,
        BackCenter = 0x100,
        SideLeft = 0x200,
        SideRight = 0x400,
        TopCenter = 0x800,
        TopFrontLeft = 0x1000,
        TopFrontCenter = 0x2000,
        TopFrontRight = 0x4000,
        TopBackLeft = 0x8000,
        TopBackCenter = 0x10000,
        TopBackRight = 0x20000
    }

    /// <summary>
    /// From MPEG1WAVEFORMAT
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=2)]
    public class MPEG1WaveFormat
    {
        public WaveFormatEx wfx;
        public AcmMpegHeadLayer fwHeadLayer;
        public int dwHeadBitrate;
        public AcmMpegHeadMode fwHeadMode;
        public short fwHeadModeExt;
        public short wHeadEmphasis;
        public AcmMpegHeadFlags fwHeadFlags;
        public int dwPTSLow;
        public int dwPTSHigh;
    }

    /// <summary>
    /// From WAVEFORMATEXTENSIBLE
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public class WaveFormatExtensible : WaveFormatEx
    {
        [FieldOffset(0)]
        public short wValidBitsPerSample;
        [FieldOffset(0)]
        public short wSamplesPerBlock;
        [FieldOffset(0)]
        public short wReserved;
        [FieldOffset(2)]
        public WaveMask dwChannelMask;
        [FieldOffset(6)]
        public Guid SubFormat;
    }

    /// <summary>
    /// From _AM_ASFWRITERCONFIG_PARAM
    /// </summary>
    public enum ASFWriterConfig
    {
        None = 0,
        AutoIndex = 1,
        MultiPass = 2,
        DontCompress = 3
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("c47a3420-005c-11d2-9038-00a0c9697298"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMParse
    {
        [PreserveSig]
        int GetParseTime(out long prtCurrent);

        [PreserveSig]
        int SetParseTime(long rtCurrent);

        [PreserveSig]
        int Flush();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("256A6A21-FBAD-11d1-82BF-00A0C9696C8F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVideoAcceleratorNotify
    {
        [PreserveSig]
        int GetUncompSurfacesInfo([In, MarshalAs(UnmanagedType.LPStruct)] Guid pGuid,
            [Out] out AMVAUncompBufferInfo pUncompBufferInfo);

        [PreserveSig]
        int SetUncompSurfacesInfo([In] int dwActualUncompSurfacesAllocated);

        [PreserveSig]
        int GetCreateVideoAcceleratorData([In, MarshalAs(UnmanagedType.LPStruct)] Guid pGuid,
            [Out] out int pdwSizeMiscData,
            [Out] IntPtr ppMiscData); // LPVOID
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("256A6A22-FBAD-11d1-82BF-00A0C9696C8F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVideoAccelerator
    {
        [PreserveSig]
        int GetVideoAcceleratorGUIDs([Out] out int pdwNumGuidsSupported,
            [In, Out] Guid [] pGuidsSupported);

        [PreserveSig]
        int GetUncompFormatsSupported( [In, MarshalAs(UnmanagedType.LPStruct)] Guid pGuid,
            [Out] out int pdwNumFormatsSupported,
            [Out] out DDPixelFormat pFormatsSupported);

        [PreserveSig]
        int GetInternalMemInfo([In, MarshalAs(UnmanagedType.LPStruct)] Guid pGuid,
            [In] AMVAUncompDataInfo pamvaUncompDataInfo,
            [Out] out AMVAInternalMemInfo pamvaInternalMemInfo);

        [PreserveSig]
        int GetCompBufferInfo([In, MarshalAs(UnmanagedType.LPStruct)] Guid pGuid,
            [In] AMVAUncompDataInfo pamvaUncompDataInfo,
            [In, Out] int pdwNumTypesCompBuffers,
            [Out] out AMVACompBufferInfo pamvaCompBufferInfo);

        [PreserveSig]
        int GetInternalCompBufferInfo([Out] out int pdwNumTypesCompBuffers,
            [Out] out AMVACompBufferInfo pamvaCompBufferInfo);

        [PreserveSig]
        int BeginFrame([In] AMVABeginFrameInfo amvaBeginFrameInfo);

        [PreserveSig]
        int EndFrame([In] AMVAEndFrameInfo pEndFrameInfo);

        [PreserveSig]
        int GetBuffer(
            [In] int dwTypeIndex,
            [In] int dwBufferIndex,
            [In, MarshalAs(UnmanagedType.Bool)] bool bReadOnly,
            [Out] IntPtr ppBuffer, // LPVOID
            [Out] out int lpStride);

        [PreserveSig]
        int ReleaseBuffer([In] int dwTypeIndex,
            [In] int dwBufferIndex);

        [PreserveSig]
        int Execute(
            [In] int dwFunction,
            [In] IntPtr lpPrivateInputData, // LPVOID
            [In] int cbPrivateInputData,
            [In] IntPtr lpPrivateOutputDat, // LPVOID
            [In] int cbPrivateOutputData,
            [In] int dwNumBuffers,
            [In] AMVABufferInfo pamvaBufferInfo);

        [PreserveSig]
        int QueryRenderStatus([In] int dwTypeIndex,
            [In] int dwBufferIndex,
            [In] int dwFlags);

        [PreserveSig]
        int DisplayFrame([In] int dwFlipToIndex,
            [In] IMediaSample pMediaSample);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("56a868fd-0ad4-11ce-b0a3-0020af0ba770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMFilterGraphCallback
    {
        [PreserveSig]
        int UnableToRender(IPin pPin);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("AB6B4AFE-F6E4-11d0-900D-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectDrawMediaSample
    {
        [PreserveSig]
        int GetSurfaceAndReleaseLock(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppDirectDrawSurface, // IDirectDrawSurface
            out Rectangle pRect);

        [PreserveSig]
        int LockMediaSamplePointer();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("AB6B4AFC-F6E4-11d0-900D-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirectDrawMediaSampleAllocator
    {
        [PreserveSig]
        int GetDirectDraw(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppDirectDraw); // IDirectDraw
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("45086030-F7E4-486a-B504-826BB5792A3B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IConfigAsfWriter
    {
        [PreserveSig,
        Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        int ConfigureFilterUsingProfileId([In] int dwProfileId);

        [PreserveSig,
        Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        int GetCurrentProfileId([Out] out int pdwProfileId);

        [PreserveSig,
        Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See ConfigureFilterUsingProfile", false)]
        int ConfigureFilterUsingProfileGuid([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile);

        [PreserveSig,
        Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See GetCurrentProfile", false)]
        int GetCurrentProfileGuid([Out] out Guid pProfileGuid);

        [PreserveSig,
        Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        int ConfigureFilterUsingProfile([In] IntPtr pProfile);

        [PreserveSig,
        Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        int GetCurrentProfile([Out] out IntPtr ppProfile);

        [PreserveSig]
        int SetIndexMode([In, MarshalAs(UnmanagedType.Bool)] bool bIndexFile);

        [PreserveSig]
        int GetIndexMode([Out, MarshalAs(UnmanagedType.Bool)] out bool pbIndexFile);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("546F4260-D53E-11cf-B3F0-00AA003761C5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMDirectSound
    {
        [PreserveSig]
        int GetDirectSoundInterface([MarshalAs(UnmanagedType.IUnknown)] out object lplpds); // IDirectSound

        [PreserveSig]
        int GetPrimaryBufferInterface([MarshalAs(UnmanagedType.IUnknown)] out object lplpdsb); // IDirectSoundBuffer

        [PreserveSig]
        int GetSecondaryBufferInterface([MarshalAs(UnmanagedType.IUnknown)] out object lplpdsb); // IDirectSoundBuffer

        [PreserveSig]
        int ReleaseDirectSoundInterface([MarshalAs(UnmanagedType.IUnknown)] object lpds); // IDirectSound

        [PreserveSig]
        int ReleasePrimaryBufferInterface([MarshalAs(UnmanagedType.IUnknown)] object lpdsb); // IDirectSoundBuffer

        [PreserveSig]
        int ReleaseSecondaryBufferInterface([MarshalAs(UnmanagedType.IUnknown)] object lpdsb); // IDirectSoundBuffer

        [PreserveSig]
        int SetFocusWindow(IntPtr hWnd, [In, MarshalAs(UnmanagedType.Bool)] bool bSet);

        [PreserveSig]
        int GetFocusWindow(out IntPtr hWnd, [Out, MarshalAs(UnmanagedType.Bool)] out bool bSet);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C056DE21-75C2-11d3-A184-00105AEF9F33"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMWstDecoder
    {
        [PreserveSig]
        int GetDecoderLevel(out WSTLevel lpLevel);

        [PreserveSig]
        int GetCurrentService(out WSTService lpService);

        [PreserveSig]
        int GetServiceState(out WSTState lpState);

        [PreserveSig]
        int SetServiceState(WSTState State);

        [PreserveSig]
        int GetOutputFormat([MarshalAs(UnmanagedType.LPStruct)] BitmapInfoHeader lpbmih);

        [PreserveSig]
        int SetOutputFormat(BitmapInfoHeader lpbmi);

        [PreserveSig]
        int GetBackgroundColor(out int pdwPhysColor);

        [PreserveSig]
        int SetBackgroundColor(int dwPhysColor);

        [PreserveSig]
        int GetRedrawAlways([MarshalAs(UnmanagedType.Bool)] out bool lpbOption);

        [PreserveSig]
        int SetRedrawAlways([MarshalAs(UnmanagedType.Bool)] bool bOption);

        [PreserveSig]
        int GetDrawBackgroundMode(out WSTDrawBGMode lpMode);

        [PreserveSig]
        int SetDrawBackgroundMode(WSTDrawBGMode Mode);

        [PreserveSig]
        int SetAnswerMode([MarshalAs(UnmanagedType.Bool)] bool bAnswer);

        [PreserveSig]
        int GetAnswerMode([MarshalAs(UnmanagedType.Bool)] out bool pbAnswer);

        [PreserveSig]
        int SetHoldPage([MarshalAs(UnmanagedType.Bool)] bool bHoldPage);

        [PreserveSig]
        int GetHoldPage([MarshalAs(UnmanagedType.Bool)] out bool pbHoldPage);

        [PreserveSig]
        int GetCurrentPage([In, Out] WSTPage pWstPage);

        [PreserveSig]
        int SetCurrentPage([In] WSTPage WstPage);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("b45dd570-3c77-11d1-abe1-00a0c905f375"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMpegAudioDecoder
    {
        [PreserveSig]
        int get_FrequencyDivider(
            out MPEGAudioDivider pDivider
            );

        [PreserveSig]
        int put_FrequencyDivider(
            MPEGAudioDivider Divider
            );

        [PreserveSig]
        int get_DecoderAccuracy(
            out MPEGAudioAccuracy pAccuracy
            );

        [PreserveSig]
        int put_DecoderAccuracy(
            MPEGAudioAccuracy Accuracy
            );

        [PreserveSig]
        int get_Stereo(
            out MPEGAudioChannels pStereo
            );

        [PreserveSig]
        int put_Stereo(
            MPEGAudioChannels Stereo
            );

        [PreserveSig]
        int get_DecoderWordSize(
            out int pWordSize
            );

        [PreserveSig]
        int put_DecoderWordSize(
            int WordSize
            );

        [PreserveSig]
        int get_IntegerDecode(
            out int pIntDecode
            );

        [PreserveSig]
        int put_IntegerDecode(
            int IntDecode
            );

        [PreserveSig]
        int get_DualMode(
            out MPEGAudioDual pIntDecode
            );

        [PreserveSig]
        int put_DualMode(
            MPEGAudioDual IntDecode
            );

        [PreserveSig]
        int get_AudioFormat(
            out MPEG1WaveFormat lpFmt
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6d5140c1-7436-11ce-8034-00aa006009fa"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        [PreserveSig]
        int QueryService(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid guidService,
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectWithSite
    {
        [PreserveSig]
        int SetSite(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkSite
            );

        [PreserveSig]
        int GetSite(
            [In, MarshalAs(UnmanagedType.LPStruct)] DsGuid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvSite
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("7989CCAA-53F0-44f0-884A-F3B03F6AE066"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IConfigAsfWriter2 : IConfigAsfWriter
    {
        #region IConfigAsfWriter Methods

        [PreserveSig,
        Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        new int ConfigureFilterUsingProfileId([In] int dwProfileId);

        [PreserveSig,
        Obsolete("This method is now obsolete because it assumes version 4.0 Windows Media Format SDK profiles. Use GetCurrentProfile or GetCurrentProfileGuid instead to correctly identify a profile.", false)]
        new int GetCurrentProfileId([Out] out int pdwProfileId);

        [PreserveSig,
        Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See ConfigureFilterUsingProfile", false)]
        new int ConfigureFilterUsingProfileGuid([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile);

        [PreserveSig,
        Obsolete("Using Guids is considered obsolete by MS.  The preferred approach is using an IWMProfile.  See GetCurrentProfile", false)]
        new int GetCurrentProfileGuid([Out] out Guid pProfileGuid);

        [PreserveSig,
        Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        new int ConfigureFilterUsingProfile([In] IntPtr pProfile);

        [PreserveSig,
        Obsolete("This method requires IWMProfile, which in turn requires several other interfaces.  Rather than duplicate all those interfaces here, it is recommended that you use the WindowsMediaLib from http://DirectShowNet.SourceForge.net", false)]
        new int GetCurrentProfile([Out] out IntPtr ppProfile);

        [PreserveSig]
        new int SetIndexMode([In, MarshalAs(UnmanagedType.Bool)] bool bIndexFile);

        [PreserveSig]
        new int GetIndexMode([Out, MarshalAs(UnmanagedType.Bool)] out bool pbIndexFile);

        #endregion

        [PreserveSig]
        int StreamNumFromPin(
            IPin pPin,
            out short pwStreamNum);

        [PreserveSig]
        int SetParam(
            ASFWriterConfig dwParam,
            int dwParam1,
            int dwParam2);

        [PreserveSig]
        int GetParam(
            ASFWriterConfig dwParam,
            out int pdwParam1,
            IntPtr pdwParam2);

        [PreserveSig]
        int ResetMultiPassState();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("a8809222-07bb-48ea-951c-33158100625b"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGetCapabilitiesKey
    {
        [PreserveSig]
        int GetCapabilitiesKey([Out] out IntPtr pHKey); // HKEY
    }

    #endregion
}
