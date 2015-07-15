﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Comms;

namespace MissionPlanner.Antenna
{
    interface ITrackerOutput
    {
        SerialPort ComPort { get; set; }

        double TrimPan { get; set; }
        double TrimTilt { get; set; }

        int PanStartRange { get; set; }
        int TiltStartRange { get; set; }
        int PanEndRange { get; set; }
        int TiltEndRange { get; set; }
        int PanPWMRange { get; set; }
        int TiltPWMRange { get; set; }
        int PanPWMCenter { get; set; }
        int TiltPWMCenter { get; set; }
        int PanSpeed { get; set; }
        int TiltSpeed { get; set; }
        int PanAccel { get; set; }
        int TiltAccel { get; set; }

        bool PanReverse { get; set; }
        bool TiltReverse { get; set; }

        bool Init();
        bool Setup();
        bool Pan(double Angle);
        bool Tilt(double Angle);
        bool PanAndTilt(double Pan, double Tilt);
        bool Close();
    }
}
