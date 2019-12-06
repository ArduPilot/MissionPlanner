﻿using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpNoPrivateKeyTest
        : SimpleTest
    {
        private static string pgpOldPass = "test";
        private static string pgpNewPass = "newtest";

        private static readonly byte[] pgpPrivateEmpty = Base64.Decode(
              "lQCVBFGSNGwBBACwABZRIEW/4vDQajcO0FW39yNDcsHBDwPkGT95D7jiVTTRoSs6"
            + "ACWRAAwGlz4V62U0+nEgasxpifHnu6jati5zxwS16qNvBcxcqZrdZWdvolzCWWsr"
            + "pFd0juhwesrvvUb5dN/xCJKyLPkp6A+uwv35/cxVSOHFvbW7nnronwinYQARAQAB"
            + "/gJlAkdOVQG0HlRlc3QgVGVzdGVyc29uIDx0ZXN0QHRlc3QubmV0Poi4BBMBAgAi"
            + "BQJRkjRsAhsDBgsJCAcDAgYVCAIJCgsEFgIDAQIeAQIXgAAKCRDSr6Hh9tuk5NfI"
            + "A/4iMPF9k2/7KanWksNrBqhKemsyI7hLTxAwv+AA9B0rOO2QoJYe9OjuKn199fNO"
            + "JPsAgwy7okvDe3QAUz3WA9GlghM5STYvonFJtl7o4kyjcZ4HO2ZI5Bdc5O9i63QA"
            + "rNv40qVp++A3Mf+13z7cftKufj0vOfw6YeayLVXcV4h95J0B/gRRkjSNAQQA2l3d"
            + "ZnFFYXYDoNHz1cOX4787CbKdBIfiALFfdbyQ6TzYkCTJTnVCZlQs2aeyrcdTSZUx"
            + "N4y9bih4nfJ8uRKyQvLm6O0u6bG16kUDDnnwlsGn3uvTXfUwnSPq8pFY2acde6ZG"
            + "N25vezNK1R6C7kU3+puNHqBIRANfHTsBElaD2V0AEQEAAf4CAwIUI0+QlwBVFdNa"
            + "S/ppOwSht7Gr19AK4SHe92VWDKnCBPN2W3vhM4NcZSQCV2oiEMI0akLZ26jqCiRl"
            + "AvTjLSVDho1rUWbaSxFfKlDQNbxCJKlMQeVfbsWXJMeDkn1AhPru3PBLl6Y1jocd"
            + "vIVM7aQugNQlwEuFWgtZeODxcgBfX2lQeEMIv0AtWTAMt6MVT8AgnFqiqC4+14t0"
            + "j2CHP2hqCDr5zw9gerAYQ0F03OS34vDm4Y5DmQFjyB05QO2cIN4DZ9gJg8NAQT+P"
            + "+bwWR3/i9pTq3InNkoi2uT41OnHsYWgKoEQn62BDxjbvO359crUiq9VvS52v2UXh"
            + "b6Z+fF3PoXXsobS1QQwTPXAeA/mlAflTp+HrkckatY7DgWbON1SSn4Z1XcWPKBSY"
            + "epS5+90Tj3byZvN7Laj61ZlXVBvU3x7z6MaBZDf4479fklcUnJ13v+P6uGnTI4YE"
            + "Q5pPjHn1dDqD2Nl8ZW9ufK9pPYkBPQQYAQIACQUCUZI0jQIbAgCoCRDSr6Hh9tuk"
            + "5J0gBBkBAgAGBQJRkjSNAAoJEPIU7wJ5Ws2K0F0D/jHx4jrZq7SCv69/4hictjgz"
            + "nNNFSOm20/brHXMBdp6p9mBqt28WU8fgRkxS0mz+1i7VNTv6ZwUXawfTyOVCPR5B"
            + "QEC+FA+LvdX0UcJBJpa9tT4koz1JBxmppxxLYdS2A5sslPD5If8QHUaOMEX9O1I+"
            + "So3rEh3+DuhQj88FUuG8uJAD/3Xtpf/5nEpghLOZdQ/7QkLCoRZk7fwjChQNFSJU"
            + "5xiZbZ/GsSvU1IqAP/NZBmBO0qDm5m7ahXy71O1bMFtaiUaw2Mb7dwqqDvppbjIB"
            + "OHdIhSnAorRLcnjm8z51QVMzHmgvKt5/e1q1fzsVzza6DWtYr2X/1VsuouSC1uz1"
            + "nPdgnQH+BFGSNJ4BBAC3KliQlchs0rctsXbhA/GEfiO0s9tAgVsfJL1PWUkC+26M"
            + "yBbqkVg5RV+J6dyTSeT6cDI8PMu8XFPO6H2WWdovfs7X9K1lxfnNWxQB2L6t2xre"
            + "XyFqvTsYEFuGvYmbNyUYvA+daHD0xqX8UrC0J6TYg5ie5I685X8gFKVEtGYG/wAR"
            + "AQAB/gIDAuMt34hcdJPX03uBj9LtjcnrMNLyF7PVJv4wBXEt7T9Kp8cYZ80Sxpd2"
            + "11LHzjgiPg1kkkImJ9Ie1qbPZjc9tyiGf47m0TIORnKtwNb2YN+sKLpqZ+ienfTs"
            + "vc0uyuVGW+8PCt409M9R++0q66sxvb3oKBp2zsr3BbGaISs4OVxY2L8uU3t5j9pi"
            + "qKdV2XTiV9OZJ+2f1au1tMwhNPzjVJ4GH53TxewSkshRJTZtw2ouUJkdA/bizfNO"
            + "9XYYvV8sW1/ASe1dnOs+ANDGzumzSA00dWPSveURroG+ZtVXVgkakJJtDwdAYutP"
            + "kSm28cnsl1OmrBKPonB5N3uDjTlq56vji1d2F5ugAXTTD5PptiML1wEB/TqsRJRX"
            + "uY7DLy+8iukOVOyoVw63UMX27YUz61JJZYcB7U28gNeRyBsnTEbjmvteoFsYnaGg"
            + "Owgc+1Zx4rQdZEqxZRmfwmiUgHGyI9OpvoVaTIuDIqDd2ZRWiJ8EGAECAAkFAlGS"
            + "NJ4CGwwACgkQ0q+h4fbbpOScsgQAmMymSfAmltnHQzKr5k2GvlAqIzl9MqKVm9wA"
            + "0Cx3grwzPaiqmfspPIueQ8Phexiy6dwfPrwNoKnJOEjM6/sOcWEmLiIoYi+/oQjU"
            + "12zwogOfzT/1hPpG5zs+GBGX4sorCK663PuovwCEoNrWm+7nItfTwdnFavNuj7s4"
            + "+b3JLdM=");

        private static readonly byte[] pgpPrivateFull = Base64.Decode(
              "lQH+BFGSNGwBBACwABZRIEW/4vDQajcO0FW39yNDcsHBDwPkGT95D7jiVTTRoSs6"
            + "ACWRAAwGlz4V62U0+nEgasxpifHnu6jati5zxwS16qNvBcxcqZrdZWdvolzCWWsr"
            + "pFd0juhwesrvvUb5dN/xCJKyLPkp6A+uwv35/cxVSOHFvbW7nnronwinYQARAQAB"
            + "/gIDAuqTuDp/Chfq0TKnSxmm2ZpDuiHD+NFVnCyNuJpvCQk0PnVwmGMH4xvsAZB2"
            + "TOrfh2XHf/n9J4vjxB6p6Zs1kGBgg9hcHoWf+oEf1Tz/PE/c1tUXG2Hz9wlAgstU"
            + "my2NpDTYUjQs45p+LaM+WFtLNXzBeqELKlMevs8Xb7n+VHwiTuM3KfXETLCoLz0Q"
            + "3GmmpOuNnvXBdza7RsDwke0r66HzwX4Le8cMH9Pe7kSMakx9S1UR/uIsxsZYZOKb"
            + "BieGEumxiAnew0Ri5/8wTd5yYC7BWbYvBUgdMQ1gzkzmJcVky8NVfoZKQ0GkdvMo"
            + "fMThIVXN1U6+aqzAuUMFCPYQ7fEpfoNLhCnzQPv3RE7Wo2vFMjWBod2J4MSLhBuq"
            + "Ut+FYLqYqU21Qe4PEyPmGnkVu7Wd8FGjBF+IKZg+ycPi++h/twloD/h7LEaq907C"
            + "4R3rdOzjZnefDfxVWjLLhqKSSuXxtjSSKwMNdbjYVVJ/tB5UZXN0IFRlc3RlcnNv"
            + "biA8dGVzdEB0ZXN0Lm5ldD6IuAQTAQIAIgUCUZI0bAIbAwYLCQgHAwIGFQgCCQoL"
            + "BBYCAwECHgECF4AACgkQ0q+h4fbbpOTXyAP+IjDxfZNv+ymp1pLDawaoSnprMiO4"
            + "S08QML/gAPQdKzjtkKCWHvTo7ip9ffXzTiT7AIMMu6JLw3t0AFM91gPRpYITOUk2"
            + "L6JxSbZe6OJMo3GeBztmSOQXXOTvYut0AKzb+NKlafvgNzH/td8+3H7Srn49Lzn8"
            + "OmHmsi1V3FeIfeSdAf4EUZI0jQEEANpd3WZxRWF2A6DR89XDl+O/OwmynQSH4gCx"
            + "X3W8kOk82JAkyU51QmZULNmnsq3HU0mVMTeMvW4oeJ3yfLkSskLy5ujtLumxtepF"
            + "Aw558JbBp97r0131MJ0j6vKRWNmnHXumRjdub3szStUegu5FN/qbjR6gSEQDXx07"
            + "ARJWg9ldABEBAAH+AgMCFCNPkJcAVRXTWkv6aTsEobexq9fQCuEh3vdlVgypwgTz"
            + "dlt74TODXGUkAldqIhDCNGpC2duo6gokZQL04y0lQ4aNa1Fm2ksRXypQ0DW8QiSp"
            + "TEHlX27FlyTHg5J9QIT67tzwS5emNY6HHbyFTO2kLoDUJcBLhVoLWXjg8XIAX19p"
            + "UHhDCL9ALVkwDLejFU/AIJxaoqguPteLdI9ghz9oagg6+c8PYHqwGENBdNzkt+Lw"
            + "5uGOQ5kBY8gdOUDtnCDeA2fYCYPDQEE/j/m8Fkd/4vaU6tyJzZKItrk+NTpx7GFo"
            + "CqBEJ+tgQ8Y27zt+fXK1IqvVb0udr9lF4W+mfnxdz6F17KG0tUEMEz1wHgP5pQH5"
            + "U6fh65HJGrWOw4FmzjdUkp+GdV3FjygUmHqUufvdE4928mbzey2o+tWZV1Qb1N8e"
            + "8+jGgWQ3+OO/X5JXFJydd7/j+rhp0yOGBEOaT4x59XQ6g9jZfGVvbnyvaT2JAT0E"
            + "GAECAAkFAlGSNI0CGwIAqAkQ0q+h4fbbpOSdIAQZAQIABgUCUZI0jQAKCRDyFO8C"
            + "eVrNitBdA/4x8eI62au0gr+vf+IYnLY4M5zTRUjpttP26x1zAXaeqfZgardvFlPH"
            + "4EZMUtJs/tYu1TU7+mcFF2sH08jlQj0eQUBAvhQPi73V9FHCQSaWvbU+JKM9SQcZ"
            + "qaccS2HUtgObLJTw+SH/EB1GjjBF/TtSPkqN6xId/g7oUI/PBVLhvLiQA/917aX/"
            + "+ZxKYISzmXUP+0JCwqEWZO38IwoUDRUiVOcYmW2fxrEr1NSKgD/zWQZgTtKg5uZu"
            + "2oV8u9TtWzBbWolGsNjG+3cKqg76aW4yATh3SIUpwKK0S3J45vM+dUFTMx5oLyre"
            + "f3tatX87Fc82ug1rWK9l/9VbLqLkgtbs9Zz3YJ0B/gRRkjSeAQQAtypYkJXIbNK3"
            + "LbF24QPxhH4jtLPbQIFbHyS9T1lJAvtujMgW6pFYOUVfienck0nk+nAyPDzLvFxT"
            + "zuh9llnaL37O1/StZcX5zVsUAdi+rdsa3l8har07GBBbhr2JmzclGLwPnWhw9Mal"
            + "/FKwtCek2IOYnuSOvOV/IBSlRLRmBv8AEQEAAf4CAwLjLd+IXHST19N7gY/S7Y3J"
            + "6zDS8hez1Sb+MAVxLe0/SqfHGGfNEsaXdtdSx844Ij4NZJJCJifSHtamz2Y3Pbco"
            + "hn+O5tEyDkZyrcDW9mDfrCi6amfonp307L3NLsrlRlvvDwreNPTPUfvtKuurMb29"
            + "6Cgads7K9wWxmiErODlcWNi/LlN7eY/aYqinVdl04lfTmSftn9WrtbTMITT841Se"
            + "Bh+d08XsEpLIUSU2bcNqLlCZHQP24s3zTvV2GL1fLFtfwEntXZzrPgDQxs7ps0gN"
            + "NHVj0r3lEa6BvmbVV1YJGpCSbQ8HQGLrT5EptvHJ7JdTpqwSj6JweTd7g405auer"
            + "44tXdheboAF00w+T6bYjC9cBAf06rESUV7mOwy8vvIrpDlTsqFcOt1DF9u2FM+tS"
            + "SWWHAe1NvIDXkcgbJ0xG45r7XqBbGJ2hoDsIHPtWceK0HWRKsWUZn8JolIBxsiPT"
            + "qb6FWkyLgyKg3dmUVoifBBgBAgAJBQJRkjSeAhsMAAoJENKvoeH226TknLIEAJjM"
            + "pknwJpbZx0Myq+ZNhr5QKiM5fTKilZvcANAsd4K8Mz2oqpn7KTyLnkPD4XsYsunc"
            + "Hz68DaCpyThIzOv7DnFhJi4iKGIvv6EI1Nds8KIDn80/9YT6Ruc7PhgRl+LKKwiu"
            + "utz7qL8AhKDa1pvu5yLX08HZxWrzbo+7OPm9yS3T");

        public override void PerformTest()
        {
            PgpSecretKeyRing pgpSecRing = new PgpSecretKeyRing(pgpPrivateFull);
            PgpSecretKey pgpSecKey = pgpSecRing.GetSecretKey();
            bool isFullEmpty = pgpSecKey.IsPrivateKeyEmpty;

            pgpSecRing = new PgpSecretKeyRing(pgpPrivateEmpty);
            pgpSecKey = pgpSecRing.GetSecretKey();
            bool isEmptyEmpty = pgpSecKey.IsPrivateKeyEmpty;

            //
            // Check isPrivateKeyEmpty() is public
            //

            if (isFullEmpty || !isEmptyEmpty)
            {
                Fail("Empty private keys not detected correctly.");
            }

            //
            // Check copyWithNewPassword doesn't throw an exception for secret
            // keys without private keys (PGPException: unknown S2K type: 101).
            //

            SecureRandom rand = new SecureRandom();

            try
            {
                PgpSecretKey pgpChangedKey = PgpSecretKey.CopyWithNewPassword(pgpSecKey,
                    pgpOldPass.ToCharArray(), pgpNewPass.ToCharArray(), pgpSecKey.KeyEncryptionAlgorithm, rand);
            }
            catch (PgpException e)
            {
                if (!e.Message.Equals("no private key in this SecretKey - public key present only."))
                {
                    Fail("wrong exception.");
                }
            }
        }

        public override string Name
        {
            get { return "PGPNoPrivateKeyTest"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new PgpNoPrivateKeyTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
