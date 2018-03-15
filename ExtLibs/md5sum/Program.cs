using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace md5sum
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories);

            var obj = new object();

            Parallel.ForEach(files,
                file =>
                {
                    lock (obj)
                        Console.Write("{0} {1}\n",
                            MD5File(file),
                            file.TrimStart('\\', '/').Replace('\\','/'));
                });
        }

        static string MD5File(string filename)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var answer = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();

                        return answer;
                    }
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
