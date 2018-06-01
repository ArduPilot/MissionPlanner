
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
    public class MultipleLayoutTestDialog : Dialog
    {
        GirdLayout girdLayout;
        FlowLayout flowLayout;
        Button closeButton;

        Label TheLabel;
        Label quickLabel;
        Label brownLabel;
        Label foxLabel;
        Label jumpsLabel;
        Label overLabel;
        Label theLabel;
        Label lazyDogLabel;

        Label northLabel;
        Label southLabel;
        Label westLabel;
        Label eastLabel;
        Label centerLabel;
        BorderLayout borderLayout;

        Panel flowPanel;
        Panel borderPanel;

        public MultipleLayoutTestDialog()
            : base("MultipleLayout Test", 350, 350, 400, 180)
        {
			girdLayout=new GirdLayout(1,2);
			girdLayout.Right = 16;
			girdLayout.Left = 16;
			girdLayout.Top = 8;
			girdLayout.Bottom = 8;
			girdLayout.Spacer = 4;

			flowLayout=new FlowLayout(2,2,2,2,4);			

			TheLabel=new Label("The");
			TheLabel.SetDrawBackground(true);

			quickLabel=new Label("quick");
			quickLabel.SetDrawBackground(true);

			brownLabel=new Label("brown");
			brownLabel.SetDrawBackground(true);

			foxLabel=new Label("Fox");
			foxLabel.SetDrawBackground(true);

			jumpsLabel=new Label("jumps");
			jumpsLabel.SetDrawBackground(true);

			overLabel=new Label("over");
			overLabel.SetDrawBackground(true);

			theLabel=new Label("a");
			theLabel.SetDrawBackground(true);

			lazyDogLabel=new Label("lazy dog.");
			lazyDogLabel.SetDrawBackground(true);

			flowPanel=new Panel();
			flowPanel.Layout = flowLayout;
			flowPanel.Add(TheLabel);
			flowPanel.Add(quickLabel);
			flowPanel.Add(brownLabel);
			flowPanel.Add(foxLabel);
			flowPanel.Add(jumpsLabel);
			flowPanel.Add(overLabel);
			flowPanel.Add(theLabel);
			flowPanel.Add(lazyDogLabel);
		
			flowPanel.Pack();

			borderLayout=new BorderLayout(2,2,2,2,4);
			closeButton=new Button("Close");
			closeButton.LayoutProperty = EArea.South;

			northLabel=new Label("North");
			northLabel.HorizontalStyle = EElementStyle.Stretch;
			northLabel.SetDrawBackground(true);
			northLabel.LayoutProperty = EArea.North;

			southLabel=new Label("South");
			southLabel.HorizontalStyle = EElementStyle.Stretch;
			southLabel.SetDrawBackground(true);
			southLabel.LayoutProperty = EArea.South;

			westLabel=new Label("West");
			westLabel.VerticalStyle = EElementStyle.Stretch;
			westLabel.SetDrawBackground(true);
			westLabel.LayoutProperty = EArea.West;

			eastLabel=new Label("East");
			eastLabel.VerticalStyle = EElementStyle.Stretch;
			eastLabel.SetDrawBackground(true);
			eastLabel.LayoutProperty = EArea.East;

			centerLabel=new Label("Center");
			centerLabel.HorizontalStyle = EElementStyle.Stretch;
            centerLabel.VerticalStyle = EElementStyle.Stretch;
			centerLabel.SetDrawBackground(true);
			centerLabel.LayoutProperty = EArea.Center;

			borderPanel=new Panel();
			borderPanel.Layout = borderLayout;

			borderPanel.Add(northLabel);
			borderPanel.Add(southLabel);
			borderPanel.Add(closeButton);
			borderPanel.Add(westLabel);
			borderPanel.Add(eastLabel);
			borderPanel.Add(centerLabel);

			borderPanel.Pack();
			
			Layout = girdLayout;
			Add(flowPanel);
			Add(borderPanel);
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
