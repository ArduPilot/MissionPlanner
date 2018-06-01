
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

using AssortedWidgets.Widgets;

namespace AssortedWidgets.Managers
{
    public class DragManager
    {
        DragAble componentOnDrag;
        int preX;
        int preY;
        int currentX;
        int currentY;

        #region Singleton

        private static object syncRoot = new Object();

        private static DragManager instance = new DragManager();
        public static DragManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DragManager();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton

        #region Constructor privado

        private DragManager()
        {
        }
        #endregion Constructor privado

        public int OldX
        {
            get;
            private set;
        }
        public int OldY
        {
            get;
            private set;
        }
        public void SetCurrent(int currentX, int currentY)
        {
            this.currentX = currentX;
            this.currentY = currentY;
        }
        public void DragBegin(int oldX, int oldY, DragAble component)
        {
            OldX = oldX;
            OldY = oldY;
            preX = currentX;
            preY = currentY;
            componentOnDrag = component;
        }

        public void DragEnd()
        {
            componentOnDrag.DragReleased(preX, preY);

            OldX = 0;
            OldY = 0;
            preX = 0;
            preY = 0;

            componentOnDrag = null;
        }

        public bool IsOnDrag()
        {
            return componentOnDrag != null;
        }

        public void ProcessDrag(int x, int y)
        {
            if (IsOnDrag())
            {
                componentOnDrag.DragMoved(x - preX, y - preY);
                preX = x;
                preY = y;
            }
        }

        public DragAble GetOnDragComponent()
        {
            return componentOnDrag;
        }
    }
}
