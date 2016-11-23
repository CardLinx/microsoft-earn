//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class to deal with IEquatable objects
    /// </summary>
    /// <typeparam name="T">The class which implements IEquatable.</typeparam>
    public static class EquatableHelper<T> where T : class, IEquatable<T>
    {
        /// <summary>
        /// Compares two IEquatable objects for equality.
        /// </summary>
        /// <param name="first">The first object to compare.</param>
        /// <param name="second">The second object to compare.</param>
        /// <returns></returns>
        public static bool AreEqual(T first, T second)
        {
            if (object.ReferenceEquals(first, second))
            {
                return true;
            }

            // Both are null.
            if (first == null && second == null)
            {
                return true;
            }

            // Both are not null, they have to be equal to each other.
            if (first != null && second != null)
            {
                return first.Equals(second);
            }

            // one is null and the other is not.
            return false;
        }

        /// <summary>
        /// Compares an instance of T against an object instance.
        /// </summary>
        /// <param name="first">The entity of type T.</param>
        /// <param name="second">The object instance to compare against the first parameter.</param>
        /// <returns>
        /// True if the objects are equal, false otherwise.
        /// </returns>
        public static bool AreEqual(T first, object second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            T entity = second as T;

            if (entity == null)
                return false;

            return entity.Equals(first);
        }

        /// <summary>
        /// Compares two collections which contains IEquatable objects.
        /// </summary>
        /// <param name="firstCollection">The first collection of IEquatable objects.</param>
        /// <param name="secondCollection">The second collection of IEquatable objects.</param>
        /// <returns></returns>
        public static bool AreEqual(ICollection<T> firstCollection, ICollection<T> secondCollection)
        {
            if (object.ReferenceEquals(firstCollection, secondCollection))
            {
                return true;
            }

            // Both null
            if (firstCollection == null && secondCollection == null)
            {
                return true;
            }

            // both collections are not null
            if (firstCollection != null && secondCollection != null)
            {
                // A quick length check.
                if (firstCollection.Count != secondCollection.Count)
                {
                    return false;
                }

                // Collections are of same length, so each item in one collection should be present in the other.
                return firstCollection.All(first => secondCollection.Any(second => second.Equals(first)));
            }

            // one collection is null and the other is not.
            return false;
        }
    }
}