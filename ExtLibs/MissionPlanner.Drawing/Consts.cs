//[assembly: AssemblyTitle("System.Drawing.dll")]

using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyDescription("System.Drawing.dll")]
[assembly: AssemblyDefaultAlias("System.Drawing.dll")]

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

    public const string VsVersion = "0.0.0.0"; // Useless ?
    public const string VsFileVersion = "11.0.0.0"; // TODO:


#if MOBILE
	const string PublicKeyToken = "7cec85d7bea7798e";
#else
    const string PublicKeyToken = "b77a5c561934e089";
#endif

    //
    // Use these assembly name constants to make code more maintainable.
    //

    public const string AssemblyI18N =
        "I18N, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=0738eb9f132ed756";

    public const string AssemblyMicrosoft_JScript =
        "Microsoft.JScript, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblyMicrosoft_VisualStudio =
        "Microsoft.VisualStudio, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblyMicrosoft_VisualStudio_Web =
        "Microsoft.VisualStudio.Web, Version=" + VsVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblyMicrosoft_VSDesigner =
        "Microsoft.VSDesigner, Version=" + VsVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblyMono_Http =
        "Mono.Http, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=0738eb9f132ed756";

    public const string AssemblyMono_Posix =
        "Mono.Posix, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=0738eb9f132ed756";

    public const string AssemblyMono_Security =
        "Mono.Security, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=0738eb9f132ed756";

    public const string AssemblyMono_Messaging_RabbitMQ =
        "Mono.Messaging.RabbitMQ, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=0738eb9f132ed756";

    public const string AssemblyCorlib =
        "mscorlib, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=" + PublicKeyToken;

    public const string AssemblySystem =
        "System, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089";

    public const string AssemblySystem_Data =
        "System.Data, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089";

    public const string AssemblySystem_Design =
        "System.Design, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_DirectoryServices =
        "System.DirectoryServices, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Drawing =
        "System.Drawing, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Drawing_Design =
        "System.Drawing.Design, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Messaging =
        "System.Messaging, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Security =
        "System.Security, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_ServiceProcess =
        "System.ServiceProcess, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Web =
        "System.Web, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    public const string AssemblySystem_Windows_Forms =
        "System.Windows.Forms, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089";
#if NET_4_0
	public const string AssemblySystem_2_0 =
 "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
	public const string AssemblySystemCore_3_5 =
 "System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
	public const string AssemblySystem_Core =
 "System.Core, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089";
	public const string WindowsBase_3_0 = "WindowsBase, Version=3.0.0.0, PublicKeyToken=31bf3856ad364e35";
	public const string AssemblyWindowsBase =
 "WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
	public const string AssemblyPresentationCore_3_5 =
 "PresentationCore, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
	public const string AssemblyPresentationCore_4_0 =
 "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
	public const string AssemblyPresentationFramework_3_5 =
 "PresentationFramework, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
	public const string AssemblySystemServiceModel_3_0 =
 "System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
#elif MOBILE
	public const string AssemblySystem_Core =
 "System.Core, Version=" + FxVersion + ", Culture=neutral, PublicKeyToken=b77a5c561934e089";
#endif
}