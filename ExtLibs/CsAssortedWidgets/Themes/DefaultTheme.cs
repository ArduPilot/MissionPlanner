
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

using AssortedWidgets.Graphics;
using AssortedWidgets.Util;
using AssortedWidgets.Widgets;

using OpenTK.Graphics.OpenGL;

namespace AssortedWidgets.Themes
{
    public class DefaultTheme : Theme
    {
        Texture2D tex2d;

        SubImage menuLeft;
        SubImage menuRight;
        SubImage menuListUpLeft;
        SubImage menuListUp;
        SubImage menuListUpRight;
        SubImage menuListLeft;
        SubImage menuListRight;
        SubImage menuListBottomLeft;
        SubImage menuListBottom;
        SubImage menuListBottomRight;
        SubImage menuItemSubMenuArrow;

        SubImage dialogUpLeftActive;
        SubImage dialogUpActive;
        SubImage dialogUpRightActive;
        SubImage dialogLeft;
        SubImage dialogRight;
        SubImage dialogBottom;
        SubImage dialogBottomLeft;
        SubImage dialogBottomRight;

        SubImage dialogUpLeftDeactive;
        SubImage dialogUpDeactive;
        SubImage dialogUpRightDeactive;

        SubImage buttonNormalLeft;
        SubImage buttonNormalRight;
        SubImage buttonHoverLeft;
        SubImage buttonHoverRight;

        SubImage checkButtonOn;
        SubImage checkButtonOff;

        SubImage radioButtonOn;
        SubImage radioButtonOff;

        SubImage progressBarLeft;
        SubImage progressBarRight;
        SubImage progressBarTop;
        SubImage progressBarBottom;

        SubImage scrollBarVerticalBottomNormal;
        SubImage scrollBarVerticalBottomHover;

        float w, h;

        public DefaultTheme(uint width, uint height)
            : base(width, height)
        {
        }

        public override void Setup(String textureThemeFileName, String textureThemePath)
        {
            String path = Theme.AdaptRelativePathToPlatform(textureThemePath);
            tex2d = TextureManager.Singleton.CreateTexture2D(textureThemeFileName, path);
            uint dtt = (uint)tex2d.SizeTexture;

            menuLeft = new SubImage(1.0f / dtt, 1.0f / dtt, 7.0f / dtt, 21.0f / dtt, (uint)tex2d.TextureId);
            menuRight = new SubImage(53.0f / dtt, 1.0f / dtt, 59.0f / dtt, 21.0f / dtt, (uint)tex2d.TextureId);

            menuListUpLeft = new SubImage(3.0f / dtt, 34.0f / dtt, 27.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            menuListUp = new SubImage(22.0f / dtt, 34.0f / dtt, 31.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            menuListUpRight = new SubImage(33.0f / dtt, 34.0f / dtt, 57.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            menuListLeft = new SubImage(3.0f / dtt, 38.0f / dtt, 27.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            menuListRight = new SubImage(33.0f / dtt, 38.0f / dtt, 57.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            menuListBottomLeft = new SubImage(3.0f / dtt, 44.0f / dtt, 27.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);
            menuListBottom = new SubImage(22.0f / dtt, 44.0f / dtt, 31.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);
            menuListBottomRight = new SubImage(33.0f / dtt, 44.0f / dtt, 57.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);
            menuItemSubMenuArrow = new SubImage(62.0f / dtt, 1.0f / dtt, 67.0f / dtt, 10.0f / dtt, (uint)tex2d.TextureId);

            dialogUpLeftActive = new SubImage(3.0f / dtt, 27.0f / dtt, 27.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            dialogUpActive = new SubImage(22.0f / dtt, 27.0f / dtt, 31.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            dialogUpRightActive = new SubImage(33.0f / dtt, 27.0f / dtt, 57.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            dialogLeft = new SubImage(3.0f / dtt, 38.0f / dtt, 27.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            dialogRight = new SubImage(33.0f / dtt, 38.0f / dtt, 57.0f / dtt, 43.0f / dtt, (uint)tex2d.TextureId);
            dialogBottomLeft = new SubImage(3.0f / dtt, 44.0f / dtt, 27.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);
            dialogBottom = new SubImage(22.0f / dtt, 44.0f / dtt, 31.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);
            dialogBottomRight = new SubImage(33.0f / dtt, 44.0f / dtt, 57.0f / dtt, 60.0f / dtt, (uint)tex2d.TextureId);

            dialogUpLeftDeactive = new SubImage(89.0f / dtt, 61.0f / dtt, 113.0f / dtt, 77.0f / dtt, (uint)tex2d.TextureId);
            dialogUpDeactive = new SubImage(111.0f / dtt, 61.0f / dtt, 116.0f / dtt, 77.0f / dtt, (uint)tex2d.TextureId);
            dialogUpRightDeactive = new SubImage(119.0f / dtt, 61.0f / dtt, 143.0f / dtt, 77.0f / dtt, (uint)tex2d.TextureId);

            buttonNormalLeft = new SubImage(1.0f / dtt, 61.0f / dtt, 5.0f / dtt, 80.0f / dtt, (uint)tex2d.TextureId);
            buttonNormalRight = new SubImage(83.0f / dtt, 61.0f / dtt, 87.0f / dtt, 80.0f / dtt, (uint)tex2d.TextureId);
            buttonHoverLeft = new SubImage(1.0f / dtt, 81.0f / dtt, 5.0f / dtt, 100.0f / dtt, (uint)tex2d.TextureId);
            buttonHoverRight = new SubImage(83.0f / dtt, 81.0f / dtt, 87.0f / dtt, 100.0f / dtt, (uint)tex2d.TextureId);

            checkButtonOn = new SubImage(81.0f / dtt, 129.0f / dtt, 92.0f / dtt, 140.0f / dtt, (uint)tex2d.TextureId);
            checkButtonOff = new SubImage(94.0f / dtt, 129.0f / dtt, 105.0f / dtt, 140.0f / dtt, (uint)tex2d.TextureId);
            radioButtonOn = new SubImage(81.0f / dtt, 117.0f / dtt, 92.0f / dtt, 128.0f / dtt, (uint)tex2d.TextureId);
            radioButtonOff = new SubImage(94.0f / dtt, 117.0f / dtt, 105.0f / dtt, 128.0f / dtt, (uint)tex2d.TextureId);

            progressBarLeft = new SubImage(1.0f / dtt, 101.0f / dtt, 5.0f / dtt, 121.0f / dtt, (uint)tex2d.TextureId);
            progressBarRight = new SubImage(47.0f / dtt, 101.0f / dtt, 51.0f / dtt, 121.0f / dtt, (uint)tex2d.TextureId);
            progressBarTop = new SubImage(106.0f / dtt, 117.0f / dtt, 126.0f / dtt, 121.0f / dtt, (uint)tex2d.TextureId);
            progressBarBottom = new SubImage(106.0f / dtt, 145.0f / dtt, 126.0f / dtt, 149.0f / dtt, (uint)tex2d.TextureId);

            scrollBarVerticalBottomHover = new SubImage(113.0f / dtt, 101.0f / dtt, 128.0f / dtt, 116.0f / dtt, (uint)tex2d.TextureId);
            scrollBarVerticalBottomNormal = new SubImage(33.0f / dtt, 122.0f / dtt, 48.0f / dtt, 137.0f / dtt, (uint)tex2d.TextureId);
        }
        public override void PaintMenuBar(MenuBar pMenuBar)
        {
            float x1 = 0.0f;
            float y1 = 0.0f;
            float x2 = pMenuBar.Size.width;
            float y2 = 30.0f;

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(BeginMode.Quads);
            GL.Color4((byte)42, (byte)55, (byte)55, (byte)125);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x1 + 40.0f, y1);
            GL.Vertex2(x1 + 40.0f, y2);
            GL.Vertex2(x1, y2);
            GL.Color3(55 / 255f, 65 / 255f, 67 / 255f);
            GL.Vertex2(x1 + 40.0f, y1);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x2, y2);
            GL.Vertex2(x1 + 40.0f, y2);
            GL.End();
        }

        public override Size GetMenuItemButtonPreferedSize(MenuItemButton component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(24 + (uint)w, 20);
        }
        public override Size GetMenuItemSeparatorPreferedSize(MenuItemSeparator component)
        {
            return new Size(component.Size);
        }
        public override Size GetMenuPreferedSize(Menu component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(12 + (uint)w, 19);
        }
        public override Size GetMenuItemSubMenuPreferedSize(MenuItemSubMenu component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(24 + 10 + (uint)w, 20);
        }
        public override Size GetDialogTittleBarPreferedSize(DialogTittleBar component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(20 + (uint)w, 20);
        }
        public override Size GetButtonPreferedSize(Button component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(component.Right + component.Left + (uint)w, 19);
        }
        public override Size GetLabelPreferedSize(Label component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(component.Right + component.Left + (uint)w, 20);
        }
        public override Size GetCheckButtonPreferedSize(CheckButton component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(component.Right + component.Left + (uint)w + 15, 19);
        }
        public override Size GetRadioButtonPreferedSize(RadioButton component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(component.Right + component.Left + (uint)w + 15, 19);
        }
        public override Size GetDropListItemPreferedSize(DropListItem component)
        {
            UI.Instance.CurrentTheme.defaultTextFont.MeasureString(component.Text, out w, out h);
            return new Size(component.Right + component.Left + (uint)w, 20);
        }
        public override void PaintSlideBarSlider(SlideBarSlider component)
        {
            Position origin = UI.Instance.GetOrigin();

            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)46, (byte)55, (byte)53);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y + component.Size.height);
            GL.End();
        }

        public override void PaintRadioButton(RadioButton component)
        {
            Position origin = UI.Instance.GetOrigin();

            SubImage checkStatus = null;
            if (component.Check == true)
            {
                checkStatus = radioButtonOn;
            }
            else
            {
                checkStatus = radioButtonOff;
            }
            switch (component.GetStatus())
            {
                case EButtonStatus.Normal:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3((byte)137, (byte)155, (byte)145);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }



                case EButtonStatus.Hover:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonHoverLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonHoverRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)175, (byte)200, (byte)28);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }



                case EButtonStatus.Pressed:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }


            }
        }
        public override void PaintMenuItemSeparator(MenuItemSeparator component)
        {
            Position origin = UI.Instance.GetOrigin();
            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)79, (byte)91, (byte)84);
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(10 + origin.X + component.Position.X, origin.Y + component.Position.Y + 1);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width - 10, origin.Y + component.Position.Y + 1);
            GL.End();
        }
        public override void PaintMenu(Menu component)
        {
            float x1 = 0;
            float y1 = 0;
            float x2 = 0;
            float y2 = 0;

            if (component.IsExpand)
            {
                x1 = component.Position.X;
                y1 = component.Position.Y;
                x2 = component.Position.X + component.Size.width;
                y2 = component.Position.Y + component.Size.height;

                GL.Disable(EnableCap.Texture2D);
                GL.Color3((byte)44, (byte)55, (byte)55);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x1 + 6, y1);
                GL.Vertex2(x2 - 6, y1);
                GL.Vertex2(x2 - 6, y2);
                GL.Vertex2(x1 + 6, y2);
                GL.End();
                GL.Enable(EnableCap.Texture2D);
                GL.Color3((byte)255, (byte)255, (byte)255);
                menuLeft.Paint(x1, y1, x1 + 6, y2);
                menuRight.Paint(x2 - 6, y1, x2, y2);
                GL.Color3((byte)150, (byte)155, (byte)161);
            }
            else
            {
                switch (component.GetStatus)
                {
                    case EStatus.Hover:
                        // flotar
                        {
                            x1 = component.Position.X;
                            y1 = component.Position.Y;
                            x2 = component.Position.X + component.Size.width;
                            y2 = component.Position.Y + component.Size.height;

                            GL.Disable(EnableCap.Texture2D);
                            GL.Color3((byte)44, (byte)55, (byte)55);
                            GL.Begin(BeginMode.Quads);
                            GL.Vertex2(x1 + 6, y1);
                            GL.Vertex2(x2 - 6, y1);
                            GL.Vertex2(x2 - 6, y2);
                            GL.Vertex2(x1 + 6, y2);
                            GL.End();
                            GL.Enable(EnableCap.Texture2D);
                            GL.Color3((byte)255, (byte)255, (byte)255);
                            menuLeft.Paint(x1, y1, x1 + 6, y2);
                            menuRight.Paint(x2 - 6, y1, x2, y2);
                            GL.Color3((byte)150, (byte)155, (byte)161);
                            break;
                        }


                    case EStatus.Normal:
                        {
                            x1 = component.Position.X;
                            y1 = component.Position.Y;
                            GL.Color3((byte)150, (byte)155, (byte)161);
                            break;
                        }


                    case EStatus.Pressed:
                        {
                            x1 = component.Position.X;
                            y1 = component.Position.Y;
                            GL.Color3((byte)250, (byte)250, (byte)250);
                            break;
                        }


                }
            }
            component.textFont.PosX = (int)(x1 + 3);
            component.textFont.PosY = (int)(y1 + 0);
            component.textFont.Render(true);
        }
        public override void PaintMenuList(MenuList component)
        {
            Position origin = UI.Instance.GetOrigin();
            float x1 = origin.X + component.Position.X;
            float y1 = origin.Y + component.Position.Y;
            float x2 = x1 + component.Size.width;
            float y2 = y1 + component.Size.height;

            GL.Enable(EnableCap.Texture2D);
            GL.Color3(1f, 1f, 1f);
            menuListUpLeft.Paint(x1, y1, x1 + 24.0f, y1 + 9.0f);
            menuListUpRight.Paint(x2 - 24.0f, y1, x2, y1 + 9.0f);
            menuListUp.Paint(x1 + 24.0f, y1, x2 - 24.0f, y1 + 9.0f);
            menuListLeft.Paint(x1, y1 + 9.0f, x1 + 24.0f, y2 - 16.0f);
            menuListRight.Paint(x2 - 24.0f, y1 + 9.0f, x2, y2 - 16.0f);
            menuListBottomLeft.Paint(x1, y2 - 16.0f, x1 + 24.0f, y2);
            menuListBottomRight.Paint(x2 - 24.0f, y2 - 16.0f, x2, y2);
            menuListBottom.Paint(x1 + 24.0f, y2 - 16.0f, x2 - 24.0f, y2);
            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)46, (byte)55, (byte)53);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(x1 + 24.0f, y1 + 9.0f);
            GL.Vertex2(x2 - 24.0f, y1 + 9.0f);
            GL.Vertex2(x2 - 24.0f, y2 - 16.0f);
            GL.Vertex2(x1 + 24.0f, y2 - 16.0f);
            GL.End();
        }

        public override void PaintMenuItemButton(MenuItemButton component)
        {
            Position origin = UI.Instance.GetOrigin();

            switch (component.GetStatus())
            {
                case EStatus.Normal:
                    {
                        GL.Color3((byte)255, (byte)255, (byte)255);
                        break;
                    }


                    ;
                case EStatus.Pressed:
                    {
                        GL.Color3((byte)180, (byte)180, (byte)180);
                        break;
                    }


                    ;
                case EStatus.Hover:
                    {
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)176, (byte)200, (byte)28);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(component.Position.X + origin.X, origin.Y + component.Position.Y);
                        GL.Vertex2(component.Position.X + origin.X + component.Size.width, origin.Y + component.Position.Y);
                        GL.Vertex2(component.Position.X + origin.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
                        GL.Vertex2(component.Position.X + origin.X, origin.Y + component.Position.Y + component.Size.height);
                        GL.End();
                        GL.Color3((byte)88, (byte)101, (byte)9);
                        break;
                    }


                    ;
            }
            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
            component.textFont.Render(true);
        }
        public override void PaintMenuItemSubMenu(MenuItemSubMenu component)
        {
            Position origin = UI.Instance.GetOrigin();

            switch (component.GetStatus)
            {
                case EStatus.Normal:
                    {
                        GL.Color3((byte)255, (byte)255, (byte)255);
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3((byte)255, (byte)255, (byte)255);
                        if (component.IsExpand)
                        {
                            menuItemSubMenuArrow.Paint(origin.X + component.Position.X + component.Size.width - 17, component.Position.Y + origin.Y + 5, origin.X + component.Position.X + component.Size.width - 12, component.Position.Y + origin.Y + 14);
                        }
                        else
                        {
                            menuItemSubMenuArrow.Paint(origin.X + component.Position.X + component.Size.width - 22, component.Position.Y + origin.Y + 5, origin.X + component.Position.X + component.Size.width - 17, component.Position.Y + origin.Y + 14);
                        }
                        break;
                    }


                case EStatus.Pressed:
                    {
                        GL.Color3((byte)200, (byte)200, (byte)200);
                        break;
                    }


                case EStatus.Hover:
                    {
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)176, (byte)200, (byte)28);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(component.Position.X + origin.X, origin.Y + component.Position.Y);
                        GL.Vertex2(component.Position.X + origin.X + component.Size.width, origin.Y + component.Position.Y);
                        GL.Vertex2(component.Position.X + origin.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
                        GL.Vertex2(component.Position.X + origin.X, origin.Y + component.Position.Y + component.Size.height);
                        GL.End();
                        GL.Color3((byte)88, (byte)101, (byte)9);
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3((byte)255, (byte)255, (byte)255);
                        if (component.IsExpand)
                        {
                            menuItemSubMenuArrow.Paint(origin.X + component.Position.X + component.Size.width - 17, component.Position.Y + origin.Y + 5, origin.X + component.Position.X + component.Size.width - 12, component.Position.Y + origin.Y + 14);
                        }
                        else
                        {
                            menuItemSubMenuArrow.Paint(origin.X + component.Position.X + component.Size.width - 22, component.Position.Y + origin.Y + 5, origin.X + component.Position.X + component.Size.width - 17, component.Position.Y + origin.Y + 14);
                        }
                        break;
                    }


            }
            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
            component.textFont.Render(true);
        }
        public override void PaintDialogTittleBar(DialogTittleBar component)
        {
            Position origin = UI.Instance.GetOrigin();

            GL.Color3((byte)31, (byte)31, (byte)31);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y + component.Size.height);
            GL.End();
            GL.Color3(1f, 1f, 1f);
            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y);
            component.textFont.Render(true);
        }
        public override void PaintDialog(Dialog component)
        {
            float x1 = component.Position.X + 24;
            float x2 = component.Position.X + component.Size.width - 24;
            float y1 = component.Position.Y + component.Size.height - 16;
            float y2 = component.Position.Y + component.Size.height;

            GL.Enable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);

            if (component.IsActive())
            {
                dialogUpLeftActive.Paint(component.Position.X, component.Position.Y, x1, component.Position.Y + 16);
                dialogUpActive.Paint(x1, component.Position.Y, x2, component.Position.Y + 16);
                dialogUpRightActive.Paint(x2, component.Position.Y, component.Position.X + component.Size.width, component.Position.Y + 16);
            }
            else
            {
                dialogUpLeftDeactive.Paint(component.Position.X, component.Position.Y, x1, component.Position.Y + 16);
                dialogUpDeactive.Paint(x1, component.Position.Y, x2, component.Position.Y + 16);
                dialogUpRightDeactive.Paint(x2, component.Position.Y, component.Position.X + component.Size.width, component.Position.Y + 16);
            }

            dialogLeft.Paint(component.Position.X, component.Position.Y + 16, x1, y1);
            dialogRight.Paint(x2, component.Position.Y + 16, component.Position.X + component.Size.width, y1);
            dialogBottomLeft.Paint(component.Position.X, y1, x1, y2);
            dialogBottom.Paint(x1, y1, x2, y2);
            dialogBottomRight.Paint(x2, y1, component.Position.X + component.Size.width, y2);

            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)46, (byte)55, (byte)53);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(x1, component.Position.Y + 16);
            GL.Vertex2(x2, component.Position.Y + 16);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x1, y1);
            GL.End();
        }
        /*
            //-------------------- Test
            GL.Color3((byte)255, (byte)0, (byte)0);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(testX, testY);
            GL.Vertex2(testX, testH);
            GL.Vertex2(testW, testH);
            GL.Vertex2(testW, testY);
            GL.End();
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);*/

        public override void PaintButton(Button component)
        {
            Position origin = UI.Instance.GetOrigin();

            switch (component.GetStatus())
            {
                case EButtonStatus.Normal:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3((byte)137, (byte)155, (byte)145);
                        break;
                    }


                    ;

                case EButtonStatus.Hover:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonHoverLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonHoverRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)175, (byte)200, (byte)28);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);
                        break;
                    }


                    ;

                case EButtonStatus.Pressed:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);
                        break;
                    }


            }
            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 2);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
            component.textFont.Render(true);
        }
        public override void PaintLabel(Label component)
        {
            Position origin = UI.Instance.GetOrigin();

            if (component.IsDrawBackground())
            {
                GL.Color3(0, 0, 0);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y);
                GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y);
                GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
                GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y + component.Size.height);
                GL.End();
            }
            GL.Color3(1f, 1f, 1f);

            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
            component.textFont.Render(true);
        }
        public override void PaintCheckButton(CheckButton component)
        {
            Position origin = UI.Instance.GetOrigin();

            SubImage checkStatus = null;

            if (component.Check == true)
            {
                checkStatus = checkButtonOn;
            }
            else
            {
                checkStatus = checkButtonOff;
            }
            switch (component.GetStatus())
            {
                case EButtonStatus.Normal:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3((byte)137, (byte)155, (byte)145);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }



                case EButtonStatus.Hover:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonHoverLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonHoverRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)175, (byte)200, (byte)28);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }



                case EButtonStatus.Pressed:
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        buttonNormalLeft.Paint(origin.X + component.Position.X, origin.Y + component.Position.Y, origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        buttonNormalRight.Paint(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y, origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + 19);
                        GL.Disable(EnableCap.Texture2D);
                        GL.Color3((byte)55, (byte)67, (byte)65);
                        GL.Begin(BeginMode.Quads);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y);
                        GL.Vertex2(origin.X + component.Position.X + component.Size.width - 4, origin.Y + component.Position.Y + 19);
                        GL.Vertex2(origin.X + component.Position.X + 4, origin.Y + component.Position.Y + 19);
                        GL.End();
                        GL.Color3(0, 0, 0);

                        component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X + 15);
                        component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 4);
                        component.textFont.Render(true);

                        GL.Enable(EnableCap.Texture2D);
                        GL.Color3(1f, 1f, 1f);
                        checkStatus.Paint(origin.X + component.Position.X + component.Left, origin.Y + component.Position.Y + component.Top, origin.X + component.Position.X + component.Left + 11, origin.Y + component.Position.Y + component.Top + 11);
                        break;
                    }


            }
        }

        public override void PaintProgressBar(ProgressBar component)
        {
            SubImage subI1, subI2;
            float x1, x2, x3, x4, y1, y2, y3, y4;

            Position origin = UI.Instance.GetOrigin();

            if (component.Type == ETypeOrientation.Horizontal)
            {
                origin = UI.Instance.GetOrigin();

                subI1 = progressBarLeft;
                subI2 = progressBarRight;

                x1 = origin.X + component.Position.X;
                x2 = origin.X + component.Position.X + 4;
                x3 = origin.X + component.Position.X + component.Size.width - 4;
                x4 = origin.X + component.Position.X + component.Size.width;
                y1 = origin.Y + component.Position.Y;
                y2 = origin.Y + component.Position.Y + component.Size.height;

                GL.Enable(EnableCap.Texture2D);
                GL.Color3(1f, 1f, 1f);
                progressBarLeft.Paint(x1, y1, x2, y2);
                progressBarRight.Paint(x3, y1, x4, y2);
                GL.Disable(EnableCap.Texture2D);
                GL.Color3((byte)79, (byte)91, (byte)84);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x2, y1);
                GL.Vertex2(x3, y1);
                GL.Vertex2(x3, y2);
                GL.Vertex2(x2, y2);
                GL.End();
                GL.Color3((byte)46, (byte)55, (byte)53);

                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x1 + 2, y1 + 2);
                GL.Vertex2(x1 + 2 + component.POfSlider, y1 + 2);
                GL.Vertex2(x1 + 2 + component.POfSlider, y2 - 2);
                GL.Vertex2(x1 + 2, y2 - 2);
                GL.End();
            }
            else
            {
                subI1 = progressBarTop;
                subI2 = progressBarBottom;

                x1 = origin.X + component.Position.X;
                x2 = origin.X + component.Position.X + component.Size.width;
                y1 = origin.Y + component.Position.Y;
                y2 = origin.Y + component.Position.Y + 4;
                y3 = origin.Y + component.Position.Y + component.Size.height - 4;
                y4 = origin.Y + component.Position.Y + component.Size.height;

                GL.Enable(EnableCap.Texture2D);
                GL.Color3(1f, 1f, 1f);
                progressBarTop.Paint(x1, y1, x2, y2);
                progressBarBottom.Paint(x1, y3, x2, y4);
                GL.Disable(EnableCap.Texture2D);
                GL.Color3((byte)79, (byte)91, (byte)84);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x1, y2);
                GL.Vertex2(x2, y2);
                GL.Vertex2(x2, y3);
                GL.Vertex2(x1, y3);
                GL.End();
                GL.Color3((byte)46, (byte)55, (byte)53);

                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x1 + 2, y4 - 2 - component.POfSlider);
                GL.Vertex2(x2 - 2, y4 - 2 - component.POfSlider);
                GL.Vertex2(x2 - 2, y4 - 2);
                GL.Vertex2(x1 + 2, y4 - 2);
                GL.End();
            }
        }
        public override void PaintSlideBar(SlideBar component)
        {
            Position origin = UI.Instance.GetOrigin();

            if (component.Type == ETypeOrientation.Horizontal)
            {
                origin = UI.Instance.GetOrigin();

                float x1 = origin.X + component.Position.X;
                float x2 = origin.X + component.Position.X + 4;
                float x3 = origin.X + component.Position.X + component.Size.width - 4;
                float x4 = origin.X + component.Position.X + component.Size.width;
                float y1 = origin.Y + component.Position.Y;
                float y2 = origin.Y + component.Position.Y + component.Size.height;

                GL.Enable(EnableCap.Texture2D);
                GL.Color3(1f, 1f, 1f);
                progressBarLeft.Paint(x1, y1, x2, y2);
                progressBarRight.Paint(x3, y1, x4, y2);
                GL.Disable(EnableCap.Texture2D);
                GL.Color3((byte)79, (byte)91, (byte)84);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x2, y1);
                GL.Vertex2(x3, y1);
                GL.Vertex2(x3, y2);
                GL.Vertex2(x2, y2);
                GL.End();
            }
            else
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Color3(1f, 1f, 1f);
                float x1 = origin.X + component.Position.X;
                float x2 = origin.X + component.Position.X + component.Size.width;
                float y1 = origin.Y + component.Position.Y;
                float y2 = origin.Y + component.Position.Y + 4;
                float y3 = origin.Y + component.Position.Y + component.Size.height - 4;
                float y4 = origin.Y + component.Position.Y + component.Size.height;

                progressBarTop.Paint(x1, y1, x2, y2);
                progressBarBottom.Paint(x1, y3, x2, y4);
                GL.Disable(EnableCap.Texture2D);
                GL.Color3((byte)79, (byte)91, (byte)84);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(x1, y2);
                GL.Vertex2(x2, y2);
                GL.Vertex2(x2, y3);
                GL.Vertex2(x1, y3);
                GL.End();
            }
        }
        public override void PaintScrollPanel(ScrollPanel component)
        {
            Position origin = UI.Instance.GetOrigin();

            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)79, (byte)91, (byte)84);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y);
            GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y + component.Size.height);
            GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y + component.Size.height);
            GL.End();
        }
        public override void PaintDropList(DropList component)
        {
            Position origin = UI.Instance.GetOrigin();

            float x1=origin.X+component.Position.X;
			float x2=origin.X+component.Position.X+4;
			float x3=origin.X+component.Position.X+component.Size.width-4;
			float x4=origin.X+component.Position.X+component.Size.width;
			float y1=origin.Y+component.Position.Y;
			float y2=origin.Y+component.Position.Y+component.Size.height;

            GL.Enable(EnableCap.Texture2D);
            GL.Color3(1f, 1f, 1f);
            progressBarLeft.Paint(x1, y1, x2, y2);
			progressBarRight.Paint(x3,y1,x4,y2);
            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)79, (byte)91, (byte)84);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(x2, y1);
            GL.Vertex2(x3, y1);
            GL.Vertex2(x3, y2);
            GL.Vertex2(x2, y2);
            GL.End();

			DropListItem selected = component.SelectedItem;
			if(selected != null)
			{
                GL.Color3(0, 0, 0);
                selected.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
                selected.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
                selected.textFont.Render(true);
			}
        }
		public override void PaintDropDown(Position position, Size area)
		{
            GL.Disable(EnableCap.Texture2D);
            GL.Color3((byte)79, (byte)91, (byte)84);
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(position.X, position.Y);
            GL.Vertex2(position.X + area.width, position.Y);
            GL.Vertex2(position.X + area.width, position.Y + area.height);
            GL.Vertex2(position.X, position.Y + area.height);
            GL.End();

            GL.Color3((byte)46, (byte)55, (byte)53);
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex2(position.X, position.Y);
            GL.Vertex2(position.X + area.width, position.Y);
            GL.Vertex2(position.X + area.width, position.Y + area.height);
            GL.Vertex2(position.X, position.Y + area.height);
            GL.Vertex2(position.X, position.Y);
            GL.End();
        }
		public override void PaintDropListButton(DropListButton component)
		{
			SubImage button = null;

			switch(component.GetStatus())
			{
			case EButtonStatus.Normal:
				{
					button=scrollBarVerticalBottomNormal;
					break;
				}
			case EButtonStatus.Hover:
				{
					button=scrollBarVerticalBottomHover;
					break;
				}
			case EButtonStatus.Pressed:
				{
					button=scrollBarVerticalBottomNormal;
					break;
				}
			}
			Position origin= UI.Instance.GetOrigin();
            GL.Enable(EnableCap.Texture2D);
            GL.Color3(1f, 1f, 1f);
            button.Paint(origin.X + component.Position.X, 
                         origin.Y + component.Position.Y, 
                         origin.X + component.Position.X + component.Size.width,
                         origin.Y + component.Position.Y + component.Size.height);
		}
		public override void PaintDropListItem(DropListItem component)
		{
			Position origin= UI.Instance.GetOrigin();

            if(component.GetStatus()== EButtonStatus.Hover)
			{
                GL.Color3((byte)175, (byte)200, (byte)28);
                GL.Begin(BeginMode.Quads);
                GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y);
                GL.Vertex2(origin.X + component.Position.X + component.Size.width, origin.Y + component.Position.Y);
                GL.Vertex2(origin.X + component.Position.X + component.Size.width,
                           origin.Y+component.Position.Y+component.Size.height);
                GL.Vertex2(origin.X + component.Position.X, origin.Y + component.Position.Y + component.Size.height);
                GL.End();
            }
            GL.Color3(0, 0, 0);
            component.textFont.PosX = (int)(component.Position.X + component.Left + origin.X);
            component.textFont.PosY = (int)(component.Top + origin.Y + component.Position.Y - 2);
            component.textFont.Render(true);
		}

        void Prueba(int texid)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Color3(1f, 1f, 1f);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, texid);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1);
            GL.Vertex2(40, 40);
            GL.TexCoord2(0, 0);
            GL.Vertex2(40, 140);
            GL.TexCoord2(1, 0);
            GL.Vertex2(140, 140);
            GL.TexCoord2(1, 1);
            GL.Vertex2(140, 40);
            GL.End();
        }
        public override void ScissorBegin(Position Position, Size area)
        {
            Position origin = UI.Instance.GetOrigin();

            GL.Enable(EnableCap.ScissorTest);
            GL.Scissor((int)(origin.X + Position.X), (int)(UI.Instance.CurrentTheme.ScreenHeight - origin.Y - area.height - Position.Y), (int)area.width, (int)area.height);
        }

        public override void ScissorEnd()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
