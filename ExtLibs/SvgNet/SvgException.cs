/*
    Copyright © 2003 RiskCare Ltd. All rights reserved.
    Copyright © 2010 SvgNet & SvgGdi Bridge Project. All rights reserved.
    Copyright © 2015, 2017 Rafael Teixeira, Mojmír Němeček, Benjamin Peterson and Other Contributors

    Original source code licensed with BSD-2-Clause spirit, treat it thus, see accompanied LICENSE for more
*/


using System;

namespace SvgNet
{
    /// <summary>
    /// A general-purpose exception for problems that occur in SvgNet.
    /// </summary>
    [Serializable]
    public class SvgException : Exception
    {
        private readonly string _ctx;
        private readonly string _msg;

        public SvgException(string msg, string ctx)
        {
            _msg = msg;
            _ctx = ctx;
        }

        public SvgException(string msg)
        {
            _msg = msg;
            _ctx = "";
        }

        /// <summary>
        /// A string intended to supply context information.
        /// </summary>
        public string Ctx
        {
            get { return _ctx; }
        }

        /// <summary>
        /// A message describing the problem.
        /// </summary>
        public string Msg
        {
            get { return _msg; }
        }
    }
}
