using System;
using System.Collections.Generic;
using System.Text;

namespace OSGeo.GDAL
{
    public class Gdal
    {
        internal static string GetConfigOption(string v, string notSet)
        {
            throw new NotImplementedException();
        }

        internal static void SetConfigOption(string v, string gdalData)
        {
            throw new NotImplementedException();
        }

        internal static void AllRegister()
        {
            throw new NotImplementedException();
        }

        internal static int GetDriverCount()
        {
            return 0;
        }

        internal static (string ShortName, string LongName) GetDriver(int i)
        {
            throw new NotImplementedException();
        }
    }
}
namespace OSGeo.OGR
{
    public class Ogr
    {
        internal static void RegisterAll()
        {
            throw new NotImplementedException();
        }

        internal static int GetDriverCount()
        {
            return 0;
        }

        internal static Driver GetDriver(int i)
        {
            throw new NotImplementedException();
        }
    }

    internal class Driver
    {
        internal string GetName()
        {
            throw new NotImplementedException();
        }
    }
}
namespace System.Speech.Synthesis
{
    public class SpeechSynthesizer
    {
        public SynthesizerState State { get; set; }

        public void SpeakAsyncCancelAll()
        {

        }

        public void Dispose()
        {

        }

        public void SpeakAsync(string text)
        {

        }
    }

    public enum SynthesizerState
    {
        Ready,
        Speaking
    }
}