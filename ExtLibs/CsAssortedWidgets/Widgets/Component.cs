
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

using AssortedWidgets.Events;
using AssortedWidgets.GLFont;
using AssortedWidgets.Layouts;
using AssortedWidgets.Util;

namespace AssortedWidgets.Widgets
{
    public delegate void MouseMoveHandler(MouseEvent me);
    public delegate void MouseEnteredHandler(MouseEvent me);
    public delegate void MouseExitedHandler(MouseEvent me);
    public delegate void MousePressedHandler(MouseEvent me);
    public delegate void MouseReleasedHandler(MouseEvent me);

    public abstract class Component : BoundingBox
    {
        public event MouseMoveHandler MouseMoveEvent;
        public event MouseEnteredHandler MouseEnteredEvent;
        public event MouseExitedHandler MouseExitedEvent;
        public event MousePressedHandler MousePressedEvent;
        public event MouseReleasedHandler MouseReleasedEvent;

        public bool isHover;
        public bool isEnable;
        public bool isVisible;

        internal Text textFont;

        #region Constructor

        public Component()
        {
            Size = new Size();
            isHover = false;
            isEnable = true;
            isVisible = true;
            LayoutProperty = 0;
        }
        #endregion Constructor

        #region Propiedades

        public EArea LayoutProperty
        {
            get;
            set;
        }
        #endregion Propiedades

        public virtual Size GetPreferedSize() { return Size; }
        public virtual void Pack() { }
        /// <summary>
        /// No llamar
        /// </summary>
        public virtual void Paint() { }

        internal void ProcessMousePressed(MouseEvent me)
        {
            OnMousePress(me);

            if (MousePressedEvent != null)
                MousePressedEvent(me);
        }
        public virtual void OnMousePress(MouseEvent me) { }

        internal void ProcessMouseReleased(MouseEvent me)
        {
            OnMouseRelease(me);

            if (MouseReleasedEvent != null)
                MouseReleasedEvent(me);
        }
        public virtual void OnMouseRelease(MouseEvent me) { }

        internal void ProcessMouseEntered(MouseEvent me)
        {
            OnMouseEnter(me);

            if (MouseEnteredEvent != null)
                MouseEnteredEvent(me);
        }
        public virtual void OnMouseEnter(MouseEvent me) { }

        internal void ProcessMouseMoved(MouseEvent me)
        {
            OnMouseMove(me);

            if (MouseMoveEvent != null)
                MouseMoveEvent(me);
        }
        public virtual void OnMouseMove(MouseEvent me) { }

        internal void ProcessMouseExited(MouseEvent me)
        {
            OnMouseExit(me);

            if (MouseExitedEvent != null)
                MouseExitedEvent(me);
        }
        public virtual void OnMouseExit(MouseEvent me) { }
    }
}
