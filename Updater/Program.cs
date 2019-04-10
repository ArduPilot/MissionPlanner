using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Updater
{
    internal static class Program
    {
        private static bool MAC = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            OperatingSystem os = Environment.OSVersion;

            Console.WriteLine(os.VersionString.ToString());

            if (os.VersionString.ToString().ToUpper().Contains("UNIX"))
            {
                MAC = true;
            }

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // give 4 seconds grace
            System.Threading.Thread.Sleep(5000);

            //UpdateFiles(path);

            if (!UpdateFiles(path))
            {
                Console.WriteLine("Update failed, please try it later.");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            else
            {
                try
                {
                    Directory.GetFiles(path, "*.old").ForEach(a =>
                    {
                        try
                        {
                            File.SetAttributes(a, FileAttributes.Normal);
                            File.Delete(a);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    });

                    System.Diagnostics.Process P = new System.Diagnostics.Process();
                    if (MAC)
                    {
                        P.StartInfo.FileName = "mono";
                        P.StartInfo.Arguments = " \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "MissionPlanner.exe\"";
                    }
                    else
                    {
                        P.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "MissionPlanner.exe";
                        P.StartInfo.Arguments = "";
                    }
                    Console.WriteLine("Start " + P.StartInfo.FileName + " with " + P.StartInfo.Arguments);
                    P.Start();
                }
                catch { } // likely file didnt exist
            }
        }

        private static bool UpdateFiles(string directory)
        {
            bool all_done = true;
            try
            {
                string[] files = Directory.GetFiles(directory);

                Console.WriteLine("dir: " + directory);

                foreach (string file in files)
                {
                    if (file.ToLower().EndsWith(".new") && file.ToLower() != ".new") // cant move ".new" to ""
                    {
                        Console.WriteLine("\t file: " + file);

                        bool done = false;
                        for (int try_count = 0; try_count < 10 && !done; try_count++)  // try no more than 5 times
                        {
                            if (file.ToLower().Contains("updater.exe"))
                            { // cant self update on windows
                                done = true;
                                break;
                            }
                            try
                            {
                                var oldfile = file.Remove(file.Length - 4) + ".old";
                                var newfile = file.Remove(file.Length - 4);

                                if (File.Exists(oldfile))
                                {
                                    try
                                    {
                                        File.SetAttributes(oldfile, FileAttributes.Normal);
                                        File.Delete(oldfile);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }

                                Console.Write("Move: " + file + " TO " + newfile);
                                // move existing to .old
                                if (File.Exists(newfile))
                                    File.Move(newfile, oldfile);
                                // move .new to existing
                                File.Move(file, newfile);
                                done = true;
                                Console.WriteLine(" Done.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(file + " Failed.");
                                Console.WriteLine(ex);
                                System.Threading.Thread.Sleep(500);
                                // normally in use by explorer.exe
                                if (file.ToLower().Contains("tlogthumbnailhandler"))
                                    done = true;
                            }
                        }
                        all_done = all_done && done;
                    }
                }
            }
            catch { }

            foreach (string subdir in Directory.GetDirectories(directory))
                all_done = all_done && UpdateFiles(subdir);

            return all_done;
            //P.StartInfo.RedirectStandardOutput = true;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T obj in enumerable)
                action(obj);
        }
    }
}