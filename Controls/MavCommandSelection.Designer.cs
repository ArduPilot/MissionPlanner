
namespace MissionPlanner.Controls
{
    partial class MavCommandSelection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MavCommandSelection));
            this.btn_AddLine = new MissionPlanner.Controls.MyButton();
            this.btn_Save = new MissionPlanner.Controls.MyButton();
            this.label1 = new System.Windows.Forms.Label();
            this.myDataGridView1 = new MissionPlanner.Controls.MyDataGridView();
            this.msgid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msgname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.param7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_AddLine
            // 
            this.btn_AddLine.Cursor = System.Windows.Forms.Cursors.Default;
            this.btn_AddLine.Location = new System.Drawing.Point(12, 438);
            this.btn_AddLine.Name = "btn_AddLine";
            this.btn_AddLine.Size = new System.Drawing.Size(75, 23);
            this.btn_AddLine.TabIndex = 1;
            this.btn_AddLine.Text = "ADD";
            this.btn_AddLine.UseVisualStyleBackColor = true;
            this.btn_AddLine.Click += new System.EventHandler(this.btn_AddLine_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(133, 438);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "Save && Exit";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(432, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(461, 65);
            this.label1.TabIndex = 3;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // myDataGridView1
            // 
            this.myDataGridView1.AllowUserToAddRows = false;
            this.myDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.myDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.msgid,
            this.msgname,
            this.param1,
            this.param2,
            this.param3,
            this.param4,
            this.param5,
            this.param6,
            this.param7});
            this.myDataGridView1.Location = new System.Drawing.Point(12, 12);
            this.myDataGridView1.Name = "myDataGridView1";
            this.myDataGridView1.Size = new System.Drawing.Size(925, 393);
            this.myDataGridView1.TabIndex = 0;
            // 
            // msgid
            // 
            this.msgid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.msgid.HeaderText = "MSG ID";
            this.msgid.Name = "msgid";
            this.msgid.ReadOnly = true;
            this.msgid.Width = 70;
            // 
            // msgname
            // 
            this.msgname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.msgname.HeaderText = "NAME";
            this.msgname.Name = "msgname";
            this.msgname.ReadOnly = true;
            this.msgname.Width = 63;
            // 
            // param1
            // 
            this.param1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param1.HeaderText = "Param1 Name";
            this.param1.Name = "param1";
            this.param1.Width = 99;
            // 
            // param2
            // 
            this.param2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param2.HeaderText = "Param2 Name";
            this.param2.Name = "param2";
            this.param2.Width = 99;
            // 
            // param3
            // 
            this.param3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param3.HeaderText = "Param3 Name";
            this.param3.Name = "param3";
            this.param3.Width = 99;
            // 
            // param4
            // 
            this.param4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param4.HeaderText = "Param4 Name";
            this.param4.Name = "param4";
            this.param4.Width = 99;
            // 
            // param5
            // 
            this.param5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param5.HeaderText = "Param5 Name";
            this.param5.Name = "param5";
            this.param5.Width = 99;
            // 
            // param6
            // 
            this.param6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param6.HeaderText = "Param6 Name";
            this.param6.Name = "param6";
            this.param6.Width = 99;
            // 
            // param7
            // 
            this.param7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.param7.HeaderText = "Param7 Name";
            this.param7.Name = "param7";
            this.param7.Width = 99;
            // 
            // MavCommandSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 493);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_AddLine);
            this.Controls.Add(this.myDataGridView1);
            this.Name = "MavCommandSelection";
            this.Text = "MavCommandSelection";
            ((System.ComponentModel.ISupportInitialize)(this.myDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyDataGridView myDataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn msgid;
        private System.Windows.Forms.DataGridViewTextBoxColumn msgname;
        private System.Windows.Forms.DataGridViewTextBoxColumn param1;
        private System.Windows.Forms.DataGridViewTextBoxColumn param2;
        private System.Windows.Forms.DataGridViewTextBoxColumn param3;
        private System.Windows.Forms.DataGridViewTextBoxColumn param4;
        private System.Windows.Forms.DataGridViewTextBoxColumn param5;
        private System.Windows.Forms.DataGridViewTextBoxColumn param6;
        private System.Windows.Forms.DataGridViewTextBoxColumn param7;
        private MyButton btn_AddLine;
        private MyButton btn_Save;
        private System.Windows.Forms.Label label1;
    }
}