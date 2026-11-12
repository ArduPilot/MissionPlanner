using System;
using System.IO;
using System.Runtime.InteropServices;
using MissionPlanner.Utilities;
using MissionPlannerTests.OpenGL;

// Renders the HUD through OpenGL in the extracted package and validates the frame.
// Compiled by Linux/build.sh together with MissionPlannerTests/OpenGL/HudOpenGLProbe.cs.
class OpenGLSelfTest
{
    // See SmokeTest.cs: Mono can hang tearing down background threads in Environment.Exit.
    [DllImport("libc", EntryPoint = "_exit")]
    private static extern void Exit(int status);

    [STAThread]
    static int Main()
    {
        Settings.CustomUserDataDirectory = Path.Combine(Path.GetTempPath(), "missionplanner-gltest-" + Guid.NewGuid());
        Directory.CreateDirectory(Settings.GetUserDataDirectory());

        var result = HudOpenGLProbe.Run();
        Console.WriteLine(result);
        if (result.Passed)
            Console.WriteLine("OPENGL_SELF_TEST_PASS");
        Console.Out.Flush();
        Console.Error.Flush();
        var status = result.Passed ? 0 : 1;
        if (Environment.OSVersion.Platform == PlatformID.Unix)
            Exit(status);
        return status;
    }
}
