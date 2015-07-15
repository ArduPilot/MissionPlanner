﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using System.IO;
using MissionPlanner;

namespace MissionPlanner
{
    public class Script
    {
        DateTime timeout = DateTime.Now;
        //List<string> items = new List<string>();
        static Microsoft.Scripting.Hosting.ScriptEngine engine;
        static Microsoft.Scripting.Hosting.ScriptScope scope;

        // keeps history
        MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

        internal Utilities.StringRedirectWriter OutputWriter { get; private set; }

        public Script(bool redirectOutput = false)
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["Debug"] = true;

            if (engine != null)
                engine.Runtime.Shutdown();

            engine = Python.CreateEngine(options);
            scope = engine.CreateScope();

            var all = System.Reflection.Assembly.GetExecutingAssembly();
            engine.Runtime.LoadAssembly(all);
            scope.SetVariable("MAV", MainV2.comPort);
            scope.SetVariable("cs", MainV2.comPort.MAV.cs);
            scope.SetVariable("Script", this);
            scope.SetVariable("mavutil", this);
            scope.SetVariable("Joystick", MainV2.joystick);

            engine.CreateScriptSourceFromString("print 'hello world from python'").Execute(scope);
            engine.CreateScriptSourceFromString("print cs.roll").Execute(scope);

            if (redirectOutput)
            {
                //Redirect output through this writer
                //this writer will not actually write to the memorystreams
                OutputWriter = new Utilities.StringRedirectWriter();
                engine.Runtime.IO.SetErrorOutput(new MemoryStream(), OutputWriter);
                engine.Runtime.IO.SetOutput(new MemoryStream(), OutputWriter);
            }
            else
                OutputWriter = null;

            /*
            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            foreach (var field in test.GetProperties())
            {
                // field.Name has the field's name.
                object fieldValue;
                try
                {
                    fieldValue = field.GetValue(thisBoxed, null); // Get value
                }
                catch { continue; }

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());

                items.Add(field.Name);
            }
             */
        }

        public object mavlink_connection(string device, int baud = 115200, int source_system = 255,
                       bool write = false, bool append = false,
                       bool robust_parsing = true, bool notimestamps = false, bool input = true)
        {

            return null;
        }

        public object recv_match(string condition = null, string type = null, bool blocking = false)
        {

            return null;
        }

        public void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public void runScript(string filename)
        {
            try
            {
                Console.WriteLine("Run Script " + scope);
                engine.CreateScriptSourceFromFile(filename).Execute(scope);
                Console.WriteLine("Run Script Done");
            }
            catch (Exception e)
            {
                CustomMessageBox.Show("Error running script " + e.Message);
            }
        }

        public enum Conditional
        {
            NONE = 0,
            LT,
            LTEQ,
            EQ,
            GT,
            GTEQ,
            NEQ
        }

        public bool ChangeParam(string param, float value)
        {
            return MainV2.comPort.setParam(param, value);
        }

        public float GetParam(string param)
        {
            if (MainV2.comPort.MAV.param[param] != null)
                return (float)MainV2.comPort.MAV.param[param];

            return 0.0f;
        }

        public bool ChangeMode(string mode)
        {
            MainV2.comPort.setMode(mode);
            return true;
        }

        public bool WaitFor(string message, int timeout)
        {
            int timein = 0;
            while (!MainV2.comPort.MAV.cs.message.Contains(message))
            {
                System.Threading.Thread.Sleep(5);
                timein += 5;
                if (timein > timeout)
                    return false;
            }

            return true;
        }

        public bool SendRC(int channel, ushort pwm, bool sendnow)
        {
            switch (channel)
            {
                case 1:
                    MainV2.comPort.MAV.cs.rcoverridech1 = pwm;
                    rc.chan1_raw = pwm;
                    break;
                case 2:
                    MainV2.comPort.MAV.cs.rcoverridech2 = pwm;
                    rc.chan2_raw = pwm;
                    break;
                case 3:
                    MainV2.comPort.MAV.cs.rcoverridech3 = pwm;
                    rc.chan3_raw = pwm;
                    break;
                case 4:
                    MainV2.comPort.MAV.cs.rcoverridech4 = pwm;
                    rc.chan4_raw = pwm;
                    break;
                case 5:
                    MainV2.comPort.MAV.cs.rcoverridech5 = pwm;
                    rc.chan5_raw = pwm;
                    break;
                case 6:
                    MainV2.comPort.MAV.cs.rcoverridech6 = pwm;
                    rc.chan6_raw = pwm;
                    break;
                case 7:
                    MainV2.comPort.MAV.cs.rcoverridech7 = pwm;
                    rc.chan7_raw = pwm;
                    break;
                case 8:
                    MainV2.comPort.MAV.cs.rcoverridech8 = pwm;
                    rc.chan8_raw = pwm;
                    break;
            }

            rc.target_component = MainV2.comPort.MAV.compid;
            rc.target_system = MainV2.comPort.MAV.sysid;

            if (sendnow)
            {
                MainV2.comPort.sendPacket(rc);
                System.Threading.Thread.Sleep(20);
                MainV2.comPort.sendPacket(rc);
            }

            return true;
        }
    }
}