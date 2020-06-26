using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

// Bindable list view.
// 2003 - Ian Griffiths (ian@interact-sw.co.uk)
//
// This code is in the public domain, and has no warranty.


namespace InteractSw.BindingDemo
{
    /// <summary>
    /// A ListView with complex data binding support.
    /// </summary>
    /// <remarks>
    /// <p>Windows Forms provides a built-in <see cref="ListView"/> control,
    /// which is essentially a wrapper of the standard Win32 list view. While
    /// this is a very powerful control, it does not support complex data
    /// binding. It supports simple binding, as all controls do, but simple
    /// binding only binds a single row of data. The absence of complex
    /// binding (i.e. the ability to bind to whole lists of data) is
    /// disappointing in a class whose main purpose is to display lists of
    /// things.</p>
    ///
    /// <p>This class derives from <see cref="ListView"/> and adds support
    /// for complex binding, through its <see cref="DataSource"/> and
    /// <see cref="DataMember"/> properties. These behave much like the
    /// equivalent properties on the =<see cref="DataGrid"/> control.</p>
    ///
    /// <p>Note that the primary purpose of this control is to illustrate
    /// data binding implementation techniques. It is NOT designed as an
    /// industrial-strength control for use in production code. If you use
    /// this in live systems, you do so at your own risk; it would almost
    /// certainly be a better idea to look at the various professional
    /// bindable grid controls on the market.</p>
    /// </remarks>
    public class BindableListView : System.Windows.Forms.ListView
    {
        /// <summary>
        /// The data source to which this control is bound.
        /// </summary>
        /// <remarks>
        /// <p>To make this control display the contents of a data source, you
        /// should set this property to refer to that data source. The source
        /// should implement either <see cref="IList"/>,
        /// <see cref="IBindingList"/>, or <see cref="IListSource"/>.</p>
        ///
        /// <p>When binding to a list container (i.e. one that implements the
        /// <see cref="IListSource"/> interface, such as <see cref="DataSet"/>)
        /// you must also set the <see cref="DataMember"/> property in order
        /// to identify which particular list you would like to display. You
        /// may also set the <see cref="DataMember"/> property even when
        /// DataSource refers to a list, since <see cref="DataMember"/> can
        /// also be used to navigate relations between lists.</p>
        /// </remarks>
        [Category("Data")]
        [TypeConverter(
            "System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        public object DataSource
        {
            get
            {
                return m_dataSource;
            }
            set
            {
                if (m_dataSource != value)
                {
                    // Must be either a list or a list source
                    if (value != null && !(value is IList) &&
                        !(value is IListSource))
                    {
                        throw new ArgumentException(
                            "Data source must be IList or IListSource");
                    }
                    m_dataSource = value;
                    SetDataBinding();
                    OnDataSourceChanged(EventArgs.Empty);
                }
            }
        }
        private object m_dataSource;

        /// <summary>
        /// Raised when the DataSource property changes.
        /// </summary>
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Called when the DataSource property changes
        /// </summary>
        /// <param name="e">The EventArgs that will be passed to any handlers
        /// of the DataSourceChanged event.</param>
        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            if (DataSourceChanged != null)
                DataSourceChanged(this, e);
        }

        /// <summary>
        /// Identifies the item or relation within the data source whose
        /// contents should be shown.
        /// </summary>
        /// <remarks>
        /// <p>If the <see cref="DataSource"/> refers to a container of lists
        /// such as a <see cref="DataSet"/>, this property should be used to
        /// indicate which list should be shown.</p>
        /// 
        /// <p>Even when <see cref="DataSource"/> refers to a specific list,
        /// you can still set this property to indicate that a related table
        /// should be shown by specifying a relation name. This will cause
        /// this control to display only those rows in the child table related
        /// to the currently selected row in the parent table.</p>
        /// </remarks>
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design",
            typeof(System.Drawing.Design.UITypeEditor))]
        public string DataMember
        {
            get
            {
                return m_DataMember;
            }
            set
            {
                if (m_DataMember != value)
                {
                    m_DataMember = value;
                    SetDataBinding();
                    OnDataMemberChanged(EventArgs.Empty);
                }
            }
        }
        private string m_DataMember;

        /// <summary>
        /// Raised when the DataMember property changes.
        ///</summary>
        public event EventHandler DataMemberChanged;

        /// <summary>
        /// Called when the DataMember property changes.
        /// </summary>
        /// <param name="e">The EventArgs that will be passed to any handlers
        /// of the DataMemberChanged event.</param>
        protected virtual void OnDataMemberChanged(EventArgs e)
        {
            if (DataMemberChanged != null)
                DataMemberChanged(this, e);
        }


        /// <summary>
        /// Handles binding context changes
        /// </summary>
        /// <param name="e">The EventArgs that will be passed to any handlers
        /// of the BindingContextChanged event.</param>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);

            // If our binding context changes, we must rebind, since we will
            // have a new currency managers, even if we are still bound to the
            // same data source.
            SetDataBinding();
        }


        /// <summary>
        /// Handles parent binding context changes
        /// </summary>
        /// <param name="e">Unused EventArgs.</param>
        protected override void OnParentBindingContextChanged(EventArgs e)
        {
            base.OnParentBindingContextChanged(e);

            // BindingContext is an ambient property - by default it simply picks
            // up the parent control's context (unless something has explicitly
            // given us our own). So we must respond to changes in our parent's
            // binding context in the same way we would changes to our own
            // binding context.
            SetDataBinding();
        }


        // Attaches the control to a data source.
        private void SetDataBinding()
        {
            // The BindingContext is initially null - in general we will not
            // obtain a BindingContext until we are attached to our parent
            // control. (OnParentBindingContextChanged will be called when
            // that happens, so this method will run again. This means it's
            // OK to ignore this call when we don't yet have a BindingContext.)
            if (BindingContext != null)
            {

                // Obtain the CurrencyManager and (if available) IBindingList
                // for the current data source.
                CurrencyManager currencyManager = null;
                IBindingList bindingList = null;

                if (DataSource != null)
                {
                    currencyManager = (CurrencyManager)
                        BindingContext[DataSource, DataMember];
                    if (currencyManager != null)
                    {
                        bindingList = currencyManager.List as IBindingList;
                    }
                }

                // Now see if anything has changed since we last bound to a source.

                bool reloadMetaData = false;
                bool reloadItems = false;
                if (currencyManager != m_currencyManager)
                {
                    // We have a new CurrencyManager. If we were previously
                    // using another CurrencyManager (i.e. if this is not the
                    // first time we've seen one), we'll have some event
                    // handlers attached to the old one, so first we must
                    // detach those.
                    if (m_currencyManager != null)
                    {
                        currencyManager.MetaDataChanged -=
                            new EventHandler(currencyManager_MetaDataChanged);
                        currencyManager.PositionChanged -=
                            new EventHandler(currencyManager_PositionChanged);
                        currencyManager.ItemChanged -=
                            new ItemChangedEventHandler(currencyManager_ItemChanged);
                    }

                    // Now hook up event handlers to the new CurrencyManager.
                    // This enables us to detect when the currently selected
                    // row changes. It also lets us find out more major changes
                    // such as binding to a different list object (this happens
                    // when binding to related views - each time the currently
                    // selected row in a parent changes, the child list object
                    // is replaced with a new object), or even changes in the
                    // set of properties.
                    m_currencyManager = currencyManager;
                    if (currencyManager != null)
                    {
                        reloadMetaData = true;
                        reloadItems = true;
                        currencyManager.MetaDataChanged +=
                            new EventHandler(currencyManager_MetaDataChanged);
                        currencyManager.PositionChanged +=
                            new EventHandler(currencyManager_PositionChanged);
                        currencyManager.ItemChanged +=
                            new ItemChangedEventHandler(currencyManager_ItemChanged);
                    }
                }

                if (bindingList != m_bindingList)
                {
                    // The IBindingList has changed. If we were previously
                    // bound to an IBindingList, detach the event handler.
                    if (m_bindingList != null)
                    {
                        m_bindingList.ListChanged -=
                            new ListChangedEventHandler(bindingList_ListChanged);
                    }

                    // Now hook up a handler to the new IBindingList - this
                    // will notify us of any changes in the list. (This is
                    // more detailed than the CurrencyManager ItemChanged
                    // event. However, we need both, because the only way we
                    // know when the list is replaced completely is when the
                    // CurrencyManager raises the ItemChanged event.)
                    m_bindingList = bindingList;
                    if (bindingList != null)
                    {
                        reloadItems = true;
                        bindingList.ListChanged +=
                            new ListChangedEventHandler(bindingList_ListChanged);
                    }
                }

                // If a change occurred that means the set of properties may
                // have changed, reload these.
                if (reloadMetaData)
                {
                    LoadColumnsFromSource();
                }

                // If a change occurred that means the set of items to be
                // shown in the list may have changed, reload those.
                if (reloadItems)
                {
                    LoadItemsFromSource();
                }
            }

        }
        private CurrencyManager m_currencyManager;
        private IBindingList m_bindingList;
        private PropertyDescriptorCollection m_properties;


        // Reload the properties, and build column headers for them.

        private void LoadColumnsFromSource()
        {
            // Retrieve and store the PropertyDescriptors. (We always go
            // via PropertyDescriptors when binding, and not the Reflection
            // API - this allows generic data sources to decide at runtime
            // what properties to present.) For data sources that don't opt
            // to have dynamic properties, the PropertyDescriptor mechanism
            // automatically falls back to Reflection under the covers.

            m_properties = m_currencyManager.GetItemProperties();


            // Build new column headers for the ListView.

            ColumnHeader[] headers = new ColumnHeader[m_properties.Count];
            Columns.Clear();
            for (int column = 0; column < m_properties.Count; ++column)
            {
                string columnName = m_properties[column].Name;

                // We set the width to be -2 in order to auto-size the column
                // to the header text. Bizarrely, this only works if we set
                // the width after adding the column. (That's we we're not
                // simply passing -2 to Add. The value passed - 0 in this case
                // - is irrelevant here.)
                Columns.Add(columnName, 0, HorizontalAlignment.Left);
                Columns[column].Width = -2;
            }
            // For some reason we seem to need to go back and set the
            // first column's Width to -2 (auto width) a second time.
            // It doesn't stick first time.
            Columns[0].Width = -2;
        }


        // Reload list items from the data source.

        private void LoadItemsFromSource()
        {
            // Tell the control not to bother redrawing until we're done
            // adding new items - avoids flicker and speeds things up.
            BeginUpdate();

            try
            {
                // We're about to rebuild the list, so get rid of the current
                // items.
                Items.Clear();

                // m_bindingList won't be set if the data source doesn't
                // implement IBindingList, so always ask the CurrencyManager
                // for the IList. (IList is all we need to retrieve the rows.)

                IList items = m_currencyManager.List;

                // Add items to list.
                int nItems = items.Count;
                for (int i = 0; i < nItems; ++i)
                {
                    Items.Add(BuildItemForRow(items[i]));
                }
                int index = m_currencyManager.Position;
                if (index != -1)
                {
                    SetSelectedIndex(index);
                }
            }
            finally
            {
                // In finally block just in case the data source does something
                // nasty to us - it feels like it might be bad to leave the
                // control in a state where we called BeginUpdate without a
                // corresponding EndUpdate.
                EndUpdate();
            }
        }

        // Build a single ListViewItem for a single row from the source. (We
        // need to do this when constructing the original list, but this is
        // also called in the IBindingList.ListChanged event handler when
        // updating individual items.)

        private ListViewItem BuildItemForRow(object row)
        {
            string[] itemText = new string[m_properties.Count];
            for (int column = 0; column < itemText.Length; ++column)
            {
                // Use the PropertyDescriptors to extract the property value -
                // this might be a virtual property.

                itemText[column] = m_properties[column].GetValue(row)?.ToString();
            }
            return new ListViewItem(itemText);
        }


        // IBindingList ListChanged event handler. Deals with fine-grained
        // changes to list items.

        private void bindingList_ListChanged(object sender,
            ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                // Well, usually fine-grained... The whole list has changed
                // utterly, so reload it.

                case ListChangedType.Reset:
                    LoadItemsFromSource();
                    break;


                // A single item has changed, so just rebuild that.

                case ListChangedType.ItemChanged:
                    object changedRow = m_currencyManager.List[e.NewIndex];
                    BeginUpdate();
                    Items[e.NewIndex] = BuildItemForRow(changedRow);
                    EndUpdate();
                    break;


                // A new item has appeared, so add that.

                case ListChangedType.ItemAdded:
                    object newRow = m_currencyManager.List[e.NewIndex];
                    // We get this event twice if certain grid controls
                    // are used to add a new row to a datatable: once when
                    // the editing of a new row begins, and once again when
                    // that editing commits. (If the user cancels the creation
                    // of the new row, we never see the second creation.)
                    // We detect this by seeing if this is a view on a
                    // row in a DataTable, and if it is, testing to see if
                    // it's a new row under creation.
                    DataRowView drv = newRow as DataRowView;
                    if (drv == null || !drv.IsNew)
                    {
                        // Either we're not dealing with a view on a data
                        // table, or this is the commit notification. Either
                        // way, this is the final notification, so we want
                        // to add the new row now!
                        BeginUpdate();
                        Items.Insert(e.NewIndex, BuildItemForRow(newRow));
                        EndUpdate();
                    }
                    break;


                // An item has gone away.

                case ListChangedType.ItemDeleted:
                    if (e.NewIndex < Items.Count)
                    {
                        Items.RemoveAt(e.NewIndex);
                    }
                    break;


                // An item has changed its index.

                case ListChangedType.ItemMoved:
                    BeginUpdate();
                    ListViewItem moving = Items[e.OldIndex];
                    Items.Insert(e.NewIndex, moving);
                    EndUpdate();
                    break;


                // Something has changed in the metadata. (This control is
                // too lazy to deal with this in a fine-grained fashion,
                // mostly because the author has never seen this event
                // occur... So we deal with it the simple way: reload
                // everything.)

                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                    LoadColumnsFromSource();
                    LoadItemsFromSource();
                    break;
            }
        }


        // The CurrencyManager calls this if the data source looks
        // different. We just reload everything.

        private void currencyManager_MetaDataChanged(object sender,
            EventArgs e)
        {
            LoadColumnsFromSource();
            LoadItemsFromSource();
        }


        // Called by the CurrencyManager when the currently selected item
        // changes. We update the ListView selection so that we stay in sync
        // with any other controls bound to the same source.

        private void currencyManager_PositionChanged(object sender,
            EventArgs e)
        {
            SetSelectedIndex(m_currencyManager.Position);
        }


        // Change the currently-selected item. (I'm sure I'm missing a simpler
        // way of doing this... If anyone knows what it is, please let me
        // know!)

        private void SetSelectedIndex(int index)
        {
            // Avoid recursion - we keep track of when we're already in the
            // middle of changing the index, in case the CurrencyManager
            // decides to call us back as a result of a change already in
            // progress. (Not sure if this will ever actually happen - the
            // OnSelectedIndexChanged method uses the m_changingIndex flag to
            // avoid modifying the CurrencyManager's Position when the change
            // in selection was caused by the CurrencyManager in the first
            // place. But it doesn't hurt to be defensive...)
            if (!m_changingIndex)
            {
                m_changingIndex = true;
                SelectedItems.Clear();
                if (Items.Count > index)
                {
                    ListViewItem item = Items[index];
                    item.Selected = true;
                    item.EnsureVisible();
                }
                m_changingIndex = false;
            }
        }
        private bool m_changingIndex;


        // Called by Windows Forms when the currently selected index of the
        // control changes. This usually happens because the user clicked on
        // the control. In this case we want to notify the CurrencyManager so
        // that any other bound controls will remain in sync. This method will
        // also be called when we changed our index as a result of a
        // notification that originated from the CurrencyManager, and in that
        // case we avoid notifying the CurrencyManager back!

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            // Did this originate from us, or was this caused by the
            // CurrencyManager in the first place. If we're sure it was us,
            // and there is actually a selected item (this event is also raised
            // when transitioning to the 'no items selected' state), and we
            // definitely do have a CurrencyManager (i.e. we are actually bound
            // to a data source), then we notify the CurrencyManager.

            if (!m_changingIndex && SelectedIndices.Count > 0 &&
                m_currencyManager != null)
            {
                m_currencyManager.Position = SelectedIndices[0];
            }
        }


        // Called by the CurrencyManager when stuff changes. (Yes I know
        // that's vague, but then so is the official documentation.)
        // At time of writing, the official docs imply that you don't need
        // to handle this event if your source implements IBindingList, since
        // IBindingList.ListChanged provides more details information about the
        // change. However, it's not quite as simple as that: when bound to a
        // related view, the list to which we are bound changes every time the
        // selected index of the parent changes, and to see that happen we
        // either have handle this event, or the CurrentChanged (also from the
        // CurrencyManager). So in practice you need to handle both.
        // It doesn't appear to matter whether you handle CurrentChanged or
        // ItemChanged in order to detect such changes - both are raised when
        // the underlying list changes. However, Mark Boulter sent me some
        // example code (thanks Mark!) that used this one, and he probably
        // knows something I don't about which is likely to work better...
        // So I'm doing what his code does and using this event.
        private void currencyManager_ItemChanged(object sender,
            ItemChangedEventArgs e)
        {
            // An index of -1 seems to be the indication that lots has
            // changed. (I've not found where it says this in the
            // documentation - I got this information from a comment in Mark
            // Boulter's code.) So we always reload all items from the
            // source when this happens.
            if (e.Index == -1)
            {
                // ...but before we reload all items from source, we also look
                // to see if the list we're supposed to bind to has changed
                // since last time, and if it has, reattach our event handlers.

                if (!object.ReferenceEquals(m_bindingList, m_currencyManager.List))
                {
                    m_bindingList.ListChanged -=
                        new ListChangedEventHandler(bindingList_ListChanged);
                    m_bindingList = m_currencyManager.List as IBindingList;
                    if (m_bindingList != null)
                    {
                        m_bindingList.ListChanged +=
                            new ListChangedEventHandler(bindingList_ListChanged);
                    }
                }
                LoadItemsFromSource();
            }
        }
    }
}