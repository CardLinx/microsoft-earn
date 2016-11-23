//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Configuration
{
    /// <summary>
    /// Specifies possible levels of functional mocking for commerce data store operations.
    /// </summary>
    public enum CommerceDataStoreMockLevel
    {
        /// <summary>
        /// Indicates that no functional mocking of data store operations will occur.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that calls to data store will be intercepted and, when applicable, altered before continuing execution.
        /// </summary>
        Interception,

        /// <summary>
        /// Indicates that calls to data store will be replaced in whole by mock functionality.
        /// </summary>
        Replacement
    }
}