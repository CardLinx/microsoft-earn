//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// A simple string configuration collection.
    /// </summary>
    public class StringConfigurationCollection : ConfigurationElementCollection, IList<StringConfigurationElement>
    {
        /// <summary>
        /// Returns a new StringConfigurationElement.
        /// </summary>
        /// <returns>
        /// A new StringConfigurationElement.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new StringConfigurationElement();
        }

        /// <summary>
        /// Gets the value fo the StringConfiguratonElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StringConfigurationElement)element).Value;
        }

        /// <summary>
        /// Indicates the index of the specified StringConfigurationElement object.
        /// </summary>
        /// <param name="item">
        /// The StringConfigurationElement object whose index to return.
        /// </param>
        /// <returns>
        /// The index of the specified StringConfigurationElement object.
        /// </returns>
        public int IndexOf(StringConfigurationElement item)
        {
            return BaseIndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the StringConfigurationCollection at the specified index.
        /// </summary>
        /// <param name="index">
        /// Index at which to insert the item.
        /// </param>
        /// <param name="item">
        /// Item to insert.
        /// </param>
        public void Insert(int index,
                           StringConfigurationElement item)
        {
            BaseAdd(index, item);
        }

        /// <summary>
        /// Removes the StringConfigurationCollection item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the item to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Returns the StringConfigurationCollection item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the StringConfigurationCollection item to return.
        /// </param>
        /// <returns>
        /// The StringConfigurationCollection item at the specified index.
        /// </returns>
        public StringConfigurationElement this[int index]
        {
            get
            {
                return (StringConfigurationElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Adds the specified item to the StringConfigurationCollection.
        /// </summary>
        /// <param name="item">
        /// Item to add to the StringConfigurationCollection.
        /// </param>
        public void Add(StringConfigurationElement item)
        {
            BaseAdd(item, true);
        }

        /// <summary>
        /// Clears the StringConfigurationCollection of all contents.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Determines whether the StringConfigurationCollection contains a specific value.
        /// </summary>
        /// <param name="item">
        /// The item whose existence to determine.
        /// </param>
        /// <returns>
        /// * True if the item exists within the StringConfigurationCollection.
        /// * Else returns false.
        /// </returns>
        public bool Contains(StringConfigurationElement item)
        {
            return (BaseIndexOf(item) < 0) == false;
        }

        /// <summary>
        /// Copies the elements of the StringConfigurationCollection array to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">
        /// The array from which to copy elements.
        /// </param>
        /// <param name="arrayIndex">
        /// The index at which to start copying elements from the array.
        /// </param>
        public void CopyTo(StringConfigurationElement[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of items in the StringConfigurationCollection
        /// </summary>
        public new int Count
        {
            get
            { 
                return base.Count;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the StringConfigurationCollection is in a read-only state.
        /// </summary>
        public new bool IsReadOnly
        {
            get
            {
                return base.IsReadOnly();
            }
        }

        /// <summary>
        /// Removes the specified item from the StringConfigurationCollection.
        /// </summary>
        /// <param name="item">
        /// The item to remove from the StringConfigurationCollection.
        /// </param>
        /// <returns>
        /// * True if successful.
        /// * Else return false.
        /// </returns>
        public bool Remove(StringConfigurationElement item)
        {
            base.BaseRemoveAt(BaseIndexOf(item));
            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the StringConfigurationCollection.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the StringConfigurationCollection.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Base data structure does not support generic enumerator.
        /// </exception>
        /// <remarks>
        /// This enumerator cannot easily to supported, and only one can be public anyway.
        /// </remarks>
        IEnumerator<StringConfigurationElement> IEnumerable<StringConfigurationElement>.GetEnumerator()
        {
            throw new NotImplementedException("Base data structure does not support generic enumerator.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the StringConfigurationCollection.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the StringConfigurationCollection.
        /// </returns>
        public new IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}