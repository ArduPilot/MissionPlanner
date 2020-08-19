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
    /// From KSPROPERTY_IPSINK
    /// </summary>
    public enum KSPropertyIPSink
    {
        MulticastList,
        AdapterDescription,
        AdapterAddress
    }

    /// <summary>
    /// From PID_MAP
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PIDMap
    {
        public int ulPID;
        public MediaSampleContent MediaSampleContent;
    }

    /// <summary>
    /// From SmartCardStatusType
    /// </summary>
    public enum SmartCardStatusType
    {
        CardInserted = 0,
        CardRemoved,
        CardError,
        CardDataChanged,
        CardFirmwareUpgrade
    }

    /// <summary>
    /// From SmartCardAssociationType
    /// </summary>
    public enum SmartCardAssociationType
    {
        NotAssociated = 0,
        Associated,
        AssociationUnknown
    }

    /// <summary>
    /// From LocationCodeSchemeType
    /// </summary>
    public enum LocationCodeSchemeType
    {
        SCTE_18 = 0
    }

    /// <summary>
    /// From EALocationCodeType
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EALocationCodeType
    {
        public LocationCodeSchemeType LocationCodeScheme;
        public byte StateCode;
        public byte CountySubdivision;
        public short CountyCode;
    }

    /// <summary>
    /// From EntitlementType
    /// </summary>
    public enum EntitlementType
    {
        Entitled = 0,
        NotEntitled,
        TechnicalFailure
    }

    /// <summary>
    /// From UICloseReasonType
    /// </summary>
    public enum UICloseReasonType
    {
        NotReady = 0,
        UserClosed,
        SystemClosed,
        DeviceClosed,
        ErrorClosed
    }

    /// <summary>
    /// From SmartCardApplication
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SmartCardApplication
    {
        public ApplicationTypeType ApplicationType;
        public short ApplicationVersion;
        [MarshalAs(UnmanagedType.BStr)]
        public string pbstrApplicationName;
        [MarshalAs(UnmanagedType.BStr)]
        public string pbstrApplicationURL;
    }

    /// <summary>
    /// From BDA_DrmPairingError
    /// </summary>
    public enum BDA_DrmPairingError
    {
        Succeeded = 0,
        HardwareFailure,
        NeedRevocationData,
        NeedIndiv,
        Other,
        DrmInitFailed,
        DrmNotPaired,
        DrmRePairSoon,
        Aborted,
        NeedSDKUpdate
    }

    /// <summary>
    /// BDA_CONDITIONALACCESS_REQUESTTYPE
    /// </summary>
    public enum BDA_CONDITIONALACCESS_REQUESTTYPE
    {
        Unspecified = 0,
        NotPossible,
        Possible,
        PossibleNoStreamingDisruption
    }

    /// <summary>
    /// From BDA_CONDITIONALACCESS_MMICLOSEREASON
    /// </summary>
    public enum BDA_CONDITIONALACCESS_MMICLOSEREASON
    {
        Unspecified = 0,
        ClosedItself,
        TunerRequestedClose,
        DialogTimeout,
        DialogFocusChange,
        DialogUserDismissed,
        DialogUserNotAvailable
    }

    /// <summary>
    /// From MUX_PID_TYPE
    /// </summary>
    public enum MUX_PID_TYPE
    {
        Other = -1,
        ElementaryStream,
        MPEG2SectionPSISI
    }

    /// <summary>
    /// From BDA_MUX_PIDLISTITEM
    /// </summary>
    public class BDA_MUX_PIDLISTITEM
    {
        public short usPIDNumber;
        public short usProgramNumber;
        public MUX_PID_TYPE ePIDType;
    }

    /// <summary>
    /// From BDA_SIGNAL_TIMEOUTS
    /// </summary>
    public class BDA_SIGNAL_TIMEOUTS
    {
        public int ulCarrierTimeoutMs;
        public int ulScanningTimeoutMs;
        public int ulTuningTimeoutMs;
    }

#endif

    /// <summary>
    /// From BDA_CHANGE_STATE
    /// </summary>
    public enum BdaChangeState
    {
        ChangesComplete = 0,
        ChangesPending
    }

    /// <summary>
    /// From BDA_MULTICAST_MODE
    /// </summary>
    public enum MulticastMode
    {
        PromiscuousMulticast = 0,
        FilteredMulticast,
        NoMulticast
    }

    /// <summary>
    /// From MEDIA_SAMPLE_CONTENT
    /// </summary>
    public enum MediaSampleContent
    {
        TransportPacket,
        ElementaryStream,
        Mpeg2Psi,
        TransportPayload
    }

    /// <summary>
    /// From BDANODE_DESCRIPTOR
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BdaNodeDescriptor
    {
        public int ulBdaNodeType;
        public Guid guidFunction;
        public Guid guidName;
    }

    /// <summary>
    /// From KS_CC_SUBSTREAM_SERVICE_* defines
    /// </summary>
    [Flags]
    public enum CCSubstreamService
    {
        None = 0,
        CC1 = 0x0001, //CC1 (caption channel) 
        CC2 = 0x0002, //CC2 (caption channel) 
        T1 = 0x0004, // T1 (text channel) 
        T2 = 0x0008, // T2 (text channel) 
        CC3 = 0x0100, // CC3 (caption channel) 
        CC4 = 0x0200, // CC4 (caption channel) 
        T3 = 0x0400, // T3 (text channel) 
        T4 = 0x0800, // T4 (text channel) 
        XDS = 0x1000, // Extended Data Services (XDS) 
        Field1 = 0x000F, // Bitmask to filter field 1 substreams. 
        Field2 = 0x1F00 //Bitmask to filter field 2 substreams 
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("fd501041-8ebe-11ce-8183-00aa00577da2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_NetworkProvider
    {
        [PreserveSig]
        int PutSignalSource([In] int ulSignalSource);

        [PreserveSig]
        int GetSignalSource([Out] out int pulSignalSource);

        [PreserveSig]
        int GetNetworkType([Out] out Guid pguidNetworkType);

        [PreserveSig]
        int PutTuningSpace([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidTuningSpace);

        [PreserveSig]
        int GetTuningSpace([Out] out Guid pguidTuingSpace);

        [PreserveSig]
        int RegisterDeviceFilter(
            [In, MarshalAs(UnmanagedType.Interface)] object pUnkFilterControl,
            [Out] out int ppvRegisitrationContext
            );

        [PreserveSig]
        int UnRegisterDeviceFilter([In] int pvRegistrationContext);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("71985F46-1CA1-11d3-9CC8-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_VoidTransform
    {
        [PreserveSig]
        int Start();

        [PreserveSig]
        int Stop();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DDF15B0D-BD25-11d2-9CA0-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_NullTransform
    {
        [PreserveSig]
        int Start();

        [PreserveSig]
        int Stop();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("afb6c2a2-2c41-11d3-8a60-0000f81e0e4a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumPIDMap
    {
        [PreserveSig]
        int Next(
            [In] int cRequest,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.Struct)] PIDMap[] pPIDMap,
            [Out] out int pcReceived
            );

        [PreserveSig]
        int Skip([In] int cRecords);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone([Out] out IEnumPIDMap ppIEnumPIDMap);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("0DED49D5-A8B7-4d5d-97A1-12B0C195874D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_PinControl
    {
        [PreserveSig]
        int GetPinID([Out] out int pulPinID);

        [PreserveSig]
        int GetPinType([Out] out int pulPinType);

        [PreserveSig]
        int RegistrationContext([Out] out int pulRegistrationCtx);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("34518D13-1182-48e6-B28F-B24987787326"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_AutoDemodulateEx : IBDA_AutoDemodulate
    {
        #region IBDA_AutoDemodulate Methods

        [PreserveSig]
        new int put_AutoDemodulate();

        #endregion

        [PreserveSig]
        int get_SupportedDeviceNodeTypes(
            [In] int ulcDeviceNodeTypesMax,
            [Out] out int pulcDeviceNodeTypes,
            [In, Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] Guid[] pguidDeviceNodeTypes
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
    Guid("D806973D-3EBE-46de-8FBB-6358FE784208"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_EasMessage
    {
        [PreserveSig]
        int get_EasMessage(
          [In] int ulEventID,
          [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppEASObject
          );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("8E882535-5F86-47AB-86CF-C281A72A0549"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_TransportStreamInfo
    {
        [PreserveSig]
        int get_PatTableTickCount([Out] out int pPatTickCount);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("CD51F1E0-7BE9-4123-8482-A2A796C0A6B0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_ConditionalAccess
    {
        [PreserveSig]
        int get_SmartCardStatus(
            [Out] out SmartCardStatusType pCardStatus,
            [Out] out SmartCardAssociationType pCardAssociation,
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrCardError,
            [Out, MarshalAs(UnmanagedType.VariantBool)] out bool pfOOBLocked
            );

        [PreserveSig]
        int get_SmartCardInfo(
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrCardName,
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrCardManufacturer,
            [Out, MarshalAs(UnmanagedType.VariantBool)] out bool pfDaylightSavings,
            [Out] out byte pbyRatingRegion,
            [Out] out int plTimeZoneOffsetMinutes,
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrLanguage,
            [Out] out EALocationCodeType pEALocationCode
            );

        [PreserveSig]
        int get_SmartCardApplications(
            [In, Out] ref int pulcApplications,
            [In] int ulcApplicationsMax,
            [In, Out] SmartCardApplication[] rgApplications
            );

        [PreserveSig]
        int get_Entitlement(
            [In] short usVirtualChannel,
            [Out] out EntitlementType pEntitlement
            );

        [PreserveSig]
        int TuneByChannel([In] short usVirtualChannel);

        [PreserveSig]
        int SetProgram([In] short usProgramNumber);

        [PreserveSig]
        int AddProgram([In] short usProgramNumber);

        [PreserveSig]
        int RemoveProgram([In] short usProgramNumber);

        [PreserveSig]
        int GetModuleUI(
            [In] byte byDialogNumber,
            [Out, MarshalAs(UnmanagedType.BStr)] out string pbstrURL
            );

        [PreserveSig]
        int InformUIClosed(
            [In] byte byDialogNumber,
            [In] UICloseReasonType CloseReason
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("20e80cb5-c543-4c1b-8eb3-49e719eee7d4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DiagnosticProperties : IPropertyBag
    {
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F98D88B0-1992-4cd6-A6D9-B9AFAB99330D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DRM
    {
        [PreserveSig]
        int GetDRMPairingStatus(
          [Out] out BDA_DrmPairingError pdwStatus,
          [Out] out int phError
          );

        [PreserveSig]
        int PerformDRMPairing([In, MarshalAs(UnmanagedType.Bool)] bool fSync);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("7DEF4C09-6E66-4567-A819-F0E17F4A81AB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_AUX
    {
        [PreserveSig]
        int QueryCapabilities(
            out int pdwNumAuxInputsBSTR
            );

        [PreserveSig]
        int EnumCapability(
            [In] int dwIndex, 
            out int dwInputID, 
            out Guid pConnectorType, 
            out int ConnTypeNum, 
            out int NumVideoStds, 
            out long AnalogStds
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("497C3418-23CB-44BA-BB62-769F506FCEA7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_ConditionalAccessEx
    {
        [PreserveSig]
        int CheckEntitlementToken(
            [In] int ulDialogRequest, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [In] BDA_CONDITIONALACCESS_REQUESTTYPE RequestType, 
            [In] int ulcbEntitlementTokenLen, 
            [In] ref byte pbEntitlementToken, 
            out int pulDescrambleStatus
            );

        [PreserveSig]
        int SetCaptureToken(
            [In] int ulcbCaptureTokenLen, 
            [In] ref byte pbCaptureToken
            );

        [PreserveSig]
        int OpenBroadcastMmi(
            [In] int ulDialogRequest, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [In] int EventId
            );

        [PreserveSig]
        int CloseMmiDialog(
            [In] int ulDialogRequest, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [In] int ulDialogNumber, 
            [In] BDA_CONDITIONALACCESS_MMICLOSEREASON ReasonCode, 
            out int pulSessionResult
            );

        [PreserveSig]
        int CreateDialogRequestNumber(
            out int pulDialogRequestNumber
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("525ED3EE-5CF3-4E1E-9A06-5368A84F9A6E")]
    public interface IBDA_DigitalDemodulator2 : IBDA_DigitalDemodulator
    {
        #region IBDA_DigitalDemodulator Methods

        [PreserveSig]
        new int put_ModulationType(
            [In] ref ModulationType pModulationType
            );

        [PreserveSig]
        new int get_ModulationType(
            [Out] out ModulationType pModulationType
            );

        [PreserveSig]
        new int put_InnerFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_InnerFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_InnerFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_InnerFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_OuterFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_OuterFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_OuterFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_OuterFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_SymbolRate(
            [In] ref int pSymbolRate
            );

        [PreserveSig]
        new int get_SymbolRate(
            [Out] out int pSymbolRate
            );

        [PreserveSig]
        new int put_SpectralInversion(
            [In] ref SpectralInversion pSpectralInversion
            );

        [PreserveSig]
        new int get_SpectralInversion(
            [Out] out SpectralInversion pSpectralInversion
            );

        #endregion

        [PreserveSig]
        int put_GuardInterval(
            [In] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        int get_GuardInterval(
            [In, Out] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        int put_TransmissionMode(
            [In] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        int get_TransmissionMode(
            [In, Out] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        int put_RollOff(
            [In] ref RollOff pRollOff
            );

        [PreserveSig]
        int get_RollOff(
            [In, Out] ref RollOff pRollOff
            );

        [PreserveSig]
        int put_Pilot(
            [In] ref Pilot pPilot
            );

        [PreserveSig]
        int get_Pilot(
            [In, Out] ref Pilot pPilot
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("F84E2AB0-3C6B-45E3-A0FC-8669D4B81F11"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DiseqCommand
    {
        [PreserveSig]
        int put_EnableDiseqCommands(
            [In] byte bEnable
            );

        [PreserveSig]
        int put_DiseqLNBSource(
            [In] int ulLNBSource
            );

        [PreserveSig]
        int put_DiseqUseToneBurst(
            [In] byte bUseToneBurst
            );

        [PreserveSig]
        int put_DiseqRepeats(
            [In] int ulRepeats
            );

        [PreserveSig]
        int put_DiseqSendCommand(
            [In] int ulRequestId, 
            [In] int ulcbCommandLen, 
            [In] ref byte pbCommand
            );

        [PreserveSig]
        int get_DiseqResponse(
            [In] int ulRequestId, 
            [In, Out] ref int pulcbResponseLen, 
            [In, Out] ref byte pbResponse
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1F9BC2A5-44A3-4C52-AAB1-0BBCE5A1381D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DRIDRMService
    {
        [PreserveSig]
        int SetDRM(
            [In, MarshalAs(UnmanagedType.BStr)] string bstrNewDrm
            );

        [PreserveSig]
        int GetDRMStatus(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrDrmUuidList, 
            out Guid DrmUuid
            );

        [PreserveSig]
        int GetPairingStatus(
            [In, Out] ref BDA_DrmPairingError penumPairingStatus
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("05C690F8-56DB-4BB2-B053-79C12098BB26"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DRIWMDRMSession
    {
        [PreserveSig]
        int AcknowledgeLicense(
            [In, MarshalAs(UnmanagedType.Error)] int hrLicenseAck
            );

        [PreserveSig]
        int ProcessLicenseChallenge(
            [In] int dwcbLicenseMessage, 
            [In] ref byte pbLicenseMessage, 
            [In, Out] ref int pdwcbLicenseResponse, 
            [In, Out] IntPtr ppbLicenseResponse
            );

        [PreserveSig]
        int ProcessRegistrationChallenge(
            [In] int dwcbRegistrationMessage, 
            [In] ref byte pbRegistrationMessage, 
            [In, Out] ref int pdwcbRegistrationResponse, 
            [In, Out] IntPtr ppbRegistrationResponse
            );

        [PreserveSig]
        int SetRevInfo(
            [In] int dwRevInfoLen, 
            [In] ref byte pbRevInfo, 
            [In, Out] ref int pdwResponse
            );

        [PreserveSig]
        int SetCrl(
            [In] int dwCrlLen, 
            [In] ref byte pbCrlLen, 
            [In, Out] ref int pdwResponse
            );

        [PreserveSig]
        int GetHMSAssociationData();

        [PreserveSig]
        int GetLastCardeaError(
            [In, Out] ref int pdwError
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("BFF6B5BB-B0AE-484C-9DCA-73528FB0B46E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DRMService
    {
        [PreserveSig]
        int SetDRM(
            [In] ref Guid puuidNewDrm
            );

        [PreserveSig]
        int GetDRMStatus(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrDrmUuidList, 
            out Guid DrmUuid
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3A8BAD59-59FE-4559-A0BA-396CFAA98AE3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_Encoder
    {
        [PreserveSig]
        int QueryCapabilities(
            out int NumAudioFmts, 
            out int NumVideoFmts
            );

        [PreserveSig]
        int EnumAudioCapability(
            [In] int FmtIndex, 
            out int MethodID, 
            out int AlgorithmType, 
            out int SamplingRate, 
            out int BitDepth, 
            out int NumChannels
            );

        [PreserveSig]
        int EnumVideoCapability(
            [In] int FmtIndex, 
            out int MethodID, 
            out int AlgorithmType, 
            out int VerticalSize, 
            out int HorizontalSize, 
            out int AspectRatio, 
            out int FrameRateCode, 
            out int ProgressiveSequence
            );

        [PreserveSig]
        int SetParameters(
            [In] int AudioBitrateMode, 
            [In] int AudioBitrate, 
            [In] int AudioMethodID, 
            [In] int AudioProgram, 
            [In] int VideoBitrateMode, 
            [In] int VideoBitrate, 
            [In] int VideoMethodID
            );

        [PreserveSig]
        int GetState(
            out int AudioBitrateMax, 
            out int AudioBitrateMin, 
            out int AudioBitrateMode, 
            out int AudioBitrateStepping, 
            out int AudioBitrate, 
            out int AudioMethodID, 
            out int AvailableAudioPrograms, 
            out int AudioProgram, 
            out int VideoBitrateMax, 
            out int VideoBitrateMin, 
            out int VideoBitrateMode, 
            out int VideoBitrate, 
            out int VideoBitrateStepping, 
            out int VideoMethodID, 
            out int SignalSourceID, 
            out long SignalFormat, 
            out int SignalLock, 
            out int SignalLevel, 
            out int SignalToNoiseRatio
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("207C413F-00DC-4C61-BAD6-6FEE1FF07064"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_EventingService
    {
        [PreserveSig]
        int CompleteEvent(
            [In] int ulEventID, 
            [In] int ulEventResult
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("138ADC7E-58AE-437F-B0B4-C9FE19D5B4AC"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_FDC
    {
        [PreserveSig]
        int GetStatus(
            out int CurrentBitrate, 
            out int CarrierLock, 
            out int CurrentFrequency, 
            out int CurrentSpectrumInversion, 
            [MarshalAs(UnmanagedType.BStr)] out string CurrentPIDList, 
            [MarshalAs(UnmanagedType.BStr)] out string CurrentTIDList, 
            out int Overflow
            );

        [PreserveSig]
        int RequestTables(
            [In, MarshalAs(UnmanagedType.BStr)] string TableIDs
            );

        [PreserveSig]
        int AddPid(
            [In, MarshalAs(UnmanagedType.BStr)] string PidsToAdd, 
            out int RemainingFilterEntries
            );

        [PreserveSig]
        int RemovePid(
            [In, MarshalAs(UnmanagedType.BStr)] string PidsToRemove
            );

        [PreserveSig]
        int AddTid(
            [In, MarshalAs(UnmanagedType.BStr)] string TidsToAdd, 
            [MarshalAs(UnmanagedType.BStr)] out string CurrentTIDList
            );

        [PreserveSig]
        int RemoveTid(
            [In, MarshalAs(UnmanagedType.BStr)] string TidsToRemove
            );

        [PreserveSig]
        int GetTableSection(
            out int Pid, 
            [In] int MaxBufferSize, 
            out int ActualSize, 
            out byte SecBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("C0AFCB73-23E7-4BC6-BAFA-FDC167B4719F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_GuideDataDeliveryService
    {
        [PreserveSig]
        int GetGuideDataType(
            out Guid pguidDataType
            );

        [PreserveSig]
        int GetGuideData(
            [In, Out] ref int pulcbBufferLen, 
            out byte pbBuffer, 
            out int pulGuideDataPercentageProgress
            );

        [PreserveSig]
        int RequestGuideDataUpdate();

        [PreserveSig]
        int GetTuneXmlFromServiceIdx(
            [In] long ul64ServiceIdx, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrTuneXml
            );

        [PreserveSig]
        int GetServices(
            [In, Out] ref int pulcbBufferLen, out byte pbBuffer
            );

        [PreserveSig]
        int GetServiceInfoFromTuneXml(
            [In, MarshalAs(UnmanagedType.BStr)] string bstrTuneXml, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrServiceDescription
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("5E68C627-16C2-4E6C-B1E2-D00170CDAA0F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_ISDBConditionalAccess
    {
        [PreserveSig]
        int SetIsdbCasRequest(
            [In] int ulRequestId, 
            [In] int ulcbRequestBufferLen, 
            [In] ref byte pbRequestBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("942AAFEC-4C05-4C74-B8EB-8706C2A4943F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_MUX
    {
        [PreserveSig]
        int SetPidList(
            [In] int ulPidListCount, 
            [In] ref BDA_MUX_PIDLISTITEM pbPidListBuffer
            );

        [PreserveSig]
        int GetPidList(
            [In, Out] ref int pulPidListCount, 
            [In, Out] ref BDA_MUX_PIDLISTITEM pbPidListBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("7F0B3150-7B81-4AD4-98E3-7E9097094301"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_NameValueService
    {
        [PreserveSig]
        int GetValueNameByIndex(
            [In] int ulIndex, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrName
            );

        [PreserveSig]
        int GetValue(
            [In, MarshalAs(UnmanagedType.BStr)] string bstrName, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [MarshalAs(UnmanagedType.BStr)] out string pbstrValue
            );

        [PreserveSig]
        int SetValue(
            [In] int ulDialogRequest, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrName, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrValue, 
            [In] int ulReserved
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1DCFAFE9-B45E-41B3-BB2A-561EB129AE98"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_TransportStreamSelector
    {
        [PreserveSig]
        int SetTSID(
            [In] short usTSID
            );

        [PreserveSig]
        int GetTSInformation(
            [In, Out] ref int pulTSInformationBufferLen, 
            out byte pbTSInformationBuffer
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("53B14189-E478-4B7A-A1FF-506DB4B99DFE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_UserActivityService
    {
        [PreserveSig]
        int SetCurrentTunerUseReason(
            [In] int dwUseReason
            );

        [PreserveSig]
        int GetUserActivityInterval(
            out int pdwActivityInterval
            );

        [PreserveSig]
        int UserActivityDetected();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4BE6FA3D-07CD-4139-8B80-8C18BA3AEC88"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_WMDRMSession
    {
        [PreserveSig]
        int GetStatus(
            out int MaxCaptureToken, 
            out int MaxStreamingPid, 
            out int MaxLicense, 
            out int MinSecurityLevel, 
            out int RevInfoSequenceNumber, 
            out long RevInfoIssuedTime, 
            out int RevInfoTTL, 
            out int RevListVersion, 
            out int ulState
            );

        [PreserveSig]
        int SetRevInfo(
            [In] int ulRevInfoLen, 
            [In] ref byte pbRevInfo
            );

        [PreserveSig]
        int SetCrl(
            [In] int ulCrlLen, 
            [In] ref byte pbCrlLen
            );

        [PreserveSig]
        int TransactMessage(
            [In] int ulcbRequest, 
            [In] ref byte pbRequest, 
            [In, Out] ref int pulcbResponse, 
            [In, Out] ref byte pbResponse
            );

        [PreserveSig]
        int GetLicense(
            [In] ref Guid uuidKey, 
            [In, Out] ref int pulPackageLen, 
            [In, Out] ref byte pbPackage
            );

        [PreserveSig]
        int ReissueLicense(
            [In] ref Guid uuidKey
            );

        [PreserveSig]
        int RenewLicense(
            [In] int ulInXmrLicenseLen, 
            [In] ref byte pbInXmrLicense, 
            [In] int ulEntitlementTokenLen, 
            [In] ref byte pbEntitlementToken, 
            out int pulDescrambleStatus, 
            [In, Out] ref int pulOutXmrLicenseLen, 
            [In, Out] ref byte pbOutXmrLicense
            );

        [PreserveSig]
        int GetKeyInfo(
            [In, Out] ref int pulKeyInfoLen, 
            [In, Out] ref byte pbKeyInfo
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("86D979CF-A8A7-4F94-B5FB-14C0ACA68FE6"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_WMDRMTuner
    {
        [PreserveSig]
        int PurchaseEntitlement(
            [In] int ulDialogRequest, 
            [In, MarshalAs(UnmanagedType.BStr)] string bstrLanguage, 
            [In] int ulPurchaseTokenLen, 
            [In] ref byte pbPurchaseToken, 
            out int pulDescrambleStatus, 
            [In, Out] ref int pulCaptureTokenLen, 
            [In, Out] ref byte pbCaptureToken
            );

        [PreserveSig]
        int CancelCaptureToken(
            [In] int ulCaptureTokenLen, 
            [In] ref byte pbCaptureToken
            );

        [PreserveSig]
        int SetPidProtection(
            [In] int ulPid, 
            [In] ref Guid uuidKey
            );

        [PreserveSig]
        int GetPidProtection(
            [In] int pulPid, 
            out Guid uuidKey
            );

        [PreserveSig]
        int SetSyncValue(
            [In] int ulSyncValue
            );

        [PreserveSig]
        int GetStartCodeProfile(
            [In, Out] ref int pulStartCodeProfileLen, 
            [In, Out] ref byte pbStartCodeProfile
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("13F19604-7D32-4359-93A2-A05205D90AC9"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DigitalDemodulator3 : IBDA_DigitalDemodulator2
    {
        #region IBDA_DigitalDemodulator Methods

        [PreserveSig]
        new int put_ModulationType(
            [In] ref ModulationType pModulationType
            );

        [PreserveSig]
        new int get_ModulationType(
            [Out] out ModulationType pModulationType
            );

        [PreserveSig]
        new int put_InnerFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_InnerFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_InnerFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_InnerFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_OuterFECMethod(
            [In] ref FECMethod pFECMethod
            );

        [PreserveSig]
        new int get_OuterFECMethod(
            [Out] out FECMethod pFECMethod
            );

        [PreserveSig]
        new int put_OuterFECRate(
            [In] ref BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int get_OuterFECRate(
            [Out] out BinaryConvolutionCodeRate pFECRate
            );

        [PreserveSig]
        new int put_SymbolRate(
            [In] ref int pSymbolRate
            );

        [PreserveSig]
        new int get_SymbolRate(
            [Out] out int pSymbolRate
            );

        [PreserveSig]
        new int put_SpectralInversion(
            [In] ref SpectralInversion pSpectralInversion
            );

        [PreserveSig]
        new int get_SpectralInversion(
            [Out] out SpectralInversion pSpectralInversion
            );

        #endregion

        #region IBDA_DigitalDemodulator2 Methods

        [PreserveSig]
        new int put_GuardInterval(
            [In] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        new int get_GuardInterval(
            [In, Out] ref GuardInterval pGuardInterval
            );

        [PreserveSig]
        new int put_TransmissionMode(
            [In] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        new int get_TransmissionMode(
            [In, Out] ref TransmissionMode pTransmissionMode
            );

        [PreserveSig]
        new int put_RollOff(
            [In] ref RollOff pRollOff
            );

        [PreserveSig]
        new int get_RollOff(
            [In, Out] ref RollOff pRollOff
            );

        [PreserveSig]
        new int put_Pilot(
            [In] ref Pilot pPilot
            );

        [PreserveSig]
        new int get_Pilot(
            [In, Out] ref Pilot pPilot
            );

        #endregion

        [PreserveSig]
        int put_SignalTimeouts(
            [In] BDA_SIGNAL_TIMEOUTS pSignalTimeouts
        );

        [PreserveSig]
        int get_SignalTimeouts(
            [In, Out] BDA_SIGNAL_TIMEOUTS pSignalTimeouts
        );

        [PreserveSig]
        int put_PLPNumber(
            [In] ref int pPLPNumber
        );

        [PreserveSig]
        int get_PLPNumber(
            [In, Out] ref int pPLPNumber
        );
    }

#endif

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("1347D106-CF3A-428a-A5CB-AC0D9A2A4338"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_SignalStatistics
    {
        [PreserveSig]
        int put_SignalStrength([In] int lDbStrength);

        [PreserveSig]
        int get_SignalStrength([Out] out int plDbStrength);

        [PreserveSig]
        int put_SignalQuality([In] int lPercentQuality);

        [PreserveSig]
        int get_SignalQuality([Out] out int plPercentQuality);

        [PreserveSig]
        int put_SignalPresent([In, MarshalAs(UnmanagedType.U1)] bool fPresent);

        [PreserveSig]
        int get_SignalPresent([Out, MarshalAs(UnmanagedType.U1)] out bool pfPresent);

        [PreserveSig]
        int put_SignalLocked([In, MarshalAs(UnmanagedType.U1)] bool fLocked);

        [PreserveSig]
        int get_SignalLocked([Out, MarshalAs(UnmanagedType.U1)] out bool pfLocked);

        [PreserveSig]
        int put_SampleTime([In] int lmsSampleTime);

        [PreserveSig]
        int get_SampleTime([Out] out int plmsSampleTime);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("79B56888-7FEA-4690-B45D-38FD3C7849BE"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_Topology
    {
        [PreserveSig]
        int GetNodeTypes(
            [Out] out int pulcNodeTypes,
            [In] int ulcNodeTypesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 1)] int[] rgulNodeTypes
            );

        [PreserveSig]
        int GetNodeDescriptors(
            [Out] out int ulcNodeDescriptors,
            [In] int ulcNodeDescriptorsMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 1)] BdaNodeDescriptor[] rgNodeDescriptors
            );

        [PreserveSig]
        int GetNodeInterfaces(
            [In] int ulNodeType,
            [Out] out int pulcInterfaces,
            [In] int ulcInterfacesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 2)] Guid[] rgguidInterfaces
            );

        [PreserveSig]
        int GetPinTypes(
            [Out] out int pulcPinTypes,
            [In] int ulcPinTypesMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 1)] int[] rgulPinTypes
            );

        [PreserveSig]
        int GetTemplateConnections(
            [Out] out int pulcConnections,
            [In] int ulcConnectionsMax,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStruct, SizeParamIndex = 1)] BDATemplateConnection[] rgConnections
            );

        [PreserveSig]
        int CreatePin(
            [In] int ulPinType,
            [Out] out int pulPinId
            );

        [PreserveSig]
        int DeletePin([In] int ulPinId);

        [PreserveSig]
        int SetMediaType(
            [In] int ulPinId,
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType
            );

        [PreserveSig]
        int SetMedium(
            [In] int ulPinId,
            [In] RegPinMedium pMedium
            );

        [PreserveSig]
        int CreateTopology(
            [In] int ulInputPinId,
            [In] int ulOutputPinId
            );

        [PreserveSig]
        int GetControlNode(
            [In] int ulInputPinId,
            [In] int ulOutputPinId,
            [In] int ulNodeType,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppControlNode // IUnknown
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("06FB45C1-693C-4ea7-B79F-7A6A54D8DEF2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFrequencyMap
    {
        [PreserveSig]
        int get_FrequencyMapping(
            [Out] out int ulCount,
            [Out] out IntPtr ppulList
            );

        [PreserveSig]
        int put_FrequencyMapping(
            [In] int ulCount,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4)] int[] pList
            );

        [PreserveSig]
        int get_CountryCode([Out] out int pulCountryCode);

        [PreserveSig]
        int put_CountryCode([In] int ulCountryCode);

        [PreserveSig]
        int get_DefaultFrequencyMapping(
            [In] int ulCountryCode,
            [Out] out int pulCount,
            [Out] out IntPtr ppulList
            );

        [PreserveSig]
        int get_CountryCodeList(
            [Out] out int pulCount,
            [Out] out IntPtr ppulList
            );
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("DDF15B12-BD25-11d2-9CA0-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_AutoDemodulate
    {
        [PreserveSig]
        int put_AutoDemodulate();
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("FD0A5AF3-B41D-11d2-9C95-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DeviceControl
    {
        [PreserveSig]
        int StartChanges();

        [PreserveSig]
        int CheckChanges();

        [PreserveSig]
        int CommitChanges();

        [PreserveSig]
        int GetChangeState([Out] out BdaChangeState pState);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("EF30F379-985B-4d10-B640-A79D5E04E1E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_DigitalDemodulator
    {
        [PreserveSig]
        int put_ModulationType([In] ref ModulationType pModulationType);

        [PreserveSig]
        int get_ModulationType([Out] out ModulationType pModulationType);

        [PreserveSig]
        int put_InnerFECMethod([In] ref FECMethod pFECMethod);

        [PreserveSig]
        int get_InnerFECMethod([Out] out FECMethod pFECMethod);

        [PreserveSig]
        int put_InnerFECRate([In] ref BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int get_InnerFECRate([Out] out BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int put_OuterFECMethod([In] ref FECMethod pFECMethod);

        [PreserveSig]
        int get_OuterFECMethod([Out] out FECMethod pFECMethod);

        [PreserveSig]
        int put_OuterFECRate([In] ref BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int get_OuterFECRate([Out] out BinaryConvolutionCodeRate pFECRate);

        [PreserveSig]
        int put_SymbolRate([In] ref int pSymbolRate);

        [PreserveSig]
        int get_SymbolRate([Out] out int pSymbolRate);

        [PreserveSig]
        int put_SpectralInversion([In] ref SpectralInversion pSpectralInversion);

        [PreserveSig]
        int get_SpectralInversion([Out] out SpectralInversion pSpectralInversion);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("71985F43-1CA1-11d3-9CC8-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_EthernetFilter
    {
        [PreserveSig]
        int GetMulticastListSize(
            out int pulcbAddresses);

        [PreserveSig]
        int PutMulticastList(
            int ulcbAddresses,
            IntPtr pAddressList);

        [PreserveSig]
        int GetMulticastList(
            ref int pulcbAddresses,
            IntPtr pAddressList);

        [PreserveSig]
        int PutMulticastMode(
            MulticastMode ulModeMask);

        [PreserveSig]
        int GetMulticastMode(
            out MulticastMode pulModeMask);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("71985F47-1CA1-11d3-9CC8-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_FrequencyFilter
    {
        [PreserveSig]
        int put_Autotune([In] int ulTransponder);

        [PreserveSig]
        int get_Autotune([Out] out int pulTransponder);

        [PreserveSig]
        int put_Frequency([In] int ulFrequency);

        [PreserveSig]
        int get_Frequency([Out] out int pulFrequency);

        [PreserveSig]
        int put_Polarity([In] Polarisation Polarity);

        [PreserveSig]
        int get_Polarity([Out] out Polarisation pPolarity);

        [PreserveSig]
        int put_Range([In] long ulRange);

        [PreserveSig]
        int get_Range([Out] out long pulRange);

        [PreserveSig]
        int put_Bandwidth([In] int ulBandwidth);

        [PreserveSig]
        int get_Bandwidth([Out] out int pulBandwidth);

        [PreserveSig]
        int put_FrequencyMultiplier([In] int ulMultiplier);

        [PreserveSig]
        int get_FrequencyMultiplier([Out] out int pulMultiplier);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("3F4DC8E2-4050-11d3-8F4B-00C04F7971E2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Obsolete("IBDA_IPSinkControl is no longer being supported for Ring 3 clients. Use the BDA_IPSinkInfo interface instead.")]
    public interface IBDA_IPSinkControl
    {
        [PreserveSig]
        int GetMulticastList(
            out int pulcbSize,
            out IntPtr pbBuffer); // BYTE **

        [PreserveSig]
        int GetAdapterIPAddress(
            out int pulcbSize,
            out IntPtr pbBuffer); // BYTE **
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("A750108F-492E-4d51-95F7-649B23FF7AD7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_IPSinkInfo
    {
        [PreserveSig]
        int get_MulticastList(
            out int pulcbAddresses,
            out IntPtr ppbAddressList); // BYTE **

        [PreserveSig]
        int get_AdapterIPAddress(
            [MarshalAs(UnmanagedType.BStr)] out string pbstrBuffer);

        [PreserveSig]
        int get_AdapterDescription(
            [MarshalAs(UnmanagedType.BStr)]  out string pbstrBuffer);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("71985F44-1CA1-11d3-9CC8-00C04F7971E0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_IPV4Filter
    {
        [PreserveSig]
        int GetMulticastListSize(
            out int pulcbAddresses);

        [PreserveSig]
        int PutMulticastList(
            int ulcbAddresses,
            IntPtr pAddressList);

        [PreserveSig]
        int GetMulticastList(
            ref int pulcbAddresses,
            IntPtr pAddressList);

        [PreserveSig]
        int PutMulticastMode(
            MulticastMode ulModeMask);

        [PreserveSig]
        int GetMulticastMode(
            out MulticastMode pulModeMask);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("E1785A74-2A23-4fb3-9245-A8F88017EF33"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_IPV6Filter
    {
        [PreserveSig]
        int GetMulticastListSize(
            out int pulcbAddresses);

        [PreserveSig]
        int PutMulticastList(
            int ulcbAddresses,
            IntPtr pAddressList);  // BYTE []

        [PreserveSig]
        int GetMulticastList(
            ref int pulcbAddresses,
            IntPtr pAddressList);  // BYTE []

        [PreserveSig]
        int PutMulticastMode(
            MulticastMode ulModeMask);

        [PreserveSig]
        int GetMulticastMode(
            out MulticastMode pulModeMask);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("992CF102-49F9-4719-A664-C4F23E2408F4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_LNBInfo
    {
        [PreserveSig]
        int put_LocalOscilatorFrequencyLowBand([In] int ulLOFLow);

        [PreserveSig]
        int get_LocalOscilatorFrequencyLowBand([Out] out int pulLOFLow);

        [PreserveSig]
        int put_LocalOscilatorFrequencyHighBand([In] int ulLOFHigh);

        [PreserveSig]
        int get_LocalOscilatorFrequencyHighBand([Out] out int pulLOFHigh);

        [PreserveSig]
        int put_HighLowSwitchFrequency([In] int ulSwitchFrequency);

        [PreserveSig]
        int get_HighLowSwitchFrequency([Out] out int pulSwitchFrequency);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("afb6c2a1-2c41-11d3-8a60-0000f81e0e4a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMPEG2PIDMap
    {
        [PreserveSig]
        int MapPID(
            [In] int culPID,
            [In, MarshalAs(UnmanagedType.LPArray)] int[] pulPID,
            [In] MediaSampleContent MediaSampleContent
            );

        [PreserveSig]
        int UnmapPID(
            [In] int culPID,
            [In, MarshalAs(UnmanagedType.LPArray)] int[] pulPID
            );

        [PreserveSig,
        Obsolete("Because of bug in DS 9.0c, you can't get the PID map from .NET", false)]
#if ALLOW_UNTESTED_INTERFACES
        int EnumPIDMap([Out] out IEnumPIDMap pIEnumPIDMap);
#else
        int EnumPIDMap([Out] out object pIEnumPIDMap);
#endif
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("D2F1644B-B409-11d2-BC69-00A0C9EE9E16"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBDA_SignalProperties
    {
        [PreserveSig]
        int PutNetworkType([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidNetworkType);

        [PreserveSig]
        int GetNetworkType([Out] out Guid pguidNetworkType);

        [PreserveSig]
        int PutSignalSource([In] int ulSignalSource);

        [PreserveSig]
        int GetSignalSource([Out] out int pulSignalSource);

        [PreserveSig]
        int PutTuningSpace([In, MarshalAs(UnmanagedType.LPStruct)] Guid guidTuningSpace);

        [PreserveSig]
        int GetTuningSpace([Out] out Guid pguidTuingSpace);
    }

    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("4B2BD7EA-8347-467b-8DBF-62F784929CC3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICCSubStreamFiltering
    {
        [PreserveSig]
        int get_SubstreamTypes([Out] out CCSubstreamService Types);

        [PreserveSig]
        int put_SubstreamTypes([In] CCSubstreamService Types);
    }

    #endregion
}
