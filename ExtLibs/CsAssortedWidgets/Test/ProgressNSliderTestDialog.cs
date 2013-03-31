
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
    public class ProgressNSliderTestDialog : Dialog
    {
        Button closeButton;
        Label valueLabel;
        ProgressBar horizontalPBar;
        ProgressBar verticalPBar;
        SlideBar horizontalSBar;
        SlideBar verticalSBar;
        BorderLayout borderLayout;
        Panel centerPanel;
        GirdLayout girdLayout;

        public ProgressNSliderTestDialog()
            : base("Progress and Slider Test", 150, 150, 320, 200)
        {
            /*
			borderLayout=new BorderLayout(16,16,16,16,8);
			borderLayout.SouthHAlignment = EHorizontalAlignment.HRight;

			closeButton=new Button("Close");
			closeButton.LayoutProperty = EArea.South;

			valueLabel=new Label("Value:0%");
			valueLabel.LayoutProperty = EArea.North;

			centerPanel=new Panel();
			centerGirdLayout=new GirdLayout(2,1);
			centerPanel.Layout = centerGirdLayout;

			horizontalPBar=new ProgressBar(0.0f,100.0f,0.0f);
			horizontalSBar=new SlideBar(0.0f,100.0f,0.0f);

			centerPanel.Add(horizontalPBar);
			centerPanel.Add(horizontalSBar);
			centerPanel.LayoutProperty = EArea.Center;
			centerPanel.Pack();

			verticalPBar=new ProgressBar(0.0f,100.0f,0.0f, ETypeOrientation.Vertical);
			verticalSBar=new SlideBar(0.0f,100.0f,0.0f, ETypeOrientation.Vertical);

			verticalPBar.LayoutProperty = EArea.East;
            verticalSBar.LayoutProperty = EArea.East;

			Add(closeButton);
			Add(valueLabel);
			Add(centerPanel);
			Add(verticalPBar);
			Add(verticalSBar);

			Layout = borderLayout;

			Pack();
            */

            girdLayout = new GirdLayout(4, 1);

            girdLayout.SetHorizontalAlignment(0, 0, EHAlignment.HLeft);
            girdLayout.SetHorizontalAlignment(1, 0, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(2, 0, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(3, 0, EHAlignment.HRight);

            girdLayout.Right = 16;
            girdLayout.Left = 16;
            girdLayout.Top = 8;
            girdLayout.Bottom = 8;
            girdLayout.Spacer = 4;

            closeButton = new Button("Close");
            closeButton.LayoutProperty = EArea.South;

            valueLabel = new Label("Value: 0%");
            valueLabel.LayoutProperty = EArea.North;

            horizontalPBar = new ProgressBar(0.0f, 100.0f, ETypeOrientation.Horizontal);
            horizontalSBar = new SlideBar(0.0f, 100.0f, ETypeOrientation.Horizontal);

            Button button1 = new Button("button1");
            Button button2 = new Button("button2");

            Add(valueLabel);
            Add(horizontalPBar);
            Add(horizontalSBar);
            //Add(button1);
            //Add(button2);
            Add(closeButton);

            Layout = girdLayout;

            Pack();

            horizontalSBar.Slider.DragMovedEvent += new DragMovedHandler(Slider_DragMovedEvent);
            closeButton.MouseReleasedEvent += new MouseReleasedHandler(closeButton_MouseReleasedEvent);
        }

        void closeButton_MouseReleasedEvent(AssortedWidgets.Events.MouseEvent me)
        {
            closeButton.isHover = false;
            closeButton.status = EButtonStatus.Normal;
            Close();
        }

        void Slider_DragMovedEvent(object sender, int offsetX, int offsetY)
        {
            horizontalPBar.Value = horizontalSBar.Value;
            String t = "Value: " + Math.Round(horizontalSBar.Value, 2) + "%";
            //String t = horizontalPBar.Size.width.ToString();
            valueLabel.Text = t;
        }
    }
}
