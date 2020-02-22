using Android.App;
using Android.OS;
using Android.Telephony;
using Android.Util;
using Com.Android.Internal.Telephony.D2d;
using Java.Lang;
using Boolean = System.Boolean;
using String = System.String;

namespace Xamarin.Droid
{
    public class D2DInfoListener : ID2DInfoListenerStub
    {
        private static String TAG = "D2DInfoListenerX";
        private static  int INVALID = 0x7FFFFFFF;
        private Handler mMsgHandler = null;

        // Definition of local event ID
        private static readonly int EVENT_GET_CURRENT_STATUS_REQUEST = 1;
        private static readonly int EVENT_GET_RADIO_POWER = 2;
        private static readonly int EVENT_SEND_D2D_CTRL_CMD = 3;
        private static readonly int EVENT_SEND_D2D_CTRL_CMD_DONE = 4;
        private static readonly int EVENT_REQUEST_FREQUENCY_NEGOTIATION = 5;
        private static readonly int EVENT_REQUEST_FREQUENCY_NEGOTIATION_DONE = 6;
        private static readonly int EVENT_QUERY_DL_FREQUENCY_POINT = 7;
        private static readonly int EVENT_FREQUENCY_HOPPING_CTRL = 8;
        private static readonly int EVENT_FREQUENCY_HOPPING_CTRL_DONE = 9;
        private static readonly int EVENT_QUERY_FREQUENCY_HOPPING_STATE = 10;
        private static readonly int EVENT_CONFIG_D2D_DL_BW = 11;
        private static readonly int EVENT_CONFIG_D2D_UL_BW = 12;
        private static readonly int EVENT_CONFIG_D2D_BW_DONE = 13;

        // Key of D2D information type 
        private static readonly String D2D_INFO_UL_HOPPING_CTRL = "UL_HOP";
        private static readonly String D2D_INFO_DL_HOPPING_CTRL = "DL_HOP";
        private static readonly String D2D_CTRL_CMD             = "CTRL_CMD";
        private static readonly String D2D_BW_CONFIG            = "BW_CFG";
        private static readonly String D2D_CMD_RESULT_CODE      = "ERR_CODE";

        private String mDlFrequencyNum = null;
        private Boolean mD2DLinkConnected = false;
        private Boolean mBound = false;
        private Boolean mIsRadioPowerOn = true;
        private Boolean mIsUlFreqHoppingEnabled = false;
        private Boolean mIsDlFreqHoppingEnabled = false;
        private ProgressDialog mProgressDialog = null;
        private int mPlaneUlGrantBW = 0;
        private int mPlaneUlDataRate = 0;
        private int mSnrGcsMaster = -40;
        private int mSnrGcsSlave = -40;
        private int mSnrUavMaster = -40;
        private int mSnrUavSlave = -40;
        private int mAgcGcsMaster = 100;
        private int mAgcGcsSlave = 100;
        private int mAgcUavMaster = 100;
        private int mAgcUavSlave = 100;
        private int mControllerMasterRsrp = INVALID;
        private int mControllerSlaveRsrp = INVALID;
        private int mPlaneMasterRsrp = INVALID;
        private int mPlaneSlaveRsrp = INVALID;
        private Runnable updateQosInfo;
        private Runnable resetD2DInfo;
        private Runnable updateServiceState;
        private Runnable updateSignalStrength;
        private Runnable updateFreqHopState;
        private Runnable updateRadioPower;
        private Runnable updateDlFreq;
        public override void OnD2DServiceStatusChanged(ServiceState ss)
        {
            Log.Verbose(TAG, "OnD2DServiceStatusChanged: service state = " + ss);
            int state = ss.getState();
            if ((int)PhoneState.InService == state)
            {
                mD2DLinkConnected = true;
            }
            else
            {
                mD2DLinkConnected = false;
                // reset D2D info when D2D link disconnected
                mMsgHandler.post(resetD2DInfo);
            }
            mMsgHandler.post(updateServiceState);
        }

        public override void OnD2DSignalStrengthChanged(SignalStrength ss)
        {
            Log.Verbose(TAG, "OnD2DSignalStrengthChanged: signal strength = ");
            if (ss.getLteRsrq() == INVALID)
            {
                Log.Verbose(TAG, "OnD2DSignalStrengthChanged: plane");
                /*if (ss.getLteRsrp() != INVALID)
                {
                    mPlaneMasterRsrp = ss.getLteRsrp();
                }

                if (ss.getLteSignalStrength() != 99)
                {
                    mPlaneSlaveRsrp = -1 * ss.getLteSignalStrength();
                }*/
            }
            else
            {
                Log.Verbose(TAG, "OnD2DSignalStrengthChanged: controller");
                /* if (ss.getLteRsrp() != INVALID)
                {
                    mControllerMasterRsrp = ss.getLteRsrp();
                }
                if (ss.getLteSignalStrength() != 99)
                {
                    mControllerSlaveRsrp = -1 * ss.getLteSignalStrength();
                }*/
            }
            mMsgHandler.post(updateSignalStrength);
        }

        public override void OnD2DFrequencyListReceived()
        {
            Log.Verbose(TAG, "OnD2DFrequencyListReceived");
        }

        public override void OnD2DULSpeedChanged(int ul_bw, int ul_bit_rate)
        {
            Log.Verbose(TAG, "OnD2DULSpeedChanged: ul_bw = " + ul_bw + ", ul_bit_rate = " + ul_bit_rate);
            mPlaneUlGrantBW = ul_bw;
            mPlaneUlDataRate = ul_bit_rate;
            mMsgHandler.post(updateQosInfo);
        }

        public override void OnD2DSnrGcsChanged(int snr_master, int snr_slave)
        {
            Log.Verbose(TAG, "OnD2DSnrGcsChanged: snr_master = " + snr_master + " snr_slave = " + snr_slave);
            mSnrGcsMaster = snr_master;
            mSnrGcsSlave = snr_slave;
            mMsgHandler.post(updateQosInfo);
        }

        public override void OnD2DSnrUavChanged(int snr_master, int snr_slave)
        {
            Log.Verbose(TAG, "OnD2DSnrUavChanged: snr_master = " + snr_master + " snr_slave = " + snr_slave);
            mSnrUavMaster = snr_master;
            mSnrUavSlave = snr_slave;
            mMsgHandler.post(updateQosInfo);
        }

        public override void OnD2DAgcGcsChanged(int agc_master, int agc_slave)
        {
            Log.Verbose(TAG, "OnD2DAgcGcsChanged: agc_master = " + agc_master + " agc_slave = " + agc_slave);
            mAgcGcsMaster = agc_master;
            mAgcGcsSlave = agc_slave;
            mMsgHandler.post(updateQosInfo);
        }

        public override void OnD2DAgcUavChanged(int agc_master, int agc_slave)
        {
            Log.Verbose(TAG, "OnD2DAgcUavChanged: agc_master = " + agc_master + " agc_slave = " + agc_slave);
            mAgcUavMaster = agc_master;
            mAgcUavSlave = agc_slave;
            mMsgHandler.post(updateQosInfo);
        }

        public override void OnRequestFreqNegotiationDone(int errorCode)
        {
            Bundle args = new Bundle();
            Message msg = mMsgHandler.obtainMessage(EVENT_REQUEST_FREQUENCY_NEGOTIATION_DONE);
            Log.Verbose(TAG, "OnRequestFreqNegotiationDone: errorCode = " + errorCode);

            args.putInt(D2D_CMD_RESULT_CODE, errorCode);
            msg.setData(args);
            mMsgHandler.sendMessage(msg);
        }

        public override void OnRequestFreqResetDone(int errorCode) { }
        public override void OnD2DInterferenceListReceived() { }

        public override void OnRequestFreqHopCtrlDone(int errorCode)
        {
            Bundle args = new Bundle();
            Message msg = mMsgHandler.obtainMessage(EVENT_FREQUENCY_HOPPING_CTRL_DONE);
            Log.Verbose(TAG, "OnRequestFreqHopCtrlDone: errorCode = " + errorCode);

            args.putInt(D2D_CMD_RESULT_CODE, errorCode);
            msg.setData(args);
            mMsgHandler.sendMessage(msg);
        }

        public override void OnRequestGetFreqHopStateDone(int dl_hop_enabled, int ul_hop_enabled, int errorCode)
        {
            Log.Verbose(TAG, "OnRequestGetFreqHopStateDone: dl_hop_enabled = " + dl_hop_enabled + ", ul_hop_enabled = " +
                             ul_hop_enabled + ", errorCode = " + errorCode);
            if (0 == errorCode)
            {
                mIsDlFreqHoppingEnabled = (1 == dl_hop_enabled) ? true : false;
                mIsUlFreqHoppingEnabled = (1 == ul_hop_enabled) ? true : false;
                mMsgHandler.post(updateFreqHopState);
            }
        }

        public override void OnD2DRadioPowerChanged(Boolean is_power_on)
        {
            Log.Verbose(TAG, "OnD2DRadioPowerChanged: is_power_on = " + is_power_on);
            mIsRadioPowerOn = is_power_on;
            mMsgHandler.post(updateRadioPower);
        }

        public override void OnRequestGetDlFrequencyPointDone(int dl_frequency_point, int errorCode)
        {
            Log.Verbose(TAG, "OnRequestGetDlFrequencyPointDone: dl_frequency_point = " + dl_frequency_point +
                             ", errorCode = " + errorCode);
            if (0 == errorCode)
            {
                mDlFrequencyNum = (dl_frequency_point).ToString();
                mMsgHandler.post(updateDlFreq);
            }
        }

        public override void OnRequestSendCtrlCmdDone(int errorCode)
        {
            Bundle args = new Bundle();
            Message msg = mMsgHandler.obtainMessage(EVENT_SEND_D2D_CTRL_CMD_DONE);
            Log.Verbose(TAG, "OnRequestSendCtrlCmdDone: errorCode = " + errorCode);

            args.putInt(D2D_CMD_RESULT_CODE, errorCode);
            msg.setData(args);
            mMsgHandler.sendMessage(msg);
        }

        public override void OnRequestConfigBandwidthDone(int errorCode)
        {
            Bundle args = new Bundle();
            Message msg = mMsgHandler.obtainMessage(EVENT_CONFIG_D2D_BW_DONE);
            Log.Verbose(TAG, "OnRequestConfigBandwidthDone: errorCode = " + errorCode);

            args.putInt(D2D_CMD_RESULT_CODE, errorCode);
            msg.setData(args);
            mMsgHandler.sendMessage(msg);
        }
    }
}