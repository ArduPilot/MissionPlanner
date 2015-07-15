﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Drawing.Drawing2D;

namespace MissionPlanner.Controls
{
    public class MyButton : Button
    {
        bool _mouseover = false;
        bool _mousedown = false;

        //internal Color _BGGradTop = Color.FromArgb(0x94, 0xc1, 0x1f);
        //internal Color _BGGradBot = Color.FromArgb(0xcd, 0xe2, 0x96);
        //internal Color _TextColor = Color.FromArgb(0x40, 0x57, 0x04);
        //internal Color _Outline = Color.FromArgb(0x79, 0x94, 0x29);

        internal Color _BGGradTop;
        internal Color _BGGradBot;
        internal Color _TextColor;
        internal Color _Outline;

       bool inOnPaint = false;

       [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
       [DefaultValue(typeof(Color), "0x94, 0xc1, 0x1f")]
       public Color BGGradTop { get { return _BGGradTop; } set { _BGGradTop = value; this.Invalidate(); } }
         [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
         [DefaultValue(typeof(Color), "0xcd, 0xe2, 0x96")]
       public Color BGGradBot { get { return _BGGradBot; } set { _BGGradBot = value; this.Invalidate(); } }

        // i want to ignore forecolor
         [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
         [DefaultValue(typeof(Color), "0x40, 0x57, 0x04")]
         public Color TextColor { get { return _TextColor; } set { _TextColor = value; this.Invalidate(); } }
         [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
         [DefaultValue(typeof(Color), "0x79, 0x94, 0x29")]
         public Color Outline { get { return _Outline; } set { _Outline = value; this.Invalidate(); } }

         public MyButton()
         {
             _BGGradTop = Color.FromArgb(0x94, 0xc1, 0x1f);
             _BGGradBot = Color.FromArgb(0xcd, 0xe2, 0x96);
             _TextColor = Color.FromArgb(0x40, 0x57, 0x04);
             _Outline = Color.FromArgb(0x79, 0x94, 0x29);
         }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //base.OnPaint(pevent);

            if (inOnPaint)
                return;

            inOnPaint = true;

            try
            {
                Graphics gr = pevent.Graphics;

                gr.Clear(this.BackColor);

                gr.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle outside = new Rectangle(0, 0, this.Width, this.Height);

                LinearGradientBrush linear = new LinearGradientBrush(outside, BGGradTop, BGGradBot, LinearGradientMode.Vertical);

                Pen mypen = new Pen(Outline, 1);

                GraphicsPath outline = new GraphicsPath();

                float wid = this.Height / 3f;

                wid = 1;

                int width = this.Width - 1;
                int height = this.Height - 1;

                // tl
                outline.AddArc(0, 0, wid, wid, 180, 90);
                // top line
                outline.AddLine(wid, 0, width - wid, 0);
                // tr
                outline.AddArc(width - wid, 0, wid, wid, 270, 90);
                // br
                outline.AddArc(width - wid, height - wid, wid, wid, 0, 90);
                // bottom line
                outline.AddLine(wid, height, width - wid, height);
                // bl
                outline.AddArc(0, height - wid, wid, wid, 90, 90);
                // left line
                outline.AddLine(0, height - wid, 0, wid - wid /2);


                gr.FillPath(linear, outline);

                gr.DrawPath(mypen, outline);

                SolidBrush mybrush = new SolidBrush(TextColor);

                if (_mouseover)
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb(73, 0x2b, 0x3a, 0x03));

                    gr.FillPath(brush, outline);
                }
                if (_mousedown)
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb(73, 0x2b, 0x3a, 0x03));

                    gr.FillPath(brush, outline);
                }

                if (!this.Enabled)
                {
                    SolidBrush brush = new SolidBrush(Color.FromArgb(150, 0x2b, 0x3a, 0x03));

                    gr.FillPath(brush, outline);
                }


                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                string display = this.Text;
                int amppos = display.IndexOf('&');
                if (amppos != -1)
                    display = display.Remove(amppos, 1);

                gr.DrawString(display, this.Font, mybrush, outside, stringFormat);
            }
            catch { }
            
            inOnPaint = false;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseover = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseover = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _mousedown = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _mousedown = false;
            base.OnMouseUp(mevent);
        }
    }
}
