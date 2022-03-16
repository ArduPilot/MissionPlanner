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

        ~ProgressReporterDialogue()
        {
            Dispose();
        }

        public ProgressReporterDialogue(string title)
        {
            this.title = title;

            _timer = new Timer((c) =>
            {
                Console.WriteLine("ProgressReporterDialogue _timer run");
                if (queue.Count == 0)
                {
                    Console.WriteLine("ProgressReporterDialogue _timer run queue = 0");
                    return;
                }

                while (queue.Count >= 2)
                {
                    lock (queue)
                    {
                        var item2 = queue.Dequeue();
                        Console.WriteLine("Dequeue >=2 " + item2);
                    }
                }

                var item = "";

                lock (queue)
                {
                    item = queue.Dequeue();
                }
                
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Toast(item, TimeSpan.FromSeconds(3));
                });
            }, this, 1000, 1000);
        }

        public ProgressWorkerEventArgs doWorkArgs { get; set; } = new ProgressWorkerEventArgs();

        public event DoWorkEventHandler DoWork;

        public IAsyncResult BeginInvoke(Delegate method)
        {
            return null;
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public void RunBackgroundOperationAsync()
        {
            DoWork?.Invoke(this);

            _timer.Stop();
        }

        Queue<string> queue = new Queue<string>();

        public void UpdateProgressAndStatus(int progress, string status)
        {
            Console.WriteLine("Queue message " + status);
            lock(queue)
                queue.Enqueue(status);
        }

        void IProgressReporterDialogue.BeginInvoke(Delegate method)
        {
          
        }
    }
}