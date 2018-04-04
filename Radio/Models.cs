using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MissionPlanner.Radio
{
    enum Model
    {
        P2P,
        MULTIPOINT,
        ASYNC
    }

    class ExtraParamControlsSet
    {
        LabelComboBoxPair NodeID;
        LabelComboBoxPair DestID;
        LabelComboBoxPair TXENCAP;
        LabelComboBoxPair RXENCAP;
        IEnumerable<Control> Others;
        bool Remote;

        public ExtraParamControlsSet(Label lblNodeID, ComboBox cmbNodeID,
            Label lblDestID, ComboBox cmbDestID,
            Label lblTXENCAP, ComboBox cmbTXENCAP,
            Label lblRXENCAP, ComboBox cmbRXENCAP,
            Control[] Others, bool Remote)
        {
            NodeID = new LabelComboBoxPair(lblNodeID, cmbNodeID);
            DestID = new LabelComboBoxPair(lblDestID, cmbDestID);
            TXENCAP = new LabelComboBoxPair(lblTXENCAP, cmbTXENCAP);
            RXENCAP = new LabelComboBoxPair(lblRXENCAP, cmbRXENCAP);
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
                    break;
                case Model.ASYNC:
                    DestID.ComboBox.Name = Prefix + "DESTID";
                    TXENCAP.ComboBox.Name = Prefix + "TXENCAP";
                    TXENCAP.Label.Text = "TXENCAP";
                    RXENCAP.ComboBox.Name = Prefix + "RXENCAP";
                    RXENCAP.Label.Text = "RXENCAP";
                    NodeID.ComboBox.DataSource = Sikradio.Range(1, 1, 32767);
                    DestID.ComboBox.DataSource = Sikradio.Range(1, 1, 65535);
                    TXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 2);
                    RXENCAP.ComboBox.DataSource = Sikradio.Range(0, 1, 2);
                    SetAllVisible(true);
                    break;
            }
        }

        void SetAllVisible(bool V)
        {
            NodeID.Visible = V;
            DestID.Visible = V;
            TXENCAP.Visible = V;
            RXENCAP.Visible = V;
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