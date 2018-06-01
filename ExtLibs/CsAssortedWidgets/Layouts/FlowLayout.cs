
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
using System.Collections.Generic;

using AssortedWidgets.Util;
using AssortedWidgets.Widgets;

namespace AssortedWidgets.Layouts
{
    public class FlowLayout : Layout
    {
        public FlowLayout()
            : this(0, 0, 0, 0, 2)
        {
        }
        public FlowLayout(uint spacer)
            : this(0, 0, 0, 0, spacer)
        {
        }
        public FlowLayout(uint top, uint bottom, uint left, uint right, uint spacer)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            Spacer = spacer;
        }
        public override void UpdateLayout(List<Component> componentList, Position origin, Size area)
        {
            if (componentList.Count > 0)
            {
                int tempX = (int)(origin.X + Left);
                int tempY = (int)(origin.Y + Top);
                uint nextY = 0;
                uint width = area.width - Left;
                uint height = area.height - Top - Bottom;

                Size preferedSize = componentList[0].GetPreferedSize();
                componentList[0].Position.X = tempX;
                componentList[0].Position.Y = tempY;
                tempX += (int)(preferedSize.width + Spacer);
                nextY = (uint)Math.Max(nextY, preferedSize.height);

                for (int i = 1; i < componentList.Count; ++i)
                {
                    preferedSize = componentList[i].GetPreferedSize();
                    if ((tempX + preferedSize.width) > width)
                    {
                        tempX = (int)(origin.X + Left);
                        tempY += (int)(nextY + Spacer);
                        nextY = 0;
                        componentList[i].Position.X = tempX;
                        componentList[i].Position.Y = tempY;
                        tempX += (int)(preferedSize.width + Spacer);
                        nextY = (uint)Math.Max(nextY, preferedSize.height);
                    }
                    else
                    {
                        componentList[i].Position.X = tempX;
                        componentList[i].Position.Y = tempY;
                        tempX += (int)(preferedSize.width + Spacer);
                        nextY = (uint)Math.Max(nextY, preferedSize.height);
                    }
                }
            }
        }
        public override Size GetPreferedSize()
        {
            return new Size(10 + Left + Right, 10 + Top + Bottom);
        }
    }
}
