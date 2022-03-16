using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Utilities;
using SharpDX.DirectInput;

namespace MissionPlanner.Joystick
{
    public class JoystickLinux : JoystickBase
    {
        private FileStream fs;
        public struct js_event
        {
            /// <summary>
            /// 0 event timestamp in milliseconds
            /// </summary>
            public UInt32 time
            {
                get { return BitConverter.ToUInt32(raw, 0); }
            } /* event timestamp in milliseconds */

            /// <summary>
            /// 4 value
            /// </summary>
            public short value
            {
                get { return BitConverter.ToInt16(raw, 4); }
            } /* value */

            /// <summary>
            /// 6 event type
            /// </summary>
            public byte type
            {
                get { return raw[6]; }
            } /* event type */

            /// <summary>
            /// 7 axis/button number
            /// </summary>
            public byte number
            {
                get { return raw[7]; }
            } /* axis/button number */

            public byte[] raw;
        }

        ~JoystickLinux()
        {
            log.Info("JoystickLinux .dtor");

            if(fs!= null)
                fs.Close();
            fs = null;
        }

        public void Start(string deviceFile)
        {
            //Mono.Unix.UnixSymbolicLinkInfo i = new Mono.Unix.UnixSymbolicLinkInfo( deviceFile );

            //Console.WriteLine(i.ContentsPath); // ../js0
            //Console.WriteLine(i.FullName); // fullpath
            //Console.WriteLine(i.Name); // filename

            //var path = "/dev/input/by-id/" + i.ContentsPath;

            var path = deviceFile;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            SemaphoreSlim run = new SemaphoreSlim(1);

            new Thread(() =>
            {
                if (fs != null)
                    fs.Close();

                // Read loop.
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                log.Info("opened " + path);

                var buff = new js_event() {raw = new byte[8]};
                Joystick j = new Joystick();

                var started = false;

                while (true)
                {
                    try
                    {
                        // Read 8 bytes from file and analyze.
                        var read = fs.Read(buff.raw, 0, 8);
                        if (read != 8)
                            continue;

                        //Console.WriteLine(buff.ToJSON());

                        j.DetectChange(buff);
                        /*
                        // Prints Axis values
                        foreach (byte key in j.Axis.Keys)
                        {
                            Console.WriteLine(string.Format("Axis{0}: {1}", key, j.Axis[key]));
                        }
                
                        // Prints Buttons values
                        foreach (byte key in j.Button.Keys)
                        {
                            Console.WriteLine(string.Format("Button{0}: {1}", key, j.Button[key]));
                        }
                        */
                        state = new LinuxJoystickState(
                            j.Axis.ToDictionary(a => a.Key, a => (ushort) ((int) a.Value + (65535 / 2))),
                            j.Button.ToDictionary(a => a.Key, a => a.Value));

                        if (started == false)
                        {
                            log.Info("Joystick Task Started");
                            run.Release();
                        }

                        started = true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        break;
                    }
                }

                log.Info("Joystick Task Ended");
            }).Start();

            // wait for the task to start
            if (!run.Wait(1000))
            {
                log.Error("Failed to Start joystick task");
                throw new Exception("Fail to start joystick task");
            }

            Thread.Sleep(200);
        }

        /// <summary>
        /// https://www.kernel.org/doc/html/latest/input/joydev/joystick-api.html
        /// http://mpolaczyk.pl/raspberry-pi-mono-c-joystick-handler/
        /// </summary>
        public class Joystick
        {
            public Joystick()
            {
                Button = new Dictionary<byte, bool>();
                Axis = new Dictionary<byte, short>();
            }

            enum STATE : byte
            {
                PRESSED = 0x01,
                RELEASED = 0x00
            }

            enum TYPE : byte
            {
                AXIS = 0x02,
                BUTTON = 0x01
            }

            enum MODE : byte
            {
                CONFIGURATION = 0x80,
                VALUE = 0x00
            }

            public Dictionary<byte, bool> Button;

            public Dictionary<byte, short> Axis;

            public void DetectChange(js_event buff)
            {
                // JS_EVENT_INIT           
                if (checkBit(buff.type, (byte) MODE.CONFIGURATION))
                {
                    if (checkBit(buff.type, (byte) TYPE.AXIS))
                    {
                        byte key = (byte) buff.number;
                        if (!Axis.ContainsKey(key))
                        {
                            Axis.Add(key, 0);
                            return;
                        }
                    }
                    else if (checkBit(buff.type, (byte) TYPE.BUTTON))
                    {
                        byte key = (byte) buff.number;
                        if (!Button.ContainsKey(key))
                        {
                            Button.Add((byte) buff.number, false);
                            return;
                        }
                    }
                }

                if (checkBit(buff.type, (byte) TYPE.AXIS))
                {
                    short value = buff.value;
                    Axis[(byte) buff.number] = value;
                    return;
                }
                else if (checkBit(buff.type, (byte) TYPE.BUTTON))
                {
                    Button[(byte) buff.number] = buff.value == (byte) STATE.PRESSED;
                    return;
                }
            }

            bool checkBit(byte value, byte flag)
            {
                byte c = (byte) (value & flag);
                return c == (byte) flag;
            }
        }

        public JoystickLinux(Func<MAVLinkInterface> currentInterface) : base(currentInterface)
        {
        }

        public override bool AcquireJoystick(string name)
        {
            if (fs != null)
            {
                log.Info("AcquireJoystick already Acquired " + name);
                return true;
            }

            try
            {
                log.Info("AcquireJoystick " + name);
                Start("/dev/input/by-id/" + name);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return false;
            }

            return true;
        }

        public override int getNumberPOV()
        {
            return 0;
        }

        public override bool IsJoystickValid()
        {
            return fs != null;
        }

        public override IMyJoystickState GetCurrentState()
        {
            if (state == null)
                log.Error("GetCurrentState returning NULL");

            if (fs == null)
                log.Error("GetCurrentState not open");

            var timeout = DateTime.Now.AddSeconds(1);
            while (state == null && DateTime.Now < timeout)
                Thread.Sleep(50);

            return state;
        }

        public override void UnAcquireJoyStick()
        {
            log.Info("UnAcquireJoyStick");
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
        }

        public override int getNumButtons()
        {
            byte number_of_buttons = 10;
            //Native.ioctl(fs.Handle.ToInt32(), (int) JSIOCGBUTTONS, ref number_of_buttons);

            GCHandle obj1Handle = GCHandle.Alloc(number_of_buttons , GCHandleType.Pinned);
            log.Info("getNumButtons");
            //Syscall.fcntl(fs.SafeFileHandle.DangerousGetHandle().ToInt32(), (FcntlCommand) JSIOCGBUTTONS, obj1Handle.AddrOfPinnedObject());
            try
            {
                Native.ioctl(fs.SafeFileHandle.DangerousGetHandle(), (int) JSIOCGBUTTONS, ref number_of_buttons);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            log.Info("getNumButtons " + number_of_buttons);
            return number_of_buttons;
        }

        /*
         * 
 * IOCTL commands for joystick driver
         *
         * https://stackoverflow.com/questions/34519564/dealing-with-file-handles-using-mono-and-p-invoke
         * https://elixir.bootlin.com/linux/latest/source/include/uapi/asm-generic/ioctl.h#L86
 
        /*#define JSIOCGVERSION	=	_IOR('j', 0x01, __u32)				/* get driver version */
        private uint JSIOCGAXES => _IOR('j', 0x11, typeof(byte));			/* get number of axes */
        private uint JSIOCGBUTTONS => _IOR('j', 0x12, typeof(byte)); /* get number of buttons */

        uint JSIOCGNAME(uint len) => _IOC(_IOC_READ, 'j', 0x13, len);		/* get identifier string */
        /*#define JSIOCSCORR	=	_IOW('j', 0x21, struct js_corr)			/* set correction values */
        /*#define JSIOCGCORR	=	_IOR('j', 0x22, struct js_corr)			/* get correction values */
        /*#define JSIOCSAXMAP	=	_IOW('j', 0x31, __u8[ABS_CNT])			/* set axis mapping */
        /*#define JSIOCGAXMAP	=	_IOR('j', 0x32, __u8[ABS_CNT])			/* get axis mapping */
        /*#define JSIOCSBTNMAP	=	_IOW('j', 0x33, __u16[KEY_MAX - BTN_MISC + 1])	/* set button mapping */
        /*#define JSIOCGBTNMAP	=	_IOR('j', 0x34, __u16[KEY_MAX - BTN_MISC + 1])	/* get button mapping */


        private const int _IOC_NRBITS = 8;
        private const int _IOC_TYPEBITS = 8;


        private const int _IOC_SIZEBITS = 14;

        private const int _IOC_DIRBITS = 2;

        private const int _IOC_NRMASK = ((1 << _IOC_NRBITS) - 1);
        private const int _IOC_TYPEMASK = ((1 << _IOC_TYPEBITS) - 1);
        private const int _IOC_SIZEMASK = ((1 << _IOC_SIZEBITS) - 1);
        private const int _IOC_DIRMASK = ((1 << _IOC_DIRBITS) - 1);

        private const int _IOC_NRSHIFT = 0;
        private const int _IOC_TYPESHIFT = (_IOC_NRSHIFT + _IOC_NRBITS);
        private const int _IOC_SIZESHIFT = (_IOC_TYPESHIFT + _IOC_TYPEBITS);
        private const int _IOC_DIRSHIFT = (_IOC_SIZESHIFT + _IOC_SIZEBITS);

        private const uint _IOC_READ = 2U;

        static uint _IOC_TYPECHECK(Type t)
        {
            return (uint) System.Runtime.InteropServices.Marshal.SizeOf(t);
        }

        static uint _IOC(uint dir, uint type, uint nr, uint size) =>
            ((uint)
                (((dir) << _IOC_DIRSHIFT) |
                 ((type) << _IOC_TYPESHIFT) |
                 ((nr) << _IOC_NRSHIFT) |
                 ((size) << _IOC_SIZESHIFT)));

        static uint _IOR(uint type, uint nr, Type size) => _IOC(_IOC_READ, (type), (nr), (_IOC_TYPECHECK(size)));

        public new static IList<string> getDevices()
        {
            try
            {
                return Directory.GetFiles("/dev/input/by-id/", "*joystick")
                    .Where(a => !a.Contains("-event-"))
                    .Select(a => a.Replace("/dev/input/by-id/", ""))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public new static JoystickBase getJoyStickByName(string name)
        {
            var joysticklist = getDevices();

            foreach (var device in joysticklist)
            {
                if (device == name)
                {
                    var js = new JoystickLinux(()=>null);
                    js.AcquireJoystick(name);
                    return js;
                }
            }

            return null;
        }

        public override void Dispose()
        {
            base.Dispose();
            if(fs != null)
                fs.Close();
            fs = null;
        }
    }
}