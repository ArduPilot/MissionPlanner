using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace MissionPlanner.Utilities
{
    public class ParameterMetaDataRepositoryPX4 
    {
        private static XDocument _parameterMetaDataXML;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetaDataRepository"/> class.
        /// </summary>
       public static void CheckLoad()
        {
            if (_parameterMetaDataXML == null)
                Reload();
        }

        public static void Reload()
        {
            string paramMetaDataXMLFileName = String.Format("{0}{1}{2}", Application.StartupPath,
                Path.DirectorySeparatorChar, "ParameterFactMetaData.xml");

            string paramMetaDataXMLFileNameBackup = String.Format("{0}{1}{2}", Application.StartupPath,
                Path.DirectorySeparatorChar, "ParameterFactMetaData.xml");

            try
            {
                if (File.Exists(paramMetaDataXMLFileName))
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileName);

                // error loading the good file, load the backup
                if (File.Exists(paramMetaDataXMLFileNameBackup) && _parameterMetaDataXML == null)
                {
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileNameBackup);
                    Console.WriteLine("Using backup param data");
                }
            }
            catch
            {
            }
        }

        static string ConvertMetaKey(string input)
        {
            if (input == ParameterMetaDataConstants.DisplayName)
                return "short_desc";

            if (input == ParameterMetaDataConstants.Range)
                return "range";

            if (input == ParameterMetaDataConstants.Range)
                return "range";

            if (input == ParameterMetaDataConstants.Description)
                return "long_desc";

            if (input == ParameterMetaDataConstants.Increment)
                return "increment";

            if (input == ParameterMetaDataConstants.Units)
                return "unit";

            if (input == ParameterMetaDataConstants.Values)
                return "values";

            return input;
        }

        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public static string GetParameterMetaData(string nodeKey, string metaKey, string vechileType = "")
        {
            CheckLoad();

            if (_parameterMetaDataXML != null)
            {
                metaKey = ConvertMetaKey(metaKey);

                try
                {
                    //parameters - group - parameter
                    //metakeys - short_desc min max decimal long_desc increment unit
                    //values value

                    var reader = _parameterMetaDataXML.CreateReader();

                    reader.ReadToFollowing("parameters");

                    while (reader.ReadToFollowing("parameter"))
                    {
                        for (int a = 0; a < reader.AttributeCount; a++)
                        {
                            reader.MoveToAttribute(a);
                            Console.WriteLine("{0} = {1}", reader.Name, reader.Value);

                            // we found the param name we are looking for
                            if (reader.Name.ToLower() == "name" && reader.Value.ToLower() == nodeKey.ToLower())
                            {
                                if (metaKey == "values")
                                {
                                    if (reader.ReadToFollowing("values"))
                                    {
                                        reader.ReadStartElement();

                                        var value = "";

                                        do
                                        {
                                            if (reader.Name == "value")
                                            {
                                                if (reader.MoveToFirstAttribute())
                                                {
                                                    var no = reader.Value;
                                                    var val = reader.ReadString();

                                                    value += no + ":" + val + ",";

                                                    //Console.WriteLine("{0} = {1}", reader.Name, value);
                                                }
                                            }
                                        } while (reader.ReadToNextSibling("value"));

                                        return value.TrimEnd(',');
                                    }
                                }
                                else if (metaKey.ToLower() == "range")
                                {
                                    return GetParameterMetaData(nodeKey,"min") + " " + GetParameterMetaData(nodeKey,"max");
                                }
                                else
                                {
                                    if (reader.ReadToFollowing(metaKey))
                                    {
                                        var value = reader.ReadString();

                                        //Console.WriteLine("{0} = {1}", reader.Name, value);

                                        return value;
                                    }
                                }
                            }
                        }
                    }

                    return string.Empty;
                }
                catch
                {
                } // Exception System.ArgumentException: '' is an invalid expanded name.
            }

            return string.Empty;
        }
    }
}