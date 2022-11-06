using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class BoardDetectTests
    {
        [TestMethod()]
        public void DetectBoardTest()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                        { board = "fmuv2", description = "", hardwareid = @"USB\VID_0483&PID_5740", name = "" },

                });

            if (ans != BoardDetect.boards.px4v2)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest1()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo()
                        { board = "fmuv2-bl", description = "", hardwareid = @"USB\VID_1209&PID_5740", name = "" },

                });

            if (ans != BoardDetect.boards.px4v2)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest2()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo()
                        { board = "CubeBlack", description = "", hardwareid = @"USB\VID_2DAE&PID_0016", name = "" },

                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest3()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "", description = "", hardwareid = @"USB\VID_2341&PID_0010", name = "" },

                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest4()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "", description = "", hardwareid = @"USB\VID_26AC&PID_0010", name = "" },

                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest5()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "", description = "", hardwareid = @"USB\VID_26AC&PID_0032", name = "" },

                });
            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest6()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {


                    new DeviceInfo() { board = "", description = "", hardwareid = @"USB\VID_26AC&PID_0001", name = "" },


                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest7()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {



                    new DeviceInfo()
                    {
                        board = "", description = "chibios or normal px4", hardwareid = @"USB\VID_0483&PID_5740",
                        name = ""
                    },

                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest8()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {




                    new DeviceInfo()
                    {
                        board = "", description = "chibios or normal px4", hardwareid = @"USB\VID_26AC&PID_0011",
                        name = ""
                    },
                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest9()
        {
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {


                    new DeviceInfo()
                    {
                        board = "", description = "chibios or normal px4", hardwareid = @"USB\VID_1209&PID_5740",
                        name = ""
                    },
                });

            if (ans != BoardDetect.boards.chbootloader)
                Assert.Fail();
        }
    }
}