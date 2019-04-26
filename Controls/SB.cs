using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class SB
    {
        public static void Show()
        {
            var l1 = "Your board has a Critical service bulletin";
            var l2 = "To send us a report about your board, and to find out about this safety issue. Click bellow";

            Form frm = new Form() {Width = 250, Height = 250, AutoSize = true};
            FlowLayoutPanel flp = new FlowLayoutPanel() {Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown};
            Label lb1 = new Label() {Text = l1, AutoSize = true};
            Label link = new Label() {Text = l2, AutoSize = true};


            Label nameLabel = new Label() {Text = "Enter your name (optional): ", AutoSize = true};
            TextBox nameTextBox = new TextBox()
                {Width = TextRenderer.MeasureText("thisismyname andmysurname", frm.Font).Width};
            Label emailLabel = new Label() {Text = "Enter your email (optional): ", AutoSize = true};
            TextBox emailTextBox = new TextBox()
                {Width = TextRenderer.MeasureText("thisisatest@thisdomain.com.fre.do", frm.Font).Width};
            MyButton submitButton = new MyButton() {Text = "Service Bulletin"};
            frm.Controls.Add(flp);

            flp.Controls.Add(lb1);

            flp.Controls.Add(nameLabel);
            flp.Controls.Add(nameTextBox);
            flp.Controls.Add(emailLabel);
            flp.Controls.Add(emailTextBox);

            flp.Controls.Add(link);

            flp.Controls.Add(submitButton);

            submitButton.Click += (sender, args) =>
            {
                var url = String.Format(
                    "https://discuss.cubepilot.org:444/CubeSB?BRD_TYPE={0}&SerialNo={1}&INS_ACC_ID={2}&INS_ACC2_ID={3}&INS_ACC3_ID={4}&INS_GYR_ID={5}&INS_GYR2_ID={6}&INS_GYR3_ID={7}&Baro1={8}&Baro2={9}&Name={10}&Email={11}",
                    MainV2.comPort.MAV.param["BRD_TYPE"], MainV2.comPort.MAV.SerialString,
                    MainV2.comPort.MAV.param["INS_ACC_ID"], MainV2.comPort.MAV.param["INS_ACC2_ID"],
                    MainV2.comPort.MAV.param["INS_ACC3_ID"],
                    MainV2.comPort.MAV.param["INS_GYR_ID"], MainV2.comPort.MAV.param["INS_GYR2_ID"],
                    MainV2.comPort.MAV.param["INS_GYR3_ID"],
                    MainV2.comPort.MAV.cs.press_abs, MainV2.comPort.MAV.cs.press_abs2, nameTextBox.Text,
                    emailTextBox.Text);

                System.Diagnostics.Process.Start(url);
            };

            ThemeManager.ApplyThemeTo(frm);

            frm.Show();

        }
    }
}
