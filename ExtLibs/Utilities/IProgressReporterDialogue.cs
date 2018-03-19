using System;
using System.ComponentModel;

namespace MissionPlanner.Utilities
{
    public delegate void DoWorkEventHandler(IProgressReporterDialogue sender);

    public interface IProgressReporterDialogue
    {
        event DoWorkEventHandler DoWork;

        ProgressWorkerEventArgs doWorkArgs { get; set; }

        void Dispose();

        /// <summary>
        /// Called at setup - will kick off the background process on a thread pool thread
        /// </summary>
        void RunBackgroundOperationAsync();

        /// <summary>
        /// Called from the BG thread
        /// </summary>
        /// <param name="progress">progress in %, -1 means inderteminate</param>
        /// <param name="status"></param>
        void UpdateProgressAndStatus(int progress, string status);

        IAsyncResult BeginInvoke(Delegate method);
    }

    public class ProgressWorkerEventArgs : EventArgs
    {
        public string ErrorMessage;
        volatile bool _CancelRequested = false;
        public bool CancelRequested
        {
            get
            {
                return _CancelRequested;
            }
            set
            {
                _CancelRequested = value; if (CancelRequestChanged != null) CancelRequestChanged(this, new PropertyChangedEventArgs("CancelRequested"));
            }
        }
        public volatile bool CancelAcknowledged;

        public event PropertyChangedEventHandler CancelRequestChanged;
    }

}