using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.plugins
{
    public class example22_fontsize : Plugin.Plugin
    {
        public override string Name => "fontsize";

        public override string Version => "0";

        public override string Author => "me";

        public override bool Exit()
        {
            return true;
        }

        public override bool Init()
        {
            Debugger.Break();
            Console.WriteLine("Font name: " + SystemFonts.DefaultFont.Name);
            Console.WriteLine("Font size: " + SystemFonts.DefaultFont.Size);

            var s = TextRenderer.MeasureText("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", SystemFonts.DefaultFont);
            int width = (int)Math.Round((float)s.Width / 62f);

            Console.WriteLine(s + " " + width);

            Console.WriteLine("AutoScaleDimensions " + Host.MainForm.AutoScaleDimensions);
            Console.WriteLine("AutoScaleMode " + Host.MainForm.AutoScaleMode);
            return false;
        }

        public override bool Loaded()
        {
            return false;
        }
    }
}
