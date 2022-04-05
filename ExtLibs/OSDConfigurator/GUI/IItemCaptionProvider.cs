using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDConfigurator.GUI
{
    public enum CaptionModes
    {
        Realistic,
        Names
    }

    public interface IItemCaptionProvider
    {
        CaptionModes CaptionMode { get; set; }

        string GetItemCaption(OSDItem item, out int xOffset);
    }
}
