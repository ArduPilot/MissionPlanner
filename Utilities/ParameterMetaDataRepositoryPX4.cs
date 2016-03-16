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

                    var nodeKeyLower = nodeKey.ToLower();

                    var groups = _parameterMetaDataXML.Element("parameters").Elements("group");

                    foreach (var group in groups)
                    {
                        if (group != null && group.HasElements)
                        {
                            var parameters = group.Elements("parameter");

                            foreach (var parameter in parameters)
                            {
                                if (parameter != null && parameter.HasElements)
                                {
                                    // match param name
                                    var node = parameter.Attribute("name");
                                    if (node.Value.ToLower() == nodeKeyLower)
                                    {
                                        if (metaKey == "values")
                                        {
                                            try
                                            {
                                                var values = parameter.Element("values");
                                                if (values == null)
                                                    return string.Empty;
                                                var valuearray = values.Elements("value");
                                                string value = "";
                                                foreach (var valueelement in valuearray)
                                                {
                                                    var no = valueelement.Attribute("code");
                                                    if (no == null)
                                                        continue;
                                                    var val = valueelement.Value;
                                                    if (val == null)
                                                        continue;
                                                    value += no.Value + ":" + val + ",";
                                                }

                                                return value.TrimEnd(',');
                                            }
                                            catch
                                            {
                                                return string.Empty;
                                            }
                                        }
                                        else if (metaKey.ToLower() == "range")
                                        {
                                            return GetParameterMetaData(nodeKey, "min") + " " + GetParameterMetaData(nodeKey, "max");
                                        }
                                        else
                                        {
                                            var key = parameter.Element(metaKey);
                                            if (key != null)
                                            {
                                                return key.Value;
                                            }
                                            else
                                            {
                                                return string.Empty;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                } // Exception System.ArgumentException: '' is an invalid expanded name.
            }

            return string.Empty;
        }
    }
}