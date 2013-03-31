using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Controls
{
    public partial class ImageLabel : UserControl //ContainerControl
    {
        public new event EventHandler Click;

        private Image picture;
        private string text = "None";

        [System.ComponentModel.Browsable(true)]
        public Label Label;
        [System.ComponentModel.Browsable(true)]
        public PictureBox PictureBox;

        public ImageLabel()
        {
            text = "";
            picture = new Bitmap(640,480);

            InitializeComponent();

            Label = label1;
            PictureBox = pictureBox1;
        }

        public void setImageandText(Image image, string text)
        {
            pictureBox1.Image = image;
            label1.Text = text;
        }

        [System.ComponentModel.Browsable(true)]
        public Image Image {
            get { return picture; }
            set { picture = value; pictureBox1.Image = picture; }
        }

        [System.ComponentModel.Browsable(true)]
        public new string Text
        {
            get { return text; }
            set { text = value; label1.Text = text; }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(sender,new EventArgs());
            }
        }
    }
}
