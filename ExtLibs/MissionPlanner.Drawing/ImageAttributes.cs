/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing.Imaging
{
    public class ImageAttributes : IDisposable
    {
        private ColorMatrix matrix;

        public void Dispose()
        {
        }

        public void SetColorMatrix(ColorMatrix matrix)
        {
            this.matrix = matrix;
        }

        public void SetWrapMode(WrapMode tileFlipXy)
        {
        }

        public void SetColorKey(Color itemImageTransparentColor, Color imageTransparentColor)
        {
        }

        public void ClearOutputChannelColorProfile()
        {
        }

        public void SetRemapTable(ColorMap[] colorMaps)
        {
        }
    }
}