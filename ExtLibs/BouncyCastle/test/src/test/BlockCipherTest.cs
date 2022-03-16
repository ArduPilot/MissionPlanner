using System;
using System.Globalization;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
    /**
    * basic test class for a block cipher, basically this just exercises the provider, and makes sure we
    * are behaving sensibly, correctness of the implementation is shown in the lightweight test classes.
    */
    [TestFixture]
    public class BlockCipherTest
        : SimpleTest
    {
        private static readonly ISet validModes = CollectionUtilities.ReadOnly(
            new HashSet(new string[]{ "CBC", "CCM", "CFB", "CTR", "CTS", "EAX", "ECB", "GCM", "OCB", "OFB" }));

        private static readonly string[] cipherTests1 =
        {
            "DES",
            "466da00648ef0e1f9617b1f002e225251a3248d09172f46b9617b1f002e225250112ecb3da61bc99",
            "DESede",
            "2f4bc6b30c893fa549d82c560d61cf3eb088aed020603de249d82c560d61cf3e529e95ecd8e05394",
            "SKIPJACK",
            "d4de46d52274dbb029f33b076043f8c40089f906751623de29f33b076043f8c4ac99b90f9396cb04",
            "Blowfish",
            "7870ebe7f6a52803eb9396ba6c5198216ce81d76d8d4c74beb9396ba6c5198211212473b05214e9f",
            "Twofish",
            "70336d9c9718a8a2ced1b19deed973a3c58af7ea71a69e7efc4df082dca581c0839e31468661bcfc57a14899ceeb0253",
            "RC2",
            "eb5b889bbcced12eb6b1a3da6a3d965bba66a5edfdd4c8a6b6b1a3da6a3d965b994a5b859e765797",
            "RC5",
            "220053543e3eca3bc9503a091ca67b08372560d8a4fdbee8c9503a091ca67b08a796d53bb8a4b7e0",
            "RC5-64",
            "e0b4a526ba3bc5f09199c3b1fe3737fe6d248cde70e565b0feea59ebfda375ae1946c386a48d8d8a74d7b1947ff6a788",
            "RC6",
            "44c97b67ca8486067f8b6c5b97632f3049e5e52c1d61fdd527dc3da39616540f19a3db39aac1ffd713795cd886cce0c0",
            "IDEA",
            "8c9fd56823ffdc523f6ccf7f614aa6173553e594fc7a21b53f6ccf7f614aa61740c54f7a66e95108",
            "TEA",
            "fcf45062104fda7c35712368b56dd4216a6ca998dc297b5435712368b56dd421208027ed2923cd0c",
            "XTEA",
            "4b427893d3d6aaded2afafabe25f7b233fb5589faa2b6389d2afafabe25f7b239d12979ac67e1c07",
            "Camellia",
            "3a68b4ad145bc2c76010669d68f2826359887afce763a78d9994143266adfaec8ba7ee562a1688ef9dfd7f897e5c44dc",
            "SEED",
            "d53d4ce1f48b9879420949467bfcbfbe2c6a7d4a8770bee0c71211def898d7c5024ce2007dd85accb3f69d906ae2164d",
            "Noekeon",
            "7e68ceb33aad9db04af6b878a16dd6c6b4f880d6c89027ba581884c10690bb6b3dbfd6ed5513e2c4f5670c3528023121",
            "DES/CBC/NoPadding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122a",
            "DESede/CBC/NoPadding",
            "4d3d7931875cf25593dc402298add8b914761e4936c9585ae22b2c1441169231",
            "SKIPJACK/CBC/NoPadding",
            "ceebcc2e5e2b847f9ed797b4930b95f115b9e6cf49c457fc2ea0df79ad5c8334",
            "Blowfish/CBC/NoPadding",
            "80823abbabc109733e7ebf3ce3344d67fc387c306b782086b452f7fbe8e844ce",
            "Twofish/CBC/NoPadding",
            "f819694251a00bdd403928745cd1d8a094de61f49ddf8e7692e9d81a83812943",
            "RC2/CBC/NoPadding",
            "a51facdb3933c9676795cd38cc3146fd4694722b468b1a979a399c77606abf99",
            "RC5/CBC/NoPadding",
            "9ee7517eab0280445f3a7c60c90c0f75029d65bca8b1af83ace5399d388c83c3",
            "RC6/CBC/NoPadding",
            "c44695633c07010f3a0d8f7ea046a642d4a96bf4e44f89fd91b46830bc95b130",
            "IDEA/CBC/NoPadding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d9",
            "DES/CBC/PKCS5Padding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122afdc70484fb9c0232",
            "DES/CBC/ISO10126Padding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122a980639850a2cc3e8",
            "DES/CBC/ISO7816-4Padding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122a1f80b9b0f1be49ac",
            "DES/CBC/X9.23Padding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122a980639850a2cc3e8",
            "DESede/CBC/PKCS7Padding",
            "4d3d7931875cf25593dc402298add8b914761e4936c9585ae22b2c1441169231a41e40695f1cff84",
            "SKIPJACK/CBC/PKCS7Padding",
            "ceebcc2e5e2b847f9ed797b4930b95f115b9e6cf49c457fc2ea0df79ad5c8334df7042de5db89c96",
            "Blowfish/CBC/PKCS7Padding",
            "80823abbabc109733e7ebf3ce3344d67fc387c306b782086b452f7fbe8e844cef986562ab1a675e8",
            "Twofish/CBC/PKCS7Padding",
            "f819694251a00bdd403928745cd1d8a094de61f49ddf8e7692e9d81a838129433e5f1343d6cdb0b41838619da1541f04",
            "RC2/CBC/PKCS7Padding",
            "a51facdb3933c9676795cd38cc3146fd4694722b468b1a979a399c77606abf9958435525f770f137",
            "RC5/CBC/PKCS7Padding",
            "9ee7517eab0280445f3a7c60c90c0f75029d65bca8b1af83ace5399d388c83c3edd95ff49be76651",
            "RC5-64/CBC/PKCS7Padding",
            "e479fd11f89dab22d2f3dd062b1d2abd5b5962553421a5c562dc7214c3b23b8e21949fda87f2f820e5f032c552c6ec78",
            "RC6/CBC/PKCS7Padding",
            "c44695633c07010f3a0d8f7ea046a642d4a96bf4e44f89fd91b46830bc95b130824b972c9019a69d2dd05ef2d36b37ac",
            "IDEA/CBC/PKCS7Padding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d9e584751325ef7c32",
            "IDEA/CBC/ISO10126Padding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d978b3fd73135f033b",
            "IDEA/CBC/X9.23Padding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d978b3fd73135f033b",
            "AES/CBC/PKCS7Padding",
            "cf87f4d8bb9d1abb36cdd9f44ead7d046db2f802d99e1ef0a5940f306079e08389a44c4a8cc1a47cbaee1128da55bbb7",
            "AES/CBC/ISO7816-4Padding",
            "cf87f4d8bb9d1abb36cdd9f44ead7d046db2f802d99e1ef0a5940f306079e08306d84876508a33efec701118d8eeaf6d",
            "Rijndael/CBC/PKCS7Padding",
            "cf87f4d8bb9d1abb36cdd9f44ead7d046db2f802d99e1ef0a5940f306079e08389a44c4a8cc1a47cbaee1128da55bbb7",
            "Serpent/CBC/PKCS7Padding",
            "d8b971931de211cb2d31721773a5b1f9dc4e263efe0465f97c024daa26dd7d03473e9beb82ba809cf36071d4807e4706",
            "Tnepres/CBC/PKCS7Padding",
            "f8940ca31aba8ce1e0693b1ae0b1e08daef6de03c80f019774280052f824ac44540bb8dd74dfad47f83f9c7ec268ca68",
            "CAST5/CBC/PKCS7Padding",
            "87b6dc0c5a1d23d42fa740b0548be0b298112000544610d889d6361994cf8e670a19d6af72d7289f",
            "CAST6/CBC/PKCS7Padding",
            "943445569cfdda174118e433828f84e137faee38cac5c827d87a3c9a5a46a07dd64e7ad8accd921f248eea627cd6826f",
            "IDEA/CBC/PKCS7Padding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d9e584751325ef7c32",
            "DES/CBC/ZeroBytePadding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122ad3b3f002c927f1fd",
            "DES/CTS/NoPadding", // official style
            "60fa2f8fae5aa2a38e9ac77d0246726bcf99f75cc6e0122aeb7511e4515feb12",
            "DESede/CTS/NoPadding",
            "4d3d7931875cf25593dc402298add8b9e22b2c144116923114761e4936c9585a",
            "SKIPJACK/CTS/NoPadding",
            "ceebcc2e5e2b847f9ed797b4930b95f12ea0df79ad5c833415b9e6cf49c457fc",
            "Blowfish/CTS/NoPadding",
            "80823abbabc109733e7ebf3ce3344d67b452f7fbe8e844cefc387c306b782086",
            "Twofish/CTS/NoPadding",
            "94de61f49ddf8e7692e9d81a83812943f819694251a00bdd403928745cd1d8a0",
            "AES/CTS/NoPadding",
            "6db2f802d99e1ef0a5940f306079e083cf87f4d8bb9d1abb36cdd9f44ead7d04",
            "Rijndael/CTS/NoPadding",
            "6db2f802d99e1ef0a5940f306079e083cf87f4d8bb9d1abb36cdd9f44ead7d04",
            "Serpent/CTS/NoPadding",
            "dc4e263efe0465f97c024daa26dd7d03d8b971931de211cb2d31721773a5b1f9",
            "Tnepres/CTS/NoPadding",
            "aef6de03c80f019774280052f824ac44f8940ca31aba8ce1e0693b1ae0b1e08d",
            "CAST5/CTS/NoPadding",
            "87b6dc0c5a1d23d42fa740b0548be0b289d6361994cf8e6798112000544610d8",
            "CAST6/CTS/NoPadding",
            "37faee38cac5c827d87a3c9a5a46a07d943445569cfdda174118e433828f84e1",
            "RC2/CTS/NoPadding",
            "a51facdb3933c9676795cd38cc3146fd9a399c77606abf994694722b468b1a97",
            "RC5/CTS/NoPadding",
            "9ee7517eab0280445f3a7c60c90c0f75ace5399d388c83c3029d65bca8b1af83",
            "RC6/CTS/NoPadding",
            "d4a96bf4e44f89fd91b46830bc95b130c44695633c07010f3a0d8f7ea046a642",
            "IDEA/CTS/NoPadding",
            "30cd990ebdae80fe12b6c6e4fcd1c06497351c8684e4c4d9a27d985c276b3d70",
            "DES/CBC/WithCTS",                  // older style
            "60fa2f8fae5aa2a38e9ac77d0246726bcf99f75cc6e0122aeb7511e4515feb12",
            "DESede/CBC/WithCTS",
            "4d3d7931875cf25593dc402298add8b9e22b2c144116923114761e4936c9585a",
            "SKIPJACK/CBC/WithCTS",
            "ceebcc2e5e2b847f9ed797b4930b95f12ea0df79ad5c833415b9e6cf49c457fc",
            "Blowfish/CBC/WithCTS",
            "80823abbabc109733e7ebf3ce3344d67b452f7fbe8e844cefc387c306b782086",
            "Twofish/CBC/WithCTS",
            "94de61f49ddf8e7692e9d81a83812943f819694251a00bdd403928745cd1d8a0",
            "AES/CBC/WithCTS",
            "6db2f802d99e1ef0a5940f306079e083cf87f4d8bb9d1abb36cdd9f44ead7d04",
            "Rijndael/CBC/WithCTS",
            "6db2f802d99e1ef0a5940f306079e083cf87f4d8bb9d1abb36cdd9f44ead7d04",
            "Serpent/CBC/WithCTS",
            "dc4e263efe0465f97c024daa26dd7d03d8b971931de211cb2d31721773a5b1f9",
            "Tnepres/CBC/WithCTS",
            "aef6de03c80f019774280052f824ac44f8940ca31aba8ce1e0693b1ae0b1e08d",
            "CAST5/CBC/WithCTS",
            "87b6dc0c5a1d23d42fa740b0548be0b289d6361994cf8e6798112000544610d8",
            "CAST6/CBC/WithCTS",
            "37faee38cac5c827d87a3c9a5a46a07d943445569cfdda174118e433828f84e1",
            "RC2/CBC/WithCTS",
            "a51facdb3933c9676795cd38cc3146fd9a399c77606abf994694722b468b1a97",
            "RC5/CBC/WithCTS",
            "9ee7517eab0280445f3a7c60c90c0f75ace5399d388c83c3029d65bca8b1af83",
            "RC6/CBC/WithCTS",
            "d4a96bf4e44f89fd91b46830bc95b130c44695633c07010f3a0d8f7ea046a642",
            "IDEA/CBC/WithCTS",
            "30cd990ebdae80fe12b6c6e4fcd1c06497351c8684e4c4d9a27d985c276b3d70",
            "DES/OFB/NoPadding",
            "537572e480c1714f5c9a4f3b874df824dc6681b1fd6c11982debcad91e3f78b7",
            "DESede/OFB/NoPadding",
            "481e9872acea7fcf8e29a453242da774e5f6a28f15f7723659a73e4ff4939f80",
            "SKIPJACK/OFB/NoPadding",
            "71143a124e3a0cde753b60fe9b200e559018b6a0fe0682659f7c13feb9df995c",
            "Blowfish/OFB/NoPadding",
            "6cd6f7c5d2c655556d7a9e98a1696d1875e9f1b2fc991e28a2d55b56861e80bd",
            "Twofish/OFB/NoPadding",
            "821c54b1b54ae113cf74595eefe10c83b61c9682fc81f92c52f39a3a693f88b8",
            "RC2/OFB/NoPadding",
            "0a07cb78537cb04c0c74e28a7b86b80f80acadf87d6ef32792f1a8cf74b39f74",
            "RC5/OFB/NoPadding",
            "c62b233df296283b918a2b4cc53a54fbf061850e781b97332ed1bd78b88d9670",
            "IDEA/OFB/NoPadding",
            "dd447da3cbdcf81f4053fb446596261cb00a3c49a66085485af5f7c10ba20dad",
            "DES/OFB8/NoPadding",
            "53cb5010d189f94cf584e5ff1c4a9d86443c45ddb6fa3c2d1a5dadfcdf01db8a",
            "DESede/OFB8/NoPadding",
            "482c0c1ccd0e6d218e1cffb0a295352c2357ffaa673f2257ef5c77b6c04f03b5",
            "SKIPJACK/OFB8/NoPadding",
            "719ea1b432b3d2c8011e5aa873f95978420022b5e2c9c1a1c1082cd1f4999da2",
            "Blowfish/OFB8/NoPadding",
            "6ca6078755b263f09787d830b6fda7b7748494634bdc73ab68540cf9f6b7eccf",
            "Twofish/OFB8/NoPadding",
            "825dcec234ad52253d6e064b0d769bc04b1142435933f4a510ffc20d70095a88",
            "RC2/OFB8/NoPadding",
            "0aa26c6f6a820fe7d38da97085995ad62e2e293323a76300fcd4eb572810f7c6",
            "RC5/OFB8/NoPadding",
            "c601a9074dbd874f4d3293f6a32d93d9f0a4f5685d8597f0102fcc96d444f976",
            "IDEA/OFB8/NoPadding",
            "dd7897b6ced43d060a518bb38d570308b83b4de577eb208130daabf619e9b1fb",
            "DES/CFB/NoPadding",
            "537572e480c1714fec3c7424f88d4202219244c5ca8f5e4361d64f08fe747bb2",
            "DESede/CFB/NoPadding",
            "481e9872acea7fcfb75bb58670fe64c59123265139e357d161cd4ddb5eba042a",
            "SKIPJACK/CFB/NoPadding",
            "71143a124e3a0cde70a69ede4ceb14376b1e6a80bafde0a6330508dfa86a7c41",
            "Blowfish/CFB/NoPadding",
            "6cd6f7c5d2c6555561167fe9b10665102206869339122f1ed89efa4a985397f6",
            "Twofish/CFB/NoPadding",
            "821c54b1b54ae113cf74595eefe10c8308b7a438277de4f40948ac2d172d53d2",
            "RC2/CFB/NoPadding",
            "0a07cb78537cb04ca1401450d5cd411c7da7fa5b6baaa17bb2137bd95c9f26a5",
            "RC5/CFB/NoPadding",
            "c62b233df296283b989352bbebf616a19e11503ac737f9e0eaf19049cde05d34",
            "IDEA/CFB/NoPadding",
            "dd447da3cbdcf81fcbe4661dcbed88aed899f87585118384bd0565067fa6c13a",
            "DES/CFB8/NoPadding",
            "53cb0cdff712a825eb283b23c31e7323aa12495e7e751428b5c4eb89b28a25d4",
            "DESede/CFB8/NoPadding",
            "482cd5bf87ca4cee0b573d66a077231bfea93843ce2d1f948550a1d208e18279",
            "SKIPJACK/CFB8/NoPadding",
            "719eef3906bef23f7b63599285437d8e34183b165acf3e855b4e160d4f036508",
            "Blowfish/CFB8/NoPadding",
            "6ca63aaada9188d2410c07513cc0736b9888770768c25a5befc776beea5bdc4c",
            "Twofish/CFB8/NoPadding",
            "825d12af040721cf5ed4a4798647837ac5eb14d752aace28728aeb37b2010abd",
            "RC2/CFB8/NoPadding",
            "0aa227f94be3a32ff927c5d25647ea41d7c2a1e94012fc7f2ad6767b9664bce5",
            "RC5/CFB8/NoPadding",
            "c601cf88725411f119965b9cd38d6c313b91128ed7c98c7604cc62d9b210be79",
            "IDEA/CFB8/NoPadding",
            "dd7839d2525420d10f95eec23dbaf3463302c445972a28c563c2635191bc19af",
            // TODO Check that PGPCFB modes are obsolete for C# build
//			"IDEA/PGPCFB/NoPadding",
//			"dd447da3cbdcf81fcbe4661dcbed88aed899f87585118384bd0565067fa6c13a",
//			"IDEA/PGPCFBwithIv/NoPadding",
//			"ed5adbac0e730cc0f00df7e4f6fef672ab042673106435faf3ecf3996a72a0e127b440ba9e5313501de3",
            "Twofish/ECB/TBCPadding",
            "70336d9c9718a8a2ced1b19deed973a3c58af7ea71a69e7efc4df082dca581c019d7daa58d02b89aab6e8c0d17202439",
            "RC2/ECB/TBCPadding",
            "eb5b889bbcced12eb6b1a3da6a3d965bba66a5edfdd4c8a6b6b1a3da6a3d965b6b5359ba5e69b179",
            "DES/CTR/NoPadding",
            "537572e480c1714fb47081d35eb18eaca9e0a5aee982f105438a0db6cece1f6d",
            "DESede/CTR/NoPadding",
            "481e9872acea7fcfa93b7d4e34ec7bab340c10faba2e43b879d40d38e07c422d",
            "SKIPJACK/CTR/NoPadding",
            "71143a124e3a0cdeee98a7b843baa05bd1d59faee8ec9b89880e070314a04cc2",
            "Blowfish/CTR/NoPadding",
            "6cd6f7c5d2c65555d2b31f8614f54ec654f5e7888d515008d59302c3edfcc6cb",
            "Twofish/CTR/NoPadding",
            "821c54b1b54ae113cf74595eefe10c83d09e95d4599190b9bbd5bc71dd703730",
            "Threefish-256/CTR/NoPadding",
            "546ea995dd302f1efcb1f27d14bad468280a3a7994c2af75dfdf1e9fc5ef2373",
            "Threefish-512/CTR/NoPadding",
            "152df966484ecc2e9ddfc386559732f7f632e4008920804a1bde4efcf2e6e2f2",
            "Threefish-1024/CTR/NoPadding",
            "03953ac751a7377812c6e3e4d14b36c6953f9b390acaa892811c10001c9be454",
            "RC2/CTR/NoPadding",
            "0a07cb78537cb04c8c5a0a39a15977a7eb19f3c48a42759c234868c391a99c63",
            "RC5/CTR/NoPadding",
            "c62b233df296283b97f17364d5f69a1ff91f46659cf9856caefd322a936203a7",
            "IDEA/CTR/NoPadding",
            "dd447da3cbdcf81f4694ab7715d79e3f90af5682e8c318b8f7dadbed6b5c9714",
            "Blowfish/EAX/NoPadding",
            "bee85ae6512b8a2346d46f7bac31526238091ccc5de75760c9a39628fb45d44a653bfac0",
            "CAST5/EAX/NoPadding",
            "85e0dbd3402f2179f96d231315ec73f04f64f1b7ab1347423b9aec51a07a7222e2bc65a3",
            "DES/EAX/NoPadding",
            "07d12249945e77607086f7463f316966466e6a0c0789b3307b8b51a7cc807e3c1fb91f98",
            "DESede/EAX/NoPadding",
            "278b28f13537dc13bb688c95391754bd6d39c79a7361b407f8dee0b111b264f20391cb0e",
            "GOST28147/EAX/NoPadding",
            "1416713d52affb595b880be996e838edd377e67dfe822fbb0ff235f1b706e6ce34d68dc5",
            "IDEA/EAX/NoPadding",
            "b2e9f3e40954c140ac60423466dee0138f84e879fbde003780202bd83c91571b64df7bb7",
            "RC2/EAX/NoPadding",
            "5d1c095de75bd5eef6a5146f7d6c44545807a8b452f7a38e2719a14f1a269709d2eda2d3",
            "SEED/EAX/NoPadding",
            "6780f18b2dd1f75a934b5a3e45e8fd44877fd3498a9b919b417b3d8a7c67c6021d74bbaef71841ef",
            "Serpent/EAX/NoPadding",
            "13c2b1fec2bda74f5ccc8ca31b36a2e91ee024a215387219808640b2fc7a6a41e017aacee3ed893a",
            "Tnepres/EAX/NoPadding",
            "8d5ac312ca0d436a0154d56568d39811ccf6bb970012398014fc8a49ed669b117443c0249b07ead8",
            //"SM4/EAX/NoPadding",
            //"e072a95da8e529b41199859482142b3fdfa6b7af27348e5ebf35445a099583dae882affde90ea4a4",
            "Twofish/EAX/NoPadding",
            "9a90dffe1233a04733fc8869e8ec4cba2fa53d9543f0206825293b1ff102e63f81a60b12204e1fd8",
            "IDEA/OFB/NoPadding",
            "dd447da3cbdcf81f4053fb446596261cb00a3c49a66085485af5f7c10ba20dad",
            "RC2/OFB/NoPadding",
            "0a07cb78537cb04c0c74e28a7b86b80f80acadf87d6ef32792f1a8cf74b39f74",
            "SEED/OFB/NoPadding",
            "9fd249435dc66d3d5d41abad270df5e3c6b972692fadfcb6c311b047f96fb114",
            "SEED/OCB/NoPadding",
            "eb04b3612769e1ad681f975af1a6f401d94dc88276dd50fc3ebce791c28825c652b7351acbad8c63d4d66191de94c970",
            "SEED/CCM/NoPadding",
            "da684e8cab782d4ebae835726f43c3aeea97ee270897255714d464e981ac39af06c9483153f8a05a",
            "SEED/GCM/NoPadding",
            "ed5f6293c9a4f280af6695750bfb3bb3b60c214565a049494df955152757812ebfb93705895606c4378498a93f2541b5",
            //"SM4/GCM/NoPadding",
            //"323b601a951da693f87e53c6832380719b4d4bd306c94248202b7e337c81e2d9de0044b77a4c556f15f6fd19f828236b",
            "DES/ECB/TBCPadding",
            "466da00648ef0e1f9617b1f002e225251a3248d09172f46b9617b1f002e22525698575eb3998481b",
            "GOST28147/ECB/TBCPadding",
            "0a77f4114451b37d44c5192619b723dd49093d1047c2373544c5192619b723dde7b0810d205c07ab",
            "IDEA/ECB/TBCPadding",
            "8c9fd56823ffdc523f6ccf7f614aa6173553e594fc7a21b53f6ccf7f614aa61747a7c95a57b9eaf4",
            "RC2/ECB/TBCPadding",
            "eb5b889bbcced12eb6b1a3da6a3d965bba66a5edfdd4c8a6b6b1a3da6a3d965b6b5359ba5e69b179",
            "SEED/ECB/TBCPadding",
            "d53d4ce1f48b9879420949467bfcbfbe2c6a7d4a8770bee0c71211def898d7c509f6e111845db39b4cce1dd155aa592b",
            "DES/CBC/TBCPadding",
            "60fa2f8fae5aa2a38e9ac77d0246726beb7511e4515feb12cf99f75cc6e0122ad3b3f002c927f1fd",
            "GOST28147/CBC/TBCPadding",
            "ba87be9c465cbb30e1bf0148daa9639c2e4cbc1b6777cfcda860760686596159aa564fd65e66c125",
            "IDEA/CBC/TBCPadding",
            "30cd990ebdae80fe12b6c6e4fcd1c064a27d985c276b3d7097351c8684e4c4d922f14e12faecaa0b",
            "RC2/CBC/TBCPadding",
            "a51facdb3933c9676795cd38cc3146fd4694722b468b1a979a399c77606abf9997b47d2f64a37e2f",
            "SEED/CBC/TBCPadding",
            "fc34f03ddf4d2a4d9934addc82011af1d5f76ee015b691a6524d7ad5464422d7989825d19e23a60ba759407e13d1ea02",
            "DES/CFB8/NoPadding",
            "53cb0cdff712a825eb283b23c31e7323aa12495e7e751428b5c4eb89b28a25d4",
            "GOST28147/CFB8/NoPadding",
            "29f6ca1ca7ae9670413183932a28cdd4a09f2ba630c3c3fbf6f071d3774d7577",
            "IDEA/CFB8/NoPadding",
            "dd7839d2525420d10f95eec23dbaf3463302c445972a28c563c2635191bc19af",
            "RC2/CFB8/NoPadding",
            "0aa227f94be3a32ff927c5d25647ea41d7c2a1e94012fc7f2ad6767b9664bce5",
            "SEED/CFB8/NoPadding",
            "9f1622c3785a034ee4c595df05fb11e69e4d52036e238d2d451e190e87ee876e",
            "DES/CTS/NoPadding",
            "60fa2f8fae5aa2a38e9ac77d0246726bcf99f75cc6e0122aeb7511e4515feb12",
            "GOST28147/CTS/NoPadding",
            "ba87be9c465cbb30e1bf0148daa9639ca8607606865961592e4cbc1b6777cfcd",
            "IDEA/CTS/NoPadding",
            "30cd990ebdae80fe12b6c6e4fcd1c06497351c8684e4c4d9a27d985c276b3d70",
            "RC2/CTS/NoPadding",
            "a51facdb3933c9676795cd38cc3146fd9a399c77606abf994694722b468b1a97",
            "SEED/CTS/NoPadding",
            "d5f76ee015b691a6524d7ad5464422d7fc34f03ddf4d2a4d9934addc82011af1",
            //"SHACAL-2/CBC/PKCS7Padding",
            //"3af7c54ea55d2497162ac9c79d9b2f7837898f83aa4b50b7b762979aa8087669b6a81cdec475ed4d2394d7ad771404a52eb52d245a39f0d7d3e8062d3b0f0e54",
            //"SHACAL-2/CBC/TBCPadding",
            //"3af7c54ea55d2497162ac9c79d9b2f7837898f83aa4b50b7b762979aa80876693f17fbe9a5baa88ed21b2e1a863dc449061f40cafadfc3cf73486208f87b9352",
        };

        private static readonly string[] cipherTests2 =
        {
            "DES/OFB64/NoPadding",
            "537572e480c1714f5c9a4f3b874df824dc6681b1fd6c11982debcad91e",
            "DES/CFB64/NoPadding",
            "537572e480c1714fec3c7424f88d4202219244c5ca8f5e4361d64f08fe",
            "DES/CTR/NoPadding",
            "537572e480c1714fb47081d35eb18eaca9e0a5aee982f105438a0db6ce",
            "DES/CTS/NoPadding",
            "60fa2f8fae5aa2a38e9ac77d0246726b32df660db51a710ceb7511e451"
        };

        private static readonly byte[] input1 = Hex.Decode("000102030405060708090a0b0c0d0e0fff0102030405060708090a0b0c0d0e0f");
        private static readonly byte[] input2 = Hex.Decode("000102030405060708090a0b0c0d0e0fff0102030405060708090a0b0c");

//		static RC2ParameterSpec rc2Spec = new RC2ParameterSpec(128, Hex.decode("0123456789abcdef"));
        private static readonly byte[] rc2IV = Hex.Decode("0123456789abcdef");

//		static RC5ParameterSpec rc5Spec = new RC5ParameterSpec(16, 16, 32, Hex.decode("0123456789abcdef"));
//		static RC5ParameterSpec rc564Spec = new RC5ParameterSpec(16, 16, 64, Hex.decode("0123456789abcdef0123456789abcdef"));
        private static readonly byte[] rc5IV = Hex.Decode("0123456789abcdef");
        private static readonly byte[] rc564IV = Hex.Decode("0123456789abcdef0123456789abcdef");
        private const int rc5Rounds = 16;

        /**
        * a fake random number generator - we just want to make sure the random numbers
        * aren't random so that we get the same output, while still getting to test the
        * key generation facilities.
        */
        private class FixedSecureRandom
            : SecureRandom
        {
            byte[] seed = {
                    (byte)0xaa, (byte)0xfd, (byte)0x12, (byte)0xf6, (byte)0x59,
                    (byte)0xca, (byte)0xe6, (byte)0x34, (byte)0x89, (byte)0xb4,
                    (byte)0x79, (byte)0xe5, (byte)0x07, (byte)0x6d, (byte)0xde,
                    (byte)0xc2, (byte)0xf0, (byte)0x6c, (byte)0xb5, (byte)0x8f
            };

            public override void NextBytes(
                byte[] bytes)
            {
                int offset = 0;

                while ((offset + seed.Length) < bytes.Length)
                {
                    Array.Copy(seed, 0, bytes, offset, seed.Length);
                    offset += seed.Length;
                }

                Array.Copy(seed, 0, bytes, offset, bytes.Length- offset);
            }
        }

        public override string Name
        {
            get { return "BlockCipher"; }
        }

        private int GetIVLength(
            string algorithm)
        {
            string[] parts = algorithm.Split('/');

            if (parts.Length < 2)
                return 0;

            string mode = parts[1];

            int pos = mode.IndexOfAny(new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            string baseMode = pos < 0 ? mode : mode.Substring(0, pos);

            if (!validModes.Contains(baseMode))
                throw new Exception("Unhandled mode: " + mode);

            if (baseMode == "CCM")
                return 12;
            if (baseMode == "ECB")
                return 0;
            if (baseMode == "OCB")
                return 15;

            string baseAlgorithm = parts[0];
            IBufferedCipher baseCipher = CipherUtilities.GetCipher(baseAlgorithm);
            return baseCipher.GetBlockSize();
        }

        private void doTest(
            string	algorithm,
            byte[]	input,
            byte[]	output)
        {
            KeyParameter key = null;
            CipherKeyGenerator keyGen;
            SecureRandom rand;
            IBufferedCipher inCipher = null, outCipher = null;
            byte[] iv = null;
            CipherStream cIn, cOut;
            MemoryStream bIn, bOut;

            rand = new FixedSecureRandom();

            string[] parts = algorithm.ToUpper(CultureInfo.InvariantCulture).Split('/');
            string baseAlgorithm = parts[0];
            string mode = parts.Length > 1 ? parts[1] : null;

            try
            {
                keyGen = GeneratorUtilities.GetKeyGenerator(baseAlgorithm);

                // TODO Add Algorithm property to CipherKeyGenerator?
//				if (!keyGen.getAlgorithm().Equals(baseAlgorithm))
//				{
//					Fail("wrong key generator returned!");
//				}

                // TODO Add new Init method to CipherKeyGenerator?
//				keyGen.Init(rand);
                keyGen.Init(new KeyGenerationParameters(rand, keyGen.DefaultStrength));

                byte[] keyBytes = keyGen.GenerateKey();

                if (algorithm.StartsWith("RC5"))
                {
                    key = new RC5Parameters(keyBytes, rc5Rounds);
                }
                else
                {
                    key = ParameterUtilities.CreateKeyParameter(baseAlgorithm, keyBytes);
                }

                inCipher = CipherUtilities.GetCipher(algorithm);
                outCipher = CipherUtilities.GetCipher(algorithm);

                if (!inCipher.AlgorithmName.ToUpper(CultureInfo.InvariantCulture).StartsWith(baseAlgorithm))
                {
                    Fail("wrong cipher returned!");
                }

                ICipherParameters parameters = key;

                int ivLength = GetIVLength(algorithm);

                if (ivLength > 0)
                {
                    if (baseAlgorithm == "RC2")
                    {
                        iv = rc2IV;
                    }
                    else if (baseAlgorithm == "RC5")
                    {
                        iv = rc5IV;
                    }
                    else if (baseAlgorithm == "RC5-64")
                    {
                        iv = rc564IV;
                    }
                    else
                    {
                        // NB: rand always generates same values each test run
                        iv = SecureRandom.GetNextBytes(rand, ivLength);
                    }

                    parameters = new ParametersWithIV(key, iv);
                }

                // NB: 'rand' still needed e.g. for some paddings
                parameters = new ParametersWithRandom(parameters, rand);

                outCipher.Init(true, parameters);
            }
            catch (Exception e)
            {
                Fail("" + algorithm + " failed initialisation - " + e.ToString(), e);
            }

            //
            // grab the iv if there is one
            //
            try
            {
                // The Java version set this implicitly, but we set it explicity
                //byte[] iv = outCipher.getIV();

                if (iv != null)
                {
                    // TODO Examine short IV handling for these FIPS-compliant modes in Java build
                    if (mode.StartsWith("CCM")
                        || mode.StartsWith("CFB")
                        || mode.StartsWith("CTR")
                        || mode.StartsWith("EAX")
                        || mode.StartsWith("GCM")
                        || mode.StartsWith("GOFB")
                        || mode.StartsWith("OCB")
                        || mode.StartsWith("OFB")
                        || mode.StartsWith("OPENPGPCFB")
                        || mode.StartsWith("SIC"))
                    {
                        // These modes automatically pad out the IV if it is short
                    }
                    else
                    {
                        try
                        {
                            byte[] nIv = new byte[iv.Length- 1];
                            inCipher.Init(false, new ParametersWithIV(key, nIv));
                            Fail("failed to pick up short IV");
                        }
                        //catch (InvalidAlgorithmParameterException e)
                        catch (ArgumentException)
                        {
                            // ignore - this is what we want...
                        }
                    }

                    //IvParameterSpec spec = new IvParameterSpec(iv);
                    inCipher.Init(false, new ParametersWithIV(key, iv));
                }
                else
                {
                    inCipher.Init(false, key);
                }
            }
            catch (Exception e)
            {
                Fail("" + algorithm + " failed initialisation - " + e.ToString());
            }

            //
            // encryption pass
            //
            bOut = new MemoryStream();
            cOut = new CipherStream(bOut, null, outCipher);

            try
            {
                for (int i = 0; i != input.Length/ 2; i++)
                {
                    cOut.WriteByte(input[i]);
                }
                cOut.Write(input, input.Length/ 2, input.Length- input.Length/ 2);
                cOut.Close();
            }
            catch (IOException e)
            {
                Fail("" + algorithm + " failed encryption - " + e.ToString());
            }

            byte[] bytes = bOut.ToArray();

            if (!AreEqual(bytes, output))
            {
                Fail("" + algorithm + " failed encryption - expected "
                    + Hex.ToHexString(output) + " got "
                    + Hex.ToHexString(bytes));
            }

            //
            // decryption pass
            //
            bIn = new MemoryStream(bytes, false);
            cIn = new CipherStream(bIn, inCipher, null);

            try
            {
                BinaryReader dIn = new BinaryReader(cIn);

                bytes = new byte[input.Length];

                for (int i = 0; i != input.Length/ 2; i++)
                {
                    bytes[i] = dIn.ReadByte();
                }

                int remaining = bytes.Length - input.Length / 2;
                byte[] extra = dIn.ReadBytes(remaining);
                if (extra.Length < remaining)
                    throw new EndOfStreamException();
                extra.CopyTo(bytes, input.Length / 2);
            }
            catch (Exception e)
            {
                Fail("" + algorithm + " failed decryption - " + e.ToString());
            }

            if (!AreEqual(bytes, input))
            {
                Fail("" + algorithm + " failed decryption - expected "
                    + Hex.ToHexString(input) + " got "
                    + Hex.ToHexString(bytes));
            }
        }

        // TODO Make private again and call from PerformTest
        public void doTestExceptions()
        {
            // TODO Put back in
//			SecretKeyFactory skF = null;
//	        
//			try
//			{
//				skF = SecretKeyFactory.getInstance("DESede");
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}
//	        
//			KeySpec ks = null;
//			SecretKey secKey = null;
//			byte[] bb = new byte[24];
//
//			try
//			{
//				skF.getKeySpec(null, null);
//	            
//				Fail("failed exception test - no exception thrown");
//			}
//			catch (InvalidKeySpecException e)
//			{
//				// ignore okay
//			}
//			catch (Exception e)
//			{
//				Fail("failed exception test.", e);
//			}
//			try
//			{
//				ks = (KeySpec)new DESedeKeySpec(bb);
//				skF.getKeySpec(null, ks.getClass());
//	            
//				Fail("failed exception test - no exception thrown");
//			}
//			catch (InvalidKeySpecException e)
//			{
//				// ignore okay;
//			}
//			catch (Exception e)
//			{
//				Fail("failed exception test.", e);
//			}
//			try
//			{
//				skF.getKeySpec(secKey, null);
//			}
//			catch (InvalidKeySpecException e)
//			{
//				// ignore okay
//			}
//			catch (Exception e)
//			{
//				Fail("failed exception test.", e);
//			}
            
            try
            {
                CipherKeyGenerator kg = GeneratorUtilities.GetKeyGenerator("DESede");

                try
                {
                    kg.Init(new KeyGenerationParameters(new SecureRandom(), int.MinValue));

                    Fail("failed exception test - no exception thrown");
                }
//				catch (InvalidParameterException)
                catch (ArgumentException)
                {
                    // ignore okay
                }
                catch (Exception e)
                {
                    Fail("failed exception test.", e);
                }
            }
            catch (Exception e)
            {
                Fail("unexpected exception.", e);
            }

            // TODO Put back in
//			try
//			{
//				skF = SecretKeyFactory.getInstance("DESede");
//
//				try
//				{
//					skF.translateKey(null);
//	                
//					Fail("failed exception test - no exception thrown");
//				}
//				catch (InvalidKeyException)
//				{
//					// ignore okay
//				}
//				catch (Exception e)
//				{
//					Fail("failed exception test.", e);
//				}
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}
            
//			try
//			{
//				byte[] rawDESKey = { (byte)128, (byte)131, (byte)133, (byte)134,
//						(byte)137, (byte)138, (byte)140, (byte)143 };
//
////				SecretKeySpec cipherKey = new SecretKeySpec(rawDESKey, "DES");
//				KeyParameter cipherKey = new DesParameters(rawDESKey);
//
//				IBufferedCipher cipher = CipherUtilities.GetCipher("DES/CBC/NoPadding");
//
//				try
//				{
//					// According specification engineInit(int opmode, Key key,
//					// SecureRandom random) throws InvalidKeyException if this
//					// cipher is being
//					// initialized for decryption and requires algorithm parameters
//					// that cannot be determined from the given key
////					cipher.Init(false, cipherKey, (SecureRandom)null);
//					cipher.Init(false, new ParametersWithRandom(cipherKey, new SecureRandom()));
//
//					Fail("failed exception test - no InvalidKeyException thrown");
//				}
//				catch (InvalidKeyException)
//				{
//					// ignore
//				}
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}

            try
            {
//				byte[] rawDESKey = { -128, -125, -123, -122, -119, -118 };
                byte[] rawDESKey = { 128, 131, 133, 134, 137, 138 };

//				SecretKeySpec cipherKey = new SecretKeySpec(rawDESKey, "DES");

//				IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/NoPadding");
                try
                {
                    KeyParameter cipherKey = new DesParameters(rawDESKey);

                    // According specification engineInit(int opmode, Key key,
                    // SecureRandom random) throws InvalidKeyException if the given
                    // key is inappropriate for initializing this cipher
//					cipher.Init(true, cipherKey);

//					Fail("failed exception test - no InvalidKeyException thrown");
                    Fail("failed exception test - no ArgumentException thrown");
                }
//				catch (InvalidKeyException)
                catch (ArgumentException)
                {
                    // ignore
                }
            }
            catch (Exception e)
            {
                Fail("unexpected exception.", e);
            }

//			try
//			{
////				byte[] rawDESKey = { -128, -125, -123, -122, -119, -118, -117, -115, -114 };
//				byte[] rawDESKey = { 128, 131, 133, 134, 137, 138, 139, 141, 142 };
//
////				SecretKeySpec cipherKey = new SecretKeySpec(rawDESKey, "DES");
//				KeyParameter cipherKey = new DesParameters(rawDESKey);
//
//				IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/NoPadding");
//				try
//				{
//					// According specification engineInit(int opmode, Key key,
//					// SecureRandom random) throws InvalidKeyException if the given
//					// key is inappropriate for initializing this cipher
//					cipher.Init(true, cipherKey);
//	                
//					Fail("failed exception test - no InvalidKeyException thrown");
//				}
//				catch (InvalidKeyException)
//				{
//					// ignore
//				}
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}


            try
            {
                byte[] rawDESKey = { (byte)128, (byte)131, (byte)133, (byte)134,
                    (byte)137, (byte)138, (byte)140, (byte)143 };

//				SecretKeySpec cipherKey = new SecretKeySpec(rawDESKey, "DES");
                KeyParameter cipherKey = new DesParameters(rawDESKey);

                IBufferedCipher ecipher = CipherUtilities.GetCipher("DES/ECB/PKCS5Padding");
                ecipher.Init(true, cipherKey);

                byte[] cipherText = new byte[0];
                try
                {
                    // According specification Method engineUpdate(byte[] input,
                    // int inputOffset, int inputLen, byte[] output, int
                    // outputOffset)
                    // throws ShortBufferException - if the given output buffer is
                    // too
                    // small to hold the result
//					ecipher.update(new byte[20], 0, 20, cipherText);
                    ecipher.ProcessBytes(new byte[20], 0, 20, cipherText, 0);

//					Fail("failed exception test - no ShortBufferException thrown");
                    Fail("failed exception test - no DataLengthException thrown");
                }
//				catch (ShortBufferException)
                catch (DataLengthException)
                {
                    // ignore
                }
            }
            catch (Exception e)
            {
                Fail("unexpected exception.", e);
            }

            // TODO Put back in
//			try
//			{
//				KeyGenerator keyGen = KeyGenerator.getInstance("DES");
//
//				keyGen.init((SecureRandom)null);
//
//				// According specification engineGenerateKey() doesn't throw any exceptions.
//
//				SecretKey key = keyGen.generateKey();
//				if (key == null)
//				{
//					Fail("key is null!");
//				}
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}
//
//			try
//			{
//				AlgorithmParameters algParams = AlgorithmParameters.getInstance("DES");
//	            
//				algParams.init(new IvParameterSpec(new byte[8]));
//
//				// According specification engineGetEncoded() returns
//				// the parameters in their primary encoding format. The primary
//				// encoding
//				// format for parameters is ASN.1, if an ASN.1 specification for
//				// this type
//				// of parameters exists.
//				byte[] iv = algParams.getEncoded();
//	            
//				if (iv.Length!= 10)
//				{
//					Fail("parameters encoding wrong length - "  + iv.Length);
//				}
//			}
//			catch (Exception e)
//			{
//				Fail("unexpected exception.", e);
//			}

            try
            {
                try
                {
//					AlgorithmParameters algParams = AlgorithmParameters.getInstance("DES");

                    byte[] encoding = new byte[10];
                    encoding[0] = 3;
                    encoding[1] = 8;

//					algParams.init(encoding, "ASN.1");
                    ParameterUtilities.GetCipherParameters(
                        "AES",
                        ParameterUtilities.CreateKeyParameter("AES", new byte[16]),
                        Asn1Object.FromByteArray(encoding));

//					Fail("failed exception test - no IOException thrown");
                    Fail("failed exception test - no Exception thrown");
                }
//				catch (IOException)
                catch (ArgumentException)
                {
                    // okay
                }

//				try
//				{
//					IBufferedCipher c = CipherUtilities.GetCipher("DES");
//
//					Key k = new PublicKey()
//					{
//
//						public string getAlgorithm()
//						{
//							return "STUB";
//						}
//
//						public string getFormat()
//						{
//							return null;
//						}
//
//						public byte[] getEncoded()
//						{
//							return null;
//						}
//	                    
//					};
//	    
//					c.Init(true, k);
//	    
//					Fail("failed exception test - no InvalidKeyException thrown for public key");
//				}
//				catch (InvalidKeyException e)
//				{
//					// okay
//				}
//	            
//				try
//				{
//					IBufferedCipher c = CipherUtilities.GetCipher("DES");
//	    
//					Key k = new PrivateKey()
//					{
//
//						public string getAlgorithm()
//						{
//							return "STUB";
//						}
//
//						public string getFormat()
//						{
//							return null;
//						}
//
//						public byte[] getEncoded()
//						{
//							return null;
//						}
//	                    
//					};
//	    
//					c.Init(false, k);
//	    
//					Fail("failed exception test - no InvalidKeyException thrown for private key");
//				}
//				catch (InvalidKeyException e)
//				{
//					// okay
//				}
            }
            catch (Exception e)
            {
                Fail("unexpected exception.", e);
            }
        }

        public override void PerformTest()
        {
            for (int i = 0; i != cipherTests1.Length; i += 2)
            {
                doTest(cipherTests1[i], input1, Hex.Decode(cipherTests1[i + 1]));
            }

            for (int i = 0; i != cipherTests2.Length; i += 2)
            {
                doTest(cipherTests2[i], input2, Hex.Decode(cipherTests2[i + 1]));
            }

            //
            // check for less than a block
            //
            try
            {
                IBufferedCipher c = CipherUtilities.GetCipher("AES/CTS/NoPadding");

                c.Init(true, ParameterUtilities.CreateKeyParameter("AES", new byte[16]));

                c.DoFinal(new byte[4]);

                Fail("CTS failed to throw exception");
            }
            catch (Exception e)
            {
//				if (!(e is IllegalBlockSizeException))
                if (!(e is DataLengthException))
                {
                    Fail("CTS exception test - " + e, e);
                }
            }

            doTestExceptions();
        }

        public static void Main(
            string[] args)
        {
            RunTest(new BlockCipherTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
