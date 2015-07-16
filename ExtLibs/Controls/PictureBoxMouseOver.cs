using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class PictureBoxMouseOver: PictureBox
    {
        public Image ImageNormal { get; set; }
        public Image ImageOver { get; set; }

        bool mouseover = false;
        bool _selected = false;
        public bool selected { get { return _selected; } set { _selected = value; ChangePicture(); this.Invalidate(); } }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            mouseover = true;

            ChangePicture();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            mouseover = false;

            ChangePicture();
        }

        void ChangePicture()
        {
            if (mouseover || selected)
            {
                Image = ImageOver;
            }
            else
            {
                Image = ImageNormal;
            }

            this.Invalidate();
        }

    }
}
