
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
    public class BorderLayout : Layout
    {
        float testNorthX;
        float testNorthY;
        float testNorthWidth;
        float testNorthHeight;

        public BorderLayout(uint top, uint bottom, uint left, uint right, uint spacer)
            : base(top, bottom, left, right, spacer)
        {
            EastFormat = EFormat.Horizontal;
            SouthFormat = EFormat.Horizontal;
            WestFormat = EFormat.Horizontal;
            NorthFormat = EFormat.Horizontal;
            CenterFormat = EFormat.Horizontal;
            EastHAlignment = EHorizontalAlignment.HLeft;
            SouthHAlignment = EHorizontalAlignment.HLeft;
            WestHAlignment = EHorizontalAlignment.HLeft;
            NorthHAlignment = EHorizontalAlignment.HLeft;
            CenterHAlignment = EHorizontalAlignment.HLeft;
            EastVAlignment = EVerticalAlignment.VCenter;
            SouthVAlignment = EVerticalAlignment.VCenter;
            WestVAlignment = EVerticalAlignment.VCenter;
            NorthVAlignment = EVerticalAlignment.VCenter;
            CenterVAlignment = EVerticalAlignment.VCenter;

            testNorthX = 0;
            testNorthY = 0;
            testNorthWidth = 0;
            testNorthHeight = 0;
        }

        #region Propiedades

        public EFormat EastFormat
        {
            set;
            private get;
        }
        public EFormat SouthFormat
        {
            set;
            private get;
        }
        public EFormat WestFormat
        {
            set;
            private get;
        }
        public EFormat NorthFormat
        {
            set;
            private get;
        }
        public EFormat CenterFormat
        {
            set;
            private get;
        }

        public EHorizontalAlignment EastHAlignment
        {
            set;
            private get;
        }
        public EHorizontalAlignment SouthHAlignment
        {
            set;
            private get;
        }
        public EHorizontalAlignment WestHAlignment
        {
            set;
            private get;
        }
        public EHorizontalAlignment NorthHAlignment
        {
            set;
            private get;
        }
        public EHorizontalAlignment CenterHAlignment
        {
            set;
            private get;
        }

        public EVerticalAlignment EastVAlignment
        {
            set;
            private get;
        }
        public EVerticalAlignment SouthVAlignment
        {
            set;
            private get;
        }
        public EVerticalAlignment WestVAlignment
        {
            set;
            private get;
        }
        public EVerticalAlignment NorthVAlignment
        {
            set;
            private get;
        }
        public EVerticalAlignment CenterVAlignment
        {
            set;
            private get;
        }
        #endregion Propiedades

        public override AssortedWidgets.Util.Size GetPreferedSize()
        {
            return base.GetPreferedSize();
        }
        public override void UpdateLayout(List<AssortedWidgets.Widgets.Component> componentList, AssortedWidgets.Util.Position origin, AssortedWidgets.Util.Size area)
        {
            int iStyle1;
            int iStyle2;

            List<Component> north = new List<Component>(20);
            EElementStyle northHStyle = EElementStyle.Any;
            EElementStyle northVStyle = EElementStyle.Any;
            List<Component> south = new List<Component>(20);
            EElementStyle southHStyle = EElementStyle.Any;
            EElementStyle southVStyle = EElementStyle.Any;
            List<Component> west = new List<Component>(20);
            EElementStyle westHStyle = EElementStyle.Any;
            EElementStyle westVStyle = EElementStyle.Any;
            List<Component> east = new List<Component>(20);
            EElementStyle eastHStyle = EElementStyle.Any;
            EElementStyle eastVStyle = EElementStyle.Any;
            List<Component> center = new List<Component>(20);
            EElementStyle centerHStyle = EElementStyle.Any;
            EElementStyle centerVStyle = EElementStyle.Any;

            foreach (Component comp in componentList)
            {
                switch (comp.LayoutProperty)
                {
                    case EArea.North:
                        {
                            north.Add(comp);
                            iStyle1 = (int)northHStyle;
                            iStyle2 = (int)(((IElement)comp).HorizontalStyle);
                            northHStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            iStyle1 = (int)northVStyle;
                            iStyle2 = (int)(((IElement)comp).VerticalStyle);
                            northVStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            break;
                        }
                    case EArea.South:
                        {
                            south.Add(comp);
                            iStyle1 = (int)southHStyle;
                            iStyle2 = (int)(((IElement)comp).HorizontalStyle);
                            southHStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            iStyle1 = (int)southVStyle;
                            iStyle2 = (int)(((IElement)comp).VerticalStyle);
                            southVStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            break;
                        }
                    case EArea.West:
                        {
                            west.Add(comp);
                            iStyle1 = (int)westHStyle;
                            iStyle2 = (int)(((IElement)comp).HorizontalStyle);
                            westHStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            iStyle1 = (int)westVStyle;
                            iStyle2 = (int)(((IElement)comp).VerticalStyle);
                            westVStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            break;
                        }
                    case EArea.East:
                        {
                            east.Add(comp);
                            iStyle1 = (int)eastHStyle;
                            iStyle2 = (int)(((IElement)comp).HorizontalStyle);
                            eastHStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            iStyle1 = (int)eastVStyle;
                            iStyle2 = (int)(((IElement)comp).VerticalStyle);
                            eastVStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            break;
                        }
                    case EArea.Center:
                        {
                            center.Add(comp);
                            iStyle1 = (int)centerHStyle;
                            iStyle2 = (int)(((IElement)comp).HorizontalStyle);
                            centerHStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            iStyle1 = (int)centerVStyle;
                            iStyle2 = (int)(((IElement)comp).VerticalStyle);
                            centerVStyle = (EElementStyle)Math.Max(iStyle1, iStyle2);
                            break;
                        }
                }
            }
            uint width = area.width - Left - Right;
            uint height = area.height - Top - Bottom;

            int tempX = (int)(origin.X + Left);
            int tempY = (int)(origin.Y + Top);

            uint westHeight = GetPreferedHeight(west, WestFormat);
            uint centerHeight = GetPreferedHeight(center, CenterFormat);
            uint eastHeight = GetPreferedHeight(east, EastFormat);
            uint northHeight = GetPreferedHeight(north, NorthFormat);
            uint southHeight = GetPreferedHeight(south, SouthFormat);

            uint heightAvailable = area.height - Top - Bottom - Spacer - Spacer;
            heightAvailable = Math.Max(heightAvailable, Math.Max(Math.Max(westHeight, eastHeight), centerHeight) + northHeight + southHeight);
            int strechAreaCount = 1;

            if (northVStyle == EElementStyle.Stretch)
            {
                ++strechAreaCount;
            }
            else
            {
                heightAvailable -= northHeight;
            }

            if (southVStyle == EElementStyle.Stretch)
            {
                ++strechAreaCount;
            }
            else
            {
                heightAvailable -= southHeight;
            }

            uint averageHeight = (uint)(heightAvailable / strechAreaCount);

            if (northVStyle == EElementStyle.Stretch)
            {
                northHeight = Math.Max(northHeight, averageHeight);
            }
            if (southVStyle == EElementStyle.Stretch)
            {
                southHeight = Math.Max(southHeight, averageHeight);
            }

            westHeight = centerHeight = eastHeight = Math.Max(Math.Max(westHeight, eastHeight), Math.Max(centerHeight, averageHeight));

            uint northWidth = GetPreferedWidth(north, NorthFormat);
            uint southWidth = GetPreferedWidth(south, SouthFormat);
            uint eastWidth = GetPreferedWidth(east, EastFormat);
            uint westWidth = GetPreferedWidth(west, WestFormat);
            uint centerWidth = GetPreferedWidth(center, CenterFormat);

            uint widthAvailable = area.width - Left - Right;
            widthAvailable = Math.Max(widthAvailable, Math.Max(westWidth + eastWidth + centerWidth + Spacer + Spacer, Math.Max(northWidth, southWidth)));
            northWidth = southWidth = widthAvailable;
            widthAvailable -= Spacer + Spacer;

            strechAreaCount = 1;

            if (westHStyle == EElementStyle.Stretch)
            {
                ++strechAreaCount;
            }
            else
            {
                widthAvailable -= westWidth;
            }

            if (eastHStyle == EElementStyle.Stretch)
            {
                ++strechAreaCount;
            }
            else
            {
                widthAvailable -= eastWidth;
            }

            uint averageWidth = (uint)(widthAvailable / strechAreaCount);
            if (westHStyle == EElementStyle.Stretch)
            {
                westWidth = averageWidth;
            }
            if (eastHStyle == EElementStyle.Stretch)
            {
                eastWidth = averageWidth;
            }
            centerWidth = Math.Max(averageWidth, centerWidth);

            Position northPosition = new Position((int)(origin.X + Left), (int)(origin.Y + Top));
            Size northArea = new Size(northWidth, northHeight);

            OrderComponents(north, NorthHAlignment, NorthVAlignment, NorthFormat, northPosition, northArea);

            Position southPosition = new Position((int)(origin.X + Left), (int)(origin.Y + Top + Spacer + centerHeight + Spacer + northHeight));
            Size southArea = new Size(southWidth, southHeight);
            OrderComponents(south, SouthHAlignment, SouthVAlignment, SouthFormat, southPosition, southArea);

            Position westPosition = new Position((int)(origin.X + Left), (int)(origin.Y + Top + northHeight + Spacer));
            Size westArea = new Size(westWidth, westHeight);
            OrderComponents(west, WestHAlignment, WestVAlignment, WestFormat, westPosition, westArea);

            Position eastPosition = new Position((int)(origin.X + Left + westWidth + Spacer + centerWidth + Spacer), (int)(origin.Y + Top + northHeight + Spacer));
            Size eastArea = new Size(eastWidth, eastHeight);

            testNorthX = eastPosition.X;
            testNorthY = eastPosition.Y;
            testNorthWidth = eastArea.width;
            testNorthHeight = eastArea.height;

            OrderComponents(east, EastHAlignment, EastVAlignment, EastFormat, eastPosition, eastArea);

            Position centerPosition = new Position((int)(origin.X + Left + Spacer + westWidth), (int)(origin.Y + Spacer + northHeight + Top));
            Size centerArea = new Size(centerWidth, centerHeight);

            OrderComponents(center, CenterHAlignment, CenterVAlignment, CenterFormat, centerPosition, centerArea);
        }
        void OrderComponents(List<Component> list,
                             EHorizontalAlignment HAlignment, EVerticalAlignment VAlignment,
                             EFormat format, Position origin, Size area)
        {
            if (list.Count > 0)
            {
                if (format == EFormat.Horizontal)
                {
                    switch (HAlignment)
                    {
                        case EHorizontalAlignment.HLeft:
                            {
                                int strechSegment = 0;
                                uint widthTakenUp = 0;
                                foreach (Component ele in list)
                                {
                                    if (((IElement)ele).HorizontalStyle == EElementStyle.Stretch)
                                    {
                                        ++strechSegment;
                                    }
                                    else
                                    {
                                        Size perfectSize = ele.GetPreferedSize();
                                        widthTakenUp += perfectSize.width;
                                    }
                                }

                                uint widthAvailable = (uint)(area.width - Spacer * (list.Count - 1) - widthTakenUp);
                                uint averageWidth = 0;
                                if (strechSegment > 0)
                                {
                                    averageWidth = (uint)(widthAvailable / strechSegment);
                                }

                                int tempX = origin.X;
                                foreach (Component comp in list)
                                {
                                    Size perfectSize = comp.GetPreferedSize();
                                    if (((IElement)comp).HorizontalStyle == EElementStyle.Fit)
                                    {
                                        comp.Position.X = tempX;
                                        comp.Size.width = perfectSize.width;
                                        tempX += (int)(Spacer + perfectSize.width);
                                    }
                                    else if (((IElement)comp).HorizontalStyle == EElementStyle.Stretch)
                                    {
                                        comp.Position.X = tempX;
                                        comp.Size.width = averageWidth;
                                        tempX += (int)(Spacer + averageWidth);
                                    }
                                }
                                break;
                            }
                        case EHorizontalAlignment.HRight:
                            {
                                int strechSegment = 0;
                                uint widthTakenUp = 0;
                                foreach (Component ele in list)
                                {
                                    if (((IElement)ele).HorizontalStyle == EElementStyle.Stretch)
                                    {
                                        ++strechSegment;
                                    }
                                    else
                                    {
                                        Size perfectSize = ele.GetPreferedSize();
                                        widthTakenUp += perfectSize.width;
                                    }
                                }

                                uint widthAvailable = (uint)(area.width - Spacer * (list.Count - 1) - widthTakenUp);
                                uint averageWidth = 0;
                                if (strechSegment > 0)
                                {
                                    averageWidth = (uint)(widthAvailable / strechSegment);
                                }

                                int tempX = (int)(origin.X + area.width);

                                for (int i = list.Count - 1; i >= 0; --i)
                                {
                                    Component iter = list[i];
                                    Size perfectSize = iter.GetPreferedSize();
                                    if (((IElement)iter).HorizontalStyle == EElementStyle.Fit)
                                    {
                                        tempX -= (int)perfectSize.width;
                                        iter.Position.X = tempX;
                                        iter.Size.width = perfectSize.width;
                                        tempX -= (int)Spacer;
                                    }
                                    else if (((IElement)iter).HorizontalStyle == EElementStyle.Stretch)
                                    {
                                        tempX -= (int)averageWidth;
                                        iter.Position.X = tempX;
                                        iter.Size.width = averageWidth;
                                        tempX -= (int)Spacer;
                                    }
                                }
                                break;
                            }
                        case EHorizontalAlignment.HCenter:
                            {
                                bool isStretch = false;
                                int strechSegment = 0;
                                uint widthTakenUp = 0;
                                foreach (Component ele in list)
                                {
                                    if (((IElement)ele).HorizontalStyle == EElementStyle.Stretch)
                                    {
                                        ++strechSegment;
                                        isStretch = true;
                                    }
                                    else
                                    {
                                        Size perfectSize = ele.GetPreferedSize();
                                        widthTakenUp += perfectSize.width;
                                    }
                                }

                                if (isStretch)
                                {
                                    uint widthAvailable = (uint)(area.width - Spacer * (list.Count - 1) - widthTakenUp);
                                    uint averageWidth = (uint)(widthAvailable / strechSegment);
                                    int tempX = origin.X;

                                    for (int iter = 0; iter < list.Count - 1; ++iter)
                                    {
                                        Component comp = list[iter];

                                        Size perfectSize = comp.GetPreferedSize();
                                        if (((IElement)comp).HorizontalStyle == EElementStyle.Fit)
                                        {
                                            comp.Position.X = tempX;
                                            comp.Size.width = perfectSize.width;
                                            tempX += (int)(Spacer + perfectSize.width);
                                        }
                                        else if (((IElement)comp).HorizontalStyle == EElementStyle.Stretch)
                                        {
                                            comp.Position.X = tempX;
                                            comp.Size.width = averageWidth;
                                            tempX += (int)(Spacer + averageWidth);
                                        }
                                    }
                                }
                                else
                                {
                                    widthTakenUp += (uint)(Spacer * (list.Count - 1));
                                    int tempX = (int)(origin.X + area.width * 0.5f - widthTakenUp * 0.5f);

                                    foreach (Component comp in list)
                                    {
                                        Size perfectSize = comp.GetPreferedSize();
                                        comp.Position.X = tempX;
                                        comp.Size.width = perfectSize.width;
                                        tempX += (int)(Spacer + perfectSize.width);
                                    }
                                }
                                break;
                            }
                    }

                    switch (VAlignment)
                    {
                        case EVerticalAlignment.VTop:
                            {
                                int tempY = origin.Y;
                                foreach (Component comp in list)
                                {
                                    Size perfectSize = comp.GetPreferedSize();
                                    if (((IElement)comp).VerticalStyle == EElementStyle.Stretch)
                                    {
                                        comp.Position.Y = tempY;
                                        comp.Size.height = area.height;
                                    }
                                    else if (((IElement)comp).VerticalStyle == EElementStyle.Fit)
                                    {
                                        comp.Position.Y = tempY;
                                        comp.Size.height = perfectSize.height;
                                    }
                                }
                                break;
                            }
                        case EVerticalAlignment.VBottom:
                            {
                                int tempY = origin.Y;
                                foreach (Component comp in list)
                                {
                                    Size perfectSize = comp.GetPreferedSize();
                                    if (((IElement)comp).VerticalStyle == EElementStyle.Stretch)
                                    {
                                        comp.Position.Y = tempY;
                                        comp.Size.height = area.height;
                                    }
                                    else if (((IElement)comp).VerticalStyle == EElementStyle.Fit)
                                    {
                                        comp.Position.Y = (int)(tempY + area.height - perfectSize.height);
                                        comp.Size.height = perfectSize.height;
                                    }
                                }
                                break;
                            }
                        case EVerticalAlignment.VCenter:
                            {
                                int tempY = origin.Y;
                                foreach (Component comp in list)
                                {
                                    Size perfectSize = comp.GetPreferedSize();
                                    if (((IElement)comp).VerticalStyle == EElementStyle.Stretch)
                                    {
                                        comp.Position.Y = tempY;
                                        comp.Size.height = area.height;
                                    }
                                    else if (((IElement)comp).VerticalStyle == EElementStyle.Fit)
                                    {
                                        comp.Position.Y = (int)(tempY + area.height * 0.5 - perfectSize.height * 0.5);
                                        comp.Size.height = perfectSize.height;
                                    }
                                }
                                break;
                            }
                    }
                }
                else if (format == EFormat.Vertical)
                {

                }

                foreach (Component ele in list)
                {
                    ele.Pack();
                }
            }
        }
        uint GetPreferedWidth(List<Component> list, EFormat format)
        {
            uint resultWidth = 0;
            if (list.Count > 0)
            {
                if (format == EFormat.Horizontal)
                {
                    foreach (Component ele in list)
                    {
                        Size perfectSize = ele.GetPreferedSize();
                        resultWidth += Spacer + perfectSize.width;
                    }
                    resultWidth -= Spacer;
                }
                else if (format == EFormat.Vertical)
                {
                    foreach (Component ele in list)
                    {
                        Size perfectSize = ele.GetPreferedSize();
                        resultWidth = Math.Max(resultWidth, perfectSize.width);
                    }
                }
            }
            return resultWidth;
        }

        uint GetPreferedHeight(List<Component> list, EFormat format)
        {
            uint resultHeight = 0;

            if (list.Count > 0)
            {
                if (format == EFormat.Horizontal)
                {
                    foreach (Component ele in list)
                    {
                        Size perfectSize = ele.GetPreferedSize();
                        resultHeight = Math.Max(resultHeight, perfectSize.height);
                    }
                }
                else if (format == EFormat.Vertical)
                {
                    foreach (Component ele in list)
                    {
                        Size perfectSize = ele.GetPreferedSize();
                        resultHeight += Spacer + perfectSize.height;
                    }
                    resultHeight -= Spacer;
                }
            }
            return resultHeight;
        }
    }
    public enum EHorizontalAlignment
    {
        HLeft,
        HCenter,
        HRight,
    }

    public enum EVerticalAlignment
    {
        VTop,
        VCenter,
        VBottom
    }
    public enum EFormat
    {
        Horizontal,
        Vertical
    }
    public enum EArea
    {
        East,
        South,
        West,
        North,
        Center
    }
}
