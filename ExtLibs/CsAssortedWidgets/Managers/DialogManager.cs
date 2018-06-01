
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
using AssortedWidgets.Widgets;

namespace AssortedWidgets.Managers
{
    public class DialogManager
    {
        private static object syncRoot = new Object();

        Dialog modalDialog;
        List<Dialog> modelessDialog = new List<Dialog>();

        #region Singleton

        private static DialogManager instance = new DialogManager();
        public static DialogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DialogManager();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton

        #region Constructor privado

        private DialogManager()
        {
        }
        #endregion Constructor privado

        public void SetModelessDialog(Dialog modelessDialog)
        {
            foreach (Dialog d in this.modelessDialog)
            {
                d.SetActive(false);
            }
            this.modelessDialog.Add(modelessDialog);
            if (modalDialog != null)
            {
                modelessDialog.SetActive(false);
            }
            else
            {
                modelessDialog.SetActive(true);
            }
            modelessDialog.SetShowType(EShowType.Modeless);
        }
        public void SetModalDialog(Dialog modalDialog)
        {
            this.modalDialog = modalDialog;
            this.modalDialog.SetActive(true);
            this.modalDialog.SetShowType(EShowType.Modal);

            foreach (Dialog d in this.modelessDialog)
            {
                d.SetActive(false);
            }
        }
        public void DropModalDialog()
        {
            modalDialog.SetActive(false);
            modalDialog.SetShowType(EShowType.None);
            modalDialog = null;
            if (modelessDialog.Count != 0)
            {
                modelessDialog[modelessDialog.Count - 1].SetActive(true);
            }
        }
        public void DropModelessDialog(Dialog toBeDropped)
        {
            for (int i = 0; i < modelessDialog.Count; ++i)
            {
                if (modelessDialog[i] == toBeDropped)
                {
                    toBeDropped.SetActive(false);
                    toBeDropped.SetShowType(EShowType.None);
                    modelessDialog[i] = modelessDialog[modelessDialog.Count - 1];
                    modelessDialog.RemoveAt(modelessDialog.Count - 1);
                }
            }
        }
        public void Paint()
        {
            foreach (Dialog d in this.modelessDialog)
            {
                d.Paint();
            }
            if (modalDialog != null)
            {
                modalDialog.Paint();
            }
        }

        public bool ImportMousePressed(int mx, int my)
        {
            bool captured = false;

            if (modalDialog != null)
            {
                if (modalDialog.IsIn(mx, my))
                {
                    captured = true;
                    MouseEvent ev = new MouseEvent(modalDialog, (int)EMouseEventTypes.MOUSE_PRESSED,
                                                  mx, my, OpenTK.Input.MouseButton.Left);
                    modalDialog.ProcessMousePressed(ev);
                }
            }
            else
            {
                if (modelessDialog.Count > 0)
                {
                    Dialog currentActive = modelessDialog[modelessDialog.Count - 1];
                    if (currentActive.IsActive())
                    {
                        if (currentActive.IsIn(mx, my))
                        {
                            captured = true;
                            MouseEvent ev = new MouseEvent(currentActive, (int)EMouseEventTypes.MOUSE_PRESSED,
                                                           mx, my, OpenTK.Input.MouseButton.Left);
                            currentActive.ProcessMousePressed(ev);
                        }
                        else
                        {
                            for (int i = modelessDialog.Count - 1; i >= 0; --i)
                            {
                                if (modelessDialog[i].IsIn(mx, my))
                                {
                                    captured = true;
                                    modelessDialog[modelessDialog.Count - 1].SetActive(false);
                                    modelessDialog[i].SetActive(true);

                                    Dialog temp = modelessDialog[i];
                                    modelessDialog[i] = modelessDialog[modelessDialog.Count - 1];
                                    modelessDialog[modelessDialog.Count - 1] = temp;

                                    MouseEvent ev = new MouseEvent(temp, (int)EMouseEventTypes.MOUSE_PRESSED,
                                                                   mx, my, OpenTK.Input.MouseButton.Left);
                                    temp.ProcessMousePressed(ev);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = modelessDialog.Count - 1; i >= 0; --i)
                        {
                            if (modelessDialog[i].IsIn(mx, my))
                            {
                                captured = true;
                                modelessDialog[modelessDialog.Count - 1].SetActive(false);
                                modelessDialog[i].SetActive(true);

                                Dialog temp = modelessDialog[i];
                                modelessDialog[i] = modelessDialog[modelessDialog.Count - 1];
                                modelessDialog[modelessDialog.Count - 1] = temp;
                            }
                        }
                    }
                }
            }
            return captured;
        }

        public void ImportMouseMotion(int mx, int my)
        {
            if (modalDialog != null)
            {
                if (modalDialog.IsIn(mx, my))
                {
                    if (modalDialog.isHover)
                    {
                        MouseEvent ev = new MouseEvent(modalDialog, (int)EMouseEventTypes.MOUSE_MOTION,
                                                       mx, my, OpenTK.Input.MouseButton.Left);
                        modalDialog.ProcessMouseMoved(ev);
                    }
                    else
                    {
                        MouseEvent ev = new MouseEvent(modalDialog, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                       mx, my, OpenTK.Input.MouseButton.Left);
                        modalDialog.ProcessMouseEntered(ev);
                    }

                }
                else
                {
                    if (modalDialog.isHover)
                    {
                        MouseEvent ev = new MouseEvent(modalDialog, (int)EMouseEventTypes.MOUSE_EXITED,
                                                       mx, my, OpenTK.Input.MouseButton.Left);
                        modalDialog.ProcessMouseExited(ev);
                    }
                }
            }
            else
            {
                if (modelessDialog.Count != 0)
                {
                    Dialog currentActive = modelessDialog[modelessDialog.Count - 1];
                    if (currentActive.IsActive())
                    {
                        if (currentActive.IsIn(mx, my))
                        {
                            if (currentActive.isHover)
                            {
                                MouseEvent ev = new MouseEvent(currentActive, (int)EMouseEventTypes.MOUSE_MOTION,
                                                               mx, my, OpenTK.Input.MouseButton.Left);
                                currentActive.ProcessMouseMoved(ev);
                            }
                            else
                            {
                                MouseEvent ev = new MouseEvent(currentActive, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                               mx, my, OpenTK.Input.MouseButton.Left);
                                currentActive.ProcessMouseEntered(ev);
                            }
                        }
                        else
                        {
                            if (currentActive.isHover)
                            {
                                MouseEvent ev = new MouseEvent(currentActive, (int)EMouseEventTypes.MOUSE_EXITED,
                                                               mx, my, OpenTK.Input.MouseButton.Left);
                                currentActive.ProcessMouseExited(ev);
                            }
                        }
                    }
                }
            }
        }
        public void ImportMouseReleased(int mx, int my)
        {
            if (modalDialog != null)
            {
                if (modalDialog.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(modalDialog, (int)EMouseEventTypes.MOUSE_RELEASED,
                                                   mx, my, OpenTK.Input.MouseButton.Left);
                    modalDialog.ProcessMouseReleased(ev);
                }
            }
            else
            {
                if (modelessDialog.Count > 0)
                {
                    Dialog currentActive = modelessDialog[modelessDialog.Count - 1];

                    if (currentActive.IsActive())
                    {
                        if (currentActive.IsIn(mx, my))
                        {
                            MouseEvent ev = new MouseEvent(currentActive, (int)EMouseEventTypes.MOUSE_RELEASED,
                                                           mx, my, OpenTK.Input.MouseButton.Left);
                            currentActive.ProcessMouseReleased(ev);
                        }
                    }
                }
            }
        }


    }
}
