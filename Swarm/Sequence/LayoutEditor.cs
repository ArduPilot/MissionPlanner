using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.Swarm.Sequence
{
    public partial class LayoutEditor: Form
    {
        private IContainer components;
        private ComboBox comboBox1;
        private Button BUT_new;
        private Button BUT_load;
        private BindingSource bindingSource1;
        private BindingSource layoutsBindingSource;
        private Button BUT_newdrone;
        private Button BUT_save;
        private ListBox listBox1;
        private Button BUT_addstep;
        private BindingSource stepsBindingSource;
        private Button BUT_runstep;
        private Label label1;
        private Button BUT_resetstep;
        private Grid grid;

        public LayoutEditor()
        {
            InitializeComponent();

            this.MouseWheel += (sender, e) => {
                if (e.Delta < 0)
                {
                    grid.setScale(grid.getScale() + 4);
                }
                else
                {
                    grid.setScale(grid.getScale() - 4);
                }
            };
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grid = new MissionPlanner.Swarm.Grid();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.layoutsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.BUT_new = new System.Windows.Forms.Button();
            this.BUT_load = new System.Windows.Forms.Button();
            this.BUT_newdrone = new System.Windows.Forms.Button();
            this.BUT_save = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.stepsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BUT_addstep = new System.Windows.Forms.Button();
            this.BUT_runstep = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BUT_resetstep = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.layoutsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.Location = new System.Drawing.Point(12, 52);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(725, 430);
            this.grid.TabIndex = 0;
            this.grid.Vertical = false;
            this.grid.UpdateOffsets += new MissionPlanner.Swarm.Grid.UpdateOffsetsEvent(this.grid_UpdateOffsets);
            // 
            // comboBox1
            // 
            this.comboBox1.DataSource = this.layoutsBindingSource;
            this.comboBox1.DisplayMember = "Id";
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(93, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // layoutsBindingSource
            // 
            this.layoutsBindingSource.DataMember = "Layouts";
            this.layoutsBindingSource.DataSource = this.bindingSource1;
            // 
            // bindingSource1
            // 
            this.bindingSource1.AllowNew = false;
            this.bindingSource1.DataSource = typeof(MissionPlanner.Swarm.Sequence.Sequence);
            // 
            // BUT_new
            // 
            this.BUT_new.Location = new System.Drawing.Point(220, 12);
            this.BUT_new.Name = "BUT_new";
            this.BUT_new.Size = new System.Drawing.Size(75, 23);
            this.BUT_new.TabIndex = 2;
            this.BUT_new.Text = "New Layout";
            this.BUT_new.UseVisualStyleBackColor = true;
            this.BUT_new.Click += new System.EventHandler(this.BUT_new_Click);
            // 
            // BUT_load
            // 
            this.BUT_load.Location = new System.Drawing.Point(12, 12);
            this.BUT_load.Name = "BUT_load";
            this.BUT_load.Size = new System.Drawing.Size(75, 23);
            this.BUT_load.TabIndex = 3;
            this.BUT_load.Text = "Load";
            this.BUT_load.UseVisualStyleBackColor = true;
            this.BUT_load.Click += new System.EventHandler(this.BUT_load_Click);
            // 
            // BUT_newdrone
            // 
            this.BUT_newdrone.Location = new System.Drawing.Point(301, 12);
            this.BUT_newdrone.Name = "BUT_newdrone";
            this.BUT_newdrone.Size = new System.Drawing.Size(75, 23);
            this.BUT_newdrone.TabIndex = 4;
            this.BUT_newdrone.Text = "New Drone";
            this.BUT_newdrone.UseVisualStyleBackColor = true;
            this.BUT_newdrone.Click += new System.EventHandler(this.BUT_newdrone_Click);
            // 
            // BUT_save
            // 
            this.BUT_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BUT_save.Location = new System.Drawing.Point(812, 12);
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.Size = new System.Drawing.Size(75, 23);
            this.BUT_save.TabIndex = 5;
            this.BUT_save.Text = "Save";
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.DataSource = this.stepsBindingSource;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(744, 52);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(143, 433);
            this.listBox1.TabIndex = 6;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragOver += new System.Windows.Forms.DragEventHandler(this.listBox1_DragOver);
            this.listBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyUp);
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            // 
            // stepsBindingSource
            // 
            this.stepsBindingSource.AllowNew = true;
            this.stepsBindingSource.DataMember = "Steps";
            this.stepsBindingSource.DataSource = this.bindingSource1;
            // 
            // BUT_addstep
            // 
            this.BUT_addstep.Location = new System.Drawing.Point(382, 12);
            this.BUT_addstep.Name = "BUT_addstep";
            this.BUT_addstep.Size = new System.Drawing.Size(75, 23);
            this.BUT_addstep.TabIndex = 7;
            this.BUT_addstep.Text = "Add Step";
            this.BUT_addstep.UseVisualStyleBackColor = true;
            this.BUT_addstep.Click += new System.EventHandler(this.BUT_addstep_Click);
            // 
            // BUT_runstep
            // 
            this.BUT_runstep.Location = new System.Drawing.Point(609, 12);
            this.BUT_runstep.Name = "BUT_runstep";
            this.BUT_runstep.Size = new System.Drawing.Size(75, 23);
            this.BUT_runstep.TabIndex = 8;
            this.BUT_runstep.Text = "Run Step";
            this.BUT_runstep.UseVisualStyleBackColor = true;
            this.BUT_runstep.Click += new System.EventHandler(this.BUT_runstep_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(691, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "label1";
            // 
            // BUT_resetstep
            // 
            this.BUT_resetstep.Location = new System.Drawing.Point(528, 12);
            this.BUT_resetstep.Name = "BUT_resetstep";
            this.BUT_resetstep.Size = new System.Drawing.Size(75, 23);
            this.BUT_resetstep.TabIndex = 10;
            this.BUT_resetstep.Text = "Reset";
            this.BUT_resetstep.UseVisualStyleBackColor = true;
            this.BUT_resetstep.Click += new System.EventHandler(this.BUT_resetstep_Click);
            // 
            // LayoutEditor
            // 
            this.ClientSize = new System.Drawing.Size(899, 494);
            this.Controls.Add(this.BUT_resetstep);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BUT_runstep);
            this.Controls.Add(this.BUT_addstep);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.BUT_newdrone);
            this.Controls.Add(this.BUT_load);
            this.Controls.Add(this.BUT_new);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.grid);
            this.Name = "LayoutEditor";
            this.Text = "Layout Editor";
            ((System.ComponentModel.ISupportInitialize)(this.layoutsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stepsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void grid_UpdateOffsets(MAVState mav, float x, float y, float z, Grid.icon ico)
        {
            workingLayout.AddOffset(mav.sysid, new Vector3(x, y, z));
        }

        private void BUT_new_Click(object sender, EventArgs e)
        {
            string name = "Layout X";
            InputBox.Show("", "Layout Name", ref name);

            var newworkingLayout = new Layout() {Id = name};

            workingSequence.Layouts.Add(newworkingLayout);

            foreach (var item in workingLayout.Offset)
            {
                newworkingLayout.AddOffset(item.Key, item.Value);
            }

            workingLayout = newworkingLayout;

            bindingSource1.ResetBindings(false);

            layoutsBindingSource.ResetBindings(false);

            UpdateDisplay();
        }

        private Layout workingLayout;
        private Sequence workingSequence = new Sequence();

        MAVLinkInterface mavint = new MAVLinkInterface();

        private void BUT_load_Click(object sender, EventArgs e)
        {
            var load = Sequence.Load("test.txt");

            if (load != null)
                workingSequence = load;

            bindingSource1.DataSource = workingSequence;

            if (load.Layouts.Count ==0)
                return;

            foreach (var sysid in load.Layouts.First().Offset.Keys)
            {
                mavs[sysid] = new MAVState(mavint, (byte) sysid, 0);
            }

            comboBox1_SelectedIndexChanged(null, null);

            UpdateDisplay();
        }

        private void BUT_newdrone_Click(object sender, EventArgs e)
        {
            int sysid = 1;
            try
            {
                sysid = workingSequence.Layouts.First().Offset.Keys.Max() + 1;
            } catch { }

            // add the drone to all layouts
            foreach (var workingSequenceLayout in workingSequence.Layouts)
            {
                workingSequenceLayout.AddOffset(sysid, new Vector3(sysid, 0, 0));
            }

            mavs[sysid] = new MAVState(mavint, (byte)sysid, 0);

            bindingSource1.DataSource = workingSequence;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (workingLayout == null)
                return;
            foreach (var vector3 in workingLayout.Offset)
            {
                grid.UpdateIcon(mavs[vector3.Key], (float)vector3.Value.x, (float)vector3.Value.y,
                    (float)vector3.Value.z, true);
            }
        }

        private Dictionary<int, MAVState> mavs = new Dictionary<int, MAVState>();

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workingLayout = (Layout)comboBox1.SelectedItem;
            bindingSource1.DataSource = workingSequence;
            grid.Clear();
            grid.Invalidate();
            UpdateDisplay();
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            workingSequence.Save("test.txt");
        }

        private void BUT_addstep_Click(object sender, EventArgs e)
        {
            workingSequence.Steps.Add(comboBox1.Text);

            stepsBindingSource.ResetBindings(false);
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                workingSequence.Steps.RemoveAt(listBox1.SelectedIndex);

                bindingSource1.ResetCurrentItem();
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            this.listBox1.DoDragDrop(this.listBox1.SelectedItem, DragDropEffects.Move);

            comboBox1.SelectedItem = workingSequence.Layouts.Find(a => a.Id == listBox1.SelectedItem.ToString());
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Point point = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = this.listBox1.IndexFromPoint(point);
            if (index < 0) index = this.listBox1.Items.Count - 1;
            var data = listBox1.SelectedItem.ToString();
            workingSequence.Steps.RemoveAt(listBox1.SelectedIndex);
            workingSequence.Steps.Insert(index, data);

            bindingSource1.ResetCurrentItem();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private int step = 0;
        private Controller controller = new Controller();
        private PointLatLngAlt startpos = PointLatLngAlt.Zero;

        private void BUT_runstep_Click(object sender, EventArgs e)
        {
            // get step layout name
            var layoutname = workingSequence.Steps[step];

            controller.Start();

            // get first homepos
            if (startpos == PointLatLngAlt.Zero)
            {
                startpos = controller.DG.Drones[0].MavState.cs.Location;
            }

            if (step == 0)
            {
                controller.DG.Drones.All(a =>
                {
                    a.MavState.parent.setMode(a.MavState.sysid, a.MavState.compid, "GUIDED");
                    a.MavState.parent.doARM(a.MavState.sysid, a.MavState.compid, true);
                    a.MavState.parent.doCommand(a.MavState.sysid, a.MavState.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                    return true;
                });
            }

            label1.Text = String.Format("{1} : {0}", step, layoutname);

            // get the layout
            var layout = workingSequence.Layouts.Find(a => a.Id == layoutname);

            foreach (var vector3 in layout.Offset)
            {
                var drone = controller.DG.Drones.Find(a => a.MavState.sysid == vector3.Key);
                var newpos = startpos.gps_offset(vector3.Value.x, vector3.Value.y);
                newpos.Alt = vector3.Value.z;
                drone.SendPositionVelocity(newpos, Vector3.Zero);
            }

            step++;
        }

        private void BUT_resetstep_Click(object sender, EventArgs e)
        {
            step = 0;
        }
    }
}
