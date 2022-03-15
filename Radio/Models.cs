using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MissionPlanner.Radio
{
    /// <summary>
    /// The radio firmware variant.
    /// </summary>
    enum Model
    {
        /// <summary>
        /// All radio point-to-point.
        /// </summary>
        P2P,
        /// <summary>
        /// A, + and u multipoint.
        /// </summary>
        MULTIPOINT,
        /// <summary>
        /// X async
        /// </summary>
        ASYNC,          
        /// <summary>
        /// X multipoint
        /// </summary>
        MULTIPOINT_X    
    }

    /// <summary>
    /// Sets of extra parameters for radio firmware variants.
    /// </summary>
    class ExtraParamControlsSet
    {
        LabelComboBoxPair NodeID;
        LabelComboBoxPair DestID;
        LabelComboBoxPair TXENCAP;
        LabelComboBoxPair RXENCAP;
        LabelComboBoxPair MAX_DATA;
        LabelComboBoxPair MAX_RETRIES;
        IEnumerable<Control> Others;
        bool Remote;

        public ExtraParamControlsSet(Label lblNodeID, ComboBox cmbNodeID,
            Label lblDestID, ComboBox cmbDestID,
            Label lblTXENCAP, ComboBox cmbTXENCAP,
            Label lblRXENCAP, ComboBox cmbRXENCAP,
            Label lblMAX_DATA, ComboBox cmbMAX_DATA,
            Label lblMAX_RETRIES, ComboBox cmbMAX_RETRIES,
            Control[] Others, bool Remote)
        {
            NodeID = new LabelComboBoxPair(lblNodeID, cmbNodeID);
            DestID = new LabelComboBoxPair(lblDestID, cmbDestID);
            TXENCAP = new LabelComboBoxPair(lblTXENCAP, cmbTXENCAP);
            RXENCAP = new LabelComboBoxPair(lblRXENCAP, cmbRXENCAP);
            MAX_DATA = new LabelComboBoxPair(lblMAX_DATA, cmbMAX_DATA);
            MAX_RETRIES = new LabelComboBoxPair(lblMAX_RETRIES, cmbMAX_RETRIES);
            this.Others = Others;
            this.Remote = Remote;
        }

        /// <summary>
        /// Reconfigure the GUI for the given modem firmware type
        /// </summary>
        /// <param name="M">The modem firmware type.</param>
        /// <param name="Settings">The settings retrieved from the modem.  Must not be null.</param>
        public void SetModel(Model M, Dictionary<string, RFD.RFD900.TBaseSetting> Settings)
        {
            NodeID.Reset();
            DestID.Reset();
            TXENCAP.Reset();
            RXENCAP.Reset();
            MAX_DATA.Reset();
            MAX_RETRIES.Reset();

            string Prefix = Remote ? "R" : "";

            switch (M)
            {
                case Model.P2P:
                    SetAllVisible(false);
                    break;
                case Model.MULTIPOINT:
                    {
                        DestID.ComboBox.Name = Prefix + "NODEDESTINATION";
                        TXENCAP.ComboBox.Name = Prefix + "SYNCANY";
                        TXENCAP.Label.Text = "Sync Any";
                        RXENCAP.ComboBox.Name = Prefix + "NODECOUNT";
                        RXENCAP.Label.Text = "Node Count";
                        NodeID.ComboBox.DataSource = Sikradio.Range(0, 1, 29);
                        var Temp = new List<int>(Sikradio.Range(0, 1, 29));
                        Temp.Add(65535);
                        DestID.ComboBox.DataSource = Temp;
                        TXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 1);
                        RXENCAP.ComboBox.DataSource = Sikradio.Range(2, 1, 30);
                        SetAllVisible(false);
                        NodeID.Visible = true;
                        DestID.Visible = true;
                        TXENCAP.Visible = true;
                        RXENCAP.Visible = true;
                    }
                    break;
                case Model.ASYNC:
                    DestID.ComboBox.Name = Prefix + "DESTID";
                    TXENCAP.ComboBox.Name = Prefix + "TX_ENCAP_METHOD";
                    TXENCAP.Label.Text = "TXENCAP";
                    RXENCAP.ComboBox.Name = Prefix + "RX_ENCAP_METHOD";
                    RXENCAP.Label.Text = "RXENCAP";
                    MAX_DATA.ComboBox.Name = Prefix + "MAX_DATA";
                    MAX_DATA.Label.Text = "Max Data";
                    NodeID.ComboBox.DataSource = Sikradio.Range(1, 1, 32767);
                    DestID.ComboBox.DataSource = Sikradio.Range(1, 1, 65535);
                    TXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 2);
                    RXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 2);
                    //MAX_DATA combo should be populated automatically from ATI5? cmd
                    SetAllVisible(true);
                    break;
                case Model.MULTIPOINT_X:
                    {
                        DestID.ComboBox.Name = Prefix + "NODEDESTINATION";
                        if (Settings.ContainsKey("NETCOUNT"))
                        {
                            TXENCAP.ComboBox.Name = Prefix + "NETCOUNT";
                            TXENCAP.Label.Text = "Net Count";
                        }
                        else
                        {
                            TXENCAP.ComboBox.Name = Prefix + "NODECOUNT";
                            TXENCAP.Label.Text = "Node Count";
                        }
                        RXENCAP.ComboBox.Name = Prefix + "SERBREAKMS10";
                        RXENCAP.Label.Text = "Ser. brk. x10ms";
                        if (Settings.ContainsKey("MASTERBACKUP"))
                        {
                            MAX_DATA.ComboBox.Name = Prefix + "MASTERBACKUP";
                            MAX_DATA.Label.Text = "Master Bckp";
                        }
                        MAX_RETRIES.ComboBox.Name = Prefix + "RXFRAME";
                        MAX_RETRIES.Label.Text = "Rx Frame";
                        NodeID.ComboBox.DataSource = Sikradio.Range(0, 1, 29);
                        var Temp = new List<int>(Sikradio.Range(0, 1, 29));
                        Temp.Add(65535);
                        DestID.ComboBox.DataSource = Temp;
                        TXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 1);
                        RXENCAP.ComboBox.DataSource = Sikradio.Range(2, 1, 30);
                        MAX_DATA.ComboBox.DataSource = Sikradio.Range(0, 1, 1);
                        SetAllVisible(false);
                        NodeID.Visible = true;
                        DestID.Visible = true;
                        TXENCAP.Visible = true;
                        RXENCAP.Visible = true;
                        if (Settings.ContainsKey("MASTERBACKUP"))
                        { 
                            MAX_DATA.Visible = true;
                        }
                        if (Settings.ContainsKey("RXFRAME"))
                        {
                            MAX_RETRIES.Visible = true;
                        }
                    }
                    break;
            }
        }

        void SetAllVisible(bool V)
        {
            NodeID.Visible = V;
            DestID.Visible = V;
            TXENCAP.Visible = V;
            RXENCAP.Visible = V;
            MAX_DATA.Visible = V;
            MAX_RETRIES.Visible = V;
            foreach (var x in Others)
            {
                x.Visible = V;
            }
        }
    }

    class LabelComboBoxPair
    {
        public readonly Label Label;
        public readonly ComboBox ComboBox;
        public readonly string OrigName;
        public readonly string OrigLabel;

        public LabelComboBoxPair(Label L, ComboBox C)
        {
            this.Label = L;
            this.ComboBox = C;
            OrigName = C.Name;
            OrigLabel = L.Text;
        }

        public bool Visible
        {
            get
            {
                return Label.Visible;
            }
            set
            {
                Label.Visible = value;
                ComboBox.Visible = value;
            }
        }

        public void Reset()
        {
            Label.Text = OrigLabel;
            ComboBox.Name = OrigName;
        }
    }
}