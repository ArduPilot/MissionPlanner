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
#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace DirectShowLib.BDA
{
    #region Declarations

    [ComImport, Guid("5FFDC5E6-B83A-4b55-B6E8-C69E765FE9DB")]
    public class TuningSpace
    {
    }

    [ComImport, Guid("8A674B4C-1F63-11d3-B64C-00C04F79498E")]
    public class AnalogRadioTuningSpace
    {
    }

    [ComImport, Guid("F9769A06-7ACA-4e39-9CFB-97BB35F0E77E")]
    public class AuxInTuningSpace
    {
    }

    [ComImport, Guid("8A674B4D-1F63-11d3-B64C-00C04F79498E")]
    public class AnalogTVTuningSpace
    {
    }

    [ComImport, Guid("1BE49F30-0E1B-11d3-9D8E-00C04F72D980")]
    public class LanguageComponentType
    {
    }

    [ComImport, Guid("418008F3-CF67-4668-9628-10DC52BE1D08")]
    public class MPEG2ComponentType
    {
    }

    [ComImport, Guid("A8DCF3D5-0780-4ef4-8A83-2CFFAACB8ACE")]
    public class ATSCComponentType
    {
    }

    [ComImport, Guid("809B6661-94C4-49e6-B6EC-3F0F862215AA")]
    public class Components
    {
    }

    [ComImport, Guid("59DC47A8-116C-11d3-9D8E-00C04F72D980")]
    public class Component
    {
    }

    [ComImport, Guid("055CB2D7-2969-45cd-914B-76890722F112")]
    public class MPEG2Component
    {
    }

    [ComImport, Guid("B46E0D38-AB35-4a06-A137-70576B01B39F")]
    public class TuneRequest
    {
    }

    [ComImport, Guid("0369B4E5-45B6-11d3-B650-00C04F79498E")]
    public class ChannelTuneRequest
    {
    }

    [ComImport, Guid("0369B4E6-45B6-11d3-B650-00C04F79498E")]
    public class ATSCChannelTuneRequest
    {
    }

    [ComImport, Guid("0955AC62-BF2E-4cba-A2B9-A63F772D46CF")]
    public class MPEG2TuneRequest
    {
    }

    [ComImport, Guid("0888C883-AC4F-4943-B516-2C38D9B34562")]
    public class Locator
    {
    }

    [ComImport, Guid("8872FF1B-98FA-4d7a-8D93-C9F1055F85BB")]
    public class ATSCLocator
    {
    }

    [ComImport, Guid("C531D9FD-9685-4028-8B68-6E1232079F1E")]
    public class DVBCLocator
    {
    }

    [ComImport, Guid("15D6504A-5494-499c-886C-973C9E53B9F1")]
    public class DVBTuneRequest
    {
    }

    [ComImport, Guid("A1A2B1C4-0E3A-11d3-9D8E-00C04F72D980")]
    public class ComponentTypes
    {
    }

    [ComImport, Guid("823535A0-0318-11d3-9D8E-00C04F72D980")]
    public class ComponentType
    {
    }

    [ComImport, Guid("A2E30750-6C3D-11d3-B653-00C04F79498E")]
    public class ATSCTuningSpace
    {
    }

    [ComImport, Guid("C6B14B32-76AA-4a86-A7AC-5C79AAF58DA7")]
    public class DVBTuningSpace
    {
    }

    [ComImport, Guid("B64016F3-C9A2-4066-96F0-BD9563314726")]
    public class DVBSTuningSpace
    {
    }

    [ComImport, Guid("9CD64701-BDF3-4d14-8E03-F12983D86664")]
    public class DVBTLocator
    {
    }

    [ComImport, Guid("1DF7D126-4050-47f0-A7CF-4C4CA9241333")]
    public class DVBSLocator
    {
    }

    [ComImport, Guid("8A674B49-1F63-11d3-B64C-00C04F79498E")]
    public class CreatePropBagOnRegKey
    {
    }

    [ComImport, Guid("D02AAC50-027E-11d3-9D8E-00C04F72D980")]
    public class SystemTuningSpaces
    {
    }

    [ComImport, Guid("2C63E4EB-4CEA-41b8-919C-E947EA19A77C")]
    public class MPEG2TuneRequestFactory
    {
    }

    [ComImport, Guid("CC829A2F-3365-463f-AF13-81DBB6F3A555")]
    public class ChannelIDTuningSpace
    {
    }

    [ComImport, Guid("03C06416-D127-407A-AB4C-FDD279ABBE5D")]
    public class DigitalCableLocator
    {
    }

    [ComImport, Guid("E77026B0-B97F-4cbb-B7FB-F4F03AD69F11")]
    public class PersistTuneXmlUtility
    {
    }

    [ComImport, Guid("26EC0B63-AA90-458A-8DF4-5659F2C8A18A")]
    public class DigitalCableTuneRequest
    {
    }

    [ComImport, Guid("D9BB4CEE-B87A-47F1-AC92-B08D9C7813FC")]
    public class DigitalCableTuningSpace
    {
    }

    [ComImport, Guid("28AB0005-E845-4FFA-AA9B-F4665236141C")]
    public class AnalogAudioComponentType
    {
    }

    [ComImport, Guid("49638B91-48AB-48B7-A47A-7D0E75A08EDE")]
    public class AnalogLocator
    {
    }

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, Guid("6E50CC0D-C19B-4BF6-810B-5BD60761F5CC")]
    public class DigitalLocator
    {
    }

    [ComImport, Guid("3A9428A7-31A4-45e9-9EFB-E055BF7BB3DB")]
    public class ChannelIDTuneRequest
    {
    }

    [ComImport, Guid("6504AFED-A629-455c-A7F1-04964DEA5CC4")]
    public class ISDBSLocator
    {
    }

    [ComImport, Guid("6438570B-0C08-4a25-9504-8012BB4D50CF")]
    public class TunerMarshaler
    {
    }

    [ComImport, Guid("C20447FC-EC60-475e-813F-D2B0A6DECEFE")]
    public class ESEventService
    {
    }

    [ComImport, Guid("8E8A07DA-71F8-40c1-A929-5E3A868AC2C6")]
    public class ESEventFactory
    {
    }
    
    /// <summary>
    /// From TunerLockType
    /// </summary>
    public enum TunerLockType
    {
        None = 0x00,
        WithinScanSensingRange = 0x01,
        Locked = 0x02
    }

    /// <summary>
    /// From LNB_Source
    /// </summary>
    public enum LNB_Source
    {
        NOT_SET = -1,
        NOT_DEFINED = 0,
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        MAX
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BAD7753B-6B37-4810-AE57-3CE0C4A9E6CB"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableTuneRequest : IATSCChannelTuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        #region IChannelTuneRequest Methods

        [PreserveSig]
        new int get_Channel([Out] out int Channel);

        [PreserveSig]
        new int put_Channel([In] int Channel);

        #endregion

        #region IATSCChannelTuneRequest Methods

        [PreserveSig]
        new int get_MinorChannel([Out] out int MinorChannel);

        [PreserveSig]
        new int put_MinorChannel([In] int MinorChannel);

        #endregion

        [PreserveSig]
        int get_MajorChannel([Out] out int pMajorChannel);

        [PreserveSig]
        int put_MajorChannel([In] int MajorChannel);

        [PreserveSig]
        int get_SourceID([Out] out int pSourceID);

        [PreserveSig]
        int put_SourceID([In] int SourceID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("04BBD195-0E2D-4593-9BD5-4F908BC33CF5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IScanningTunerEx : IScanningTuner
    {
        #region ITuner Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int put_TuningSpace([In] ITuningSpace TuningSpace);

        [PreserveSig]
        new int EnumTuningSpaces([Out] out IEnumTuningSpaces ppEnum);

        [PreserveSig]
        new int get_TuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int put_TuneRequest([In] ITuneRequest TuneRequest);

        [PreserveSig]
        new int Validate([In] ITuneRequest TuneRequest);

        [PreserveSig]
        new int get_PreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_PreferredComponentTypes([In] IComponentTypes ComponentTypes);

        [PreserveSig]
        new int get_SignalStrength([Out] out int Strength);

        [PreserveSig]
        new int TriggerSignalEvents([In] int Interval);

        #endregion

        #region IScanningTuner Methods

        [PreserveSig]
        new int SeekUp();

        [PreserveSig]
        new int SeekDown();

        [PreserveSig]
        new int ScanUp([In] int MillisecondsPause);

        [PreserveSig]
        new int ScanDown([Out] out int MillisecondsPause);

        [PreserveSig]
        new int AutoProgram();

        #endregion

        [PreserveSig]
        int GetCurrentLocator([Out] out ILocator pILocator);

        [PreserveSig]
        int PerformExhaustiveScan(
          [In] int dwLowerFreq,
          [In] int dwHigherFreq,
          [In, MarshalAs(UnmanagedType.VariantBool)] bool bFineTune,
          [In] IntPtr hEvent
          );

        [PreserveSig]
        int TerminateCurrentScan([Out] out int pcurrentFreq);

        [PreserveSig]
        int ResumeCurrentScan([In] IntPtr hEvent);

        [PreserveSig]
        int GetTunerScanningCapability(
          [Out, MarshalAs(UnmanagedType.Bool)] out bool HardwareAssistedScanning,
          [In, Out] ref int NumStandardsSupported,
          [In, Out] Guid[] BroadcastStandards
          );

        [PreserveSig]
        int GetTunerStatus(
          [Out] out int SecondsLeft,
          [Out] out TunerLockType CurrentLockType,
          [Out, MarshalAs(UnmanagedType.Bool)] out bool AutoDetect,
          [Out] out int CurrentFreq
          );

        [PreserveSig]
        int GetCurrentTunerStandardCapability(
          [In] Guid CurrentBroadcastStandard,
          [Out] out int SettlingTime,
          [Out] out AnalogVideoStandard TvStandardsSupported
          );

        [PreserveSig]
        int SetScanSignalTypeFilter(
          [In] ScanModulationTypes ScanModulationTypes,
          [In] AnalogVideoStandard AnalogVideoStandard
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("359B3901-572C-4854-BB49-CDEF66606A25"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRegisterTuner
    {
        int Register(
            ITuner pTuner,
            IGraphBuilder pGraph
            );

        int Unregister();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1DFD0A5C-0284-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IScanningTuner : ITuner
    {
        #region ITuner Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int put_TuningSpace([In] ITuningSpace TuningSpace);

        [PreserveSig]
        new int EnumTuningSpaces([Out] out IEnumTuningSpaces ppEnum);

        [PreserveSig]
        new int get_TuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int put_TuneRequest([In] ITuneRequest TuneRequest);

        [PreserveSig]
        new int Validate([In] ITuneRequest TuneRequest);

        [PreserveSig]
        new int get_PreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_PreferredComponentTypes([In] IComponentTypes ComponentTypes);

        [PreserveSig]
        new int get_SignalStrength([Out] out int Strength);

        [PreserveSig]
        new int TriggerSignalEvents([In] int Interval);

        #endregion

        [PreserveSig]
        int SeekUp();

        [PreserveSig]
        int SeekDown();

        [PreserveSig]
        int ScanUp([In] int MillisecondsPause);

        [PreserveSig]
        int ScanDown([Out] out int MillisecondsPause);

        [PreserveSig]
        int AutoProgram();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6044634A-1733-4F99-B982-5FB12AFCE4F0"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBSLocator2 : IDVBSLocator
    {
        [DispId(1)]
        int CarrierFrequency { 
            [PreserveSig, DispId(1)] get; 
            [param: In] [PreserveSig, DispId(1)] set; 
        }

        [DispId(2)]
        FECMethod InnerFEC { 
            [PreserveSig, DispId(2)] get; 
            [param: In] [PreserveSig, DispId(2)] set; 
        }

        [DispId(3)]
        BinaryConvolutionCodeRate InnerFECRate { 
            [PreserveSig, DispId(3)] get; 
            [param: In] [PreserveSig, DispId(3)] set; 
        }

        [DispId(4)]
        FECMethod OuterFEC { 
            [PreserveSig, DispId(4)] get; 
            [param: In] [PreserveSig, DispId(4)] set; 
        }

        [DispId(5)]
        BinaryConvolutionCodeRate OuterFECRate { 
            [PreserveSig, DispId(5)] get; 
            [param: In] [PreserveSig, DispId(5)] set; 
        }

        [DispId(6)]
        ModulationType Modulation { 
            [PreserveSig, DispId(6)] get; 
            [param: In] [PreserveSig, DispId(6)] set; 
        }

        [DispId(7)]
        int SymbolRate { 
            [PreserveSig, DispId(7)] get; 
            [param: In] [PreserveSig, DispId(7)] set; 
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        [PreserveSig, DispId(8)]
        ILocator Clone();

        [DispId(0x191)]
        Polarisation SignalPolarisation { 
            [PreserveSig, DispId(0x191)] get; 
            [param: In] [PreserveSig, DispId(0x191)] set; 
        }

        [DispId(0x192)]
        bool WestPosition { 
            [PreserveSig, DispId(0x192)] get; 
            [param: In] [PreserveSig, DispId(0x192)] set; 
        }

        [DispId(0x193)]
        int OrbitalPosition { 
            [PreserveSig, DispId(0x193)] get; 
            [param: In] [PreserveSig, DispId(0x193)] set; 
        }

        [DispId(0x194)]
        int Azimuth { 
            [PreserveSig, DispId(0x194)] get; 
            [param: In] [PreserveSig, DispId(0x194)] set; 
        }

        [DispId(0x195)]
        int Elevation { 
            [PreserveSig, DispId(0x195)] get; 
            [param: In] [PreserveSig, DispId(0x195)] set; 
        }

        [DispId(0x196)]
        LNB_Source DiseqLNBSource { 
            [PreserveSig, DispId(0x196)] get; 
            [param: In] [PreserveSig, DispId(0x196)] set; 
        }

        [DispId(0x197)]
        int LocalOscillatorOverrideLow { 
            [PreserveSig, DispId(0x197)] get; 
            [param: In] [PreserveSig, DispId(0x197)] set; 
        }

        [DispId(0x198)]
        int LocalOscillatorOverrideHigh { 
            [PreserveSig, DispId(0x198)] get; 
            [param: In] [PreserveSig, DispId(0x198)] set; 
        }

        [DispId(0x199)]
        int LocalLNBSwitchOverride { 
            [PreserveSig, DispId(0x199)] get; 
            [param: In] [PreserveSig, DispId(0x199)] set; 
        }

        [DispId(410)]
        SpectralInversion LocalSpectralInversionOverride { 
            [PreserveSig, DispId(410)] get; 
            [param: In] [PreserveSig, DispId(410)] set; 
        }

        [DispId(0x19b)]
        RollOff SignalRollOff { 
            [PreserveSig, DispId(0x19b)] get; 
            [param: In] [PreserveSig, DispId(0x19b)] set; 
        }

        [DispId(0x19c)]
        Pilot SignalPilot { 
            [PreserveSig, DispId(0x19c)] get; 
            [param: In] [PreserveSig, DispId(0x19c)] set; 
        }

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6B80E96F-55E2-45AA-B754-0C23C8E7D5C1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESCloseMmiEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        int GetDialogNumber();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1F0E5357-AF43-44E6-8547-654C645145D2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESEvent
    {
        [PreserveSig]
        int GetEventId();

        [PreserveSig]
        Guid GetEventType();

        [PreserveSig]
        int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        string GetStringData();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("506A09B8-7F86-4E04-AC05-3303BFE8FC49"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESEventFactory
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [PreserveSig]
        IESEvent CreateESEvent([In, MarshalAs(UnmanagedType.IUnknown)] object pServiceProvider, [In] int dwEventId, [In] Guid guidEventType, [In] int dwEventDataLength, [In] ref byte pEventData, [In, MarshalAs(UnmanagedType.BStr)] string bstrBaseUrl, [In, MarshalAs(UnmanagedType.IUnknown)] object pInitContext);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ABD414BF-CFE5-4E5E-AF5B-4B4E49C5BFEB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESEvents
    {
        [PreserveSig]
        int OnESEventReceived([In] Guid guidEventType, [In, MarshalAs(UnmanagedType.Interface)] IESEvent pESEvent);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ED89A619-4C06-4B2F-99EB-C7669B13047C"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESEventService
    {
        [PreserveSig]
        int FireESEvent([MarshalAs(UnmanagedType.Interface)] IESEvent pESEvent);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("33B9DAAE-9309-491D-A051-BCAD2A70CD66"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType((short)0x90)]
    public interface IESEventServiceConfiguration
    {
        [PreserveSig]
        int SetParent([MarshalAs(UnmanagedType.Interface)] IESEventService pEventService);

        [PreserveSig]
        int RemoveParent();

        [PreserveSig]
        int SetOwner([MarshalAs(UnmanagedType.Interface)] IESEvents pESEvents);

        [PreserveSig]
        int RemoveOwner();

        [PreserveSig]
        int SetGraph([MarshalAs(UnmanagedType.Interface)] IFilterGraph pGraph);

        [PreserveSig]
        int RemoveGraph([MarshalAs(UnmanagedType.Interface)] IFilterGraph pGraph);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BA9EDCB6-4D36-4CFE-8C56-87A6B0CA48E1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESFileExpiryDateEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        Guid GetTunerId();

        [PreserveSig]
        long GetExpiryDate();

        [PreserveSig]
        long GetFinalExpiryDate();

        [PreserveSig]
        int GetMaxRenewalCount();

        [PreserveSig]
        int IsEntitlementTokenPresent();

        [PreserveSig]
        int DoesExpireAfterFirstUse();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2017CB03-DC0F-4C24-83CA-36307B2CD19F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESIsdbCasResponseEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        int GetRequestId();

        [PreserveSig]
        int GetStatus();

        [PreserveSig]
        int GetDataLength();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        byte[] GetResponseData();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D5A48EF5-A81B-4DF0-ACAA-5E35E7EA45D4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESLicenseRenewalResultEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        int GetCallersId();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        string GetFileName();

        [PreserveSig]
        int IsRenewalSuccessful();

        [PreserveSig]
        int IsCheckEntitlementCallRequired();

        [PreserveSig]
        int GetDescrambledStatus();

        [PreserveSig]
        int GetRenewalResultCode();

        [PreserveSig]
        int GetCASFailureCode();

        [return: MarshalAs(UnmanagedType.Error)]
        [PreserveSig]
        int GetRenewalHResult();

        [PreserveSig]
        int GetEntitlementTokenLength();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        byte[] GetEntitlementToken();

        [PreserveSig]
        long GetExpiryDate();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BA4B6526-1A35-4635-8B56-3EC612746A8C"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESOpenMmiEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        int GetDialogNumber(out int pDialogRequest);

        [PreserveSig]
        Guid GetDialogType();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        byte[] GetDialogData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        string GetDialogStringData([MarshalAs(UnmanagedType.BStr)] out string pbstrBaseUrl);

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("54C7A5E8-C3BB-4F51-AF14-E0E2C0E34C6D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESRequestTunerEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [PreserveSig]
        byte GetPriority();

        [PreserveSig]
        byte GetReason();

        [PreserveSig]
        byte GetConsequences();

        [PreserveSig]
        int GetEstimatedTime();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8A24C46E-BB63-4664-8602-5D9C718C146D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IESValueUpdatedEvent : IESEvent
    {
        #region IESEvent Methods

        [PreserveSig]
        new int GetEventId();

        [PreserveSig]
        new Guid GetEventType();

        [PreserveSig]
        new int SetCompletionStatus([In] int dwResult);

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
        [PreserveSig]
        new byte[] GetData();

        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        new string GetStringData();

        #endregion

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        [PreserveSig]
        string[] GetValueNames();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("907E0B5C-E42D-4F04-91F0-26F401F36907"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IGpnvsCommonBase
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        [PreserveSig]
        string GetValueUpdateName();

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C9897087-E29C-473F-9E4B-7072123DEA14"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IISDBSLocator : IDVBSLocator
    {
        [DispId(1)]
        int CarrierFrequency { 
            [PreserveSig, DispId(1)] get; 
            [param: In] [PreserveSig, DispId(1)] set; 
        }

        [DispId(2)]
        FECMethod InnerFEC { 
            [PreserveSig, DispId(2)] get; 
            [param: In] [PreserveSig, DispId(2)] set; 
        }

        [DispId(3)]
        BinaryConvolutionCodeRate InnerFECRate { 
            [PreserveSig, DispId(3)] get; 
            [param: In] [PreserveSig, DispId(3)] set; 
        }

        [DispId(4)]
        FECMethod OuterFEC { 
            [PreserveSig, DispId(4)] get; 
            [param: In] [PreserveSig, DispId(4)] set; 
        }

        [DispId(5)]
        BinaryConvolutionCodeRate OuterFECRate { 
            [PreserveSig, DispId(5)] get; 
            [param: In] [PreserveSig, DispId(5)] set; 
        }

        [DispId(6)]
        ModulationType Modulation { 
            [PreserveSig, DispId(6)] get; 
            [param: In] [PreserveSig, DispId(6)] set;
        }

        [DispId(7)]
        int SymbolRate { 
            [PreserveSig, DispId(7)] get; 
            [param: In] [PreserveSig, DispId(7)] set; 
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        [PreserveSig, DispId(8)]
        ILocator Clone();

        [DispId(0x191)]
        Polarisation SignalPolarisation { 
            [PreserveSig, DispId(0x191)] get; 
            [param: In] [PreserveSig, DispId(0x191)] set; 
        }

        [DispId(0x192)]
        bool WestPosition { 
            [PreserveSig, DispId(0x192)] get; 
            [param: In] [PreserveSig, DispId(0x192)] set; 
        }

        [DispId(0x193)]
        int OrbitalPosition { 
            [PreserveSig, DispId(0x193)] get; 
            [param: In] [PreserveSig, DispId(0x193)] set; 
        }

        [DispId(0x194)]
        int Azimuth { 
            [PreserveSig, DispId(0x194)] get; 
            [param: In] [PreserveSig, DispId(0x194)] set; 
        }

        [DispId(0x195)]
        int Elevation { 
            [PreserveSig, DispId(0x195)] get; 
            [param: In] [PreserveSig, DispId(0x195)] set; 
        }

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ED3E0C66-18C8-4EA6-9300-F6841FDD35DC"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITunerCapEx
    {
        [DispId(0x60010000)]
        bool Has608_708Caption { 
            [PreserveSig] get; 
        }

    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("448A2EDF-AE95-4b43-A3CC-747843C453D4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVBTLocator2 : IDVBTLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        #region IDVBTLocator Methods

        [PreserveSig]
        new int get_Bandwidth([Out] out int BandwidthVal);

        [PreserveSig]
        new int put_Bandwidth([In] int BandwidthVal);

        [PreserveSig]
        new int get_LPInnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_LPInnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_LPInnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_LPInnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_HAlpha([Out] out HierarchyAlpha Alpha);

        [PreserveSig]
        new int put_HAlpha([In] HierarchyAlpha Alpha);

        [PreserveSig]
        new int get_Guard([Out] out GuardInterval GI);

        [PreserveSig]
        new int put_Guard([In] GuardInterval GI);

        [PreserveSig]
        new int get_Mode([Out] out TransmissionMode mode);

        [PreserveSig]
        new int put_Mode([In] TransmissionMode mode);

        [PreserveSig]
        new int get_OtherFrequencyInUse([Out, MarshalAs(UnmanagedType.VariantBool)] out bool OtherFrequencyInUseVal);

        [PreserveSig]
        new int put_OtherFrequencyInUse([In, MarshalAs(UnmanagedType.VariantBool)] bool OtherFrequencyInUseVal);

        #endregion

        [PreserveSig]
        int get_PhysicalLayerPipeId( 
            out int PhysicalLayerPipeIdVal
            );
        
        [PreserveSig]
        int put_PhysicalLayerPipeId( 
            int PhysicalLayerPipeIdVal
            );        
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("061C6E30-E622-11d2-9493-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuningSpace
    {
        [PreserveSig]
        int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        int Clone([Out] out ITuningSpace NewTS);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("28C52640-018A-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITuner
    {
        [PreserveSig]
        int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        int put_TuningSpace([In] ITuningSpace TuningSpace);

        [PreserveSig]
        int EnumTuningSpaces([Out] out IEnumTuningSpaces ppEnum);

        [PreserveSig]
        int get_TuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        int put_TuneRequest([In] ITuneRequest TuneRequest);

        [PreserveSig]
        int Validate([In] ITuneRequest TuneRequest);

        [PreserveSig]
        int get_PreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        int put_PreferredComponentTypes([In] IComponentTypes ComponentTypes);

        [PreserveSig]
        int get_SignalStrength([Out] out int Strength);

        [PreserveSig]
        int TriggerSignalEvents([In] int Interval);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("07DDC146-FC3D-11d2-9D8C-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuneRequest
    {
        [PreserveSig]
        int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        int get_Components([Out] out IComponents Components);

        [PreserveSig]
        int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        int put_Locator([In] ILocator Locator);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3D7C353C-0D04-45f1-A742-F97CC1188DC8"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBSLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_SignalPolarisation([Out] out Polarisation PolarisationVal);

        [PreserveSig]
        int put_SignalPolarisation([In] Polarisation PolarisationVal);

        [PreserveSig]
        int get_WestPosition([Out, MarshalAs(UnmanagedType.VariantBool)] out bool WestLongitude);

        [PreserveSig]
        int put_WestPosition([In, MarshalAs(UnmanagedType.VariantBool)] bool WestLongitude);

        [PreserveSig]
        int get_OrbitalPosition([Out] out int longitude);

        [PreserveSig]
        int put_OrbitalPosition([In] int longitude);

        [PreserveSig]
        int get_Azimuth([Out] out int Azimuth);

        [PreserveSig]
        int put_Azimuth([In] int Azimuth);

        [PreserveSig]
        int get_Elevation([Out] out int Elevation);

        [PreserveSig]
        int put_Elevation([In] int Elevation);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("CDF7BE60-D954-42fd-A972-78971958E470"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBSTuningSpace : IDVBTuningSpace2
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IDVBTuningSpace Methods

        [PreserveSig]
        new int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        new int put_SystemType([In] DVBSystemType SysType);

        #endregion

        #region IDVBTuningSpace2 Methods

        [PreserveSig]
        new int get_NetworkID([Out] out int NetworkID);

        [PreserveSig]
        new int put_NetworkID([In] int NetworkID);

        #endregion

        [PreserveSig]
        int get_LowOscillator([Out] out int LowOscillator);

        [PreserveSig]
        int put_LowOscillator([In] int LowOscillator);

        [PreserveSig]
        int get_HighOscillator([Out] out int HighOscillator);

        [PreserveSig]
        int put_HighOscillator([In] int HighOscillator);

        [PreserveSig]
        int get_LNBSwitch([Out] out int LNBSwitch);

        [PreserveSig]
        int put_LNBSwitch([In] int LNBSwitch);

        [PreserveSig]
        int get_InputRange([Out, MarshalAs(UnmanagedType.BStr)] out string InputRange);

        [PreserveSig]
        int put_InputRange([Out, MarshalAs(UnmanagedType.BStr)] string InputRange);

        [PreserveSig]
        int get_SpectralInversion([Out] out SpectralInversion SpectralInversionVal);

        [PreserveSig]
        int put_SpectralInversion([In] SpectralInversion SpectralInversionVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("286D7F89-760C-4F89-80C4-66841D2507AA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ILocator
    {
        [PreserveSig]
        int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        int put_SymbolRate([In] int Rate);

        [PreserveSig]
        int Clone([Out] out ILocator NewLocator);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("ADA0B268-3B19-4e5b-ACC4-49F852BE13BA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        [PreserveSig]
        int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        int put_SystemType([In] DVBSystemType SysType);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("843188B4-CE62-43db-966B-8145A094E040"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuningSpace2 : IDVBTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IDVBTuningSpace Methods

        [PreserveSig]
        new int get_SystemType([Out] out DVBSystemType SysType);

        [PreserveSig]
        new int put_SystemType([In] DVBSystemType SysType);

        #endregion

        [PreserveSig]
        int get_NetworkID([Out] out int NetworkID);

        [PreserveSig]
        int put_NetworkID([In] int NetworkID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8B8EB248-FC2B-11d2-9D8C-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumTuningSpaces
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ITuningSpace[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumTuningSpaces ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    CLSCompliant(false), // because of _TuningSpacesForCLSID
    Guid("5B692E84-E2F1-11d2-9493-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuningSpaceContainer
    {
        [PreserveSig]
        int get_Count([Out] out int Count);

        [PreserveSig]
#if USING_NET11
        int get__NewEnum([Out] out UCOMIEnumVARIANT ppNewEnum);
#else
        int get__NewEnum([Out] out IEnumVARIANT ppNewEnum);
#endif

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out ITuningSpace TuningSpace
            );

        [PreserveSig]
        int put_Item(
            [In] object varIndex,
            [In] ITuningSpace TuningSpace
            );

        [PreserveSig]
        int TuningSpacesForCLSID(
            [In, MarshalAs(UnmanagedType.BStr)] string SpaceCLSID,
            [Out] out ITuningSpaces NewColl
            );

        [PreserveSig]
        int _TuningSpacesForCLSID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid SpaceCLSID,
            [Out] out ITuningSpaces NewColl
            );

        [PreserveSig]
        int TuningSpacesForName(
            [In, MarshalAs(UnmanagedType.BStr)] string Name,
            [Out] out ITuningSpaces NewColl
            );

        [PreserveSig]
        int FindID(
            [In] ITuningSpace TuningSpace,
            [Out] out int ID
            );

        [PreserveSig]
        int Add(
            [In] ITuningSpace TuningSpace,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int get_EnumTuningSpaces([Out] out IEnumTuningSpaces ppEnum);

        [PreserveSig]
        int Remove([In] object Index);

        [PreserveSig]
        int get_MaxCount([Out] out int MaxCount);

        [PreserveSig]
        int put_MaxCount([In] int MaxCount);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("901284E4-33FE-4b69-8D63-634A596F3756"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ITuningSpaces
    {
        [PreserveSig]
        int get_Count([Out] out int Count);

        [PreserveSig]
#if USING_NET11
        int get__NewEnum([Out] out UCOMIEnumVARIANT ppNewEnum);
#else
        int get__NewEnum([Out] out IEnumVARIANT ppNewEnum);
#endif

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out ITuningSpace TuningSpace
            );

        [PreserveSig]
        int get_EnumTuningSpaces([Out] out IEnumTuningSpaces NewEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0D6F567E-A636-42bb-83BA-CE4C1704AFA2"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_ONID([Out] out int ONID);

        [PreserveSig]
        int put_ONID([In] int ONID);

        [PreserveSig]
        int get_TSID([Out] out int TSID);

        [PreserveSig]
        int put_TSID([In] int TSID);

        [PreserveSig]
        int get_SID([Out] out int SID);

        [PreserveSig]
        int put_SID([In] int SID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8664DA16-DDA2-42ac-926A-C18F9127C302"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBTLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_Bandwidth([Out] out int BandwidthVal);

        [PreserveSig]
        int put_Bandwidth([In] int BandwidthVal);

        [PreserveSig]
        int get_LPInnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        int put_LPInnerFEC([In] FECMethod FEC);

        [PreserveSig]
        int get_LPInnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int put_LPInnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        int get_HAlpha([Out] out HierarchyAlpha Alpha);

        [PreserveSig]
        int put_HAlpha([In] HierarchyAlpha Alpha);

        [PreserveSig]
        int get_Guard([Out] out GuardInterval GI);

        [PreserveSig]
        int put_Guard([In] GuardInterval GI);

        [PreserveSig]
        int get_Mode([Out] out TransmissionMode mode);

        [PreserveSig]
        int put_Mode([In] TransmissionMode mode);

        [PreserveSig]
        int get_OtherFrequencyInUse([Out, MarshalAs(UnmanagedType.VariantBool)] out bool OtherFrequencyInUseVal);

        [PreserveSig]
        int put_OtherFrequencyInUse([In, MarshalAs(UnmanagedType.VariantBool)] bool OtherFrequencyInUseVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8A674B4A-1F63-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumComponentTypes
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IComponentType[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumComponentTypes ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6A340DC0-0311-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponentType
    {
        [PreserveSig]
        int get_Category(
            [Out] out ComponentCategory Category
            );

        [PreserveSig]
        int put_Category(
            [In] ComponentCategory Category
            );

        [PreserveSig]
        int get_MediaMajorType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType
            );

        [PreserveSig]
        int put_MediaMajorType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType
            );

        [PreserveSig]
        int get__MediaMajorType(
            [Out] out Guid MediaMajorType
            );

        [PreserveSig]
        int put__MediaMajorType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType
            );

        [PreserveSig]
        int get_MediaSubType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType
            );

        [PreserveSig]
        int put_MediaSubType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaSubType
            );

        [PreserveSig]
        int get__MediaSubType(
            [Out] out Guid MediaSubType
            );

        [PreserveSig]
        int put__MediaSubType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType
            );

        [PreserveSig]
        int get_MediaFormatType(
            [Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType
            );

        [PreserveSig]
        int put_MediaFormatType(
            [In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType
            );

        [PreserveSig]
        int get__MediaFormatType(
            [Out] out Guid MediaFormatType
            );

        [PreserveSig]
        int put__MediaFormatType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType
            );

        [PreserveSig]
        int get_MediaType(
            [Out] AMMediaType MediaType
            );

        [PreserveSig]
        int put_MediaType(
            [In] AMMediaType MediaType
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponentType NewCT
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0DC13D4A-0313-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponentTypes
    {
        [PreserveSig]
        int get_Count(
            [Out] out int Count
            );

        [PreserveSig]
        int get__NewEnum(
#if USING_NET11
            [Out] out UCOMIEnumVARIANT ppNewEnum
#else
[Out] out IEnumVARIANT ppNewEnum
#endif
);

        [PreserveSig]
        int EnumComponentTypes(
            [Out] out IEnumComponentTypes ppNewEnum
            );

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out IComponentType TuningSpace
            );

        [PreserveSig]
        int put_Item(
            [In] object NewIndex,
            [In] IComponentType ComponentType
            );

        [PreserveSig]
        int Add(
            [In] IComponentType ComponentType,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int Remove(
            [In] object Index
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponentTypes NewList
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("39A48091-FFFE-4182-A161-3FF802640E26"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponentsNew
    {
        [PreserveSig]
        int get_Count([Out] out int Count);

        [PreserveSig]
#if USING_NET11
        int get__NewEnum([Out] out UCOMIEnumVARIANT ppNewEnum);
#else
        int get__NewEnum([Out] out IEnumVARIANT ppNewEnum);
#endif

        [PreserveSig]
        int EnumComponents([Out] out IEnumComponents ppNewEnum);

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out IComponent TuningSpace
            );

        [PreserveSig]
        int Add(
            [In] IComponent Component,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int Remove([In] object Index);

        [PreserveSig]
        int Clone([Out] out IComponentsNew NewList);

        [PreserveSig]
        int put_Item(
            object Index,
            IComponent ppComponent
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2A6E2939-2595-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumComponents
    {
        int Next(
            [In] int celt,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IComponent[] rgelt,
            [In] IntPtr pceltFetched
            );

        int Skip([In] int celt);

        int Reset();

        int Clone([Out] out IEnumComponents ppEnum);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1A5576FC-0E19-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponent
    {
        [PreserveSig]
        int get_Type([Out] out IComponentType CT);

        [PreserveSig]
        int put_Type([In] IComponentType CT);

        [PreserveSig]
        int get_DescLangID([Out] out int LangID);

        [PreserveSig]
        int put_DescLangID([In] int LangID);

        [PreserveSig]
        int get_Status([Out] out ComponentStatus Status);

        [PreserveSig]
        int put_Status([In] ComponentStatus Status);

        [PreserveSig]
        int get_Description([Out, MarshalAs(UnmanagedType.BStr)] out string Description);

        [PreserveSig]
        int put_Description([In, MarshalAs(UnmanagedType.BStr)] string Description);

        [PreserveSig]
        int Clone([Out] out IComponent NewComponent);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EB7D987F-8A01-42ad-B8AE-574DEEE44D1A"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMPEG2TuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_TSID([Out] out int TSID);

        [PreserveSig]
        int put_TSID([In] int TSID);

        [PreserveSig]
        int get_ProgNo([Out] out int ProgNo);

        [PreserveSig]
        int put_ProgNo([In] int ProgNo);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("14E11ABD-EE37-4893-9EA1-6964DE933E39"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMPEG2TuneRequestFactory
    {
        [PreserveSig]
        int CreateTuneRequest(
            [In] ITuningSpace TuningSpace,
            [Out] out IMPEG2TuneRequest TuneRequest
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FCD01846-0E19-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IComponents
    {
        [PreserveSig]
        int get_Count(
            [Out] out int Count
            );

        [PreserveSig]
        int get__NewEnum(
#if USING_NET11
            [Out] out UCOMIEnumVARIANT ppNewEnum
#else
[Out] out IEnumVARIANT ppNewEnum
#endif
);

        [PreserveSig]
        int EnumComponents(
            [Out] out IEnumComponents ppNewEnum
            );

        [PreserveSig]
        int get_Item(
            [In] object varIndex,
            [Out] out IComponent TuningSpace
            );

        [PreserveSig]
        int Add(
            [In] IComponent Component,
            [Out] out object NewIndex
            );

        [PreserveSig]
        int Remove(
            [In] object Index
            );

        [PreserveSig]
        int Clone(
            [Out] out IComponents NewList
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2A6E293C-2595-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogTVTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        [PreserveSig]
        int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        int put_CountryCode([In] int NewCountryCodeVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0369B4E1-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCChannelTuneRequest : IChannelTuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        #region IChannelTuneRequest Methods

        [PreserveSig]
        new int get_Channel([Out] out int Channel);

        [PreserveSig]
        new int put_Channel([In] int Channel);

        #endregion

        [PreserveSig]
        int get_MinorChannel([Out] out int MinorChannel);

        [PreserveSig]
        int put_MinorChannel([In] int MinorChannel);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FC189E4D-7BD4-4125-B3B3-3A76A332CC96"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCComponentType : IMPEG2ComponentType
    {
        #region IComponentType Methods

        [PreserveSig]
        new int get_Category([Out] out ComponentCategory Category);

        [PreserveSig]
        new int put_Category([In] ComponentCategory Category);

        [PreserveSig]
        new int get_MediaMajorType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType);

        [PreserveSig]
        new int put_MediaMajorType([In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType);

        [PreserveSig]
        new int get__MediaMajorType([Out] out Guid MediaMajorType);

        [PreserveSig]
        new int put__MediaMajorType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType);

        [PreserveSig]
        new int get_MediaSubType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType);

        [PreserveSig]
        new int put_MediaSubType([In, MarshalAs(UnmanagedType.BStr)] string MediaSubType);

        [PreserveSig]
        new int get__MediaSubType([Out] out Guid MediaSubType);

        [PreserveSig]
        new int put__MediaSubType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType);

        [PreserveSig]
        new int get_MediaFormatType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType);

        [PreserveSig]
        new int put_MediaFormatType([In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType);

        [PreserveSig]
        new int get__MediaFormatType([Out] out Guid MediaFormatType);

        [PreserveSig]
        new int put__MediaFormatType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType);

        [PreserveSig]
        new int get_MediaType([Out] AMMediaType MediaType);

        [PreserveSig]
        new int put_MediaType([In] AMMediaType MediaType);

        [PreserveSig]
        new int Clone([Out] out IComponentType NewCT);

        #endregion

        #region ILanguageComponentType Methods

        [PreserveSig]
        new int get_LangID([Out] out int LangID);

        [PreserveSig]
        new int put_LangID([In] int LangID);

        #endregion

        #region IMPEG2ComponentType Methods

        [PreserveSig]
        new int get_StreamType([Out] out MPEG2StreamType MP2StreamType);

        [PreserveSig]
        new int put_StreamType([In] MPEG2StreamType MP2StreamType);

        #endregion

        [PreserveSig]
        int get_Flags([Out] out ATSCComponentTypeFlags Flags);

        [PreserveSig]
        int put_Flags([In] ATSCComponentTypeFlags Flags);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BF8D986F-8C2B-4131-94D7-4D3D9FCC21EF"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCLocator : IDigitalLocator
    {
        #region ILocator Methods
        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        int get_TSID([Out] out int TSID);

        [PreserveSig]
        int put_TSID([In] int TSID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0369B4E2-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCTuningSpace : IAnalogTVTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IAnalogTVTuningSpace Methods

        [PreserveSig]
        new int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        new int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        new int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        new int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        new int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        new int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        new int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        new int put_CountryCode([In] int NewCountryCodeVal);

        #endregion

        [PreserveSig]
        int get_MinMinorChannel([Out] out int MinMinorChannelVal);

        [PreserveSig]
        int put_MinMinorChannel([In] int NewMinMinorChannelVal);

        [PreserveSig]
        int get_MaxMinorChannel([Out] out int MaxMinorChannelVal);

        [PreserveSig]
        int put_MaxMinorChannel([In] int NewMaxMinorChannelVal);

        [PreserveSig]
        int get_MinPhysicalChannel([Out] out int MinPhysicalChannelVal);

        [PreserveSig]
        int put_MinPhysicalChannel([In] int NewMinPhysicalChannelVal);

        [PreserveSig]
        int get_MaxPhysicalChannel([Out] out int MaxPhysicalChannelVal);

        [PreserveSig]
        int put_MaxPhysicalChannel([In] int NewMaxPhysicalChannelVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0369B4E0-45B6-11d3-B650-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IChannelTuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_Channel([Out] out int Channel);

        [PreserveSig]
        int put_Channel([In] int Channel);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B874C8BA-0FA2-11d3-9D8E-00C04F72D980"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ILanguageComponentType : IComponentType
    {
        #region IComponentType Methods

        [PreserveSig]
        new int get_Category([Out] out ComponentCategory Category);

        [PreserveSig]
        new int put_Category([In] ComponentCategory Category);

        [PreserveSig]
        new int get_MediaMajorType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType);

        [PreserveSig]
        new int put_MediaMajorType([In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType);

        [PreserveSig]
        new int get__MediaMajorType([Out] out Guid MediaMajorType);

        [PreserveSig]
        new int put__MediaMajorType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType);

        [PreserveSig]
        new int get_MediaSubType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType);

        [PreserveSig]
        new int put_MediaSubType([In, MarshalAs(UnmanagedType.BStr)] string MediaSubType);

        [PreserveSig]
        new int get__MediaSubType([Out] out Guid MediaSubType);

        [PreserveSig]
        new int put__MediaSubType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType);

        [PreserveSig]
        new int get_MediaFormatType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType);

        [PreserveSig]
        new int put_MediaFormatType([In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType);

        [PreserveSig]
        new int get__MediaFormatType([Out] out Guid MediaFormatType);

        [PreserveSig]
        new int put__MediaFormatType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType);

        [PreserveSig]
        new int get_MediaType([Out] AMMediaType MediaType);

        [PreserveSig]
        new int put_MediaType([In] AMMediaType MediaType);

        [PreserveSig]
        new int Clone([Out] out IComponentType NewCT);

        #endregion

        [PreserveSig]
        int get_LangID([Out] out int LangID);

        [PreserveSig]
        int put_LangID([In] int LangID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1493E353-1EB6-473c-802D-8E6B8EC9D2A9"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMPEG2Component : IComponent
    {
        #region IComponent Methods

        [PreserveSig]
        new int get_Type([Out] out IComponentType CT);

        [PreserveSig]
        new int put_Type([In] IComponentType CT);

        [PreserveSig]
        new int get_DescLangID([Out] out int LangID);

        [PreserveSig]
        new int put_DescLangID([In] int LangID);

        [PreserveSig]
        new int get_Status([Out] out ComponentStatus Status);

        [PreserveSig]
        new int put_Status([In] ComponentStatus Status);

        [PreserveSig]
        new int get_Description([Out, MarshalAs(UnmanagedType.BStr)] out string Description);

        [PreserveSig]
        new int put_Description([In, MarshalAs(UnmanagedType.BStr)] string Description);

        [PreserveSig]
        new int Clone([Out] out IComponent NewComponent);

        #endregion

        [PreserveSig]
        int get_PID([Out] out int PID);

        [PreserveSig]
        int put_PID([In] int PID);

        [PreserveSig]
        int get_PCRPID([Out] out int PCRPID);

        [PreserveSig]
        int put_PCRPID([In] int PCRPID);

        [PreserveSig]
        int get_ProgramNumber([Out] out int ProgramNumber);

        [PreserveSig]
        int put_ProgramNumber([In] int ProgramNumber);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2C073D84-B51C-48c9-AA9F-68971E1F6E38"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMPEG2ComponentType : ILanguageComponentType
    {
        #region IComponentType Methods

        [PreserveSig]
        new int get_Category([Out] out ComponentCategory Category);

        [PreserveSig]
        new int put_Category([In] ComponentCategory Category);

        [PreserveSig]
        new int get_MediaMajorType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType);

        [PreserveSig]
        new int put_MediaMajorType([In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType);

        [PreserveSig]
        new int get__MediaMajorType([Out] out Guid MediaMajorType);

        [PreserveSig]
        new int put__MediaMajorType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType);

        [PreserveSig]
        new int get_MediaSubType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType);

        [PreserveSig]
        new int put_MediaSubType([In, MarshalAs(UnmanagedType.BStr)] string MediaSubType);

        [PreserveSig]
        new int get__MediaSubType([Out] out Guid MediaSubType);

        [PreserveSig]
        new int put__MediaSubType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType);

        [PreserveSig]
        new int get_MediaFormatType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType);

        [PreserveSig]
        new int put_MediaFormatType([In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType);

        [PreserveSig]
        new int get__MediaFormatType([Out] out Guid MediaFormatType);

        [PreserveSig]
        new int put__MediaFormatType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType);

        [PreserveSig]
        new int get_MediaType([Out] AMMediaType MediaType);

        [PreserveSig]
        new int put_MediaType([In] AMMediaType MediaType);

        [PreserveSig]
        new int Clone([Out] out IComponentType NewCT);

        #endregion

        #region ILanguageComponentType Methods

        [PreserveSig]
        new int get_LangID([Out] out int LangID);

        [PreserveSig]
        new int put_LangID([In] int LangID);

        #endregion

        [PreserveSig]
        int get_StreamType([Out] out MPEG2StreamType MP2StreamType);

        [PreserveSig]
        int put_StreamType([In] MPEG2StreamType MP2StreamType);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("E48244B8-7E17-4f76-A763-5090FF1E2F30"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAuxInTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("6E42F36E-1DD2-43c4-9F78-69D25AE39034"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDVBCLocator : IDigitalLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1B9D5FC3-5BBC-4b6c-BB18-B9D10E3EEEBF"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMPEG2TuneRequestSupport
    {
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3B21263F-26E8-489d-AAC4-924F7EFD9511"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBroadcastEvent
    {
        [PreserveSig]
        int Fire(Guid EventID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B34505E0-2F0E-497b-80BC-D43F3B24ED7F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDAComparable
    {
        [PreserveSig]
        int CompareExact(
            [In, MarshalAs(UnmanagedType.IDispatch)] object CompareTo,
            [Out] out int Result
            );

        [PreserveSig]
        int CompareEquivalent(
              [In, MarshalAs(UnmanagedType.IDispatch)] object CompareTo,
              [In] BDACompFlags dwFlags,
              [Out] out int Result
              );

        [PreserveSig]
        int HashExact(
              [Out] out long Result
              );

        [PreserveSig]
        int HashExactIncremental(
              [In] long PartialResult,
              [Out] out long Result
              );

        [PreserveSig]
        int HashEquivalent(
              [In] BDACompFlags dwFlags,
              [Out] out long Result
              );

        [PreserveSig]
        int HashEquivalentIncremental(
              [In] long PartialResult,
              [In] BDACompFlags dwFlags,
              [Out] out long Result
              );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("19B595D8-839A-47F0-96DF-4F194F3C768C"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalLocator : ILocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3d9e3887-1929-423f-8021-43682de95448"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBroadcastEventEx : IBroadcastEvent
    {
        #region IBroadcastEvent Methods

        [PreserveSig]
        new int Fire(Guid EventID);

        #endregion

        [PreserveSig]
        int FireEx(
          [In] Guid EventID,
          [In] int Param1,
          [In] int Param2,
          [In] int Param3,
          [In] int Param4
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("E60DFA45-8D56-4e65-A8AB-D6BE9412C249"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITunerCap
    {
        [PreserveSig]
        int get_SupportedNetworkTypes(
          [In] int ulcNetworkTypesMax,
          [Out] out int pulcNetworkTypes,
          [In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] Guid[] pguidNetworkTypes
          );

        [PreserveSig]
        int get_SupportedVideoFormats(
          [Out] out AMTunerModeType pulAMTunerModeType,
          [Out] out AnalogVideoStandard pulAnalogVideoStandard
          );

        [PreserveSig]
        int get_AuxInputCount(
          [Out] out int pulCompositeCount,
          [Out] out int pulSvideoCount
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2A6E293B-2595-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogRadioTuningSpace : ITuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        [PreserveSig]
        int get_MinFrequency([Out] out int MinFrequencyVal);

        [PreserveSig]
        int put_MinFrequency([In] int NewMinFrequencyVal);

        [PreserveSig]
        int get_MaxFrequency([Out] out int MaxFrequencyVal);

        [PreserveSig]
        int put_MaxFrequency([In] int NewMaxFrequencyVal);

        [PreserveSig]
        int get_Step([Out] out int StepVal);

        [PreserveSig]
        int put_Step([In] int StepVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("39DD45DA-2DA8-46BA-8A8A-87E2B73D983A"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogRadioTuningSpace2 : IAnalogRadioTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IAnalogRadioTuningSpace Methods

        [PreserveSig]
        new int get_MinFrequency([Out] out int MinFrequencyVal);

        [PreserveSig]
        new int put_MinFrequency([In] int NewMinFrequencyVal);

        [PreserveSig]
        new int get_MaxFrequency([Out] out int MaxFrequencyVal);

        [PreserveSig]
        new int put_MaxFrequency([In] int NewMaxFrequencyVal);

        [PreserveSig]
        new int get_Step([Out] out int StepVal);

        [PreserveSig]
        new int put_Step([In] int StepVal);

        #endregion

        int get_CountryCode(
            out int CountryCodeVal
            );

        int put_CountryCode(
            int NewCountryCodeVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("612AA885-66CF-4090-BA0A-566F5312E4CA"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IATSCLocator2 : IATSCLocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        #region IATSCLocator Methods

        [PreserveSig]
        new int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        new int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        new int get_TSID([Out] out int TSID);

        [PreserveSig]
        new int put_TSID([In] int TSID);

        #endregion

        [PreserveSig]
        int get_ProgramNumber([Out] out int ProgramNumber);

        [PreserveSig]
        int put_ProgramNumber([In] int ProgramNumber);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B10931ED-8BFE-4AB0-9DCE-E469C29A9729"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAuxInTuningSpace2 : IAuxInTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
        new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        int get_CountryCode(
            out int CountryCodeVal);

        int put_CountryCode(
            int NewCountryCodeVal
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("990237AE-AC11-4614-BE8F-DD217A4CB4CB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistTuneXmlUtility
    {
        [PreserveSig]
        int Deserialize(
            [In] object varValue,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppObject
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("992E165F-EA24-4b2f-9A1D-009D92120451"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistTuneXmlUtility2 : IPersistTuneXmlUtility
    {
        #region IPersistTuneXmlUtility Methods

        [PreserveSig]
        new int Deserialize(
            [In] object varValue,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppObject
            );

        #endregion

        [PreserveSig]
        int Serialize(
            ITuneRequest piTuneRequest,
            [Out, MarshalAs(UnmanagedType.BStr)] out string pString
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0754CD31-8D15-47A9-8215-D20064157244"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistTuneXml : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID(
            out Guid pClassID
            );

        #endregion

        [PreserveSig]
        int InitNew();

        [PreserveSig]
        int Load(
            [In] object varValue
            );

        [PreserveSig]
        int Save(
            [Out] out object pvarFragment
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C0A4A1D4-2B3C-491A-BA22-499FBADD4D12"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDACreateTuneRequestEx
    {
        [PreserveSig]
        int CreateTuneRequestEx(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid TuneRequestIID,
            [Out] out ITuneRequest ppTuneRequest
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("48F66A11-171A-419A-9525-BEEECD51584C"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableLocator : IATSCLocator2
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        #region IATSCLocator Methods

        [PreserveSig]
        new int get_PhysicalChannel([Out] out int PhysicalChannel);

        [PreserveSig]
        new int put_PhysicalChannel([In] int PhysicalChannel);

        [PreserveSig]
        new int get_TSID([Out] out int TSID);

        [PreserveSig]
        new int put_TSID([In] int TSID);

        #endregion

        #region IATSCLocator2 Methods

        [PreserveSig]
        new int get_ProgramNumber([Out] out int ProgramNumber);

        [PreserveSig]
        new int put_ProgramNumber([In] int ProgramNumber);

        #endregion
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("156EFF60-86F4-4E28-89FC-109799FD57EE"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IChannelIDTuneRequest : ITuneRequest
    {
        #region ITuneRequest Methods

        [PreserveSig]
        new int get_TuningSpace([Out] out ITuningSpace TuningSpace);

        [PreserveSig]
        new int get_Components([Out] out IComponents Components);

        [PreserveSig]
        new int Clone([Out] out ITuneRequest NewTuneRequest);

        [PreserveSig]
        new int get_Locator([Out] out ILocator Locator);

        [PreserveSig]
        new int put_Locator([In] ILocator Locator);

        #endregion

        [PreserveSig]
        int get_ChannelID([Out, MarshalAs(UnmanagedType.BStr)] out string ChannelID);

        [PreserveSig]
        int put_ChannelID([In, MarshalAs(UnmanagedType.BStr)] string ChannelID);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("34D1F26B-E339-430D-ABCE-738CB48984DC"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogLocator : ILocator
    {
        #region ILocator Methods

        [PreserveSig]
        new int get_CarrierFrequency([Out] out int Frequency);

        [PreserveSig]
        new int put_CarrierFrequency([In] int Frequency);

        [PreserveSig]
        new int get_InnerFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_InnerFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_InnerFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_InnerFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_OuterFEC([Out] out FECMethod FEC);

        [PreserveSig]
        new int put_OuterFEC([In] FECMethod FEC);

        [PreserveSig]
        new int get_OuterFECRate([Out] out BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int put_OuterFECRate([In] BinaryConvolutionCodeRate FEC);

        [PreserveSig]
        new int get_Modulation([Out] out ModulationType Modulation);

        [PreserveSig]
        new int put_Modulation([In] ModulationType Modulation);

        [PreserveSig]
        new int get_SymbolRate([Out] out int Rate);

        [PreserveSig]
        new int put_SymbolRate([In] int Rate);

        [PreserveSig]
        new int Clone([Out] out ILocator NewLocator);

        #endregion

        [PreserveSig]
        int get_VideoStandard([Out] out AnalogVideoStandard AVS);

        [PreserveSig]
        int put_VideoStandard([In] AnalogVideoStandard AVS);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("013F9F9C-B449-4ec7-A6D2-9D4F2FC70AE5"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDigitalCableTuningSpace : IATSCTuningSpace
    {
        #region ITuningSpace Methods

        [PreserveSig]
        new int get_UniqueName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_UniqueName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_FriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string Name);

        [PreserveSig]
        new int put_FriendlyName([In, MarshalAs(UnmanagedType.BStr)] string Name);

        [PreserveSig]
        new int get_CLSID([Out, MarshalAs(UnmanagedType.BStr)] out string SpaceCLSID);

        [PreserveSig]
        new int get_NetworkType([Out, MarshalAs(UnmanagedType.BStr)] out string NetworkTypeGuid);

        [PreserveSig]
        new int put_NetworkType([In, MarshalAs(UnmanagedType.BStr)] string NetworkTypeGuid);

        [PreserveSig]
        new int get__NetworkType([Out] out Guid NetworkTypeGuid);

        [PreserveSig]
        new int put__NetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid NetworkTypeGuid);

        [PreserveSig]
        new int CreateTuneRequest([Out] out ITuneRequest TuneRequest);

        [PreserveSig]
        new int EnumCategoryGUIDs([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEnum); // IEnumGUID**

        [PreserveSig]
#if USING_NET11
      new int EnumDeviceMonikers([Out] out UCOMIEnumMoniker ppEnum);
#else
        new int EnumDeviceMonikers([Out] out IEnumMoniker ppEnum);
#endif

        [PreserveSig]
        new int get_DefaultPreferredComponentTypes([Out] out IComponentTypes ComponentTypes);

        [PreserveSig]
        new int put_DefaultPreferredComponentTypes([In] IComponentTypes NewComponentTypes);

        [PreserveSig]
        new int get_FrequencyMapping([Out, MarshalAs(UnmanagedType.BStr)] out string pMapping);

        [PreserveSig]
        new int put_FrequencyMapping([In, MarshalAs(UnmanagedType.BStr)] string Mapping);

        [PreserveSig]
        new int get_DefaultLocator([Out] out ILocator LocatorVal);

        [PreserveSig]
        new int put_DefaultLocator([In] ILocator LocatorVal);

        [PreserveSig]
        new int Clone([Out] out ITuningSpace NewTS);

        #endregion

        #region IAnalogTVTuningSpace Methods

        [PreserveSig]
        new int get_MinChannel([Out] out int MinChannelVal);

        [PreserveSig]
        new int put_MinChannel([In] int NewMinChannelVal);

        [PreserveSig]
        new int get_MaxChannel([Out] out int MaxChannelVal);

        [PreserveSig]
        new int put_MaxChannel([In] int NewMaxChannelVal);

        [PreserveSig]
        new int get_InputType([Out] out TunerInputType InputTypeVal);

        [PreserveSig]
        new int put_InputType([In] TunerInputType NewInputTypeVal);

        [PreserveSig]
        new int get_CountryCode([Out] out int CountryCodeVal);

        [PreserveSig]
        new int put_CountryCode([In] int NewCountryCodeVal);

        #endregion

        #region IATSCTuningSpace Methods

        [PreserveSig]
        new int get_MinMinorChannel([Out] out int MinMinorChannelVal);

        [PreserveSig]
        new int put_MinMinorChannel([In] int NewMinMinorChannelVal);

        [PreserveSig]
        new int get_MaxMinorChannel([Out] out int MaxMinorChannelVal);

        [PreserveSig]
        new int put_MaxMinorChannel([In] int NewMaxMinorChannelVal);

        [PreserveSig]
        new int get_MinPhysicalChannel([Out] out int MinPhysicalChannelVal);

        [PreserveSig]
        new int put_MinPhysicalChannel([In] int NewMinPhysicalChannelVal);

        [PreserveSig]
        new int get_MaxPhysicalChannel([Out] out int MaxPhysicalChannelVal);

        [PreserveSig]
        new int put_MaxPhysicalChannel([In] int NewMaxPhysicalChannelVal);

        #endregion

        [PreserveSig]
        int get_MinMajorChannel([Out] out int MinMajorChannelVal);

        [PreserveSig]
        int put_MinMajorChannel([In] int NewMinMajorChannelVal);

        [PreserveSig]
        int get_MaxMajorChannel([Out] out int MaxMajorChannelVal);

        [PreserveSig]
        int put_MaxMajorChannel([In] int NewMaxMajorChannelVal);

        [PreserveSig]
        int get_MinSourceID([Out] out int MinSourceIDVal);

        [PreserveSig]
        int put_MinSourceID([In] int NewMinSourceIDVal);

        [PreserveSig]
        int get_MaxSourceID([Out] out int MaxSourceIDVal);

        [PreserveSig]
        int put_MaxSourceID([In] int NewMaxSourceIDVal);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("2CFEB2A8-1787-4A24-A941-C6EAEC39C842"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IAnalogAudioComponentType : IComponentType
    {
        #region IComponentType Methods

        [PreserveSig]
        new int get_Category([Out] out ComponentCategory Category);

        [PreserveSig]
        new int put_Category([In] ComponentCategory Category);

        [PreserveSig]
        new int get_MediaMajorType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaMajorType);

        [PreserveSig]
        new int put_MediaMajorType([In, MarshalAs(UnmanagedType.BStr)] string MediaMajorType);

        [PreserveSig]
        new int get__MediaMajorType([Out] out Guid MediaMajorType);

        [PreserveSig]
        new int put__MediaMajorType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaMajorType);

        [PreserveSig]
        new int get_MediaSubType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaSubType);

        [PreserveSig]
        new int put_MediaSubType([In, MarshalAs(UnmanagedType.BStr)] string MediaSubType);

        [PreserveSig]
        new int get__MediaSubType([Out] out Guid MediaSubType);

        [PreserveSig]
        new int put__MediaSubType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaSubType);

        [PreserveSig]
        new int get_MediaFormatType([Out, MarshalAs(UnmanagedType.BStr)] out string MediaFormatType);

        [PreserveSig]
        new int put_MediaFormatType([In, MarshalAs(UnmanagedType.BStr)] string MediaFormatType);

        [PreserveSig]
        new int get__MediaFormatType([Out] out Guid MediaFormatType);

        [PreserveSig]
        new int put__MediaFormatType([In, MarshalAs(UnmanagedType.LPStruct)] Guid MediaFormatType);

        [PreserveSig]
        new int get_MediaType([Out] AMMediaType MediaType);

        [PreserveSig]
        new int put_MediaType([In] AMMediaType MediaType);

        [PreserveSig]
        new int Clone([Out] out IComponentType NewCT);

        #endregion

        [PreserveSig]
        int get_AnalogAudioMode([Out] out TVAudioMode Mode);

        [PreserveSig]
        int put_AnalogAudioMode([In] TVAudioMode Mode);
    }

    #endregion
}
