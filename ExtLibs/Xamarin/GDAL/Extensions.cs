using Org.Gdal.Gdal;

namespace GDAL
{
    public static class Extensions
    {
        public static int GetCount(this ColorTable ct)
        {
            return ct.Count;
        }

        public static ColorTable GetRasterColorTable(this Band ct)
        {
            return ct.RasterColorTable;
        }   

        public static int GetColorInterpretation(this Band ct)
        {
            return ct.ColorInterpretation;
        }
        
    }

    public class ColorEntry
    {
        public static implicit operator ColorEntry(int d)
        {
            return new ColorEntry()
            {
                c1 = (d) & 0xff, //r 
                c2 = (d >> 8) & 0xff, //g 
                c3 = (d >> 16) & 0xff, //b
                c4 = (d >> 24) & 0xff //a
            };
        }

        public int c4;
        public int c1;
        public int c2;
        public int c3;
    }

    public enum DataType
    {
        GDT_Unknown,
        GDT_Byte,
        GDT_UInt16,
        GDT_Int16,
        GDT_UInt32,
        GDT_Int32,
        GDT_Float32,
        GDT_Float64,
        GDT_CInt16,
        GDT_CInt32,
        GDT_CFloat32,
        GDT_CFloat64,
        GDT_TypeCount
    }
    public enum ColorInterp
    {
        GCI_Undefined = 0,
        GCI_GrayIndex = 1,
        GCI_PaletteIndex = 2,
        GCI_RedBand = 3,
        GCI_GreenBand = 4,
        GCI_BlueBand = 5,
        GCI_AlphaBand = 6,
        GCI_HueBand = 7,
        GCI_SaturationBand = 8,
        GCI_LightnessBand = 9,
        GCI_CyanBand = 10,
        GCI_MagentaBand = 11,
        GCI_YellowBand = 12,
        GCI_BlackBand = 13,
        GCI_YCbCr_YBand = 14,
        GCI_YCbCr_CbBand = 0xF,
        GCI_YCbCr_CrBand = 0x10,
        GCI_Max = 0x10
    }

}