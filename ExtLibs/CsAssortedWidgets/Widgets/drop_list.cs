
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

using AssortedWidgets.GLFont;
using AssortedWidgets.Util;
using AssortedWidgets.Events;
using AssortedWidgets.Managers;

namespace AssortedWidgets.Widgets
{
    public class DropList : Component, IElement
    {
        DropListButton button = new DropListButton();
        List<DropListItem> ItemList_ = new List<DropListItem>();

        public DropList()
        {
            Spacer = 2;
            Left = 4;
            Right = 4;
            Top = 4;
            Bottom = 4;
            Dropped = false;

            Size = GetPreferedSize();
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
            button.Position.X = (int)(Size.width - 18);
            button.Position.Y = 2;

            button.MouseReleasedEvent += new MouseReleasedHandler(button_DropReleasedEvent);
        }

        void button_DropReleasedEvent(MouseEvent me)
        {
            if (Dropped)
            {
                DropListManager.Instance.ShrinkBack();
                Dropped = false;
            }
            else
            {
                DropListManager.Instance.SetDropped(this, me.X, me.Y);
                Dropped = true;
            }
        }
        public uint Spacer
        {
            get;
            private set;
        }
        public uint Top
        {
            get;
            private set;
        }

        public uint Bottom
        {
            get;
            private set;
        }

        public uint Right
        {
            get;
            private set;
        }

        public uint Left
        {
            get;
            private set;
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

        public bool Dropped
        {
            get;
            private set;
        }
        public void ShrinkBack()
        {
            Dropped = false;
        }
        public List<DropListItem> ItemList
        {
            get { return ItemList_; }
        }
        public DropListItem SelectedItem
        {
            get;
            private set;
        }
        public void Add(DropListItem item)
        {
            ItemList_.Add(item);
            Size = GetPreferedSize();
        }
        public void SetSelection(int index)
        {
            SelectedItem = ItemList_[index];
        }
        public void SetSelection(DropListItem selected)
        {
            SelectedItem = selected;
        }
        public override Size GetPreferedSize()
        {
            uint miniSize = 0;
            foreach (DropListItem iter in ItemList_)
            {
                miniSize = Math.Max(iter.GetPreferedSize().width, miniSize);
            }
            return new Size(miniSize + 23, 20);
        }
        public override void Pack()
        {
            button.Position.X = (int)(Size.width - 18);
            button.Position.Y = 2;
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintDropList(this);
            UI.Instance.PushPosition(new Position(Position));
            button.Paint();
            UI.Instance.PopPosition();
        }
        public override void OnMouseEnter(AssortedWidgets.Events.MouseEvent me)
        {
            isHover = true;
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (button.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my, me.MouseButton);
                button.ProcessMouseEntered(ev);
                return;
            }
        }
        public override void OnMousePress(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (button.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_PRESSED, mx, my, me.MouseButton);
                button.ProcessMousePressed(ev);
                return;
            }
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (button.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_RELEASED, mx, my, me.MouseButton);
                button.ProcessMouseReleased(ev);
                return;
            }
        }
        public override void OnMouseExit(MouseEvent me)
        {
            isHover = false;

            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (button.isHover)
            {
                MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_EXITED, mx, my, me.MouseButton);
                button.ProcessMouseExited(ev);
                return;
            }
        }
        public override void OnMouseMove(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (button.IsIn(mx, my))
            {
                if (button.isHover == false)
                {
                    MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my, me.MouseButton);
                    button.ProcessMouseEntered(ev);
                }
            }
            else
            {
                if (button.isHover)
                {
                    MouseEvent ev = new MouseEvent(button, (int)EMouseEventTypes.MOUSE_EXITED, mx, my, me.MouseButton);
                    button.ProcessMouseExited(ev);
                }
            }
        }
    }
    public class DropListButton : AbstractButton
    {
        public DropListButton()
        {
            this.Size = GetPreferedSize();
            HorizontalStyle = EElementStyle.Fit;
            VerticalStyle = EElementStyle.Fit;
        }
        public override Size GetPreferedSize()
        {
		    return new Size(15,15);
        }
        public override void Paint()
        {
		    UI.Instance.CurrentTheme.PaintDropListButton(this);
        }
    }
    public class DropListItem : AbstractButton
    {
        public DropListItem(String text)
        {
            Text_ = text;
            this.textFont = new Text("DropListItem", UI.Instance.CurrentTheme.defaultTextFont, text);

            Size = GetPreferedSize();
        }
        public override Size GetPreferedSize()
        {
		    return UI.Instance.CurrentTheme.GetDropListItemPreferedSize(this);
        }
        public override void Paint()
        {
		    UI.Instance.CurrentTheme.PaintDropListItem(this);
        }
    }
}
