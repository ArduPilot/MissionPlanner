using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
	[TestFixture]
    public class PgpDsaTest
		: SimpleTest
    {
		private static readonly byte[] testPubKey = Base64.Decode(
              "mQGiBD9HBzURBACzkxRCVGJg5+Ld9DU4Xpnd4LCKgMq7YOY7Gi0EgK92gbaa6+zQ"
            + "oQFqz1tt3QUmpz3YVkm/zLESBBtC1ACIXGggUdFMUr5I87+1Cb6vzefAtGt8N5VV"
            + "1F/MXv1gJz4Bu6HyxL/ncfe71jsNhav0i4yAjf2etWFj53zK6R+Ojg5H6wCgpL9/"
            + "tXVfGP8SqFvyrN/437MlFSUEAIN3V6j/MUllyrZglrtr2+RWIwRrG/ACmrF6hTug"
            + "Ol4cQxaDYNcntXbhlTlJs9MxjTH3xxzylyirCyq7HzGJxZzSt6FTeh1DFYzhJ7Qu"
            + "YR1xrSdA6Y0mUv0ixD5A4nPHjupQ5QCqHGeRfFD/oHzD4zqBnJp/BJ3LvQ66bERJ"
            + "mKl5A/4uj3HoVxpb0vvyENfRqKMmGBISycY4MoH5uWfb23FffsT9r9KL6nJ4syLz"
            + "aRR0gvcbcjkc9Z3epI7gr3jTrb4d8WPxsDbT/W1tv9bG/EHawomLcihtuUU68Uej"
            + "6/wZot1XJqu2nQlku57+M/V2X1y26VKsipolPfja4uyBOOyvbLQzRXJpYyBFY2hp"
            + "ZG5hIChEU0EgVGVzdCBLZXkpIDxlcmljQGJvdW5jeWNhc3RsZS5vcmc+iFkEExEC"
            + "ABkFAj9HBzUECwcDAgMVAgMDFgIBAh4BAheAAAoJEM0j9enEyjRDAlwAn2rrom0s"
            + "MhufWK5vIRwg7gj5qsLEAJ4vnT5dPBVblofsG+pDoCVeJXGGng==");

		private static readonly byte[] testPrivKey = Base64.Decode(
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

		private static readonly byte[] testPrivKey2 = Base64.Decode(
              "lQHhBEAnoewRBADRvKgDhbV6pMzqYfUgBsLxSHzmycpuxGbjMrpyKHDOEemj"
            + "iQb6TyyBKUoR28/pfshFP9R5urtKIT7wjVrDuOkxYkgRhNm+xmPXW2Lw3D++"
            + "MQrC5VWe8ywBltz6T9msmChsaKo2hDhIiRI/mg9Q6rH9pJKtVGi4R7CgGxM2"
            + "STQ5fwCgub38qGS1W2O4hUsa+3gva5gaNZUEAItegda4/H4t88XdWxW3D8pv"
            + "RnFz26/ADdImVaQlBoumD15VmcgYoT1Djizey7X8vfV+pntudESzLbn3GHlI"
            + "6C09seH4e8eYP63t7KU/qbUCDomlSswd1OgQ/RxfN86q765K2t3K1i3wDSxe"
            + "EgSRyGKee0VNvOBFOFhuWt+patXaBADE1riNkUxg2P4lBNWwu8tEZRmsl/Ys"
            + "DBIzXBshoMzZCvS5PnNXMW4G3SAaC9OC9jvKSx9IEWhKjfjs3QcWzXR28mcm"
            + "5na0bTxeOMlaPPhBdkTCmFl0IITWlH/pFlR2ah9WYoWYhZEL2tqB82wByzxH"
            + "SkSeD9V5oeSCdCcqiqkEmv4DAwLeNsQ2XGJVRmA4lld+CR5vRxpT/+/2xklp"
            + "lxVf/nx0+thrHDpro3u/nINIIObk0gh59+zaEEe3APlHqbQVYWFhIGJiYiA8"
            + "Y2NjQGRkZC5lZWU+iFoEExECABoFAkAnoewFCwcDAgEDFQIDAxYCAQIeAQIX"
            + "gAAKCRA5nBpCS63az85BAKCbPfU8ATrFvkXhzGNGlc1BJo6DWQCgnK125xVK"
            + "lWLpt6ZJJ7TXcx3nkm6wAgAAnQFXBEAnoe0QBACsQxPvaeBcv2TkbgU/5Wc/"
            + "tO222dPE1mxFbXjGTKfb+6ge96iyD8kTRLrKCkEEeVBa8AZqMSoXUVN6tV8j"
            + "/zD8Bc76o5iJ6wgpg3Mmy2GxInVfsfZN6/G3Y2ukmouz+CDNvQdUw8cTguIb"
            + "QoV3XhQ03MLbfVmNcHsku9F4CuKNWwADBQP0DSSe8v5PXF9CSCXOIxBDcQ5x"
            + "RKjyYOveqoH/4lbOV0YNUbIDZq4RaUdotpADuPREFmWf0zTB6KV/WIiag8XU"
            + "WU9zdDvLKR483Bo6Do5pDBcN+NqfQ+ntGY9WJ7BSFnhQ3+07i1K+NsfFTRfv"
            + "hf9X3MP75rCf7MxAIWHTabEmUf4DAwLeNsQ2XGJVRmA8DssBUCghogG9n8T3"
            + "qfBeKsplGyCcF+JjPeQXkKQaoYGJ0aJz36qFP9d8DuWtT9soQcqIxVf6mTa8"
            + "kN1594hGBBgRAgAGBQJAJ6HtAAoJEDmcGkJLrdrPpMkAnRyjQSKugz0YJqOB"
            + "yGasMLQLxd2OAKCEIlhtCarlufVQNGZsuWxHVbU8crACAAA=");

		private static readonly byte[] sig1 = Base64.Decode(
              "owGbwMvMwCR4VvnryyOnTJwZ10gncZSkFpfolVSU2Ltz78hIzcnJVyjPL8pJUeTq"
            + "sGdmZQCJwpQLMq3ayTA/0Fj3xf4jbwPfK/H3zj55Z9L1n2k/GOapKJrvMZ4tLiCW"
            + "GtP/XeDqX4fORDUA");

		private static readonly byte[] sig1crc = Base64.Decode("OZa/");

		private static readonly byte[] testPubWithUserAttr = Base64.Decode(
              "mQGiBD2Rqv0RBADqKCkhVEtB/lEEr/9CubuHEy2oN/yU5j+2GXSdcNdVnRI/rwFy"
            + "fHEQIk3uU7zHSUKFrC59yDm0sODYyjEdE3BVb0xvEJ5LE/OdndcIMXT1DungZ1vB"
            + "zIK/3lr33W/PHixYxv9jduH3WrTehBpiKkgMZp8XloSFj2Cnw9LDyfqB7QCg/8K1"
            + "o2k75NkOd9ZjnA9ye7Ri3bEEAKyr61Mo7viPWBK1joWAEsxG0OBWM+iSlG7kwh31"
            + "8efgC/7Os6x4Y0jzs8mpcbBjeZtZjS9lRbfp7RinhF269xL0TZ3JxIdtaAV/6yDQ"
            + "9NXfZY9dskN++HIR/5GCEEgq/qTJZt6ti5k7aV19ZFfO6wiK3NUy08wOrVsdOkVE"
            + "w9IcBADaplhpcel3201uU3OCboogJtw81R5MJMZ4Y9cKL/ca2jGISn0nA7KrAw9v"
            + "ShheSixGO4BV9JECkLEbtg7i+W/j/De6S+x2GLNcphuTP3UmgtKbhs0ItRqzW561"
            + "s6gLkqi6aWmgaFLd8E1pMJcd9DSY95P13EYB9VJIUxFNUopzo7QcUmFsZiBIYXVz"
            + "ZXIgPGhhdXNlckBhY20ub3JnPokAWAQQEQIAGAUCPZGq/QgLAwkIBwIBCgIZAQUb"
            + "AwAAAAAKCRAqIBiOh4JvOKg4AJ9j14yygOqqzqiLKeaasIzqT8LCIgCggx14WuLO"
            + "wOUTUswTaVKMFnU7tseJAJwEEAECAAYFAj2Rqx8ACgkQ9aWTKMpUDFV+9QP/RiWT"
            + "5FAF5Rgb7beaApsgXsME+Pw7HEYFtqGa6VcXEpbcUXO6rjaXsgMgY90klWlWCF1T"
            + "HOyKITvj2FdhE+0j8NQn4vaGpiTwORW/zMf/BZ0abdSWQybp10Yjs8gXw30UheO+"
            + "F1E524MC+s2AeUi2hwHMiS+AVYd4WhxWHmWuBpTRypP/AAALTgEQAAEBAAAAAQAA"
            + "AAABAAAA/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAoHBwgHBgoICAgLCgoLDhgQ"
            + "Dg0NDh0VFhEYIx8lJCIfIiEmKzcvJik0KSEiMEExNDk7Pj4+JS5ESUM8SDc9Pjv/"
            + "2wBDAQoLCw4NDhwQEBw7KCIoOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7"
            + "Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozv/wAARCABqAF0DASIAAhEBAxEB/8QAHwAAAQUB"
            + "AQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQID"
            + "AAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0"
            + "NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKT"
            + "lJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl"
            + "5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL"
            + "/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHB"
            + "CSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpj"
            + "ZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3"
            + "uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIR"
            + "AxEAPwD2aiiq9xcxWsRllcKqjOT06E/0oAsVm6jrmm6VGXvLuOPGflz8x+grzXxV"
            + "8U51u5LXRgBGowZHXknnkc9OQcV51caneXdw9xPOXlckl2AJHY4J6cD1oA9J1z4p"
            + "TRkrYQhRyQ0hIY5/2QRx7k9ulczN8SvEEshdZkX0UorDrznI759a5Mksckkknqec"
            + "mkoA7WD4oavEoEttbTepYEZ+mCMVv6H8SLTULhbe/gFozAYkD5Unp3Ax/kV5XRQB"
            + "9EAhgCDkHkEcgilryTwd4zn0m4WzvpTJZSMBuY5MfbueletKyugZWDKwyCOc/j3o"
            + "AduyWLDeWB5Ynj8jSUUUAdFXn/xU15dO0RbGGYC5uWwUB6L1Jx+n413F1cJa2stz"
            + "J92JC5+gGa+bdfvp9S1q4urmRneQg5Yk4HGAPYZoAzySxySSSep5yaSvQvAPhOHU"
            + "rB7u5iLGUlIwQRx7HPr/AJ9LGsfC+dJGngc+X12gc8nvx1/rQB5rRXS3Xg28t9ye"
            + "VLvA7Ddj8MDt6Vnx6JKJCsocnBwqqQSOxPH+fWgDKorTl0SaLGXxkZ+ZcZ4z1yfb"
            + "P1qg0MqLueN1A6kqRigCOvVPh74mF9YjS7tgLi3GIm6b17c+oOfrXlda3haeW38R"
            + "WjxfeMgBOCcD/PHpzQB7nRRRQBqarZjUNLubPJXz4yhI64PFfO3iDRrnRtdm0+cq"
            + "0ocEbehzyOv1xX0vXnHxU8Kf2hYf23aRk3VsMTAZO6MZ5x7UAbfga1W00WzjRSF8"
            + "kbsg5z744HT/ADmuoysikdQSVP8AI1yPgq6il0axk27V8sDcTg5x7V1qSxOcJIrH"
            + "/ZOaAKV5p8JgJSPJGMr97PNcxqOiRXLiRI8nONoIGO55z/8AqyeldhPcQxwyOzoQ"
            + "owRkflXH6t4q0nTLjy57mNXfJCA5x+Qx0NAGXd6LD5iiaPYwTAAx07+vXvXOXmiR"
            + "Qu6u5VTk/MQQV7cdvxPT866KbxTpt7HGR8p7SMw5HuOP8/Ws/ULlb2No0bKMOGBJ"
            + "BHrjHHXn6D8QDzWZQk8iAYVWIA9K6LwDZNeeJ4sEqsaF2YHBHpz2/wA/WsG+V0vZ"
            + "kkGGVsEZz9OcntXffC62iiS7vJTsklKxRFuAw6nBP+eKAPRKKKKAOiqOSNJYzHIo"
            + "ZGGCD0NSUUAeRajIunwzQG4e3tYZTHGsPzOxJ6ADuQcH8Pw5v+19Q0rVJVgl1JG3"
            + "cxykEj13cnHT1r1C38OQ3l063cIkkhmkZDKSeCfx9R/kVLeeGIRKs7hVVDn5OCx9"
            + "yeTjqMf0oAo3k1xP4biuJFeKV4w7gDaQcen1/wAjt5gbK81HW41kIiJBZppULe47"
            + "eoxx+YzivW9Vh/0FAE+XPIJGCOR0rnbPT7eG+LyxlkAG1wQSPXrjvg9MfjQBycNj"
            + "4hMRZgJkUjETQqAy/UAY6DoO/wCNbVlYTNbSNJbmBlBwoUfM30B7j2/lz20VhbKA"
            + "wHmZOQWbOfyrO1G3jil8tBhWToOcdu+c/wAvagDzbUdGlu9aRxFiB/vsuBggZOfq"
            + "cfWujSIR2dnNZTEeXKgMcb4BUHjofbjNKmI5juiabaGGxVJLcdh/nFWtI0oxagsD"
            + "DIkkWXYp4VQDnOemSfyHbigDtgSQMjBI6HqKKKKAOiopoPXjGKdQBnXLiDUI5SMK"
            + "VwxHGf8APFUtW1A+YkMKmbnc23njuf6D/ObWquoaNSQCM/rwP1rMYxxTGWR1UsoU"
            + "biAcdep+o/KgDG1LxdpracIirCVRjaykHr6cHGQe1cv/AGjNcXBW3sntyT/rHcjj"
            + "Hp6Z+nQdAK6PXIdIvcE3Fv5rEfNgP9eRn8c8d/rgzX2i2sqo1y8745CD5WPseOnH"
            + "f8aANiz1O9gjiR5FMUhAV1wcH0Ix6jHHSrMsskz7pGy2MZNc8PEEM7xxWsM/lr8r"
            + "b4jtI9CcHt7nr7Vqi4JuEjB2qse9y2Ace47dRn/OQDMuRMl8RHw7SgDBPGT6jpwf"
            + "yzXa2NmbYF3IMrDB2kkAe3HP5Vwk99u1hdg3ANuOOOB0z6ZwPz6c8eiAhgCDkHkE"
            + "cgigBaKKKAOiqJiMEb9mBknjim3LFIGcOU285ArNa8mKIN3QclScn6+/FADL9xOc"
            + "K2Tj7xAxnAwQPqOmawdSNpeSJBfQyGNXwQpIAPvjqOPyPT12nYsxYnJIGSeMnHP+"
            + "e9UL7TUumEqOYp1GNw6N/vDv/wDXoA5+70vSbFGlhtopUxkBl3EZ45z7/kKwTdpN"
            + "cIsOmeSCduUiCnB9cdeg/M/j0v8AbFtY5hu0gjmGSRICT19cdMDt3+lULzxPZGZv"
            + "LXcBnCrwB6Y4PX+ZoAptMRbiMDAGSSMksf8A9Q6DuKzJtVYs+BvcPgMTkEdOTnrx"
            + "/KoLzVmvZZQjjaT82DyPbqcdx+GKitLf7TNsLYAGWPfH+TQBcsYJDE0rOyu4wjHk"
            + "gfQ+p/zzWjpnja5sdSOm6yyK0Z2pMCQjZ+6SM9CCMdhnp3E1hYy393FaW0eXfjAx"
            + "gAdT26D+X4Vg/EuFLbxOsCYBitkQkEdsgcADsB+lAHplvqUbsu5vlYA5PIB7468e"
            + "nPf8lfUlDkRRrIvqZNn6EV41o3iO/wBFcCJ/MhBP7pjwD6g9ua7G08b6TcRl7h5L"
            + "eTPKvGz5+hUH9cUAeo3uFDrt+Y4O7HOOB69Pr/8AXqhUlx/r2/z2qOgBCQoJJwBy"
            + "SeABXHeIfHVvbXcemaW4luHlVJJlIKxjODgg8nqKq/Em6uItOhWOeVAx5CuRnrXn"
            + "+jf8hyw/6+Y//QhQB6xrmlxzXc0NyuHVyQcdjnBz379D1BGeK5u88LMJGlt2RlX7"
            + "qkEsPXn6/pXo/ilVzbttG7DDOOeornqAONbRpI4v3pKOQcAqQD+Y/P6j052NK0p5"
            + "HWHy3IBPyqrfN6gZz+P4/hpXoGzOOiP/ACNdH4XRftsp2jIBxx70AX9E0pdMtvMm"
            + "VRNt5xyEGOgPf3NeDeLdVOs+J768zlGkKx+yjgfy/WvoPXeNEvMcfujXzJQAUUUU"
            + "Af/ZiQBGBBARAgAGBQI9katEAAoJECogGI6Hgm84xz8AoNGz1fJrVPxqkBrUDmWA"
            + "GsP6qVGYAJ0ZOftw/GfQHzdGR8pOK85DLUPEErQkUmFsZiBIYXVzZXIgPGhhdXNl"
            + "ckBwcml2YXNwaGVyZS5jb20+iQBGBBARAgAGBQI9katmAAoJECogGI6Hgm84m0oA"
            + "oJS3CTrgpqRZfhgPtHGtUVjRCJbbAJ9stJgPcbqA2xXEg9yl2TQToWdWxbQkUmFs"
            + "ZiBIYXVzZXIgPGhhdXNlckBwcml2YXNwaGVyZS5vcmc+iQBGBBARAgAGBQI9kauJ"
            + "AAoJECogGI6Hgm84GfAAnRswktLMzDfIjv6ni76Qp5B850byAJ90I0LEHOLhda7r"
            + "kqTwZ8rguNssUrQkUmFsZiBIYXVzZXIgPGhhdXNlckBwcml2YXNwaGVyZS5uZXQ+"
            + "iQBGBBARAgAGBQI9kaubAAoJECogGI6Hgm84zi0An16C4s/B9Z0/AtfoN4ealMh3"
            + "i3/7AJ9Jg4GOUqGCGRRKUA9Gs5pk8yM8GbQmUmFsZiBDLiBIYXVzZXIgPHJhbGZo"
            + "YXVzZXJAYmx1ZXdpbi5jaD6JAEYEEBECAAYFAj2Rq8oACgkQKiAYjoeCbzhPOACg"
            + "iiTohKuIa66FNiI24mQ+XR9nTisAoLmh3lJf16/06qLPsRd9shTkLfmHtB9SYWxm"
            + "IEhhdXNlciA8cmFsZmhhdXNlckBnbXguY2g+iQBGBBARAgAGBQI9kavvAAoJECog"
            + "GI6Hgm84ZE8An0RlgL8mPBa/P08S5e/lD35MlDdgAJ99pjCeY46S9+nVyx7ACyKO"
            + "SZ4OcLQmUmFsZiBIYXVzZXIgPGhhdXNlci5yYWxmQG15c3VucmlzZS5jaD6JAEYE"
            + "EBECAAYFAj2RrEEACgkQKiAYjoeCbzjz0wCg+q801XrXk+Rf+koSI50MW5OaaKYA"
            + "oKOVA8SLxE29qSR/bJeuW0ryzRLqtCVSYWxmIEhhdXNlciA8aGF1c2VyLnJhbGZA"
            + "ZnJlZXN1cmYuY2g+iQBGBBARAgAGBQI9kaxXAAoJECogGI6Hgm848zoAnRBtWH6e"
            + "fTb3is63s8J2zTfpsyS0AKDxTjl+ZZV0COHLrSCaNLZVcpImFrkEDQQ9kar+EBAA"
            + "+RigfloGYXpDkJXcBWyHhuxh7M1FHw7Y4KN5xsncegus5D/jRpS2MEpT13wCFkiA"
            + "tRXlKZmpnwd00//jocWWIE6YZbjYDe4QXau2FxxR2FDKIldDKb6V6FYrOHhcC9v4"
            + "TE3V46pGzPvOF+gqnRRh44SpT9GDhKh5tu+Pp0NGCMbMHXdXJDhK4sTw6I4TZ5dO"
            + "khNh9tvrJQ4X/faY98h8ebByHTh1+/bBc8SDESYrQ2DD4+jWCv2hKCYLrqmus2UP"
            + "ogBTAaB81qujEh76DyrOH3SET8rzF/OkQOnX0ne2Qi0CNsEmy2henXyYCQqNfi3t"
            + "5F159dSST5sYjvwqp0t8MvZCV7cIfwgXcqK61qlC8wXo+VMROU+28W65Szgg2gGn"
            + "VqMU6Y9AVfPQB8bLQ6mUrfdMZIZJ+AyDvWXpF9Sh01D49Vlf3HZSTz09jdvOmeFX"
            + "klnN/biudE/F/Ha8g8VHMGHOfMlm/xX5u/2RXscBqtNbno2gpXI61Brwv0YAWCvl"
            + "9Ij9WE5J280gtJ3kkQc2azNsOA1FHQ98iLMcfFstjvbzySPAQ/ClWxiNjrtVjLhd"
            + "ONM0/XwXV0OjHRhs3jMhLLUq/zzhsSlAGBGNfISnCnLWhsQDGcgHKXrKlQzZlp+r"
            + "0ApQmwJG0wg9ZqRdQZ+cfL2JSyIZJrqrol7DVes91hcAAgIQAKD9MGkS8SUD2irI"
            + "AiwVHU0WXLBnk2CvvueSmT9YtC34UKkIkDPZ7VoeuXDfqTOlbiE6T16zPvArZfbl"
            + "JGdrU7HhsTdu+ADxRt1dPur0G0ICJ3pBD3ydGWpdLI/94x1BvTY4rsR5mS4YWmpf"
            + "e2kWc7ZqezhP7Xt9q7m4EK456ddeUZWtkwGU+PKyRAZ+CK82Uhouw+4aW0NjiqmX"
            + "hfH9/BUhI1P/8R9VkTfAFGPmZzqoHr4AuO5tLRLD2RFSmQCP8nZTiP9nP+wBBvn7"
            + "vuqKRQsj9PwwPD4V5SM+kpW+rUIWr9TZYl3UqSnlXlpEZFd2Bfl6NloeH0cfU69E"
            + "gtjcWGvGxYKPS0cg5yhVb4okka6RqIPQiYl6eJgv4tRTKoPRX29o0aUVdqVvDr5u"
            + "tnFzcINq7jTo8GiO8Ia3cIFWfo0LyQBd1cf1U+eEOz+DleEFqyljaz9VCbDPE4GP"
            + "o+ALESBlOwn5daUSaah9iU8aVPaSjn45hoQqxOKPwJxnCKKQ01iy0Gir+CDU8JJB"
            + "7bmbvQN4bke30EGAeED3oi+3VaBHrhjYLv7SHIxP5jtCJKWMJuLRV709HsWJi3kn"
            + "fGHwH+yCDF8+PDeROAzpXBaD2EFhKgeUTjP5Rgn6ltRf8TQnfbW4qlwyiXMhPOfC"
            + "x6qNmwaFPKQJpIkVq5VGfRXAERfkiQBMBBgRAgAMBQI9kar+BRsMAAAAAAoJECog"
            + "GI6Hgm84CDMAoNrNeP4c8XqFJnsLLPcjk5YGLaVIAKCrL5KFuLQVIp7d0Fkscx3/"
            + "7DGrzw==");

		private static readonly byte[] aesSecretKey = Base64.Decode(
              "lQHpBEBSdIYRBADpd7MeIxRk4RsvyMnJNIYe4FiVv6i7I7+LPRvnIjDct0bN"
            + "1gCV48QFej7g/PsvXRjYSowV3VIvchWX8OERd/5i10cLbcs7X52EP1vwYaLj"
            + "uRfNUBg8Q51RQsKR+/rBmnVsi68rjU4yTH6wpo6FOO4pz4wFV+tWwGOwOitA"
            + "K31L4wCgqh59eFFBrOlRFAbDvaL7emoCIR8EAOLxDKiLQJYQrKZfXdZnifeo"
            + "dhEP0uuV4O5TG6nrqkhWffzC9cSoFD0BhMl979d8IB2Uft4FNvQc2u8hbJL5"
            + "7OCGDCUAidlB9jSdu0/J+kfRaTGhYDjBgw7AA42576BBSMNouJg/aOOQENEN"
            + "Nn4n7NxR3viBzIsL/OIeU8HSkBgaA/41PsvcgZ3kwpdltJ/FVRWhmMmv/q/X"
            + "qp1YOnF8xPU9bv2ofELrxJfRsbS4GW1etzD+nXs/woW4Vfixs01x+cutR4iF"
            + "3hw+eU+yLToMPmmo8D2LUvX1SRODJpx5yBBeRIYv6nz9H3sQRDx3kaLASxDV"
            + "jTxKmrLYnZz5w5qyVpvRyv4JAwKyWlhdblPudWBFXNkW5ydKn0AV2f51wEtj"
            + "Zy0aLIeutVMSJf1ytLqjFqrnFe6pdJrHO3G00TE8OuFhftWosLGLbEGytDtF"
            + "cmljIEguIEVjaGlkbmEgKHRlc3Qga2V5IC0gQUVTMjU2KSA8ZXJpY0Bib3Vu"
            + "Y3ljYXN0bGUub3JnPohZBBMRAgAZBQJAUnSGBAsHAwIDFQIDAxYCAQIeAQIX"
            + "gAAKCRBYt1NnUiCgeFKaAKCiqtOO+NQES1gJW6XuOGmSkXt8bQCfcuW7SXZH"
            + "zxK1FfdcG2HEDs3YEVawAgAA");

		private static readonly byte[] aesPublicKey = Base64.Decode(
              "mQGiBEBSdIYRBADpd7MeIxRk4RsvyMnJNIYe4FiVv6i7I7+LPRvnIjDct0bN"
            + "1gCV48QFej7g/PsvXRjYSowV3VIvchWX8OERd/5i10cLbcs7X52EP1vwYaLj"
            + "uRfNUBg8Q51RQsKR+/rBmnVsi68rjU4yTH6wpo6FOO4pz4wFV+tWwGOwOitA"
            + "K31L4wCgqh59eFFBrOlRFAbDvaL7emoCIR8EAOLxDKiLQJYQrKZfXdZnifeo"
            + "dhEP0uuV4O5TG6nrqkhWffzC9cSoFD0BhMl979d8IB2Uft4FNvQc2u8hbJL5"
            + "7OCGDCUAidlB9jSdu0/J+kfRaTGhYDjBgw7AA42576BBSMNouJg/aOOQENEN"
            + "Nn4n7NxR3viBzIsL/OIeU8HSkBgaA/41PsvcgZ3kwpdltJ/FVRWhmMmv/q/X"
            + "qp1YOnF8xPU9bv2ofELrxJfRsbS4GW1etzD+nXs/woW4Vfixs01x+cutR4iF"
            + "3hw+eU+yLToMPmmo8D2LUvX1SRODJpx5yBBeRIYv6nz9H3sQRDx3kaLASxDV"
            + "jTxKmrLYnZz5w5qyVpvRyrQ7RXJpYyBILiBFY2hpZG5hICh0ZXN0IGtleSAt"
            + "IEFFUzI1NikgPGVyaWNAYm91bmN5Y2FzdGxlLm9yZz6IWQQTEQIAGQUCQFJ0"
            + "hgQLBwMCAxUCAwMWAgECHgECF4AACgkQWLdTZ1IgoHhSmgCfU83BLBF2nCua"
            + "zk2dXB9zO1l6XS8AnA07U4cq5W0GrKM6/kP9HWtPhgOFsAIAAA==");

		private static readonly byte[] twofishSecretKey = Base64.Decode(
              "lQHpBEBSdtIRBACf7WfrqTl8F051+EbaljPf/8/ajFpAfMq/7p3Hri8OCsuc"
            + "fJJIufEEOV1/Lt/wkN67MmSyrU0fUCsRbEckRiB4EJ0zGHVFfAnku2lzdgc8"
            + "AVounqcHOmqA/gliFDEnhYOx3bOIAOav+yiOqfKVBhWRCpFdOTE+w/XoDM+p"
            + "p8bH5wCgmP2FuWpzfSut7GVKp51xNEBRNuED/3t2Q+Mq834FVynmLKEmeXB/"
            + "qtIz5reHEQR8eMogsOoJS3bXs6v3Oblj4in1gLyTVfcID5tku6kLP20xMRM2"
            + "zx2oRbz7TyOCrs15IpRXyqqJxUWD8ipgJPkPXE7hK8dh4YSTUi4i5a1ug8xG"
            + "314twlPzrchpWZiutDvZ+ks1rzOtBACHrEFG2frUu+qVkL43tySE0cV2bnuK"
            + "LVhXbpzF3Qdkfxou2nuzsCbl6m87OWocJX8uYcQGlHLKv8Q2cfxZyieLFg6v"
            + "06LSFdE9drGBWz7mbrT4OJjxPyvnkffPfLOOqae3PMYIIuscvswuhm4X5aoj"
            + "KJs01YT3L6f0iIj03hCeV/4KAwLcGrxT3X0qR2CZyZYSVBdjXeNYKXuGBtOf"
            + "ood26WOtwLw4+l9sHVoiXNv0LomkO58ndJRPGCeZWZEDMVrfkS7rcOlktDxF"
            + "cmljIEguIEVjaGlkbmEgKHRlc3Qga2V5IC0gdHdvZmlzaCkgPGVyaWNAYm91"
            + "bmN5Y2FzdGxlLm9yZz6IWQQTEQIAGQUCQFJ20gQLBwMCAxUCAwMWAgECHgEC"
            + "F4AACgkQaCCMaHh9zR2+RQCghcQwlt4B4YmNxp2b3v6rP3E8M0kAn2Gspi4u"
            + "A/ynoqnC1O8HNlbjPdlVsAIAAA==");

		private static readonly byte[] twofishPublicKey = Base64.Decode(
              "mQGiBEBSdtIRBACf7WfrqTl8F051+EbaljPf/8/ajFpAfMq/7p3Hri8OCsuc"
            + "fJJIufEEOV1/Lt/wkN67MmSyrU0fUCsRbEckRiB4EJ0zGHVFfAnku2lzdgc8"
            + "AVounqcHOmqA/gliFDEnhYOx3bOIAOav+yiOqfKVBhWRCpFdOTE+w/XoDM+p"
            + "p8bH5wCgmP2FuWpzfSut7GVKp51xNEBRNuED/3t2Q+Mq834FVynmLKEmeXB/"
            + "qtIz5reHEQR8eMogsOoJS3bXs6v3Oblj4in1gLyTVfcID5tku6kLP20xMRM2"
            + "zx2oRbz7TyOCrs15IpRXyqqJxUWD8ipgJPkPXE7hK8dh4YSTUi4i5a1ug8xG"
            + "314twlPzrchpWZiutDvZ+ks1rzOtBACHrEFG2frUu+qVkL43tySE0cV2bnuK"
            + "LVhXbpzF3Qdkfxou2nuzsCbl6m87OWocJX8uYcQGlHLKv8Q2cfxZyieLFg6v"
            + "06LSFdE9drGBWz7mbrT4OJjxPyvnkffPfLOOqae3PMYIIuscvswuhm4X5aoj"
            + "KJs01YT3L6f0iIj03hCeV7Q8RXJpYyBILiBFY2hpZG5hICh0ZXN0IGtleSAt"
            + "IHR3b2Zpc2gpIDxlcmljQGJvdW5jeWNhc3RsZS5vcmc+iFkEExECABkFAkBS"
            + "dtIECwcDAgMVAgMDFgIBAh4BAheAAAoJEGggjGh4fc0dvkUAn2QGdNk8Wrrd"
            + "+DvKECrO5+yoPRx3AJ91DhCMme6uMrQorKSDYxHlgc7iT7ACAAA=");

		private static readonly char[] pass = "hello world".ToCharArray();

		/**
        * Generated signature test
        *
        * @param sKey
        * @param pgpPrivKey
        * @return test result
        */
        public void GenerateTest(
            PgpSecretKeyRing sKey,
            PgpPublicKey     pgpPubKey,
            PgpPrivateKey    pgpPrivKey)
        {
            string data = "hello world!";
            MemoryStream bOut = new MemoryStream();

            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            MemoryStream testIn = new MemoryStream(dataBytes, false);

            PgpSignatureGenerator sGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);

            PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();

            IEnumerator enumerator = sKey.GetSecretKey().PublicKey.GetUserIds().GetEnumerator();
            enumerator.MoveNext();
            string primaryUserId = (string) enumerator.Current;

            spGen.SetSignerUserId(true, primaryUserId);

            sGen.SetHashedSubpackets(spGen.Generate());

            PgpCompressedDataGenerator cGen = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);

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

			int ch;
            while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte) ch);
                sGen.Update((byte)ch);
            }

            lGen.Close();

            sGen.Generate().Encode(bcOut);

            cGen.Close();

            PgpObjectFactory pgpFact = new PgpObjectFactory(bOut.ToArray());
            PgpCompressedData c1 = (PgpCompressedData)pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

			PgpOnePassSignatureList p1 = (PgpOnePassSignatureList)pgpFact.NextPgpObject();
            PgpOnePassSignature ops = p1[0];

            PgpLiteralData p2 = (PgpLiteralData) pgpFact.NextPgpObject();
			if (!p2.ModificationTime.Equals(testDateTime))
			{
				Fail("Modification time not preserved");
			}

			Stream dIn = p2.GetInputStream();

            ops.InitVerify(pgpPubKey);

			while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte) ch);
            }

			PgpSignatureList p3 = (PgpSignatureList) pgpFact.NextPgpObject();

			if (!ops.Verify(p3[0]))
            {
                Fail("Failed generated signature check");
            }
        }

		public override void PerformTest()
        {
            PgpPublicKey pubKey = null;

			//
            // Read the public key
            //
            PgpPublicKeyRing pgpPub = new PgpPublicKeyRing(testPubKey);

			pubKey = pgpPub.GetPublicKey();

            //
            // Read the private key
            //
            PgpSecretKeyRing sKey = new PgpSecretKeyRing(testPrivKey);
            PgpSecretKey secretKey = sKey.GetSecretKey();
            PgpPrivateKey pgpPrivKey = secretKey.ExtractPrivateKey(pass);

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

            ops.InitVerify(pubKey);

			int ch;
			while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte) ch);
            }

            PgpSignatureList p3 = (PgpSignatureList) pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed signature check");
            }

            //
            // signature generation
            //
            GenerateTest(sKey, pubKey, pgpPrivKey);

            //
            // signature generation - canonical text
            //
            const string data = "hello world!";
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            MemoryStream bOut = new MemoryStream();
            MemoryStream testIn = new MemoryStream(dataBytes, false);
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(
                PublicKeyAlgorithmTag.Dsa, HashAlgorithmTag.Sha1);

			sGen.InitSign(PgpSignature.CanonicalTextDocument, pgpPrivKey);

            PgpCompressedDataGenerator  cGen = new PgpCompressedDataGenerator(
                CompressionAlgorithmTag.Zip);

            BcpgOutputStream bcOut = new BcpgOutputStream(cGen.Open(new UncloseableStream(bOut)));

            sGen.GenerateOnePassVersion(false).Encode(bcOut);

            PgpLiteralDataGenerator lGen = new PgpLiteralDataGenerator();
			DateTime testDateTime = new DateTime(1973, 7, 27);
			Stream lOut = lGen.Open(
				new UncloseableStream(bcOut),
				PgpLiteralData.Text,
				"_CONSOLE",
				dataBytes.Length,
				testDateTime);

			while ((ch = testIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte) ch);
                sGen.Update((byte)ch);
            }

			lGen.Close();

			sGen.Generate().Encode(bcOut);

            cGen.Close();

            //
            // verify Generated signature - canconical text
            //
            pgpFact = new PgpObjectFactory(bOut.ToArray());

            c1 = (PgpCompressedData) pgpFact.NextPgpObject();

            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            p1 = (PgpOnePassSignatureList) pgpFact.NextPgpObject();

            ops = p1[0];

            p2 = (PgpLiteralData) pgpFact.NextPgpObject();
			if (!p2.ModificationTime.Equals(testDateTime))
			{
				Fail("Modification time not preserved");
			}

            dIn = p2.GetInputStream();

            ops.InitVerify(pubKey);

            while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
            }

            p3 = (PgpSignatureList) pgpFact.NextPgpObject();

            if (!ops.Verify(p3[0]))
            {
                Fail("Failed generated signature check");
            }

            //
            // Read the public key with user attributes
            //
            pgpPub = new PgpPublicKeyRing(testPubWithUserAttr);

            pubKey = pgpPub.GetPublicKey();

            int count = 0;
            foreach (PgpUserAttributeSubpacketVector attributes in pubKey.GetUserAttributes())
            {
                int sigCount = 0;
                foreach (object sigs in pubKey.GetSignaturesForUserAttribute(attributes))
                {
					if (sigs == null)
						Fail("null signature found");

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
                Fail("Failed user attributes check");
            }

            byte[]  pgpPubBytes = pgpPub.GetEncoded();
            pgpPub = new PgpPublicKeyRing(pgpPubBytes);
            pubKey = pgpPub.GetPublicKey();
            count = 0;

			foreach (object ua in pubKey.GetUserAttributes())
            {
				if (ua == null)
					Fail("null user attribute found");

				count++;
            }

			if (count != 1)
            {
                Fail("Failed user attributes reread");
            }

            //
            // reading test extra data - key with edge condition for DSA key password.
            //
            char[] passPhrase = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

			sKey = new PgpSecretKeyRing(testPrivKey2);
            pgpPrivKey = sKey.GetSecretKey().ExtractPrivateKey(passPhrase);

			//
            // reading test - aes256 encrypted passphrase.
            //
            sKey = new PgpSecretKeyRing(aesSecretKey);
            pgpPrivKey = sKey.GetSecretKey().ExtractPrivateKey(pass);

            //
            // reading test - twofish encrypted passphrase.
            //
            sKey = new PgpSecretKeyRing(twofishSecretKey);
            pgpPrivKey = sKey.GetSecretKey().ExtractPrivateKey(pass);

			//
            // use of PgpKeyPair
            //
            DsaParametersGenerator pGen = new DsaParametersGenerator();
            pGen.Init(512, 80, new SecureRandom()); // TODO Is the certainty okay?
            DsaParameters dsaParams = pGen.GenerateParameters();
            DsaKeyGenerationParameters kgp = new DsaKeyGenerationParameters(new SecureRandom(), dsaParams);
			IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("DSA");
			kpg.Init(kgp);


			AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            PgpKeyPair pgpKp = new PgpKeyPair(PublicKeyAlgorithmTag.Dsa,
                kp.Public, kp.Private, DateTime.UtcNow);

			PgpPublicKey k1 = pgpKp.PublicKey;
            PgpPrivateKey k2 = pgpKp.PrivateKey;
        }

		public override string Name
        {
			get { return "PGPDSATest"; }
        }

		public static void Main(
			string[] args)
        {
			RunTest(new PgpDsaTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
