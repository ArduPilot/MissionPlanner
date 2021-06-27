using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil.Cil;
using System.Linq;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>(new DebugInProcessConfig());

            var st = DateTime.Now.ToString("ss.fffff");

            Console.WriteLine(st);
            //ModifyEXE(args);
        }


        [Benchmark]
        public void ByteArrayToStructure()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }
 var obj = (object) new MAVLink.mavlink_heartbeat_t();
            int a = 0;
            for (a = 0; a < 1000000; a++)
            {
               
                MavlinkUtil.ByteArrayToStructure(array, ref obj, 6, 5);
            }

        }

        [Benchmark]
        public void ByteArrayToStructureT()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }

            int a = 0;
            for (a = 0; a < 1000000; a++)
            {
                var ans1 = MavlinkUtil.ByteArrayToStructureT<MAVLink.mavlink_heartbeat_t>(array, 6);
            }

        }

        [Benchmark]
        public void ReadUsingPointer()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }

            int a = 0;
            for (a = 0; a < 1000000; a++)
            {
                var ans2 = MavlinkUtil.ReadUsingPointer<MAVLink.mavlink_heartbeat_t>(array, 6);
            }


        }

        [Benchmark]
        public void ByteArrayToStructureGCT()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }

            int a = 0;

            for (a = 0; a < 1000000; a++)
            {
                var ans3 = MavlinkUtil.ByteArrayToStructureGC<MAVLink.mavlink_heartbeat_t>(array, 6);
            }

        }

        [Benchmark]
        public void ByteArrayToStructureGC()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }

            int a = 0;


            for (a = 0; a < 1000000; a++)
            {
                var ans4 = MavlinkUtil.ByteArrayToStructureGC(array, typeof(MAVLink.mavlink_heartbeat_t), 6, 5);
            }


        }

        [Benchmark]
        public void ByteArrayToStructureGCArray()
        {
            var array = new byte[100];

            for (var l = 0; l < array.Length; l++)
            {
                array[l] = (byte) l;
            }

            int a = 0;
            for (a = 0; a < 1000000; a++)
            {
                var ans4 = MavlinkUtil.ByteArrayToStructureGCArray(array, typeof(MAVLink.mavlink_heartbeat_t), 6,
                    5);
            }


        }

        private static void ModifyEXE(string[] args)
        {
            string fileName = args[0];
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(fileName)));
            if (module.Kind == ModuleKind.Windows)
                module.Kind = ModuleKind.Console;
            MethodReference consoleWriteLine =
                module.ImportReference(typeof(Console).GetMethod("WriteLine", new Type[] {typeof(object)}));
            
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    var ilProcessor = method.Body.GetILProcessor();

                    var ldStrInstruction = Instruction.Create(OpCodes.Ldstr,
                        $"+ {method.FullName}");
                    var callInstruction = Instruction.Create(OpCodes.Call, consoleWriteLine);

                    var firstInstruction = method.Body.Instructions.First();
                    ilProcessor.InsertBefore(firstInstruction, ldStrInstruction);
                    ilProcessor.InsertBefore(firstInstruction, callInstruction);

                    var ldStr2Instruction = Instruction.Create(OpCodes.Ldstr, $"- {method.FullName}");
                    var lastInstruction = method.Body.Instructions.Last();
                    //ilProcessor.InsertBefore(lastInstruction, ldStr2Instruction);
                    //ilProcessor.InsertBefore(lastInstruction, callInstruction);
                }
            }

            module.Write(Path.GetFileNameWithoutExtension(fileName) + /*".modified" +*/ Path.GetExtension(fileName));
        }
    }
}