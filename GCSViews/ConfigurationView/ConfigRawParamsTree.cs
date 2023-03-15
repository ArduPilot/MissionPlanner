using log4net;
using Microsoft.Scripting.Utils;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRawParamsTree : MyUserControl, IActivate, IDeactivate
    {
        public ConfigRawParamsTree()
        {
            InitializeComponent();
            configRawParams1.ParameterListChanged += ParameterListChanged;
        }

        public void Activate()
        {
            configRawParams1.Activate();
            BuildTree();
        }

        public void Deactivate()
        {
            configRawParams1.Deactivate();
        }

        private void BuildTree()
        {
            treeView1.Nodes.Clear();
            var currentNode = treeView1.Nodes.Add("All");
            string currentPrefix = "";
            DataGridViewRowCollection rows = configRawParams1.Params.Rows;
            for(int i = 0; i < rows.Count - 1; i++)
            {
                string param = rows[i].Cells[0].Value.ToString();
                string next_param = rows[i + 1].Cells[0].Value.ToString();

                // While param does not start with currentPrefix, step up a layer in the tree
                while (!param.StartsWith(currentPrefix))
                {
                    currentPrefix = currentPrefix.RemoveFromEnd(currentNode.Text.Split('_').Last() + "_");
                    currentNode = currentNode.Parent;
                }

                // While the next parameter has a common prefix with this, add branch nodes
                string nodeToAdd = param.Substring(currentPrefix.Length).Split('_')[0] + "_";
                while (nodeToAdd.Length > 1 // While the currentPrefix is smaller than param
                    && param.StartsWith(currentPrefix + nodeToAdd) // And while this parameter starts with currentPrefix+nodeToAdd (needed for edge case where next_param starts with the full name of this param; see Q_PLT_Y_RATE and Q_PLT_Y_RATE_TC)
                    && next_param.StartsWith(currentPrefix + nodeToAdd)) // And the next parameter also starts with currentPrefix
                {
                    currentPrefix += nodeToAdd;
                    currentNode = currentNode.Nodes.Add(currentPrefix.Substring(0, currentPrefix.Length - 1));
                    nodeToAdd = param.Substring(currentPrefix.Length).Split('_')[0] + "_";
                }
                currentNode.Nodes.Add(param);
            }
            treeView1.TopNode.Expand();
        }

        private void ParameterListChanged(object sender, EventArgs e)
        {
            BuildTree();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string txt = treeView1.SelectedNode.Text + "_";
            if (txt == "All_") txt = "";
            configRawParams1.filterPrefix = txt;
            configRawParams1.FilterTimerOnElapsed(null, null);
        }
    }
}
