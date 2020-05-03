#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using netDxf.Collections;

namespace netDxf.Tables
{
    /// <summary>
    /// Defines classes that can be accessed by name. They are usually part of the dxf table section but can also be part of the objects section.
    /// </summary>
    public abstract class TableObject :
        DxfObject,
        IHasXData,
        ICloneable,
        IComparable,
        IComparable<TableObject>,
        IEquatable<TableObject>
    {
        #region delegates and events

        public delegate void NameChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<string> e);
        public event NameChangedEventHandler NameChanged;
        protected virtual void OnNameChangedEvent(string oldName, string newName)
        {
            NameChangedEventHandler ae = this.NameChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<string> eventArgs = new TableObjectChangedEventArgs<string>(oldName, newName);
                ae(this, eventArgs);
            }
        }

        public event XDataAddAppRegEventHandler XDataAddAppReg;
        protected virtual void OnXDataAddAppRegEvent(ApplicationRegistry item)
        {
            XDataAddAppRegEventHandler ae = this.XDataAddAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        public event XDataRemoveAppRegEventHandler XDataRemoveAppReg;
        protected virtual void OnXDataRemoveAppRegEvent(ApplicationRegistry item)
        {
            XDataRemoveAppRegEventHandler ae = this.XDataRemoveAppReg;
            if (ae != null)
                ae(this, new ObservableCollectionEventArgs<ApplicationRegistry>(item));
        }

        #endregion

        #region private fields

        private static readonly IReadOnlyList<string> invalidCharacters = new[] {"\\", "/", ":", "*", "?", "\"", "<", ">", "|", ";", ",", "=", "`"};
        private bool reserved;
        private string name;
        private readonly XDataDictionary xData;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>TableObject</c> class.
        /// </summary>
        /// <param name="name">Table name. The following characters \&lt;&gt;/?":;*|,=` are not supported for table object names.</param>
        /// <param name="codeName">Table <see cref="DxfObjectCode">code name</see>.</param>
        /// <param name="checkName">Defines if the table object name needs to be checked for invalid characters.</param>
        protected TableObject(string name, string codeName, bool checkName)
            : base(codeName)
        {
            if (checkName)
            {
                if (!IsValidName(name))
                    throw new ArgumentException("The name should be at least one character long and the following characters \\<>/?\":;*|,=` are not supported.", nameof(name));
            }

            this.name = name;
            this.reserved = false;
            this.xData = new XDataDictionary();
            this.xData.AddAppReg += this.XData_AddAppReg;
            this.xData.RemoveAppReg += this.XData_RemoveAppReg;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the name of the table object.
        /// </summary>
        /// <remarks>Table object names are case insensitive.</remarks>
        public string Name
        {
            get { return this.name; }
            set { this.SetName(value, true); }
        }

        /// <summary>
        /// Gets if the table object is reserved and cannot be deleted.
        /// </summary>
        public bool IsReserved
        {
            get { return this.reserved; }
            internal set { this.reserved = value; }
        }

        /// <summary>
        /// Gets the array of characters not supported as table object names.
        /// </summary>
        public static IReadOnlyList<string> InvalidCharacters
        {
            get { return invalidCharacters; }
        }

        /// <summary>
        /// Gets the table <see cref="XDataDictionary">extended data</see>.
        /// </summary>
        public XDataDictionary XData
        {
            get { return this.xData; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Checks if a string is valid as a table object name.
        /// </summary>
        /// <param name="name">String to check.</param>
        /// <returns>True if the string is valid as a table object name, or false otherwise.</returns>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            foreach (string s in InvalidCharacters)
            {
                if (name.Contains(s))
                    return false;
            }

            // using regular expressions is slower
            //if (Regex.IsMatch(name, "[\\<>/?\":;*|,=`]"))
            //    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", "name");

            return true;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Hack to change the table name without having to check its name. Some invalid characters are used for internal purposes only.
        /// </summary>
        /// <param name="newName">Table object new name.</param>
        internal void SetName(string newName, bool checkName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException(nameof(newName));
            if (this.IsReserved)
                throw new ArgumentException("Reserved table objects cannot be renamed.", nameof(newName));
            if (string.Equals(this.name, newName, StringComparison.OrdinalIgnoreCase))
                return;
            if (checkName)
                if (!IsValidName(newName))
                    throw new ArgumentException("The following characters \\<>/?\":;*|,=` are not supported for table object names.", nameof(newName));
            this.OnNameChangedEvent(this.name, newName);
            this.name = newName;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region implements IComparable

        /// <summary>
        /// Compares the current TableObject with another TableObject of the same type.
        /// </summary>
        /// <param name="other">A TableObject to compare with this TableObject.</param>
        /// <returns>
        /// An integer that indicates the relative order of the table objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        /// <remarks>If both table objects are no of the same type it will return zero. The comparison is made by their names.</remarks>
        public int CompareTo(object other)
        {
            return this.CompareTo((TableObject) other);
        }

        /// <summary>
        /// Compares the current TableObject with another TableObject of the same type.
        /// </summary>
        /// <param name="other">A TableObject to compare with this TableObject.</param>
        /// <returns>
        /// An integer that indicates the relative order of the table objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        /// <remarks>If both table objects are not of the same type it will return zero. The comparison is made by their names.</remarks>
        public int CompareTo(TableObject other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return this.GetType() == other.GetType() ? string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) : 0;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        ///// <summary>
        ///// Check if the tables are equal.
        ///// </summary>
        ///// <param name="u">TableObject.</param>
        ///// <param name="v">TableObject.</param>
        ///// <returns>True if the two table names are equal, false in any other case.</returns>
        //public static bool operator ==(TableObject u, TableObject v)
        //{
        //    if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
        //        return true;

        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return string.Equals(u.Name, v.Name, StringComparison.OrdinalIgnoreCase);
        //}

        ///// <summary>
        ///// Check if the tables are different.
        ///// </summary>
        ///// <param name="u">TableObject.</param>
        ///// <param name="v">TableObject.</param>
        ///// <returns>True if the two table names are different, false in any other case.</returns>
        //public static bool operator !=(TableObject u, TableObject v)
        //{
        //    if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
        //        return false;

        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return true;

        //    return !string.Equals(u.Name, v.Name, StringComparison.OrdinalIgnoreCase);
        //}

        ///// <summary>
        ///// Check if the first table is lesser than the second.
        ///// </summary>
        ///// <param name="u">TableObject.</param>
        ///// <param name="v">TableObject.</param>
        ///// <returns>True if the first table name is lesser than the second, false in any other case.</returns>
        //public static bool operator <(TableObject u, TableObject v)
        //{
        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return string.Compare(u.Name, v.Name, StringComparison.OrdinalIgnoreCase) < 0;
        //}

        ///// <summary>
        ///// Check if first table is greater than the second.
        ///// </summary>
        ///// <param name="u">TableObject.</param>
        ///// <param name="v">TableObject.</param>
        ///// <returns>True if the first table name is greater than the second, false in any other case.</returns>
        //public static bool operator >(TableObject u, TableObject v)
        //{
        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return string.Compare(u.Name, v.Name, StringComparison.OrdinalIgnoreCase) > 0;
        //}

        #endregion

        #region implements IEquatable

        /// <summary>
        /// Check if two TableObject are equal.
        /// </summary>
        /// <param name="other">Another TableObject to compare to.</param>
        /// <returns>True if two TableObject are equal or false in any other case.</returns>
        /// <remarks>
        /// Two TableObjects are considered equals if their names are the same, regardless of their internal values.
        /// This is done this way because in a dxf two TableObjects cannot have the same name.
        /// </remarks>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals((TableObject) other);
        }

        /// <summary>
        /// Check if two TableObject are equal.
        /// </summary>
        /// <param name="other">Another TableObject to compare to.</param>
        /// <returns>True if two TableObject are equal or false in any other case.</returns>
        /// <remarks>
        /// Two TableObjects are considered equals if their names are the same, regardless of their internal values.
        /// This is done this way because in a dxf two TableObjects cannot have the same name.
        /// </remarks>
        public bool Equals(TableObject other)
        {
            if (other == null)
                return false;

            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new table object that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">TableObject name of the copy.</param>
        /// <returns>A new table object that is a copy of this instance.</returns>
        public abstract TableObject Clone(string newName);

        /// <summary>
        /// Creates a new table object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new table object that is a copy of this instance.</returns>
        public abstract object Clone();

        #endregion

        #region XData events

        private void XData_AddAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataAddAppRegEvent(e.Item);
        }

        private void XData_RemoveAppReg(XDataDictionary sender, ObservableCollectionEventArgs<ApplicationRegistry> e)
        {
            this.OnXDataRemoveAppRegEvent(e.Item);
        }

        #endregion
    }
}