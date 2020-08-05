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
    /// From ATSC_ETM_LOCATION_*
    /// </summary>
    public enum AtscEtmLocation
    {
        NotPresent = 0x00,
        InPtcForPsip = 0x01,
        InPtcForEvent = 0x02,
        Reserved = 0x03,
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B2C98995-5EB2-4fb1-B406-F3E8E2026A9A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAtscPsipParser
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
          [In] IntPtr pwProgramNumber,
          [Out] out IPMT ppPMT
          );

        [PreserveSig]
        int GetTSDT([Out] out ITSDT ppTSDT);

        [PreserveSig]
        int GetMGT([Out] out IATSC_MGT ppMGT);

        [PreserveSig]
        int GetVCT(
          [In] byte tableId,
          [In, MarshalAs(UnmanagedType.Bool)] bool fGetNextTable,
          [Out] out IATSC_VCT ppVCT
          );

        [PreserveSig]
        int GetEIT(
          [In] short pid,
          [In] IntPtr pwSourceId,
          [In] int dwTimeout,
          [Out] out IATSC_EIT ppEIT
          );

        [PreserveSig]
        int GetETT(
          [In] short pid,
          [In] IntPtr wSourceId,
          [In] IntPtr pwEventId,
          [Out] out IATSC_ETT ppETT
          );

        [PreserveSig]
        int GetSTT([Out] out IATSC_STT ppSTT);

        [PreserveSig]
        int GetEAS(
          [In] short pid,
          [Out] out ISCTE_EAS ppEAS
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8877dabd-c137-4073-97e3-779407a5d87a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IATSC_MGT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetProtocolVersion([Out] out byte pbVal);

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordType(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordTypePid(
          [In] int dwRecordIndex,
          [Out] out short ppidVal
          );

        [PreserveSig]
        int GetRecordVersionNumber(
          [In] int dwRecordIndex,
          [Out] out byte pbVal
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
        int GetCountOfTableDescriptors([In, Out] ref int pdwVal);

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
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("26879a18-32f9-46c6-91f0-fb6479270e8c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IATSC_VCT
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
        int GetProtocolVersion([Out] out byte pbVal);

        [PreserveSig]
        int GetCountOfRecords([Out] out int pdwVal);

        [PreserveSig]
        int GetRecordName(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.LPWStr)] out string pwsName);

        [PreserveSig]
        int GetRecordMajorChannelNumber(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordMinorChannelNumber(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordModulationMode(
          [In] int dwRecordIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordCarrierFrequency(
          [In] int dwRecordIndex,
          [Out] out int pdwVal
          );

        [PreserveSig]
        int GetRecordTransportStreamId(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordProgramNumber(
          [In] int dwRecordIndex,
          [Out] out short pwVal
          );

        [PreserveSig]
        int GetRecordEtmLocation(
          [In] int dwRecordIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordIsAccessControlledBitSet(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordIsHiddenBitSet(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordIsPathSelectBitSet(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordIsOutOfBandBitSet(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordIsHideGuideBitSet(
          [In] int dwRecordIndex,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal
          );

        [PreserveSig]
        int GetRecordServiceType(
          [In] int dwRecordIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordSourceId(
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
        int GetCountOfTableDescriptors([In, Out] ref int pdwVal);

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
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("d7c212d7-76a2-4b4b-aa56-846879a80096"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IATSC_EIT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetSourceId([Out] out short pwVal);

        [PreserveSig]
        int GetProtocolVersion([Out] out byte pbVal);

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
        int GetRecordEtmLocation(
          [In] int dwRecordIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordDuration(
          [In] int dwRecordIndex,
          [Out] out MpegDuration pmdVal
          );

        [PreserveSig]
        int GetRecordTitleText(
          [In] int dwRecordIndex,
          [Out] out int pdwLength,
          [Out] out IntPtr ppText
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
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("5a142cc9-b8cf-4a86-a040-e9cadf3ef3e7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IATSC_ETT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetProtocolVersion([Out] out byte pbVal);

        [PreserveSig]
        int GetEtmId([Out] out int pdwVal);

        int GetExtendedMessageText(
          [Out] out int pdwLength,
          [Out] out IntPtr ppText
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6bf42423-217d-4d6f-81e1-3a7b360ec896"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IATSC_STT
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetProtocolVersion([Out] out byte pbVal);

        [PreserveSig]
        int GetSystemTime([Out] out MpegDateAndTime pmdtSystemTime);

        [PreserveSig]
        int GetGpsUtcOffset([Out] out byte pbVal);

        [PreserveSig]
        int GetDaylightSavings([Out] out short pwVal);

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
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1FF544D6-161D-4fae-9FAA-4F9F492AE999"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISCTE_EAS
    {
        [PreserveSig]
        int Initialize(
          [In] ISectionList pSectionList,
          [In] IMpeg2Data pMPEGData
          );

        [PreserveSig]
        int GetVersionNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetSequencyNumber([Out] out byte pbVal);

        [PreserveSig]
        int GetProtocolVersion([Out] out byte pbVal);

        [PreserveSig]
        int GetEASEventID([Out] out short pwVal);

        [PreserveSig]
        int GetOriginatorCode([Out] out byte pbVal);

        [PreserveSig]
        int GetEASEventCodeLen([Out] out byte pbVal);

        [PreserveSig]
        int GetEASEventCode([Out] out byte pbVal);

        [PreserveSig]
        int GetRawNatureOfActivationTextLen([Out] out byte pbVal);

        [PreserveSig]
        int GetRawNatureOfActivationText([Out] out byte pbVal);

        [PreserveSig]
        int GetNatureOfActivationText(
          [In, MarshalAs(UnmanagedType.BStr)] string bstrIS0639code,
          [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrString
          );

        [PreserveSig]
        int GetTimeRemaining([Out] out  byte pbVal);

        [PreserveSig]
        int GetStartTime([Out] out int pdwVal);

        [PreserveSig]
        int GetDuration([Out] out short pwVal);

        [PreserveSig]
        int GetAlertPriority([Out] out byte pbVal);

        [PreserveSig]
        int GetDetailsOOBSourceID([Out] out short pwVal);

        [PreserveSig]
        int GetDetailsMajor([Out] out short pwVal);

        [PreserveSig]
        int GetDetailsMinor([Out] out short pwVal);

        [PreserveSig]
        int GetDetailsAudioOOBSourceID([Out] out short pwVal);

        [PreserveSig]
        int GetAlertText(
          [In, MarshalAs(UnmanagedType.BStr)] string bstrIS0639code,
          [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrString
          );

        [PreserveSig]
        int GetRawAlertTextLen([Out] out short pwVal);

        [PreserveSig]
        int GetRawAlertText([Out] out byte pbVal);

        [PreserveSig]
        int GetLocationCount([Out] out byte pbVal);

        [PreserveSig]
        int GetLocationCodes(
          [In] byte bIndex,
          [Out] out byte pbState,
          [Out] out byte pbCountySubdivision,
          [Out] out short pwCounty
          );

        [PreserveSig]
        int GetExceptionCount([Out] out byte pbVal);

        [PreserveSig]
        int GetExceptionService(
          [In] byte bIndex,
          [Out] out byte pbIBRef,
          [Out] out byte pwFirst,
          [Out] out short pwSecond
          );

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
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FF76E60C-0283-43ea-BA32-B422238547EE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAtscContentAdvisoryDescriptor
    {
        [PreserveSig]
        int GetTag([Out] out byte pbVal);

        [PreserveSig]
        int GetLength([Out] out byte pbVal);

        [PreserveSig]
        int GetRatingRegionCount([Out] out byte pbVal);

        [PreserveSig]
        int GetRecordRatingRegion(
          [In] byte bIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordRatedDimensions(
          [In] byte bIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordRatingDimension(
          [In] byte bIndexOuter,
          [In] byte bIndexInner,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordRatingValue(
          [In] byte bIndexOuter,
          [In] byte bIndexInner,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetRecordRatingDescriptionText(
          [In] byte bIndex,
          [Out] out byte pbLength,
          [Out] out IntPtr ppText
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("40834007-6834-46f0-BD45-D5F6A6BE258C"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICaptionServiceDescriptor
    {
        [PreserveSig]
        int GetNumberOfServices([Out] out byte pbVal);

        [PreserveSig]
        int GetLanguageCode(
          [In] byte bIndex,
          [Out] out int LangCode // probably a byte[3]
          );

        [PreserveSig]
        int GetCaptionServiceNumber(
          [In] byte bIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetCCType(
          [In] byte bIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetEasyReader(
          [In] byte bIndex,
          [Out] out byte pbVal
          );

        [PreserveSig]
        int GetWideAspectRatio(
          [In] byte bIndex,
          [Out] out byte pbVal
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("58C3C827-9D91-4215-BFF3-820A49F0904C"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceLocationDescriptor
    {
        [PreserveSig]
        int GetPCR_PID( 
            out short pwVal
            );
        
        [PreserveSig]
        int GetNumberOfElements( 
            out byte pbVal
            );
        
        [PreserveSig]
        int  GetElementStreamType( 
            byte bIndex,
            out byte pbVal
            );
        
        [PreserveSig]
        int  GetElementPID( 
            byte bIndex,
            out short pwVal
            );
        
        [PreserveSig]
        int  GetElementLanguageCode( 
            byte bIndex,
            [MarshalAs(UnmanagedType.LPArray, SizeConst=3)] out byte[] LangCode
            );
        
    };
    
#endif

    #endregion
}
