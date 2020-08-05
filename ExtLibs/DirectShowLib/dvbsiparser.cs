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
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From DVB_STRCONV_MODE
    /// </summary>
    public enum DVB_STRCONV_MODE
    {
        DVB = 0,
        DVB_EMPHASIS = (DVB + 1),
        DVB_WITHOUT_EMPHASIS = (DVB_EMPHASIS + 1),
        ISDB = (DVB_WITHOUT_EMPHASIS + 1)
    }

#endif


    /// <summary>
    /// Define possible values for a running_status field according to ETSI EN 300 468
    /// This enum doesn't exist in the c++ headers
    /// </summary>
    public enum RunningStatus : byte
    {
        Undefined = 0,
        NotRunning = 1,
        StartInAFewSeconds = 2,
        Pausing = 3,
        Running = 4,
        Reserved1 = 5,
        Reserved2 = 6,
        Reserved3 = 7
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B758A7BD-14DC-449d-B828-35909ACB3B1E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbSiParser
    {
        [PreserveSig]
        int Initialize([In] IMpeg2Data punkMpeg2Data);

        [PreserveSig]
        int GetPAT([Out] out IPAT ppPAT);

        [PreserveSig]
        int GetCAT(
          [In] int dwTimeout,
          [Out] out ICAT ppCAT
          );

        [PreserveSig]
        int GetPMT(
          [In] short pid,
          [In] DsShort pwProgramNumber,
          [Out] out IPMT ppPMT
          );

        [PreserveSig]
        int GetTSDT([Out] out ITSDT ppTSDT);

        [PreserveSig]
        int GetNIT(
          [In] byte tableId,
          [In] DsShort pwNetworkId,
          [Out] out IDVB_NIT ppNIT
          );

        [PreserveSig]
        int GetSDT(
          [In] byte tableId,
          [In] DsShort pwTransportStreamId,
          [Out] out IDVB_SDT ppSDT
          );

        [PreserveSig]
        int GetEIT(
          [In] byte tableId,
          [In] DsShort pwServiceId,
          [Out] out IDVB_EIT ppEIT
          );

        [PreserveSig]
        int GetBAT(
          [In] DsShort pwBouquetId,
          [Out] out IDVB_BAT ppBAT
          );

        [PreserveSig]
        int GetRST(
          [In] int dwTimeout,
          [Out] out IDVB_RST ppRST
          );

        [PreserveSig]
        int GetST(
          [In] short pid,
          [In] int dwTimeout,
          [Out] out IDVB_ST ppST
          );

        [PreserveSig]
        int GetTDT([Out] out IDVB_TDT ppTDT);

        [PreserveSig]
        int GetTOT([Out] out IDVB_TOT ppTOT);

        [PreserveSig]
        int GetDIT(
          [In] int dwTimeout,
          [Out] out IDVB_DIT ppDIT
          );

        [PreserveSig]
        int GetSIT(
          [In] int dwTimeout,
          [Out] out IDVB_SIT ppSIT
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F47DCD04-1E23-4fb7-9F96-B40EEAD10B2B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_RST
    {
        [PreserveSig]
        int Initialize([In] ISectionList pSectionList);

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordTransportStreamId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordOriginalNetworkId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordServiceId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordEventId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4D5B9F23-2A02-45de-BCDA-5D5DBFBFBE62"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_ST
    {
        [PreserveSig]
        int Initialize([In] ISectionList pSectionList);

        [PreserveSig]
        int GetDataLength([Out] out short pwVal);

        [PreserveSig]
        int GetData([Out] out IntPtr ppData);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0780DC7D-D55C-4aef-97E6-6B75906E2796"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_TDT
    {
        [PreserveSig]
        int Initialize([In] ISectionList pSectionList);

        [PreserveSig]
        int GetUTCTime([Out] out MpegDateAndTime pmdtVal);
    }


    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("91BFFDF9-9432-410f-86EF-1C228ED0AD70"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_DIT
    {
        [PreserveSig]
        int Initialize([In] ISectionList pSectionList);

        [PreserveSig]
        int GetTransitionFlag([Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("68CDCE53-8BEA-45c2-9D9D-ACF575A089B5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_SIT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetCountOfTableDescriptors([Out] out int pdwVal);

        [PreserveSig]
        int GetTableDescriptorByIndex(
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetTableDescriptorByTag(
          [In] byte bTag,
          [In] IntPtr pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordServiceId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int RegisterForNextTable(
            [In] IntPtr hNextTableAvailable
            );

        [PreserveSig]
        int GetNextTable(
          [In] int dwTimeout,
          [Out] out IDVB_SIT ppSIT
          );

        [PreserveSig]
        int RegisterForWhenCurrent(
            [In] IntPtr hNextTableIsCurrent
            );

        [PreserveSig]
        int ConvertNextToCurrent();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DFB98E36-9E1A-4862-9946-993A4E59017B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbCableDeliverySystemDescriptor
    {
        [PreserveSig]
        int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetFrequency(
            [Out] out int pdwVal
            );

        [PreserveSig]
        int GetFECOuter(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetModulation(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetSymbolRate(
            [Out] out int pdwVal
            );

        [PreserveSig]
        int GetFECInner(
            [Out] out byte pbVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1CADB613-E1DD-4512-AFA8-BB7A007EF8B1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbFrequencyListDescriptor
    {
        [PreserveSig]
        int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetCodingType(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetRecordCentreFrequency(
          [In] byte bRecordIndex,
          [Out] out int pdwVal
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F9C7FBCF-E2D6-464d-B32D-2EF526E49290"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbServiceDescriptor
    {
        [PreserveSig]
        int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetServiceType(
            [Out] out byte pbVal
            );

        [PreserveSig]
        int GetServiceProviderName(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszName
            );

        [PreserveSig]
        int GetServiceProviderNameW(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetServiceName(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszName
            );

        [PreserveSig]
        int GetProcessedServiceName(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetServiceNameEmphasized(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("91E405CF-80E7-457F-9096-1B9D1CE32141"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbComponentDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetStreamContent(
            out byte pbVal
            );

        [PreserveSig]
        int GetComponentType(
            out byte pbVal
            );

        [PreserveSig]
        int GetComponentTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLanguageCode(
            out byte pszCode
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2E883881-A467-412A-9D63-6F2B6DA05BF0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbContentDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordContentNibbles(
            [In] byte bRecordIndex, 
            out byte pbValLevel1, 
            out byte pbValLevel2
            );

        [PreserveSig]
        int GetRecordUserNibbles(
            [In] byte bRecordIndex, 
            out byte pbVal1, 
            out byte pbVal2
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("05E0C1EA-F661-4053-9FBF-D93B28359838"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComConversionLoss]
    public interface IDvbContentIdentifierDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCrid(
            [In] byte bRecordIndex, 
            out byte pbType, 
            out byte pbLocation, 
            out byte pbLength, 
            [Out] IntPtr ppbBytes
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D1EBC1D6-8B60-4C20-9CAF-E59382E7C400"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbDataBroadcastDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetDataBroadcastID(
            out short pwVal
            );

        [PreserveSig]
        int GetComponentTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetSelectorLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetSelectorBytes(
            [In, Out] ref byte pbLen, 
            out byte pbVal
            );

        [PreserveSig]
        int GetLangID(
            out int pulVal
            );

        [PreserveSig]
        int GetTextLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetText(
            [In, Out] ref byte pbLen, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("5F26F518-65C8-4048-91F2-9290F59F7B90")]
    public interface IDvbDataBroadcastIDDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetDataBroadcastID(
            out short pwVal
            );

        [PreserveSig]
        int GetIDSelectorBytes(
            [In, Out] ref byte pbLen, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("05EC24D1-3A31-44E7-B408-67C60A352276")]
    public interface IDvbDefaultAuthorityDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetDefaultAuthority(
            out byte pbLength, 
            [Out] IntPtr ppbBytes
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C9B22ECA-85F4-499F-B1DB-EFA93A91EE57"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbExtendedEventDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetDescriptorNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetLastDescriptorNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetLanguageCode(
            out byte pszCode
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordItemW(
            [In] byte bRecordIndex, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrDesc, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrItem
            );

        [PreserveSig]
        int GetConcatenatedItemW(
            [In, MarshalAs(UnmanagedType.Interface)] IDvbExtendedEventDescriptor pFollowingDescriptor, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrDesc, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrItem
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

        [PreserveSig]
        int GetConcatenatedTextW(
            [In, MarshalAs(UnmanagedType.Interface)] IDvbExtendedEventDescriptor FollowingDescriptor, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

        [PreserveSig]
        int GetRecordItemRawBytes(
            [In] byte bRecordIndex, 
            [Out] IntPtr ppbRawItem, 
            out byte pbItemLength
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("43ACA974-4BE8-4b98-BC17-9EAFD788B1D7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbLogicalChannelDescriptor2 : IDvbLogicalChannelDescriptor
    {
        #region IDvbLogicalChannelDescriptor methods

        [PreserveSig]
        new int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetCountOfRecords(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetRecordServiceId(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        new int GetRecordLogicalChannelNumber(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        #endregion

        [PreserveSig]
        int GetRecordLogicalChannelAndVisibility( 
            byte bRecordIndex,
            out short pwVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1EA8B738-A307-4680-9E26-D0A908C824F4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbHDSimulcastLogicalChannelDescriptor : IDvbLogicalChannelDescriptor2
    {
        #region IDvbLogicalChannelDescriptor methods

        [PreserveSig]
        new int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetCountOfRecords(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetRecordServiceId(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        new int GetRecordLogicalChannelNumber(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        #endregion

        #region IDvbLogicalChannelDescriptor2 Methods

        [PreserveSig]
        new int GetRecordLogicalChannelAndVisibility(
            byte bRecordIndex,
            out short pwVal);

        #endregion

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1CDF8B31-994A-46FC-ACFD-6A6BE8934DD5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbLinkageDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetTSId(
            out short pwVal
            );

        [PreserveSig]
        int GetONId(
            out short pwVal
            );

        [PreserveSig]
        int GetServiceId(
            out short pwVal
            );

        [PreserveSig]
        int GetLinkageType(
            out byte pbVal
            );

        [PreserveSig]
        int GetPrivateDataLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetPrivateData(
            [In, Out] ref byte pbLen, 
            out byte pbData
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F69C3747-8A30-4980-998C-01FE7F0BA35A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbLogicalChannel2Descriptor : IDvbLogicalChannelDescriptor2
    {
        #region IDvbLogicalChannelDescriptor methods

        [PreserveSig]
        new int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetCountOfRecords(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetRecordServiceId(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        new int GetRecordLogicalChannelNumber(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        #endregion

        #region IDvbLogicalChannelDescriptor2 Methods

        [PreserveSig]
        new int GetRecordLogicalChannelAndVisibility(
            byte bRecordIndex,
            out short pwVal);

        #endregion

        [PreserveSig]
        int GetCountOfLists(
            out byte pbVal
            );

        [PreserveSig]
        int GetListId(
            [In] byte bListIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetListNameW(
            [In] byte bListIndex, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetListCountryCode(
            [In] byte bListIndex, 
            out byte pszCode
            );

        [PreserveSig]
        int GetListCountOfRecords(
            [In] byte bChannelListIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetListRecordServiceId(
            [In] byte bListIndex, 
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetListRecordLogicalChannelNumber(
            [In] byte bListIndex, 
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetListRecordLogicalChannelAndVisibility(
            [In] byte bListIndex, 
            [In] byte bRecordIndex, 
            out short pwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2D80433B-B32C-47EF-987F-E78EBB773E34"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbMultilingualServiceNameDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordLangId(
            [In] byte bRecordIndex, out int ulVal
            );

        [PreserveSig]
        int GetRecordServiceProviderNameW(
            [In] byte bRecordIndex, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetRecordServiceNameW(
            [In] byte bRecordIndex, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("5B2A80CF-35B9-446C-B3E4-048B761DBC51"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbNetworkNameDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetNetworkName(
            [Out] IntPtr pszName
            );

        [PreserveSig]
        int GetNetworkNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("3AD9DDE1-FB1B-4186-937F-22E6B5A72A10")]
    public interface IDvbParentalRatingDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordRating(
            [In] byte bRecordIndex, 
            out byte pszCountryCode, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("5660A019-E75A-4B82-9B4C-ED2256D165A2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbPrivateDataSpecifierDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetPrivateDataSpecifier(
            out int pdwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0F37BD92-D6A1-4854-B950-3A969D27F30E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbServiceAttributeDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordServiceId(
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordNumericSelectionFlag(
            [In] byte bRecordIndex, 
            out int pfVal
            );

        [PreserveSig]
        int GetRecordVisibleServiceFlag(
            [In] byte bRecordIndex, 
            out int pfVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D6C76506-85AB-487C-9B2B-36416511E4A2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbServiceDescriptor2 : IDvbServiceDescriptor
    {
        #region IDvbServiceDescriptor Methods

        [PreserveSig]
        new int GetTag(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetLength(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetServiceType(
            [Out] out byte pbVal
            );

        [PreserveSig]
        new int GetServiceProviderName(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszName
            );

        [PreserveSig]
        new int GetServiceProviderNameW(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        new int GetServiceName(
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pszName
            );

        [PreserveSig]
        new int GetProcessedServiceName(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        new int GetServiceNameEmphasized(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        #endregion

        [PreserveSig]
        int GetServiceProviderNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetServiceNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("05DB0D8F-6008-491A-ACD3-7090952707D0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbServiceListDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordServiceId(
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordServiceType(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B170BE92-5B75-458E-9C6E-B0008231491A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbShortEventDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetLanguageCode(
            out byte pszCode
            );

        [PreserveSig]
        int GetEventNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0AC5525F-F816-42F4-93BA-4C0F32F46E54"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbSiParser2 : IDvbSiParser
    {
        #region IDvbSiParser Methods

        [PreserveSig]
        new int Initialize(
            [In] IMpeg2Data punkMpeg2Data)
            ;

        [PreserveSig]
        new int GetPAT(
            [Out] out IPAT ppPAT
            );

        [PreserveSig]
        new int GetCAT(
          [In] int dwTimeout,
          [Out] out ICAT ppCAT
          );

        [PreserveSig]
        new int GetPMT(
          [In] short pid,
          [In] DsShort pwProgramNumber,
          [Out] out IPMT ppPMT
          );

        [PreserveSig]
        new int GetTSDT(
            [Out] out ITSDT ppTSDT
            );

        [PreserveSig]
        new int GetNIT(
          [In] byte tableId,
          [In] DsShort pwNetworkId,
          [Out] out IDVB_NIT ppNIT
          );

        [PreserveSig]
        new int GetSDT(
          [In] byte tableId,
          [In] DsShort pwTransportStreamId,
          [Out] out IDVB_SDT ppSDT
          );

        [PreserveSig]
        new int GetEIT(
          [In] byte tableId,
          [In] DsShort pwServiceId,
          [Out] out IDVB_EIT ppEIT
          );

        [PreserveSig]
        new int GetBAT(
          [In] DsShort pwBouquetId,
          [Out] out IDVB_BAT ppBAT
          );

        [PreserveSig]
        new int GetRST(
          [In] int dwTimeout,
          [Out] out IDVB_RST ppRST
          );

        [PreserveSig]
        new int GetST(
          [In] short pid,
          [In] int dwTimeout,
          [Out] out IDVB_ST ppST
          );

        [PreserveSig]
        new int GetTDT(
            [Out] out IDVB_TDT ppTDT
            );

        [PreserveSig]
        new int GetTOT(
            [Out] out IDVB_TOT ppTOT
            );

        [PreserveSig]
        new int GetDIT(
          [In] int dwTimeout,
          [Out] out IDVB_DIT ppDIT
          );

        [PreserveSig]
        new int GetSIT(
          [In] int dwTimeout,
          [Out] out IDVB_SIT ppSIT
          );

        #endregion

        [PreserveSig]
        int GetEIT2(
            [In] byte TableId, 
            [In] ref short pwServiceId, 
            [In] ref byte pbSegment, 
            [MarshalAs(UnmanagedType.Interface)] out IDVB_EIT2 ppEIT
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9B25FE1D-FA23-4E50-9784-6DF8B26F8A49"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbSubtitlingDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordLangId(
            [In] byte bRecordIndex, 
            out int pulVal
            );

        [PreserveSig]
        int GetRecordSubtitlingType(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCompositionPageID(
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordAncillaryPageID(
            [In] byte bRecordIndex, 
            out short pwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9CD29D47-69C6-4F92-98A9-210AF1B7303A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbTeletextDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordLangId(
            [In] byte bRecordIndex, 
            out int pulVal
            );

        [PreserveSig]
        int GetRecordTeletextType(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordMagazineNumber(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordPageNumber(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("537CD71E-0E46-4173-9001-BA043F3E49E2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_BIT
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetOriginalNetworkId(
            out short pwVal
            );

        [PreserveSig]
        int GetBroadcastViewPropriety(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfTableDescriptors(
            out int pdwVal
            );

        [PreserveSig]
        int GetTableDescriptorByIndex(
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetTableDescriptorByTag(
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordBroadcasterId(
            [In] int dwRecordIndex, out byte pbVal
            );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
            [In] int dwRecordIndex, out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetRecordDescriptorByTag(
            [In] int dwRecordIndex, 
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("25FA92C2-8B80-4787-A841-3A0E8F17984B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_CDT
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData, 
            [In] byte bSectionNumber
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetDownloadDataId(
            out short pwVal
            );

        [PreserveSig]
        int GetSectionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetOriginalNetworkId(
            out short pwVal
            );

        [PreserveSig]
        int GetDataType(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfTableDescriptors(
            out int pdwVal
            );

        [PreserveSig]
        int GetTableDescriptorByIndex(
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetTableDescriptorByTag(
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetSizeOfDataModule(
            out int pdwVal
            );

        [PreserveSig]
        int GetDataModule(
            [Out] IntPtr pbData
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0EDB556D-43AD-4938-9668-321B2FFECFD3"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_EMM
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetTableIdExtension(
            out short pwVal
            );

        [PreserveSig]
        int GetDataBytes(
            [In, Out] ref short pwBufferLength, 
            out byte pbBuffer
            );

        [PreserveSig]
        int GetSharedEmmMessage(
            ref short pwLength, IntPtr ppbMessage
            );

        [PreserveSig]
        int GetIndividualEmmMessage(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnknown, 
            ref short pwLength, 
            IntPtr ppbMessage
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("141A546B-02FF-4FB9-A3A3-2F074B74A9A9"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_LDT
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetOriginalServiceId(
            out short pwVal
            );

        [PreserveSig]
        int GetTransportStreamId(
            out short pwVal
            );

        [PreserveSig]
        int GetOriginalNetworkId(
            out short pwVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptionId(
            [In] int dwRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
            [In] int dwRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetRecordDescriptorByTag(
            [In] int dwRecordIndex, 
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1B1863EF-08F1-40B7-A559-3B1EFF8CAFA6"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_NBIT
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetOriginalNetworkId(
            out short pwVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordInformationId(
            [In] int dwRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordInformationType(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordDescriptionBodyLocation(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordMessageSectionNumber(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordUserDefined(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordNumberOfKeys(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordKeys(
            [In] int dwRecordIndex, 
            [Out] IntPtr pbKeys
            );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
            [In] int dwRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetRecordDescriptorByTag(
            [In] int dwRecordIndex, 
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3F3DC9A2-BB32-4FB9-AE9E-D856848927A3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_SDT : IDVB_SDT
    {
        #region IDVB_SDT Methods

        [PreserveSig]
        new int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        new int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        new int GetTransportStreamId([Out] out short pwVal);

        [PreserveSig]
        new int GetOriginalNetworkId([Out] out short pwVal);

        [PreserveSig]
        new int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        new int GetRecordServiceId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        new int GetRecordEITScheduleFlag(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        new int GetRecordEITPresentFollowingFlag(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        new int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );

        [PreserveSig]
        new int GetRecordFreeCAMode(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        new int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        new int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        new int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        new int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        new int GetNextTable([Out] out IDVB_SDT ppSDT);

        [PreserveSig]
        new int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        new int ConvertNextToCurrent();

        [PreserveSig]
        new int GetVersionHash([Out] out int pdwVersionHash);

        #endregion

        [PreserveSig]
        int GetRecordEITUserDefinedFlags(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EE60EF2D-813A-4DC7-BF92-EA13DAC85313"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IISDB_SDTT
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.Interface)] ISectionList pSectionList, 
            [In, MarshalAs(UnmanagedType.Interface)] IMpeg2Data pMPEGData
            );

        [PreserveSig]
        int GetVersionNumber(
            out byte pbVal
            );

        [PreserveSig]
        int GetTableIdExt(
            out short pwVal
            );

        [PreserveSig]
        int GetTransportStreamId(
            out short pwVal
            );

        [PreserveSig]
        int GetOriginalNetworkId(
            out short pwVal
            );

        [PreserveSig]
        int GetServiceId(
            out short pwVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordGroup(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordTargetVersion(
            [In] int dwRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordNewVersion(
            [In] int dwRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordDownloadLevel(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordVersionIndicator(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordScheduleTimeShiftInformation(
            [In] int dwRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCountOfSchedules(
            [In] int dwRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordStartTimeByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex,
            out MpegDateAndTime pmdtVal
            );

        [PreserveSig]
        int GetRecordDurationByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex,
            out MpegDuration pmdVal
            );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
            [In] int dwRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetRecordDescriptorByTag(
            [In] int dwRecordIndex, 
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetVersionHash(
            out int pdwVersionHash
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("679D2002-2425-4BE4-A4C7-D6632A574F4D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbAudioComponentDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetStreamContent(
            out byte pbVal
            );

        [PreserveSig]
        int GetComponentType(
            out byte pbVal
            );

        [PreserveSig]
        int GetComponentTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetStreamType(
            out byte pbVal
            );

        [PreserveSig]
        int GetSimulcastGroupTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetESMultiLingualFlag(
            out int pfVal
            );

        [PreserveSig]
        int GetMainComponentFlag(
            out int pfVal
            );

        [PreserveSig]
        int GetQualityIndicator(
            out byte pbVal
            );

        [PreserveSig]
        int GetSamplingRate(
            out byte pbVal
            );

        [PreserveSig]
        int GetLanguageCode(
            out byte pszCode
            );

        [PreserveSig]
        int GetLanguageCode2(
            out byte pszCode
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("08E18B25-A28F-4E92-821E-4FCED5CC2291"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbCAContractInformationDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCASystemId(
            out short pwVal
            );

        [PreserveSig]
        int GetCAUnitId(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordComponentTag(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetContractVerificationInfoLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetContractVerificationInfo(
            [In] byte bBufLength, 
            out byte pbBuf
            );

        [PreserveSig]
        int GetFeeNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0570AA47-52BC-42AE-8CA5-969F41E81AEA"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbCADescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCASystemId(
            out short pwVal
            );

        [PreserveSig]
        int GetReservedBits(
            out byte pbVal
            );

        [PreserveSig]
        int GetCAPID(
            out short pwVal
            );

        [PreserveSig]
        int GetPrivateDataBytes(
            [In, Out] ref byte pbBufferLength, 
            out byte pbBuffer
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("39CBEB97-FF0B-42A7-9AB9-7B9CFE70A77A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbCAServiceDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCASystemId(
            out short pwVal
            );

        [PreserveSig]
        int GetCABroadcasterGroupId(
            out byte pbVal
            );

        [PreserveSig]
        int GetMessageControl(
            out byte pbVal
            );

        [PreserveSig]
        int GetServiceIds(
            [In, Out] ref byte pbNumServiceIds, 
            out short pwServiceIds
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A494F17F-C592-47D8-8943-64C9A34BE7B9"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbComponentGroupDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetComponentGroupType(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordGroupId(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordNumberOfCAUnit(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCAUnitCAUnitId(
            [In] byte bRecordIndex, 
            [In] byte bCAUnitIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCAUnitNumberOfComponents(
            [In] byte bRecordIndex, [
            In] byte bCAUnitIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCAUnitComponentTag(
            [In] byte bRecordIndex, 
            [In] byte bCAUnitIndex, 
            [In] byte bComponentIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordTotalBitRate(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordTextW(
            [In] byte bRecordIndex, 
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A428100A-E646-4BD6-AA14-6087BDC08CD5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbDataContentDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetDataComponentId(
            out short pwVal
            );

        [PreserveSig]
        int GetEntryComponent(
            out byte pbVal
            );

        [PreserveSig]
        int GetSelectorLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetSelectorBytes(
            [In] byte bBufLength, 
            out byte pbBuf
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordComponentRef(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetLanguageCode(
            out byte pszCode
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrText
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1A28417E-266A-4BB8-A4BD-D782BCFB8161"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbDigitalCopyControlDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCopyControl(
            out byte pbDigitalRecordingControlData, 
            out byte pbCopyControlType, 
            out byte pbAPSControlData, 
            out byte pbMaximumBitrate
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordCopyControl(
            [In] byte bRecordIndex, 
            out byte pbComponentTag, 
            out byte pbDigitalRecordingControlData, 
            out byte pbCopyControlType, 
            out byte pbAPSControlData, 
            out byte pbMaximumBitrate
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("5298661E-CB88-4F5F-A1DE-5F440C185B92"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbDownloadContentDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetFlags(
            out int pfReboot, 
            out int pfAddOn, 
            out int pfCompatibility, 
            out int pfModuleInfo, 
            out int pfTextInfo
            );

        [PreserveSig]
        int GetComponentSize(
            out int pdwVal
            );

        [PreserveSig]
        int GetDownloadId(
            out int pdwVal
            );

        [PreserveSig]
        int GetTimeOutValueDII(
            out int pdwVal
            );

        [PreserveSig]
        int GetLeakRate(
            out int pdwVal
            );

        [PreserveSig]
        int GetComponentTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetCompatiblityDescriptorLength(
            out short pwLength
            );

        [PreserveSig]
        int GetCompatiblityDescriptor(
            [Out] IntPtr ppbData
            );

        [PreserveSig]
        int GetCountOfRecords(
            out short pwVal
            );

        [PreserveSig]
        int GetRecordModuleId(
            [In] short wRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetRecordModuleSize(
            [In] short wRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordModuleInfoLength(
            [In] short wRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordModuleInfo(
            [In] short wRecordIndex, 
            [Out] IntPtr ppbData
            );

        [PreserveSig]
        int GetTextLanguageCode(
            out byte szCode
            );

        [PreserveSig]
        int GetTextW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BA6FA681-B973-4DA1-9207-AC3E7F0341EB"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbEmergencyInformationDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetServiceId(
            [In] byte bRecordIndex, 
            out short pwVal
            );

        [PreserveSig]
        int GetStartEndFlag(
            [In] byte bRecordIndex, 
            out byte pVal
            );

        [PreserveSig]
        int GetSignalLevel(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetAreaCode(
            [In] byte bRecordIndex, 
            [Out] IntPtr ppwVal, 
            out byte pbNumAreaCodes
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("94B06780-2E2A-44DC-A966-CC56FDABC6C2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbEventGroupDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetGroupType(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordEvent(
            [In] byte bRecordIndex, 
            out short pwServiceId, 
            out short pwEventId
            );

        [PreserveSig]
        int GetCountOfRefRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRefRecordEvent(
            [In] byte bRecordIndex, 
            out short pwOriginalNetworkId, 
            out short pwTransportStreamId, 
            out short pwServiceId, 
            out short pwEventId
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B7B3AE90-EE0B-446D-8769-F7E2AA266AA6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbHierarchicalTransmissionDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetFutureUse1(
            out byte pbVal
            );

        [PreserveSig]
        int GetQualityLevel(
            out byte pbVal
            );

        [PreserveSig]
        int GetFutureUse2(
            out byte pbVal
            );

        [PreserveSig]
        int GetReferencePid(
            out short pwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("E0103F49-4AE1-4F07-9098-756DB1FA88CD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbLogoTransmissionDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetLogoTransmissionType(
            out byte pbVal
            );

        [PreserveSig]
        int GetLogoId(
            out short pwVal
            );

        [PreserveSig]
        int GetLogoVersion(
            out short pwVal
            );

        [PreserveSig]
        int GetDownloadDataId(
            out short pwVal
            );

        [PreserveSig]
        int GetLogoCharW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrChar
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("07EF6370-1660-4F26-87FC-614ADAB24B11"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbSeriesDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetSeriesId(
            out short pwVal
            );

        [PreserveSig]
        int GetRepeatLabel(
            out byte pbVal
            );

        [PreserveSig]
        int GetProgramPattern(
            out byte pbVal
            );

        [PreserveSig]
        int GetExpireDate(
            out int pfValid,
            out MpegDateAndTime pmdtVal
            );

        [PreserveSig]
        int GetEpisodeNumber(
            out short pwVal
            );

        [PreserveSig]
        int GetLastEpisodeNumber(
            out short pwVal
            );

        [PreserveSig]
        int GetSeriesNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F837DC36-867C-426A-9111-F62093951A45"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbSIParameterDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetParameterVersion(
            out byte pbVal
            );

        [PreserveSig]
        int GetUpdateTime(
            out MpegDate pVal
            );

        [PreserveSig]
        int GetRecordNumberOfTable(
            out byte pbVal
            );

        [PreserveSig]
        int GetTableId(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetTableDescriptionLength(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetTableDescriptionBytes(
            [In] byte bRecordIndex, 
            [In, Out] ref byte pbBufferLength, 
            out byte pbBuffer
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("900E4BB7-18CD-453F-98BE-3BE6AA211772"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbSiParser2 : IDvbSiParser2
    {
        #region IDvbSiParser Methods

        [PreserveSig]
        new int Initialize(
            [In] IMpeg2Data punkMpeg2Data)
            ;

        [PreserveSig]
        new int GetPAT(
            [Out] out IPAT ppPAT
            );

        [PreserveSig]
        new int GetCAT(
          [In] int dwTimeout,
          [Out] out ICAT ppCAT
          );

        [PreserveSig]
        new int GetPMT(
          [In] short pid,
          [In] DsShort pwProgramNumber,
          [Out] out IPMT ppPMT
          );

        [PreserveSig]
        new int GetTSDT(
            [Out] out ITSDT ppTSDT
            );

        [PreserveSig]
        new int GetNIT(
          [In] byte tableId,
          [In] DsShort pwNetworkId,
          [Out] out IDVB_NIT ppNIT
          );

        [PreserveSig]
        new int GetSDT(
          [In] byte tableId,
          [In] DsShort pwTransportStreamId,
          [Out] out IDVB_SDT ppSDT
          );

        [PreserveSig]
        new int GetEIT(
          [In] byte tableId,
          [In] DsShort pwServiceId,
          [Out] out IDVB_EIT ppEIT
          );

        [PreserveSig]
        new int GetBAT(
          [In] DsShort pwBouquetId,
          [Out] out IDVB_BAT ppBAT
          );

        [PreserveSig]
        new int GetRST(
          [In] int dwTimeout,
          [Out] out IDVB_RST ppRST
          );

        [PreserveSig]
        new int GetST(
          [In] short pid,
          [In] int dwTimeout,
          [Out] out IDVB_ST ppST
          );

        [PreserveSig]
        new int GetTDT(
            [Out] out IDVB_TDT ppTDT
            );

        [PreserveSig]
        new int GetTOT(
            [Out] out IDVB_TOT ppTOT
            );

        [PreserveSig]
        new int GetDIT(
          [In] int dwTimeout,
          [Out] out IDVB_DIT ppDIT
          );

        [PreserveSig]
        new int GetSIT(
          [In] int dwTimeout,
          [Out] out IDVB_SIT ppSIT
          );

        #endregion

        #region IDvbSiParser2 Methods

        [PreserveSig]
        new int GetEIT2(
            [In] byte TableId,
            [In] ref short pwServiceId,
            [In] ref byte pbSegment,
            [MarshalAs(UnmanagedType.Interface)] out IDVB_EIT2 ppEIT
            );

        #endregion

        [PreserveSig]
        int GetSDT(
            [In] byte TableId, 
            [In] ref short pwTransportStreamId, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_SDT ppSDT
            );

        [PreserveSig]
        int GetBIT(
            [In] byte TableId, 
            [In] ref short pwOriginalNetworkId, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_BIT ppBIT
            );

        [PreserveSig]
        int GetNBIT(
            [In] byte TableId, 
            [In] ref short pwOriginalNetworkId, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_NBIT ppNBIT
            );

        [PreserveSig]
        int GetLDT(
            [In] byte TableId, 
            [In] ref short pwOriginalServiceId, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_LDT ppLDT
            );

        [PreserveSig]
        int GetSDTT(
            [In] byte TableId, 
            [In] ref short pwTableIdExt, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_SDTT ppSDTT
            );

        [PreserveSig]
        int GetCDT(
            [In] byte TableId, 
            [In] byte bSectionNumber, 
            [In] ref short pwDownloadDataId, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_CDT ppCDT
            );

        [PreserveSig]
        int GetEMM(
            [In] short Pid, 
            [In] short wTableIdExt, 
            [MarshalAs(UnmanagedType.Interface)] out IISDB_EMM ppEMM
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("39FAE0A6-D151-44DD-A28A-765DE5991670"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbTerrestrialDeliverySystemDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetAreaCode(
            out short pwVal
            );

        [PreserveSig]
        int GetGuardInterval(
            out byte pbVal
            );

        [PreserveSig]
        int GetTransmissionMode(
            out byte pbVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordFrequency(
            [In] byte bRecordIndex, 
            out int pdwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D7AD183E-38F5-4210-B55F-EC8D601BBD47"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IIsdbTSInformationDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out byte pbVal
            );

        [PreserveSig]
        int GetRemoteControlKeyId(
            out byte pbVal
            );

        [PreserveSig]
        int GetTSNameW(
            [In] DVB_STRCONV_MODE convMode, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetCountOfRecords(
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordTransmissionTypeInfo(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordNumberOfServices(
            [In] byte bRecordIndex, 
            out byte pbVal
            );

        [PreserveSig]
        int GetRecordServiceIdByIndex(
            [In] byte bRecordIndex, 
            [In] byte bServiceIndex, 
            out short pdwVal
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A35F2DEA-098F-4EBD-984C-2BD4C3C8CE0A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPBDA_EIT
    {
        [PreserveSig]
        int Initialize(
            [In] int size, 
            [In] ref byte pBuffer
            );

        [PreserveSig]
        int GetTableId(
            out byte pbVal
            );

        [PreserveSig]
        int GetVersionNumber(
            out short pwVal
            );

        [PreserveSig]
        int GetServiceIdx(
            out long plwVal
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordEventId(
            [In] int dwRecordIndex, 
            out long plwVal
            );

        [PreserveSig]
        int GetRecordStartTime(
            [In] int dwRecordIndex,
            out MpegDateAndTime pmdtVal
            );

        [PreserveSig]
        int GetRecordDuration(
            [In] int dwRecordIndex,
            out MpegDuration pmdVal
            );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
            [In] int dwRecordIndex, 
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
            [In] int dwRecordIndex, 
            [In] int dwIndex, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

        [PreserveSig]
        int GetRecordDescriptorByTag(
            [In] int dwRecordIndex, 
            [In] byte bTag, 
            [In, Out] ref int pdwCookie, 
            [MarshalAs(UnmanagedType.Interface)] out IGenericDescriptor ppDescriptor
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("944EAB37-EED4-4850-AFD2-77E7EFEB4427"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPBDA_Services
    {
        [PreserveSig]
        int Initialize(
            [In] int size, 
            [In] ref byte pBuffer
            );

        [PreserveSig]
        int GetCountOfRecords(
            out int pdwVal
            );

        [PreserveSig]
        int GetRecordByIndex(
            [In] int dwRecordIndex, 
            out long pul64ServiceIdx
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("313B3620-3263-45A6-9533-968BEFBEAC03"), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPBDAAttributesDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out short pwVal
            );

        [PreserveSig]
        int GetAttributePayload(
            [Out] IntPtr ppbAttributeBuffer, 
            out int pdwAttributeLength
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("22632497-0DE3-4587-AADC-D8D99017E760"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPBDAEntitlementDescriptor
    {
        [PreserveSig]
        int GetTag(
            out byte pbVal
            );

        [PreserveSig]
        int GetLength(
            out short pwVal
            );

        [PreserveSig]
        int GetToken(
            [Out] IntPtr ppbTokenBuffer, 
            out int pdwTokenLength
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("9DE49A74-ABA2-4A18-93E1-21F17F95C3C3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPBDASiParser
    {
        [PreserveSig]
        int Initialize(
            [In, MarshalAs(UnmanagedType.IUnknown)] object punk
            );

        [PreserveSig]
        int GetEIT(
            [In] int dwSize, 
            [In] ref byte pBuffer, 
            [MarshalAs(UnmanagedType.Interface)] out IPBDA_EIT ppEIT
            );

        [PreserveSig]
        int GetServices(
            [In] int dwSize, 
            [In] ref byte pBuffer, 
            [MarshalAs(UnmanagedType.Interface)] out IPBDA_Services ppServices
            );

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("20EE9BE9-CD57-49ab-8F6E-1D07AEB8E482"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbTerrestrial2DeliverySystemDescriptor
    {
        [PreserveSig]
        int GetTag([Out] byte  pbVal);

        [PreserveSig]
        int GetLength([Out] byte pbVal);

        [PreserveSig]
        int GetTagExtension([Out] byte pbVal);

        [PreserveSig]
        int GetCentreFrequency([Out] out int pdwVal);

        [PreserveSig]
        int GetPLPId([Out] byte pbVal);

        [PreserveSig]
        int GetT2SystemId([Out] out short pwVal);

        [PreserveSig]
        int GetMultipleInputMode([Out] byte pbVal);

        [PreserveSig]
        int GetBandwidth([Out] byte pbVal);

        [PreserveSig]
        int GetGuardInterval([Out] byte pbVal);

        [PreserveSig]
        int GetTransmissionMode([Out] byte pbVal);

        [PreserveSig]
        int GetCellId([Out] out short pwVal);

        [PreserveSig]
        int GetOtherFrequencyFlag([Out] byte pbVal);

        [PreserveSig]
        int GetTFSFlag([Out] byte pbVal);

    };

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("61A389E0-9B9E-4ba0-AEEA-5DDD159820EA"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_EIT2 : IDVB_EIT
    {
        #region Methods

        [PreserveSig]
        new int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        new int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        new int GetServiceId([Out] out short pwVal);

        [PreserveSig]
        new int GetTransportStreamId([Out] out short pwVal);

        [PreserveSig]
        new int GetOriginalNetworkId([Out] out short pwVal);

        [PreserveSig]
        new int GetSegmentLastSectionNumber([Out] out byte pbVal);

        [PreserveSig]
        new int GetLastTableId([Out] out byte pbVal);

        [PreserveSig]
        new int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        new int GetRecordEventId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        new int GetRecordStartTime(
          [In] int dwRecordIndex,
          [Out] out MpegDateAndTime pmdtVal
          );

        [PreserveSig]
        new int GetRecordDuration(
          [In] int dwRecordIndex,
          [Out] out MpegDuration pmdVal
          );

        [PreserveSig]
        new int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );

        [PreserveSig]
        new int GetRecordFreeCAMode(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        new int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        new int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        new int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        new int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        new int GetNextTable([Out] out IDVB_EIT ppEIT);

        [PreserveSig]
        new int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        new int ConvertNextToCurrent();

        [PreserveSig]
        new int GetVersionHash([Out] out int pdwVersionHash);

        #endregion

        [PreserveSig]
        int GetSegmentInfo( 
            out byte pbTid,
            out byte pbSegment
            );
        
        [PreserveSig]
        int GetRecordSection( 
            int dwRecordIndex,
            out byte pbVal
            );
        
    };
    
#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C64935F4-29E4-4e22-911A-63F7F55CB097"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_NIT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetNetworkId([Out] out short pwVal);

        [PreserveSig]
        int GetCountOfTableDescriptors([Out] out int pdwVal);

        [PreserveSig]
        int GetTableDescriptorByIndex(
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetTableDescriptorByTag(
          [In] byte bTag,
          [In] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordTransportStreamId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordOriginalNetworkId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        int GetNextTable([Out] out IDVB_NIT ppNIT);

        [PreserveSig]
        int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        int ConvertNextToCurrent();

        [PreserveSig]
        int GetVersionHash([Out] out int pdwVersionHash);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ECE9BB0C-43B6-4558-A0EC-1812C34CD6CA"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_BAT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetBouquetId([Out] out short pwVal);

        [PreserveSig]
        int GetCountOfTableDescriptors([Out] out int pdwVal);

        [PreserveSig]
        int GetTableDescriptorByIndex(
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetTableDescriptorByTag(
          [In] byte bTag,
          [In] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordTransportStreamId(
          [In] int dwRecordIndex,
          [Out] out short pwVal);

        [PreserveSig]
        int GetRecordOriginalNetworkId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        int GetNextTable([Out] out IDVB_BAT ppBAT);

        [PreserveSig]
        int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        int ConvertNextToCurrent();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ED7E1B91-D12E-420c-B41D-A49D84FE1823"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbTerrestrialDeliverySystemDescriptor
    {
        [PreserveSig]
        int GetTag([Out] out byte pbVal);

        [PreserveSig]
        int GetLength([Out] out byte pbVal);

        [PreserveSig]
        int GetCentreFrequency([Out] out int pdwVal);

        [PreserveSig]
        int GetBandwidth([Out] out byte pbVal);

        [PreserveSig]
        int GetConstellation([Out] out byte pbVal);

        [PreserveSig]
        int GetHierarchyInformation([Out] out byte pbVal);

        [PreserveSig]
        int GetCodeRateHPStream([Out] out byte pbVal);

        [PreserveSig]
        int GetCodeRateLPStream([Out] out byte pbVal);

        [PreserveSig]
        int GetGuardInterval([Out] out byte pbVal);

        [PreserveSig]
        int GetTransmissionMode([Out] out byte pbVal);

        [PreserveSig]
        int GetOtherFrequencyFlag([Out] out byte pbVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("442DB029-02CB-4495-8B92-1C13375BCE99"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_EIT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetServiceId([Out] out short pwVal);

        [PreserveSig]
        int GetTransportStreamId([Out] out short pwVal);

        [PreserveSig]
        int GetOriginalNetworkId([Out] out short pwVal);

        [PreserveSig]
        int GetSegmentLastSectionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetLastTableId([Out] out byte pbVal);

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordEventId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordStartTime(
          [In] int dwRecordIndex,
          [Out] out MpegDateAndTime pmdtVal
          );

        [PreserveSig]
        int GetRecordDuration(
          [In] int dwRecordIndex,
          [Out] out MpegDuration pmdVal
          );

        [PreserveSig]
        int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );

        [PreserveSig]
        int GetRecordFreeCAMode(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        int GetNextTable([Out] out IDVB_EIT ppEIT);

        [PreserveSig]
        int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        int ConvertNextToCurrent();

        [PreserveSig]
        int GetVersionHash([Out] out int pdwVersionHash);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("02F2225A-805B-4ec5-A9A6-F9B5913CD470"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbSatelliteDeliverySystemDescriptor
    {
        [PreserveSig]
        int GetTag([Out] out byte pbVal);

        [PreserveSig]
        int GetLength([Out] out byte pbVal);

        [PreserveSig]
        int GetFrequency([Out] out int pdwVal);

        [PreserveSig]
        int GetOrbitalPosition([Out] out short pwVal);

        [PreserveSig]
        int GetWestEastFlag([Out] out byte pbVal);

        [PreserveSig]
        int GetPolarization([Out] out byte pbVal);

        [PreserveSig]
        int GetModulation([Out] out byte pbVal);

        [PreserveSig]
        int GetSymbolRate([Out] out int pdwVal);

        [PreserveSig]
        int GetFECInner([Out] out byte pbVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("02CAD8D3-FE43-48e2-90BD-450ED9A8A5FD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_SDT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetTransportStreamId([Out] out short pwVal);

        [PreserveSig]
        int GetOriginalNetworkId([Out] out short pwVal);

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordServiceId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordEITScheduleFlag(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordEITPresentFollowingFlag(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordRunningStatus(
          [In] int dwRecordIndex,
          [Out] out RunningStatus pbVal
          );

        [PreserveSig]
        int GetRecordFreeCAMode(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordCountOfDescriptors(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordDescriptorByIndex(
          [In] int dwRecordIndex,
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetRecordDescriptorByTag(
          [In] int dwRecordIndex,
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int RegisterForNextTable([In] IntPtr hNextTableAvailable);

        [PreserveSig]
        int GetNextTable([Out] out IDVB_SDT ppSDT);

        [PreserveSig]
        int RegisterForWhenCurrent([In] IntPtr hNextTableIsCurrent);

        [PreserveSig]
        int ConvertNextToCurrent();

        [PreserveSig]
        int GetVersionHash([Out] out int pdwVersionHash);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("CF1EDAFF-3FFD-4cf7-8201-35756ACBF85F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDvbLogicalChannelDescriptor
    {
        [PreserveSig]
        int GetTag([Out] out byte pbVal);

        [PreserveSig]
        int GetLength([Out] out byte pbVal);

        [PreserveSig]
        int GetCountOfRecords([Out] out byte pbVal);

        [PreserveSig]
        int GetRecordServiceId(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordLogicalChannelNumber(
          [In] byte bRecordIndex,
          [Out] out short pwVal
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("83295D6A-FABA-4ee1-9B15-8067696910AE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVB_TOT
    {
        [PreserveSig]
        int Initialize([In] ISectionList pSectionList);

        [PreserveSig]
        int GetUTCTime([Out] out MpegDateAndTime pmdtVal);

        [PreserveSig]
        int GetCountOfTableDescriptors([Out] out int pdwVal);

        [PreserveSig]
        int GetTableDescriptorByIndex(
          [In] int dwIndex,
          [Out] out IGenericDescriptor ppDescriptor
          );

        [PreserveSig]
        int GetTableDescriptorByTag(
          [In] byte bTag,
          [In, Out] DsInt pdwCookie,
          [Out] out IGenericDescriptor ppDescriptor
          );
    }


    #endregion

}
