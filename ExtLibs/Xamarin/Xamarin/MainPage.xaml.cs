using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MissionPlanner.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static MainPage Instance;

        public MainPage()
        {
            Instance = this;
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs exception)
        {
            log.Info(string.Format("{0} ",
                string.IsNullOrEmpty(exception.Exception.StackTrace)
                    ? exception.ToString()
                    : exception.Exception.StackTrace));
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            log.Info(string.Format("{0} ",
                string.IsNullOrEmpty((exception.ExceptionObject as Exception).StackTrace)
                    ? exception.ToString()
                    : (exception.ExceptionObject as Exception).StackTrace));
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailPage1MenuItem;
            if (item == null)
                return;

            var page = (Page) Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            try
            {
                if (Detail is IDeactivate)
                    ((IDeactivate) page).Deactivate();
            }
            catch
            {
            }

            Detail = new NavigationPage(page);
            IsPresented = false;

            try
            {
                if (page is IActivate)
                    ((IActivate) page).Activate();
            }
            catch
            {
            }

            //MasterPage.ListView.SelectedItem = null;
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Test.Radio?.Toggle();
        }
    }
}