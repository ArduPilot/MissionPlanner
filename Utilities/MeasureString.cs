using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;
using log4net;

namespace System.Drawing
{
    public static class graphicsext
    {
        //SizeF MeasureString(string text, Font font, SizeF layoutArea);
        public static SizeF MeasureStringCache(this Graphics g, string text, Font font, SizeF layoutArea)
        {
            SizeF size = Measure.MeasureString(g, font, text);
            if (size.Width > layoutArea.Width)
            {
                size.Width = layoutArea.Width;
                size.Height += font.Height * (int)Math.Round(size.Width / layoutArea.Width,0,MidpointRounding.AwayFromZero);
            }

            return size;
        }

        public static SizeF MeasureStringCache(this Graphics g, string text, Font font)
        {
        //    Console.WriteLine("MeasureStringCache > " + text);
            if (text == null)
                return new SizeF();
            return Measure.MeasureString(g, font, text);
        }
    }
}

internal class FontData
{
    private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private const TextFormatFlags FormatFlags =
        TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;

    public FontData(IDeviceContext g, Font font)
    {
        log.Info("Build Font Table " + font.Name + " " + font.Size);
        BuildLookupList(g, font, ref NormalCharacter);
        BuildLookupList(g, new Font(font, FontStyle.Bold), ref BoldCharacter);
        BuildLookupList(g, new Font(font, FontStyle.Italic), ref ItalicCharacter);
        log.Info("Build Font Table... done");
    }

    public SizeF[] NormalCharacter;
    public SizeF[] BoldCharacter;
    public SizeF[] ItalicCharacter;

    private static void BuildLookupList(IDeviceContext g, Font font, ref SizeF[] size)
    {
        size = new SizeF[char.MaxValue];
        for (var i = (char) 0; i < (char) 256; i++)
        {
            size[i] = MeasureString(g, font, i.ToString());
        }
    }

    internal static SizeF MeasureString(IDeviceContext g, Font font, string text)
    {
        return string.IsNullOrEmpty(text) ? new SizeF(0, 0) : TextRenderer.MeasureText(text, font);
            //((Graphics)g).MeasureString(text, font);
    }

}

public sealed class Measure
{
    private static readonly Dictionary<Font, FontData> Fonts = new Dictionary<Font, FontData>();

    public static SizeF MeasureString(IDeviceContext g, Font font, string text)
    {
        FontData data;
        if (!Fonts.ContainsKey(font))
        {
            data = new FontData(g, font);
            Fonts.Add(font, data);
        }
        else
        {
            data = Fonts[font];
        }

        return MeasureString(g, text.ToCharArray(), font, data);
    }

    private static SizeF MeasureString(IDeviceContext g, IList text, Font font, FontData data)
    {
        SizeF ans = new SizeF();
        foreach (char chr in text)
        {
            SizeF temp = MeasureString(g, chr, font, data);
            ans = new SizeF(ans.Width + temp.Width, temp.Height);
        }
        return ans;
    }

    private static SizeF MeasureString(IDeviceContext g, char chr, Font font, FontData data)
    {
        var chrValue = (int) chr;
        if ((font.Bold && font.Italic) || font.Bold)
        {
            if (chrValue > 255 && data.BoldCharacter[chrValue].Width == 0)
            {
                data.BoldCharacter[chrValue] = FontData.MeasureString(g, font, chr.ToString());
            }
            return data.BoldCharacter[chrValue];
        }
        if (font.Italic)
        {
            if (chrValue > 255 && data.ItalicCharacter[chrValue].Width == 0)
            {
                data.ItalicCharacter[chrValue] = FontData.MeasureString(g, font, chr.ToString());
            }
            return data.ItalicCharacter[chrValue];
        }
        if (chrValue > 255 && data.NormalCharacter[chrValue].Width == 0)
        {
            data.NormalCharacter[chrValue] = FontData.MeasureString(g, font, chr.ToString());
        }
        return data.NormalCharacter[chrValue];
    }
}