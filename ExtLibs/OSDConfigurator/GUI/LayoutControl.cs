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
        private class VisualItem
        {
            public OSDItem Item { get; private set; }

        }

        private readonly ScreenControl screenControl;
        private readonly ICollection<OSDItem> items;
        private readonly Visualizer visualizer;

        private bool itemMoving;
        private Point moveShift;
        
        public LayoutControl(ScreenControl screenControl, ICollection<OSDItem> items, Visualizer visualizer)
        {
            this.screenControl = screenControl;

            this.items = items;
            this.visualizer = visualizer;

            InitializeComponent();
            
            this.Size = visualizer.GetCanvasSize();

            this.Paint += Draw;

            Load += LayoutControlLoad;
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
            if (!itemMoving || screenControl.SelectedItem == null)
                return;

            var loc = e.Location;
            loc.Offset(moveShift);

            var newLocation = visualizer.ToOSDLocation(loc);
            
            if (newLocation.X != screenControl.SelectedItem.X.Value 
                || newLocation.Y != screenControl.SelectedItem.Y.Value)
            {
                screenControl.SelectedItem.X.Value = newLocation.X;
                screenControl.SelectedItem.Y.Value = newLocation.Y;
                
                ReDraw();
            }
        }

        private void LayoutControlMouseDown(object sender, MouseEventArgs e)
        {
            var hitItem = items.FirstOrDefault(i => i.Enabled.Value > 0 && visualizer.Contains(i, e.Location));
            
            if (hitItem != null)
            {
                screenControl.ItemSelected(hitItem);
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
            foreach (var item in items)
                foreach (var option in item.Options)
                    option.Updated -= OptionUpdated;
        }

        private void LayoutControlLoad(object sender, EventArgs e)
        {
            foreach(var item in items)
                foreach(var option in item.Options)
                    option.Updated += OptionUpdated;
        }

        private void OptionUpdated(IOSDSetting obj)
        {
            ReDraw();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            visualizer.DrawBackground(e.Graphics);

            foreach(var i in items.Where(o => o.Enabled.Value > 0))
            {
                visualizer.Draw(i, e.Graphics);
                visualizer.DrawSelection(screenControl.SelectedItem, e.Graphics);
            }
        }
    }
}
