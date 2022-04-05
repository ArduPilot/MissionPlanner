using OSDConfigurator.Extensions;
using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace OSDConfigurator.GUI.Osd56ItemsSetup
{
    public partial class SetupDialog : Form
    {
        public class Change
        {
            public byte Screen { get; set; }
            public byte Index { get; set; }
            public string ParamName { get; set; }
            public byte Type { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public double Increment { get; set; }
        }

        public class OsdItemWrapper
        {
            public OSDItem OSDItem { get; set; }
            public string AssignedFunction { get; set; }

            public byte Screen
            {
                get 
                {
                    return OSDItem.Options.First().Name.TryParseScreenAndIndex(out byte screen, out byte index)
                        ? screen
                        : throw new Exception();
                }
            }

            public byte Index
            {
                get
                {
                    return OSDItem.Options.First().Name.TryParseScreenAndIndex(out byte screen, out byte index)
                        ? index
                        : throw new Exception();
                }
            }
        }

        public SetupDialog(
            string[] allMavParameters,
            IEnumerable<OSDScreen> osdScreens,
            ICollection<(byte Screen, byte Index, string Name)> assignedFunctions)
            :this()
        {
            var osdItems = new List<OsdItemWrapper>();

            foreach(var osdScreen in osdScreens)
            {
                foreach (var item in osdScreen.Items)
                {
                    var wrapper = new OsdItemWrapper { OSDItem = item };
                    wrapper.AssignedFunction = assignedFunctions.FirstOrDefault(o => wrapper.Screen == o.Screen && wrapper.Index == o.Index).Name;
                    osdItems.Add(wrapper);
                }
            }

            CreateOsdPanels(osdItems.GroupBy(o => o.Screen), allMavParameters);
        }

        private void CreateOsdPanels(IEnumerable<IGrouping<byte, OsdItemWrapper>> perScreenGroups, string[] allMavParameters)
        {
            foreach (var screenGroup in perScreenGroups.Reverse())
            {
                var sortedList = new List<OsdItemWrapper>(screenGroup);
                sortedList.Sort((x, y) => x.Index.CompareTo(y.Index));
                sortedList.Reverse();

                // Controls
                foreach (var osdItem in sortedList)
                {
                    pContent.Controls.Add(new ItemControl(osdItem, allMavParameters)
                    {
                        Dock = DockStyle.Top
                    });

                    pContent.Controls.Add(new Label { Height = 3, Dock = DockStyle.Top });
                }

                // OSD Panel Name
                pContent.Controls.Add(new Label { Text = $"OSD {screenGroup.Key}", Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleCenter});
            }
        }

        public SetupDialog()
        {
            InitializeComponent();
        }

        public IEnumerable<Change> GetChangedItems()
        {
            foreach (var itemControl in pContent.Controls.OfType<ItemControl>())
            {
                if (itemControl.IsDirty(out SetupDialog.Change change))
                    yield return change;
            }
        }
    }
}
