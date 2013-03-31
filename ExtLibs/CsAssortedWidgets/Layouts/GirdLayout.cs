
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
    public class GirdLayout : Layout
    {
        Alignment[][] alignment;
        uint rowCount;
        uint columnCount;

        public GirdLayout(uint _rowCount, uint _columnCount)
        {
            rowCount = _rowCount;
            columnCount = _columnCount;

            alignment = new Alignment[rowCount][];

            for (int i = 0; i < rowCount; ++i)
            {
                alignment[i] = new Alignment[columnCount];
                for (int e = 0; e < columnCount; ++e)
                {
                    alignment[i][e].HAlignment = EHAlignment.HLeft;
                    alignment[i][e].VAlignment = EVAlignment.VTop;
                }
            }
        }
        public void SetHorizontalAlignment(uint rowIndex, uint columnIndex, EHAlignment HAlignment)
        {
            if (rowIndex < rowCount && columnIndex < columnCount)
            {
                alignment[rowIndex][columnIndex].HAlignment = HAlignment;
            }
        }

        public void SetVerticalAlignment(uint i, uint e, EVAlignment VAlignment)
        {
            if (i < rowCount && e < columnCount)
            {
                alignment[i][e].VAlignment = VAlignment;
            }
        }
        public override void UpdateLayout(List<Component> componentList, AssortedWidgets.Util.Position origin, AssortedWidgets.Util.Size area)
        {
            List<Component>.Enumerator clEnum = componentList.GetEnumerator();

            for (int i = 0; i < rowCount; ++i)
            {
                for (int e = 0; e < columnCount; ++e)
                {
                    if (clEnum.MoveNext())
                    {
                        alignment[i][e].component = clEnum.Current;
                        Size perfectSize = clEnum.Current.GetPreferedSize();
                        alignment[i][e].width = perfectSize.width;
                        alignment[i][e].height = perfectSize.height;
                        alignment[i][e].HStyle = ((IElement)clEnum.Current).HorizontalStyle;
                        alignment[i][e].VStyle = ((IElement)clEnum.Current).VerticalStyle;
                    }
                    else
                    {
                        alignment[i][e].component = null;
                        alignment[i][e].width = 0;
                        alignment[i][e].height = 0;
                        alignment[i][e].HStyle = EElementStyle.Fit;
                        alignment[i][e].VStyle = EElementStyle.Fit;
                    }
                }
            }
            OneLineInfo[] columnInfo = new OneLineInfo[columnCount];

            for (int e = 0; e < columnCount; ++e)
            {
                columnInfo[e].miniSize = 0;
                columnInfo[e].isStretch = false;
                for (int i = 0; i < rowCount; ++i)
                {
                    if (alignment[i][e].HStyle == EElementStyle.Stretch)
                    {
                        columnInfo[e].isStretch = true;
                    }
                    columnInfo[e].miniSize = Math.Max(columnInfo[e].miniSize, alignment[i][e].width);
                }
            }

            OneLineInfo[] rowInfo = new OneLineInfo[rowCount];

            for (int i = 0; i < rowCount; ++i)
            {
                rowInfo[i].miniSize = 0;
                rowInfo[i].isStretch = false;
                for (int e = 0; e < columnCount; ++e)
                {
                    if (alignment[i][e].VStyle == EElementStyle.Stretch)
                    {
                        rowInfo[i].isStretch = true;
                    }
                    rowInfo[i].miniSize = Math.Max(rowInfo[i].miniSize, alignment[i][e].height);
                }
            }

            int widthAvailable = (int)(area.width - (columnCount - 1) * Spacer - Left - Right);
            uint stretchSegment = 0;

            for (int e = 0; e < columnCount; ++e)
            {
                if (columnInfo[e].isStretch)
                {
                    ++stretchSegment;
                }
                else
                {
                    widthAvailable -= (int)columnInfo[e].miniSize;
                }
            }

            if (widthAvailable > 0)
            {
                if (stretchSegment > 0)
                {
                    uint averageWidth = (uint)widthAvailable / stretchSegment;

                    for (int e = 0; e < columnCount; ++e)
                    {
                        if (columnInfo[e].isStretch)
                        {
                            columnInfo[e].miniSize = Math.Max(columnInfo[e].miniSize, averageWidth);
                        }
                    }
                }
                else
                {
                    uint averageAppend = (uint)widthAvailable / columnCount;

                    for (int e = 0; e < columnCount; ++e)
                    {
                        columnInfo[e].miniSize += averageAppend;
                    }
                }
            }

            int heightAvailable = (int)(area.height - Top - Bottom - (rowCount - 1) * Spacer);
            stretchSegment = 0;

            for (int i = 0; i < rowCount; ++i)
            {
                if (rowInfo[i].isStretch)
                {
                    ++stretchSegment;
                }
                else
                {
                    heightAvailable -= (int)rowInfo[i].miniSize;
                }
            }

            if (heightAvailable > 0)
            {
                if (stretchSegment > 0)
                {
                    uint averageHeight = (uint)heightAvailable / stretchSegment;

                    for (int i = 0; i < rowCount; ++i)
                    {
                        if (rowInfo[i].isStretch)
                        {
                            rowInfo[i].miniSize = Math.Max(rowInfo[i].miniSize, averageHeight);
                        }
                    }
                }
                else
                {
                    uint averageAppend = (uint)heightAvailable / rowCount;
                    for (int i = 0; i < rowCount; ++i)
                    {
                        rowInfo[i].miniSize += averageAppend;
                    }
                }
            }

            int tempX = (int)Left + origin.X;
            int tempY = (int)Top + origin.Y;

            for (int i = 0; i < rowCount; ++i)
            {
                for (int e = 0; e < columnCount; ++e)
                {
                    Position CPosition = new Position(tempX, tempY);
                    Size Carea = new Size(columnInfo[e].miniSize, rowInfo[i].miniSize);
                    OrderComponent((uint)i, (uint)e, CPosition, Carea);
                    tempX += (int)(columnInfo[e].miniSize + Spacer);
                }
                tempX = (int)Left + origin.X;
                tempY += (int)(Spacer + rowInfo[i].miniSize);
            }
        }
        void OrderComponent(uint row, uint column, Position origin, Size area)
        {
            Alignment compAlignment = alignment[row][column];

            if (compAlignment.component != null)
            {
                if (compAlignment.HStyle == EElementStyle.Stretch)
                {
                    compAlignment.component.Size.width = area.width;
                    compAlignment.component.Position.X = origin.X;
                }
                else
                {
                    switch (compAlignment.HAlignment)
                    {
                        case EHAlignment.HLeft:
                            {
                                compAlignment.component.Position.X = origin.X;
                                break;
                            }
                        case EHAlignment.HCenter:
                            {
                                compAlignment.component.Position.X = (int)(origin.X + (area.width - compAlignment.width) * 0.5f);
                                break;
                            }
                        case EHAlignment.HRight:
                            {
                                compAlignment.component.Position.X = (int)(origin.X + (area.width - compAlignment.width));
                                break;
                            }
                    }
                }

                if (compAlignment.VStyle == EElementStyle.Stretch)
                {
                    compAlignment.component.Size.height = area.height;
                    compAlignment.component.Position.Y = origin.Y;

                }
                else
                {
                    switch (compAlignment.VAlignment)
                    {
                        case EVAlignment.VTop:
                            {
                                compAlignment.component.Position.Y = origin.Y;
                                break;
                            }
                        case EVAlignment.VCenter:
                            {
                                compAlignment.component.Position.Y = (int)(origin.Y + (area.height - compAlignment.height) * 0.5f);
                                break;
                            }
                        case EVAlignment.VBottom:
                            {
                                compAlignment.component.Position.Y = (int)(origin.Y + (area.height - compAlignment.height));
                                break;
                            }
                    }
                }
                compAlignment.component.Pack();
            }
        }

        struct OneLineInfo
        {
            public uint miniSize;
            public bool isStretch;
        }
        struct Alignment
        {
            public EHAlignment HAlignment;
            public EVAlignment VAlignment;
            public Component component;
            public uint width;
            public uint height;
            public EElementStyle HStyle;
            public EElementStyle VStyle;
        }
    }
    public enum EHAlignment
    {
        HLeft,
        HCenter,
        HRight
    }

    public enum EVAlignment
    {
        VTop,
        VCenter,
        VBottom
    }
}
