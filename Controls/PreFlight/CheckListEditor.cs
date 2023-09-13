using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.Controls.PreFlight
{
    public partial class CheckListEditor : Form
    {
        CheckListControl _parent;
        //Initialize new bool for when pressing the Add button.
        private bool addButtonClick = false;
        //Checklist Input variable
        private CheckListInput wrnctl;
        //Initialize List of Controls' desc textboxes
        private List<TextBox> wrnctlDescList = new List<TextBox>();
        //Initialize List of Controls' text textboxes
        private List<TextBox> wrnctlTextList = new List<TextBox>();

        public CheckListEditor(CheckListControl parent)
        {
            _parent = parent;

            InitializeComponent();

            if (DesignMode)
            {
                _parent.CheckListItems.Add(new CheckListItem() { Description = "desc", Name = "name", Text = "text" });
            }

            reload();
        }

        public void AddNewCheck()
        {
            //Make the panel invisible.
            panel1.Visible = false;
            //Initialize the y for the new checklist item.
            int y = 0;

            lock (_parent.CheckListItems)
            {
                //Variable for the last item in the checklist items list.
                var lastItem = _parent.CheckListItems.LastOrDefault();
                //Set the Y for the checklist item as the bottom of the previous input.
                y = wrnctl.Bottom;
                //Set the input using the lastItem variable.
                wrnctl = addwarningcontrol(5, y, lastItem);
            }
            //Set the theme, make the layout for the panel1 and make the panel1 visible
            Utilities.ThemeManager.ApplyThemeTo(this);
            panel1.PerformLayout();
            panel1.Visible = true;
        }
        public void reload()
        {
            panel1.Visible = false;
            panel1.Controls.Clear();
            //clear the list of wrnctl desc textboxes.
            wrnctlDescList.Clear();
            //clear the list of wrnctl text textboxes.
            wrnctlTextList.Clear();
            int y = 0;

            lock (_parent.CheckListItems)
            {
                foreach (var item in _parent.CheckListItems)
                {
                    var wrnctl = addwarningcontrol(5, y, item);

                    y = wrnctl.Bottom;
                }
            }

            Utilities.ThemeManager.ApplyThemeTo(this);
            panel1.PerformLayout();
            panel1.Visible = true;
        }

        CheckListInput addwarningcontrol(int x, int y, CheckListItem item, bool hideforchild = false)
        {
            //Set instance of the Checklist Input
            wrnctl = new CheckListInput(_parent, item);

            wrnctl.ReloadList += wrnctl_ChildAdd;

            wrnctl.Location = new Point(x, y);

            if (hideforchild)
            {
                wrnctl.TXT_text.Visible = false;
                wrnctl.TXT_desc.Visible = false;
                wrnctl.CMB_colour1.Visible = false;
                wrnctl.CMB_colour2.Visible = false;
            }

            panel1.Controls.Add(wrnctl);
            //Add the wrnctl's desc textbox to the list of desc textboxes.
            wrnctlDescList.Add(wrnctl.TXT_desc);
            //Add the wrnctl's text textbox to the list of text textboxes.
            wrnctlTextList.Add(wrnctl.TXT_text);

            //If on the last control & the add button was clicked, then add PlaceHolder text for both the desc and text textboxes for the wrnctl input.
            if (addButtonClick == true && panel1.Controls.Count == _parent.CheckListItems.Count)
            {
                //set the text for the textboxes accordingly.
                wrnctl.TXT_desc.Text = "add description";
                wrnctl.TXT_text.Text = "add value";
            }
            y = wrnctl.Bottom;

            if (item.Child != null)
            {
                wrnctl = addwarningcontrol(x += 5, y, item.Child, true);
            }
            return wrnctl;
        }

        void wrnctl_ChildAdd(object sender, EventArgs e)
        {
            reload();
            //Clear placeholder text method
            ClearPlaceHolderText();
        }

        private void BUT_Add_Click(object sender, EventArgs e)
        {
            //Set the addButtonClick bool to true when the Add button is clicked.
            addButtonClick = true;
            var newcw = new CheckListItem();

            CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;
            newcw.SetField(newcw.GetOptions()[0]);

            lock (_parent.CheckListItems)
            {
                _parent.CheckListItems.Add(newcw);
            }
            //Add the new checklist item with the AddNewCheck method.
            AddNewCheck();
            //Event Handler for entering the warning controls' Text and Description Fields - needed in the button add method for when the add button is clicked.
            wrnctl.TXT_desc.Enter += new EventHandler(TXT_desc_Enter);
            wrnctl.TXT_text.Enter += new EventHandler(TXT_text_Enter);

            //Set the bool to false at the end of the Add button method
            addButtonClick = false;
        }

        //Used for PlaceHolder Text of the Checklist Input
        private void TXT_desc_Enter(object sender, EventArgs e)
        {
            //string for the Textbox's Text (The clicked on textbox)
            string itemClickedText = (sender as TextBox).Text;
            //Check if the string equals the text of a new desc Textbox. If the same, make its value an empty string.
            if (itemClickedText == "add description")
            {
                itemClickedText = string.Empty;
            }
            //loop until the textbox which was clicked on is reached.
            for (int i = 0; i < _parent.CheckListItems.Count; i++)
            {
                if ((sender as TextBox) == wrnctlDescList[i])
                {
                    //change the text of the textbox to the itemClickedText string value(Empty string).
                    wrnctlDescList[i].Text = itemClickedText;
                    return;
                }
            }
        }
        private void TXT_text_Enter(object sender, EventArgs e)
        {
            //string for the Textbox's Text (The clicked on textbox)
            string itemClickedText = (sender as TextBox).Text;
            //Check if the string equals the text of a new text Textbox. If the same, make its value an empty string.
            if (itemClickedText == "add value")
            {
                itemClickedText = string.Empty;
            }
            //loop until the textbox which was clicked on is reached.
            for (int i = 0; i < _parent.CheckListItems.Count; i++)
            {
                if ((sender as TextBox) == wrnctlTextList[i])
                {
                    //change the text of the textbox to the itemClickedText string value(Empty string).
                    wrnctlTextList[i].Text = itemClickedText;
                    return;
                }
            }
        }

        private void ClearPlaceHolderText()
        {
            //Eventhandlers for the desc and text texboxes are added when entering the text boxes
            //checks if the PlaceHolder text is in the textboxes for all checklist inputs
            for (int i = 0; i < _parent.CheckListItems.Count; i++)
            {
                //if the place holder text is the text in the textboxes, then the relevant event handlers are used.
                if (wrnctlDescList[i].Text == "add description")
                {
                    wrnctlDescList[i].Enter += new EventHandler(TXT_desc_Enter);
                }
                if (wrnctlTextList[i].Text == "add value")
                {
                    wrnctlTextList[i].Enter += new EventHandler(TXT_text_Enter);
                }
            }
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            _parent.SaveConfig();
            //Clear placeholder text method
            ClearPlaceHolderText();
        }

        private void CheckListEditor_Load(object sender, EventArgs e)
        {
            //Clear placeholder text method
            ClearPlaceHolderText();
        }
    }
}