using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AltitudeAngelWings
{
    public static class WpfExtensions
    {
        public static void ScrollToBottom(this ItemsControl itemsControl)
        {
            if (!TryScrollToBottom(itemsControl))
            {
                itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                    new Action(() => TryScrollToBottom(itemsControl)));
            }
        }

        private static bool TryScrollToBottom(ItemsControl itemsControl)
        {
            ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
            object item = generator.Items[generator.Items.Count - 1];
            var container = generator.ContainerFromItem(item) as UIElement;

            if (container == null) return false;

            ScrollContentPresenter presenter = null;
            var current = VisualTreeHelper.GetParent(container) as Visual;

            while ((presenter == null) && (current != itemsControl) && (current != null))
            {
                current = VisualTreeHelper.GetParent(current) as Visual;

                presenter = current as ScrollContentPresenter;
            }

            if (presenter == null) return false;

            presenter.SetVerticalOffset(presenter.ExtentHeight);

            return true;
        }
    }
}
