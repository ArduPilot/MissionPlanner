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
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a list of support folders for the document.
    /// </summary>
    public class SupportFolders :
        IList<string>
    {
        #region private fields

        private readonly List<string> folders;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of <c>SupportFolders</c> class.
        /// </summary>
        public SupportFolders()
        {
            this.folders = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of <c>SupportFolders</c> class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the list.</param>
        public SupportFolders(int capacity)
        {
            this.folders = new List<string>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of <c>SupportFolders</c> class.
        /// </summary>
        /// <param name="folders">The collection whose elements should be added to the list. The items in the collection cannot be null.</param>
        public SupportFolders(IEnumerable<string> folders)
        {
            if (folders == null)
                throw new ArgumentNullException(nameof(folders));
            this.folders = new List<string>();
            this.AddRange(folders);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Looks for a file in one of the support folders.
        /// </summary>
        /// <param name="file">File name to find in one of the support folders.</param>
        /// <returns>The path to the file found in one of the support folders. It includes both the path and the specified file name.</returns>
        /// <remarks>If the specified file already exists it return the same value, if neither it cannot be found in any of the support folders it will return an empty string.</remarks>
        public string FindFile(string file)
        {
            if(File.Exists(file)) return file;
            string name = Path.GetFileName(file);
            foreach (string folder in this.folders)
            {
                string newFile = string.Format("{0}{1}{2}", folder, Path.DirectorySeparatorChar, name);
                if (File.Exists(newFile)) return newFile;
            }

            return string.Empty;
        }

        #endregion

        #region implements IList<string>

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public string this[int index]
        {
            get { return this.folders[index]; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                this.folders[index] = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the list.
        /// </summary>
        /// <returns>The number of elements contained in the list.</returns>
        public int Count
        {
            get { return this.folders.Count; }
        }

        /// <summary>
        /// Returns if the list is read only.
        /// </summary>
        /// <returns>Return always true.</returns>
        public bool IsReadOnly
        {
            get {return false;}
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>The enumerator for the list.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.folders.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>The enumerator for the list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.folders.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">Folder path to add to the list. The item cannot be null.</param>
        public void Add(string item)
        {
            if(string.IsNullOrEmpty(item))
                throw new ArgumentNullException(nameof(item));
            this.folders.Add(item);
        }

        /// <summary>
        /// Adds the elements of the collection to the list.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the list. The items in the collection cannot be null.</param>
        public void AddRange(IEnumerable<string> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            foreach (string s in collection)
            {
                this.folders.Add(s);
            }
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public void Clear()
        {
            this.folders.Clear();
        }

        /// <summary>
        /// Determines whether an element is in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list. The value cannot be null.</param>
        /// <returns>True if the item is found in the list; otherwise, false.</returns>
        public bool Contains(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentNullException(nameof(item));
            return this.folders.Contains(item);
        }

        /// <summary>
        /// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from list. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            this.folders.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="item">The object to remove from the list. The value cannot be null.</param>
        /// <returns>True if the item is successfully removed; otherwise, false. This method also returns false the item was not found in the list.</returns>
        public bool Remove(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentNullException(nameof(item));
            return this.folders.Remove(item);
        }

        /// <summary>
        /// Determines the index of a specific item in the list.
        /// </summary>
        /// <param name="item">The object to locate in the list.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(string item)
        {
            return this.folders.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <param name="item">The object to insert into the list.</param>
        public void Insert(int index, string item)
        {
            this.folders.Insert(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index from the list.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            this.folders.RemoveAt(index);
        }

        #endregion
    }
}