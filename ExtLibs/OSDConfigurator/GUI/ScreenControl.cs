using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSDConfigurator.Models;
using OSDConfigurator.GUI.ItemControls;

namespace OSDConfigurator.GUI
{
    public partial class ScreenControl : UserControl
    {
        public static OSDScreen ScreenToCopy { get; set; }

        private readonly OSDScreen screen;
        private readonly IItemCaptionProvider captionProvider;
        private OSDItem selectedItem;

        public OSDItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem == value)
                    return;
                
                selectedItem = value;

                if (groupOptions.Text == selectedItem.Name)
                    return;
                
                groupOptions.Text = selectedItem.Name;
                groupOptions.Controls.Clear();

                foreach (var ctr in OptionControlFactory.Create(selectedItem.Options))
                {
                    ctr.Dock = DockStyle.Top;
                    groupOptions.Controls.Add(ctr);
                }
                
                layoutControl.ReDraw();
            }
        }

        public ScreenControl(OSDScreen screen, IItemCaptionProvider captionProvider)
        {
            this.screen = screen ?? throw new ArgumentNullException(nameof(screen));
            this.captionProvider = captionProvider;

            InitializeComponent();

            layoutControl.ScreenControl = this;
            layoutControl.Items = screen.Items;
            layoutControl.Visualizer = new Visualizer(captionProvider);
           
            cbReducedView.CheckedChanged += (s, e) => SetViewSize();
            cbUseNameCaptions.CheckedChanged += (s, e) => SetCaptionMode();

            btnClearAll.Click += (s, e) => { foreach (var i in screen.Items) i.Enabled.Value = 0; };

            btnCopy.Click += (s, e) => { if (screen.Items.Any()) ScreenToCopy = screen; };

            btnPaste.Click += (s, e) => { if (screen.Items.Any() && ScreenToCopy != null) ScreenToCopy.CopyTo(screen); };                                        

            SetViewSize();
        }

        private void SetCaptionMode()
        {
            captionProvider.CaptionMode = cbUseNameCaptions.Checked ? CaptionModes.Names : CaptionModes.Realistic;
            layoutControl.ReDraw();
        }

        private void SetViewSize()
        {
            layoutControl.CharSize = cbReducedView.Checked ? new Size(12, 18) : new Size(24, 36);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FillScreenOptions();

            if (screen.Items.Any())
            {
                var items = new List<UserControl>();

                foreach (var item in screen.Items)
                    items.Add(new CommonItemControl(this, item));

                items.Reverse();
                items.ForEach(AddItem);
            }
            else
            {
                grEditorOptions.Enabled = false;
            }
        }

        private void FillScreenOptions()
        {
            foreach (var ctr in OptionControlFactory.Create(screen.Options))
            {
                ctr.Dock = DockStyle.Top;
                groupScreenOptions.Controls.Add(ctr);
            }
        }

        private void AddItem(UserControl control)
        {
            panelItemList.Controls.Add(control);
        }        
    }
}
