using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MissionPlannerTests.OpenGL
{
    /// <summary>
    /// Renders the FlightData HUD through OpenGL in a real window and checks the frame.
    /// Needs an interactive desktop. CI points MP_OPENGL32_PATH at Mesa's llvmpipe
    /// opengl32.dll and sets MP_EXPECT_GL_RENDERER=llvmpipe; locally it validates
    /// whatever driver is installed. See HudOpenGLProbe for the environment variables.
    /// </summary>
    [TestClass]
    public class HudOpenGLTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("OpenGL")]
        public void HudRendersWithOpenGL()
        {
            var result = HudOpenGLProbe.Run();
            TestContext.WriteLine(result.ToString());
            Assert.IsTrue(result.Passed, result.ToString());
        }
    }
}
