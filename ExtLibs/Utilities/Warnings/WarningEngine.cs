using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Utilities;

namespace MissionPlanner.Warnings
{
    public class WarningEngine
    {
        public static List<CustomWarning> warnings = new List<CustomWarning>();

        public static string warningconfigfile = Settings.GetUserDataDirectory() + "warnings.xml";

        static bool run = false;

        static WarningEngine()
        {
            try
            {
                LoadConfig();
            }
            catch
            {
                Console.WriteLine("Failed to read Warning config file " + warningconfigfile);
            }
        }

        public static void LoadConfig()
        {
            if (!File.Exists(warningconfigfile))
                return;

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof (List<CustomWarning>),
                    new Type[] {typeof (CustomWarning)});

            using (StreamReader sr = new StreamReader(warningconfigfile))
            {
                warnings = (List<CustomWarning>) reader.Deserialize(sr);
            }
        }

        public static void SaveConfig()
        {
            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof (List<CustomWarning>),
                    new Type[] {typeof (CustomWarning)});

            using (StreamWriter sw = new StreamWriter(warningconfigfile))
            {
                lock (warnings)
                {
                    writer.Serialize(sw, warnings);
                }
            }
        }

        public static event EventHandler<string> WarningMessage;

        //Called for QV Panel background changes
        //first arg: datasource field name, second arg: colorname
        //Nota bene: using Action is not elegant, but quick and since it used only one other place, no harm done
        public static event Action<string, string> QuickPanelColoring;

        public static void Start(ISpeech speech)
        {
            if (run == false)
            {
                MainLoop();
            }

            _speech = speech;
        }

        public static void Stop()
        {
            run = false;
        }

        private static ISpeech _speech;

        public static async Task MainLoop()
        {
            run = true;
            while (run)
            {

                try
                {
                    lock (warnings)
                    {
                        foreach (var item in warnings)
                        {
                            // check primary condition
                            if (checkCond(item))
                            {
                                //Check item type
                                if (item.type == CustomWarning.WarningType.SpeakAndText)
                                {
                                    if (_speech != null)
                                    {
                                        while (!_speech.IsReady)
                                            Thread.Yield();

                                        _speech.SpeakAsync(item.SayText());
                                    }

                                    WarningMessage?.Invoke(null, item.SayText());
                                }
                                else if (item.type == CustomWarning.WarningType.Coloring)
                                {
                                    QuickPanelColoring?.Invoke(item.Name, item.color);
                                }
                            }
                            // if condition is not met, then color back the QV panel to default BackGround
                            else if (item.type == CustomWarning.WarningType.Coloring)
                            {
                                QuickPanelColoring?.Invoke(item.Name, "NoColor");
                            }
                        }
                    }
                }
                catch
                {
                }

                await Task.Delay(250).ConfigureAwait(false);
            }
        }

        static bool checkCond(CustomWarning item)
        {
            // if there is a child go recursive
            if (item.Child != null)
            {
                if (item.CheckValue() && checkCond(item.Child))
                    return true;
            }
            else
            {
                // is no child then simple check
                if (item.CheckValue())
                    return true;
            }

            return false;
        }
    }
}