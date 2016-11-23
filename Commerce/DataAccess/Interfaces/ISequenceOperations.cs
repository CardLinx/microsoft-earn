//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.Context;

    /// <summary>
    /// Represents operations on Sequence objects within the data store.
    /// </summary>
    public interface ISequenceOperations
    {
        /// <summary>
        /// Given a sequence name, get the next value in the series.
        /// </summary>
        /// <returns>
        /// Next value
        /// </returns>
        int RetrieveNextValue();

        /// <summary>
        /// Given a sequence name, decrement the value to its previous value.
        /// </summary>
        /// <returns>
        /// Previous value
        /// </returns>
        int DecrementSequenceValue();

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        CommerceContext Context { get; set; }
    }
}