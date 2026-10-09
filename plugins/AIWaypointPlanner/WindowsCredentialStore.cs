using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class WindowsCredentialStore
    {
        public const string CredentialTarget = "MissionPlanner.AIWaypointPlanner.OpenAI";
        private readonly string targetName;

        private const uint CredTypeGeneric = 1;
        private const uint CredPersistLocalMachine = 2;
        private const int ErrorNotFound = 1168;

        public WindowsCredentialStore()
            : this(CredentialTarget)
        {
        }

        public WindowsCredentialStore(string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
                throw new ArgumentException("A credential target is required.", "targetName");
            this.targetName = targetName;
        }

        public string Read()
        {
            IntPtr credentialPointer;
            if (!CredRead(targetName, CredTypeGeneric, 0, out credentialPointer))
            {
                int error = Marshal.GetLastWin32Error();
                if (error == ErrorNotFound)
                    return null;
                throw new Win32Exception(error, "Windows Credential Manager could not be read.");
            }

            try
            {
                var credential = (NativeCredential)Marshal.PtrToStructure(
                    credentialPointer, typeof(NativeCredential));
                if (credential.CredentialBlob == IntPtr.Zero || credential.CredentialBlobSize == 0)
                    return null;

                var bytes = new byte[credential.CredentialBlobSize];
                Marshal.Copy(credential.CredentialBlob, bytes, 0, bytes.Length);
                return Encoding.Unicode.GetString(bytes).TrimEnd('\0');
            }
            finally
            {
                CredFree(credentialPointer);
            }
        }

        public void Write(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("An API key is required.", "apiKey");

            byte[] secretBytes = Encoding.Unicode.GetBytes(apiKey.Trim());
            IntPtr secretPointer = Marshal.AllocCoTaskMem(secretBytes.Length);
            try
            {
                Marshal.Copy(secretBytes, 0, secretPointer, secretBytes.Length);
                var credential = new NativeCredential
                {
                    Type = CredTypeGeneric,
                    TargetName = targetName,
                    CredentialBlobSize = (uint)secretBytes.Length,
                    CredentialBlob = secretPointer,
                    Persist = CredPersistLocalMachine,
                    UserName = Environment.UserName
                };

                if (!CredWrite(ref credential, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Windows Credential Manager could not be written.");
            }
            finally
            {
                for (int i = 0; i < secretBytes.Length; i++)
                    secretBytes[i] = 0;
                ZeroMemory(secretPointer, secretBytes.Length);
                Marshal.FreeCoTaskMem(secretPointer);
            }
        }

        public bool Delete()
        {
            if (CredDelete(targetName, CredTypeGeneric, 0))
                return true;

            int error = Marshal.GetLastWin32Error();
            if (error == ErrorNotFound)
                return false;
            throw new Win32Exception(error, "The Windows Credential Manager entry could not be deleted.");
        }

        private static void ZeroMemory(IntPtr pointer, int length)
        {
            for (int i = 0; i < length; i++)
                Marshal.WriteByte(pointer, i, 0);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NativeCredential
        {
            public uint Flags;
            public uint Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }

        [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredRead(string target, uint type, uint flags, out IntPtr credentialPointer);

        [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredWrite(ref NativeCredential credential, uint flags);

        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredDelete(string target, uint type, uint flags);

        [DllImport("advapi32.dll", SetLastError = false)]
        private static extern void CredFree(IntPtr credentialPointer);
    }
}
