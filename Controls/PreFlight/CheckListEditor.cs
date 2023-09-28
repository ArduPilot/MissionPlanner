﻿using System;
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

        //bool to see if the item has been removed
        private bool removed = false;
        //List for Panel1 controls - used to distinguish between the PArent and Child checklistItems (Parent Items being the items with visible textboxes, Child Items are the items with textboxes not showing)
        List<object> PanelOneControls = new List<object>();
        List<object> PanelOneChildControls = new List<object>();
        //used for the _parent.CheckListItems Child, removing from the Preflight ChecklistItem list
        CheckListItem currentChildItem = new CheckListItem();

        //List of All the Panel1 Controls in the Editor Form
        List<CheckListInput> EditorChecklistInputList = new List<CheckListInput>();

        //Parent item Control - used for setting the panel1 Control item needed
        Control ParentItem = new Control();
        //Bool to check when the secondary add button is clicked to add a child ChecklistInput Item
        private bool secondaryAddButton = false;

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
            try
            {
                //Initialize the y for the new checklist item.
                int y = 0;

                lock (_parent.CheckListItems)
                {
                    //Variable for the last item in the checklist items list.
                    var lastItem = _parent.CheckListItems.LastOrDefault();
                    //Set the Y for the checklist item as the bottom of the previous input.
                    y = panel1.Controls[panel1.Controls.Count - 1].Bottom;
                    //Set the input using the lastItem variable.
                    wrnctl = addwarningcontrol(5, y, lastItem);
                    wrnctl.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

            //If the add button was clicked, then add PlaceHolder text for both the desc and text textboxes for the wrnctl input.
            if (addButtonClick == true)
            {
                //set the text for the textboxes accordingly.
                wrnctl.TXT_desc.Text = "add description";
                wrnctl.TXT_text.Text = "add value";
            }
            y = wrnctl.Bottom;

            //used for when there is a secondary + button click
            if (item.Child != null)
            {
                wrnctl = addwarningcontrol(x += 5, y, item.Child, true);
            }
            return wrnctl;
        }

        //remove selected item
        private void RemoveSelectedItem(int i, int k, Control _ChecklistControl, CheckListInput _ChecklistInputItem, CheckListItem _ChecklistItemSelected)
        {
            //Remove Control that is clicked on
            if ((panel1.Controls[i] == _ChecklistControl))
            {
                removed = true;
                //If the selected item is not a child (but a parent that has textboxes, etc. visible), then remove the item
                if (_ChecklistInputItem.TXT_text.Visible == true || _ChecklistInputItem.TXT_desc.Visible == true || _ChecklistInputItem.CMB_colour1.Visible == true || _ChecklistInputItem.CMB_colour2.Visible == true)
                {
                    //Variable for the Selected item where the parent Checklist item is equal to the Checklist item needed
                    var PanelChecklistItemSelected = _parent.CheckListItems.Select(x => _ChecklistItemSelected).FirstOrDefault();
                    //Remove the checklist item - Preflight Checks
                    _parent.CheckListItems.Remove(PanelChecklistItemSelected);
                    //Remove from the checklist Controls, Edit screen
                    panel1.Controls.RemoveAt(i);
                    //Remove the item from the list of desc textboxes.
                    wrnctlDescList.RemoveAt(i);
                    //Remove the item from the list of text textboxes.
                    wrnctlTextList.RemoveAt(i);
                    //If the selected checklist items' child item is not null, then do the following
                    if (_ChecklistItemSelected.Child != null)
                    {
                        //Removing the item from the checklistinput list, which was removed in the block above
                        EditorChecklistInputList.RemoveAt(i);
                        //For the items that are after the currently selected item
                        for (int j = i; j < panel1.Controls.Count; j++)
                        {
                            if (j < panel1.Controls.Count)
                            {
                                //Variable for the next Checklist Input in the list of all the Checklist Inputs
                                var nextChecklistInput = EditorChecklistInputList[j];

                                //If the nextChecklistInput variable is a Child  Checklist Input, remove the item from the panel controls and the full ChecklistInput list at the 'j' position.                                
                                if (nextChecklistInput.TXT_desc.Visible == false)
                                {
                                    //remove the item in teh Editor Checklist Input List
                                    EditorChecklistInputList.RemoveAt(j);
                                    //Remove the Control at position 'j'.
                                    panel1.Controls.RemoveAt(j);
                                    //Make the selected ChecklistItem's Child null
                                    _ChecklistItemSelected.Child = null;
                                    //Make 'j' j-1 to be used in the loop again for the removed item from the list
                                    j -= 1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                //If the Item selected is a Child Item.
                else if (_ChecklistInputItem.TXT_text.Visible == false || _ChecklistInputItem.TXT_desc.Visible == false || _ChecklistInputItem.CMB_colour1.Visible == false || _ChecklistInputItem.CMB_colour2.Visible == false)
                {
                    //Variable for the previous ChecklistItem before the selected Checklist Item
                    var previousItemBefore_ChecklistItemSelected = new CheckListItem();
                    //Get and Set the previous checklist Item to _checklistitemSelected using for loop t
                    for (int t = i; t >= 0; t--)
                    {
                        //if the position of t is at i-1, then set the previous Item as the ParentItem Control variable at t.
                        if (t == (i - 1))
                        {
                            ParentItem = panel1.Controls[t];
                            var TheItemSelected = (ParentItem as CheckListInput);
                            previousItemBefore_ChecklistItemSelected = TheItemSelected.CheckListItem;
                            break;
                        }
                    }
                    for (int p = i; p < panel1.Controls.Count; p++)
                    {
                        //variable for the checklist input at the position 'p'
                        ParentItem = panel1.Controls[p];
                        //Variable to set the Parent Item as a Checklist input.
                        var TheItemSelected = (ParentItem as CheckListInput);

                        //Variable to make a new ChecklistItem - Used for _parent.Checklistitem
                        var ParentChecklistItem = new CheckListItem();
                        //Set the checklist item selected
                        ParentChecklistItem = TheItemSelected.CheckListItem;
                        //If the item selected has no textboxes(A Child Item), then remove the item
                        if (TheItemSelected.TXT_desc.Visible == false)
                        {
                            //Remove Control item from panel1 Controls at position 'p'
                            panel1.Controls.RemoveAt(p);
                            //Remove the item from the list of desc textboxes.
                            wrnctlDescList.RemoveAt(p);
                            //Remove the item from the list of text textboxes.
                            wrnctlTextList.RemoveAt(p);
                            //Remove the ChecklistInput Item from the full list of checklist inputs at position 'p'
                            EditorChecklistInputList.RemoveAt(p);

                            //Make the selected Checklist Item's Child null - This is where the ChecklistItem Child becomes null but the one selected is not null yet
                            _ChecklistItemSelected.Child = null;
                            //Make 'p' p-1 to be used in the loop again for the removed item from the list.
                            p -= 1;
                        }
                        //If the Selected Item textbox is visible or the item is the last in the list of items, then Set the _Parent ChecklistItem's relevant child to null, so that it is no longer visible in the Items list.
                        if (TheItemSelected.TXT_desc.Visible == true || (p == panel1.Controls.Count - 1))
                        {
                            //Variable to count the amount of Children Items there are before the selected item.
                            int countPreviousChildrenItems = 0;
                            //Count the amount of Child Items which occur before the selected item
                            for (int r = p; r > 0; r--)
                            {
                                //Set the Parent Item to be the panel1 Control at position 'r'.
                                ParentItem = panel1.Controls[r];
                                var ThePreviousItemSelected = (ParentItem as CheckListInput);
                                if (ThePreviousItemSelected.TXT_desc.Visible == false)
                                {
                                    countPreviousChildrenItems += 1;
                                }
                            }
                            //Get the count for the parent ChecklistItem that is required to remove the child item from.
                            int parentChecklistItemNeededCount = 0;
                            //Calculate the Parent Item needed.
                            parentChecklistItemNeededCount = p - countPreviousChildrenItems;

                            //Loop through the Children Items of the Parent Item
                            //Set the currentChildItem variable for the Children Items that willl be looped through in the for loop 's'.
                            currentChildItem = _parent.CheckListItems[parentChecklistItemNeededCount - 1].Child;
                            for (int s = parentChecklistItemNeededCount - 1; s <= p; s++)
                            {
                                //If the currentChildItem is equal to the Selected ChecklistItem
                                //, then make the currentChild Item, the selected checklist Item and the previousItemBefore_ChecklistItemNeeded.Child set to null
                                if (currentChildItem == _ChecklistItemSelected)
                                {
                                    currentChildItem = null;
                                    _ChecklistItemSelected = null;
                                    previousItemBefore_ChecklistItemSelected.Child = null;
                                    //Break once this has been completed.
                                    break;
                                }
                                else
                                {
                                    //If the CurrentChildItem and its Child Item are not null,make the currentChildItem the Child of the currentChildItem
                                    if (currentChildItem != null && currentChildItem.Child != null)
                                    {
                                        currentChildItem = currentChildItem.Child;
                                    }
                                    //If the currentChildItem is not null and the Child of the currentChildItem is null
                                    //, then make the currentChildItem, the selected checklist Item and the previousItemBefore_ChecklistItemNeeded.Child set to null
                                    if ((currentChildItem != null && currentChildItem.Child == null))
                                    {
                                        currentChildItem = null;
                                        _ChecklistItemSelected = null;
                                        previousItemBefore_ChecklistItemSelected.Child = null;
                                    }
                                }
                            }
                            //If the last Item is being looped
                            //, then make the currentChild Item, the selected checklist Item and the previousItemBefore_ChecklistItemNeeded.Child set to null
                            if (p == (panel1.Controls.Count - 1))
                            {
                                currentChildItem = null;
                                _ChecklistItemSelected = null;
                                previousItemBefore_ChecklistItemSelected.Child = null;
                            }
                            break;
                        }
                    }
                }
            }
        }

        void wrnctl_ChildAdd(object sender, EventArgs e)
        {
            //Variable for the Checklist Item
            var ChecklistItemSelected = (sender as CheckListInput).CheckListItem;
            //Variable for the ChecklistInput
            var ChecklistInputItem = (sender as CheckListInput);
            //Variable for the Checklist Input Text
            var ChecklistInputText = (sender as CheckListInput).ActiveControl.Text;
            //Variable for the Checklist Control
            var ChecklistControl = (sender as Control);

            //Add panel1 Controls to the PanelOneControls lists
            foreach (var item in panel1.Controls)
            {
                var checklistInputs = (item as CheckListInput);
                //If checklist input description textbox is visible, then add the item to the PanelOneControlsList
                if (checklistInputs.TXT_desc.Visible == true)
                {
                    //Add parent Controls
                    PanelOneControls.Add(item);
                }
                else
                {
                    //Add Child Controls
                    PanelOneChildControls.Add(item);
                }
            }
            //Add all checklist Inputs into the editor Checklist List
            foreach (var item in panel1.Controls)
            {
                var checklistInput = (item as CheckListInput);
                EditorChecklistInputList.Add(checklistInput);
            }
            //List for panel1Controls
            int k = 0;
            int i = 0;
            if (ChecklistInputText == "-" || (secondaryAddButton == true && ChecklistInputText == "-"))
            {
                //For the controls on the Panel One Controls list
                for (i = 0; i <= panel1.Controls.Count - 1; i++)
                {
                    try
                    {
                        RemoveSelectedItem(i, k, ChecklistControl, ChecklistInputItem, ChecklistItemSelected);
                        //Once the Item Removal Method has finished, continue with the relocation of the Control Items.
                        if (removed == true)
                        {
                            //Set k equal to i - used for setting locations of the Controls below the removed item/s.
                            k = i;
                            //If k == 0, then move the next first item to position 0,5
                            if (k == 0)
                            {
                                panel1.Controls[i].Location = new Point(0, 5);
                                k = 1;
                            }
                            removed = false;
                        }
                        //If k is greater than i, relocate the Controls to be in the corrrect space
                        if (i >= k && k > 0 && i <= panel1.Controls.Count)
                        {
                            if (i < panel1.Controls.Count)
                            {
                                panel1.Controls[i].Location = new Point(panel1.Controls[i].Location.X, panel1.Controls[i - 1].Bottom);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.Message);
                    }
                }
            }
            else if (ChecklistInputText == "+")
            {
                //When the + button is clicked, the secondary Addbutton bool is set to true - used for removal function above.
                secondaryAddButton = true;
                //The original reload method is used for the secondary Add Button.
                reload();
            }
            //Clear the lists necessary: containing the Panel One Controls.
            PanelOneControls.Clear();
            PanelOneChildControls.Clear();
            EditorChecklistInputList.Clear();
            //ClearPlaceholder text method.
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

            wrnctl.Visible = true;
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
            for (int i = 0; i < wrnctlDescList.Count; i++)
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
            for (int i = 0; i < wrnctlDescList.Count; i++)
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
            for (int i = 0; i < wrnctlDescList.Count; i++)
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