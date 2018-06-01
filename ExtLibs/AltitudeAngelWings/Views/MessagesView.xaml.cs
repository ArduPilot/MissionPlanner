using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AltitudeAngelWings.ViewModels;

namespace AltitudeAngelWings.Views
{
    public partial class MessagesView : IView<MessagesViewModel>
    {
        public MessagesView()
        {
            InitializeComponent();
            Messages.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            
        }

        public MessagesViewModel ViewModel
        {
            get { return DataContext as MessagesViewModel; }
            set { DataContext = value; }
        }

        public void ViewInitialized()
        {
        }

        void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //Messages.ScrollToBottom();
            }
        }
    }
}
