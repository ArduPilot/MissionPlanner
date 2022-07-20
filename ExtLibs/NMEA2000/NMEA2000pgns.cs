using System;
using MissionPlanner.Utilities;
using System.Collections.Generic;

namespace NMEA2000
{
    public class NMEA2000msgs
    {
        public static readonly (int, Type, PgnType, int)[] msgs = {

                (59392, typeof(isoAcknowledgement), PgnType.Single, 8),
                (59904, typeof(isoRequest), PgnType.Single, 3),
                (60160, typeof(isoTransportProtocolDataTransfer), PgnType.Single, 8),
                (60416, typeof(isoTransportProtocolConnectionManagementRequestToSend), PgnType.Single, 8),
                (60416, typeof(isoTransportProtocolConnectionManagementClearToSend), PgnType.Single, 8),
                (60416, typeof(isoTransportProtocolConnectionManagementEndOfMessage), PgnType.Single, 8),
                (60416, typeof(isoTransportProtocolConnectionManagementBroadcastAnnounce), PgnType.Single, 8),
                (60416, typeof(isoTransportProtocolConnectionManagementAbort), PgnType.Single, 8),
                (60928, typeof(isoAddressClaim), PgnType.Single, 8),
                (61184, typeof(seatalkWirelessKeypadLightControl), PgnType.Single, 8),
                (61184, typeof(seatalkWirelessKeypadLightControl), PgnType.Single, 8),
                (61184, typeof(victronBatteryRegister), PgnType.Single, 8),
                (61184, typeof(manufacturerProprietarySingleFrameAddressed), PgnType.Single, 8),
                (61440, typeof(unknownSingleFrameNonAddressed), PgnType.Single, 8),
                (65001, typeof(bus1PhaseCBasicAcQuantities), PgnType.Single, 8),
                (65002, typeof(bus1PhaseBBasicAcQuantities), PgnType.Single, 8),
                (65003, typeof(bus1PhaseABasicAcQuantities), PgnType.Single, 8),
                (65004, typeof(bus1AverageBasicAcQuantities), PgnType.Single, 8),
                (65005, typeof(utilityTotalAcEnergy), PgnType.Single, 8),
                (65006, typeof(utilityPhaseCAcReactivePower), PgnType.Single, 8),
                (65007, typeof(utilityPhaseCAcPower), PgnType.Single, 8),
                (65008, typeof(utilityPhaseCBasicAcQuantities), PgnType.Single, 8),
                (65009, typeof(utilityPhaseBAcReactivePower), PgnType.Single, 8),
                (65010, typeof(utilityPhaseBAcPower), PgnType.Single, 8),
                (65011, typeof(utilityPhaseBBasicAcQuantities), PgnType.Single, 8),
                (65012, typeof(utilityPhaseAAcReactivePower), PgnType.Single, 8),
                (65013, typeof(utilityPhaseAAcPower), PgnType.Single, 8),
                (65014, typeof(utilityPhaseABasicAcQuantities), PgnType.Single, 8),
                (65015, typeof(utilityTotalAcReactivePower), PgnType.Single, 8),
                (65016, typeof(utilityTotalAcPower), PgnType.Single, 8),
                (65017, typeof(utilityAverageBasicAcQuantities), PgnType.Single, 8),
                (65018, typeof(generatorTotalAcEnergy), PgnType.Single, 8),
                (65019, typeof(generatorPhaseCAcReactivePower), PgnType.Single, 8),
                (65020, typeof(generatorPhaseCAcPower), PgnType.Single, 8),
                (65021, typeof(generatorPhaseCBasicAcQuantities), PgnType.Single, 8),
                (65022, typeof(generatorPhaseBAcReactivePower), PgnType.Single, 8),
                (65023, typeof(generatorPhaseBAcPower), PgnType.Single, 8),
                (65024, typeof(generatorPhaseBBasicAcQuantities), PgnType.Single, 8),
                (65025, typeof(generatorPhaseAAcReactivePower), PgnType.Single, 8),
                (65026, typeof(generatorPhaseAAcPower), PgnType.Single, 8),
                (65027, typeof(generatorPhaseABasicAcQuantities), PgnType.Single, 8),
                (65028, typeof(generatorTotalAcReactivePower), PgnType.Single, 8),
                (65029, typeof(generatorTotalAcPower), PgnType.Single, 8),
                (65030, typeof(generatorAverageBasicAcQuantities), PgnType.Single, 8),
                (65240, typeof(isoCommandedAddress), PgnType.Iso, 9),
                (65280, typeof(furunoHeave), PgnType.Single, 8),
                (65280, typeof(manufacturerProprietarySingleFrameNonAddressed), PgnType.Single, 8),
                (65284, typeof(maretronProprietaryDcBreakerCurrent), PgnType.Single, 8),
                (65285, typeof(airmarBootStateAcknowledgment), PgnType.Single, 8),
                (65285, typeof(lowranceTemperature), PgnType.Single, 8),
                (65286, typeof(chetcoDimmer), PgnType.Single, 8),
                (65286, typeof(airmarBootStateRequest), PgnType.Single, 8),
                (65287, typeof(airmarAccessLevel), PgnType.Single, 8),
                (65287, typeof(simnetConfigureTemperatureSensor), PgnType.Single, 8),
                (65288, typeof(seatalkAlarm), PgnType.Single, 8),
                (65289, typeof(simnetTrimTabSensorCalibration), PgnType.Single, 8),
                (65290, typeof(simnetPaddleWheelSpeedConfiguration), PgnType.Single, 8),
                (65292, typeof(simnetClearFluidLevelWarnings), PgnType.Single, 8),
                (65293, typeof(simnetLgc2000Configuration), PgnType.Single, 8),
                (65309, typeof(navicoWirelessBatteryStatus), PgnType.Single, 8),
                (65312, typeof(navicoWirelessSignalStatus), PgnType.Single, 8),
                (65325, typeof(simnetReprogramStatus), PgnType.Single, 8),
                (65341, typeof(simnetAutopilotMode), PgnType.Single, 8),
                (65345, typeof(seatalkPilotWindDatum), PgnType.Single, 8),
                (65359, typeof(seatalkPilotHeading), PgnType.Single, 8),
                (65360, typeof(seatalkPilotLockedHeading), PgnType.Single, 8),
                (65361, typeof(seatalkSilenceAlarm), PgnType.Single, 8),
                (65371, typeof(seatalkKeypadMessage), PgnType.Single, 8),
                (65374, typeof(seatalkKeypadHeartbeat), PgnType.Single, 8),
                (65379, typeof(seatalkPilotMode), PgnType.Single, 8),
                (65408, typeof(airmarDepthQualityFactor), PgnType.Single, 8),
                (65409, typeof(airmarSpeedPulseCount), PgnType.Single, 8),
                (65410, typeof(airmarDeviceInformation), PgnType.Single, 8),
                (65480, typeof(simnetAutopilotMode), PgnType.Single, 8),
                (65536, typeof(unknownFastPacketAddressed), PgnType.Fast, 255),
                (126208, typeof(nmeaRequestGroupFunction), PgnType.Fast, 12),
                (126208, typeof(nmeaCommandGroupFunction), PgnType.Fast, 8),
                (126208, typeof(nmeaAcknowledgeGroupFunction), PgnType.Fast, 8),
                (126208, typeof(nmeaReadFieldsGroupFunction), PgnType.Fast, 8),
                (126208, typeof(nmeaReadFieldsReplyGroupFunction), PgnType.Fast, 8),
                (126208, typeof(nmeaWriteFieldsGroupFunction), PgnType.Fast, 8),
                (126208, typeof(nmeaWriteFieldsReplyGroupFunction), PgnType.Fast, 8),
                (126464, typeof(pgnListTransmitAndReceive), PgnType.Fast, 8),
                (126720, typeof(seatalk1PilotMode), PgnType.Fast, 21),
                (126720, typeof(fusionMediaControl), PgnType.Fast, 6),
                (126720, typeof(fusionSiriusControl), PgnType.Fast, 7),
                (126720, typeof(fusionRequestStatus), PgnType.Fast, 3),
                (126720, typeof(fusionSetSource), PgnType.Fast, 3),
                (126720, typeof(fusionMute), PgnType.Fast, 3),
                (126720, typeof(fusionSetZoneVolume), PgnType.Fast, 6),
                (126720, typeof(fusionSetAllVolumes), PgnType.Fast, 9),
                (126720, typeof(seatalk1Keystroke), PgnType.Fast, 21),
                (126720, typeof(seatalk1DeviceIdentification), PgnType.Fast, 8),
                (126720, typeof(airmarAttitudeOffset), PgnType.Fast, 9),
                (126720, typeof(airmarCalibrateCompass), PgnType.Fast, 24),
                (126720, typeof(airmarTrueWindOptions), PgnType.Fast, 6),
                (126720, typeof(airmarSimulateMode), PgnType.Fast, 6),
                (126720, typeof(airmarCalibrateDepth), PgnType.Fast, 6),
                (126720, typeof(airmarCalibrateSpeed), PgnType.Fast, 12),
                (126720, typeof(airmarCalibrateTemperature), PgnType.Fast, 6),
                (126720, typeof(airmarSpeedFilter), PgnType.Fast, 8),
                (126720, typeof(airmarTemperatureFilter), PgnType.Fast, 8),
                (126720, typeof(airmarNmea2000Options), PgnType.Fast, 6),
                (126720, typeof(airmarAddressableMultiFrame), PgnType.Fast, 4),
                (126720, typeof(maretronSlaveResponse), PgnType.Fast, 8),
                (126720, typeof(manufacturerProprietaryFastPacketAddressed), PgnType.Fast, 223),
                (126976, typeof(unknownFastPacketNonAddressed), PgnType.Fast, 255),
                (126983, typeof(alert), PgnType.Fast, 28),
                (126984, typeof(alertResponse), PgnType.Fast, 25),
                (126985, typeof(alertText), PgnType.Fast, 49),
                (126986, typeof(alertConfiguration), PgnType.Fast, 8),
                (126987, typeof(alertThreshold), PgnType.Fast, 8),
                (126988, typeof(alertValue), PgnType.Fast, 8),
                (126992, typeof(systemTime), PgnType.Single, 8),
                (126993, typeof(heartbeat), PgnType.Single, 8),
                (126996, typeof(productInformation), PgnType.Fast, 134),
                (126998, typeof(configurationInformation), PgnType.Fast, 42),
                (127233, typeof(manOverboardNotification), PgnType.Fast, 35),
                (127237, typeof(headingTrackControl), PgnType.Fast, 21),
                (127245, typeof(rudder), PgnType.Single, 8),
                (127250, typeof(vesselHeading), PgnType.Single, 8),
                (127251, typeof(rateOfTurn), PgnType.Single, 5),
                (127252, typeof(heave), PgnType.Single, 8),
                (127257, typeof(attitude), PgnType.Single, 7),
                (127258, typeof(magneticVariation), PgnType.Single, 6),
                (127488, typeof(engineParametersRapidUpdate), PgnType.Single, 8),
                (127489, typeof(engineParametersDynamic), PgnType.Fast, 26),
                (127493, typeof(transmissionParametersDynamic), PgnType.Single, 8),
                (127496, typeof(tripParametersVessel), PgnType.Fast, 10),
                (127497, typeof(tripParametersEngine), PgnType.Fast, 9),
                (127498, typeof(engineParametersStatic), PgnType.Fast, 52),
                (127500, typeof(loadControllerConnectionStateControl), PgnType.Fast, 10),
                (127501, typeof(binarySwitchBankStatus), PgnType.Single, 8),
                (127502, typeof(switchBankControl), PgnType.Single, 8),
                (127503, typeof(acInputStatus), PgnType.Fast, 56),
                (127504, typeof(acOutputStatus), PgnType.Single, 56),
                (127505, typeof(fluidLevel), PgnType.Single, 8),
                (127506, typeof(dcDetailedStatus), PgnType.Fast, 11),
                (127507, typeof(chargerStatus), PgnType.Fast, 6),
                (127508, typeof(batteryStatus), PgnType.Single, 8),
                (127509, typeof(inverterStatus), PgnType.Single, 4),
                (127510, typeof(chargerConfigurationStatus), PgnType.Fast, 13),
                (127511, typeof(inverterConfigurationStatus), PgnType.Single, 8),
                (127512, typeof(agsConfigurationStatus), PgnType.Single, 8),
                (127513, typeof(batteryConfigurationStatus), PgnType.Fast, 10),
                (127514, typeof(agsStatus), PgnType.Single, 8),
                (127744, typeof(acPowerCurrentPhaseA), PgnType.Single, 8),
                (127745, typeof(acPowerCurrentPhaseB), PgnType.Single, 8),
                (127746, typeof(acPowerCurrentPhaseC), PgnType.Single, 8),
                (127750, typeof(converterStatus), PgnType.Single, 8),
                (127751, typeof(dcVoltageCurrent), PgnType.Single, 8),
                (128000, typeof(leewayAngle), PgnType.Single, 8),
                (128006, typeof(thrusterControlStatus), PgnType.Single, 8),
                (128007, typeof(thrusterInformation), PgnType.Single, 8),
                (128008, typeof(thrusterMotorStatus), PgnType.Single, 8),
                (128259, typeof(speed), PgnType.Single, 8),
                (128267, typeof(waterDepth), PgnType.Single, 8),
                (128275, typeof(distanceLog), PgnType.Fast, 14),
                (128520, typeof(trackedTargetData), PgnType.Fast, 27),
                (128776, typeof(windlassControlStatus), PgnType.Single, 7),
                (128777, typeof(anchorWindlassOperatingStatus), PgnType.Single, 8),
                (128778, typeof(anchorWindlassMonitoringStatus), PgnType.Single, 8),
                (129025, typeof(positionRapidUpdate), PgnType.Single, 8),
                (129026, typeof(cogSogRapidUpdate), PgnType.Single, 8),
                (129027, typeof(positionDeltaRapidUpdate), PgnType.Single, 8),
                (129028, typeof(altitudeDeltaRapidUpdate), PgnType.Single, 8),
                (129029, typeof(gnssPositionData), PgnType.Fast, 43),
                (129033, typeof(timeDate), PgnType.Single, 8),
                (129038, typeof(aisClassAPositionReport), PgnType.Fast, 28),
                (129039, typeof(aisClassBPositionReport), PgnType.Fast, 26),
                (129040, typeof(aisClassBExtendedPositionReport), PgnType.Fast, 33),
                (129041, typeof(aisAidsToNavigationAtonReport), PgnType.Fast, 60),
                (129044, typeof(datum), PgnType.Fast, 20),
                (129045, typeof(userDatum), PgnType.Fast, 37),
                (129283, typeof(crossTrackError), PgnType.Single, 8),
                (129284, typeof(navigationData), PgnType.Fast, 34),
                (129285, typeof(navigationRouteWpInformation), PgnType.Fast, 233),
                (129291, typeof(setDriftRapidUpdate), PgnType.Single, 8),
                (129301, typeof(navigationRouteTimeToFromMark), PgnType.Fast, 10),
                (129302, typeof(bearingAndDistanceBetweenTwoMarks), PgnType.Fast, 8),
                (129538, typeof(gnssControlStatus), PgnType.Fast, 13),
                (129539, typeof(gnssDops), PgnType.Single, 8),
                (129540, typeof(gnssSatsInView), PgnType.Fast, 233),
                (129541, typeof(gpsAlmanacData), PgnType.Fast, 26),
                (129542, typeof(gnssPseudorangeNoiseStatistics), PgnType.Fast, 9),
                (129545, typeof(gnssRaimOutput), PgnType.Fast, 9),
                (129546, typeof(gnssRaimSettings), PgnType.Single, 8),
                (129547, typeof(gnssPseudorangeErrorStatistics), PgnType.Fast, 9),
                (129549, typeof(dgnssCorrections), PgnType.Fast, 13),
                (129550, typeof(gnssDifferentialCorrectionReceiverInterface), PgnType.Fast, 8),
                (129551, typeof(gnssDifferentialCorrectionReceiverSignal), PgnType.Fast, 8),
                (129556, typeof(glonassAlmanacData), PgnType.Fast, 8),
                (129792, typeof(aisDgnssBroadcastBinaryMessage), PgnType.Fast, 8),
                (129793, typeof(aisUtcAndDateReport), PgnType.Fast, 26),
                (129794, typeof(aisClassAStaticAndVoyageRelatedData), PgnType.Fast, 24),
                (129795, typeof(aisAddressedBinaryMessage), PgnType.Fast, 13),
                (129796, typeof(aisAcknowledge), PgnType.Fast, 12),
                (129797, typeof(aisBinaryBroadcastMessage), PgnType.Fast, 233),
                (129798, typeof(aisSarAircraftPositionReport), PgnType.Fast, 8),
                (129799, typeof(radioFrequencyModePower), PgnType.Fast, 9),
                (129800, typeof(aisUtcDateInquiry), PgnType.Fast, 8),
                (129801, typeof(aisAddressedSafetyRelatedMessage), PgnType.Fast, 12),
                (129802, typeof(aisSafetyRelatedBroadcastMessage), PgnType.Fast, 8),
                (129803, typeof(aisInterrogation), PgnType.Single, 8),
                (129804, typeof(aisAssignmentModeCommand), PgnType.Fast, 23),
                (129805, typeof(aisDataLinkManagementMessage), PgnType.Fast, 8),
                (129806, typeof(aisChannelManagement), PgnType.Fast, 8),
                (129807, typeof(aisClassBGroupAssignment), PgnType.Fast, 8),
                (129808, typeof(dscDistressCallInformation), PgnType.Fast, 8),
                (129808, typeof(dscCallInformation), PgnType.Fast, 8),
                (129809, typeof(aisClassBStaticDataMsg24PartA), PgnType.Fast, 27),
                (129810, typeof(aisClassBStaticDataMsg24PartB), PgnType.Fast, 34),
                (130060, typeof(label), PgnType.Fast, 0),
                (130061, typeof(channelSourceConfiguration), PgnType.Fast, 0),
                (130064, typeof(routeAndWpServiceDatabaseList), PgnType.Fast, 8),
                (130065, typeof(routeAndWpServiceRouteList), PgnType.Fast, 8),
                (130066, typeof(routeAndWpServiceRouteWpListAttributes), PgnType.Fast, 8),
                (130067, typeof(routeAndWpServiceRouteWpNamePosition), PgnType.Fast, 8),
                (130068, typeof(routeAndWpServiceRouteWpName), PgnType.Fast, 8),
                (130069, typeof(routeAndWpServiceXteLimitNavigationMethod), PgnType.Fast, 8),
                (130070, typeof(routeAndWpServiceWpComment), PgnType.Fast, 8),
                (130071, typeof(routeAndWpServiceRouteComment), PgnType.Fast, 8),
                (130072, typeof(routeAndWpServiceDatabaseComment), PgnType.Fast, 8),
                (130073, typeof(routeAndWpServiceRadiusOfTurn), PgnType.Fast, 8),
                (130074, typeof(routeAndWpServiceWpListWpNamePosition), PgnType.Fast, 8),
                (130306, typeof(windData), PgnType.Single, 8),
                (130310, typeof(environmentalParameters), PgnType.Single, 8),
                (130311, typeof(environmentalParameters), PgnType.Single, 8),
                (130312, typeof(temperature), PgnType.Single, 8),
                (130313, typeof(humidity), PgnType.Single, 8),
                (130314, typeof(actualPressure), PgnType.Single, 8),
                (130315, typeof(setPressure), PgnType.Single, 8),
                (130316, typeof(temperatureExtendedRange), PgnType.Single, 8),
                (130320, typeof(tideStationData), PgnType.Fast, 20),
                (130321, typeof(salinityStationData), PgnType.Fast, 22),
                (130322, typeof(currentStationData), PgnType.Fast, 8),
                (130323, typeof(meteorologicalStationData), PgnType.Fast, 30),
                (130324, typeof(mooredBuoyStationData), PgnType.Fast, 8),
                (130560, typeof(payloadMass), PgnType.Fast, 0),
                (130567, typeof(watermakerInputSettingAndStatus), PgnType.Fast, 24),
                (130569, typeof(currentStatusAndFile), PgnType.Fast, 233),
                (130570, typeof(libraryDataFile), PgnType.Fast, 233),
                (130571, typeof(libraryDataGroup), PgnType.Fast, 233),
                (130572, typeof(libraryDataSearch), PgnType.Fast, 233),
                (130573, typeof(supportedSourceData), PgnType.Fast, 233),
                (130574, typeof(supportedZoneData), PgnType.Fast, 233),
                (130576, typeof(smallCraftStatus), PgnType.Single, 2),
                (130577, typeof(directionData), PgnType.Fast, 14),
                (130578, typeof(vesselSpeedComponents), PgnType.Fast, 12),
                (130579, typeof(systemConfiguration), PgnType.Fast, 8),
                (130580, typeof(systemConfigurationDeprecated), PgnType.Fast, 2),
                (130581, typeof(zoneConfigurationDeprecated), PgnType.Fast, 14),
                (130582, typeof(zoneVolume), PgnType.Fast, 4),
                (130583, typeof(availableAudioEqPresets), PgnType.Fast, 233),
                (130584, typeof(availableBluetoothAddresses), PgnType.Fast, 233),
                (130585, typeof(bluetoothSourceStatus), PgnType.Fast, 233),
                (130586, typeof(zoneConfiguration), PgnType.Fast, 14),
                (130816, typeof(sonichubInit2), PgnType.Fast, 9),
                (130816, typeof(sonichubAmRadio), PgnType.Fast, 64),
                (130816, typeof(sonichubZoneInfo), PgnType.Fast, 6),
                (130816, typeof(sonichubSource), PgnType.Fast, 64),
                (130816, typeof(sonichubSourceList), PgnType.Fast, 64),
                (130816, typeof(sonichubControl), PgnType.Fast, 64),
                (130816, typeof(sonichubUnknown), PgnType.Fast, 64),
                (130816, typeof(sonichubFmRadio), PgnType.Fast, 64),
                (130816, typeof(sonichubPlaylist), PgnType.Fast, 64),
                (130816, typeof(sonichubTrack), PgnType.Fast, 64),
                (130816, typeof(sonichubArtist), PgnType.Fast, 64),
                (130816, typeof(sonichubAlbum), PgnType.Fast, 64),
                (130816, typeof(sonichubMenuItem), PgnType.Fast, 64),
                (130816, typeof(sonichubZones), PgnType.Fast, 64),
                (130816, typeof(sonichubMaxVolume), PgnType.Fast, 64),
                (130816, typeof(sonichubVolume), PgnType.Fast, 8),
                (130816, typeof(sonichubInit1), PgnType.Fast, 64),
                (130816, typeof(sonichubPosition), PgnType.Fast, 64),
                (130816, typeof(sonichubInit3), PgnType.Fast, 9),
                (130816, typeof(simradTextMessage), PgnType.Fast, 64),
                (130816, typeof(manufacturerProprietaryFastPacketNonAddressed), PgnType.Fast, 223),
                (130817, typeof(navicoProductInformation), PgnType.Fast, 14),
                (130818, typeof(simnetReprogramData), PgnType.Fast, 223),
                (130819, typeof(simnetRequestReprogram), PgnType.Fast, 8),
                (130820, typeof(simnetReprogramStatus), PgnType.Fast, 8),
                (130820, typeof(furunoUnknown), PgnType.Fast, 8),
                (130820, typeof(fusionSourceName), PgnType.Fast, 13),
                (130820, typeof(fusionTrackInfo), PgnType.Fast, 23),
                (130820, typeof(fusionTrack), PgnType.Fast, 32),
                (130820, typeof(fusionArtist), PgnType.Fast, 32),
                (130820, typeof(fusionAlbum), PgnType.Fast, 32),
                (130820, typeof(fusionUnitName), PgnType.Fast, 32),
                (130820, typeof(fusionZoneName), PgnType.Fast, 32),
                (130820, typeof(fusionPlayProgress), PgnType.Fast, 9),
                (130820, typeof(fusionAmFmStation), PgnType.Fast, 10),
                (130820, typeof(fusionVhf), PgnType.Fast, 9),
                (130820, typeof(fusionSquelch), PgnType.Fast, 6),
                (130820, typeof(fusionScan), PgnType.Fast, 6),
                (130820, typeof(fusionMenuItem), PgnType.Fast, 23),
                (130820, typeof(fusionReplay), PgnType.Fast, 23),
                (130820, typeof(fusionMute), PgnType.Fast, 5),
                (130820, typeof(fusionSubVolume), PgnType.Fast, 8),
                (130820, typeof(fusionTone), PgnType.Fast, 8),
                (130820, typeof(fusionVolume), PgnType.Fast, 10),
                (130820, typeof(fusionPowerState), PgnType.Fast, 5),
                (130820, typeof(fusionSiriusxmChannel), PgnType.Fast, 32),
                (130820, typeof(fusionSiriusxmTitle), PgnType.Fast, 32),
                (130820, typeof(fusionSiriusxmArtist), PgnType.Fast, 32),
                (130820, typeof(fusionSiriusxmGenre), PgnType.Fast, 32),
                (130821, typeof(furunoUnknown), PgnType.Fast, 12),
                (130823, typeof(maretronProprietaryTemperatureHighRange), PgnType.Fast, 9),
                (130824, typeof(bGWindData), PgnType.Single, 8),
                (130824, typeof(maretronAnnunciator), PgnType.Fast, 9),
                (130827, typeof(lowranceUnknown), PgnType.Fast, 10),
                (130828, typeof(simnetSetSerialNumber), PgnType.Fast, 8),
                (130831, typeof(suzukiEngineAndStorageDeviceConfig), PgnType.Fast, 8),
                (130832, typeof(simnetFuelUsedHighResolution), PgnType.Fast, 8),
                (130834, typeof(simnetEngineAndTankConfiguration), PgnType.Fast, 8),
                (130835, typeof(simnetSetEngineAndTankConfiguration), PgnType.Fast, 8),
                (130836, typeof(simnetFluidLevelSensorConfiguration), PgnType.Fast, 14),
                (130836, typeof(maretronProprietarySwitchStatusCounter), PgnType.Fast, 16),
                (130837, typeof(simnetFuelFlowTurbineConfiguration), PgnType.Fast, 8),
                (130837, typeof(maretronProprietarySwitchStatusTimer), PgnType.Fast, 23),
                (130838, typeof(simnetFluidLevelWarning), PgnType.Fast, 8),
                (130839, typeof(simnetPressureSensorConfiguration), PgnType.Fast, 8),
                (130840, typeof(simnetDataUserGroupConfiguration), PgnType.Fast, 8),
                (130842, typeof(simnetAisClassBStaticDataMsg24PartA), PgnType.Fast, 29),
                (130842, typeof(furunoSixDegreesOfFreedomMovement), PgnType.Fast, 29),
                (130842, typeof(simnetAisClassBStaticDataMsg24PartB), PgnType.Fast, 37),
                (130843, typeof(furunoHeelAngleRollInformation), PgnType.Fast, 8),
                (130843, typeof(simnetSonarStatusFrequencyAndDspVoltage), PgnType.Fast, 8),
                (130845, typeof(simnetCompassHeadingOffset), PgnType.Fast, 14),
                (130845, typeof(furunoMultiSatsInViewExtended), PgnType.Fast, 8),
                (130845, typeof(simnetCompassLocalField), PgnType.Fast, 14),
                (130845, typeof(simnetCompassFieldAngle), PgnType.Fast, 14),
                (130845, typeof(simnetParameterHandle), PgnType.Fast, 14),
                (130846, typeof(furunoMotionSensorStatusExtended), PgnType.Fast, 8),
                (130847, typeof(seatalkNodeStatistics), PgnType.Fast, 0),
                (130850, typeof(simnetEventCommandApCommand), PgnType.Fast, 12),
                (130850, typeof(simnetEventCommandAlarm), PgnType.Fast, 12),
                (130850, typeof(simnetEventCommandUnknown), PgnType.Fast, 12),
                (130851, typeof(simnetEventReplyApCommand), PgnType.Fast, 12),
                (130856, typeof(simnetAlarmMessage), PgnType.Fast, 8),
                (130880, typeof(airmarAdditionalWeatherData), PgnType.Fast, 30),
                (130881, typeof(airmarHeaterControl), PgnType.Fast, 9),
                (130944, typeof(airmarPost), PgnType.Fast, 8),
};
    }



/// Description: ISO Acknowledgement
/// Type: Single
/// PGNNO: 59392
/// Length: 8
public class isoAcknowledgement : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoAcknowledgement(byte[] packet) { _packet = packet; }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int groupFunction { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 24, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  1); } set { }  }
}
/// Description: ISO Request
/// Type: Single
/// PGNNO: 59904
/// Length: 3
public class isoRequest : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoRequest(byte[] packet) { _packet = packet; }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 24, false,  1); } set { }  }
}
/// Description: ISO Transport Protocol, Data Transfer
/// Type: Single
/// PGNNO: 60160
/// Length: 8
public class isoTransportProtocolDataTransfer : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolDataTransfer(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public long data { get { return _packet.GetBitOffsetLength<int, long>(0, 8, 56, false,  0); } set { }  }
}
/// Description: ISO Transport Protocol, Connection Management - Request To Send
/// Type: Single
/// PGNNO: 60416
/// Length: 8
public class isoTransportProtocolConnectionManagementRequestToSend : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolConnectionManagementRequestToSend(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int messageSize { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int packets { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int packetsReply { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  1); } set { }  }
}
/// Description: ISO Transport Protocol, Connection Management - Clear To Send
/// Type: Single
/// PGNNO: 60416
/// Length: 8
public class isoTransportProtocolConnectionManagementClearToSend : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolConnectionManagementClearToSend(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int maxPackets { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int nextSid { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 24, 16, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  1); } set { }  }
}
/// Description: ISO Transport Protocol, Connection Management - End Of Message
/// Type: Single
/// PGNNO: 60416
/// Length: 8
public class isoTransportProtocolConnectionManagementEndOfMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolConnectionManagementEndOfMessage(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int totalMessageSize { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int totalNumberOfPacketsReceived { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 32, 8, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  1); } set { }  }
}
/// Description: ISO Transport Protocol, Connection Management - Broadcast Announce
/// Type: Single
/// PGNNO: 60416
/// Length: 8
public class isoTransportProtocolConnectionManagementBroadcastAnnounce : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolConnectionManagementBroadcastAnnounce(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int messageSize { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int packets { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 32, 8, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  1); } set { }  }
}
/// Description: ISO Transport Protocol, Connection Management - Abort
/// Type: Single
/// PGNNO: 60416
/// Length: 8
public class isoTransportProtocolConnectionManagementAbort : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoTransportProtocolConnectionManagementAbort(byte[] packet) { _packet = packet; }
    public int groupFunctionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public byte[] reason { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 8, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 16, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 24, false,  1); } set { }  }
}
/// Description: ISO Address Claim
/// Type: Single
/// PGNNO: 60928
/// Length: 8
public class isoAddressClaim : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoAddressClaim(byte[] packet) { _packet = packet; }
    public byte[] uniqueNumber { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 21, false,  0); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(5, 21, 11, false,  0); } set { }  }
    public int deviceInstanceLower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 3, false,  0); } set { }  }
    public int deviceInstanceUpper { get { return _packet.GetBitOffsetLength<int, int>(3, 35, 5, false,  0); } set { }  }
    public int deviceFunction { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 48, 1, false,  0); } set { }  }
    public string deviceClass { get { return _packet.GetBitOffsetLength<string, string>(1, 49, 7, false,  0); } set { }  }
    public int systemInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 4, false,  0); } set { }  }
    public string industryGroup { get { return _packet.GetBitOffsetLength<string, string>(4, 60, 3, false,  0); } set { }  }
}
/// Description: Seatalk: Wireless Keypad Light Control
/// Type: Single
/// PGNNO: 61184
/// Length: 8
public class seatalkWirelessKeypadLightControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkWirelessKeypadLightControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int variant { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int wirelessSetting { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int wiredSetting { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
}
/// Description: Victron Battery Register
/// Type: Single
/// PGNNO: 61184
/// Length: 8
public class victronBatteryRegister : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public victronBatteryRegister(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int registerId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int payload { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Manufacturer Proprietary single-frame addressed
/// Type: Single
/// PGNNO: 61184
/// Length: 8
public class manufacturerProprietarySingleFrameAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public manufacturerProprietarySingleFrameAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 48, false,  0); } set { }  }
}
/// Description: Unknown single-frame non-addressed
/// Type: Single
/// PGNNO: 61440
/// Length: 8
public class unknownSingleFrameNonAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public unknownSingleFrameNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 48, false,  0); } set { }  }
}
/// Description: Bus #1 Phase C Basic AC Quantities
/// Type: Single
/// PGNNO: 65001
/// Length: 8
public class bus1PhaseCBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bus1PhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
}
/// Description: Bus #1 Phase B Basic AC Quantities
/// Type: Single
/// PGNNO: 65002
/// Length: 8
public class bus1PhaseBBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bus1PhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
}
/// Description: Bus #1 Phase A Basic AC Quantities
/// Type: Single
/// PGNNO: 65003
/// Length: 8
public class bus1PhaseABasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bus1PhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
}
/// Description: Bus #1 Average Basic AC Quantities
/// Type: Single
/// PGNNO: 65004
/// Length: 8
public class bus1AverageBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bus1AverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
}
/// Description: Utility Total AC Energy
/// Type: Single
/// PGNNO: 65005
/// Length: 8
public class utilityTotalAcEnergy : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityTotalAcEnergy(byte[] packet) { _packet = packet; }
    public int totalEnergyExport { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0); } set { }  }
    public int totalEnergyImport { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Utility Phase C AC Reactive Power
/// Type: Single
/// PGNNO: 65006
/// Length: 8
public class utilityPhaseCAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseCAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Utility Phase C AC Power
/// Type: Single
/// PGNNO: 65007
/// Length: 8
public class utilityPhaseCAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseCAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: Utility Phase C Basic AC Quantities
/// Type: Single
/// PGNNO: 65008
/// Length: 8
public class utilityPhaseCBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Utility Phase B AC Reactive Power
/// Type: Single
/// PGNNO: 65009
/// Length: 8
public class utilityPhaseBAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseBAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Utility Phase B AC Power
/// Type: Single
/// PGNNO: 65010
/// Length: 8
public class utilityPhaseBAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseBAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: Utility Phase B Basic AC Quantities
/// Type: Single
/// PGNNO: 65011
/// Length: 8
public class utilityPhaseBBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Utility Phase A AC Reactive Power
/// Type: Single
/// PGNNO: 65012
/// Length: 8
public class utilityPhaseAAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseAAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, true,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 2, false,  0); } set { }  }
}
/// Description: Utility Phase A AC Power
/// Type: Single
/// PGNNO: 65013
/// Length: 8
public class utilityPhaseAAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseAAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: Utility Phase A Basic AC Quantities
/// Type: Single
/// PGNNO: 65014
/// Length: 8
public class utilityPhaseABasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityPhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Utility Total AC Reactive Power
/// Type: Single
/// PGNNO: 65015
/// Length: 8
public class utilityTotalAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityTotalAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 2, false,  0); } set { }  }
}
/// Description: Utility Total AC Power
/// Type: Single
/// PGNNO: 65016
/// Length: 8
public class utilityTotalAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityTotalAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, true,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: Utility Average Basic AC Quantities
/// Type: Single
/// PGNNO: 65017
/// Length: 8
public class utilityAverageBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public utilityAverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Generator Total AC Energy
/// Type: Single
/// PGNNO: 65018
/// Length: 8
public class generatorTotalAcEnergy : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorTotalAcEnergy(byte[] packet) { _packet = packet; }
    public int totalEnergyExport { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0); } set { }  }
    public int totalEnergyImport { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Generator Phase C AC Reactive Power
/// Type: Single
/// PGNNO: 65019
/// Length: 8
public class generatorPhaseCAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseCAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Generator Phase C AC Power
/// Type: Single
/// PGNNO: 65020
/// Length: 8
public class generatorPhaseCAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseCAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
}
/// Description: Generator Phase C Basic AC Quantities
/// Type: Single
/// PGNNO: 65021
/// Length: 8
public class generatorPhaseCBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseCBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Generator Phase B AC Reactive Power
/// Type: Single
/// PGNNO: 65022
/// Length: 8
public class generatorPhaseBAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseBAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Generator Phase B AC Power
/// Type: Single
/// PGNNO: 65023
/// Length: 8
public class generatorPhaseBAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseBAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
}
/// Description: Generator Phase B Basic AC Quantities
/// Type: Single
/// PGNNO: 65024
/// Length: 8
public class generatorPhaseBBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseBBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Generator Phase A AC Reactive Power
/// Type: Single
/// PGNNO: 65025
/// Length: 8
public class generatorPhaseAAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseAAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Generator Phase A AC Power
/// Type: Single
/// PGNNO: 65026
/// Length: 8
public class generatorPhaseAAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseAAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Generator Phase A Basic AC Quantities
/// Type: Single
/// PGNNO: 65027
/// Length: 8
public class generatorPhaseABasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorPhaseABasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Generator Total AC Reactive Power
/// Type: Single
/// PGNNO: 65028
/// Length: 8
public class generatorTotalAcReactivePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorTotalAcReactivePower(byte[] packet) { _packet = packet; }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int powerFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string powerFactorLagging { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
}
/// Description: Generator Total AC Power
/// Type: Single
/// PGNNO: 65029
/// Length: 8
public class generatorTotalAcPower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorTotalAcPower(byte[] packet) { _packet = packet; }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0); } set { }  }
    public int apparentPower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Generator Average Basic AC Quantities
/// Type: Single
/// PGNNO: 65030
/// Length: 8
public class generatorAverageBasicAcQuantities : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public generatorAverageBasicAcQuantities(byte[] packet) { _packet = packet; }
    public int lineLineAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int lineNeutralAcRmsVoltage { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double acFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0078125); } set { }  }
    public int acRmsCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: ISO Commanded Address
/// Type: Iso
/// PGNNO: 65240
/// Length: 9
public class isoCommandedAddress : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public isoCommandedAddress(byte[] packet) { _packet = packet; }
    public byte[] uniqueNumber { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 21, false,  0); } set { }  }
    public int manufacturerCode { get { return _packet.GetBitOffsetLength<int, int>(5, 21, 11, false,  0); } set { }  }
    public int deviceInstanceLower { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 3, false,  0); } set { }  }
    public int deviceInstanceUpper { get { return _packet.GetBitOffsetLength<int, int>(3, 35, 5, false,  0); } set { }  }
    public int deviceFunction { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 48, 1, false,  0); } set { }  }
    public string deviceClass { get { return _packet.GetBitOffsetLength<string, string>(1, 49, 7, false,  0); } set { }  }
    public int systemInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 4, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(4, 60, 3, false,  0); } set { }  }
    public int newSourceAddress { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
}
/// Description: Furuno: Heave
/// Type: Single
/// PGNNO: 65280
/// Length: 8
public class furunoHeave : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoHeave(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public double heave { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 32, true,  0.001); } set { }  }
}
/// Description: Manufacturer Proprietary single-frame non-addressed
/// Type: Single
/// PGNNO: 65280
/// Length: 8
public class manufacturerProprietarySingleFrameNonAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public manufacturerProprietarySingleFrameNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 48, false,  0); } set { }  }
}
/// Description: Maretron: Proprietary DC Breaker Current
/// Type: Single
/// PGNNO: 65284
/// Length: 8
public class maretronProprietaryDcBreakerCurrent : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronProprietaryDcBreakerCurrent(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int bankInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public double breakerCurrent { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.1); } set { }  }
}
/// Description: Airmar: Boot State Acknowledgment
/// Type: Single
/// PGNNO: 65285
/// Length: 8
public class airmarBootStateAcknowledgment : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarBootStateAcknowledgment(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string bootState { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 4, false,  0); } set { }  }
}
/// Description: Lowrance: Temperature
/// Type: Single
/// PGNNO: 65285
/// Length: 8
public class lowranceTemperature : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public lowranceTemperature(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string temperatureSource { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
}
/// Description: Chetco: Dimmer
/// Type: Single
/// PGNNO: 65286
/// Length: 8
public class chetcoDimmer : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public chetcoDimmer(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public int industryCode { get { return _packet.GetBitOffsetLength<int, int>(5, 13, 3, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int dimmer1 { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int dimmer2 { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int dimmer3 { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int dimmer4 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int control { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Airmar: Boot State Request
/// Type: Single
/// PGNNO: 65286
/// Length: 8
public class airmarBootStateRequest : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarBootStateRequest(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Airmar: Access Level
/// Type: Single
/// PGNNO: 65287
/// Length: 8
public class airmarAccessLevel : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarAccessLevel(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string formatCode { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 3, false,  0); } set { }  }
    public string accessLevel { get { return _packet.GetBitOffsetLength<string, string>(3, 19, 3, false,  0); } set { }  }
    public int accessSeedKey { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  1); } set { }  }
}
/// Description: Simnet: Configure Temperature Sensor
/// Type: Single
/// PGNNO: 65287
/// Length: 8
public class simnetConfigureTemperatureSensor : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetConfigureTemperatureSensor(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Seatalk: Alarm
/// Type: Single
/// PGNNO: 65288
/// Length: 8
public class seatalkAlarm : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 8, false,  0); } set { }  }
    public string alarmStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string alarmId { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string alarmGroup { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public byte[] alarmPriority { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Simnet: Trim Tab Sensor Calibration
/// Type: Single
/// PGNNO: 65289
/// Length: 8
public class simnetTrimTabSensorCalibration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetTrimTabSensorCalibration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Paddle Wheel Speed Configuration
/// Type: Single
/// PGNNO: 65290
/// Length: 8
public class simnetPaddleWheelSpeedConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetPaddleWheelSpeedConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Clear Fluid Level Warnings
/// Type: Single
/// PGNNO: 65292
/// Length: 8
public class simnetClearFluidLevelWarnings : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetClearFluidLevelWarnings(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: LGC-2000 Configuration
/// Type: Single
/// PGNNO: 65293
/// Length: 8
public class simnetLgc2000Configuration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetLgc2000Configuration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Navico: Wireless Battery Status
/// Type: Single
/// PGNNO: 65309
/// Length: 8
public class navicoWirelessBatteryStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navicoWirelessBatteryStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int batteryStatus { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int batteryChargeStatus { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
}
/// Description: Navico: Wireless Signal Status
/// Type: Single
/// PGNNO: 65312
/// Length: 8
public class navicoWirelessSignalStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navicoWirelessSignalStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
}
/// Description: Simnet: Reprogram Status
/// Type: Single
/// PGNNO: 65325
/// Length: 8
public class simnetReprogramStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetReprogramStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Autopilot Mode
/// Type: Single
/// PGNNO: 65341
/// Length: 8
public class simnetAutopilotMode : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetAutopilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Seatalk: Pilot Wind Datum
/// Type: Single
/// PGNNO: 65345
/// Length: 8
public class seatalkPilotWindDatum : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkPilotWindDatum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public double windDatum { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.0001); } set { }  }
    public double rollingAverageWindAngle { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0001); } set { }  }
}
/// Description: Seatalk: Pilot Heading
/// Type: Single
/// PGNNO: 65359
/// Length: 8
public class seatalkPilotHeading : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkPilotHeading(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 8, false,  0); } set { }  }
    public double headingTrue { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.0001); } set { }  }
    public double headingMagnetic { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.0001); } set { }  }
}
/// Description: Seatalk: Pilot Locked Heading
/// Type: Single
/// PGNNO: 65360
/// Length: 8
public class seatalkPilotLockedHeading : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkPilotLockedHeading(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 8, false,  0); } set { }  }
    public double targetHeadingTrue { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.0001); } set { }  }
    public double targetHeadingMagnetic { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.0001); } set { }  }
}
/// Description: Seatalk: Silence Alarm
/// Type: Single
/// PGNNO: 65361
/// Length: 8
public class seatalkSilenceAlarm : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkSilenceAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string alarmId { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public string alarmGroup { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
}
/// Description: Seatalk: Keypad Message
/// Type: Single
/// PGNNO: 65371
/// Length: 8
public class seatalkKeypadMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkKeypadMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int firstKey { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int secondKey { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int firstKeyState { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 2, false,  0); } set { }  }
    public int secondKeyState { get { return _packet.GetBitOffsetLength<int, int>(2, 42, 2, false,  0); } set { }  }
    public int encoderPosition { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
}
/// Description: SeaTalk: Keypad Heartbeat
/// Type: Single
/// PGNNO: 65374
/// Length: 8
public class seatalkKeypadHeartbeat : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkKeypadHeartbeat(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int variant { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
}
/// Description: Seatalk: Pilot Mode
/// Type: Single
/// PGNNO: 65379
/// Length: 8
public class seatalkPilotMode : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkPilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] pilotMode { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 8, false,  0); } set { }  }
    public byte[] subMode { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 24, 8, false,  0); } set { }  }
    public byte[] pilotModeData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 32, 8, false,  0); } set { }  }
}
/// Description: Airmar: Depth Quality Factor
/// Type: Single
/// PGNNO: 65408
/// Length: 8
public class airmarDepthQualityFactor : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarDepthQualityFactor(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public string depthQualityFactor { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 4, false,  0); } set { }  }
}
/// Description: Airmar: Speed Pulse Count
/// Type: Single
/// PGNNO: 65409
/// Length: 8
public class airmarSpeedPulseCount : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarSpeedPulseCount(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public double durationOfInterval { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.001); } set { }  }
    public int numberOfPulsesReceived { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 16, false,  0); } set { }  }
}
/// Description: Airmar: Device Information
/// Type: Single
/// PGNNO: 65410
/// Length: 8
public class airmarDeviceInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarDeviceInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public double internalDeviceTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double supplyVoltage { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
}
/// Description: Unknown fast-packet addressed
/// Type: Fast
/// PGNNO: 65536
/// Length: 255
public class unknownFastPacketAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public unknownFastPacketAddressed(byte[] packet) { _packet = packet; }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 2040, false,  0); } set { }  }
}
/// Description: NMEA - Request group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 12
public class nmeaRequestGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaRequestGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public double transmissionInterval { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, false,  0.001); } set { }  }
    public double transmissionIntervalOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, false,  0.01); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 0, false,  0); } set { }  }
}
/// Description: NMEA - Command group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaCommandGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaCommandGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public string priority { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 36, 4, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 0, false,  0); } set { }  }
}
/// Description: NMEA - Acknowledge group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaAcknowledgeGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaAcknowledgeGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public string pgnErrorCode { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 4, false,  0); } set { }  }
    public string transmissionIntervalPriorityErrorCode { get { return _packet.GetBitOffsetLength<string, string>(4, 36, 4, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string parameter { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 4, false,  0); } set { }  }
}
/// Description: NMEA - Read Fields group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaReadFieldsGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaReadFieldsGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 32, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 43, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 45, 3, false,  0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 0, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
}
/// Description: NMEA - Read Fields reply group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaReadFieldsReplyGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaReadFieldsReplyGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 32, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 43, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 45, 3, false,  0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 0, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 0, false,  0); } set { }  }
}
/// Description: NMEA - Write Fields group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaWriteFieldsGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaWriteFieldsGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 32, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 43, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 45, 3, false,  0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 0, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 0, false,  0); } set { }  }
}
/// Description: NMEA - Write Fields reply group function
/// Type: Fast
/// PGNNO: 126208
/// Length: 8
public class nmeaWriteFieldsReplyGroupFunction : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public nmeaWriteFieldsReplyGroupFunction(byte[] packet) { _packet = packet; }
    public int functionCode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 32, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 43, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 45, 3, false,  0); } set { }  }
    public int uniqueId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int OfSelectionPairs { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int OfParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int selectionParameter { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public int selectionValue { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 0, false,  0); } set { }  }
    public int parameter { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
    public int value { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 0, false,  0); } set { }  }
}
/// Description: PGN List (Transmit and Receive)
/// Type: Fast
/// PGNNO: 126464
/// Length: 8
public class pgnListTransmitAndReceive : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public pgnListTransmitAndReceive(byte[] packet) { _packet = packet; }
    public string functionCode { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int pgn { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 24, false,  1); } set { }  }
}
/// Description: Seatalk1: Pilot Mode
/// Type: Fast
/// PGNNO: 126720
/// Length: 21
public class seatalk1PilotMode : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalk1PilotMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public int command { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public byte[] unknown1 { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 24, false,  0); } set { }  }
    public int pilotMode { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  1); } set { }  }
    public int subMode { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public byte[] pilotModeData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 80, 8, false,  0); } set { }  }
    public byte[] unknown2 { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 88, 80, false,  0); } set { }  }
}
/// Description: Fusion: Media Control
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class fusionMediaControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionMediaControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
}
/// Description: Fusion: Sirius Control
/// Type: Fast
/// PGNNO: 126720
/// Length: 7
public class fusionSiriusControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSiriusControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
}
/// Description: Fusion: Request Status
/// Type: Fast
/// PGNNO: 126720
/// Length: 3
public class fusionRequestStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionRequestStatus(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
}
/// Description: Fusion: Set Source
/// Type: Fast
/// PGNNO: 126720
/// Length: 3
public class fusionSetSource : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSetSource(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
}
/// Description: Fusion: Mute
/// Type: Fast
/// PGNNO: 126720
/// Length: 3
public class fusionMute : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionMute(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string command { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
}
/// Description: Fusion: Set Zone Volume
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class fusionSetZoneVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSetZoneVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int zone { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public int volume { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
}
/// Description: Fusion: Set All Volumes
/// Type: Fast
/// PGNNO: 126720
/// Length: 9
public class fusionSetAllVolumes : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSetAllVolumes(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int unknown { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  1); } set { }  }
}
/// Description: Seatalk1: Keystroke
/// Type: Fast
/// PGNNO: 126720
/// Length: 21
public class seatalk1Keystroke : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalk1Keystroke(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public byte[] command { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 32, 8, false,  0); } set { }  }
    public string device { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public string key { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 16, false,  0); } set { }  }
    public byte[] unknownData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 64, 112, false,  0); } set { }  }
}
/// Description: Seatalk1: Device Identification
/// Type: Fast
/// PGNNO: 126720
/// Length: 8
public class seatalk1DeviceIdentification : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalk1DeviceIdentification(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public byte[] command { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 32, 8, false,  0); } set { }  }
    public string device { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 8, false,  0); } set { }  }
}
/// Description: Airmar: Attitude Offset
/// Type: Fast
/// PGNNO: 126720
/// Length: 9
public class airmarAttitudeOffset : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarAttitudeOffset(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public double azimuthOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.0001); } set { }  }
    public double pitchOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.0001); } set { }  }
    public double rollOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, true,  0.0001); } set { }  }
}
/// Description: Airmar: Calibrate Compass
/// Type: Fast
/// PGNNO: 126720
/// Length: 24
public class airmarCalibrateCompass : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarCalibrateCompass(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string calibrateFunction { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string calibrationStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int verifyScore { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public double xAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, true,  0.01); } set { }  }
    public double yAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, true,  0.01); } set { }  }
    public double zAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, true,  0.01); } set { }  }
    public double xAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 96, 16, true,  0.01); } set { }  }
    public double yAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, true,  0.01); } set { }  }
    public double zAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, true,  0.01); } set { }  }
    public double xAxisAngularOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 16, true,  0.1); } set { }  }
    public double pitchAndRollDamping { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 16, true,  0.05); } set { }  }
    public double compassRateGyroDamping { get { return _packet.GetBitOffsetLength<int, double>(0, 176, 16, true,  0.05); } set { }  }
}
/// Description: Airmar: True Wind Options
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class airmarTrueWindOptions : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarTrueWindOptions(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string cogSubstitutionForHdg { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string calibrationStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 8, false,  0); } set { }  }
    public int verifyScore { get { return _packet.GetBitOffsetLength<int, int>(2, 34, 8, false,  1); } set { }  }
    public double xAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(2, 42, 16, true,  0.01); } set { }  }
    public double yAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(2, 58, 16, true,  0.01); } set { }  }
    public double zAxisGainValue { get { return _packet.GetBitOffsetLength<int, double>(2, 74, 16, true,  0.01); } set { }  }
    public double xAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(2, 90, 16, true,  0.01); } set { }  }
    public double yAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(2, 106, 16, true,  0.01); } set { }  }
    public double zAxisLinearOffset { get { return _packet.GetBitOffsetLength<int, double>(2, 122, 16, true,  0.01); } set { }  }
    public double xAxisAngularOffset { get { return _packet.GetBitOffsetLength<int, double>(2, 138, 16, true,  0.1); } set { }  }
    public double pitchAndRollDamping { get { return _packet.GetBitOffsetLength<int, double>(2, 154, 16, true,  0.05); } set { }  }
    public double compassRateGyroDamping { get { return _packet.GetBitOffsetLength<int, double>(2, 170, 16, true,  0.05); } set { }  }
}
/// Description: Airmar: Simulate Mode
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class airmarSimulateMode : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarSimulateMode(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string simulateMode { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
}
/// Description: Airmar: Calibrate Depth
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class airmarCalibrateDepth : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarCalibrateDepth(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public double speedOfSoundMode { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.1); } set { }  }
}
/// Description: Airmar: Calibrate Speed
/// Type: Fast
/// PGNNO: 126720
/// Length: 12
public class airmarCalibrateSpeed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarCalibrateSpeed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int numberOfPairsOfDataPoints { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public double inputFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.1); } set { }  }
    public double outputSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.01); } set { }  }
}
/// Description: Airmar: Calibrate Temperature
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class airmarCalibrateTemperature : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarCalibrateTemperature(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string temperatureInstance { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public double temperatureOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.001); } set { }  }
}
/// Description: Airmar: Speed Filter
/// Type: Fast
/// PGNNO: 126720
/// Length: 8
public class airmarSpeedFilter : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarSpeedFilter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string filterType { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 4, false,  0); } set { }  }
    public double sampleInterval { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public double filterDuration { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.01); } set { }  }
}
/// Description: Airmar: Temperature Filter
/// Type: Fast
/// PGNNO: 126720
/// Length: 8
public class airmarTemperatureFilter : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarTemperatureFilter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string filterType { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 4, false,  0); } set { }  }
    public double sampleInterval { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public double filterDuration { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.01); } set { }  }
}
/// Description: Airmar: NMEA 2000 options
/// Type: Fast
/// PGNNO: 126720
/// Length: 6
public class airmarNmea2000Options : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarNmea2000Options(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string transmissionInterval { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
}
/// Description: Airmar: Addressable Multi-Frame
/// Type: Fast
/// PGNNO: 126720
/// Length: 4
public class airmarAddressableMultiFrame : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarAddressableMultiFrame(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int proprietaryId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
}
/// Description: Maretron: Slave Response
/// Type: Fast
/// PGNNO: 126720
/// Length: 8
public class maretronSlaveResponse : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronSlaveResponse(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int softwareCode { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  0); } set { }  }
    public int command { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Manufacturer Proprietary fast-packet addressed
/// Type: Fast
/// PGNNO: 126720
/// Length: 223
public class manufacturerProprietaryFastPacketAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public manufacturerProprietaryFastPacketAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 1768, false,  0); } set { }  }
}
/// Description: Unknown fast-packet non-addressed
/// Type: Fast
/// PGNNO: 126976
/// Length: 255
public class unknownFastPacketNonAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public unknownFastPacketNonAddressed(byte[] packet) { _packet = packet; }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 2040, false,  0); } set { }  }
}
/// Description: Alert
/// Type: Fast
/// PGNNO: 126983
/// Length: 28
public class alert : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alert(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public long dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int, long>(0, 40, 64, false,  0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 8, false,  0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 8, false,  0); } set { }  }
    public string temporarySilenceStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 128, 1, false,  0); } set { }  }
    public string acknowledgeStatus { get { return _packet.GetBitOffsetLength<string, string>(1, 129, 1, false,  0); } set { }  }
    public string escalationStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 130, 1, false,  0); } set { }  }
    public string temporarySilenceSupport { get { return _packet.GetBitOffsetLength<string, string>(3, 131, 1, false,  0); } set { }  }
    public string acknowledgeSupport { get { return _packet.GetBitOffsetLength<string, string>(4, 132, 1, false,  0); } set { }  }
    public string escalationSupport { get { return _packet.GetBitOffsetLength<string, string>(5, 133, 1, false,  0); } set { }  }
    public byte[] nmeaReserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 134, 2, false,  0); } set { }  }
    public long acknowledgeSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int, long>(0, 136, 64, false,  0); } set { }  }
    public string triggerCondition { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 4, false,  0); } set { }  }
    public string thresholdStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 204, 4, false,  0); } set { }  }
    public int alertPriority { get { return _packet.GetBitOffsetLength<int, int>(0, 208, 8, false,  0); } set { }  }
    public string alertState { get { return _packet.GetBitOffsetLength<string, string>(0, 216, 8, false,  0); } set { }  }
}
/// Description: Alert Response
/// Type: Fast
/// PGNNO: 126984
/// Length: 25
public class alertResponse : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alertResponse(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public long dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int, long>(0, 40, 64, false,  0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 8, false,  0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 8, false,  0); } set { }  }
    public long acknowledgeSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int, long>(0, 128, 64, false,  0); } set { }  }
    public string responseCommand { get { return _packet.GetBitOffsetLength<string, string>(0, 192, 2, false,  0); } set { }  }
    public byte[] nmeaReserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 194, 6, false,  0); } set { }  }
}
/// Description: Alert Text
/// Type: Fast
/// PGNNO: 126985
/// Length: 49
public class alertText : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alertText(byte[] packet) { _packet = packet; }
    public string alertType { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public string alertCategory { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public int alertSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int alertSubSystem { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int alertId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public long dataSourceNetworkIdName { get { return _packet.GetBitOffsetLength<int, long>(0, 40, 64, false,  0); } set { }  }
    public int dataSourceInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  0); } set { }  }
    public int dataSourceIndexSource { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 8, false,  0); } set { }  }
    public int alertOccurrenceNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 8, false,  0); } set { }  }
    public string languageId { get { return _packet.GetBitOffsetLength<string, string>(0, 128, 8, false,  0); } set { }  }
    public string alertTextDescription { get { return _packet.GetBitOffsetLength<string, string>(0, 136, 128, false,  0); } set { }  }
    public string alertLocationTextDescription { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 128, false,  0); } set { }  }
}
/// Description: Alert Configuration
/// Type: Fast
/// PGNNO: 126986
/// Length: 8
public class alertConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alertConfiguration(byte[] packet) { _packet = packet; }
}
/// Description: Alert Threshold
/// Type: Fast
/// PGNNO: 126987
/// Length: 8
public class alertThreshold : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alertThreshold(byte[] packet) { _packet = packet; }
}
/// Description: Alert Value
/// Type: Fast
/// PGNNO: 126988
/// Length: 8
public class alertValue : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public alertValue(byte[] packet) { _packet = packet; }
}
/// Description: System Time
/// Type: Single
/// PGNNO: 126992
/// Length: 8
public class systemTime : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public systemTime(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 12, 4, false,  0); } set { }  }
    public int date { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public int time { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0.0001); } set { }  }
}
/// Description: Heartbeat
/// Type: Single
/// PGNNO: 126993
/// Length: 8
public class heartbeat : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public heartbeat(byte[] packet) { _packet = packet; }
    public double dataTransmitOffset { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 16, false,  0.001); } set { }  }
    public int sequenceCounter { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string controller1State { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string controller2State { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public string equipmentStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 30, 34, false,  0); } set { }  }
}
/// Description: Product Information
/// Type: Fast
/// PGNNO: 126996
/// Length: 134
public class productInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public productInformation(byte[] packet) { _packet = packet; }
    public int nmea2000Version { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string modelId { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 256, false,  0); } set { }  }
    public string softwareVersionCode { get { return _packet.GetBitOffsetLength<string, string>(0, 288, 256, false,  0); } set { }  }
    public string modelVersion { get { return _packet.GetBitOffsetLength<string, string>(0, 544, 256, false,  0); } set { }  }
    public string modelSerialCode { get { return _packet.GetBitOffsetLength<string, string>(0, 800, 256, false,  0); } set { }  }
    public int certificationLevel { get { return _packet.GetBitOffsetLength<int, int>(0, 1056, 8, false,  0); } set { }  }
    public int loadEquivalency { get { return _packet.GetBitOffsetLength<int, int>(0, 1064, 8, false,  0); } set { }  }
}
/// Description: Configuration Information
/// Type: Fast
/// PGNNO: 126998
/// Length: 42
public class configurationInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public configurationInformation(byte[] packet) { _packet = packet; }
    public string installationDescription1 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
    public string installationDescription2 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
    public string manufacturerInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
}
/// Description: Man Overboard Notification
/// Type: Fast
/// PGNNO: 127233
/// Length: 35
public class manOverboardNotification : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public manOverboardNotification(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int mobEmitterId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public string manOverboardStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 3, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(3, 43, 5, false,  0); } set { }  }
    public int activationTime { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0.0001); } set { }  }
    public string positionSource { get { return _packet.GetBitOffsetLength<string, string>(0, 80, 3, false,  0); } set { }  }
    public int positionDate { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, false,  1); } set { }  }
    public int positionTime { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 32, false,  0.0001); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 32, true,  1E-07); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 32, true,  1E-07); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 2, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 208, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 224, 16, false,  0.01); } set { }  }
    public int mmsiOfVesselOfOrigin { get { return _packet.GetBitOffsetLength<int, int>(0, 240, 32, false,  1); } set { }  }
    public string mobEmitterBatteryStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 272, 3, false,  0); } set { }  }
}
/// Description: Heading/Track control
/// Type: Fast
/// PGNNO: 127237
/// Length: 21
public class headingTrackControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public headingTrackControl(byte[] packet) { _packet = packet; }
    public string rudderLimitExceeded { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public string offHeadingLimitExceeded { get { return _packet.GetBitOffsetLength<string, string>(2, 2, 2, false,  0); } set { }  }
    public string offTrackLimitExceeded { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 2, false,  0); } set { }  }
    public string @override { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public string steeringMode { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 3, false,  0); } set { }  }
    public string turnMode { get { return _packet.GetBitOffsetLength<string, string>(3, 11, 3, false,  0); } set { }  }
    public string headingReference { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 5, false,  0); } set { }  }
    public string commandedRudderDirection { get { return _packet.GetBitOffsetLength<string, string>(5, 21, 3, false,  0); } set { }  }
    public double commandedRudderAngle { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.0001); } set { }  }
    public double headingToSteerCourse { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.0001); } set { }  }
    public double track { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.0001); } set { }  }
    public double rudderLimit { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.0001); } set { }  }
    public double offHeadingLimit { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 16, false,  0.0001); } set { }  }
    public double radiusOfTurnOrder { get { return _packet.GetBitOffsetLength<int, double>(0, 104, 16, true,  0.0001); } set { }  }
    public double rateOfTurnOrder { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 16, true,  3.125E-05); } set { }  }
    public int offTrackLimit { get { return _packet.GetBitOffsetLength<int, int>(0, 136, 16, true,  0); } set { }  }
    public double vesselHeading { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 16, false,  0.0001); } set { }  }
}
/// Description: Rudder
/// Type: Single
/// PGNNO: 127245
/// Length: 8
public class rudder : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public rudder(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string directionOrder { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 3, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(3, 11, 5, false,  0); } set { }  }
    public double angleOrder { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, true,  0.0001); } set { }  }
    public double position { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.0001); } set { }  }
}
/// Description: Vessel Heading
/// Type: Single
/// PGNNO: 127250
/// Length: 8
public class vesselHeading : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public vesselHeading(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double heading { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.0001); } set { }  }
    public double deviation { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.0001); } set { }  }
    public double variation { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.0001); } set { }  }
    public string reference { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 58, 6, false,  0); } set { }  }
}
/// Description: Rate of Turn
/// Type: Single
/// PGNNO: 127251
/// Length: 5
public class rateOfTurn : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public rateOfTurn(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double rate { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 32, true,  3.125E-08); } set { }  }
}
/// Description: Heave
/// Type: Single
/// PGNNO: 127252
/// Length: 8
public class heave : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public heave(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double heave_ { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, true,  0.01); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 40, false,  0); } set { }  }
}
/// Description: Attitude
/// Type: Single
/// PGNNO: 127257
/// Length: 7
public class attitude : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public attitude(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double yaw { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, true,  0.0001); } set { }  }
    public double pitch { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.0001); } set { }  }
    public double roll { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.0001); } set { }  }
}
/// Description: Magnetic Variation
/// Type: Single
/// PGNNO: 127258
/// Length: 6
public class magneticVariation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public magneticVariation(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 12, 4, false,  0); } set { }  }
    public int ageOfService { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public double variation { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.0001); } set { }  }
}
/// Description: Engine Parameters, Rapid Update
/// Type: Single
/// PGNNO: 127488
/// Length: 8
public class engineParametersRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public engineParametersRapidUpdate(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public double speed { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.25); } set { }  }
    public double boostPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0); } set { }  }
    public int tiltTrim { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, true,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Engine Parameters, Dynamic
/// Type: Fast
/// PGNNO: 127489
/// Length: 26
public class engineParametersDynamic : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public engineParametersDynamic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public double oilPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0); } set { }  }
    public double oilTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.1); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
    public double alternatorPotential { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, true,  0.01); } set { }  }
    public double fuelRate { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, true,  0.1); } set { }  }
    public int totalEngineHours { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 32, false,  0); } set { }  }
    public double coolantPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 16, false,  0); } set { }  }
    public int fuelPressure { get { return _packet.GetBitOffsetLength<int, int>(0, 136, 16, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 152, 8, false,  0); } set { }  }
    public long discreteStatus1 { get { return _packet.GetBitOffsetLength<long, long>(0, 160, 16, false,  0); } set { }  }
    public long discreteStatus2 { get { return _packet.GetBitOffsetLength<long, long>(0, 176, 16, false,  0); } set { }  }
    public int percentEngineLoad { get { return _packet.GetBitOffsetLength<int, int>(0, 192, 8, true,  1); } set { }  }
    public int percentEngineTorque { get { return _packet.GetBitOffsetLength<int, int>(0, 200, 8, true,  1); } set { }  }
}
/// Description: Transmission Parameters, Dynamic
/// Type: Single
/// PGNNO: 127493
/// Length: 8
public class transmissionParametersDynamic : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public transmissionParametersDynamic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string transmissionGear { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 10, 6, false,  0); } set { }  }
    public double oilPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0); } set { }  }
    public double oilTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.1); } set { }  }
    public int discreteStatus1 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
}
/// Description: Trip Parameters, Vessel
/// Type: Fast
/// PGNNO: 127496
/// Length: 10
public class tripParametersVessel : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public tripParametersVessel(byte[] packet) { _packet = packet; }
    public double timeToEmpty { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, false,  0.001); } set { }  }
    public double distanceToEmpty { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, false,  0.01); } set { }  }
    public int estimatedFuelRemaining { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
    public double tripRunTime { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 32, false,  0.001); } set { }  }
}
/// Description: Trip Parameters, Engine
/// Type: Fast
/// PGNNO: 127497
/// Length: 9
public class tripParametersEngine : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public tripParametersEngine(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int tripFuelUsed { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public double fuelRateAverage { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.1); } set { }  }
    public double fuelRateEconomy { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.1); } set { }  }
    public double instantaneousFuelEconomy { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, true,  0.1); } set { }  }
}
/// Description: Engine Parameters, Static
/// Type: Fast
/// PGNNO: 127498
/// Length: 52
public class engineParametersStatic : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public engineParametersStatic(byte[] packet) { _packet = packet; }
    public string instance { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public double ratedEngineSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.25); } set { }  }
    public string vin { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 136, false,  0); } set { }  }
    public string softwareId { get { return _packet.GetBitOffsetLength<string, string>(0, 160, 256, false,  0); } set { }  }
}
/// Description: Load Controller Connection State/Control
/// Type: Fast
/// PGNNO: 127500
/// Length: 10
public class loadControllerConnectionStateControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public loadControllerConnectionStateControl(byte[] packet) { _packet = packet; }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int connectionId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int state { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int status { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int operationalStatusControl { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  1); } set { }  }
    public int pwmDutyCycle { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public int timeon { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  1); } set { }  }
    public int timeoff { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  1); } set { }  }
}
/// Description: Binary Switch Bank Status
/// Type: Single
/// PGNNO: 127501
/// Length: 8
public class binarySwitchBankStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public binarySwitchBankStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string indicator1 { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public string indicator2 { get { return _packet.GetBitOffsetLength<string, string>(2, 10, 2, false,  0); } set { }  }
    public string indicator3 { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public string indicator4 { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public string indicator5 { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string indicator6 { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public string indicator7 { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public string indicator8 { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public string indicator9 { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string indicator10 { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public string indicator11 { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public string indicator12 { get { return _packet.GetBitOffsetLength<string, string>(6, 30, 2, false,  0); } set { }  }
    public string indicator13 { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
    public string indicator14 { get { return _packet.GetBitOffsetLength<string, string>(2, 34, 2, false,  0); } set { }  }
    public string indicator15 { get { return _packet.GetBitOffsetLength<string, string>(4, 36, 2, false,  0); } set { }  }
    public string indicator16 { get { return _packet.GetBitOffsetLength<string, string>(6, 38, 2, false,  0); } set { }  }
    public string indicator17 { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 2, false,  0); } set { }  }
    public string indicator18 { get { return _packet.GetBitOffsetLength<string, string>(2, 42, 2, false,  0); } set { }  }
    public string indicator19 { get { return _packet.GetBitOffsetLength<string, string>(4, 44, 2, false,  0); } set { }  }
    public string indicator20 { get { return _packet.GetBitOffsetLength<string, string>(6, 46, 2, false,  0); } set { }  }
    public string indicator21 { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 2, false,  0); } set { }  }
    public string indicator22 { get { return _packet.GetBitOffsetLength<string, string>(2, 50, 2, false,  0); } set { }  }
    public string indicator23 { get { return _packet.GetBitOffsetLength<string, string>(4, 52, 2, false,  0); } set { }  }
    public string indicator24 { get { return _packet.GetBitOffsetLength<string, string>(6, 54, 2, false,  0); } set { }  }
    public string indicator25 { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 2, false,  0); } set { }  }
    public string indicator26 { get { return _packet.GetBitOffsetLength<string, string>(2, 58, 2, false,  0); } set { }  }
    public string indicator27 { get { return _packet.GetBitOffsetLength<string, string>(4, 60, 2, false,  0); } set { }  }
    public string indicator28 { get { return _packet.GetBitOffsetLength<string, string>(6, 62, 2, false,  0); } set { }  }
}
/// Description: Switch Bank Control
/// Type: Single
/// PGNNO: 127502
/// Length: 8
public class switchBankControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public switchBankControl(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string switch1 { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public string switch2 { get { return _packet.GetBitOffsetLength<string, string>(2, 10, 2, false,  0); } set { }  }
    public string switch3 { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public string switch4 { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public string switch5 { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string switch6 { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public string switch7 { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public string switch8 { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public string switch9 { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string switch10 { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public string switch11 { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public string switch12 { get { return _packet.GetBitOffsetLength<string, string>(6, 30, 2, false,  0); } set { }  }
    public string switch13 { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
    public string switch14 { get { return _packet.GetBitOffsetLength<string, string>(2, 34, 2, false,  0); } set { }  }
    public string switch15 { get { return _packet.GetBitOffsetLength<string, string>(4, 36, 2, false,  0); } set { }  }
    public string switch16 { get { return _packet.GetBitOffsetLength<string, string>(6, 38, 2, false,  0); } set { }  }
    public string switch17 { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 2, false,  0); } set { }  }
    public string switch18 { get { return _packet.GetBitOffsetLength<string, string>(2, 42, 2, false,  0); } set { }  }
    public string switch19 { get { return _packet.GetBitOffsetLength<string, string>(4, 44, 2, false,  0); } set { }  }
    public string switch20 { get { return _packet.GetBitOffsetLength<string, string>(6, 46, 2, false,  0); } set { }  }
    public string switch21 { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 2, false,  0); } set { }  }
    public string switch22 { get { return _packet.GetBitOffsetLength<string, string>(2, 50, 2, false,  0); } set { }  }
    public string switch23 { get { return _packet.GetBitOffsetLength<string, string>(4, 52, 2, false,  0); } set { }  }
    public string switch24 { get { return _packet.GetBitOffsetLength<string, string>(6, 54, 2, false,  0); } set { }  }
    public string switch25 { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 2, false,  0); } set { }  }
    public string switch26 { get { return _packet.GetBitOffsetLength<string, string>(2, 58, 2, false,  0); } set { }  }
    public string switch27 { get { return _packet.GetBitOffsetLength<string, string>(4, 60, 2, false,  0); } set { }  }
    public string switch28 { get { return _packet.GetBitOffsetLength<string, string>(6, 62, 2, false,  0); } set { }  }
}
/// Description: AC Input Status
/// Type: Fast
/// PGNNO: 127503
/// Length: 56
public class acInputStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public acInputStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int numberOfLines { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string line { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string acceptability { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 20, 4, false,  0); } set { }  }
    public double voltage { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double current { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.1); } set { }  }
    public double frequency { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
    public double breakerSize { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.1); } set { }  }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 32, false,  1); } set { }  }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 32, false,  1); } set { }  }
    public double powerFactor { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 8, false,  0.01); } set { }  }
}
/// Description: AC Output Status
/// Type: Single
/// PGNNO: 127504
/// Length: 56
public class acOutputStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public acOutputStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int numberOfLines { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string line { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string waveform { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 3, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(5, 21, 3, false,  0); } set { }  }
    public double voltage { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double current { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.1); } set { }  }
    public double frequency { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
    public double breakerSize { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.1); } set { }  }
    public int realPower { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 32, false,  1); } set { }  }
    public int reactivePower { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 32, false,  1); } set { }  }
    public double powerFactor { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 8, false,  0.01); } set { }  }
}
/// Description: Fluid Level
/// Type: Single
/// PGNNO: 127505
/// Length: 8
public class fluidLevel : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fluidLevel(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 4, false,  0); } set { }  }
    public string type { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public double level { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.004); } set { }  }
    public double capacity { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 32, false,  0.1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 56, 8, false,  0); } set { }  }
}
/// Description: DC Detailed Status
/// Type: Fast
/// PGNNO: 127506
/// Length: 11
public class dcDetailedStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public dcDetailedStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string dcType { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public int stateOfCharge { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int stateOfHealth { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int timeRemaining { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 16, false,  0); } set { }  }
    public double rippleVoltage { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
    public double ampHours { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  3600); } set { }  }
}
/// Description: Charger Status
/// Type: Fast
/// PGNNO: 127507
/// Length: 6
public class chargerStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public chargerStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int batteryInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 4, false,  0); } set { }  }
    public string chargeMode { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 4, false,  0); } set { }  }
    public string enabled { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string equalizationPending { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 28, 4, false,  0); } set { }  }
    public int equalizationTimeRemaining { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  0); } set { }  }
}
/// Description: Battery Status
/// Type: Single
/// PGNNO: 127508
/// Length: 8
public class batteryStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public batteryStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double voltage { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, true,  0.01); } set { }  }
    public double current { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.1); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Inverter Status
/// Type: Single
/// PGNNO: 127509
/// Length: 4
public class inverterStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public inverterStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int acInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int dcInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 4, false,  0); } set { }  }
    public string inverterEnableDisable { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 30, 2, false,  0); } set { }  }
}
/// Description: Charger Configuration Status
/// Type: Fast
/// PGNNO: 127510
/// Length: 13
public class chargerConfigurationStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public chargerConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int batteryInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int chargerEnableDisable { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 18, 6, false,  0); } set { }  }
    public double chargeCurrentLimit { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.1); } set { }  }
    public int chargingAlgorithm { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int chargerMode { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public double estimatedTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
    public int equalizeOneTimeEnableDisable { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 4, false,  0); } set { }  }
    public int overChargeEnableDisable { get { return _packet.GetBitOffsetLength<int, int>(4, 76, 4, false,  0); } set { }  }
    public int equalizeTime { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  0); } set { }  }
}
/// Description: Inverter Configuration Status
/// Type: Single
/// PGNNO: 127511
/// Length: 8
public class inverterConfigurationStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public inverterConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int acInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int dcInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int inverterEnableDisable { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 2, false,  0); } set { }  }
    public int inverterMode { get { return _packet.GetBitOffsetLength<int, int>(2, 26, 8, false,  0); } set { }  }
    public int loadSenseEnableDisable { get { return _packet.GetBitOffsetLength<int, int>(2, 34, 8, false,  0); } set { }  }
    public int loadSensePowerThreshold { get { return _packet.GetBitOffsetLength<int, int>(2, 42, 8, false,  0); } set { }  }
    public int loadSenseInterval { get { return _packet.GetBitOffsetLength<int, int>(2, 50, 8, false,  0); } set { }  }
}
/// Description: AGS Configuration Status
/// Type: Single
/// PGNNO: 127512
/// Length: 8
public class agsConfigurationStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public agsConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int generatorInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int agsMode { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
}
/// Description: Battery Configuration Status
/// Type: Fast
/// PGNNO: 127513
/// Length: 10
public class batteryConfigurationStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public batteryConfigurationStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string batteryType { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public string supportsEqualization { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 14, 2, false,  0); } set { }  }
    public string nominalVoltage { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 4, false,  0); } set { }  }
    public string chemistry { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 4, false,  0); } set { }  }
    public int capacity { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public int temperatureCoefficient { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public double peukertExponent { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 8, false,  0.002); } set { }  }
    public int chargeEfficiencyFactor { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: AGS Status
/// Type: Single
/// PGNNO: 127514
/// Length: 8
public class agsStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public agsStatus(byte[] packet) { _packet = packet; }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int generatorInstance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int agsOperatingState { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int generatorState { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int generatorOnReason { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int generatorOffReason { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
}
/// Description: AC Power / Current - Phase A
/// Type: Single
/// PGNNO: 127744
/// Length: 8
public class acPowerCurrentPhaseA : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public acPowerCurrentPhaseA(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public double acRmsCurrent { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: AC Power / Current - Phase B
/// Type: Single
/// PGNNO: 127745
/// Length: 8
public class acPowerCurrentPhaseB : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public acPowerCurrentPhaseB(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public double acRmsCurrent { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: AC Power / Current - Phase C
/// Type: Single
/// PGNNO: 127746
/// Length: 8
public class acPowerCurrentPhaseC : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public acPowerCurrentPhaseC(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public double acRmsCurrent { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.1); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, true,  0); } set { }  }
}
/// Description: Converter Status
/// Type: Single
/// PGNNO: 127750
/// Length: 8
public class converterStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public converterStatus(byte[] packet) { _packet = packet; }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 8, false,  0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string operatingState { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public string temperatureState { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string overloadState { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public string lowDcVoltageState { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public string rippleState { get { return _packet.GetBitOffsetLength<string, string>(6, 30, 2, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: DC Voltage/Current
/// Type: Single
/// PGNNO: 127751
/// Length: 8
public class dcVoltageCurrent : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public dcVoltageCurrent(byte[] packet) { _packet = packet; }
    public byte[] sid { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 0, 8, false,  0); } set { }  }
    public int connectionNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public double dcVoltage { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.1); } set { }  }
    public double dcCurrent { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 24, true,  0.01); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Leeway Angle
/// Type: Single
/// PGNNO: 128000
/// Length: 8
public class leewayAngle : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public leewayAngle(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double leewayAngle_ { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, true,  0.0001); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 24, 40, false,  0); } set { }  }
}
/// Description: Thruster Control Status
/// Type: Single
/// PGNNO: 128006
/// Length: 8
public class thrusterControlStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public thrusterControlStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int identifier { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string directionControl { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 4, false,  0); } set { }  }
    public string powerEnabled { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public string retractControl { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public double speedControl { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 8, false,  0.004); } set { }  }
    public long controlEvents { get { return _packet.GetBitOffsetLength<long, long>(0, 32, 8, false,  0); } set { }  }
    public double commandTimeout { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 8, false,  0.001); } set { }  }
    public double azimuthControl { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.0001); } set { }  }
}
/// Description: Thruster Information
/// Type: Single
/// PGNNO: 128007
/// Length: 8
public class thrusterInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public thrusterInformation(byte[] packet) { _packet = packet; }
    public int identifier { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string motorType { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 12, 4, false,  0); } set { }  }
    public int powerRating { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public double maximumTemperatureRating { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public double maximumRotationalSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.25); } set { }  }
}
/// Description: Thruster Motor Status
/// Type: Single
/// PGNNO: 128008
/// Length: 8
public class thrusterMotorStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public thrusterMotorStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int identifier { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public long motorEvents { get { return _packet.GetBitOffsetLength<long, long>(0, 16, 8, false,  0); } set { }  }
    public int current { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public int operatingTime { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
}
/// Description: Speed
/// Type: Single
/// PGNNO: 128259
/// Length: 8
public class speed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public speed(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double speedWaterReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.01); } set { }  }
    public double speedGroundReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public string speedWaterReferencedType { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public int speedDirection { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 52, 12, false,  0); } set { }  }
}
/// Description: Water Depth
/// Type: Single
/// PGNNO: 128267
/// Length: 8
public class waterDepth : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public waterDepth(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double depth { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 32, false,  0.01); } set { }  }
    public double offset { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.001); } set { }  }
    public double range { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 8, false,  10); } set { }  }
}
/// Description: Distance Log
/// Type: Fast
/// PGNNO: 128275
/// Length: 14
public class distanceLog : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public distanceLog(byte[] packet) { _packet = packet; }
    public int date { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int time { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 32, false,  0.0001); } set { }  }
    public int log { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0); } set { }  }
    public int tripLog { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 32, false,  0); } set { }  }
}
/// Description: Tracked Target Data
/// Type: Fast
/// PGNNO: 128520
/// Length: 27
public class trackedTargetData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public trackedTargetData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int targetId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string trackStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string reportedTarget { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 1, false,  0); } set { }  }
    public string targetAcquisition { get { return _packet.GetBitOffsetLength<string, string>(3, 19, 1, false,  0); } set { }  }
    public string bearingReference { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 22, 2, false,  0); } set { }  }
    public double bearing { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.0001); } set { }  }
    public double distance { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, false,  0.001); } set { }  }
    public double course { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.0001); } set { }  }
    public double speed { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 16, false,  0.01); } set { }  }
    public double cpa { get { return _packet.GetBitOffsetLength<int, double>(0, 104, 32, false,  0.01); } set { }  }
    public double tcpa { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 32, false,  0.001); } set { }  }
    public int utcOfFix { get { return _packet.GetBitOffsetLength<int, int>(0, 168, 32, false,  0.0001); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 2040, false,  0); } set { }  }
}
/// Description: Windlass Control Status
/// Type: Single
/// PGNNO: 128776
/// Length: 7
public class windlassControlStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public windlassControlStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string windlassDirectionControl { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string anchorDockingControl { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public string speedControlType { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 22, 2, false,  0); } set { }  }
    public byte[] speedControl { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 24, 8, false,  0); } set { }  }
    public string powerEnable { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 2, false,  0); } set { }  }
    public string mechanicalLock { get { return _packet.GetBitOffsetLength<string, string>(2, 34, 2, false,  0); } set { }  }
    public string deckAndAnchorWash { get { return _packet.GetBitOffsetLength<string, string>(4, 36, 2, false,  0); } set { }  }
    public string anchorLight { get { return _packet.GetBitOffsetLength<string, string>(6, 38, 2, false,  0); } set { }  }
    public double commandTimeout { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 8, false,  0.005); } set { }  }
    public long windlassControlEvents { get { return _packet.GetBitOffsetLength<long, long>(0, 48, 4, false,  0); } set { }  }
}
/// Description: Anchor Windlass Operating Status
/// Type: Single
/// PGNNO: 128777
/// Length: 8
public class anchorWindlassOperatingStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public anchorWindlassOperatingStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string windlassDirectionControl { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string windlassMotionStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public string rodeTypeStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 22, 2, false,  0); } set { }  }
    public double rodeCounterValue { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.1); } set { }  }
    public double windlassLineSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
    public string anchorDockingStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 2, false,  0); } set { }  }
    public long windlassOperatingEvents { get { return _packet.GetBitOffsetLength<long, long>(2, 58, 6, false,  0); } set { }  }
}
/// Description: Anchor Windlass Monitoring Status
/// Type: Single
/// PGNNO: 128778
/// Length: 8
public class anchorWindlassMonitoringStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public anchorWindlassMonitoringStatus(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int windlassId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public long windlassMonitoringEvents { get { return _packet.GetBitOffsetLength<long, long>(0, 16, 8, false,  0); } set { }  }
    public double controllerVoltage { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 8, false,  0.2); } set { }  }
    public int motorCurrent { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public double totalMotorTime { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  60); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Position, Rapid Update
/// Type: Single
/// PGNNO: 129025
/// Length: 8
public class positionRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public positionRapidUpdate(byte[] packet) { _packet = packet; }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  1E-07); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, true,  1E-07); } set { }  }
}
/// Description: COG & SOG, Rapid Update
/// Type: Single
/// PGNNO: 129026
/// Length: 8
public class cogSogRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public cogSogRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 10, 6, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
}
/// Description: Position Delta, Rapid Update
/// Type: Single
/// PGNNO: 129027
/// Length: 8
public class positionDeltaRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public positionDeltaRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int timeDelta { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int latitudeDelta { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, true,  0); } set { }  }
    public int longitudeDelta { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 16, true,  0); } set { }  }
}
/// Description: Altitude Delta, Rapid Update
/// Type: Single
/// PGNNO: 129028
/// Length: 8
public class altitudeDeltaRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public altitudeDeltaRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int timeDelta { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, true,  0); } set { }  }
    public int gnssQuality { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 2, false,  0); } set { }  }
    public int direction { get { return _packet.GetBitOffsetLength<int, int>(2, 26, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 28, 4, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0001); } set { }  }
    public int altitudeDelta { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, true,  0); } set { }  }
}
/// Description: GNSS Position Data
/// Type: Fast
/// PGNNO: 129029
/// Length: 43
public class gnssPositionData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssPositionData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int date { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int time { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<long, double>(0, 56, 64, true,  1E-16); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<long, double>(0, 120, 64, true,  1E-16); } set { }  }
    public double altitude { get { return _packet.GetBitOffsetLength<int, double>(0, 184, 64, true,  1E-06); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string, string>(0, 248, 4, false,  0); } set { }  }
    public string method { get { return _packet.GetBitOffsetLength<string, string>(4, 252, 4, false,  0); } set { }  }
    public string integrity { get { return _packet.GetBitOffsetLength<string, string>(0, 256, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 258, 6, false,  0); } set { }  }
    public int numberOfSvs { get { return _packet.GetBitOffsetLength<int, int>(0, 264, 8, false,  0); } set { }  }
    public double hdop { get { return _packet.GetBitOffsetLength<int, double>(0, 272, 16, true,  0.01); } set { }  }
    public double pdop { get { return _packet.GetBitOffsetLength<int, double>(0, 288, 16, true,  0.01); } set { }  }
    public double geoidalSeparation { get { return _packet.GetBitOffsetLength<int, double>(0, 304, 32, true,  0.01); } set { }  }
    public int referenceStations { get { return _packet.GetBitOffsetLength<int, int>(0, 336, 8, false,  0); } set { }  }
    public string referenceStationType { get { return _packet.GetBitOffsetLength<string, string>(0, 344, 4, false,  0); } set { }  }
    public int referenceStationId { get { return _packet.GetBitOffsetLength<int, int>(4, 348, 12, false,  0); } set { }  }
    public double ageOfDgnssCorrections { get { return _packet.GetBitOffsetLength<int, double>(0, 360, 16, false,  0.01); } set { }  }
}
/// Description: Time & Date
/// Type: Single
/// PGNNO: 129033
/// Length: 8
public class timeDate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public timeDate(byte[] packet) { _packet = packet; }
    public int date { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int time { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 32, false,  0.0001); } set { }  }
    public int localOffset { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, true,  1); } set { }  }
}
/// Description: AIS Class A Position Report
/// Type: Fast
/// PGNNO: 129038
/// Length: 28
public class aisClassAPositionReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassAPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string, string>(2, 106, 6, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, false,  0.01); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 144, 19, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(3, 163, 5, false,  0); } set { }  }
    public double heading { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 16, false,  0.0001); } set { }  }
    public double rateOfTurn { get { return _packet.GetBitOffsetLength<int, double>(0, 184, 16, true,  3.125E-05); } set { }  }
    public string navStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 4, false,  0); } set { }  }
    public string specialManeuverIndicator { get { return _packet.GetBitOffsetLength<string, string>(4, 204, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 206, 2, false,  0); } set { }  }
    public byte[] aisSpare { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 208, 3, false,  0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int, int>(0, 216, 8, false,  1); } set { }  }
}
/// Description: AIS Class B Position Report
/// Type: Fast
/// PGNNO: 129039
/// Length: 26
public class aisClassBPositionReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassBPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string, string>(2, 106, 6, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, false,  0.01); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 144, 19, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(3, 163, 5, false,  0); } set { }  }
    public double heading { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 16, false,  0.0001); } set { }  }
    public int regionalApplication { get { return _packet.GetBitOffsetLength<int, int>(0, 184, 8, false,  0); } set { }  }
    public string unitType { get { return _packet.GetBitOffsetLength<string, string>(2, 194, 1, false,  0); } set { }  }
    public string integratedDisplay { get { return _packet.GetBitOffsetLength<string, string>(3, 195, 1, false,  0); } set { }  }
    public string dsc { get { return _packet.GetBitOffsetLength<string, string>(4, 196, 1, false,  0); } set { }  }
    public string band { get { return _packet.GetBitOffsetLength<string, string>(5, 197, 1, false,  0); } set { }  }
    public string canHandleMsg22 { get { return _packet.GetBitOffsetLength<string, string>(6, 198, 1, false,  0); } set { }  }
    public string aisMode { get { return _packet.GetBitOffsetLength<string, string>(7, 199, 1, false,  0); } set { }  }
    public string aisCommunicationState { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 1, false,  0); } set { }  }
}
/// Description: AIS Class B Extended Position Report
/// Type: Fast
/// PGNNO: 129040
/// Length: 33
public class aisClassBExtendedPositionReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassBExtendedPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string aisRaimFlag { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string, string>(2, 106, 6, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, false,  0.01); } set { }  }
    public int regionalApplication { get { return _packet.GetBitOffsetLength<int, int>(0, 144, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 156, 4, false,  0); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string, string>(0, 160, 8, false,  0); } set { }  }
    public double trueHeading { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 16, false,  0.0001); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string, string>(4, 188, 4, false,  0); } set { }  }
    public double length { get { return _packet.GetBitOffsetLength<int, double>(0, 192, 16, false,  0.1); } set { }  }
    public double beam { get { return _packet.GetBitOffsetLength<int, double>(0, 208, 16, false,  0.1); } set { }  }
    public double positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int, double>(0, 224, 16, false,  0.1); } set { }  }
    public double positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int, double>(0, 240, 16, false,  0.1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 256, 160, false,  0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string, string>(0, 416, 1, false,  0); } set { }  }
    public int aisMode { get { return _packet.GetBitOffsetLength<int, int>(1, 417, 1, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(6, 422, 5, false,  0); } set { }  }
}
/// Description: AIS Aids to Navigation (AtoN) Report
/// Type: Fast
/// PGNNO: 129041
/// Length: 60
public class aisAidsToNavigationAtonReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisAidsToNavigationAtonReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string aisRaimFlag { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string, string>(2, 106, 6, false,  0); } set { }  }
    public double lengthDiameter { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.1); } set { }  }
    public double beamDiameter { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, false,  0.1); } set { }  }
    public double positionReferenceFromStarboardEdge { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 16, false,  0.1); } set { }  }
    public double positionReferenceFromTrueNorthFacingEdge { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 16, false,  0.1); } set { }  }
    public string atonType { get { return _packet.GetBitOffsetLength<string, string>(0, 176, 5, false,  0); } set { }  }
    public string offPositionIndicator { get { return _packet.GetBitOffsetLength<string, string>(5, 181, 1, false,  0); } set { }  }
    public string virtualAtonFlag { get { return _packet.GetBitOffsetLength<string, string>(6, 182, 1, false,  0); } set { }  }
    public string assignedModeFlag { get { return _packet.GetBitOffsetLength<string, string>(7, 183, 1, false,  0); } set { }  }
    public byte[] aisSpare { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 184, 1, false,  0); } set { }  }
    public string positionFixingDeviceType { get { return _packet.GetBitOffsetLength<string, string>(1, 185, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(5, 189, 3, false,  0); } set { }  }
    public byte[] atonStatus { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 192, 8, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 5, false,  0); } set { }  }
    public string atonName { get { return _packet.GetBitOffsetLength<string, string>(0, 208, 272, false,  0); } set { }  }
}
/// Description: Datum
/// Type: Fast
/// PGNNO: 129044
/// Length: 20
public class datum : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public datum(byte[] packet) { _packet = packet; }
    public string localDatum { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 32, false,  0); } set { }  }
    public double deltaLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, true,  1E-07); } set { }  }
    public double deltaLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 32, true,  1E-07); } set { }  }
    public double deltaAltitude { get { return _packet.GetBitOffsetLength<int, double>(0, 96, 32, true,  1E-06); } set { }  }
    public string referenceDatum { get { return _packet.GetBitOffsetLength<string, string>(0, 128, 32, false,  0); } set { }  }
}
/// Description: User Datum
/// Type: Fast
/// PGNNO: 129045
/// Length: 37
public class userDatum : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public userDatum(byte[] packet) { _packet = packet; }
    public double deltaX { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  0.01); } set { }  }
    public double deltaY { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, true,  0.01); } set { }  }
    public double deltaZ { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 32, true,  0.01); } set { }  }
    public float rotationInX { get { return _packet.GetBitOffsetLength<float, float>(0, 96, 32, true,  0); } set { }  }
    public float rotationInY { get { return _packet.GetBitOffsetLength<float, float>(0, 128, 32, true,  0); } set { }  }
    public float rotationInZ { get { return _packet.GetBitOffsetLength<float, float>(0, 160, 32, true,  0); } set { }  }
    public float scale { get { return _packet.GetBitOffsetLength<float, float>(0, 192, 32, true,  0); } set { }  }
    public double ellipsoidSemiMajorAxis { get { return _packet.GetBitOffsetLength<int, double>(0, 224, 32, true,  0.01); } set { }  }
    public float ellipsoidFlatteningInverse { get { return _packet.GetBitOffsetLength<float, float>(0, 256, 32, true,  0); } set { }  }
    public string datumName { get { return _packet.GetBitOffsetLength<string, string>(0, 288, 32, false,  0); } set { }  }
}
/// Description: Cross Track Error
/// Type: Single
/// PGNNO: 129283
/// Length: 8
public class crossTrackError : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public crossTrackError(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string xteMode { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 12, 2, false,  0); } set { }  }
    public string navigationTerminated { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public double xte { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 32, true,  0.01); } set { }  }
}
/// Description: Navigation Data
/// Type: Fast
/// PGNNO: 129284
/// Length: 34
public class navigationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navigationData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double distanceToWaypoint { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 32, false,  0.01); } set { }  }
    public string courseBearingReference { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 2, false,  0); } set { }  }
    public string perpendicularCrossed { get { return _packet.GetBitOffsetLength<string, string>(2, 42, 2, false,  0); } set { }  }
    public string arrivalCircleEntered { get { return _packet.GetBitOffsetLength<string, string>(4, 44, 2, false,  0); } set { }  }
    public string calculationType { get { return _packet.GetBitOffsetLength<string, string>(6, 46, 2, false,  0); } set { }  }
    public int etaTime { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0.0001); } set { }  }
    public int etaDate { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  1); } set { }  }
    public double bearingOriginToDestinationWaypoint { get { return _packet.GetBitOffsetLength<int, double>(0, 96, 16, false,  0.0001); } set { }  }
    public double bearingPositionToDestinationWaypoint { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.0001); } set { }  }
    public int originWaypointNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 128, 32, false,  0); } set { }  }
    public int destinationWaypointNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 160, 32, false,  0); } set { }  }
    public double destinationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 192, 32, true,  1E-07); } set { }  }
    public double destinationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 224, 32, true,  1E-07); } set { }  }
    public double waypointClosingVelocity { get { return _packet.GetBitOffsetLength<int, double>(0, 256, 16, true,  0.01); } set { }  }
}
/// Description: Navigation - Route/WP Information
/// Type: Fast
/// PGNNO: 129285
/// Length: 233
public class navigationRouteWpInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navigationRouteWpInformation(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int navigationDirectionInRoute { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 2, false,  0); } set { }  }
    public int supplementaryRouteWpDataAvailable { get { return _packet.GetBitOffsetLength<int, int>(2, 66, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 68, 4, false,  0); } set { }  }
    public string routeName { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 2040, false,  0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int, int>(0, 2120, 16, false,  0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string, string>(0, 2136, 2040, false,  0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 4176, 32, true,  1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 4208, 32, true,  1E-07); } set { }  }
}
/// Description: Set & Drift, Rapid Update
/// Type: Single
/// PGNNO: 129291
/// Length: 8
public class setDriftRapidUpdate : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public setDriftRapidUpdate(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string setReference { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 10, 6, false,  0); } set { }  }
    public double set { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.0001); } set { }  }
    public double drift { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
}
/// Description: Navigation - Route / Time to+from Mark
/// Type: Fast
/// PGNNO: 129301
/// Length: 10
public class navigationRouteTimeToFromMark : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navigationRouteTimeToFromMark(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double timeToMark { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 32, true,  0.001); } set { }  }
    public string markType { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 44, 4, false,  0); } set { }  }
    public int markId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0); } set { }  }
}
/// Description: Bearing and Distance between two Marks
/// Type: Fast
/// PGNNO: 129302
/// Length: 8
public class bearingAndDistanceBetweenTwoMarks : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bearingAndDistanceBetweenTwoMarks(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string bearingReference { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public string calculationType { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 14, 2, false,  0); } set { }  }
    public double bearingOriginToDestination { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.0001); } set { }  }
    public double distance { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, false,  0.01); } set { }  }
    public string originMarkType { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 4, false,  0); } set { }  }
    public string destinationMarkType { get { return _packet.GetBitOffsetLength<string, string>(4, 68, 4, false,  0); } set { }  }
    public int originMarkId { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 32, false,  0); } set { }  }
    public int destinationMarkId { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 32, false,  0); } set { }  }
}
/// Description: GNSS Control Status
/// Type: Fast
/// PGNNO: 129538
/// Length: 13
public class gnssControlStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssControlStatus(byte[] packet) { _packet = packet; }
    public int svElevationMask { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public double pdopMask { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.01); } set { }  }
    public double pdopSwitch { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public double snrMask { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.01); } set { }  }
    public string gnssModeDesired { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 3, false,  0); } set { }  }
    public string dgnssModeDesired { get { return _packet.GetBitOffsetLength<string, string>(3, 67, 3, false,  0); } set { }  }
    public int positionVelocityFilter { get { return _packet.GetBitOffsetLength<int, int>(6, 70, 2, false,  0); } set { }  }
    public int maxCorrectionAge { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 16, false,  0); } set { }  }
    public double antennaAltitudeFor2dMode { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 16, false,  0.01); } set { }  }
    public string useAntennaAltitudeFor2dMode { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 2, false,  0); } set { }  }
}
/// Description: GNSS DOPs
/// Type: Single
/// PGNNO: 129539
/// Length: 8
public class gnssDops : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssDops(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string desiredMode { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 3, false,  0); } set { }  }
    public string actualMode { get { return _packet.GetBitOffsetLength<string, string>(3, 11, 3, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 14, 2, false,  0); } set { }  }
    public double hdop { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, true,  0.01); } set { }  }
    public double vdop { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.01); } set { }  }
    public double tdop { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, true,  0.01); } set { }  }
}
/// Description: GNSS Sats in View
/// Type: Fast
/// PGNNO: 129540
/// Length: 233
public class gnssSatsInView : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssSatsInView(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string mode { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 10, 6, false,  0); } set { }  }
    public int satsInView { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int prn { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public double elevation { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.0001); } set { }  }
    public double azimuth { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.0001); } set { }  }
    public double snr { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, false,  0.01); } set { }  }
    public int rangeResiduals { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 32, true,  0); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string, string>(0, 112, 4, false,  0); } set { }  }
}
/// Description: GPS Almanac Data
/// Type: Fast
/// PGNNO: 129541
/// Length: 26
public class gpsAlmanacData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gpsAlmanacData(byte[] packet) { _packet = packet; }
    public int prn { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int gpsWeekNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public byte[] svHealthBits { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 24, 8, false,  0); } set { }  }
    public double eccentricity { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  1E-21); } set { }  }
    public double almanacReferenceTime { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 8, false,  1000000000000); } set { }  }
    public double inclinationAngle { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, true,  1E-19); } set { }  }
    public double rateOfRightAscension { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, true,  1E-38); } set { }  }
    public double rootOfSemiMajorAxis { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 24, false,  1E-11); } set { }  }
    public double argumentOfPerigee { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 24, true,  1E-23); } set { }  }
    public double longitudeOfAscensionNode { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 24, true,  1E-23); } set { }  }
    public double meanAnomaly { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 24, true,  1E-23); } set { }  }
    public double clockParameter1 { get { return _packet.GetBitOffsetLength<int, double>(0, 184, 11, true,  1E-20); } set { }  }
    public double clockParameter2 { get { return _packet.GetBitOffsetLength<int, double>(3, 195, 11, true,  1E-38); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 206, 2, false,  0); } set { }  }
}
/// Description: GNSS Pseudorange Noise Statistics
/// Type: Fast
/// PGNNO: 129542
/// Length: 9
public class gnssPseudorangeNoiseStatistics : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssPseudorangeNoiseStatistics(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int rmsOfPositionUncertainty { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int stdOfMajorAxis { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int stdOfMinorAxis { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int orientationOfMajorAxis { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int stdOfLatError { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int stdOfLonError { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int stdOfAltError { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
}
/// Description: GNSS RAIM Output
/// Type: Fast
/// PGNNO: 129545
/// Length: 9
public class gnssRaimOutput : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssRaimOutput(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int integrityFlag { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 12, 4, false,  0); } set { }  }
    public int latitudeExpectedError { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int longitudeExpectedError { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int altitudeExpectedError { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int svIdOfMostLikelyFailedSat { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int probabilityOfMissedDetection { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int estimateOfPseudorangeBias { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int stdDeviationOfBias { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
}
/// Description: GNSS RAIM Settings
/// Type: Single
/// PGNNO: 129546
/// Length: 8
public class gnssRaimSettings : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssRaimSettings(byte[] packet) { _packet = packet; }
    public int radialPositionErrorMaximumThreshold { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int probabilityOfFalseAlarm { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int probabilityOfMissedDetection { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int pseudorangeResidualFilteringTimeConstant { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
}
/// Description: GNSS Pseudorange Error Statistics
/// Type: Fast
/// PGNNO: 129547
/// Length: 9
public class gnssPseudorangeErrorStatistics : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssPseudorangeErrorStatistics(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int rmsStdDevOfRangeInputs { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int stdDevOfMajorErrorEllipse { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int stdDevOfMinorErrorEllipse { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int orientationOfErrorEllipse { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int stdDevLatError { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int stdDevLonError { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int stdDevAltError { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
}
/// Description: DGNSS Corrections
/// Type: Fast
/// PGNNO: 129549
/// Length: 13
public class dgnssCorrections : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public dgnssCorrections(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int referenceStationId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  0); } set { }  }
    public int referenceStationType { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public int timeOfCorrections { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int stationHealth { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public byte[] @reservedBits { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 56, 8, false,  0); } set { }  }
    public int satelliteId { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int prc { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public int rrc { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int udre { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
    public int iod { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 8, false,  0); } set { }  }
}
/// Description: GNSS Differential Correction Receiver Interface
/// Type: Fast
/// PGNNO: 129550
/// Length: 8
public class gnssDifferentialCorrectionReceiverInterface : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssDifferentialCorrectionReceiverInterface(byte[] packet) { _packet = packet; }
    public int channel { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int serialInterfaceBitRate { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int serialInterfaceDetectionMode { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int differentialSource { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int differentialOperationMode { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
}
/// Description: GNSS Differential Correction Receiver Signal
/// Type: Fast
/// PGNNO: 129551
/// Length: 8
public class gnssDifferentialCorrectionReceiverSignal : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public gnssDifferentialCorrectionReceiverSignal(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int channel { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int signalSnr { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int frequency { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int stationType { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int stationId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int differentialSignalBitRate { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int differentialSignalDetectionMode { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int usedAsCorrectionSource { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 80, 8, false,  0); } set { }  }
    public int differentialSource { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
    public int timeSinceLastSatDifferentialSync { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 8, false,  0); } set { }  }
    public int satelliteServiceIdNo { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  0); } set { }  }
}
/// Description: GLONASS Almanac Data
/// Type: Fast
/// PGNNO: 129556
/// Length: 8
public class glonassAlmanacData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public glonassAlmanacData(byte[] packet) { _packet = packet; }
    public int prn { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int na { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int cna { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int hna { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int EpsilonNa { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int DeltatnaDot { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int OmegaNa { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int DeltaTna { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int tna { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int LambdaNa { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public int DeltaIna { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int tca { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
}
/// Description: AIS DGNSS Broadcast Binary Message
/// Type: Fast
/// PGNNO: 129792
/// Length: 8
public class aisDgnssBroadcastBinaryMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisDgnssBroadcastBinaryMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public int repeatIndicator { get { return _packet.GetBitOffsetLength<int, int>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  0); } set { }  }
    public byte[] nmea2000Reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 8, false,  0); } set { }  }
    public int aisTransceiverInformation { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int longitude { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 32, false,  0); } set { }  }
    public int latitude { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 32, false,  0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int, int>(0, 144, 8, false,  0); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 152, 64, false,  0); } set { }  }
}
/// Description: AIS UTC and Date Report
/// Type: Fast
/// PGNNO: 129793
/// Length: 26
public class aisUtcAndDateReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisUtcAndDateReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 106, 6, false,  0); } set { }  }
    public int positionTime { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 32, false,  0.0001); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 144, 19, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(3, 163, 5, false,  0); } set { }  }
    public int positionDate { get { return _packet.GetBitOffsetLength<int, int>(0, 168, 16, false,  1); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string, string>(4, 188, 4, false,  0); } set { }  }
    public byte[] spare { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 192, 8, false,  0); } set { }  }
}
/// Description: AIS Class A Static and Voyage Related Data
/// Type: Fast
/// PGNNO: 129794
/// Length: 24
public class aisClassAStaticAndVoyageRelatedData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassAStaticAndVoyageRelatedData(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public int imoNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 56, false,  0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 128, 160, false,  0); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string, string>(0, 288, 8, false,  0); } set { }  }
    public double length { get { return _packet.GetBitOffsetLength<int, double>(0, 296, 16, false,  0.1); } set { }  }
    public double beam { get { return _packet.GetBitOffsetLength<int, double>(0, 312, 16, false,  0.1); } set { }  }
    public double positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int, double>(0, 328, 16, false,  0.1); } set { }  }
    public double positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int, double>(0, 344, 16, false,  0.1); } set { }  }
    public int etaDate { get { return _packet.GetBitOffsetLength<int, int>(0, 360, 16, false,  1); } set { }  }
    public int etaTime { get { return _packet.GetBitOffsetLength<int, int>(0, 376, 32, false,  0.0001); } set { }  }
    public double draft { get { return _packet.GetBitOffsetLength<int, double>(0, 408, 16, false,  0.01); } set { }  }
    public string destination { get { return _packet.GetBitOffsetLength<string, string>(0, 424, 160, false,  0); } set { }  }
    public string aisVersionIndicator { get { return _packet.GetBitOffsetLength<string, string>(0, 584, 2, false,  0); } set { }  }
    public string gnssType { get { return _packet.GetBitOffsetLength<string, string>(2, 586, 4, false,  0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string, string>(6, 590, 1, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(7, 591, 1, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 592, 5, false,  0); } set { }  }
}
/// Description: AIS Addressed Binary Message
/// Type: Fast
/// PGNNO: 129795
/// Length: 13
public class aisAddressedBinaryMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisAddressedBinaryMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 1, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(1, 41, 5, false,  0); } set { }  }
    public int sequenceNumber { get { return _packet.GetBitOffsetLength<int, int>(6, 46, 2, false,  0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  1); } set { }  }
    public int retransmitFlag { get { return _packet.GetBitOffsetLength<int, int>(6, 86, 1, false,  0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, false,  1); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 104, 64, false,  0); } set { }  }
}
/// Description: AIS Acknowledge
/// Type: Fast
/// PGNNO: 129796
/// Length: 12
public class aisAcknowledge : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisAcknowledge(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 1, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(1, 41, 5, false,  0); } set { }  }
    public int destinationId1 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0); } set { }  }
    public byte[] sequenceNumberForId1 { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 80, 2, false,  0); } set { }  }
    public byte[] sequenceNumberForIdN { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 88, 2, false,  0); } set { }  }
}
/// Description: AIS Binary Broadcast Message
/// Type: Fast
/// PGNNO: 129797
/// Length: 233
public class aisBinaryBroadcastMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisBinaryBroadcastMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 1, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(1, 41, 5, false,  0); } set { }  }
    public int numberOfBitsInBinaryDataField { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public byte[] binaryData { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 64, 2040, false,  0); } set { }  }
}
/// Description: AIS SAR Aircraft Position Report
/// Type: Fast
/// PGNNO: 129798
/// Length: 8
public class aisSarAircraftPositionReport : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisSarAircraftPositionReport(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public double longitude { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, true,  1E-07); } set { }  }
    public double latitude { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 32, true,  1E-07); } set { }  }
    public string positionAccuracy { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 1, false,  0); } set { }  }
    public string raim { get { return _packet.GetBitOffsetLength<string, string>(1, 105, 1, false,  0); } set { }  }
    public string timeStamp { get { return _packet.GetBitOffsetLength<string, string>(2, 106, 6, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, false,  0.1); } set { }  }
    public byte[] communicationState { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 144, 19, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(3, 163, 5, false,  0); } set { }  }
    public double altitude { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 64, true,  1E-06); } set { }  }
    public byte[] @reservedForRegionalApplications { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 232, 8, false,  0); } set { }  }
    public string dte { get { return _packet.GetBitOffsetLength<string, string>(0, 240, 1, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(1, 241, 7, false,  0); } set { }  }
}
/// Description: Radio Frequency/Mode/Power
/// Type: Fast
/// PGNNO: 129799
/// Length: 9
public class radioFrequencyModePower : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public radioFrequencyModePower(byte[] packet) { _packet = packet; }
    public double rxFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, false,  10); } set { }  }
    public double txFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 32, false,  10); } set { }  }
    public int radioChannel { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int txPower { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public int mode { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int channelBandwidth { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
}
/// Description: AIS UTC/Date Inquiry
/// Type: Fast
/// PGNNO: 129800
/// Length: 8
public class aisUtcDateInquiry : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisUtcDateInquiry(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 30, false,  0); } set { }  }
}
/// Description: AIS Addressed Safety Related Message
/// Type: Fast
/// PGNNO: 129801
/// Length: 12
public class aisAddressedSafetyRelatedMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisAddressedSafetyRelatedMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int sequenceNumber { get { return _packet.GetBitOffsetLength<int, int>(5, 45, 2, false,  0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int, int>(7, 47, 30, false,  0); } set { }  }
    public int retransmitFlag { get { return _packet.GetBitOffsetLength<int, int>(7, 79, 1, false,  0); } set { }  }
    public string safetyRelatedText { get { return _packet.GetBitOffsetLength<string, string>(7, 87, 2040, false,  0); } set { }  }
}
/// Description: AIS Safety Related Broadcast Message
/// Type: Fast
/// PGNNO: 129802
/// Length: 8
public class aisSafetyRelatedBroadcastMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisSafetyRelatedBroadcastMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public string safetyRelatedText { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 288, false,  0); } set { }  }
}
/// Description: AIS Interrogation
/// Type: Single
/// PGNNO: 129803
/// Length: 8
public class aisInterrogation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisInterrogation(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 30, false,  1); } set { }  }
    public int messageIdA { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
    public int slotOffsetA { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 14, false,  1); } set { }  }
    public int messageIdB { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  1); } set { }  }
    public int slotOffsetB { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 14, false,  1); } set { }  }
}
/// Description: AIS Assignment Mode Command
/// Type: Fast
/// PGNNO: 129804
/// Length: 23
public class aisAssignmentModeCommand : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisAssignmentModeCommand(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int destinationId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  1); } set { }  }
    public int offset { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  1); } set { }  }
    public int increment { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 16, false,  1); } set { }  }
}
/// Description: AIS Data Link Management Message
/// Type: Fast
/// PGNNO: 129805
/// Length: 8
public class aisDataLinkManagementMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisDataLinkManagementMessage(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int offset { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 10, false,  1); } set { }  }
    public int numberOfSlots { get { return _packet.GetBitOffsetLength<int, int>(2, 58, 8, false,  1); } set { }  }
    public int timeout { get { return _packet.GetBitOffsetLength<int, int>(2, 66, 8, false,  1); } set { }  }
    public int increment { get { return _packet.GetBitOffsetLength<int, int>(2, 74, 8, false,  1); } set { }  }
}
/// Description: AIS Channel Management
/// Type: Fast
/// PGNNO: 129806
/// Length: 8
public class aisChannelManagement : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisChannelManagement(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 5, false,  0); } set { }  }
    public int channelA { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 7, false,  0); } set { }  }
    public int channelB { get { return _packet.GetBitOffsetLength<int, int>(7, 55, 7, false,  0); } set { }  }
    public int power { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int txRxMode { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  1); } set { }  }
    public double northEastLongitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 32, true,  1E-07); } set { }  }
    public double northEastLatitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 32, true,  1E-07); } set { }  }
    public double southWestLongitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 32, true,  1E-07); } set { }  }
    public double southWestLatitudeCorner2 { get { return _packet.GetBitOffsetLength<int, double>(0, 176, 32, true,  1E-07); } set { }  }
    public int addressedOrBroadcastMessageIndicator { get { return _packet.GetBitOffsetLength<int, int>(6, 214, 2, false,  0); } set { }  }
    public int channelABandwidth { get { return _packet.GetBitOffsetLength<int, int>(0, 216, 7, false,  1); } set { }  }
    public int channelBBandwidth { get { return _packet.GetBitOffsetLength<int, int>(7, 223, 7, false,  1); } set { }  }
    public int transitionalZoneSize { get { return _packet.GetBitOffsetLength<int, int>(0, 232, 8, false,  0); } set { }  }
}
/// Description: AIS Class B Group Assignment
/// Type: Fast
/// PGNNO: 129807
/// Length: 8
public class aisClassBGroupAssignment : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassBGroupAssignment(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 30, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 38, 2, false,  0); } set { }  }
    public int txRxMode { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 2, false,  1); } set { }  }
    public double northEastLongitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 32, true,  1E-07); } set { }  }
    public double northEastLatitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 32, true,  1E-07); } set { }  }
    public double southWestLongitudeCorner1 { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 32, true,  1E-07); } set { }  }
    public double southWestLatitudeCorner2 { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 32, true,  1E-07); } set { }  }
    public int stationType { get { return _packet.GetBitOffsetLength<int, int>(0, 176, 6, false,  0); } set { }  }
    public int shipAndCargoFilter { get { return _packet.GetBitOffsetLength<int, int>(0, 184, 6, false,  0); } set { }  }
    public int reportingInterval { get { return _packet.GetBitOffsetLength<int, int>(0, 192, 16, false,  0); } set { }  }
    public int quietTime { get { return _packet.GetBitOffsetLength<int, int>(0, 208, 16, false,  0); } set { }  }
}
/// Description: DSC Distress Call Information
/// Type: Fast
/// PGNNO: 129808
/// Length: 8
public class dscDistressCallInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public dscDistressCallInformation(byte[] packet) { _packet = packet; }
    public string dscFormat { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string dscCategory { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 8, false,  0); } set { }  }
    public decimal dscMessageAddress { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 16, 40, false,  0); } set { }  }
    public string natureOfDistress { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 8, false,  0); } set { }  }
    public string subsequentCommunicationModeOr2ndTelecommand { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 8, false,  0); } set { }  }
    public string proposedRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 48, false,  0); } set { }  }
    public string proposedTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 120, 48, false,  0); } set { }  }
    public string telephoneNumber { get { return _packet.GetBitOffsetLength<string, string>(0, 168, 16, false,  0); } set { }  }
    public double latitudeOfVesselReported { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  1E-07); } set { }  }
    public double longitudeOfVesselReported { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  1E-07); } set { }  }
    public int timeOfPosition { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0.0001); } set { }  }
    public decimal mmsiOfShipInDistress { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 0, 40, false,  0); } set { }  }
    public int dscEosSymbol { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string expansionEnabled { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 0, 6, false,  0); } set { }  }
    public string callingRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 48, false,  0); } set { }  }
    public string callingTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 48, false,  0); } set { }  }
    public int timeOfReceipt { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0.0001); } set { }  }
    public int dateOfReceipt { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int dscEquipmentAssignedMessageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public string dscExpansionFieldSymbol { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string dscExpansionFieldData { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
}
/// Description: DSC Call Information
/// Type: Fast
/// PGNNO: 129808
/// Length: 8
public class dscCallInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public dscCallInformation(byte[] packet) { _packet = packet; }
    public string dscFormatSymbol { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string dscCategorySymbol { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 8, false,  0); } set { }  }
    public decimal dscMessageAddress { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 16, 40, false,  0); } set { }  }
    public string _1stTelecommand { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 8, false,  0); } set { }  }
    public string subsequentCommunicationModeOr2ndTelecommand { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 8, false,  0); } set { }  }
    public string proposedRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 48, false,  0); } set { }  }
    public string proposedTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 120, 48, false,  0); } set { }  }
    public string telephoneNumber { get { return _packet.GetBitOffsetLength<string, string>(0, 168, 16, false,  0); } set { }  }
    public double latitudeOfVesselReported { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  1E-07); } set { }  }
    public double longitudeOfVesselReported { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, true,  1E-07); } set { }  }
    public int timeOfPosition { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0.0001); } set { }  }
    public decimal mmsiOfShipInDistress { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 0, 40, false,  0); } set { }  }
    public int dscEosSymbol { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public string expansionEnabled { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(2, 0, 6, false,  0); } set { }  }
    public string callingRxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 48, false,  0); } set { }  }
    public string callingTxFrequencyChannel { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 48, false,  0); } set { }  }
    public int timeOfReceipt { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 32, false,  0.0001); } set { }  }
    public int dateOfReceipt { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int dscEquipmentAssignedMessageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  0); } set { }  }
    public string dscExpansionFieldSymbol { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string dscExpansionFieldData { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
}
/// Description: AIS Class B static data (msg 24 Part A)
/// Type: Fast
/// PGNNO: 129809
/// Length: 27
public class aisClassBStaticDataMsg24PartA : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassBStaticDataMsg24PartA(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 160, false,  0); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 5, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(5, 205, 3, false,  0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int, int>(0, 208, 8, false,  1); } set { }  }
}
/// Description: AIS Class B static data (msg 24 Part B)
/// Type: Fast
/// PGNNO: 129810
/// Length: 34
public class aisClassBStaticDataMsg24PartB : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public aisClassBStaticDataMsg24PartB(byte[] packet) { _packet = packet; }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 32, false,  1); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public string vendorId { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 56, false,  0); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 56, false,  0); } set { }  }
    public double length { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 16, false,  0.1); } set { }  }
    public double beam { get { return _packet.GetBitOffsetLength<int, double>(0, 176, 16, false,  0.1); } set { }  }
    public double positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int, double>(0, 192, 16, false,  0.1); } set { }  }
    public double positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int, double>(0, 208, 16, false,  0.1); } set { }  }
    public int mothershipUserId { get { return _packet.GetBitOffsetLength<int, int>(0, 224, 32, false,  1); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 256, 2, false,  0); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int, int>(2, 258, 6, false,  1); } set { }  }
    public string aisTransceiverInformation { get { return _packet.GetBitOffsetLength<string, string>(0, 264, 5, false,  0); } set { }  }
    public int sequenceId { get { return _packet.GetBitOffsetLength<int, int>(0, 272, 8, false,  1); } set { }  }
}
/// Description: Label
/// Type: Fast
/// PGNNO: 130060
/// Length: 0
public class label : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public label(byte[] packet) { _packet = packet; }
}
/// Description: Channel Source Configuration
/// Type: Fast
/// PGNNO: 130061
/// Length: 0
public class channelSourceConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public channelSourceConfiguration(byte[] packet) { _packet = packet; }
}
/// Description: Route and WP Service - Database List
/// Type: Fast
/// PGNNO: 130064
/// Length: 8
public class routeAndWpServiceDatabaseList : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceDatabaseList(byte[] packet) { _packet = packet; }
    public int startDatabaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfDatabasesAvailable { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string databaseName { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 64, false,  0); } set { }  }
    public int databaseTimestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 32, false,  0.0001); } set { }  }
    public int databaseDatestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 128, 16, false,  1); } set { }  }
    public int wpPositionResolution { get { return _packet.GetBitOffsetLength<int, int>(0, 144, 6, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 150, 2, false,  0); } set { }  }
    public int numberOfRoutesInDatabase { get { return _packet.GetBitOffsetLength<int, int>(0, 152, 16, false,  0); } set { }  }
    public int numberOfWpsInDatabase { get { return _packet.GetBitOffsetLength<int, int>(0, 168, 16, false,  0); } set { }  }
    public int numberOfBytesInDatabase { get { return _packet.GetBitOffsetLength<int, int>(0, 184, 16, false,  0); } set { }  }
}
/// Description: Route and WP Service - Route List
/// Type: Fast
/// PGNNO: 130065
/// Length: 8
public class routeAndWpServiceRouteList : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRouteList(byte[] packet) { _packet = packet; }
    public int startRouteId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfRoutesInDatabase { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public string routeName { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 64, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 104, 4, false,  0); } set { }  }
    public int wpIdentificationMethod { get { return _packet.GetBitOffsetLength<int, int>(4, 108, 2, false,  0); } set { }  }
    public int routeStatus { get { return _packet.GetBitOffsetLength<int, int>(6, 110, 2, false,  0); } set { }  }
}
/// Description: Route and WP Service - Route/WP-List Attributes
/// Type: Fast
/// PGNNO: 130066
/// Length: 8
public class routeAndWpServiceRouteWpListAttributes : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRouteWpListAttributes(byte[] packet) { _packet = packet; }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string routeWpListName { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 64, false,  0); } set { }  }
    public int routeWpListTimestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 32, false,  0.0001); } set { }  }
    public int routeWpListDatestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 16, false,  1); } set { }  }
    public int changeAtLastTimestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 128, 8, false,  0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int, int>(0, 136, 16, false,  0); } set { }  }
    public int criticalSupplementaryParameters { get { return _packet.GetBitOffsetLength<int, int>(0, 152, 8, false,  0); } set { }  }
    public int navigationMethod { get { return _packet.GetBitOffsetLength<int, int>(0, 160, 2, false,  0); } set { }  }
    public int wpIdentificationMethod { get { return _packet.GetBitOffsetLength<int, int>(2, 162, 2, false,  0); } set { }  }
    public int routeStatus { get { return _packet.GetBitOffsetLength<int, int>(4, 164, 2, false,  0); } set { }  }
    public int xteLimitForTheRoute { get { return _packet.GetBitOffsetLength<int, int>(6, 166, 16, false,  0); } set { }  }
}
/// Description: Route and WP Service - Route - WP Name & Position
/// Type: Fast
/// PGNNO: 130067
/// Length: 8
public class routeAndWpServiceRouteWpNamePosition : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRouteWpNamePosition(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 64, false,  0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 32, true,  1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 32, true,  1E-07); } set { }  }
}
/// Description: Route and WP Service - Route - WP Name
/// Type: Fast
/// PGNNO: 130068
/// Length: 8
public class routeAndWpServiceRouteWpName : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRouteWpName(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfWpsInTheRouteWpList { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 64, false,  0); } set { }  }
}
/// Description: Route and WP Service - XTE Limit & Navigation Method
/// Type: Fast
/// PGNNO: 130069
/// Length: 8
public class routeAndWpServiceXteLimitNavigationMethod : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceXteLimitNavigationMethod(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfWpsWithASpecificXteLimitOrNavMethod { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int rps { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int xteLimitInTheLegAfterWp { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  0); } set { }  }
    public int navMethodInTheLegAfterWp { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 76, 4, false,  0); } set { }  }
}
/// Description: Route and WP Service - WP Comment
/// Type: Fast
/// PGNNO: 130070
/// Length: 8
public class routeAndWpServiceWpComment : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceWpComment(byte[] packet) { _packet = packet; }
    public int startId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfWpsWithComments { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int wpIdRps { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 64, false,  0); } set { }  }
}
/// Description: Route and WP Service - Route Comment
/// Type: Fast
/// PGNNO: 130071
/// Length: 8
public class routeAndWpServiceRouteComment : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRouteComment(byte[] packet) { _packet = packet; }
    public int startRouteId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfRoutesWithComments { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 64, false,  0); } set { }  }
}
/// Description: Route and WP Service - Database Comment
/// Type: Fast
/// PGNNO: 130072
/// Length: 8
public class routeAndWpServiceDatabaseComment : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceDatabaseComment(byte[] packet) { _packet = packet; }
    public int startDatabaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfDatabasesWithComments { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public string comment { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 64, false,  0); } set { }  }
}
/// Description: Route and WP Service - Radius of Turn
/// Type: Fast
/// PGNNO: 130073
/// Length: 8
public class routeAndWpServiceRadiusOfTurn : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceRadiusOfTurn(byte[] packet) { _packet = packet; }
    public int startRps { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfWpsWithASpecificRadiusOfTurn { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int routeId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int rps { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int radiusOfTurn { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  0); } set { }  }
}
/// Description: Route and WP Service - WP List - WP Name & Position
/// Type: Fast
/// PGNNO: 130074
/// Length: 8
public class routeAndWpServiceWpListWpNamePosition : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public routeAndWpServiceWpListWpNamePosition(byte[] packet) { _packet = packet; }
    public int startWpId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int nitems { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public int numberOfValidWpsInTheWpList { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int databaseId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 40, 8, false,  0); } set { }  }
    public int wpId { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public string wpName { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 64, false,  0); } set { }  }
    public double wpLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 32, true,  1E-07); } set { }  }
    public double wpLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 32, true,  1E-07); } set { }  }
}
/// Description: Wind Data
/// Type: Single
/// PGNNO: 130306
/// Length: 8
public class windData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public windData(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double windSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.01); } set { }  }
    public double windAngle { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.0001); } set { }  }
    public string reference { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 3, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(3, 43, 21, false,  0); } set { }  }
}
/// Description: Environmental Parameters
/// Type: Single
/// PGNNO: 130310
/// Length: 8
public class environmentalParameters : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public environmentalParameters(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 8, 16, false,  0.01); } set { }  }
    public double outsideAmbientAirTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Temperature
/// Type: Single
/// PGNNO: 130312
/// Length: 8
public class temperature : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public temperature(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
}
/// Description: Humidity
/// Type: Single
/// PGNNO: 130313
/// Length: 8
public class humidity : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public humidity(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double actualHumidity { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, true,  0.004); } set { }  }
    public double setHumidity { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, true,  0.004); } set { }  }
}
/// Description: Actual Pressure
/// Type: Single
/// PGNNO: 130314
/// Length: 8
public class actualPressure : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public actualPressure(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double pressure { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 32, true,  0.1); } set { }  }
}
/// Description: Set Pressure
/// Type: Single
/// PGNNO: 130315
/// Length: 8
public class setPressure : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public setPressure(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double pressure { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 32, false,  0.1); } set { }  }
}
/// Description: Temperature Extended Range
/// Type: Single
/// PGNNO: 130316
/// Length: 8
public class temperatureExtendedRange : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public temperatureExtendedRange(byte[] packet) { _packet = packet; }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public double temperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 24, false,  0.001); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.1); } set { }  }
}
/// Description: Tide Station Data
/// Type: Fast
/// PGNNO: 130320
/// Length: 20
public class tideStationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public tideStationData(byte[] packet) { _packet = packet; }
    public string mode { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public string tideTendency { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 6, 2, false,  0); } set { }  }
    public int measurementDate { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int measurementTime { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 32, true,  1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 32, true,  1E-07); } set { }  }
    public double tideLevel { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 16, true,  0.001); } set { }  }
    public double tideLevelStandardDeviation { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 16, false,  0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string, string>(0, 152, 16, false,  0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string, string>(0, 168, 16, false,  0); } set { }  }
}
/// Description: Salinity Station Data
/// Type: Fast
/// PGNNO: 130321
/// Length: 22
public class salinityStationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public salinityStationData(byte[] packet) { _packet = packet; }
    public string mode { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 4, 4, false,  0); } set { }  }
    public int measurementDate { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int measurementTime { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 32, true,  1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 32, true,  1E-07); } set { }  }
    public float salinity { get { return _packet.GetBitOffsetLength<float, float>(0, 120, 32, true,  0); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 16, false,  0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string, string>(0, 168, 16, false,  0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string, string>(0, 184, 16, false,  0); } set { }  }
}
/// Description: Current Station Data
/// Type: Fast
/// PGNNO: 130322
/// Length: 8
public class currentStationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public currentStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 4, 4, false,  0); } set { }  }
    public int measurementDate { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int measurementTime { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 32, true,  1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 32, true,  1E-07); } set { }  }
    public double measurementDepth { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 32, false,  0.01); } set { }  }
    public double currentSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 16, false,  0.01); } set { }  }
    public double currentFlowDirection { get { return _packet.GetBitOffsetLength<int, double>(0, 168, 16, false,  0.0001); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 184, 16, false,  0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string, string>(0, 200, 16, false,  0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string, string>(0, 216, 16, false,  0); } set { }  }
}
/// Description: Meteorological Station Data
/// Type: Fast
/// PGNNO: 130323
/// Length: 30
public class meteorologicalStationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public meteorologicalStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 4, 4, false,  0); } set { }  }
    public int measurementDate { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int measurementTime { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 32, true,  1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 32, true,  1E-07); } set { }  }
    public double windSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 16, false,  0.01); } set { }  }
    public double windDirection { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 16, false,  0.0001); } set { }  }
    public string windReference { get { return _packet.GetBitOffsetLength<string, string>(0, 152, 3, false,  0); } set { }  }
    public double windGusts { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 16, false,  0.01); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 176, 16, false,  0); } set { }  }
    public double ambientTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 192, 16, false,  0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string, string>(0, 208, 2056, false,  0); } set { }  }
    public string stationName { get { return _packet.GetBitOffsetLength<string, string>(0, 2264, 2056, false,  0); } set { }  }
}
/// Description: Moored Buoy Station Data
/// Type: Fast
/// PGNNO: 130324
/// Length: 8
public class mooredBuoyStationData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public mooredBuoyStationData(byte[] packet) { _packet = packet; }
    public int mode { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 4, 4, false,  0); } set { }  }
    public int measurementDate { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 16, false,  1); } set { }  }
    public int measurementTime { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0.0001); } set { }  }
    public double stationLatitude { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 32, true,  1E-07); } set { }  }
    public double stationLongitude { get { return _packet.GetBitOffsetLength<int, double>(0, 88, 32, true,  1E-07); } set { }  }
    public double windSpeed { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 16, false,  0.01); } set { }  }
    public double windDirection { get { return _packet.GetBitOffsetLength<int, double>(0, 136, 16, false,  0.0001); } set { }  }
    public string windReference { get { return _packet.GetBitOffsetLength<string, string>(0, 152, 3, false,  0); } set { }  }
    public double windGusts { get { return _packet.GetBitOffsetLength<int, double>(0, 160, 16, false,  0.01); } set { }  }
    public int waveHeight { get { return _packet.GetBitOffsetLength<int, int>(0, 176, 16, false,  0); } set { }  }
    public int dominantWavePeriod { get { return _packet.GetBitOffsetLength<int, int>(0, 192, 16, false,  0); } set { }  }
    public double atmosphericPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 208, 16, false,  0); } set { }  }
    public int pressureTendencyRate { get { return _packet.GetBitOffsetLength<int, int>(0, 224, 16, false,  0); } set { }  }
    public double airTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 240, 16, false,  0.01); } set { }  }
    public double waterTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 256, 16, false,  0.01); } set { }  }
    public string stationId { get { return _packet.GetBitOffsetLength<string, string>(0, 272, 64, false,  0); } set { }  }
}
/// Description: Payload Mass
/// Type: Fast
/// PGNNO: 130560
/// Length: 0
public class payloadMass : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public payloadMass(byte[] packet) { _packet = packet; }
}
/// Description: Watermaker Input Setting and Status
/// Type: Fast
/// PGNNO: 130567
/// Length: 24
public class watermakerInputSettingAndStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public watermakerInputSettingAndStatus(byte[] packet) { _packet = packet; }
    public string watermakerOperatingState { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 6, false,  0); } set { }  }
    public string productionStartStop { get { return _packet.GetBitOffsetLength<string, string>(6, 6, 2, false,  0); } set { }  }
    public string rinseStartStop { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 2, false,  0); } set { }  }
    public string lowPressurePumpStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 10, 2, false,  0); } set { }  }
    public string highPressurePumpStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public string emergencyStop { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public string productSolenoidValveStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string flushModeStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public string salinityStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 20, 2, false,  0); } set { }  }
    public string sensorStatus { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public string oilChangeIndicatorStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 2, false,  0); } set { }  }
    public string filterStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 26, 2, false,  0); } set { }  }
    public string systemStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 28, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 30, 2, false,  0); } set { }  }
    public int salinity { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  1); } set { }  }
    public double productWaterTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.01); } set { }  }
    public double preFilterPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, false,  0); } set { }  }
    public double postFilterPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, false,  0); } set { }  }
    public double feedPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 96, 16, true,  0); } set { }  }
    public double systemHighPressure { get { return _packet.GetBitOffsetLength<int, double>(0, 112, 16, false,  0); } set { }  }
    public double productWaterFlow { get { return _packet.GetBitOffsetLength<int, double>(0, 128, 16, true,  0.1); } set { }  }
    public double brineWaterFlow { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 16, true,  0.1); } set { }  }
    public int runTime { get { return _packet.GetBitOffsetLength<int, int>(0, 160, 32, false,  1); } set { }  }
}
/// Description: Current Status and File
/// Type: Fast
/// PGNNO: 130569
/// Length: 233
public class currentStatusAndFile : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public currentStatusAndFile(byte[] packet) { _packet = packet; }
    public string zone { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  1); } set { }  }
    public string playStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 8, false,  0); } set { }  }
    public int elapsedTrackTime { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0.0001); } set { }  }
    public int trackTime { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  0.0001); } set { }  }
    public string repeatStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 96, 4, false,  0); } set { }  }
    public string shuffleStatus { get { return _packet.GetBitOffsetLength<string, string>(4, 100, 4, false,  0); } set { }  }
    public int saveFavoriteNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  1); } set { }  }
    public int playFavoriteNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 16, false,  1); } set { }  }
    public int thumbsUpDown { get { return _packet.GetBitOffsetLength<int, int>(0, 128, 8, false,  1); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int, int>(0, 136, 8, false,  1); } set { }  }
    public double radioFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 32, false,  10); } set { }  }
    public int hdFrequencyMulticast { get { return _packet.GetBitOffsetLength<int, int>(0, 176, 8, false,  1); } set { }  }
    public int deleteFavoriteNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 184, 8, false,  1); } set { }  }
    public int totalNumberOfTracks { get { return _packet.GetBitOffsetLength<int, int>(0, 192, 16, false,  1); } set { }  }
}
/// Description: Library Data File
/// Type: Fast
/// PGNNO: 130570
/// Length: 233
public class libraryDataFile : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public libraryDataFile(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 32, false,  1); } set { }  }
    public string type { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 8, false,  0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 16, false,  0); } set { }  }
    public int track { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int station { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int favorite { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public double radioFrequency { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 32, false,  10); } set { }  }
    public int hdFrequency { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string inPlayQueue { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public string lockStatus { get { return _packet.GetBitOffsetLength<string, string>(2, 0, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 0, 4, false,  0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
    public string album { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
}
/// Description: Library Data Group
/// Type: Fast
/// PGNNO: 130571
/// Length: 233
public class libraryDataGroup : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public libraryDataGroup(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public int groupId { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  1); } set { }  }
    public int idOffset { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  1); } set { }  }
    public int idCount { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 16, false,  1); } set { }  }
    public int totalIdCount { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, false,  1); } set { }  }
    public string idType { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 8, false,  0); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 32, false,  1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 144, 16, false,  0); } set { }  }
}
/// Description: Library Data Search
/// Type: Fast
/// PGNNO: 130572
/// Length: 233
public class libraryDataSearch : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public libraryDataSearch(byte[] packet) { _packet = packet; }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int groupId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 32, false,  1); } set { }  }
    public string groupType1 { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 8, false,  0); } set { }  }
    public string groupName1 { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 16, false,  0); } set { }  }
    public string groupType2 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string groupName2 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
    public string groupType3 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public string groupName3 { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 16, false,  0); } set { }  }
}
/// Description: Supported Source Data
/// Type: Fast
/// PGNNO: 130573
/// Length: 233
public class supportedSourceData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public supportedSourceData(byte[] packet) { _packet = packet; }
    public int idOffset { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 16, false,  1); } set { }  }
    public int idCount { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public int totalIdCount { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  1); } set { }  }
    public int id { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 16, false,  0); } set { }  }
    public long playSupport { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 16, false,  0); } set { }  }
    public long browseSupport { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 16, false,  0); } set { }  }
    public string thumbsSupport { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public string connected { get { return _packet.GetBitOffsetLength<string, string>(2, 0, 2, false,  0); } set { }  }
    public long repeatSupport { get { return _packet.GetBitOffsetLength<long, long>(4, 0, 2, false,  0); } set { }  }
    public long shuffleSupport { get { return _packet.GetBitOffsetLength<long, long>(6, 0, 2, false,  0); } set { }  }
}
/// Description: Supported Zone Data
/// Type: Fast
/// PGNNO: 130574
/// Length: 233
public class supportedZoneData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public supportedZoneData(byte[] packet) { _packet = packet; }
    public int firstZoneId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int zoneCount { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int totalZoneCount { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string zoneId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 16, false,  0); } set { }  }
}
/// Description: Small Craft Status
/// Type: Single
/// PGNNO: 130576
/// Length: 2
public class smallCraftStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public smallCraftStatus(byte[] packet) { _packet = packet; }
    public int portTrimTab { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, true,  0); } set { }  }
    public int starboardTrimTab { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, true,  0); } set { }  }
}
/// Description: Direction Data
/// Type: Fast
/// PGNNO: 130577
/// Length: 14
public class directionData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public directionData(byte[] packet) { _packet = packet; }
    public string dataMode { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 4, false,  0); } set { }  }
    public string cogReference { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(6, 6, 2, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  0); } set { }  }
    public double cog { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, false,  0.0001); } set { }  }
    public double sog { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, false,  0.01); } set { }  }
    public double heading { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, false,  0.0001); } set { }  }
    public double speedThroughWater { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, false,  0.01); } set { }  }
    public double set { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, false,  0.0001); } set { }  }
    public double drift { get { return _packet.GetBitOffsetLength<int, double>(0, 96, 16, false,  0.01); } set { }  }
}
/// Description: Vessel Speed Components
/// Type: Fast
/// PGNNO: 130578
/// Length: 12
public class vesselSpeedComponents : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public vesselSpeedComponents(byte[] packet) { _packet = packet; }
    public double longitudinalSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 0, 16, true,  0.001); } set { }  }
    public double transverseSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 16, 16, true,  0.001); } set { }  }
    public double longitudinalSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.001); } set { }  }
    public double transverseSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, true,  0.001); } set { }  }
    public double sternSpeedWaterReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, true,  0.001); } set { }  }
    public double sternSpeedGroundReferenced { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, true,  0.001); } set { }  }
}
/// Description: System Configuration
/// Type: Fast
/// PGNNO: 130579
/// Length: 8
public class systemConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public systemConfiguration(byte[] packet) { _packet = packet; }
    public string power { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public string defaultSettings { get { return _packet.GetBitOffsetLength<string, string>(2, 2, 2, false,  0); } set { }  }
    public string tunerRegions { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public int maxFavorites { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public long videoProtocols { get { return _packet.GetBitOffsetLength<long, long>(0, 16, 4, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 20, 44, false,  0); } set { }  }
}
/// Description: System Configuration (deprecated)
/// Type: Fast
/// PGNNO: 130580
/// Length: 2
public class systemConfigurationDeprecated : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public systemConfigurationDeprecated(byte[] packet) { _packet = packet; }
    public string power { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 2, false,  0); } set { }  }
    public string defaultSettings { get { return _packet.GetBitOffsetLength<string, string>(2, 2, 2, false,  0); } set { }  }
    public string tunerRegions { get { return _packet.GetBitOffsetLength<string, string>(4, 4, 4, false,  0); } set { }  }
    public int maxFavorites { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
}
/// Description: Zone Configuration (deprecated)
/// Type: Fast
/// PGNNO: 130581
/// Length: 14
public class zoneConfigurationDeprecated : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public zoneConfigurationDeprecated(byte[] packet) { _packet = packet; }
    public int firstZoneId { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int zoneCount { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int totalZoneCount { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string zoneId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string zoneName { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 16, false,  0); } set { }  }
}
/// Description: Zone Volume
/// Type: Fast
/// PGNNO: 130582
/// Length: 4
public class zoneVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public zoneVolume(byte[] packet) { _packet = packet; }
    public string zoneId { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int volume { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public string volumeChange { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 2, false,  0); } set { }  }
    public string mute { get { return _packet.GetBitOffsetLength<string, string>(2, 18, 2, false,  0); } set { }  }
    public byte[] @reserved { get { return _packet.GetBitOffsetLength<byte[], byte[]>(4, 20, 4, false,  0); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
}
/// Description: Available Audio EQ presets
/// Type: Fast
/// PGNNO: 130583
/// Length: 233
public class availableAudioEqPresets : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public availableAudioEqPresets(byte[] packet) { _packet = packet; }
    public int firstPreset { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int presetCount { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int totalPresetCount { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public string presetType { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string presetName { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 16, false,  0); } set { }  }
}
/// Description: Available Bluetooth addresses
/// Type: Fast
/// PGNNO: 130584
/// Length: 233
public class availableBluetoothAddresses : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public availableBluetoothAddresses(byte[] packet) { _packet = packet; }
    public int firstAddress { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public int addressCount { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int totalAddressCount { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  1); } set { }  }
    public int bluetoothAddress { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 48, false,  1); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 8, false,  0); } set { }  }
    public string deviceName { get { return _packet.GetBitOffsetLength<string, string>(0, 80, 16, false,  0); } set { }  }
    public int signalStrength { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
}
/// Description: Bluetooth source status
/// Type: Fast
/// PGNNO: 130585
/// Length: 233
public class bluetoothSourceStatus : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bluetoothSourceStatus(byte[] packet) { _packet = packet; }
    public int sourceNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 0, 8, false,  1); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string, string>(0, 8, 4, false,  0); } set { }  }
    public string forgetDevice { get { return _packet.GetBitOffsetLength<string, string>(4, 12, 2, false,  0); } set { }  }
    public string discovering { get { return _packet.GetBitOffsetLength<string, string>(6, 14, 2, false,  0); } set { }  }
    public int bluetoothAddress { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 48, false,  1); } set { }  }
}
/// Description: Zone Configuration
/// Type: Fast
/// PGNNO: 130586
/// Length: 14
public class zoneConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public zoneConfiguration(byte[] packet) { _packet = packet; }
    public string zoneId { get { return _packet.GetBitOffsetLength<string, string>(0, 0, 8, false,  0); } set { }  }
    public int volumeLimit { get { return _packet.GetBitOffsetLength<int, int>(0, 8, 8, false,  1); } set { }  }
    public int fade { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, true,  1); } set { }  }
    public int balance { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, true,  1); } set { }  }
    public int subVolume { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, true,  1); } set { }  }
    public int eqTreble { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, true,  1); } set { }  }
    public int eqMidRange { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, true,  1); } set { }  }
    public int eqBass { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, true,  1); } set { }  }
    public string presetType { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 8, false,  0); } set { }  }
    public string audioFilter { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 8, false,  0); } set { }  }
    public int highPassFilterFrequency { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  1); } set { }  }
    public int lowPassFilterFrequency { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 16, false,  1); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string, string>(0, 112, 8, false,  0); } set { }  }
}
/// Description: SonicHub: Init #2
/// Type: Fast
/// PGNNO: 130816
/// Length: 9
public class sonichubInit2 : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubInit2(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 16, false,  1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  1); } set { }  }
}
/// Description: SonicHub: AM Radio
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubAmRadio : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubAmRadio(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public double frequency { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 32, false,  0.001); } set { }  }
    public int noiseLevel { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 2, false,  0); } set { }  }
    public int signalLevel { get { return _packet.GetBitOffsetLength<int, int>(2, 82, 4, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 88, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Zone info
/// Type: Fast
/// PGNNO: 130816
/// Length: 6
public class sonichubZoneInfo : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubZoneInfo(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int zone { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
}
/// Description: SonicHub: Source
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubSource : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubSource(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
}
/// Description: SonicHub: Source List
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubSourceList : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubSourceList(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Control
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
}
/// Description: SonicHub: Unknown
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubUnknown : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
}
/// Description: SonicHub: FM Radio
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubFmRadio : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubFmRadio(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public double frequency { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 32, false,  0.001); } set { }  }
    public int noiseLevel { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 2, false,  0); } set { }  }
    public int signalLevel { get { return _packet.GetBitOffsetLength<int, int>(2, 82, 4, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 88, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Playlist
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubPlaylist : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubPlaylist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string item { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
    public int currentTrack { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 32, false,  1); } set { }  }
    public int tracks { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 32, false,  1); } set { }  }
    public double length { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 32, false,  0.001); } set { }  }
    public double positionInTrack { get { return _packet.GetBitOffsetLength<int, double>(0, 152, 32, false,  0.001); } set { }  }
}
/// Description: SonicHub: Track
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubTrack : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubTrack(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Artist
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubArtist : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Album
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubAlbum : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubAlbum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Menu Item
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubMenuItem : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubMenuItem(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int item { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 96, 256, false,  0); } set { }  }
}
/// Description: SonicHub: Zones
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubZones : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubZones(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int zones { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
}
/// Description: SonicHub: Max Volume
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubMaxVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubMaxVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public int level { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
}
/// Description: SonicHub: Volume
/// Type: Fast
/// PGNNO: 130816
/// Length: 8
public class sonichubVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public string zone { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
    public int level { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
}
/// Description: SonicHub: Init #1
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubInit1 : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubInit1(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
}
/// Description: SonicHub: Position
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class sonichubPosition : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubPosition(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public double position { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 32, false,  0.001); } set { }  }
}
/// Description: SonicHub: Init #3
/// Type: Fast
/// PGNNO: 130816
/// Length: 9
public class sonichubInit3 : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public sonichubInit3(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  1); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  1); } set { }  }
}
/// Description: Simrad: Text Message
/// Type: Fast
/// PGNNO: 130816
/// Length: 64
public class simradTextMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simradTextMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 24, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int prio { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 256, false,  0); } set { }  }
}
/// Description: Manufacturer Proprietary fast-packet non-addressed
/// Type: Fast
/// PGNNO: 130816
/// Length: 223
public class manufacturerProprietaryFastPacketNonAddressed : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public manufacturerProprietaryFastPacketNonAddressed(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 16, 1768, false,  0); } set { }  }
}
/// Description: Navico: Product Information
/// Type: Fast
/// PGNNO: 130817
/// Length: 14
public class navicoProductInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public navicoProductInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public string model { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 256, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 288, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 296, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 304, 8, false,  0); } set { }  }
    public string firmwareVersion { get { return _packet.GetBitOffsetLength<string, string>(0, 312, 80, false,  0); } set { }  }
    public string firmwareDate { get { return _packet.GetBitOffsetLength<string, string>(0, 392, 256, false,  0); } set { }  }
    public string firmwareTime { get { return _packet.GetBitOffsetLength<string, string>(0, 648, 256, false,  0); } set { }  }
}
/// Description: Simnet: Reprogram Data
/// Type: Fast
/// PGNNO: 130818
/// Length: 223
public class simnetReprogramData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetReprogramData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int version { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  1); } set { }  }
    public int sequence { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  1); } set { }  }
    public byte[] data { get { return _packet.GetBitOffsetLength<byte[], byte[]>(0, 48, 1736, false,  0); } set { }  }
}
/// Description: Simnet: Request Reprogram
/// Type: Fast
/// PGNNO: 130819
/// Length: 8
public class simnetRequestReprogram : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetRequestReprogram(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Furuno: Unknown
/// Type: Fast
/// PGNNO: 130820
/// Length: 8
public class furunoUnknown : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
}
/// Description: Fusion: Source Name
/// Type: Fast
/// PGNNO: 130820
/// Length: 13
public class fusionSourceName : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSourceName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int sourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int currentSourceId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 40, false,  0); } set { }  }
}
/// Description: Fusion: Track Info
/// Type: Fast
/// PGNNO: 130820
/// Length: 23
public class fusionTrackInfo : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionTrackInfo(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public string transport { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 4, false,  0); } set { }  }
    public int x { get { return _packet.GetBitOffsetLength<int, int>(4, 44, 4, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int track { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 16, false,  0); } set { }  }
    public int trackCount { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 16, false,  0); } set { }  }
    public double trackLength { get { return _packet.GetBitOffsetLength<int, double>(0, 120, 24, false,  0.001); } set { }  }
    public double g { get { return _packet.GetBitOffsetLength<int, double>(0, 144, 24, false,  0.001); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 168, 16, false,  0); } set { }  }
}
/// Description: Fusion: Track
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionTrack : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionTrack(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public long b { get { return _packet.GetBitOffsetLength<int, long>(0, 32, 40, false,  0); } set { }  }
    public string track { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 80, false,  0); } set { }  }
}
/// Description: Fusion: Artist
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionArtist : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public long b { get { return _packet.GetBitOffsetLength<int, long>(0, 32, 40, false,  0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 80, false,  0); } set { }  }
}
/// Description: Fusion: Album
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionAlbum : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionAlbum(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public long b { get { return _packet.GetBitOffsetLength<int, long>(0, 32, 40, false,  0); } set { }  }
    public string album { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 80, false,  0); } set { }  }
}
/// Description: Fusion: Unit Name
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionUnitName : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionUnitName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 112, false,  0); } set { }  }
}
/// Description: Fusion: Zone Name
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionZoneName : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionZoneName(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int number { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 104, false,  0); } set { }  }
}
/// Description: Fusion: Play Progress
/// Type: Fast
/// PGNNO: 130820
/// Length: 9
public class fusionPlayProgress : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionPlayProgress(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public double progress { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 24, false,  0.001); } set { }  }
}
/// Description: Fusion: AM/FM Station
/// Type: Fast
/// PGNNO: 130820
/// Length: 10
public class fusionAmFmStation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionAmFmStation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string amFm { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public double frequency { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 32, false,  1E-06); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public string track { get { return _packet.GetBitOffsetLength<string, string>(0, 88, 80, false,  0); } set { }  }
}
/// Description: Fusion: VHF
/// Type: Fast
/// PGNNO: 130820
/// Length: 9
public class fusionVhf : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionVhf(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int channel { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 24, false,  0); } set { }  }
}
/// Description: Fusion: Squelch
/// Type: Fast
/// PGNNO: 130820
/// Length: 6
public class fusionSquelch : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSquelch(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int squelch { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
}
/// Description: Fusion: Scan
/// Type: Fast
/// PGNNO: 130820
/// Length: 6
public class fusionScan : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionScan(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public string scan { get { return _packet.GetBitOffsetLength<string, string>(0, 40, 8, false,  0); } set { }  }
}
/// Description: Fusion: Menu Item
/// Type: Fast
/// PGNNO: 130820
/// Length: 23
public class fusionMenuItem : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionMenuItem(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int line { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 88, 40, false,  0); } set { }  }
}
/// Description: Fusion: Replay
/// Type: Fast
/// PGNNO: 130820
/// Length: 23
public class fusionReplay : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionReplay(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string mode { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 24, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public string status { get { return _packet.GetBitOffsetLength<string, string>(0, 80, 8, false,  0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 8, false,  0); } set { }  }
    public int j { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, false,  0); } set { }  }
}
/// Description: Fusion: Sub Volume
/// Type: Fast
/// PGNNO: 130820
/// Length: 8
public class fusionSubVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSubVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Fusion: Tone
/// Type: Fast
/// PGNNO: 130820
/// Length: 8
public class fusionTone : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionTone(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int bass { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, true,  0); } set { }  }
    public int mid { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, true,  0); } set { }  }
    public int treble { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, true,  0); } set { }  }
}
/// Description: Fusion: Volume
/// Type: Fast
/// PGNNO: 130820
/// Length: 10
public class fusionVolume : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionVolume(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int zone1 { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int zone2 { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int zone3 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int zone4 { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
}
/// Description: Fusion: Power State
/// Type: Fast
/// PGNNO: 130820
/// Length: 5
public class fusionPowerState : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionPowerState(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string state { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
}
/// Description: Fusion: SiriusXM Channel
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionSiriusxmChannel : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSiriusxmChannel(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0); } set { }  }
    public string channel { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 96, false,  0); } set { }  }
}
/// Description: Fusion: SiriusXM Title
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionSiriusxmTitle : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSiriusxmTitle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0); } set { }  }
    public string title { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 96, false,  0); } set { }  }
}
/// Description: Fusion: SiriusXM Artist
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionSiriusxmArtist : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSiriusxmArtist(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0); } set { }  }
    public string artist { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 96, false,  0); } set { }  }
}
/// Description: Fusion: SiriusXM Genre
/// Type: Fast
/// PGNNO: 130820
/// Length: 32
public class fusionSiriusxmGenre : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public fusionSiriusxmGenre(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 32, false,  0); } set { }  }
    public string genre { get { return _packet.GetBitOffsetLength<string, string>(0, 56, 96, false,  0); } set { }  }
}
/// Description: Maretron: Proprietary Temperature High Range
/// Type: Fast
/// PGNNO: 130823
/// Length: 9
public class maretronProprietaryTemperatureHighRange : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronProprietaryTemperatureHighRange(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int sid { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public string source { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public double actualTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.1); } set { }  }
    public double setTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.1); } set { }  }
}
/// Description: B&G: Wind data
/// Type: Single
/// PGNNO: 130824
/// Length: 8
public class bGWindData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public bGWindData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int field4 { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int field5 { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int timestamp { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 32, false,  0); } set { }  }
}
/// Description: Maretron: Annunciator
/// Type: Fast
/// PGNNO: 130824
/// Length: 9
public class maretronAnnunciator : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronAnnunciator(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int field4 { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int field5 { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int field6 { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  0); } set { }  }
    public int field7 { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int field8 { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 16, false,  0); } set { }  }
}
/// Description: Lowrance: unknown
/// Type: Fast
/// PGNNO: 130827
/// Length: 10
public class lowranceUnknown : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public lowranceUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
}
/// Description: Simnet: Set Serial Number
/// Type: Fast
/// PGNNO: 130828
/// Length: 8
public class simnetSetSerialNumber : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetSetSerialNumber(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Suzuki: Engine and Storage Device Config
/// Type: Fast
/// PGNNO: 130831
/// Length: 8
public class suzukiEngineAndStorageDeviceConfig : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public suzukiEngineAndStorageDeviceConfig(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Fuel Used - High Resolution
/// Type: Fast
/// PGNNO: 130832
/// Length: 8
public class simnetFuelUsedHighResolution : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetFuelUsedHighResolution(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Engine and Tank Configuration
/// Type: Fast
/// PGNNO: 130834
/// Length: 8
public class simnetEngineAndTankConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetEngineAndTankConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Set Engine and Tank Configuration
/// Type: Fast
/// PGNNO: 130835
/// Length: 8
public class simnetSetEngineAndTankConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetSetEngineAndTankConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Fluid Level Sensor Configuration
/// Type: Fast
/// PGNNO: 130836
/// Length: 14
public class simnetFluidLevelSensorConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetFluidLevelSensorConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int device { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  1); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 4, false,  0); } set { }  }
    public string tankType { get { return _packet.GetBitOffsetLength<string, string>(4, 44, 4, false,  0); } set { }  }
    public double capacity { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 32, false,  0.1); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, true,  0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int, int>(0, 104, 8, true,  0); } set { }  }
}
/// Description: Maretron Proprietary Switch Status Counter
/// Type: Fast
/// PGNNO: 130836
/// Length: 16
public class maretronProprietarySwitchStatusCounter : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronProprietarySwitchStatusCounter(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int startDate { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  1); } set { }  }
    public int startTime { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0.0001); } set { }  }
    public int offCounter { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  1); } set { }  }
    public int onCounter { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  1); } set { }  }
    public int errorCounter { get { return _packet.GetBitOffsetLength<int, int>(0, 96, 8, false,  1); } set { }  }
    public string switchStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 104, 2, false,  0); } set { }  }
}
/// Description: Simnet: Fuel Flow Turbine Configuration
/// Type: Fast
/// PGNNO: 130837
/// Length: 8
public class simnetFuelFlowTurbineConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetFuelFlowTurbineConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Maretron Proprietary Switch Status Timer
/// Type: Fast
/// PGNNO: 130837
/// Length: 23
public class maretronProprietarySwitchStatusTimer : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public maretronProprietarySwitchStatusTimer(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int instance { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int indicatorNumber { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int startDate { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 16, false,  1); } set { }  }
    public int startTime { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, false,  0.0001); } set { }  }
    public decimal accumulatedOffPeriod { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 80, 32, false,  0); } set { }  }
    public decimal accumulatedOnPeriod { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 112, 32, false,  0); } set { }  }
    public decimal accumulatedErrorPeriod { get { return _packet.GetBitOffsetLength<decimal, decimal>(0, 144, 32, false,  0); } set { }  }
    public string switchStatus { get { return _packet.GetBitOffsetLength<string, string>(0, 176, 2, false,  0); } set { }  }
}
/// Description: Simnet: Fluid Level Warning
/// Type: Fast
/// PGNNO: 130838
/// Length: 8
public class simnetFluidLevelWarning : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetFluidLevelWarning(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Pressure Sensor Configuration
/// Type: Fast
/// PGNNO: 130839
/// Length: 8
public class simnetPressureSensorConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetPressureSensorConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Data User Group Configuration
/// Type: Fast
/// PGNNO: 130840
/// Length: 8
public class simnetDataUserGroupConfiguration : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetDataUserGroupConfiguration(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: AIS Class B static data (msg 24 Part A)
/// Type: Fast
/// PGNNO: 130842
/// Length: 29
public class simnetAisClassBStaticDataMsg24PartA : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetAisClassBStaticDataMsg24PartA(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string name { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 160, false,  0); } set { }  }
}
/// Description: Furuno: Six Degrees Of Freedom Movement
/// Type: Fast
/// PGNNO: 130842
/// Length: 29
public class furunoSixDegreesOfFreedomMovement : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoSixDegreesOfFreedomMovement(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 32, true,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 32, true,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 32, true,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 112, 8, true,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 120, 32, true,  0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 152, 32, true,  0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 184, 16, true,  0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 200, 16, true,  0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int, int>(0, 216, 16, true,  0); } set { }  }
}
/// Description: Simnet: AIS Class B static data (msg 24 Part B)
/// Type: Fast
/// PGNNO: 130842
/// Length: 37
public class simnetAisClassBStaticDataMsg24PartB : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetAisClassBStaticDataMsg24PartB(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int userId { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 32, false,  1); } set { }  }
    public string typeOfShip { get { return _packet.GetBitOffsetLength<string, string>(0, 72, 8, false,  0); } set { }  }
    public string vendorId { get { return _packet.GetBitOffsetLength<string, string>(0, 80, 56, false,  0); } set { }  }
    public string callsign { get { return _packet.GetBitOffsetLength<string, string>(0, 136, 56, false,  0); } set { }  }
    public double length { get { return _packet.GetBitOffsetLength<int, double>(0, 192, 16, false,  0.1); } set { }  }
    public double beam { get { return _packet.GetBitOffsetLength<int, double>(0, 208, 16, false,  0.1); } set { }  }
    public double positionReferenceFromStarboard { get { return _packet.GetBitOffsetLength<int, double>(0, 224, 16, false,  0.1); } set { }  }
    public double positionReferenceFromBow { get { return _packet.GetBitOffsetLength<int, double>(0, 240, 16, false,  0.1); } set { }  }
    public int mothershipUserId { get { return _packet.GetBitOffsetLength<int, int>(0, 256, 32, false,  1); } set { }  }
    public int spare { get { return _packet.GetBitOffsetLength<int, int>(2, 290, 6, false,  1); } set { }  }
}
/// Description: Furuno: Heel Angle, Roll Information
/// Type: Fast
/// PGNNO: 130843
/// Length: 8
public class furunoHeelAngleRollInformation : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoHeelAngleRollInformation(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public double yaw { get { return _packet.GetBitOffsetLength<int, double>(0, 32, 16, true,  0.0001); } set { }  }
    public double pitch { get { return _packet.GetBitOffsetLength<int, double>(0, 48, 16, true,  0.0001); } set { }  }
    public double roll { get { return _packet.GetBitOffsetLength<int, double>(0, 64, 16, true,  0.0001); } set { }  }
}
/// Description: Simnet: Sonar Status, Frequency and DSP Voltage
/// Type: Fast
/// PGNNO: 130843
/// Length: 8
public class simnetSonarStatusFrequencyAndDspVoltage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetSonarStatusFrequencyAndDspVoltage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Compass Heading Offset
/// Type: Fast
/// PGNNO: 130845
/// Length: 14
public class simnetCompassHeadingOffset : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetCompassHeadingOffset(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 24, false,  0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
    public double angle { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, true,  0.0001); } set { }  }
}
/// Description: Furuno: Multi Sats In View Extended
/// Type: Fast
/// PGNNO: 130845
/// Length: 8
public class furunoMultiSatsInViewExtended : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoMultiSatsInViewExtended(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: Simnet: Compass Local Field
/// Type: Fast
/// PGNNO: 130845
/// Length: 14
public class simnetCompassLocalField : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetCompassLocalField(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 24, false,  0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
    public double localField { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, false,  0.004); } set { }  }
}
/// Description: Simnet: Compass Field Angle
/// Type: Fast
/// PGNNO: 130845
/// Length: 14
public class simnetCompassFieldAngle : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetCompassFieldAngle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int @unused { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 24, false,  0); } set { }  }
    public int type { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
    public double fieldAngle { get { return _packet.GetBitOffsetLength<int, double>(0, 80, 16, true,  0.0001); } set { }  }
}
/// Description: Simnet: Parameter Handle
/// Type: Fast
/// PGNNO: 130845
/// Length: 14
public class simnetParameterHandle : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetParameterHandle(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 6, false,  0); } set { }  }
    public string repeatIndicator { get { return _packet.GetBitOffsetLength<string, string>(6, 22, 2, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 8, false,  0); } set { }  }
    public int group { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 8, false,  0); } set { }  }
    public int h { get { return _packet.GetBitOffsetLength<int, int>(0, 56, 8, false,  0); } set { }  }
    public int i { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 8, false,  0); } set { }  }
    public int j { get { return _packet.GetBitOffsetLength<int, int>(0, 72, 8, false,  0); } set { }  }
    public string backlight { get { return _packet.GetBitOffsetLength<string, string>(0, 80, 8, false,  0); } set { }  }
    public int l { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 16, false,  0); } set { }  }
}
/// Description: Furuno: Motion Sensor Status Extended
/// Type: Fast
/// PGNNO: 130846
/// Length: 8
public class furunoMotionSensorStatusExtended : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public furunoMotionSensorStatusExtended(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
}
/// Description: SeaTalk: Node Statistics
/// Type: Fast
/// PGNNO: 130847
/// Length: 0
public class seatalkNodeStatistics : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public seatalkNodeStatistics(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int softwareRelease { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 16, false,  0); } set { }  }
    public int developmentVersion { get { return _packet.GetBitOffsetLength<int, int>(3, 27, 8, false,  0); } set { }  }
    public int productCode { get { return _packet.GetBitOffsetLength<int, int>(3, 35, 16, false,  0); } set { }  }
    public int year { get { return _packet.GetBitOffsetLength<int, int>(3, 51, 8, false,  0); } set { }  }
    public int month { get { return _packet.GetBitOffsetLength<int, int>(3, 59, 8, false,  0); } set { }  }
    public int deviceNumber { get { return _packet.GetBitOffsetLength<int, int>(3, 67, 16, false,  0); } set { }  }
    public double nodeVoltage { get { return _packet.GetBitOffsetLength<int, double>(3, 83, 16, false,  0.01); } set { }  }
}
/// Description: Simnet: Event Command: AP command
/// Type: Fast
/// PGNNO: 130850
/// Length: 12
public class simnetEventCommandApCommand : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetEventCommandApCommand(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public int controllingDevice { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string @event { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 16, false,  0); } set { }  }
    public string direction { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 8, false,  0); } set { }  }
    public double angle { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.0001); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
}
/// Description: Simnet: Event Command: Alarm?
/// Type: Fast
/// PGNNO: 130850
/// Length: 12
public class simnetEventCommandAlarm : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetEventCommandAlarm(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string alarm { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 16, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  1); } set { }  }
    public int f { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 8, false,  0); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
}
/// Description: Simnet: Event Command: Unknown
/// Type: Fast
/// PGNNO: 130850
/// Length: 12
public class simnetEventCommandUnknown : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetEventCommandUnknown(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int a { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 32, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 48, 16, false,  0); } set { }  }
    public int d { get { return _packet.GetBitOffsetLength<int, int>(0, 64, 16, false,  0); } set { }  }
    public int e { get { return _packet.GetBitOffsetLength<int, int>(0, 80, 16, false,  0); } set { }  }
}
/// Description: Simnet: Event Reply: AP command
/// Type: Fast
/// PGNNO: 130851
/// Length: 12
public class simnetEventReplyApCommand : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetEventReplyApCommand(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string proprietaryId { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 8, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 24, 16, false,  0); } set { }  }
    public int controllingDevice { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string @event { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 16, false,  0); } set { }  }
    public string direction { get { return _packet.GetBitOffsetLength<string, string>(0, 64, 8, false,  0); } set { }  }
    public double angle { get { return _packet.GetBitOffsetLength<int, double>(0, 72, 16, false,  0.0001); } set { }  }
    public int g { get { return _packet.GetBitOffsetLength<int, int>(0, 88, 8, false,  0); } set { }  }
}
/// Description: Simnet: Alarm Message
/// Type: Fast
/// PGNNO: 130856
/// Length: 8
public class simnetAlarmMessage : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public simnetAlarmMessage(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int messageId { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 16, false,  0); } set { }  }
    public int b { get { return _packet.GetBitOffsetLength<int, int>(0, 32, 8, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 40, 8, false,  0); } set { }  }
    public string text { get { return _packet.GetBitOffsetLength<string, string>(0, 48, 2040, false,  0); } set { }  }
}
/// Description: Airmar: Additional Weather Data
/// Type: Fast
/// PGNNO: 130880
/// Length: 30
public class airmarAdditionalWeatherData : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarAdditionalWeatherData(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public double apparentWindchillTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double trueWindchillTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
    public double dewpoint { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
}
/// Description: Airmar: Heater Control
/// Type: Fast
/// PGNNO: 130881
/// Length: 9
public class airmarHeaterControl : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarHeaterControl(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public int c { get { return _packet.GetBitOffsetLength<int, int>(0, 16, 8, false,  0); } set { }  }
    public double plateTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 24, 16, false,  0.01); } set { }  }
    public double airTemperature { get { return _packet.GetBitOffsetLength<int, double>(0, 40, 16, false,  0.01); } set { }  }
    public double dewpoint { get { return _packet.GetBitOffsetLength<int, double>(0, 56, 16, false,  0.01); } set { }  }
}
/// Description: Airmar: POST
/// Type: Fast
/// PGNNO: 130944
/// Length: 8
public class airmarPost : INMEA2000 {
    byte[] _packet;
    public void SetPacketData(byte[] packet) { _packet = packet; }
    public airmarPost(byte[] packet) { _packet = packet; }
    public long manufacturerCode { get { return _packet.GetBitOffsetLength<long, long>(0, 0, 11, false,  0); } set { }  }
    public int @reserved { get { return _packet.GetBitOffsetLength<int, int>(3, 11, 2, false,  0); } set { }  }
    public string industryCode { get { return _packet.GetBitOffsetLength<string, string>(5, 13, 3, false,  0); } set { }  }
    public string control { get { return _packet.GetBitOffsetLength<string, string>(0, 16, 4, false,  0); } set { }  }
    public int numberOfIdTestResultPairsToFollow { get { return _packet.GetBitOffsetLength<int, int>(3, 27, 8, false,  1); } set { }  }
    public string testId { get { return _packet.GetBitOffsetLength<string, string>(3, 35, 8, false,  0); } set { }  }
    public string testResult { get { return _packet.GetBitOffsetLength<string, string>(3, 43, 8, false,  0); } set { }  }
}

}

