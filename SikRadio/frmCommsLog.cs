using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SikRadio
{
    public partial class frmCommsLog : Form
    {
        MissionPlanner.Comms.ICommsSerial _Port;
        bool _Write = false;
        StringBuilder _SB = new StringBuilder();
        int _ListIndex = 0;

        public frmCommsLog(MissionPlanner.Comms.ICommsSerial Port)
        {
            InitializeComponent();
            _Port = Port;
            _Port.ByteWritten += CommsReadEvtHdlr;
            _Port.ByteRead += CommsWriteEvtHdlr;
        }

        void WriteByte(byte B)
        {
            if (B == '\n')
            {
                WriteByte((byte)'\\');
                WriteByte((byte)'n');
                NewLine();
            }
            else if (B == '\r')
            {
                WriteByte((byte)'\\');
                WriteByte((byte)'r');
            }
            else
            {
                _SB.Append(B);
                lb.Items[_ListIndex] = _SB.ToString();
            }
        }

        void NewLine()
        {
            _SB.Clear();
            _ListIndex++;
            if (_Write)
            {
                WriteByte((byte)'>');
            }
            else
            {
                WriteByte((byte)'<');
            }
        }

        void CommsReadEvtHdlr(byte B)
        {
            if (_Write)
            {
                _Write = false;
                NewLine();
            }
            WriteByte(B);
        }

        void CommsWriteEvtHdlr(byte B)
        {
            if (!_Write)
            {
                _Write = true;
                NewLine();
            }
            WriteByte(B);
        }

        private void frmCommsLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Port.ByteRead -= CommsReadEvtHdlr;
            _Port.ByteWritten -= CommsWriteEvtHdlr;
        }
    }
}
