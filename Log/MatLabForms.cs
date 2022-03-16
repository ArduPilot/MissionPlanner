﻿using System;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;

namespace MissionPlanner.Log
{
    public class MatLabForms
    {
        public static void ProcessTLog()
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        try
                        {
                            MissionPlanner.Log.MatLab.tlog(logfile);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Error converting file " + ex.ToString(), Strings.ERROR);
                        }
                    }
                }
            }
        }

        public static void ProcessLog()
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Log Files|*.log;*.bin;*.BIN;*.LOG";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        try
                        {
                            MissionPlanner.Log.MatLab.ProcessLog(logfile);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Error converting file " + ex.ToString(), Strings.ERROR);
                        }
                    }
                }
            }
        }
    }
}