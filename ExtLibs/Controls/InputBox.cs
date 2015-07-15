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

        public static string value = "";

        //from http://www.csharp-examples.net/inputbox/
        public static DialogResult Show(string title, string promptText, ref string value, bool password = false)
        {
            DialogResult answer = DialogResult.Cancel;

            InputBox.value = value;
            string passin = value;

            // ensure we run this on the right thread - mono - mac
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke((MethodInvoker)delegate
                {
                    answer = ShowUI(title, promptText, passin, password);
                });
            }
            else
            {
                answer = ShowUI(title, promptText, passin, password);
            }

            value = InputBox.value;

            return answer;
        }

        static DialogResult ShowUI(string title, string promptText, string value, bool password = false)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            TextBox textBox = new TextBox();
            if (password)
                textBox.UseSystemPasswordChar = true;
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

            label.SetBounds(9, 10, 372, 26);
            textBox.SetBounds(12, 46, 372, 20);
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

            Console.WriteLine("Input Box " + System.Threading.Thread.CurrentThread.Name);

            Application.DoEvents();

            form.ShowDialog();

            Console.WriteLine("Input Box 2 " + System.Threading.Thread.CurrentThread.Name);

            dialogResult = form.DialogResult;

            if (dialogResult == DialogResult.OK)
            {
                value = textBox.Text;
                InputBox.value = value;
            }

            form.Dispose();

            form = null;

            return dialogResult;
        }
    }
}
