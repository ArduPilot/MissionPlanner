namespace System.Drawing.Imaging
{
    public enum PixelFormat
    {
        Indexed = 0x10000,
        Gdi = 0x20000,
        Alpha = 0x40000,
        PAlpha = 0x80000,
        Extended = 0x100000,
        Canonical = 0x200000,
        Undefined = 0,
        DontCare = 0,
        Format1bppIndexed = 196865,
        Format4bppIndexed = 197634,
        Format8bppIndexed = 198659,
        Format16bppGrayScale = 1052676,
        Format16bppRgb555 = 135173,
        Format16bppRgb565 = 135174,
        Format16bppArgb1555 = 397319,
        Format24bppRgb = 137224,
        Format32bppRgb = 139273,
        Format32bppArgb = 2498570,
        Format32bppPArgb = 925707,
        Format48bppRgb = 1060876,
        Format64bppArgb = 3424269,
        Format64bppPArgb = 1851406,
        Max = 0xF
    }
}