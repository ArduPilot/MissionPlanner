
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

using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
    public class ScrollPanel : Component, IElement
    {
        uint scissorWidth;
        uint scissorHeight;

        public ScrollPanel()
        {
            HorizontalStyle = EElementStyle.Stretch;
            VerticalStyle = EElementStyle.Stretch;
        }

        #region Miembros de IElement

        public Container Parent
        {
            get;
            set;
        }

        public EElementStyle HorizontalStyle
        {
            get;
            set;
        }

        public EElementStyle VerticalStyle
        {
            get;
            set;
        }

        #endregion

        public override AssortedWidgets.Util.Size GetPreferedSize()
        {
            return new Size(60, 60);
        }
        public override void Pack()
        {
            scissorWidth = Size.width - 2;
            scissorHeight = Size.height - 2;
        }
        public override void Paint()
        {
			UI.Instance.CurrentTheme.PaintScrollPanel(this);
			UI.Instance.PushPosition(new Position(Position));

            Position sPosition = new Position(2,2);
			Size sArea = new Size(scissorWidth,scissorHeight);
			UI.Instance.CurrentTheme.ScissorBegin(sPosition,sArea);
			/*if(content)
			{
				content->paint();
			}*/
			UI.Instance.CurrentTheme.ScissorEnd();
            UI.Instance.PopPosition();
        }
    }
    public enum EScrollStyle
    {
        Auto,
        Never
    }
}
