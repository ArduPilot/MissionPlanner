
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

using AssortedWidgets.Widgets;

namespace AssortedWidgets.Events
{
    public class Event
    {
        public Event(Component source, int type)
        {
            this.Source = source;
            this.EventType = type;
        }
        public Component Source
        {
            get;
            private set;
        }
        public int EventType
        {
            get;
            private set;
        }
    }
    public enum EMouseEventTypes
    {
        MOUSE_PRESSED = 0,
        MOUSE_RELEASED = 1,
        MOUSE_CLICKED = 2,
        MOUSE_EXITED = 3,
        MOUSE_DRAGGED = 4,
        MOUSE_ENTERED = 5,
        MOUSE_MOTION = 6
    }
    public enum EKeyEventTypes
    {
        KEY_PRESSED = 7,
        KEY_RELEASED = 8,
        KEY_TYPED = 9
    }
}
