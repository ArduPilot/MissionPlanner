using System;
using Acr.UserDialogs;
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

        public ProgressWorkerEventArgs doWorkArgs { get; set; } = new ProgressWorkerEventArgs();

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
            UserDialogs.Instance.Toast(status, TimeSpan.FromSeconds(3));
            //throw new NotImplementedException();
        }
    }
}