
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
using AssortedWidgets.Util;

using OpenTK.Input;

using Size = AssortedWidgets.Util.Size;
using Text = AssortedWidgets.GLFont.Text;

namespace AssortedWidgets.Widgets
{
    public class Menu : Component
    {
        bool expand;
        EStatus status;
        MenuList menuList = new MenuList();
        MenuBar menuBar;

        #region Constructor

        public Menu(String text)
        {
            Text = text;
            this.textFont = new Text("Menu", UI.Instance.CurrentTheme.defaultTextFont, text);
            float w, h;
            Size = new Size(23, 10);
            //Size=Font::FontEngine::getSingleton().getFont().getStringBoundingBox(textFont);
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(text, out w, out h);
            Size.width = (uint)w;
            Size.height = (uint)h;

            Size.width += 10;
            Size.height = 20;
            Position.X = 100;
            Position.Y = 100;

            status = EStatus.Normal;
            expand = false;

            menuList.Position.X = -9;
            menuList.Position.Y = 25;
        }
        #endregion Constructor

        #region Propiedades

        public String Text
        {
            get;
            private set;
        }
        public bool IsExpand
        {
            get { return expand; }
        }
        public EStatus GetStatus
        {
            get { return status; }
        }
        #endregion Propiedades

        #region Métodos públicos

        public void AddItem(MenuItem item)
        {
            menuList.AddItem(item);
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetMenuPreferedSize(this);
        }
        /// <summary>
        /// Contraer el menú
        /// </summary>
        public void Shrink()
        {
            expand = false;
            status = EStatus.Normal;
        }
        public void SetMenuBar(MenuBar mb)
        {
            this.menuBar = mb;
        }
        #endregion Métodos públicos

        #region Paint()

        public override void Paint()
        {
            //base.BeginPaint();

            UI.Instance.CurrentTheme.PaintMenu(this);
            if (expand && menuList.ItemListIsEmpty == false)
            {
                UI.Instance.PushPosition(new Position(Position.X, Position.Y));
                menuList.Paint();
                UI.Instance.PopPosition();
            }
        }
        #endregion Paint()

        #region ListMouseXXXX()

        internal void ListMousePressed(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_PRESSED, mx, my,
                                              me.MouseButton);
                menuList.ProcessMousePressed(e);
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_PRESSED,
                                              mx - menuList.Position.X, my - menuList.Position.Y,
                                              me.MouseButton);
                menuList.ExpandMenu.ListMousePressed(e);
            }
        }
        internal void ListMouseReleased(MouseEvent e)
        {
            int mx = e.X - Position.X;
            int my = e.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_RELEASED, mx, my,
                                               e.MouseButton);
                menuList.ProcessMouseReleased(ev);
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_RELEASED,
                                               mx - menuList.Position.X, my - menuList.Position.Y,
                                               e.MouseButton);
                menuList.ExpandMenu.ListMouseReleased(ev);
            }
        }
        /// <summary>
        /// Movimiento del ratón por la lista de menús
        /// </summary>
        /// <param name="me"></param>
        internal void ListMouseMotion(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                if (menuList.isHover)
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseMoved(e);
                }
                else
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseEntered(e);
                }
            }
            else
            {
                if (menuList.isHover)
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_EXITED, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseExited(e);
                }
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_MOTION,
                                              mx - menuList.Position.X, my - menuList.Position.Y,
                                              MouseButton.Left);
                menuList.ExpandMenu.ListMouseMotion(e);
            }
        }
        #endregion ListMouseXXXX()

        #region OnXXXX()

        public override void OnMousePress(MouseEvent me)
        {
            //base.OnMousePress(me);

            status = EStatus.Pressed;
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            //base.OnMouseRelease(me);

            status = EStatus.Hover;
            if (expand)
            {
                menuBar.SetShrink();
                expand = false;
            }
            else
            {
                menuBar.SetExpand(this);
                expand = true;
            }
        }
        public override void OnMouseEnter(MouseEvent me)
        {
            //base.OnMouseEnter(me);

            isHover = true;
            if (menuBar.IsExpand)
            {
                menuBar.SetExpand(this);
                expand = true;
            }
            else
            {
                if (!expand)
                {
                    status = EStatus.Hover;
                }
            }
        }
        public override void OnMouseExit(MouseEvent me)
        {
            //base.OnMouseExit(me);
            isHover = false;
            if (expand == false)
                status = EStatus.Normal;
        }
        #endregion OnXXXX()
   }
    /// <summary>
    /// Representa la barra de menús, de esta cuelgan todos los menús superiores.
    /// </summary>
    public class MenuBar : Component
    {
        private static object syncRoot = new Object();

        #region Miembros de clase

        List<Menu> menuList = new List<Menu>();
        List<Component> inList = new List<Component>();

        int spacer;
        //int leftSpacer;
        int topSpacer;
        int rightSpacer;
        //int bottomSpacer;
        /// <summary>
        /// Indica el menú que está espandido. Solo puede haber un menú expandido a la vez.
        /// </summary>
        Menu expandMenu;
        /// <summary>
        /// Indica que <see cref="expandMenu"/> está expandido.
        /// </summary>
        bool expand;
        
        #endregion Miembros de clase

        #region Singleton

        private static MenuBar instance = new MenuBar();
        public static MenuBar Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new MenuBar();
                    }
                }

                return instance;
            }
        }
        #endregion Singleton

        #region Constructor privado

        private MenuBar()
        {
            //leftSpacer = 45;
            rightSpacer = 45;
            topSpacer = 5;
            //bottomSpacer = 5;
            expand = false;
            expandMenu = null;
        }
        #endregion Constructor privado

        #region Init()

        public void Init(uint width)
        {
            Init(width, 30, 5);
        }
        public void Init(uint width, uint height, int spacer)
        {
            this.spacer = spacer;
            Position.X = 0;
            Position.Y = 0;
            Size.width = width;
            Size.height = height;
        }
        #endregion Init()

        #region Propiedades

        public bool IsExpand
        {
            get { return expand; }
        }
        #endregion Propiedades

        #region AddMenu()

        public void AddMenu(Menu menu)
        {
            menuList.Add(menu);
            menu.SetMenuBar(this);
            UpdateLayout();
        }
        #endregion AddMenu()

        #region UpdateLayout()

        void UpdateLayout()
        {
            int tempBegin = rightSpacer;

            for (int cont = 0; cont < menuList.Count; cont++)
            {
                menuList[cont].Position.X = tempBegin;
                menuList[cont].Position.Y = topSpacer;
                tempBegin += spacer + (int)menuList[cont].GetPreferedSize().width;
            }
        }
        #endregion UpdateLayout()

        #region Set Expand-Shrink-Width

        /// <summary>
        /// Contrae el menú expandido actual Y después establece el menú expandido al pasado como parámetro.
        /// </summary>
        /// <param name="expandMenu"></param>
        public void SetExpand(Menu expandMenu)
        {
            if (this.expandMenu != null)
                this.expandMenu.Shrink();

            this.expandMenu = expandMenu;
            expand = true;
        }
        /// <summary>
        /// Encoge-contrae el menú expandido. Shrink = encoger
        /// </summary>
        public void SetShrink()
        {
            if (expandMenu != null)
            {
                expandMenu.Shrink();
            }
            expandMenu = null;
            expand = false;
        }
        public void SetWidth(uint width)
        {
            Size.width = width;
        }
        #endregion Set Expand-Shrink-Width

        #region Override OnXXXXX()

        public override void OnMouseEnter(MouseEvent me)
        {
            //base.OnMouseEnter(me);

            isHover = true;
            OnMouseMove(me);
        }
        public override void OnMouseExit(MouseEvent me)
        {
            //base.OnMouseExit(me);

            isHover = false;
            OnMouseMove(me);
        }
        public override void OnMousePress(MouseEvent me)
        {
            //base.OnMousePress(me);

            foreach (Menu m in menuList)
            {
                if (m.IsIn(me.X, me.Y))
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_PRESSED, me.X, me.Y, me.MouseButton);
                    m.ProcessMousePressed(e);
                }
            }
            if (IsExpand && expandMenu != null)
            {
                expandMenu.ListMousePressed(me);
            }
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            //base.OnMouseRelease(me);

            foreach (Menu m in menuList)
            {
                if (m.IsIn(me.X, me.Y))
                {
                    MouseEvent e = new MouseEvent(null, (int)EMouseEventTypes.MOUSE_RELEASED, me.X, me.Y,
                                                  me.MouseButton);
                    m.ProcessMouseReleased(e);
                }
            }
            if (IsExpand && expandMenu != null)
            {
                expandMenu.ListMouseReleased(me);
            }
        }
        public override void OnMouseMove(MouseEvent me)
        {
            //base.OnMouseMove(me);

            foreach (Menu m in menuList)
            {
                if (m.IsIn(me.X, me.Y))
                {
                    if (m.isHover == false)
                    {
                        MouseEvent e = new MouseEvent(m, (int)EMouseEventTypes.MOUSE_ENTERED, me.X, me.Y,
                                                      MouseButton.Left);
                        m.ProcessMouseEntered(e);
                    }
                }
                else
                {
                    if (m.isHover == true)
                    {
                        MouseEvent e = new MouseEvent(m, (int)EMouseEventTypes.MOUSE_EXITED, me.X, me.Y,
                                                      MouseButton.Left);
                        m.ProcessMouseExited(e);
                    }
                }
            }
            if (expand && expandMenu != null)
            {
                expandMenu.ListMouseMotion(me);
            }
        }
        #endregion Override OnXXXXX()

        #region Paint()

        public override void Paint()
        {
            UI.Instance.CurrentTheme.PaintMenuBar(this);

            foreach (Menu m in menuList)
                m.Paint();
        }
        #endregion Paint()
    }
    
    public abstract class MenuItem : Component
    {
 		protected MenuList parentMenuList;

        public void SetMenuList(MenuList menuList)
        {
            parentMenuList = menuList;
        }
    }
    
    public class MenuItemButton : MenuItem
    {
        //String textFont;
        //EStyle style;
        EStatus status;

        public MenuItemButton(String text)
        {
            Text = text;
            this.textFont = new Text("MenuItemButton", UI.Instance.CurrentTheme.defaultTextFont, text);
            //this.textFont = textFont;
            //style = EStyle.Stretch;
            Left = 24;
            Top = 2;
            Bottom = 2;
            Right = 2;
            status = EStatus.Normal;

            Size = GetPreferedSize();
        }
        public String Text
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
        public EStatus GetStatus()
        {
            return status;
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetMenuItemButtonPreferedSize(this);
        }
        public override void Paint()
        {
            //base.BeginPaint();
            UI.Instance.CurrentTheme.PaintMenuItemButton(this);
        }

        public override void OnMousePress(AssortedWidgets.Events.MouseEvent me)
        {
            //base.OnMousePress(me);

            status = EStatus.Pressed;
        }
        public override void OnMouseEnter(AssortedWidgets.Events.MouseEvent me)
        {
            //base.OnMouseEnter(me);

            isHover = true;
            status = EStatus.Hover;
        }
        public override void OnMouseRelease(AssortedWidgets.Events.MouseEvent me)
        {
            //base.OnMouseRelease(me);

            status = EStatus.Normal;
            MenuBar.Instance.SetShrink();
        }
        public override void OnMouseExit(AssortedWidgets.Events.MouseEvent me)
        {
            //base.OnMouseExit(me);

            isHover = false;
            status = EStatus.Normal;
        }
    }
    
    public class MenuItemSeparator : MenuItem
    {
        public MenuItemSeparator()
        {
            Size.width = 10;
            Size.height = 3;
        }
        public Size getPreferedSize()
		{
			return UI.Instance.CurrentTheme.GetMenuItemSeparatorPreferedSize(this);
		}
        public override void Paint()
        {
            //base.BeginPaint();
			UI.Instance.CurrentTheme.PaintMenuItemSeparator(this);
        }
    }
    
    public class MenuItemSubMenu : MenuItem
    {
        bool expand;
        MenuList menuList;
        EStatus status;
        
        #region Constructor

        public MenuItemSubMenu(String text)
        {
            Text = text;
            this.textFont = new Text("MenuItemSubMenu", UI.Instance.CurrentTheme.defaultTextFont, text);
            //this.textFont = textFont;
            status = EStatus.Normal;
            expand = false;
            Left = 24;
            Top = 2;
            Bottom = 4;
            Right = 2;
            menuList = new MenuList();

            Size = GetPreferedSize();

            menuList.Position.X = 232 - 9;
            menuList.Position.Y = 0;
        }
        #endregion Constructor

        public String Text
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

        public bool IsExpand
        {
            get { return expand; }
        }
        public EStatus GetStatus
        {
            get { return status; }
        }
        public void AddItem(MenuItem item)
        {
            menuList.AddItem(item);
        }
        public override Size GetPreferedSize()
        {
            return UI.Instance.CurrentTheme.GetMenuItemSubMenuPreferedSize(this);
        }
        public void Shrink()
        {
            expand = false;
            status = EStatus.Normal;
        }
        public override void Paint()
        {
            //base.BeginPaint();

            UI.Instance.CurrentTheme.PaintMenuItemSubMenu(this);

            if (expand && menuList.ItemListIsEmpty == false)
            {
                UI.Instance.PushPosition(new Position(Position));
                menuList.Paint();
                UI.Instance.PopPosition();
            }
        }

        internal void ListMousePressed(MouseEvent e)
        {
            int mx = e.X - Position.X;
            int my = e.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_PRESSED,
                                               mx, my, e.MouseButton);
                menuList.ProcessMousePressed(ev);
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_PRESSED,
                                               mx - menuList.Position.X, my - menuList.Position.Y, e.MouseButton);
                menuList.ExpandMenu.ListMousePressed(ev);
            }
        }
        internal void ListMouseReleased(MouseEvent e)
        {
            int mx = e.X - Position.X;
            int my = e.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_RELEASED, mx, my,
                                               e.MouseButton);
                menuList.ProcessMouseReleased(ev);
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent ev = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_RELEASED,
                                               mx - menuList.Position.X, my - menuList.Position.Y,
                                               e.MouseButton);
                menuList.ExpandMenu.ListMouseReleased(ev);
            }
        }
        internal void ListMouseMotion(MouseEvent me)
        {
            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            if (expand && menuList.IsIn(mx, my))
            {
                if (menuList.isHover)
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseMoved(e);
                }
                else
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseEntered(e);
                }
            }
            else
            {
                if (menuList.isHover)
                {
                    MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_EXITED, mx, my,
                                                  MouseButton.Left);
                    menuList.ProcessMouseExited(e);
                }
            }

            if (menuList.IsExpand && menuList.ExpandMenu != null)
            {
                MouseEvent e = new MouseEvent(menuList, (int)EMouseEventTypes.MOUSE_MOTION,
                                              mx - menuList.Position.X, my - menuList.Position.Y,
                                              MouseButton.Left);
                menuList.ExpandMenu.ListMouseMotion(e);
            }
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            //base.OnMouseRelease(me);

            if (expand)
            {
                parentMenuList.SetShrink();
                expand = false;
            }
            else
            {
                parentMenuList.SetExpand(this);
                expand = true;
            }
            status = EStatus.Hover;
        }
        public override void OnMousePress(MouseEvent me)
        {
            //base.OnMousePress(me);

            status = EStatus.Pressed;
        }
        public override void OnMouseEnter(MouseEvent me)
        {
            //base.OnMouseEnter(me);
            isHover = true;
            status = EStatus.Hover;
        }
        public override void OnMouseExit(MouseEvent me)
        {
            //base.OnMouseExit(me);
            isHover = false;
            status = EStatus.Normal;
        }
    }

    public class MenuList : Component
    {
        MenuItemSubMenu expandSubMenu;
        List<MenuItem> itemList = new List<MenuItem>();

        uint minimizeSize;
        uint top;
        uint bottom;
        uint left;
        uint right;
        uint spacer;
        bool expand;

        #region Constructor

        public MenuList()
        {
            minimizeSize = 232;
            spacer = 2;
            top = 6;
            left = 9;
            right = 9;
            bottom = 16;
            expand = false;
        }
        #endregion Constructor

        #region Propiedades

        public bool ItemListIsEmpty
        {
            get { return itemList.Count == 0; }
        }
        public bool IsExpand
        {
            get { return expand; }
        }
        public MenuItemSubMenu ExpandMenu
        {
            get { return expandSubMenu; }
        }
        #endregion Propiedades

        #region AddItem()

        public void AddItem(MenuItem item)
        {
            itemList.Add(item);
            item.SetMenuList(this);
            UpdateLayout();
        }
        #endregion AddItem()

        #region UpdateLayout()

        void UpdateLayout()
        {
            uint tempX = left;
            uint tempY = top;
            Size.width = minimizeSize;
            Size.height = 0;

            foreach (MenuItem mi in itemList)
            {
                Size itemSize = mi.GetPreferedSize();
                Size.width = Math.Max(Size.width, itemSize.width);
                Size.height += itemSize.height + spacer;
                mi.Position.X = (int)tempX;
                mi.Position.Y = (int)tempY;
                tempY += spacer + itemSize.height;
            }

            for (int cont = 0; cont < itemList.Count; cont++)
                itemList[cont].Size.width = Size.width;

            Size.width += left + right;
            Size.height += top + bottom - spacer;
        }
        #endregion UpdateLayout()

        #region SetExpand() & SetShrink()

        public void SetExpand(MenuItemSubMenu expandSubMenu)
        {
            if (this.expandSubMenu != null)
            {
                this.expandSubMenu.Shrink();
            }
            this.expandSubMenu = expandSubMenu;
            expand = true;
        }

        public void SetShrink()
        {
            if (expandSubMenu != null)
            {
                expandSubMenu.Shrink();
            }
            expandSubMenu = null;
            expand = false;
        }
        #endregion Set Expand() & Shrink()

        #region Paint()

        public override void Paint()
        {
            //base.BeginPaint();
            UI.Instance.CurrentTheme.PaintMenuList(this);
            UI.Instance.PushPosition(new Position(Position));
            foreach (MenuItem mi in itemList)
                mi.Paint();
            UI.Instance.PopPosition();
        }
        #endregion Paint()

        #region OnMouseXXXX()

        public override void OnMousePress(MouseEvent me)
        {
            //base.OnMousePress(me);

            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            foreach (MenuItem mi in itemList)
            {
                if (mi.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(mi, (int)EMouseEventTypes.MOUSE_PRESSED, mx, my,
                                                   me.MouseButton);
                    mi.ProcessMousePressed(ev);
                }
            }
        }
        public override void OnMouseEnter(MouseEvent me)
        {
            //base.OnMouseEnter(me);

            isHover = true;
            OnMouseMove(me);
        }
        public override void OnMouseRelease(MouseEvent me)
        {
            //base.OnMouseRelease(me);

            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            foreach (MenuItem mi in itemList)
            {
                if (mi.IsIn(mx, my))
                {
                    MouseEvent ev = new MouseEvent(mi, (int)EMouseEventTypes.MOUSE_RELEASED, mx, my,
                                                   me.MouseButton);
                    mi.ProcessMouseReleased(ev);
                }
            }
        }
        public override void OnMouseMove(MouseEvent me)
        {
            //base.OnMouseMove(me);

            int mx = me.X - Position.X;
            int my = me.Y - Position.Y;

            foreach (MenuItem mi in itemList)
            {
                if (mi.IsIn(mx, my))
                {
                    if (mi.isHover)
                    {
                        MouseEvent ev = new MouseEvent(mi, (int)EMouseEventTypes.MOUSE_MOTION, mx, my,
                                                       me.MouseButton);
                        mi.ProcessMouseMoved(ev);
                    }
                    else
                    {
                        MouseEvent ev = new MouseEvent(mi, (int)EMouseEventTypes.MOUSE_ENTERED, mx, my,
                                                      me.MouseButton);
                        mi.ProcessMouseEntered(ev);
                    }
                }
                else
                {
                    if (mi.isHover)
                    {
                        MouseEvent ev = new MouseEvent(mi, (int)EMouseEventTypes.MOUSE_EXITED, mx, my,
                                                      me.MouseButton);
                        mi.ProcessMouseExited(ev);
                    }
                }
            }
        }
        public override void OnMouseExit(MouseEvent me)
        {
            //base.OnMouseExit(me);

            isHover = false;
            OnMouseMove(me);
        }
        #endregion OnMouseXXXX()
    }

    public enum EStatus
    {
        Normal,
        Hover,
        Pressed
    }
    public enum EStyle
    {
        Any,
        Shrink,
        Stretch
    }
}
