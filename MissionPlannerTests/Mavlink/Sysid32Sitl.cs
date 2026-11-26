// GUI integration test for an isolated Rover SITL on TCP port 6060. See README.md.
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
class Sysid32Sitl
{
 [DllImport("libc", EntryPoint="_exit")] static extern void Exit(int status);
 static System.Windows.Forms.Timer timer;
 static void Check(bool value, string name) { if (!value) throw new Exception(name); Console.WriteLine("SITL_PASS " + name); }
 [STAThread] static void Main()
 {
  Settings.CustomUserDataDirectory = Path.Combine(Path.GetTempPath(), "mp-sysid-gui-" + Guid.NewGuid());
  Directory.CreateDirectory(Settings.GetUserDataDirectory());
  Settings.Instance["update_check"] = DateTime.Now.ToShortDateString();
  Settings.Instance["MainWidth"]="1300"; Settings.Instance["MainHeight"]="900";
  Settings.Instance["gcsid"]="2147483649"; Settings.Instance["AutoConnect"]="[]"; Settings.Instance.Save();
  timer=new System.Windows.Forms.Timer { Interval=10000 };
  timer.Tick += async (sender,args) => {
   timer.Stop();
   try {
    var port=MainV2.comPort;
    port.BaseStream = new TcpSerial { client = new TcpClient("127.0.0.1",6060) };
    MainV2.instance.doConnect(port,"preset","6060",false,false);
    uint id=uint.Parse(Environment.GetEnvironmentVariable("SYSID32_SITL_ID") ?? uint.MaxValue.ToString());
    Check(MAVLinkInterface.gcssysid==0x80000001,"wide GCS system ID");
    Check(port.BaseStream.IsOpen && port.sysidcurrent==id,"GUI connected to " + id);
    int targeted=0, ftp=0, paramLists=0;
    port.OnPacketSent += (s,m) => { if(m.target_system==id) targeted++; if(m.msgid==110 && m.target_system==id) ftp++; if(m.msgid==21) paramLists++; };
    await Task.Run(async () => {
     Check(port.getVersion(id,1),"version response matched full ID");
     float speed=port.GetParam(id,1,"CRUISE_SPEED");
     Check(speed>0,"PARAM_REQUEST_READ");
     try { Check(port.setParam(id,1,"CRUISE_SPEED",speed+0.25),"PARAM_SET"); }
     finally { port.setParam(id,1,"CRUISE_SPEED",speed); }
     var mavftp=new MAVFtp(port,id,1);
     using(var cancel=new CancellationTokenSource(TimeSpan.FromSeconds(60)))
     using(var file=mavftp.GetFile("@PARAM/param.pck",cancel))
       Check(file!=null && file.Length>1000,"MAVFTP parameter download");
     int before=paramLists;
     var list=port.getParamListMavftp(id,1);
     Check(paramLists==before,"MAVFTP stays on fast path for wide IDs");
     Check(list!=null && list.ContainsKey("CRUISE_SPEED"),"MAVFTP parameter decode");
     var points=new List<Locationwp> {
      new Locationwp { id=16, lat=-35.362938, lng=149.165085, alt=584, frame=0 },
      new Locationwp { id=16, lat=-35.362838, lng=149.165185, alt=10, frame=3 }
     };
     await mav_mission.upload(port,id,1,MAVLink.MAV_MISSION_TYPE.MISSION,points);
     Check(port.getWPCount(id,1)==2,"mission upload/count");
     var point=port.getWP(id,1,1);
     Check(Math.Abs(point.lat-points[1].lat)<0.000001,"mission download");
     Check(port.doCommand(id,1,MAVLink.MAV_CMD.DO_SET_MODE,1,4,0,0,0,0,0),"COMMAND_LONG acknowledgement");
     Check(targeted>5 && ftp>1,"outgoing header targets");
    });
    Check(MainV2.instance.Visible && MainV2.instance.FlightData.Visible,"FlightData visible after traffic");
    var selector = (ComboBox)MainV2.instance.Controls.Find("cmb_sysid", true).Single();
    Check(selector.Text.Contains(id.ToString()), "vehicle selector displays unsigned ID");
    using (var deviceOps = new MissionPlanner.Controls.DevopsUI()) {
     var sysidControl = (NumericUpDown)deviceOps.Controls.Find("num_sysid", true).Single();
     sysidControl.Value=uint.MaxValue;
     Check(sysidControl.Value==uint.MaxValue,"Device Op accepts full unsigned ID");
    }
    var flightData=MainV2.instance.FlightData;
    var action=(ComboBox)flightData.Controls.Find("CMB_action",true).Single();
    action.SelectedItem="Toggle_Safety_Switch";
    int safetyPackets=0;
    port.OnPacketSent += (s,m) => {
     if(m.msgid==(uint)MAVLink.MAVLINK_MSG_ID.SET_MODE && m.GetTargetSystem()==id) safetyPackets++;
    };
    using(var confirm=new System.Windows.Forms.Timer { Interval=100 }) {
     confirm.Tick += (s,e) => {
      foreach(Form form in Application.OpenForms.Cast<Form>().ToArray())
       if(form.Text=="Action" && form.AcceptButton!=null) form.AcceptButton.PerformClick();
     };
     confirm.Start();
     flightData.GetType().GetMethod("BUTactiondo_Click",System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
      .Invoke(flightData,new object[] {new Button(),EventArgs.Empty});
     confirm.Stop();
    }
    Check(safetyPackets>0,"safety toggle keeps the full destination ID");
    Console.WriteLine("MISSIONPLANNER_SYSID32_SITL_PASS");
    await Task.Delay(20000);
    Console.Out.Flush(); Console.Error.Flush(); Exit(0);
   } catch(Exception e) { Console.Error.WriteLine("MISSIONPLANNER_SYSID32_SITL_FAIL "+e); Console.Out.Flush(); Console.Error.Flush(); Exit(1); }
  };
  timer.Start(); MissionPlanner.Program.Main(new string[0]);
 }
}
