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
   public static class ParameterMetaDataRepository
   {
      private static XDocument _parameterMetaDataXML;
      private static XDocument _parameterMetaDataXMLEn;

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
          string paramMetaDataXMLFileName = String.Format("{0}{1}{2}", Application.StartupPath, Path.DirectorySeparatorChar, ConfigurationManager.AppSettings["ParameterMetaDataXMLFileName"]);
          string paramMetaDataXMLFileNameEn = paramMetaDataXMLFileName;
          bool isNotEn = false;

          switch (System.Globalization.CultureInfo.CurrentUICulture.Name)
          {
              case "zh-Hans":
                  isNotEn = true;
                  paramMetaDataXMLFileName = String.Format("{0}{1}{2}", Application.StartupPath, Path.DirectorySeparatorChar, "ParameterMetaData.zh-Hans.xml");
                  break;
              case "zh-CN":
                  isNotEn = true;
                  paramMetaDataXMLFileName = String.Format("{0}{1}{2}", Application.StartupPath, Path.DirectorySeparatorChar, "ParameterMetaData.zh-Hans.xml");
                  break;
              default:
                  break;
          }

          string paramMetaDataXMLFileNameBackup = String.Format("{0}{1}{2}", Application.StartupPath, Path.DirectorySeparatorChar, ConfigurationManager.AppSettings["ParameterMetaDataXMLFileNameBackup"]);

          try
          {
              if (File.Exists(paramMetaDataXMLFileName))
                  _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileName);
              if (isNotEn)
                  if (File.Exists(paramMetaDataXMLFileNameEn))
                      _parameterMetaDataXMLEn = XDocument.Load(paramMetaDataXMLFileNameEn);
                  else if (File.Exists(paramMetaDataXMLFileNameBackup))
                      _parameterMetaDataXMLEn = XDocument.Load(paramMetaDataXMLFileNameBackup);

          }
          catch (Exception ex) { Console.WriteLine(ex.ToString()); } // Exception System.Xml.XmlException: Root element is missing.

          try
          {
              // error loading the good file, load the backup
              if (File.Exists(paramMetaDataXMLFileNameBackup) && _parameterMetaDataXML == null)
              {
                  _parameterMetaDataXML = XDocument.Load(paramMetaDataXMLFileNameBackup);
                  Console.WriteLine("Using backup param data");
              }
          }
          catch { }
      
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

         if(_parameterMetaDataXML != null)
         {
            // Use this to find the endpoint node we are looking for
            // Either it will be pulled from a file in the ArduPlane hierarchy or the ArduCopter hierarchy
             try
             {
                 var elements = _parameterMetaDataXML.Element("Params").Elements(vechileType);

                 foreach (var element in elements)
                 {
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
             }
             catch { } // Exception System.ArgumentException: '' is an invalid expanded name.
         }

         if (_parameterMetaDataXMLEn != null)
         {
             try
             {
                 var element = _parameterMetaDataXMLEn.Element("Params").Element(vechileType);
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
             catch { }
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
      public static List<KeyValuePair<int, string>> GetParameterOptionsInt(string nodeKey, string vechileType)
      {
          CheckLoad();

          string availableValuesRaw;

          if (nodeKey == "FLTMODE1" && _parameterMetaDataXMLEn != null)
          {
              _parameterMetaDataXML = _parameterMetaDataXMLEn;
              availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Values, vechileType);
              Reload();
          }
          else
          {
              availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Values, vechileType);
          }
          

          string[] availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (availableValues.Any())
          {
              var splitValues = new List<KeyValuePair<int, string>>();
              // Add the values to the ddl
              foreach (string val in availableValues)
              {
                  try
                  {
                      string[] valParts = val.Split(new[] { ':' });
                      splitValues.Add(new KeyValuePair<int, string>(int.Parse(valParts[0].Trim()), (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                  }
                  catch { Console.WriteLine("Bad entry in param meta data: " + nodeKey); }
              };

              return splitValues;
          }

          return new List<KeyValuePair<int, string>>();
      }

      public static List<KeyValuePair<int, string>> GetParameterBitMaskInt(string nodeKey, string vechileType)
      {
          CheckLoad();

          string availableValuesRaw;

          availableValuesRaw = GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Bitmask, vechileType);

          string[] availableValues = availableValuesRaw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (availableValues.Any())
          {
              var splitValues = new List<KeyValuePair<int, string>>();
              // Add the values to the ddl
              foreach (string val in availableValues)
              {
                  try
                  {
                      string[] valParts = val.Split(new[] { ':' });
                      splitValues.Add(new KeyValuePair<int, string>(int.Parse(valParts[0].Trim()), (valParts.Length > 1) ? valParts[1].Trim() : valParts[0].Trim()));
                  }
                  catch { Console.WriteLine("Bad entry in param meta data: " + nodeKey); }
              };

              return splitValues;
          }

          return new List<KeyValuePair<int, string>>();
      }

      public static bool GetParameterRange(string nodeKey, ref double min, ref double max, string vechileType)
      {
          CheckLoad();

          string rangeRaw = ParameterMetaDataRepository.GetParameterMetaData(nodeKey, ParameterMetaDataConstants.Range, vechileType);

          string[] rangeParts = rangeRaw.Split(new[] { ' ' });
          if (rangeParts.Count() == 2)
          {
              float lowerRange;
              if (float.TryParse(rangeParts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out lowerRange))
              {
                  float upperRange;
                  if (float.TryParse(rangeParts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out upperRange))
                  {
                      min = lowerRange;
                      max = upperRange;

                      return true;
                  }
              }
          }

          return false;
      }

      public static bool GetParameterRebootRequired(string nodeKey, string vechileType)
      {
          // set the default answer
          bool answer = false;

          CheckLoad();

          string rebootrequired = ParameterMetaDataRepository.GetParameterMetaData(nodeKey, ParameterMetaDataConstants.RebootRequired, vechileType);

          if (!string.IsNullOrEmpty(rebootrequired))
          {
              bool.TryParse(rebootrequired, out answer);
          }

          return answer;
      }
   }
}
