using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Bcpg.Attr;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpRsaTest
        : SimpleTest
    {
        private static readonly byte[] testPubKey = Base64.Decode(
              "mIsEPz2nJAEEAOTVqWMvqYE693qTgzKv/TJpIj3hI8LlYPC6m1dk0z3bDLwVVk9F"
            + "FAB+CWS8RdFOWt/FG3tEv2nzcoNdRvjv9WALyIGNawtae4Ml6oAT06/511yUzXHO"
            + "k+9xK3wkXN5jdzUhf4cA2oGpLSV/pZlocsIDL+jCUQtumUPwFodmSHhzAAYptC9F"
            + "cmljIEVjaGlkbmEgKHRlc3Qga2V5KSA8ZXJpY0Bib3VuY3ljYXN0bGUub3JnPoi4"
            + "BBMBAgAiBQI/PackAhsDBQkAg9YABAsHAwIDFQIDAxYCAQIeAQIXgAAKCRA1WGFG"
            + "/fPzc8WMA/9BbjuB8E48QAlxoiVf9U8SfNelrz/ONJA/bMvWr/JnOGA9PPmFD5Uc"
            + "+kV/q+i94dEMjsC5CQ1moUHWSP2xlQhbOzBP2+oPXw3z2fBs9XJgnTH6QWMAAvLs"
            + "3ug9po0loNHLobT/D/XdXvcrb3wvwvPT2FptZqrtonH/OdzT9JdfrA==");

        private static readonly byte[] testPrivKey = Base64.Decode(
              "lQH8BD89pyQBBADk1aljL6mBOvd6k4Myr/0yaSI94SPC5WDwuptXZNM92wy8FVZP"
            + "RRQAfglkvEXRTlrfxRt7RL9p83KDXUb47/VgC8iBjWsLWnuDJeqAE9Ov+ddclM1x"
            + "zpPvcSt8JFzeY3c1IX+HANqBqS0lf6WZaHLCAy/owlELbplD8BaHZkh4cwAGKf4D"
            + "AwKbLeIOVYTEdWD5v/YgW8ERs0pDsSIfBTvsJp2qA798KeFuED6jGsHUzdi1M990"
            + "6PRtplQgnoYmYQrzEc6DXAiAtBR4Kuxi4XHx0ZR2wpVlVxm2Ypgz7pbBNWcWqzvw"
            + "33inl7tR4IDsRdJOY8cFlN+1tSCf16sDidtKXUVjRjZNYJytH18VfSPlGXMeYgtw"
            + "3cSGNTERwKaq5E/SozT2MKTiORO0g0Mtyz+9MEB6XVXFavMun/mXURqbZN/k9BFb"
            + "z+TadpkihrLD1xw3Hp+tpe4CwPQ2GdWKI9KNo5gEnbkJgLrSMGgWalPhknlNHRyY"
            + "bSq6lbIMJEE3LoOwvYWwweR1+GrV9farJESdunl1mDr5/d6rKru+FFDwZM3na1IF"
            + "4Ei4FpqhivZ4zG6pN5XqLy+AK85EiW4XH0yAKX1O4YlbmDU4BjxhiwTdwuVMCjLO"
            + "5++jkz5BBQWdFX8CCMA4FJl36G70IbGzuFfOj07ly7QvRXJpYyBFY2hpZG5hICh0"
            + "ZXN0IGtleSkgPGVyaWNAYm91bmN5Y2FzdGxlLm9yZz6IuAQTAQIAIgUCPz2nJAIb"
            + "AwUJAIPWAAQLBwMCAxUCAwMWAgECHgECF4AACgkQNVhhRv3z83PFjAP/QW47gfBO"
            + "PEAJcaIlX/VPEnzXpa8/zjSQP2zL1q/yZzhgPTz5hQ+VHPpFf6voveHRDI7AuQkN"
            + "ZqFB1kj9sZUIWzswT9vqD18N89nwbPVyYJ0x+kFjAALy7N7oPaaNJaDRy6G0/w/1"
            + "3V73K298L8Lz09habWaq7aJx/znc0/SXX6w=");

        private static readonly byte[] testPubKeyV3 = Base64.Decode(
              "mQCNAz+zvlEAAAEEAMS22jgXbOZ/D3xWgM2kauSdzrwlU7Ms5hDW05ObqQyO"
            + "FfQoKKMhfupyoa7J3x04VVBKu6Eomvr1es+VImH0esoeWFFahNOYq/I+jRRB"
            + "woOhAGZ5UB2/hRd7rFmxqp6sCXi8wmLO2tAorlTzAiNNvl7xF4cQZpc0z56F"
            + "wdi2fBUJAAURtApGSVhDSVRZX1FBiQCVAwUQP7O+UZ6Fwdi2fBUJAQFMwwQA"
            + "qRnFsdg4xQnB8Y5d4cOpXkIn9AZgYS3cxtuSJB84vG2CgC39nfv4c+nlLkWP"
            + "4puG+mZuJNgVoE84cuAF4I//1anKjlU7q1M6rFQnt5S4uxPyG3dFXmgyU1b4"
            + "PBOnA0tIxjPzlIhJAMsPCGGA5+5M2JP0ad6RnzqzE3EENMX+GqY=");

        private static readonly byte[] testPrivKeyV3 = Base64.Decode(
              "lQHfAz+zvlEAAAEEAMS22jgXbOZ/D3xWgM2kauSdzrwlU7Ms5hDW05ObqQyO"
            + "FfQoKKMhfupyoa7J3x04VVBKu6Eomvr1es+VImH0esoeWFFahNOYq/I+jRRB"
            + "woOhAGZ5UB2/hRd7rFmxqp6sCXi8wmLO2tAorlTzAiNNvl7xF4cQZpc0z56F"
            + "wdi2fBUJAAURAXWwRBZQHNikA/f0ScLLjrXi4s0hgQecg+dkpDow94eu5+AR"
            + "0DzZnfurpgfUJCNiDi5W/5c3Zj/xyrfMAgkbCgJ1m6FZqAQh7Mq73l7Kfu4/"
            + "XIkyDF3tDgRuZNezB+JuElX10tV03xumHepp6M6CfhXqNJ15F33F99TA5hXY"
            + "CPYD7SiSOpIhQkCOAgDAA63imxbpuKE2W7Y4I1BUHB7WQi8ZdkZd04njNTv+"
            + "rFUuOPapQVfbWG0Vq8ld3YmJB4QWsa2mmqn+qToXbwufAgBpXkjvqK5yPiHF"
            + "Px2QbFc1VqoCJB6PO5JRIqEiUZBFGdDlLxt3VSyqz7IZ/zEnxZq+tPCGGGSm"
            + "/sAGiMvENcHVAfy0kTXU42TxEAYJyyNyqjXOobDJpEV1mKhFskRXt7tbMfOS"
            + "Yf91oX8f6xw6O2Nal+hU8dS0Bmfmk5/enHmvRLHQocO0CkZJWENJVFlfUUE=");

        private static readonly byte[] sig1 = Base64.Decode(
              "owGbwMvMwMRoGpHo9vfz52LGNTJJnBmpOTn5eiUVJfb23JvAHIXy/KKcFEWuToap"
            + "zKwMIGG4Bqav0SwMy3yParsEKi2LMGI9xhh65sBxb05n5++ZLcWNJ/eLFKdWbm95"
            + "tHbDV7GMwj/tUctUpFUXWPYFCLdNsDiVNuXbQvZtdXV/5xzY+9w1nCnijH9JoNiJ"
            + "22n2jo0zo30/TZLo+jDl2vTzIvPeLEsPM3ZUE/1Ytqs4SG2TxIQbH7xf3uzcYXq2"
            + "5Fw9AA==");

//		private static readonly byte[] sig1crc = Base64.Decode("+3i0");

        private static readonly byte[] subKey = Base64.Decode(
              "lQH8BD89pyQBBADk1aljL6mBOvd6k4Myr/0yaSI94SPC5WDwuptXZNM92wy8FVZP"
            + "RRQAfglkvEXRTlrfxRt7RL9p83KDXUb47/VgC8iBjWsLWnuDJeqAE9Ov+ddclM1x"
            + "zpPvcSt8JFzeY3c1IX+HANqBqS0lf6WZaHLCAy/owlELbplD8BaHZkh4cwAGKf4D"
            + "AwKt6ZC7iqsQHGDNn2ZAuhS+ZwiFC+BToW9Vq6rwggWjgM/SThv55rfDk7keiXUT"
            + "MyUcZVeYBe4Jttb4fAAm83hNztFu6Jvm9ITcm7YvnasBtVQjppaB+oYZgsTtwK99"
            + "LGC3mdexnriCLxPN6tDFkGhzdOcYZfK6py4Ska8Dmq9nOZU9Qtv7Pm3qa5tuBvYw"
            + "myTxeaJYifZTu/sky3Gj+REb8WonbgAJX/sLNBPUt+vYko+lxU8uqZpVEMU//hGG"
            + "Rns2gIHdbSbIe1vGgIRUEd7Z0b7jfVQLUwqHDyfh5DGvAUhvtJogjUyFIXZzpU+E"
            + "9ES9t7LZKdwNZSIdNUjM2eaf4g8BpuQobBVkj/GUcotKyeBjwvKxHlRefL4CCw28"
            + "DO3SnLRKxd7uBSqeOGUKxqasgdekM/xIFOrJ85k7p89n6ncLQLHCPGVkzmVeRZro"
            + "/T7zE91J57qBGZOUAP1vllcYLty1cs9PCc5oWnj3XbQvRXJpYyBFY2hpZG5hICh0"
            + "ZXN0IGtleSkgPGVyaWNAYm91bmN5Y2FzdGxlLm9yZz6IuAQTAQIAIgUCPz2nJAIb"
            + "AwUJAIPWAAQLBwMCAxUCAwMWAgECHgECF4AACgkQNVhhRv3z83PFjAP/QW47gfBO"
            + "PEAJcaIlX/VPEnzXpa8/zjSQP2zL1q/yZzhgPTz5hQ+VHPpFf6voveHRDI7AuQkN"
            + "ZqFB1kj9sZUIWzswT9vqD18N89nwbPVyYJ0x+kFjAALy7N7oPaaNJaDRy6G0/w/1"
            + "3V73K298L8Lz09habWaq7aJx/znc0/SXX6y0JEVyaWMgRWNoaWRuYSA8ZXJpY0Bi"
            + "b3VuY3ljYXN0bGUub3JnPoi4BBMBAgAiBQI/RxQNAhsDBQkAg9YABAsHAwIDFQID"
            + "AxYCAQIeAQIXgAAKCRA1WGFG/fPzc3O6A/49tXFCiiP8vg77OXvnmbnzPBA1G6jC"
            + "RZNP1yIXusOjpHqyLN5K9hw6lq/o4pNiCuiq32osqGRX3lv/nDduJU1kn2Ow+I2V"
            + "ci+ojMXdCGdEqPwZfv47jHLwRrIUJ22OOoWsORtgvSeRUd4Izg8jruaFM7ufr5hr"
            + "jEl1cuLW1Hr8Lp0B/AQ/RxxQAQQA0J2BIdqb8JtDGKjvYxrju0urJVVzyI1CnCjA"
            + "p7CtLoHQJUQU7PajnV4Jd12ukfcoK7MRraYydQEjxh2MqPpuQgJS3dgQVrxOParD"
            + "QYBFrZNd2tZxOjYakhErvUmRo6yWFaxChwqMgl8XWugBNg1Dva+/YcoGQ+ly+Jg4"
            + "RWZoH88ABin+AwMCldD/2v8TyT1ghK70IuFs4MZBhdm6VgyGR8DQ/Ago6IAjA4BY"
            + "Sol3lJb7+IIGsZaXwEuMRUvn6dWfa3r2I0p1t75vZb1Ng1YK32RZ5DNzl4Xb3L8V"
            + "D+1Fiz9mHO8wiplAwDudB+RmQMlth3DNi/UsjeCTdEJAT+TTC7D40DiHDb1bR86Y"
            + "2O5Y7MQ3SZs3/x0D/Ob6PStjfQ1kiqbruAMROKoavG0zVgxvspkoKN7h7BapnwJM"
            + "6yf4qN/aByhAx9sFvADxu6z3SVcxiFw3IgAmabyWYb85LP8AsTYAG/HBoC6yob47"
            + "Mt+GEDeyPifzzGXBWYIH4heZbSQivvA0eRwY5VZsMsBkbY5VR0FLVWgplbuO21bS"
            + "rPS1T0crC+Zfj7FQBAkTfsg8RZQ8MPaHng01+gnFd243DDFvTAHygvm6a2X2fiRw"
            + "5epAST4wWfY/BZNOxmfSKH6QS0oQMRscw79He6vGTB7vunLrKQYD4veInwQYAQIA"
            + "CQUCP0ccUAIbDAAKCRA1WGFG/fPzczmFA/wMg5HhN5NkqmjnHUFfeXNXdHzmekyw"
            + "38RnuCMKmfc43AiDs+FtJ62gpQ6PEsZF4o9S5fxcjVk3VSg00XMDtQ/0BsKBc5Gx"
            + "hJTq7G+/SoeM433WG19uoS0+5Lf/31wNoTnpv6npOaYpcTQ7L9LCnzwAF4H0hJPE"
            + "6bhmW2CMcsE/IZUB4QQ/Rwc1EQQAs5MUQlRiYOfi3fQ1OF6Z3eCwioDKu2DmOxot"
            + "BICvdoG2muvs0KEBas9bbd0FJqc92FZJv8yxEgQbQtQAiFxoIFHRTFK+SPO/tQm+"
            + "r83nwLRrfDeVVdRfzF79YCc+Abuh8sS/53H3u9Y7DYWr9IuMgI39nrVhY+d8yukf"
            + "jo4OR+sAoKS/f7V1Xxj/Eqhb8qzf+N+zJRUlBACDd1eo/zFJZcq2YJa7a9vkViME"
            + "axvwApqxeoU7oDpeHEMWg2DXJ7V24ZU5SbPTMY0x98cc8pcoqwsqux8xicWc0reh"
            + "U3odQxWM4Se0LmEdca0nQOmNJlL9IsQ+QOJzx47qUOUAqhxnkXxQ/6B8w+M6gZya"
            + "fwSdy70OumxESZipeQP+Lo9x6FcaW9L78hDX0aijJhgSEsnGODKB+bln29txX37E"
            + "/a/Si+pyeLMi82kUdIL3G3I5HPWd3qSO4K94062+HfFj8bA20/1tbb/WxvxB2sKJ"
            + "i3IobblFOvFHo+v8GaLdVyartp0JZLue/jP1dl9ctulSrIqaJT342uLsgTjsr2z+"
            + "AwMCAyAU8Vo5AhhgFkDto8vQk7yxyRKEzu5qB66dRcTlaUPIiR8kamcy5ZTtujs4"
            + "KIW4j2M/LvagrpWfV5+0M0VyaWMgRWNoaWRuYSAoRFNBIFRlc3QgS2V5KSA8ZXJp"
            + "Y0Bib3VuY3ljYXN0bGUub3JnPohZBBMRAgAZBQI/Rwc1BAsHAwIDFQIDAxYCAQIe"
            + "AQIXgAAKCRDNI/XpxMo0QwJcAJ40447eezSiIMspuzkwsMyFN8YBaQCdFTuZuT30"
            + "CphiUYWnsC0mQ+J15B4=");

        private static readonly byte[] enc1 = Base64.Decode(
              "hIwDKwfQexPJboABA/4/7prhYYMORTiQ5avQKx0XYpCLujzGefYjnyuWZnx3Iev8"
            + "Pmsguumm+OLLvtXhhkXQmkJRXbIg6Otj2ubPYWflRPgpJSgOrNOreOl5jeABOrtw"
            + "bV6TJb9OTtZuB7cTQSCq2gmYiSZkluIiDjNs3R3mEanILbYzOQ3zKSggKpzlv9JQ"
            + "AZUqTyDyJ6/OUbJF5fI5uiv76DCsw1zyMWotUIu5/X01q+AVP5Ly3STzI7xkWg/J"
            + "APz4zUHism7kSYz2viAQaJx9/bNnH3AM6qm1Fuyikl4=");

//		private static readonly byte[] enc1crc = Base64.Decode("lv4o");

//        private static readonly byte[] enc2 = Base64.Decode(
//			  "hIwDKwfQexPJboABBAC62jcJH8xKnKb1neDVmiovYON04+7VQ2v4BmeHwJrdag1g"
//			+ "Ya++6PeBlQ2Q9lSGBwLobVuJmQ7cOnPUJP727JeSGWlMyFtMbBSHekOaTenT5lj7"
//			+ "Zk7oRHxMp/hByzlMacIDzOn8LPSh515RHM57eDLCOwqnAxGQwk67GRl8f5dFH9JQ"
//			+ "Aa7xx8rjCqPbiIQW6t5LqCNvPZOiSCmftll6+se1XJhFEuq8WS4nXtPfTiJ3vib4"
//			+ "3soJdHzGB6AOs+BQ6aKmmNTVAxa5owhtSt1Z/6dfSSk=");

        private static readonly byte[] subPubKey = Base64.Decode(
              "mIsEPz2nJAEEAOTVqWMvqYE693qTgzKv/TJpIj3hI8LlYPC6m1dk0z3bDLwVVk9F"
            + "FAB+CWS8RdFOWt/FG3tEv2nzcoNdRvjv9WALyIGNawtae4Ml6oAT06/511yUzXHO"
            + "k+9xK3wkXN5jdzUhf4cA2oGpLSV/pZlocsIDL+jCUQtumUPwFodmSHhzAAYptC9F"
            + "cmljIEVjaGlkbmEgKHRlc3Qga2V5KSA8ZXJpY0Bib3VuY3ljYXN0bGUub3JnPoi4"
            + "BBMBAgAiBQI/PackAhsDBQkAg9YABAsHAwIDFQIDAxYCAQIeAQIXgAAKCRA1WGFG"
            + "/fPzc8WMA/9BbjuB8E48QAlxoiVf9U8SfNelrz/ONJA/bMvWr/JnOGA9PPmFD5Uc"
            + "+kV/q+i94dEMjsC5CQ1moUHWSP2xlQhbOzBP2+oPXw3z2fBs9XJgnTH6QWMAAvLs"
            + "3ug9po0loNHLobT/D/XdXvcrb3wvwvPT2FptZqrtonH/OdzT9JdfrIhMBBARAgAM"
            + "BQI/RxooBYMAemL8AAoJEM0j9enEyjRDiBgAn3RcLK+gq90PvnQFTw2DNqdq7KA0"
            + "AKCS0EEIXCzbV1tfTdCUJ3hVh3btF7QkRXJpYyBFY2hpZG5hIDxlcmljQGJvdW5j"
            + "eWNhc3RsZS5vcmc+iLgEEwECACIFAj9HFA0CGwMFCQCD1gAECwcDAgMVAgMDFgIB"
            + "Ah4BAheAAAoJEDVYYUb98/Nzc7oD/j21cUKKI/y+Dvs5e+eZufM8EDUbqMJFk0/X"
            + "Ihe6w6OkerIs3kr2HDqWr+jik2IK6KrfaiyoZFfeW/+cN24lTWSfY7D4jZVyL6iM"
            + "xd0IZ0So/Bl+/juMcvBGshQnbY46haw5G2C9J5FR3gjODyOu5oUzu5+vmGuMSXVy"
            + "4tbUevwuiEwEEBECAAwFAj9HGigFgwB6YvwACgkQzSP16cTKNEPwBQCdHm0Amwza"
            + "NmVmDHm3rmqI7rp2oQ0An2YbiP/H/kmBNnmTeH55kd253QOhuIsEP0ccUAEEANCd"
            + "gSHam/CbQxio72Ma47tLqyVVc8iNQpwowKewrS6B0CVEFOz2o51eCXddrpH3KCuz"
            + "Ea2mMnUBI8YdjKj6bkICUt3YEFa8Tj2qw0GARa2TXdrWcTo2GpIRK71JkaOslhWs"
            + "QocKjIJfF1roATYNQ72vv2HKBkPpcviYOEVmaB/PAAYpiJ8EGAECAAkFAj9HHFAC"
            + "GwwACgkQNVhhRv3z83M5hQP8DIOR4TeTZKpo5x1BX3lzV3R85npMsN/EZ7gjCpn3"
            + "ONwIg7PhbSetoKUOjxLGReKPUuX8XI1ZN1UoNNFzA7UP9AbCgXORsYSU6uxvv0qH"
            + "jON91htfbqEtPuS3/99cDaE56b+p6TmmKXE0Oy/Swp88ABeB9ISTxOm4ZltgjHLB"
            + "PyGZAaIEP0cHNREEALOTFEJUYmDn4t30NThemd3gsIqAyrtg5jsaLQSAr3aBtprr"
            + "7NChAWrPW23dBSanPdhWSb/MsRIEG0LUAIhcaCBR0UxSvkjzv7UJvq/N58C0a3w3"
            + "lVXUX8xe/WAnPgG7ofLEv+dx97vWOw2Fq/SLjICN/Z61YWPnfMrpH46ODkfrAKCk"
            + "v3+1dV8Y/xKoW/Ks3/jfsyUVJQQAg3dXqP8xSWXKtmCWu2vb5FYjBGsb8AKasXqF"
            + "O6A6XhxDFoNg1ye1duGVOUmz0zGNMffHHPKXKKsLKrsfMYnFnNK3oVN6HUMVjOEn"
            + "tC5hHXGtJ0DpjSZS/SLEPkDic8eO6lDlAKocZ5F8UP+gfMPjOoGcmn8Encu9Drps"
            + "REmYqXkD/i6PcehXGlvS+/IQ19GooyYYEhLJxjgygfm5Z9vbcV9+xP2v0ovqcniz"
            + "IvNpFHSC9xtyORz1nd6kjuCveNOtvh3xY/GwNtP9bW2/1sb8QdrCiYtyKG25RTrx"
            + "R6Pr/Bmi3Vcmq7adCWS7nv4z9XZfXLbpUqyKmiU9+Nri7IE47K9stDNFcmljIEVj"
            + "aGlkbmEgKERTQSBUZXN0IEtleSkgPGVyaWNAYm91bmN5Y2FzdGxlLm9yZz6IWQQT"
            + "EQIAGQUCP0cHNQQLBwMCAxUCAwMWAgECHgECF4AACgkQzSP16cTKNEMCXACfauui"
            + "bSwyG59Yrm8hHCDuCPmqwsQAni+dPl08FVuWh+wb6kOgJV4lcYae");

//		private static readonly byte[] subPubCrc = Base64.Decode("rikt");

        private static readonly byte[] pgp8Key = Base64.Decode(
              "lQIEBEBXUNMBBADScQczBibewnbCzCswc/9ut8R0fwlltBRxMW0NMdKJY2LF"
            + "7k2COeLOCIU95loJGV6ulbpDCXEO2Jyq8/qGw1qD3SCZNXxKs3GS8Iyh9Uwd"
            + "VL07nMMYl5NiQRsFB7wOb86+94tYWgvikVA5BRP5y3+O3GItnXnpWSJyREUy"
            + "6WI2QQAGKf4JAwIVmnRs4jtTX2DD05zy2mepEQ8bsqVAKIx7lEwvMVNcvg4Y"
            + "8vFLh9Mf/uNciwL4Se/ehfKQ/AT0JmBZduYMqRU2zhiBmxj4cXUQ0s36ysj7"
            + "fyDngGocDnM3cwPxaTF1ZRBQHSLewP7dqE7M73usFSz8vwD/0xNOHFRLKbsO"
            + "RqDlLA1Cg2Yd0wWPS0o7+qqk9ndqrjjSwMM8ftnzFGjShAdg4Ca7fFkcNePP"
            + "/rrwIH472FuRb7RbWzwXA4+4ZBdl8D4An0dwtfvAO+jCZSrLjmSpxEOveJxY"
            + "GduyR4IA4lemvAG51YHTHd4NXheuEqsIkn1yarwaaj47lFPnxNOElOREMdZb"
            + "nkWQb1jfgqO24imEZgrLMkK9bJfoDnlF4k6r6hZOp5FSFvc5kJB4cVo1QJl4"
            + "pwCSdoU6luwCggrlZhDnkGCSuQUUW45NE7Br22NGqn4/gHs0KCsWbAezApGj"
            + "qYUCfX1bcpPzUMzUlBaD5rz2vPeO58CDtBJ0ZXN0ZXIgPHRlc3RAdGVzdD6I"
            + "sgQTAQIAHAUCQFdQ0wIbAwQLBwMCAxUCAwMWAgECHgECF4AACgkQs8JyyQfH"
            + "97I1QgP8Cd+35maM2cbWV9iVRO+c5456KDi3oIUSNdPf1NQrCAtJqEUhmMSt"
            + "QbdiaFEkPrORISI/2htXruYn0aIpkCfbUheHOu0sef7s6pHmI2kOQPzR+C/j"
            + "8D9QvWsPOOso81KU2axUY8zIer64Uzqc4szMIlLw06c8vea27RfgjBpSCryw"
            + "AgAA");

        private static readonly char[] pgp8Pass = "2002 Buffalo Sabres".ToCharArray();

        private static readonly char[] pass = "hello world".ToCharArray();

        private static readonly byte[] fingerprintKey = Base64.Decode(
              "mQEPA0CiJdUAAAEIAMI+znDlPd2kQoEcnxqxLcRz56Z7ttFKHpnYp0UkljZdquVc"
            + "By1jMfXGVV64xN1IvMcyenLXUE0IUeUBCQs6tHunFRAPSeCxJ3FdFe1B5MpqQG8A"
            + "BnEpAds/hAUfRDZD5y/lolk1hjvFMrRh6WXckaA/QQ2t00NmTrJ1pYUpkw9tnVQb"
            + "LUjWJhfZDBBcN0ADtATzgkugxMtcDxR6I5x8Ndn+IilqIm23kxGIcmMd/BHOec4c"
            + "jRwJXXDb7u8tl+2knAf9cwhPHp3+Zy4uGSQPdzQnXOhBlA+4WDa0RROOevWgq8uq"
            + "8/9Xp/OlTVL+OoIzjsI6mJP1Joa4qmqAnaHAmXcAEQEAAbQoQk9BM1JTS1kgPEJP"
            + "QSBNb25pdG9yaW5nIEAgODg4LTI2OS01MjY2PokBFQMFEECiJdWqaoCdocCZdwEB"
            + "0RsH/3HPxoUZ3G3K7T3jgOnJUckTSHWU3XspHzMVgqOxjTrcexi5IsAM5M+BulfW"
            + "T2aO+Kqf5w8cKTKgW02DNpHUiPjHx0nzDE+Do95zbIErGeK+Twkc4O/aVsvU9GGO"
            + "81VFI6WMvDQ4CUAUnAdk03MRrzI2nAuhn4NJ5LQS+uJrnqUJ4HmFAz6CQZQKd/kS"
            + "Xgq+A6i7aI1LG80YxWa9ooQgaCrb9dwY/kPQ+yC22zQ3FExtv+Fv3VtAKTilO3vn"
            + "BA4Y9uTHuObHfI+1yxUS2PrlRUX0m48ZjpIX+cEN3QblGBJudI/A1QSd6P0LZeBr"
            + "7F1Z1aF7ZDo0KzgiAIBvgXkeTpw=");

//		private static readonly byte[] fingerprintCheck = Base64.Decode("CTv2");

        private static readonly byte[] jpegImage = Base64.Decode(
              "/9j/4AAQSkZJRgABAQEASABIAAD/4QAWRXhpZgAATU0AKgAAAAgAAAAAAAD/2wBDAAUDBAQEAwUE"
            + "BAQFBQUGBwwIBwcHBw8LCwkMEQ8SEhEPERETFhwXExQaFRERGCEYGh0dHx8fExciJCIeJBweHx7/"
            + "wAALCAA6AFABASIA/8QAHAAAAgMAAwEAAAAAAAAAAAAABQcABAYBAggD/8QAMRAAAgEDBAEDAwME"
            + "AQUAAAAAAQIDBAURAAYSITEHIkETFFEjYXEVMkKRCCUzQ4Gh/9oACAEBAAA/APX1TdKCmlaOoqoo"
            + "WXzzbiP9nWaS71lXuA2tqrgopBOxpyGyWLAEEd4GAf3+fOjLPXoVaOcNzYAhl8HskADwAPz37f3z"
            + "opSvI9Mjypwcr7l/B1XuFwSmoTVooljB9xDYAH51Vor191F9dKGb6Py3yo4huwcHwf8AYP7ZLIyu"
            + "gZSGBGQQejrnU1NKn1EqVi3sZJOBCwxxIp9xzksfb5PR+Mdga+ljqIKje1TNBBNToYYgU4477HwQ"
            + "Bn9z8/nW6mqxLR0NzpJkMLx8lJUkOGAIx4I/0f41lJ93UkkrRxVKvNKVjZfpSe6RyqhCp7wCSD89"
            + "EEDRWppEkgqKdYohGcoZAjAlSMMcZ+PHH/3odsG6VLW2qaoqV+nTyFZpHOFQL0Sc9ADGTnHWtZap"
            + "EpoamJm/TgYkfgJ5H/zGuKieVJIGkqCgmfCJFFy64s3Z+Oh58fHyNfGavipIJ2BrZcKXA+mzEd9Y"
            + "OCcHI/gDV62SzvBGKhQHaNWzj8jvP750oN/xM3qkshLPEstOhj7IVyvkY+f7Nd7hf9vbc9QbVb7n"
            + "dadLldqc00FMCwlmZnCrgL2v/cAySPBPwSD+/wC+3HbWx3rLbaqW81CVHOWnetMZjRm9h7VvClcj"
            + "oDB7PymPTvem+a6roxvC10sd3ScmlucdEyUtRADxdice9wY3PQGRgj4OnHU3u5RW+op6imo4q+KA"
            + "1UKGQ/bzrnt0biWxkgFOJK9ZyCCVX6f3T1Rh9RawbltdQNv18CGe2wxBDQyvGrowIJd15HEnHvP+"
            + "OBjXoGzS0tNTpQipFTIw48Xn5SSBVUMw5e5wMgZ/j86yVNvvZ9TeDR1c9XSV0bl443dmYZXiCSCR"
            + "jvxkjR1L1b46iWpStpIRLOWkCqyniP8AJjxPIniBjr+etFdu11DVu321WZiFHRjZcA/gsO+seNYf"
            + "fVpq6n1Eo5KNATIYmb5Bx7csP4z/AKz8aX1N6Q7W3FuWWrS1TRzi+tXSutUESQhCGiVAvJVRgfcc"
            + "HkeidM6tSmTbps9RHIH4KoqC8j/VC8R0+CSScZLdknPZGgNfYpUUUzfewxxcWpopWbhL715KgBIQ"
            + "MCQc4A84+dD963X7ywQ0NIVW60qqzkzIfoszAMGUNyUHORkDrHxo3sSaOhtX2hnp3uNRF9b7hqtO"
            + "DxM3Rcj3dMCPHXLGfOkLuPddp9R/ViOa62KppqK3Vctvsz0UylKtWfgXy3+L8WIZFBGRhs407rTT"
            + "bcuFDRWmtsNGIZ1MMEU9GPqRorKPcJEzhich8Anz350Wk2zs2OsT7D7RZJpChMEk0MoypJZWVwM9"
            + "ZzjWw2lbKaioFjQy/U9shLyu7Esi5JLEnsgnQlaSqhqayWSRZ5JaiSSNPoBCiq54jPuJyA2W+QfA"
            + "+FrSXq4bdulZHRpWRzpArPK0SSNUExh14qB4c5X9ipz41Zud0juVouVooHN6rrZKVaoek/VhYgqE"
            + "4v7cZPTfPHwT7tZX0e2NVUV5rK2ku9TeY6aFZJ6GuLALKzNnizE4CsqHIyBxJCk4AYFNt2wSUExm"
            + "pP1lqgq1zkfXUtIgkiOFHQCsCM/kfOtZU7GsNZU1FFc1lrqCSNSlFOQ8SJk8kC4/tJx1rMwbWt0V"
            + "CW21VW+krVoFTCRrPC0bf+NF8ocqMcT/AIg6EVF5/p9U6zPXLVFGpoKlSpMiEkniSCcqVY+eQIPW"
            + "NULf/UNxJNS0dhklu8SK9Lco6pUcEr0JOu1HQ7z+R5OndaI5leWV0VQ54kA5KlWIx/Gqd2t6vcqe"
            + "FIXNJMs71SoCMsQuG5jsN8AAjyTnrGlt6mVlqswtS0SG71NTXpSiCQFpogckll6Y4wvyD/OToVd7"
            + "3tLedda4Nr3iRK2mqJhW1K0qxSSGJf1OTOAwwVADLkA9fPV2W77msVfPTClNRUyJCla0SqS5dR5J"
            + "b2kluKlQc5BbHnWu2xTS0G4qmjvSq6RwrPHJUMHkkYDhzJHXIhmBAHnxpaL6j3il3D6g1VLuSz1k"
            + "1ht//S6SZQ4KoTI6MyMOb9hR85HedM/0wqn3RsC0bhgq/pQV9J9WELEFaNWGARg+04xkd95xjQTe"
            + "df6c7U+ysl3mtMFJe5JYGkkmAVKgKZCZGzlVbBySemA/OgvpZUQxvaqitgoqSsiX6XKh5RwVCBP0"
            + "8KCTIoU8VJyDjIA8Bs2e5CprDTR8VXi8pRgyyZMh8qQMDHz850ZOlVv30RsW5blcL5S3a626+1cq"
            + "TirFQ0qJIgAQCNjgIMeFKn9wQCMA3o2vprca/ctp29Jv6/3aoZ4IRRx08dC5D8nWQv7FJYHByeuv"
            + "zo5SWn1Z2ttahutFZqbcG6JK5ZLu1TNEzzUq5ASNyVw6pxUMc5Oc5znR6KyXffldUVW4rBcbAqos"
            + "EUq1qrUzUkwy8bFB+m4ZI2IBbAJAbOdau0+nmybJYqe027atvNHTRlYomhVz+Tln8knyScn50j/+"
            + "SOyd3VO2oDtmPcNPYqJgDt23xKtOIiTy6gYO/Z5YOcAHGsJ/x39NgbzuDc+0bNt6/wAySmltbXGv"
            + "flaT8ST07xBjIR30RjsL+dex9uwT/wBKo6i5UtPFdHp4/u/pgECTiOQDYBIByB+w0RVEVmZUUM39"
            + "xA7P867ampqampqaq09BQwV9RWwUVNFU1AUTTJEoeQLnHJgMnGTjP51a1Nf/2Q==");

        private static readonly byte[] embeddedJPEGKey = Base64.Decode(
              "mI0ER0JXuwEEAKNqsXwLU6gu6P2Q/HJqEJVt3A7Kp1yucn8HWVeJF9JLAKVjVU8jrvz9Bw4NwaRJ"
            + "NGYEAgdRq8Hx3WP9FXFCIVfCdi+oQrphcHWzzBFul8sykUGT+LmcBdqQGU9WaWSJyCOmUht4j7t0"
            + "zk/IXX0YxGmkqR+no5rTj9LMDG8AQQrFABEBAAG0P0VyaWMgSCBFY2hpZG5hIChpbWFnZSB0ZXN0"
            + "IGtleSkgPGVyaWMuZWNoaWRuYUBib3VuY3ljYXN0bGUub3JnPoi2BBMBAgAgBQJHQle7AhsDBgsJ"
            + "CAcDAgQVAggDBBYCAwECHgECF4AACgkQ1+RWqFFpjMTKtgP+Okqkn0gVpQyNYXM/hWX6f3UQcyXk"
            + "2Sd/fWW0XG+LBjhhBo+lXRWK0uYF8OMdZwsSl9HimpgYD5/kNs0Seh417DioP1diOgxkgezyQgMa"
            + "+ODZfNnIvVaBr1pHLPLeqIBxBVMWBfa4wDXnLLGu8018uvI2yBhz5vByB1ntxwgKMXCwAgAD0cf3"
            + "x/UBEAABAQAAAAAAAAAAAAAAAP/Y/+AAEEpGSUYAAQEBAEgASAAA/+EAFkV4aWYAAE1NACoAAAAI"
            + "AAAAAAAA/9sAQwAFAwQEBAMFBAQEBQUFBgcMCAcHBwcPCwsJDBEPEhIRDxERExYcFxMUGhURERgh"
            + "GBodHR8fHxMXIiQiHiQcHh8e/8AACwgAOgBQAQEiAP/EABwAAAIDAAMBAAAAAAAAAAAAAAUHAAQG"
            + "AQIIA//EADEQAAIBAwQBAwMDBAEFAAAAAAECAwQFEQAGEiExByJBExRRI2FxFTJCkQglM0OBof/a"
            + "AAgBAQAAPwD19U3SgppWjqKqKFl8824j/Z1mku9ZV7gNraq4KKQTsachsliwBBHeBgH9/nzoyz16"
            + "FWjnDc2AIZfB7JAA8AD89+3986KUryPTI8qcHK+5fwdV7hcEpqE1aKJYwfcQ2AB+dVaK9fdRfXSh"
            + "m+j8t8qOIbsHB8H/AGD+2SyMroGUhgRkEHo651NTSp9RKlYt7GSTgQsMcSKfcc5LH2+T0fjHYGvp"
            + "Y6iCo3tUzQQTU6GGIFOOO+x8EAZ/c/P51upqsS0dDc6SZDC8fJSVJDhgCMeCP9H+NZSfd1JJK0cV"
            + "SrzSlY2X6UnukcqoQqe8Akg/PRBA0VqaRJIKinWKIRnKGQIwJUjDHGfjxx/96HbBulS1tqmqKlfp"
            + "08hWaRzhUC9EnPQAxk5x1rWWqRKaGpiZv04GJH4CeR/8xrionlSSBpKgoJnwiRRcuuLN2fjoefHx"
            + "8jXxmr4qSCdga2XClwPpsxHfWDgnByP4A1etks7wRioUB2jVs4/I7z++dKDf8TN6pLISzxLLToY+"
            + "yFcr5GPn+zXe4X/b23PUG1W+53WnS5XanNNBTAsJZmZwq4C9r/3AMkjwT8Eg/v8Avtx21sd6y22q"
            + "lvNQlRzlp3rTGY0ZvYe1bwpXI6Awez8pj073pvmuq6MbwtdLHd0nJpbnHRMlLUQA8XYnHvcGNz0B"
            + "kYI+Dpx1N7uUVvqKeopqOKvigNVChkP28657dG4lsZIBTiSvWcgglV+n909UYfUWsG5bXUDb9fAh"
            + "ntsMQQ0Mrxq6MCCXdeRxJx7z/jgY16Bs0tLTU6UIqRUyMOPF5+UkgVVDMOXucDIGf4/OslTb72fU"
            + "3g0dXPV0ldG5eON3ZmGV4gkgkY78ZI0dS9W+OolqUraSESzlpAqsp4j/ACY8TyJ4gY6/nrRXbtdQ"
            + "1bt9tVmYhR0Y2XAP4LDvrHjWH31aaup9RKOSjQEyGJm+Qce3LD+M/wCs/Gl9TekO1txbllq0tU0c"
            + "4vrV0rrVBEkIQholQLyVUYH3HB5HonTOrUpk26bPURyB+CqKgvI/1QvEdPgkknGS3ZJz2RoDX2KV"
            + "FFM33sMcXFqaKVm4S+9eSoASEDAkHOAPOPnQ/et1+8sENDSFVutKqs5MyH6LMwDBlDclBzkZA6x8"
            + "aN7EmjobV9oZ6d7jURfW+4arTg8TN0XI93TAjx1yxnzpC7j3XafUf1Yjmutiqaait1XLb7M9FMpS"
            + "rVn4F8t/i/FiGRQRkYbONO60023LhQ0VprbDRiGdTDBFPRj6kaKyj3CRM4YnIfAJ89+dFpNs7Njr"
            + "E+w+0WSaQoTBJNDKMqSWVlcDPWc41sNpWymoqBY0Mv1PbIS8ruxLIuSSxJ7IJ0JWkqoamslkkWeS"
            + "WokkjT6AQoqueIz7icgNlvkHwPha0l6uG3bpWR0aVkc6QKzytEkjVBMYdeKgeHOV/Yqc+NWbndI7"
            + "laLlaKBzeq62SlWqHpP1YWIKhOL+3GT03zx8E+7WV9HtjVVFeaytpLvU3mOmhWSehriwCyszZ4sx"
            + "OArKhyMgcSQpOAGBTbdsElBMZqT9ZaoKtc5H11LSIJIjhR0ArAjP5HzrWVOxrDWVNRRXNZa6gkjU"
            + "pRTkPEiZPJAuP7ScdazMG1rdFQlttVVvpK1aBUwkazwtG3/jRfKHKjHE/wCIOhFRef6fVOsz1y1R"
            + "RqaCpUqTIhJJ4kgnKlWPnkCD1jVC3/1DcSTUtHYZJbvEivS3KOqVHBK9CTrtR0O8/keTp3WiOZXl"
            + "ldFUOeJAOSpViMfxqndrer3KnhSFzSTLO9UqAjLELhuY7DfAAI8k56xpbeplZarMLUtEhu9TU16U"
            + "ogkBaaIHJJZemOML8g/zk6FXe97S3nXWuDa94kStpqiYVtStKsUkhiX9TkzgMMFQAy5APXz1dlu+"
            + "5rFXz0wpTUVMiQpWtEqkuXUeSW9pJbipUHOQWx51rtsU0tBuKpo70qukcKzxyVDB5JGA4cyR1yIZ"
            + "gQB58aWi+o94pdw+oNVS7ks9ZNYbf/0ukmUOCqEyOjMjDm/YUfOR3nTP9MKp90bAtG4YKv6UFfSf"
            + "VhCxBWjVhgEYPtOMZHfecY0E3nX+nO1PsrJd5rTBSXuSWBpJJgFSoCmQmRs5VWwcknpgPzoL6WVE"
            + "Mb2qorYKKkrIl+lyoeUcFQgT9PCgkyKFPFScg4yAPAbNnuQqaw00fFV4vKUYMsmTIfKkDAx8/OdG"
            + "TpVb99EbFuW5XC+Ut2utuvtXKk4qxUNKiSIAEAjY4CDHhSp/cEAjAN6Nr6a3Gv3LadvSb+v92qGe"
            + "CEUcdPHQuQ/J1kL+xSWBwcnrr86OUlp9WdrbWobrRWam3BuiSuWS7tUzRM81KuQEjclcOqcVDHOT"
            + "nOc50eisl335XVFVuKwXGwKqLBFKtaq1M1JMMvGxQfpuGSNiAWwCQGznWrtPp5smyWKntNu2rbzR"
            + "00ZWKJoVc/k5Z/JJ8knJ+dI//kjsnd1TtqA7Zj3DT2KiYA7dt8SrTiIk8uoGDv2eWDnABxrCf8d/"
            + "TYG87g3PtGzbev8AMkppbW1xr35Wk/Ek9O8QYyEd9EY7C/nXsfbsE/8ASqOouVLTxXR6eP7v6YBA"
            + "k4jkA2ASAcgfsNEVRFZmVFDN/cQOz/Ou2pqampqamqtPQUMFfUVsFFTRVNQFE0yRKHkC5xyYDJxk"
            + "4z+dWtTX/9mItgQTAQIAIAUCR0JYkAIbAwYLCQgHAwIEFQIIAwQWAgMBAh4BAheAAAoJENfkVqhR"
            + "aYzEAPYD/iHdLOAE8r8HHF3F4z28vtIT8iiRB9aPC/YH0xqV1qeEKG8+VosBaQAOCEquONtRWsww"
            + "gO3XB0d6VAq2kMOKc2YiB4ZtZcFvvmP9KdmVIZxVjpa9ozjP5j9zFso1HOpFcsn/VDBEqy5TvsNx"
            + "Qvmtc8X7lqK/zLRVkSSBItik2IIhsAIAAw==");

        private void FingerPrintTest()
        {
            //
            // version 3
            //
            PgpPublicKeyRing pgpPub = new PgpPublicKeyRing(fingerprintKey);

            PgpPublicKey pubKey = pgpPub.GetPublicKey();

            if (!Arrays.AreEqual(pubKey.GetFingerprint(), Hex.Decode("4FFB9F0884266C715D1CEAC804A3BBFA")))
            {
                Fail("version 3 fingerprint test failed");
            }

            //
            // version 4
            //
            pgpPub = new PgpPublicKeyRing(testPubKey);

            pubKey = pgpPub.GetPublicKey();

            if (!Arrays.AreEqual(pubKey.GetFingerprint(), Hex.Decode("3062363c1046a01a751946bb35586146fdf3f373")))
            {
               Fail("version 4 fingerprint test failed");
            }
        }

        private void MixedTest(
            PgpPrivateKey	pgpPrivKey,
            PgpPublicKey	pgpPubKey)
        {
            byte[] text = Encoding.ASCII.GetBytes("hello world!\n");

            //
            // literal data
            //
            MemoryStream bOut = new MemoryStream();
            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(
                bOut,
                PgpLiteralData.Binary,
                PgpLiteralData.Console,
                text.Length,
                DateTime.UtcNow);

            lOut.Write(text, 0, text.Length);

            lGen.Close();

            byte[] bytes = bOut.ToArray();

            PgpObjectFactory f = new PgpObjectFactory(bytes);
            CheckLiteralData((PgpLiteralData)f.NextPgpObject(), text);

            MemoryStream bcOut = new MemoryStream();

            PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(
                SymmetricKeyAlgorithmTag.Aes128,
                true,
                new SecureRandom());

            encGen.AddMethod(pgpPubKey);

            encGen.AddMethod("password".ToCharArray(), HashAlgorithmTag.Sha1);

            Stream cOut = encGen.Open(bcOut, bytes.Length);

            cOut.Write(bytes, 0, bytes.Length);

            cOut.Close();

            byte[] encData = bcOut.ToArray();

            //
            // asymmetric
            //
            PgpObjectFactory pgpF = new PgpObjectFactory(encData);

            PgpEncryptedDataList encList = (PgpEncryptedDataList) pgpF.NextPgpObject();

            PgpPublicKeyEncryptedData  encP = (PgpPublicKeyEncryptedData)encList[0];

            Stream clear = encP.GetDataStream(pgpPrivKey);

            PgpObjectFactory pgpFact = new PgpObjectFactory(clear);

            CheckLiteralData((PgpLiteralData)pgpFact.NextPgpObject(), text);

            //
            // PBE
            //
            pgpF = new PgpObjectFactory(encData);

            encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            PgpPbeEncryptedData encPbe = (PgpPbeEncryptedData) encList[1];

            clear = encPbe.GetDataStream("password".ToCharArray());

            pgpF = new PgpObjectFactory(clear);

            CheckLiteralData((PgpLiteralData) pgpF.NextPgpObject(), text);
        }

        private void CheckLiteralData(
            PgpLiteralData	ld,
            byte[]			data)
        {
            if (!ld.FileName.Equals(PgpLiteralData.Console))
                throw new Exception("wrong filename in packet");

            Stream inLd = ld.GetDataStream();
            byte[] bytes = Streams.ReadAll(inLd);

            if (!AreEqual(bytes, data))
            {
                Fail("wrong plain text in decrypted packet");
            }
        }

        private void ExistingEmbeddedJpegTest()
        {
            PgpPublicKeyRing pgpPub = new PgpPublicKeyRing(embeddedJPEGKey);

            PgpPublicKey pubKey = pgpPub.GetPublicKey();

            int count = 0;
            foreach (PgpUserAttributeSubpacketVector attributes in pubKey.GetUserAttributes())
            {
                int sigCount = 0;
                foreach (PgpSignature sig in pubKey.GetSignaturesForUserAttribute(attributes))
                {
                    sig.InitVerify(pubKey);

                    if (!sig.VerifyCertification(attributes, pubKey))
                    {
                        Fail("signature failed verification");
                    }

                    sigCount++;
                }

                if (sigCount != 1)
                {
                    Fail("Failed user attributes signature check");
                }
                count++;
            }

            if (count != 1)
            {
                Fail("didn't find user attributes");
            }
        }

        private void EmbeddedJpegTest()
        {
            PgpPublicKeyRing pgpPub = new PgpPublicKeyRing(testPubKey);
            PgpSecretKeyRing pgpSec = new PgpSecretKeyRing(testPrivKey);

            PgpPublicKey pubKey = pgpPub.GetPublicKey();

            PgpUserAttributeSubpacketVectorGenerator vGen = new PgpUserAttributeSubpacketVectorGenerator();

            vGen.SetImageAttribute(ImageAttrib.Format.Jpeg, jpegImage);

            PgpUserAttributeSubpacketVector uVec = vGen.Generate();

            PgpSignatureGenerator sGen = new PgpSignatureGenerator(
                PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.PositiveCertification, pgpSec.GetSecretKey().ExtractPrivateKey(pass));

            PgpSignature sig = sGen.GenerateCertification(uVec, pubKey);

            PgpPublicKey nKey = PgpPublicKey.AddCertification(pubKey, uVec, sig);

            int count = 0;
            foreach (PgpUserAttributeSubpacketVector attributes in nKey.GetUserAttributes())
            {
                int sigCount = 0;
                foreach (PgpSignature s in nKey.GetSignaturesForUserAttribute(attributes))
                {
                    s.InitVerify(pubKey);

                    if (!s.VerifyCertification(attributes, pubKey))
                    {
                        Fail("added signature failed verification");
                    }

                    sigCount++;
                }

                if (sigCount != 1)
                {
                    Fail("Failed added user attributes signature check");
                }
                count++;
            }

            if (count != 1)
            {
                Fail("didn't find added user attributes");
            }

            nKey = PgpPublicKey.RemoveCertification(nKey, uVec);

            if (nKey.GetUserAttributes().GetEnumerator().MoveNext())
            {
                Fail("found attributes where none expected");
            }
        }

        public override void PerformTest()
        {
            //
            // Read the public key
            //
            PgpPublicKeyRing pgpPub = new PgpPublicKeyRing(testPubKey);

            AsymmetricKeyParameter pubKey = pgpPub.GetPublicKey().GetKey();

            IEnumerator enumerator = pgpPub.GetPublicKey().GetUserIds().GetEnumerator();
            enumerator.MoveNext();
            string uid = (string) enumerator.Current;


            enumerator = pgpPub.GetPublicKey().GetSignaturesForId(uid).GetEnumerator();
            enumerator.MoveNext();
            PgpSignature sig = (PgpSignature) enumerator.Current;

            sig.InitVerify(pgpPub.GetPublicKey());

            if (!sig.VerifyCertification(uid, pgpPub.GetPublicKey()))
            {
                Fail("failed to verify certification");
            }

            //
            // write a public key
            //
            MemoryStream bOut = new UncloseableMemoryStream();
            BcpgOutputStream pOut = new BcpgOutputStream(bOut);

            pgpPub.Encode(pOut);

            if (!Arrays.AreEqual(bOut.ToArray(), testPubKey))
            {
                Fail("public key rewrite failed");
            }

            //
            // Read the public key
            //
            PgpPublicKeyRing pgpPubV3 = new PgpPublicKeyRing(testPubKeyV3);
            AsymmetricKeyParameter pubKeyV3 = pgpPub.GetPublicKey().GetKey();

            //
            // write a V3 public key
            //
            bOut = new UncloseableMemoryStream();
            pOut = new BcpgOutputStream(bOut);

            pgpPubV3.Encode(pOut);

            //
            // Read a v3 private key
            //
            char[] passP = "FIXCITY_QA".ToCharArray();

            {
                PgpSecretKeyRing pgpPriv2 = new PgpSecretKeyRing(testPrivKeyV3);
                PgpSecretKey pgpPrivSecretKey = pgpPriv2.GetSecretKey();
                PgpPrivateKey pgpPrivKey2 = pgpPrivSecretKey.ExtractPrivateKey(passP);

                //
                // write a v3 private key
                //
                bOut = new UncloseableMemoryStream();
                pOut = new BcpgOutputStream(bOut);

                pgpPriv2.Encode(pOut);

                byte[] result = bOut.ToArray();
                if (!Arrays.AreEqual(result, testPrivKeyV3))
                {
                    Fail("private key V3 rewrite failed");
                }
            }

            //
            // Read the private key
            //
            PgpSecretKeyRing pgpPriv = new PgpSecretKeyRing(testPrivKey);
            PgpPrivateKey pgpPrivKey = pgpPriv.GetSecretKey().ExtractPrivateKey(pass);

            //
            // write a private key
            //
            bOut = new UncloseableMemoryStream();
            pOut = new BcpgOutputStream(bOut);

            pgpPriv.Encode(pOut);

            if (!Arrays.AreEqual(bOut.ToArray(), testPrivKey))
            {
                Fail("private key rewrite failed");
            }

            //
            // test encryption
            //
            IBufferedCipher c = CipherUtilities.GetCipher("RSA");

//                c.Init(Cipher.ENCRYPT_MODE, pubKey);
            c.Init(true, pubKey);

            byte[] inBytes = Encoding.ASCII.GetBytes("hello world");
            byte[] outBytes = c.DoFinal(inBytes);

//                c.Init(Cipher.DECRYPT_MODE, pgpPrivKey.GetKey());
            c.Init(false, pgpPrivKey.Key);

            outBytes = c.DoFinal(outBytes);

            if (!Arrays.AreEqual(inBytes, outBytes))
            {
                Fail("decryption failed.");
            }

            //
            // test signature message
            //
            PgpObjectFactory pgpFact = new PgpObjectFactory(sig1);

            PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

            PgpOnePassSignature ops = p1[0];

            PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();

            Stream dIn = p2.GetInputStream();

            ops.InitVerify(pgpPub.GetPublicKey(ops.KeyId));

            int ch;
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed signature check");
            }

            //
            // encrypted message - read subkey
            //
            pgpPriv = new PgpSecretKeyRing(subKey);

            //
            // encrypted message
            //
            byte[] text = Encoding.ASCII.GetBytes("hello world!\n");

            PgpObjectFactory pgpF = new PgpObjectFactory(enc1);

            PgpEncryptedDataList encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            PgpPublicKeyEncryptedData encP = (PgpPublicKeyEncryptedData)encList[0];

            pgpPrivKey = pgpPriv.GetSecretKey(encP.KeyId).ExtractPrivateKey(pass);

            Stream clear = encP.GetDataStream(pgpPrivKey);

            pgpFact = new PgpObjectFactory(clear);

            c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpLiteralData ld = (PgpLiteralData)pgpFact.NextPgpObject();

            if (!ld.FileName.Equals("test.txt"))
            {
                throw new Exception("wrong filename in packet");
            }

            Stream inLd = ld.GetDataStream();
            byte[] bytes = Streams.ReadAll(inLd);

            if (!Arrays.AreEqual(bytes, text))
            {
                Fail("wrong plain text in decrypted packet");
            }

            //
            // encrypt - short message
            //
            byte[] shortText = { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };

            MemoryStream cbOut = new UncloseableMemoryStream();
            PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, new SecureRandom());
            PgpPublicKey puK = pgpPriv.GetSecretKey(encP.KeyId).PublicKey;

            cPk.AddMethod(puK);

            Stream cOut = cPk.Open(new UncloseableStream(cbOut), shortText.Length);

            cOut.Write(shortText, 0, shortText.Length);

            cOut.Close();

            pgpF = new PgpObjectFactory(cbOut.ToArray());

            encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            encP = (PgpPublicKeyEncryptedData)encList[0];

            pgpPrivKey = pgpPriv.GetSecretKey(encP.KeyId).ExtractPrivateKey(pass);

            if (encP.GetSymmetricAlgorithm(pgpPrivKey) != SymmetricKeyAlgorithmTag.Cast5)
            {
                Fail("symmetric algorithm mismatch");
            }

            clear = encP.GetDataStream(pgpPrivKey);
            outBytes = Streams.ReadAll(clear);

            if (!Arrays.AreEqual(outBytes, shortText))
            {
                Fail("wrong plain text in generated short text packet");
            }

            //
            // encrypt
            //
            cbOut = new UncloseableMemoryStream();
            cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, new SecureRandom());
            puK = pgpPriv.GetSecretKey(encP.KeyId).PublicKey;

            cPk.AddMethod(puK);

            cOut = cPk.Open(new UncloseableStream(cbOut), text.Length);

            cOut.Write(text, 0, text.Length);

            cOut.Close();

            pgpF = new PgpObjectFactory(cbOut.ToArray());

            encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            encP = (PgpPublicKeyEncryptedData)encList[0];

            pgpPrivKey = pgpPriv.GetSecretKey(encP.KeyId).ExtractPrivateKey(pass);

            clear = encP.GetDataStream(pgpPrivKey);
            outBytes = Streams.ReadAll(clear);

            if (!Arrays.AreEqual(outBytes, text))
            {
                Fail("wrong plain text in generated packet");
            }

            //
            // read public key with sub key.
            //
            pgpF = new PgpObjectFactory(subPubKey);
            object o;
            while ((o = pgpFact.NextPgpObject()) != null)
            {
                // TODO Should something be tested here?
                // Console.WriteLine(o);
            }

            //
            // key pair generation - CAST5 encryption
            //
            char[] passPhrase = "hello".ToCharArray();
            IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");
            RsaKeyGenerationParameters genParam = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 25);

            kpg.Init(genParam);


            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                kp.Public,
                kp.Private,
                DateTime.UtcNow,
                "fred",
                SymmetricKeyAlgorithmTag.Cast5,
                passPhrase,
                null,
                null,
                new SecureRandom()
                );

            PgpPublicKey key = secretKey.PublicKey;


            enumerator = key.GetUserIds().GetEnumerator();
            enumerator.MoveNext();
            uid = (string) enumerator.Current;


            enumerator = key.GetSignaturesForId(uid).GetEnumerator();
            enumerator.MoveNext();
            sig = (PgpSignature) enumerator.Current;

            sig.InitVerify(key);

            if (!sig.VerifyCertification(uid, key))
            {
                Fail("failed to verify certification");
            }

            pgpPrivKey = secretKey.ExtractPrivateKey(passPhrase);

            key = PgpPublicKey.RemoveCertification(key, uid, sig);

            if (key == null)
            {
                Fail("failed certification removal");
            }

            byte[] keyEnc = key.GetEncoded();

            key = PgpPublicKey.AddCertification(key, uid, sig);

            keyEnc = key.GetEncoded();

            PgpSignatureGenerator sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.KeyRevocation, secretKey.ExtractPrivateKey(passPhrase));

            sig = sGen.GenerateCertification(key);

            key = PgpPublicKey.AddCertification(key, sig);

            keyEnc = key.GetEncoded();

            PgpPublicKeyRing tmpRing = new PgpPublicKeyRing(keyEnc);

            key = tmpRing.GetPublicKey();

            IEnumerator sgEnum = key.GetSignaturesOfType(PgpSignature.KeyRevocation).GetEnumerator();
            sgEnum.MoveNext();
            sig = (PgpSignature) sgEnum.Current;

            sig.InitVerify(key);

            if (!sig.VerifyCertification(key))
            {
                Fail("failed to verify revocation certification");
            }

            //
            // use of PgpKeyPair
            //
            PgpKeyPair pgpKp = new PgpKeyPair(PublicKeyAlgorithmTag.RsaGeneral,
                kp.Public, kp.Private, DateTime.UtcNow);

            PgpPublicKey k1 = pgpKp.PublicKey;
            PgpPrivateKey k2 = pgpKp.PrivateKey;

            k1.GetEncoded();

            MixedTest(k2, k1);

            //
            // key pair generation - AES_256 encryption.
            //
            kp = kpg.GenerateKeyPair();

            secretKey = new PgpSecretKey(PgpSignature.DefaultCertification, PublicKeyAlgorithmTag.RsaGeneral, kp.Public, kp.Private, DateTime.UtcNow, "fred", SymmetricKeyAlgorithmTag.Aes256, passPhrase, null, null, new SecureRandom());

            secretKey.ExtractPrivateKey(passPhrase);

            secretKey.Encode(new UncloseableMemoryStream());

            //
            // secret key password changing.
            //
            const string newPass = "newPass";

            secretKey = PgpSecretKey.CopyWithNewPassword(secretKey, passPhrase, newPass.ToCharArray(), secretKey.KeyEncryptionAlgorithm, new SecureRandom());

            secretKey.ExtractPrivateKey(newPass.ToCharArray());

            secretKey.Encode(new UncloseableMemoryStream());

            key = secretKey.PublicKey;

            key.Encode(new UncloseableMemoryStream());


            enumerator = key.GetUserIds().GetEnumerator();
            enumerator.MoveNext();
            uid = (string) enumerator.Current;


            enumerator = key.GetSignaturesForId(uid).GetEnumerator();
            enumerator.MoveNext();
            sig = (PgpSignature) enumerator.Current;

            sig.InitVerify(key);

            if (!sig.VerifyCertification(uid, key))
            {
                Fail("failed to verify certification");
            }

            pgpPrivKey = secretKey.ExtractPrivateKey(newPass.ToCharArray());

            //
            // signature generation
            //
            const string data = "hello world!";
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            bOut = new UncloseableMemoryStream();

            MemoryStream testIn = new MemoryStream(dataBytes, false);

            sGen = new PgpSignatureGenerator(
                PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);

            PgpCompressedDataGenerator cGen = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);

            BcpgOutputStream bcOut = new BcpgOutputStream(cGen.Open(new UncloseableStream(bOut)));

            sGen.GenerateOnePassVersion(false).Encode(bcOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();

            DateTime testDateTime = new DateTime(1973, 7, 27);
            Stream lOut = lGen.Open(new UncloseableStream(bcOut), PgpLiteralData.Binary, "_CONSOLE",
                dataBytes.Length, testDateTime);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Close();

            sGen.Generate().Encode(bcOut);

            bcOut.Close();

            //
            // verify generated signature
            //
            pgpFact = new PgpObjectFactory(bOut.ToArray());

            c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

            ops = p1[0];

            p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            if (!p2.ModificationTime.Equals(testDateTime))
            {
                Fail("Modification time not preserved");
            }

            dIn = p2.GetInputStream();

            ops.InitVerify(secretKey.PublicKey);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            p3 = (PgpSignatureList)pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed generated signature check");
            }

            //
            // signature generation - version 3
            //
            bOut = new UncloseableMemoryStream();

            testIn = new MemoryStream(dataBytes);
            PgpV3SignatureGenerator sGenV3 = new PgpV3SignatureGenerator(
                PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);

            cGen = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

            bcOut = new BcpgOutputStream(cGen.Open(new UncloseableStream(bOut)));

            sGen.GenerateOnePassVersion(false).Encode(bcOut);

            lGen = new PgpLiteralDataGenerator();
            lOut = lGen.Open(
                new UncloseableStream(bcOut),
                PgpLiteralData.Binary,
                "_CONSOLE",
                dataBytes.Length,
                testDateTime);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte) ch);
                sGen.Update((byte)ch);
            }

            lOut.Close();

            sGen.Generate().Encode(bcOut);

            bcOut.Close();

            //
            // verify generated signature
            //
            pgpFact = new PgpObjectFactory(bOut.ToArray());

            c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

            ops = p1[0];

            p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            if (!p2.ModificationTime.Equals(testDateTime))
            {
                Fail("Modification time not preserved");
            }

            dIn = p2.GetInputStream();

            ops.InitVerify(secretKey.PublicKey);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            p3 = (PgpSignatureList)pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed v3 generated signature check");
            }

            //
            // extract PGP 8 private key
            //
            pgpPriv = new PgpSecretKeyRing(pgp8Key);

            secretKey = pgpPriv.GetSecretKey();

            pgpPrivKey = secretKey.ExtractPrivateKey(pgp8Pass);

            //
            // other sig tests
            //
            PerformTestSig(HashAlgorithmTag.Sha256, secretKey.PublicKey, pgpPrivKey);
            PerformTestSig(HashAlgorithmTag.Sha384, secretKey.PublicKey, pgpPrivKey);
            PerformTestSig(HashAlgorithmTag.Sha512, secretKey.PublicKey, pgpPrivKey);
            FingerPrintTest();
            ExistingEmbeddedJpegTest();
            EmbeddedJpegTest();
        }

        private void PerformTestSig(
            HashAlgorithmTag	hashAlgorithm,
            PgpPublicKey		pubKey,
            PgpPrivateKey		privKey)
        {
            const string data = "hello world!";
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            MemoryStream bOut = new UncloseableMemoryStream();
            MemoryStream testIn = new MemoryStream(dataBytes, false);
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.RsaGeneral, hashAlgorithm);

            sGen.InitSign(PgpSignature.BinaryDocument, privKey);

            PgpCompressedDataGenerator cGen = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

            BcpgOutputStream bcOut = new BcpgOutputStream(cGen.Open(new UncloseableStream(bOut)));

            sGen.GenerateOnePassVersion(false).Encode(bcOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            DateTime testDateTime = new DateTime(1973, 7, 27);
            Stream lOut = lGen.Open(
                new UncloseableStream(bcOut),
                PgpLiteralData.Binary,
                "_CONSOLE",
                dataBytes.Length,
                testDateTime);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Close();

            sGen.Generate().Encode(bcOut);

            bcOut.Close();

            //
            // verify generated signature
            //
            PgpObjectFactory pgpFact = new PgpObjectFactory(bOut.ToArray());

            PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();

            PgpOnePassSignature ops = p1[0];

            PgpLiteralData p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            if (!p2.ModificationTime.Equals(testDateTime))
            {
                Fail("Modification time not preserved");
            }

            Stream dIn = p2.GetInputStream();

            ops.InitVerify(pubKey);

            // TODO Need a stream object to automatically call Update?
            // (via ISigner implementation of PgpSignatureGenerator)
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed generated signature check - " + hashAlgorithm);
            }
        }

        private class UncloseableMemoryStream
            : MemoryStream
        {
            public override void Close()
            {
                throw new Exception("Close() called on underlying stream");
            }
        }

        public override string Name
        {
            get { return "PGPRSATest"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new PgpRsaTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
