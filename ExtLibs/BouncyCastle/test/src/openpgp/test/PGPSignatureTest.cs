using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpSignatureTest
        : SimpleTest
    {
        private const int[] NO_PREFERENCES = null;
        private static readonly int[] PREFERRED_SYMMETRIC_ALGORITHMS
            = new int[] { (int)SymmetricKeyAlgorithmTag.Aes128, (int)SymmetricKeyAlgorithmTag.TripleDes };
        private static readonly int[] PREFERRED_HASH_ALGORITHMS
            = new int[] { (int)HashAlgorithmTag.Sha1, (int)HashAlgorithmTag.Sha256 };
        private static readonly int[] PREFERRED_COMPRESSION_ALGORITHMS
            = new int[] { (int)CompressionAlgorithmTag.ZLib };

        private const int TEST_EXPIRATION_TIME = 10000;
        private const string TEST_USER_ID = "test user id";
        private static readonly byte[] TEST_DATA = Encoding.ASCII.GetBytes("hello world!\nhello world!\n");
        private static readonly byte[] TEST_DATA_WITH_CRLF = Encoding.ASCII.GetBytes("hello world!\r\nhello world!\r\n");

        private static readonly byte[] dsaKeyRing = Base64.Decode(
            "lQHhBD9HBzURBACzkxRCVGJg5+Ld9DU4Xpnd4LCKgMq7YOY7Gi0EgK92gbaa6+zQ"
            + "oQFqz1tt3QUmpz3YVkm/zLESBBtC1ACIXGggUdFMUr5I87+1Cb6vzefAtGt8N5VV"
            + "1F/MXv1gJz4Bu6HyxL/ncfe71jsNhav0i4yAjf2etWFj53zK6R+Ojg5H6wCgpL9/"
            + "tXVfGP8SqFvyrN/437MlFSUEAIN3V6j/MUllyrZglrtr2+RWIwRrG/ACmrF6hTug"
            + "Ol4cQxaDYNcntXbhlTlJs9MxjTH3xxzylyirCyq7HzGJxZzSt6FTeh1DFYzhJ7Qu"
            + "YR1xrSdA6Y0mUv0ixD5A4nPHjupQ5QCqHGeRfFD/oHzD4zqBnJp/BJ3LvQ66bERJ"
            + "mKl5A/4uj3HoVxpb0vvyENfRqKMmGBISycY4MoH5uWfb23FffsT9r9KL6nJ4syLz"
            + "aRR0gvcbcjkc9Z3epI7gr3jTrb4d8WPxsDbT/W1tv9bG/EHawomLcihtuUU68Uej"
            + "6/wZot1XJqu2nQlku57+M/V2X1y26VKsipolPfja4uyBOOyvbP4DAwIDIBTxWjkC"
            + "GGAWQO2jy9CTvLHJEoTO7moHrp1FxOVpQ8iJHyRqZzLllO26OzgohbiPYz8u9qCu"
            + "lZ9Xn7QzRXJpYyBFY2hpZG5hIChEU0EgVGVzdCBLZXkpIDxlcmljQGJvdW5jeWNh"
            + "c3RsZS5vcmc+iFkEExECABkFAj9HBzUECwcDAgMVAgMDFgIBAh4BAheAAAoJEM0j"
            + "9enEyjRDAlwAnjTjjt57NKIgyym7OTCwzIU3xgFpAJ0VO5m5PfQKmGJRhaewLSZD"
            + "4nXkHg==");

        private static readonly char[] dsaPass = "hello world".ToCharArray();

        private static readonly byte[] rsaKeyRing = Base64.Decode(
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

        private static readonly char[] rsaPass = "2002 Buffalo Sabres".ToCharArray();

        private static readonly byte[] nullPacketsSubKeyBinding = Base64.Decode(
            "iDYEGBECAAAAACp9AJ9PlJCrFpi+INwG7z61eku2Wg1HaQCgl33X5Egj+Kf7F9CXIWj2iFCvQDo=");

        private static readonly byte[] okAttr = Base64.Decode(
                "mQENBFOkuoMBCAC+8WcWLBZovlR5pLW4tbOoH3APia+poMEeTNkXKe8yAH0f"
              + "ZmTQgeXFBIizd4Ka1QETbayv+C6Axt6Ipdwf+3N/lqcOqg6PEwuIX4MBrv5R"
              + "ILMH5QyM3a3RlyXa7xES3I9t2VHiZvl15OrTZe67YNGtxlXyeawt6v/9d/a3"
              + "M1EaUzjN4H2EfI3P/VWpMUvQkn70996UKInOyaSB0hef/QS10jshG9DdgmLM"
              + "1/mJFRp8ynZOV4yGLnAdoEoPGG/HJZEzWfqOiwmWZOIrZIwedY1eKuMIhUGv"
              + "LTC9u+9X0h+Y0st5eb1pf8OLvrpRpEyHMrxXfj/V3rxom4d160ifGihPABEB"
              + "AAG0GndpdGggYXR0dHIgPGF0dHJAYXR0ci5uZXQ+iQE4BBMBAgAiBQJTpLqD"
              + "AhsDBgsJCAcDAgYVCAIJCgsEFgIDAQIeAQIXgAAKCRBCjbg0bKVgCXJiB/wO"
              + "6ksdrAy+zVxygFhk6Ju2vpMAOGnLl1nqBVT1mA5XiJu3rSiJmROLF2l21K0M"
              + "BICZfz+mjIwN56RZNzZnEmXk/E2+PgADV5VTRRsjqlyoeN/NrLWuTm9FyngJ"
              + "f96jVPysN6FzYRUB5Fuys57P+nu0RMoLGkHmQhp4L5hgNJTBy1SRnXukoIgJ"
              + "2Ra3EBQ7dBrzuWW1ycwU5acfOoxfcVqgXkiXaxgvujFChZGWT6djbnbbzlMm"
              + "sMKr6POKChEPWo1HJXXz1OaPsd75JA8bImgnrHhB3dHhD2wIqzQrtTxvraqz"
              + "ZWWR2xYZPltzBSlaAdn8Hf0GGBoMhutb3tJLzbAX0cybzJkBEAABAQAAAAAA"
              + "AAAAAAAAAP/Y/+AAEEpGSUYAAQEAAAEAAQAA/9sAQwAKBwcIBwYKCAgICwoK"
              + "Cw4YEA4NDQ4dFRYRGCMfJSQiHyIhJis3LyYpNCkhIjBBMTQ5Oz4+PiUuRElD"
              + "PEg3PT47/9sAQwEKCwsODQ4cEBAcOygiKDs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7"
              + "Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7/8AAEQgAkAB4AwEiAAIR"
              + "AQMRAf/EAB8AAAEFAQEBAQEBAAAAAAAAAAABAgMEBQYHCAkKC//EALUQAAIB"
              + "AwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHw"
              + "JDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVm"
              + "Z2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4"
              + "ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+v/EAB8B"
              + "AAMBAQEBAQEBAQEAAAAAAAABAgMEBQYHCAkKC//EALURAAIBAgQEAwQHBQQE"
              + "AAECdwABAgMRBAUhMQYSQVEHYXETIjKBCBRCkaGxwQkjM1LwFWJy0QoWJDTh"
              + "JfEXGBkaJicoKSo1Njc4OTpDREVGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2"
              + "d3h5eoKDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXG"
              + "x8jJytLT1NXW19jZ2uLj5OXm5+jp6vLz9PX29/j5+v/aAAwDAQACEQMRAD8A"
              + "9moqtf30Gm2cl3cvtijGSa4a++LNlGStlZvKR0ZuBWkKU6nwomU4x3PQqK8g"
              + "uPinrEzYhhihX86ns/Ffia/XzElJUHOV4/rW/wBUqJXlZEe2i9j1iivMP+Ex"
              + "1q3+/KCw6gip4PiXdREC5tUkHcrwaTwlVbK4e1iekUVzmheNdO1ycWyK8U5G"
              + "drf410QOa55RcXaSNE09ULRRRUjCiiigAooooAKKKKAOY+IblfCN1g9cA/rX"
              + "h1fQPiXT4dU0o2dwXEcrclCARgE8ZB9K4J/AGkKeJr38ZU/+Ir0MLiIUoNSO"
              + "erTlJ3R54v3hXpfg3UdNGmrHPMsToOc9+KrQeBdAd2SS7vkYdPnX/wCIqy3g"
              + "fRoThb+9GP8AaQ/+yVdavRqxs2yYU5wdzH164t57+V7XHlZOCOh5rn5n5Ndr"
              + "J4U0xBt/tC8x16p/8RTP+EK0uRQ32q9IPfzE/wDiKuGKpRSSYnSm3c5/wjP5"
              + "XiKFywUDqScelevR6/pCR4k1S0DDqPOXI/WvPLjwdplpbtPG9zI6so2yspU5"
              + "YDoFHrW7pOmRWpEiqVyuPlHH41xYmPgpPmibU4uKszqY9f0aZtseq2bN6eeu"
              + "f51fVldQyMGU9CDkGueMCOpxtYe3NYWoabJJOZrWV7V1yFe1cxnH1HX8a57G"
              + "lz0CiuFg8U6rpjql2PtkXTMgCv8Agw4/MfjXU6VrthrCH7NKRIoy8LjDr+Hp"
              + "7jIosFzRooopDCiiigClqXKRD1c/+gtWPLFitnUfuRH/AG//AGUiqDKGFAzA"
              + "mFzG7rGhAJJyB604XtzGGjeAuD3GR2x260t1fTJf3EChAsLKo+XOcorZP/fV"
              + "Qm8lPXZ/3yKLCJDPIBsjUjIHUewFWoYWS2jDDBArPN1IQR8o/wCAirdvcERw"
              + "u33ZYkdgOgLKCcfnRYBL0f8AEvmz6x/+jUqxbzyCLCKoC92NRaiMWLkHhmj/"
              + "AB+dTWlarutdoIXI64oQETXJ25MbA9DsolCEY4zjpVswL5QXgMB1xWZMRDIy"
              + "woJn6HnAWmIzb+GZyyIisD0Vl4Nc5I0ulXSO8zQtnMTrkGM/71dVNpufnMkm"
              + "7Odwfmqd5CGi8tuQB0b5v51SEzf8M+Kl1QixvdqXoHysOFmA7j0PqPxHoOlr"
              + "xm5DROrRkxvGQVZOCpHQivSPCfiEa9px80gXlvhZ1Hf0Yex/mDRKNtQTN6ii"
              + "ioKKmoD9zGfSVf1OP61QrUuovOgZM4PBB9CDkH865PxJrVx4d057yS0inAcI"
              + "qq5TJJ+hoAqXg/4m9/8A9dU/9FR1CRUGlan/AG7Fcal9n+z+dNjy9+/btRV6"
              + "4GemelWiKoRHVuIf6Ha/9e0X/oC1VIrIt/FtxNGsFtoxk+zoITI1zhWKjbn7"
              + "vt0zSYzfvJSLAIennIB+p/pWtZy4hXmuQa71fUzGhtre1jR920MXLHGMk+2T"
              + "6da1oZb22ULM6FDwGCkHNFhGzNqCbjAmXkPGF7VJFAkEQHBNQWkMUcQIwc85"
              + "9fepJJeOtNIVyK4bg1jXjda0LiTg1k3b9atEsxr3qai0LWDoOvQXpYiEny5x"
              + "6oep/Dg/hT7s9ayLoZVs1VriPeQcjIorC8F37ah4Vs3kbdLCvkyexXjn3xg/"
              + "jRWBqb1ee/FqYLpun24P+snLMPoOK9Crzb4uKQumSfwl2H44qo7iexB4JQHR"
              + "wCMj7Q39K2roRRXTkqPLU8iuB8NFl8S6ftdgrSHIycH5T2rvb8b2uap6MS1R"
              + "DJcWsq7YUCt65J4rA0FUCHKjh2/9CNYfjDUSkS2lskrlHDTSR/8ALPjocUaH"
              + "4msUtVjCM0qLyqkAH8TyKSBnoELoOgFJf3VoITFcTBNy546gevtzXM6Rqd3f"
              + "akWadyigsYw3y+gAH410O/PDZHHcU7E3LWnXED2SC2nE0ajG4HJ/GpJJeOtY"
              + "lxYpJdxXMcssLxkE+SwXdj14qrf6jrP22SK0t4RFkFZZMYx/n8aANieXg1mX"
              + "MnWla5lKRCSMFmB8xoz8qHHvzg1TnlzVIRTuW61l3MyQRSTuNwjXdt9T2FXZ"
              + "3zWfcRpPG8Mn3JBtJ9PQ/nVCO7+Dl49z4f1BJG3Mt6XJ/wB5V/woqD4LwvDp"
              + "urK45W5VT9QtFYPc1Wx6VXDfFi0M3hmG6A5trhSfoRj/AAruaz9d01dY0O80"
              + "9v8AlvEVX2bt+uKFowZ4z4Zbd4h04/8ATRv/AEBq7+T53ufrXnXhffF4ls4J"
              + "QVkildWB7EKwNehwnfLcD/aFXLcUThGs5bDUpYrgFWZ2dGHR1J6ip57C0voR"
              + "HcQq6htwI+Ug4xkEVo+MJ0jksrYA+ZuMhPouMfzP6VnQyEqKqOqJejMmfSr/"
              + "AE8NNbzC6hjG7aQVlA/kcVueFtR+12Mrpceagk4Abdt4/rUiMeOeaqS6UhuV"
              + "ubSaWymxtdrbC+YvoR6+9FhHRPcCNGaRgiqNzFjgAVmya/pYkZftSnH8QQlT"
              + "9D3rmdbefT4o7KO6ne3ky+yV9xBB9euO+Kw2mfruNAj0OW8t/K837TB5eM7/"
              + "ADBjFVp3IAOQQwyCDkEexrz95W9vrirula1LYyiOQu9s2Q0YPT3GehpgdJK2"
              + "apzt8hottQgv1k8pZEeMZIYg5GcZyKjuFkkKQxKXklYKijqSeAKdwPUvhdbe"
              + "X4ZmutpH2y7eUZ9AAv8ANTRXSaJpqaPotnpyYP2eIKxHdv4j+JyaKwe5qi/R"
              + "RRSGeaeJ/Dx03x7Yavbr/o967eZj+GQI38xz+dXdPffczD1cVu+Lzi0tT6Tj"
              + "/wBBNc3oz7r5x6uKroIwPFt5BeazFbQKGa1BWSQdycfL+GP1qCCPgU3+yprC"
              + "/ltrpcSqxOezAnhge9aMNv04rRaIh7jEiNSSFLeF55c7I1LNjrgVcjt/alu9"
              + "O+12U1uSUEqFNyjlcjrRcVjzzVL6bU5xJIioqjCIo4Uf1NUDEfStiXTLizuH"
              + "tboL5qc7l6OvZhTTZ+1K4WMZoSe1NFuSelbP2M9xT47As2FXJp3FYqaUptJ2"
              + "fZu3IVwSR1r0L4f6FHqmsf2w8bC3sjhA2CGlx29duc/UisHQ/DlzreoiwtPl"
              + "24NxPjKwL/Vj2H9K9m07T7bStPhsbOPy4IV2qO/uT6knkmoky4otUUUVBYUU"
              + "UUAc54yP+hWv/XwB+hrntOTyNbSP+84rs9Z04ajaqu7a8bh0OMjI9a5O6gvo"
              + "b3zjZAuDwyOMfryKaegEHjZTYva6qV8yFf3MqKMsueQw9uDmq+nPZahGJLSd"
              + "Hz2zyKsXEOpagyC4IWOM5WNOmfUnuaxtT8NOJPtFoGt5uu6PjP4U0xNHSx2b"
              + "jtmrC2p/u1xEOr+J9MO1sXCj++OavxeO9Tj4m0vJ9jTuI09c8NrqUavGfKuI"
              + "/wDVyhc49iO4rnToV/A/lXCI5xkPGCFI/HvWhL491BhiLSufc1l6hrXiTVZQ"
              + "IALaPGOFyfc0gHzadBZxGW9nSFBydxp+nafPrEii0RrOyP3rmRfncf7Cn+Z/"
              + "Wo9K8NXEl0Lm+L3EgOQZTux9K7W0s5BgYNFwsbOg2tlpVilnYxCOMHJ7s7Hq"
              + "xPc1sqcjNZNnbsuM1qoMLUlD6KKKACiiigBCM1E9tG55UVNRQBWNlF2UVC+m"
              + "xP8Aw1fooAx5NDgfqg/KoG8N2p/5ZL+Vb9FAHPjw1ag/6pfyqZNBt06IPyra"
              + "ooAzU0qJOiirCWcadBVqigBixhegp1LRQAUUUUAf/9mJATgEEwECACIFAlOk"
              + "xL4CGwMGCwkIBwMCBhUIAgkKCwQWAgMBAh4BAheAAAoJEEKNuDRspWAJhi8I"
              + "AKhIemGlaKtuZxBA4bQcJOy/ZdGJriJuu3OQl2m6CAwxaGMncpxHFVTT6GqI"
              + "Vu4/b4SSwYP1pI24MqAkdEudjFSi15ByogPFpUoDJC44zrO64b/mv3L5iq1C"
              + "PY+VvgLMAdvA3Tsoj/rNYlD0fieBa9EF8BtoAkaA4X6pihNPGsVe0AxlJhQw"
              + "eMgLXwTjllJm1iWa/fEQvv5Uk01gzayH1TIwkNAJ0E8s6Ontu2szUHjFGRNA"
              + "llR5OJzt/loo9p53zWddFfxlCfn2w+smHyB4i+FfpQfFSMLnwew7wncHs6XE"
              + "PevLPcW66T3w2/oMd0fC7GwhnCiebDYjl8ymF+4b0N65AQ0EU6S6gwEIAOAC"
              + "NRzXH0dc5wwkucFdTMs1nxr16y+Kk3zF3R21OkHLHazXVC7ZP2HurTFGd5VP"
              + "Yd+vv0CrYHCjjMu0lIeMfTlpJswvJRBxVw8vIVLpOSqxtJS+zysE8/LpKw6i"
              + "ti51ydalhm6VYGPm+OAoAAO1pLriwR132caoye5vqxGKEUCmkaNLl8LCljyH"
              + "kMgL5nQr+7cerTcGd2MaC8Y5vQuZBpVVBZcVt004iP3bCJu2l2RKskIoSysC"
              + "68bqV4XLMnoVeM97VPdwdb0Y7tGXCW8YodN8ni43YOaQxfr7fHx8nyzQ5S8w"
              + "a701GKWcQqCb0DR1ngCRAgWLzj8HDlZoofPL8d0AEQEAAYkBHwQYAQIACQUC"
              + "U6S6gwIbDAAKCRBCjbg0bKVgCWPSB/wN9Z5ayWiox5xxouAQv4W2JZGPiqk8"
              + "nFF5fzSgQxV4Xo63IaC1bD8411pgRlj1aWtt8pvWjEW9WWxvyPnkz0xldErb"
              + "NRZ9482TknY0dsrbmg6jwLOlNvLhLVhWUWt+DkH20daVCADV/0p2/2OPodn+"
              + "MYnueL5ljoJxzTO84WMz1u7qumMdX4EcLAFblelmPsGiNsnGabc148+TgYZI"
              + "1fBucn5Xrk4fxVCuqa8QjOa37aHHT5Li/xGIDCbtCqPPIi7M7O1yq8gXLWP9"
              + "TV7nsu99t4EiZT4zov9rCS+tgvBiFrRqsHL37PGrS27s+gMw3GR7F6BiDiqa"
              + "0GvLdt0Lx24c"
            );

        private static readonly byte[] attrLongLength = Base64.Decode(
                "mQENBEGz0vIBCADLb2Sb5QbOhRIzfOg3u9F338gK1XZWJG8JwXP8DSGbQEof"
              + "0+YoT/7bA+3h1ljh3LG0m8JUEdolrxLz/8Mguu2TA2UQiMwRaRChSVvBgkCR"
              + "Ykr97+kClNgmi+PLuUN1z4tspqdE761nRVvUl2x4XvLTJ21hU5eXGGsC+qFP"
              + "4Efe8B5kH+FexAfnFPPzou3GjbDbYv4CYi0pyhTxmauxyJyQrQ/MQUt0RFRk"
              + "L8qCzWCR2BmH3jM3M0Wt0oKn8C8+fWItUh5U9fzv/K9GeO/SV8+zdL4MrdqD"
              + "stgqXNs27H+WeIgbXlUGIs0mONE6TtKZ5PXG5zFM1bz1vDdAYbY4eUWDABEB"
              + "AAGJAhwEHwEIAAYFAlLd55oACgkQ5ppjUk3RnxANSBAAqzYF/9hu7x7wtmi7"
              + "ScmIal6bXP14ZJaRVibMnAPEPIHAULPVa8x9QX/fGW8px5tK9YU41wigLXe6"
              + "3eC5MOLc+wkouELsBeeA3zap51/5HhsuHq5AYtL2tigce9epYUVNV9LaZd2U"
              + "vQOQ6RqyTMhSADN9mD0kR+Nu1+ns7Ur7qAq6UI39hFIGKPoZQ61pTrVsi8N7"
              + "GxHoNwa1FAxm0Dm4XvyiJHPOYs0K4OnNWLKLCcSVOx453Zj3JnllRrCFLpIt"
              + "H27jAxcbGStxWpJvlVMSylcP/x0ATjGfp+kSv2TpU2wK0W5iUtrn30W+WZp4"
              + "+BIXL0NSi4XPksoUoM9dOTsOCPh/ntiWJBlzIdhQuxgcwymoYnaAG0ermI+R"
              + "djB0gCj0AfMDZEOW+thFKg1kEkYrUnAISNDt+VZNUtk26tJ7PDitC9EY6IA6"
              + "vbKeh47LmqpyK3gqQiIA/XuWhdUOr1Wv3H8qxumFjxQQh9sr72IbWFJ+tSNl"
              + "UtrohK7N6CoJQidkj2qFsuGLcFKypAdS7Y0s0t9uOYJLwj1c+2KG0mrA2PvW"
              + "1vng9mMN6AHIx9oRSwQc1+OV29ws2hfNB3JQnpdzBYAy8C5haUWG7E7WFg+j"
              + "pNpeREVX0S+1ibmWDVs+trSQI8hd58j91Kc2YvwE13YigC9nlU2R853Gsox4"
              + "oazn75iJAhwEHwEIAAYFAlMkBMIACgkQcssEwQwvQ5L2yxAAmND9w3OZsJpF"
              + "tTAJFpfg8Bacy0Xs/+LipA1hB8vG+mvaiedcqc5KTpuFQ4bffH1swMRjXAM7"
              + "ZP/u/6qX2LL9kjxCtwDUjDT8YcphTnRxSu5Jv3w4Rf0zWIRWHhnbswiBuGwE"
              + "zQN8V20AYxfZ+ffkR0wymm/y8qLQ1oNynweijXHSlaG/sVmvDxkuc77n4hLi"
              + "4UVQiSAP7dRIkcOh6QCBW4TxoZkDfxIhASFQWl1paCagO1rwyo7YY42O4c16"
              + "+UZBMZtWTvRO2rThz1g9SxAyx8FZ7SxMv140C7VGQmdag97dA1WgBOCuLzLi"
              + "cYT+o/bL9vpFXSI7LVflQEqauzL4fs2X8ggckoI4lkjcDe8DhiDmCoju5Lat"
              + "Q/7DqV8T6z/Gv0sK2hqKr4ULC3By4N11WDCg6wXa72tMQoFBT1vOC+UzLHOj"
              + "vgWBJKE7q3E7kFfq22D0ZX0BPTYy2mcrghMzvvOe74Dx495zlUJhtBfr8MC2"
              + "uPnjsv6PjCYAaomQcvvI0o/5k8JIFi1P0pwLM5VjfujdAuCpAwQuy9AeGlz2"
              + "TEuZZlWBZuyBqZ7JyHx5xz1aVXbY7kofqO+njyyZ+MakZRLYpBI+B/8KomQP"
              + "pqWVARw4uPAXVTd1fjW2CTQtt7Ia6BRWMSblxTv3VWosTSgPnCXmzYEpGvCL"
              + "bIauL8UEhzS0JVBHUCBHbG9iYWwgRGlyZWN0b3J5IFZlcmlmaWNhdGlvbiBL"
              + "ZXmJAV4EEAECAEAFAkJRtHAHCwkIBwMCCgIZARkYbGRhcDovL2tleXNlcnZl"
              + "ci5wZ3AuY29tBRsDAAAAAxYCAQUeAQAAAAQVCAIKABIJEJcQuJvKV618B2VH"
              + "UEcAAQH35ggAnVHdAh2KqrvwSnPos73YdlVbeF9Lcbxs4oYPDCk6AHiDpjr2"
              + "nxu48i1BiLea7aTEEwwAkcIa/3lCLP02NjGXq5gRnWpW/d0xtsaDDj8yYWus"
              + "WGhEJsUlrq5Cz2KjwxNQHXRhHXEDR8vq9uzw5EjCB0u69vlwNmo8+fa17YMN"
              + "VdXaXsmXJlJciVHazdvGoscTzZOuKDHdaJmY8nJcCydk4qsFOiGOcFm5UOKP"
              + "nzdBh31NKglqw/xh+1nTA2z5orsY4jVFIB6sWqutIcVQYt/J78diAKFemkEO"
              + "Qe0kU5JZrY34E8pp4BmS6mfPyr8NtHFfMOAE4m8acFeaZK1X6+uW57QpRE5S"
              + "IEtTMSA8ZG8tbm90LXJlcGx5QGtleXNlcnZlcjEucGdwLmNvbT6JAVMEEAEC"
              + "AD0FAkmgVoIHCwkIBwMCChkYbGRhcDovL2tleXNlcnZlci5wZ3AuY29tBRsD"
              + "AAAAAxYCAQUeAQAAAAQVCAIKAAoJEJcQuJvKV618t6wH/1RFTp9Z7QUZFR5h"
              + "r8eHFWhPoeTCMXF3Vikgw2mZsjN43ZyzpxrIdUwwHROQXn1BzAvOS0rGNiDs"
              + "fOOmQFulz+Oc14xxGox2TZbdnDnXEb8ReZnimQCWYERfpRtY6GSY7uWzNjG2"
              + "dLB1y3XfsOBG+QqTULSJSZqRYD+2IpwPlAdl6qncqRvFzGcPXPIp0RS6nvoP"
              + "Jfe0u2sETDRAUDwivr7ZU/xCA12txELhcsvMQP0fy0CRNgN+pQ2b6iBL2x1l"
              + "jHgSG1r3g3gQjHEk3UCTEKHq9+mFhd/Gi0RXz6i1AmrvW4pKhbtN76WrXeF+"
              + "FXTsB09f1xKnWi4c303Ms1tIJQC0KUROUi1LUzIgPGRvLW5vdC1yZXBseUBr"
              + "ZXlzZXJ2ZXIyLnBncC5jb20+iQFTBBABAgA9BQJJoFabBwsJCAcDAgoZGGxk"
              + "YXA6Ly9rZXlzZXJ2ZXIucGdwLmNvbQUbAwAAAAMWAgEFHgEAAAAEFQgCCgAK"
              + "CRCXELibyletfBwzB/41/OkBDVLgEYnGJ78rKHLtgMdRfrL8gmZn9KhMi44H"
              + "nlFl1NAgi1yuWA2wC8DziVKIiu8YCaCVP0FFXuBK1BF8uZDRp8lZuT3Isf0/"
              + "4DX4yuvZwY5nmtDu3qXrjZ7bZi1W2A8c9Hgc+5A30R9PtiYy5Lz2m8xZl4P6"
              + "wDrYCQA2RLfzGC887bIPBK/tvXTRUFZfj2X1o/q4pr8z4NJTaFUl/XrseGcJ"
              + "R2PP3S2/fU5LErqLJhlj690xofRkf9oYrUiyyb1/UbWmNJsOHSHyy8FEc9lv"
              + "lSJIa39niSQKK6I0Mh1LheXNL7aG152KkXiH0mi6bH4EOzaTR7dfLey3o9Ph"
              + "0cye/wAADVkBEAABAQAAAAAAAAAAAAAAAP/Y/+AAEEpGSUYAAQEAAAEAAQAA"
              + "/9sAQwAKBwcIBwYKCAgICwoKCw4YEA4NDQ4dFRYRGCMfJSQiHyIhJis3LyYp"
              + "NCkhIjBBMTQ5Oz4+PiUuRElDPEg3PT47/9sAQwEKCwsODQ4cEBAcOygiKDs7"
              + "Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7"
              + "Ozs7/8AAEQgAkAB4AwEiAAIRAQMRAf/EAB8AAAEFAQEBAQEBAAAAAAAAAAAB"
              + "AgMEBQYHCAkKC//EALUQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNR"
              + "YQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNE"
              + "RUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeY"
              + "mZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn"
              + "6Onq8fLz9PX29/j5+v/EAB8BAAMBAQEBAQEBAQEAAAAAAAABAgMEBQYHCAkK"
              + "C//EALURAAIBAgQEAwQHBQQEAAECdwABAgMRBAUhMQYSQVEHYXETIjKBCBRC"
              + "kaGxwQkjM1LwFWJy0QoWJDThJfEXGBkaJicoKSo1Njc4OTpDREVGR0hJSlNU"
              + "VVZXWFlaY2RlZmdoaWpzdHV2d3h5eoKDhIWGh4iJipKTlJWWl5iZmqKjpKWm"
              + "p6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uLj5OXm5+jp6vLz9PX2"
              + "9/j5+v/aAAwDAQACEQMRAD8A9moqtf30Gm2cl3cvtijGSa4a++LNlGStlZvK"
              + "R0ZuBWkKU6nwomU4x3PQqK8guPinrEzYhhihX86ns/Ffia/XzElJUHOV4/rW"
              + "/wBUqJXlZEe2i9j1iivMP+Ex1q3+/KCw6gip4PiXdREC5tUkHcrwaTwlVbK4"
              + "e1iekUVzmheNdO1ycWyK8U5Gdrf410QOa55RcXaSNE09ULRRRUjCiiigAooo"
              + "oAKKKKAOY+IblfCN1g9cA/rXh1fQPiXT4dU0o2dwXEcrclCARgE8ZB9K4J/A"
              + "GkKeJr38ZU/+Ir0MLiIUoNSOerTlJ3R54v3hXpfg3UdNGmrHPMsToOc9+KrQ"
              + "eBdAd2SS7vkYdPnX/wCIqy3gfRoThb+9GP8AaQ/+yVdavRqxs2yYU5wdzH16"
              + "4t57+V7XHlZOCOh5rn5n5NdrJ4U0xBt/tC8x16p/8RTP+EK0uRQ32q9IPfzE"
              + "/wDiKuGKpRSSYnSm3c5/wjP5XiKFywUDqScelevR6/pCR4k1S0DDqPOXI/Wv"
              + "PLjwdplpbtPG9zI6so2yspU5YDoFHrW7pOmRWpEiqVyuPlHH41xYmPgpPmib"
              + "U4uKszqY9f0aZtseq2bN6eeuf51fVldQyMGU9CDkGueMCOpxtYe3NYWoabJJ"
              + "OZrWV7V1yFe1cxnH1HX8a57Glz0CiuFg8U6rpjql2PtkXTMgCv8Agw4/MfjX"
              + "U6VrthrCH7NKRIoy8LjDr+Hp7jIosFzRooopDCiiigClqXKRD1c/+gtWPLFi"
              + "tnUfuRH/AG//AGUiqDKGFAzAmFzG7rGhAJJyB604XtzGGjeAuD3GR2x260t1"
              + "fTJf3EChAsLKo+XOcorZP/fVQm8lPXZ/3yKLCJDPIBsjUjIHUewFWoYWS2jD"
              + "DBArPN1IQR8o/wCAirdvcERwu33ZYkdgOgLKCcfnRYBL0f8AEvmz6x/+jUqx"
              + "bzyCLCKoC92NRaiMWLkHhmj/AB+dTWlarutdoIXI64oQETXJ25MbA9DsolCE"
              + "Y4zjpVswL5QXgMB1xWZMRDIywoJn6HnAWmIzb+GZyyIisD0Vl4Nc5I0ulXSO"
              + "8zQtnMTrkGM/71dVNpufnMkm7Odwfmqd5CGi8tuQB0b5v51SEzf8M+Kl1Qix"
              + "vdqXoHysOFmA7j0PqPxHoOlrxm5DROrRkxvGQVZOCpHQivSPCfiEa9px80gX"
              + "lvhZ1Hf0Yex/mDRKNtQTN6iiioKKmoD9zGfSVf1OP61QrUuovOgZM4PBB9CD"
              + "kH865PxJrVx4d057yS0inAcIqq5TJJ+hoAqXg/4m9/8A9dU/9FR1CRUGlan/"
              + "AG7Fcal9n+z+dNjy9+/btRV64GemelWiKoRHVuIf6Ha/9e0X/oC1VIrIt/Ft"
              + "xNGsFtoxk+zoITI1zhWKjbn7vt0zSYzfvJSLAIennIB+p/pWtZy4hXmuQa71"
              + "fUzGhtre1jR920MXLHGMk+2T6da1oZb22ULM6FDwGCkHNFhGzNqCbjAmXkPG"
              + "F7VJFAkEQHBNQWkMUcQIwc859fepJJeOtNIVyK4bg1jXjda0LiTg1k3b9atE"
              + "sxr3qai0LWDoOvQXpYiEny5x6oep/Dg/hT7s9ayLoZVs1VriPeQcjIorC8F3"
              + "7ah4Vs3kbdLCvkyexXjn3xg/jRWBqb1ee/FqYLpun24P+snLMPoOK9Crzb4u"
              + "KQumSfwl2H44qo7iexB4JQHRwCMj7Q39K2roRRXTkqPLU8iuB8NFl8S6ftdg"
              + "rSHIycH5T2rvb8b2uap6MS1RDJcWsq7YUCt65J4rA0FUCHKjh2/9CNYfjDUS"
              + "kS2lskrlHDTSR/8ALPjocUaH4msUtVjCM0qLyqkAH8TyKSBnoELoOgFJf3Vo"
              + "ITFcTBNy546gevtzXM6Rqd3fakWadyigsYw3y+gAH410O/PDZHHcU7E3LWnX"
              + "ED2SC2nE0ajG4HJ/GpJJeOtYlxYpJdxXMcssLxkE+SwXdj14qrf6jrP22SK0"
              + "t4RFkFZZMYx/n8aANieXg1mXMnWla5lKRCSMFmB8xoz8qHHvzg1TnlzVIRTu"
              + "W61l3MyQRSTuNwjXdt9T2FXZ3zWfcRpPG8Mn3JBtJ9PQ/nVCO7+Dl49z4f1B"
              + "JG3Mt6XJ/wB5V/woqD4LwvDpurK45W5VT9QtFYPc1Wx6VXDfFi0M3hmG6A5t"
              + "rhSfoRj/AAruaz9d01dY0O809v8AlvEVX2bt+uKFowZ4z4Zbd4h04/8ATRv/"
              + "AEBq7+T53ufrXnXhffF4ls4JQVkildWB7EKwNehwnfLcD/aFXLcUThGs5bDU"
              + "pYrgFWZ2dGHR1J6ip57C0voRHcQq6htwI+Ug4xkEVo+MJ0jksrYA+ZuMhPou"
              + "MfzP6VnQyEqKqOqJejMmfSr/AE8NNbzC6hjG7aQVlA/kcVueFtR+12Mrpcea"
              + "gk4Abdt4/rUiMeOeaqS6UhuVubSaWymxtdrbC+YvoR6+9FhHRPcCNGaRgiqN"
              + "zFjgAVmya/pYkZftSnH8QQlT9D3rmdbefT4o7KO6ne3ky+yV9xBB9euO+Kw2"
              + "mfruNAj0OW8t/K837TB5eM7/ADBjFVp3IAOQQwyCDkEexrz95W9vrirula1L"
              + "YyiOQu9s2Q0YPT3GehpgdJK2apzt8hottQgv1k8pZEeMZIYg5GcZyKjuFkkK"
              + "QxKXklYKijqSeAKdwPUvhdbeX4ZmutpH2y7eUZ9AAv8ANTRXSaJpqaPotnpy"
              + "YP2eIKxHdv4j+JyaKwe5qi/RRRSGeaeJ/Dx03x7Yavbr/o967eZj+GQI38xz"
              + "+dXdPffczD1cVu+Lzi0tT6Tj/wBBNc3oz7r5x6uKroIwPFt5BeazFbQKGa1B"
              + "WSQdycfL+GP1qCCPgU3+yprC/ltrpcSqxOezAnhge9aMNv04rRaIh7jEiNSS"
              + "FLeF55c7I1LNjrgVcjt/alu9O+12U1uSUEqFNyjlcjrRcVjzzVL6bU5xJIio"
              + "qjCIo4Uf1NUDEfStiXTLizuHtboL5qc7l6OvZhTTZ+1K4WMZoSe1NFuSelbP"
              + "2M9xT47As2FXJp3FYqaUptJ2fZu3IVwSR1r0L4f6FHqmsf2w8bC3sjhA2CGl"
              + "x29duc/UisHQ/DlzreoiwtPl24NxPjKwL/Vj2H9K9m07T7bStPhsbOPy4IV2"
              + "qO/uT6knkmoky4otUUUVBYUUUUAc54yP+hWv/XwB+hrntOTyNbSP+84rs9Z0"
              + "4ajaqu7a8bh0OMjI9a5O6gvob3zjZAuDwyOMfryKaegEHjZTYva6qV8yFf3M"
              + "qKMsueQw9uDmq+nPZahGJLSdHz2zyKsXEOpagyC4IWOM5WNOmfUnuaxtT8NO"
              + "JPtFoGt5uu6PjP4U0xNHSx2bjtmrC2p/u1xEOr+J9MO1sXCj++OavxeO9Tj4"
              + "m0vJ9jTuI09c8NrqUavGfKuI/wDVyhc49iO4rnToV/A/lXCI5xkPGCFI/HvW"
              + "hL491BhiLSufc1l6hrXiTVZQIALaPGOFyfc0gHzadBZxGW9nSFBydxp+nafP"
              + "rEii0RrOyP3rmRfncf7Cn+Z/Wo9K8NXEl0Lm+L3EgOQZTux9K7W0s5BgYNFw"
              + "sbOg2tlpVilnYxCOMHJ7s7HqxPc1sqcjNZNnbsuM1qoMLUlD6KKKACiiigBC"
              + "M1E9tG55UVNRQBWNlF2UVC+mxP8Aw1fooAx5NDgfqg/KoG8N2p/5ZL+Vb9FA"
              + "HPjw1ag/6pfyqZNBt06IPyraooAzU0qJOiirCWcadBVqigBixhegp1LRQAUU"
              + "UUAf/9mJAVYEEAECADgFAkJRtHAHCwkIBwMCChkYbGRhcDovL2tleXNlcnZl"
              + "ci5wZ3AuY29tBRsDAAAAAxYCAQUeAQAAAAASCRCXELibyletfAdlR1BHAAEB"
              + "SBIH/j+RGcMuHmVoZq4+XbmCunnbft4T0Ta4o6mxNkc6wk5P9PpcE9ixztjV"
              + "ysMmv2i4Y746dCY9B1tfhQW10S39HzrYHh3I4a2wb9zQniZCf1XnbCe1eRss"
              + "NhTpLVXXnXKEsc9EwD5MtiPICluZIXB08Zx2uJSZ+/i9TqSM5EUuJk+lXqgX"
              + "GUiTaSXN63I/4BnbFzCw8SaST7d7nok45UC9I/+gcKVO+oYETgrsU7AL6uk1"
              + "6YD9JpfYZHEFmpYoS+qQ3tLfPCG3gaS/djBZWWkNt5z7e6sbRko49XEj3EUh"
              + "33HgjrOlL8uJNbhlZ5NeILcxHqGTHji+5wMEDBjfNT/C6m0=");

        private void DoTestRemoveSignature()
        {
            byte[] testPubKeyRing =
                Base64.Decode(
                    "mQGiBEAR8jYRBADNifuSopd20JOQ5x30ljIaY0M6927+vo09NeNxS3KqItba"
                        + "nz9o5e2aqdT0W1xgdHYZmdElOHTTsugZxdXTEhghyxoo3KhVcNnTABQyrrvX"
                        + "qouvmP2fEDEw0Vpyk+90BpyY9YlgeX/dEA8OfooRLCJde/iDTl7r9FT+mts8"
                        + "g3azjwCgx+pOLD9LPBF5E4FhUOdXISJ0f4EEAKXSOi9nZzajpdhe8W2ZL9gc"
                        + "BpzZi6AcrRZBHOEMqd69gtUxA4eD8xycUQ42yH89imEcwLz8XdJ98uHUxGJi"
                        + "qp6hq4oakmw8GQfiL7yQIFgaM0dOAI9Afe3m84cEYZsoAFYpB4/s9pVMpPRH"
                        + "NsVspU0qd3NHnSZ0QXs8L8DXGO1uBACjDUj+8GsfDCIP2QF3JC+nPUNa0Y5t"
                        + "wKPKl+T8hX/0FBD7fnNeC6c9j5Ir/Fp/QtdaDAOoBKiyNLh1JaB1NY6US5zc"
                        + "qFks2seZPjXEiE6OIDXYra494mjNKGUobA4hqT2peKWXt/uBcuL1mjKOy8Qf"
                        + "JxgEd0MOcGJO+1PFFZWGzLQ3RXJpYyBILiBFY2hpZG5hICh0ZXN0IGtleSBv"
                        + "bmx5KSA8ZXJpY0Bib3VuY3ljYXN0bGUub3JnPohZBBMRAgAZBQJAEfI2BAsH"
                        + "AwIDFQIDAxYCAQIeAQIXgAAKCRAOtk6iUOgnkDdnAKC/CfLWikSBdbngY6OK"
                        + "5UN3+o7q1ACcDRqjT3yjBU3WmRUNlxBg3tSuljmwAgAAuQENBEAR8jgQBAC2"
                        + "kr57iuOaV7Ga1xcU14MNbKcA0PVembRCjcVjei/3yVfT/fuCVtGHOmYLEBqH"
                        + "bn5aaJ0P/6vMbLCHKuN61NZlts+LEctfwoya43RtcubqMc7eKw4k0JnnoYgB"
                        + "ocLXOtloCb7jfubOsnfORvrUkK0+Ne6anRhFBYfaBmGU75cQgwADBQP/XxR2"
                        + "qGHiwn+0YiMioRDRiIAxp6UiC/JQIri2AKSqAi0zeAMdrRsBN7kyzYVVpWwN"
                        + "5u13gPdQ2HnJ7d4wLWAuizUdKIQxBG8VoCxkbipnwh2RR4xCXFDhJrJFQUm+"
                        + "4nKx9JvAmZTBIlI5Wsi5qxst/9p5MgP3flXsNi1tRbTmRhqIRgQYEQIABgUC"
                        + "QBHyOAAKCRAOtk6iUOgnkBStAJoCZBVM61B1LG2xip294MZecMtCwQCbBbsk"
                        + "JVCXP0/Szm05GB+WN+MOCT2wAgAA");

            PgpObjectFactory pgpFact = new PgpObjectFactory(testPubKeyRing);

            PgpPublicKeyRing pgpPub = (PgpPublicKeyRing)pgpFact.NextPgpObject();

            foreach (PgpSignature sig in pgpPub.GetPublicKey().GetSignatures())
            { 
                if (sig.SignatureType == PgpSignature.PositiveCertification)
                {
                    PgpPublicKey.RemoveCertification(pgpPub.GetPublicKey(), sig);
                }
            }
        }

        public override void PerformTest()
        {
            DoTestRemoveSignature();

            //
            // RSA tests
            //
            PgpSecretKeyRing pgpPriv = new PgpSecretKeyRing(rsaKeyRing);
            PgpSecretKey secretKey = pgpPriv.GetSecretKey();
            PgpPrivateKey pgpPrivKey = secretKey.ExtractPrivateKey(rsaPass);

            try
            {
                doTestSig(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);

                Fail("RSA wrong key test failed.");
            }
            catch (PgpException)
            {
                // expected
            }

            try
            {
                doTestSigV3(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);

                Fail("RSA V3 wrong key test failed.");
            }
            catch (PgpException)
            {
                // expected
            }

            //
            // certifications
            //
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.KeyRevocation, pgpPrivKey);

            PgpSignature sig = sGen.GenerateCertification(secretKey.PublicKey);

            sig.InitVerify(secretKey.PublicKey);

            if (!sig.VerifyCertification(secretKey.PublicKey))
            {
                Fail("revocation verification failed.");
            }

            PgpSecretKeyRing pgpDSAPriv = new PgpSecretKeyRing(dsaKeyRing);
            PgpSecretKey secretDSAKey = pgpDSAPriv.GetSecretKey();
            PgpPrivateKey pgpPrivDSAKey = secretDSAKey.ExtractPrivateKey(dsaPass);

            sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.SubkeyBinding, pgpPrivDSAKey);

            PgpSignatureSubpacketGenerator    unhashedGen = new PgpSignatureSubpacketGenerator();
            PgpSignatureSubpacketGenerator    hashedGen = new PgpSignatureSubpacketGenerator();

            hashedGen.SetSignatureExpirationTime(false, TEST_EXPIRATION_TIME);
            hashedGen.SetSignerUserId(true, TEST_USER_ID);
            hashedGen.SetPreferredCompressionAlgorithms(false, PREFERRED_COMPRESSION_ALGORITHMS);
            hashedGen.SetPreferredHashAlgorithms(false, PREFERRED_HASH_ALGORITHMS);
            hashedGen.SetPreferredSymmetricAlgorithms(false, PREFERRED_SYMMETRIC_ALGORITHMS);

            sGen.SetHashedSubpackets(hashedGen.Generate());
            sGen.SetUnhashedSubpackets(unhashedGen.Generate());

            sig = sGen.GenerateCertification(secretDSAKey.PublicKey, secretKey.PublicKey);

            byte[] sigBytes = sig.GetEncoded();

            PgpObjectFactory f = new PgpObjectFactory(sigBytes);

            sig = ((PgpSignatureList) f.NextPgpObject())[0];

            sig.InitVerify(secretDSAKey.PublicKey);

            if (!sig.VerifyCertification(secretDSAKey.PublicKey, secretKey.PublicKey))
            {
                Fail("subkey binding verification failed.");
            }

            PgpSignatureSubpacketVector hashedPcks = sig.GetHashedSubPackets();
            PgpSignatureSubpacketVector unhashedPcks = sig.GetUnhashedSubPackets();

            if (hashedPcks.Count != 6)
            {
                Fail("wrong number of hashed packets found.");
            }

            if (unhashedPcks.Count != 1)
            {
                Fail("wrong number of unhashed packets found.");
            }

            if (!hashedPcks.GetSignerUserId().Equals(TEST_USER_ID))
            {
                Fail("test userid not matching");
            }

            if (hashedPcks.GetSignatureExpirationTime() != TEST_EXPIRATION_TIME)
            {
                Fail("test signature expiration time not matching");
            }

            if (unhashedPcks.GetIssuerKeyId() != secretDSAKey.KeyId)
            {
                Fail("wrong issuer key ID found in certification");
            }

            int[] prefAlgs = hashedPcks.GetPreferredCompressionAlgorithms();
            preferredAlgorithmCheck("compression", PREFERRED_COMPRESSION_ALGORITHMS, prefAlgs);

            prefAlgs = hashedPcks.GetPreferredHashAlgorithms();
            preferredAlgorithmCheck("hash", PREFERRED_HASH_ALGORITHMS, prefAlgs);

            prefAlgs = hashedPcks.GetPreferredSymmetricAlgorithms();
            preferredAlgorithmCheck("symmetric", PREFERRED_SYMMETRIC_ALGORITHMS, prefAlgs);

            SignatureSubpacketTag[] criticalHashed = hashedPcks.GetCriticalTags();

            if (criticalHashed.Length != 1)
            {
                Fail("wrong number of critical packets found.");
            }

            if (criticalHashed[0] != SignatureSubpacketTag.SignerUserId)
            {
                Fail("wrong critical packet found in tag list.");
            }

            //
            // no packets passed
            //
            sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.SubkeyBinding, pgpPrivDSAKey);

            sGen.SetHashedSubpackets(null);
            sGen.SetUnhashedSubpackets(null);

            sig = sGen.GenerateCertification(TEST_USER_ID, secretKey.PublicKey);

            sig.InitVerify(secretDSAKey.PublicKey);

            if (!sig.VerifyCertification(TEST_USER_ID, secretKey.PublicKey))
            {
                Fail("subkey binding verification failed.");
            }

            hashedPcks = sig.GetHashedSubPackets();

            if (hashedPcks.Count != 1)
            {
                Fail("found wrong number of hashed packets");
            }

            unhashedPcks = sig.GetUnhashedSubPackets();

            if (unhashedPcks.Count != 1)
            {
                Fail("found wrong number of unhashed packets");
            }

            try
            {
                sig.VerifyCertification(secretKey.PublicKey);

                Fail("failed to detect non-key signature.");
            }
            catch (InvalidOperationException)
            {
                // expected
            }

            //
            // override hash packets
            //
            sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.SubkeyBinding, pgpPrivDSAKey);

            hashedGen = new PgpSignatureSubpacketGenerator();

            DateTime creationTime = new DateTime(1973, 7, 27);
            hashedGen.SetSignatureCreationTime(false, creationTime);

            sGen.SetHashedSubpackets(hashedGen.Generate());

            sGen.SetUnhashedSubpackets(null);

            sig = sGen.GenerateCertification(TEST_USER_ID, secretKey.PublicKey);

            sig.InitVerify(secretDSAKey.PublicKey);

            if (!sig.VerifyCertification(TEST_USER_ID, secretKey.PublicKey))
            {
                Fail("subkey binding verification failed.");
            }

            hashedPcks = sig.GetHashedSubPackets();

            if (hashedPcks.Count != 1)
            {
                Fail("found wrong number of hashed packets in override test");
            }

            if (!hashedPcks.HasSubpacket(SignatureSubpacketTag.CreationTime))
            {
                Fail("hasSubpacket test for creation time failed");
            }

            DateTime sigCreationTime = hashedPcks.GetSignatureCreationTime();
            if (!sigCreationTime.Equals(creationTime))
            {
                Fail("creation of overridden date failed.");
            }

            prefAlgs = hashedPcks.GetPreferredCompressionAlgorithms();
            preferredAlgorithmCheck("compression", NO_PREFERENCES, prefAlgs);

            prefAlgs = hashedPcks.GetPreferredHashAlgorithms();
            preferredAlgorithmCheck("hash", NO_PREFERENCES, prefAlgs);

            prefAlgs = hashedPcks.GetPreferredSymmetricAlgorithms();
            preferredAlgorithmCheck("symmetric", NO_PREFERENCES, prefAlgs);

            if (hashedPcks.GetKeyExpirationTime() != 0)
            {
                Fail("unexpected key expiration time found");
            }

            if (hashedPcks.GetSignatureExpirationTime() != 0)
            {
                Fail("unexpected signature expiration time found");
            }

            if (hashedPcks.GetSignerUserId() != null)
            {
                Fail("unexpected signer user ID found");
            }

            criticalHashed = hashedPcks.GetCriticalTags();

            if (criticalHashed.Length != 0)
            {
                Fail("critical packets found when none expected");
            }

            unhashedPcks = sig.GetUnhashedSubPackets();

            if (unhashedPcks.Count != 1)
            {
                Fail("found wrong number of unhashed packets in override test");
            }

            //
            // general signatures
            //
            doTestSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha256, secretKey.PublicKey, pgpPrivKey);
            doTestSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha384, secretKey.PublicKey, pgpPrivKey);
            doTestSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha512, secretKey.PublicKey, pgpPrivKey);
            doTestSigV3(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);
            doTestTextSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA_WITH_CRLF, TEST_DATA_WITH_CRLF);
            doTestTextSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA, TEST_DATA_WITH_CRLF);
            doTestTextSigV3(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA_WITH_CRLF, TEST_DATA_WITH_CRLF);
            doTestTextSigV3(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA, TEST_DATA_WITH_CRLF);

            //
            // DSA Tests
            //
            pgpPriv = new PgpSecretKeyRing(dsaKeyRing);
            secretKey = pgpPriv.GetSecretKey();
            pgpPrivKey = secretKey.ExtractPrivateKey(dsaPass);

            try
            {
                doTestSig(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);

                Fail("DSA wrong key test failed.");
            }
            catch (PgpException)
            {
                // expected
            }

            try
            {
                doTestSigV3(PublicKeyAlgorithmTag.RsaGeneral, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);

                Fail("DSA V3 wrong key test failed.");
            }
            catch (PgpException)
            {
                // expected
            }

            doTestSig(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);
            doTestSigV3(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey);
            doTestTextSig(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA_WITH_CRLF, TEST_DATA_WITH_CRLF);
            doTestTextSig(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA, TEST_DATA_WITH_CRLF);
            doTestTextSigV3(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA_WITH_CRLF, TEST_DATA_WITH_CRLF);
            doTestTextSigV3(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1, secretKey.PublicKey, pgpPrivKey, TEST_DATA, TEST_DATA_WITH_CRLF);

            // special cases
            //
            doTestMissingSubpackets(nullPacketsSubKeyBinding);

            doTestMissingSubpackets(generateV3BinarySig(pgpPrivKey, PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1));

            // keyflags
            doTestKeyFlagsValues();

            // TODO Seems to depend on some other functionality that's yet to be ported
            //doTestUserAttributeEncoding();
        }

        //private void doTestUserAttributeEncoding()
        //{
        //    PgpPublicKeyRing pkr = new PgpPublicKeyRing(okAttr);

        //    CheckUserAttribute("normal", pkr, pkr.GetPublicKey());

        //    pkr = new PgpPublicKeyRing(attrLongLength);

        //    CheckUserAttribute("long", pkr, pkr.GetPublicKey());
        //}

        //private void CheckUserAttribute(String type, PgpPublicKeyRing pkr, PgpPublicKey masterPk)
        //{
        //    foreach (PgpUserAttributeSubpacketVector attr in pkr.GetPublicKey().GetUserAttributes())
        //    {
        //        foreach (PgpSignature sig in masterPk.GetSignaturesForUserAttribute(attr))
        //        {
        //            sig.InitVerify(masterPk);
        //            if (!sig.VerifyCertification(attr, masterPk))
        //            {
        //                Fail("user attribute sig failed to verify on " + type);
        //            }
        //        }
        //    }
        //}

        private void doTestKeyFlagsValues()
        {
            checkValue(KeyFlags.CertifyOther, 0x01);
            checkValue(KeyFlags.SignData, 0x02);
            checkValue(KeyFlags.EncryptComms, 0x04);
            checkValue(KeyFlags.EncryptStorage, 0x08);
            checkValue(KeyFlags.Split, 0x10);
            checkValue(KeyFlags.Authentication, 0x20);
            checkValue(KeyFlags.Shared, 0x80);

            // yes this actually happens
            checkValue(new byte[] { 4, 0, 0, 0 }, 0x04);
            checkValue(new byte[] { 4, 0, 0 }, 0x04);
            checkValue(new byte[] { 4, 0 }, 0x04);
            checkValue(new byte[] { 4 }, 0x04);
        }

        private void checkValue(int flag, int val)
        {
            KeyFlags f = new KeyFlags(true, flag);

            if (f.Flags != val)
            {
                Fail("flag value mismatch");
            }
        }

        private void checkValue(byte[] flag, int val)
        {
            KeyFlags f = new KeyFlags(true, false, flag);

            if (f.Flags != val)
            {
                Fail("flag value mismatch");
            }
        }

        private void doTestMissingSubpackets(byte[] signature)
        {
            PgpObjectFactory f = new PgpObjectFactory(signature);
            object obj = f.NextPgpObject();

            while (!(obj is PgpSignatureList))
            {
                obj = f.NextPgpObject();
                if (obj is PgpLiteralData)
                {
                    Stream input = ((PgpLiteralData)obj).GetDataStream();
                    Streams.Drain(input);
                }
            }

            PgpSignature sig = ((PgpSignatureList)obj)[0];

            if (sig.Version > 3)
            {
                PgpSignatureSubpacketVector v = sig.GetHashedSubPackets();

                if (v.GetKeyExpirationTime() != 0)
                {
                    Fail("key expiration time not zero for missing subpackets");
                }

                if (!sig.HasSubpackets)
                {
                    Fail("HasSubpackets property was false with packets");
                }
            }
            else
            {
                if (sig.GetHashedSubPackets() != null)
                {
                    Fail("hashed sub packets found when none expected");
                }

                if (sig.GetUnhashedSubPackets() != null)
                {
                    Fail("unhashed sub packets found when none expected");
                }

                if (sig.HasSubpackets)
                {
                    Fail("HasSubpackets property was true with no packets");
                }
            }
        }

        private void preferredAlgorithmCheck(
            string	type,
            int[]	expected,
            int[]	prefAlgs)
        {
            if (expected == null)
            {
                if (prefAlgs != null)
                {
                    Fail("preferences for " + type + " found when none expected");
                }
            }
            else
            {
                if (prefAlgs.Length != expected.Length)
                {
                    Fail("wrong number of preferred " + type + " algorithms found");
                }

                for (int i = 0; i != expected.Length; i++)
                {
                    if (expected[i] != prefAlgs[i])
                    {
                        Fail("wrong algorithm found for " + type + ": expected " + expected[i] + " got " + prefAlgs);
                    }
                }
            }
        }

        private void doTestSig(
            PublicKeyAlgorithmTag	encAlgorithm,
            HashAlgorithmTag		hashAlgorithm,
            PgpPublicKey			pubKey,
            PgpPrivateKey			privKey)
        {
            MemoryStream bOut = new MemoryStream();
            MemoryStream testIn = new MemoryStream(TEST_DATA, false);
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(encAlgorithm, hashAlgorithm);

            sGen.InitSign(PgpSignature.BinaryDocument, privKey);
            sGen.GenerateOnePassVersion(false).Encode(bOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(
                new UncloseableStream(bOut),
                PgpLiteralData.Binary,
                "_CONSOLE",
                TEST_DATA.Length * 2,
                DateTime.UtcNow);

            int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Write(TEST_DATA, 0, TEST_DATA.Length);
            sGen.Update(TEST_DATA);

            lGen.Close();

            sGen.Generate().Encode(bOut);

            verifySignature(bOut.ToArray(), hashAlgorithm, pubKey, TEST_DATA);
        }

        private void doTestTextSig(
            PublicKeyAlgorithmTag	encAlgorithm,
            HashAlgorithmTag		hashAlgorithm,
            PgpPublicKey			pubKey,
            PgpPrivateKey			privKey,
            byte[]					data,
            byte[]					canonicalData)
        {
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(encAlgorithm, HashAlgorithmTag.Sha1);
            MemoryStream bOut = new MemoryStream();
            MemoryStream testIn = new MemoryStream(data, false);
            DateTime creationTime = DateTime.UtcNow;

            sGen.InitSign(PgpSignature.CanonicalTextDocument, privKey);
            sGen.GenerateOnePassVersion(false).Encode(bOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(
                new UncloseableStream(bOut),
                PgpLiteralData.Text,
                "_CONSOLE",
                data.Length * 2,
                creationTime);

            int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Write(data, 0, data.Length);
            sGen.Update(data);

            lGen.Close();

            PgpSignature sig = sGen.Generate();

            if (sig.CreationTime == DateTimeUtilities.UnixMsToDateTime(0))
            {
                Fail("creation time not set in v4 signature");
            }

            sig.Encode(bOut);

            verifySignature(bOut.ToArray(), hashAlgorithm, pubKey, canonicalData);
        }

        private void doTestSigV3(
            PublicKeyAlgorithmTag	encAlgorithm,
            HashAlgorithmTag		hashAlgorithm,
            PgpPublicKey			pubKey,
            PgpPrivateKey			privKey)
        {
            byte[] bytes = generateV3BinarySig(privKey, encAlgorithm, hashAlgorithm);

            verifySignature(bytes, hashAlgorithm, pubKey, TEST_DATA);
        }

        private byte[] generateV3BinarySig(
            PgpPrivateKey			privKey,
            PublicKeyAlgorithmTag	encAlgorithm,
            HashAlgorithmTag		hashAlgorithm)
        {
            MemoryStream bOut = new MemoryStream();
            MemoryStream testIn = new MemoryStream(TEST_DATA, false);
            PgpV3SignatureGenerator sGen = new PgpV3SignatureGenerator(encAlgorithm, hashAlgorithm);

            sGen.InitSign(PgpSignature.BinaryDocument, privKey);
            sGen.GenerateOnePassVersion(false).Encode(bOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(
                new UncloseableStream(bOut),
                PgpLiteralData.Binary,
                "_CONSOLE",
                TEST_DATA.Length * 2,
                DateTime.UtcNow);

            int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Write(TEST_DATA, 0, TEST_DATA.Length);
            sGen.Update(TEST_DATA);

            lGen.Close();

            sGen.Generate().Encode(bOut);

            return bOut.ToArray();
        }

        private void doTestTextSigV3(
            PublicKeyAlgorithmTag	encAlgorithm,
            HashAlgorithmTag		hashAlgorithm,
            PgpPublicKey			pubKey,
            PgpPrivateKey			privKey,
            byte[]					data,
            byte[]					canonicalData)
        {
            PgpV3SignatureGenerator sGen = new PgpV3SignatureGenerator(encAlgorithm, HashAlgorithmTag.Sha1);
            MemoryStream bOut = new MemoryStream();
            MemoryStream testIn = new MemoryStream(data, false);

            sGen.InitSign(PgpSignature.CanonicalTextDocument, privKey);
            sGen.GenerateOnePassVersion(false).Encode(bOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
            Stream lOut = lGen.Open(
                new UncloseableStream(bOut),
                PgpLiteralData.Text,
                "_CONSOLE",
                data.Length * 2,
                DateTime.UtcNow);

            int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte)ch);
                sGen.Update((byte)ch);
            }

            lOut.Write(data, 0, data.Length);
            sGen.Update(data);

            lGen.Close();

            PgpSignature sig = sGen.Generate();

            if (sig.CreationTime == DateTimeUtilities.UnixMsToDateTime(0))
            {
                Fail("creation time not set in v3 signature");
            }

            sig.Encode(bOut);

            verifySignature(bOut.ToArray(), hashAlgorithm, pubKey, canonicalData);
        }

        private void verifySignature(
            byte[] encodedSig,
            HashAlgorithmTag hashAlgorithm,
            PgpPublicKey pubKey,
            byte[] original)
        {
            PgpObjectFactory        pgpFact = new PgpObjectFactory(encodedSig);
            PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
            PgpOnePassSignature     ops = p1[0];
            PgpLiteralData          p2 = (PgpLiteralData)pgpFact.NextPgpObject();
            Stream					dIn = p2.GetInputStream();

            ops.InitVerify(pubKey);

            int ch;
            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            PgpSignatureList p3 = (PgpSignatureList)pgpFact.NextPgpObject();
            PgpSignature sig = p3[0];

            DateTime creationTime = sig.CreationTime;

            // Check creationTime is recent
            if (creationTime.CompareTo(DateTime.UtcNow) > 0
                || creationTime.CompareTo(DateTime.UtcNow.AddMinutes(-10)) < 0)
            {
                Fail("bad creation time in signature: " + creationTime);
            }

            if (sig.KeyId != pubKey.KeyId)
            {
                Fail("key id mismatch in signature");
            }

            if (!ops.Verify(sig))
            {
                Fail("Failed generated signature check - " + hashAlgorithm);
            }

            sig.InitVerify(pubKey);

            for (int i = 0; i != original.Length; i++)
            {
                sig.Update(original[i]);
            }

            sig.Update(original);

            if (!sig.Verify())
            {
                Fail("Failed generated signature check against original data");
            }
        }

        public override string Name
        {
            get { return "PGPSignatureTest"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new PgpSignatureTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
