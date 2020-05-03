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
        IEnumerable<Control> Others;
        bool Remote;

        public ExtraParamControlsSet(Label lblNodeID, ComboBox cmbNodeID,
            Label lblDestID, ComboBox cmbDestID,
            Label lblTXENCAP, ComboBox cmbTXENCAP,
            Label lblRXENCAP, ComboBox cmbRXENCAP,
            Label lblMAX_DATA, ComboBox cmbMAX_DATA,
            Control[] Others, bool Remote)
        {
            NodeID = new LabelComboBoxPair(lblNodeID, cmbNodeID);
            DestID = new LabelComboBoxPair(lblDestID, cmbDestID);
            TXENCAP = new LabelComboBoxPair(lblTXENCAP, cmbTXENCAP);
            RXENCAP = new LabelComboBoxPair(lblRXENCAP, cmbRXENCAP);
            MAX_DATA = new LabelComboBoxPair(lblMAX_DATA, cmbMAX_DATA);
            this.Others = Others;
            this.Remote = Remote;
        }

        public void SetModel(Model M)
        {
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
                        var Temp = (List<int>)Sikradio.Range(0, 1, 29);
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
                        TXENCAP.ComboBox.Name = Prefix + "NODECOUNT";
                        TXENCAP.Label.Text = "Node Count";
                        RXENCAP.ComboBox.Name = Prefix + "SERBREAKMS10";
                        RXENCAP.Label.Text = "Ser. brk. x10ms";
                        MAX_DATA.ComboBox.Name = "GPO1_3STATLED";
                        MAX_DATA.Label.Text = "GPO1_\n3STATLED";
                        NodeID.ComboBox.DataSource = Sikradio.Range(0, 1, 29);
                        var Temp = (List<int>)Sikradio.Range(0, 1, 29);
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
                        MAX_DATA.Visible = true;
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

        public LabelComboBoxPair(Label L, ComboBox C)
        {
            this.Label = L;
            this.ComboBox = C;
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
    }
}