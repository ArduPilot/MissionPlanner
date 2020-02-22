/*******************************************************************************
    Copyright(c) 2014 - 2018 Pinecone Electronics
    All Rights Reserved. By using this module you agree to the terms of the
    Pinecone Electronics License Agreement for it.
********************************************************************************
* Filename      : ID2DService.aidl
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

import com.android.internal.telephony.d2d.ID2DInfoListener;

interface ID2DService {
    void registerForD2dInfoChanged(in ID2DInfoListener listener);
    void unregisterCallback(in ID2DInfoListener listener);

    void requestGetD2dInfo();
    void requestD2dFreqNegotiation();
    void requestD2dFreqReset(String freq_point);
    void requestGetD2dFreqHopState();
    void requestD2dFreqHopCtrl(String dl_freq_hopping_enabled, String ul_freq_hopping_enabled);
    void requestGetD2dDlFrequencyPoint();
    void requestSendD2DCtrlCmd(String ctrl_cmd);
    void requestConfigD2dBandwidth(boolean is_downlink, int bw_cfg);
    boolean requestGetRadioPower();

    void onD2dInfoChanged(int cmd_id);
    void onD2dRequestDone(int cmd_id, int errorCode);
}
