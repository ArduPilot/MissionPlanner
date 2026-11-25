using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MissionPlanner.Utilities.Tests
{
    /// <summary>
    /// The GeoRef image route on the built-in web server must only serve .jpg files directly inside
    /// the GeoRef image directory (CWE-22).
    /// </summary>
    [TestClass]
    public class HttpServerGeoRefPathTests
    {
        private string root;
        private string georefDir;
        private string inside;
        private string outside;

        [TestInitialize]
        public void Setup()
        {
            root = Path.Combine(Path.GetTempPath(), "mp-georef-" + Guid.NewGuid().ToString("N"));
            georefDir = Path.Combine(root, "images");
            Directory.CreateDirectory(georefDir);

            inside = Path.Combine(georefDir, "img_0001.jpg");
            outside = Path.Combine(root, "secret.jpg");
            File.WriteAllText(inside, "x");
            File.WriteAllText(outside, "x");
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Directory.Delete(root, true); } catch { }
        }

        // georefimage.cs sets the base as the chosen directory plus a trailing separator
        private string Base => georefDir + Path.DirectorySeparatorChar;

        [TestMethod]
        public void ServesImageInGeoRefDirectory()
        {
            Assert.AreEqual(Path.GetFullPath(inside),
                httpserver.ResolveGeoRefImagePath(Base, "/img_0001.jpg"));
        }

        [TestMethod]
        public void IgnoresQueryString()
        {
            Assert.AreEqual(Path.GetFullPath(inside),
                httpserver.ResolveGeoRefImagePath(Base, "/img_0001.jpg?x=1"));
        }

        [TestMethod]
        public void RejectsDotDotTraversal()
        {
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/../secret.jpg"));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/..\\secret.jpg"));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/%2e%2e/secret.jpg"));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/%2e%2e%5csecret.jpg"));
        }

        [TestMethod]
        public void RejectsAbsolutePaths()
        {
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, outside));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/" + outside));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "//server/share/a.jpg"));
        }

        [TestMethod]
        public void RejectsWhenNoGeoRefDirectorySet()
        {
            // before GeoRef has run the base is "", which used to let an absolute path straight through
            Assert.IsNull(httpserver.ResolveGeoRefImagePath("", outside));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath("", "/img_0001.jpg"));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(null, "/img_0001.jpg"));
        }

        [TestMethod]
        public void RejectsNonJpegAndMissingFiles()
        {
            File.WriteAllText(Path.Combine(georefDir, "notes.txt"), "x");
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/notes.txt"));
            Assert.IsNull(httpserver.ResolveGeoRefImagePath(Base, "/missing.jpg"));
        }
    }
}
