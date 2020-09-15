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

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// From AMExtendedSeekingCapabilities
    /// </summary>
    [Flags]
    public enum AMExtendedSeekingCapabilities
    {
        None = 0,
        CanSeek = 1,
        CanScan = 2,
        MarkerSeek = 4,
        ScanWithoutClock = 8,
        NoStandardRepaint = 16,
        Buffering = 32,
        SendsVideoFrameReady = 64
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F1-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMNetShowConfig
    {
        [PreserveSig]
        int get_BufferingTime(out double pBufferingTime);

        [PreserveSig]
        int put_BufferingTime(double BufferingTime);

        [PreserveSig]
        int get_UseFixedUDPPort([MarshalAs(UnmanagedType.VariantBool)] out bool pUseFixedUDPPort);

        [PreserveSig]
        int put_UseFixedUDPPort([MarshalAs(UnmanagedType.VariantBool)] bool UseFixedUDPPort);

        [PreserveSig]
        int get_FixedUDPPort(out int pFixedUDPPort);

        [PreserveSig]
        int put_FixedUDPPort(int FixedUDPPort);

        [PreserveSig]
        int get_UseHTTPProxy([MarshalAs(UnmanagedType.VariantBool)] out bool pUseHTTPProxy);

        [PreserveSig]
        int put_UseHTTPProxy([MarshalAs(UnmanagedType.VariantBool)] bool UseHTTPProxy);

        [PreserveSig]
        int get_EnableAutoProxy([MarshalAs(UnmanagedType.VariantBool)] out bool pEnableAutoProxy);

        [PreserveSig]
        int put_EnableAutoProxy([MarshalAs(UnmanagedType.VariantBool)] bool EnableAutoProxy);

        [PreserveSig]
        int get_HTTPProxyHost([MarshalAs(UnmanagedType.BStr)] out string pbstrHTTPProxyHost);

        [PreserveSig]
        int put_HTTPProxyHost([MarshalAs(UnmanagedType.BStr)] string bstrHTTPProxyHost);

        [PreserveSig]
        int get_HTTPProxyPort(out int pHTTPProxyPort);

        [PreserveSig]
        int put_HTTPProxyPort(int HTTPProxyPort);

        [PreserveSig]
        int get_EnableMulticast([MarshalAs(UnmanagedType.VariantBool)] out bool pEnableMulticast);

        [PreserveSig]
        int put_EnableMulticast([MarshalAs(UnmanagedType.VariantBool)] bool EnableMulticast);

        [PreserveSig]
        int get_EnableUDP([MarshalAs(UnmanagedType.VariantBool)] out bool pEnableUDP);

        [PreserveSig]
        int put_EnableUDP([MarshalAs(UnmanagedType.VariantBool)] bool EnableUDP);

        [PreserveSig]
        int get_EnableTCP([MarshalAs(UnmanagedType.VariantBool)] out bool pEnableTCP);

        [PreserveSig]
        int put_EnableTCP([MarshalAs(UnmanagedType.VariantBool)] bool EnableTCP);

        [PreserveSig]
        int get_EnableHTTP([MarshalAs(UnmanagedType.VariantBool)] out bool pEnableHTTP);

        [PreserveSig]
        int put_EnableHTTP([MarshalAs(UnmanagedType.VariantBool)] bool EnableHTTP);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F2-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMChannelInfo
    {
        [PreserveSig]
        int get_ChannelName([MarshalAs(UnmanagedType.BStr)] out string pbstrChannelName);

        [PreserveSig]
        int get_ChannelDescription([MarshalAs(UnmanagedType.BStr)] out string pbstrChannelDescription);

        [PreserveSig]
        int get_ChannelURL([MarshalAs(UnmanagedType.BStr)] out string pbstrChannelURL);

        [PreserveSig]
        int get_ContactAddress([MarshalAs(UnmanagedType.BStr)] out string pbstrContactAddress);

        [PreserveSig]
        int get_ContactPhone([MarshalAs(UnmanagedType.BStr)] out string pbstrContactPhone);

        [PreserveSig]
        int get_ContactEmail([MarshalAs(UnmanagedType.BStr)] out string pbstrContactEmail);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F3-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMNetworkStatus
    {
        [PreserveSig]
        int get_ReceivedPackets(out int pReceivedPackets);

        [PreserveSig]
        int get_RecoveredPackets(out int pRecoveredPackets);

        [PreserveSig]
        int get_LostPackets(out int pLostPackets);

        [PreserveSig]
        int get_ReceptionQuality(out int pReceptionQuality);

        [PreserveSig]
        int get_BufferingCount(out int pBufferingCount);

        [PreserveSig]
        int get_IsBroadcast([MarshalAs(UnmanagedType.VariantBool)] out bool pIsBroadcast);

        [PreserveSig]
        int get_BufferingProgress(out int pBufferingProgress);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F5-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMNetShowExProps
    {
        [PreserveSig]
        int get_SourceProtocol(out int pSourceProtocol);

        [PreserveSig]
        int get_Bandwidth(out int pBandwidth);

        [PreserveSig]
        int get_ErrorCorrection([MarshalAs(UnmanagedType.BStr)] out string pbstrErrorCorrection);

        [PreserveSig]
        int get_CodecCount(out int pCodecCount);

        [PreserveSig]
        int GetCodecInstalled(int CodecNum, [MarshalAs(UnmanagedType.VariantBool)] out bool pCodecInstalled);

        [PreserveSig]
        int GetCodecDescription(int CodecNum, [MarshalAs(UnmanagedType.BStr)] out string pbstrCodecDescription);

        [PreserveSig]
        int GetCodecURL(int CodecNum, [MarshalAs(UnmanagedType.BStr)] out string pbstrCodecURL);

        [PreserveSig]
        int get_CreationDate(out double pCreationDate);

        [PreserveSig]
        int get_SourceLink([MarshalAs(UnmanagedType.BStr)] out string pbstrSourceLink);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F6-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMExtendedErrorInfo
    {
        [PreserveSig]
        int get_HasError([MarshalAs(UnmanagedType.VariantBool)] out bool pHasError);

        [PreserveSig]
        int get_ErrorDescription([MarshalAs(UnmanagedType.BStr)] out string pbstrErrorDescription);

        [PreserveSig]
        int get_ErrorCode(out int pErrorCode);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("AAE7E4E2-6388-11D1-8D93-006097C9A2B2"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMNetShowPreroll
    {
        [PreserveSig]
        int put_Preroll([MarshalAs(UnmanagedType.VariantBool)] bool fPreroll);

        [PreserveSig]
        int get_Preroll([MarshalAs(UnmanagedType.VariantBool)] out bool pfPreroll);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4746B7C8-700E-11D1-BECC-00C04FB6E937"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDShowPlugin
    {
        [PreserveSig]
        int get_URL([MarshalAs(UnmanagedType.BStr)] out string pURL);

        [PreserveSig]
        int get_UserAgent([MarshalAs(UnmanagedType.BStr)] out string pUserAgent);
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F4-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMMediaContent
    {
        [PreserveSig]
        int get_AuthorName([MarshalAs(UnmanagedType.BStr)] out string pbstrAuthorName);

        [PreserveSig]
        int get_Title([MarshalAs(UnmanagedType.BStr)] out string pbstrTitle);

        [PreserveSig]
        int get_Rating([MarshalAs(UnmanagedType.BStr)] out string pbstrRating);

        [PreserveSig]
        int get_Description([MarshalAs(UnmanagedType.BStr)] out string pbstrDescription);

        [PreserveSig]
        int get_Copyright([MarshalAs(UnmanagedType.BStr)] out string pbstrCopyright);

        [PreserveSig]
        int get_BaseURL([MarshalAs(UnmanagedType.BStr)] out string pbstrBaseURL);

        [PreserveSig]
        int get_LogoURL([MarshalAs(UnmanagedType.BStr)] out string pbstrLogoURL);

        [PreserveSig]
        int get_LogoIconURL([MarshalAs(UnmanagedType.BStr)] out string pbstrLogoURL);

        [PreserveSig]
        int get_WatermarkURL([MarshalAs(UnmanagedType.BStr)] out string pbstrWatermarkURL);

        [PreserveSig]
        int get_MoreInfoURL([MarshalAs(UnmanagedType.BStr)] out string pbstrMoreInfoURL);

        [PreserveSig]
        int get_MoreInfoBannerImage([MarshalAs(UnmanagedType.BStr)] out string pbstrMoreInfoBannerImage);

        [PreserveSig]
        int get_MoreInfoBannerURL([MarshalAs(UnmanagedType.BStr)] out string pbstrMoreInfoBannerURL);

        [PreserveSig]
        int get_MoreInfoText([MarshalAs(UnmanagedType.BStr)] out string pbstrMoreInfoText);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FA2AA8F9-8B62-11D0-A520-000000000000"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMExtendedSeeking
    {
        [PreserveSig]
        int get_ExSeekCapabilities(out AMExtendedSeekingCapabilities pExCapabilities);

        [PreserveSig]
        int get_MarkerCount(out int pMarkerCount);

        [PreserveSig]
        int get_CurrentMarker(out int pCurrentMarker);

        [PreserveSig]
        int GetMarkerTime(int MarkerNum, out double pMarkerTime);

        [PreserveSig]
        int GetMarkerName(
            int MarkerNum,
            [MarshalAs(UnmanagedType.BStr)] out string pbstrMarkerName
            );

        [PreserveSig]
        int put_PlaybackSpeed(double Speed);

        [PreserveSig]
        int get_PlaybackSpeed(out double pSpeed);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("CE8F78C1-74D9-11D2-B09D-00A0C9A81117"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAMMediaContent2
    {
        [PreserveSig]
        int get_MediaParameter(int EntryNum, [MarshalAs(UnmanagedType.BStr)] string bstrName, [MarshalAs(UnmanagedType.BStr)] out string pbstrValue);

        [PreserveSig]
        int get_MediaParameterName(int EntryNum, int Index, [MarshalAs(UnmanagedType.BStr)] out string pbstrName);

        [PreserveSig]
        int get_PlaylistCount(out int pNumberEntries);
    }

    #endregion
}
