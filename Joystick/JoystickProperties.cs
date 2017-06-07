using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Joystick
{
    public static class JoystickProperties
    {
        public enum joystickaxis
        {
            None,
            Pass,
            ARx,
            ARy,
            ARz,
            AX,
            AY,
            AZ,
            FRx,
            FRy,
            FRz,
            FX,
            FY,
            FZ,
            Rx,
            Ry,
            Rz,
            VRx,
            VRy,
            VRz,
            VX,
            VY,
            VZ,
            X,
            Y,
            Z,
            Slider1,
            Slider2,
            Hatud1,
            Hatlr2,
            Custom1,
            Custom2,
        }
        public struct JoyChannel
        {
            public int channel;
            public joystickaxis axis;
            public bool reverse;
            public int expo;

            // for camera:
            public CameraJoystick.CameraAxis camaxis;
            public int overridecenter;
            internal bool rateconv;
        }

        public struct JoyButton
        {
            /// <summary>
            /// System button number
            /// </summary>
            public int buttonno;

            /// <summary>
            /// Fucntion we are doing for this button press
            /// </summary>
            public buttonfunction function;

            /// <summary>
            /// Mode we are changing to on button press
            /// </summary>
            public string mode;

            /// <summary>
            /// param 1
            /// </summary>
            public float p1;

            /// <summary>
            /// param 2
            /// </summary>
            public float p2;

            /// <summary>
            /// param 3
            /// </summary>
            public float p3;

            /// <summary>
            /// param 4
            /// </summary>
            public float p4;

            /// <summary>
            /// Relay state
            /// </summary>
            public bool state;
        }

        public enum buttonfunction
        {
            ChangeMode,
            Do_Set_Relay,
            Do_Repeat_Relay,
            Do_Set_Servo,
            Do_Repeat_Servo,
            Arm,
            Disarm,
            Digicam_Control,
            TakeOff,
            Mount_Mode,
            Toggle_Pan_Stab,
            Gimbal_pnt_track,
            Mount_Control_0,
            Button_axis0,
            Button_axis1,
            Toggle_CameraJoystick,
            Switch_CameraJoystick,
        }
    }
}
