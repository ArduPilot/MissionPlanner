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

namespace DirectShowLib.BDA
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From BDA_EVENT_ID
    /// </summary>
    public enum BDAEventID
    {
        SignalLoss = 0,
        SignalLock,
        DataStart,
        DataStop,
        ChannelAcquired,
        ChannelLost,
        ChannelSourceChanged,
        ChannelActivated,
        ChannelDeactivated,
        SubChannelAcquired,
        SubChannelLost,
        SubChannelSourceChanged,
        SubChannelActivated,
        SubChannelDeactivated,
        AccessGranted,
        AccessDenied,
        OfferExtended,
        PurchaseCompleted,
        SmartCardInserted,
        SmartCardRemoved
    }

    /// <summary>
    /// From BDA_TEMPLATE_PIN_JOINT
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BDATemplatePinJoint
    {
        public int uliTemplateConnection;
        public int ulcInstancesMax;
    }

    /// <summary>
    /// From KS_BDA_FRAME_INFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KSBDAFrameInfo
    {
        public int ExtendedHeaderSize; // Size of this extended header
        public int dwFrameFlags; //
        public int ulEvent; //
        public int ulChannelNumber; //
        public int ulSubchannelNumber; //
        public int ulReason; //
    }

    /// <summary>
    /// From MPEG2_TRANSPORT_STRIDE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MPEG2TransportStride
    {
        public int dwOffset;
        public int dwPacketLength;
        public int dwStride;
    }

    /// <summary>
    /// From ScanModulationTypes
    /// </summary>
    [Flags]
    public enum ScanModulationTypes
    {
      ScanMod16QAM = 0x00000001,
      ScanMod32QAM = 0x00000002,
      ScanMod64QAM = 0x00000004,
      ScanMod80QAM = 0x00000008,
      ScanMod96QAM = 0x00000010,
      ScanMod112QAM = 0x00000020,
      ScanMod128QAM = 0x00000040,
      ScanMod160QAM = 0x00000080,
      ScanMod192QAM = 0x00000100,
      ScanMod224QAM = 0x00000200,
      ScanMod256QAM = 0x00000400,
      ScanMod320QAM = 0x00000800,
      ScanMod384QAM = 0x00001000,
      ScanMod448QAM = 0x00002000,
      ScanMod512QAM = 0x00004000,
      ScanMod640QAM = 0x00008000,
      ScanMod768QAM = 0x00010000,
      ScanMod896QAM = 0x00020000,
      ScanMod1024QAM = 0x00040000,
      ScanModQPSK = 0x00080000,
      ScanModBPSK = 0x00100000,
      ScanModOQPSK = 0x00200000,
      ScanMod8VSB = 0x00400000,
      ScanMod16VSB = 0x00800000,
      ScanModAM_RADIO = 0x01000000,
      ScanModFM_RADIO = 0x02000000,
      ScanMod8PSK = 0x04000000,
      ScanModRF = 0x08000000,
      MCEDigitalCable = ModulationType.Mod640Qam | ModulationType.Mod256Qam,
      MCETerrestrialATSC = ModulationType.Mod8Vsb,
      MCEAnalogTv = ModulationType.ModRF,
      MCEAll_TV = unchecked((int)0xffffffff),
    }

    /// <summary>
    /// From RollOff
    /// </summary>
    public enum RollOff
    {
      NotSet = -1,
      NotDefined = 0,
      Twenty = 1,
      TwentyFive,
      ThirtyFive,
      Max
    }

    /// <summary>
    /// From Pilot
    /// </summary>
    public enum Pilot
    {
      NotSet = -1,
      NotDefined = 0,
      Off = 1,
      On,
      Max
    }

    /// <summary>
    /// From ApplicationTypeType
    /// </summary>
    public enum ApplicationTypeType
    {
      SCTE28ConditionalAccess = 0,
      SCTE28PODHostBindingInformation,
      SCTE28IPService,
      SCTE28NetworkInterfaceSCTE55_2,
      SCTE28NetworkInterfaceSCTE55_1,
      SCTE28CopyProtection,
      SCTE28Diagnostic,
      SCTE28Undesignated,
      SCTE28Reserved,
    }

#endif

    /// <summary>
    /// From FECMethod
    /// </summary>
    public enum FECMethod
    {
        MethodNotSet = -1,
        MethodNotDefined = 0,
        Viterbi = 1, // FEC is a Viterbi Binary Convolution.
        RS204_188, // The FEC is Reed-Solomon 204/188 (outer FEC)
        Ldpc,
        Bch,
        RS147_130,
        Max,
    }

    /// <summary>
    /// From BinaryConvolutionCodeRate
    /// </summary>
    public enum BinaryConvolutionCodeRate
    {
        RateNotSet = -1,
        RateNotDefined = 0,
        Rate1_2 = 1, // 1/2
        Rate2_3, // 2/3
        Rate3_4, // 3/4
        Rate3_5,
        Rate4_5,
        Rate5_6, // 5/6
        Rate5_11,
        Rate7_8, // 7/8
        Rate1_4,
        Rate1_3,
        Rate2_5,
        Rate6_7,
        Rate8_9,
        Rate9_10,
        RateMax
    }

    /// <summary>
    /// From Polarisation
    /// </summary>
    public enum Polarisation
    {
        NotSet = -1,
        NotDefined = 0,
        LinearH = 1, // Linear horizontal polarisation
        LinearV, // Linear vertical polarisation
        CircularL, // Circular left polarisation
        CircularR, // Circular right polarisation
        Max,
    }

    /// <summary>
    /// From SpectralInversion
    /// </summary>
    public enum SpectralInversion
    {
        NotSet = -1,
        NotDefined = 0,
        Automatic = 1,
        Normal,
        Inverted,
        Max
    }

    /// <summary>
    /// From ModulationType
    /// </summary>
    public enum ModulationType
    {
        ModNotSet = -1,
        ModNotDefined = 0,
        Mod16Qam = 1,
        Mod32Qam,
        Mod64Qam,
        Mod80Qam,
        Mod96Qam,
        Mod112Qam,
        Mod128Qam,
        Mod160Qam,
        Mod192Qam,
        Mod224Qam,
        Mod256Qam,
        Mod320Qam,
        Mod384Qam,
        Mod448Qam,
        Mod512Qam,
        Mod640Qam,
        Mod768Qam,
        Mod896Qam,
        Mod1024Qam,
        ModQpsk,
        ModBpsk,
        ModOqpsk,
        Mod8Vsb,
        Mod16Vsb,
        ModAnalogAmplitude, // std am
        ModAnalogFrequency, // std fm
        Mod8Psk,
        ModRF,
        Mod16Apsk,
        Mod32Apsk,
        ModNbcQpsk,
        ModNbc8Psk,
        ModDirectTv,
        ModMax
    }

    /// <summary>
    /// From DVBSystemType
    /// </summary>
    public enum DVBSystemType
    {
        Cable,
        Terrestrial,
        Satellite,
    }

    /// <summary>
    /// From HierarchyAlpha
    /// </summary>
    public enum HierarchyAlpha
    {
        HAlphaNotSet = -1,
        HAlphaNotDefined = 0,
        HAlpha1 = 1, // Hierarchy alpha is 1.
        HAlpha2, // Hierarchy alpha is 2.
        HAlpha4, // Hierarchy alpha is 4.
        HAlphaMax,
    }

    /// <summary>
    /// From GuardInterval
    /// </summary>
    public enum GuardInterval
    {
        GuardNotSet = -1,
        GuardNotDefined = 0,
        Guard1_32 = 1, // Guard interval is 1/32
        Guard1_16, // Guard interval is 1/16
        Guard1_8, // Guard interval is 1/8
        Guard1_4, // Guard interval is 1/4
        GuardMax,
    }

    /// <summary>
    /// From TransmissionMode
    /// </summary>
    public enum TransmissionMode
    {
        ModeNotSet = -1,
        ModeNotDefined = 0,
        Mode2K = 1, // Transmission uses 1705 carriers (use a 2K FFT)
        Mode8K, // Transmission uses 6817 carriers (use an 8K FFT)
        Mode4K,
        Mode2KInterleaved,
        Mode4KInterleaved,
        ModeMax,
    }

    /// <summary>
    /// From ComponentStatus
    /// </summary>
    public enum ComponentStatus
    {
        Active,
        Inactive,
        Unavailable
    }

    /// <summary>
    /// From ComponentCategory
    /// </summary>
    public enum ComponentCategory
    {
        NotSet = -1,
        Other = 0,
        Video,
        Audio,
        Text,
        Data
    }

    /// <summary>
    /// From MPEG2StreamType
    /// </summary>
    public enum MPEG2StreamType
    {
        BdaUninitializedMpeg2StreamType = -1,
        Reserved1 = 0x00,
        IsoIec11172_2_Video = 0x01,
        IsoIec13818_2_Video = 0x02,
        IsoIec11172_3_Audio = 0x03,
        IsoIec13818_3_Audio = 0x04,
        IsoIec13818_1_PrivateSection = 0x05,
        IsoIec13818_1_Pes = 0x06,
        IsoIec13522_Mheg = 0x07,
        AnnexADsmCC = 0x08,
        ItuTRecH222_1 = 0x09,
        IsoIec13818_6_TypeA = 0x0a,
        IsoIec13818_6_TypeB = 0x0b,
        IsoIec13818_6_TypeC = 0x0c,
        IsoIec13818_6_TypeD = 0x0d,
        IsoIec13818_1_Auxiliary = 0x0e,
        IsoIec13818_1_Reserved = 0x0f,
        UserPrivate = 0x10,
        IsoIecUserPrivate = 0x80,
        DolbyAc3Audio = 0x81
    }

    /// <summary>
    /// From ATSCComponentTypeFlags
    /// </summary>
    [Flags]
    public enum ATSCComponentTypeFlags
    {
        None = 0x0,
        ATSCCT_AC3 = 0x00000001
    }

    /// <summary>
    /// From BDA_TEMPLATE_CONNECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BDATemplateConnection
    {
        public int FromNodeType;
        public int FromNodePinType;
        public int ToNodeType;
        public int ToNodePinType;
    }

    /// <summary>
    /// From BDA_Comp_Flags
    /// </summary>
    [Flags]
    public enum BDACompFlags
    {
        NotDefined = 0x00000000, // BDACOMP_NOT_DEFINED
        ExcludeTSFromTR = 0x00000001, // BDACOMP_EXCLUDE_TS_FROM_TR
        IncludeLocatorInTR = 0x00000002, // BDACOMP_INCLUDE_LOCATOR_IN_TR
    }


    #endregion
}
