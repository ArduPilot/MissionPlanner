using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyDescription("System.Drawing.Common")]
[assembly: AssemblyDefaultAlias("System.Drawing.Common")]

//[assembly: AssemblyCompany(Consts.MonoCompany)]
//[assembly: AssemblyProduct(Consts.MonoProduct)]
[assembly: AssemblyCopyright(Consts.MonoCopyright)]
//[assembly: AssemblyVersion(Consts.FxVersion)]
[assembly: SatelliteContractVersion(Consts.FxVersion)]
//[assembly: AssemblyInformationalVersion(Consts.FxFileVersion)]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: ComVisible(false)]
[assembly: ComCompatibleVersion(1, 0, 3300, 0)]
//[assembly: AllowPartiallyTrustedCallers]

[assembly: CLSCompliant(true)]
//[assembly: AssemblyDelaySign (true)]

//[assembly: AssemblyFileVersion(Consts.FxFileVersion)]
[assembly: CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
[assembly: Dependency("System,", LoadHint.Always)]



//[assembly: AssemblyDelaySign(true)]
//[assembly: AssemblyKeyFile("ecma.pub")]

[assembly: TypeForwardedTo(typeof(Color))]
//[assembly: TypeForwardedTo(typeof(ColorConverter))]
//[assembly: TypeForwardedTo(typeof(ColorTranslator))]
[assembly: TypeForwardedTo(typeof(Point))]
//[assembly: TypeForwardedTo(typeof(PointConverter))]
[assembly: TypeForwardedTo(typeof(PointF))]
[assembly: TypeForwardedTo(typeof(Size))]
//[assembly: TypeForwardedTo(typeof(SizeConverter))]
[assembly: TypeForwardedTo(typeof(SizeF))]
//[assembly: TypeForwardedTo(typeof(SizeFConverter))]
[assembly: TypeForwardedTo(typeof(Rectangle))]
//[assembly: TypeForwardedTo(typeof(RectangleConverter))]
[assembly: TypeForwardedTo(typeof(RectangleF))]

static class Consts
{
    //
    // Use these assembly version constants to make code more maintainable.
    //

    public const string MonoVersion = "@MONO_VERSION@";
    public const string MonoCompany = "Mono development team";
    public const string MonoProduct = "Mono Common Language Infrastructure";
    public const string MonoCopyright = "(c) Various Mono authors";
    public const int MonoCorlibVersion = 1;


    public const string FxVersion = "4.0.0.0";
    public const string FxFileVersion = "4.6.57.0";
    public const string EnvironmentVersion = "4.0.30319.42000";

}