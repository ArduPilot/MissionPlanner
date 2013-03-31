
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

using AssortedWidgets.Events;
using AssortedWidgets.Managers;

namespace AssortedWidgets.Widgets
{
    public delegate void DragMovedHandler(object sender, int offsetX, int offsetY);
    public delegate void DragReleasedHandler(object sender, int offsetX, int offsetY);

    public class DragAble : Component
    {
        public event DragMovedHandler DragMovedEvent;
        public event DragReleasedHandler DragReleasedEvent;

        public int OffsetX
        {
            get;
            private set;
        }
        public int OffsetY
        {
            get;
            private set;
        }

        #region DragMoved

        public override void OnMousePress(MouseEvent me)
        {
            DragManager.Instance.DragBegin(Position.X, Position.Y, this);
        }
        public virtual void DragMoved(int offsetX, int offsetY)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;

            OnDragMoved(offsetX, offsetY);
            if (DragMovedEvent != null)
                DragMovedEvent(this, offsetX, offsetY);
        }
        public virtual void OnDragMoved(int offsetX, int offsetY) { }

        #endregion DragMoved

        #region DragReleased

        public virtual void DragReleased(int offsetX, int offsetY)
        {
            if (DragReleasedEvent != null)
                DragReleasedEvent(this, offsetX, offsetY);
            OnDragReleased(offsetX, offsetY);
        }
        public virtual void OnDragReleased(int offsetX, int offsetY) { }

        #endregion DragReleased
    }
}
