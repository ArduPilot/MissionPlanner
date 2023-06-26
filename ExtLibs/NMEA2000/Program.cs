namespace NMEA2000
{
    class Program
    {
        static void Main(string[] args)
        {
            msggen.run();

            ConvertCANLogToYDWG.runme();
        }
    }
}