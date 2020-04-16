using System;
using System.Collections.Generic;
using System.Threading;
using Acr.UserDialogs;
using MissionPlanner.Utilities;

namespace Xamarin
{
    internal class ProgressReporterDialogue : IProgressReporterDialogue, IDisposable
    {
        private string title;
        private Timer _timer;

        public ProgressReporterDialogue(string title)
        {
            this.title = title;

            _timer = new Timer((c) =>
            {
                if (queue.Count == 0)
                    return;

                while (queue.Count > 2)
                    queue.Dequeue();

                UserDialogs.Instance.Toast(queue.Dequeue(), TimeSpan.FromSeconds(3));
            }, this, 1000, 1000);
        }

        public ProgressWorkerEventArgs doWorkArgs { get; set; } = new ProgressWorkerEventArgs();

        public event DoWorkEventHandler DoWork;

        public IAsyncResult BeginInvoke(Delegate method)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _timer.Stop();
            //throw new NotImplementedException();
        }

        public void RunBackgroundOperationAsync()
        {
            DoWork?.Invoke(this);
            //throw new NotImplementedException();
        }

        Queue<string> queue = new Queue<string>();

        public void UpdateProgressAndStatus(int progress, string status)
        {
            queue.Enqueue(status);
        }
    }
}