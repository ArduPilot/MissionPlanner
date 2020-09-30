using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.GCSViews;

namespace Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Master : ContentPage
    {
        public ListView ListView;

        public MasterDetailPage1Master()
        {
            InitializeComponent();

            BindingContext = new MasterDetailPage1MasterViewModel();
            ListView = MenuItemsListView;
        }

        class MasterDetailPage1MasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailPage1MenuItem> MenuItems { get; set; }
            
            public MasterDetailPage1MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPage1MenuItem>(new[]
                {
                    new MasterDetailPage1MenuItem { Id = 0, Title = "FlightData", TargetType = typeof(FlightData) },
                    new MasterDetailPage1MenuItem { Id = 1, Title = "Video", TargetType = typeof(Video)  },
                    //new MasterDetailPage1MenuItem { Id = 2, Title = "FlightPlanner", TargetType = typeof(MasterDetailPage1Detail)  },
                    //new MasterDetailPage1MenuItem { Id = 3, Title = "Initial Setup", TargetType = typeof(MasterDetailPage1Detail)  },
                    //new MasterDetailPage1MenuItem { Id = 4, Title = "Configuration", TargetType = typeof(MasterDetailPage1Detail)  },
                    //new MasterDetailPage1MenuItem { Id = 5, Title = "Simulation", TargetType = typeof(MasterDetailPage1Detail)  },
                    new MasterDetailPage1MenuItem { Id = 5, Title = "WinForms", TargetType = typeof(WinForms)  },
                    new MasterDetailPage1MenuItem { Id = 6, Title = "Firmware", TargetType = typeof(Firmware)  }
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}