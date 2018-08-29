using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    [TestFixture]
    public class OpenBsdBCryptTest
        :   SimpleTest
    {
        private static readonly string[][] BCryptTest1 = // vectors from http://cvsweb.openwall.com/cgi/cvsweb.cgi/Owl/packages/glibc/crypt_blowfish/wrapper.c?rev=HEAD
        {
            new string[]{"$2a$05$CCCCCCCCCCCCCCCCCCCCC.E5YPO9kmyuRGyh0XouQYb4YMJKvyOeW", "U*U"},
            new string[]{"$2a$05$CCCCCCCCCCCCCCCCCCCCC.VGOzA784oUp/Z0DY336zx7pLYAy0lwK", "U*U*"},
            new string[]{"$2a$05$XXXXXXXXXXXXXXXXXXXXXOAcXxm9kjPGEMsLznoKqmqw7tc8WCx4a", "U*U*U"},
            new string[]{"$2a$05$CCCCCCCCCCCCCCCCCCCCC.7uG0VCzI2bS7j6ymqJi9CdcdxiRTWNy", ""},
            new string[]{"$2a$05$abcdefghijklmnopqrstuu5s2v8.iXieOjg/.AySBTTZIIVFJeBui",
                "0123456789abcdefghijklmnopqrstuvwxyz"
                + "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
                + "chars after 72 are ignored"},
        };

        private static readonly string[] BCryptTest2 = { // from: http://openwall.info/wiki/john/sample-hashes
            "$2a$05$bvIG6Nmid91Mu9RcmmWZfO5HJIMCT8riNW0hEp8f6/FuA2/mHZFpe", "password"
        };

        private static readonly string[] BCryptTest2b = { // from: http://stackoverflow.com/questions/11654684/verifying-a-bcrypt-hash
            "$2a$10$.TtQJ4Jr6isd4Hp.mVfZeuh6Gws4rOQ/vdBczhDx.19NFK0Y84Dle", "ππππππππ"
        };

        private static readonly string[][] BCryptTest3 = // from: https://bitbucket.org/vadim/bcrypt.net/src/464c41416dc9/BCrypt.Net.Test/TestBCrypt.cs - plain - salt - expected
        {
            new string[]{"", "$2a$06$DCq7YPn5Rq63x1Lad4cll.", "$2a$06$DCq7YPn5Rq63x1Lad4cll.TV4S6ytwfsfvkgY8jIucDrjc8deX1s."},
            new string[]{"", "$2a$08$HqWuK6/Ng6sg9gQzbLrgb.", "$2a$08$HqWuK6/Ng6sg9gQzbLrgb.Tl.ZHfXLhvt/SgVyWhQqgqcZ7ZuUtye"},
            new string[]{"", "$2a$10$k1wbIrmNyFAPwPVPSVa/ze", "$2a$10$k1wbIrmNyFAPwPVPSVa/zecw2BCEnBwVS2GbrmgzxFUOqW9dk4TCW"},
            new string[]{"", "$2a$12$k42ZFHFWqBp3vWli.nIn8u", "$2a$12$k42ZFHFWqBp3vWli.nIn8uYyIkbvYRvodzbfbK18SSsY.CsIQPlxO"},
            new string[]{"a", "$2a$06$m0CrhHm10qJ3lXRY.5zDGO", "$2a$06$m0CrhHm10qJ3lXRY.5zDGO3rS2KdeeWLuGmsfGlMfOxih58VYVfxe"},
            new string[]{"a", "$2a$08$cfcvVd2aQ8CMvoMpP2EBfe", "$2a$08$cfcvVd2aQ8CMvoMpP2EBfeodLEkkFJ9umNEfPD18.hUF62qqlC/V."},
            new string[]{"a", "$2a$10$k87L/MF28Q673VKh8/cPi.", "$2a$10$k87L/MF28Q673VKh8/cPi.SUl7MU/rWuSiIDDFayrKk/1tBsSQu4u"},
            new string[]{"a", "$2a$12$8NJH3LsPrANStV6XtBakCe", "$2a$12$8NJH3LsPrANStV6XtBakCez0cKHXVxmvxIlcz785vxAIZrihHZpeS"},
            new string[]{"abc", "$2a$06$If6bvum7DFjUnE9p2uDeDu", "$2a$06$If6bvum7DFjUnE9p2uDeDu0YHzrHM6tf.iqN8.yx.jNN1ILEf7h0i"},
            new string[]{"abc", "$2a$08$Ro0CUfOqk6cXEKf3dyaM7O", "$2a$08$Ro0CUfOqk6cXEKf3dyaM7OhSCvnwM9s4wIX9JeLapehKK5YdLxKcm"},
            new string[]{"abc", "$2a$10$WvvTPHKwdBJ3uk0Z37EMR.", "$2a$10$WvvTPHKwdBJ3uk0Z37EMR.hLA2W6N9AEBhEgrAOljy2Ae5MtaSIUi"},
            new string[]{"abc", "$2a$12$EXRkfkdmXn2gzds2SSitu.", "$2a$12$EXRkfkdmXn2gzds2SSitu.MW9.gAVqa9eLS1//RYtYCmB1eLHg.9q"},
            new string[]{"abcdefghijklmnopqrstuvwxyz", "$2a$06$.rCVZVOThsIa97pEDOxvGu", "$2a$06$.rCVZVOThsIa97pEDOxvGuRRgzG64bvtJ0938xuqzv18d3ZpQhstC"},
            new string[]{"abcdefghijklmnopqrstuvwxyz", "$2a$08$aTsUwsyowQuzRrDqFflhge", "$2a$08$aTsUwsyowQuzRrDqFflhgekJ8d9/7Z3GV3UcgvzQW3J5zMyrTvlz."},
            new string[]{"abcdefghijklmnopqrstuvwxyz", "$2a$10$fVH8e28OQRj9tqiDXs1e1u", "$2a$10$fVH8e28OQRj9tqiDXs1e1uxpsjN0c7II7YPKXua2NAKYvM6iQk7dq"},
            new string[]{"abcdefghijklmnopqrstuvwxyz", "$2a$12$D4G5f18o7aMMfwasBL7Gpu", "$2a$12$D4G5f18o7aMMfwasBL7GpuQWuP3pkrZrOAnqP.bmezbMng.QwJ/pG"},
            new string[]{"~!@#$%^&*()      ~!@#$%^&*()PNBFRD", "$2a$06$fPIsBO8qRqkjj273rfaOI.", "$2a$06$fPIsBO8qRqkjj273rfaOI.HtSV9jLDpTbZn782DC6/t7qT67P6FfO"},
            new string[]{"~!@#$%^&*()      ~!@#$%^&*()PNBFRD", "$2a$08$Eq2r4G/76Wv39MzSX262hu", "$2a$08$Eq2r4G/76Wv39MzSX262huzPz612MZiYHVUJe/OcOql2jo4.9UxTW"},
            new string[]{"~!@#$%^&*()      ~!@#$%^&*()PNBFRD", "$2a$10$LgfYWkbzEvQ4JakH7rOvHe", "$2a$10$LgfYWkbzEvQ4JakH7rOvHe0y8pHKF9OaFgwUZ2q7W2FFZmZzJYlfS"},
            new string[]{"~!@#$%^&*()      ~!@#$%^&*()PNBFRD", "$2a$12$WApznUOJfkEGSmYRfnkrPO", "$2a$12$WApznUOJfkEGSmYRfnkrPOr466oFDCaj4b6HY3EXGvfxm43seyhgC"},
        };

        private static readonly string[][] BCryptTest4 = { // from: https://github.com/ChrisMcKee/cryptsharp/blob/master/Tests/vectors/BCrypt.txt
            new string[]{"n6HyjrYTo/r4lgjvM7L<`iM", "$2a$07$XPrYfnqc5ankSHnRfmPVu.A0trKq3VdczdbJjKaWIksKF.GfFCxv."},
            new string[]{"~s0quB/K8zRtRT:QtZr`s|^O", "$2a$07$5zzz8omiaStXwOetWwlmuePPRwUt0jhNBPYGGgAMcUDvqsGVqv9Cy"},
            new string[]{"r>8y3uE}6<7nI34?Q2rR0JEw", "$2a$07$k5AH9bO9aplPYdZMZ155qOcY1FewMXcupWewW6fViUtsVQ2Umg6LS"},
            new string[]{">l_7}xxH3|Cr{dCR[HTUN@k~", "$2a$05$24xz81ZZsMUMm940bbWMCeHsO.s6A3MG0JZzm4y3.Ti6P96bz6RN6"},
            new string[]{"D`lCFYTe9_8IW6nEB:oPjEk/S", "$2a$05$bA1xkp4NqFvDmtQJtDO9CugW0INxQLpMZha8AaHmBj9Zg9HlfQtBa"},
            new string[]{"UBGYU6|a|RpA:bp[;}p.ZY4f1", "$2a$08$gu4KBnkla.bEqHiwaJ8.z.0ixfzE1Q0/iPfmpfRmUA.NUhUdZboxa"},
            new string[]{"O9X[kP6{63F3rXKtN>n?zh2_", "$2a$04$yRZW9xEsqN9DL19jveqFyO1bljZ0r5KNCYqQzMqYpDB7XHWqDWNGC"},
            new string[]{":Sa:BknepsG}\\5dOj>kh0KAk", "$2a$04$KhDTFUlakUsPNuLQSgyr7.xQZxkTSIvo0nFw0XyjvrH6n5kZkYDLG"},// extra escape sequence added
            new string[]{"2_9J6k:{z?SSjCzL/GT/5CMgc", "$2a$05$eN1jCXDxN9HmuIARJlwH4ewsEyYbAmq7Cw99gEHqGRXtWyrRNLScy"},

            new string[]{"2KNy`Kodau14?s8XVru<IIw0eDw|.64MM^Wtv;3sfZt~3`2QN6/U]0^1HtETqWHt<lMfD-LX::zo7AcNLQ.Q.@.g5kX`j7hRi", "$2a$04$xUNE1aUuNlpNwSOuz1VpjuBgW95ImLccIquQxyGLeinucvokg2Ale"},
            new string[]{"0yWE>E;h/kdCRd@T]fQiv`Vz]KC0zaIAIeyY4zcooQ0^DfP{hHsw9?atO}CxbkbnK-LxUe;|FiBEluVqO@ysHhXQDdXPt0p", "$2a$07$pNHi/IxrSUohtsD5/eIv4O324ZPGfJE7mUAaNpIPkpyxjW9kqIk76"},
            new string[]{"ilWj~2mLBa1Pq`sxrW8fNNq:XF0@KP5RLW9u?[E_wwkROmCSWudYoS5I2HGI-1-?Pd0zVxTIeNbF;nLDUGtce{8dHmx90:;N<8", "$2a$07$ePVgkQl8QKSG2Xv6o0bnOe4SZp4ejag5CP44tjxfmY17F5VzRgwF6"},
            new string[]{"dj~OsXmQGj6FXnPGgwg9]G@75~L@G[|e<hgh2vaNqIyYZPh@M;I1DTgZS/~Q:i[6d]oei:hBw4}{}y7k9K^4SoN}wb8mrg[", "$2a$04$BZT7YoAYAgtNkD0/BOl.jOi0dDni7WtmB8.wAebHeHkOs.TpRgml."},
            new string[]{"7;PjW]RYJoZXf.r2M^Mm1jVIe0wJ=Kdd2iUBuu1v3HGI1-S[TB6yg{0~:nbpeA08dysS5d}@Oxbrpj[~i-60mpq1WZqQmSVpnR", "$2a$07$fa9NDzoPKiSWC67cP/tj2OqE0PqvGwzRoJiCKj.czyqKyvpdtVpKe"},
            new string[]{"8nv;PAN~-FQ]Emh@.TKG=^.t8R0EQC0T?x9|9g4xzxYmSbBO1qDx8kv-ehh0IBv>3KWhz.Z~jUF0tt8[5U@8;5:=[v6pf.IEJ", "$2a$08$eXo9KDc1BZyybBgMurpcD.GA1/ch3XhgBnIH10Xvjc2ogZaGg3t/m"},
        };


        // 2y vectors generated from htpasswd -nB -C 12, nb leading username was removed.
        private static readonly string[,] twoYVec = new string[,]{
            {"a", "$2y$12$DB3BUbYa/SsEL7kCOVji0OauTkPkB5Y1OeyfxJHM7jvMrbml5sgD2"},
            {"abc", "$2y$12$p.xODEbFcXUlHGbNxWZqAe6AA5FWupqXmN9tZea2ACDhwIx4EA2a6"},
            {"hello world", "$2y$12$wfkxITYXjNLVpEi9nOjz7uXMhCXKSTY7O2y7X4bwY89aGSvRziguq"},
            {"ABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXY", "$2y$12$QwAt5kuG68nW7v.87q0QPuwdki3romFc/RU/RV3Qqk4FPw6WdbQzu"}
        };

        // Same as 2y vectors only version changed to 2b to verify handling of that version.
        private static readonly string[,] twoBVec = new string[,]{
            {"a", "$2b$12$DB3BUbYa/SsEL7kCOVji0OauTkPkB5Y1OeyfxJHM7jvMrbml5sgD2"},
            {"abc", "$2b$12$p.xODEbFcXUlHGbNxWZqAe6AA5FWupqXmN9tZea2ACDhwIx4EA2a6"},
            {"hello world", "$2b$12$wfkxITYXjNLVpEi9nOjz7uXMhCXKSTY7O2y7X4bwY89aGSvRziguq"},
            {"ABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXYABCDEFGHIJKLMNOPQRSTUVWXY", "$2b$12$QwAt5kuG68nW7v.87q0QPuwdki3romFc/RU/RV3Qqk4FPw6WdbQzu"}
        };

        public static void Main(string[] args)
        {
            RunTest(new OpenBsdBCryptTest());
        }

        public override string Name
        {
            get { return "OpenBsdBCrypt"; }
        }

        public override void PerformTest()
        {
            string encoded, password;

            for (int i = 0; i < BCryptTest1.Length; i++)
            {
                string[] testString = BCryptTest1[i];
                encoded = testString[0];
                password = testString[1];
                if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
                {
                    Fail("test1 mismatch: " + "[" + i + "] " + password);
                }
            }

            encoded = BCryptTest2[0];
            password = BCryptTest2[1];
            if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
            {
                Fail("bcryptTest2 mismatch: " + password);
            }

            encoded = BCryptTest2b[0];
            password = BCryptTest2b[1];
            if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
            {
                Fail("bcryptTest2b mismatch: " + password);
            }

            for (int i = 0; i < BCryptTest3.Length; i++)
            {
                string[] testString = BCryptTest3[i];
                encoded = testString[2];
                password = testString[0];
                if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
                {
                    Fail("test3 mismatch: " + "[" + i + "] " + password);
                }
            }

            for (int i = 0; i < BCryptTest4.Length; i++)
            {
                string[] testString = BCryptTest4[i];
                encoded = testString[1];
                password = testString[0];
                if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
                {
                    Fail("test4 mismatch: " + "[" + i + "] " + password);
                }
            }

            {
                int lower = twoYVec.GetLowerBound(0);
                int upper = twoYVec.GetUpperBound(0);
                for (int i = lower; i <= upper; i++)
                {
                    password = twoYVec[i, 0];
                    encoded = twoYVec[i, 1];

                    if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
                    {
                        Fail("twoYVec mismatch: " + "[" + i + "] " + password);
                    }
                }
            }

            {
                int lower = twoBVec.GetLowerBound(0);
                int upper = twoBVec.GetUpperBound(0);
                for (int i = lower; i <= upper; i++)
                {
                    password = twoBVec[i, 0];
                    encoded = twoBVec[i, 1];

                    if (!OpenBsdBCrypt.CheckPassword(encoded, password.ToCharArray()))
                    {
                        Fail("twoBVec mismatch: " + "[" + i + "] " + password);
                    }
                }
            }
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
