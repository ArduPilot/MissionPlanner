using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDConfigurator.GUI
{
    [DebuggerDisplay("{setting.Name} ({setting.Value})")]
    public class OptionControlBase : UserControl, IPrioritizedItem
    {
        protected readonly IOSDSetting setting;

        public int Weight { get; private set; }

        // For winforms designer
        public OptionControlBase()
        {

        }

        public OptionControlBase(IOSDSetting setting, int weight)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            setting.Updated += UpdateControl;

            Weight = weight;

            Load += OptionControlLoad;
            Disposed += OptionControlDisposed;
        }

        private void OptionControlLoad(object sender, EventArgs e)
        {
            UpdateControl(setting);
        }

        private void OptionControlDisposed(object sender, EventArgs e)
        {
            setting.Updated -= UpdateControl;
        }

        protected virtual void UpdateControl(IOSDSetting setting)
        {
        }
    }
}
