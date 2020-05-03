using System;

namespace ClipperLib
{
    class ClipperException : Exception
    {
        public ClipperException(string description) : base(description) { }
    }
}