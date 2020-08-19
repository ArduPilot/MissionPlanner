﻿/*
 * Groups - Enhancements to the normal ListViewGroup
 *
 * Author: Phillip Piper
 * Date: 22/08/2009 6:03PM
 *
 * Change log:
 * v2.3
 * 2009-09-09   JPP  - Added Collapsed and Collapsible properties
 * 2009-09-01   JPP  - Cleaned up code, added more docs
 *                   - Works under VS2005 again
 * 2009-08-22   JPP  - Initial version
 *
 * To do:
 * - Implement subseting
 * - Implement footer items
 * 
 * Copyright (C) 2009-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BrightIdeasSoftware
{
    /// <summary>
    /// These values indicate what is the state of the group. These values
    /// are taken directly from the SDK and many are not used by ObjectListView.
    /// </summary>
    [Flags]
    public enum GroupState
    {
        /// <summary>
        /// Normal
        /// </summary>
        LVGS_NORMAL = 0x0,

        /// <summary>
        /// Collapsed
        /// </summary>
        LVGS_COLLAPSED = 0x1,

        /// <summary>
        /// Hidden
        /// </summary>
        LVGS_HIDDEN = 0x2,

        /// <summary>
        /// NoHeader
        /// </summary>
        LVGS_NOHEADER = 0x4,

        /// <summary>
        /// Can be collapsed
        /// </summary>
        LVGS_COLLAPSIBLE = 0x8,

        /// <summary>
        /// Has focus
        /// </summary>
        LVGS_FOCUSED = 0x10,

        /// <summary>
        /// Is Selected
        /// </summary>
        LVGS_SELECTED = 0x20,

        /// <summary>
        /// Is subsetted
        /// </summary>
        LVGS_SUBSETED = 0x40,

        /// <summary>
        /// Subset link has focus
        /// </summary>
        LVGS_SUBSETLINKFOCUSED = 0x80,

        /// <summary>
        /// All styles
        /// </summary>
        LVGS_ALL = 0xFFFF
    }

    /// <summary>
    /// This mask indicates which members of a LVGROUP have valid data. These values
    /// are taken directly from the SDK and many are not used by ObjectListView.
    /// </summary>
    [Flags]
    public enum GroupMask
    {
        /// <summary>
        /// No mask
        /// </summary>
        LVGF_NONE = 0,

        /// <summary>
        /// Group has header
        /// </summary>
        LVGF_HEADER = 1,

        /// <summary>
        /// Group has footer
        /// </summary>
        LVGF_FOOTER = 2,

        /// <summary>
        /// Group has state
        /// </summary>
        LVGF_STATE = 4,

        /// <summary>
        /// 
        /// </summary>
        LVGF_ALIGN = 8,

        /// <summary>
        /// 
        /// </summary>
        LVGF_GROUPID = 0x10,

        /// <summary>
        /// pszSubtitle is valid
        /// </summary>
        LVGF_SUBTITLE = 0x00100,  

        /// <summary>
        /// pszTask is valid
        /// </summary>
        LVGF_TASK = 0x00200, 

        /// <summary>
        /// pszDescriptionTop is valid
        /// </summary>
        LVGF_DESCRIPTIONTOP = 0x00400,  

        /// <summary>
        /// pszDescriptionBottom is valid
        /// </summary>
        LVGF_DESCRIPTIONBOTTOM = 0x00800,  

        /// <summary>
        /// iTitleImage is valid
        /// </summary>
        LVGF_TITLEIMAGE = 0x01000,  

        /// <summary>
        /// iExtendedImage is valid
        /// </summary>
        LVGF_EXTENDEDIMAGE = 0x02000,  
        
        /// <summary>
        /// iFirstItem and cItems are valid
        /// </summary>
        LVGF_ITEMS = 0x04000,  
        
        /// <summary>
        /// pszSubsetTitle is valid
        /// </summary>
        LVGF_SUBSET = 0x08000,  
     
        /// <summary>
        /// readonly, cItems holds count of items in visible subset, iFirstItem is valid
        /// </summary>
        LVGF_SUBSETITEMS = 0x10000  
    }

    /// <summary>
    /// This mask indicates which members of a GROUPMETRICS structure are valid
    /// </summary>
    [Flags]
    public enum GroupMetricsMask
    {
        /// <summary>
        /// 
        /// </summary>
        LVGMF_NONE = 0,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_BORDERSIZE = 1,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_BORDERCOLOR = 2,

        /// <summary>
        /// 
        /// </summary>
        LVGMF_TEXTCOLOR = 4
    }

    /// <summary>
    /// Instances of this class enhance the capabilities of a normal ListViewGroup,
    /// enabling the functionality that was released in v6 of the common controls.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In this implementation (2009-09), these objects are essentially passive.
    /// Setting properties does not automatically change the associated group in
    /// the listview. Collapsed and Collapsible are two exceptions to this and 
    /// give immediate results.
    /// </para>
    /// <para>
    /// This really should be a subclass of ListViewGroup, but that class is 
    /// sealed (why is that?). So this class provides the same interface as a
    /// ListViewGroup, plus many other new properties.
    /// </para>
    /// </remarks>
    public class OLVGroup
    {
        #region Creation

        /// <summary>
        /// Create an OLVGroup
        /// </summary>
        public OLVGroup() : this("Default group header") {
        }

        /// <summary>
        /// Create a group with the given title
        /// </summary>
        /// <param name="header">Title of the group</param>
        public OLVGroup(string header) {
            this.Header = header;
            this.Id = OLVGroup.nextId++;
            this.TitleImage = -1;
            this.ExtendedImage = -1;
        }
        private static int nextId;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the bottom description of the group
        /// </summary>
        /// <remarks>
        /// Descriptions only appear when group is centered and there is a title image
        /// </remarks>
        public string BottomDescription {
            get { return this.bottomDescription; }
            set { this.bottomDescription = value; }
        }
        private string bottomDescription;

        /// <summary>
        /// Gets or sets whether or not this group is collapsed
        /// </summary>
        public bool Collapsed {
            get { return this.GetOneState(GroupState.LVGS_COLLAPSED); }
            set { this.SetOneState(value, GroupState.LVGS_COLLAPSED); }
        }

        /// <summary>
        /// Gets or sets whether or not this group can be collapsed
        /// </summary>
        public bool Collapsible {
            get { return this.GetOneState(GroupState.LVGS_COLLAPSIBLE); }
            set { this.SetOneState(value, GroupState.LVGS_COLLAPSIBLE); }
        }

        /// <summary>
        /// Gets or sets some representation of the contents of this group
        /// </summary>
        /// <remarks>This is user defined (like Tag)</remarks>
        public IList Contents {
            get { return this.contents; }
            set { this.contents = value; }
        }
        private IList contents;

        /// <summary>
        /// Gets whether this group has been created.
        /// </summary>
        public bool Created {
            get { return this.ListView != null; }
        }

        /// <summary>
        /// Gets or sets the int or string that will select the extended image to be shown against the title
        /// </summary>
        public object ExtendedImage {
            get { return this.extendedImage; }
            set { this.extendedImage = value; }
        }
        private object extendedImage;

        /// <summary>
        /// Gets or sets the footer of the group
        /// </summary>
        public string Footer {
            get { return this.footer; }
            set { this.footer = value; }
        }
        private string footer;

        /// <summary>
        /// Gets the internal id of our associated ListViewGroup.
        /// </summary>
        public int GroupId {
            get {
                if (this.ListViewGroup == null)
                    return this.Id;

                // Use reflection to get around the access control on the ID property
                if (OLVGroup.groupIdPropInfo == null) {
                    OLVGroup.groupIdPropInfo = typeof(ListViewGroup).GetProperty("ID", 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    System.Diagnostics.Debug.Assert(OLVGroup.groupIdPropInfo != null);
                }
                
                int? groupId = OLVGroup.groupIdPropInfo.GetValue(this.ListViewGroup, null) as int?;
                return groupId.HasValue ? groupId.Value : -1;
            }
        }
        private static PropertyInfo groupIdPropInfo;

        /// <summary>
        /// Gets or sets the header of the group
        /// </summary>
        public string Header {
            get { return this.header; }
            set { this.header = value; }
        }
        private string header;

        /// <summary>
        /// Gets or sets the horizontal alignment of the group header
        /// </summary>
        public HorizontalAlignment HeaderAlignment {
            get { return this.headerAlignment; }
            set { this.headerAlignment = value; }
        }
        private HorizontalAlignment headerAlignment;

        /// <summary>
        /// Gets or sets the internally created id of the group
        /// </summary>
        public int Id {
            get { return this.id; }
            set { this.id = value; }
        }
        private int id;

        /// <summary>
        /// Gets or sets ListViewItems that are members of this group
        /// </summary>
        /// <remarks>Listener of the BeforeCreatingGroups event can populate this collection.
        /// It is only used on non-virtual lists.</remarks>
        public IList<OLVListItem> Items {
            get { return this.items; }
            set { this.items = value; }
        }
        private IList<OLVListItem> items = new List<OLVListItem>();

        /// <summary>
        /// Gets or sets the key that was used to partition objects into this group
        /// </summary>
        /// <remarks>This is user defined (like Tag)</remarks>
        public object Key {
            get { return this.key; }
            set { this.key = value; }
        }
        private object key;

        /// <summary>
        /// Gets the ObjectListView that this group belongs to
        /// </summary>
        /// <remarks>If this is null, the group has not yet been created.</remarks>
        public ObjectListView ListView {
            get { return this.listView; }
            protected set { this.listView = value; }
        }
        private ObjectListView listView;

        /// <summary>
        /// Gets or sets the name of the group
        /// </summary>
        /// <remarks>As of 2009-09-01, this property is not used.</remarks>
        public string Name {
            get { return this.name; }
            set { this.name = value; }
        }
        private string name;

        /// <summary>
        /// Gets or sets whether this group is focused
        /// </summary>
        public bool Focused
        {
            get { return this.GetOneState(GroupState.LVGS_FOCUSED); }
            set { this.SetOneState(value, GroupState.LVGS_FOCUSED); }
        }

        /// <summary>
        /// Gets or sets whether this group is selected
        /// </summary>
        public bool Selected
        {
            get { return this.GetOneState(GroupState.LVGS_SELECTED); }
            set { this.SetOneState(value, GroupState.LVGS_SELECTED); }
        }

        /// <summary>
        /// Gets or sets the text that will show that this group is subsetted
        /// </summary>
        /// <remarks>
        /// As of WinSDK v7.0, subsetting of group is officially unimplemented.
        /// We can get around this using undocumented interfaces and may do so.
        /// </remarks>
        public string SubsetTitle {
            get { return this.subsetTitle; }
            set { this.subsetTitle = value; }
        }
        private string subsetTitle;

        /// <summary>
        /// Gets or set the subtitleof the task
        /// </summary>
        public string Subtitle {
            get { return this.subtitle; }
            set { this.subtitle = value; }
        }
        private string subtitle;

        /// <summary>
        /// Gets or sets the value by which this group will be sorted.
        /// </summary>
        public IComparable SortValue {
            get { return this.sortValue; }
            set { this.sortValue = value; }
        }
        private IComparable sortValue;

        /// <summary>
        /// Gets or sets the state of the group
        /// </summary>
        public GroupState State {
            get { return this.state; }
            set { this.state = value; }
        }
        private GroupState state;

        /// <summary>
        /// Gets or sets which bits of State are valid
        /// </summary>
        public GroupState StateMask {
            get { return this.stateMask; }
            set { this.stateMask = value; }
        }
        private GroupState stateMask;

        /// <summary>
        /// Gets or sets whether this group is showing only a subset of its elements
        /// </summary>
        /// <remarks>
        /// As of WinSDK v7.0, this property officially does nothing.
        /// </remarks>
        public bool Subseted {
            get { return this.GetOneState(GroupState.LVGS_SUBSETED); }
            set { this.SetOneState(value, GroupState.LVGS_SUBSETED); }
        }

        /// <summary>
        /// Gets or sets the user-defined data attached to this group
        /// </summary>
        public object Tag {
            get { return this.tag; }
            set { this.tag = value; }
        }
        private object tag;

        /// <summary>
        /// Gets or sets the task of this group
        /// </summary>
        /// <remarks>This task is the clickable text that appears on the right margin
        /// of the group header.</remarks>
        public string Task {
            get { return this.task; }
            set { this.task = value; }
        }
        private string task;

        /// <summary>
        /// Gets or sets the int or string that will select the image to be shown against the title
        /// </summary>
        public object TitleImage {
            get { return this.titleImage; }
            set { this.titleImage = value; }
        }
        private object titleImage;

        /// <summary>
        /// Gets or sets the top description of the group
        /// </summary>
        /// <remarks>
        /// Descriptions only appear when group is centered and there is a title image
        /// </remarks>
        public string TopDescription {
            get { return this.topDescription; }
            set { this.topDescription = value; }
        }
        private string topDescription;

        /// <summary>
        /// Gets or sets the number of items that are within this group.
        /// </summary>
        /// <remarks>This should only be used for virtual groups.</remarks>
        public int VirtualItemCount {
            get { return this.virtualItemCount; }
            set { this.virtualItemCount = value; }
        }
        private int virtualItemCount;

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets the ListViewGroup that is shadowed by this group.
        /// </summary>
        /// <remarks>For virtual groups, this will always be null.</remarks>
        protected ListViewGroup ListViewGroup {
            get { return this.listViewGroup; }
            set { this.listViewGroup = value; }
        }
        private ListViewGroup listViewGroup;
        #endregion

        #region Calculations/Conversions

        /// <summary>
        /// Calculate the index into the group image list of the given image selector
        /// </summary>
        /// <param name="imageSelector"></param>
        /// <returns></returns>
        public int GetImageIndex(object imageSelector) {
            if (imageSelector == null || this.ListView == null || this.ListView.GroupImageList == null)
                return -1;

            if (imageSelector is Int32)
                return (int)imageSelector;

            String imageSelectorAsString = imageSelector as String;
            if (imageSelectorAsString != null)
                return this.ListView.GroupImageList.Images.IndexOfKey(imageSelectorAsString);

            return -1;
        }

        /// <summary>
        /// Convert this object to a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.Header;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Insert a native group into the underlying Windows control,
        /// *without* using a ListViewGroup
        /// </summary>
        /// <param name="olv"></param>
        /// <remarks>This is used when creating virtual groups</remarks>
        public void InsertGroupNewStyle(ObjectListView olv) {
            this.ListView = olv;
            NativeMethods.InsertGroup(olv, this.AsNativeGroup(true));
        }

        /// <summary>
        /// Insert a native group into the underlying control via a ListViewGroup
        /// </summary>
        /// <param name="olv"></param>
        public void InsertGroupOldStyle(ObjectListView olv) {
            this.ListView = olv;

            // Create/update the associated ListViewGroup
            if (this.ListViewGroup == null)
                this.ListViewGroup = new ListViewGroup();
            this.ListViewGroup.Header = this.Header;
            this.ListViewGroup.HeaderAlignment = this.HeaderAlignment;
            this.ListViewGroup.Name = this.Name;

            // Remember which OLVGroup created the ListViewGroup
            this.ListViewGroup.Tag = this; 
            
            // Add the group to the control
            olv.Groups.Add(this.ListViewGroup);

            // Add any extra information
            NativeMethods.SetGroupInfo(olv, this.GroupId, this.AsNativeGroup(false));
        }

        /// <summary>
        /// Change the members of the group to match the current contents of Items,
        /// using a ListViewGroup
        /// </summary>
        public void SetItemsOldStyle() {
            List<OLVListItem> list = this.Items as List<OLVListItem>;
            if (list == null) {
                foreach (OLVListItem item in this.Items) {
                    this.ListViewGroup.Items.Add(item);
                }
            } else {
                this.ListViewGroup.Items.AddRange(list.ToArray());
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Create a native LVGROUP structure that matches this group
        /// </summary>
        internal NativeMethods.LVGROUP2 AsNativeGroup(bool withId) {

            NativeMethods.LVGROUP2 group = new NativeMethods.LVGROUP2();
            group.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.LVGROUP2));
            group.mask = (uint)(GroupMask.LVGF_HEADER ^ GroupMask.LVGF_ALIGN ^ GroupMask.LVGF_STATE);
            group.pszHeader = this.Header;
            group.uAlign = (uint)this.HeaderAlignment;
            group.stateMask = (uint)this.StateMask;
            group.state = (uint)this.State;

            if (withId) {
                group.iGroupId = this.GroupId;
                group.mask ^= (uint)GroupMask.LVGF_GROUPID;
            }

            if (!String.IsNullOrEmpty(this.Footer)) {
                group.pszFooter = this.Footer;
                group.mask ^= (uint)GroupMask.LVGF_FOOTER;
            }

            if (!String.IsNullOrEmpty(this.Subtitle)) {
                group.pszSubtitle = this.Subtitle;
                group.mask ^= (uint)GroupMask.LVGF_SUBTITLE;
            }

            if (!String.IsNullOrEmpty(this.Task)) {
                group.pszTask = this.Task;
                group.mask ^= (uint)GroupMask.LVGF_TASK;
            }

            if (!String.IsNullOrEmpty(this.TopDescription)) {
                group.pszDescriptionTop = this.TopDescription;
                group.mask ^= (uint)GroupMask.LVGF_DESCRIPTIONTOP;
            }

            if (!String.IsNullOrEmpty(this.BottomDescription)) {
                group.pszDescriptionBottom = this.BottomDescription;
                group.mask ^= (uint)GroupMask.LVGF_DESCRIPTIONBOTTOM;
            }

            int imageIndex = this.GetImageIndex(this.TitleImage);
            if (imageIndex >= 0) {
                group.iTitleImage = imageIndex;
                group.mask ^= (uint)GroupMask.LVGF_TITLEIMAGE;
            }

            imageIndex = this.GetImageIndex(this.ExtendedImage);
            if (imageIndex >= 0) {
                group.iExtendedImage = imageIndex;
                group.mask ^= (uint)GroupMask.LVGF_EXTENDEDIMAGE;
            }

            if (!String.IsNullOrEmpty(this.SubsetTitle)) {
                group.pszSubsetTitle = this.SubsetTitle;
                group.mask ^= (uint)GroupMask.LVGF_SUBSET;
            }

            if (this.VirtualItemCount > 0) {
                group.cItems = this.VirtualItemCount;
                group.mask ^= (uint)GroupMask.LVGF_ITEMS;
            }

            return group;
        }

        private bool GetOneState(GroupState mask) {
            if (this.Created)
                this.State = this.GetState();
            return (this.State & mask) == mask;
        }

        /// <summary>
        /// Get the current state of this group from the underlying control
        /// </summary>
        protected GroupState GetState() {
            return NativeMethods.GetGroupState(this.ListView, this.GroupId, GroupState.LVGS_ALL);
        }

        /// <summary>
        /// Get the current state of this group from the underlying control
        /// </summary>
        protected int SetState(GroupState newState, GroupState mask) {
            NativeMethods.LVGROUP2 group = new NativeMethods.LVGROUP2();
            group.cbSize = ((uint)Marshal.SizeOf(typeof(NativeMethods.LVGROUP2)));
            group.mask = (uint)GroupMask.LVGF_STATE;
            group.state = (uint)newState;
            group.stateMask = (uint)mask;
            return NativeMethods.SetGroupInfo(this.ListView, this.GroupId, group);
        }

        private void SetOneState(bool value, GroupState mask)
        {
            this.StateMask ^= mask;
            if (value)
                this.State ^= mask;
            else
                this.State &= ~mask;

            if (this.Created)
                this.SetState(this.State, mask);
        }

        #endregion

    }
}
