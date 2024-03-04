using System;

namespace SikRadio
{
    public interface ISikRadioForm : IDisposable
    {
        void Connect();
        void Disconnect();
        void Show();
        bool Enabled { get; set; }
        string Header { get; }
    }
}