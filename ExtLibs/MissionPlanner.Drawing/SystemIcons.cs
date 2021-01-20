using System;
using System.IO;

namespace System.Drawing
{
    public sealed class SystemIcons
    {
        public static Icon Application
        {
            get { return new Icon(new MemoryStream(Resource.Application)); }
        }

        public static Icon Asterisk
        {
            get { return new Icon(new MemoryStream(Resource.Asterisk)); }
        }

        public static Icon Error
        {
            get { return new Icon(new MemoryStream(Resource.Error)); }
        }

        public static Icon Exclamation
        {
            get { return new Icon(new MemoryStream(Resource.Exclamation)); }
        }

        public static Icon Hand
        {
            get { return new Icon(new MemoryStream(Resource.Hand)); }
        }

        public static Icon Information
        {
            get { return new Icon(new MemoryStream(Resource.Information)); }
        }

        public static Icon Question
        {
            get { return new Icon(new MemoryStream(Resource.Question)); }
        }

        public static Icon Shield
        {
            get { return new Icon(new MemoryStream(Resource.Shield)); }
        }

        public static Icon Warning
        {
            get { return new Icon(new MemoryStream(Resource.Warning)); }
        }

        public static Icon WinLogo
        {
            get { return new Icon(new MemoryStream(Resource.WinLogo)); }
        }
    }
}