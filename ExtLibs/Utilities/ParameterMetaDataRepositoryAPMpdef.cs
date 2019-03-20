using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using log4net;

namespace MissionPlanner.Utilities
{
    public static class ParameterMetaDataRepositoryAPMpdef
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static XDocument _parameterMetaDataXML;

        //http://autotest.ardupilot.org/Parameters/apm.pdef.xml

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
            string paramMetaDataXMLFileName = String.Format("{0}{1}", Settings.GetUserDataDirectory(), "apm.pdef.xml");

            try
            {
                if (File.Exists(paramMetaDataXMLFileName))
                    _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileName);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public static string GetParameterMetaData(string nodeKey, string metaKey, string vechileType)
        {
            CheckLoad();

            // remap names
            if (vechileType == "ArduCopter2")
                vechileType = "ArduCopter";
            if (vechileType == "ArduRover")
                vechileType = "APMrover2";
            if (vechileType == "ArduTracker")
                vechileType = "AntennaTracker";

            // remap keys
            if (metaKey == ParameterMetaDataConstants.DisplayName)
                metaKey = "humanName";
            if (metaKey == ParameterMetaDataConstants.Description)
                metaKey = "documentation";
            if (metaKey == ParameterMetaDataConstants.User)
                metaKey = "user";

            if (_parameterMetaDataXML != null)
            {
                try
                {
                    foreach (var paramfile in _parameterMetaDataXML.Element("paramfile").Elements())
                    {
                        foreach (var parameters in paramfile.Elements())
                        {
                            if (parameters.HasAttributes)
                            {
                                foreach (var param in parameters.Elements())
                                {
                                    if (param.Attribute("name").Value == (vechileType + ":" + nodeKey) ||
                                        param.Attribute("name").Value == nodeKey)
                                    {
                                        if (param.Attribute(metaKey) != null)
                                        {
                                            return param.Attribute(metaKey).Value;
                                        }
                                        if (metaKey == ParameterMetaDataConstants.Values)
                                        {
                                            var ans = "";
                                            param.Elements("values").Elements().ForEach(a =>
                                            {
                                                if (a.Name == "value")
                                                {
                                                    var code = a.Attribute("code");
                                                    var value = a.Value.ToString();
                                                    ans += String.Format("{0}:{1},", code.Value, value);
                                                }
                                            });
                                            return ans;
                                        }
                                        foreach (var xElement in param.Elements())
                                        {
                                            if (xElement.Name == "field")
                                            {
                                                var name = xElement.Attribute("name");
                                                if (name != null && name.Value == metaKey)
                                                {
                                                    return xElement.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                } 
            }

            return string.Empty;
        }
    }
}