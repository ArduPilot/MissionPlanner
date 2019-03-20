using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using OSDConfigurator.Models;

namespace OSDConfigurator.GUI
{
    public partial class LayoutControl : UserControl
    {
        private readonly Visualizer visualizer;
        private ICollection<OSDItem> items;
        private bool itemMoving;
        private Point moveShift;

        public ScreenControl ScreenControl { get; set; }

        public ICollection<OSDItem> Items
        {
            set
            {
                if (items != null)
                    foreach (var item in items)
                        foreach (var option in item.Options)
                            option.Updated -= OptionUpdated;

                items = value;

                if (items != null)
                    foreach (var item in items)
                        foreach (var option in item.Options)
                            option.Updated += OptionUpdated;
            }
        }
        
        public Size CharSize
        {
            set
            {
                this.Size = visualizer.GetCanvasSize(value);
                ReDraw();
            }
        }

        public LayoutControl()
        {
            visualizer = new Visualizer();

            InitializeComponent();
            
            this.Paint += Draw;
            
            Disposed += LayoutControlDisposed;

            this.MouseDown += LayoutControlMouseDown;
            this.MouseMove += LayoutControlMouseMove;
            this.MouseUp += LayoutControlMouseUp;
        }
        
        private void LayoutControlMouseUp(object sender, MouseEventArgs e)
        {
            itemMoving = false;
        }

        private void LayoutControlMouseMove(object sender, MouseEventArgs e)
        {
            if (!itemMoving || ScreenControl == null || ScreenControl.SelectedItem == null)
                return;

            var loc = e.Location;
            loc.Offset(moveShift);

            var newLocation = visualizer.ToOSDLocation(loc);
            
            if (newLocation.X != ScreenControl.SelectedItem.X.Value 
                || newLocation.Y != ScreenControl.SelectedItem.Y.Value)
            {
                ScreenControl.SelectedItem.X.Value = newLocation.X;
                ScreenControl.SelectedItem.Y.Value = newLocation.Y;
                
                ReDraw();
            }
        }

        private void LayoutControlMouseDown(object sender, MouseEventArgs e)
        {
            var hitItem = items?.FirstOrDefault(i => i.Enabled.Value > 0 && visualizer.Contains(i, e.Location));
            
            if (hitItem != null && ScreenControl != null)
            {
                ScreenControl.SelectedItem = hitItem;
                itemMoving = true;

                var currentLoc = visualizer.ToScreenPoint(hitItem, 0);
                moveShift.X = currentLoc.X - e.X;
                moveShift.Y = currentLoc.Y - e.Y;

                ReDraw();
            }
        }

        public void ReDraw()
        {
            this.Invalidate();
        }

        private void LayoutControlDisposed(object sender, EventArgs e)
        {
            Items = null;
        }
        
        private void OptionUpdated(IOSDSetting obj)
        {
            ReDraw();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            visualizer.DrawBackground(e.Graphics);

            if (items != null)
            {
                foreach (var i in items.Where(o => o.Enabled.Value > 0))
                {
                    visualizer.Draw(i, e.Graphics);
                    visualizer.DrawSelection(ScreenControl.SelectedItem, e.Graphics);
                }
            }
        }
    }
}
