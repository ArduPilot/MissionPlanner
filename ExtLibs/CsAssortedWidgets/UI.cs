
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
using AssortedWidgets.Managers;
using AssortedWidgets.Test;
using AssortedWidgets.Themes;
using AssortedWidgets.Util;
using AssortedWidgets.Widgets;

using OpenTK.Graphics.OpenGL;

using OpenTK.Input;

namespace AssortedWidgets
{
    public class UI
    {
        private static object syncRoot = new Object();

        Stack<Position> PositionStack = new Stack<Position>();

        uint width;
        uint height;
        bool pressed;
        /*
        Menu menuFile;
        MenuItemButton menuItemFileOpen;
        MenuItemButton menuItemFileSave;
        MenuItemButton menuItemFileSaveAs;
        MenuItemSeparator menuItemFileSeparator;
        public MenuItemButton menuItemFileExit;
        MenuItemSubMenu menuItemFileExport;
        MenuItemSubMenu menuItemFilePNG;
        MenuItemButton menuItemFilePNGNone;
        MenuItemButton menuItemFilePNGInterlaced;
        MenuItemButton menuItemFileJPEG;
        MenuItemSubMenu menuItemFileImport;
        MenuItemButton menuItemFile3DS;
        MenuItemButton menuItemFileOBJ;
        MenuItemButton menuItemFileSIA;

        Menu menuHelp;
        MenuItemButton menuItemHelpAbout;
        MenuItemButton menuItemHelpHelp;

        Menu menuAssortedWidgetsTest;
        MenuItemButton menuItemDialogTest;
        MenuItemButton menuItemCheckNRadioTest;

        DialogTestDialog dialogTestDialog;
        CheckNRadioTestDialog checkNRadioTestDialog;

        MenuItemButton menuItemProgressNSliderTest;
        ProgressNSliderTestDialog progressNSliderTestDialog;

        MenuItemButton menuItemMultipleTest;
        MultipleLayoutTestDialog multipleLayoutTestDialog;

        MenuItemButton menuItemScrollPanelTest;
        ScrollPanelTestDialog scrollPanelTestDialog;

        MenuItemButton menuItemTextNDropTest;
        TextNDropTestDialog textNDropTestDialog;

        */
        
        List<Component> componentList = new List<Component>();

        #region Singleton

        private static UI instance = new UI();
        public static UI Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UI();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton

        #region Constructor Privado

        private UI()
        {
        }
        #endregion Constructor Privado

        #region Init

        public void Init(uint width, uint height)
        {
            ReSize(width, height);

            DefaultTheme theme = new DefaultTheme(width, height);
            theme.Setup("aw.png", "./Resources/");
            //selectionManager.setup(width,height);
            UI.Instance.CurrentTheme = theme;
            MenuBar.Instance.Init(width);

            /*

            menuFile = new Menu("FlightData");
            menuItemFileOpen = new MenuItemButton("Open");
            menuItemFileSave = new MenuItemButton("Save");
            menuItemFileSaveAs = new MenuItemButton("Save As");
            menuItemFileSeparator = new MenuItemSeparator();
            menuItemFileExit = new MenuItemButton("Exit");
            menuItemFileExport = new MenuItemSubMenu("Export");
            menuItemFilePNG = new MenuItemSubMenu("PNG Image");
            menuItemFilePNGNone = new MenuItemButton("None");
            menuItemFilePNGInterlaced = new MenuItemButton("Interlaced");
            menuItemFilePNG.AddItem(menuItemFilePNGNone);
            menuItemFilePNG.AddItem(menuItemFilePNGInterlaced);
            menuItemFileJPEG = new MenuItemButton("JPEG Image");
            menuItemFileExport.AddItem(menuItemFilePNG);
            menuItemFileExport.AddItem(menuItemFileJPEG);
            menuItemFileImport = new MenuItemSubMenu("Import");
            menuItemFile3DS = new MenuItemButton("3DS Model");
            menuItemFileOBJ = new MenuItemButton("OBJ Model");
            menuItemFileSIA = new MenuItemButton("SIA Model");
            menuItemFileImport.AddItem(menuItemFile3DS);
            menuItemFileImport.AddItem(menuItemFileOBJ);
            menuItemFileImport.AddItem(menuItemFileSIA);
            menuFile.AddItem(menuItemFileOpen);
            menuFile.AddItem(menuItemFileSave);
            menuFile.AddItem(menuItemFileSaveAs);
            menuFile.AddItem(menuItemFileExport);
            menuFile.AddItem(menuItemFileImport);
            menuFile.AddItem(menuItemFileSeparator);
            menuFile.AddItem(menuItemFileExit);

            menuHelp = new Menu("Help");
            menuItemHelpAbout = new MenuItemButton("About");
            menuItemHelpHelp = new MenuItemButton("Help");
            menuHelp.AddItem(menuItemHelpAbout);
            menuHelp.AddItem(menuItemHelpHelp);

            menuAssortedWidgetsTest = new Menu("Assorted Widgets Test");

            dialogTestDialog = new DialogTestDialog();
            menuItemDialogTest = new MenuItemButton("Modal Dialog Test");
            menuAssortedWidgetsTest.AddItem(menuItemDialogTest);

            checkNRadioTestDialog = new CheckNRadioTestDialog();
            menuItemCheckNRadioTest = new MenuItemButton("Check & Radio Test");
            menuAssortedWidgetsTest.AddItem(menuItemCheckNRadioTest);

            progressNSliderTestDialog = new ProgressNSliderTestDialog();
            menuItemProgressNSliderTest = new MenuItemButton("Progress & Slider Test");
            menuAssortedWidgetsTest.AddItem(menuItemProgressNSliderTest);

            multipleLayoutTestDialog = new MultipleLayoutTestDialog();
            menuItemMultipleTest = new MenuItemButton("MultipleLayout Test");
            menuAssortedWidgetsTest.AddItem(menuItemMultipleTest);

            scrollPanelTestDialog = new ScrollPanelTestDialog();
            menuItemScrollPanelTest = new MenuItemButton("Scroll Panel Test");
            menuAssortedWidgetsTest.AddItem(menuItemScrollPanelTest);

            textNDropTestDialog = new TextNDropTestDialog();
            menuItemTextNDropTest = new MenuItemButton("DropList Test");
            menuAssortedWidgetsTest.AddItem(menuItemTextNDropTest);

            MenuBar.Instance.AddMenu(menuFile);
            MenuBar.Instance.AddMenu(menuHelp);
            MenuBar.Instance.AddMenu(menuAssortedWidgetsTest);

            menuItemDialogTest.MousePressedEvent += new MousePressedHandler(menuItemDialogTest_MousePressedEvent);
            menuItemCheckNRadioTest.MousePressedEvent += new MousePressedHandler(menuItemCheckNRadioTest_MousePressedEvent);
            menuItemProgressNSliderTest.MousePressedEvent += new MousePressedHandler(menuItemProgressNSliderTest_MousePressedEvent);
            menuItemMultipleTest.MousePressedEvent += new MousePressedHandler(menuItemMultipleTest_MousePressedEvent);
            menuItemScrollPanelTest.MousePressedEvent += new MousePressedHandler(menuItemScrollPanelTest_MousePressedEvent);
            menuItemTextNDropTest.MousePressedEvent += new MousePressedHandler(menuItemTextNDropTest_MousePressedEvent);

            */
        }


        #endregion Init

        public void SetBackgroundMenuList(MenuList mList)
        {
        }
        public Theme CurrentTheme
        {
            get;
            set;
        }

        #region Util PositionStack

        public Position GetOrigin()
        {
            if (PositionStack.Count == 0)
            {
                return new Position(0, 0);
            }
            else
            {
                return PositionStack.Peek();
            }
        }
        internal void PushPosition(Position newPosition)
        {
            if (PositionStack.Count == 0)
            {
                PositionStack.Push(newPosition);
            }
            else
            {
                newPosition.X += PositionStack.Peek().X;
                newPosition.Y += PositionStack.Peek().Y;
                PositionStack.Push(newPosition);
            }
        }
        internal Position PopPosition()
        {
            Position result = PositionStack.Peek();
            PositionStack.Pop();

            return result;
        }
        #endregion Util PositionStack

        void gluOrtho2D(double left, double right, double bottom, double top)
        {
            GL.Ortho(left, right, bottom, top, -1.0, 1.0);
        }

        void Begin2D()
        {
            GL.Viewport(0, 0, (int)width, (int)height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            gluOrtho2D((double)0, (double)width, (double)height, (double)0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
        }
        void End2D()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
        }

        public void BeginPaint()
        {
            Begin2D();

            foreach (Component c in componentList)
                c.Paint();

            DialogManager.Instance.Paint();

            if(DropListManager.Instance.Dropped != null)
		    {
			    DropListManager.Instance.Paint();
	        }

            MenuBar.Instance.Paint();
        }
        public void EndPaint()
        {
            End2D();
        }
        public void ReSize(uint width, uint height)
        {
            MenuBar.Instance.SetWidth(width);
            this.width = width;
            this.height = height;
            if (UI.Instance.CurrentTheme != null)
            {
                UI.Instance.CurrentTheme.ScreenHeight = height;
                UI.Instance.CurrentTheme.ScreenWidth = width;
            }
        }
        public int MouseX
        {
            get;
            private set;
        }
        public int MouseY
        {
            get;
            private set;
        }
        public int OldMouseX
        {
            get;
            private set;
        }
        public int OldMouseY
        {
            get;
            private set;
        }
        public void ImportMouseMotion(int mx, int my)
        {
            MouseX = mx;
            MouseY = my;

            if (mx == OldMouseX && my == OldMouseY)
                return;
            else
            {
                OldMouseX = mx;
                OldMouseY = my;
            }
            if (pressed && DragManager.Instance.IsOnDrag())
            {
                DragManager.Instance.ProcessDrag(mx, my);
                return;
            }
			if(DropListManager.Instance.IsDropped)
			{
				if(DropListManager.Instance.IsIn(mx,my))
				{
					if(DropListManager.Instance.isHover)
					{
                        MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_MOTION,
                                                      mx, my, MouseButton.Left);
                        DropListManager.Instance.ImportMouseMotion(e);
                    }
					else
					{
                        MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_ENTERED,
                                                      mx, my, MouseButton.Left);
                        DropListManager.Instance.ImportMouseEntered(e);
                    }
				
				}
				else
				{
                    if (DropListManager.Instance.isHover)
					{
                        MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_EXITED,
                                                      mx, my, MouseButton.Left);
                        DropListManager.Instance.ImportMouseExited(e);
                    }
				}
			}
            if (MenuBar.Instance.IsIn(mx, my))
            {
                if (MenuBar.Instance.isHover)
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                  MouseButton.Left);
                    MenuBar.Instance.ProcessMouseMoved(e);
                }
                else
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my,
                                                  MouseButton.Left);
                    MenuBar.Instance.ProcessMouseEntered(e);
                }
            }
            else
            {
                if (MenuBar.Instance.isHover)
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_EXITED, mx, my,
                                                  MouseButton.Left);
                    MenuBar.Instance.ProcessMouseExited(e);
                }
                if (MenuBar.Instance.IsExpand)
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                  MouseButton.Left);
                    MenuBar.Instance.ProcessMouseMoved(e);
                }
            }

            DialogManager.Instance.ImportMouseMotion(mx, my);

            if (componentList.Count > 0)
            {
                foreach (Component c in componentList)
                {
                    if (c.IsIn(mx, my))
                    {
                        if (c.isHover)
                        {
                            MouseEvent ev = new MouseEvent(c, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                           MouseButton.Left);
                            c.ProcessMouseMoved(ev);
                        }
                        else
                        {
                            MouseEvent ev = new MouseEvent(c, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my,
                                                           MouseButton.Left);
                            c.ProcessMouseEntered(ev);
                        }
                    }
                    else
                    {
                        if (c.isHover)
                        {
                            MouseEvent ev = new MouseEvent(c, (int)EMouseEventTypes.MOUSE_EXITED, mx, my,
                                                          MouseButton.Left);
                            c.ProcessMouseExited(ev);
                        }
                    }
                }
            }
        }
        public void ImportMousePress(MouseButton button, int x, int y)
        {
            pressed = true;

            DragManager.Instance.SetCurrent(x, y);

            if(DropListManager.Instance.IsDropped)
			{
				if(DropListManager.Instance.IsIn(x,y))
				{
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_PRESSED, x, y, button);
                    DropListManager.Instance.ImportMousePressed(e);
                }
				else
				{
					DropListManager.Instance.ShrinkBack();
				}
			}

            if (MenuBar.Instance.IsIn(x, y))
            {
                MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_PRESSED, x, y, button);
                MenuBar.Instance.ProcessMousePressed(e);
            }
            else
            {
                if (MenuBar.Instance.IsExpand)
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_PRESSED, x, y, button);
                    MenuBar.Instance.ProcessMousePressed(e);
                }
            }

            DialogManager.Instance.ImportMousePressed(x, y);

            if (componentList.Count > 0)
            {
                foreach (Component c in componentList)
                {
                    if (c.IsIn(x, y))
                    {
                        MouseEvent ev = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_PRESSED, x, y,
                                                       button);
                        c.ProcessMousePressed(ev);
                    }
                }
            }
        }
        public void ImportMouseRelease(MouseButton button, int x, int y)
        {
 			DropListManager.Instance.SetCurrent(x,y);

            if (pressed && DragManager.Instance.IsOnDrag())
            {
                DragManager.Instance.DragEnd();
            }

            pressed = false;

            if (MenuBar.Instance.IsIn(x, y))
            {
                MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_RELEASED, x, y, button);
                MenuBar.Instance.ProcessMouseReleased(e);
            }
            else
            {
                if (MenuBar.Instance.IsExpand)
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_RELEASED, x, y, button);
                    MenuBar.Instance.ProcessMouseReleased(e);
                }
            }

            DialogManager.Instance.ImportMouseReleased(x, y);

            if (componentList.Count > 0)
            {
                foreach (Component c in componentList)
                {
                    if (c.IsIn(x, y))
                    {
                        MouseEvent ev = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_RELEASED, x, y,
                                                       button);
                        c.ProcessMouseReleased(ev);
                    }
                }
            }
        }
    }
}
