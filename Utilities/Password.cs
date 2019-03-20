using System;
using System.Security.Cryptography;
using System.Text;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    // one way encryption
    public static class Password
    {
        static string pw = "";

        public static void EnterPassword()
        {
            // keep this one local
            string pw = "";

            InputBox.Show("Enter Password", "Please enter a password", ref pw, true);

            Settings.Instance["password"] =
                Convert.ToBase64String(Password.GenerateSaltedHash(UTF8Encoding.UTF8.GetBytes(pw),
                    new byte[] {(byte) 'M', (byte) 'P'}));
        }

        public static bool VerifyPassword()
        {
            //check if we have already entered the pw and its valid.
            if (ValidatePassword(pw) == true)
                return true;

            if (InputBox.Show("Enter Password", "Please enter your password", ref pw, true) ==
                System.Windows.Forms.DialogResult.OK)
            {
                bool ans = ValidatePassword(pw);

                if (ans == false)
                {
                    CustomMessageBox.Show("Bad Password", "Bad Password");
                }

                return ans;
            }

            return false;
        }

        static bool ValidatePassword(string pw)
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
            HashAlgorithm algorithm = new SHA256Managed();

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