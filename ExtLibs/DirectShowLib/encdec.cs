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

namespace DirectShowLib.BDA
{
    #region COM Class Objects

    /// <summary>
    /// CLSID_ETFilterEncProperties
    /// </summary>
    [ComImport, Guid("C4C4C481-0049-4E2B-98FB-9537F6CE516D")]
    public class ETFilterEncProperties
    {
    }

    /// <summary>
    /// CLSID_ETFilterTagProperties
    /// </summary>
    [ComImport, Guid("C4C4C491-0049-4E2B-98FB-9537F6CE516D")]
    public class ETFilterTagProperties
    {
    }

    /// <summary>
    /// CLSID_DTFilterEncProperties
    /// </summary>
    [ComImport, Guid("C4C4C482-0049-4E2B-98FB-9537F6CE516D")]
    public class DTFilterEncProperties
    {
    }

    /// <summary>
    /// CLSID_DTFilterTagProperties
    /// </summary>
    [ComImport, Guid("C4C4C492-0049-4E2B-98FB-9537F6CE516D")]
    public class DTFilterTagProperties
    {
    }

    /// <summary>
    /// CLSID_XDSCodecProperties
    /// </summary>
    [ComImport, Guid("C4C4C483-0049-4E2B-98FB-9537F6CE516D")]
    public class XDSCodecProperties
    {
    }

    /// <summary>
    /// CLSID_XDSCodecTagProperties
    /// </summary>
    [ComImport, Guid("C4C4C493-0049-4E2B-98FB-9537F6CE516D")]
    public class XDSCodecTagProperties
    {
    }

    /// <summary>
    /// CLSID_CXDSData
    /// </summary>
    [ComImport, Guid("C4C4C4F4-0049-4E2B-98FB-9537F6CE516D")]
    public class CXDSData
    {
    }

    /// <summary>
    /// CLSID_XDSCodec
    /// </summary>
    [ComImport, Guid("C4C4C4F3-0049-4E2B-98FB-9537F6CE516D")]
    public class XDSCodec
    {
    }

    #endregion

    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From FormatNotSupportedEvents
    /// </summary>
    public enum FormatNotSupportedEvents
    {
        Clear = 0,
        NotSupported = 1
    }

    /// <summary>
    /// From WMDRMProtectionInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WMDRMProtectionInfo
    {
        [MarshalAs(UnmanagedType.LPWStr, SizeConst = 25)] string   wszKID;
        public long qwCounter;
        public long qwIndex;
        public byte bOffset;
    }

    /// <summary>
    /// From BadSampleInfo
    /// </summary>
    public class BadSampleInfo
    {
        public int hrReason;
    } 

    /// <summary>
    /// From COPPEventBlockReason
    /// </summary>
    public enum COPPEventBlockReason
    {
        Unknown = -1,
        BadDriver = 0,
        NoCardHDCPSupport = 1,
        NoMonitorHDCPSupport = 2,
        BadCertificate = 3,
        InvalidBusProtection = 4,
        AeroGlassOff = 5,
        RogueApp = 6,
        ForbiddenVideo = 7,
        Activate = 8,
        DigitalAudioUnprotected = 9
    }

    /// <summary>
    /// From LicenseEventBlockReason
    /// </summary>
    public enum LicenseEventBlockReason
    {
        BadLicense = 0,
        NeedIndiv = 1,
        Expired = 2,
        NeedActivation = 3,
        ExtenderBlocked = 4
    }

    /// <summary>
    /// From CPEventBitShift
    /// </summary>
    public enum CPEventBitShift
    {
        Ratings = 0,
        COPP,
        License,
        Rollback,
        SAC,
        DownRes,
        StubLib,
        UntrustedGraph,
        PendingCertificate,
        NoPlayReady
    }

    /// <summary>
    /// From CPEvents
    /// </summary>
    public enum CPEvents
    {
        None = 0,
        Ratings,
        COPP,
        License,
        Rollback,
        SAC,
        DownRes,
        StubLib,
        UntrustedGraph,
        ProtectWindowed
    }

    /// <summary>
    /// From EncDecEvents
    /// </summary>
    public enum EncDecEvents
    {
        CPEvent = 0,
        RecordingStatus
    }

    /// <summary>
    /// From CPRecordingStatus
    /// </summary>
    public enum CPRecordingStatus
    {
        Stopped = 0,
        Started = 1
    }

/// <summary>
/// From RevokedComponent
/// </summary>
    public enum RevokedComponent
    {
        COPP = 0,
        SAC,
        APPStub,
        SecurePipeline,
        MaxTypes
    }

    /// <summary>
    /// From EnTag_Mode
    /// </summary>
    public enum EnTag_Mode
    {
        Remove = 0x0,
        Once = 0x1,
        Repeat = 0x2
    }

    /// <summary>
    /// From DownResEventParam
    /// </summary>
    public enum DownResEventParam
    {
        Always = 0,
        InWindowOnly = 1,
        Undefined = 2
    }

#endif

    /// <summary>
    /// From ProtType
    /// </summary>
    public enum ProtType
    {
        None = 0,
        Free = 1,
        Once = 2,
        Never = 3,
        NeverReally = 4,
        NoMore = 5,
        FreeCit = 6,
        BF = 7,
        CnRecordingStop = 8,
        FreeSecure = 9,
        Invalid = 50
    }

    static public class EventID
    {
        /// <summary> EVENTID_XDSCodecNewXDSRating </summary>
        public static readonly Guid XDSCodecNewXDSRating = new Guid(0xC4C4C4E0, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_XDSCodecDuplicateXDSRating </summary>
        public static readonly Guid XDSCodecDuplicateXDSRating = new Guid(0xC4C4C4DF, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_XDSCodecNewXDSPacket </summary>
        public static readonly Guid XDSCodecNewXDSPacket = new Guid(0xC4C4C4E1, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterRatingChange </summary>
        public static readonly Guid DTFilterRatingChange = new Guid(0xC4C4C4E2, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterRatingsBlock </summary>
        public static readonly Guid DTFilterRatingsBlock = new Guid(0xC4C4C4E3, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterRatingsUnblock </summary>
        public static readonly Guid DTFilterRatingsUnblock = new Guid(0xC4C4C4E4, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterXDSPacket </summary>
        public static readonly Guid DTFilterXDSPacket = new Guid(0xC4C4C4E5, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETFilterEncryptionOn </summary>
        public static readonly Guid ETFilterEncryptionOn = new Guid(0xC4C4C4E6, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETFilterEncryptionOff </summary>
        public static readonly Guid ETFilterEncryptionOff = new Guid(0xC4C4C4E7, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterCOPPUnblock </summary>
        public static readonly Guid DTFilterCOPPUnblock = new Guid(0xC4C4C4E8, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_EncDecFilterError </summary>
        public static readonly Guid EncDecFilterError = new Guid(0xC4C4C4E9, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterCOPPBlock </summary>
        public static readonly Guid DTFilterCOPPBlock = new Guid(0xC4C4C4EA, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETFilterCopyOnce </summary>
        public static readonly Guid ETFilterCopyOnce = new Guid(0xC4C4C4EB, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETFilterCopyNever </summary>
        public static readonly Guid ETFilterCopyNever = new Guid(0xC4C4C4F0, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterDataFormatOK </summary>
        public static readonly Guid DTFilterDataFormatOK = new Guid(0xC4C4C4EC, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_DTFilterDataFormatFailure </summary>
        public static readonly Guid DTFilterDataFormatFailure = new Guid(0xC4C4C4ED, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETDTFilterLicenseOK </summary>
        public static readonly Guid ETDTFilterLicenseOK = new Guid(0xC4C4C4EE, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_ETDTFilterLicenseFailure </summary>
        public static readonly Guid ETDTFilterLicenseFailure = new Guid(0xC4C4C4EF, 0x0049, 0x4E2B, 0x98, 0xFB, 0x95, 0x37, 0xF6, 0xCE, 0x51, 0x6D);

        /// <summary> EVENTID_EncDecFilterEvent </summary>
        public static readonly Guid EncDecFilterEvent = new Guid(0x4a1b465b, 0xfb9, 0x4159, 0xaf, 0xbd, 0xe3, 0x30, 0x6, 0xa0, 0xf9, 0xf4);

        /// <summary> EVENTID_FormatNotSupportedEvent </summary>
        public static readonly Guid FormatNotSupportedEvent = new Guid(0x24b2280a, 0xb2aa, 0x4777, 0xbf, 0x65, 0x63, 0xf3, 0x5e, 0x7b, 0x2, 0x4a);

        /// <summary> EVENTID_DemultiplexerFilterDiscontinuity </summary>
        public static readonly Guid DemultiplexerFilterDiscontinuity = new Guid(0x16155770, 0xaed5, 0x475c, 0xbb, 0x98, 0x95, 0xa3, 0x30, 0x70, 0xdf, 0xc);
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FAF37694-909C-49cd-886F-C7382E5DB596"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTFilterBlockedOverlay
    {
        [PreserveSig]
        int SetOverlay(
            int dwOverlayCause
            );

        [PreserveSig]
        int ClearOverlay(
            int dwOverlayCause
            );

        [PreserveSig]
        int GetOverlay(
            out int pdwOverlayCause
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4C2-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDTFilterEvents
    {
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4C1-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IETFilterEvents
    {
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4C3-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IXDSCodecEvents
    {
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4B1-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IETFilter
    {
        [PreserveSig]
        int get_EvalRatObjOK(
            out int pHrCoCreateRetVal
            );

        [PreserveSig]
        int GetCurrRating(
            out EnTvRat_System pEnSystem,
            out EnTvRat_GenericLevel pEnRating,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        int GetCurrLicenseExpDate(
            ProtType protType,
            out int lpDateTime
            );

        [PreserveSig]
        int GetLastErrorCode();

        [PreserveSig]
        int SetRecordingOn(
            [MarshalAs(UnmanagedType.Bool)] bool fRecState
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4B3-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXDSCodec
    {
        [PreserveSig]
        int get_XDSToRatObjOK(
            out int pHrCoCreateRetVal
                );

        [PreserveSig]
        int put_CCSubstreamService(
            int SubstreamMask
            );

        [PreserveSig]
        int get_CCSubstreamService(
            out int pSubstreamMask
            );

        [PreserveSig]
        int GetContentAdvisoryRating(
            out int pRat,
            out int pPktSeqID,
            out int pCallSeqID,
            out long pTimeStart,
            out long pTimeEnd
            );

        [PreserveSig]
        int GetXDSPacket(
            out int pXDSClassPkt,
            out int pXDSTypePkt,
            [MarshalAs(UnmanagedType.BStr)] out string pBstrXDSPkt,
            out int pPktSeqID,
            out int pCallSeqID,
            out long pTimeStart,
            out long pTimeEnd
            );

        [PreserveSig]
        int GetCurrLicenseExpDate(
            ProtType protType,
            out int lpDateTime
            );

        [PreserveSig]
        int GetLastErrorCode();
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4D3-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IXDSCodecConfig
    {
        [PreserveSig]
        int GetSecureChannelObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkDRMSecureChannel
            );

        [PreserveSig]
        int SetPauseBufferTime(
            int dwPauseBufferTime
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4B2-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTFilter
    {
        [PreserveSig]
        int get_EvalRatObjOK(
            out int pHrCoCreateRetVal
            );

        [PreserveSig]
        int GetCurrRating(
            out EnTvRat_System pEnSystem,
            out EnTvRat_GenericLevel pEnRating,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        int get_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        int put_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            BfEnTvRat_GenericAttributes lbfAttrs
            );

        [PreserveSig]
        int get_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] out bool pfBlockUnRatedShows
            );

        [PreserveSig]
        int put_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] bool fBlockUnRatedShows
            );

        [PreserveSig]
        int get_BlockUnRatedDelay(
            out int pmsecsDelayBeforeBlock
            );

        [PreserveSig]
        int put_BlockUnRatedDelay(
            int msecsDelayBeforeBlock
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4B4-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTFilter2 : IDTFilter
    {

        #region IDTFilter methods

        [PreserveSig]
        new int get_EvalRatObjOK(
            out int pHrCoCreateRetVal
            );

        [PreserveSig]
        new int GetCurrRating(
            out EnTvRat_System pEnSystem,
            out EnTvRat_GenericLevel pEnRating,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        new int get_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        new int put_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            BfEnTvRat_GenericAttributes lbfAttrs
            );

        [PreserveSig]
        new int get_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] out bool pfBlockUnRatedShows
            );

        [PreserveSig]
        new int put_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] bool fBlockUnRatedShows
            );

        [PreserveSig]
        new int get_BlockUnRatedDelay(
            out int pmsecsDelayBeforeBlock
            );

        [PreserveSig]
        new int put_BlockUnRatedDelay(
            int msecsDelayBeforeBlock
            );

        #endregion

        [PreserveSig]
        int get_ChallengeUrl(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrChallengeUrl
            );

        [PreserveSig]
        int GetCurrLicenseExpDate(
            ProtType protType,
           out int lpDateTime
            );

        [PreserveSig]
        int GetLastErrorCode();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4D1-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IETFilterConfig
    {
        [PreserveSig]
        int InitLicense(
            int LicenseId
            );

        [PreserveSig]
        int GetSecureChannelObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkDRMSecureChannel
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C4C4C4D2-0049-4E2B-98FB-9537F6CE516D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTFilterConfig
    {
        [PreserveSig]
        int GetSecureChannelObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkDRMSecureChannel
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("513998cc-e929-4cdf-9fbd-bad1e0314866"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDTFilter3 : IDTFilter2
    {

        #region IDTFilter methods

        [PreserveSig]
        new int get_EvalRatObjOK(
            out int pHrCoCreateRetVal
            );

        [PreserveSig]
        new int GetCurrRating(
            out EnTvRat_System pEnSystem,
            out EnTvRat_GenericLevel pEnRating,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        new int get_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            out BfEnTvRat_GenericAttributes plbfEnAttr
            );

        [PreserveSig]
        new int put_BlockedRatingAttributes(
            EnTvRat_System enSystem,
            EnTvRat_GenericLevel enLevel,
            BfEnTvRat_GenericAttributes lbfAttrs
            );

        [PreserveSig]
        new int get_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] out bool pfBlockUnRatedShows
            );

        [PreserveSig]
        new int put_BlockUnRated(
            [MarshalAs(UnmanagedType.Bool)] bool fBlockUnRatedShows
            );

        [PreserveSig]
        new int get_BlockUnRatedDelay(
            out int pmsecsDelayBeforeBlock
            );

        [PreserveSig]
        new int put_BlockUnRatedDelay(
            int msecsDelayBeforeBlock
            );

        #endregion

        #region IDTFilter2 methods

        [PreserveSig]
        new int get_ChallengeUrl(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrChallengeUrl
            );

        [PreserveSig]
        new int GetCurrLicenseExpDate(
            ProtType protType,
            out int lpDateTime
            );

        [PreserveSig]
        new int GetLastErrorCode();

        #endregion

        [PreserveSig]
        int GetProtectionType(
            out ProtType pProtectionType
            );

        [PreserveSig]
        int LicenseHasExpirationDate(
            [MarshalAs(UnmanagedType.Bool)] out bool pfLicenseHasExpirationDate
            );

        [PreserveSig]
        int SetRights(
            [MarshalAs(UnmanagedType.BStr)] string bstrRights
            );
    }

    #endregion

}
