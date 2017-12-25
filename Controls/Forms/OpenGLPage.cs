using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using OpenTK.Graphics.ES30;

namespace MissionPlanner.Controls.Forms
{
    public class OpenGLPage : ContentPage
    {
        float red, green, blue;

        public OpenGLPage()
        {
            Title = "OpenGL";
            var view = new OpenGLView { HasRenderLoop = true };
            var toggle = new Switch { IsToggled = true };
            var button = new Button { Text = "Display" };

            view.HeightRequest = 300;
            view.WidthRequest = 300;

            view.OnDisplay = r => {

                GL.ClearColor(red, green, blue, 1.0f);
                GL.Clear((ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

                red += 0.01f;
                if (red >= 1.0f)
                    red -= 1.0f;
                green += 0.02f;
                if (green >= 1.0f)
                    green -= 1.0f;
                blue += 0.03f;
                if (blue >= 1.0f)
                    blue -= 1.0f;
            };

            toggle.Toggled += (s, a) => {
                view.HasRenderLoop = toggle.IsToggled;
            };
            button.Clicked += (s, a) => view.Display();

            var stack = new StackLayout
            {
                Padding = new Size(20, 20),
                Children = { view, toggle, button }
            };

            Content = stack;
        }
    }
}
