using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Media.Animation;
using AltitudeAngelWings.ViewModels;

namespace AltitudeAngelWings.Views
{
    public partial class MainView : IView<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel { get { return DataContext as MainViewModel; } set { DataContext = value; } }

        public void ViewInitialized()
        {
            ViewModel.SignInState.SubscribeVisualState(LayoutRoot);
            ViewModel.PermitStatus.SubscribeVisualState(LayoutRoot);
            ViewModel.TelemetryPulse.ObserveOnDispatcher()
                .Subscribe(isGood =>
                {
                    if (isGood)
                    {
                        ((Storyboard)Resources["GoodPulse"]).Begin();
                    }
                });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
