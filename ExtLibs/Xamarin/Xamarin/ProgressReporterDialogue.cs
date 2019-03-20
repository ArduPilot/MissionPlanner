using System;
using MissionPlanner.Utilities;

namespace Xamarin
{
    internal class ProgressReporterDialogue : IProgressReporterDialogue
    {
        private string title;

        public ProgressReporterDialogue(string title)
        {
            this.title = title;
        }

        public ProgressWorkerEventArgs doWorkArgs { get; set; }

        public event DoWorkEventHandler DoWork;

        public IAsyncResult BeginInvoke(Delegate method)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void RunBackgroundOperationAsync()
        {
            DoWork?.Invoke(this);
            //throw new NotImplementedException();
        }

        public void UpdateProgressAndStatus(int progress, string status)
        {
            //throw new NotImplementedException();
        }
    }
}