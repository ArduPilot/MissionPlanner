using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;

namespace Core.Xml
{
    public static class XMLSerializeManager
    {
        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
        public static XmlSerializer GetSerializer(Type t)
        {
            if (!serializers.ContainsKey(t)) {
                serializers[t] = new XmlSerializer(t);
            }
            return serializers[t];
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }
        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public static MemoryStream SerializeObjectToMemoryStream(Object pObject, Type pType)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = GetSerializer(pType);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            m_Serializing = true;
            xs.Serialize(xmlTextWriter, pObject, xmlns);
            m_Serializing = false;
            return (MemoryStream)xmlTextWriter.BaseStream;
        }

        public static String SerializeObject(Object pObject, Type pType)
        {
            return UTF8ByteArrayToString(SerializeObjectToMemoryStream(pObject, pType).ToArray());
        }

        public static void SerializeObjectToFile(Object pObject, Type pType, string file_path)
        {
            SerializeObjectToFile(pObject, pType, file_path, true);
        }
        public static void SerializeObjectToFile(Object pObject, Type pType, string file_path, bool useSafeOverwrite)
        {
            string streamPath = (useSafeOverwrite) ? file_path + ".tmp" : file_path;
            bool success = false;
            string directory = Path.GetDirectoryName(file_path);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            FileStream fs = new FileStream(streamPath, FileMode.Create);
            XmlSerializer xs = GetSerializer(pType);
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            //xmlns.Add("", "");
            try {
                m_Serializing = true;
                xs.Serialize(fs, pObject, xmlns);
                success = true;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                m_Serializing = false;
            }

            fs.Close();

            if (success && useSafeOverwrite) {
                try {
                    File.Delete(file_path);
                    File.Move(streamPath, file_path);
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
        }

        public static MemoryStream DeserializeStringToMemoryStream(String pXmlizedString)
        {
            return new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        }

        public static object DeserializeObject(MemoryStream stream, Type pType)
        {
            XmlSerializer xs = GetSerializer(pType);
            try {
                m_Deserializing = true;
                return xs.Deserialize(stream);
            }
            catch {
                return null;
            }
            finally {
                m_Deserializing = false;
            }
        }

        public static object DeserializeObject(String pXmlizedString, Type pType)
        {
            return DeserializeObject(DeserializeStringToMemoryStream(pXmlizedString), pType);
        }

        public static object DeserializeObjectFromFile(string file_path, Type pType)
        {
            XmlSerializer xs = GetSerializer(pType);
            FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            m_Deserializing = true;
            Object ans = xs.Deserialize(fs);
            m_Deserializing = false;
            fs.Close();
            return ans;
        }

        public static bool Verbose;
        internal static void Report(string message)
        {
            if (Verbose) {
                Debug.Print(message);
            }
        }

        private static bool m_Serializing;
        public static bool Serializing
        {
            get
            {
                return m_Serializing;
            }            
        }

        private static bool m_Deserializing;
        public static bool Deserializing
        {
            get
            {
                return m_Deserializing;
            }            
        }
    }

    public class XmlSerializer<T>
    {
        public static string SerializeObject(T obj)
        {
            return XMLSerializeManager.SerializeObject(obj, typeof(T));
        }

        public static MemoryStream SerializeObjectToMemoryStream(T obj)
        {
            return XMLSerializeManager.SerializeObjectToMemoryStream(obj, typeof(T));
        }

        public static void SerializeObjectToFile(T obj, string file_path, bool useSafeOverwrite)
        {
            XMLSerializeManager.SerializeObjectToFile(obj, typeof(T), file_path, useSafeOverwrite);
        }
        public static void SerializeObjectToFile(T obj, string file_path)
        {
            XMLSerializeManager.SerializeObjectToFile(obj, typeof(T), file_path);
        }

        public static T DeserializeObject(MemoryStream stream)
        {
            return (T)XMLSerializeManager.DeserializeObject(stream, typeof(T));
        }

        public static T DeserializeObject(string xmlStr)
        {
            return (T)XMLSerializeManager.DeserializeObject(xmlStr, typeof(T));
        }

        public static T DeserializeObjectFromFile(string file_path)
        {
            return (T)XMLSerializeManager.DeserializeObjectFromFile(file_path, typeof(T));
        }
    }
}
