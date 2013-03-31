
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

using AssortedWidgets.Layouts;
using AssortedWidgets.Widgets;

namespace AssortedWidgets.Test
{
    public class ScrollPanelTestDialog : Dialog
    {
        Button closeButton;
        ScrollPanel panel;
        GirdLayout girdLayout;

        public ScrollPanelTestDialog()
            : base("Scroll Panel Test", 400, 400, 320, 240)
        {
			girdLayout=new GirdLayout(2,1);
			girdLayout.Right = 16;
			girdLayout.Left = 16;
			girdLayout.Top = 8;
			girdLayout.Bottom = 8;
			girdLayout.Spacer = 4;

			girdLayout.SetHorizontalAlignment(1,0, EHAlignment.HRight);

			closeButton=new Button("Close");
			panel=new ScrollPanel();

			Layout = girdLayout;
			Add(panel);
			Add(closeButton);

			Pack();

            closeButton.MouseReleasedEvent += new MouseReleasedHandler(closeButton_MouseReleasedEvent);
        }

        void closeButton_MouseReleasedEvent(AssortedWidgets.Events.MouseEvent me)
        {
            closeButton.isHover = false;
            closeButton.status = EButtonStatus.Normal;
            Close();
        }
    }
}
