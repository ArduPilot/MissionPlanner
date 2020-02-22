/*******************************************************************************
    Copyright(c) 2014 - 2018 Pinecone Electronics
    All Rights Reserved. By using this module you agree to the terms of the
    Pinecone Electronics License Agreement for it.
********************************************************************************
* Filename      : ID2DInfoListener.aidl
*
* Description   : <describing what this file is to do>
*
* Notes         : <the limitations to use this file>
*
*--------------------------------------------------------------------------------
* Change History:
*--------------------------------------------------------------------------------
*
*
*
*******************************************************************************/

package com.android.internal.telephony.d2d;

import android.telephony.ServiceState;
import android.telephony.SignalStrength;

interface ID2DInfoListener {
    void onD2DServiceStatusChanged(in ServiceState ss);
    void onD2DSignalStrengthChanged(in SignalStrength ss);
    void onD2DFrequencyListReceived();
    void onD2DULSpeedChanged(int ul_bw, int ul_bit_rate);
    void onD2DSnrGcsChanged(int snr_master, int snr_slave);
    void onD2DSnrUavChanged(int snr_master, int snr_slave);
    void onD2DAgcGcsChanged(int agc_master, int agc_slave);
    void onD2DAgcUavChanged(int agc_master, int agc_slave);
    void onD2DInterferenceListReceived();
    void onD2DRadioPowerChanged(boolean is_power_on);

    void onRequestFreqNegotiationDone(int errorCode);
    void onRequestFreqResetDone(int errorCode);
    void onRequestFreqHopCtrlDone(int errorCode);
    void onRequestGetFreqHopStateDone(int dl_hop_enabled, int ul_hop_enabled, int errorCode);
    void onRequestGetDlFrequencyPointDone(int dl_frequency_point, int errorCode);
    void onRequestSendCtrlCmdDone(int errorCode);
    void onRequestConfigBandwidthDone(int errorCode);
}
