using System;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ModifyandSet : UserControl
    {
        [System.ComponentModel.Browsable(false)]
        public NumericUpDown NumericUpDown {
            get { return numericUpDown1; }
        }

        [System.ComponentModel.Browsable(false)]
        public MyButton Button {
            get { return myButton1; }
        }

        [System.ComponentModel.Browsable(true)]
        public String ButtonText
        {
            get { return Button.Text; }
            set { Button.Text = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Value
        {
            get { return NumericUpDown.Value; }
            set { NumericUpDown.Value = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Minimum
        {
            get { return NumericUpDown.Minimum; }
            set { NumericUpDown.Minimum = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public Decimal Maximum
        {
            get { return NumericUpDown.Maximum; }
            set { NumericUpDown.Maximum = value; }
        }

        public new event EventHandler Click;
        public event EventHandler ValueChanged;

        public ModifyandSet()
        {
            InitializeComponent();
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }
    }
}