using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
   public partial class ValuesControl : UserControl, IDynamicParameterControl
   {
      #region Properties

       public event EventValueChanged ValueChanged;

      public string LabelText { get { return myLabel1.Text; } set { myLabel1.Text = value; } }
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
      public ComboBox ComboBoxControl { get { return comboBox1; } set { comboBox1 = value; } }

      #region Interface Properties

      public string Value
      {
          get { return comboBox1.SelectedValue.ToString(); }
          set
          {
              comboBox1.SelectedValue = value;
              if (ValueChanged != null)
                  ValueChanged(this,Name, Value);
          }
      }

      #endregion

      #endregion

      #region Constructor

      public ValuesControl()
      {
         InitializeComponent();
      }

      #endregion

      private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
          if (ValueChanged != null)
              ValueChanged(this,Name, Value);
      }

      public void DeAttachEvents()
      {
          Delegate[] subscribers = ValueChanged.GetInvocationList();
          for (int i = 0; i < subscribers.Length; i++)
          {
              ValueChanged -= subscribers[i] as EventValueChanged;
          }
      }
   }
}
