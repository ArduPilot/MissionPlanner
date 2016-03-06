using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using log4net;

namespace MissionPlanner.Utilities
{
    public class Speech: IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        SpeechSynthesizer _speechwindows;
        System.Diagnostics.Process _speechlinux;

        System.Speech.Synthesis.SynthesizerState _state = SynthesizerState.Ready;

        bool MONO = false;

        public bool IsReady 
        {
            get {
                if (MONO)
                {
                    return _state == SynthesizerState.Ready;
                }
                else
                {
                    return _speechwindows.State == SynthesizerState.Ready;
                }
            }
        }

        public Speech()
        {
            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            log.Info("TTS: init, mono = " + MONO);

            if (MONO)
            {
                _state = SynthesizerState.Ready;
            }
            else
            {
                _speechwindows = new SpeechSynthesizer();
            }
        }

        public void SpeakAsync(string text)
        {
            if (text == null)
                return;

            text = text.Replace("PreArm", "Pre Arm");

            if (MONO)
            {
                try
                {
                    //if (_speechlinux == null)
                    {
                        _state = SynthesizerState.Speaking;
                        _speechlinux = new System.Diagnostics.Process();
                        _speechlinux.StartInfo.RedirectStandardInput = true;
                        _speechlinux.StartInfo.UseShellExecute = false;
                        _speechlinux.StartInfo.FileName = "festival";
                        _speechlinux.Start();
                        _speechlinux.Exited += new EventHandler(_speechlinux_Exited);

                        log.Info("TTS: start " + _speechlinux.StartTime);

                    }

                    _state = SynthesizerState.Speaking;
                    _speechlinux.StandardInput.WriteLine("(SayText \"" + text + "\")");
                    _speechlinux.StandardInput.WriteLine("(quit)");

                    _speechlinux.Close();
                }
                catch { } // ignore errors

                _state = SynthesizerState.Ready;
            }
            else
            {
                _speechwindows.SpeakAsync(text);
            }

            log.Info("TTS: say " + text);
        }

        void _speechlinux_Exited(object sender, EventArgs e)
        {
            log.Info("TTS: exit " + _speechlinux.ExitTime);
            _state = SynthesizerState.Ready;
        }

        public void SpeakAsyncCancelAll()
        {
            if (MONO)
            {
                try
                {
                    if (_speechlinux != null)
                        _speechlinux.Close();
                }
                catch { }
                _state = SynthesizerState.Ready;
            }
            else
            {
                try
                {
                    _speechwindows.SpeakAsyncCancelAll();
                }
                catch { } // System.PlatformNotSupportedException:
            }
        }

        public void Dispose()
        {
            if (_speechwindows != null)
                _speechwindows.Dispose();
            if (_speechlinux != null)
                _speechlinux.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}