namespace MissionPlanner.Controls
{
    partial class MainMenuWidget
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainButton = new MissionPlanner.Controls.MyButton();
            this.MapChoiseButton = new MissionPlanner.Controls.MyButton();
            this.EKFButton = new MissionPlanner.Controls.MyButton();
            this.ParamsButton = new MissionPlanner.Controls.MyButton();
            this.RulerButton = new MissionPlanner.Controls.MyButton();
            this.centeringButton = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.homeButton = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // MainButton
            // 
            this.MainButton.Location = new System.Drawing.Point(15, 13);
            this.MainButton.Name = "MainButton";
            this.MainButton.Size = new System.Drawing.Size(75, 69);
            this.MainButton.TabIndex = 0;
            this.MainButton.Text = "Main Button";
            this.MainButton.UseVisualStyleBackColor = true;
            this.MainButton.Click += new System.EventHandler(this.MainButton_Click);
            this.MainButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // MapChoiseButton
            // 
            this.MapChoiseButton.Location = new System.Drawing.Point(109, 13);
            this.MapChoiseButton.Name = "MapChoiseButton";
            this.MapChoiseButton.Size = new System.Drawing.Size(75, 69);
            this.MapChoiseButton.TabIndex = 1;
            this.MapChoiseButton.Text = "Map Choise";
            this.MapChoiseButton.UseVisualStyleBackColor = true;
            this.MapChoiseButton.Click += new System.EventHandler(this.MapChoiseButton_Click);
            this.MapChoiseButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // EKFButton
            // 
            this.EKFButton.Location = new System.Drawing.Point(201, 13);
            this.EKFButton.Name = "EKFButton";
            this.EKFButton.Size = new System.Drawing.Size(75, 69);
            this.EKFButton.TabIndex = 2;
            this.EKFButton.Text = "EKF";
            this.EKFButton.UseVisualStyleBackColor = true;
            this.EKFButton.Click += new System.EventHandler(this.EKFButton_Click);
            this.EKFButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // ParamsButton
            // 
            this.ParamsButton.Location = new System.Drawing.Point(292, 13);
            this.ParamsButton.Name = "ParamsButton";
            this.ParamsButton.Size = new System.Drawing.Size(75, 69);
            this.ParamsButton.TabIndex = 3;
            this.ParamsButton.Text = "Params";
            this.ParamsButton.UseVisualStyleBackColor = true;
            //this.ParamsButton.Click += new System.EventHandler(this.ParamsButton_Click);
            this.ParamsButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // RulerButton
            // 
            this.RulerButton.Location = new System.Drawing.Point(384, 13);
            this.RulerButton.Name = "RulerButton";
            this.RulerButton.Size = new System.Drawing.Size(75, 69);
            this.RulerButton.TabIndex = 4;
            this.RulerButton.Text = "RulerButton";
            this.RulerButton.UseVisualStyleBackColor = true;
            this.RulerButton.Click += new System.EventHandler(this.RulerButton_Click);
            this.RulerButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // centeringButton
            // 
            this.centeringButton.Location = new System.Drawing.Point(475, 13);
            this.centeringButton.Name = "centeringButton";
            this.centeringButton.Size = new System.Drawing.Size(75, 69);
            this.centeringButton.TabIndex = 5;
            this.centeringButton.Text = "Centering";
            this.centeringButton.UseVisualStyleBackColor = true;
            this.centeringButton.Click += new System.EventHandler(this.centeringButton_Click);
            this.centeringButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // myButton2
            // 
            this.myButton2.Enabled = false;
            this.myButton2.Location = new System.Drawing.Point(565, 13);
            this.myButton2.Name = "myButton2";
            this.myButton2.Size = new System.Drawing.Size(75, 69);
            this.myButton2.TabIndex = 6;
            this.myButton2.Text = "useless";
            this.myButton2.UseVisualStyleBackColor = true;
            this.myButton2.Click += new System.EventHandler(this.myButton2_Click);
            this.myButton2.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // homeButton
            // 
            this.homeButton.Location = new System.Drawing.Point(656, 13);
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(75, 69);
            this.homeButton.TabIndex = 7;
            this.homeButton.Text = "Home";
            this.homeButton.UseVisualStyleBackColor = true;
            this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
            this.homeButton.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            // 
            // MainMenuWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.homeButton);
            this.Controls.Add(this.MapChoiseButton);
            this.Controls.Add(this.myButton2);
            this.Controls.Add(this.centeringButton);
            this.Controls.Add(this.RulerButton);
            this.Controls.Add(this.ParamsButton);
            this.Controls.Add(this.EKFButton);
            this.Controls.Add(this.MainButton);
            this.Name = "MainMenuWidget";
            this.Size = new System.Drawing.Size(750, 100);
            this.MouseEnter += new System.EventHandler(this.MainMenuWidget_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.MainMenuWidget_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private MyButton MainButton;
        public MyButton MapChoiseButton;
        public MyButton EKFButton;
        public MyButton ParamsButton;
        public MyButton RulerButton;
        public MyButton centeringButton;
        public MyButton myButton2;
        public MyButton homeButton;
    }
}
