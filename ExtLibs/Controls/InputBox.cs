using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class InputBox
    {
        public delegate void ThemeManager(Control ctl);

        public static event ThemeManager ApplyTheme;

        //from http://www.csharp-examples.net/inputbox/
        public static DialogResult Show(string title, string promptText, ref string value)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            TextBox textBox = new TextBox();
            Controls.MyButton buttonOk = new Controls.MyButton();
            Controls.MyButton buttonCancel = new Controls.MyButton();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            //form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            form.TopMost = true;
            form.TopLevel = true;

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            if (ApplyTheme != null)
                ApplyTheme(form);

            DialogResult dialogResult = DialogResult.Cancel;

            Console.WriteLine("Input Box");

            form.ShowDialog();

            Console.WriteLine("Input Box 2");

            dialogResult = form.DialogResult;

            if (dialogResult == DialogResult.OK)
            {
                value = textBox.Text;
            }

            form.Dispose();

            form = null;

            return dialogResult;
        }
    }
}
