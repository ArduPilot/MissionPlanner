
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
using System.Drawing;
using System.Windows.Forms;

using AssortedWidgets.Events;
using AssortedWidgets.GLFont;
using AssortedWidgets.Managers;
using AssortedWidgets.Util;

using Size = AssortedWidgets.Util.Size;

namespace AssortedWidgets.Widgets
{
    #region class Dialog

    public class Dialog : Container
    {
        DialogTittleBar tittleBar;
        DialogBorder borderUpLeft;
        DialogBorder borderUpRight;
        DialogBorder borderUp;
        DialogBorder borderLeft;
        DialogBorder borderRight;
        DialogBorder borderBottomLeft;
        DialogBorder borderBottom;
        DialogBorder borderBottomRight;

        DialogBorder[] dialogBorders = new DialogBorder[8];

        bool active;

        EShowType showType;

        uint top;
        uint bottom;
        uint left;
        uint right;

        Position contentPosition;
        Size contentSize;

        Cursor currentCursor = Cursors.Default;

        #region Constructor

        public Dialog(String title, int x, int y, uint width, uint height)
        {
            tittleBar = new DialogTittleBar(title);

            top = 12;
            bottom = 14;
            left = 12;
            right = 12;

            borderUpLeft = new DialogBorder(9, 7, 4, 4);
            borderUpRight = new DialogBorder((int)(width - 9 - 4), 7, 4, 4);
            borderUp = new DialogBorder(13, 7, width - 26, 4);
            borderLeft = new DialogBorder(9, 11, 4, height - 27);
            borderRight = new DialogBorder((int)(width - 13), 11, 4, height - 27);
            borderBottomLeft = new DialogBorder(9, (int)(height - 27), 4, 4);
            borderBottom = new DialogBorder(13, (int)(height - 27), width - 26, 4);
            borderBottomRight = new DialogBorder((int)(width - 13), (int)(height - 27), 4, 4);

            dialogBorders[0] = borderUpLeft;
            dialogBorders[1] = borderUpRight;
            dialogBorders[2] = borderUp;
            dialogBorders[3] = borderLeft;
            dialogBorders[4] = borderRight;
            dialogBorders[5] = borderBottomLeft;
            dialogBorders[6] = borderBottom;
            dialogBorders[7] = borderBottomRight;

            Dragable = true;
            Resizable = true;
            showType = EShowType.None;

            Position.X = x;
            Position.Y = y;
            Size.width = width;
            Size.height = height;

            Pack();

            tittleBar.DragMovedEvent += new DragMovedHandler(tittleBar_DragMovedEvent);

            borderLeft.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderLeft.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderLeft.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderRight.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderRight.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderRight.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderUp.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderUp.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderUp.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderBottom.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderBottom.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderBottom.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);
            //===========================================================================

            borderUpLeft.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderUpLeft.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderUpLeft.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderUpRight.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderUpRight.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderUpRight.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderBottomLeft.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderBottomLeft.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderBottomLeft.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);

            borderBottomRight.MouseEnteredEvent += new MouseEnteredHandler(border_MouseEvent);
            borderBottomRight.MousePressedEvent += new MousePressedHandler(border_MouseEvent);
            borderBottomRight.DragMovedEvent += new DragMovedHandler(border_DragMovedEvent);
        }
        #endregion Constructor

        #region ReSize & Drag

        void SetCurrentCursor(Cursor cur)
        {
            currentCursor = cur;
            Cursor.Current = cur;
        }
        void border_MouseEvent(MouseEvent me)
        {
            if ((DialogBorder)me.Source == borderLeft || (DialogBorder)me.Source == borderRight)
                SetCurrentCursor(Cursors.SizeWE);
            else if ((DialogBorder)me.Source == borderUp || (DialogBorder)me.Source == borderBottom)
                SetCurrentCursor(Cursors.SizeNS);
            else if ((DialogBorder)me.Source == borderUpLeft || (DialogBorder)me.Source == borderBottomRight)
                SetCurrentCursor(Cursors.SizeNWSE);
            else if ((DialogBorder)me.Source == borderUpRight || (DialogBorder)me.Source == borderBottomLeft)
                SetCurrentCursor(Cursors.SizeNESW);
        }
        
        void border_DragMovedEvent(object sender, int offsetX, int offsetY)
        {
            Size minimize = GetPreferedSize();
            
            DialogBorder db = sender as DialogBorder;

            if (db == borderLeft)
            {
          		if ((Size.width - offsetX) > minimize.width)
                {
                	Position.X += offsetX;
                    Size.width -= (uint)offsetX;
                }
            }
            else if ((DialogBorder)sender == borderRight)
            {
                if ((Size.width + offsetX) > minimize.width)
                {
                    Size.width += (uint)offsetX;
                }
            }
            else if ((DialogBorder)sender == borderUp)
            {
                if ((Size.height - offsetY) > minimize.height)
                {
                    Position.Y += offsetY;
                    Size.height -= (uint)offsetY;
                }
            }
            else if ((DialogBorder)sender == borderBottom)
            {
                if ((Size.height + offsetY) > minimize.height)
                {
                    Size.height += (uint)offsetY;
                }
            }
            else if ((DialogBorder)sender == borderUpLeft)
            {
                if ((Size.width - offsetX) > minimize.width)
                {
                    Position.X += offsetX;
                    Size.width -= (uint)offsetX;
                }
                if ((Size.height - offsetY) > minimize.height)
                {
                    Position.Y += offsetY;
                    Size.height -= (uint)offsetY;
                }
            }
            else if ((DialogBorder)sender == borderBottomRight)
            {
                if ((Size.width + offsetX) > minimize.width)
                {
                    Size.width += (uint)offsetX;
                }
                if ((Size.height + offsetY) > minimize.height)
                {
                    Size.height += (uint)offsetY;
                }
            }
            else if ((DialogBorder)sender == borderUpRight)
            {
                if ((Size.width + offsetX) > minimize.width)
                {
                    Size.width += (uint)offsetX;
                }
                if ((Size.height - offsetY) > minimize.height)
                {
                    Position.Y += offsetY;
                    Size.height -= (uint)offsetY;
                }
            }
            else if ((DialogBorder)sender == borderBottomLeft)
            {
                if ((Size.width - offsetX) > minimize.width)
                {
                    Position.X += offsetX;
                    Size.width -= (uint)offsetX;
                }
                if ((Size.height + offsetY) > minimize.height)
                {
                    Size.height += (uint)offsetY;
                }
            }
            
            Pack();

            Cursor.Current = currentCursor;
        }

        void tittleBar_DragMovedEvent(object sender, int offsetX, int offsetY)
        {
            Position.X += offsetX;
            Position.Y += offsetY;
        }
        #endregion ReSize & Drag

        protected bool Resizable
        {
            get;
            set;
        }
        protected bool Dragable
        {
            get;
            set;
        }
        public EShowType GetShowType()
        {
            return showType;
        }
        public void SetShowType(EShowType showType)
        {
            this.showType = showType;
        }
        public virtual void SetActive(bool active)
        {
            this.active = active;
        }
        public bool IsActive()
        {
            return active;
        }
        public override Size GetPreferedSize()
        {
            Size result = new Size(tittleBar.GetPreferedSize());
            result.width += left + right;
            result.height += top + bottom;
            return result;
        }
        public override void Pack()
        {
            tittleBar.Position.X = (int)left;
            tittleBar.Position.Y = (int)top;
            tittleBar.Size.width = Size.width - left - right;
            tittleBar.Size.height = 20;

            borderUpRight.Position.X = (int)Size.width - 13;
            borderUp.Size.width = Size.width - 26;
            borderLeft.Size.height = Size.height - 27;
            borderRight.Position.X = (int)Size.width - 13;
            borderRight.Size.height = Size.height - 27;

            borderBottomLeft.Position.Y = (int)Size.height - 15;

            borderBottom.Position.Y = (int)Size.height - 15;
            borderBottom.Size.width = Size.width - 26;

            borderBottomRight.Position.X = (int)Size.width - 13;
            borderBottomRight.Position.Y = (int)Size.height - 15;

            contentPosition = new Position((int)left, (int)(top + tittleBar.Size.height + 2));
            contentSize = new Size(Size.width - left - right, Size.height - top - bottom - 2 - tittleBar.Size.height);

            if (Layout_ != null)
            {
                Layout_.UpdateLayout(childList, contentPosition, contentSize);
            }
        }
        public override void PaintChild()
        {
            foreach (Component ele in childList)
            {
                UI.Instance.CurrentTheme.ScissorBegin(contentPosition, contentSize);
                ele.Paint();
                UI.Instance.CurrentTheme.ScissorEnd();
            }
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintDialog(this);
            UI.Instance.PushPosition(new Position(Position));
            tittleBar.Paint();
            PaintChild();
            UI.Instance.PopPosition();
        }
        public virtual void Close()
        {

            if (showType == EShowType.Modal)
            {
                DialogManager.Instance.DropModalDialog();
            }
            else if (showType == EShowType.Modeless)
            {
                DialogManager.Instance.DropModelessDialog(this);
            }
            Cursor.Current = Cursors.Default;
        }
        private void MouseMove(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

        }
        public override void OnMouseMove(MouseEvent me)
        {
            int mx = 0;
            int my = 0;

            mx = me.X - Position.X;
            my = me.Y - Position.Y;

            if (Resizable)
            {
                /*
                UI.Instance.CurrentTheme.testX = (uint)(Position.X + borderLeft.Position.X);
                UI.Instance.CurrentTheme.testY = (uint)(Position.Y + borderLeft.Position.Y);
                UI.Instance.CurrentTheme.testW = (uint)(Position.X + borderLeft.Size.width);
                UI.Instance.CurrentTheme.testH = (uint)(Position.Y + borderLeft.Size.height);
                */
                bool exit = false;

                foreach (DialogBorder db in dialogBorders)
                {
                    if (db.IsIn(mx, my))
                    {
                        if (db.isHover == false)
                        {
                            MouseEvent ev = new MouseEvent(db, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                           mx, my, me.MouseButton);
                       		db.ProcessMouseEntered(ev);

                            exit = true;
                        }
                    }
                    else
                    {
                        if (db.isHover == true)
                        {
                            MouseEvent ev = new MouseEvent(db, (int)EMouseEventTypes.MOUSE_EXITED,
                                                           mx, my, me.MouseButton);
                            db.ProcessMouseExited(ev);
                            exit = true;
                        }
                    }
                    if (exit)
                        break;
                }
            }

            foreach (Component ele in childList)
            {
                if (ele.IsIn(mx, my))
                {
                    if (ele.isHover)
                    {
                        MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_MOTION,
                                                       mx, my, me.MouseButton);
                        ele.ProcessMouseMoved(ev);
                        break;
                    }
                    else
                    {
                        MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                       mx, my, me.MouseButton);
                        ele.ProcessMouseEntered(ev);
                        break;
                    }
                }
                else
                {
                    if (ele.isHover)
                    {
                        MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_EXITED,
                                                       mx, my, OpenTK.Input.MouseButton.Left);
                        ele.ProcessMouseExited(ev);
                        break;
                    }
                }
            }
        }
        public override void OnMousePress(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (Dragable)
            {
                if (tittleBar.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(tittleBar, (int)EMouseEventTypes.MOUSE_PRESSED,
                                                   mx, my, me.MouseButton);
                    tittleBar.ProcessMousePressed(ev);
                    return;
                }
            }
            if (Resizable)
            {
                if (borderUpLeft.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderUpLeft, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderUpLeft.ProcessMousePressed(ev);
                    return;
                }
                else if (borderUpRight.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderUpRight, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderUpRight.ProcessMousePressed(ev);
                    return;
                }
                else if (borderUp.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderUp, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderUp.ProcessMousePressed(ev);
                    return;
                }
                else if (borderLeft.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderLeft, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderLeft.ProcessMousePressed(ev);
                    return;
                }
                else if (borderRight.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderRight, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderRight.ProcessMousePressed(ev);
                    return;
                }
                else if (borderBottomLeft.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderBottomLeft, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderBottomLeft.ProcessMousePressed(ev);
                    return;
                }
                else if (borderBottom.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderBottom, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderBottom.ProcessMousePressed(ev);
                    return;
                }
                else if (borderBottomRight.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(borderBottomRight, (int)EMouseEventTypes.MOUSE_PRESSED,
                               mx, my, me.MouseButton);
                    borderBottomRight.ProcessMousePressed(ev);
                    return;
                }
            }
            foreach (Component ele in childList)
            {
                if (ele.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_PRESSED,
                                                   mx, my, me.MouseButton);
                    ele.ProcessMousePressed(ev);
                    break;
                }
            }
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            foreach (Component ele in childList)
            {
                if (ele.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_RELEASED,
                                                   mx, my, me.MouseButton);
                    ele.ProcessMouseReleased(ev);
                    break;
                }
            }
        }
    }
    #endregion class Dialog

    #region class DialogTittleBar

    public class DialogTittleBar : DragAble
    {
        //Cursor moveAll;

        public DialogTittleBar(String text)
        {
            this.textFont = new Text("DialogTittleBar", UI.Instance.CurrentTheme.defaultTextFont, text);
            Text = text;
            Left = 10;
            Right = 10;
            Bottom = 1;
            Top = 1;
            /*Stream file;
            Assembly a = Assembly.GetExecutingAssembly();
            file = a.GetManifestResourceStream("AssortedWidgets.Resources.arrow-transfer.png");
            Bitmap bmp = new Bitmap(file);
            moveAll = new Cursor(bmp.GetHicon());*/
        }
        public String Text
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
        public uint Left
        {
            get;
            private set;
        }
        public uint Right
        {
            get;
            private set;
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetDialogTittleBarPreferedSize(this);
        }
        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintDialogTittleBar(this);
        }
        public override void OnMousePress(MouseEvent me)
        {
            base.OnMousePress(me);

            Cursor.Current = Cursors.SizeAll;
        }
        public override void OnDragMoved(int offsetX, int offsetY)
        {
            Cursor.Current = Cursors.SizeAll;
        }
        public override void OnDragReleased(int offsetX, int offsetY)
        {
            Cursor.Current = Cursors.Default;
        }
    }
    #endregion class DialogTittleBar

    #region class DialogBorder

    public class DialogBorder : DragAble
    {
        public DialogBorder(int x, int y, uint width, uint height)
        {
            Position.X = x;
            Position.Y = y;
            Size.width = width;
            Size.height = height;
        }
        public override Size GetPreferedSize()
        {
            return Size;
        }
    }
    #endregion class DialogBorder

    public enum EShowType
    {
        None,
        Modal,
        Modeless
    }
}
