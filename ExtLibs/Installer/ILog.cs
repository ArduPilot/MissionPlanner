using System;

namespace log4net
{
    internal interface ILog
    {
        void Debug(string v);
        void Info(string v);
        void InfoFormat(string Format, params object[] args);
    }

    internal class Log : ILog
    {
        public void Debug(string v)
        {
            Console.WriteLine(v);
        }

        public void Info(string v)
        {
            Console.WriteLine(v);
        }

        public void InfoFormat(string Format, params object[] args)
        {
            Console.WriteLine(Format, args);
        }
    }
}