
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
    public class CheckNRadioTestDialog : Dialog
    {
        GirdLayout girdLayout;
        Button closeButton;
        CheckButton checkButton1;
        CheckButton checkButton2;
        CheckButton checkButton3;
        RadioButton radioButton1;
        RadioButton radioButton2;
        RadioButton radioButton3;
        RadioGroup radioGroup;
        Spacer spacer;

        public CheckNRadioTestDialog()
            : base("Check And Radio Test", 100, 100, 320, 200)
        {
            girdLayout = new GirdLayout(4, 2);

            girdLayout.SetHorizontalAlignment(0, 0, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(1, 0, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(2, 0, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(3, 0, EHAlignment.HCenter);

            girdLayout.SetHorizontalAlignment(0, 1, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(1, 1, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(2, 1, EHAlignment.HCenter);
            girdLayout.SetHorizontalAlignment(3, 1, EHAlignment.HCenter);

            girdLayout.SetVerticalAlignment(0, 0, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(1, 0, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(2, 0, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(3, 0, EVAlignment.VCenter);

            girdLayout.SetVerticalAlignment(0, 1, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(1, 1, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(2, 1, EVAlignment.VCenter);
            girdLayout.SetVerticalAlignment(3, 1, EVAlignment.VCenter);

            girdLayout.Right = 16;
            girdLayout.Left = 16;
            girdLayout.Top = 8;
            girdLayout.Bottom = 8;
            girdLayout.Spacer = 4;

            closeButton = new Button("Close");
            checkButton1 = new CheckButton("Check 1");
            checkButton2 = new CheckButton("Check 2");
            checkButton3 = new CheckButton("Check 3");
            radioGroup = new RadioGroup();
            radioButton1 = new RadioButton("Radio 1", radioGroup);
            radioButton2 = new RadioButton("Radio 2", radioGroup);
            radioButton3 = new RadioButton("Radio 3", radioGroup);
            spacer = new Spacer(ESpacerType.Fit);

            Add(checkButton1);
            Add(radioButton1);
            Add(checkButton2);
            Add(radioButton2);
            Add(checkButton3);
            Add(radioButton3);
            Add(spacer);
            Add(closeButton);
            Layout = girdLayout;

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
