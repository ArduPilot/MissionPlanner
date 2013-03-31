
#region BSD License
/*
    Copyright (c) 2010 Miguel Angel Guirado López

    This file is part of CsAssortedWidgets.

    All rights reserved.
 
    This file is a C# port of AssortedWidgets project. Original authors see readme.txt file.

    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Windows.Forms;

using AssortedWidgets.Events;
using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
	#region SlideBar

	public class SlideBar : Component, IElement
    {
        SlideBarSlider Slider_;
        float Value_;
        float minV;
        float MaxV_;

        public SlideBar()
            : this(ETypeOrientation.Horizontal)
        {
        }
        public SlideBar(ETypeOrientation type)
            : this(0, 100f, type)
        {
        }
        public SlideBar(float minV, float maxV, ETypeOrientation type)
            : this(minV, maxV, 0, type)
        {
        }
        public SlideBar(float minV, float maxV, float value, ETypeOrientation type)
        {
            this.minV = minV;
            MaxV_ = maxV;
            Type = type;
            Value = value;

            if (Type == ETypeOrientation.Horizontal)
            {
                Slider_ = new SlideBarSlider(ETypeOrientation.Horizontal);
                HorizontalStyle = EElementStyle.Stretch;
                VerticalStyle = EElementStyle.Fit;
                Size.width = 10;
                Size.height = 20;
                Slider_.Size.width = (uint)Math.Max((Size.width - 4) * 0.1f, 4);
                Slider_.Size.height = 16;
                Slider_.Position.X = (int)(((Size.width - 4) - Slider_.Size.width) * Value + 2);
                Slider_.Position.Y = 2;
                Slider_.SetSlideBar(this);
            }
            else
            {
                Slider_ = new SlideBarSlider(ETypeOrientation.Vertical);
                HorizontalStyle = EElementStyle.Fit;
                VerticalStyle = EElementStyle.Stretch;
                Size.width = 20;
                Size.height = 10;
                Slider_.Size.width = 16;
                Slider_.Size.height = (uint)Math.Max((Size.height - 4) * 0.1f, 4);
                Slider_.Position.X = 2;
                Slider_.Position.Y = (int)(((Size.height - 4) - Slider_.Size.height) * Value + 2);
                Slider_.SetSlideBar(this);
            }
        }
        public SlideBarSlider Slider
        {
            get { return Slider_; }
        }
		public Container Parent {
			get;
			set;
		}
    	
		public EElementStyle HorizontalStyle {
			get;
			set;
		}
    	
		public EElementStyle VerticalStyle {
			get;
			set;
		}
        public ETypeOrientation Type
        {
            get;
            private set;
        }
        public void SetPercent(float value)
        {
            Value_ = value;
        }
        public float Value
        {
            get { return (MaxV_ - minV) * Value_ + minV; }
            set
            {
                if (value >= minV && value <= MaxV_)
                {
                    Value_ = (value - minV) / (MaxV_ - minV);
                }
            }
        }
        public float MaxV
        {
            get { return MaxV_; }
        }
        public override Size GetPreferedSize()
        {
            if (Type == ETypeOrientation.Horizontal)
            {
                return new Size(10, 20);
            }
            else
            {
                return new Size(20, 10);
            }
        }
		public override void OnMouseEnter(MouseEvent me)
		{
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (Slider_.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(Slider_, (int)EMouseEventTypes.MOUSE_ENTERED,
                                               mx, my, me.MouseButton);
                Slider_.ProcessMouseEntered(ev);
                return;
            }
		}
        public override void OnMousePress(AssortedWidgets.Events.MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (Slider_.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(Slider_, (int)EMouseEventTypes.MOUSE_PRESSED,
                                               mx, my, me.MouseButton);
                Slider_.ProcessMousePressed(ev);
                return;
            }
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintSlideBar(this);
            UI.Instance.PushPosition(new Position(Position));
            Slider_.Paint();
            UI.Instance.PopPosition();
        }
        public override void Pack()
        {
            if (Type == ETypeOrientation.Horizontal)
            {
                Slider_.Size.width = (uint)Math.Max((Size.width - 4) * 0.1f, 4);
                Slider_.Size.height = 16;
                Slider_.Position.X = (int)(((Size.width - 4) - Slider_.Size.width) * Value_ + 2);
                Slider_.Position.Y = 2;
            }
            else if (Type == ETypeOrientation.Vertical)
            {
                Slider_.Size.width = 16;
                Slider_.Size.height = (uint)Math.Max((Size.height - 4) * 0.1f, 4);
                Slider_.Position.X = 2;
                Slider_.Position.Y = (int)(((Size.height - 4) - Slider_.Size.height) * Value_ + 2);
            }
        }
    }
	#endregion SlideBar

    #region SlideBarSlider

    public class SlideBarSlider : DragAble
    {
        SlideBar parent;

        public SlideBarSlider(ETypeOrientation type)
        {
            Type = type;
        }
        public ETypeOrientation Type
        {
            get;
            private set;
        }
        public void SetSlideBar(SlideBar parent)
        {
            this.parent = parent;
        }
		public override void OnMouseEnter(MouseEvent me)
		{
			Cursor.Current = Cursors.SizeWE;
		}
		public override void OnMousePress(MouseEvent me)
		{
			base.OnMousePress(me);
			
			Cursor.Current = Cursors.SizeWE;
		}
        public override void DragMoved(int offsetX, int offsetY)
        {   	
            base.DragMoved(offsetX, offsetY);
            float m;
     
            Cursor.Current = Cursors.SizeWE;

            if (Type == ETypeOrientation.Horizontal)
            {
                Position.X += offsetX;
                if (Position.X < 2)
                {
                    Position.X = 2;
                }
                else if (Position.X > (int)(parent.Size.width - 2 - Size.width))
                {
                    Position.X = (int)(parent.Size.width - 2 - Size.width);
                }
                m = (float)((Position.X - 2) / (float)(parent.Size.width - 4 - Size.width));
                parent.SetPercent(Math.Min(1.0f, m));
            }
            else
            {
                Position.Y += offsetY;
                if (Position.Y < 2)
                {
                    Position.Y = 2;
                }
                else if (Position.Y > (int)parent.Size.height - 2 - Size.height)
                {
                    Position.Y = (int)(parent.Size.height - 2 - Size.height);
                }
                m = (float)((Position.Y - 2) / (float)(parent.Size.height - 4 - Size.height));
                parent.SetPercent(Math.Min(1.0f, m));
            }
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintSlideBarSlider(this);
        }
    }
    #endregion SlideBarSlider
}
