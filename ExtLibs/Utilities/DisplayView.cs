using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace MissionPlanner.Utilities
{
    [Serializable]
    public enum DisplayNames
    {
        Basic,
        Advanced
    }
    [Serializable]
    public class DisplayView
    {
        public DisplayNames displayName { get; set; }
        public Boolean displaySimulation { get; set; }
        public Boolean displayTerminal { get; set; }
        public Boolean displayDonate { get; set; }
        public Boolean displayHelp { get; set; }
        public Boolean displayAnenometer { get; set; }
        public Boolean displayAdvActionsTab { get; set; }
        public Boolean displaySimpleActionsTab { get; set; }
        public Boolean displayStatusTab { get; set; }
        public Boolean displayServoTab { get; set; }
        public Boolean displayScriptsTab { get; set; }
        public Boolean displayAdvancedParams { get; set; }
        public Boolean displayFullParamList { get; set; }
        public Boolean displayFullParamTree { get; set; }
        public Boolean displayBaudCMB { get; set; }
        public Boolean displaySerialPortCMB { get; set; }
        public bool isAdvancedMode { get; set; }

        public DisplayView()
        {
            // default to basic.
            //also when a new field is added/created this defines the template for missing options
            displayName = DisplayNames.Basic;
            displaySimulation = false;
            displayTerminal = false;
            displayDonate = true;
            displayHelp = true;
            displayAnenometer = true;
            displayAdvActionsTab = false;
            displaySimpleActionsTab = true;
            displayStatusTab = false;
            displayServoTab = false;
            displayScriptsTab = false;
            displayAdvancedParams = false;
            displayFullParamList = false;
            displayFullParamTree = false;
            displayBaudCMB = true;
            displaySerialPortCMB = true;
            isAdvancedMode = false;
        }
    }
    public static class DisplayViewExtensions
    {
        public static bool TryParse(string value, out DisplayView result)
        {
            result = new DisplayView();
            var serializer = new XmlSerializer(result.GetType());


            using (TextReader reader = new StringReader(value))
            {
                try
                {
                    result = (DisplayView)serializer.Deserialize(reader);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public static string ConvertToString(this DisplayView v)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(v.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, v);
                return textWriter.ToString();
            }
        }
        public static DisplayView Basic(this DisplayView v)
        {
            return new DisplayView()
            {
                displayName = DisplayNames.Basic,
                displaySimulation = false,
                displayTerminal = false,
                displayDonate = true,
                displayHelp = true,
                displayAnenometer = true,
                displayAdvActionsTab = false,
                displaySimpleActionsTab = true,
                displayStatusTab = false,
                displayServoTab = false,
                displayScriptsTab = false,
                displayAdvancedParams = false,
                displayFullParamList = false,
                displayFullParamTree = false,
                displayBaudCMB = true,
                displaySerialPortCMB = true,
                isAdvancedMode = false
            };
        }
        public static DisplayView Advanced(this DisplayView v)
        {
            return new DisplayView()
            {
                displayName = DisplayNames.Advanced,
                displaySimulation = true,
                displayTerminal = true,
                displayDonate = true,
                displayHelp = true,
                displayAnenometer = true,
                displayAdvActionsTab = true,
                displaySimpleActionsTab = false,
                displayStatusTab = true,
                displayServoTab = true,
                displayScriptsTab = true,
                displayAdvancedParams = true,
                displayFullParamList = true,
                displayFullParamTree = true,
                displayBaudCMB = true,
                displaySerialPortCMB = true,
                isAdvancedMode = true
            };
        }
    }
}