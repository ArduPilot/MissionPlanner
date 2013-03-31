
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

using AssortedWidgets.Events;
using AssortedWidgets.Layouts;

namespace AssortedWidgets.Widgets
{
    public class Container : Component
    {
        protected List<Component> childList = new List<Component>();
        protected Layout Layout_;

        public Layout Layout
        {
            set { Layout_ = value; }
        }
        public void Add(Component child)
        {
            childList.Add(child);
        }
        public void Remove(Component child)
        {
            childList.Remove(child);
        }
        public virtual void PaintChild() { }

        public override void OnMouseEnter(AssortedWidgets.Events.MouseEvent me)
        {
            isHover = true;
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            foreach (Component ele in childList)
            {
                if (ele.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(ele, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                   mx,my, me.MouseButton);
                    ele.ProcessMouseEntered(ev);

                    break;
                }
            }
        }
    }

    public enum EElementStyle
    {
        Any,
        Fit,        // Ajuste al texto
        Stretch
    }
}
