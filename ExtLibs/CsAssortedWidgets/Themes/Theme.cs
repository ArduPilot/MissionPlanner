
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
using System.Drawing.Text;
using System.IO;

using AssortedWidgets.Graphics;
using AssortedWidgets.Util;
using AssortedWidgets.Widgets;

using Size = AssortedWidgets.Util.Size;

namespace AssortedWidgets.Themes
{
    public abstract class Theme
    {
        internal TextFontTexture defaultTextFont;

        public uint testX, testY, testW, testH;

        public Theme(uint width, uint height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            PrivateFontCollection pfc = new PrivateFontCollection();
            //String path = Theme.AdaptRelativePathToPlatform("../../../../media/"); 
            //pfc.AddFontFile(path + "arial.ttf");
            //Font f = new Font(pfc.Families[0], 12);
            //defaultTextFont = new TextFontTexture(f); //FontStyle.Bold | FontStyle.Italic));
            defaultTextFont = new TextFontTexture(new Font("Arial", 10, FontStyle.Regular)); //FontStyle.Bold | FontStyle.Italic));
        }
        public uint ScreenWidth
        {
        	get;
        	set;
        }
        public uint ScreenHeight
        {
        	get;
        	set;
        }
        public abstract void Setup(String textureThemeFileName, String textureThemePath);

        public abstract Size GetMenuItemButtonPreferedSize(MenuItemButton component);
        public abstract Size GetMenuItemSeparatorPreferedSize(MenuItemSeparator component);
        public abstract Size GetMenuPreferedSize(Menu component);
        public abstract Size GetMenuItemSubMenuPreferedSize(MenuItemSubMenu component);
        public abstract Size GetDialogTittleBarPreferedSize(DialogTittleBar component);
        public abstract Size GetButtonPreferedSize(Button component);
        public abstract Size GetLabelPreferedSize(Label component);
        public abstract Size GetCheckButtonPreferedSize(CheckButton component);
        public abstract Size GetRadioButtonPreferedSize(RadioButton component);
        public abstract Size GetDropListItemPreferedSize(DropListItem component);

        public abstract void PaintMenuBar(MenuBar pMenuBar);
        public abstract void PaintMenu(Menu component);
        public abstract void PaintMenuList(MenuList component);
        public abstract void PaintMenuItemSeparator(MenuItemSeparator component);
        public abstract void PaintMenuItemButton(MenuItemButton component);
        public abstract void PaintMenuItemSubMenu(MenuItemSubMenu component);
        public abstract void PaintDialogTittleBar(DialogTittleBar component);
        public abstract void PaintDialog(Dialog component);
        public abstract void PaintButton(Button component);
        public abstract void PaintLabel(Label component);
        public abstract void PaintCheckButton(CheckButton component);
        public abstract void PaintRadioButton(RadioButton component);
        public abstract void PaintProgressBar(ProgressBar component);
        public abstract void PaintSlideBarSlider(SlideBarSlider component);
        public abstract void PaintSlideBar(SlideBar component);
        public abstract void PaintScrollPanel(ScrollPanel component);
		public abstract void PaintDropList(DropList component);
        public abstract void PaintDropDown(Position position, Size area);
        public abstract void PaintDropListButton(DropListButton component);
        public abstract void PaintDropListItem(DropListItem component);

        public abstract void ScissorBegin(Position Position, Size area);
        public abstract void ScissorEnd();

        public static String AdaptRelativePathToPlatform(string relativePath)
        {
            if (!relativePath.StartsWith("."))
                throw new Exception("Error en Utility.AdaptRelativePathToPlatform().Una ruta relativa debe comenzar por '.'.");
            String result;
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            if (pid == PlatformID.Unix || pid == PlatformID.MacOSX)
                result = relativePath.Replace('\\', Path.DirectorySeparatorChar);
            else
                result = relativePath.Replace('/', Path.DirectorySeparatorChar);

            return result;
        }
    }
}
