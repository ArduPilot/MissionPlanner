using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;

namespace MissionPlanner
{
    public class GridPlugin : MissionPlanner.Plugin.Plugin
    {
        

        ToolStripMenuItem but;

        public override string Name
        {
            get { return "Grid"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()   //seems to be where the application is loaded/initialized/and added to the program
        {                               //can't be unloaded after it is loaded && also is loaded at the startup of the build/run
            Grid.Host2 = Host;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridUI));
            var temp = (string)(resources.GetObject("$this.Text"));

            but = new ToolStripMenuItem(temp);
            but.Click += but_Click;

            bool hit = false;                           //variable to show if the dropdown was created or not
            ToolStripItemCollection col = Host.FPMenuMap.Items;
            int index = col.Count;
            foreach (ToolStripItem item in col)         //increments through all of the dropdowns to find the AutoWP one
            {
                if (item.Text.Equals(Strings.AutoWP)) //this is what adds the Survey(Grid) part to the dropdown on AutoWP
                {
                    index = col.IndexOf(item);
                    ((ToolStripMenuItem)item).DropDownItems.Add(but);       //adds the Survey(Grid) dropdown
                    hit = true;              //shows that the Survey(Grid) dropdown was successfully created
                    break;                   ///exit the creation of the plugin dropdown
                }
            }

            if (hit == false)
                col.Add(but);                //not quite sure what this line does yet -> maybe just adds the dropdown elsewhere

            return true;
        }


        void but_Click(object sender, EventArgs e)
        {                                       //line below is passing in itself as its own plugin
            var gridui = new GridUI(this);      //calls the GridUI.cs program file and created a new List and Dictionary
            MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(gridui);

            if (Host.FPDrawnPolygon != null && Host.FPDrawnPolygon.Points.Count > 2)    //needs more than 3 WP to create a grid
            {                                     //because obviouslly you can't create a grid with just a line that is defined
                gridui.ShowDialog();              //this is where the separate window opens with the yellow flight path shown
            }
            else
            {
                if (CustomMessageBox.Show("No polygon defined. Load a file?", "Load File", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {                        //this asks the user a yes or no question to load a file
                    gridui.LoadGrid();   //if yes, they get the loaded file and produce a result and load the grid
                    gridui.ShowDialog(); //I think that this is the function that adds the grid to the map (addtomap replacement)
                }
                else                 //if no, tells the user to define a polygon (becasue you can't create a grid with no polygon)
                {
                    CustomMessageBox.Show("Please define a polygon.", "Error");
                }
            }
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
