using log4net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// This is a mono fix, windows handles this error, mono crashs
    /// </summary>
    [ComVisible(true)]
    public class MyUserControl : System.Windows.Forms.UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// implement an on closing event to tidy up enviroment. 
        /// Using preedefined refrence as can easerly change between form and user control this way.
        /// </summary>
        public event FormClosingEventHandler FormClosing;

        public void Close(object sender, FormClosingEventArgs e)
        {
            if (FormClosing != null)
                FormClosing(sender,e);
        }

        public void Close()
        {
            Close(this, new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
