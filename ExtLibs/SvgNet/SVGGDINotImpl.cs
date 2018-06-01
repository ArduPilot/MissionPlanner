/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015, 2017 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/

using System;

namespace SvgNet.SvgGdi
{
    /// <summary>
    /// Exception thrown when a GDI+ operation is attempted on an IGraphics implementor that does not support the operation.
    /// For instance, <c>SvgGraphics</c> does not support any of the <c>MeasureString</c> methods.
    /// </summary>
    [Serializable]
    public class SvgGdiNotImpl : Exception
    {
        public SvgGdiNotImpl(string method)
        {
            _method = method;
        }

        public string Method => _method;

        private readonly string _method;
    }
}
