using System;
using MissionPlanner.Utilities;
using System.Collections.Generic;

namespace NMEA2000
{
    public class NMEA2000msgs
    {
        public static readonly (int, Type)[] msgs = {

                (59392, typeof(isoAcknowledgement)),
                (59904, typeof(isoRequest)),
                (60160, typeof(isoTransportProtocolDataTransfer)),
                (60416, typeof(isoTransportProtocolConnectionManagementRequestToSend)),
                (60416, typeof(isoTransportProtocolConnectionManagementClearToSend)),
                (60416, typeof(isoTransportProtocolConnectionManagementEndOfMessage)),
                (60416, typeof(isoTransportProtocolConnectionManagementBroadcastAnnounce)),
                (60416, typeof(isoTransportProtocolConnectionManagementAbort)),
                (60928, typeof(isoAddressClaim)),
                (61184, typeof(seatalkWirelessKeypadLightControl)),
                (61184, typeof(seatalkWirelessKeypadLightControl)),
                (61184, typeof(victronBatteryRegister)),
                (61184, typeof(manufacturerProprietarySingleFrameAddressed)),
                (61440, typeof(unknownSingleFrameNonAddressed)),
                (65001, typeof(bus1PhaseCBasicAcQuantities)),
                (65002, typeof(bus1PhaseBBasicAcQuantities)),
                (65003, typeof(bus1PhaseABasicAcQuantities)),
                (65004, typeof(bus1AverageBasicAcQuantities)),
                (65005, typeof(utilityTotalAcEnergy)),
                (65006, typeof(utilityPhaseCAcReactivePower)),
                (65007, typeof(utilityPhaseCAcPower)),
                (65008, typeof(utilityPhaseCBasicAcQuantities)),
                (65009, typeof(utilityPhaseBAcReactivePower)),
                (65010, typeof(utilityPhaseBAcPower)),
                (65011, typeof(utilityPhaseBBasicAcQuantities)),
                (65012, typeof(utilityPhaseAAcReactivePower)),
                (65013, typeof(utilityPhaseAAcPower)),
                (65014, typeof(utilityPhaseABasicAcQuantities)),
                (65015, typeof(utilityTotalAcReactivePower)),
                (65016, typeof(utilityTotalAcPower)),
                (65017, typeof(utilityAverageBasicAcQuantities)),
                (65018, typeof(generatorTotalAcEnergy)),
                (65019, typeof(generatorPhaseCAcReactivePower)),
                (65020, typeof(generatorPhaseCAcPower)),
                (65021, typeof(generatorPhaseCBasicAcQuantities)),
                (65022, typeof(generatorPhaseBAcReactivePower)),
                (65023, typeof(generatorPhaseBAcPower)),
                (65024, typeof(generatorPhaseBBasicAcQuantities)),
                (65025, typeof(generatorPhaseAAcReactivePower)),
                (65026, typeof(generatorPhaseAAcPower)),
                (65027, typeof(generatorPhaseABasicAcQuantities)),
                (65028, typeof(generatorTotalAcReactivePower)),
                (65029, typeof(generatorTotalAcPower)),
                (65030, typeof(generatorAverageBasicAcQuantities)),
                (65240, typeof(isoCommandedAddress)),
                (65280, typeof(furunoHeave)),
                (65280, typeof(manufacturerProprietarySingleFrameNonAddressed)),
                (65284, typeof(maretronProprietaryDcBreakerCurrent)),
                (65285, typeof(airmarBootStateAcknowledgment)),
                (65285, typeof(lowranceTemperature)),
                (65286, typeof(chetcoDimmer)),
                (65286, typeof(airmarBootStateRequest)),
                (65287, typeof(airmarAccessLevel)),
                (65287, typeof(simnetConfigureTemperatureSensor)),
                (65288, typeof(seatalkAlarm)),
                (65289, typeof(simnetTrimTabSensorCalibration)),
                (65290, typeof(simnetPaddleWheelSpeedConfiguration)),
                (65292, typeof(simnetClearFluidLevelWarnings)),
                (65293, typeof(simnetLgc2000Configuration)),
                (65309, typeof(navicoWirelessBatteryStatus)),
                (65312, typeof(navicoWirelessSignalStatus)),
                (65325, typeof(simnetReprogramStatus)),
                (65341, typeof(simnetAutopilotMode)),
                (65345, typeof(seatalkPilotWindDatum)),
                (65359, typeof(seatalkPilotHeading)),
                (65360, typeof(seatalkPilotLockedHeading)),
                (65361, typeof(seatalkSilenceAlarm)),
                (65371, typeof(seatalkKeypadMessage)),
                (65374, typeof(seatalkKeypadHeartbeat)),
                (65379, typeof(seatalkPilotMode)),
                (65408, typeof(airmarDepthQualityFactor)),
                (65409, typeof(airmarSpeedPulseCount)),
                (65410, typeof(airmarDeviceInformation)),
                (65480, typeof(simnetAutopilotMode)),
                (65536, typeof(unknownFastPacketAddressed)),
                (126208, typeof(nmeaRequestGroupFunction)),
                (126208, typeof(nmeaCommandGroupFunction)),
                (126208, typeof(nmeaAcknowledgeGroupFunction)),
                (126208, typeof(nmeaReadFieldsGroupFunction)),
                (126208, typeof(nmeaReadFieldsReplyGroupFunction)),
                (126208, typeof(nmeaWriteFieldsGroupFunction)),
                (126208, typeof(nmeaWriteFieldsReplyGroupFunction)),
                (126464, typeof(pgnListTransmitAndReceive)),
                (126720, typeof(seatalk1PilotMode)),
                (126720, typeof(fusionMediaControl)),
                (126720, typeof(fusionSiriusControl)),
                (126720, typeof(fusionRequestStatus)),
                (126720, typeof(fusionSetSource)),
                (126720, typeof(fusionMute)),
                (126720, typeof(fusionSetZoneVolume)),
                (126720, typeof(fusionSetAllVolumes)),
                (126720, typeof(seatalk1Keystroke)),
                (126720, typeof(seatalk1DeviceIdentification)),
                (126720, typeof(airmarAttitudeOffset)),
                (126720, typeof(airmarCalibrateCompass)),
                (126720, typeof(airmarTrueWindOptions)),
                (126720, typeof(airmarSimulateMode)),
                (126720, typeof(airmarCalibrateDepth)),
                (126720, typeof(airmarCalibrateSpeed)),
                (126720, typeof(airmarCalibrateTemperature)),
                (126720, typeof(airmarSpeedFilter)),
                (126720, typeof(airmarTemperatureFilter)),
                (126720, typeof(airmarNmea2000Options)),
                (126720, typeof(airmarAddressableMultiFrame)),
                (126720, typeof(maretronSlaveResponse)),
                (126720, typeof(manufacturerProprietaryFastPacketAddressed)),
                (126976, typeof(unknownFastPacketNonAddressed)),
                (126983, typeof(alert)),
                (126984, typeof(alertResponse)),
                (126985, typeof(alertText)),
                (126986, typeof(alertConfiguration)),
                (126987, typeof(alertThreshold)),
                (126988, typeof(alertValue)),
                (126992, typeof(systemTime)),
                (126993, typeof(heartbeat)),
                (126996, typeof(productInformation)),
                (126998, typeof(configurationInformation)),
                (127233, typeof(manOverboardNotification)),
                (127237, typeof(headingTrackControl)),
                (127245, typeof(rudder)),
                (127250, typeof(vesselHeading)),
                (127251, typeof(rateOfTurn)),
                (127252, typeof(heave)),
                (127257, typeof(attitude)),
                (127258, typeof(magneticVariation)),
                (127488, typeof(engineParametersRapidUpdate)),
                (127489, typeof(engineParametersDynamic)),
                (127493, typeof(transmissionParametersDynamic)),
                (127496, typeof(tripParametersVessel)),
                (127497, typeof(tripParametersEngine)),
                (127498, typeof(engineParametersStatic)),
                (127500, typeof(loadControllerConnectionStateControl)),
                (127501, typeof(binarySwitchBankStatus)),
                (127502, typeof(switchBankControl)),
                (127503, typeof(acInputStatus)),
                (127504, typeof(acOutputStatus)),
                (127505, typeof(fluidLevel)),
                (127506, typeof(dcDetailedStatus)),
                (127507, typeof(chargerStatus)),
                (127508, typeof(batteryStatus)),
                (127509, typeof(inverterStatus)),
                (127510, typeof(chargerConfigurationStatus)),
                (127511, typeof(inverterConfigurationStatus)),
                (127512, typeof(agsConfigurationStatus)),
                (127513, typeof(batteryConfigurationStatus)),
                (127514, typeof(agsStatus)),
                (127744, typeof(acPowerCurrentPhaseA)),
                (127745, typeof(acPowerCurrentPhaseB)),
                (127746, typeof(acPowerCurrentPhaseC)),
                (127750, typeof(converterStatus)),
                (127751, typeof(dcVoltageCurrent)),
                (128000, typeof(leewayAngle)),
                (128006, typeof(thrusterControlStatus)),
                (128007, typeof(thrusterInformation)),
                (128008, typeof(thrusterMotorStatus)),
                (128259, typeof(speed)),
                (128267, typeof(waterDepth)),
                (128275, typeof(distanceLog)),
                (128520, typeof(trackedTargetData)),
                (128776, typeof(windlassControlStatus)),
                (128777, typeof(anchorWindlassOperatingStatus)),
                (128778, typeof(anchorWindlassMonitoringStatus)),
                (129025, typeof(positionRapidUpdate)),
                (129026, typeof(cogSogRapidUpdate)),
                (129027, typeof(positionDeltaRapidUpdate)),
                (129028, typeof(altitudeDeltaRapidUpdate)),
                (129029, typeof(gnssPositionData)),
                (129033, typeof(timeDate)),
                (129038, typeof(aisClassAPositionReport)),
                (129039, typeof(aisClassBPositionReport)),
                (129040, typeof(aisClassBExtendedPositionReport)),
                (129041, typeof(aisAidsToNavigationAtonReport)),
                (129044, typeof(datum)),
                (129045, typeof(userDatum)),
                (129283, typeof(crossTrackError)),
                (129284, typeof(navigationData)),
                (129285, typeof(navigationRouteWpInformation)),
                (129291, typeof(setDriftRapidUpdate)),
                (129301, typeof(navigationRouteTimeToFromMark)),
                (129302, typeof(bearingAndDistanceBetweenTwoMarks)),
                (129538, typeof(gnssControlStatus)),
                (129539, typeof(gnssDops)),
                (129540, typeof(gnssSatsInView)),
                (129541, typeof(gpsAlmanacData)),
                (129542, typeof(gnssPseudorangeNoiseStatistics)),
                (129545, typeof(gnssRaimOutput)),
                (129546, typeof(gnssRaimSettings)),
                (129547, typeof(gnssPseudorangeErrorStatistics)),
                (129549, typeof(dgnssCorrections)),
                (129550, typeof(gnssDifferentialCorrectionReceiverInterface)),
                (129551, typeof(gnssDifferentialCorrectionReceiverSignal)),
                (129556, typeof(glonassAlmanacData)),
                (129792, typeof(aisDgnssBroadcastBinaryMessage)),
                (129793, typeof(aisUtcAndDateReport)),
                (129794, typeof(aisClassAStaticAndVoyageRelatedData)),
                (129795, typeof(aisAddressedBinaryMessage)),
                (129796, typeof(aisAcknowledge)),
                (129797, typeof(aisBinaryBroadcastMessage)),
                (129798, typeof(aisSarAircraftPositionReport)),
                (129799, typeof(radioFrequencyModePower)),
                (129800, typeof(aisUtcDateInquiry)),
                (129801, typeof(aisAddressedSafetyRelatedMessage)),
                (129802, typeof(aisSafetyRelatedBroadcastMessage)),
                (129803, typeof(aisInterrogation)),
                (129804, typeof(aisAssignmentModeCommand)),
                (129805, typeof(aisDataLinkManagementMessage)),
                (129806, typeof(aisChannelManagement)),
                (129807, typeof(aisClassBGroupAssignment)),
                (129808, typeof(dscDistressCallInformation)),
                (129808, typeof(dscCallInformation)),
                (129809, typeof(aisClassBStaticDataMsg24PartA)),
                (129810, typeof(aisClassBStaticDataMsg24PartB)),
                (130060, typeof(label)),
                (130061, typeof(channelSourceConfiguration)),
                (130064, typeof(routeAndWpServiceDatabaseList)),
                (130065, typeof(routeAndWpServiceRouteList)),
                (130066, typeof(routeAndWpServiceRouteWpListAttributes)),
                (130067, typeof(routeAndWpServiceRouteWpNamePosition)),
                (130068, typeof(routeAndWpServiceRouteWpName)),
                (130069, typeof(routeAndWpServiceXteLimitNavigationMethod)),
                (130070, typeof(routeAndWpServiceWpComment)),
                (130071, typeof(routeAndWpServiceRouteComment)),
                (130072, typeof(routeAndWpServiceDatabaseComment)),
                (130073, typeof(routeAndWpServiceRadiusOfTurn)),
                (130074, typeof(routeAndWpServiceWpListWpNamePosition)),
                (130306, typeof(windData)),
                (130310, typeof(environmentalParameters)),
                (130311, typeof(environmentalParameters)),
                (130312, typeof(temperature)),
                (130313, typeof(humidity)),
                (130314, typeof(actualPressure)),
                (130315, typeof(setPressure)),
                (130316, typeof(temperatureExtendedRange)),
                (130320, typeof(tideStationData)),
                (130321, typeof(salinityStationData)),
                (130322, typeof(currentStationData)),
                (130323, typeof(meteorologicalStationData)),
                (130324, typeof(mooredBuoyStationData)),
                (130560, typeof(payloadMass)),
                (130567, typeof(watermakerInputSettingAndStatus)),
                (130569, typeof(currentStatusAndFile)),
                (130570, typeof(libraryDataFile)),
                (130571, typeof(libraryDataGroup)),
                (130572, typeof(libraryDataSearch)),
                (130573, typeof(supportedSourceData)),
                (130574, typeof(supportedZoneData)),
                (130576, typeof(smallCraftStatus)),
                (130577, typeof(directionData)),
                (130578, typeof(vesselSpeedComponents)),
                (130579, typeof(systemConfiguration)),
                (130580, typeof(systemConfigurationDeprecated)),
                (130581, typeof(zoneConfigurationDeprecated)),
                (130582, typeof(zoneVolume)),
                (130583, typeof(availableAudioEqPresets)),
                (130584, typeof(availableBluetoothAddresses)),
                (130585, typeof(bluetoothSourceStatus)),
                (130586, typeof(zoneConfiguration)),
                (130816, typeof(sonichubInit2)),
                (130816, typeof(sonichubAmRadio)),
                (130816, typeof(sonichubZoneInfo)),
                (130816, typeof(sonichubSource)),
                (130816, typeof(sonichubSourceList)),
                (130816, typeof(sonichubControl)),
                (130816, typeof(sonichubUnknown)),
                (130816, typeof(sonichubFmRadio)),
                (130816, typeof(sonichubPlaylist)),
                (130816, typeof(sonichubTrack)),
                (130816, typeof(sonichubArtist)),
                (130816, typeof(sonichubAlbum)),
                (130816, typeof(sonichubMenuItem)),
                (130816, typeof(sonichubZones)),
                (130816, typeof(sonichubMaxVolume)),
                (130816, typeof(sonichubVolume)),
                (130816, typeof(sonichubInit1)),
                (130816, typeof(sonichubPosition)),
                (130816, typeof(sonichubInit3)),
                (130816, typeof(simradTextMessage)),
                (130816, typeof(manufacturerProprietaryFastPacketNonAddressed)),
                (130817, typeof(navicoProductInformation)),
                (130818, typeof(simnetReprogramData)),
                (130819, typeof(simnetRequestReprogram)),
                (130820, typeof(simnetReprogramStatus)),
                (130820, typeof(furunoUnknown)),
                (130820, typeof(fusionSourceName)),
                (130820, typeof(fusionTrackInfo)),
                (130820, typeof(fusionTrack)),
                (130820, typeof(fusionArtist)),
                (130820, typeof(fusionAlbum)),
                (130820, typeof(fusionUnitName)),
                (130820, typeof(fusionZoneName)),
                (130820, typeof(fusionPlayProgress)),
                (130820, typeof(fusionAmFmStation)),
                (130820, typeof(fusionVhf)),
                (130820, typeof(fusionSquelch)),
                (130820, typeof(fusionScan)),
                (130820, typeof(fusionMenuItem)),
                (130820, typeof(fusionReplay)),
                (130820, typeof(fusionMute)),
                (130820, typeof(fusionSubVolume)),
                (130820, typeof(fusionTone)),
                (130820, typeof(fusionVolume)),
                (130820, typeof(fusionPowerState)),
                (130820, typeof(fusionSiriusxmChannel)),
                (130820, typeof(fusionSiriusxmTitle)),
                (130820, typeof(fusionSiriusxmArtist)),
                (130820, typeof(fusionSiriusxmGenre)),
                (130821, typeof(furunoUnknown)),
                (130823, typeof(maretronProprietaryTemperatureHighRange)),
                (130824, typeof(bGWindData)),
                (130824, typeof(maretronAnnunciator)),
                (130827, typeof(lowranceUnknown)),
                (130828, typeof(simnetSetSerialNumber)),
                (130831, typeof(suzukiEngineAndStorageDeviceConfig)),
                (130832, typeof(simnetFuelUsedHighResolution)),
                (130834, typeof(simnetEngineAndTankConfiguration)),
                (130835, typeof(simnetSetEngineAndTankConfiguration)),
                (130836, typeof(simnetFluidLevelSensorConfiguration)),
                (130836, typeof(maretronProprietarySwitchStatusCounter)),
                (130837, typeof(simnetFuelFlowTurbineConfiguration)),
                (130837, typeof(maretronProprietarySwitchStatusTimer)),
                (130838, typeof(simnetFluidLevelWarning)),
                (130839, typeof(simnetPressureSensorConfiguration)),
                (130840, typeof(simnetDataUserGroupConfiguration)),
                (130842, typeof(simnetAisClassBStaticDataMsg24PartA)),
                (130842, typeof(furunoSixDegreesOfFreedomMovement)),
                (130842, typeof(simnetAisClassBStaticDataMsg24PartB)),
                (130843, typeof(furunoHeelAngleRollInformation)),
                (130843, typeof(simnetSonarStatusFrequencyAndDspVoltage)),
                (130845, typeof(simnetCompassHeadingOffset)),
                (130845, typeof(furunoMultiSatsInViewExtended)),
                (130845, typeof(simnetCompassLocalField)),
                (130845, typeof(simnetCompassFieldAngle)),
                (130845, typeof(simnetParameterHandle)),
                (130846, typeof(furunoMotionSensorStatusExtended)),
                (130847, typeof(seatalkNodeStatistics)),
                (130850, typeof(simnetEventCommandApCommand)),
                (130850, typeof(simnetEventCommandAlarm)),
                (130850, typeof(simnetEventCommandUnknown)),
                (130851, typeof(simnetEventReplyApCommand)),
                (130856, typeof(simnetAlarmMessage)),
                (130880, typeof(airmarAdditionalWeatherData)),
                (130881, typeof(airmarHeaterControl)),
                (130944, typeof(airmarPost)),
};
    }

    public interface INMEA2000
    {
        void SetPacketData(byte[] packet);
    }

 /// ISO Acknowledgement
 /// Single
 /// 59392
 public class isoAcknowledgement : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoAcknowledgement(byte[] packet) { _packet = packet; }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int groupFunction { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 24, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 1); } set { }  }
}
 /// ISO Request
 /// Single
 /// 59904
 public class isoRequest : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoRequest(byte[] packet) { _packet = packet; }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 0, 24, false, 1); } set { }  }
}
 /// ISO Transport Protocol, Data Transfer
 /// Single
 /// 60160
 public class isoTransportProtocolDataTransfer : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolDataTransfer(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int data { get { return _packet.GetBitOffsetLength<int>(0, 8, 56, false, 0); } set { }  }
}
 /// ISO Transport Protocol, Connection Management - Request To Send
 /// Single
 /// 60416
 public class isoTransportProtocolConnectionManagementRequestToSend : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolConnectionManagementRequestToSend(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int messageSize { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int packets { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int packetsReply { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 1); } set { }  }
}
 /// ISO Transport Protocol, Connection Management - Clear To Send
 /// Single
 /// 60416
 public class isoTransportProtocolConnectionManagementClearToSend : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolConnectionManagementClearToSend(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int maxPackets { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int nextSid { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 24, 16, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 1); } set { }  }
}
 /// ISO Transport Protocol, Connection Management - End Of Message
 /// Single
 /// 60416
 public class isoTransportProtocolConnectionManagementEndOfMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolConnectionManagementEndOfMessage(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int totalMessageSize { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int totalNumberOfPacketsReceived { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 32, 8, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 1); } set { }  }
}
 /// ISO Transport Protocol, Connection Management - Broadcast Announce
 /// Single
 /// 60416
 public class isoTransportProtocolConnectionManagementBroadcastAnnounce : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolConnectionManagementBroadcastAnnounce(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int messageSize { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int packets { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 32, 8, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 1); } set { }  }
}
 /// ISO Transport Protocol, Connection Management - Abort
 /// Single
 /// 60416
 public class isoTransportProtocolConnectionManagementAbort : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoTransportProtocolConnectionManagementAbort(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public byte[] reason { get { return _packet.GetBitOffsetLength<byte[]>(0, 8, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 16, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 32, 24, false, 1); } set { }  }
}
 /// ISO Address Claim
 /// Single
 /// 60928
 public class isoAddressClaim : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoAddressClaim(byte[] packet) { _packet = packet; }
    public byte[] uniqueNumber { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 21, false, 0); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(5, 21, 11, false, 0); } set { }  }
    public int deviceInstanceLower { get { return _packet.GetBitOffsetLength<int>(0, 32, 3, false, 0); } set { }  }
    public int deviceInstanceUpper { get { return _packet.GetBitOffsetLength<int>(3, 35, 5, false, 0); } set { }  }
    public int deviceFunction { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 48, 1, false, 0); } set { }  }
    public string deviceClass { get { return _packet.GetBitOffsetLength<string>(1, 49, 7, false, 0); } set { }  }
    public int systemInstance { get { return _packet.GetBitOffsetLength<int>(0, 56, 4, false, 0); } set { }  }
    public string industryGroup { get { return _packet.GetBitOffsetLength<string>(4, 60, 3, false, 0); } set { }  }
}
 /// Seatalk: Wireless Keypad Light Control
 /// Single
 /// 61184
 public class seatalkWirelessKeypadLightControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkWirelessKeypadLightControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int variant { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int wirelessSetting { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int wiredSetting { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
}
 /// Victron Battery Register
 /// Single
 /// 61184
 public class victronBatteryRegister : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public victronBatteryRegister(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int registerId { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int payload { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Manufacturer Proprietary single-frame addressed
 /// Single
 /// 61184
 public class manufacturerProprietarySingleFrameAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public manufacturerProprietarySingleFrameAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 48, false, 0); } set { }  }
}
 /// Unknown single-frame non-addressed
 /// Single
 /// 61440
 public class unknownSingleFrameNonAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public unknownSingleFrameNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 48, false, 0); } set { }  }
}
 /// Bus #1 Phase C Basic AC Quantities
 /// Single
 /// 65001
 public class bus1PhaseCBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bus1PhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
}
 /// Bus #1 Phase B Basic AC Quantities
 /// Single
 /// 65002
 public class bus1PhaseBBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bus1PhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
}
 /// Bus #1 Phase A Basic AC Quantities
 /// Single
 /// 65003
 public class bus1PhaseABasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bus1PhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
}
 /// Bus #1 Average Basic AC Quantities
 /// Single
 /// 65004
 public class bus1AverageBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bus1AverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
}
 /// Utility Total AC Energy
 /// Single
 /// 65005
 public class utilityTotalAcEnergy : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityTotalAcEnergy(byte[] packet) { _packet = packet; }
    public int totalEnergyExport { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 0); } set { }  }
    public int totalEnergyImport { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Utility Phase C AC Reactive Power
 /// Single
 /// 65006
 public class utilityPhaseCAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseCAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Utility Phase C AC Power
 /// Single
 /// 65007
 public class utilityPhaseCAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseCAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// Utility Phase C Basic AC Quantities
 /// Single
 /// 65008
 public class utilityPhaseCBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Utility Phase B AC Reactive Power
 /// Single
 /// 65009
 public class utilityPhaseBAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseBAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Utility Phase B AC Power
 /// Single
 /// 65010
 public class utilityPhaseBAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseBAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// Utility Phase B Basic AC Quantities
 /// Single
 /// 65011
 public class utilityPhaseBBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Utility Phase A AC Reactive Power
 /// Single
 /// 65012
 public class utilityPhaseAAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseAAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 48, 2, false, 0); } set { }  }
}
 /// Utility Phase A AC Power
 /// Single
 /// 65013
 public class utilityPhaseAAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseAAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// Utility Phase A Basic AC Quantities
 /// Single
 /// 65014
 public class utilityPhaseABasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityPhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Utility Total AC Reactive Power
 /// Single
 /// 65015
 public class utilityTotalAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityTotalAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 48, 2, false, 0); } set { }  }
}
 /// Utility Total AC Power
 /// Single
 /// 65016
 public class utilityTotalAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityTotalAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// Utility Average Basic AC Quantities
 /// Single
 /// 65017
 public class utilityAverageBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public utilityAverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Generator Total AC Energy
 /// Single
 /// 65018
 public class generatorTotalAcEnergy : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorTotalAcEnergy(byte[] packet) { _packet = packet; }
    public int totalEnergyExport { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 0); } set { }  }
    public int totalEnergyImport { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Generator Phase C AC Reactive Power
 /// Single
 /// 65019
 public class generatorPhaseCAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseCAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Generator Phase C AC Power
 /// Single
 /// 65020
 public class generatorPhaseCAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseCAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
}
 /// Generator Phase C Basic AC Quantities
 /// Single
 /// 65021
 public class generatorPhaseCBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Generator Phase B AC Reactive Power
 /// Single
 /// 65022
 public class generatorPhaseBAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseBAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Generator Phase B AC Power
 /// Single
 /// 65023
 public class generatorPhaseBAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseBAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
}
 /// Generator Phase B Basic AC Quantities
 /// Single
 /// 65024
 public class generatorPhaseBBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Generator Phase A AC Reactive Power
 /// Single
 /// 65025
 public class generatorPhaseAAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseAAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Generator Phase A AC Power
 /// Single
 /// 65026
 public class generatorPhaseAAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseAAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Generator Phase A Basic AC Quantities
 /// Single
 /// 65027
 public class generatorPhaseABasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorPhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Generator Total AC Reactive Power
 /// Single
 /// 65028
 public class generatorTotalAcReactivePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorTotalAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
}
 /// Generator Total AC Power
 /// Single
 /// 65029
 public class generatorTotalAcPower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorTotalAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Generator Average Basic AC Quantities
 /// Single
 /// 65030
 public class generatorAverageBasicAcQuantities : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public generatorAverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int acFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// ISO Commanded Address
 /// Iso
 /// 65240
 public class isoCommandedAddress : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public isoCommandedAddress(byte[] packet) { _packet = packet; }
    public byte[] uniqueNumber { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 21, false, 0); } set { }  }
    public int manufacturerCode { get { return _packet.GetBitOffsetLength<int>(5, 21, 11, false, 0); } set { }  }
    public int deviceInstanceLower { get { return _packet.GetBitOffsetLength<int>(0, 32, 3, false, 0); } set { }  }
    public int deviceInstanceUpper { get { return _packet.GetBitOffsetLength<int>(3, 35, 5, false, 0); } set { }  }
    public int deviceFunction { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 48, 1, false, 0); } set { }  }
    public string deviceClass { get { return _packet.GetBitOffsetLength<string>(1, 49, 7, false, 0); } set { }  }
    public int systemInstance { get { return _packet.GetBitOffsetLength<int>(0, 56, 4, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(4, 60, 3, false, 0); } set { }  }
    public int newSourceAddress { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
}
 /// Furuno: Heave
 /// Single
 /// 65280
 public class furunoHeave : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoHeave(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int heave { get { return _packet.GetBitOffsetLength<int>(0, 16, 32, true, 0.001); } set { }  }
}
 /// Manufacturer Proprietary single-frame non-addressed
 /// Single
 /// 65280
 public class manufacturerProprietarySingleFrameNonAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public manufacturerProprietarySingleFrameNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 48, false, 0); } set { }  }
}
 /// Maretron: Proprietary DC Breaker Current
 /// Single
 /// 65284
 public class maretronProprietaryDcBreakerCurrent : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronProprietaryDcBreakerCurrent(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int bankInstance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int breakerCurrent { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.1); } set { }  }
}
 /// Airmar: Boot State Acknowledgment
 /// Single
 /// 65285
 public class airmarBootStateAcknowledgment : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarBootStateAcknowledgment(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string bootState { get { return _packet.GetBitOffsetLength<string>(0, 16, 4, false, 0); } set { }  }
}
 /// Lowrance: Temperature
 /// Single
 /// 65285
 public class lowranceTemperature : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public lowranceTemperature(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string temperatureSource { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
}
 /// Chetco: Dimmer
 /// Single
 /// 65286
 public class chetcoDimmer : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public chetcoDimmer(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public int industryCode { get { return _packet.GetBitOffsetLength<int>(5, 13, 3, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int dimmer1 { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int dimmer2 { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int dimmer3 { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int dimmer4 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int control { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Airmar: Boot State Request
 /// Single
 /// 65286
 public class airmarBootStateRequest : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarBootStateRequest(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Airmar: Access Level
 /// Single
 /// 65287
 public class airmarAccessLevel : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarAccessLevel(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string formatCode { get { return _packet.GetBitOffsetLength<string>(0, 16, 3, false, 0); } set { }  }
    public string accessLevel { get { return _packet.GetBitOffsetLength<string>(3, 19, 3, false, 0); } set { }  }
    public int accessSeedKey { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 1); } set { }  }
}
 /// Simnet: Configure Temperature Sensor
 /// Single
 /// 65287
 public class simnetConfigureTemperatureSensor : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetConfigureTemperatureSensor(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Seatalk: Alarm
 /// Single
 /// 65288
 public class seatalkAlarm : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 8, false, 0); } set { }  }
    public string alarmStatus { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string alarmId { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string alarmGroup { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public byte[] alarmPriority { get { return _packet.GetBitOffsetLength<byte[]>(0, 48, 16, false, 0); } set { }  }
}
 /// Simnet: Trim Tab Sensor Calibration
 /// Single
 /// 65289
 public class simnetTrimTabSensorCalibration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetTrimTabSensorCalibration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Paddle Wheel Speed Configuration
 /// Single
 /// 65290
 public class simnetPaddleWheelSpeedConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetPaddleWheelSpeedConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Clear Fluid Level Warnings
 /// Single
 /// 65292
 public class simnetClearFluidLevelWarnings : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetClearFluidLevelWarnings(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: LGC-2000 Configuration
 /// Single
 /// 65293
 public class simnetLgc2000Configuration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetLgc2000Configuration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Navico: Wireless Battery Status
 /// Single
 /// 65309
 public class navicoWirelessBatteryStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navicoWirelessBatteryStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int batteryStatus { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int batteryChargeStatus { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
}
 /// Navico: Wireless Signal Status
 /// Single
 /// 65312
 public class navicoWirelessSignalStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navicoWirelessSignalStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
}
 /// Simnet: Reprogram Status
 /// Single
 /// 65325
 public class simnetReprogramStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetReprogramStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Autopilot Mode
 /// Single
 /// 65341
 public class simnetAutopilotMode : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetAutopilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Seatalk: Pilot Wind Datum
 /// Single
 /// 65345
 public class seatalkPilotWindDatum : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkPilotWindDatum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int windDatum { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.0001); } set { }  }
    public int rollingAverageWindAngle { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0001); } set { }  }
}
 /// Seatalk: Pilot Heading
 /// Single
 /// 65359
 public class seatalkPilotHeading : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkPilotHeading(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 8, false, 0); } set { }  }
    public int headingTrue { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.0001); } set { }  }
    public int headingMagnetic { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.0001); } set { }  }
}
 /// Seatalk: Pilot Locked Heading
 /// Single
 /// 65360
 public class seatalkPilotLockedHeading : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkPilotLockedHeading(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 8, false, 0); } set { }  }
    public int targetHeadingTrue { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.0001); } set { }  }
    public int targetHeadingMagnetic { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.0001); } set { }  }
}
 /// Seatalk: Silence Alarm
 /// Single
 /// 65361
 public class seatalkSilenceAlarm : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkSilenceAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string alarmId { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public string alarmGroup { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
}
 /// Seatalk: Keypad Message
 /// Single
 /// 65371
 public class seatalkKeypadMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkKeypadMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int firstKey { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int secondKey { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int firstKeyState { get { return _packet.GetBitOffsetLength<int>(0, 40, 2, false, 0); } set { }  }
    public int secondKeyState { get { return _packet.GetBitOffsetLength<int>(2, 42, 2, false, 0); } set { }  }
    public int encoderPosition { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
}
 /// SeaTalk: Keypad Heartbeat
 /// Single
 /// 65374
 public class seatalkKeypadHeartbeat : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkKeypadHeartbeat(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int variant { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
}
 /// Seatalk: Pilot Mode
 /// Single
 /// 65379
 public class seatalkPilotMode : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkPilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] pilotMode { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 8, false, 0); } set { }  }
    public byte[] subMode { get { return _packet.GetBitOffsetLength<byte[]>(0, 24, 8, false, 0); } set { }  }
    public byte[] pilotModeData { get { return _packet.GetBitOffsetLength<byte[]>(0, 32, 8, false, 0); } set { }  }
}
 /// Airmar: Depth Quality Factor
 /// Single
 /// 65408
 public class airmarDepthQualityFactor : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarDepthQualityFactor(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public string depthQualityFactor { get { return _packet.GetBitOffsetLength<string>(0, 24, 4, false, 0); } set { }  }
}
 /// Airmar: Speed Pulse Count
 /// Single
 /// 65409
 public class airmarSpeedPulseCount : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarSpeedPulseCount(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int durationOfInterval { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.001); } set { }  }
    public int numberOfPulsesReceived { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0); } set { }  }
}
 /// Airmar: Device Information
 /// Single
 /// 65410
 public class airmarDeviceInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarDeviceInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public double internalDeviceTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
    public int supplyVoltage { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.01); } set { }  }
}
 /// Unknown fast-packet addressed
 /// Fast
 /// 65536
 public class unknownFastPacketAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public unknownFastPacketAddressed(byte[] packet) { _packet = packet; }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 2040, false, 0); } set { }  }
}
 /// NMEA - Request group function
 /// Fast
 /// 126208
 public class nmeaRequestGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaRequestGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public int transmissionInterval { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0.001); } set { }  }
    public int transmissionIntervalOffset { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0.01); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int>(0, 96, 0, false, 0); } set { }  }
}
 /// NMEA - Command group function
 /// Fast
 /// 126208
 public class nmeaCommandGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaCommandGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public string priority { get { return _packet.GetBitOffsetLength<string>(0, 32, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 36, 4, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int>(0, 56, 0, false, 0); } set { }  }
}
 /// NMEA - Acknowledge group function
 /// Fast
 /// 126208
 public class nmeaAcknowledgeGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaAcknowledgeGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public string pgnErrorCode { get { return _packet.GetBitOffsetLength<string>(0, 32, 4, false, 0); } set { }  }
    public string transmissionIntervalPriorityErrorCode { get { return _packet.GetBitOffsetLength<string>(4, 36, 4, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string parameter { get { return _packet.GetBitOffsetLength<string>(0, 48, 4, false, 0); } set { }  }
}
 /// NMEA - Read Fields group function
 /// Fast
 /// 126208
 public class nmeaReadFieldsGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaReadFieldsGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 32, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 43, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 45, 3, false, 0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int>(0, 80, 0, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
}
 /// NMEA - Read Fields reply group function
 /// Fast
 /// 126208
 public class nmeaReadFieldsReplyGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaReadFieldsReplyGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 32, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 43, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 45, 3, false, 0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int>(0, 80, 0, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int>(0, 88, 0, false, 0); } set { }  }
}
 /// NMEA - Write Fields group function
 /// Fast
 /// 126208
 public class nmeaWriteFieldsGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaWriteFieldsGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 32, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 43, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 45, 3, false, 0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int>(0, 80, 0, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int>(0, 88, 0, false, 0); } set { }  }
}
 /// NMEA - Write Fields reply group function
 /// Fast
 /// 126208
 public class nmeaWriteFieldsReplyGroupFunction : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public nmeaWriteFieldsReplyGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 32, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 43, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 45, 3, false, 0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int>(0, 80, 0, false, 0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int>(0, 88, 0, false, 0); } set { }  }
}
 /// PGN List (Transmit and Receive)
 /// Fast
 /// 126464
 public class pgnListTransmitAndReceive : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public pgnListTransmitAndReceive(byte[] packet) { _packet = packet; }
    public string functionCode { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int>(0, 8, 24, false, 1); } set { }  }
}
 /// Seatalk1: Pilot Mode
 /// Fast
 /// 126720
 public class seatalk1PilotMode : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalk1PilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public int command { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public byte[] unknown1 { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 24, false, 0); } set { }  }
    public int pilotMode { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 1); } set { }  }
    public int subMode { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public byte[] pilotModeData { get { return _packet.GetBitOffsetLength<byte[]>(0, 80, 8, false, 0); } set { }  }
    public byte[] unknown2 { get { return _packet.GetBitOffsetLength<byte[]>(0, 88, 80, false, 0); } set { }  }
}
 /// Fusion: Media Control
 /// Fast
 /// 126720
 public class fusionMediaControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionMediaControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
}
 /// Fusion: Sirius Control
 /// Fast
 /// 126720
 public class fusionSiriusControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSiriusControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
}
 /// Fusion: Request Status
 /// Fast
 /// 126720
 public class fusionRequestStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionRequestStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
}
 /// Fusion: Set Source
 /// Fast
 /// 126720
 public class fusionSetSource : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSetSource(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
}
 /// Fusion: Mute
 /// Fast
 /// 126720
 public class fusionMute : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionMute(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
}
 /// Fusion: Set Zone Volume
 /// Fast
 /// 126720
 public class fusionSetZoneVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSetZoneVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int zone { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public int volume { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
}
 /// Fusion: Set All Volumes
 /// Fast
 /// 126720
 public class fusionSetAllVolumes : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSetAllVolumes(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 1); } set { }  }
}
 /// Seatalk1: Keystroke
 /// Fast
 /// 126720
 public class seatalk1Keystroke : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalk1Keystroke(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public byte[] command { get { return _packet.GetBitOffsetLength<byte[]>(0, 32, 8, false, 0); } set { }  }
    public string device { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public string key { get { return _packet.GetBitOffsetLength<string>(0, 48, 16, false, 0); } set { }  }
    public byte[] unknownData { get { return _packet.GetBitOffsetLength<byte[]>(0, 64, 112, false, 0); } set { }  }
}
 /// Seatalk1: Device Identification
 /// Fast
 /// 126720
 public class seatalk1DeviceIdentification : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalk1DeviceIdentification(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public byte[] command { get { return _packet.GetBitOffsetLength<byte[]>(0, 32, 8, false, 0); } set { }  }
    public string device { get { return _packet.GetBitOffsetLength<string>(0, 48, 8, false, 0); } set { }  }
}
 /// Airmar: Attitude Offset
 /// Fast
 /// 126720
 public class airmarAttitudeOffset : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarAttitudeOffset(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int azimuthOffset { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.0001); } set { }  }
    public int pitchOffset { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.0001); } set { }  }
    public int rollOffset { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, true, 0.0001); } set { }  }
}
 /// Airmar: Calibrate Compass
 /// Fast
 /// 126720
 public class airmarCalibrateCompass : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarCalibrateCompass(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string calibrateFunction { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string calibrationStatus { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int verifyScore { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int xAxisGainValue { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 0.01); } set { }  }
    public int yAxisGainValue { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, true, 0.01); } set { }  }
    public int zAxisGainValue { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, true, 0.01); } set { }  }
    public int xAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(0, 96, 16, true, 0.01); } set { }  }
    public int yAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, true, 0.01); } set { }  }
    public int zAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, true, 0.01); } set { }  }
    public int xAxisAngularOffset { get { return _packet.GetBitOffsetLength<int>(0, 144, 16, true, 0.1); } set { }  }
    public int pitchAndRollDamping { get { return _packet.GetBitOffsetLength<int>(0, 160, 16, true, 0.05); } set { }  }
    public int compassRateGyroDamping { get { return _packet.GetBitOffsetLength<int>(0, 176, 16, true, 0.05); } set { }  }
}
 /// Airmar: True Wind Options
 /// Fast
 /// 126720
 public class airmarTrueWindOptions : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarTrueWindOptions(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string cogSubstitutionForHdg { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string calibrationStatus { get { return _packet.GetBitOffsetLength<string>(2, 26, 8, false, 0); } set { }  }
    public int verifyScore { get { return _packet.GetBitOffsetLength<int>(2, 34, 8, false, 1); } set { }  }
    public int xAxisGainValue { get { return _packet.GetBitOffsetLength<int>(2, 42, 16, true, 0.01); } set { }  }
    public int yAxisGainValue { get { return _packet.GetBitOffsetLength<int>(2, 58, 16, true, 0.01); } set { }  }
    public int zAxisGainValue { get { return _packet.GetBitOffsetLength<int>(2, 74, 16, true, 0.01); } set { }  }
    public int xAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(2, 90, 16, true, 0.01); } set { }  }
    public int yAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(2, 106, 16, true, 0.01); } set { }  }
    public int zAxisLinearOffset { get { return _packet.GetBitOffsetLength<int>(2, 122, 16, true, 0.01); } set { }  }
    public int xAxisAngularOffset { get { return _packet.GetBitOffsetLength<int>(2, 138, 16, true, 0.1); } set { }  }
    public int pitchAndRollDamping { get { return _packet.GetBitOffsetLength<int>(2, 154, 16, true, 0.05); } set { }  }
    public int compassRateGyroDamping { get { return _packet.GetBitOffsetLength<int>(2, 170, 16, true, 0.05); } set { }  }
}
 /// Airmar: Simulate Mode
 /// Fast
 /// 126720
 public class airmarSimulateMode : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarSimulateMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string simulateMode { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
}
 /// Airmar: Calibrate Depth
 /// Fast
 /// 126720
 public class airmarCalibrateDepth : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarCalibrateDepth(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int speedOfSoundMode { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.1); } set { }  }
}
 /// Airmar: Calibrate Speed
 /// Fast
 /// 126720
 public class airmarCalibrateSpeed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarCalibrateSpeed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int numberOfPairsOfDataPoints { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int inputFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.1); } set { }  }
    public int outputSpeed { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.01); } set { }  }
}
 /// Airmar: Calibrate Temperature
 /// Fast
 /// 126720
 public class airmarCalibrateTemperature : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarCalibrateTemperature(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string temperatureInstance { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public int temperatureOffset { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.001); } set { }  }
}
 /// Airmar: Speed Filter
 /// Fast
 /// 126720
 public class airmarSpeedFilter : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarSpeedFilter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string filterType { get { return _packet.GetBitOffsetLength<string>(0, 24, 4, false, 0); } set { }  }
    public int sampleInterval { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
    public int filterDuration { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.01); } set { }  }
}
 /// Airmar: Temperature Filter
 /// Fast
 /// 126720
 public class airmarTemperatureFilter : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarTemperatureFilter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string filterType { get { return _packet.GetBitOffsetLength<string>(0, 24, 4, false, 0); } set { }  }
    public int sampleInterval { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
    public int filterDuration { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.01); } set { }  }
}
 /// Airmar: NMEA 2000 options
 /// Fast
 /// 126720
 public class airmarNmea2000Options : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarNmea2000Options(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string transmissionInterval { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
}
 /// Airmar: Addressable Multi-Frame
 /// Fast
 /// 126720
 public class airmarAddressableMultiFrame : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarAddressableMultiFrame(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
}
 /// Maretron: Slave Response
 /// Fast
 /// 126720
 public class maretronSlaveResponse : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronSlaveResponse(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int softwareCode { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0); } set { }  }
    public int command { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Manufacturer Proprietary fast-packet addressed
 /// Fast
 /// 126720
 public class manufacturerProprietaryFastPacketAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public manufacturerProprietaryFastPacketAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 1768, false, 0); } set { }  }
}
 /// Unknown fast-packet non-addressed
 /// Fast
 /// 126976
 public class unknownFastPacketNonAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public unknownFastPacketNonAddressed(byte[] packet) { _packet = packet; }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 2040, false, 0); } set { }  }
}
 /// Alert
 /// Fast
 /// 126983
 public class alert : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alert(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int>(0, 40, 64, false, 0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int>(0, 112, 8, false, 0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int>(0, 120, 8, false, 0); } set { }  }
    public string temporarySilenceStatus { get { return _packet.GetBitOffsetLength<string>(0, 128, 1, false, 0); } set { }  }
    public string acknowledgeStatus { get { return _packet.GetBitOffsetLength<string>(1, 129, 1, false, 0); } set { }  }
    public string escalationStatus { get { return _packet.GetBitOffsetLength<string>(2, 130, 1, false, 0); } set { }  }
    public string temporarySilenceSupport { get { return _packet.GetBitOffsetLength<string>(3, 131, 1, false, 0); } set { }  }
    public string acknowledgeSupport { get { return _packet.GetBitOffsetLength<string>(4, 132, 1, false, 0); } set { }  }
    public string escalationSupport { get { return _packet.GetBitOffsetLength<string>(5, 133, 1, false, 0); } set { }  }
    public byte[] nmeaReserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 134, 2, false, 0); } set { }  }
    public int acknowledgeSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int>(0, 136, 64, false, 0); } set { }  }
    public string triggerCondition { get { return _packet.GetBitOffsetLength<string>(0, 200, 4, false, 0); } set { }  }
    public string thresholdStatus { get { return _packet.GetBitOffsetLength<string>(4, 204, 4, false, 0); } set { }  }
    public int alertPriority { get { return _packet.GetBitOffsetLength<int>(0, 208, 8, false, 0); } set { }  }
    public string alertState { get { return _packet.GetBitOffsetLength<string>(0, 216, 8, false, 0); } set { }  }
}
 /// Alert Response
 /// Fast
 /// 126984
 public class alertResponse : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alertResponse(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int>(0, 40, 64, false, 0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int>(0, 112, 8, false, 0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int>(0, 120, 8, false, 0); } set { }  }
    public int acknowledgeSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int>(0, 128, 64, false, 0); } set { }  }
    public string responseCommand { get { return _packet.GetBitOffsetLength<string>(0, 192, 2, false, 0); } set { }  }
    public byte[] nmeaReserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 194, 6, false, 0); } set { }  }
}
 /// Alert Text
 /// Fast
 /// 126985
 public class alertText : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alertText(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int>(0, 40, 64, false, 0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int>(0, 112, 8, false, 0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int>(0, 120, 8, false, 0); } set { }  }
    public string languageId { get { return _packet.GetBitOffsetLength<string>(0, 128, 8, false, 0); } set { }  }
    public string alertTextDescription { get { return _packet.GetBitOffsetLength<string>(0, 136, 128, false, 0); } set { }  }
    public string alertLocationTextDescription { get { return _packet.GetBitOffsetLength<string>(0, 0, 128, false, 0); } set { }  }
}
 /// Alert Configuration
 /// Fast
 /// 126986
 public class alertConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alertConfiguration(byte[] packet) { _packet = packet; }
}
 /// Alert Threshold
 /// Fast
 /// 126987
 public class alertThreshold : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alertThreshold(byte[] packet) { _packet = packet; }
}
 /// Alert Value
 /// Fast
 /// 126988
 public class alertValue : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public alertValue(byte[] packet) { _packet = packet; }
}
 /// System Time
 /// Single
 /// 126992
 public class systemTime : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public systemTime(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 12, 4, false, 0); } set { }  }
    public DateTime date { get { return _packet.GetBitOffsetLength<DateTime>(0, 16, 16, false, 1); } set { }  }
    public DateTime time { get { return _packet.GetBitOffsetLength<DateTime>(0, 32, 32, false, 0.0001); } set { }  }
}
 /// Heartbeat
 /// Single
 /// 126993
 public class heartbeat : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public heartbeat(byte[] packet) { _packet = packet; }
    public int dataTransmitOffset { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0.001); } set { }  }
    public int sequenceCounter { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string controller1State { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string controller2State { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public string equipmentStatus { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 30, 34, false, 0); } set { }  }
}
 /// Product Information
 /// Fast
 /// 126996
 public class productInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public productInformation(byte[] packet) { _packet = packet; }
    public int nmea2000Version { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string modelId { get { return _packet.GetBitOffsetLength<string>(0, 32, 256, false, 0); } set { }  }
    public string softwareVersionCode { get { return _packet.GetBitOffsetLength<string>(0, 288, 256, false, 0); } set { }  }
    public string modelVersion { get { return _packet.GetBitOffsetLength<string>(0, 544, 256, false, 0); } set { }  }
    public string modelSerialCode { get { return _packet.GetBitOffsetLength<string>(0, 800, 256, false, 0); } set { }  }
    public int certificationLevel { get { return _packet.GetBitOffsetLength<int>(0, 1056, 8, false, 0); } set { }  }
    public int loadEquivalency { get { return _packet.GetBitOffsetLength<int>(0, 1064, 8, false, 0); } set { }  }
}
 /// Configuration Information
 /// Fast
 /// 126998
 public class configurationInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public configurationInformation(byte[] packet) { _packet = packet; }
    public string installationDescription1 { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
    public string installationDescription2 { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
    public string manufacturerInformation { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
}
 /// Man Overboard Notification
 /// Fast
 /// 127233
 public class manOverboardNotification : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public manOverboardNotification(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int mobEmitterId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public string manOverboardStatus { get { return _packet.GetBitOffsetLength<string>(0, 40, 3, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(3, 43, 5, false, 0); } set { }  }
    public DateTime activationTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 48, 32, false, 0.0001); } set { }  }
    public string positionSource { get { return _packet.GetBitOffsetLength<string>(0, 80, 3, false, 0); } set { }  }
    public DateTime positionDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 88, 16, false, 1); } set { }  }
    public DateTime positionTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 104, 32, false, 0.0001); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 136, 32, true, 1E-07); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 168, 32, true, 1E-07); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string>(0, 200, 2, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 208, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 224, 16, false, 0.01); } set { }  }
    public int mmsiOfVesselOfOrigin { get { return _packet.GetBitOffsetLength<int>(0, 240, 32, false, 1); } set { }  }
    public string mobEmitterBatteryStatus { get { return _packet.GetBitOffsetLength<string>(0, 272, 3, false, 0); } set { }  }
}
 /// Heading/Track control
 /// Fast
 /// 127237
 public class headingTrackControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public headingTrackControl(byte[] packet) { _packet = packet; }
    public string rudderLimitExceeded { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public string offHeadingLimitExceeded { get { return _packet.GetBitOffsetLength<string>(2, 2, 2, false, 0); } set { }  }
    public string offTrackLimitExceeded { get { return _packet.GetBitOffsetLength<string>(4, 4, 2, false, 0); } set { }  }
    public string @override { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public string steeringMode { get { return _packet.GetBitOffsetLength<string>(0, 8, 3, false, 0); } set { }  }
    public string turnMode { get { return _packet.GetBitOffsetLength<string>(3, 11, 3, false, 0); } set { }  }
    public string headingReference { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 5, false, 0); } set { }  }
    public string commandedRudderDirection { get { return _packet.GetBitOffsetLength<string>(5, 21, 3, false, 0); } set { }  }
    public int commandedRudderAngle { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.0001); } set { }  }
    public int headingToSteerCourse { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.0001); } set { }  }
    public int track { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0.0001); } set { }  }
    public int rudderLimit { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.0001); } set { }  }
    public int offHeadingLimit { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 0.0001); } set { }  }
    public int radiusOfTurnOrder { get { return _packet.GetBitOffsetLength<int>(0, 104, 16, true, 0.0001); } set { }  }
    public int rateOfTurnOrder { get { return _packet.GetBitOffsetLength<int>(0, 120, 16, true, 3.125E-05); } set { }  }
    public int offTrackLimit { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, true, 0); } set { }  }
    public int vesselHeading { get { return _packet.GetBitOffsetLength<int>(0, 152, 16, false, 0.0001); } set { }  }
}
 /// Rudder
 /// Single
 /// 127245
 public class rudder : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public rudder(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string directionOrder { get { return _packet.GetBitOffsetLength<string>(0, 8, 3, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(3, 11, 5, false, 0); } set { }  }
    public int angleOrder { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, true, 0.0001); } set { }  }
    public int position { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.0001); } set { }  }
}
 /// Vessel Heading
 /// Single
 /// 127250
 public class vesselHeading : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public vesselHeading(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int heading { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.0001); } set { }  }
    public int deviation { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.0001); } set { }  }
    public int variation { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.0001); } set { }  }
    public string reference { get { return _packet.GetBitOffsetLength<string>(0, 56, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 58, 6, false, 0); } set { }  }
}
 /// Rate of Turn
 /// Single
 /// 127251
 public class rateOfTurn : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public rateOfTurn(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int rate { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, true, 3.125E-08); } set { }  }
}
 /// Heave
 /// Single
 /// 127252
 public class heave : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public heave(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int heave_ { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, true, 0.01); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(0, 24, 40, false, 0); } set { }  }
}
 /// Attitude
 /// Single
 /// 127257
 public class attitude : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public attitude(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int yaw { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, true, 0.0001); } set { }  }
    public int pitch { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.0001); } set { }  }
    public int roll { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.0001); } set { }  }
}
 /// Magnetic Variation
 /// Single
 /// 127258
 public class magneticVariation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public magneticVariation(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 12, 4, false, 0); } set { }  }
    public DateTime ageOfService { get { return _packet.GetBitOffsetLength<DateTime>(0, 16, 16, false, 1); } set { }  }
    public int variation { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.0001); } set { }  }
}
 /// Engine Parameters, Rapid Update
 /// Single
 /// 127488
 public class engineParametersRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public engineParametersRapidUpdate(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int speed { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.25); } set { }  }
    public double boostPressure { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0); } set { }  }
    public int tiltTrim { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, true, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 48, 16, false, 0); } set { }  }
}
 /// Engine Parameters, Dynamic
 /// Fast
 /// 127489
 public class engineParametersDynamic : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public engineParametersDynamic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public double oilPressure { get { return _packet.GetBitOffsetLength<double>(0, 8, 16, false, 0); } set { }  }
    public double oilTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.1); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.01); } set { }  }
    public int alternatorPotential { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, true, 0.01); } set { }  }
    public int fuelRate { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, true, 0.1); } set { }  }
    public int totalEngineHours { get { return _packet.GetBitOffsetLength<int>(0, 88, 32, false, 0); } set { }  }
    public double coolantPressure { get { return _packet.GetBitOffsetLength<double>(0, 120, 16, false, 0); } set { }  }
    public int fuelPressure { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 152, 8, false, 0); } set { }  }
    public long discreteStatus1 { get { return _packet.GetBitOffsetLength<long>(0, 160, 16, false, 0); } set { }  }
    public long discreteStatus2 { get { return _packet.GetBitOffsetLength<long>(0, 176, 16, false, 0); } set { }  }
    public int percentEngineLoad { get { return _packet.GetBitOffsetLength<int>(0, 192, 8, true, 1); } set { }  }
    public int percentEngineTorque { get { return _packet.GetBitOffsetLength<int>(0, 200, 8, true, 1); } set { }  }
}
 /// Transmission Parameters, Dynamic
 /// Single
 /// 127493
 public class transmissionParametersDynamic : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public transmissionParametersDynamic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string transmissionGear { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 10, 6, false, 0); } set { }  }
    public double oilPressure { get { return _packet.GetBitOffsetLength<double>(0, 16, 16, false, 0); } set { }  }
    public double oilTemperature { get { return _packet.GetBitOffsetLength<double>(0, 32, 16, false, 0.1); } set { }  }
    public int discreteStatus1 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
}
 /// Trip Parameters, Vessel
 /// Fast
 /// 127496
 public class tripParametersVessel : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public tripParametersVessel(byte[] packet) { _packet = packet; }
    public int timeToEmpty { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 0.001); } set { }  }
    public int distanceToEmpty { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0.01); } set { }  }
    public int estimatedFuelRemaining { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
    public int tripRunTime { get { return _packet.GetBitOffsetLength<int>(0, 80, 32, false, 0.001); } set { }  }
}
 /// Trip Parameters, Engine
 /// Fast
 /// 127497
 public class tripParametersEngine : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public tripParametersEngine(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int tripFuelUsed { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int fuelRateAverage { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.1); } set { }  }
    public int fuelRateEconomy { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.1); } set { }  }
    public int instantaneousFuelEconomy { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, true, 0.1); } set { }  }
}
 /// Engine Parameters, Static
 /// Fast
 /// 127498
 public class engineParametersStatic : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public engineParametersStatic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int ratedEngineSpeed { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.25); } set { }  }
    public string vin { get { return _packet.GetBitOffsetLength<string>(0, 24, 136, false, 0); } set { }  }
    public string softwareId { get { return _packet.GetBitOffsetLength<string>(0, 160, 256, false, 0); } set { }  }
}
 /// Load Controller Connection State/Control
 /// Fast
 /// 127500
 public class loadControllerConnectionStateControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public loadControllerConnectionStateControl(byte[] packet) { _packet = packet; }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int connectionId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int state { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int operationalStatusControl { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 1); } set { }  }
    public int pwmDutyCycle { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int timeon { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 1); } set { }  }
    public int timeoff { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 1); } set { }  }
}
 /// Binary Switch Bank Status
 /// Single
 /// 127501
 public class binarySwitchBankStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public binarySwitchBankStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string indicator1 { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public string indicator2 { get { return _packet.GetBitOffsetLength<string>(2, 10, 2, false, 0); } set { }  }
    public string indicator3 { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public string indicator4 { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public string indicator5 { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string indicator6 { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public string indicator7 { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public string indicator8 { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public string indicator9 { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string indicator10 { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public string indicator11 { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public string indicator12 { get { return _packet.GetBitOffsetLength<string>(6, 30, 2, false, 0); } set { }  }
    public string indicator13 { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
    public string indicator14 { get { return _packet.GetBitOffsetLength<string>(2, 34, 2, false, 0); } set { }  }
    public string indicator15 { get { return _packet.GetBitOffsetLength<string>(4, 36, 2, false, 0); } set { }  }
    public string indicator16 { get { return _packet.GetBitOffsetLength<string>(6, 38, 2, false, 0); } set { }  }
    public string indicator17 { get { return _packet.GetBitOffsetLength<string>(0, 40, 2, false, 0); } set { }  }
    public string indicator18 { get { return _packet.GetBitOffsetLength<string>(2, 42, 2, false, 0); } set { }  }
    public string indicator19 { get { return _packet.GetBitOffsetLength<string>(4, 44, 2, false, 0); } set { }  }
    public string indicator20 { get { return _packet.GetBitOffsetLength<string>(6, 46, 2, false, 0); } set { }  }
    public string indicator21 { get { return _packet.GetBitOffsetLength<string>(0, 48, 2, false, 0); } set { }  }
    public string indicator22 { get { return _packet.GetBitOffsetLength<string>(2, 50, 2, false, 0); } set { }  }
    public string indicator23 { get { return _packet.GetBitOffsetLength<string>(4, 52, 2, false, 0); } set { }  }
    public string indicator24 { get { return _packet.GetBitOffsetLength<string>(6, 54, 2, false, 0); } set { }  }
    public string indicator25 { get { return _packet.GetBitOffsetLength<string>(0, 56, 2, false, 0); } set { }  }
    public string indicator26 { get { return _packet.GetBitOffsetLength<string>(2, 58, 2, false, 0); } set { }  }
    public string indicator27 { get { return _packet.GetBitOffsetLength<string>(4, 60, 2, false, 0); } set { }  }
    public string indicator28 { get { return _packet.GetBitOffsetLength<string>(6, 62, 2, false, 0); } set { }  }
}
 /// Switch Bank Control
 /// Single
 /// 127502
 public class switchBankControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public switchBankControl(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string switch1 { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public string switch2 { get { return _packet.GetBitOffsetLength<string>(2, 10, 2, false, 0); } set { }  }
    public string switch3 { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public string switch4 { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public string switch5 { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string switch6 { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public string switch7 { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public string switch8 { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public string switch9 { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string switch10 { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public string switch11 { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public string switch12 { get { return _packet.GetBitOffsetLength<string>(6, 30, 2, false, 0); } set { }  }
    public string switch13 { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
    public string switch14 { get { return _packet.GetBitOffsetLength<string>(2, 34, 2, false, 0); } set { }  }
    public string switch15 { get { return _packet.GetBitOffsetLength<string>(4, 36, 2, false, 0); } set { }  }
    public string switch16 { get { return _packet.GetBitOffsetLength<string>(6, 38, 2, false, 0); } set { }  }
    public string switch17 { get { return _packet.GetBitOffsetLength<string>(0, 40, 2, false, 0); } set { }  }
    public string switch18 { get { return _packet.GetBitOffsetLength<string>(2, 42, 2, false, 0); } set { }  }
    public string switch19 { get { return _packet.GetBitOffsetLength<string>(4, 44, 2, false, 0); } set { }  }
    public string switch20 { get { return _packet.GetBitOffsetLength<string>(6, 46, 2, false, 0); } set { }  }
    public string switch21 { get { return _packet.GetBitOffsetLength<string>(0, 48, 2, false, 0); } set { }  }
    public string switch22 { get { return _packet.GetBitOffsetLength<string>(2, 50, 2, false, 0); } set { }  }
    public string switch23 { get { return _packet.GetBitOffsetLength<string>(4, 52, 2, false, 0); } set { }  }
    public string switch24 { get { return _packet.GetBitOffsetLength<string>(6, 54, 2, false, 0); } set { }  }
    public string switch25 { get { return _packet.GetBitOffsetLength<string>(0, 56, 2, false, 0); } set { }  }
    public string switch26 { get { return _packet.GetBitOffsetLength<string>(2, 58, 2, false, 0); } set { }  }
    public string switch27 { get { return _packet.GetBitOffsetLength<string>(4, 60, 2, false, 0); } set { }  }
    public string switch28 { get { return _packet.GetBitOffsetLength<string>(6, 62, 2, false, 0); } set { }  }
}
 /// AC Input Status
 /// Fast
 /// 127503
 public class acInputStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public acInputStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int numberOfLines { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string line { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string acceptability { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 20, 4, false, 0); } set { }  }
    public int voltage { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.01); } set { }  }
    public int current { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.1); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0.01); } set { }  }
    public int breakerSize { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.1); } set { }  }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 88, 32, false, 1); } set { }  }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 120, 32, false, 1); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 152, 8, false, 0.01); } set { }  }
}
 /// AC Output Status
 /// Single
 /// 127504
 public class acOutputStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public acOutputStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int numberOfLines { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string line { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string waveform { get { return _packet.GetBitOffsetLength<string>(2, 18, 3, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(5, 21, 3, false, 0); } set { }  }
    public int voltage { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.01); } set { }  }
    public int current { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.1); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0.01); } set { }  }
    public int breakerSize { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.1); } set { }  }
    public int realPower { get { return _packet.GetBitOffsetLength<int>(0, 88, 32, false, 1); } set { }  }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int>(0, 120, 32, false, 1); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int>(0, 152, 8, false, 0.01); } set { }  }
}
 /// Fluid Level
 /// Single
 /// 127505
 public class fluidLevel : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fluidLevel(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 4, false, 0); } set { }  }
    public string type { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int level { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.004); } set { }  }
    public int capacity { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 0.1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 56, 8, false, 0); } set { }  }
}
 /// DC Detailed Status
 /// Fast
 /// 127506
 public class dcDetailedStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public dcDetailedStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string dcType { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public int stateOfCharge { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int stateOfHealth { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int timeRemaining { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0); } set { }  }
    public int rippleVoltage { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0.01); } set { }  }
    public int ampHours { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 3600); } set { }  }
}
 /// Charger Status
 /// Fast
 /// 127507
 public class chargerStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public chargerStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int batteryInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string>(0, 16, 4, false, 0); } set { }  }
    public string chargeMode { get { return _packet.GetBitOffsetLength<string>(4, 20, 4, false, 0); } set { }  }
    public string enabled { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string equalizationPending { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 28, 4, false, 0); } set { }  }
    public int equalizationTimeRemaining { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0); } set { }  }
}
 /// Battery Status
 /// Single
 /// 127508
 public class batteryStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public batteryStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int voltage { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, true, 0.01); } set { }  }
    public int current { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.1); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.01); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Inverter Status
 /// Single
 /// 127509
 public class inverterStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public inverterStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int acInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int dcInstance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string>(0, 24, 4, false, 0); } set { }  }
    public string inverterEnableDisable { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 30, 2, false, 0); } set { }  }
}
 /// Charger Configuration Status
 /// Fast
 /// 127510
 public class chargerConfigurationStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public chargerConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int batteryInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int chargerEnableDisable { get { return _packet.GetBitOffsetLength<int>(0, 16, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 18, 6, false, 0); } set { }  }
    public int chargeCurrentLimit { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.1); } set { }  }
    public int chargingAlgorithm { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int chargerMode { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public double estimatedTemperature { get { return _packet.GetBitOffsetLength<double>(0, 56, 16, false, 0.01); } set { }  }
    public int equalizeOneTimeEnableDisable { get { return _packet.GetBitOffsetLength<int>(0, 72, 4, false, 0); } set { }  }
    public int overChargeEnableDisable { get { return _packet.GetBitOffsetLength<int>(4, 76, 4, false, 0); } set { }  }
    public int equalizeTime { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 0); } set { }  }
}
 /// Inverter Configuration Status
 /// Single
 /// 127511
 public class inverterConfigurationStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public inverterConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int acInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int dcInstance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int inverterEnableDisable { get { return _packet.GetBitOffsetLength<int>(0, 24, 2, false, 0); } set { }  }
    public int inverterMode { get { return _packet.GetBitOffsetLength<int>(2, 26, 8, false, 0); } set { }  }
    public int loadSenseEnableDisable { get { return _packet.GetBitOffsetLength<int>(2, 34, 8, false, 0); } set { }  }
    public int loadSensePowerThreshold { get { return _packet.GetBitOffsetLength<int>(2, 42, 8, false, 0); } set { }  }
    public int loadSenseInterval { get { return _packet.GetBitOffsetLength<int>(2, 50, 8, false, 0); } set { }  }
}
 /// AGS Configuration Status
 /// Single
 /// 127512
 public class agsConfigurationStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public agsConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int generatorInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int agsMode { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
}
 /// Battery Configuration Status
 /// Fast
 /// 127513
 public class batteryConfigurationStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public batteryConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string batteryType { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public string supportsEqualization { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 14, 2, false, 0); } set { }  }
    public string nominalVoltage { get { return _packet.GetBitOffsetLength<string>(0, 16, 4, false, 0); } set { }  }
    public string chemistry { get { return _packet.GetBitOffsetLength<string>(4, 20, 4, false, 0); } set { }  }
    public int capacity { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int temperatureCoefficient { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int peukertExponent { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0.002); } set { }  }
    public int chargeEfficiencyFactor { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// AGS Status
 /// Single
 /// 127514
 public class agsStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public agsStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int generatorInstance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int agsOperatingState { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int generatorState { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int generatorOnReason { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int generatorOffReason { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
}
 /// AC Power / Current - Phase A
 /// Single
 /// 127744
 public class acPowerCurrentPhaseA : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public acPowerCurrentPhaseA(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// AC Power / Current - Phase B
 /// Single
 /// 127745
 public class acPowerCurrentPhaseB : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public acPowerCurrentPhaseB(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// AC Power / Current - Phase C
 /// Single
 /// 127746
 public class acPowerCurrentPhaseC : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public acPowerCurrentPhaseC(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0); } set { }  }
}
 /// Converter Status
 /// Single
 /// 127750
 public class converterStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public converterStatus(byte[] packet) { _packet = packet; }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 8, false, 0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public string temperatureState { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string overloadState { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public string lowDcVoltageState { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public string rippleState { get { return _packet.GetBitOffsetLength<string>(6, 30, 2, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// DC Voltage/Current
 /// Single
 /// 127751
 public class dcVoltageCurrent : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public dcVoltageCurrent(byte[] packet) { _packet = packet; }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[]>(0, 0, 8, false, 0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int dcVoltage { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.1); } set { }  }
    public int dcCurrent { get { return _packet.GetBitOffsetLength<int>(0, 32, 24, true, 0.01); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Leeway Angle
 /// Single
 /// 128000
 public class leewayAngle : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public leewayAngle(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int leewayAngle_ { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, true, 0.0001); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 24, 40, false, 0); } set { }  }
}
 /// Thruster Control Status
 /// Single
 /// 128006
 public class thrusterControlStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public thrusterControlStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int identifier { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string directionControl { get { return _packet.GetBitOffsetLength<string>(0, 16, 4, false, 0); } set { }  }
    public string powerEnabled { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public string retractControl { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int speedControl { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0.004); } set { }  }
    public long controlEvents { get { return _packet.GetBitOffsetLength<long>(0, 32, 8, false, 0); } set { }  }
    public int commandTimeout { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0.001); } set { }  }
    public int azimuthControl { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.0001); } set { }  }
}
 /// Thruster Information
 /// Single
 /// 128007
 public class thrusterInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public thrusterInformation(byte[] packet) { _packet = packet; }
    public int identifier { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string motorType { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 12, 4, false, 0); } set { }  }
    public int powerRating { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public double maximumTemperatureRating { get { return _packet.GetBitOffsetLength<double>(0, 32, 16, false, 0.01); } set { }  }
    public int maximumRotationalSpeed { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.25); } set { }  }
}
 /// Thruster Motor Status
 /// Single
 /// 128008
 public class thrusterMotorStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public thrusterMotorStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int identifier { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public long motorEvents { get { return _packet.GetBitOffsetLength<long>(0, 16, 8, false, 0); } set { }  }
    public int current { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<double>(0, 32, 16, false, 0.01); } set { }  }
    public int operatingTime { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
}
 /// Speed
 /// Single
 /// 128259
 public class speed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public speed(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int speedWaterReferenced { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.01); } set { }  }
    public int speedGroundReferenced { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.01); } set { }  }
    public string speedWaterReferencedType { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int speedDirection { get { return _packet.GetBitOffsetLength<int>(0, 48, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 52, 12, false, 0); } set { }  }
}
 /// Water Depth
 /// Single
 /// 128267
 public class waterDepth : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public waterDepth(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int depth { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 0.01); } set { }  }
    public int offset { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.001); } set { }  }
    public int range { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 10); } set { }  }
}
 /// Distance Log
 /// Fast
 /// 128275
 public class distanceLog : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public distanceLog(byte[] packet) { _packet = packet; }
    public DateTime date { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 16, false, 1); } set { }  }
    public DateTime time { get { return _packet.GetBitOffsetLength<DateTime>(0, 16, 32, false, 0.0001); } set { }  }
    public int log { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0); } set { }  }
    public int tripLog { get { return _packet.GetBitOffsetLength<int>(0, 80, 32, false, 0); } set { }  }
}
 /// Tracked Target Data
 /// Fast
 /// 128520
 public class trackedTargetData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public trackedTargetData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int targetId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string trackStatus { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string reportedTarget { get { return _packet.GetBitOffsetLength<string>(2, 18, 1, false, 0); } set { }  }
    public string targetAcquisition { get { return _packet.GetBitOffsetLength<string>(3, 19, 1, false, 0); } set { }  }
    public string bearingReference { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 22, 2, false, 0); } set { }  }
    public int bearing { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.0001); } set { }  }
    public int distance { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 0.001); } set { }  }
    public int course { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.0001); } set { }  }
    public int speed { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 0.01); } set { }  }
    public int cpa { get { return _packet.GetBitOffsetLength<int>(0, 104, 32, false, 0.01); } set { }  }
    public int tcpa { get { return _packet.GetBitOffsetLength<int>(0, 136, 32, false, 0.001); } set { }  }
    public DateTime utcOfFix { get { return _packet.GetBitOffsetLength<DateTime>(0, 168, 32, false, 0.0001); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 200, 2040, false, 0); } set { }  }
}
 /// Windlass Control Status
 /// Single
 /// 128776
 public class windlassControlStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public windlassControlStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string windlassDirectionControl { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string anchorDockingControl { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public string speedControlType { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 22, 2, false, 0); } set { }  }
    public byte[] speedControl { get { return _packet.GetBitOffsetLength<byte[]>(0, 24, 8, false, 0); } set { }  }
    public string powerEnable { get { return _packet.GetBitOffsetLength<string>(0, 32, 2, false, 0); } set { }  }
    public string mechanicalLock { get { return _packet.GetBitOffsetLength<string>(2, 34, 2, false, 0); } set { }  }
    public string deckAndAnchorWash { get { return _packet.GetBitOffsetLength<string>(4, 36, 2, false, 0); } set { }  }
    public string anchorLight { get { return _packet.GetBitOffsetLength<string>(6, 38, 2, false, 0); } set { }  }
    public int commandTimeout { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0.005); } set { }  }
    public long windlassControlEvents { get { return _packet.GetBitOffsetLength<long>(0, 48, 4, false, 0); } set { }  }
}
 /// Anchor Windlass Operating Status
 /// Single
 /// 128777
 public class anchorWindlassOperatingStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public anchorWindlassOperatingStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string windlassDirectionControl { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string windlassMotionStatus { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public string rodeTypeStatus { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 22, 2, false, 0); } set { }  }
    public int rodeCounterValue { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.1); } set { }  }
    public int windlassLineSpeed { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 0.01); } set { }  }
    public string anchorDockingStatus { get { return _packet.GetBitOffsetLength<string>(0, 56, 2, false, 0); } set { }  }
    public long windlassOperatingEvents { get { return _packet.GetBitOffsetLength<long>(2, 58, 6, false, 0); } set { }  }
}
 /// Anchor Windlass Monitoring Status
 /// Single
 /// 128778
 public class anchorWindlassMonitoringStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public anchorWindlassMonitoringStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public long windlassMonitoringEvents { get { return _packet.GetBitOffsetLength<long>(0, 16, 8, false, 0); } set { }  }
    public int controllerVoltage { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0.2); } set { }  }
    public int motorCurrent { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int totalMotorTime { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 60); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 56, 8, false, 0); } set { }  }
}
 /// Position, Rapid Update
 /// Single
 /// 129025
 public class positionRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public positionRapidUpdate(byte[] packet) { _packet = packet; }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 0, 32, true, 1E-07); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 32, 32, true, 1E-07); } set { }  }
}
 /// COG & SOG, Rapid Update
 /// Single
 /// 129026
 public class cogSogRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public cogSogRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 10, 6, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
}
 /// Position Delta, Rapid Update
 /// Single
 /// 129027
 public class positionDeltaRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public positionDeltaRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int timeDelta { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int latitudeDelta { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0); } set { }  }
    public int longitudeDelta { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0); } set { }  }
}
 /// Altitude Delta, Rapid Update
 /// Single
 /// 129028
 public class altitudeDeltaRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public altitudeDeltaRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int timeDelta { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, true, 0); } set { }  }
    public int gnssQuality { get { return _packet.GetBitOffsetLength<int>(0, 24, 2, false, 0); } set { }  }
    public int direction { get { return _packet.GetBitOffsetLength<int>(2, 26, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 28, 4, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0001); } set { }  }
    public int altitudeDelta { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 0); } set { }  }
}
 /// GNSS Position Data
 /// Fast
 /// 129029
 public class gnssPositionData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssPositionData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public DateTime date { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime time { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 64, true, 1E-16); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 120, 64, true, 1E-16); } set { }  }
    public int altitude { get { return _packet.GetBitOffsetLength<int>(0, 184, 64, true, 1E-06); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string>(0, 248, 4, false, 0); } set { }  }
    public string method { get { return _packet.GetBitOffsetLength<string>(4, 252, 4, false, 0); } set { }  }
    public string integrity { get { return _packet.GetBitOffsetLength<string>(0, 256, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 258, 6, false, 0); } set { }  }
    public int numberOfSvs { get { return _packet.GetBitOffsetLength<int>(0, 264, 8, false, 0); } set { }  }
    public int hdop { get { return _packet.GetBitOffsetLength<int>(0, 272, 16, true, 0.01); } set { }  }
    public int pdop { get { return _packet.GetBitOffsetLength<int>(0, 288, 16, true, 0.01); } set { }  }
    public int geoidalSeparation { get { return _packet.GetBitOffsetLength<int>(0, 304, 32, true, 0.01); } set { }  }
    public int referenceStations { get { return _packet.GetBitOffsetLength<int>(0, 336, 8, false, 0); } set { }  }
    public string referenceStationType { get { return _packet.GetBitOffsetLength<string>(0, 344, 4, false, 0); } set { }  }
    public int referenceStationId { get { return _packet.GetBitOffsetLength<int>(4, 348, 12, false, 0); } set { }  }
    public int ageOfDgnssCorrections { get { return _packet.GetBitOffsetLength<int>(0, 360, 16, false, 0.01); } set { }  }
}
 /// Time & Date
 /// Single
 /// 129033
 public class timeDate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public timeDate(byte[] packet) { _packet = packet; }
    public DateTime date { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 16, false, 1); } set { }  }
    public DateTime time { get { return _packet.GetBitOffsetLength<DateTime>(0, 16, 32, false, 0.0001); } set { }  }
    public int localOffset { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 1); } set { }  }
}
 /// AIS Class A Position Report
 /// Fast
 /// 129038
 public class aisClassAPositionReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassAPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string>(2, 106, 6, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, false, 0.01); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[]>(0, 144, 19, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(3, 163, 5, false, 0); } set { }  }
    public int heading { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0.0001); } set { }  }
    public int rateOfTurn { get { return _packet.GetBitOffsetLength<int>(0, 184, 16, true, 3.125E-05); } set { }  }
    public string navStatus { get { return _packet.GetBitOffsetLength<string>(0, 200, 4, false, 0); } set { }  }
    public string specialManeuverIndicator { get { return _packet.GetBitOffsetLength<string>(4, 204, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 206, 2, false, 0); } set { }  }
    public byte[] aisSpare { get { return _packet.GetBitOffsetLength<byte[]>(0, 208, 3, false, 0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int>(0, 216, 8, false, 1); } set { }  }
}
 /// AIS Class B Position Report
 /// Fast
 /// 129039
 public class aisClassBPositionReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassBPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string>(2, 106, 6, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, false, 0.01); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[]>(0, 144, 19, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(3, 163, 5, false, 0); } set { }  }
    public int heading { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0.0001); } set { }  }
    public int regionalApplication { get { return _packet.GetBitOffsetLength<int>(0, 184, 8, false, 0); } set { }  }
    public string unitType { get { return _packet.GetBitOffsetLength<string>(2, 194, 1, false, 0); } set { }  }
    public string integratedDisplay { get { return _packet.GetBitOffsetLength<string>(3, 195, 1, false, 0); } set { }  }
    public string dsc { get { return _packet.GetBitOffsetLength<string>(4, 196, 1, false, 0); } set { }  }
    public string band { get { return _packet.GetBitOffsetLength<string>(5, 197, 1, false, 0); } set { }  }
    public string canHandleMsg22 { get { return _packet.GetBitOffsetLength<string>(6, 198, 1, false, 0); } set { }  }
    public string aisMode { get { return _packet.GetBitOffsetLength<string>(7, 199, 1, false, 0); } set { }  }
    public string aisCommunicationState { get { return _packet.GetBitOffsetLength<string>(0, 200, 1, false, 0); } set { }  }
}
 /// AIS Class B Extended Position Report
 /// Fast
 /// 129040
 public class aisClassBExtendedPositionReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassBExtendedPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string aisRaimFlag { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string>(2, 106, 6, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, false, 0.01); } set { }  }
    public int regionalApplication { get { return _packet.GetBitOffsetLength<int>(0, 144, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 156, 4, false, 0); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string>(0, 160, 8, false, 0); } set { }  }
    public int trueHeading { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0.0001); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string>(4, 188, 4, false, 0); } set { }  }
    public int length { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 0.1); } set { }  }
    public int beam { get { return _packet.GetBitOffsetLength<int>(0, 208, 16, false, 0.1); } set { }  }
    public int positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int>(0, 224, 16, false, 0.1); } set { }  }
    public int positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int>(0, 240, 16, false, 0.1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 256, 160, false, 0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string>(0, 416, 1, false, 0); } set { }  }
    public int aisMode { get { return _packet.GetBitOffsetLength<int>(1, 417, 1, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(6, 422, 5, false, 0); } set { }  }
}
 /// AIS Aids to Navigation (AtoN) Report
 /// Fast
 /// 129041
 public class aisAidsToNavigationAtonReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisAidsToNavigationAtonReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string aisRaimFlag { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string>(2, 106, 6, false, 0); } set { }  }
    public int lengthDiameter { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.1); } set { }  }
    public int beamDiameter { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, false, 0.1); } set { }  }
    public int positionReferenceFromStarboardEdge { get { return _packet.GetBitOffsetLength<int>(0, 144, 16, false, 0.1); } set { }  }
    public int positionReferenceFromTrueNorthFacingEdge { get { return _packet.GetBitOffsetLength<int>(0, 160, 16, false, 0.1); } set { }  }
    public string atonType { get { return _packet.GetBitOffsetLength<string>(0, 176, 5, false, 0); } set { }  }
    public string offPositionIndicator { get { return _packet.GetBitOffsetLength<string>(5, 181, 1, false, 0); } set { }  }
    public string virtualAtonFlag { get { return _packet.GetBitOffsetLength<string>(6, 182, 1, false, 0); } set { }  }
    public string assignedModeFlag { get { return _packet.GetBitOffsetLength<string>(7, 183, 1, false, 0); } set { }  }
    public byte[] aisSpare { get { return _packet.GetBitOffsetLength<byte[]>(0, 184, 1, false, 0); } set { }  }
    public string positionFixingDeviceType { get { return _packet.GetBitOffsetLength<string>(1, 185, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(5, 189, 3, false, 0); } set { }  }
    public byte[] atonStatus { get { return _packet.GetBitOffsetLength<byte[]>(0, 192, 8, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 200, 5, false, 0); } set { }  }
    public string atonName { get { return _packet.GetBitOffsetLength<string>(0, 208, 272, false, 0); } set { }  }
}
 /// Datum
 /// Fast
 /// 129044
 public class datum : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public datum(byte[] packet) { _packet = packet; }
    public string localDatum { get { return _packet.GetBitOffsetLength<string>(0, 0, 32, false, 0); } set { }  }
    public double deltaLatitude { get { return _packet.GetBitOffsetLength<double>(0, 32, 32, true, 1E-07); } set { }  }
    public double deltaLongitude { get { return _packet.GetBitOffsetLength<double>(0, 64, 32, true, 1E-07); } set { }  }
    public int deltaAltitude { get { return _packet.GetBitOffsetLength<int>(0, 96, 32, true, 1E-06); } set { }  }
    public string referenceDatum { get { return _packet.GetBitOffsetLength<string>(0, 128, 32, false, 0); } set { }  }
}
 /// User Datum
 /// Fast
 /// 129045
 public class userDatum : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public userDatum(byte[] packet) { _packet = packet; }
    public int deltaX { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, true, 0.01); } set { }  }
    public int deltaY { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, true, 0.01); } set { }  }
    public int deltaZ { get { return _packet.GetBitOffsetLength<int>(0, 64, 32, true, 0.01); } set { }  }
    public float rotationInX { get { return _packet.GetBitOffsetLength<float>(0, 96, 32, true, 0); } set { }  }
    public float rotationInY { get { return _packet.GetBitOffsetLength<float>(0, 128, 32, true, 0); } set { }  }
    public float rotationInZ { get { return _packet.GetBitOffsetLength<float>(0, 160, 32, true, 0); } set { }  }
    public float scale { get { return _packet.GetBitOffsetLength<float>(0, 192, 32, true, 0); } set { }  }
    public int ellipsoidSemiMajorAxis { get { return _packet.GetBitOffsetLength<int>(0, 224, 32, true, 0.01); } set { }  }
    public float ellipsoidFlatteningInverse { get { return _packet.GetBitOffsetLength<float>(0, 256, 32, true, 0); } set { }  }
    public string datumName { get { return _packet.GetBitOffsetLength<string>(0, 288, 32, false, 0); } set { }  }
}
 /// Cross Track Error
 /// Single
 /// 129283
 public class crossTrackError : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public crossTrackError(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string xteMode { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 12, 2, false, 0); } set { }  }
    public string navigationTerminated { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public int xte { get { return _packet.GetBitOffsetLength<int>(0, 16, 32, true, 0.01); } set { }  }
}
 /// Navigation Data
 /// Fast
 /// 129284
 public class navigationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navigationData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int distanceToWaypoint { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 0.01); } set { }  }
    public string courseBearingReference { get { return _packet.GetBitOffsetLength<string>(0, 40, 2, false, 0); } set { }  }
    public string perpendicularCrossed { get { return _packet.GetBitOffsetLength<string>(2, 42, 2, false, 0); } set { }  }
    public string arrivalCircleEntered { get { return _packet.GetBitOffsetLength<string>(4, 44, 2, false, 0); } set { }  }
    public string calculationType { get { return _packet.GetBitOffsetLength<string>(6, 46, 2, false, 0); } set { }  }
    public DateTime etaTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 48, 32, false, 0.0001); } set { }  }
    public DateTime etaDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 80, 16, false, 1); } set { }  }
    public int bearingOriginToDestinationWaypoint { get { return _packet.GetBitOffsetLength<int>(0, 96, 16, false, 0.0001); } set { }  }
    public int bearingPositionToDestinationWaypoint { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.0001); } set { }  }
    public int originWaypointNumber { get { return _packet.GetBitOffsetLength<int>(0, 128, 32, false, 0); } set { }  }
    public int destinationWaypointNumber { get { return _packet.GetBitOffsetLength<int>(0, 160, 32, false, 0); } set { }  }
    public double destinationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 192, 32, true, 1E-07); } set { }  }
    public double destinationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 224, 32, true, 1E-07); } set { }  }
    public int waypointClosingVelocity { get { return _packet.GetBitOffsetLength<int>(0, 256, 16, true, 0.01); } set { }  }
}
 /// Navigation - Route/WP Information
 /// Fast
 /// 129285
 public class navigationRouteWpInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navigationRouteWpInformation(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int navigationDirectionInRoute { get { return _packet.GetBitOffsetLength<int>(0, 64, 2, false, 0); } set { }  }
    public int supplementaryRouteWpDataAvailable { get { return _packet.GetBitOffsetLength<int>(2, 66, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 68, 4, false, 0); } set { }  }
    public string routeName { get { return _packet.GetBitOffsetLength<string>(0, 72, 2040, false, 0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int>(0, 2120, 16, false, 0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string>(0, 2136, 2040, false, 0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<double>(0, 4176, 32, true, 1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<double>(0, 4208, 32, true, 1E-07); } set { }  }
}
 /// Set & Drift, Rapid Update
 /// Single
 /// 129291
 public class setDriftRapidUpdate : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public setDriftRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string setReference { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 10, 6, false, 0); } set { }  }
    public int set { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.0001); } set { }  }
    public int drift { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
}
 /// Navigation - Route / Time to+from Mark
 /// Fast
 /// 129301
 public class navigationRouteTimeToFromMark : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navigationRouteTimeToFromMark(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int timeToMark { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, true, 0.001); } set { }  }
    public string markType { get { return _packet.GetBitOffsetLength<string>(0, 40, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 44, 4, false, 0); } set { }  }
    public int markId { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0); } set { }  }
}
 /// Bearing and Distance between two Marks
 /// Fast
 /// 129302
 public class bearingAndDistanceBetweenTwoMarks : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bearingAndDistanceBetweenTwoMarks(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string bearingReference { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public string calculationType { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 14, 2, false, 0); } set { }  }
    public int bearingOriginToDestination { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.0001); } set { }  }
    public int distance { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0.01); } set { }  }
    public string originMarkType { get { return _packet.GetBitOffsetLength<string>(0, 64, 4, false, 0); } set { }  }
    public string destinationMarkType { get { return _packet.GetBitOffsetLength<string>(4, 68, 4, false, 0); } set { }  }
    public int originMarkId { get { return _packet.GetBitOffsetLength<int>(0, 72, 32, false, 0); } set { }  }
    public int destinationMarkId { get { return _packet.GetBitOffsetLength<int>(0, 104, 32, false, 0); } set { }  }
}
 /// GNSS Control Status
 /// Fast
 /// 129538
 public class gnssControlStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssControlStatus(byte[] packet) { _packet = packet; }
    public int svElevationMask { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public int pdopMask { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.01); } set { }  }
    public int pdopSwitch { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
    public int snrMask { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.01); } set { }  }
    public string gnssModeDesired { get { return _packet.GetBitOffsetLength<string>(0, 64, 3, false, 0); } set { }  }
    public string dgnssModeDesired { get { return _packet.GetBitOffsetLength<string>(3, 67, 3, false, 0); } set { }  }
    public int positionVelocityFilter { get { return _packet.GetBitOffsetLength<int>(6, 70, 2, false, 0); } set { }  }
    public int maxCorrectionAge { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0); } set { }  }
    public int antennaAltitudeFor2dMode { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 0.01); } set { }  }
    public string useAntennaAltitudeFor2dMode { get { return _packet.GetBitOffsetLength<string>(0, 104, 2, false, 0); } set { }  }
}
 /// GNSS DOPs
 /// Single
 /// 129539
 public class gnssDops : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssDops(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string desiredMode { get { return _packet.GetBitOffsetLength<string>(0, 8, 3, false, 0); } set { }  }
    public string actualMode { get { return _packet.GetBitOffsetLength<string>(3, 11, 3, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 14, 2, false, 0); } set { }  }
    public int hdop { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, true, 0.01); } set { }  }
    public int vdop { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.01); } set { }  }
    public int tdop { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 0.01); } set { }  }
}
 /// GNSS Sats in View
 /// Fast
 /// 129540
 public class gnssSatsInView : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssSatsInView(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string mode { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 10, 6, false, 0); } set { }  }
    public int satsInView { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int prn { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int elevation { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.0001); } set { }  }
    public int azimuth { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.0001); } set { }  }
    public int snr { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0.01); } set { }  }
    public int rangeResiduals { get { return _packet.GetBitOffsetLength<int>(0, 80, 32, true, 0); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string>(0, 112, 4, false, 0); } set { }  }
}
 /// GPS Almanac Data
 /// Fast
 /// 129541
 public class gpsAlmanacData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gpsAlmanacData(byte[] packet) { _packet = packet; }
    public int prn { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int gpsWeekNumber { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 1); } set { }  }
    public byte[] svHealthBits { get { return _packet.GetBitOffsetLength<byte[]>(0, 24, 8, false, 0); } set { }  }
    public int eccentricity { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 1E-21); } set { }  }
    public int almanacReferenceTime { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1000000000000); } set { }  }
    public int inclinationAngle { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, true, 1E-19); } set { }  }
    public int rateOfRightAscension { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, true, 1E-38); } set { }  }
    public int rootOfSemiMajorAxis { get { return _packet.GetBitOffsetLength<int>(0, 88, 24, false, 1E-11); } set { }  }
    public int argumentOfPerigee { get { return _packet.GetBitOffsetLength<int>(0, 112, 24, true, 1E-23); } set { }  }
    public int longitudeOfAscensionNode { get { return _packet.GetBitOffsetLength<int>(0, 136, 24, true, 1E-23); } set { }  }
    public int meanAnomaly { get { return _packet.GetBitOffsetLength<int>(0, 160, 24, true, 1E-23); } set { }  }
    public int clockParameter1 { get { return _packet.GetBitOffsetLength<int>(0, 184, 11, true, 1E-20); } set { }  }
    public int clockParameter2 { get { return _packet.GetBitOffsetLength<int>(3, 195, 11, true, 1E-38); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 206, 2, false, 0); } set { }  }
}
 /// GNSS Pseudorange Noise Statistics
 /// Fast
 /// 129542
 public class gnssPseudorangeNoiseStatistics : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssPseudorangeNoiseStatistics(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int rmsOfPositionUncertainty { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int stdOfMajorAxis { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int stdOfMinorAxis { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int orientationOfMajorAxis { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int stdOfLatError { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int stdOfLonError { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int stdOfAltError { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
}
 /// GNSS RAIM Output
 /// Fast
 /// 129545
 public class gnssRaimOutput : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssRaimOutput(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int integrityFlag { get { return _packet.GetBitOffsetLength<int>(0, 8, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 12, 4, false, 0); } set { }  }
    public int latitudeExpectedError { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int longitudeExpectedError { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int altitudeExpectedError { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int svIdOfMostLikelyFailedSat { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int probabilityOfMissedDetection { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int estimateOfPseudorangeBias { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int stdDeviationOfBias { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
}
 /// GNSS RAIM Settings
 /// Single
 /// 129546
 public class gnssRaimSettings : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssRaimSettings(byte[] packet) { _packet = packet; }
    public int radialPositionErrorMaximumThreshold { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int probabilityOfFalseAlarm { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int probabilityOfMissedDetection { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int pseudorangeResidualFilteringTimeConstant { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
}
 /// GNSS Pseudorange Error Statistics
 /// Fast
 /// 129547
 public class gnssPseudorangeErrorStatistics : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssPseudorangeErrorStatistics(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int rmsStdDevOfRangeInputs { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int stdDevOfMajorErrorEllipse { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int stdDevOfMinorErrorEllipse { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int orientationOfErrorEllipse { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int stdDevLatError { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int stdDevLonError { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int stdDevAltError { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
}
 /// DGNSS Corrections
 /// Fast
 /// 129549
 public class dgnssCorrections : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public dgnssCorrections(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int referenceStationId { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0); } set { }  }
    public int referenceStationType { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int timeOfCorrections { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int stationHealth { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public byte[] @reservedBits { get { return _packet.GetBitOffsetLength<byte[]>(0, 56, 8, false, 0); } set { }  }
    public int satelliteId { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int prc { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public int rrc { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int udre { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
    public int iod { get { return _packet.GetBitOffsetLength<int>(0, 96, 8, false, 0); } set { }  }
}
 /// GNSS Differential Correction Receiver Interface
 /// Fast
 /// 129550
 public class gnssDifferentialCorrectionReceiverInterface : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssDifferentialCorrectionReceiverInterface(byte[] packet) { _packet = packet; }
    public int channel { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int serialInterfaceBitRate { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int serialInterfaceDetectionMode { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int differentialSource { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int differentialOperationMode { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
}
 /// GNSS Differential Correction Receiver Signal
 /// Fast
 /// 129551
 public class gnssDifferentialCorrectionReceiverSignal : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public gnssDifferentialCorrectionReceiverSignal(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int channel { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int signalSnr { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int stationType { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int stationId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int differentialSignalBitRate { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int differentialSignalDetectionMode { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int usedAsCorrectionSource { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 80, 8, false, 0); } set { }  }
    public int differentialSource { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
    public int timeSinceLastSatDifferentialSync { get { return _packet.GetBitOffsetLength<int>(0, 96, 8, false, 0); } set { }  }
    public int satelliteServiceIdNo { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 0); } set { }  }
}
 /// GLONASS Almanac Data
 /// Fast
 /// 129556
 public class glonassAlmanacData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public glonassAlmanacData(byte[] packet) { _packet = packet; }
    public int prn { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int na { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int cna { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int hna { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int EpsilonNa { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int DeltatnaDot { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int OmegaNa { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int DeltaTna { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int tna { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int LambdaNa { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public int DeltaIna { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int tca { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
}
 /// AIS DGNSS Broadcast Binary Message
 /// Fast
 /// 129792
 public class aisDgnssBroadcastBinaryMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisDgnssBroadcastBinaryMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public int repeatIndicator { get { return _packet.GetBitOffsetLength<int>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 0); } set { }  }
    public byte[] nmea2000Reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 8, false, 0); } set { }  }
    public int aisTransceiverInformation { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int longitude { get { return _packet.GetBitOffsetLength<int>(0, 64, 32, false, 0); } set { }  }
    public int latitude { get { return _packet.GetBitOffsetLength<int>(0, 96, 32, false, 0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int>(0, 144, 8, false, 0); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[]>(0, 152, 64, false, 0); } set { }  }
}
 /// AIS UTC and Date Report
 /// Fast
 /// 129793
 public class aisUtcAndDateReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisUtcAndDateReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 106, 6, false, 0); } set { }  }
    public DateTime positionTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 112, 32, false, 0.0001); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[]>(0, 144, 19, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(3, 163, 5, false, 0); } set { }  }
    public DateTime positionDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 168, 16, false, 1); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string>(4, 188, 4, false, 0); } set { }  }
    public byte[] spare { get { return _packet.GetBitOffsetLength<byte[]>(0, 192, 8, false, 0); } set { }  }
}
 /// AIS Class A Static and Voyage Related Data
 /// Fast
 /// 129794
 public class aisClassAStaticAndVoyageRelatedData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassAStaticAndVoyageRelatedData(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public int imoNumber { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string>(0, 72, 56, false, 0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 128, 160, false, 0); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string>(0, 288, 8, false, 0); } set { }  }
    public int length { get { return _packet.GetBitOffsetLength<int>(0, 296, 16, false, 0.1); } set { }  }
    public int beam { get { return _packet.GetBitOffsetLength<int>(0, 312, 16, false, 0.1); } set { }  }
    public int positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int>(0, 328, 16, false, 0.1); } set { }  }
    public int positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int>(0, 344, 16, false, 0.1); } set { }  }
    public DateTime etaDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 360, 16, false, 1); } set { }  }
    public DateTime etaTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 376, 32, false, 0.0001); } set { }  }
    public int draft { get { return _packet.GetBitOffsetLength<int>(0, 408, 16, false, 0.01); } set { }  }
    public string destination { get { return _packet.GetBitOffsetLength<string>(0, 424, 160, false, 0); } set { }  }
    public string aisVersionIndicator { get { return _packet.GetBitOffsetLength<string>(0, 584, 2, false, 0); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string>(2, 586, 4, false, 0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string>(6, 590, 1, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(7, 591, 1, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 592, 5, false, 0); } set { }  }
}
 /// AIS Addressed Binary Message
 /// Fast
 /// 129795
 public class aisAddressedBinaryMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisAddressedBinaryMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 1, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(1, 41, 5, false, 0); } set { }  }
    public int sequenceNumber { get { return _packet.GetBitOffsetLength<int>(6, 46, 2, false, 0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 1); } set { }  }
    public int retransmitFlag { get { return _packet.GetBitOffsetLength<int>(6, 86, 1, false, 0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 1); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[]>(0, 104, 64, false, 0); } set { }  }
}
 /// AIS Acknowledge
 /// Fast
 /// 129796
 public class aisAcknowledge : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisAcknowledge(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 1, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(1, 41, 5, false, 0); } set { }  }
    public int destinationId1 { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0); } set { }  }
    public byte[] sequenceNumberForId1 { get { return _packet.GetBitOffsetLength<byte[]>(0, 80, 2, false, 0); } set { }  }
    public byte[] sequenceNumberForIdN { get { return _packet.GetBitOffsetLength<byte[]>(0, 88, 2, false, 0); } set { }  }
}
 /// AIS Binary Broadcast Message
 /// Fast
 /// 129797
 public class aisBinaryBroadcastMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisBinaryBroadcastMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 1, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(1, 41, 5, false, 0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[]>(0, 64, 2040, false, 0); } set { }  }
}
 /// AIS SAR Aircraft Position Report
 /// Fast
 /// 129798
 public class aisSarAircraftPositionReport : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisSarAircraftPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<double>(0, 40, 32, true, 1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<double>(0, 72, 32, true, 1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string>(0, 104, 1, false, 0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string>(1, 105, 1, false, 0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string>(2, 106, 6, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, false, 0.1); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[]>(0, 144, 19, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(3, 163, 5, false, 0); } set { }  }
    public int altitude { get { return _packet.GetBitOffsetLength<int>(0, 168, 64, true, 1E-06); } set { }  }
    public byte[] @reservedForRegionalApplications { get { return _packet.GetBitOffsetLength<byte[]>(0, 232, 8, false, 0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string>(0, 240, 1, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(1, 241, 7, false, 0); } set { }  }
}
 /// Radio Frequency/Mode/Power
 /// Fast
 /// 129799
 public class radioFrequencyModePower : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public radioFrequencyModePower(byte[] packet) { _packet = packet; }
    public int rxFrequency { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 10); } set { }  }
    public int txFrequency { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 10); } set { }  }
    public int radioChannel { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int txPower { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public int mode { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int channelBandwidth { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
}
 /// AIS UTC/Date Inquiry
 /// Fast
 /// 129800
 public class aisUtcDateInquiry : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisUtcDateInquiry(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int>(0, 48, 30, false, 0); } set { }  }
}
 /// AIS Addressed Safety Related Message
 /// Fast
 /// 129801
 public class aisAddressedSafetyRelatedMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisAddressedSafetyRelatedMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int sequenceNumber { get { return _packet.GetBitOffsetLength<int>(5, 45, 2, false, 0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int>(7, 47, 30, false, 0); } set { }  }
    public int retransmitFlag { get { return _packet.GetBitOffsetLength<int>(7, 79, 1, false, 0); } set { }  }
    public string safetyRelatedText { get { return _packet.GetBitOffsetLength<string>(7, 87, 2040, false, 0); } set { }  }
}
 /// AIS Safety Related Broadcast Message
 /// Fast
 /// 129802
 public class aisSafetyRelatedBroadcastMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisSafetyRelatedBroadcastMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public string safetyRelatedText { get { return _packet.GetBitOffsetLength<string>(0, 48, 288, false, 0); } set { }  }
}
 /// AIS Interrogation
 /// Single
 /// 129803
 public class aisInterrogation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisInterrogation(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int>(0, 48, 30, false, 1); } set { }  }
    public int messageIdA { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
    public int slotOffsetA { get { return _packet.GetBitOffsetLength<int>(0, 88, 14, false, 1); } set { }  }
    public int messageIdB { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 1); } set { }  }
    public int slotOffsetB { get { return _packet.GetBitOffsetLength<int>(0, 112, 14, false, 1); } set { }  }
}
 /// AIS Assignment Mode Command
 /// Fast
 /// 129804
 public class aisAssignmentModeCommand : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisAssignmentModeCommand(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 1); } set { }  }
    public int offset { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 1); } set { }  }
    public int increment { get { return _packet.GetBitOffsetLength<int>(0, 96, 16, false, 1); } set { }  }
}
 /// AIS Data Link Management Message
 /// Fast
 /// 129805
 public class aisDataLinkManagementMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisDataLinkManagementMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int offset { get { return _packet.GetBitOffsetLength<int>(0, 48, 10, false, 1); } set { }  }
    public int numberOfSlots { get { return _packet.GetBitOffsetLength<int>(2, 58, 8, false, 1); } set { }  }
    public int timeout { get { return _packet.GetBitOffsetLength<int>(2, 66, 8, false, 1); } set { }  }
    public int increment { get { return _packet.GetBitOffsetLength<int>(2, 74, 8, false, 1); } set { }  }
}
 /// AIS Channel Management
 /// Fast
 /// 129806
 public class aisChannelManagement : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisChannelManagement(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 40, 5, false, 0); } set { }  }
    public int channelA { get { return _packet.GetBitOffsetLength<int>(0, 48, 7, false, 0); } set { }  }
    public int channelB { get { return _packet.GetBitOffsetLength<int>(7, 55, 7, false, 0); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int txRxMode { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 1); } set { }  }
    public double northEastLongitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 80, 32, true, 1E-07); } set { }  }
    public double northEastLatitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 112, 32, true, 1E-07); } set { }  }
    public double southWestLongitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 144, 32, true, 1E-07); } set { }  }
    public double southWestLatitudeCorner2 { get { return _packet.GetBitOffsetLength<double>(0, 176, 32, true, 1E-07); } set { }  }
    public int addressedOrBroadcastMessageIndicator { get { return _packet.GetBitOffsetLength<int>(6, 214, 2, false, 0); } set { }  }
    public int channelABandwidth { get { return _packet.GetBitOffsetLength<int>(0, 216, 7, false, 1); } set { }  }
    public int channelBBandwidth { get { return _packet.GetBitOffsetLength<int>(7, 223, 7, false, 1); } set { }  }
    public int transitionalZoneSize { get { return _packet.GetBitOffsetLength<int>(0, 232, 8, false, 0); } set { }  }
}
 /// AIS Class B Group Assignment
 /// Fast
 /// 129807
 public class aisClassBGroupAssignment : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassBGroupAssignment(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 8, 30, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 38, 2, false, 0); } set { }  }
    public int txRxMode { get { return _packet.GetBitOffsetLength<int>(0, 40, 2, false, 1); } set { }  }
    public double northEastLongitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 48, 32, true, 1E-07); } set { }  }
    public double northEastLatitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 80, 32, true, 1E-07); } set { }  }
    public double southWestLongitudeCorner1 { get { return _packet.GetBitOffsetLength<double>(0, 112, 32, true, 1E-07); } set { }  }
    public double southWestLatitudeCorner2 { get { return _packet.GetBitOffsetLength<double>(0, 144, 32, true, 1E-07); } set { }  }
    public int stationType { get { return _packet.GetBitOffsetLength<int>(0, 176, 6, false, 0); } set { }  }
    public int shipAndCargoFilter { get { return _packet.GetBitOffsetLength<int>(0, 184, 6, false, 0); } set { }  }
    public int reportingInterval { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 0); } set { }  }
    public int quietTime { get { return _packet.GetBitOffsetLength<int>(0, 208, 16, false, 0); } set { }  }
}
 /// DSC Distress Call Information
 /// Fast
 /// 129808
 public class dscDistressCallInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public dscDistressCallInformation(byte[] packet) { _packet = packet; }
    public string dscFormat { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string dscCategory { get { return _packet.GetBitOffsetLength<string>(0, 8, 8, false, 0); } set { }  }
    public decimal dscMessageAddress { get { return _packet.GetBitOffsetLength<decimal>(0, 16, 40, false, 0); } set { }  }
    public string natureOfDistress { get { return _packet.GetBitOffsetLength<string>(0, 56, 8, false, 0); } set { }  }
    public string subsequentCommunicationModeOr2ndTelecommand { get { return _packet.GetBitOffsetLength<string>(0, 64, 8, false, 0); } set { }  }
    public string proposedRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 72, 48, false, 0); } set { }  }
    public string proposedTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 120, 48, false, 0); } set { }  }
    public string telephoneNumber { get { return _packet.GetBitOffsetLength<string>(0, 168, 16, false, 0); } set { }  }
    public double latitudeOfVesselReported { get { return _packet.GetBitOffsetLength<double>(0, 0, 32, true, 1E-07); } set { }  }
    public double longitudeOfVesselReported { get { return _packet.GetBitOffsetLength<double>(0, 0, 32, true, 1E-07); } set { }  }
    public DateTime timeOfPosition { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 32, false, 0.0001); } set { }  }
    public decimal mmsiOfShipInDistress { get { return _packet.GetBitOffsetLength<decimal>(0, 0, 40, false, 0); } set { }  }
    public int dscEosSymbol { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string expansionEnabled { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 0, 6, false, 0); } set { }  }
    public string callingRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 0, 48, false, 0); } set { }  }
    public string callingTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 0, 48, false, 0); } set { }  }
    public DateTime timeOfReceipt { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 32, false, 0.0001); } set { }  }
    public DateTime dateOfReceipt { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 16, false, 1); } set { }  }
    public int dscEquipmentAssignedMessageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public string dscExpansionFieldSymbol { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string dscExpansionFieldData { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
}
 /// DSC Call Information
 /// Fast
 /// 129808
 public class dscCallInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public dscCallInformation(byte[] packet) { _packet = packet; }
    public string dscFormatSymbol { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string dscCategorySymbol { get { return _packet.GetBitOffsetLength<string>(0, 8, 8, false, 0); } set { }  }
    public decimal dscMessageAddress { get { return _packet.GetBitOffsetLength<decimal>(0, 16, 40, false, 0); } set { }  }
    public string _1stTelecommand { get { return _packet.GetBitOffsetLength<string>(0, 56, 8, false, 0); } set { }  }
    public string subsequentCommunicationModeOr2ndTelecommand { get { return _packet.GetBitOffsetLength<string>(0, 64, 8, false, 0); } set { }  }
    public string proposedRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 72, 48, false, 0); } set { }  }
    public string proposedTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 120, 48, false, 0); } set { }  }
    public string telephoneNumber { get { return _packet.GetBitOffsetLength<string>(0, 168, 16, false, 0); } set { }  }
    public double latitudeOfVesselReported { get { return _packet.GetBitOffsetLength<double>(0, 0, 32, true, 1E-07); } set { }  }
    public double longitudeOfVesselReported { get { return _packet.GetBitOffsetLength<double>(0, 0, 32, true, 1E-07); } set { }  }
    public DateTime timeOfPosition { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 32, false, 0.0001); } set { }  }
    public decimal mmsiOfShipInDistress { get { return _packet.GetBitOffsetLength<decimal>(0, 0, 40, false, 0); } set { }  }
    public int dscEosSymbol { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public string expansionEnabled { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(2, 0, 6, false, 0); } set { }  }
    public string callingRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 0, 48, false, 0); } set { }  }
    public string callingTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string>(0, 0, 48, false, 0); } set { }  }
    public DateTime timeOfReceipt { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 32, false, 0.0001); } set { }  }
    public DateTime dateOfReceipt { get { return _packet.GetBitOffsetLength<DateTime>(0, 0, 16, false, 1); } set { }  }
    public int dscEquipmentAssignedMessageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 0); } set { }  }
    public string dscExpansionFieldSymbol { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string dscExpansionFieldData { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
}
 /// AIS Class B static data (msg 24 Part A)
 /// Fast
 /// 129809
 public class aisClassBStaticDataMsg24PartA : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassBStaticDataMsg24PartA(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 40, 160, false, 0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 200, 5, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(5, 205, 3, false, 0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int>(0, 208, 8, false, 1); } set { }  }
}
 /// AIS Class B static data (msg 24 Part B)
 /// Fast
 /// 129810
 public class aisClassBStaticDataMsg24PartB : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public aisClassBStaticDataMsg24PartB(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 0, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 8, 32, false, 1); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public string vendorId { get { return _packet.GetBitOffsetLength<string>(0, 48, 56, false, 0); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string>(0, 104, 56, false, 0); } set { }  }
    public int length { get { return _packet.GetBitOffsetLength<int>(0, 160, 16, false, 0.1); } set { }  }
    public int beam { get { return _packet.GetBitOffsetLength<int>(0, 176, 16, false, 0.1); } set { }  }
    public int positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 0.1); } set { }  }
    public int positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int>(0, 208, 16, false, 0.1); } set { }  }
    public int mothershipUserId { get { return _packet.GetBitOffsetLength<int>(0, 224, 32, false, 1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 256, 2, false, 0); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int>(2, 258, 6, false, 1); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string>(0, 264, 5, false, 0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int>(0, 272, 8, false, 1); } set { }  }
}
 /// Label
 /// Fast
 /// 130060
 public class label : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public label(byte[] packet) { _packet = packet; }
}
 /// Channel Source Configuration
 /// Fast
 /// 130061
 public class channelSourceConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public channelSourceConfiguration(byte[] packet) { _packet = packet; }
}
 /// Route and WP Service - Database List
 /// Fast
 /// 130064
 public class routeAndWpServiceDatabaseList : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceDatabaseList(byte[] packet) { _packet = packet; }
    public int startDatabaseId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfDatabasesAvailable { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string databaseName { get { return _packet.GetBitOffsetLength<string>(0, 32, 64, false, 0); } set { }  }
    public DateTime databaseTimestamp { get { return _packet.GetBitOffsetLength<DateTime>(0, 96, 32, false, 0.0001); } set { }  }
    public DateTime databaseDatestamp { get { return _packet.GetBitOffsetLength<DateTime>(0, 128, 16, false, 1); } set { }  }
    public int wpPositionResolution { get { return _packet.GetBitOffsetLength<int>(0, 144, 6, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 150, 2, false, 0); } set { }  }
    public int numberOfRoutesInDatabase { get { return _packet.GetBitOffsetLength<int>(0, 152, 16, false, 0); } set { }  }
    public int numberOfWpsInDatabase { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0); } set { }  }
    public int numberOfBytesInDatabase { get { return _packet.GetBitOffsetLength<int>(0, 184, 16, false, 0); } set { }  }
}
 /// Route and WP Service - Route List
 /// Fast
 /// 130065
 public class routeAndWpServiceRouteList : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRouteList(byte[] packet) { _packet = packet; }
    public int startRouteId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfRoutesInDatabase { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public string routeName { get { return _packet.GetBitOffsetLength<string>(0, 40, 64, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 104, 4, false, 0); } set { }  }
    public int wpIdentificationMethod { get { return _packet.GetBitOffsetLength<int>(4, 108, 2, false, 0); } set { }  }
    public int routeStatus { get { return _packet.GetBitOffsetLength<int>(6, 110, 2, false, 0); } set { }  }
}
 /// Route and WP Service - Route/WP-List Attributes
 /// Fast
 /// 130066
 public class routeAndWpServiceRouteWpListAttributes : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRouteWpListAttributes(byte[] packet) { _packet = packet; }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string routeWpListName { get { return _packet.GetBitOffsetLength<string>(0, 16, 64, false, 0); } set { }  }
    public DateTime routeWpListTimestamp { get { return _packet.GetBitOffsetLength<DateTime>(0, 80, 32, false, 0.0001); } set { }  }
    public DateTime routeWpListDatestamp { get { return _packet.GetBitOffsetLength<DateTime>(0, 112, 16, false, 1); } set { }  }
    public int changeAtLastTimestamp { get { return _packet.GetBitOffsetLength<int>(0, 128, 8, false, 0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, false, 0); } set { }  }
    public int criticalSupplementaryParameters { get { return _packet.GetBitOffsetLength<int>(0, 152, 8, false, 0); } set { }  }
    public int navigationMethod { get { return _packet.GetBitOffsetLength<int>(0, 160, 2, false, 0); } set { }  }
    public int wpIdentificationMethod { get { return _packet.GetBitOffsetLength<int>(2, 162, 2, false, 0); } set { }  }
    public int routeStatus { get { return _packet.GetBitOffsetLength<int>(4, 164, 2, false, 0); } set { }  }
    public int xteLimitForTheRoute { get { return _packet.GetBitOffsetLength<int>(6, 166, 16, false, 0); } set { }  }
}
 /// Route and WP Service - Route - WP Name & Position
 /// Fast
 /// 130067
 public class routeAndWpServiceRouteWpNamePosition : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRouteWpNamePosition(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string>(0, 56, 64, false, 0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<double>(0, 120, 32, true, 1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<double>(0, 152, 32, true, 1E-07); } set { }  }
}
 /// Route and WP Service - Route - WP Name
 /// Fast
 /// 130068
 public class routeAndWpServiceRouteWpName : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRouteWpName(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string>(0, 56, 64, false, 0); } set { }  }
}
 /// Route and WP Service - XTE Limit & Navigation Method
 /// Fast
 /// 130069
 public class routeAndWpServiceXteLimitNavigationMethod : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceXteLimitNavigationMethod(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfWpsWithASpecificXteLimitOrNavMethod { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int rps { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int xteLimitInTheLegAfterWp { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0); } set { }  }
    public int navMethodInTheLegAfterWp { get { return _packet.GetBitOffsetLength<int>(0, 72, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 76, 4, false, 0); } set { }  }
}
 /// Route and WP Service - WP Comment
 /// Fast
 /// 130070
 public class routeAndWpServiceWpComment : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceWpComment(byte[] packet) { _packet = packet; }
    public int startId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfWpsWithComments { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int wpIdRps { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string>(0, 56, 64, false, 0); } set { }  }
}
 /// Route and WP Service - Route Comment
 /// Fast
 /// 130071
 public class routeAndWpServiceRouteComment : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRouteComment(byte[] packet) { _packet = packet; }
    public int startRouteId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfRoutesWithComments { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string>(0, 48, 64, false, 0); } set { }  }
}
 /// Route and WP Service - Database Comment
 /// Fast
 /// 130072
 public class routeAndWpServiceDatabaseComment : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceDatabaseComment(byte[] packet) { _packet = packet; }
    public int startDatabaseId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfDatabasesWithComments { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string>(0, 40, 64, false, 0); } set { }  }
}
 /// Route and WP Service - Radius of Turn
 /// Fast
 /// 130073
 public class routeAndWpServiceRadiusOfTurn : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceRadiusOfTurn(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfWpsWithASpecificRadiusOfTurn { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int rps { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int radiusOfTurn { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0); } set { }  }
}
 /// Route and WP Service - WP List - WP Name & Position
 /// Fast
 /// 130074
 public class routeAndWpServiceWpListWpNamePosition : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public routeAndWpServiceWpListWpNamePosition(byte[] packet) { _packet = packet; }
    public int startWpId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int numberOfValidWpsInTheWpList { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 40, 8, false, 0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string>(0, 56, 64, false, 0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<double>(0, 120, 32, true, 1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<double>(0, 152, 32, true, 1E-07); } set { }  }
}
 /// Wind Data
 /// Single
 /// 130306
 public class windData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public windData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int windSpeed { get { return _packet.GetBitOffsetLength<int>(0, 8, 16, false, 0.01); } set { }  }
    public int windAngle { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0.0001); } set { }  }
    public string reference { get { return _packet.GetBitOffsetLength<string>(0, 40, 3, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(3, 43, 21, false, 0); } set { }  }
}
 /// Environmental Parameters
 /// Single
 /// 130310
 public class environmentalParameters : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public environmentalParameters(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<double>(0, 8, 16, false, 0.01); } set { }  }
    public double outsideAmbientAirTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(0, 56, 8, false, 0); } set { }  }
}
 /// Temperature
 /// Single
 /// 130312
 public class temperature : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public temperature(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.01); } set { }  }
}
 /// Humidity
 /// Single
 /// 130313
 public class humidity : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public humidity(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public int actualHumidity { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, true, 0.004); } set { }  }
    public int setHumidity { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, true, 0.004); } set { }  }
}
 /// Actual Pressure
 /// Single
 /// 130314
 public class actualPressure : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public actualPressure(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public double pressure { get { return _packet.GetBitOffsetLength<double>(0, 24, 32, true, 0.1); } set { }  }
}
 /// Set Pressure
 /// Single
 /// 130315
 public class setPressure : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public setPressure(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public double pressure { get { return _packet.GetBitOffsetLength<double>(0, 24, 32, false, 0.1); } set { }  }
}
 /// Temperature Extended Range
 /// Single
 /// 130316
 public class temperatureExtendedRange : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public temperatureExtendedRange(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 24, false, 0.001); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<double>(0, 48, 16, false, 0.1); } set { }  }
}
 /// Tide Station Data
 /// Fast
 /// 130320
 public class tideStationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public tideStationData(byte[] packet) { _packet = packet; }
    public string mode { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public string tideTendency { get { return _packet.GetBitOffsetLength<string>(4, 4, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 6, 2, false, 0); } set { }  }
    public DateTime measurementDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime measurementTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 32, true, 1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 88, 32, true, 1E-07); } set { }  }
    public int tideLevel { get { return _packet.GetBitOffsetLength<int>(0, 120, 16, true, 0.001); } set { }  }
    public int tideLevelStandardDeviation { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, false, 0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string>(0, 152, 16, false, 0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string>(0, 168, 16, false, 0); } set { }  }
}
 /// Salinity Station Data
 /// Fast
 /// 130321
 public class salinityStationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public salinityStationData(byte[] packet) { _packet = packet; }
    public string mode { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 4, 4, false, 0); } set { }  }
    public DateTime measurementDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime measurementTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 32, true, 1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 88, 32, true, 1E-07); } set { }  }
    public float salinity { get { return _packet.GetBitOffsetLength<float>(0, 120, 32, true, 0); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<double>(0, 152, 16, false, 0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string>(0, 168, 16, false, 0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string>(0, 184, 16, false, 0); } set { }  }
}
 /// Current Station Data
 /// Fast
 /// 130322
 public class currentStationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public currentStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int>(0, 0, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 4, 4, false, 0); } set { }  }
    public DateTime measurementDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime measurementTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 32, true, 1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 88, 32, true, 1E-07); } set { }  }
    public int measurementDepth { get { return _packet.GetBitOffsetLength<int>(0, 120, 32, false, 0.01); } set { }  }
    public int currentSpeed { get { return _packet.GetBitOffsetLength<int>(0, 152, 16, false, 0.01); } set { }  }
    public int currentFlowDirection { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0.0001); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<double>(0, 184, 16, false, 0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string>(0, 200, 16, false, 0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string>(0, 216, 16, false, 0); } set { }  }
}
 /// Meteorological Station Data
 /// Fast
 /// 130323
 public class meteorologicalStationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public meteorologicalStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int>(0, 0, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 4, 4, false, 0); } set { }  }
    public DateTime measurementDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime measurementTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 32, true, 1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 88, 32, true, 1E-07); } set { }  }
    public int windSpeed { get { return _packet.GetBitOffsetLength<int>(0, 120, 16, false, 0.01); } set { }  }
    public int windDirection { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, false, 0.0001); } set { }  }
    public string windReference { get { return _packet.GetBitOffsetLength<string>(0, 152, 3, false, 0); } set { }  }
    public int windGusts { get { return _packet.GetBitOffsetLength<int>(0, 160, 16, false, 0.01); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<double>(0, 176, 16, false, 0); } set { }  }
    public double ambientTemperature { get { return _packet.GetBitOffsetLength<double>(0, 192, 16, false, 0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string>(0, 208, 2056, false, 0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string>(0, 2264, 2056, false, 0); } set { }  }
}
 /// Moored Buoy Station Data
 /// Fast
 /// 130324
 public class mooredBuoyStationData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public mooredBuoyStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int>(0, 0, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 4, 4, false, 0); } set { }  }
    public DateTime measurementDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 8, 16, false, 1); } set { }  }
    public DateTime measurementTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 24, 32, false, 0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<double>(0, 56, 32, true, 1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<double>(0, 88, 32, true, 1E-07); } set { }  }
    public int windSpeed { get { return _packet.GetBitOffsetLength<int>(0, 120, 16, false, 0.01); } set { }  }
    public int windDirection { get { return _packet.GetBitOffsetLength<int>(0, 136, 16, false, 0.0001); } set { }  }
    public string windReference { get { return _packet.GetBitOffsetLength<string>(0, 152, 3, false, 0); } set { }  }
    public int windGusts { get { return _packet.GetBitOffsetLength<int>(0, 160, 16, false, 0.01); } set { }  }
    public int waveHeight { get { return _packet.GetBitOffsetLength<int>(0, 176, 16, false, 0); } set { }  }
    public int dominantWavePeriod { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 0); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<double>(0, 208, 16, false, 0); } set { }  }
    public int pressureTendencyRate { get { return _packet.GetBitOffsetLength<int>(0, 224, 16, false, 0); } set { }  }
    public double airTemperature { get { return _packet.GetBitOffsetLength<double>(0, 240, 16, false, 0.01); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<double>(0, 256, 16, false, 0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string>(0, 272, 64, false, 0); } set { }  }
}
 /// Payload Mass
 /// Fast
 /// 130560
 public class payloadMass : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public payloadMass(byte[] packet) { _packet = packet; }
}
 /// Watermaker Input Setting and Status
 /// Fast
 /// 130567
 public class watermakerInputSettingAndStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public watermakerInputSettingAndStatus(byte[] packet) { _packet = packet; }
    public string watermakerOperatingState { get { return _packet.GetBitOffsetLength<string>(0, 0, 6, false, 0); } set { }  }
    public string productionStartStop { get { return _packet.GetBitOffsetLength<string>(6, 6, 2, false, 0); } set { }  }
    public string rinseStartStop { get { return _packet.GetBitOffsetLength<string>(0, 8, 2, false, 0); } set { }  }
    public string lowPressurePumpStatus { get { return _packet.GetBitOffsetLength<string>(2, 10, 2, false, 0); } set { }  }
    public string highPressurePumpStatus { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public string emergencyStop { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public string productSolenoidValveStatus { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string flushModeStatus { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public string salinityStatus { get { return _packet.GetBitOffsetLength<string>(4, 20, 2, false, 0); } set { }  }
    public string sensorStatus { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public string oilChangeIndicatorStatus { get { return _packet.GetBitOffsetLength<string>(0, 24, 2, false, 0); } set { }  }
    public string filterStatus { get { return _packet.GetBitOffsetLength<string>(2, 26, 2, false, 0); } set { }  }
    public string systemStatus { get { return _packet.GetBitOffsetLength<string>(4, 28, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 30, 2, false, 0); } set { }  }
    public int salinity { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 1); } set { }  }
    public double productWaterTemperature { get { return _packet.GetBitOffsetLength<double>(0, 48, 16, false, 0.01); } set { }  }
    public double preFilterPressure { get { return _packet.GetBitOffsetLength<double>(0, 64, 16, false, 0); } set { }  }
    public double postFilterPressure { get { return _packet.GetBitOffsetLength<double>(0, 80, 16, false, 0); } set { }  }
    public double feedPressure { get { return _packet.GetBitOffsetLength<double>(0, 96, 16, true, 0); } set { }  }
    public double systemHighPressure { get { return _packet.GetBitOffsetLength<double>(0, 112, 16, false, 0); } set { }  }
    public int productWaterFlow { get { return _packet.GetBitOffsetLength<int>(0, 128, 16, true, 0.1); } set { }  }
    public int brineWaterFlow { get { return _packet.GetBitOffsetLength<int>(0, 144, 16, true, 0.1); } set { }  }
    public int runTime { get { return _packet.GetBitOffsetLength<int>(0, 160, 32, false, 1); } set { }  }
}
 /// Current Status and File
 /// Fast
 /// 130569
 public class currentStatusAndFile : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public currentStatusAndFile(byte[] packet) { _packet = packet; }
    public string zone { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 8, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 1); } set { }  }
    public string playStatus { get { return _packet.GetBitOffsetLength<string>(0, 56, 8, false, 0); } set { }  }
    public DateTime elapsedTrackTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 64, 16, false, 0.0001); } set { }  }
    public DateTime trackTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 80, 16, false, 0.0001); } set { }  }
    public string repeatStatus { get { return _packet.GetBitOffsetLength<string>(0, 96, 4, false, 0); } set { }  }
    public string shuffleStatus { get { return _packet.GetBitOffsetLength<string>(4, 100, 4, false, 0); } set { }  }
    public int saveFavoriteNumber { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 1); } set { }  }
    public int playFavoriteNumber { get { return _packet.GetBitOffsetLength<int>(0, 112, 16, false, 1); } set { }  }
    public int thumbsUpDown { get { return _packet.GetBitOffsetLength<int>(0, 128, 8, false, 1); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int>(0, 136, 8, false, 1); } set { }  }
    public int radioFrequency { get { return _packet.GetBitOffsetLength<int>(0, 144, 32, false, 10); } set { }  }
    public int hdFrequencyMulticast { get { return _packet.GetBitOffsetLength<int>(0, 176, 8, false, 1); } set { }  }
    public int deleteFavoriteNumber { get { return _packet.GetBitOffsetLength<int>(0, 184, 8, false, 1); } set { }  }
    public int totalNumberOfTracks { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 1); } set { }  }
}
 /// Library Data File
 /// Fast
 /// 130570
 public class libraryDataFile : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public libraryDataFile(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int>(0, 16, 32, false, 1); } set { }  }
    public string type { get { return _packet.GetBitOffsetLength<string>(0, 48, 8, false, 0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 56, 16, false, 0); } set { }  }
    public int track { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 1); } set { }  }
    public int station { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 1); } set { }  }
    public int favorite { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int radioFrequency { get { return _packet.GetBitOffsetLength<int>(0, 0, 32, false, 10); } set { }  }
    public int hdFrequency { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string inPlayQueue { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public string lockStatus { get { return _packet.GetBitOffsetLength<string>(2, 0, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 0, 4, false, 0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
    public string album { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
}
 /// Library Data Group
 /// Fast
 /// 130571
 public class libraryDataGroup : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public libraryDataGroup(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public int groupId { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 1); } set { }  }
    public int idOffset { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 1); } set { }  }
    public int idCount { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 1); } set { }  }
    public int totalIdCount { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 1); } set { }  }
    public string idType { get { return _packet.GetBitOffsetLength<string>(0, 104, 8, false, 0); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int>(0, 112, 32, false, 1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 144, 16, false, 0); } set { }  }
}
 /// Library Data Search
 /// Fast
 /// 130572
 public class libraryDataSearch : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public libraryDataSearch(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int groupId { get { return _packet.GetBitOffsetLength<int>(0, 16, 32, false, 1); } set { }  }
    public string groupType1 { get { return _packet.GetBitOffsetLength<string>(0, 48, 8, false, 0); } set { }  }
    public string groupName1 { get { return _packet.GetBitOffsetLength<string>(0, 56, 16, false, 0); } set { }  }
    public string groupType2 { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string groupName2 { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
    public string groupType3 { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public string groupName3 { get { return _packet.GetBitOffsetLength<string>(0, 0, 16, false, 0); } set { }  }
}
 /// Supported Source Data
 /// Fast
 /// 130573
 public class supportedSourceData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public supportedSourceData(byte[] packet) { _packet = packet; }
    public int idOffset { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, false, 1); } set { }  }
    public int idCount { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public int totalIdCount { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 56, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 72, 16, false, 0); } set { }  }
    public long playSupport { get { return _packet.GetBitOffsetLength<long>(0, 0, 16, false, 0); } set { }  }
    public long browseSupport { get { return _packet.GetBitOffsetLength<long>(0, 0, 16, false, 0); } set { }  }
    public string thumbsSupport { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public string connected { get { return _packet.GetBitOffsetLength<string>(2, 0, 2, false, 0); } set { }  }
    public long repeatSupport { get { return _packet.GetBitOffsetLength<long>(4, 0, 2, false, 0); } set { }  }
    public long shuffleSupport { get { return _packet.GetBitOffsetLength<long>(6, 0, 2, false, 0); } set { }  }
}
 /// Supported Zone Data
 /// Fast
 /// 130574
 public class supportedZoneData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public supportedZoneData(byte[] packet) { _packet = packet; }
    public int firstZoneId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int zoneCount { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int totalZoneCount { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string zoneId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 32, 16, false, 0); } set { }  }
}
 /// Small Craft Status
 /// Single
 /// 130576
 public class smallCraftStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public smallCraftStatus(byte[] packet) { _packet = packet; }
    public int portTrimTab { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, true, 0); } set { }  }
    public int starboardTrimTab { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, true, 0); } set { }  }
}
 /// Direction Data
 /// Fast
 /// 130577
 public class directionData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public directionData(byte[] packet) { _packet = packet; }
    public string dataMode { get { return _packet.GetBitOffsetLength<string>(0, 0, 4, false, 0); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string>(4, 4, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(6, 6, 2, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 0); } set { }  }
    public int cog { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0.0001); } set { }  }
    public int sog { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0.01); } set { }  }
    public int heading { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0.0001); } set { }  }
    public int speedThroughWater { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0.01); } set { }  }
    public int set { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 0.0001); } set { }  }
    public int drift { get { return _packet.GetBitOffsetLength<int>(0, 96, 16, false, 0.01); } set { }  }
}
 /// Vessel Speed Components
 /// Fast
 /// 130578
 public class vesselSpeedComponents : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public vesselSpeedComponents(byte[] packet) { _packet = packet; }
    public int longitudinalSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int>(0, 0, 16, true, 0.001); } set { }  }
    public int transverseSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, true, 0.001); } set { }  }
    public int longitudinalSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.001); } set { }  }
    public int transverseSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 0.001); } set { }  }
    public int sternSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, true, 0.001); } set { }  }
    public int sternSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, true, 0.001); } set { }  }
}
 /// System Configuration
 /// Fast
 /// 130579
 public class systemConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public systemConfiguration(byte[] packet) { _packet = packet; }
    public string power { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public string defaultSettings { get { return _packet.GetBitOffsetLength<string>(2, 2, 2, false, 0); } set { }  }
    public string tunerRegions { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int maxFavorites { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public long videoProtocols { get { return _packet.GetBitOffsetLength<long>(0, 16, 4, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 20, 44, false, 0); } set { }  }
}
 /// System Configuration (deprecated)
 /// Fast
 /// 130580
 public class systemConfigurationDeprecated : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public systemConfigurationDeprecated(byte[] packet) { _packet = packet; }
    public string power { get { return _packet.GetBitOffsetLength<string>(0, 0, 2, false, 0); } set { }  }
    public string defaultSettings { get { return _packet.GetBitOffsetLength<string>(2, 2, 2, false, 0); } set { }  }
    public string tunerRegions { get { return _packet.GetBitOffsetLength<string>(4, 4, 4, false, 0); } set { }  }
    public int maxFavorites { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
}
 /// Zone Configuration (deprecated)
 /// Fast
 /// 130581
 public class zoneConfigurationDeprecated : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public zoneConfigurationDeprecated(byte[] packet) { _packet = packet; }
    public int firstZoneId { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int zoneCount { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int totalZoneCount { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string zoneId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string zoneName { get { return _packet.GetBitOffsetLength<string>(0, 32, 16, false, 0); } set { }  }
}
 /// Zone Volume
 /// Fast
 /// 130582
 public class zoneVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public zoneVolume(byte[] packet) { _packet = packet; }
    public string zoneId { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int volume { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public string volumeChange { get { return _packet.GetBitOffsetLength<string>(0, 16, 2, false, 0); } set { }  }
    public string mute { get { return _packet.GetBitOffsetLength<string>(2, 18, 2, false, 0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[]>(4, 20, 4, false, 0); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
}
 /// Available Audio EQ presets
 /// Fast
 /// 130583
 public class availableAudioEqPresets : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public availableAudioEqPresets(byte[] packet) { _packet = packet; }
    public int firstPreset { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int presetCount { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int totalPresetCount { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public string presetType { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string presetName { get { return _packet.GetBitOffsetLength<string>(0, 32, 16, false, 0); } set { }  }
}
 /// Available Bluetooth addresses
 /// Fast
 /// 130584
 public class availableBluetoothAddresses : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public availableBluetoothAddresses(byte[] packet) { _packet = packet; }
    public int firstAddress { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public int addressCount { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int totalAddressCount { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 1); } set { }  }
    public int bluetoothAddress { get { return _packet.GetBitOffsetLength<int>(0, 24, 48, false, 1); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string>(0, 72, 8, false, 0); } set { }  }
    public string deviceName { get { return _packet.GetBitOffsetLength<string>(0, 80, 16, false, 0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
}
 /// Bluetooth source status
 /// Fast
 /// 130585
 public class bluetoothSourceStatus : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bluetoothSourceStatus(byte[] packet) { _packet = packet; }
    public int sourceNumber { get { return _packet.GetBitOffsetLength<int>(0, 0, 8, false, 1); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string>(0, 8, 4, false, 0); } set { }  }
    public string forgetDevice { get { return _packet.GetBitOffsetLength<string>(4, 12, 2, false, 0); } set { }  }
    public string discovering { get { return _packet.GetBitOffsetLength<string>(6, 14, 2, false, 0); } set { }  }
    public int bluetoothAddress { get { return _packet.GetBitOffsetLength<int>(0, 16, 48, false, 1); } set { }  }
}
 /// Zone Configuration
 /// Fast
 /// 130586
 public class zoneConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public zoneConfiguration(byte[] packet) { _packet = packet; }
    public string zoneId { get { return _packet.GetBitOffsetLength<string>(0, 0, 8, false, 0); } set { }  }
    public int volumeLimit { get { return _packet.GetBitOffsetLength<int>(0, 8, 8, false, 1); } set { }  }
    public int fade { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, true, 1); } set { }  }
    public int balance { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, true, 1); } set { }  }
    public int subVolume { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, true, 1); } set { }  }
    public int eqTreble { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, true, 1); } set { }  }
    public int eqMidRange { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, true, 1); } set { }  }
    public int eqBass { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, true, 1); } set { }  }
    public string presetType { get { return _packet.GetBitOffsetLength<string>(0, 64, 8, false, 0); } set { }  }
    public string audioFilter { get { return _packet.GetBitOffsetLength<string>(0, 72, 8, false, 0); } set { }  }
    public int highPassFilterFrequency { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 1); } set { }  }
    public int lowPassFilterFrequency { get { return _packet.GetBitOffsetLength<int>(0, 96, 16, false, 1); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string>(0, 112, 8, false, 0); } set { }  }
}
 /// SonicHub: Init #2
 /// Fast
 /// 130816
 public class sonichubInit2 : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubInit2(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 40, 16, false, 1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 1); } set { }  }
}
 /// SonicHub: AM Radio
 /// Fast
 /// 130816
 public class sonichubAmRadio : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubAmRadio(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0.001); } set { }  }
    public int noiseLevel { get { return _packet.GetBitOffsetLength<int>(0, 80, 2, false, 0); } set { }  }
    public int signalLevel { get { return _packet.GetBitOffsetLength<int>(2, 82, 4, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 88, 256, false, 0); } set { }  }
}
 /// SonicHub: Zone info
 /// Fast
 /// 130816
 public class sonichubZoneInfo : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubZoneInfo(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int zone { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
}
 /// SonicHub: Source
 /// Fast
 /// 130816
 public class sonichubSource : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubSource(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
}
 /// SonicHub: Source List
 /// Fast
 /// 130816
 public class sonichubSourceList : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubSourceList(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 56, 256, false, 0); } set { }  }
}
 /// SonicHub: Control
 /// Fast
 /// 130816
 public class sonichubControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
}
 /// SonicHub: Unknown
 /// Fast
 /// 130816
 public class sonichubUnknown : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
}
 /// SonicHub: FM Radio
 /// Fast
 /// 130816
 public class sonichubFmRadio : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubFmRadio(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0.001); } set { }  }
    public int noiseLevel { get { return _packet.GetBitOffsetLength<int>(0, 80, 2, false, 0); } set { }  }
    public int signalLevel { get { return _packet.GetBitOffsetLength<int>(2, 82, 4, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 88, 256, false, 0); } set { }  }
}
 /// SonicHub: Playlist
 /// Fast
 /// 130816
 public class sonichubPlaylist : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubPlaylist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
    public int currentTrack { get { return _packet.GetBitOffsetLength<int>(0, 56, 32, false, 1); } set { }  }
    public int tracks { get { return _packet.GetBitOffsetLength<int>(0, 88, 32, false, 1); } set { }  }
    public int length { get { return _packet.GetBitOffsetLength<int>(0, 120, 32, false, 0.001); } set { }  }
    public int positionInTrack { get { return _packet.GetBitOffsetLength<int>(0, 152, 32, false, 0.001); } set { }  }
}
 /// SonicHub: Track
 /// Fast
 /// 130816
 public class sonichubTrack : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubTrack(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 72, 256, false, 0); } set { }  }
}
 /// SonicHub: Artist
 /// Fast
 /// 130816
 public class sonichubArtist : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 72, 256, false, 0); } set { }  }
}
 /// SonicHub: Album
 /// Fast
 /// 130816
 public class sonichubAlbum : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubAlbum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 72, 256, false, 0); } set { }  }
}
 /// SonicHub: Menu Item
 /// Fast
 /// 130816
 public class sonichubMenuItem : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubMenuItem(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 96, 256, false, 0); } set { }  }
}
 /// SonicHub: Zones
 /// Fast
 /// 130816
 public class sonichubZones : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubZones(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int zones { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
}
 /// SonicHub: Max Volume
 /// Fast
 /// 130816
 public class sonichubMaxVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubMaxVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int level { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
}
 /// SonicHub: Volume
 /// Fast
 /// 130816
 public class sonichubVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
    public int level { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
}
 /// SonicHub: Init #1
 /// Fast
 /// 130816
 public class sonichubInit1 : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubInit1(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
}
 /// SonicHub: Position
 /// Fast
 /// 130816
 public class sonichubPosition : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubPosition(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int position { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 0.001); } set { }  }
}
 /// SonicHub: Init #3
 /// Fast
 /// 130816
 public class sonichubInit3 : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public sonichubInit3(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 1); } set { }  }
}
 /// Simrad: Text Message
 /// Fast
 /// 130816
 public class simradTextMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simradTextMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 24, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int prio { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 72, 256, false, 0); } set { }  }
}
 /// Manufacturer Proprietary fast-packet non-addressed
 /// Fast
 /// 130816
 public class manufacturerProprietaryFastPacketNonAddressed : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public manufacturerProprietaryFastPacketNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 16, 1768, false, 0); } set { }  }
}
 /// Navico: Product Information
 /// Fast
 /// 130817
 public class navicoProductInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public navicoProductInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public string model { get { return _packet.GetBitOffsetLength<string>(0, 32, 256, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 288, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 296, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 304, 8, false, 0); } set { }  }
    public string firmwareVersion { get { return _packet.GetBitOffsetLength<string>(0, 312, 80, false, 0); } set { }  }
    public string firmwareDate { get { return _packet.GetBitOffsetLength<string>(0, 392, 256, false, 0); } set { }  }
    public string firmwareTime { get { return _packet.GetBitOffsetLength<string>(0, 648, 256, false, 0); } set { }  }
}
 /// Simnet: Reprogram Data
 /// Fast
 /// 130818
 public class simnetReprogramData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetReprogramData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int version { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 1); } set { }  }
    public int sequence { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 1); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[]>(0, 48, 1736, false, 0); } set { }  }
}
 /// Simnet: Request Reprogram
 /// Fast
 /// 130819
 public class simnetRequestReprogram : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetRequestReprogram(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Furuno: Unknown
 /// Fast
 /// 130820
 public class furunoUnknown : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
}
 /// Fusion: Source Name
 /// Fast
 /// 130820
 public class fusionSourceName : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSourceName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int currentSourceId { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 64, 40, false, 0); } set { }  }
}
 /// Fusion: Track Info
 /// Fast
 /// 130820
 public class fusionTrackInfo : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionTrackInfo(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public string transport { get { return _packet.GetBitOffsetLength<string>(0, 40, 4, false, 0); } set { }  }
    public int x { get { return _packet.GetBitOffsetLength<int>(4, 44, 4, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int track { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0); } set { }  }
    public int trackCount { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 104, 16, false, 0); } set { }  }
    public int trackLength { get { return _packet.GetBitOffsetLength<int>(0, 120, 24, false, 0.001); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 144, 24, false, 0.001); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 168, 16, false, 0); } set { }  }
}
 /// Fusion: Track
 /// Fast
 /// 130820
 public class fusionTrack : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionTrack(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 40, false, 0); } set { }  }
    public string track { get { return _packet.GetBitOffsetLength<string>(0, 72, 80, false, 0); } set { }  }
}
 /// Fusion: Artist
 /// Fast
 /// 130820
 public class fusionArtist : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 40, false, 0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string>(0, 72, 80, false, 0); } set { }  }
}
 /// Fusion: Album
 /// Fast
 /// 130820
 public class fusionAlbum : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionAlbum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 40, false, 0); } set { }  }
    public string album { get { return _packet.GetBitOffsetLength<string>(0, 72, 80, false, 0); } set { }  }
}
 /// Fusion: Unit Name
 /// Fast
 /// 130820
 public class fusionUnitName : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionUnitName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 32, 112, false, 0); } set { }  }
}
 /// Fusion: Zone Name
 /// Fast
 /// 130820
 public class fusionZoneName : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionZoneName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 40, 104, false, 0); } set { }  }
}
 /// Fusion: Play Progress
 /// Fast
 /// 130820
 public class fusionPlayProgress : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionPlayProgress(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int progress { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 0.001); } set { }  }
}
 /// Fusion: AM/FM Station
 /// Fast
 /// 130820
 public class fusionAmFmStation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionAmFmStation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string amFm { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 1E-06); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public string track { get { return _packet.GetBitOffsetLength<string>(0, 88, 80, false, 0); } set { }  }
}
 /// Fusion: VHF
 /// Fast
 /// 130820
 public class fusionVhf : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionVhf(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int channel { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 48, 24, false, 0); } set { }  }
}
 /// Fusion: Squelch
 /// Fast
 /// 130820
 public class fusionSquelch : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSquelch(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int squelch { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
}
 /// Fusion: Scan
 /// Fast
 /// 130820
 public class fusionScan : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionScan(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public string scan { get { return _packet.GetBitOffsetLength<string>(0, 40, 8, false, 0); } set { }  }
}
 /// Fusion: Menu Item
 /// Fast
 /// 130820
 public class fusionMenuItem : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionMenuItem(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int line { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 88, 40, false, 0); } set { }  }
}
 /// Fusion: Replay
 /// Fast
 /// 130820
 public class fusionReplay : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionReplay(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string mode { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 40, 24, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string>(0, 80, 8, false, 0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int>(0, 96, 8, false, 0); } set { }  }
    public int j { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, false, 0); } set { }  }
}
 /// Fusion: Sub Volume
 /// Fast
 /// 130820
 public class fusionSubVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSubVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Fusion: Tone
 /// Fast
 /// 130820
 public class fusionTone : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionTone(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int bass { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, true, 0); } set { }  }
    public int mid { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, true, 0); } set { }  }
    public int treble { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, true, 0); } set { }  }
}
 /// Fusion: Volume
 /// Fast
 /// 130820
 public class fusionVolume : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
}
 /// Fusion: Power State
 /// Fast
 /// 130820
 public class fusionPowerState : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionPowerState(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string state { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
}
 /// Fusion: SiriusXM Channel
 /// Fast
 /// 130820
 public class fusionSiriusxmChannel : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSiriusxmChannel(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 0); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string>(0, 56, 96, false, 0); } set { }  }
}
 /// Fusion: SiriusXM Title
 /// Fast
 /// 130820
 public class fusionSiriusxmTitle : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSiriusxmTitle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 0); } set { }  }
    public string title { get { return _packet.GetBitOffsetLength<string>(0, 56, 96, false, 0); } set { }  }
}
 /// Fusion: SiriusXM Artist
 /// Fast
 /// 130820
 public class fusionSiriusxmArtist : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSiriusxmArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string>(0, 56, 96, false, 0); } set { }  }
}
 /// Fusion: SiriusXM Genre
 /// Fast
 /// 130820
 public class fusionSiriusxmGenre : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public fusionSiriusxmGenre(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 24, 32, false, 0); } set { }  }
    public string genre { get { return _packet.GetBitOffsetLength<string>(0, 56, 96, false, 0); } set { }  }
}
 /// Maretron: Proprietary Temperature High Range
 /// Fast
 /// 130823
 public class maretronProprietaryTemperatureHighRange : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronProprietaryTemperatureHighRange(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.1); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<double>(0, 56, 16, false, 0.1); } set { }  }
}
 /// B&G: Wind data
 /// Single
 /// 130824
 public class bGWindData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public bGWindData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int field4 { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int field5 { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int timestamp { get { return _packet.GetBitOffsetLength<int>(0, 32, 32, false, 0); } set { }  }
}
 /// Maretron: Annunciator
 /// Fast
 /// 130824
 public class maretronAnnunciator : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronAnnunciator(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int field4 { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int field5 { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int field6 { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, false, 0); } set { }  }
    public int field7 { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int field8 { get { return _packet.GetBitOffsetLength<int>(0, 56, 16, false, 0); } set { }  }
}
 /// Lowrance: unknown
 /// Fast
 /// 130827
 public class lowranceUnknown : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public lowranceUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
}
 /// Simnet: Set Serial Number
 /// Fast
 /// 130828
 public class simnetSetSerialNumber : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetSetSerialNumber(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Suzuki: Engine and Storage Device Config
 /// Fast
 /// 130831
 public class suzukiEngineAndStorageDeviceConfig : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public suzukiEngineAndStorageDeviceConfig(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Fuel Used - High Resolution
 /// Fast
 /// 130832
 public class simnetFuelUsedHighResolution : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetFuelUsedHighResolution(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Engine and Tank Configuration
 /// Fast
 /// 130834
 public class simnetEngineAndTankConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetEngineAndTankConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Set Engine and Tank Configuration
 /// Fast
 /// 130835
 public class simnetSetEngineAndTankConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetSetEngineAndTankConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Fluid Level Sensor Configuration
 /// Fast
 /// 130836
 public class simnetFluidLevelSensorConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetFluidLevelSensorConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int device { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 1); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 40, 4, false, 0); } set { }  }
    public string tankType { get { return _packet.GetBitOffsetLength<string>(4, 44, 4, false, 0); } set { }  }
    public int capacity { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, false, 0.1); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, true, 0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int>(0, 104, 8, true, 0); } set { }  }
}
 /// Maretron Proprietary Switch Status Counter
 /// Fast
 /// 130836
 public class maretronProprietarySwitchStatusCounter : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronProprietarySwitchStatusCounter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public DateTime startDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 32, 16, false, 1); } set { }  }
    public DateTime startTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 48, 32, false, 0.0001); } set { }  }
    public int offCounter { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 1); } set { }  }
    public int onCounter { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 1); } set { }  }
    public int errorCounter { get { return _packet.GetBitOffsetLength<int>(0, 96, 8, false, 1); } set { }  }
    public string switchStatus { get { return _packet.GetBitOffsetLength<string>(0, 104, 2, false, 0); } set { }  }
}
 /// Simnet: Fuel Flow Turbine Configuration
 /// Fast
 /// 130837
 public class simnetFuelFlowTurbineConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetFuelFlowTurbineConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Maretron Proprietary Switch Status Timer
 /// Fast
 /// 130837
 public class maretronProprietarySwitchStatusTimer : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public maretronProprietarySwitchStatusTimer(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public DateTime startDate { get { return _packet.GetBitOffsetLength<DateTime>(0, 32, 16, false, 1); } set { }  }
    public DateTime startTime { get { return _packet.GetBitOffsetLength<DateTime>(0, 48, 32, false, 0.0001); } set { }  }
    public decimal accumulatedOffPeriod { get { return _packet.GetBitOffsetLength<decimal>(0, 80, 32, false, 0); } set { }  }
    public decimal accumulatedOnPeriod { get { return _packet.GetBitOffsetLength<decimal>(0, 112, 32, false, 0); } set { }  }
    public decimal accumulatedErrorPeriod { get { return _packet.GetBitOffsetLength<decimal>(0, 144, 32, false, 0); } set { }  }
    public string switchStatus { get { return _packet.GetBitOffsetLength<string>(0, 176, 2, false, 0); } set { }  }
}
 /// Simnet: Fluid Level Warning
 /// Fast
 /// 130838
 public class simnetFluidLevelWarning : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetFluidLevelWarning(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Pressure Sensor Configuration
 /// Fast
 /// 130839
 public class simnetPressureSensorConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetPressureSensorConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Data User Group Configuration
 /// Fast
 /// 130840
 public class simnetDataUserGroupConfiguration : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetDataUserGroupConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: AIS Class B static data (msg 24 Part A)
 /// Fast
 /// 130842
 public class simnetAisClassBStaticDataMsg24PartA : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetAisClassBStaticDataMsg24PartA(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string>(0, 72, 160, false, 0); } set { }  }
}
 /// Furuno: Six Degrees Of Freedom Movement
 /// Fast
 /// 130842
 public class furunoSixDegreesOfFreedomMovement : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoSixDegreesOfFreedomMovement(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 32, true, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 48, 32, true, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 80, 32, true, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 112, 8, true, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 120, 32, true, 0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 152, 32, true, 0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 184, 16, true, 0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 200, 16, true, 0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int>(0, 216, 16, true, 0); } set { }  }
}
 /// Simnet: AIS Class B static data (msg 24 Part B)
 /// Fast
 /// 130842
 public class simnetAisClassBStaticDataMsg24PartB : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetAisClassBStaticDataMsg24PartB(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int>(0, 40, 32, false, 1); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string>(0, 72, 8, false, 0); } set { }  }
    public string vendorId { get { return _packet.GetBitOffsetLength<string>(0, 80, 56, false, 0); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string>(0, 136, 56, false, 0); } set { }  }
    public int length { get { return _packet.GetBitOffsetLength<int>(0, 192, 16, false, 0.1); } set { }  }
    public int beam { get { return _packet.GetBitOffsetLength<int>(0, 208, 16, false, 0.1); } set { }  }
    public int positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int>(0, 224, 16, false, 0.1); } set { }  }
    public int positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int>(0, 240, 16, false, 0.1); } set { }  }
    public int mothershipUserId { get { return _packet.GetBitOffsetLength<int>(0, 256, 32, false, 1); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int>(2, 290, 6, false, 1); } set { }  }
}
 /// Furuno: Heel Angle, Roll Information
 /// Fast
 /// 130843
 public class furunoHeelAngleRollInformation : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoHeelAngleRollInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int yaw { get { return _packet.GetBitOffsetLength<int>(0, 32, 16, true, 0.0001); } set { }  }
    public int pitch { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, true, 0.0001); } set { }  }
    public int roll { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, true, 0.0001); } set { }  }
}
 /// Simnet: Sonar Status, Frequency and DSP Voltage
 /// Fast
 /// 130843
 public class simnetSonarStatusFrequencyAndDspVoltage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetSonarStatusFrequencyAndDspVoltage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Compass Heading Offset
 /// Fast
 /// 130845
 public class simnetCompassHeadingOffset : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetCompassHeadingOffset(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int>(0, 24, 24, false, 0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
    public int angle { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, true, 0.0001); } set { }  }
}
 /// Furuno: Multi Sats In View Extended
 /// Fast
 /// 130845
 public class furunoMultiSatsInViewExtended : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoMultiSatsInViewExtended(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// Simnet: Compass Local Field
 /// Fast
 /// 130845
 public class simnetCompassLocalField : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetCompassLocalField(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int>(0, 24, 24, false, 0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
    public int localField { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 0.004); } set { }  }
}
 /// Simnet: Compass Field Angle
 /// Fast
 /// 130845
 public class simnetCompassFieldAngle : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetCompassFieldAngle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int>(0, 24, 24, false, 0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
    public int fieldAngle { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, true, 0.0001); } set { }  }
}
 /// Simnet: Parameter Handle
 /// Fast
 /// 130845
 public class simnetParameterHandle : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetParameterHandle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 6, false, 0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string>(6, 22, 2, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 24, 8, false, 0); } set { }  }
    public int group { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 48, 8, false, 0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int>(0, 56, 8, false, 0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int>(0, 64, 8, false, 0); } set { }  }
    public int j { get { return _packet.GetBitOffsetLength<int>(0, 72, 8, false, 0); } set { }  }
    public string backlight { get { return _packet.GetBitOffsetLength<string>(0, 80, 8, false, 0); } set { }  }
    public int l { get { return _packet.GetBitOffsetLength<int>(0, 88, 16, false, 0); } set { }  }
}
 /// Furuno: Motion Sensor Status Extended
 /// Fast
 /// 130846
 public class furunoMotionSensorStatusExtended : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public furunoMotionSensorStatusExtended(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
}
 /// SeaTalk: Node Statistics
 /// Fast
 /// 130847
 public class seatalkNodeStatistics : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public seatalkNodeStatistics(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int softwareRelease { get { return _packet.GetBitOffsetLength<int>(3, 11, 16, false, 0); } set { }  }
    public int developmentVersion { get { return _packet.GetBitOffsetLength<int>(3, 27, 8, false, 0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int>(3, 35, 16, false, 0); } set { }  }
    public int year { get { return _packet.GetBitOffsetLength<int>(3, 51, 8, false, 0); } set { }  }
    public int month { get { return _packet.GetBitOffsetLength<int>(3, 59, 8, false, 0); } set { }  }
    public int deviceNumber { get { return _packet.GetBitOffsetLength<int>(3, 67, 16, false, 0); } set { }  }
    public int nodeVoltage { get { return _packet.GetBitOffsetLength<int>(3, 83, 16, false, 0.01); } set { }  }
}
 /// Simnet: Event Command: AP command
 /// Fast
 /// 130850
 public class simnetEventCommandApCommand : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetEventCommandApCommand(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int controllingDevice { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string @event { get { return _packet.GetBitOffsetLength<string>(0, 48, 16, false, 0); } set { }  }
    public string direction { get { return _packet.GetBitOffsetLength<string>(0, 64, 8, false, 0); } set { }  }
    public int angle { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.0001); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
}
 /// Simnet: Event Command: Alarm?
 /// Fast
 /// 130850
 public class simnetEventCommandAlarm : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetEventCommandAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string alarm { get { return _packet.GetBitOffsetLength<string>(0, 48, 16, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 1); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int>(0, 80, 8, false, 0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
}
 /// Simnet: Event Command: Unknown
 /// Fast
 /// 130850
 public class simnetEventCommandUnknown : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetEventCommandUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 32, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 48, 16, false, 0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int>(0, 64, 16, false, 0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int>(0, 80, 16, false, 0); } set { }  }
}
 /// Simnet: Event Reply: AP command
 /// Fast
 /// 130851
 public class simnetEventReplyApCommand : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetEventReplyApCommand(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string>(0, 16, 8, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 24, 16, false, 0); } set { }  }
    public int controllingDevice { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string @event { get { return _packet.GetBitOffsetLength<string>(0, 48, 16, false, 0); } set { }  }
    public string direction { get { return _packet.GetBitOffsetLength<string>(0, 64, 8, false, 0); } set { }  }
    public int angle { get { return _packet.GetBitOffsetLength<int>(0, 72, 16, false, 0.0001); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int>(0, 88, 8, false, 0); } set { }  }
}
 /// Simnet: Alarm Message
 /// Fast
 /// 130856
 public class simnetAlarmMessage : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public simnetAlarmMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int>(0, 16, 16, false, 0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int>(0, 32, 8, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 40, 8, false, 0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string>(0, 48, 2040, false, 0); } set { }  }
}
 /// Airmar: Additional Weather Data
 /// Fast
 /// 130880
 public class airmarAdditionalWeatherData : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarAdditionalWeatherData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public double apparentWindchillTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
    public double trueWindchillTemperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.01); } set { }  }
    public double dewpoint { get { return _packet.GetBitOffsetLength<double>(0, 56, 16, false, 0.01); } set { }  }
}
 /// Airmar: Heater Control
 /// Fast
 /// 130881
 public class airmarHeaterControl : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarHeaterControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int>(0, 16, 8, false, 0); } set { }  }
    public double plateTemperature { get { return _packet.GetBitOffsetLength<double>(0, 24, 16, false, 0.01); } set { }  }
    public double airTemperature { get { return _packet.GetBitOffsetLength<double>(0, 40, 16, false, 0.01); } set { }  }
    public double dewpoint { get { return _packet.GetBitOffsetLength<double>(0, 56, 16, false, 0.01); } set { }  }
}
 /// Airmar: POST
 /// Fast
 /// 130944
 public class airmarPost : INMEA2000 { 
 byte[] _packet;
 public void SetPacketData(byte[] packet) { _packet = packet; }
 public airmarPost(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long>(0, 0, 11, false, 0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int>(3, 11, 2, false, 0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string>(5, 13, 3, false, 0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string>(0, 16, 4, false, 0); } set { }  }
    public int numberOfIdTestResultPairsToFollow { get { return _packet.GetBitOffsetLength<int>(3, 27, 8, false, 1); } set { }  }
    public string testId { get { return _packet.GetBitOffsetLength<string>(3, 35, 8, false, 0); } set { }  }
    public string testResult { get { return _packet.GetBitOffsetLength<string>(3, 43, 8, false, 0); } set { }  }
}

}

