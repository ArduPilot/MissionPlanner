
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

using AssortedWidgets.Widgets;
using AssortedWidgets.Util;
using AssortedWidgets.Events;

namespace AssortedWidgets.Managers
{
    public class DropListManager
    {
        int currentX;
        int currentY;

        Size size = new Size();
        Position position = new Position();
        public bool isHover = false;

        #region Singleton

        private static object syncRoot = new Object();

        private static DropListManager instance = new DropListManager();
        public static DropListManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DropListManager();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton

        #region Constructor privado

        private DropListManager()
        {
        }
        #endregion Constructor privado

        public DropList Dropped
        {
            get;
            private set;
        }
        public bool IsDropped
        {
            get { return Dropped != null; }
        }
        public void SetCurrent(int currentX, int currentY)
        {
            this.currentX = currentX;
            this.currentY = currentY;
        }
        public bool IsIn(int mx, int my)
        {
            if ((mx > position.X && mx < position.X + size.width) && (my > position.Y && my < position.Y + size.height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ShrinkBack()
        {
            if (Dropped != null)
            {
                Dropped.ShrinkBack();
                Dropped = null;
            }
        }
        public void SetDropped(DropList dropped, int rx, int ry)
        {
            Dropped = dropped;
            position.X = currentX - rx;
            position.Y = currentY - ry + 22;
            size.width = 0;
            size.height = 0;
            uint spacer = Dropped.Spacer;
            List<DropListItem> itemList = Dropped.ItemList;
            int tempY = (int)Dropped.Top;

            foreach (DropListItem iter in itemList)
            {
                Size perfectSize = iter.GetPreferedSize();
                iter.Position.X = (int)Dropped.Left;
                iter.Position.Y = tempY;
                size.width = Math.Max(perfectSize.width, size.width);
                size.height += spacer + perfectSize.height;
                tempY += (int)(perfectSize.height + spacer);
            }

            foreach (DropListItem iter in itemList)
            {
                iter.Size.width = size.width;
            }

            size.width += Dropped.Left + Dropped.Right;
            size.height += Dropped.Top + Dropped.Bottom - spacer;
        }
        public void ImportMouseEntered(MouseEvent e)
        {
            isHover = true;
            ImportMouseMotion(e);
        }
        public void ImportMouseExited(MouseEvent e)
        {
            isHover = false;
            ImportMouseMotion(e);
        }
		public void ImportMousePressed(MouseEvent e)
		{
            int mx = e.X - position.X;
            int my = e.Y - position.Y;

            List<DropListItem> itemList = Dropped.ItemList;

            foreach (DropListItem iter in itemList)
            {
				if(iter.IsIn(mx,my))
				{
					Dropped.SetSelection(iter);
					ShrinkBack();
					return;
				}
			}
			ShrinkBack();
		}
        public void ImportMouseMotion(MouseEvent e)
        {
            int mx = e.X - position.X;
            int my = e.Y - position.Y;

            List<DropListItem> itemList = Dropped.ItemList;

            foreach (DropListItem iter in itemList)
            {
                if (iter.IsIn(mx, my))
                {
                    if (iter.isHover)
                    {

                    }
                    else
                    {
                        MouseEvent ev = new MouseEvent(iter, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my, e.MouseButton);
                        iter.ProcessMouseEntered(ev);
                    }
                }
                else
                {
                    if (iter.isHover)
                    {
                        MouseEvent ev = new MouseEvent(iter, (int)EMouseEventTypes.MOUSE_EXITED, mx, my, e.MouseButton);
                        iter.ProcessMouseExited(ev);
                    }
                }
            }
        }
        public void Paint()
        {
			UI.Instance.CurrentTheme.PaintDropDown(position,size);

			UI.Instance.PushPosition(new Position(position));
			List<DropListItem> itemList= Dropped.ItemList;

            foreach (DropListItem iter in itemList)
            {
				iter.Paint();
			}
			UI.Instance.PopPosition();
        }
    }
}
