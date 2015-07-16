using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;

namespace MissionPlanner.Controls.BackstageView
{
    public class BackstageViewCollection : CollectionBase
    {
        public BackstageViewPage this[int Index]
        {
            get
            {
                return (BackstageViewPage)List[Index];
            }
        }

        public bool Contains(BackstageViewPage itemType)
        {
            return List.Contains(itemType);
        }

        public int Add(BackstageViewPage itemType)
        {
            return List.Add(itemType);
        }

        public void Remove(BackstageViewPage itemType)
        {
            List.Remove(itemType);
        }

        public void Insert(int index, BackstageViewPage itemType)
        {
            List.Insert(index, itemType);
        }

        public int IndexOf(BackstageViewPage itemType)
        {
            return List.IndexOf(itemType);
        }

    }
}
