
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

using AssortedWidgets.GLFont;
using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
    #region AbstractButton class

    public abstract class AbstractButton : Component, IElement
    {
        public EButtonStatus status;
        protected String Text_ = String.Empty;

        public AbstractButton()
            : this(4, 4, 8, 8, EButtonStatus.Normal)
        {
        }
        public AbstractButton(uint top, uint bottom, uint left, uint right)
            : this(top, bottom, left, right, EButtonStatus.Normal)
        {
        }
        public AbstractButton(uint top, uint bottom, uint left, uint right, EButtonStatus status)
        {
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;

            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            this.status = status;
        }
        
        public String Text
        {
            get { return Text_; }
        }
        public uint Top
        {
            get;
            private set;
        }

        public uint Bottom
        {
            get;
            private set;
        }

        public uint Right
        {
            get;
            private set;
        }

        public uint Left
        {
            get;
            private set;
        }

        public EButtonStatus GetStatus()
        {
            return status;
        }

        public override void OnMousePress(AssortedWidgets.Events.MouseEvent me)
        {
            status = EButtonStatus.Pressed;
            Cursor.Current = Cursors.Hand;
        }
        public override void OnMouseEnter(AssortedWidgets.Events.MouseEvent me)
        {
            isHover = true;
            status = EButtonStatus.Hover;
            Cursor.Current = Cursors.Hand;
        }
        public override void OnMouseMove(AssortedWidgets.Events.MouseEvent me)
        {
            Cursor.Current = Cursors.Hand;
        }
        public override void OnMouseRelease(AssortedWidgets.Events.MouseEvent me)
        {
            status = EButtonStatus.Hover;
            Cursor.Current = Cursors.Hand;
        }
        public override void OnMouseExit(AssortedWidgets.Events.MouseEvent me)
        {
            isHover = false;
            status = EButtonStatus.Normal;
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
    }
    #endregion AbstractButton class

    #region Button class

    public class Button : AbstractButton
    {
        public Button(String text)
            : base(2, 4, 2, 8)
        {
            Text_ = text;
            this.textFont = new Text("Button", UI.Instance.CurrentTheme.defaultTextFont, text);

            Size = GetPreferedSize();
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetButtonPreferedSize(this);
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintButton(this);
        }
    }
    #endregion Button class

    #region CheckButton class

    public class CheckButton : AbstractButton
    {
        public CheckButton(String text)
            :this(text, false)
        {
        }
        public CheckButton(String text, bool check)
        {
            Text_ = text;
            this.textFont = new Text("CheckButton", UI.Instance.CurrentTheme.defaultTextFont, text);

            Check = check;
            Size = GetPreferedSize();
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
        }
        public bool Check
        {
            get;
            set;
        }

        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetCheckButtonPreferedSize(this);
        }

        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintCheckButton(this);
        }
        public override void OnMouseRelease(AssortedWidgets.Events.MouseEvent me)
        {
            base.OnMouseRelease(me);

            Check = !Check;
        }
    }
    #endregion CheckButton class

    #region RadioButton

    public class RadioButton : AbstractButton
    {
        RadioGroup rg;

        public RadioButton(String text, RadioGroup rg)
        {
            Text_ = text;
            this.textFont = new Text("RadioButton", UI.Instance.CurrentTheme.defaultTextFont, text);

            this.rg = rg;
            Size = GetPreferedSize();
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
        }
        public bool Check
        {
            get;
            set;
        }
        public override void OnMouseRelease(AssortedWidgets.Events.MouseEvent me)
        {
            base.OnMouseRelease(me);

            if (Check == false)
                rg.Checked = this;
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetRadioButtonPreferedSize(this);
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintRadioButton(this);
        }
    }
    #endregion RadioButton

    public class RadioGroup
    {
        RadioButton Checked_;

        public RadioButton Checked
        {
            get { return Checked_; }
            set
            {
                if (Checked_ != null)
                {
                    Checked_.Check = false;
                }
                Checked_ = value;
                Checked_.Check = true;
            }
        }
    }

    public enum EButtonStatus
    {
        Normal,
        Hover,
        Pressed
    }
}
