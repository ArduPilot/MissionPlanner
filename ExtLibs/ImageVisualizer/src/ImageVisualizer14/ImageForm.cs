using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SkiaSharp;

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    public partial class ImageForm : Form
    {
        [DllImport("gdi32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteObject(IntPtr hObject);

        public static Font UIFont
        {
            get
            {
                return System.Drawing.SystemFonts.DefaultFont;
            }
        }

        public ImageForm(IVisualizerObjectProvider objectProvider)
        {
            InitializeComponent();

#if DEBUG
            this.ShowInTaskbar = true;
#endif

            this.label1.Font = UIFont;
            SetFontAndScale(this.label1, UIFont);
            this.label2.Font = UIFont;
            this.txtExpression.Font = UIFont;
            this.btnClose.Font = UIFont;

            object objectBitmap = objectProvider.GetObject();
            if (objectBitmap != null)
            {
#if DEBUG
                string expression = objectBitmap.ToString();
#endif

                Debug.WriteLine("ImageForm " + objectBitmap.ToString());

                var method = objectBitmap.GetType().GetMethod("ToBitmap", new Type[] { });
                if (method != null)
                {
                    pictureBox1.Image = (Bitmap)method.Invoke(objectBitmap, null);
                }
                else if (objectBitmap is SerializableBitmapImage serializableBitmapImage)
                {
                    pictureBox1.Image = Image.FromStream(new MemoryStream((SerializableBitmapImage)objectBitmap));
                }
                else {
                    txtExpression.Text += "No image found";
                }
            }
     
            txtExpression.Text += objectProvider.GetObject().ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetFontAndScale(Control ctlControl, Font objFont)
        {
            float sngRatio = objFont.Size / ctlControl.Font.Size;
            if (ctlControl is Form form)
            {
                form.AutoScaleMode = AutoScaleMode.None;
            }
            ctlControl.Font = objFont;
            ctlControl.Scale(new SizeF(sngRatio, sngRatio));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
