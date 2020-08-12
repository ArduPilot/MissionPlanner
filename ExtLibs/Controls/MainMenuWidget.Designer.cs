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
            this.MainButton.Location = new System.Drawing.Point(0, 0);
            this.MainButton.Name = "MainButton";
            this.MainButton.Size = new System.Drawing.Size(75, 69);
            this.MainButton.TabIndex = 0;
            this.MainButton.Text = "Main Button";
            this.MainButton.UseVisualStyleBackColor = true;
            this.MainButton.Click += new System.EventHandler(this.MainButton_Click);
            // 
            // MapChoiseButton
            // 
            this.MapChoiseButton.Location = new System.Drawing.Point(94, 0);
            this.MapChoiseButton.Name = "MapChoiseButton";
            this.MapChoiseButton.Size = new System.Drawing.Size(75, 69);
            this.MapChoiseButton.TabIndex = 1;
            this.MapChoiseButton.Text = "Map Choise";
            this.MapChoiseButton.UseVisualStyleBackColor = true;
            // 
            // EKFButton
            // 
            this.EKFButton.Location = new System.Drawing.Point(186, 0);
            this.EKFButton.Name = "EKFButton";
            this.EKFButton.Size = new System.Drawing.Size(75, 69);
            this.EKFButton.TabIndex = 2;
            this.EKFButton.Text = "EKF";
            this.EKFButton.UseVisualStyleBackColor = true;
            // 
            // ParamsButton
            // 
            this.ParamsButton.Location = new System.Drawing.Point(277, 0);
            this.ParamsButton.Name = "ParamsButton";
            this.ParamsButton.Size = new System.Drawing.Size(75, 69);
            this.ParamsButton.TabIndex = 3;
            this.ParamsButton.Text = "Params";
            this.ParamsButton.UseVisualStyleBackColor = true;
            // 
            // RulerButton
            // 
            this.RulerButton.Location = new System.Drawing.Point(369, 0);
            this.RulerButton.Name = "RulerButton";
            this.RulerButton.Size = new System.Drawing.Size(75, 69);
            this.RulerButton.TabIndex = 4;
            this.RulerButton.Text = "RulerButton";
            this.RulerButton.UseVisualStyleBackColor = true;
            // 
            // centeringButton
            // 
            this.centeringButton.Location = new System.Drawing.Point(460, 0);
            this.centeringButton.Name = "centeringButton";
            this.centeringButton.Size = new System.Drawing.Size(75, 69);
            this.centeringButton.TabIndex = 5;
            this.centeringButton.Text = "Centering";
            this.centeringButton.UseVisualStyleBackColor = true;
            // 
            // myButton2
            // 
            this.myButton2.Enabled = false;
            this.myButton2.Location = new System.Drawing.Point(550, 0);
            this.myButton2.Name = "myButton2";
            this.myButton2.Size = new System.Drawing.Size(75, 69);
            this.myButton2.TabIndex = 6;
            this.myButton2.Text = "useless";
            this.myButton2.UseVisualStyleBackColor = true;
            // 
            // homeButton
            // 
            this.homeButton.Location = new System.Drawing.Point(641, 0);
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(75, 69);
            this.homeButton.TabIndex = 7;
            this.homeButton.Text = "Home";
            this.homeButton.UseVisualStyleBackColor = true;
            // 
            // MainMenuWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.homeButton);
            this.Controls.Add(this.myButton2);
            this.Controls.Add(this.centeringButton);
            this.Controls.Add(this.RulerButton);
            this.Controls.Add(this.ParamsButton);
            this.Controls.Add(this.EKFButton);
            this.Controls.Add(this.MapChoiseButton);
            this.Controls.Add(this.MainButton);
            this.Name = "MainMenuWidget";
            this.Size = new System.Drawing.Size(717, 71);
            this.ResumeLayout(false);

        }

        #endregion

        private MyButton MainButton;
        private MyButton MapChoiseButton;
        private MyButton EKFButton;
        private MyButton ParamsButton;
        private MyButton RulerButton;
        private MyButton centeringButton;
        private MyButton myButton2;
        private MyButton homeButton;
    }
}
