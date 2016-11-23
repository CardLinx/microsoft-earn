//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The all transient strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoMo.TransientFaultHandling
{
    using System;

    using Microsoft.Practices.TransientFaultHandling;

    /// <summary>
    /// The all transient strategy.
    /// </summary>
    public class AllTransientStrategy : ITransientErrorDetectionStrategy
    {
        #region Public Methods and Operators

        /// <summary>
        /// Transient error detection - returns true always.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns> Always true </returns>
        public bool IsTransient(Exception ex)
        {
            return true;
        }

        #endregion
    }
}