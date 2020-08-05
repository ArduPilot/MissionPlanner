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

#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace DirectShowLib.BDA
{
    #region Declarations

    /// <summary>
    /// From CLSID_TIFLoad
    /// </summary>
    [ComImport, Guid("14EB8748-1753-4393-95AE-4F7E7A87AAD6")]
    public class TIFLoad
    {
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DFEF4A68-EE61-415f-9CCB-CD95F2F98A3A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_TIF_REGISTRATION
    {
        [PreserveSig]
        int RegisterTIFEx(
          [In] IPin pTIFInputPin,
          [Out] out int ppvRegistrationContext,
          [Out, MarshalAs(UnmanagedType.Interface)] out object ppMpeg2DataControl
          );

        [PreserveSig]
        int UnregisterTIF([In] int pvRegistrationContext);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F9BAC2F9-4149-4916-B2EF-FAA202326862"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMPEG2_TIF_CONTROL
    {
        [PreserveSig]
        int RegisterTIF(
            [In, MarshalAs(UnmanagedType.Interface)] object pUnkTIF,
            [In, Out] ref int ppvRegistrationContext
            );

        [PreserveSig]
        int UnregisterTIF([In] int pvRegistrationContext);

        [PreserveSig]
        int AddPIDs(
            [In] int ulcPIDs,
            [In] ref int pulPIDs
            );

        [PreserveSig]
        int DeletePIDs(
            [In] int ulcPIDs,
            [In] ref int pulPIDs
            );

        [PreserveSig]
        int GetPIDCount([Out] out int pulcPIDs);

        [PreserveSig]
        int GetPIDs(
            [Out] out int pulcPIDs,
            [Out] out int pulPIDs
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A3B152DF-7A90-4218-AC54-9830BEE8C0B6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITuneRequestInfo
    {
        [PreserveSig]
        int GetLocatorData([In] ITuneRequest Request);

        [PreserveSig]
        int GetComponentData([In] ITuneRequest CurrentRequest);

        [PreserveSig]
        int CreateComponentList([In] ITuneRequest CurrentRequest);

        [PreserveSig]
        int GetNextProgram(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        int GetPreviousProgram(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        int GetNextLocator(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        int GetPreviousLocator(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EFDA0C80-F395-42c3-9B3C-56B37DEC7BB7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGuideDataEvent
    {
        [PreserveSig]
        int GuideDataAcquired();

        [PreserveSig]
        int ProgramChanged([In] object varProgramDescriptionID);

        [PreserveSig]
        int ServiceChanged([In] object varProgramDescriptionID);

        [PreserveSig]
        int ScheduleEntryChanged([In] object varProgramDescriptionID);

        [PreserveSig]
        int ProgramDeleted([In] object varProgramDescriptionID);

        [PreserveSig]
        int ServiceDeleted([In] object varProgramDescriptionID);

        [PreserveSig]
        int ScheduleDeleted([In] object varProgramDescriptionID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("88EC5E58-BB73-41d6-99CE-66C524B8B591"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGuideDataProperty
    {
        [PreserveSig]
        int get_Name([Out, MarshalAs(UnmanagedType.BStr)] out string pbstrName);

        [PreserveSig]
        int get_Language([Out] out int idLang);

        [PreserveSig]
        int get_Value([Out] out object pvar);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("AE44423B-4571-475c-AD2C-F40A771D80EF"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumGuideDataProperties
    {
        [PreserveSig]
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] IGuideDataProperty [] ppprop,
            [In] IntPtr pcelt
            );

        [PreserveSig]
        int Skip([In] int celt);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumGuideDataProperties ppenum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1993299C-CED6-4788-87A3-420067DCE0C7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumTuneRequests
    {
        [PreserveSig]
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] ITuneRequest[] ppprop,
            [In] IntPtr pcelt
            );

        [PreserveSig]
        int Skip([In] int celt);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumTuneRequests ppenum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("61571138-5B01-43cd-AEAF-60B784A0BF93"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGuideData
    {
        [PreserveSig]
        int GetServices([Out] out IEnumTuneRequests ppEnumTuneRequests);

        [PreserveSig]
        int GetServiceProperties(
            [In] ITuneRequest pTuneRequest,
            [Out] out IEnumGuideDataProperties ppEnumProperties
            );

        [PreserveSig]
#if USING_NET11
        int GetGuideProgramIDs([Out] out UCOMIEnumVARIANT pEnumPrograms);
#else
        int GetGuideProgramIDs([Out] out IEnumVARIANT pEnumPrograms);
#endif

        [PreserveSig]
        int GetProgramProperties(
            [In] object varProgramDescriptionID,
            [Out] out IEnumGuideDataProperties ppEnumProperties
            );

        [PreserveSig]
#if USING_NET11
        int GetScheduleEntryIDs([Out] out UCOMIEnumVARIANT pEnumScheduleEntries);
#else
        int GetScheduleEntryIDs([Out] out IEnumVARIANT pEnumScheduleEntries);
#endif

        [PreserveSig]
        int GetScheduleEntryProperties(
            [In] object varScheduleEntryDescriptionID,
            [Out] out IEnumGuideDataProperties ppEnumProperties
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4764ff7c-fa95-4525-af4d-d32236db9e38"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGuideDataLoader
    {
        [PreserveSig]
        int Init([In] IGuideData pGuideStore);

        [PreserveSig]
        int Terminate();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EE957C52-B0D0-4e78-8DD1-B87A08BFD893"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITuneRequestInfoEx : ITuneRequestInfo
    {
        #region ITuneRequestInfo Methods

        [PreserveSig]
        new int GetLocatorData(
            [In] ITuneRequest Request
            );

        [PreserveSig]
        new int GetComponentData(
            [In] ITuneRequest CurrentRequest
            );

        [PreserveSig]
        new int CreateComponentList(
            [In] ITuneRequest CurrentRequest
            );

        [PreserveSig]
        new int GetNextProgram(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        new int GetPreviousProgram(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        new int GetNextLocator(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        [PreserveSig]
        new int GetPreviousLocator(
            [In] ITuneRequest CurrentRequest,
            [Out] out ITuneRequest TuneRequest
            );

        #endregion

        [PreserveSig]
        int CreateComponentListEx (
            ITuneRequest CurrentRequest,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppCurPMT
        );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("7E47913A-5A89-423d-9A2B-E15168858934"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISIInbandEPGEvent
    {
        [PreserveSig]
        int SIObjectEvent(
            IDVB_EIT2 pIDVB_EIT,
            int dwTable_ID,
            int dwService_ID
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F90AD9D0-B854-4b68-9CC1-B2CC96119D85"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISIInbandEPG
    {
        [PreserveSig]
        int StartSIEPGScan();

        [PreserveSig]
        int StopSIEPGScan();

        [PreserveSig]
        int IsSIEPGScanRunning(
            [MarshalAs(UnmanagedType.Bool)] out bool bRunning
            );
    }

#endif

    #endregion
}
