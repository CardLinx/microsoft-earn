//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The event type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lomo.Logging
{
    /// <summary>
    /// The event type.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Critical event
        /// </summary>
        Critical = 1, 

        /// <summary>
        /// Error event
        /// </summary>
        Error = 2, 

        /// <summary>
        /// Warning event
        /// </summary>
        Warning = 4, 

        /// <summary>
        /// Information event
        /// </summary>
        Information = 8, 

        /// <summary>
        /// Verbose event
        /// </summary>
        Verbose = 16, 
    }
}