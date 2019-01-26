using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDConfigurator.GUI
{
    public interface IPrioritizedItem
    {
        int Weight { get; }
    }

    public static class OptionControlFactory
    {
        private static class Weight
        {
            public static int ENABLED = 100;
            public static int X = 80;
            public static int Y = 75;

            public static int MEDIUM = 50;
        }
        
        public static IEnumerable<Control> Create(IEnumerable<IOSDSetting> settings)
        {
            var controls = settings.Select(Create).ToList();

            controls.Sort(ControlComparer);

            return controls;
        }

        private static int ControlComparer(Control x, Control y)
        {
            var xpi = x as IPrioritizedItem;
            var ypi = y as IPrioritizedItem;

            var xp = xpi != null ? xpi.Weight : Weight.MEDIUM;
            var yp = ypi != null ? ypi.Weight : Weight.MEDIUM;

            if (xp == yp) return 0;
            if (xp > yp) return 1;
            else return -1;
        }

        public static Control Create(IOSDSetting setting)
        {
            switch(setting.Name)
            {
                case "OSD_FONT":
                    return new DropdownSettingControl(setting, new[] { "Clarity", "Clarity Medium", "BF Style", "Bold", "Digital" }, Weight.MEDIUM);

                case "OSD_UNITS":
                    return new DropdownSettingControl(setting, new[] { "Metric", "Imperial", "SI", "Aviation" }, Weight.MEDIUM);

                case "OSD_SW_METHOD":
                    return new DropdownSettingControl(setting, new[] { "0: RC change", "1: PWM", "2: Low-to-High" }, Weight.MEDIUM);

                case "OSD_OPTIONS":
                    return new BitwiseSettingControl(setting, new[] { "Decimal Pack", "Inverted Wind", "Inverted AH Roll" }, Weight.MEDIUM);

                case "OSD_H_OFFSET":
                case "OSD_V_OFFSET":
                    return new IntSpinSettingControl(setting, Weight.MEDIUM);
            }

            if (setting.Name.EndsWith("_EN") || setting.Name.EndsWith("_ENABLE"))
                return new BoolSettingControl(setting, Weight.ENABLED);

            if (setting.Name.EndsWith("_X"))
                return new IntSpinSettingControl(setting, Weight.X);

            if (setting.Name.EndsWith("_Y"))
                return new IntSpinSettingControl(setting, Weight.Y);
            
            return new IntSettingControl(setting, Weight.MEDIUM);
        }
    }
}
