using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace MissionPlanner.Utilities
{
    public class ParameterMetaDataRepositoryAutoTest
    {
        private static XDocument _parameterMetaDataXML;

        //http://autotest.diydrones.com/Parameters/apm.pdef.xml

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetaDataRepository"/> class.
        /// </summary>
        public ParameterMetaDataRepositoryAutoTest()
        {
            if (_parameterMetaDataXML == null)
                Reload();
        }

        public static void Reload()
        {
            string paramMetaDataXMLFileName = String.Format("{0}{1}{2}", Application.StartupPath,
                Path.DirectorySeparatorChar, "apm.pdef.xml");

            string paramMetaDataXMLFileNameBackup = String.Format("{0}{1}{2}", Application.StartupPath,
                Path.DirectorySeparatorChar, "apm.pdef.xml");

            try
            {
                if (File.Exists(paramMetaDataXMLFileName))
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileName);
            }
            catch
            {
            } // Exception System.Xml.XmlException: Root element is missing.

            try
            {
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

        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public string GetParameterMetaData(string nodeKey, string metaKey)
        {
            if (_parameterMetaDataXML != null)
            {
                // Use this to find the endpoint node we are looking for
                // Either it will be pulled from a file in the ArduPlane hierarchy or the ArduCopter hierarchy
                try
                {
                    MainV2.Firmwares selected = MainV2.comPort.MAV.cs.firmware;

                    var element = _parameterMetaDataXML.Element("paramfile").Element(selected.ToString());
                    if (element != null && element.HasElements)
                    {
                        var node = element.Element(nodeKey);
                        if (node != null && node.HasElements)
                        {
                            var metaValue = node.Element(metaKey);
                            if (metaValue != null)
                            {
                                return metaValue.Value;
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

        /// <summary>
        /// Return a key, value list off all options selectable
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        /*    public List<KeyValuePair<string,string>> GetParameterOptions(string nodeKey)
      {
          string availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Values);

          string[] availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (availableValues.Any())
          {
              var splitValues = new List<KeyValuePair<string, string>>();
              // Add the values to the ddl
              foreach (string val in availableValues)
              {
                  string[] valParts = val.Split(new[] { ':' });
                  splitValues.Add(new KeyValuePair<string, string>(valParts[0].Trim(), (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
              };

              return splitValues;
          }

          return new List<KeyValuePair<string, string>>();
      }*/
        /// <summary>
        /// Return a key, value list off all options selectable
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        public List<KeyValuePair<int, string>> GetParameterOptionsInt(string nodeKey)
        {
            string availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Values);

            string[] availableValues = availableValuesRaw.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (availableValues.Any())
            {
                var splitValues = new List<KeyValuePair<int, string>>();
                // Add the values to the ddl
                foreach (string val in availableValues)
                {
                    string[] valParts = val.Split(new[] {':'});
                    splitValues.Add(new KeyValuePair<int, string>(int.Parse(valParts[0].Trim()),
                        (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                }
                ;

                return splitValues;
            }

            return new List<KeyValuePair<int, string>>();
        }
    }
}