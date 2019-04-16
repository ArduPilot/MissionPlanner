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

        public static event EventHandler TextChanged;

        public static DialogResult Show(string title, string promptText, ref int value, bool password = false)
        {
            string answer = value.ToString();
            var dialog = Show(title, promptText, ref answer, password);
            value = int.Parse(answer);
            return dialog;
        }

        public static DialogResult Show(string title, string promptText, ref double value, bool password = false)
        {
            string answer = value.ToString();
            var dialog = Show(title, promptText, ref answer, password);
            value = double.Parse(answer);
            return dialog;
        }
        //from http://www.csharp-examples.net/inputbox/
        public static DialogResult Show(string title, string promptText, ref string value, bool password = false, bool multiline = false)
        {
            DialogResult answer = DialogResult.Cancel;

            InputBox.value = value;
            string passin = value;

            // ensure we run this on the right thread - mono - mac
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke((MethodInvoker)delegate
                {
                    answer = ShowUI(title, promptText, passin, password, multiline);
                });
            }
            else
            {
                answer = ShowUI(title, promptText, passin, password, multiline);
            }

            value = InputBox.value;

            return answer;
        }

        static DialogResult ShowUI(string title, string promptText, string value, bool password = false, bool multiline = false)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            if (password)
                textBox.UseSystemPasswordChar = true;
            if (multiline)
                textBox.Multiline = true;
            MyButton buttonOk = new MyButton();
            MyButton buttonCancel = new MyButton();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            //form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            // Suspend form layout.
            form.SuspendLayout();
            const int yMargin = 10;

            //
            // label
            //
            var y = 20;
            label.AutoSize = true;
            label.Location = new Point(9, y);
            label.Size = new Size(372, 13);
            label.Text = promptText;
            label.MaximumSize = new Size(372, 0);

            //
            // textBox
            //
            textBox.Size = new Size(372, 20);
            textBox.Text = value;
            textBox.TextChanged += textBox_TextChanged;

            //
            // buttonOk
            //
            buttonOk.Size = new Size(75, 23);
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;
            
            //
            // buttonCancel
            //
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            
            //
            // form
            //
            form.TopMost = true;
            form.TopLevel = true;
            form.Text = title;
            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            // Resume form layout
            form.ResumeLayout(false);
            form.PerformLayout();

            // Adjust the location of textBox, buttonOk, buttonCancel based on the content of the label.
            y = y + label.Height + yMargin;
            textBox.Location = new Point(12, y);
            y = y + textBox.Height + yMargin;
            buttonOk.Location = new Point(228, y);
            buttonCancel.Location = new Point(309, y);
            // Increase the size of the form.
            form.ClientSize = new Size(396, y + buttonOk.Height + yMargin);

            if (ApplyTheme != null)
                ApplyTheme(form);
            

            Console.WriteLine("Input Box " + System.Threading.Thread.CurrentThread.Name);

            Application.DoEvents();

            form.ShowDialog();

            Console.WriteLine("Input Box 2 " + System.Threading.Thread.CurrentThread.Name);

            DialogResult dialogResult = form.DialogResult;

            if (dialogResult == DialogResult.OK)
            {
                value = textBox.Text;
                InputBox.value = value;
            }

            form.Dispose();

            TextChanged = null;

            form = null;

            return dialogResult;
        }

        static void textBox_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(sender, e);
        }
    }
}
