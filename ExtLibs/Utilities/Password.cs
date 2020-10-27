using System;
using System.Security.Cryptography;
using System.Text;

namespace MissionPlanner.Utilities
{
    // one way encryption
    public static class Password
    {
        static string pw = "";

        public static void EnterPassword(string pw)
        {
            Settings.Instance["password"] =
                Convert.ToBase64String(Password.GenerateSaltedHash(UTF8Encoding.UTF8.GetBytes(pw),
                    new byte[] {(byte) 'M', (byte) 'P'}));
        }

        public static bool VerifyPassword(string pw)
        {
            //check if we have already entered the pw and its valid.
            if (ValidatePassword(pw) == true)
                return true;

            return false;
        }

        public static bool ValidatePassword(string pw)
        {
            byte[] ans = Password.GenerateSaltedHash(UTF8Encoding.UTF8.GetBytes(pw), new byte[] {(byte) 'M', (byte) 'P'});

            if (Password.CompareByteArrays(ans, Convert.FromBase64String(""+Settings.Instance["password"])))
            {
                return true;
            }

            return false;
        }

        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            using (SHA256CryptoServiceProvider algorithm = new SHA256CryptoServiceProvider())
            {

                byte[] plainTextWithSaltBytes =
                    new byte[plainText.Length + salt.Length];

                for (int i = 0; i < plainText.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainText[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainText.Length + i] = salt[i];
                }

                return algorithm.ComputeHash(plainTextWithSaltBytes);
            }
        }

        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}