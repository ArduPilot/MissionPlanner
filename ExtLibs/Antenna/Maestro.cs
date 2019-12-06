using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Comms;

namespace MissionPlanner.Antenna
{
    public class Maestro : ITrackerOutput
    {
        public SerialPort ComPort { get; set; }

        /// <summary>
        ///  0-360
        /// </summary>
        public double TrimPan { get; set; }

        /// <summary>
        /// -90 - 90
        /// </summary>
        public double TrimTilt { get; set; }

        public int PanStartRange { get; set; }
        public int TiltStartRange { get; set; }
        public int PanEndRange { get; set; }
        public int TiltEndRange { get; set; }
        public int PanPWMRange { get; set; }
        public int TiltPWMRange { get; set; }
        public int PanPWMCenter { get; set; }
        public int TiltPWMCenter { get; set; }
        public int PanSpeed { get; set; }
        public int TiltSpeed { get; set; }
        public int PanAccel { get; set; }
        public int TiltAccel { get; set; }

        public bool PanReverse
        {
            get { return _panreverse == -1; }
            set { _panreverse = value == true ? -1 : 1; }
        }

        public bool TiltReverse
        {
            get { return _tiltreverse == -1; }
            set { _tiltreverse = value == true ? -1 : 1; }
        }

        int _panreverse = 1;
        int _tiltreverse = 1;

        byte PanAddress = 0;
        byte TiltAddress = 1;

        private const byte SetTarget = 0x84;
        private const byte SetSpeed = 0x87;
        private const byte SetAccel = 0x89;
        private const byte GetPos = 0x90;
        private const byte GetState = 0x93;
        private const byte GetErrors = 0xA1;
        private const byte GoHome = 0xA2;

        public bool Init()
        {
            if ((PanStartRange - PanEndRange) == 0)
            {
                CustomMessageBox.Show(Strings.InvalidPanRange, Strings.ERROR);
                return false;
            }

            if ((TiltStartRange - TiltEndRange) == 0)
            {
                CustomMessageBox.Show(Strings.InvalidTiltRange, Strings.ERROR);
                return false;
            }

            try
            {
                ComPort.Open();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorConnecting + ex.Message, Strings.ERROR);
                return false;
            }

            return true;
        }

        public bool Setup()
        {
            // speed
            SendCompactMaestroCommand(SetSpeed, 0, PanAddress, PanSpeed);
            SendCompactMaestroCommand(SetSpeed, 0, TiltAddress, TiltSpeed);

            // accel
            SendCompactMaestroCommand(SetAccel, 0, PanAddress, PanAccel);
            SendCompactMaestroCommand(SetAccel, 0, TiltAddress, TiltAccel);

            //getCenterPWs();

            return true;
        }

        void getCenterPWs()
        {
            byte[] buffer;
            // set all to home (center)
            SendCompactMaestroCommand(GoHome);

            while (SendCompactMaestroCommand(GetState, 1)[0] == 0x01)
            {
            }

            // get center position -- pan
            buffer = SendCompactMaestroCommand(GetPos, 2, PanAddress);
            this.PanPWMCenter = (int) ((buffer[1] << 8) | buffer[0]);

            // get center position -- tilt
            buffer = SendCompactMaestroCommand(GetPos, 2, TiltAddress);
            this.TiltPWMCenter = (int) ((buffer[1] << 8) | buffer[0]);
        }

        double wrap_180(double input)
        {
            if (input > 180)
                return input - 360;
            if (input < -180)
                return input + 360;
            return input;
        }

        public bool Pan(double Angle)
        {
            double angleRange = Math.Abs(this.PanStartRange - this.PanEndRange);

            double pulseWidth = ((((double) this.PanPWMRange)/angleRange)*wrap_180(Angle - TrimPan)*_panreverse) +
                                (double) this.PanPWMCenter;

            short target = Constrain(pulseWidth, this.PanPWMCenter - (this.PanPWMRange/2),
                this.PanPWMCenter + (this.PanPWMRange/2));
            target *= 4;

            SendCompactMaestroCommand(SetTarget, 0, PanAddress, target);
            return true;
        }

        public bool Tilt(double Angle)
        {
            double angleRange = Math.Abs(this.TiltStartRange - this.TiltEndRange);

            double pulseWidth = ((((double) this.TiltPWMRange)/angleRange)*(Angle - TrimTilt)*_tiltreverse) +
                                (double) this.TiltPWMCenter;

            short target = Constrain(pulseWidth, this.TiltPWMCenter - (this.TiltPWMRange/2),
                this.TiltPWMCenter + (this.TiltPWMRange/2));
            target *= 4;

            SendCompactMaestroCommand(SetTarget, 0, TiltAddress, target);
            return true;
        }

        public bool PanAndTilt(double pan, double tilt)
        {
            // check if we are using 180 + 180 servos
            if (Math.Abs(this.TiltStartRange - this.TiltEndRange) > 120)
            {
                double target = wrap_180(pan - TrimPan);

                Console.WriteLine(target);

                // target > +-90
                if (Math.Abs(target) > 90)
                {
                    if (Tilt(180 - tilt) && Pan(target))
                        return true;
                }
                else
                {
                    if (Tilt(tilt) && Pan(pan))
                        return true;
                }
            }
            else
            {
                if (Tilt(tilt) && Pan(pan))
                    return true;
            }

            return false;
        }

        public bool Close()
        {
            try
            {
                ComPort.Close();
            }
            catch
            {
            }
            return true;
        }

        short Constrain(double input, double min, double max)
        {
            if (input < min)
                return (short) min;
            if (input > max)
                return (short) max;
            return (short) input;
        }

        byte[] SendCompactMaestroCommand(byte cmd, int respByteCount = 0, byte addr = 0xFF, int data = -1)
        {
            byte[] buffer;
            if (addr == 0xFF)
                buffer = new byte[] {cmd};
            else if (data < 0)
                buffer = new byte[] {cmd, addr};
            else
                buffer = new byte[] {cmd, addr, (byte) (data & 0x7F), (byte) ((data >> 7) & 0x7F)};
            ComPort.DiscardInBuffer();
            ComPort.Write(buffer, 0, buffer.Length);
            if (respByteCount > 0)
            {
                buffer = new byte[respByteCount];
                while (ComPort.BytesToRead < respByteCount)
                {
                }
                ComPort.Read(buffer, 0, respByteCount);
            }
            return buffer;
        }
    }
}