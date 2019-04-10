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

namespace Xamarin
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
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
                    new MasterDetailPage1MenuItem { Id = 0, Title = "FlightData", TargetType = typeof(MainPage_old) },
                    new MasterDetailPage1MenuItem { Id = 1, Title = "FlightPlanner", TargetType = typeof(MasterDetailPage1Detail)  },
                    new MasterDetailPage1MenuItem { Id = 2, Title = "Initial Setup", TargetType = typeof(MasterDetailPage1Detail)  },
                    new MasterDetailPage1MenuItem { Id = 3, Title = "Configuration", TargetType = typeof(MasterDetailPage1Detail)  },
                    new MasterDetailPage1MenuItem { Id = 4, Title = "Simulation", TargetType = typeof(MasterDetailPage1Detail)  },
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