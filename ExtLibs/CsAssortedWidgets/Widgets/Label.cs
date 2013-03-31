
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

using AssortedWidgets.GLFont;
using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
    public class Label : Component, IElement
    {
        bool drawBackground;
        String Text_;

        public Label(String text)
            :this(text, text.Length * 2)
        {
        }
        public Label(String text, int maxChars)
        {
            Text_ = text;
            this.textFont = new Text("Label", UI.Instance.CurrentTheme.defaultTextFont, maxChars, text);

            Top = 4;
            Right = 10;
            Left = 10;
            Bottom = 4;
            drawBackground = false;
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
            Size = GetPreferedSize();
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
        public void SetDrawBackground(bool drawBackground)
        {
            this.drawBackground = drawBackground;
        }
        public bool IsDrawBackground()
        {
            return drawBackground;
        }
        public String Text
        {
            get { return Text_; }
            set
            {
                textFont.TextValue = value;
            }
        }
        public uint Top
        {
            get;
            private set;
        }

        public uint Left
        {
            get;
            private set;
        }

        public uint Right
        {
            get;
            private set;
        }

        public uint Bottom
        {
            get;
            private set;
        }

        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetLabelPreferedSize(this);
        }

        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintLabel(this);
        }
    }
}
