using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using AltitudeAngelWings.ApiClient.CodeProvider;
using AltitudeAngelWings.Views;

namespace AltitudeAngelWings
{
    public class UserInterfaceMain : IDisposable
    {
        public SynchronizationContext Context { get; private set; }

        public bool IsInDesignMode => _mainView == null || DesignerProperties.GetIsInDesignMode(_mainView.Value);


        public UserInterfaceMain(Lazy<MainView> mainView)
        {
            _mainView = mainView;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Run()
        {
            _uiThread = new Thread(UIThreadDipatcherLoop)
            {
                IsBackground = true,
                Name = "AA Wings UI Thread",
                CurrentUICulture = CultureInfo.CurrentCulture,
                CurrentCulture = CultureInfo.CurrentCulture
            };
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
        }

        public void ShowMainWindow()
        {
            Context.Post(s =>
            {
                WPFAuthorizeDisplay = new WpfAuthorizeDisplay();
                DoShowMainWindow();
            }, null);
        }

        public bool WaitUntilUIReady(int timeout)
        {
            return _uiReady.WaitOne(timeout);
        }

        public void WaitUntilUIReady()
        {
            _uiReady.WaitOne();
        }

        private void UIThreadDipatcherLoop()
        {
            Debug.WriteLine("Starting AA Wings UI thread.");

            // Set the UI culture for WPF correctly
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof (FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            _dispatcher = Dispatcher.CurrentDispatcher;
            Context = new DispatcherSynchronizationContext(_dispatcher);
            _dispatcher.UnhandledException += UnhandledException;
            SynchronizationContext.SetSynchronizationContext(Context);

            // Ensure that the view models are created on the UI thread or it'll barf later

            _uiReady.Set();

            Dispatcher.Run();

            Debug.WriteLine("AA Wings UI thread has exited.");
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("Unhandled exception in AA Wings UI thread");
            Debug.WriteLine("{0}", e.Exception);
        }

        private void DoShowMainWindow()
        {
            _mainView.Value.Show();
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_uiThread != null)
                {
                    Debug.WriteLine("Shutting down AA Wings UI Thread.");
                    if (_uiReady.WaitOne(2000))
                    {
                        _dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
                        if (!_uiThread.Join(5000))
                        {
                            _uiThread.Abort();
                            Debug.WriteLine("Aborted AA Wings UI Thread.");
                        }

                        Debug.WriteLine("Stopped AA Wings UI Thread.");
                    }
                    else
                    {
                        _uiThread.Abort();
                        Debug.WriteLine("Aborted AA Wings UI Thread.");
                    }


                    _uiThread = null;
                    _dispatcher = null;
                    Context = null;
                }
            }
        }

        private readonly Lazy<MainView> _mainView;
        private readonly ManualResetEvent _uiReady = new ManualResetEvent(false);
        private Dispatcher _dispatcher;
        private Thread _uiThread;
        public WpfAuthorizeDisplay WPFAuthorizeDisplay { get; private set; }
    }
}
