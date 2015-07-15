using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class RangeControl : UserControl, IDynamicParameterControl
    {
        #region Properties

        public event EventValueChanged ValueChanged;

        public NumericUpDown NumericUpDownControl
        {
            get { return numericUpDown1; }
            set { numericUpDown1 = value; }
        }

        public string DescriptionText
        {
            get { return label1.Text; }
            set
            {
                //label1.MaximumSize = new Size(this.Width - 30, 0);
                label1.Text = value;
                //this.Height += label1.Height;
            }
        }

        public string LabelText
        {
            get { return myLabel1.Text; }
            set { myLabel1.Text = value; }
        }

        public TrackBar TrackBarControl
        {
            get { return trackBar1; }
            set { trackBar1 = value; }
        }

        public float Increment
        {
            get { return _increment; }
            set
            {
                _increment = value;
                numericUpDown1.Increment = (decimal) _increment;
                numericUpDown1.DecimalPlaces = _increment.ToString().Length - 1;
            }
        }

        public float DisplayScale { get; set; }

        public float MinRange
        {
            get { return _minrange; }
            set
            {
                _minrange = value;
                numericUpDown1.Minimum = (decimal) (value/DisplayScale);
                LBL_min.Text = numericUpDown1.Minimum.ToString();
            }
        }

        public float MaxRange
        {
            get { return _maxrange; }
            set
            {
                _maxrange = value;
                numericUpDown1.Maximum = (decimal) (value/DisplayScale);
                LBL_max.Text = numericUpDown1.Maximum.ToString();
            }
        }

        private float _minrange = 0;
        private float _maxrange = 10;
        private float _increment = 1;
        private bool intrackbarchange = false;

        #region Interface Properties

        public string Value
        {
            get { return ((float) numericUpDown1.Value*DisplayScale).ToString(CultureInfo.InvariantCulture); }
            set
            {
                float back1 = _minrange;
                float back2 = _maxrange;

                MinRange = (float) Math.Min(MinRange, double.Parse(value));
                MaxRange = (float) Math.Max(MaxRange, double.Parse(value));

                _minrange = back1;
                _maxrange = back2;

                if (double.Parse(value) > _maxrange || double.Parse(value) < _minrange)
                {
                    numericUpDown1.BackColor = Color.Orange;
                }

                numericUpDown1.Value = (decimal) ((float) decimal.Parse(value)/DisplayScale);
                numericUpDown1_ValueChanged(null, null);

                if (ValueChanged != null)
                    ValueChanged(this, Name, Value);
            }
        }

        #endregion

        #endregion

        #region Constructor

        public RangeControl()
        {
            InitializeComponent();
            DisplayScale = 1;
        }


        public RangeControl(string param, String Desc, string Label, float increment, float Displayscale, float minrange,
            float maxrange, string value)
        {
            InitializeComponent();
            this.DisplayScale = Displayscale;

            Name = param;
            Increment = increment;
            DescriptionText = Desc;
            LabelText = Label;
            MinRange = minrange;
            MaxRange = maxrange;
            float delta = maxrange - minrange;

            this.Value = value;
        }

        #endregion

        #region Methods

        public void AttachEvents()
        {
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
        }

        public void DeAttachEvents()
        {
            numericUpDown1.ValueChanged -= numericUpDown1_ValueChanged;
            trackBar1.ValueChanged -= trackBar1_ValueChanged;
        }

        #endregion

        #region Events

        private decimal map(decimal x, decimal in_min, decimal in_max, decimal out_min, decimal out_max)
        {
            return (x - in_min)*(out_max - out_min)/(in_max - in_min) + out_min;
        }

        protected void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // update trackbar value
            if (!intrackbarchange)
                trackBar1.Value =
                    (int) map(numericUpDown1.Value, numericUpDown1.Minimum, numericUpDown1.Maximum, 0, 1000);

            // if the increment is 1, its most likerly a int on the ap, so treat it as such
            if (Increment == 1.0)
                numericUpDown1.Value = (int) (numericUpDown1.Value);

            numericUpDown1.BackColor = Color.Green;

            if ((float) numericUpDown1.Value < (_minrange))
                numericUpDown1.BackColor = Color.Orange;

            if ((float) numericUpDown1.Value > (_maxrange))
                numericUpDown1.BackColor = Color.Orange;

            if (ValueChanged != null)
                ValueChanged(this, Name, Value);
        }

        protected void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            intrackbarchange = true;
            numericUpDown1.Value = map(trackBar1.Value, 0, 1000, numericUpDown1.Minimum, numericUpDown1.Maximum);
            intrackbarchange = false;
        }

        #endregion
    }
}