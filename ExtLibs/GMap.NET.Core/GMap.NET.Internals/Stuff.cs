
namespace GMap.NET.Internals
{
    using Org.BouncyCastle.Crypto.Digests;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using System;
   using System.Collections.Generic;
   using System.ComponentModel;
   using System.IO;
   using System.Reflection;
   using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// etc functions...
    /// </summary>
    public class Stuff
   {
      public static string EnumToString(Enum value)
      {
         FieldInfo fi = value.GetType().GetField(value.ToString());
         DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
               typeof(DescriptionAttribute), false);

         return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
      }

      [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
      [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
      public static extern bool SetCursorPos(int X, int Y);

      public static readonly Random random = new System.Random();

      public static void Shuffle<T>(List<T> deck)
      {
         int N = deck.Count;

         for(int i = 0; i < N; ++i)
         {
            int r = i + (int)(random.Next(N - i));
            T t = deck[r];
            deck[r] = deck[i];
            deck[i] = t;
         }
      }

      public static MemoryStream CopyStream(Stream inputStream, bool SeekOriginBegin)
      {
         const int readSize = 32 * 1024;
         byte[] buffer = new byte[readSize];
         MemoryStream ms = new MemoryStream();
         {
            int count = 0;
            while((count = inputStream.Read(buffer, 0, readSize)) > 0)
            {
               ms.Write(buffer, 0, count);
            }
         }
         buffer = null;
         if(SeekOriginBegin)
         {
            inputStream.Seek(0, SeekOrigin.Begin);
         }
         ms.Seek(0, SeekOrigin.Begin);
         return ms;
      }

      public static bool IsRunningOnVistaOrLater()
      {
         OperatingSystem os = Environment.OSVersion;

         if(os.Platform == PlatformID.Win32NT)
         {
            Version vs = os.Version;

            if(vs.Major >= 6 && vs.Minor >= 0)
            {
               return true;
            }
         }

         return false;
      }

      public static bool IsRunningOnWin7orLater()
      {
         OperatingSystem os = Environment.OSVersion;

         if(os.Platform == PlatformID.Win32NT)
         {
            Version vs = os.Version;

            if(vs.Major >= 6 && vs.Minor > 0)
            {
               return true;
            }
         }

         return false;
      }

      public static void RemoveInvalidPathSymbols(ref string url)
      {
#if !PocketPC
         char[] ilg = Path.GetInvalidFileNameChars();
#else
            char[] ilg = new char[41];
            for(int i = 0; i < 32; i++)
               ilg[i] = (char) i;

            ilg[32] = '"';
            ilg[33] = '<';
            ilg[34] = '>';
            ilg[35] = '|';
            ilg[36] = '?';
            ilg[37] = ':';
            ilg[38] = '/';
            ilg[39] = '\\';
            ilg[39] = '*';
#endif
         foreach(char c in ilg)
         {
            url = url.Replace(c, '_');
         }
      }

        #region -- encryption --
        static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;

            var md = new Sha1Digest();
            byte[] TDESKey = new byte[32];
            var data = Encoding.UTF8.GetBytes(Passphrase);
            md.BlockUpdate(data, 0, data.Length);
            md.DoFinal(TDESKey, 0);
            Array.Resize(ref TDESKey, 16);

            var cp = CipherUtilities.GetCipher("DESede/ECB/PKCS7Padding");
            var keySpec = new DesEdeParameters(TDESKey);
            cp.Init(true, keySpec);
            Results = cp.DoFinal(Encoding.UTF8.GetBytes(Message));
            return Convert.ToBase64String(Results);
        }

        static string DecryptString(string Message, string Passphrase)
        {
            byte[] Results;

            var md = new Sha1Digest();
            byte[] TDESKey = new byte[32];
            var data = Encoding.UTF8.GetBytes(Passphrase);
            md.BlockUpdate(data, 0, data.Length);
            md.DoFinal(TDESKey, 0);
            Array.Resize(ref TDESKey, 16);

            var cp = CipherUtilities.GetCipher("DESede/ECB/PKCS7Padding");
            var keySpec = new DesEdeParameters(TDESKey);
            cp.Init(false, keySpec);
            Results = cp.DoFinal(Convert.FromBase64String(Message));
            return Encoding.UTF8.GetString(Results, 0, Results.Length);
        }

      public static string EncryptString(string Message)
      {
          return EncryptString(Message, manifesto);
      }

      public static string GString(string Message)
      {
          var ret = DecryptString(Message, manifesto);

          return ret;
      }

      static readonly string manifesto = "GMap.NET is great and Powerful, Free, cross platform, open source .NET control.";
      #endregion
   }

#if PocketPC
   static class Monitor
   {
      static readonly OpenNETCF.Threading.Monitor2 wait = new OpenNETCF.Threading.Monitor2();

      public static void Enter(Stack<LoadTask> tileLoadQueue)
      {
         wait.Enter();
      }

      public static void Exit(Stack<LoadTask> tileLoadQueue)
      {
         wait.Exit();
      }

      public static void Wait(Stack<LoadTask> tileLoadQueue)
      {
         wait.Wait();
      }

      public static bool Wait(Stack<LoadTask> tileLoadQueue, int WaitForTileLoadThreadTimeout, bool p)
      {
         wait.Wait();
         return true;
      }

      internal static void PulseAll(Stack<LoadTask> tileLoadQueue)
      {
         wait.PulseAll();
      }
   }
#endif
}
