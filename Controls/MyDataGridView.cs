using System;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class MyDataGridView : DataGridView
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            // mono bug - when deleting all rows, the sharedrow no longer exists and throws System.ArgumentOutOfRangeException
            try
            {
                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        protected override void OnCellMouseLeave(DataGridViewCellEventArgs e)
        {
            // mono bug - when you use a button in the row to delete the row, this fires
            try
            {
                base.OnCellMouseLeave(e);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                base.OnMouseMove(e);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }
    }
}