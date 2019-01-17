using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDConfigurator.GUI.ItemControls
{
    public class ItemControlBase : UserControl
    {
        protected readonly ScreenControl screenControl;
        protected readonly OSDItem item;

        public ItemControlBase()
        {
        }

        public ItemControlBase(ScreenControl screenControl, OSDItem item)
        {
            this.screenControl = screenControl ?? throw new ArgumentNullException(nameof(screenControl));
            this.item = item ?? throw new ArgumentNullException(nameof(item));

            item.Enabled.Updated += EnabledUpdated;

            Load += ItemControlLoad;
            Disposed += ItemControlDisposed;
        }

        private void ItemControlLoad(object sender, EventArgs e)
        {
            EnabledUpdated(item.Enabled);
        }

        private void ItemControlDisposed(object sender, EventArgs e)
        {
            item.Enabled.Updated -= EnabledUpdated;
        }

        protected virtual void EnabledUpdated(IOSDSetting obj)
        {
        }

        protected void DoSelect()
        {
            screenControl.ItemSelected(item);
        }
    }
}
