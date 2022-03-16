using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Controls
{
    public partial class MavCommandSelection : Form
    {
        public MavCommandSelection()
        {
            InitializeComponent();
            loadFromSettings();
        }

        //All commands that added (not including ID)
        public Dictionary<string, string[]> commands = new Dictionary<string, string[]>();
        //ID of addedd commands that are not in MAV_CMD enum
        public Dictionary<string, ushort> extraCommands = new Dictionary<string, ushort>();


        private void loadFromSettings()
        {

            commands.Clear();
            extraCommands.Clear();

            try
            {
                commands = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(Settings.Instance["PlannerExtraCommand"]);
                extraCommands = JsonConvert.DeserializeObject<Dictionary<string, ushort>>(Settings.Instance["PlannerExtraCommandIDs"]);
            }
            catch { }

            foreach (var cmd in commands)
            {
                //Is it a cmd already in the Mavlink Library ?
                if (Enum.IsDefined(typeof(MAVLink.MAV_CMD), cmd.Key))
                {
                    //Yes no need to look up extraCommands
                    var selectedrow = myDataGridView1.Rows.Add();
                    myDataGridView1.Rows[selectedrow].Cells[msgid.Index].Value = ((ushort)Enum.Parse(typeof(MAVLink.MAV_CMD),cmd.Key)).ToString();
                    myDataGridView1.Rows[selectedrow].Cells[msgname.Index].Value = cmd.Key;
                    myDataGridView1.Rows[selectedrow].Cells[param1.Index].Value = cmd.Value[0];
                    myDataGridView1.Rows[selectedrow].Cells[param2.Index].Value = cmd.Value[1];
                    myDataGridView1.Rows[selectedrow].Cells[param3.Index].Value = cmd.Value[2];
                    myDataGridView1.Rows[selectedrow].Cells[param4.Index].Value = cmd.Value[3];
                    myDataGridView1.Rows[selectedrow].Cells[param5.Index].Value = cmd.Value[4];
                    myDataGridView1.Rows[selectedrow].Cells[param6.Index].Value = cmd.Value[5];
                    myDataGridView1.Rows[selectedrow].Cells[param7.Index].Value = cmd.Value[6];
                }
                else
                {
                    var selectedrow = myDataGridView1.Rows.Add();
                    myDataGridView1.Rows[selectedrow].Cells[msgid.Index].Value = extraCommands[cmd.Key];
                    myDataGridView1.Rows[selectedrow].Cells[msgname.Index].Value = cmd.Key;
                    myDataGridView1.Rows[selectedrow].Cells[param1.Index].Value = cmd.Value[0];
                    myDataGridView1.Rows[selectedrow].Cells[param2.Index].Value = cmd.Value[1];
                    myDataGridView1.Rows[selectedrow].Cells[param3.Index].Value = cmd.Value[2];
                    myDataGridView1.Rows[selectedrow].Cells[param4.Index].Value = cmd.Value[3];
                    myDataGridView1.Rows[selectedrow].Cells[param5.Index].Value = cmd.Value[4];
                    myDataGridView1.Rows[selectedrow].Cells[param6.Index].Value = cmd.Value[5];
                    myDataGridView1.Rows[selectedrow].Cells[param7.Index].Value = cmd.Value[6];
                }
            }


        }


        //Check if the given id or name is already in the table
        private bool checkIDandName(ushort cmdId, string cmdName)
        {

            bool retval = false;

            for (int i = 0; i < myDataGridView1.RowCount; i++)
            {
                string cmdNameRow = myDataGridView1.Rows[i].Cells[msgname.Index].Value.ToString();
                if (cmdNameRow == cmdName)
                {
                    retval = true;
                    break;
                }
                ushort cmdIdRow = ushort.Parse(myDataGridView1.Rows[i].Cells[msgid.Index].Value.ToString());
                if (cmdIdRow == cmdId)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }

        private void btn_AddLine_Click(object sender, EventArgs e)
        {
            var cmdid = "-1";
            InputBox.Show("Enter command ID","ID number of the command", ref cmdid);

            try
            {

                if (cmdid != "-1")
                {
                    ushort i = ushort.Parse(cmdid);
                    string a = ((MAVLink.MAV_CMD)i).ToString();

                    if (checkIDandName(i, a))
                    {
                        CustomMessageBox.Show("Name or ID exists");
                        return;
                    }

                    if (a == i.ToString())
                    {
                        string cmdName = "NEW_COMMAND";
                        InputBox.Show("What is the name", "Name", ref cmdName);
                        if (cmdName.Length > 0)
                        {
                            if (checkIDandName(i, cmdName))
                            {
                                MessageBox.Show("Name or ID exists");
                                return;
                            }
                            var selectedrow = myDataGridView1.Rows.Add();
                            myDataGridView1.Rows[selectedrow].Cells[msgid.Index].Value = i.ToString();
                            myDataGridView1.Rows[selectedrow].Cells[msgname.Index].Value = cmdName;
                            myDataGridView1.Rows[selectedrow].Cells[param5.Index].Value = "Lat";
                            myDataGridView1.Rows[selectedrow].Cells[param6.Index].Value = "Long";
                            myDataGridView1.Rows[selectedrow].Cells[param7.Index].Value = "Alt";
                        }
                    }
                    else
                    {
                        var selectedrow = myDataGridView1.Rows.Add();
                        myDataGridView1.Rows[selectedrow].Cells[msgid.Index].Value = i.ToString();
                        myDataGridView1.Rows[selectedrow].Cells[msgname.Index].Value = a;
                        myDataGridView1.Rows[selectedrow].Cells[param5.Index].Value = "Lat";
                        myDataGridView1.Rows[selectedrow].Cells[param6.Index].Value = "Long";
                        myDataGridView1.Rows[selectedrow].Cells[param7.Index].Value = "Alt";

                    }

                }
            }
            catch { }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Go through the rows and fill up data table
            commands.Clear();
            extraCommands.Clear();
            for (int i = 0; i < myDataGridView1.RowCount; i++)
            {
                try
                {

                    string cmdName = myDataGridView1.Rows[i].Cells[msgname.Index].Value.ToString();
                    ushort cmdId = ushort.Parse(myDataGridView1.Rows[i].Cells[msgid.Index].Value.ToString());

                    string[] p = new string[7];

                    p[0] = myDataGridView1.Rows[i].Cells[param1.Index].Value?.ToString();
                    p[1] = myDataGridView1.Rows[i].Cells[param2.Index].Value?.ToString();
                    p[2] = myDataGridView1.Rows[i].Cells[param3.Index].Value?.ToString();
                    p[3] = myDataGridView1.Rows[i].Cells[param4.Index].Value?.ToString();
                    p[4] = myDataGridView1.Rows[i].Cells[param5.Index].Value?.ToString();
                    p[5] = myDataGridView1.Rows[i].Cells[param6.Index].Value?.ToString();
                    p[6] = myDataGridView1.Rows[i].Cells[param7.Index].Value?.ToString();


                    commands.Add(cmdName, p);
                    if (!Enum.IsDefined(typeof(MAVLink.MAV_CMD), cmdId))
                    {
                        extraCommands.Add(cmdName, cmdId);
                    }
                }
                catch { }
            }

            Settings.Instance["PlannerExtraCommand"] = JsonConvert.SerializeObject(commands);
            Settings.Instance["PlannerExtraCommandIDs"] =  JsonConvert.SerializeObject(extraCommands);

            //This needed in case the user already have a mission in the planner tab with added items.
            MainV2.instance.FlightPlanner.updateCMDParams();

            this.Close();

        }

    }
}
